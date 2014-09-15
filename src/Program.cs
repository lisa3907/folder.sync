using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace foldersync
{
    public partial class Program
    {
        public string __rootA = @"D:\ubs-git1-server\ubsvc5";
        public string __rootB = @"D:\lisa-git-server\ubsvc5";

        public int __offset = 0;
        public bool __removeOk = false;

        public string[] __exclude_dir_a = new string[] { ".git", "obj", "bin" };
        public string[] __exclude_dir_b = new string[] { ".git" };
        public string[] __exclude_file_a = new string[] { "*.suo", "*.vsscc" };
        public string[] __exclude_file_b = new string[] { ".gitignore" };

        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: {0} original destination remove_true_false", AppDomain.CurrentDomain.FriendlyName);
                return;
            }

            try
            {
                Program _p = new Program();

                _p.__rootA = args[0];
                _p.__rootB = args[1];

                _p.__removeOk = Boolean.Parse(args[2]);

                var _a1 = args[0].Split(Path.DirectorySeparatorChar);
                var _a2 = args[1].Split(Path.DirectorySeparatorChar);

                if (_a1.Length != _a2.Length || _a1.Length <= 0)
                {
                    Console.WriteLine("error: origin_root's depth is not equal dest's");
                    return;
                }

                _p.__offset = _a1.Length - 1;

                var _ndir = _p.TraverseDir(_p.__rootA, _p.__rootB);
                Console.WriteLine("number of un-deleted folders: {0}", _ndir);

                var _nfile = _p.TraverseFile(_p.__rootA, _p.__rootB);
                Console.WriteLine("number of un-deleted files: {0}", _nfile);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
        }

        public int TraverseFile(string pathA, string pathB)
        {
            var _result = 0;

            _result += RemoveFiles(pathA, pathB);

            var _dirA = Directory.EnumerateDirectories(pathA, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(d => ContainsFolder(__exclude_dir_a, d) == false)
                        .ToList();

            var _dirB = Directory.EnumerateDirectories(pathB, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(d => ContainsFolder(__exclude_dir_b, d) == false)
                        .ToList();

            FolderCompare _folder_compare = new FolderCompare(__offset);
            var _intersect = (from _dir in _dirB
                              select _dir.ToLower()).Intersect(_dirA, _folder_compare);

            foreach (var _intB in _intersect)
            {
                foreach (var _intA in _dirA)
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

        public int RemoveFiles(string pathA, string pathB)
        {
            var _result = 0;

            var _filesA = Directory.EnumerateFiles(pathA, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(f => ContainsFile(__exclude_file_a, f) == false)
                        .ToList();

            var _filesB = Directory.EnumerateFiles(pathB, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(f => ContainsFile(__exclude_file_b, f) == false)
                        .ToList();

            FileCompare _file_compare = new FileCompare(__offset);
            var _filesB_only = (from _file in _filesB
                                select _file.ToLower()).Except(_filesA, _file_compare);

            //Console.WriteLine("The following files are in {0} but not {1}:", pathB, pathA);
            foreach (var _file in _filesB_only)
            {
                if (__removeOk == true)
                {
                    try
                    {
                        File.SetAttributes(_file, FileAttributes.Normal);
                        File.Delete(_file);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("{0}", _file);
                        _result++;
                    }
                }
                else
                {
                    Console.WriteLine("{0}", _file);
                    _result++;
                }
            }

            return _result;
        }

        public int TraverseDir(string pathA, string pathB)
        {
            var _result = 0;

            _result += RemoveDir(pathA, pathB);

            var _dirA = Directory.EnumerateDirectories(pathA, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(d => ContainsFolder(__exclude_dir_a, d) == false)
                        .ToList();

            var _dirB = Directory.EnumerateDirectories(pathB, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(d => ContainsFolder(__exclude_dir_b, d) == false)
                        .ToList();

            FolderCompare _folder_compare = new FolderCompare(__offset);
            var _intersect = (from dir in _dirB
                              select dir.ToLower()).Intersect(_dirA, _folder_compare);

            foreach (var _intB in _intersect)
            {
                foreach (var _intA in _dirA)
                {
                    if (_folder_compare.Equals(_intA, _intB) == true)
                    {
                        _result += TraverseDir(_intA, _intB);
                        break;
                    }
                }
            }

            return _result;
        }

        private bool ContainsFolder(string[] exclude_dir, string folder)
        {
            var _result = false;

            foreach (var _d in exclude_dir)
            {
                if (Path.GetFileName(folder).ToLower() == _d.ToLower())
                {
                    _result = true;
                    break;
                }
            }

            return _result;
        }

        private bool ContainsFile(string[] exclude_file, string file)
        {
            var _result = false;

            foreach (var _d in exclude_file)
            {
                if (Path.GetFileName(file).ToLower() == _d.ToLower())
                {
                    _result = true;
                    break;
                }
            }

            return _result;
        }

        public int RemoveDir(string pathA, string pathB)
        {
            var _result = 0;

            var dirA = Directory.EnumerateDirectories(pathA, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(d => ContainsFolder(__exclude_dir_a, d) == false)
                        .ToList();

            var dirB = Directory.EnumerateDirectories(pathB, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(d => ContainsFolder(__exclude_dir_b, d) == false)
                        .ToList();

            FolderCompare _folder_compare = new FolderCompare(__offset);
            var _dirB_only = (from dir in dirB
                              select dir.ToLower()).Except(dirA, _folder_compare);

            //Console.WriteLine("The following files are in {0} but not {1}:", pathB, pathA);
            foreach (var v in _dirB_only)
            {
                if (__removeOk == true)
                {
                    try
                    {
                        ClearDirAttributes(v);

                        Directory.Delete(v, true);
                        //Console.WriteLine("removed folder: {0}", v);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("{0}", v);
                        _result++;
                    }
                }
                else
                {
                    Console.WriteLine("{0}", v);
                    _result++;
                }
            }

            return _result;
        }

        public void ClearDirAttributes(string currentDir)
        {
            if (Directory.Exists(currentDir) == true)
            {
                var _dirInfo = new DirectoryInfo(currentDir);
                _dirInfo.Attributes = _dirInfo.Attributes & ~FileAttributes.ReadOnly;

                string[] _subDirs = Directory.GetDirectories(currentDir);
                foreach (string _dir in _subDirs)
                    ClearDirAttributes(_dir);

                string[] _files = Directory.GetFiles(currentDir);
                foreach (string _file in _files)
                    File.SetAttributes(_file, FileAttributes.Normal);
            }
        }

        public class FolderCompare : System.Collections.Generic.IEqualityComparer<string>
        {
            private int __offset;

            public FolderCompare(int offset)
            {
                __offset = offset;
            }

            public bool Equals(string d1, string d2)
            {
                var _a1 = d1.Split(Path.DirectorySeparatorChar);
                var _e1 = "";
                for (int i = __offset; i < _a1.Length; i++)
                    _e1 += _a1[i] + Path.DirectorySeparatorChar;

                var _a2 = d2.Split(Path.DirectorySeparatorChar);
                var _e2 = "";
                for (int i = __offset; i < _a2.Length; i++)
                    _e2 += _a2[i] + Path.DirectorySeparatorChar;

                return (_e1.ToLower() == _e2.ToLower());
            }

            public int GetHashCode(string dir)
            {
                var _arr = dir.Split(Path.DirectorySeparatorChar);
                var _exc = "";
                for (int i = __offset; i < _arr.Length; i++)
                    _exc += _arr[i] + Path.DirectorySeparatorChar;

                string s = String.Format("{0}{1}", _exc.ToLower(), _exc.Length);
                return s.GetHashCode();
            }
        }

        class FileCompare : System.Collections.Generic.IEqualityComparer<string>
        {
            private int __offset;

            public FileCompare(int offset)
            {
                __offset = offset;
            }

            public bool Equals(string f1, string f2)
            {
                var _a1 = f1.Split(Path.DirectorySeparatorChar);
                var _e1 = "";
                for (int i = __offset; i < _a1.Length; i++)
                    _e1 += _a1[i] + Path.DirectorySeparatorChar;

                var _a2 = f2.Split(Path.DirectorySeparatorChar);
                var _e2 = "";
                for (int i = __offset; i < _a2.Length; i++)
                    _e2 += _a2[i] + Path.DirectorySeparatorChar;

                return (_e1.ToLower() == _e2.ToLower());
            }

            public int GetHashCode(string file)
            {
                var _arr = file.Split(Path.DirectorySeparatorChar);
                var _exc = "";
                for (int i = __offset; i < _arr.Length; i++)
                    _exc += _arr[i] + Path.DirectorySeparatorChar;

                string s = String.Format("{0}{1}", _exc.ToLower(), _exc.Length);
                return s.GetHashCode();
            }
        }
    }
}