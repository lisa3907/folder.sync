using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dirctory.Sync
{
    public class FolderSync : FileSync
    {
        public FolderSync(SyncConfig p_config)
            : base(p_config)
        {
        }

        public int TraverseFolder(string p_source, string p_target)
        {
            var _result = 0;

            if (Directory.Exists(p_source) && Directory.Exists(p_target))
            {
                _result += RemoveFolder(p_source, p_target);
                CopyFolder(p_source, p_target);

                var _dirsA = Directory.EnumerateDirectories(p_source, "*.*", SearchOption.TopDirectoryOnly)
                            .Where(d => ContainsFolder(__config.SourceExcludeDirs, d) == false)
                            .ToList();

                var _dirsB = Directory.EnumerateDirectories(p_target, "*.*", SearchOption.TopDirectoryOnly)
                            .Where(d => ContainsFolder(__config.TargetExcludeDirs, d) == false)
                            .ToList();

                var _folder_compare = new FolderCompare(__config.Offset);
                var _intersect = (from dir in _dirsB
                                  select dir.ToLower()).Intersect(_dirsA, _folder_compare);

                Parallel.ForEach(_intersect, _intB =>
                {
                    foreach (var _intA in _dirsA)
                    {
                        if (_folder_compare.Equals(_intA, _intB) == true)
                        {
                            _result += TraverseFolder(_intA, _intB);
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

            foreach (var _d in _dirB_only)
            {
                if (__config.RemoveDirs == true)
                {
                    try
                    {
                        ClearAttributes(_d);
                        Directory.Delete(_d, true);

                        Console.WriteLine("-[{0}]: {1}", ++__config.DeletedFolder, _d);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("-[{0}]: {1}", ++__config.UnDeletedFolder, _d);
                    }
                }
                else
                {
                    Console.WriteLine("-[{0}]: {1}", ++__config.UnDeletedFolder, _d);
                }

                _result++;
            }

            return _result;
        }

        private int CopyFolder(string p_source, string p_target)
        {
            var _result = 0;

            var _dirsA = Directory.EnumerateDirectories(p_source, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(d => ContainsFolder(__config.SourceExcludeDirs, d) == false)
                        .ToList();

            var _dirsB = Directory.EnumerateDirectories(p_target, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(d => ContainsFolder(__config.TargetExcludeDirs, d) == false)
                        .ToList();

            var _folder_compare = new FolderCompare(__config.Offset);
            var _dirA_only = (from _d in _dirsA
                              select _d.ToLower()).Except(_dirsB, _folder_compare);

            foreach (var _d in _dirA_only)
            {
                if (__config.RemoveDirs == true)
                {
                    try
                    {
                        var _target = Path.Combine(p_target, Path.GetFileName(_d));
                        if (!Directory.Exists(_target))
                            Directory.CreateDirectory(_target);

                        CopyFiles(_d, _target);

                        Console.WriteLine("+[{0}]: {1}", ++__config.CopiedFolder, _d);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("+[{0}]: {1}", ++__config.UnCopiedFolder, _d);
                    }
                }
                else
                {
                    Console.WriteLine("+[{0}]: {1}", ++__config.UnCopiedFolder, _d);
                }

                _result++;
            }

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