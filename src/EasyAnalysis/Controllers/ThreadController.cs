using EasyAnalysis.Models;
using EasyAnalysis.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using Utility.MSDN;
using Newtonsoft.Json;
using System.Text;
using System.Web.Http.Results;
using MongoDB.Driver;
using MongoDB.Bson.IO;
using MongoDB.Bson;
using EasyAnalysis.Framework.ConnectionStringProviders;

namespace EasyAnalysis.Controllers
{
    public class ThreadController : ApiController
    {
        private readonly IThreadRepository _threadRepository;
        private readonly ITagRepository _tagRepository;
        private readonly FeedFactory _feedFactory;

        public ThreadController()
        {
            var context = new DefaultDbConext();
            _threadRepository = new ThreadRepository(context);
            _tagRepository = new TagRepository(context);
            _feedFactory = new FeedFactory();
        }

        // GET api/thread
        public IEnumerable<string> Get()
        {
            throw new NotImplementedException();
        }

        // GET api/values/5
        public ThreadModel Get(string id)
        {
            return _threadRepository.Get(id);
        }


        // TODO: CODE_REFACTOR
        [Route("api/thread/{repository}/types"), HttpGet]
        public CategoryResult GetTypes(string repository)
        {
            if (repository == "SOUWP")
            {
                repository = "UWP";
            }

            var provider = new TypeProvider();

            var types = provider.GetTypesByRepository(repository);

            var result = new CategoryResult();

            var categories = types.Select(m => m.CategoryName).Distinct().ToList();

            var typeGroups = new List<IEnumerable<TypeObject>>();

            var tempCategories = new List<Category>();

            int i = 0;

            categories.ForEach((cat) =>
            {
                tempCategories.Add(new Category { index = i++, name = cat });
            });

            result.typeGroups = typeGroups;
            result.categories = tempCategories;

            foreach (var category in result.categories)
            {
                var group = types
                    .Where(m => m.CategoryName == category.name)
                    .Select(m => new TypeObject { id = m.Id, name = m.TypeName });

                typeGroups.Add(group);
            }

            return result;
        }

        [Route("api/thread/{id}/detail"), HttpGet]
        public ThreadViewModel GetDetail(string id)
        {
            var basic = _threadRepository.Get(id);

            var vm = new ThreadViewModel
            {
                Id = basic.Id,
                Title = basic.Title,
                TypeId = basic.TypeId.GetValueOrDefault(),
                Tags = basic.Tags
                            .Select(m => m.Name)
                            .AsEnumerable()
            };

            return vm;
        }

        [Route("api/thread/{id}/classify/{typeId}"), HttpPost]
        public void Classify(string id, int typeId)
        {
            _threadRepository.Change(id, (model) =>
            {
                if (model != null)
                {
                    model.TypeId = typeId;
                }
            });
        }


        [Route("api/thread/{repository}/{id}/field/{name}"), HttpPost]
        public HttpResponseMessage SetField(
            [FromUri]string repository,
            [FromUri]string id,
            [FromUri]string name,
            [FromBody]string value)
        {
            var updateAction = Builders<BsonDocument>
                .Update
                .Set(name, value)
                .Set("timestamp", DateTime.UtcNow);

            var collection = GetCollection(repository);

            var identity = Builders<BsonDocument>.Filter.Eq("_id", id);

            collection.UpdateOneAsync(
                      identity,
                      updateAction,
                      new UpdateOptions { IsUpsert = true });

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [Route("api/thread/{repository}/{id}/field"), HttpGet]
        public async Task<HttpResponseMessage> GetFieldValues(
             [FromUri]string repository,
             [FromUri]string id)
        {
            var collection = GetCollection(repository);

            var identity = Builders<BsonDocument>.Filter.Eq("_id", id);

            var values = await collection.Find(identity).Limit(1).FirstOrDefaultAsync();

            if (values == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }

            var response = Request.CreateResponse();

            response.StatusCode = HttpStatusCode.OK;

            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };

            response.Content = new StringContent(values.ToJson(jsonWriterSettings), Encoding.UTF8, "application/json");

            return response;
        }

