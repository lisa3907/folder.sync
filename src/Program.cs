using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dirctory.Sync
{
    public class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                if (args.Length < 3)
                {
                    Console.WriteLine("Usage: {0} origin target removable[true/false]", AppDomain.CurrentDomain.FriendlyName);
                    return;
                }

                var _config = new SyncConfig();
                {
                    _config.Source = args[0];
                    _config.Target = args[1];

                    _config.Remove = Boolean.Parse(args[2]);

                    var _a1 = args[0].Split(Path.DirectorySeparatorChar);
                    var _a2 = args[1].Split(Path.DirectorySeparatorChar);

                    if (_a1.Length != _a2.Length || _a1.Length <= 0)
                    {
                        Console.WriteLine("error: original's depth is not equal to target's");
                        return;
                    }

                    _config.Offset = _a1.Length - 1;
                }

                var _syncer = new FileSync(_config);
                Parallel.Invoke(() =>
                    {
                        var _ndir = _syncer.TraverseFolder(_config.Source, _config.Target);
                        Console.WriteLine("number of un-deleted folders: {0}", _ndir);
                    },
                    () =>
                    {
                        var _nfile = _syncer.TraverseFile(_config.Source, _config.Target);
                        Console.WriteLine("number of un-deleted files: {0}", _nfile);
                    }
                );
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
    }
}