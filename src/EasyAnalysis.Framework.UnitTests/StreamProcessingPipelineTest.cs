using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyAnalysis.Framework.UnitTests
{
    public class FakeModuleOne : Analysis.IMetadataProcessModule
    {
        public void OnProcess(IDictionary<string, object> metadata, Stream stream)
        {
            metadata.Add("hello", "world");
        }
    }

    public class FakeModuleTwo : Analysis.IMetadataProcessModule
    {
        public void OnProcess(IDictionary<string, object> metadata, Stream stream)
        {
            metadata.Add("hello2", "world2");
        }
    }


    [TestClass]
    public class StreamProcessingPipelineTest
    {
        [TestMethod]
        public void BasicRunWithSpecificModule()
        {
            var pipeline = new StreamProcessingPipeline(new List<Analysis.IMetadataProcessModule> {
                new FakeModuleOne(),
                new FakeModuleTwo()
            });

            var total = 0;

            pipeline.OnOutput += (dict) => {
                Assert.AreEqual("world", dict["hello"] as string);
                Assert.AreEqual("world2", dict["hello2"] as string);

                total++;
            };

            for(int i = 0; i < 10; i++)
            {
                pipeline.Process(null);
            }

            Assert.AreEqual(10, total);
        }

    }
}
