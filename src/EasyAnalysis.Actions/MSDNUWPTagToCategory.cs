using EasyAnalysis.Framework.Analysis;
using EasyAnalysis.Framework.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Actions
{
    public class MSDNUWPTagToCategory : IAction
    {
        public class TagToCategoryMapping
        {
            public string TagName { get; set; }

            public string FieldName { get; set; }

            public string Value { get; set; }
        }

        public string Description
        {
            get
            {
                return "mapping the user tags to category";
            }
        }

        public async Task RunAsync(string[] args)
        {
            IReadOnlyCollection inputDs = MongoDataCollection.Parse("uwp.thread_profiles");

            IReadOnlyCollection outputDs = MongoDataCollection.Parse("eas.ext_fields_uwp");

            IMongoCollection<BsonDocument> inputCollection = inputDs.GetData() as IMongoCollection<BsonDocument>;

            IMongoCollection<BsonDocument> outputCollection = outputDs.GetData() as IMongoCollection<BsonDocument>;

            await inputCollection.Find("{}")
                           .Project("{tags: 1}")
                           .ForEachAsync(async (item) =>
                           {

                               Console.Write(".");

                               var indentifier = Builders<BsonDocument>
                                                    .Filter
                                                    .Eq("_id", item.GetValue("_id").AsString);

                               var tagArray = item.GetValue("tags", BsonArray.Create(new List<string>())).AsBsonArray;

                               var tags = new List<string>();

                               foreach (var tag in tagArray)
                               {
                                   tags.Add(tag.AsString);
                               }

                               UpdateDefinition<BsonDocument> updates = null;

                               var mappings = new List<TagToCategoryMapping>
                               {
                                   new TagToCategoryMapping {  TagName = "uwp", FieldName = "platform", Value = "uwp"},
                                   new TagToCategoryMapping {  TagName = "wp8.1", FieldName = "platform", Value = "wp8.1"},
                                   new TagToCategoryMapping {  TagName = "w8.1", FieldName = "platform", Value = "w8.1"},
                                   new TagToCategoryMapping {  TagName = "wpsl", FieldName = "platform", Value = "wpsl"},
                                   new TagToCategoryMapping {  TagName = "c#", FieldName = "language", Value = "c#"},
                                   new TagToCategoryMapping {  TagName = "c++", FieldName = "language", Value = "c++"},
                                   new TagToCategoryMapping {  TagName = "vb", FieldName = "language", Value = "vb"},
                                   new TagToCategoryMapping {  TagName = "javascript", FieldName = "language", Value = "javascript"}
                               };

                               foreach(var mapping in mappings)
                               {
                                   if (tags.Contains(mapping.TagName))
                                   {
                                       if (updates == null)
                                       {
                                           updates = Builders<BsonDocument>.Update.Set(mapping.FieldName, mapping.Value);
                                       }
                                       else
                                       {
                                           updates = updates.Set(mapping.FieldName, mapping.Value);
                                       }
                                   }
                               }

                               if (updates != null)
                               {
                                   await outputCollection.UpdateOneAsync(indentifier, updates, new UpdateOptions
                                   {
                                       IsUpsert = true
                                   });
                               }
                           });

            Console.WriteLine();
        }
    }
}
