#region Header
// **********
// ServUO - Skills.cs
// **********
#endregion

#region References
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Server.Network;
#endregion

namespace Server
{
	public enum StatCode
	{
		Str,
		Dex,
		Int
	}

	public delegate TimeSpan SkillUseCallback(Mobile user);

	public enum SkillLock : byte
	{
		Up = 0,
		Down = 1,
		Locked = 2
	}

	public enum SkillName
	{
		Alchemy = 0,
		Anatomy = 1,
		AnimalLore = 2,
		ItemID = 3,
		ArmsLore = 4,
		Parry = 5,
		Begging = 6,
		Blacksmith = 7,
		Fletching = 8,
		Peacemaking = 9,
		Camping = 10,
		Carpentry = 11,
		Cartography = 12,
		Cooking = 13,
		DetectHidden = 14,
		Discordance = 15,
		EvalInt = 16,
		Healing = 17,
		Fishing = 18,
		Forensics = 19,
		Herding = 20,
		Hiding = 21,
		Provocation = 22,
		Inscribe = 23,
		Lockpicking = 24,
		Magery = 25,
		MagicResist = 26,
		Tactics = 27,
		Snooping = 28,
		Musicianship = 29,
		Poisoning = 30,
		Archery = 31,
		SpiritSpeak = 32,
		Stealing = 33,
		Tailoring = 34,
		AnimalTaming = 35,
		TasteID = 36,
		Tinkering = 37,
		Tracking = 38,
		Veterinary = 39,
		Swords = 40,
		Macing = 41,
		Fencing = 42,
		Wrestling = 43,
		Lumberjacking = 44,
		Mining = 45,
		Meditation = 46,
		Stealth = 47,
		RemoveTrap = 48,
		Necromancy = 49,
		Focus = 50,
		Chivalry = 51,
		Bushido = 52,
		Ninjitsu = 53,
		Spellweaving = 54,
		Mysticism = 55,
		Imbuing = 56,
		Throwing = 57
	}

	[PropertyObject]
	public class Skill
	{
		private readonly Skills m_Owner;
		private readonly SkillInfo m_Info;
		private ushort m_Base;
		private ushort m_Cap;
		private SkillLock m_Lock;

		public override string ToString()
		{
			return String.Format("[{0}: {1}]", Name, Base);
		}

		public Skill(Skills owner, SkillInfo info, GenericReader reader)
		{
			m_Owner = owner;
			m_Info = info;

			int version = reader.ReadByte();

			switch (version)
			{
				case 0:
					{
						m_Base = reader.ReadUShort();
						m_Cap = reader.ReadUShort();
						m_Lock = (SkillLock)reader.ReadByte();

						break;
					}
				case 0xFF:
					{
						m_Base = 0;
						m_Cap = 1000;
						m_Lock = SkillLock.Up;

						break;
					}
				default:
					{
						if ((version & 0xC0) == 0x00)
						{
							if ((version & 0x1) != 0)
							{
								m_Base = reader.ReadUShort();
							}

							if ((version & 0x2) != 0)
							{
								m_Cap = reader.ReadUShort();
							}
							else
							{
								m_Cap = 1000;
							}

							if ((version & 0x4) != 0)
							{
								m_Lock = (SkillLock)reader.ReadByte();
							}

                            if ((version & 0x8) != 0)
                            {
                                KnownMasteries = reader.ReadInt();
                            }
						}

						break;
					}
			}

			if (m_Lock < SkillLock.Up || m_Lock > SkillLock.Locked)
			{
				Console.WriteLine("Bad skill lock -> {0}.{1}", owner.Owner, m_Lock);
				m_Lock = SkillLock.Up;
			}
		}

		public Skill(Skills owner, SkillInfo info, int baseValue, int cap, SkillLock skillLock)
		{
			m_Owner = owner;
			m_Info = info;
			m_Base = (ushort)baseValue;
			m_Cap = (ushort)cap;
			m_Lock = skillLock;
		}

		public void SetLockNoRelay(SkillLock skillLock)
		{
			if (skillLock < SkillLock.Up || skillLock > SkillLock.Locked)
			{
				return;
			}

			m_Lock = skillLock;
		}

