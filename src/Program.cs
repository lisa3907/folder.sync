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
                if (args.Length < 4)
                {
                    Console.WriteLine("Usage: {0} origin target remove_dirs remove_files", AppDomain.CurrentDomain.FriendlyName);
                    return;
                }

                var _config = new SyncConfig();
                {
                    _config.Source = args[0];
                    _config.Target = args[1];

                    _config.RemoveDirs = Boolean.Parse(args[2]);
                    _config.RemoveFiles = Boolean.Parse(args[3]);

                    var _a1 = args[0].Split(Path.DirectorySeparatorChar);
                    var _a2 = args[1].Split(Path.DirectorySeparatorChar);

                    if (_a1.Length != _a2.Length || _a1.Length <= 0)
                    {
                        Console.WriteLine("error: original's depth is not equal to target's");
                        return;
                    }

                    _config.Offset = _a1.Length - 1;
                }

                var _syncer = new FolderSync(_config);
                Parallel.Invoke(() =>
                    {
                        _syncer.TraverseFolder(_config.Source, _config.Target);
                    },
                    () =>
                    {
                        _syncer.TraverseFile(_config.Source, _config.Target);
                    }
                );

                Console.WriteLine();
                Console.WriteLine("un-deleted folders: {0}", _config.UnDeletedFolder);
                Console.WriteLine("   deleted folders: {0}", _config.DeletedFolder);
                Console.WriteLine("un-copied  folders: {0}", _config.UnCopiedFolder);
                Console.WriteLine("   copied  folders: {0}", _config.CopiedFolder);

                Console.WriteLine();

                Console.WriteLine("un-deleted files: {0}", _config.UnDeletedFile);
                Console.WriteLine("   deleted files: {0}", _config.DeletedFile);
                Console.WriteLine("un-copied  files: {0}", _config.UnCopiedFile);
                Console.WriteLine("   copied  files: {0}", _config.CopiedFile);
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            //finally
            //{
            //    Console.WriteLine("Press any key to exit.");
            //    Console.ReadKey();
            //}
        }
    }
}