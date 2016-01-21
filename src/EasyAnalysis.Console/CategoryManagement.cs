using EasyAnalysis.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Repository { get; set; }

        public IEnumerable<Topic> Topics { get; set; }
    }

    public class Topic
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class CategoryManagement
    {
        private IList<Category> _categories = new List<Category>();

        public void AddCategory(Category category)
        {
            _categories.Add(category);
        } 

        public string OutputSQL()
        {
            var sb = new StringBuilder();

            foreach(var category in _categories)
            {
                sb.AppendLine(
                    string.Format("INSERT INTO [dbo].[Categories]([Id],[Name],[Repository]) VALUES ({0}, '{1}', '{2}');", category.Id, category.Name, category.Repository));

                foreach(var topic in category.Topics)
                {
                    sb.AppendLine(
                        string.Format("INSERT INTO [dbo].[Types] ([Id] ,[CategoryId] ,[Name]) VALUES ({0}, {1}, '{2}')", topic.Id, category.Id, topic.Name));
                }
            }

            return sb.ToString();
        }
    }
}
