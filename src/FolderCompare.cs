using System;
using System.IO;

namespace Dirctory.Sync
{
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
            for (var i = __offset; i < _a1.Length; i++)
            {
                _e1 += _a1[i] + Path.DirectorySeparatorChar;
            }
            var _a2 = d2.Split(Path.DirectorySeparatorChar);
            var _e2 = "";
            for (var i = __offset; i < _a2.Length; i++)
            {
                _e2 += _a2[i] + Path.DirectorySeparatorChar;
            }
            return (_e1.ToLower() == _e2.ToLower());
        }

        public int GetHashCode(string dir)
        {
            var _arr = dir.Split(Path.DirectorySeparatorChar);
            
            var _exc = "";
            for (var i = __offset; i < _arr.Length; i++)
            {
                _exc += _arr[i] + Path.DirectorySeparatorChar;
            }
            
            var s = String.Format("{0}{1}", _exc.ToLower(), _exc.Length); 
            return s.GetHashCode();
        }
    }
}