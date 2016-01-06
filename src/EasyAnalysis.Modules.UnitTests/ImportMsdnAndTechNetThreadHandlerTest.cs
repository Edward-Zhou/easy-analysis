using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace EasyAnalysis.Modules.UnitTests
{
    [TestClass]
    public class ImportMsdnAndTechNetThreadHandlerTest
    {
        [TestMethod]
        public void ImportMsdnAndTechNetQuestion()
        {
            var handler = new ImportMsdnAndTechNetThreadHandler();

            handler.OnProcess(new Dictionary<string, object> {
                {
                    "url", "https://social.msdn.microsoft.com/Forums/windowsapps/en-US/413b39a9-0f01-483b-a665-fed5b673366f/wpsl-cnet-code-for-dynamic-controls-add-to-grid-for-windows-phone-8?forum=wpdevelop&outputAs=xml"
                },
                {
                    "output", "unittest.threads"
                }
            });
        }
    }
}
