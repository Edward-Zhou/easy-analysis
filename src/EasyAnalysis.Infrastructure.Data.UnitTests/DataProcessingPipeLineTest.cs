using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyAnalysis.Infrastructure.Data.UnitTests
{
    [TestClass]
    public class DataProcessingPipeLineTest
    {
        public class TestTempModel
        {
            public string Name { get; set; }

            public DateTime CreatedOn { get; set; }
        }

        public class FakeCollection :
            Framework.Data.IReadOnlyCollection<TestTempModel>
        {
            public IEnumerable<TestTempModel> _list;

            public FakeCollection()
            {
                _list = new List<TestTempModel>
                {
                    new TestTempModel {
                        Name = "HELLO WORLD",
                        CreatedOn = DateTime.Parse("2015-11-16T09:45:21") }
                };
            }

            public Task ForEachAsync(Action<TestTempModel> processor)
            {
                var task = new Task(() =>
                {
                    foreach (var item in _list)
                    {
                        processor(item);
                    }
                });

                task.RunSynchronously();

                return task;
            }

            public Task ForEachAsync(Func<TestTempModel, Task> processor)
            {
                var task = new Task(async () =>
                {
                    foreach (var item in _list)
                    {
                        await processor(item);
                    }
                });

                task.RunSynchronously();

                return task;
            }
        }

        [TestMethod]
        public void BasicTest()
        {
            var total = 0;

            var pipieline = new EasyAnalysis.Data.DataProcessingPipeLine<TestTempModel>();

            pipieline.Source = new FakeCollection();

            pipieline.OnOutput += (outputRecord) => {
                total++;
            };

            var task = pipieline.Process((context, record) =>
            {
                var date = record.CreatedOn.Date;

                context.Output(new Dictionary<string, object> {
                    { "date", date},
                    { "name", record.Name.ToLower()}
                });
            });

            task.Wait();

            Assert.AreEqual(1, total);
        }
    }
}
