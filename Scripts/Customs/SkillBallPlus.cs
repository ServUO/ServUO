/* SkillBallPlus.cs v2.3.2 (Complete rewrite) by Ixtabay
Based on original SkillBall from Romanthebrain
Updates by Hawthornetr, ntony, JamzeMcC, MrNice, Ixtabay, and others since 2010
*/
using Server.Items;
using Server.Network;
using Server.Gumps;

namespace Server
{
	public class SkillPickGump : Gump
	{
		public static int skillsToBoost = 15;  // How many skills to boost
		private SkillBallPlus m_SkillBallPlus;
		public static double boostValue = 50;  // How high to boost each selected skill
		public string blueSix = "<BASEFONT SIZE=6 FACE=1 COLOR=#001052>";
		public string blueEight = "<BASEFONT SIZE=8 FACE=1 COLOR=#001052>";
		public string blueTen = "<BASEFONT SIZE=10 FACE=1 COLOR=#001052>";
		public string brownEight = "<BASEFONT SIZE=8 FACE=1 COLOR=#5a4a31>";
		public string endFont = "</BASEFONT>";

		private static Item MakeNewbie(Item item)
		{
			if (!Core.AOS)
				item.LootType = LootType.Newbied;

			return item;
		}
		public SkillPickGump(SkillBallPlus ball, Mobile player)
			: base(0, 0)
		{
			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;
			m_SkillBallPlus = ball;

			AddPage(0);
			// ----------------- y, x, y, x, id
			AddBackground(39, 33, 555, 545, 9380);
			AddHtml(67, 41, 1153, 20, blueTen + "Skillball Plus!" + endFont, false, false);
			AddHtml(67, 555, 1153, 20, blueTen + "Please select " + skillsToBoost + " skills to raise to " + boostValue + "" + endFont, false, false);
			AddButton(420, 555, 2071, 2072, (int)Buttons.Close, GumpButtonType.Reply, 0);
			AddButton(490, 555, 2311, 2312, (int)Buttons.FinishButton, GumpButtonType.Reply, 0);

			// Miscellaneous
			AddImage(183, 73, 2101);
			AddImage(200, 73, 2101);
			AddImage(217, 73, 2101);

			// Combat
			AddImage(132, 233, 2101);
			AddImage(149, 233, 2101);
			AddImage(166, 233, 2101);
			AddImage(183, 233, 2101);
			AddImage(200, 233, 2101);
			AddImage(217, 233, 2101);

			// Trade Skills
			AddImage(356, 73, 2101);
			AddImage(373, 73, 2101);
			AddImage(390, 73, 2101);
			AddImage(407, 73, 2101);

			// Magic
			AddImage(305, 293, 2101);
			AddImage(322, 293, 2101);
			AddImage(339, 293, 2101);
			AddImage(356, 293, 2101);
			AddImage(373, 293, 2101);
			AddImage(390, 293, 2101);
			AddImage(407, 293, 2101);

			// Wilderness
			AddImage(535, 73, 2101);
			AddImage(552, 73, 2101);

			// Thieving
			AddImage(518, 213, 2101);
			AddImage(535, 213, 2101);
			AddImage(552, 213, 2101);

			// Bard
			AddImage(484, 393, 2101);
			AddImage(501, 393, 2101);
			AddImage(518, 393, 2101);
			AddImage(535, 393, 2101);
			AddImage(552, 393, 2101);

			AddImage(65, 480, 9811); // Image of backpack
			AddHtml(122, 478, 1154, 20, blueSix + "Items added to" + endFont, false, false);
			AddHtml(122, 491, 1154, 20, blueSix + "backpack based" + endFont, false, false);
			AddHtml(122, 504, 1154, 20, blueSix + "based on your" + endFont, false, false);
			AddHtml(122, 517, 1154, 20, blueSix + "selected skills!" + endFont, false, false);

			// this.AddImage(400, 495, 52); // Signature line image
			AddHtml(422, 501, 1154, 20, blueTen + "Choose Carefully," + endFont, false, false);
			AddHtml(410, 518, 1154, 20, blueTen + "this cannot be undone!" + endFont, false, false);

			AddPage(1);
			//********************************************** Column 1 *****************************************
			AddImage(64, 68, 2086); // ------------------------------------------------------------------------  Miscellaneous
			AddCheck(65, 85, 2510, 2511, false, (int)SkillName.ArmsLore);
			AddCheck(65, 105, 2510, 2511, false, (int)SkillName.Begging);
			AddCheck(65, 125, 2510, 2511, false, (int)SkillName.Camping);
			AddCheck(65, 145, 2510, 2511, false, (int)SkillName.Cartography);
			AddCheck(65, 165, 2510, 2511, false, (int)SkillName.Forensics);
			AddCheck(65, 185, 2510, 2511, false, (int)SkillName.ItemID);
			AddCheck(65, 205, 2510, 2511, false, (int)SkillName.TasteID);
			AddImage(64, 228, 2086); // ----------------------------------------------------------------------- Combat
			AddCheck(65, 245, 2510, 2511, false, (int)SkillName.Anatomy);

			if (player.Race != Race.Gargoyle)
				AddCheck(65, 265, 2510, 2511, false, (int)SkillName.Archery);

			AddCheck(65, 285, 2510, 2511, false, (int)SkillName.Fencing);
			AddCheck(65, 305, 2510, 2511, false, (int)SkillName.Focus);
			AddCheck(65, 325, 2510, 2511, false, (int)SkillName.Healing);
			AddCheck(65, 345, 2510, 2511, false, (int)SkillName.Macing);
			AddCheck(65, 365, 2510, 2511, false, (int)SkillName.Parry);
			AddCheck(65, 385, 2510, 2511, false, (int)SkillName.Swords);
			AddCheck(65, 405, 2510, 2511, false, (int)SkillName.Tactics);

			if (Core.SA && player.Race == Race.Gargoyle)
					AddCheck(65, 425, 2510, 2511, false, (int)SkillName.Throwing);

			AddCheck(65, 445, 2510, 2511, false, (int)SkillName.Wrestling);
			AddHtml(85,  65, 2314, 20, blueEight  + "Miscellaneous" + endFont, false, false); // --------------  Miscellaneous
			AddHtml(85,  85, 2314, 20, brownEight + "Arms Lore" + endFont, false, false);
			AddHtml(85, 105, 2314, 20, brownEight + "Begging" + endFont, false, false);
			AddHtml(85, 125, 2314, 20, brownEight + "Camping" + endFont, false, false);
			AddHtml(85, 145, 2314, 20, brownEight + "Cartography" + endFont, false, false);
			AddHtml(85, 165, 2314, 20, brownEight + "Forensic Evaluation" + endFont, false, false);
			AddHtml(85, 185, 2314, 20, brownEight + "Item Identification" + endFont, false, false);
			AddHtml(85, 205, 2314, 20, brownEight + "Taste Identification" + endFont, false, false);
			AddHtml(85, 225, 2314, 20, blueEight  + "Combat" + endFont, false, false); // --------------------- Combat
			AddHtml(85, 245, 2314, 20, brownEight + "Anatomy" + endFont, false, false);
			AddHtml(85, 265, 2314, 20, brownEight + "Archery" + endFont, false, false);
			AddHtml(85, 285, 2314, 20, brownEight + "Fencing" + endFont, false, false);
			AddHtml(85, 305, 2314, 20, brownEight + "Focus" + endFont, false, false);
			AddHtml(85, 325, 2314, 20, brownEight + "Healing" + endFont, false, false);
			AddHtml(85, 345, 2314, 20, brownEight + "Mace Fighting" + endFont, false, false);
			AddHtml(85, 365, 2314, 20, brownEight + "Parrying" + endFont, false, false);
			AddHtml(85, 385, 2314, 20, brownEight + "Swordfighting" + endFont, false, false);
			AddHtml(85, 405, 2314, 20, brownEight + "Tactics" + endFont, false, false);
			AddHtml(85, 425, 2314, 20, brownEight + "Throwing" + endFont, false, false);
			AddHtml(85, 445, 2314, 20, brownEight + "Wrestling" + endFont, false, false);
			// ********************************************** Column 2 ****************************************
			AddImage(239, 68, 2086); // -----------------------------------------------------------------------  Trade Skills
			AddCheck(240, 85, 2510, 2511, false, (int)SkillName.Alchemy);
			AddCheck(240, 105, 2510, 2511, false, (int)SkillName.Blacksmith);
			AddCheck(240, 125, 2510, 2511, false, (int)SkillName.Fletching);
			AddCheck(240, 145, 2510, 2511, false, (int)SkillName.Carpentry);
			AddCheck(240, 165, 2510, 2511, false, (int)SkillName.Cooking);
			AddCheck(240, 185, 2510, 2511, false, (int)SkillName.Inscribe);
			AddCheck(240, 205, 2510, 2511, false, (int)SkillName.Lumberjacking);
			AddCheck(240, 225, 2510, 2511, false, (int)SkillName.Mining);
			AddCheck(240, 245, 2510, 2511, false, (int)SkillName.Tailoring);
			AddCheck(240, 265, 2510, 2511, false, (int)SkillName.Tinkering);
			AddImage(239, 288, 2086); // ----------------------------------------------------------------------  Magic
			if (Core.SE)
				AddCheck(240, 305, 2510, 2511, false, (int)SkillName.Bushido);

			if (Core.AOS)
				AddCheck(240, 325, 2510, 2511, false, (int)SkillName.Chivalry);

			AddCheck(240, 345, 2510, 2511, false, (int)SkillName.EvalInt);

			if (Core.SA)
				AddCheck(240, 365, 2510, 2511, false, (int)SkillName.Imbuing);

			AddCheck(240, 385, 2510, 2511, false, (int)SkillName.Magery);
			AddCheck(240, 405, 2510, 2511, false, (int)SkillName.Meditation);

			if (Core.SA)
				AddCheck(240, 425, 2510, 2511, false, (int)SkillName.Mysticism);

			if (Core.AOS)
				AddCheck(240, 445, 2510, 2511, false, (int)SkillName.Necromancy);

			if (Core.SE)
				AddCheck(240, 465, 2510, 2511, false, (int)SkillName.Ninjitsu);
			AddCheck(240, 485, 2510, 2511, false, (int)SkillName.MagicResist);

			if (Core.ML)
				AddCheck(240, 505, 2510, 2511, false, (int)SkillName.Spellweaving);

			AddCheck(240, 525, 2510, 2511, false, (int)SkillName.SpiritSpeak);
			AddHtml(259,  65, 2314, 20, blueEight  + "Trade Skills" + endFont, false, false); // --------------  Trade Skills
			AddHtml(260,  85, 2314, 20, brownEight + "Alchemy" + endFont, false, false);
			AddHtml(260, 105, 2314, 20, brownEight + "Blacksmithy" + endFont, false, false);
			AddHtml(260, 125, 2314, 20, brownEight + "Bowcraft/Fletching" + endFont, false, false);
			AddHtml(260, 145, 2314, 20, brownEight + "Carpentry" + endFont, false, false);
			AddHtml(260, 165, 2314, 20, brownEight + "Cooking" + endFont, false, false);
			AddHtml(260, 185, 2314, 20, brownEight + "Inscription" + endFont, false, false);
			AddHtml(260, 205, 2314, 20, brownEight + "Lumberjacking" + endFont, false, false);
			AddHtml(260, 225, 2314, 20, brownEight + "Mining" + endFont, false, false);
			AddHtml(260, 245, 2314, 20, brownEight + "Tailoring" + endFont, false, false);
			AddHtml(259, 265, 2314, 20, brownEight + "Tinkering" + endFont, false, false);
			AddHtml(260, 285, 2314, 20, blueEight  + "Magic" + endFont, false, false); // ---------------------  Magic
			AddHtml(260, 305, 2314, 20, brownEight + "Bushido" + endFont, false, false);
			AddHtml(260, 325, 2314, 20, brownEight + "Chivalry" + endFont, false, false);
			AddHtml(260, 345, 2314, 20, brownEight + "Evaluating Intelligence" + endFont, false, false);
			AddHtml(260, 365, 2314, 20, brownEight + "Imbuing" + endFont, false, false);
			AddHtml(260, 385, 2314, 20, brownEight + "Magery" + endFont, false, false);
			AddHtml(260, 405, 2314, 20, brownEight + "Meditation" + endFont, false, false);
			AddHtml(260, 425, 2314, 20, brownEight + "Mysticism" + endFont, false, false);
			AddHtml(260, 445, 2314, 20, brownEight + "Necromancy" + endFont, false, false);
			AddHtml(260, 465, 2314, 20, brownEight + "Ninjitsu" + endFont, false, false);
			AddHtml(260, 485, 2314, 20, brownEight + "Resisting Spells" + endFont, false, false);
			AddHtml(260, 505, 2314, 20, brownEight + "Spellweaving" + endFont, false, false);
			AddHtml(260, 525, 2314, 20, brownEight + "Spirit Speak" + endFont, false, false);
			// ************************************************* Column 3 *************************************
			AddImage(429, 68, 2086); // -----------------------------------------------------------------------  Wilderness
			AddCheck(430, 85, 2510, 2511, false, (int)SkillName.AnimalLore);
			AddCheck(430, 105, 2510, 2511, false, (int)SkillName.AnimalTaming);
			AddCheck(430, 125, 2510, 2511, false, (int)SkillName.Fishing);
			AddCheck(430, 145, 2510, 2511, false, (int)SkillName.Herding);
			AddCheck(430, 165, 2510, 2511, false, (int)SkillName.Tracking);
			AddCheck(430, 185, 2510, 2511, false, (int)SkillName.Veterinary);
			AddImage(429, 208, 2086); // ----------------------------------------------------------------------  Thieving
			AddCheck(430, 225, 2510, 2511, false, (int)SkillName.DetectHidden);
			AddCheck(430, 245, 2510, 2511, false, (int)SkillName.Hiding);
			AddCheck(430, 265, 2510, 2511, false, (int)SkillName.Lockpicking);
			AddCheck(430, 285, 2510, 2511, false, (int)SkillName.Poisoning);
			AddCheck(430, 305, 2510, 2511, false, (int)SkillName.RemoveTrap);
			AddCheck(430, 325, 2510, 2511, false, (int)SkillName.Snooping);
			AddCheck(430, 345, 2510, 2511, false, (int)SkillName.Stealing);
			AddCheck(430, 365, 2510, 2511, false, (int)SkillName.Stealth);
			AddImage(429, 388, 2086); // ----------------------------------------------------------------------  Bard
			AddCheck(430, 405, 2510, 2511, false, (int)SkillName.Discordance);
			AddCheck(430, 425, 2510, 2511, false, (int)SkillName.Musicianship);
			AddCheck(430, 445, 2510, 2511, false, (int)SkillName.Peacemaking);
			AddCheck(430, 465, 2510, 2511, false, (int)SkillName.Provocation);
			AddHtml(450, 65, 2314, 20, blueEight + "Wilderness" + endFont, false, false); // ------------------  Wilderness
			AddHtml(450, 85, 2314, 20, brownEight + "Animal Lore" + endFont, false, false);
			AddHtml(450, 105, 2314, 20, brownEight + "Animal Taming" + endFont, false, false);
			AddHtml(450, 125, 2314, 20, brownEight + "Fishing" + endFont, false, false);
			AddHtml(450, 145, 2314, 20, brownEight + "Herding" + endFont, false, false);
			AddHtml(450, 165, 2314, 20, brownEight + "Tracking" + endFont, false, false);
			AddHtml(450, 185, 2314, 20, brownEight + "Veterinary" + endFont, false, false);
			AddHtml(450, 205, 2314, 20, blueEight + "Thieving" + endFont, false, false); // -------------------  Thieving
			AddHtml(450, 225, 2314, 20, brownEight + "Detect Hidden" + endFont, false, false);
			AddHtml(450, 245, 2314, 20, brownEight + "Hiding" + endFont, false, false);
			AddHtml(450, 265, 2314, 20, brownEight + "Lockpicking" + endFont, false, false);
			AddHtml(450, 285, 2314, 20, brownEight + "Poisoning" + endFont, false, false);
			AddHtml(450, 305, 2314, 20, brownEight + "Remove Trap" + endFont, false, false);
			AddHtml(450, 325, 2314, 20, brownEight + "Snooping" + endFont, false, false);
			AddHtml(450, 345, 2314, 20, brownEight + "Stealing" + endFont, false, false);
			AddHtml(450, 365, 2314, 20, brownEight + "Stealth" + endFont, false, false);
			AddHtml(450, 385, 2314, 20, blueEight + "Bard" + endFont, false, false); // -----------------------  Bard
			AddHtml(450, 405, 2314, 20, brownEight + "Discordance" + endFont, false, false);
			AddHtml(450, 425, 2314, 20, brownEight + "Musicianship" + endFont, false, false);
			AddHtml(450, 445, 2314, 20, brownEight + "Peacemaking" + endFont, false, false);
			AddHtml(450, 465, 2314, 20, brownEight + "Provocation" + endFont, false, false);
			//*************************************************************************************************



		}

