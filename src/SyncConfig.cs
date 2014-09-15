using System;
using System.Collections.Generic;
using System.Linq;

namespace Dirctory.Sync
{
    public class SyncConfig
    {
        public string Source
        {
            get;
            set;
        }

        public string Target
        {
            get;
            set;
        }


        public int Offset
        {
            get;
            set;
        }

        public bool Remove
        {
            get;
            set;
        }

        public string[] SourceExcludeDirs
        {
            get;
            set;
        }

        public string[] SourceExcludeFiles
        {
            get;
            set;
        }

        public string[] TargetExcludeDirs
        {
            get;
            set;
        }

        public string[] TargetExcludeFiles
        {
            get;
            set;
        }

        public int UnDeletedFolder
        {
            get;
            set;
        }

        public int UnDeletedFile
        {
            get;
            set;
        }

        public SyncConfig()
        {
            Source = @"d:\source-folder";
            Target = @"d:\target-folder";

            Offset = 0;
            Remove = false;

            SourceExcludeDirs = new string[] { ".git", "obj", "bin", "packages" };
            SourceExcludeFiles = new string[] { "*.suo", "*.vsscc" };

            TargetExcludeDirs = new string[] { ".git" };
            TargetExcludeFiles = new string[] { ".gitignore", ".gitattributes" };

            UnDeletedFolder = 0;
            UnDeletedFile = 0;
        }
    }
}
