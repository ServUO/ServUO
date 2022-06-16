#region References
using System;

using Server.Accounting;
using Server.Items;
using Server.Mobiles;
using Server.Network;
#endregion

namespace Server.Misc
{
	public class CharacterCreation
	{
		public static void Initialize()
		{
			// Register our event handler
			EventSink.CharacterCreation += EventSink_CharacterCreation;
		}

		private static void AddBackpack(Mobile m)
		{
			var pack = m.Backpack;

			if (pack == null)
			{
				pack = new Backpack
				{
					Movable = false
				};

				m.AddItem(pack);
			}

			PackItem(m, new Gold(1000)); // Starting gold can be customized here
		}

		private static void AddShirt(Mobile m, int shirtHue)
		{
			var hue = Utility.ClipDyedHue(shirtHue & 0x3FFF);

			if (m.Race == Race.Elf)
			{
				EquipItem(m, new ElvenShirt(hue), true);
			}
			else if (m.Race == Race.Human)
			{
				switch (Utility.Random(3))
				{
					case 0:
						EquipItem(m, new Shirt(hue), true);
						break;
					case 1:
						EquipItem(m, new FancyShirt(hue), true);
						break;
					case 2:
						EquipItem(m, new Doublet(hue), true);
						break;
				}
			}
			else if (m.Race == Race.Gargoyle)
			{
				EquipItem(m, new GargishClothChestArmor(hue));
			}
		}

		private static void AddPants(Mobile m, int pantsHue)
		{
			var hue = Utility.ClipDyedHue(pantsHue & 0x3FFF);

			if (m.Race == Race.Elf)
			{
				EquipItem(m, new ElvenPants(hue), true);
			}
			else if (m.Race == Race.Human)
			{
				if (m.Female)
				{
					switch (Utility.Random(2))
					{
						case 0:
							EquipItem(m, new Skirt(hue), true);
							break;
						case 1:
							EquipItem(m, new Kilt(hue), true);
							break;
					}
				}
				else
				{
					switch (Utility.Random(2))
					{
						case 0:
							EquipItem(m, new LongPants(hue), true);
							break;
						case 1:
							EquipItem(m, new ShortPants(hue), true);
							break;
					}
				}
			}
			else if (m.Race == Race.Gargoyle)
			{
				EquipItem(m, new GargishClothKiltArmor(hue));
			}
		}

		private static void AddShoes(Mobile m)
		{
			if (m.Race == Race.Elf)
				EquipItem(m, new ElvenBoots(), true);
			else if (m.Race == Race.Human)
				EquipItem(m, new Shoes(Utility.RandomYellowHue()), true);
		}

		private static Mobile CreateMobile(IAccount a)
		{
			if (a.Count >= a.Limit)
				return null;

			for (var i = 0; i < a.Length; ++i)
			{
				if (a[i] == null)
					return a[i] = new PlayerMobile();
			}

			return null;
		}

