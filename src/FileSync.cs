using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dirctory.Sync
{
    public class FileSync : FolderSync
    {
        public FileSync(SyncConfig p_config)
            : base(p_config)
        {
        }

        public int TraverseFile(string p_source, string p_target)
        {
            var _result = 0;

            _result += RemoveFiles(p_source, p_target);

            var _dirsA = Directory.EnumerateDirectories(p_source, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(d => ContainsFolder(__config.SourceExcludeDirs, d) == false)
                        .ToList();

            var _dirsB = Directory.EnumerateDirectories(p_target, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(d => ContainsFolder(__config.TargetExcludeDirs, d) == false)
                        .ToList();

            var _folder_compare = new FolderCompare(__config.Offset);
            var _intersect = (from _dir in _dirsB
                              select _dir.ToLower()).Intersect(_dirsA, _folder_compare);

            foreach (var _intB in _intersect)
            {
                foreach (var _intA in _dirsA)
                {
                    if (_folder_compare.Equals(_intA, _intB) == true)
                    {
                        _result += TraverseFile(_intA, _intB);
                        break;
                    }
                }
            }

            return _result;
        }

        private int RemoveFiles(string p_source, string p_target)
        {
            var _result = 0;

            var _filesA = Directory.EnumerateFiles(p_source, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(f => ContainsFile(__config.SourceExcludeFiles, f) == false)
                        .ToList();

            var _filesB = Directory.EnumerateFiles(p_target, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(f => ContainsFile(__config.TargetExcludeFiles, f) == false)
                        .ToList();

            var _file_compare = new FileCompare(__config.Offset);
            var _filesB_only = (from _file in _filesB
                                select _file.ToLower()).Except(_filesA, _file_compare);

            Parallel.ForEach(_filesB_only, _f =>
            {
                if (__config.Remove == true)
                {
                    try
                    {
                        File.SetAttributes(_f, FileAttributes.Normal);
                        File.Delete(_f);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("'{0}': {1}", ++__config.UnDeletedFile, _f);
                        Interlocked.Increment(ref _result);
                    }
                }
                else
                {
                    Console.WriteLine("'{0}': {1}", ++__config.UnDeletedFile, _f);
                    Interlocked.Increment(ref _result);
                }
            });

            return _result;
        }

        private bool ContainsFile(string[] p_exclude, string p_file)
        {
            var _result = false;

            foreach (var _d in p_exclude)
            {
                if (Path.GetFileName(p_file).ToLower() == _d.ToLower())
                {
                    _result = true;
                    break;
                }
            }

            return _result;
        }
    }
}