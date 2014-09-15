using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dirctory.Sync
{
    public class FileSync
    {
        internal SyncConfig __config = null;

        public FileSync(SyncConfig p_config)
        {
            __config = p_config;
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

        public int TraverseFile(string p_source, string p_target)
        {
            var _result = 0;

            if (Directory.Exists(p_source) && Directory.Exists(p_target))
            {
                _result += RemoveFiles(p_source, p_target);
                CopyFiles(p_source, p_target);

                var _dirsA = Directory.EnumerateDirectories(p_source, "*.*", SearchOption.TopDirectoryOnly)
                            .Where(d => ContainsFolder(__config.SourceExcludeDirs, d) == false)
                            .ToList();

                var _dirsB = Directory.EnumerateDirectories(p_target, "*.*", SearchOption.TopDirectoryOnly)
                            .Where(d => ContainsFolder(__config.TargetExcludeDirs, d) == false)
                            .ToList();

                var _folder_compare = new FolderCompare(__config.Offset);
                var _intersect = (from _dir in _dirsB
                                  select _dir.ToLower()).Intersect(_dirsA, _folder_compare);

                Parallel.ForEach(_intersect, _intB =>
                {
                    foreach (var _intA in _dirsA)
                    {
                        if (_folder_compare.Equals(_intA, _intB) == true)
                        {
                            _result += TraverseFile(_intA, _intB);
                            break;
                        }
                    }
                });
            }
            else
            {
                Console.WriteLine("not exists folder: {0} or {1}", p_source, p_target);
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

            foreach (var _f in _filesB_only)
            {
                if (__config.RemoveFiles == true)
                {
                    try
                    {
                        File.SetAttributes(_f, FileAttributes.Normal);
                        File.Delete(_f);

                        Console.WriteLine("-'{0}': {1}", ++__config.DeletedFile, _f);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("-'{0}': {1}", ++__config.UnDeletedFile, _f);
                    }
                }
                else
                {
                    Console.WriteLine("-'{0}': {1}", ++__config.UnDeletedFile, _f);
                }

                _result++;
            }

            return _result;
        }

        internal int CopyFiles(string p_source, string p_target)
        {
            var _result = 0;

            var _filesA = Directory.EnumerateFiles(p_source, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(f => ContainsFile(__config.SourceExcludeFiles, f) == false)
                        .ToList();

            var _filesB = Directory.EnumerateFiles(p_target, "*.*", SearchOption.TopDirectoryOnly)
                        //.Where(f => ContainsFile(__config.TargetExcludeFiles, f) == false)
                        .ToList();

            var _file_compare = new FileCompare(__config.Offset);
            var _filesA_only = (from _file in _filesA
                                select _file.ToLower()).Except(_filesB, _file_compare);

            foreach (var _f in _filesA_only)
            {
                if (__config.RemoveFiles == true)
                {
                    try
                    {
                        var _fileName = Path.GetFileName(_f);
                        var _destFile = Path.Combine(p_target, _fileName);
                        File.Copy(_f, _destFile, true);

                        Console.WriteLine("+'{0}': {1}", ++__config.CopiedFile, _f);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("+'{0}': {1}", ++__config.UnCopiedFile, _f);
                    }
                }
                else
                {
                    Console.WriteLine("+'{0}': {1}", ++__config.UnCopiedFile, _f);
                }

                _result++;
            }

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