		private static void EventSink_CharacterCreation(CharacterCreationEventArgs args)
		{
			var state = args.State;

			if (state == null)
			{
				return;
			}

			var newChar = args.Mobile ?? CreateMobile(state.Account);

			if (newChar == null)
			{
				Utility.WriteLine(ConsoleColor.Red, "Login: {0}: Character creation failed, account full", state);
				return;
			}

			args.Mobile = newChar;

			newChar.Player = true;
			newChar.AccessLevel = args.Account.AccessLevel;
			newChar.Female = args.Female;

			var race = args.Race;

			if (Core.Expansion < race.RequiredExpansion)
			{
				race = Race.DefaultRace;
			}

			newChar.Race = race;

			newChar.BodyHue = race.ClipSkinHue(args.SkinHue) | Mobile.HuePartialFlag;

			newChar.Hunger = 20;

			var young = false;

			if (newChar is PlayerMobile pm)
			{
				pm.AutoRenewInsurance = true;

				var skillcap = Config.Get("PlayerCaps.SkillCap", 1000) / 10.0;

				if (skillcap != 100.0)
				{
					foreach (var s in pm.Skills)
						s.Cap = skillcap;
				}

				pm.Profession = args.Profession;

				if (pm.IsPlayer() && pm.Account.Young && !Siege.SiegeShard)
					young = pm.Young = true;
			}

			SetName(newChar, args.Name);

			AddBackpack(newChar);

			SetStats(newChar, state, args.Stats, args.Profession);
			SetSkills(newChar, args.Skills, args.Profession);

			if (race.ValidateHair(newChar, args.HairID))
			{
				newChar.HairItemID = args.HairID;
				newChar.HairHue = race.ClipHairHue(args.HairHue & 0x3FFF);
			}

			if (race.ValidateFacialHair(newChar, args.BeardID))
			{
				newChar.FacialHairItemID = args.BeardID;
				newChar.FacialHairHue = race.ClipHairHue(args.BeardHue & 0x3FFF);
			}

			if (race.ValidateFace(newChar.Female, args.FaceID))
			{
				newChar.FaceItemID = args.FaceID;
				newChar.FaceHue = args.FaceHue;
			}
			else
			{
				newChar.FaceItemID = race.RandomFace(newChar.Female);
				newChar.FaceHue = newChar.Hue;
			}

			if (args.Profession <= Profession.Blacksmith)
			{
				AddShirt(newChar, args.ShirtHue);
				AddPants(newChar, args.PantsHue);
				AddShoes(newChar);
			}

			if (TestCenter.Enabled)
				TestCenter.FillBankbox(newChar);

			if (young)
			{
				var ticket = new NewPlayerTicket
				{
					Owner = newChar
				};

				newChar.BankBox.DropItem(ticket);
			}

			var city = args.City;
			var map = Siege.SiegeShard && city.Map == Map.Trammel ? Map.Felucca : city.Map;

			newChar.MoveToWorld(city.Location, map);

			Utility.WriteLine(ConsoleColor.Green, "Login: {0}: New character being created (account={1})", state, args.Account.Username);
			Utility.WriteLine(ConsoleColor.DarkGreen, " - Character: {0} (serial={1})", newChar.Name, newChar.Serial);
			Utility.WriteLine(ConsoleColor.DarkGreen, " - Started: {0} {1} in {2}", city.City, city.Location, city.Map);
		}

		private static void SetName(Mobile m, string name)
		{
			name = name.Trim();

			if (!NameVerification.Validate(name, 2, 16, true, false, true, 1, NameVerification.SpaceDashPeriodQuote))
				name = "Generic Player";

			m.Name = name;
		}

		private static void FixStats(ref int str, ref int dex, ref int intel, int max)
		{
			var vMax = max - 30;

			var vStr = str - 10;
			var vDex = dex - 10;
			var vInt = intel - 10;

			if (vStr < 0)
				vStr = 0;

			if (vDex < 0)
				vDex = 0;

			if (vInt < 0)
				vInt = 0;

			var total = vStr + vDex + vInt;

			if (total == 0 || total == vMax)
				return;

			var scalar = vMax / (double)total;

			vStr = (int)(vStr * scalar);
			vDex = (int)(vDex * scalar);
			vInt = (int)(vInt * scalar);

			FixStat(ref vStr, (vStr + vDex + vInt) - vMax, vMax);
			FixStat(ref vDex, (vStr + vDex + vInt) - vMax, vMax);
			FixStat(ref vInt, (vStr + vDex + vInt) - vMax, vMax);

			str = vStr + 10;
			dex = vDex + 10;
			intel = vInt + 10;
		}

		private static void FixStat(ref int stat, int diff, int max)
		{
			stat += diff;

			if (stat < 0)
				stat = 0;
			else if (stat > max)
				stat = max;
		}

