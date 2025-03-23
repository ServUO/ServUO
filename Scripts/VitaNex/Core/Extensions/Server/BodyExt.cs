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
using System.Collections.Generic;

using VitaNex;
#endregion

namespace Server
{
	public static class BodyExtUtility
	{
		public static Dictionary<int, string> Names { get; private set; }

		static BodyExtUtility()
		{
			Names = new Dictionary<int, string>();
		}

		public static string GetName(this Body body)
		{
			if (body.IsEmpty)
			{
				return String.Empty;
			}


			if (Names.TryGetValue(body.BodyID, out var name) && !String.IsNullOrWhiteSpace(name))
			{
				return name;
			}

			var itemID = ShrinkTable.Lookup(body.BodyID) & TileData.MaxItemValue;

			if (itemID == ShrinkTable.DefaultItemID)
			{
				name = String.Empty;
			}

			if (String.IsNullOrWhiteSpace(name))
			{
				name = ClilocLNG.NULL.GetRawString(itemID + (itemID < 0x4000 ? 1020000 : 1078872));
			}

			if (String.IsNullOrWhiteSpace(name))
			{
				name = TileData.ItemTable[itemID].Name;
			}

			if (String.IsNullOrWhiteSpace(name))
			{
				name = body.Type.ToString();
			}

			if (!String.IsNullOrWhiteSpace(name))
			{
				name = name.SpaceWords().ToUpperWords();

				name = String.Concat(" ", name, " ");

				if (body.IsHuman || body.IsGhost)
				{
					if (body >= 400 && body <= 403)
					{
						name = "Human";
					}
					else if (body >= 605 && body <= 608)
					{
						name = "Elf";
					}
					else if ((body >= 666 && body <= 667) || (body >= 694 && body <= 695))
					{
						name = "Gargoyle";
					}

					if (body.IsMale && !Insensitive.Contains(name, "Male"))
					{
						name += " Male";
					}
					else if (body.IsFemale && !Insensitive.Contains(name, "Female"))
					{
						name += " Female";
					}

					if (body.IsGhost && !Insensitive.Contains(name, "Ghost"))
					{
						name += " Ghost";
					}
				}
				else
				{
					switch (itemID)
					{
						case 9611:
							name = "Evil Mage";
							break;
						case 9776:
							name = "Wanderer Of The Void";
							break;
						case 11676:
							name = "Charger";
							break;
						case 38990:
							name = "Baby Dragon Turtle";
							break;
						case 40369:
							name = "Aztec Golem";
							break;
						case 40374:
							name = "Myrmadex Queen";
							break;
						case 40420:
							name = "Spector";
							break;
						case 40429:
							name = "T-Rex";
							break;
						case 40501:
							name = "Rainbow Unicorn";
							break;
						case 40661:
							name = "Windrunner";
							break;
						case 40704:
							name = "Sabertooth Tiger";
							break;
						case 40705:
							name = "Small Platinum Dragon";
							break;
						case 40706:
							name = "Platinum Dragon";
							break;
						case 40710:
							name = "Small Crimson Dragon";
							break;
						case 40711:
							name = "Crimson Dragon";
							break;
						case 40713:
							name = "Small Fox";
							break;
						case 40714:
							name = "Small Stygian Dragon";
							break;
						case 40718:
							name = "Stygian Dragon";
							break;
						case 40976:
							name = "Eastern Dragon";
							break;
					}
				}

				name = name.Replace(" Fr ", " Frame ");

				name = name.Replace("Frame", String.Empty);
				name = name.Replace("Statuette", String.Empty);
				name = name.Replace("Statue", String.Empty);

				name = name.StripExcessWhiteSpace().Trim();
			}

			if (String.IsNullOrWhiteSpace(name))
			{
				name = body.Type.ToString();
			}

			Names[body.BodyID] = name;

			return name;
		}

		public static int GetPaperdoll(this Body body)
		{
			return ArtworkSupport.LookupGump(body);
		}

		public static bool HasPaperdoll(this Body body)
		{
			return GetPaperdoll(body) >= 0;
		}
	}
}