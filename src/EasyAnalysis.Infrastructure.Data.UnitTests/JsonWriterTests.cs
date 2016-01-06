using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyAnalysis.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EasyAnalysis.Data.Tests
{
    [TestClass()]
    public class JsonWriterTests
    {
        [TestMethod()]
        public void ExportToSpreadSheetTest()
        {
            string jsonString = "[{'Name':'Jack','ExpiryDate':'2008-12-28T00:00:00','Price':12.0},{'Name':'Rose','ExpiryDate':'1/1/2016 12:00:00 AM','Price':12.0}]";
            string fileName = "ExportSpredsheet.xlsx";

            File.Delete(fileName);

            JsonWriter.ExportToSpreadSheet(jsonString, fileName,"Sheet1");

            CollectionAssert.AreEqual(new[]{ "Name", "ExpiryDate", "Price"},new SpreadsheetCollection(fileName, "Sheet1").GetRow(1));
            CollectionAssert.AreEqual(new[] { "Rose", "1/1/2016 12:00:00 AM", "12" }, new SpreadsheetCollection(fileName, "Sheet1").GetRow(3));

            File.Delete(fileName);
            string sheetName = "Sheet2";
            JsonWriter.ExportToSpreadSheet(jsonString, fileName, sheetName);
            CollectionAssert.AreEqual(new[] { "Name", "ExpiryDate", "Price" }, new SpreadsheetCollection(fileName, sheetName).GetRow(1));
            CollectionAssert.AreEqual(new[] { "Rose", "1/1/2016 12:00:00 AM", "12" }, new SpreadsheetCollection(fileName, sheetName).GetRow(3));
        }
    }

   
}