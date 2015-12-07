using Dapper;
using EasyAnalysis.Api.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;


namespace EasyAnalysis.Api.Controllers
{
    [EnableCors("*", "*", "*")]
    public class ThreadController : ApiController
    {
        /// <summary>
        /// Get Percent Data
        /// </summary>
        /// <param name="ConnectionString">Connection String</param>
        /// <param name="repository">Repository</param>
        /// <param name="startDate">Start Date</param>
        /// <param name="endDate">End Date</param>
        /// <param name="dataType">0: Tag 1: Category</param>
        /// <returns></returns>
        private double GetPercent(string ConnectionString, string repository, string startDate, string endDate, int dataType)
        {
            // Creates a SQL connection
            using (var conn = new SqlConnection(ConnectionString))
            {
                dynamic qAll = conn.Query("SELECT COUNT(*) AS Total FROM [VwThreadOperationStatus] WHERE [Repository] = @repo AND [CreateOn] >= @start AND [CreateOn] < DATEADD(day,1,@end)", new { repo = repository.ToUpper().Trim(), start = startDate, end = endDate }).First();

                int allCaseCount = qAll.Total;
                string sql = string.Empty;
                if (dataType == 0)
                    sql = "SELECT COUNT(*) AS Total FROM [VwThreadOperationStatus] WHERE [Repository] = @repo AND [IsTagged] = 'Yes' AND [CreateOn] >= @start AND [CreateOn] < DATEADD(day,1,@end)";
                else
                    sql = "SELECT COUNT(*) AS Total FROM [VwThreadOperationStatus] WHERE [Repository] = @repo AND [IsCatagrized] = 'Yes' AND [CreateOn] >= @start AND [CreateOn] < DATEADD(day,1,@end)";
                dynamic qTag = conn.Query(sql, new { repo = repository.ToUpper().Trim(), start = startDate, end = endDate }).First();

                int tagCaseCount = qTag.Total;

                if (allCaseCount != 0)
                    return tagCaseCount / (allCaseCount * 1.0);
                else
                    return 0.0;
            }
        }

        // GET: api/ThreadController
        public HttpResponseMessage Get(
            [FromUri] string repository,
            [FromUri] int datatype
            )
        {
            EasyAnalysis.Framework.ConnectionStringProviders.IConnectionStringProvider mssqlCSProvider =
                   EasyAnalysis.Framework.ConnectionStringProviders.ConnectionStringProvider.CreateConnectionStringProvider(EasyAnalysis.Framework.ConnectionStringProviders.ConnectionStringProvider.ConnectionStringProviderType.MSSQLConnectionStringProvider);

            DateTime dtUtcNow = DateTime.UtcNow;
            var firstDayOfMonth = new DateTime(dtUtcNow.Year, dtUtcNow.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            double dailyData = GetPercent(mssqlCSProvider.GetConnectionString(), repository, dtUtcNow.ToString("yyyy-MM-dd"), dtUtcNow.ToString("yyyy-MM-dd"), datatype);
            double weeklyData = GetPercent(mssqlCSProvider.GetConnectionString(), repository, dtUtcNow.AddDays(-6).ToString("yyyy-MM-dd"), dtUtcNow.ToString("yyyy-MM-dd"), datatype); //(today-6)-today
            double monthlyData = GetPercent(mssqlCSProvider.GetConnectionString(), repository, firstDayOfMonth.ToString("yyyy-MM-dd"), lastDayOfMonth.ToString("yyyy-MM-dd"), datatype);

            var result = new DataCoverage() { daily = dailyData, weekly = weeklyData, monthly = monthlyData };

            return Request.CreateResponse(HttpStatusCode.OK, result, new JsonMediaTypeFormatter());
        }
    }
}
