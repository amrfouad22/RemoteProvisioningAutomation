using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace  $safeprojectname$.ProvisioningHelper
{
    public class Constants
    {
        public const string LIST_DEF_FOLDER = "Lists";
        public const string FILE_COPY_DEF_FOLDER = "Files";
        public static string CUSTOM_ACTIONS_FOLDER="CustomActions";
        public static string Type="Type";
        public  enum  RemoteProvisioningType
        {
            List=0,
            File=1,
            CustomAction=2,
            Page=3,
            View=4
        }
        
        public class CustomActionLocations
        {
            public const string ScriptLink = "ScriptLink";
        }
        public class CustomActionAttributes
        {
            public const string Description = "Description";
            public const string ScriptSrc = "ScriptSrc";
            public const string ScriptBlock = "ScriptBlock";
            public const string Location = "Location";
            public static string Sequence = "Sequence";
            public static string Name="Name";

        }
        public class SequenceItemAttributes
        {
            public const string Type = "Type";
            public const string Path = "Path";
        }
        public class FileCopyAttributeNames
        {
            public const string LocalFolder = "LocalFolder";
            public const string Filter = "Filter";
            public const string Recursive = "Recursive";
            public static string TaregtFolder="TargetFolder";
            public static string TaregtList = "TargetList";
        }
         public class PageAttributeNames
        {
            public const string Title = "Title";
            public const string ContentTypeId = "ContentTypeId";
            public const string PageLayout = "PageLayout";
            public const string ListName = "ListName";
            public const string ContentPath = "ContentPath";
        }
        public class ViewAttributeNames
        {
            public const string Title = "Title";
            public const string ListName = "ListName";
            public const string ViewType = "ViewType";
            public const string DefaultView = "DefaultView";
            public const string ViewFields = "ViewFields";
            public const string JSLink = "JSLink";
            public const string Toolbar = "Toolbar";
        }
    }
}