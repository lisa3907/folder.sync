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
        static void Main2(string[] args)
        {

            // Create two identical or different temporary folders  
            // on a local drive and change these file paths. 
            string pathA = @"D:\ubs-git1-server\ubsvc5\sdk";
            string pathB = @"D:\lisa-git-server\ubsvc5\sdk";


            System.IO.DirectoryInfo dir1 = new System.IO.DirectoryInfo(pathA);
            System.IO.DirectoryInfo dir2 = new System.IO.DirectoryInfo(pathB);


            // Take a snapshot of the file system.
            IEnumerable<System.IO.FileInfo> list1 = dir1.GetFiles("*.*", System.IO.SearchOption.AllDirectories);
            IEnumerable<System.IO.FileInfo> list2 = dir2.GetFiles("*.*", System.IO.SearchOption.AllDirectories);

            //A custom file comparer defined below
            FileCompare2 myFileCompare = new FileCompare2();

            // This query determines whether the two folders contain 
            // identical file lists, based on the custom file comparer 
            // that is defined in the FileCompare2 class. 
            // The query executes immediately because it returns a bool. 
            bool areIdentical = list1.SequenceEqual(list2, myFileCompare);

            if (areIdentical == true)
            {
                Console.WriteLine("the two folders are the same");
            }
            else
            {
                Console.WriteLine("The two folders are not the same");
            }

            // Find the common files. It produces a sequence and doesn't  
            // execute until the foreach statement. 
            var queryCommonFiles = list1.Intersect(list2, myFileCompare);

            if (queryCommonFiles.Count() > 0)
            {
                Console.WriteLine("The following files are in both folders:");
                foreach (var v in queryCommonFiles)
                {
                    Console.WriteLine(v.FullName); //shows which items end up in result list
                }
            }
            else
            {
                Console.WriteLine("There are no common files in the two folders.");
            }

            // Find the set difference between the two folders. 
            // For this example we only check one way. 
            var queryList1Only = (from file in list1
                                  select file).Except(list2, myFileCompare);

            Console.WriteLine("The following files are in list1 but not list2:");
            foreach (var v in queryList1Only)
            {
                Console.WriteLine(v.FullName);
            }

            // Keep the console window open in debug mode.
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }

    // This implementation defines a very simple comparison 
    // between two FileInfo objects. It only compares the name 
    // of the files being compared and their length in bytes. 
    class FileCompare2 : System.Collections.Generic.IEqualityComparer<System.IO.FileInfo>
    {
        public FileCompare2()
        {
        }

        public bool Equals(System.IO.FileInfo f1, System.IO.FileInfo f2)
        {
            return (f1.Name == f2.Name &&
                    f1.Length == f2.Length);
        }

        // Return a hash that reflects the comparison criteria. According to the  
        // rules for IEqualityComparer<T>, if Equals is true, then the hash codes must 
        // also be equal. Because equality as defined here is a simple value equality, not 
        // reference identity, it is possible that two or more objects will produce the same 
        // hash code. 
        public int GetHashCode(System.IO.FileInfo fi)
        {
            string s = String.Format("{0}{1}", fi.Name, fi.Length);
            return s.GetHashCode();
        }
    }
}