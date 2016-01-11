using EasyAnalysis.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis
{
    public class CategoryManagement
    {
        public class CascadeObject : IEquatable<CascadeObject>
        {
            public string LevelOne { get; set; }

            public string LevelTwo { get; set; }


            public bool Equals(CascadeObject other)
            {
                return LevelTwo.Equals(other.LevelTwo) && LevelOne.Equals(other.LevelOne);
            }

            public override int GetHashCode()
            {
                return LevelOne.GetHashCode() + LevelTwo.GetHashCode();
            }
        }

        private Dictionary<string, string> NameMapping = new Dictionary<string, string>  {
                 {"SQL Server Integration Services", "00E50AF7-5F43-43AD-AF05-D98B73C1F760" },
                 {"SQL Server Reporting Services", "055CCFC1-78F5-46FF-8171-33F8FF760A7C" },
                 {"SQL Server XML", "0B6C62B4-3A1C-41B1-8596-E76B7703D78A" },
                 {"SQL Server Notification Services", "0F05A8B5-5589-4CE4-B66F-2DBF12153379" },
                 {"Getting started with SQL Server", "13E5C981-20C0-47DB-9DF8-5BF7AAE3A479" },
                 {"SQL Server PowerPivot for Excel", "20542506-d051-4902-b765-b9c1003aa758" },
                 {"SQL Server Spatial", "264AFBFA-113F-400E-B4A9-5FAEAD328944" },
                 {"SQL Server Compact", "2C3752D7-6550-4F74-85BE-10395B003938" },
                 {"SQL Server Search", "2F82D42A-4723-4F6C-B545-395BD9A7BF96" },
                 {"SQL Server Driver for PHP", "325DB3F8-BD74-4D6D-BF84-109416629A0E" },
                 {"SQL Server Express", "4DFC1132-ED0E-4490-865B-89DCC2402AF9" },
                 {"SQL Server Database Engine", "53C5F5C4-8FAB-48FB-92EF-A8A0AC73BEFB" },
                 {"Database Design", "5a78c299-4be8-4edd-8d87-b48b193da81c" },
                 {"SQL Server Setup & Upgrade", "5BC30920-5D3C-4A4E-8672-FF97B0E9252F" },
                 {"SQL Server Data Access", "5E806456-F67C-4874-B193-28232ADD9C53" },
                 {"SQL Server Data Warehousing", "6053929A-7D13-41D8-8360-E3BCDFB63CD3" },
                 {"Transact-SQL", "6A68166E-B521-48A8-9454-EC36622EB8AE" },
                 {"SQL Server PowerPivot for SharePoint", "70a00407-df18-4e37-b54b-cc6611396ef6" },
                 {"Data Mining", "7A905E4F-2B22-40A8-ABCE-AAA14AB7A09A" },
                 {"SQL Server Master Data Services", "7d8f2e7b-d70b-4799-8771-f09d6cf08b23" },
                 {"SQL Server Disaster Recovery and Availability", "91A6D335-4C9F-485D-A943-74957CE01D38" },
                 {".NET Framework inside SQL Server", "B5A4D3F3-22A4-481F-A422-6FC395CC3302" },
                 {"SQL Server Security", "BE43918E-C6D3-45C5-9CAF-769AB0D180EA" },
                 {"Microsoft StreamInsight", "bf8613c5-ec73-4823-8735-4b47c74a31fd" },
                 {"SQL Service Broker", "CB51583A-90D0-4960-9362-AC25870E5252" },
                 {"SQL Server Application and Multi-Server Management", "d4dfa0e2-6997-472a-9ea6-c2421c0c98bc" },
                 {"SQL Server Documentation", "D9BAE94C-BB95-4702-BBC3-A23933ACD40A" },
                 {"Database Mirroring", "DD785EAB-C668-4C0A-AF4F-0054BC91294D" },
                 {"SQL Server Manageability", "de03fcce-5cd0-4e5f-809c-471c9a62476e" },
                 {"SQL Server Tools General", "E59F31F8-8C25-4FA0-9DF4-45706C4981B4" },
                 {"SQL Server Replication", "E77B6AB1-F790-4F28-B3FD-C354173746BA" },
                 {"SQL Server Analysis Services", "ECDE1C15-32E3-464C-96C4-0DF1D801872D" },
                 {"SQL Server Migration", "fa5df8d1-f53a-4783-81a1-ba0015e69a2a" },
                 {"SQL Server SMO/DMO", "FEC0C733-C7AC-4CDE-BB42-C6A76D06AE18" }
                 };

        public void ImportCategory(string file, string sheet)
        {
            var collection = new SpreadsheetCollection(file, sheet);

            var total = 0;

            var categories = new List<CascadeObject>();

            var types = new List<CascadeObject>();

            var task = collection.ForEachAsync((item) => {

                var l1 = item.GetValue("L1").AsString;

                var l2 = item.GetValue("L2").AsString;

                var l3 = item.GetValue("L3").AsString;

                if (!NameMapping.Keys.Contains(l1))
                {
                    Console.WriteLine(l1);
                }

                categories.Add(new CascadeObject { LevelOne = NameMapping[l1].ToLower(), LevelTwo = l2 });

                types.Add(new CascadeObject { LevelOne = l2, LevelTwo = l3 });

                total++;
            });

            task.Wait();

            categories = categories.Distinct().ToList();

            int seed = 19;

            var dict = new Dictionary<string, int>();

            foreach(var category in categories)
            {
                // Console.WriteLine("({0}, '{1}', '{2}'),", seed, category.LevelTwo, category.LevelOne);

                dict.Add(category.LevelTwo, seed);

                seed++;
            }

            int typeSeed = 5000;

            foreach(var t in types)
            {
                Console.WriteLine("({0}, '{1}', '{2}'),", typeSeed, dict[t.LevelOne], t.LevelTwo);

                typeSeed++;
            }

            Console.WriteLine("total: " + total);
        }
    }
}