		public void Serialize(GenericWriter writer)
		{
			if (m_Base == 0 && m_Cap == 1000 && m_Lock == SkillLock.Up)
			{
				writer.Write((byte)0xFF); // default
			}
			else
			{
				int flags = 0x0;

				if (m_Base != 0)
				{
					flags |= 0x1;
				}

				if (m_Cap != 1000)
				{
					flags |= 0x2;
				}

				if (m_Lock != SkillLock.Up)
				{
					flags |= 0x4;
				}

                if (KnownMasteries != 0)
                {
                    flags |= 0x8;
                }

				writer.Write((byte)flags); // version

				if (m_Base != 0)
				{
					writer.Write((short)m_Base);
				}

				if (m_Cap != 1000)
				{
					writer.Write((short)m_Cap);
				}

				if (m_Lock != SkillLock.Up)
				{
					writer.Write((byte)m_Lock);
				}

                if (KnownMasteries != 0)
                {
                    writer.Write((int)KnownMasteries);
                }
			}
		}

		public Skills Owner { get { return m_Owner; } }

		public SkillName SkillName { get { return (SkillName)m_Info.SkillID; } }

		public int SkillID { get { return m_Info.SkillID; } }

		[CommandProperty(AccessLevel.Counselor)]
		public string Name { get { return m_Info.Name; } }

		public SkillInfo Info { get { return m_Info; } }

		[CommandProperty(AccessLevel.Counselor)]
		public SkillLock Lock { get { return m_Lock; } }

        [CommandProperty(AccessLevel.Counselor)]
        public int KnownMasteries
        {
            get;
            set;
        }

		public int BaseFixedPoint
		{
			get { return m_Base; }
			set
			{
				if (value < 0)
				{
					value = 0;
				}
				else if (value >= 0x10000)
				{
					value = 0xFFFF;
				}

				ushort sv = (ushort)value;

				int oldBase = m_Base;

				if (m_Base != sv)
				{
					m_Owner.Total = (m_Owner.Total - m_Base) + sv;

					m_Base = sv;

					m_Owner.OnSkillChange(this);

					Mobile m = m_Owner.Owner;

					if (m != null)
					{
						m.OnSkillChange(SkillName, (double)oldBase / 10);
					}
				}
			}
		}

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public double Base { get { return (m_Base / 10.0); } set { BaseFixedPoint = (int)(value * 10.0); } }

		public int CapFixedPoint
		{
			get { return m_Cap; }
			set
			{
				if (value < 0)
				{
					value = 0;
				}
				else if (value >= 0x10000)
				{
					value = 0xFFFF;
				}

				ushort sv = (ushort)value;

				if (m_Cap != sv)
				{
					m_Cap = sv;

					m_Owner.OnSkillChange(this);
				}
			}
		}

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public double Cap { get { return (m_Cap / 10.0); } set { CapFixedPoint = (int)(value * 10.0); } }

		private static bool m_UseStatMods;

		public static bool UseStatMods { get { return m_UseStatMods; } set { m_UseStatMods = value; } }

		public int Fixed { get { return (int)(Value * 10); } }

		[CommandProperty(AccessLevel.Counselor)]
		public double Value
		{
			get
			{
				//There has to be this distinction between the racial values and not to account for gaining skills and these skills aren't displayed nor Totaled up.
				double value = NonRacialValue;

				double raceBonus = m_Owner.Owner.GetRacialSkillBonus(this.SkillName);

				if (raceBonus > value)
				{
					value = raceBonus;
				}

				return value;
			}
		}

