using EasyAnalysis.Framework.ConnectionStringProviders;
using System.Data.SqlClient;
using Dapper;
using EasyAnalysis.Algorithm;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace EasyAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            // ParseLog(@"D:\Test\logs");

            // MergeTags(from: "build10581", to: "build-10581");

            // OutputSimilarTags();
        }

        static void CreateCustomDropDownField()
        {
            var repository = new EasyAnalysis.Repository.DropDownFieldRepository();

            repository.New(new Models.DropDownField
            {
                Name = "office_product",
                DisplayName = "Product",
                Repository = "office",
                Options = new List<Models.Option>
                {
                    new Models.Option {  Value = "", Name = "Unknown"},
                    new Models.Option {  Value = "outlook", Name = "Outlook"},
                    new Models.Option {  Value = "excel", Name = "Excel"},
                    new Models.Option {  Value = "word", Name = "Word"},
                    new Models.Option {  Value = "ppt", Name = "PowerPoint"},
                    new Models.Option {  Value = "visio", Name = "Visio"},
                    new Models.Option {  Value = "onenote", Name = "OneNote"},
                    new Models.Option {  Value = "onedrive", Name = "OneDrive"},
                    new Models.Option {  Value = "sfb", Name = "SfB"},
                    new Models.Option {  Value = "access", Name = "Access"},
                    new Models.Option {  Value = "publisher", Name = "Publisher"},
                    new Models.Option {  Value = "project", Name = "Project"}
                }
            });
        }

        static void ParseLog(string folderPath)
        {
            var dir = new DirectoryInfo(folderPath);

            foreach(var subDir in dir.GetDirectories())
            {
                foreach(var file in subDir.GetFiles())
                {
                    var lines = File.ReadLines(file.FullName).ToList();

                    foreach(var line in lines)
                    {
                        if(line.Contains("itemtype"))
                        {
                            Console.WriteLine(line);
                        }
                    }
                }
            }
        }

        static void MergeTags(string from, string to)
        {
            IConnectionStringProvider mssqlconnectionStringProvider = new SqlServerConnectionStringProvider();

            using (var connection = new SqlConnection(mssqlconnectionStringProvider.GetConnectionString()))
            {
                var findTagByNameSql = "SELECT [Id],[Name] FROM [dbo].[Tags] WHERE [Name] = @Name";

                var fromTag = connection
                    .Query(findTagByNameSql, new { Name = from })
                    .FirstOrDefault();

                var toTag = connection
                    .Query(findTagByNameSql, new { Name = to })
                    .FirstOrDefault();

                var selectThreadsByTagIdSql = "SELECT [ThreadId] FROM [dbo].[ThreadTags] WHERE [TagId] = @TagId";

                if(fromTag == null || toTag == null)
                {
                    return;
                }

                var records = connection.Query(selectThreadsByTagIdSql, new { @TagId = fromTag.Id })
                                        .Select(m => m.ThreadId);

                int deleted = 0;
                foreach(string threadId in records)
                {
                    var count = connection.Query("SELECT [ThreadId] FROM [dbo].[ThreadTags] WHERE [TagId] = @TagId AND [ThreadId] = @ThreadId",
                        new { TagId = toTag.Id, ThreadId = threadId }).Count();

                    if(count > 0)
                    {
                        connection.Execute("DELETE [dbo].[ThreadTags] WHERE [TagId] = @TagId AND [ThreadId] = @ThreadId",
                            new { TagId = fromTag.Id, ThreadId = threadId });

                        deleted++;
                    }
                }

                var updated = connection.Execute("UPDATE [dbo].[ThreadTags] SET [TagId] = @To WHERE [TagId] = @From",
                    new { From = fromTag.Id, To = toTag.Id });

                connection.Execute("DELETE [dbo].[Tags] WHERE [Id] = @Id", new { @Id = fromTag.Id });

                Console.WriteLine("Deleted: {0}, Updated: {1}", deleted, updated);

                Console.ReadLine();
            }
        }

        static void OutputSimilarTags()
        {
            var distance = new LevenshteinDistance();

            IConnectionStringProvider mssqlconnectionStringProvider = new SqlServerConnectionStringProvider();

            using (var connection = new SqlConnection(mssqlconnectionStringProvider.GetConnectionString()))
            {
                var tags = connection.Query("SELECT [Id],[Name] FROM [dbo].[Tags]")
                                     .AsList()
                                     .ToArray();

                var length = tags.Length;

                for (int i = 0; i < length - 1; i++)
                {
                    for (int j = i + 1; j < length; j++)
                    {
                        var left = (tags[i].Name as string).ToLower();

                        var right = (tags[j].Name as string).ToLower();

                        var percentage = distance.LevenshteinDistancePercent(left, right) * 100;

                        if (percentage >= 60m)
                        {
                            Console.WriteLine("{0} is similar with {1} [{2}]", left, right, percentage);
                        }
                    }
                }
            }
        }
    }
}