		public enum Buttons
		{
			Close,
			FinishButton,
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{
			Mobile m = state.Mobile;

			switch (info.ButtonID)
			{
				case 0: { break; }
				case 1:
					{

						if (info.Switches.Length < skillsToBoost)
						{
							m.SendGump(new SkillPickGump(m_SkillBallPlus, m));
							m.SendMessage(0, "Please try again.  You must pick {0} more skills for a total of " + skillsToBoost + ".", skillsToBoost - info.Switches.Length);
							break;
						}
						else if (info.Switches.Length > skillsToBoost)
						{
							m.SendGump(new SkillPickGump(m_SkillBallPlus, m));
							m.SendMessage(0, "Please try again.  You choose {0} more skills than the " + skillsToBoost + " allowed.", info.Switches.Length - skillsToBoost);
							break;
						}

						else
						{
							Skills skills = m.Skills;
							Container pack = m.Backpack;

							//for (int i = 0; i < skills.Length; ++i) // If you want to set all skills to zero uncomment this
							// skills[i].Base = 0;
							for (int i = 0; i < skills.Length; ++i)
								if (info.IsSwitched(i))
									m.Skills[i].Base = boostValue;
							if (pack != null)
								for (int i = 0; i < skills.Length; ++i)
									if (info.IsSwitched(i))
										switch ((SkillName)i)
										{
											case SkillName.Alchemy:
												pack.DropItem(new Bottle(3));
												pack.DropItem(new MortarPestle());
												pack.DropItem(new BagOfReagents(10));
												break;
											case SkillName.Anatomy:
												pack.DropItem(new Bandage(10));
												break;
											case SkillName.AnimalLore:
												pack.DropItem(new ShepherdsCrook());
												break;
											case SkillName.AnimalTaming:
												pack.DropItem(new Apple(5));
												break;
											case SkillName.Archery:
												pack.DropItem(new Arrow(50));
												pack.DropItem(new Bow());
												break;
											case SkillName.ArmsLore:
												pack.DropItem(new IronIngot(10));
												break;
											case SkillName.Begging:
												pack.DropItem(new BankCheck(10000));
												break;
											case SkillName.Blacksmith:
												pack.DropItem(new IronIngot(10));
												pack.DropItem(new Tongs());
												break;
											case SkillName.Camping:
												pack.DropItem(new Bedroll());
												pack.DropItem(new Kindling(3));
												break;
											case SkillName.Carpentry:
												pack.DropItem(new Saw());
												pack.DropItem(new Board(10));
												break;
											case SkillName.Cooking:
												pack.DropItem(new Kindling(3));
												pack.DropItem(new RawFishSteak(10));
												break;
											case SkillName.Fishing:
												pack.DropItem(new FishingPole());
												pack.DropItem(new FloppyHat(Utility.RandomYellowHue()));
												break;
											case SkillName.Healing:
												pack.DropItem(new Bandage(10));
												pack.DropItem(new Scissors());
												break;
											case SkillName.Herding:
												pack.DropItem(new ShepherdsCrook());
												break;
											case SkillName.Lockpicking:
												pack.DropItem(new Lockpick(10));
												break;
											case SkillName.Lumberjacking:
												pack.DropItem(new Hatchet());
												pack.DropItem(new FullApron(Utility.RandomYellowHue()));
												break;
											case SkillName.Magery:
												pack.DropItem(new Spellbook(ulong.MaxValue));
												pack.DropItem(new BagOfReagents(10));
												break;
											case SkillName.Mining:
												pack.DropItem(new Pickaxe());
												pack.DropItem(new Shovel());
												break;
											case SkillName.Musicianship:
												pack.DropItem(new Lute());
												pack.DropItem(new TambourineTassel());
												pack.DropItem(new Drums());
												break;
											case SkillName.RemoveTrap:
												pack.DropItem(new GreaterHealPotion(3));
												break;
											case SkillName.Snooping:
												goto case SkillName.Stealing;
											case SkillName.Stealing:
												pack.DropItem(new BankCheck(1000));
												break;
											case SkillName.Stealth:
												pack.DropItem(new BurglarsBandana());
												break;
											case SkillName.Tailoring:
												pack.DropItem(new Cloth(10));
												pack.DropItem(new SewingKit());
												break;
											case SkillName.Tinkering:
												pack.DropItem(new TinkerTools());
												pack.DropItem(new IronIngot(10));
												break;
											case SkillName.Veterinary:
												goto case SkillName.Anatomy;
											case SkillName.Fencing:
												pack.DropItem(new Kryss());
												break;
											case SkillName.Macing:
												pack.DropItem(new Mace());
												break;
											case SkillName.Parry:
												pack.DropItem(new MetalKiteShield());
												break;
											case SkillName.Swords:
												pack.DropItem(new Longsword());
												break;
											case SkillName.Wrestling:
												pack.DropItem(new LeatherGloves());
												break;
											case SkillName.Cartography:
												pack.DropItem(new BlankMap());
												pack.DropItem(new Sextant());
												break;
											case SkillName.DetectHidden:
												pack.DropItem(new Cloak(0x455));
												break;
											case SkillName.Inscribe:
												pack.DropItem(new BlankScroll(5));
												pack.DropItem(new BlueBook());
												break;
											case SkillName.Peacemaking:
												pack.DropItem(new Tambourine());
												break;
											case SkillName.Poisoning:
												pack.DropItem(new LesserPoisonPotion());
												pack.DropItem(new LesserPoisonPotion());
												pack.DropItem(new LesserPoisonPotion());
												break;
											case SkillName.Provocation:
												pack.DropItem(new BambooFlute());
												break;
											case SkillName.SpiritSpeak:
												pack.DropItem(new BagOfNecroReagents(10));
												break;
											case SkillName.Tracking:
												pack.DropItem(new BearMask(0x1545));
												break;
											case SkillName.EvalInt:
												pack.DropItem(new BagOfReagents(10));
												break;
											case SkillName.TasteID:
												pack.DropItem(new GreaterHealPotion(1));
												pack.DropItem(new GreaterAgilityPotion(1));
												pack.DropItem(new GreaterStrengthPotion(1));
												break;
											case SkillName.Hiding:
												pack.DropItem(new Robe(0x497));
												break;
											case SkillName.Fletching:
												pack.DropItem(new FletcherTools(0x1022));
												pack.DropItem(new Shaft(10));
												pack.DropItem(new Feather(10));
												break;
											case SkillName.Throwing:
												pack.DropItem(new ThrowingDagger());
												break;
											case SkillName.Bushido:
												pack.DropItem(new BookOfBushido());
												break;
											case SkillName.Chivalry:
												pack.DropItem(new BookOfChivalry((ulong)0x3FF));
												break;
											case SkillName.Imbuing:
												pack.DropItem(new RunicHammer(CraftResource.Valorite));
												break;
											case SkillName.Mysticism:
												pack.DropItem(new Bone(10));
												pack.DropItem(new DaemonBone(10));
												pack.DropItem(new FertileDirt(10));
												break;
											case SkillName.Necromancy:
												pack.DropItem(new NecromancerSpellbook((ulong)0xFFFF));
												pack.DropItem(new BagOfNecroReagents(10));
												break;
											case SkillName.Ninjitsu:
												pack.DropItem(new BookOfNinjitsu());
												break;
											case SkillName.Spellweaving:
												new SpellweavingBook((ulong)0xFFFF);
												break;
											case SkillName.Discordance:
												pack.DropItem(new Harp());
												break;
										}
							m_SkillBallPlus.Delete();
						}
						break;
					}
			}
		}

	}

