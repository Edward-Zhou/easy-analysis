using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using System.Configuration;

namespace EasyAnalysis.Reporting
{
    public class CategoryReportObject
    {
        public Object Run(IDictionary<string, string> nameValues)
        {
            var repository = nameValues["repository"];

            var start = DateTime.Parse(nameValues["start"]);

            var end = DateTime.Parse(nameValues["end"]);

            return Run(repository, start, end);
        }

        public IEnumerable<Model.CategoryNodeElement> Run(string repository, DateTime start, DateTime end)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Reporting"].ConnectionString;

            using (var sqlconnection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                var categories = sqlconnection
                    .Query<Model.CategoryAggregationModel>(
                    Query.Category_Aggregation, new
                    {
                        Repository = repository,
                        Start = start,
                        End = end
                    })
                    .ToList();

                var elements = new List<Model.CategoryNodeElement>();

                var names = categories.Select(m => m.CategoryName).Distinct();

                foreach (var name in names)
                {
                    var typeNodes = categories.Where(m => m.CategoryName == name);

                    var subElements = new List<Model.TypeNodeEelment>();

                    foreach (var node in typeNodes)
                    {
                        var tags = sqlconnection.Query(Query.Tags_In_Category, new
                        {
                            Repository = repository,
                            Category = name,
                            Type = node.TypeName,
                            Start = start,
                            End = end
                        })
                        .Select(m => m.Tag as string)
                        .ToList();

                        var typeElement = new Model.TypeNodeEelment
                        {
                            TypeName = node.TypeName,
                            Answered = node.Answered,
                            Total = node.Total,
                            Tags = tags
                        };

                        subElements.Add(typeElement);
                    }

                    var element = new Model.CategoryNodeElement
                    {
                        CategoryName = name,
                        SubElements = subElements
                    };

                    elements.Add(element);
                }

                return elements;
            }
        }
    }
}