		private static void SetStats(Mobile m, NetState state, StatNameValue[] stats, Profession prof)
		{
			var max = 0;

			int str = 0, dex = 0, intel = 0;

			var idx = (int)prof;

			if (idx < 0 || idx >= ProfessionInfo.Professions.Length)
			{
				idx = 0;
				prof = Profession.Advanced;
			}

			if (prof != Profession.Advanced)
			{
				stats = ProfessionInfo.Professions[idx]?.Stats;

				foreach (var snv in stats)
				{
					switch (snv.Name)
					{
						case StatType.Str: str = snv.Value; break;
						case StatType.Dex: dex = snv.Value; break;
						case StatType.Int: intel = snv.Value; break;
					}

					max += snv.Value;
				}
			}
			else
			{
				max = state.NewCharacterCreation ? 90 : 80;

				foreach (var snv in stats)
				{
					switch (snv.Name)
					{
						case StatType.Str: str = snv.Value; break;
						case StatType.Dex: dex = snv.Value; break;
						case StatType.Int: intel = snv.Value; break;
					}
				}

				FixStats(ref str, ref dex, ref intel, max);
			}

			if (str < 10 || str > 60 || dex < 10 || dex > 60 || intel < 10 || intel > 60 || (str + dex + intel) != max)
			{
				str = 10;
				dex = 10;
				intel = 10;
			}

			m.InitStats(str, dex, intel);
		}

		private static bool ValidSkills(SkillNameValue[] skills)
		{
			if (skills == null || skills.Length < 3)
			{
				return false;
			}

			var total = 0;

			for (var i = 0; i < skills.Length; ++i)
			{
				if (skills[i].Value < 0 || skills[i].Value > 50)
				{
					return false;
				}

				total += skills[i].Value;

				for (var j = i + 1; j < skills.Length; ++j)
				{
					if (skills[j].Value > 0 && skills[j].Name == skills[i].Name)
					{
						return false;
					}
				}
			}

			return total <= 150;
		}