	public class SkillBallPlus : Item
	{
		private int m_skillsToBoost = SkillPickGump.skillsToBoost; // Default number of skills to boost
		private double m_boostValue = SkillPickGump.boostValue; // Default level skills will be boosted to
		private string m_BaseName = " Skill Booster with Items";

		[CommandProperty(AccessLevel.GameMaster)]
		public int skillsToBoost
		{
			get { return m_skillsToBoost; }
			set
			{
				m_skillsToBoost = value;
			}
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public double boostValue
		{
			get { return m_boostValue; }
			set
			{
				m_boostValue = value;
			}
		}

		public override string DefaultName
		{
			get
			{
				return m_skillsToBoost + "/" + m_boostValue + m_BaseName;
			}
		}

		[Constructable]
		public SkillBallPlus() : base(0xE73)
		{
			Weight = 1.0;
			Hue = 1287;
			Movable = false;
		}
		public override void OnDoubleClick(Mobile m)
		{

			if (m.Backpack != null && m.Backpack.GetAmount(typeof(SkillBallPlus)) > 0)
			{
				m.SendMessage("Please choose " + SkillPickGump.skillsToBoost + " skills to set to " + SkillPickGump.boostValue + ".");
				m.CloseGump(typeof(SkillPickGump));
				m.SendGump(new SkillPickGump(this, m));
			}
			else
				m.SendMessage(" This must be in your backpack to function.");

		}

		public SkillBallPlus(Serial serial) : base(serial)
		{}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}

	}
}