        #region helper methods
        private static IMongoCollection<BsonDocument> GetCollection(string repository)
        {
            repository = repository.ToLower();

            var databaseName = "eas";

            var collectionName = "ext_fields_" + repository;

            IConnectionStringProvider mongoDBCSProvider = new MongoDBConnectionStringProvider();

            var client = new MongoClient(mongoDBCSProvider.GetConnectionString(databaseName));

            var database = client.GetDatabase(databaseName);

            var extFields = database.GetCollection<BsonDocument>(collectionName);

            return extFields;
        }
        #endregion

        // POST api/values
        public async Task<string> Post([FromBody]string value)
        {
            string identifier = string.Empty;

            Regex msdnUrlRegex = new Regex(@"(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})");

            Regex soUrlRegex = new Regex(@"questions/(\d+)");

            if(msdnUrlRegex.IsMatch(value))
            {
                identifier = await HandleMSNDRequest(value);
            }
            else if(soUrlRegex.IsMatch(value))
            {
                identifier = HandleSORequest(value);
            }

            if(!_threadRepository.Exists(identifier))
            {
                identifier = string.Empty;
            }

            return identifier;
        }

        private string HandleSORequest(string value)
        {
            Regex soUrlRegex = new Regex(@"questions/(\d+)");

            var match = soUrlRegex.Match(value);

            if (!match.Success)
            {
                return string.Empty;
            }

            return string.Format("SO_{0}", match.Groups[1].Value);
        }

        private async Task<string> HandleMSNDRequest(string value)
        {
            Regex guidRegex = new Regex(@"(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})");

            var match = guidRegex.Match(value);

            if (!match.Success)
            {
                return string.Empty;
            }

            var identifier = match.Groups[0].ToString();

            if (!_threadRepository.Exists(identifier))
            {
                await RegisterNewThreadAsync(identifier);
            }

            return identifier;
        }

        [Route("api/thread/{repository}/todo"), HttpGet]
        public IEnumerable<TodoItem> GetTodoList(string repository)
        {
            var result = _feedFactory.GenerateTodoItems(repository).ToList();

            int i = 1;

            foreach (var todo in result)
            {
                todo.Index = i++;
            }

            return result;
        }

        [Route("api/thread/{id}/tag"), HttpPost]
        public string AddTagToThread(string id, [FromBody]string value)
        {
            _threadRepository.Change(id, (model) =>
            {
                if (model != null && !model.Tags
                         .Select(m => m.Name.ToLower())
                         .Contains(value.ToLower()))
                {
                    var newTag = _tagRepository.CreateTagIfNotExists(value);

                    model.Tags.Add(newTag);
                }
            });
            
            return value;
        }

        [Route("api/thread/{id}/tag"), HttpDelete]
        public string RemoveTagFromThread(string id, [FromBody]string value)
        {
            _threadRepository.Change(id, (model) =>
            {
                if (model != null)
                {
                    var tagToRemove = model.Tags.Where(m => m.Name.ToLower().Equals(value.ToLower())).FirstOrDefault();

                    if (tagToRemove != null)
                    {
                        model.Tags.Remove(tagToRemove);
                    }
                }
            });

            return value;
        }

        private async Task<bool> RegisterNewThreadAsync(string identifier)
        {
            // register a new thread item
            try
            {
                var parser = new ThreadParser(Guid.Parse(identifier));

                var info = await parser.ReadThreadInfoAsync();

                if (info == null)
                {
                    return false;
                }

                // query the database by the identifer / create a new item if not exist
                var model = new ThreadModel
                {
                    Id = info.Id,
                    Title = info.Title,
                    AuthorId = info.AuthorId,
                    CreateOn = info.CreateOn,
                    ForumId = info.ForumId
                };

                var tags = Utils.DetectTagsFromTitle(info.Title);

                foreach (var name in tags)
                {
                    var tag = _tagRepository.CreateTagIfNotExists(name);

                    model.Tags.Add(tag);
                }

                _threadRepository.Create(model);

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        [Route("api/thread/import"), HttpPost]
        public void Import([FromBody]ThreadImportModel model)
        {

        }

        // PUT api/values/5
        public void Put(string id, [FromBody]ThreadModel value)
        {
            // Not Support
        }

        // DELETE api/values/5
        public void Delete(string id)
        {
            
        }
    }
}