		[CommandProperty(AccessLevel.Counselor)]
		public double NonRacialValue
		{
			get
			{
				double baseValue = Base;
				double inv = 100.0 - baseValue;

				if (inv < 0.0)
				{
					inv = 0.0;
				}

				inv /= 100.0;

				double statsOffset = ((m_UseStatMods ? m_Owner.Owner.Str : m_Owner.Owner.RawStr) * m_Info.StrScale) +
									 ((m_UseStatMods ? m_Owner.Owner.Dex : m_Owner.Owner.RawDex) * m_Info.DexScale) +
									 ((m_UseStatMods ? m_Owner.Owner.Int : m_Owner.Owner.RawInt) * m_Info.IntScale);
				double statTotal = m_Info.StatTotal * inv;

				statsOffset *= inv;

				if (statsOffset > statTotal)
				{
					statsOffset = statTotal;
				}

				double value = baseValue + statsOffset;

				m_Owner.Owner.ValidateSkillMods();

				var mods = m_Owner.Owner.SkillMods;

				double bonusObey = 0.0, bonusNotObey = 0.0;

				for (int i = 0; i < mods.Count; ++i)
				{
					SkillMod mod = mods[i];

					if (mod.Skill == (SkillName)m_Info.SkillID)
					{
						if (mod.Relative)
						{
							if (mod.ObeyCap)
							{
								bonusObey += mod.Value;
							}
							else
							{
								bonusNotObey += mod.Value;
							}
						}
						else
						{
							bonusObey = 0.0;
							bonusNotObey = 0.0;
							value = mod.Value;
						}
					}
				}

				value += bonusNotObey;

				if (value < Cap)
				{
					value += bonusObey;

					if (value > Cap)
					{
						value = Cap;
					}
				}

				return value;
			}
		}

        public bool IsMastery
        {
            get
            {
                return m_Info.IsMastery;
            }
        }

        public bool LearnMastery(int volume)
        {
            if (!IsMastery || HasLearnedMastery(volume))
                return false;

            KnownMasteries = volume;

            if (KnownMasteries > 3)
                KnownMasteries = 3;

            return true;
        }

        public bool HasLearnedMastery(int volume)
        {
            return KnownMasteries >= volume;
        }

        public bool SetCurrent()
        {
            if (IsMastery)
            {
                m_Owner.CurrentMastery = (SkillName)m_Info.SkillID;
                return true;
            }

            return false;
        }

		public void Update()
		{
			m_Owner.OnSkillChange(this);
		}
	}

	public class SkillInfo
	{
		private readonly int m_SkillID;

		public SkillInfo(
			int skillID,
			string name,
			double strScale,
			double dexScale,
			double intScale,
			string title,
			SkillUseCallback callback,
			double strGain,
			double dexGain,
			double intGain,
			double gainFactor,
			StatCode primary,
            StatCode secondary, 
            bool mastery = false)
		{
			Name = name;
			Title = title;
			m_SkillID = skillID;
			StrScale = strScale / 100.0;
			DexScale = dexScale / 100.0;
			IntScale = intScale / 100.0;
			Callback = callback;
			StrGain = strGain;
			DexGain = dexGain;
			IntGain = intGain;
			GainFactor = gainFactor;
			Primary = primary;
			Secondary = secondary;
            IsMastery = mastery;

			StatTotal = strScale + dexScale + intScale;
		}

		public StatCode Primary { get; private set; }
		public StatCode Secondary { get; private set; }

		public SkillUseCallback Callback { get; set; }

		public int SkillID { get { return m_SkillID; } }

		public string Name { get; set; }

		public string Title { get; set; }

		public double StrScale { get; set; }

		public double DexScale { get; set; }

		public double IntScale { get; set; }

		public double StatTotal { get; set; }

		public double StrGain { get; set; }

		public double DexGain { get; set; }

		public double IntGain { get; set; }

		public double GainFactor { get; set; }

        public bool IsMastery { get; set; }

