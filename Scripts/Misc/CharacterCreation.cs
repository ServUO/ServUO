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
		private static readonly CityInfo m_NewHavenInfo = new CityInfo(
			"New Haven",
			"The Bountiful Harvest Inn",
			3503,
			2574,
			14,
			Map.Trammel);

		private static readonly CityInfo m_SiegeInfo = new CityInfo(
			"Britain",
			"The Wayfarer's Inn",
			1075074,
			1602,
			1591,
			20,
			Map.Felucca);

		private static Mobile m_Mobile;

		public static void Initialize()
		{
			// Register our event handler
			EventSink.CharacterCreated += EventSink_CharacterCreated;
		}

		public static bool VerifyProfession(int profession)
		{
			if (profession < 0)
				return false;
			if (profession < 4)
				return true;
			if (Core.AOS && profession < 6)
				return true;
			if (Core.SE && profession < 8)
				return true;
			return false;
		}

		private static void AddBackpack(Mobile m)
		{
			var pack = m.Backpack;

			if (pack == null)
			{
				pack = new Backpack();
				pack.Movable = false;

				m.AddItem(pack);
			}

			PackItem(new RedBook("a book", m.Name, 20, true));
			PackItem(new Gold(1000)); // Starting gold can be customized here
			PackItem(new Candle());

			if (m.Race != Race.Gargoyle)
				PackItem(new Dagger());
			else
				PackItem(new GargishDagger());
		}

		private static void AddShirt(Mobile m, int shirtHue)
		{
			var hue = Utility.ClipDyedHue(shirtHue & 0x3FFF);

			if (m.Race == Race.Elf)
			{
				EquipItem(new ElvenShirt(hue), true);
			}
			else if (m.Race == Race.Human)
			{
				switch (Utility.Random(3))
				{
					case 0:
						EquipItem(new Shirt(hue), true);
						break;
					case 1:
						EquipItem(new FancyShirt(hue), true);
						break;
					case 2:
						EquipItem(new Doublet(hue), true);
						break;
				}
			}
			else if (m.Race == Race.Gargoyle)
			{
				EquipItem(new GargishClothChestArmor(hue));
			}
		}

		private static void AddPants(Mobile m, int pantsHue)
		{
			var hue = Utility.ClipDyedHue(pantsHue & 0x3FFF);

			if (m.Race == Race.Elf)
			{
				EquipItem(new ElvenPants(hue), true);
			}
			else if (m.Race == Race.Human)
			{
				if (m.Female)
				{
					switch (Utility.Random(2))
					{
						case 0:
							EquipItem(new Skirt(hue), true);
							break;
						case 1:
							EquipItem(new Kilt(hue), true);
							break;
					}
				}
				else
				{
					switch (Utility.Random(2))
					{
						case 0:
							EquipItem(new LongPants(hue), true);
							break;
						case 1:
							EquipItem(new ShortPants(hue), true);
							break;
					}
				}
			}
			else if (m.Race == Race.Gargoyle)
			{
				EquipItem(new GargishClothKiltArmor(hue));
			}
		}

		private static void AddShoes(Mobile m)
		{
			if (m.Race == Race.Elf)
				EquipItem(new ElvenBoots(), true);
			else if (m.Race == Race.Human)
				EquipItem(new Shoes(Utility.RandomYellowHue()), true);
		}

		private static Mobile CreateMobile(Account a)
		{
			if (a.Count >= a.Limit)
				return null;

			for (var i = 0; i < a.Length; ++i)
			{
				if (a[i] == null)
					return (a[i] = new PlayerMobile());
			}

			return null;
		}

		private static void EventSink_CharacterCreated(CharacterCreatedEventArgs args)
		{
			if (!VerifyProfession(args.Profession))
				args.Profession = 0;

			var state = args.State;

			if (state == null)
				return;

			var newChar = CreateMobile(args.Account as Account);

			if (newChar == null)
			{
				Utility.PushColor(ConsoleColor.Red);
				Console.WriteLine("Login: {0}: Character creation failed, account full", state);
				Utility.PopColor();
				return;
			}

			args.Mobile = newChar;
			m_Mobile = newChar;

			newChar.Player = true;
			newChar.AccessLevel = args.Account.AccessLevel;
			newChar.Female = args.Female;
			//newChar.Body = newChar.Female ? 0x191 : 0x190;

			if (Core.Expansion >= args.Race.RequiredExpansion)
				newChar.Race = args.Race; //Sets body
			else
				newChar.Race = Race.DefaultRace;

			newChar.Hue = args.Hue | 0x8000;

			newChar.Hunger = 20;

			var young = false;

			if (newChar is PlayerMobile)
			{
				var pm = (PlayerMobile)newChar;
				
				pm.AutoRenewInsurance = true;

				var skillcap = Config.Get("PlayerCaps.SkillCap", 1000.0d) / 10;
				
				if (skillcap != 100.0)
				{
					for (var i = 0; i < Enum.GetNames(typeof(SkillName)).Length; ++i)
						pm.Skills[i].Cap = skillcap;
				}
				
				pm.Profession = args.Profession;

				if (pm.IsPlayer() && pm.Account.Young && !Siege.SiegeShard)
					young = pm.Young = true;
			}

			SetName(newChar, args.Name);

			AddBackpack(newChar);

            SetStats(newChar, state, args.Profession, args.Str, args.Dex, args.Int);
			SetSkills(newChar, args.Skills, args.Profession);

			var race = newChar.Race;

			if (race.ValidateHair(newChar, args.HairID))
			{
				newChar.HairItemID = args.HairID;
				newChar.HairHue = args.HairHue;
			}

			if (race.ValidateFacialHair(newChar, args.BeardID))
			{
				newChar.FacialHairItemID = args.BeardID;
				newChar.FacialHairHue = args.BeardHue;
			}

			var faceID = args.FaceID;

			if (faceID > 0 && race.ValidateFace(newChar.Female, faceID))
			{
				newChar.FaceItemID = faceID;
				newChar.FaceHue = args.FaceHue;
			}
			else
			{
				newChar.FaceItemID = race.RandomFace(newChar.Female);
				newChar.FaceHue = newChar.Hue;
			}

			if (args.Profession <= 3)
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

			Utility.PushColor(ConsoleColor.Green);
			Console.WriteLine("Login: {0}: New character being created (account={1})", state, args.Account.Username);
			Utility.PopColor();
			Utility.PushColor(ConsoleColor.DarkGreen);
			Console.WriteLine(" - Character: {0} (serial={1})", newChar.Name, newChar.Serial);
			Console.WriteLine(" - Started: {0} {1} in {2}", city.City, city.Location, city.Map);
			Utility.PopColor();

			new WelcomeTimer(newChar).Start();
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

		private static void SetStats(Mobile m, NetState state, int str, int dex, int intel)
		{
			var max = state.NewCharacterCreation ? 90 : 80;

			FixStats(ref str, ref dex, ref intel, max);

			if (str < 10 || str > 60 || dex < 10 || dex > 60 || intel < 10 || intel > 60 || (str + dex + intel) != max)
			{
				str = 10;
				dex = 10;
				intel = 10;
			}

			m.InitStats(str, dex, intel);
		}

		private static void SetName(Mobile m, string name)
		{
			name = name.Trim();

			if (!NameVerification.Validate(name, 2, 16, true, false, true, 1, NameVerification.SpaceDashPeriodQuote))
				name = "Generic Player";

			m.Name = name;
		}

		private static bool ValidSkills(SkillNameValue[] skills)
		{
			var total = 0;

			for (var i = 0; i < skills.Length; ++i)
			{
				if (skills[i].Value < 0 || skills[i].Value > 50)
					return false;

				total += skills[i].Value;

				for (var j = i + 1; j < skills.Length; ++j)
				{
					if (skills[j].Value > 0 && skills[j].Name == skills[i].Name)
						return false;
				}
			}

			return (total == 100 || total == 120);
		}

        private static void SetStats(Mobile m, NetState state, int prof, int str, int dex, int intel)
        {
            switch (prof)
            {
                case 1: // Warrior
                    {
                        str = 45;
                        dex = 35;
                        intel = 10;
                        break;
                    }
                case 2: // Magician
                    {
                        str = 25;
                        dex = 20;
                        intel = 45;
                        break;
                    }
                case 3: // Blacksmith
                    {
                        str = 60;
                        dex = 15;
                        intel = 15;
                        break;
                    }
                case 4: // Necromancer
                    {
                        str = 25;
                        dex = 20;
                        intel = 45;
                        break;
                    }
                case 5: // Paladin
                    {
                        str = 45;
                        dex = 20;
                        intel = 25;
                        break;
                    }
                case 6: //Samurai
                    {
                        str = 40;
                        dex = 30;
                        intel = 20;
                        break;
                    }
                case 7: //Ninja
                    {
                        str = 40;
                        dex = 30;
                        intel = 20;
                        break;
                    }
                default:
                    {
                        SetStats(m, state, str, dex, intel);

                        return;
                    }
            }

            m.InitStats(str, dex, intel);
        }

		private static void SetSkills(Mobile m, SkillNameValue[] skills, int prof)
		{
			switch (prof)
			{
				case 1: // Warrior
				{
					skills = new[]
					{
						new SkillNameValue(SkillName.Anatomy, 30), new SkillNameValue(SkillName.Healing, 30),
						new SkillNameValue(SkillName.Swords, 30), new SkillNameValue(SkillName.Tactics, 30)
					};

					break;
				}
				case 2: // Magician
				{
					skills = new[]
					{
						new SkillNameValue(SkillName.EvalInt, 30), new SkillNameValue(SkillName.Wrestling, 30),
						new SkillNameValue(SkillName.Magery, 30), new SkillNameValue(SkillName.Meditation, 30)
					};

					break;
				}
				case 3: // Blacksmith
				{
					skills = new[]
					{
						new SkillNameValue(SkillName.Mining, 30), new SkillNameValue(SkillName.ArmsLore, 30),
						new SkillNameValue(SkillName.Blacksmith, 30), new SkillNameValue(SkillName.Tinkering, 30)
					};

					break;
				}
				case 4: // Necromancer
				{
					skills = new[]
					{
						new SkillNameValue(SkillName.Necromancy, 30),
						new SkillNameValue(SkillName.SpiritSpeak, 30), new SkillNameValue(SkillName.Swords, 30),
						new SkillNameValue(SkillName.Meditation, 20)
					};

					break;
				}
				case 5: // Paladin
				{
					skills = new[]
					{
						new SkillNameValue(SkillName.Chivalry, 30), new SkillNameValue(SkillName.Swords, 30),
						new SkillNameValue(SkillName.Focus, 30), new SkillNameValue(SkillName.Tactics, 30)
					};

					break;
				}
				case 6: //Samurai
				{
					skills = new[]
					{
						new SkillNameValue(SkillName.Bushido, 30), new SkillNameValue(SkillName.Swords, 30),
						new SkillNameValue(SkillName.Anatomy, 30), new SkillNameValue(SkillName.Healing, 30)
					};
					break;
				}
				case 7: //Ninja
				{
					skills = new[]
					{
						new SkillNameValue(SkillName.Ninjitsu, 30), new SkillNameValue(SkillName.Hiding, 30),
						new SkillNameValue(SkillName.Fencing, 30), new SkillNameValue(SkillName.Stealth, 30)
					};
					break;
				}
				default:
				{
					if (!ValidSkills(skills))
						return;

					break;
				}
			}

			var addSkillItems = true;
			var elf = (m.Race == Race.Elf);
			var human = (m.Race == Race.Human);
			var gargoyle = (m.Race == Race.Gargoyle);

			switch (prof)
			{
				case 1: // Warrior
				{
					if (elf)
						EquipItem(new LeafChest());
					else if (human)
						EquipItem(new LeatherChest());
					else if (gargoyle)
					{
						EquipItem(new GargishLeatherChest());
					}

					break;
				}
				case 4: // Necromancer
				{
					Container regs = new BagOfNecroReagents(50);

					if (!Core.AOS)
					{
						foreach (var item in regs.Items)
							item.LootType = LootType.Newbied;
					}

					PackItem(regs);

					regs.LootType = LootType.Regular;

					if (elf || human)
						EquipItem(new BoneHelm());

					if (elf)
					{
						EquipItem(new ElvenMachete());
						EquipItem(NecroHue(new LeafChest()));
						EquipItem(NecroHue(new LeafArms()));
						EquipItem(NecroHue(new LeafGloves()));
						EquipItem(NecroHue(new LeafGorget()));
						EquipItem(NecroHue(new LeafGorget()));
						EquipItem(NecroHue(new ElvenPants())); //TODO: Verify the pants
						EquipItem(new ElvenBoots());
					}
					else if (human)
					{
						EquipItem(new BoneHarvester());
						EquipItem(NecroHue(new LeatherChest()));
						EquipItem(NecroHue(new LeatherArms()));
						EquipItem(NecroHue(new LeatherGloves()));
						EquipItem(NecroHue(new LeatherGorget()));
						EquipItem(NecroHue(new LeatherLegs()));
						EquipItem(NecroHue(new Skirt()));
						EquipItem(new Sandals(0x8FD));
					}
					else if (gargoyle)
					{
						EquipItem(new GlassSword());
						EquipItem(NecroHue(new GargishLeatherChest()));
						EquipItem(NecroHue(new GargishLeatherArms()));
						EquipItem(NecroHue(new GargishLeatherLegs()));
						EquipItem(NecroHue(new GargishLeatherKilt()));
					}

					Spellbook
						book = new NecromancerSpellbook(
							(ulong)0x8981); // animate dead, evil omen, pain spike, summon familiar, wraith form

					PackItem(book);

					book.LootType = LootType.Blessed;

					addSkillItems = false;
					break;
				}
				case 5: // Paladin
				{
					if (elf)
					{
						EquipItem(new ElvenMachete());
						EquipItem(new WingedHelm());
						EquipItem(new LeafGorget());
						EquipItem(new LeafArms());
						EquipItem(new LeafChest());
						EquipItem(new LeafLegs());
						EquipItem(new ElvenBoots()); //Verify hue
					}
					else if (human)
					{
						EquipItem(new Broadsword());
						EquipItem(new Helmet());
						EquipItem(new PlateGorget());
						EquipItem(new RingmailArms());
						EquipItem(new RingmailChest());
						EquipItem(new RingmailLegs());
						EquipItem(new ThighBoots(0x748));
						EquipItem(new Cloak(0xCF));
						EquipItem(new BodySash(0xCF));
					}
					else if (gargoyle)
					{
						EquipItem(new DreadSword());
						EquipItem(new GargishPlateChest());
						EquipItem(new GargishPlateArms());
						EquipItem(new GargishPlateLegs());
						EquipItem(new GargishPlateKilt());
					}

					Spellbook book = new BookOfChivalry((ulong)0x3FF);
					book.LootType = LootType.Blessed;
					PackItem(book);

					addSkillItems = false;
					break;
				}

				case 6: // Samurai
				{
					if (elf || human)
					{
						EquipItem(new HakamaShita(0x2C3));
						EquipItem(new Hakama(0x2C3));
						EquipItem(new SamuraiTabi(0x2C3));
						EquipItem(new TattsukeHakama(0x22D));
						EquipItem(new Bokuto());

						if (elf)
							EquipItem(new RavenHelm());
						else
							EquipItem(new LeatherJingasa());
					}
					else if (gargoyle)
					{
						EquipItem(new GlassSword());
						EquipItem(new GargishPlateChest());
						EquipItem(new GargishPlateArms());
						EquipItem(new GargishPlateLegs());
						EquipItem(new GargishPlateKilt());
					}

					PackItem(new Scissors());
					PackItem(new Bandage(50));

					Spellbook book = new BookOfBushido();
					PackItem(book);

					addSkillItems = false;
					break;
				}
				case 7: // Ninja
				{
					var hues = new[] {0x1A8, 0xEC, 0x99, 0x90, 0xB5, 0x336, 0x89};
					//TODO: Verify that's ALL the hues for that above.

					if (elf || human)
					{
						EquipItem(new Kasa());
						EquipItem(new TattsukeHakama(hues[Utility.Random(hues.Length)]));
						EquipItem(new HakamaShita(0x2C3));
						EquipItem(new NinjaTabi(0x2C3));

						if (elf)
							EquipItem(new AssassinSpike());
						else
							EquipItem(new Tekagi());
					}
					else if (gargoyle)
					{
						EquipItem(new GargishDagger());

						var hue = hues[Utility.Random(hues.Length)];

						EquipItem(new GargishClothChestArmor(hue));
						EquipItem(new GargishClothArmsArmor(hue));
						EquipItem(new GargishClothLegsArmor(hue));
						EquipItem(new GargishClothKiltArmor(hue));
					}

					PackItem(new SmokeBomb());

					Spellbook book = new BookOfNinjitsu();
					PackItem(book);

					addSkillItems = false;
					break;
				}
			}

			for (var i = 0; i < skills.Length; ++i)
			{
				var snv = skills[i];

				if (snv.Value > 0 && (snv.Name != SkillName.Stealth || prof == 7) && snv.Name != SkillName.RemoveTrap &&
					snv.Name != SkillName.Spellweaving)
				{
					var skill = m.Skills[snv.Name];

					if (skill != null)
					{
						skill.BaseFixedPoint = snv.Value * 10;

						if (addSkillItems)
							AddSkillItems(snv.Name, m);
					}
				}
			}
		}

		private static void EquipItem(Item item)
		{
			EquipItem(item, false);
		}

		private static void EquipItem(Item item, bool mustEquip)
		{
			if (!Core.AOS)
				item.LootType = LootType.Newbied;

			if (m_Mobile != null && m_Mobile.EquipItem(item))
				return;

			var pack = m_Mobile.Backpack;

			if (!mustEquip && pack != null)
				pack.DropItem(item);
			else
				item.Delete();
		}

		private static void PackItem(Item item)
		{
			if (!Core.AOS)
				item.LootType = LootType.Newbied;

			var pack = m_Mobile.Backpack;

			if (pack != null)
				pack.DropItem(item);
			else
				item.Delete();
		}

		private static void PackInstrument()
		{
			switch (Utility.Random(6))
			{
				case 0:
					PackItem(new Drums());
					break;
				case 1:
					PackItem(new Harp());
					break;
				case 2:
					PackItem(new LapHarp());
					break;
				case 3:
					PackItem(new Lute());
					break;
				case 4:
					PackItem(new Tambourine());
					break;
				case 5:
					PackItem(new TambourineTassel());
					break;
			}
		}

		private static void PackScroll(int circle)
		{
			switch (Utility.Random(8) * (circle + 1))
			{
				case 0:
					PackItem(new ClumsyScroll());
					break;
				case 1:
					PackItem(new CreateFoodScroll());
					break;
				case 2:
					PackItem(new FeeblemindScroll());
					break;
				case 3:
					PackItem(new HealScroll());
					break;
				case 4:
					PackItem(new MagicArrowScroll());
					break;
				case 5:
					PackItem(new NightSightScroll());
					break;
				case 6:
					PackItem(new ReactiveArmorScroll());
					break;
				case 7:
					PackItem(new WeakenScroll());
					break;
				case 8:
					PackItem(new AgilityScroll());
					break;
				case 9:
					PackItem(new CunningScroll());
					break;
				case 10:
					PackItem(new CureScroll());
					break;
				case 11:
					PackItem(new HarmScroll());
					break;
				case 12:
					PackItem(new MagicTrapScroll());
					break;
				case 13:
					PackItem(new MagicUnTrapScroll());
					break;
				case 14:
					PackItem(new ProtectionScroll());
					break;
				case 15:
					PackItem(new StrengthScroll());
					break;
				case 16:
					PackItem(new BlessScroll());
					break;
				case 17:
					PackItem(new FireballScroll());
					break;
				case 18:
					PackItem(new MagicLockScroll());
					break;
				case 19:
					PackItem(new PoisonScroll());
					break;
				case 20:
					PackItem(new TelekinisisScroll());
					break;
				case 21:
					PackItem(new TeleportScroll());
					break;
				case 22:
					PackItem(new UnlockScroll());
					break;
				case 23:
					PackItem(new WallOfStoneScroll());
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
			var elf = (m.Race == Race.Elf);
			var human = (m.Race == Race.Human);
			var gargoyle = (m.Race == Race.Gargoyle);

			switch (skill)
			{
				case SkillName.Alchemy:
				{
					PackItem(new Bottle(4));
					PackItem(new MortarPestle());

					var hue = Utility.RandomPinkHue();

					if (elf)
					{
						if (m.Female)
							EquipItem(new FemaleElvenRobe(hue));
						else
							EquipItem(new MaleElvenRobe(hue));
					}
					else
					{
						EquipItem(new Robe(Utility.RandomPinkHue()));
					}
					break;
				}
				case SkillName.Anatomy:
				{
					PackItem(new Bandage(3));

					var hue = Utility.RandomYellowHue();

					if (elf)
					{
						if (m.Female)
							EquipItem(new FemaleElvenRobe(hue));
						else
							EquipItem(new MaleElvenRobe(hue));
					}
					else
					{
						EquipItem(new Robe(hue));
					}
					break;
				}
				case SkillName.AnimalLore:
				{
					var hue = Utility.RandomBlueHue();

					if (elf)
					{
						EquipItem(new WildStaff());

						if (m.Female)
							EquipItem(new FemaleElvenRobe(hue));
						else
							EquipItem(new MaleElvenRobe(hue));
					}
					else
					{
						EquipItem(new ShepherdsCrook());
						EquipItem(new Robe(hue));
					}
					break;
				}
				case SkillName.Archery:
				{
					PackItem(new Arrow(25));

					if (elf)
						EquipItem(new ElvenCompositeLongbow());
					else if (human)
						EquipItem(new Bow());

					break;
				}
				case SkillName.ArmsLore:
				{
					if (elf)
					{
						switch (Utility.Random(3))
						{
							case 0:
								EquipItem(new Leafblade());
								break;
							case 1:
								EquipItem(new RuneBlade());
								break;
							case 2:
								EquipItem(new DiamondMace());
								break;
						}
					}
					else if (human)
					{
						switch (Utility.Random(3))
						{
							case 0:
								EquipItem(new Kryss());
								break;
							case 1:
								EquipItem(new Katana());
								break;
							case 2:
								EquipItem(new Club());
								break;
						}
					}
					else if (gargoyle)
					{
						switch (Utility.Random(3))
						{
							case 0:
								EquipItem(new BloodBlade());
								break;
							case 1:
								EquipItem(new GlassSword());
								break;
							case 2:
								EquipItem(new DiscMace());
								break;
						}
					}

					break;
				}
				case SkillName.Begging:
				{
					if (elf)
						EquipItem(new WildStaff());
					else if (human)
						EquipItem(new GnarledStaff());
					else if (gargoyle)
						EquipItem(new SerpentStoneStaff());

					break;
				}
				case SkillName.Blacksmith:
				{
					PackItem(new Tongs());
					PackItem(new Pickaxe());
					PackItem(new Pickaxe());
					PackItem(new IronIngot(50));

					if (human || elf)
					{
						EquipItem(new HalfApron(Utility.RandomYellowHue()));
					}

					break;
				}
				case SkillName.Bushido:
				{
					if (human || elf)
					{
						EquipItem(new Hakama());
						EquipItem(new Kasa());
					}

					EquipItem(new BookOfBushido());
					break;
				}
				case SkillName.Fletching:
				{
					PackItem(new Board(14));
					PackItem(new Feather(5));
					PackItem(new Shaft(5));
					break;
				}
				case SkillName.Camping:
				{
					PackItem(new Bedroll());
					PackItem(new Kindling(5));
					break;
				}
				case SkillName.Carpentry:
				{
					PackItem(new Board(10));
					PackItem(new Saw());

					if (human || elf)
					{
						EquipItem(new HalfApron(Utility.RandomYellowHue()));
					}

					break;
				}
				case SkillName.Cartography:
				{
					PackItem(new BlankMap());
					PackItem(new BlankMap());
					PackItem(new BlankMap());
					PackItem(new BlankMap());
					PackItem(new Sextant());
					break;
				}
				case SkillName.Cooking:
				{
					PackItem(new Kindling(2));
					PackItem(new RawLambLeg());
					PackItem(new RawChickenLeg());
					PackItem(new RawFishSteak());
					PackItem(new SackFlour());
					PackItem(new Pitcher(BeverageType.Water));
					break;
				}
				case SkillName.Chivalry:
				{
					if (Core.ML)
						PackItem(new BookOfChivalry((ulong)0x3FF));

					break;
				}
				case SkillName.DetectHidden:
				{
					if (human || elf)
						EquipItem(new Cloak(0x455));

					break;
				}
				case SkillName.Discordance:
				{
					PackInstrument();
					break;
				}
				case SkillName.Fencing:
				{
					if (elf)
						EquipItem(new Leafblade());
					else if (human)
						EquipItem(new Kryss());
					else if (gargoyle)
						EquipItem(new BloodBlade());

					break;
				}
				case SkillName.Fishing:
				{
					EquipItem(new FishingPole());

					var hue = Utility.RandomYellowHue();

					if (elf)
					{
						Item i = new Circlet();
						i.Hue = hue;
						EquipItem(i);
					}
					else if (human)
					{
						EquipItem(new FloppyHat(hue));
					}

					break;
				}
				case SkillName.Healing:
				{
					PackItem(new Bandage(50));
					PackItem(new Scissors());
					break;
				}
				case SkillName.Herding:
				{
					if (elf)
						EquipItem(new WildStaff());
					else
						EquipItem(new ShepherdsCrook());

					break;
				}
				case SkillName.Hiding:
				{
					if (human || elf)
						EquipItem(new Cloak(0x455));

					break;
				}
				case SkillName.Inscribe:
				{
					PackItem(new BlankScroll(2));
					PackItem(new BlueBook());
					break;
				}
				case SkillName.ItemID:
				{
					if (elf)
						EquipItem(new WildStaff());
					else if (human)
						EquipItem(new GnarledStaff());
					else if (gargoyle)
						EquipItem(new SerpentStoneStaff());

					break;
				}
				case SkillName.Lockpicking:
				{
					PackItem(new Lockpick(20));
					break;
				}
				case SkillName.Lumberjacking:
				{
					if (human || elf)
						EquipItem(new Hatchet());
					else if (gargoyle)
						EquipItem(new DualShortAxes());

					break;
				}
				case SkillName.Macing:
				{
					if (elf)
						EquipItem(new DiamondMace());
					else if (human)
						EquipItem(new Club());
					else if (gargoyle)
						EquipItem(new DiscMace());

					break;
				}
				case SkillName.Magery:
				{
					var regs = new BagOfReagents(50);

					if (!Core.AOS)
					{
						foreach (var item in regs.Items)
							item.LootType = LootType.Newbied;
					}

					PackItem(regs);

					regs.LootType = LootType.Regular;

					PackScroll(0);
					PackScroll(1);
					PackScroll(2);

					var book = new Spellbook((ulong)0x382A8C38);
					book.LootType = LootType.Blessed;
					EquipItem(book);

					if (elf)
					{
						EquipItem(new Circlet());

						if (m.Female)
							EquipItem(new FemaleElvenRobe(Utility.RandomBlueHue()));
						else
							EquipItem(new MaleElvenRobe(Utility.RandomBlueHue()));
					}
					else
					{
						if (human)
							EquipItem(new WizardsHat());

						EquipItem(new Robe(Utility.RandomBlueHue()));
					}

					break;
				}
				case SkillName.Mining:
				{
					PackItem(new Pickaxe());
					break;
				}
				case SkillName.Musicianship:
				{
					PackInstrument();
					break;
				}
				case SkillName.Necromancy:
				{
					if (Core.ML)
					{
						Container regs = new BagOfNecroReagents(50);

						PackItem(regs);

						regs.LootType = LootType.Regular;
					}

					// RunUO fix
					Spellbook
						book = new NecromancerSpellbook(
							(ulong)0x8981); // animate dead, evil omen, pain spike, summon familiar, wraith form
					book.LootType = LootType.Blessed;
					PackItem(book);

					break;
				}
				case SkillName.Ninjitsu:
				{
					if (human || elf)
					{
						EquipItem(new Hakama(0x2C3)); //Only ninjas get the hued one.
						EquipItem(new Kasa());
					}

					EquipItem(new BookOfNinjitsu());
					break;
				}
				case SkillName.Parry:
				{
					if (human || elf)
						EquipItem(new WoodenShield());
					else if (gargoyle)
						EquipItem(new GargishWoodenShield());

					break;
				}
				case SkillName.Peacemaking:
				{
					PackInstrument();
					break;
				}
				case SkillName.Poisoning:
				{
					PackItem(new LesserPoisonPotion());
					PackItem(new LesserPoisonPotion());
					break;
				}
				case SkillName.Provocation:
				{
					PackInstrument();
					break;
				}
				case SkillName.Snooping:
				{
					PackItem(new Lockpick(20));
					break;
				}
				case SkillName.SpiritSpeak:
				{
					if (human || elf)
					{
						EquipItem(new Cloak(0x455));
					}

					break;
				}
				case SkillName.Stealing:
				{
					PackItem(new Lockpick(20));
					break;
				}
				case SkillName.Swords:
				{
					if (elf)
						EquipItem(new RuneBlade());
					else if (human)
						EquipItem(new Katana());
					else if (gargoyle)
						EquipItem(new GlassSword());

					break;
				}
				case SkillName.Tactics:
				{
					if (elf)
						EquipItem(new RuneBlade());
					else if (human)
						EquipItem(new Katana());
					else if (gargoyle)
						EquipItem(new GlassSword());

					break;
				}
				case SkillName.Tailoring:
				{
					PackItem(new BoltOfCloth());
					PackItem(new SewingKit());
					break;
				}
				case SkillName.Tinkering:
				{
					PackItem(new TinkerTools());
					PackItem(new IronIngot(50));
					PackItem(new Axle());
					PackItem(new AxleGears());
					PackItem(new Springs());
					PackItem(new ClockFrame());
					break;
				}
				case SkillName.Tracking:
				{
					if (human || elf)
					{
						if (m_Mobile != null)
						{
							var shoes = m_Mobile.FindItemOnLayer(Layer.Shoes);

							if (shoes != null)
								shoes.Delete();
						}

						var hue = Utility.RandomYellowHue();

						if (elf)
							EquipItem(new ElvenBoots(hue));
						else
							EquipItem(new Boots(hue));

						EquipItem(new SkinningKnife());
					}
					else if (gargoyle)
						PackItem(new SkinningKnife());

					break;
				}
				case SkillName.Veterinary:
				{
					PackItem(new Bandage(5));
					PackItem(new Scissors());
					break;
				}
				case SkillName.Wrestling:
				{
					if (elf)
						EquipItem(new LeafGloves());
					else if (human)
						EquipItem(new LeatherGloves());
					else if (gargoyle)
					{
						// Why not give them arm armor?
						EquipItem(new GargishLeatherArms());
					}

					break;
				}
				case SkillName.Throwing:
				{
					if (gargoyle)
						EquipItem(new Boomerang());

					break;
				}
				case SkillName.Mysticism:
				{
					PackItem(new MysticBook((ulong)0xAB));
					break;
				}
			}
		}

		private class BadStartMessage : Timer
		{
			readonly Mobile m_Mobile;
			readonly int m_Message;

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