		private static void SetSkills(Mobile m, SkillNameValue[] skills, Profession prof)
		{
			var idx = (int)prof;

			if (idx < 0 || idx >= ProfessionInfo.Professions.Length)
			{
				idx = 0;
				prof = Profession.Advanced;
			}

			if (prof != Profession.Advanced)
			{
				skills = ProfessionInfo.Professions[idx]?.Skills;
			}
			else if (!ValidSkills(skills))
			{
				return;
			}

			var addSkillItems = true;
			var elf = m.Race == Race.Elf;
			var human = m.Race == Race.Human;
			var gargoyle = m.Race == Race.Gargoyle;

			switch (prof)
			{
				case Profession.Warrior: // Warrior
					{
						if (elf)
							EquipItem(m, new LeafChest());
						else if (human)
							EquipItem(m, new LeatherChest());
						else if (gargoyle)
							EquipItem(m, new GargishLeatherChest());

						break;
					}
				case Profession.Necromancer: // Necromancer
					{
						Container regs = new BagOfNecroReagents(50);

						if (!Core.AOS)
						{
							foreach (var item in regs.Items)
								item.LootType = LootType.Newbied;
						}

						PackItem(m, regs);

						if (elf || human)
							EquipItem(m, new BoneHelm());

						if (elf)
						{
							EquipItem(m, new ElvenMachete());
							EquipItem(m, NecroHue(new LeafChest()));
							EquipItem(m, NecroHue(new LeafArms()));
							EquipItem(m, NecroHue(new LeafGloves()));
							EquipItem(m, NecroHue(new LeafGorget()));
							EquipItem(m, NecroHue(new LeafGorget()));
							EquipItem(m, NecroHue(new ElvenPants())); //TODO: Verify the pants
							EquipItem(m, new ElvenBoots());
						}
						else if (human)
						{
							EquipItem(m, new BoneHarvester());
							EquipItem(m, NecroHue(new LeatherChest()));
							EquipItem(m, NecroHue(new LeatherArms()));
							EquipItem(m, NecroHue(new LeatherGloves()));
							EquipItem(m, NecroHue(new LeatherGorget()));
							EquipItem(m, NecroHue(new LeatherLegs()));
							EquipItem(m, NecroHue(new Skirt()));
							EquipItem(m, new Sandals(0x8FD));
						}
						else if (gargoyle)
						{
							EquipItem(m, new GlassSword());
							EquipItem(m, NecroHue(new GargishLeatherChest()));
							EquipItem(m, NecroHue(new GargishLeatherArms()));
							EquipItem(m, NecroHue(new GargishLeatherLegs()));
							EquipItem(m, NecroHue(new GargishLeatherKilt()));
						}

						Spellbook book = new NecromancerSpellbook(0x8981UL); // animate dead, evil omen, pain spike, summon familiar, wraith form

						PackItem(m, book);

						book.LootType = LootType.Blessed;

						addSkillItems = false;
						break;
					}
				case Profession.Paladin: // Paladin
					{
						if (elf)
						{
							EquipItem(m, new ElvenMachete());
							EquipItem(m, new WingedHelm());
							EquipItem(m, new LeafGorget());
							EquipItem(m, new LeafArms());
							EquipItem(m, new LeafChest());
							EquipItem(m, new LeafLegs());
							EquipItem(m, new ElvenBoots()); //Verify hue
						}
						else if (human)
						{
							EquipItem(m, new Broadsword());
							EquipItem(m, new Helmet());
							EquipItem(m, new PlateGorget());
							EquipItem(m, new RingmailArms());
							EquipItem(m, new RingmailChest());
							EquipItem(m, new RingmailLegs());
							EquipItem(m, new ThighBoots(0x748));
							EquipItem(m, new Cloak(0xCF));
							EquipItem(m, new BodySash(0xCF));
						}
						else if (gargoyle)
						{
							EquipItem(m, new DreadSword());
							EquipItem(m, new GargishPlateChest());
							EquipItem(m, new GargishPlateArms());
							EquipItem(m, new GargishPlateLegs());
							EquipItem(m, new GargishPlateKilt());
						}

						Spellbook book = new BookOfChivalry(0x3FFUL)
						{
							LootType = LootType.Blessed
						};
						PackItem(m, book);

						addSkillItems = false;
						break;
					}
				case Profession.Samurai: // Samurai
					{
						if (elf || human)
						{
							EquipItem(m, new HakamaShita(0x2C3));
							EquipItem(m, new Hakama(0x2C3));
							EquipItem(m, new SamuraiTabi(0x2C3));
							EquipItem(m, new TattsukeHakama(0x22D));
							EquipItem(m, new Bokuto());

							if (elf)
								EquipItem(m, new RavenHelm());
							else
								EquipItem(m, new LeatherJingasa());
						}
						else if (gargoyle)
						{
							EquipItem(m, new GlassSword());
							EquipItem(m, new GargishPlateChest());
							EquipItem(m, new GargishPlateArms());
							EquipItem(m, new GargishPlateLegs());
							EquipItem(m, new GargishPlateKilt());
						}

						PackItem(m, new Scissors());
						PackItem(m, new Bandage(50));

						Spellbook book = new BookOfBushido();
						PackItem(m, book);

						addSkillItems = false;
						break;
					}
				case Profession.Ninja: // Ninja
					{
						var hues = new[] { 0x1A8, 0xEC, 0x99, 0x90, 0xB5, 0x336, 0x89 };
						//TODO: Verify that's ALL the hues for that above.

						if (elf || human)
						{
							EquipItem(m, new Kasa());
							EquipItem(m, new TattsukeHakama(hues[Utility.Random(hues.Length)]));
							EquipItem(m, new HakamaShita(0x2C3));
							EquipItem(m, new NinjaTabi(0x2C3));

							if (elf)
								EquipItem(m, new AssassinSpike());
							else
								EquipItem(m, new Tekagi());
						}
						else if (gargoyle)
						{
							EquipItem(m, new GargishDagger());

							var hue = hues[Utility.Random(hues.Length)];

							EquipItem(m, new GargishClothChestArmor(hue));
							EquipItem(m, new GargishClothArmsArmor(hue));
							EquipItem(m, new GargishClothLegsArmor(hue));
							EquipItem(m, new GargishClothKiltArmor(hue));
						}

						PackItem(m, new SmokeBomb());

						Spellbook book = new BookOfNinjitsu();
						PackItem(m, book);

						addSkillItems = false;
						break;
					}
			}

			for (var i = 0; i < skills.Length; ++i)
			{
				var snv = skills[i];

				if (snv.Value <= 0 || !Enum.IsDefined(typeof(SkillName), snv.Name))
				{
					continue;
				}

				if (prof == Profession.Advanced && (snv.Name == SkillName.Stealth || snv.Name == SkillName.RemoveTrap || snv.Name == SkillName.Spellweaving))
				{
					continue;
				}

				var skill = m.Skills[snv.Name];

				if (skill != null)
				{
					skill.BaseFixedPoint = snv.Value * 10;

					if (addSkillItems)
					{
						AddSkillItems(skill.SkillName, m);
					}
				}
			}
		}

