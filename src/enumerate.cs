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
        static void Main3(string[] args)
        {

            // Create two identical or different temporary folders  
            // on a local drive and change these file paths. 
            string pathA = @"D:\ubs-git1-server\ubsvc5";
            string pathB = @"D:\lisa-git-server\ubsvc5";

            var dirA = Directory.EnumerateDirectories(pathA, "*.*", SearchOption.AllDirectories)
                        .Where(d => d.LastIndexOf(".git") <= 0)
                        .ToList();

            var dirB = Directory.EnumerateDirectories(pathB, "*.*", SearchOption.AllDirectories)
                        .Where(d => d.LastIndexOf(".git") <= 0)
                        .ToList();

            DirCompare myDirCompare = new DirCompare();
            var queryList1Only = (from dir in dirB select dir.ToLower()).Except(dirA, myDirCompare);

            Console.WriteLine("The following files are in dirB but not dirA:");
            foreach (var v in queryList1Only)
            {
                Console.WriteLine(v);
            }

            // Keep the console window open in debug mode.
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }

    // This implementation defines a very simple comparison 
    // between two FileInfo objects. It only compares the name 
    // of the files being compared and their length in bytes. 
    class DirCompare : System.Collections.Generic.IEqualityComparer<string>
    {
        public DirCompare()
        {
        }

        public bool Equals(string d1, string d2)
        {
            var _a1 = d1.Split(Path.DirectorySeparatorChar);
            var _e1 = "";
            for (int i = 2; i < _a1.Length; i++)
                _e1 += _a1[i] + Path.DirectorySeparatorChar;
            
            var _a2 = d2.Split(Path.DirectorySeparatorChar);
            var _e2 = "";
            for (int i = 2; i < _a2.Length; i++)
                _e2 += _a2[i] + Path.DirectorySeparatorChar;

            return (_e1.ToLower() == _e2.ToLower());
        }

        public int GetHashCode(string dir)
        {
            var _arr = dir.Split(Path.DirectorySeparatorChar);
            var _exc = "";
            for (int i = 2; i < _arr.Length; i++)
                _exc += _arr[i] + Path.DirectorySeparatorChar;

            string s = String.Format("{0}{1}", _exc.ToLower(), _exc.Length);
            return s.GetHashCode();
        }
    }
}