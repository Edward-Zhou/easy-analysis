using EasyAnalysis.Framework.Analysis;
using EasyAnalysis.Framework.ConnectionStringProviders;
using EasyAnalysis.Framework.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EasyAnalysis.Actions
{
    public class TagMapping
    {
        public Regex Pattern { get; set; }

        public string AsTag { get; set; }
    }

    public class DetectTagsForMSDNForum : IAction
    {
        private IEnumerable<TagMapping> _tagMappings;

        private IConnectionStringProvider _connectionStringProvider;

        public string Description
        {
            get
            {
                return "detect tags automatically for MSDN Forum threads";
            }
        }


        public DetectTagsForMSDNForum()
        {
            _connectionStringProvider = new UniversalConnectionStringProvider();
        }


        public async Task RunAsync(string[] args)
        {
            var repository = args[0];

            IReadOnlyCollection datasource = MongoDataCollection.Parse(args[1]);

            TimeFrameRange timeFrameRange = null;

            if (args.Length > 2)
            {
                timeFrameRange = TimeFrameRange.Parse(args[2]);
            }

            using (var connection = new SqlConnection(_connectionStringProvider.GetConnectionString()))
            {
                var tagMappings = new List<TagMapping>();

                var mappingsCollection = new NamedQueryReadOnlyCollection("get_tagmappings", connection);

                mappingsCollection.SetParameters(new { Repository = repository });

                var list = (mappingsCollection.GetData() as IEnumerable<dynamic>).ToList();

                foreach(dynamic item in list)
                {
                    tagMappings.Add(new TagMapping
                    {
                        Pattern = new Regex(item.Pattern as string, RegexOptions.IgnoreCase),
                        AsTag = item.AsTag as string
                    });
                }

                _tagMappings = tagMappings;
            }  

            var collection = datasource.GetData() as IMongoCollection<BsonDocument>;

            var filterBuilder = Builders<BsonDocument>.Filter;

            FilterDefinition<BsonDocument> filter = "{}";

            if (timeFrameRange != null)
            {
                filter = MongoHelper.CreateTimeFrameFilter(timeFrameRange);
            }

            var host = "analyzeit.azurewebsites.net";

            await collection
                    .Find(filter)
                    .Project("{title: 1}")
                    .ForEachAsync(async (item) =>
                    {
                        var title = item.GetValue("title").AsString;

                        var tags = DetectTagsByKeyWords(title);

                        foreach (var tag in tags)
                        {
                            string apiUrl = "http://{0}/api/thread/{1}/tag/";

                            using (var client = new HttpClient())
                            {
                                var request = new StringContent("=" + tag)
                                {
                                    Headers = { ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded") }
                                };

                                var response = await client.PostAsync(string.Format(apiUrl, host, item.GetValue("_id").AsString), request);
                            }

                            Console.WriteLine("add tag [{0}]", tag);
                        }
                    });
        }

        private IEnumerable<string> DetectTagsByKeyWords(string text)
        {
            var tags = new List<string>();

            foreach(var mapping in _tagMappings)
            {
                if(mapping.Pattern.IsMatch(text))
                {
                    tags.Add(mapping.AsTag);
                }
            }

            return tags.Distinct();
        }

        private static IEnumerable<string> DetectTagsByPattern(string text)
        {
            var result = new List<string>();

            var pattern = new Regex(@"\[([^\]]+)\]");

            var collection = pattern.Matches(text);

            foreach (Match match in collection)
            {
                var value = match.Groups[1].Value;

                result.Add(value.ToLower());
            }

            return result;
        }
    }
}
