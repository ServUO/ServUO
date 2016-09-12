using System;
using System.IO;
using System.Linq;

using Microsoft.Win32;

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
        private static readonly string CustomPath = Config.Get(@"DataPath.CustomPath", null);
        private static readonly bool IgnoreStandardPaths = Config.Get("DataPath.IgnoreStandardPaths", false);
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
            if (CustomPath != null)
                Core.DataDirectories.Add(CustomPath);

            if (!IgnoreStandardPaths)
            {
                string pathUO = GetPath(@"Origin Worlds Online\Ultima Online\1.0", "ExePath");
                string pathTD = GetPath(@"Origin Worlds Online\Ultima Online Third Dawn\1.0", "ExePath"); //These refer to 2D & 3D, not the Third Dawn expansion
                string pathKR = GetPath(@"Origin Worlds Online\Ultima Online\KR Legacy Beta", "ExePath"); //After KR, This is the new registry key for the 2D client
                string pathSA = GetPath(@"Electronic Arts\EA Games\Ultima Online Stygian Abyss Classic", "InstallDir");
                string pathHS = GetPath(@"Electronic Arts\EA Games\Ultima Online Classic", "InstallDir");

                if (pathUO != null)
                    Core.DataDirectories.Add(pathUO);

                if (pathTD != null)
                    Core.DataDirectories.Add(pathTD);

                if (pathKR != null)
                    Core.DataDirectories.Add(pathKR);

                if (pathSA != null)
                    Core.DataDirectories.Add(pathSA);

                if (pathHS != null)
                    Core.DataDirectories.Add(pathHS);
            }

            if (Core.DataDirectories.Count == 0 && !Core.Service)
            {
                Console.WriteLine("Enter the Ultima Online directory:");
                Console.Write("> ");

                Core.DataDirectories.Add(Console.ReadLine());
            }

	        foreach (var dir in Core.DataDirectories)
	        {
		        Ultima.Files.SetMulPath(dir);
	        }
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

                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(String.Format(keyString, subName)))
                {
                    if (key == null)
                        return null;

                    string v = key.GetValue(keyName) as string;

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