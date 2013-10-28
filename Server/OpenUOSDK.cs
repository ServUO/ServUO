#region Header
// **********
// ServUO - OpenUOSDK.cs
// **********
#endregion

#region References
using System;
using System.IO;
using System.Linq;

using OpenUO.Core.Patterns;
using OpenUO.Ultima;
using OpenUO.Ultima.Windows.Forms;
#endregion

namespace Server
{
	public class OpenUOSDK
	{
		//! You should point this to a directory containing a COPY of your client
		//! files if you are having conflict issues with ServUO using the same files
		//! that your client is using.
		//!+ Example: private static string _ClientData = @"C:\Server Files";
		public static string ClientDataPath = @"C:\Server";

		public static AnimationDataFactory AnimationDataFactory { get; set; }
		public static AnimationFactory AnimationFactory { get; set; }
		public static ArtworkFactory ArtFactory { get; set; }
		public static ASCIIFontFactory AsciiFontFactory { get; set; }
		public static ClilocFactory ClilocFactory { get; set; }
		public static GumpFactory GumpFactory { get; set; }
		public static SkillsFactory SkillsFactory { get; set; }
		public static SoundFactory SoundFactory { get; set; }
		public static TexmapFactory TexmapFactory { get; set; }
		public static UnicodeFontFactory UnicodeFontFactory { get; set; }

		public OpenUOSDK(string path = "")
		{
			if (ClientDataPath == null && !String.IsNullOrWhiteSpace(path))
			{
				if (Directory.Exists(path))
				{
					ClientDataPath = path;
				}
			}

			Container container = new Container();
			container.RegisterModule<UltimaSDKCoreModule>();
			container.RegisterModule<UltimaSDKBitmapModule>();

			InstallLocation location = (ClientDataPath == null
											? InstallationLocator.Locate().FirstOrDefault()
											: (InstallLocation)ClientDataPath);

			if (location == null || String.IsNullOrWhiteSpace(location) || !Directory.Exists(location.ToString()))
			{
				Utility.PushColor(ConsoleColor.Red);
				Console.WriteLine("OpenUO Error: Client files not found.");
				Utility.PopColor();
			}

			AnimationDataFactory = new AnimationDataFactory(location, container);
			AnimationFactory = new AnimationFactory(location, container);
			ArtFactory = new ArtworkFactory(location, container);
			AsciiFontFactory = new ASCIIFontFactory(location, container);
			ClilocFactory = new ClilocFactory(location, container);
			GumpFactory = new GumpFactory(location, container);
			SkillsFactory = new SkillsFactory(location, container);
			SoundFactory = new SoundFactory(location, container);
			TexmapFactory = new TexmapFactory(location, container);
			UnicodeFontFactory = new UnicodeFontFactory(location, container);
		}
	}
}