		private static void EquipItem(Mobile mobile, Item item)
		{
			EquipItem(mobile, item, false);
		}

		private static void EquipItem(Mobile mobile, Item item, bool mustEquip)
		{
			PackItem(mobile, item);

			if (!mobile.EquipItem(item))
			{
				if (mustEquip)
					item.Delete();
			}
		}

		private static void PackItem(Mobile mobile, Item item)
		{
			if (!Core.AOS && item.LootType == LootType.Regular)
				item.LootType = LootType.Newbied;

			var pack = mobile.Backpack;

			if (pack != null)
				pack.DropItem(item);
			else
				item.Delete();
		}

		private static void PackInstrument(Mobile mobile)
		{
			switch (Utility.Random(6))
			{
				case 0:
					PackItem(mobile, new Drums());
					break;
				case 1:
					PackItem(mobile, new Harp());
					break;
				case 2:
					PackItem(mobile, new LapHarp());
					break;
				case 3:
					PackItem(mobile, new Lute());
					break;
				case 4:
					PackItem(mobile, new Tambourine());
					break;
				case 5:
					PackItem(mobile, new TambourineTassel());
					break;
			}
		}

		private static void PackScroll(Mobile mobile, int circle)
		{
			switch (Utility.Random(8) * (circle + 1))
			{
				case 0:
					PackItem(mobile, new ClumsyScroll());
					break;
				case 1:
					PackItem(mobile, new CreateFoodScroll());
					break;
				case 2:
					PackItem(mobile, new FeeblemindScroll());
					break;
				case 3:
					PackItem(mobile, new HealScroll());
					break;
				case 4:
					PackItem(mobile, new MagicArrowScroll());
					break;
				case 5:
					PackItem(mobile, new NightSightScroll());
					break;
				case 6:
					PackItem(mobile, new ReactiveArmorScroll());
					break;
				case 7:
					PackItem(mobile, new WeakenScroll());
					break;
				case 8:
					PackItem(mobile, new AgilityScroll());
					break;
				case 9:
					PackItem(mobile, new CunningScroll());
					break;
				case 10:
					PackItem(mobile, new CureScroll());
					break;
				case 11:
					PackItem(mobile, new HarmScroll());
					break;
				case 12:
					PackItem(mobile, new MagicTrapScroll());
					break;
				case 13:
					PackItem(mobile, new MagicUnTrapScroll());
					break;
				case 14:
					PackItem(mobile, new ProtectionScroll());
					break;
				case 15:
					PackItem(mobile, new StrengthScroll());
					break;
				case 16:
					PackItem(mobile, new BlessScroll());
					break;
				case 17:
					PackItem(mobile, new FireballScroll());
					break;
				case 18:
					PackItem(mobile, new MagicLockScroll());
					break;
				case 19:
					PackItem(mobile, new PoisonScroll());
					break;
				case 20:
					PackItem(mobile, new TelekinisisScroll());
					break;
				case 21:
					PackItem(mobile, new TeleportScroll());
					break;
				case 22:
					PackItem(mobile, new UnlockScroll());
					break;
				case 23:
					PackItem(mobile, new WallOfStoneScroll());
					break;
			}
		}

		private static Item NecroHue(Item item)
		{
			item.Hue = 0x2C3;

			return item;
		}

