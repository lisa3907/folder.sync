using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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

        public bool RemoveDirs
        {
            get;
            set;
        }

        public bool RemoveFiles
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
        
        public int DeletedFolder
        {
            get;
            set;
        }

        public int DeletedFile
        {
            get;
            set;
        }
        
        public int NoSourceFolders
        {
            get;
            set;
        }

        public int NoSourceFiles
        {
            get;
            set;
        }

        public int NoTargetFolders
        {
            get;
            set;
        }

        public int NoTargetFiles
        {
            get;
            set;
        }

        public SyncConfig()
        {
            Source = @"d:\source-folder";
            Target = @"d:\target-folder";

            Offset = 0;
            
            RemoveDirs = false;
            RemoveFiles = false;

            SourceExcludeDirs = new string[] { ".git" };
            SourceExcludeFiles = new string[] { "" };

            TargetExcludeDirs = new string[] { ".git" };
            TargetExcludeFiles = new string[] { ".gitignore", ".gitattributes" };

            UnDeletedFolder = 0;
            UnDeletedFile = 0;

            DeletedFolder = 0;
            DeletedFile = 0;

            NoSourceFolders = 0;
            NoSourceFiles = 0;

            NoTargetFolders = 0;
            NoTargetFiles = 0;
        }
    }
}
