using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using EasyAnalysis.Models;
using System.Linq;

namespace EasyAnalysis.Repository.UnitTests
{
    [TestClass]
    public class DropDownFieldRepositoryTest
    {
        [TestMethod]
        public void AddNewDropDownFieldTest()
        {
            var repository = new DropDownFieldRepository();

            repository.New(new DropDownField
            {
                Name = "platform",
                DisplayName = "Platform",
                Repository = "uwp",
                Options = new List<Option>
                    {
                        new Option { Value = "", Name = "All" },
                        new Option { Value = "uwp", Name = "Universal Windows Platform" },
                        new Option { Value = "wp8.1", Name = "Windows Phone 8.1" },
                        new Option { Value = "w8.1", Name = "Windows 8.1" },
                        new Option { Value = "wpsl", Name = "Windows Phone Silverlight" },
                    }
            });

            var fields = repository.ListByRepository("uwp");

            Assert.AreEqual(1, fields.Count());
        }
    }
}
