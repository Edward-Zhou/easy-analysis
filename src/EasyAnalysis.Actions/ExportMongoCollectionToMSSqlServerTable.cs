using EasyAnalysis.Framework.Analysis;
using EasyAnalysis.Framework.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;

namespace EasyAnalysis.Actions
{
    public class ExportMongoCollectionToMSSqlServerTable : IAction
    {
        public string Description
        {
            get
            {
                return "convert the mongo collection to a sql server table";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// [0-repository]: uwp
        /// [1-fields]: platform&language
        /// <returns></returns>
        public async Task RunAsync(string[] args)
        {
            var repository = args[0];

            var fields = args[1].Split('&');

            IReadOnlyCollection ds = MongoDataCollection.Parse("eas.ext_fields_" + repository);

            var collectionName = "ext_fields_" + repository;

            IMongoCollection<BsonDocument> sourceCollection = ds.GetData() as IMongoCollection<BsonDocument>;

            var sb = new StringBuilder();

            sb.AppendLine(string.Format("IF EXISTS (SELECT * FROM sysobjects WHERE name='{0}' and xtype='U')", collectionName));

            sb.AppendLine(string.Format("DROP Table [dbo].[{0}]", collectionName));

            sb.AppendLine(string.Format("CREATE TABLE [dbo].[{0}](", collectionName));

            sb.AppendLine("[Id] [nvarchar](128) NOT NULL,");

            foreach (var item in fields)
            {
                sb.AppendLine(string.Format("[{0}] [nvarchar](128) NOT NULL,", item));
            }

            sb.AppendLine(string.Format("CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED", collectionName));

            sb.AppendLine("([Id] ASC)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY]");

            var createTableSql = sb.ToString();

            var sb2 = new StringBuilder();

            sb2.Append(string.Format("INSERT INTO [dbo].[{0}]([Id] ", collectionName));

            foreach (var field in fields)
            {
                sb2.AppendFormat(", [{0}]", field);
            }

            sb2.Append(") VALUES(@_id ");

            foreach (var field in fields)
            {
                sb2.AppendFormat(", @{0}", field);
            }

            sb2.Append(")");

            var insertDataSql = sb2.ToString();

            using (var connection = 
                new SqlConnection(new UniversalConnectionStringProvider()
                .GetConnectionString("reporting")))
            {
                await connection.ExecuteAsync(createTableSql);

                await sourceCollection.Find("{}").ForEachAsync(async (item) => {
                    var dict = new Dictionary<string, object>();

                    dict["_id"] = item.GetValue("_id").AsString;

                    foreach(var field in fields)
                    {
                        var fieldValue = item.GetValue(field, new BsonString(string.Empty));

                        dict[field] = (fieldValue == null || fieldValue.BsonType == BsonType.Null)
                                        ? string.Empty 
                                        : fieldValue.AsString;
                    }

                    await connection.ExecuteAsync(insertDataSql, dict);
                });
            }
        }
    }
}
