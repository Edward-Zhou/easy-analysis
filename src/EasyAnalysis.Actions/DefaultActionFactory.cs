﻿using EasyAnalysis.Actions;
using EasyAnalysis.Data;
using EasyAnalysis.Framework.Analysis;
using System;

namespace EasyAnalysis.Backend
{
    public class DefaultActionFactory : IActionFactory
    {
        public IAction Activate(string name)
        {
            switch(name.ToLower())
            {
                case "build-basic-threadprofiles":
                    return new BuildBasicThreadProfiles(new UniversalConnectionStringProvider());

                case "extract-user-activies":
                    return new ExtractUserActivies(new UniversalConnectionStringProvider());

                case "import-new-users":
                    return new ImportNewUsers(new UniversalConnectionStringProvider());

                case "clean-up-data":
                    return new CleanUpData();

                case "sync-up-data":
                    return new SyncUpData();

                case "add-metadata-to-threadprofile":
                    return new AddMetadataToThreadProfile(new UniversalConnectionStringProvider());

                case "sync-with-web-database":
                    return new SyncWithWebDatabase();

                case "sync-with-stackoverflow":
                    return new SyncWithStackoverflow();

                case "sync-with-stackoverflow-tags":
                    return new SyncWithStackoverflowTags();

                case "build-so-question-profile":
                    return new BuildStackoverflowQuestionProfile();

                case "detect-tags-for-msdn":
                    return new DetectTagsForMSDNForum();

                case "export-mongo-to-mssql":
                    return new ExportMongoCollectionToMSSqlServerTable();

                case "msdn-uwp-tag-to-category":
                    return new MSDNUWPTagToCategory();

                case "set-forum-attributes":
                    return new SetForumAttributes();

                case "attach-thread-ownership":
                    return new AttachThreadOwnership();

                default: throw new NotImplementedException(string.Format("Action[{0}] is not supported yet"));
            }
        }
    }
}