        private static SkillInfo[] m_Table = new SkillInfo[58]
		{
			new SkillInfo(0, "Alchemy", 0.0, 5.0, 5.0, "Alchemist", null, 0.0, 0.5, 0.5, 1.0, StatCode.Int, StatCode.Dex),
			new SkillInfo(1, "Anatomy", 0.0, 0.0, 0.0, "Biologist", null, 0.15, 0.15, 0.7, 1.0, StatCode.Int, StatCode.Str),
			new SkillInfo(2, "Animal Lore", 0.0, 0.0, 0.0, "Naturalist", null, 0.0, 0.0, 1.0, 1.0, StatCode.Int, StatCode.Str),
			new SkillInfo(3, "Item Identification", 0.0, 0.0, 0.0, "Merchant", null, 0.0, 0.0, 1.0, 1.0, StatCode.Int, StatCode.Dex),
			new SkillInfo(4, "Arms Lore", 0.0, 0.0, 0.0, "Weapon Master", null, 0.75, 0.15, 0.1, 1.0, StatCode.Int, StatCode.Str),
			new SkillInfo(5, "Parrying", 7.5, 2.5, 0.0, "Duelist", null, 0.75, 0.25, 0.0, 1.0, StatCode.Dex, StatCode.Str, true ),
			new SkillInfo(6, "Begging", 0.0, 0.0, 0.0, "Beggar", null, 0.0, 0.0, 0.0, 1.0, StatCode.Dex, StatCode.Int),
			new SkillInfo(7, "Blacksmithy", 10.0, 0.0, 0.0, "Blacksmith", null, 1.0, 0.0, 0.0, 1.0, StatCode.Str, StatCode.Dex),
			new SkillInfo(8, "Bowcraft/Fletching", 6.0, 16.0, 0.0, "Bowyer", null, 0.6, 1.6, 0.0, 1.0, StatCode.Dex, StatCode.Str),
			new SkillInfo(9, "Peacemaking", 0.0, 0.0, 0.0, "Pacifier", null, 0.0, 0.0, 0.0, 1.0, StatCode.Int, StatCode.Dex, true ),
			new SkillInfo(10, "Camping", 20.0, 15.0, 15.0, "Explorer", null, 2.0, 1.5, 1.5, 1.0, StatCode.Dex, StatCode.Int),
			new SkillInfo(11, "Carpentry", 20.0, 5.0, 0.0, "Carpenter", null, 2.0, 0.5, 0.0, 1.0, StatCode.Str, StatCode.Dex),
			new SkillInfo(12, "Cartography", 0.0, 7.5, 7.5, "Cartographer", null, 0.0, 0.75, 0.75, 1.0, StatCode.Int, StatCode.Dex),
			new SkillInfo(13, "Cooking", 0.0, 20.0, 30.0, "Chef", null, 0.0, 2.0, 3.0, 1.0, StatCode.Int, StatCode.Dex),
			new SkillInfo(14, "Detecting Hidden", 0.0, 0.0, 0.0, "Scout", null, 0.0, 0.4, 0.6, 1.0, StatCode.Int, StatCode.Dex),
			new SkillInfo(15, "Discordance", 0.0, 2.5, 2.5, "Demoralizer", null, 0.0, 0.25, 0.25, 1.0, StatCode.Dex, StatCode.Int, true ),
			new SkillInfo(16, "Evaluating Intelligence", 0.0, 0.0, 0.0, "Scholar", null, 0.0, 0.0, 1.0, 1.0, StatCode.Int, StatCode.Str),
			new SkillInfo(17, "Healing", 6.0, 6.0, 8.0, "Healer", null, 0.6, 0.6, 0.8, 1.0, StatCode.Int, StatCode.Dex),
			new SkillInfo(18, "Fishing", 0.0, 0.0, 0.0, "Fisherman", null, 0.5, 0.5, 0.0, 1.0, StatCode.Dex, StatCode.Str),
			new SkillInfo(19, "Forensic Evaluation", 0.0, 0.0, 0.0, "Detective", null, 0.0, 0.2, 0.8, 1.0, StatCode.Int, StatCode.Dex),
			new SkillInfo(20, "Herding", 16.25, 6.25, 2.5, "Shepherd", null, 1.625, 0.625, 0.25, 1.0, StatCode.Int, StatCode.Dex),
			new SkillInfo(21, "Hiding", 0.0, 0.0, 0.0, "Shade", null, 0.0, 0.8, 0.2, 1.0, StatCode.Dex, StatCode.Int),
			new SkillInfo(22, "Provocation", 0.0, 4.5, 0.5, "Rouser", null, 0.0, 0.45, 0.05, 1.0, StatCode.Int, StatCode.Dex, true ),
			new SkillInfo(23, "Inscription", 0.0, 2.0, 8.0, "Scribe", null, 0.0, 0.2, 0.8, 1.0, StatCode.Int, StatCode.Dex),
			new SkillInfo(24, "Lockpicking", 0.0, 25.0, 0.0, "Infiltrator", null, 0.0, 2.0, 0.0, 1.0, StatCode.Dex, StatCode.Int),
			new SkillInfo(25, "Magery", 0.0, 0.0, 15.0, "Mage", null, 0.0, 0.0, 1.5, 1.0, StatCode.Int, StatCode.Str, true ),
			new SkillInfo(26, "Resisting Spells", 0.0, 0.0, 0.0, "Warder", null, 0.25, 0.25, 0.5, 1.0, StatCode.Str, StatCode.Dex),
			new SkillInfo(27, "Tactics", 0.0, 0.0, 0.0, "Tactician", null, 0.0, 0.0, 0.0, 1.0, StatCode.Str, StatCode.Dex),
			new SkillInfo(28, "Snooping", 0.0, 25.0, 0.0, "Spy", null, 0.0, 2.5, 0.0, 1.0, StatCode.Dex, StatCode.Int),
			new SkillInfo(29, "Musicianship", 0.0, 0.0, 0.0, "Bard", null, 0.0, 0.8, 0.2, 1.0, StatCode.Dex, StatCode.Int),
			new SkillInfo(30, "Poisoning", 0.0, 4.0, 16.0, "Assassin", null, 0.0, 0.4, 1.6, 1.0, StatCode.Int, StatCode.Dex, true ),
			new SkillInfo(31, "Archery", 2.5, 7.5, 0.0, "Archer", null, 0.25, 0.75, 0.0, 1.0, StatCode.Dex, StatCode.Str, true ),
			new SkillInfo(32, "Spirit Speak", 0.0, 0.0, 0.0, "Medium", null, 0.0, 0.0, 1.0, 1.0, StatCode.Int, StatCode.Str),
			new SkillInfo(33, "Stealing", 0.0, 10.0, 0.0, "Pickpocket", null, 0.0, 1.0, 0.0, 1.0, StatCode.Dex, StatCode.Int),
			new SkillInfo(34, "Tailoring", 3.75, 16.25, 5.0, "Tailor", null, 0.38, 1.63, 0.5, 1.0, StatCode.Dex, StatCode.Int),
			new SkillInfo(35, "Animal Taming", 14.0, 2.0, 4.0, "Tamer", null, 1.4, 0.2, 0.4, 1.0, StatCode.Str, StatCode.Int, true ),
			new SkillInfo(36, "Taste Identification", 0.0, 0.0, 0.0, "Praegustator", null, 0.2, 0.0, 0.8, 1.0, StatCode.Int, StatCode.Str),
			new SkillInfo(37, "Tinkering", 5.0, 2.0, 3.0, "Tinker", null, 0.5, 0.2, 0.3, 1.0, StatCode.Dex, StatCode.Int),
			new SkillInfo(38, "Tracking", 0.0, 12.5, 12.5, "Ranger", null, 0.0, 1.25, 1.25, 1.0, StatCode.Int, StatCode.Dex),
			new SkillInfo(39, "Veterinary", 8.0, 4.0, 8.0, "Veterinarian", null, 0.8, 0.4, 0.8, 1.0, StatCode.Int, StatCode.Dex),
			new SkillInfo(40, "Swordsmanship", 7.5, 2.5, 0.0, "Swordsman", null, 0.75, 0.25, 0.0, 1.0, StatCode.Str, StatCode.Dex, true ),
			new SkillInfo(41, "Mace Fighting", 9.0, 1.0, 0.0, "Armsman", null, 0.9, 0.1, 0.0, 1.0, StatCode.Str, StatCode.Dex, true ),
			new SkillInfo(42, "Fencing", 4.5, 5.5, 0.0, "Fencer", null, 0.45, 0.55, 0.0, 1.0, StatCode.Dex, StatCode.Str, true ),
			new SkillInfo(43, "Wrestling", 9.0, 1.0, 0.0, "Wrestler", null, 0.9, 0.1, 0.0, 1.0, StatCode.Str, StatCode.Dex, true ),
			new SkillInfo(44, "Lumberjacking", 20.0, 0.0, 0.0, "Lumberjack", null, 2.0, 0.0, 0.0, 1.0, StatCode.Str, StatCode.Dex),
			new SkillInfo(45, "Mining", 20.0, 0.0, 0.0, "Miner", null, 2.0, 0.0, 0.0, 1.0, StatCode.Str, StatCode.Dex),
			new SkillInfo(46, "Meditation", 0.0, 0.0, 0.0, "Stoic", null, 0.0, 0.0, 0.0, 1.0, StatCode.Int, StatCode.Str),
			new SkillInfo(47, "Stealth", 0.0, 0.0, 0.0, "Rogue", null, 0.0, 0.0, 0.0, 1.0, StatCode.Dex, StatCode.Int),
			new SkillInfo(48, "Remove Trap", 0.0, 0.0, 0.0, "Trap Specialist", null, 0.0, 0.0, 0.0, 1.0, StatCode.Dex, StatCode.Int),
			new SkillInfo(49, "Necromancy", 0.0, 0.0, 0.0, "Necromancer", null, 0.0, 0.0, 0.0, 1.0, StatCode.Int, StatCode.Str, true ),
			new SkillInfo(50, "Focus", 0.0, 0.0, 0.0, "Driven", null, 0.0, 0.0, 0.0, 1.0, StatCode.Dex, StatCode.Int),
			new SkillInfo(51, "Chivalry", 0.0, 0.0, 0.0, "Paladin", null, 0.0, 0.0, 0.0, 1.0, StatCode.Str, StatCode.Int, true ),
			new SkillInfo(52, "Bushido", 0.0, 0.0, 0.0, "Samurai", null, 0.0, 0.0, 0.0, 1.0, StatCode.Str, StatCode.Int, true ),
			new SkillInfo(53, "Ninjitsu", 0.0, 0.0, 0.0, "Ninja", null, 0.0, 0.0, 0.0, 1.0, StatCode.Dex, StatCode.Int, true ),
			new SkillInfo(54, "Spellweaving", 0.0, 0.0, 0.0, "Arcanist", null, 0.0, 0.0, 0.0, 1.0, StatCode.Int, StatCode.Str),
			new SkillInfo(55, "Mysticism", 0.0, 0.0, 0.0, "Mystic", null, 0.0, 0.0, 0.0, 1.0, StatCode.Str, StatCode.Int, true ),
			new SkillInfo(56, "Imbuing", 0.0, 0.0, 0.0, "Artificer", null, 0.0, 0.0, 0.0, 1.0, StatCode.Int, StatCode.Str),
			new SkillInfo(57, "Throwing", 0.0, 0.0, 0.0, "Bladeweaver", null, 0.0, 0.0, 0.0, 1.0, StatCode.Dex, StatCode.Str, true ),
        };

