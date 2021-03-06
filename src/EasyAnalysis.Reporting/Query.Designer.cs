﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EasyAnalysis.Reporting {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Query {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Query() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("EasyAnalysis.Reporting.Query", typeof(Query).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT ISNULL([Questions].[Category], &apos;NA&apos;) AS [CategoryName]
        ///,ISNULL([Questions].[Type], &apos;NA&apos;) AS [TypeName]
        ///,COUNT([Questions].[Id]) AS [Total]
        ///,SUM((CASE WHEN [Attrs].[Answered] = 1 THEN 1 ELSE 0 END)) AS [Answered]
        ///FROM
        ///[uwpdb].[dbo].[VwThreads] [Questions]
        ///LEFT OUTER JOIN [eas_dev_db].[dbo].[ForumAttributes] [Attrs]
        ///ON [Questions].Id = [Attrs].Id
        ///WHERE [Questions].Repository = @Repository 
        ///AND ([Questions].[CreateOn] BETWEEN @Start AND @End)
        ///GROUP BY [Category], [Type]
        ///ORDER BY [Category], [ [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Category_Aggregation {
            get {
                return ResourceManager.GetString("Category_Aggregation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT 
        ///[Tags].[Name] AS [Tag],
        ///COUNT([Tags].[Id]) AS [Freq]
        ///FROM
        ///[uwpdb].[dbo].VwThreads [Question]
        ///LEFT OUTER JOIN [uwpdb].[dbo].[ThreadTags]
        ///ON [Question].[Id] = [ThreadTags].[ThreadId]
        ///LEFT OUTER JOIN [uwpdb].[dbo].[Tags]
        ///ON [ThreadTags].[TagId] = [Tags].[Id]
        ///WHERE [Question].[Repository] = @Repository
        ///AND [Tags].[Name] IS NOT NULL
        ///AND [Question].[Category] = @Category
        ///AND [Question].[Type] = @Type
        ///AND [Question].[CreateOn] BETWEEN @Start AND @End
        ///GROUP BY [Tags].[Name]
        ///ORDER BY [Freq] DE [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Tags_In_Category {
            get {
                return ResourceManager.GetString("Tags_In_Category", resourceCulture);
            }
        }
    }
}
