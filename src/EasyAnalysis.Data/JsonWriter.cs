using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Data
{
    public class JsonWriter
    {
        public static void ExportToSpreadSheet(string jsonString, string filePath, string sheetName = "Sheet1")
        {
            object o1 = JsonConvert.DeserializeObject(jsonString);
            List<string> headers = new List<string>();
            List<string> row = new List<string>();
            //get properties
            foreach (var pair in (JObject)((JArray)o1)[0])
            {
                headers.Add(pair.Key);
            }

            //export header
            ExportToSpreadSheet(headers.ToArray(), filePath, sheetName);
            int rowIndex = 2;
            //loop and export data
            foreach (var item in ((JArray)o1).Children())
            {
                foreach (string property in headers)
                {
                    row.Add(item.Children<JProperty>().FirstOrDefault(x => x.Name == property).Value.ToString());
                }
                ExportToSpreadSheet(row.ToArray(), filePath, sheetName, rowIndex++, 1);
                row.Clear();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="filePath"></param>
        /// <param name="sheetName"></param>
        /// <param name="fromRow">start from 1</param>
        /// <param name="fromColumn">start from 1</param>
        public static void ExportToSpreadSheet(string[] data, string filePath, string sheetName = "Sheet1", int fromRow = 1, int fromColumn = 1)
        {
            SpreadsheetDocument spreadSheet;
            if (!File.Exists(filePath))
            {
                spreadSheet = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook);
                // Add a WorkbookPart to the document.
                WorkbookPart workbookpart = spreadSheet.AddWorkbookPart();
                workbookpart.Workbook = new Workbook();
                // Add a WorksheetPart to the WorkbookPart.
                WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet(new SheetData());

                // Add Sheets to the Workbook.
                Sheets sheets = spreadSheet.WorkbookPart.Workbook.
                    AppendChild<Sheets>(new Sheets());

                // Append a new worksheet and associate it with the workbook.
                Sheet sheet = new Sheet()
                {
                    Id = spreadSheet.WorkbookPart.
                    GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = sheetName
                };
                sheets.Append(sheet);
            }

            else
                spreadSheet = SpreadsheetDocument.Open(filePath, true);
            // Open the document for editing.
            using (spreadSheet)
            {
                // Insert other code here.
                foreach (string text in data)
                {
                  
                   

                    //Get worksheet
                    Sheets sheets = spreadSheet.WorkbookPart.Workbook.GetFirstChild<Sheets>();

                    WorksheetPart worksheetPart;
                    string sheetId = spreadSheet.WorkbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Name.ToString().ToLower().Equals(sheetName.ToLower())).Id;
                  
                    // Insert a new worksheet.
                    if (sheetId == null || sheetId=="")
                        worksheetPart = InsertWorksheet(spreadSheet.WorkbookPart);
                    else
                    {
                        worksheetPart =(WorksheetPart) spreadSheet.WorkbookPart.GetPartById(sheetId);
                    }

                    // Insert cell A1 into the new worksheet.
                    Cell cell = InsertCellInWorksheet(GetColumn(fromColumn++), (UInt32)fromRow, worksheetPart);

                    decimal decData;
                    bool isNumber = decimal.TryParse(text, out decData);
                  
                    // Set the value of cell A1.
                    if (!isNumber)
                    {
                        // Get the SharedStringTablePart. If it does not exist, create a new one.
                        SharedStringTablePart shareStringPart;
                        if (spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart>().Count() > 0)
                        {
                            shareStringPart = spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
                        }
                        else
                        {
                            shareStringPart = spreadSheet.WorkbookPart.AddNewPart<SharedStringTablePart>();
                        }

                        // Insert the text into the SharedStringTablePart.
                        int index = InsertSharedStringItem(text, shareStringPart);
                        cell.CellValue = new CellValue(index.ToString());
                        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
                    }
                    else
                    {
                        cell.CellValue =new CellValue(text) ;
                        cell.DataType = new EnumValue<CellValues>(CellValues.Number);
                    }

                 
                    worksheetPart.Worksheet.Save();
                }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnIndex">starte from 1</param>
        /// <returns></returns>
        private static string GetColumn(int columnIndex)
        {
            List<int> valueList = new List<int>();

            do
            {
                valueList.Add(columnIndex % 26);
                columnIndex /= 26;
            }
            while (columnIndex % 26 > 0);

            valueList.Reverse();
            string strColumn = "";
            foreach (int v in valueList)
            {
                strColumn += (char)(v + (65 - 1));
            }

            return strColumn;
        }

        // Given text and a SharedStringTablePart, creates a SharedStringItem with the specified text 
        // and inserts it into the SharedStringTablePart. If the item already exists, returns its index.
        private static int InsertSharedStringItem(string text, SharedStringTablePart shareStringPart)
        {
            // If the part does not contain a SharedStringTable, create one.
            if (shareStringPart.SharedStringTable == null)
            {
                shareStringPart.SharedStringTable = new SharedStringTable();
            }

            int i = 0;

            // Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
            foreach (SharedStringItem item in shareStringPart.SharedStringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == text)
                {
                    return i;
                }

                i++;
            }

            // The text does not exist in the part. Create the SharedStringItem and return its index.
            shareStringPart.SharedStringTable.AppendChild(new SharedStringItem(new DocumentFormat.OpenXml.Spreadsheet.Text(text)));
            shareStringPart.SharedStringTable.Save();

            return i;
        }

        // Given a WorkbookPart, inserts a new worksheet.
        private static WorksheetPart InsertWorksheet(WorkbookPart workbookPart)
        {
            // Add a new worksheet part to the workbook.
            WorksheetPart newWorksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            newWorksheetPart.Worksheet = new Worksheet(new SheetData());
            newWorksheetPart.Worksheet.Save();

            Sheets sheets = workbookPart.Workbook.GetFirstChild<Sheets>();
            string relationshipId = workbookPart.GetIdOfPart(newWorksheetPart);

            // Get a unique ID for the new sheet.
            uint sheetId = 1;
            if (sheets != null && sheets.Elements<Sheet>().Count() > 0)
            {
                sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
            }

            string sheetName = "Sheet" + sheetId;

            // Append the new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = sheetName };
            sheets.Append(sheet);
            workbookPart.Workbook.Save();

            return newWorksheetPart;
        }

        // Given a column name, a row index, and a WorksheetPart, inserts a cell into the worksheet. 
        // If the cell already exists, returns it. 
        private static Cell InsertCellInWorksheet(string columnName, uint rowIndex, WorksheetPart worksheetPart)
        {
            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();
            string cellReference = columnName + rowIndex;

            // If the worksheet does not contain a row with the specified row index, insert one.
            Row row;
            if (sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).Count() != 0)
            {
                row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
            }
            else
            {
                row = new Row() { RowIndex = rowIndex };
                sheetData.Append(row);
            }

            // If there is not a cell with the specified column name, insert one.  
            if (row.Elements<Cell>().Where(c => c.CellReference.Value == columnName + rowIndex).Count() > 0)
            {
                return row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).First();
            }
            else
            {
                // Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
                Cell refCell = null;
                foreach (Cell cell in row.Elements<Cell>())
                {
                    if (string.Compare(cell.CellReference.Value, cellReference, true) > 0)
                    {
                        refCell = cell;
                        break;
                    }
                }

                Cell newCell = new Cell() { CellReference = cellReference };
                row.InsertBefore(newCell, refCell);

                worksheet.Save();
                return newCell;
            }
        }


    }
}