		public static SkillInfo[] Table { get { return m_Table; } set { m_Table = value; } }
	}

	[PropertyObject]
	public class Skills : IEnumerable<Skill>
	{
		private readonly Mobile m_Owner;
		private readonly Skill[] m_Skills;
		private int m_Total, m_Cap;
		private Skill m_Highest;

		#region Skill Getters & Setters
		[CommandProperty(AccessLevel.Counselor)]
		public Skill Alchemy { get { return this[SkillName.Alchemy]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Anatomy { get { return this[SkillName.Anatomy]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill AnimalLore { get { return this[SkillName.AnimalLore]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill ItemID { get { return this[SkillName.ItemID]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill ArmsLore { get { return this[SkillName.ArmsLore]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Parry { get { return this[SkillName.Parry]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Begging { get { return this[SkillName.Begging]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Blacksmith { get { return this[SkillName.Blacksmith]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Fletching { get { return this[SkillName.Fletching]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Peacemaking { get { return this[SkillName.Peacemaking]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Camping { get { return this[SkillName.Camping]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Carpentry { get { return this[SkillName.Carpentry]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Cartography { get { return this[SkillName.Cartography]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Cooking { get { return this[SkillName.Cooking]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill DetectHidden { get { return this[SkillName.DetectHidden]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Discordance { get { return this[SkillName.Discordance]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill EvalInt { get { return this[SkillName.EvalInt]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Healing { get { return this[SkillName.Healing]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Fishing { get { return this[SkillName.Fishing]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Forensics { get { return this[SkillName.Forensics]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Herding { get { return this[SkillName.Herding]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Hiding { get { return this[SkillName.Hiding]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Provocation { get { return this[SkillName.Provocation]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Inscribe { get { return this[SkillName.Inscribe]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Lockpicking { get { return this[SkillName.Lockpicking]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Magery { get { return this[SkillName.Magery]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill MagicResist { get { return this[SkillName.MagicResist]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Tactics { get { return this[SkillName.Tactics]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Snooping { get { return this[SkillName.Snooping]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Musicianship { get { return this[SkillName.Musicianship]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Poisoning { get { return this[SkillName.Poisoning]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Archery { get { return this[SkillName.Archery]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill SpiritSpeak { get { return this[SkillName.SpiritSpeak]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Stealing { get { return this[SkillName.Stealing]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Tailoring { get { return this[SkillName.Tailoring]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill AnimalTaming { get { return this[SkillName.AnimalTaming]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill TasteID { get { return this[SkillName.TasteID]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Tinkering { get { return this[SkillName.Tinkering]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Tracking { get { return this[SkillName.Tracking]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Veterinary { get { return this[SkillName.Veterinary]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Swords { get { return this[SkillName.Swords]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Macing { get { return this[SkillName.Macing]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Fencing { get { return this[SkillName.Fencing]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Wrestling { get { return this[SkillName.Wrestling]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Lumberjacking { get { return this[SkillName.Lumberjacking]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Mining { get { return this[SkillName.Mining]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Meditation { get { return this[SkillName.Meditation]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Stealth { get { return this[SkillName.Stealth]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill RemoveTrap { get { return this[SkillName.RemoveTrap]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Necromancy { get { return this[SkillName.Necromancy]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Focus { get { return this[SkillName.Focus]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Chivalry { get { return this[SkillName.Chivalry]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Bushido { get { return this[SkillName.Bushido]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Ninjitsu { get { return this[SkillName.Ninjitsu]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Spellweaving { get { return this[SkillName.Spellweaving]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Mysticism { get { return this[SkillName.Mysticism]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Imbuing { get { return this[SkillName.Imbuing]; } set { } }

		[CommandProperty(AccessLevel.Counselor)]
		public Skill Throwing { get { return this[SkillName.Throwing]; } set { } }
		#endregion

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public int Cap { get { return m_Cap; } set { m_Cap = value; } }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public SkillName CurrentMastery
        {
            get;
            set;
        }

		public int Total { get { return m_Total; } set { m_Total = value; } }

		public Mobile Owner { get { return m_Owner; } }

		public int Length { get { return m_Skills.Length; } }

		public Skill this[SkillName name] { get { return this[(int)name]; } }

		public Skill this[int skillID]
		{
			get
			{
				if (skillID < 0 || skillID >= m_Skills.Length)
				{
					return null;
				}

				Skill sk = m_Skills[skillID];

				if (sk == null)
				{
					m_Skills[skillID] = sk = new Skill(this, SkillInfo.Table[skillID], 0, 1000, SkillLock.Up);
				}

				return sk;
			}
		}

		public override string ToString()
		{
			return "...";
		}

		public static bool UseSkill(Mobile from, SkillName name)
		{
			return UseSkill(from, (int)name);
		}

		public static bool UseSkill(Mobile from, int skillID)
		{
			if (!from.CheckAlive())
			{
				return false;
			}
			else if (!from.Region.OnSkillUse(from, skillID))
			{
				return false;
			}
			else if (!from.AllowSkillUse((SkillName)skillID))
			{
				return false;
			}

			if (skillID >= 0 && skillID < SkillInfo.Table.Length)
			{
				SkillInfo info = SkillInfo.Table[skillID];

				if (info.Callback != null)
				{
					if (Core.TickCount - from.NextSkillTime >= 0 && from.Spell == null)
					{
						from.DisruptiveAction();

						from.NextSkillTime = Core.TickCount + (int)(info.Callback(from)).TotalMilliseconds;

						return true;
					}
					else
					{
						from.SendSkillMessage();
					}
				}
				else
				{
					from.SendLocalizedMessage(500014); // That skill cannot be used directly.
				}
			}

			return false;
		}

		public Skill Highest
		{
			get
			{
				if (m_Highest == null)
				{
					Skill highest = null;
					int value = int.MinValue;

					for (int i = 0; i < m_Skills.Length; ++i)
					{
						Skill sk = m_Skills[i];

						if (sk != null && sk.BaseFixedPoint > value)
						{
							value = sk.BaseFixedPoint;
							highest = sk;
						}
					}

					if (highest == null && m_Skills.Length > 0)
					{
						highest = this[0];
					}

					m_Highest = highest;
				}

				return m_Highest;
			}
		}

		public void Serialize(GenericWriter writer)
		{
			m_Total = 0;

			writer.Write(4); // version

            writer.Write((int)CurrentMastery);

			writer.Write(m_Cap);
			writer.Write(m_Skills.Length);

			for (int i = 0; i < m_Skills.Length; ++i)
			{
				Skill sk = m_Skills[i];

				if (sk == null)
				{
					writer.Write((byte)0xFF);
				}
				else
				{
					sk.Serialize(writer);
					m_Total += sk.BaseFixedPoint;
				}
			}
		}

		public Skills(Mobile owner)
		{
			m_Owner = owner;
            m_Cap = Config.Get("PlayerCaps.TotalSkillCap", 7000); ;

			var info = SkillInfo.Table;

			m_Skills = new Skill[info.Length];

			//for ( int i = 0; i < info.Length; ++i )
			//	m_Skills[i] = new Skill( this, info[i], 0, 1000, SkillLock.Up );
		}

		public Skills(Mobile owner, GenericReader reader)
		{
			m_Owner = owner;

			int version = reader.ReadInt();

			switch (version)
			{
                case 4:
                    CurrentMastery = (SkillName)reader.ReadInt();
                    goto case 3;
				case 3:
				case 2:
					{
						m_Cap = reader.ReadInt();

						goto case 1;
					}
				case 1:
					{
						if (version < 2)
						{
							m_Cap = 7000;
						}

						if (version < 3)
						{
							/*m_Total =*/
							reader.ReadInt();
						}

						var info = SkillInfo.Table;

						m_Skills = new Skill[info.Length];

						int count = reader.ReadInt();

						for (int i = 0; i < count; ++i)
						{
							if (i < info.Length)
							{
								Skill sk = new Skill(this, info[i], reader);

                                if (sk.BaseFixedPoint != 0 || sk.CapFixedPoint != 1000 || sk.Lock != SkillLock.Up || sk.KnownMasteries != 0)
								{
									m_Skills[i] = sk;
									m_Total += sk.BaseFixedPoint;
								}
							}
							else
							{
								new Skill(this, null, reader);
							}
						}

						//for ( int i = count; i < info.Length; ++i )
						//	m_Skills[i] = new Skill( this, info[i], 0, 1000, SkillLock.Up );

						break;
					}
				case 0:
					{
						reader.ReadInt();

						goto case 1;
					}
			}
		}

		public void OnSkillChange(Skill skill)
		{
			if (skill == m_Highest) // could be downgrading the skill, force a recalc
			{
				m_Highest = null;
			}
			else if (m_Highest != null && skill.BaseFixedPoint > m_Highest.BaseFixedPoint)
			{
				m_Highest = skill;
			}

			m_Owner.OnSkillInvalidated(skill);

			NetState ns = m_Owner.NetState;

			if (ns != null)
			{
				ns.Send(new SkillChange(skill));
			}
		}

		public IEnumerator<Skill> GetEnumerator()
		{
			return m_Skills.Where(s => s != null).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return m_Skills.Where(s => s != null).GetEnumerator();
		}
	}
}