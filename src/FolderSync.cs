using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dirctory.Sync
{
    public class FolderSync
    {
        internal SyncConfig __config = null;

        public FolderSync(SyncConfig p_config)
        {
            __config = p_config;
        }

        public int TraverseFolder(string p_source, string p_target)
        {
            var _result = 0;

            _result += RemoveFolder(p_source, p_target);

            var _dirsA = Directory.EnumerateDirectories(p_source, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(d => ContainsFolder(__config.SourceExcludeDirs, d) == false)
                        .ToList();

            var _dirsB = Directory.EnumerateDirectories(p_target, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(d => ContainsFolder(__config.TargetExcludeDirs, d) == false)
                        .ToList();

            var _folder_compare = new FolderCompare(__config.Offset);
            var _intersect = (from dir in _dirsB
                              select dir.ToLower()).Intersect(_dirsA, _folder_compare);

            foreach (var _intB in _intersect)
            {
                foreach (var _intA in _dirsA)
                {
                    if (_folder_compare.Equals(_intA, _intB) == true)
                    {
                        _result += TraverseFolder(_intA, _intB);
                        break;
                    }
                }
            }

            return _result;
        }

        internal bool ContainsFolder(string[] p_exclude, string p_folder)
        {
            var _result = false;

            foreach (var _d in p_exclude)
            {
                if (Path.GetFileName(p_folder).ToLower() == _d.ToLower())
                {
                    _result = true;
                    break;
                }
            }

            return _result;
        }

        private int RemoveFolder(string p_source, string p_target)
        {
            var _result = 0;

            var _dirsA = Directory.EnumerateDirectories(p_source, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(d => ContainsFolder(__config.SourceExcludeDirs, d) == false)
                        .ToList();

            var _dirsB = Directory.EnumerateDirectories(p_target, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(d => ContainsFolder(__config.TargetExcludeDirs, d) == false)
                        .ToList();

            var _folder_compare = new FolderCompare(__config.Offset);
            var _dirB_only = (from dir in _dirsB
                              select dir.ToLower()).Except(_dirsA, _folder_compare);

            Parallel.ForEach(_dirB_only, _d =>
            {
                if (__config.Remove == true)
                {
                    try
                    {
                        ClearAttributes(_d);
                        Directory.Delete(_d, true);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("[{0}]: {1}", ++__config.UnDeletedFolder, _d);
                        Interlocked.Increment(ref _result);
                    }
                }
                else
                {
                    Console.WriteLine("[{0}]: {1}", ++__config.UnDeletedFolder, _d);
                    Interlocked.Increment(ref _result);
                }
            });

            return _result;
        }

        private void ClearAttributes(string p_folder)
        {
            if (Directory.Exists(p_folder) == true)
            {
                var _dirInfo = new DirectoryInfo(p_folder);
                _dirInfo.Attributes = _dirInfo.Attributes & ~FileAttributes.ReadOnly;

                var _subDirs = Directory.GetDirectories(p_folder);
                foreach (string _d in _subDirs)
                {
                    ClearAttributes(_d);
                }

                var _files = Directory.GetFiles(p_folder);
                foreach (string _f in _files)
                {
                    File.SetAttributes(_f, FileAttributes.Normal);
                }
            }
        }
    }
}