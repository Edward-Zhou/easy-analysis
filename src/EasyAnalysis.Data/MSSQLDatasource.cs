namespace EasyAnalysis.Data
{
    public class MSSQLDatasource
    {
        public string DatabaseName { get; set; }

        public string ObjectName { get; set; }

        public static MSSQLDatasource Parse(string text)
        {
            var temp = text.Split('.');

            var databaseName = temp[0];

            var objectName = temp[1];

            return new MSSQLDatasource
            {
                DatabaseName = databaseName,
                ObjectName = objectName
            };
        }
    }
}
