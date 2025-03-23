#region Header
//               _,-'/-'/
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2023  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #                                       #
#endregion

#region References
using System;
using System.IO;

using Server;
#endregion

namespace VitaNex
{
	public static class ArtworkSupport
	{
		public static short[] LandTextures { get; private set; }
		public static short[] StaticAnimations { get; private set; }

		static ArtworkSupport()
		{
			LandTextures = new short[TileData.MaxLandValue];
			LandTextures.SetAll((short)-1);

			StaticAnimations = new short[TileData.MaxItemValue];
			StaticAnimations.SetAll((short)-1);

			var filePath = Core.FindDataFile("tiledata.mul");

			if (String.IsNullOrWhiteSpace(filePath))
			{
				return;
			}

			var file = new FileInfo(filePath);

			if (!file.Exists)
			{
				return;
			}

			using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				var bin = new BinaryReader(fs);
				var buffer = new byte[25];

				if (fs.Length == 3188736)
				{
					// 7.0.9.0
					LandTextures = new short[0x4000];

					for (var i = 0; i < 0x4000; ++i)
					{
						if (i == 1 || (i > 0 && (i & 0x1F) == 0))
						{
							bin.Read(buffer, 0, 4);
						}

						bin.Read(buffer, 0, 8);

						var texture = bin.ReadInt16();

						bin.Read(buffer, 0, 20);

						LandTextures[i] = texture;
					}

					StaticAnimations = new short[0x10000];

					for (var i = 0; i < 0x10000; ++i)
					{
						if ((i & 0x1F) == 0)
						{
							bin.Read(buffer, 0, 4);
						}

						bin.Read(buffer, 0, 14);

						var anim = bin.ReadInt16();

						bin.Read(buffer, 0, 25);

						StaticAnimations[i] = anim;
					}
				}
				else
				{
					LandTextures = new short[0x4000];

					for (var i = 0; i < 0x4000; ++i)
					{
						if ((i & 0x1F) == 0)
						{
							bin.Read(buffer, 0, 4);
						}

						bin.Read(buffer, 0, 4);

						var texture = bin.ReadInt16();

						bin.Read(buffer, 0, 20);

						LandTextures[i] = texture;
					}

					if (fs.Length == 1644544)
					{
						// 7.0.0.0
						StaticAnimations = new short[0x8000];

						for (var i = 0; i < 0x8000; ++i)
						{
							if ((i & 0x1F) == 0)
							{
								bin.Read(buffer, 0, 4);
							}

							bin.Read(buffer, 0, 10);

							var anim = bin.ReadInt16();

							bin.Read(buffer, 0, 25);

							StaticAnimations[i] = anim;
						}
					}
					else
					{
						StaticAnimations = new short[0x4000];

						for (var i = 0; i < 0x4000; ++i)
						{
							if ((i & 0x1F) == 0)
							{
								bin.Read(buffer, 0, 4);
							}

							bin.Read(buffer, 0, 10);

							var anim = bin.ReadInt16();

							bin.Read(buffer, 0, 25);

							StaticAnimations[i] = anim;
						}
					}
				}
			}
		}

		public static short LookupTexture(int landID)
		{
			return LandTextures.InBounds(landID) ? LandTextures[landID] : (short)0;
		}

		public static short LookupAnimation(int staticID)
		{
			return StaticAnimations.InBounds(staticID) ? StaticAnimations[staticID] : (short)0;
		}

		public static int LookupGump(int staticID, bool female)
		{
			int value = LookupAnimation(staticID);

			if (value > 0)
			{
				value += female ? 60000 : 50000;
			}

			return value;
		}

		public static int LookupGump(Body body)
		{
			switch (body.BodyID)
			{
				case 183:
				case 185:
				case 400:
				case 402:
					return 12;
				case 184:
				case 186:
				case 401:
				case 403:
					return 13;
				case 605:
				case 607:
					return 14;
				case 606:
				case 608:
					return 15;
				case 666:
					return 666;
				case 667:
					return 665;
				case 694:
					return 666;
				case 695:
					return 665;
				case 750:
					return 12;
				case 751:
					return 13;
			}

			return -1;
		}
	}
}