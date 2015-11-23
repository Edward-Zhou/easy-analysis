using EasyAnalysis.Framework.ConnectionStringProviders;
using System.Data.SqlClient;
using Dapper;
using EasyAnalysis.Algorithm;
using System;
using System.Linq;

namespace EasyAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            MergeTags(from: "build10581", to: "build-10581");

            // OutputSimilarTags();
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