		private static void AddSkillItems(SkillName skill, Mobile m)
		{
			var elf = m.Race == Race.Elf;
			var human = m.Race == Race.Human;
			var gargoyle = m.Race == Race.Gargoyle;

			switch (skill)
			{
				case SkillName.Alchemy:
					{
						PackItem(m, new Bottle(4));
						PackItem(m, new MortarPestle());

						var hue = Utility.RandomPinkHue();

						if (elf)
						{
							if (m.Female)
								EquipItem(m, new FemaleElvenRobe(hue));
							else
								EquipItem(m, new MaleElvenRobe(hue));
						}
						else
						{
							EquipItem(m, new Robe(Utility.RandomPinkHue()));
						}
						break;
					}
				case SkillName.Anatomy:
					{
						PackItem(m, new Bandage(3));

						var hue = Utility.RandomYellowHue();

						if (elf)
						{
							if (m.Female)
								EquipItem(m, new FemaleElvenRobe(hue));
							else
								EquipItem(m, new MaleElvenRobe(hue));
						}
						else
						{
							EquipItem(m, new Robe(hue));
						}
						break;
					}
				case SkillName.AnimalLore:
					{
						var hue = Utility.RandomBlueHue();

						if (elf)
						{
							EquipItem(m, new WildStaff());

							if (m.Female)
								EquipItem(m, new FemaleElvenRobe(hue));
							else
								EquipItem(m, new MaleElvenRobe(hue));
						}
						else
						{
							EquipItem(m, new ShepherdsCrook());
							EquipItem(m, new Robe(hue));
						}
						break;
					}
				case SkillName.Archery:
					{
						PackItem(m, new Arrow(25));

						if (elf)
							EquipItem(m, new ElvenCompositeLongbow());
						else if (human)
							EquipItem(m, new Bow());

						break;
					}
				case SkillName.ArmsLore:
					{
						if (elf)
						{
							switch (Utility.Random(3))
							{
								case 0:
									EquipItem(m, new Leafblade());
									break;
								case 1:
									EquipItem(m, new RuneBlade());
									break;
								case 2:
									EquipItem(m, new DiamondMace());
									break;
							}
						}
						else if (human)
						{
							switch (Utility.Random(3))
							{
								case 0:
									EquipItem(m, new Kryss());
									break;
								case 1:
									EquipItem(m, new Katana());
									break;
								case 2:
									EquipItem(m, new Club());
									break;
							}
						}
						else if (gargoyle)
						{
							switch (Utility.Random(3))
							{
								case 0:
									EquipItem(m, new BloodBlade());
									break;
								case 1:
									EquipItem(m, new GlassSword());
									break;
								case 2:
									EquipItem(m, new DiscMace());
									break;
							}
						}

						break;
					}
				case SkillName.Begging:
					{
						if (elf)
							EquipItem(m, new WildStaff());
						else if (human)
							EquipItem(m, new GnarledStaff());
						else if (gargoyle)
							EquipItem(m, new SerpentStoneStaff());

						break;
					}
				case SkillName.Blacksmith:
					{
						PackItem(m, new Tongs());
						PackItem(m, new Pickaxe());
						PackItem(m, new Pickaxe());
						PackItem(m, new IronIngot(50));

						if (human || elf)
						{
							EquipItem(m, new HalfApron(Utility.RandomYellowHue()));
						}

						break;
					}
				case SkillName.Bushido:
					{
						if (human || elf)
						{
							EquipItem(m, new Hakama());
							EquipItem(m, new Kasa());
						}

						EquipItem(m, new BookOfBushido());
						break;
					}
				case SkillName.Fletching:
					{
						PackItem(m, new Board(14));
						PackItem(m, new Feather(5));
						PackItem(m, new Shaft(5));
						break;
					}
				case SkillName.Camping:
					{
						PackItem(m, new Bedroll());
						PackItem(m, new Kindling(5));
						break;
					}
				case SkillName.Carpentry:
					{
						PackItem(m, new Board(10));
						PackItem(m, new Saw());

						if (human || elf)
						{
							EquipItem(m, new HalfApron(Utility.RandomYellowHue()));
						}

						break;
					}
				case SkillName.Cartography:
					{
						PackItem(m, new BlankMap());
						PackItem(m, new BlankMap());
						PackItem(m, new BlankMap());
						PackItem(m, new BlankMap());
						PackItem(m, new Sextant());
						break;
					}
				case SkillName.Cooking:
					{
						PackItem(m, new Kindling(2));
						PackItem(m, new RawLambLeg());
						PackItem(m, new RawChickenLeg());
						PackItem(m, new RawFishSteak());
						PackItem(m, new SackFlour());
						PackItem(m, new Pitcher(BeverageType.Water));
						break;
					}
				case SkillName.Chivalry:
					{
						PackItem(m, new BookOfChivalry(0x3FF));
						break;
					}
				case SkillName.DetectHidden:
					{
						if (human || elf)
							EquipItem(m, new Cloak(0x455));

						break;
					}
				case SkillName.Discordance:
					{
						PackInstrument(m);
						break;
					}
				case SkillName.Fencing:
					{
						if (elf)
							EquipItem(m, new Leafblade());
						else if (human)
							EquipItem(m, new Kryss());
						else if (gargoyle)
							EquipItem(m, new BloodBlade());

						break;
					}
				case SkillName.Fishing:
					{
						EquipItem(m, new FishingPole());

						var hue = Utility.RandomYellowHue();

						if (elf)
						{
							Item i = new Circlet
							{
								Hue = hue
							};
							EquipItem(m, i);
						}
						else if (human)
						{
							EquipItem(m, new FloppyHat(hue));
						}

						break;
					}
				case SkillName.Healing:
					{
						PackItem(m, new Bandage(50));
						PackItem(m, new Scissors());
						break;
					}
				case SkillName.Herding:
					{
						if (elf)
							EquipItem(m, new WildStaff());
						else
							EquipItem(m, new ShepherdsCrook());

						break;
					}
				case SkillName.Hiding:
					{
						if (human || elf)
							EquipItem(m, new Cloak(0x455));

						break;
					}
				case SkillName.Inscribe:
					{
						PackItem(m, new BlankScroll(2));
						PackItem(m, new BlueBook());
						break;
					}
				case SkillName.ItemID:
					{
						if (elf)
							EquipItem(m, new WildStaff());
						else if (human)
							EquipItem(m, new GnarledStaff());
						else if (gargoyle)
							EquipItem(m, new SerpentStoneStaff());

						break;
					}
				case SkillName.Lockpicking:
					{
						PackItem(m, new Lockpick(20));
						break;
					}
				case SkillName.Lumberjacking:
					{
						if (human || elf)
							EquipItem(m, new Hatchet());
						else if (gargoyle)
							EquipItem(m, new DualShortAxes());

						break;
					}
				case SkillName.Macing:
					{
						if (elf)
							EquipItem(m, new DiamondMace());
						else if (human)
							EquipItem(m, new Club());
						else if (gargoyle)
							EquipItem(m, new DiscMace());

						break;
					}
				case SkillName.Magery:
					{
						var regs = new BagOfReagents(50);
						PackItem(m, regs);

						PackScroll(m, 0);
						PackScroll(m, 1);
						PackScroll(m, 2);

						var book = new Spellbook(0x382A8C38)
						{
							LootType = LootType.Blessed
						};
						EquipItem(m, book);

						if (elf)
						{
							EquipItem(m, new Circlet());

							if (m.Female)
								EquipItem(m, new FemaleElvenRobe(Utility.RandomBlueHue()));
							else
								EquipItem(m, new MaleElvenRobe(Utility.RandomBlueHue()));
						}
						else
						{
							if (human)
								EquipItem(m, new WizardsHat());

							EquipItem(m, new Robe(Utility.RandomBlueHue()));
						}

						break;
					}
				case SkillName.Mining:
					{
						PackItem(m, new Pickaxe());
						break;
					}
				case SkillName.Musicianship:
					{
						PackInstrument(m);
						break;
					}
				case SkillName.Necromancy:
					{
						Container regs = new BagOfNecroReagents(50);

						if (!Core.AOS)
						{
							foreach (var item in regs.Items)
								item.LootType = LootType.Newbied;
						}

						PackItem(m, regs);

						regs.LootType = LootType.Regular;

						// RunUO fix
						Spellbook book = new NecromancerSpellbook(0x8981UL)
						{
							LootType = LootType.Blessed
						}; // animate dead, evil omen, pain spike, summon familiar, wraith form
						PackItem(m, book);

						break;
					}
				case SkillName.Ninjitsu:
					{
						if (human || elf)
						{
							EquipItem(m, new Hakama(0x2C3)); //Only ninjas get the hued one.
							EquipItem(m, new Kasa());
						}

						EquipItem(m, new BookOfNinjitsu());
						break;
					}
				case SkillName.Parry:
					{
						if (human || elf)
							EquipItem(m, new WoodenShield());
						else if (gargoyle)
							EquipItem(m, new GargishWoodenShield());

						break;
					}
				case SkillName.Peacemaking:
					{
						PackInstrument(m);
						break;
					}
				case SkillName.Poisoning:
					{
						PackItem(m, new LesserPoisonPotion());
						PackItem(m, new LesserPoisonPotion());
						break;
					}
				case SkillName.Provocation:
					{
						PackInstrument(m);
						break;
					}
				case SkillName.Snooping:
					{
						PackItem(m, new Lockpick(20));
						break;
					}
				case SkillName.SpiritSpeak:
					{
						if (human || elf)
						{
							EquipItem(m, new Cloak(0x455));
						}

						break;
					}
				case SkillName.Stealing:
					{
						PackItem(m, new Lockpick(20));
						break;
					}
				case SkillName.Swords:
					{
						if (elf)
							EquipItem(m, new RuneBlade());
						else if (human)
							EquipItem(m, new Katana());
						else if (gargoyle)
							EquipItem(m, new GlassSword());

						break;
					}
				case SkillName.Tactics:
					{
						if (elf)
							EquipItem(m, new RuneBlade());
						else if (human)
							EquipItem(m, new Katana());
						else if (gargoyle)
							EquipItem(m, new GlassSword());

						break;
					}
				case SkillName.Tailoring:
					{
						PackItem(m, new BoltOfCloth());
						PackItem(m, new SewingKit());
						break;
					}
				case SkillName.Tinkering:
					{
						PackItem(m, new TinkerTools());
						PackItem(m, new IronIngot(50));
						PackItem(m, new Axle());
						PackItem(m, new AxleGears());
						PackItem(m, new Springs());
						PackItem(m, new ClockFrame());
						break;
					}
				case SkillName.Tracking:
					{
						if (human || elf)
						{
							var shoes = m.FindItemOnLayer(Layer.Shoes);

							if (shoes != null)
								shoes.Delete();

							var hue = Utility.RandomYellowHue();

							if (elf)
								EquipItem(m, new ElvenBoots(hue));
							else
								EquipItem(m, new Boots(hue));

							EquipItem(m, new SkinningKnife());
						}
						else if (gargoyle)
							PackItem(m, new SkinningKnife());

						break;
					}
				case SkillName.Veterinary:
					{
						PackItem(m, new Bandage(5));
						PackItem(m, new Scissors());
						break;
					}
				case SkillName.Wrestling:
					{
						if (elf)
							EquipItem(m, new LeafGloves());
						else if (human)
							EquipItem(m, new LeatherGloves());
						else if (gargoyle)
						{
							// Why not give them arm armor?
							EquipItem(m, new GargishLeatherArms());
						}

						break;
					}
				case SkillName.Throwing:
					{
						if (gargoyle)
							EquipItem(m, new Boomerang());

						break;
					}
				case SkillName.Mysticism:
					{
						PackItem(m, new MysticBook(0xABUL));
						break;
					}
			}
		}

		private class BadStartMessage : Timer
		{
			private readonly Mobile m_Mobile;
			private readonly int m_Message;

			public BadStartMessage(Mobile m, int message)
				: base(TimeSpan.FromSeconds(3.5))
			{
				m_Mobile = m;
				m_Message = message;
				Start();
			}

			protected override void OnTick()
			{
				m_Mobile.SendLocalizedMessage(m_Message);
			}
		}
	}
}
