﻿using EasyAnalysis.Framework.Analysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace EasyAnalysis.Modules
{
    public class MSDNMetadataModule : IMetadataProcessModule
    {
        public void OnProcess(IDictionary<string, object> metadata, Stream stream)
        {
            FillMetadata(metadata, stream);
        }

        private static void FillMetadata(IDictionary<string, object> metadata, Stream stream)
        {
            XmlReaderSettings settings = new XmlReaderSettings();

            settings.DtdProcessing = DtdProcessing.Ignore;

            using (XmlReader reader = XmlReader.Create(stream, settings))
            {
                var xDoc = XDocument.Load(reader);

                var xUsers = xDoc.Element("root").Element("users").Descendants("user");

                var users = (from u in xUsers
                             select new Dictionary<string, string>
                             {
                                 { "id", u.Attribute("id").Value },
                                 { "display_name", u.Element("displayName").Value },
                                 { "msft", u.Element("msft").Value },
                                 { "mscs", u.Element("mscs").Value },
                                 { "mvp", u.Element("mvp").Value },
                                 { "partner", u.Element("partner").Value },
                                 { "mcc", u.Element("mcc").Value },
                             }).ToList();

                var xMessages = xDoc.Element("root").Element("messages").Descendants("message");

                var messages = new List<Dictionary<string, object>>();

                foreach (var msg in xMessages)
                {
                    var histories = msg.Element("histories").Descendants("history").Select(m => new {
                        type = m.Element("type").Value,
                        date = m.Element("date").Value,
                        user = m.Element("user").Value
                    }).ToList();

                    var item = new Dictionary<string, object>
                                {
                                    { "id", msg.Attribute("id").Value },
                                    { "authorId", msg.Attribute("authorId").Value },
                                    { "createdOn", DateTime.Parse(msg.Element("createdOn").Value) },
                                    { "body", msg.Element("body").Value },
                                    { "is_answer", msg.Element("answer") == null ? "false" : msg.Element("answer").Value},
                                    { "histories", histories }
                                };

                    messages.Add(item);
                }

                var xThread = xDoc.Element("root").Element("thread");

                var thread = new Dictionary<string, object>
                {
                    { "_id", xThread.Attribute("id") == null ? string.Empty : xThread.Attribute("id").Value},
                    { "authorId", xThread.Attribute("authorId") == null ? string.Empty : xThread.Attribute("authorId").Value },
                    { "threadType", xThread.Attribute("threadType") == null ? string.Empty : xThread.Attribute("threadType").Value},
                    { "title", xThread.Element("topic") == null ? string.Empty : xThread.Element("topic").Value },
                    { "url", xThread.Element("url") == null ? string.Empty : xThread.Element("url").Value},
                    { "createdOn", DateTime.Parse(xThread.Element("createdOn").Value)},
                    { "answered", xThread.Attribute("answered") == null ? "false" : xThread.Attribute("answered").Value},
                    { "views", int.Parse(xThread.Attribute("views") == null ? string.Empty : xThread.Attribute("views").Value) },
                    { "forumId", xThread.Attribute("discussionGroupId") == null ? string.Empty : xThread.Attribute("discussionGroupId").Value},
                    { "messages", messages},
                    { "users", users}
                };

                // push to result
                foreach (var kv in thread)
                {
                    metadata.Add(kv);
                }
            }
        }
    }
}
