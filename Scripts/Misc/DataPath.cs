#region References
using System;
using System.IO;

using Microsoft.Win32;

using Ultima;
#endregion

namespace Server.Misc
{
    public class DataPath
    {
        /* If you have not installed Ultima Online,
        * or wish the server to use a separate set of datafiles,
        * change the 'CustomPath' value.
        * Example:
        *  private static string CustomPath = @"C:\Program Files\Ultima Online";
        */
		private static readonly string CustomPath = Config.Get(@"DataPath.CustomPath", default(string));

		static DataPath()
		{
			string path;

			if (CustomPath != null)
			{
				path = CustomPath;
			}
			else if (!Core.Unix)
			{
				path = Files.LoadDirectory();
			}
			else
			{
				path = null;
			}

			if (!String.IsNullOrWhiteSpace(path))
			{
				Core.DataDirectories.Add(path);
			}
		}

        /* The following is a list of files which a required for proper execution:
        * 
        * Multi.idx
        * Multi.mul
        * VerData.mul
        * TileData.mul
        * Map*.mul or Map*LegacyMUL.uop
        * StaIdx*.mul
        * Statics*.mul
        * MapDif*.mul
        * MapDifL*.mul
        * StaDif*.mul
        * StaDifL*.mul
        * StaDifI*.mul
        */
        public static void Configure()
        {
			if (Core.DataDirectories.Count == 0 && !Core.Service)
			{
				Console.WriteLine("Enter the Ultima Online directory:");
	            Console.Write("> ");
	
	            Core.DataDirectories.Add(Console.ReadLine());
			}
	
			foreach (var path in Core.DataDirectories)
			{
				Files.SetMulPath(path);
			}
		
			Utility.PushColor(ConsoleColor.DarkYellow);
			Console.WriteLine("DataPath: " + Core.DataDirectories[0]);
			Utility.PopColor();
        }

        private static string GetPath(string subName, string keyName)
        {
            try
            {
                string keyString;

                if (Core.Is64Bit)
                    keyString = @"SOFTWARE\Wow6432Node\{0}";
                else
                    keyString = @"SOFTWARE\{0}";

                using (var key = Registry.LocalMachine.OpenSubKey(String.Format(keyString, subName)))
                {
                    if (key == null)
                        return null;

                    var v = key.GetValue(keyName) as string;

                    if (String.IsNullOrEmpty(v))
                        return null;

                    if (keyName == "InstallDir")
                        v = v + @"\";

                    v = Path.GetDirectoryName(v);

                    if (String.IsNullOrEmpty(v))
                        return null;

                    return v;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
