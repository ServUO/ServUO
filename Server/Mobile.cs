#region References
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Server.Accounting;
using Server.Commands;
using Server.ContextMenus;
using Server.Guilds;
using Server.Gumps;
using Server.HuePickers;
using Server.Items;
using Server.Menus;
using Server.Mobiles;
using Server.Network;
using Server.Prompts;
using Server.Targeting;
#endregion

namespace Server
{
	#region Callbacks
	public delegate void TargetCallback(Mobile from, object targeted);

	public delegate void TargetStateCallback(Mobile from, object targeted, object state);

	public delegate void TargetStateCallback<T>(Mobile from, object targeted, T state);

	public delegate void PromptCallback(Mobile from, string text);

	public delegate void PromptStateCallback(Mobile from, string text, object state);

	public delegate void PromptStateCallback<T>(Mobile from, string text, T state);
	#endregion

	#region [...]Mods
	public class TimedSkillMod : SkillMod
	{
		private readonly DateTime m_Expire;

		public TimedSkillMod(SkillName skill, bool relative, double value, TimeSpan delay)
			: this(skill, relative, value, DateTime.UtcNow + delay)
		{ }

		public TimedSkillMod(SkillName skill, bool relative, double value, DateTime expire)
			: base(skill, relative, value)
		{
			m_Expire = expire;
		}

		public override bool CheckCondition()
		{
			return DateTime.UtcNow < m_Expire;
		}
	}

	public class EquipedSkillMod : SkillMod
	{
		private readonly Item m_Item;
		private readonly Mobile m_Mobile;

		public EquipedSkillMod(SkillName skill, bool relative, double value, Item item, Mobile mobile)
			: base(skill, relative, value)
		{
			m_Item = item;
			m_Mobile = mobile;
		}

		public override bool CheckCondition()
		{
			return !m_Item.Deleted && !m_Mobile.Deleted && m_Item.Parent == m_Mobile;
		}
	}

	public class DefaultSkillMod : SkillMod
	{
		public DefaultSkillMod(SkillName skill, bool relative, double value)
			: base(skill, relative, value)
		{ }

		public override bool CheckCondition()
		{
			return true;
		}
	}

	public abstract class SkillMod
	{
		private Mobile m_Owner;
		private SkillName m_Skill;
		private bool m_Relative;
		private double m_Value;
		private bool m_ObeyCap;

		protected SkillMod(SkillName skill, bool relative, double value)
		{
			m_Skill = skill;
			m_Relative = relative;
			m_Value = value;
		}

		public bool ObeyCap
		{
			get => m_ObeyCap;
			set
			{
				m_ObeyCap = value;

				if (m_Owner != null)
				{
					var sk = m_Owner.Skills[m_Skill];

					if (sk != null)
					{
						sk.Update();
					}
				}
			}
		}

		public Mobile Owner
		{
			get => m_Owner;
			set
			{
				if (m_Owner != value)
				{
					if (m_Owner != null)
					{
						m_Owner.RemoveSkillMod(this);
					}

					m_Owner = value;

					if (m_Owner != value)
					{
						m_Owner.AddSkillMod(this);
					}
				}
			}
		}

		public void Remove()
		{
			Owner = null;
		}

		public SkillName Skill
		{
			get => m_Skill;
			set
			{
				if (m_Skill != value)
				{
					var oldUpdate = m_Owner != null ? m_Owner.Skills[m_Skill] : null;

					m_Skill = value;

					if (m_Owner != null)
					{
						var sk = m_Owner.Skills[m_Skill];

						if (sk != null)
						{
							sk.Update();
						}
					}

					if (oldUpdate != null)
					{
						oldUpdate.Update();
					}
				}
			}
		}

		public bool Relative
		{
			get => m_Relative;
			set
			{
				if (m_Relative != value)
				{
					m_Relative = value;

					if (m_Owner != null)
					{
						var sk = m_Owner.Skills[m_Skill];

						if (sk != null)
						{
							sk.Update();
						}
					}
				}
			}
		}

		public bool Absolute
		{
			get => !m_Relative;
			set
			{
				if (m_Relative == value)
				{
					m_Relative = !value;

					if (m_Owner != null)
					{
						var sk = m_Owner.Skills[m_Skill];

						if (sk != null)
						{
							sk.Update();
						}
					}
				}
			}
		}

		public double Value
		{
			get => m_Value;
			set
			{
				if (m_Value != value)
				{
					m_Value = value;

					if (m_Owner != null)
					{
						var sk = m_Owner.Skills[m_Skill];

						if (sk != null)
						{
							sk.Update();
						}
					}
				}
			}
		}

		public abstract bool CheckCondition();
	}

	public class ResistanceMod
	{
		private Mobile m_Owner;
		private ResistanceType m_Type;
		private int m_Offset;

		public Mobile Owner { get => m_Owner; set => m_Owner = value; }

		public ResistanceType Type
		{
			get => m_Type;
			set
			{
				if (m_Type != value)
				{
					m_Type = value;

					if (m_Owner != null)
					{
						m_Owner.UpdateResistances();
					}
				}
			}
		}

		public int Offset
		{
			get => m_Offset;
			set
			{
				if (m_Offset != value)
				{
					m_Offset = value;

					if (m_Owner != null)
					{
						m_Owner.UpdateResistances();
					}
				}
			}
		}

		public ResistanceMod(ResistanceType type, int offset)
		{
			m_Type = type;
			m_Offset = offset;
		}
	}

	public class StatMod
	{
		private readonly StatType m_Type;
		private readonly string m_Name;
		private readonly int m_Offset;
		private readonly TimeSpan m_Duration;
		private readonly DateTime m_Added;

		public StatType Type => m_Type;
		public string Name => m_Name;
		public int Offset => m_Offset;

		public bool HasElapsed()
		{
			if (m_Duration == TimeSpan.Zero)
			{
				return false;
			}

			return (DateTime.UtcNow - m_Added) >= m_Duration;
		}

		public TimeSpan TimeLeft()
		{
			return m_Duration - (DateTime.UtcNow - m_Added);
		}

		public StatMod(StatType type, string name, int offset, TimeSpan duration)
		{
			m_Type = type;
			m_Name = name;
			m_Offset = offset;
			m_Duration = duration;
			m_Added = DateTime.UtcNow;
		}
	}
	#endregion

	public class DamageEntry
	{
		private readonly Mobile m_Damager;
		private DateTime m_LastDamage;

		public Mobile Damager => m_Damager;
		public int DamageGiven { get; set; }
		public DateTime LastDamage { get => m_LastDamage; set => m_LastDamage = value; }
		public bool HasExpired => DateTime.UtcNow > (m_LastDamage + m_ExpireDelay);
		public List<DamageEntry> Responsible { get; set; }

		private static TimeSpan m_ExpireDelay = TimeSpan.FromMinutes(2.0);

		public static TimeSpan ExpireDelay { get => m_ExpireDelay; set => m_ExpireDelay = value; }

		public DamageEntry(Mobile damager)
		{
			m_Damager = damager;
		}
	}

	#region Enums
	[Flags]
	public enum StatType
	{
		Str = 1,
		Dex = 2,
		Int = 4,
		All = 7
	}

	public enum StatLockType : byte
	{
		Up,
		Down,
		Locked
	}

	[CustomEnum(new[] { "North", "Right", "East", "Down", "South", "Left", "West", "Up" })]
	[Flags]
	public enum Direction : byte
	{
		North = 0x0,
		Right = 0x1,
		East = 0x2,
		Down = 0x3,
		South = 0x4,
		Left = 0x5,
		West = 0x6,
		Up = 0x7,

		Mask = 0x7,
		Running = 0x80,
		ValueMask = 0x87
	}

	[Flags]
	public enum MobileDelta
	{
		None = 0x00000000,
		Name = 0x00000001,
		Flags = 0x00000002,
		Hits = 0x00000004,
		Mana = 0x00000008,
		Stam = 0x00000010,
		Stat = 0x00000020,
		Noto = 0x00000040,
		Gold = 0x00000080,
		Weight = 0x00000100,
		Direction = 0x00000200,
		Hue = 0x00000400,
		Body = 0x00000800,
		Armor = 0x00001000,
		StatCap = 0x00002000,
		GhostUpdate = 0x00004000,
		Followers = 0x00008000,
		Properties = 0x00010000,
		TithingPoints = 0x00020000,
		Resistances = 0x00040000,
		WeaponDamage = 0x00080000,
		Hair = 0x00100000,
		FacialHair = 0x00200000,
		Race = 0x00400000,
		HealthbarYellow = 0x00800000,
		HealthbarPoison = 0x01000000,
		Face = 0x08000000,
		Skills = 0x10000000,

		Attributes = 0x0000001C
	}

	public enum AccessLevel
	{
		Player,
		VIP,
		Counselor,
		Decorator,
		Spawner,
		GameMaster,
		Seer,
		Administrator,
		Developer,
		CoOwner,
		Owner
	}

	public enum VisibleDamageType
	{
		None,
		Related,
		Everyone
	}

	public enum ResistanceType
	{
		Physical,
		Fire,
		Cold,
		Poison,
		Energy
	}

	public enum ApplyPoisonResult
	{
		Poisoned,
		Immune,
		HigherPoisonActive,
		Cured
	}

	public enum AnimationType
	{
		Attack = 0,
		Parry = 1,
		Block = 2,
		Die = 3,
		Impact = 4,
		Fidget = 5,
		Eat = 6,
		Emote = 7,
		Alert = 8,
		TakeOff = 9,
		Land = 10,
		Spell = 11,
		StartCombat = 12,
		EndCombat = 13,
		Pillage = 14,
		Spawn = 15
	}

	public enum DFAlgorithm
	{
		Standard,
		PainSpike
	}
	#endregion

	[Serializable]
	public class MobileNotConnectedException : Exception
	{
		public MobileNotConnectedException(Mobile source, string message)
			: base(message)
		{
			Source = source.ToString();
		}
	}

	#region Delegates
	public delegate bool SkillCheckTargetHandler(
		Mobile from, SkillName skill, object target, double minSkill, double maxSkill);

	public delegate bool SkillCheckLocationHandler(Mobile from, SkillName skill, double minSkill, double maxSkill);

	public delegate bool SkillCheckDirectTargetHandler(Mobile from, SkillName skill, object target, double chance);

	public delegate bool SkillCheckDirectLocationHandler(Mobile from, SkillName skill, double chance);

	public delegate TimeSpan RegenRateHandler(Mobile from);

	public delegate bool AllowBeneficialHandler(Mobile from, Mobile target);

	public delegate bool AllowHarmfulHandler(Mobile from, IDamageable target);

	public delegate void FatigueHandler(Mobile m, int damage, DFAlgorithm df);

	public delegate Container CreateCorpseHandler(
		Mobile from, HairInfo hair, FacialHairInfo facialhair, List<Item> initialContent, List<Item> equipedItems);

	public delegate int AOSStatusHandler(Mobile from, int index);
	#endregion

	/// <summary>
	///     Base class representing players, npcs, and creatures.
	/// </summary>
	[System.Runtime.InteropServices.ComVisible(true)]
	public class Mobile : IEntity, IHued, IComparable<Mobile>, ISerializable, ISpawnable, IDamageable
	{
		#region CompareTo(...)
		public int CompareTo(IEntity other)
		{
			if (other == null)
			{
				return -1;
			}

			return m_Serial.CompareTo(other.Serial);
		}

		public int CompareTo(Mobile other)
		{
			return CompareTo((IEntity)other);
		}

		public int CompareTo(object other)
		{
			if (other == null || other is IEntity)
			{
				return CompareTo((IEntity)other);
			}

			throw new ArgumentException();
		}
		#endregion

		private static bool m_DragEffects = true;

		public static bool DragEffects { get => m_DragEffects; set => m_DragEffects = value; }

		#region Handlers
		public static AllowBeneficialHandler AllowBeneficialHandler { get; set; }
		public static AllowHarmfulHandler AllowHarmfulHandler { get; set; }

		public static FatigueHandler FatigueHandler { get; set; }

		private static SkillCheckTargetHandler m_SkillCheckTargetHandler;
		private static SkillCheckLocationHandler m_SkillCheckLocationHandler;
		private static SkillCheckDirectTargetHandler m_SkillCheckDirectTargetHandler;
		private static SkillCheckDirectLocationHandler m_SkillCheckDirectLocationHandler;

		public static SkillCheckTargetHandler SkillCheckTargetHandler { get => m_SkillCheckTargetHandler; set => m_SkillCheckTargetHandler = value; }

		public static SkillCheckLocationHandler SkillCheckLocationHandler { get => m_SkillCheckLocationHandler; set => m_SkillCheckLocationHandler = value; }

		public static SkillCheckDirectTargetHandler SkillCheckDirectTargetHandler { get => m_SkillCheckDirectTargetHandler; set => m_SkillCheckDirectTargetHandler = value; }

		public static SkillCheckDirectLocationHandler SkillCheckDirectLocationHandler { get => m_SkillCheckDirectLocationHandler; set => m_SkillCheckDirectLocationHandler = value; }

		private static AOSStatusHandler m_AOSStatusHandler;

		public static AOSStatusHandler AOSStatusHandler { get => m_AOSStatusHandler; set => m_AOSStatusHandler = value; }
		#endregion

		#region Regeneration
		private static RegenRateHandler m_HitsRegenRate, m_StamRegenRate, m_ManaRegenRate;
		private static TimeSpan m_DefaultHitsRate, m_DefaultStamRate, m_DefaultManaRate;

		public static RegenRateHandler HitsRegenRateHandler { get => m_HitsRegenRate; set => m_HitsRegenRate = value; }

		public static TimeSpan DefaultHitsRate { get => m_DefaultHitsRate; set => m_DefaultHitsRate = value; }

		public static RegenRateHandler StamRegenRateHandler { get => m_StamRegenRate; set => m_StamRegenRate = value; }

		public static TimeSpan DefaultStamRate { get => m_DefaultStamRate; set => m_DefaultStamRate = value; }

		public static RegenRateHandler ManaRegenRateHandler { get => m_ManaRegenRate; set => m_ManaRegenRate = value; }

		public static TimeSpan DefaultManaRate { get => m_DefaultManaRate; set => m_DefaultManaRate = value; }

		public static TimeSpan GetHitsRegenRate(Mobile m)
		{
			if (m_HitsRegenRate == null)
			{
				return m_DefaultHitsRate;
			}

			return m_HitsRegenRate(m);
		}

		public static TimeSpan GetStamRegenRate(Mobile m)
		{
			if (m_StamRegenRate == null)
			{
				return m_DefaultStamRate;
			}

			return m_StamRegenRate(m);
		}

		public static TimeSpan GetManaRegenRate(Mobile m)
		{
			if (m_ManaRegenRate == null)
			{
				return m_DefaultManaRate;
			}

			return m_ManaRegenRate(m);
		}
		#endregion

		private class MovementRecord
		{
			public long m_End;

			private static readonly Queue<MovementRecord> m_InstancePool = new Queue<MovementRecord>();

			public static MovementRecord NewInstance(long end)
			{
				MovementRecord r;

				if (m_InstancePool.Count > 0)
				{
					r = m_InstancePool.Dequeue();

					r.m_End = end;
				}
				else
				{
					r = new MovementRecord(end);
				}

				return r;
			}

			private MovementRecord(long end)
			{
				m_End = end;
			}

			public bool Expired()
			{
				var v = Core.TickCount - m_End >= 0;

				if (v)
				{
					m_InstancePool.Enqueue(this);
				}

				return v;
			}
		}

		#region Var declarations
		private readonly Serial m_Serial;
		private Map m_Map;
		private Point3D m_Location;
		private Direction m_Direction;
		private Body m_Body;
		private int m_Hue;
		private Poison m_Poison;
		private Timer m_PoisonTimer;
		private BaseGuild m_Guild;
		private string m_GuildTitle;
		private bool m_Criminal;
		private string m_Name;
		private int m_Deaths, m_Kills, m_ShortTermMurders;
		private int m_SpeechHue, m_EmoteHue, m_WhisperHue, m_YellHue;
		private string m_Language;
		private NetState m_NetState;
		private bool m_Female, m_Warmode, m_Hidden, m_Blessed, m_Flying;
		private int m_StatCap;
		private int m_StrCap;
		private int m_DexCap;
		private int m_IntCap;
		private int m_StrMaxCap;
		private int m_DexMaxCap;
		private int m_IntMaxCap;
		private int m_Str, m_Dex, m_Int;
		private int m_Hits, m_Stam, m_Mana;
		private int m_Fame, m_Karma;
		private AccessLevel m_AccessLevel;
		private Skills m_Skills;
		private List<Item> m_Items;
		private bool m_Player;
		private string m_Title;
		private string m_Profile;
		private bool m_ProfileLocked;
		private int m_LightLevel;
		private int m_TotalGold, m_TotalItems, m_TotalWeight;
		private List<StatMod> m_StatMods;
		private ISpell m_Spell;
		private Target m_Target;
		private Prompt m_Prompt;
		private ContextMenu m_ContextMenu;
		private List<AggressorInfo> m_Aggressors, m_Aggressed;
		private IDamageable m_Combatant;
		private List<Mobile> m_Stabled;
		private bool m_AutoPageNotify;
		private bool m_CanHearGhosts;
		private bool m_CanSwim, m_CantWalk;
		private int m_TithingPoints;
		private bool m_DisplayGuildTitle;
		private bool m_DisplayGuildAbbr;
		private Mobile m_GuildFealty;
		private DateTime[] m_StuckMenuUses;
		private long m_NextSkillTime;
		private long m_NextActionMessage;
		private bool m_Paralyzed;
		private bool m_Frozen;
		private int m_AllowedStealthSteps;
		private int m_Hunger;
		private int m_NameHue = -1;
		private Region m_Region;
		private int m_BaseSoundID;
		private int m_VirtualArmor;
		private bool m_Squelched;
		private int m_MagicDamageAbsorb;
		private int m_Followers, m_FollowersMax;
		private List<object> _actions; // prefer List<object> over ArrayList for more specific profiling information
		private Queue<MovementRecord> m_MoveRecords;
		private int m_WarmodeChanges;
		private DateTime m_NextWarmodeChange;
		private WarmodeTimer m_WarmodeTimer;
		private int m_Thirst, m_BAC;
		private VirtueInfo m_Virtues;
		private object m_Party;
		private List<SkillMod> m_SkillMods;
		private Body m_BodyMod;
		private DateTime m_LastStrGain;
		private DateTime m_LastIntGain;
		private DateTime m_LastDexGain;
		private Race m_Race;
		#endregion

		private static readonly TimeSpan WarmodeSpamCatch = TimeSpan.FromSeconds(1.0);
		private static readonly TimeSpan WarmodeSpamDelay = TimeSpan.FromSeconds(4.0);

		private const int WarmodeCatchCount = 4;
		// Allow four warmode changes in 0.5 seconds, any more will be delay for two seconds

		[CommandProperty(AccessLevel.Decorator)]
		public Race Race
		{
			get
			{
				if (m_Race == null)
				{
					m_Race = Race.DefaultRace;
				}

				return m_Race;
			}
			set
			{
				var oldRace = Race;

				m_Race = value;

				if (m_Race == null)
				{
					m_Race = Race.DefaultRace;
				}

				Body = m_Race.Body(this);
				UpdateResistances();

				Delta(MobileDelta.Race);

				OnRaceChange(oldRace);
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool CharacterOut { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool PublicHouseContent { get; set; }

		public DFAlgorithm DFA { get; set; }

		protected virtual void OnRaceChange(Race oldRace)
		{ }

		public virtual double RacialSkillBonus => 0;

		public virtual double GetRacialSkillBonus(SkillName skill)
		{
			return RacialSkillBonus;
		}

		public virtual void MutateSkill(SkillName skill, ref double value)
		{ }

		private List<ResistanceMod> m_ResistMods;

		private int[] m_Resistances;

		protected List<string> m_SlayerVulnerabilities = new List<string>();
		protected bool m_SpecialSlayerMechanics; // false

		public List<string> SlayerVulnerabilities => m_SlayerVulnerabilities;

		[CommandProperty(AccessLevel.Decorator)]
		public bool SpecialSlayerMechanics => m_SpecialSlayerMechanics;

		public int[] Resistances => m_Resistances;

		public virtual int BasePhysicalResistance => 0;
		public virtual int BaseFireResistance => 0;
		public virtual int BaseColdResistance => 0;
		public virtual int BasePoisonResistance => 0;
		public virtual int BaseEnergyResistance => 0;

		public virtual void ComputeLightLevels(out int global, out int personal)
		{
			ComputeBaseLightLevels(out global, out personal);

			if (m_Region != null)
			{
				m_Region.AlterLightLevel(this, ref global, ref personal);
			}
		}

		public virtual void ComputeBaseLightLevels(out int global, out int personal)
		{
			global = 0;
			personal = m_LightLevel;
		}

		public virtual void CheckLightLevels(bool forceResend)
		{ }

		[CommandProperty(AccessLevel.Counselor)]
		public virtual int PhysicalResistance => GetResistance(ResistanceType.Physical);

		[CommandProperty(AccessLevel.Counselor)]
		public virtual int FireResistance => GetResistance(ResistanceType.Fire);

		[CommandProperty(AccessLevel.Counselor)]
		public virtual int ColdResistance => GetResistance(ResistanceType.Cold);

		[CommandProperty(AccessLevel.Counselor)]
		public virtual int PoisonResistance => GetResistance(ResistanceType.Poison);

		[CommandProperty(AccessLevel.Counselor)]
		public virtual int EnergyResistance => GetResistance(ResistanceType.Energy);

		public virtual void UpdateResistances()
		{
			if (m_Resistances == null)
			{
				m_Resistances = new[] { Int32.MinValue, Int32.MinValue, Int32.MinValue, Int32.MinValue, Int32.MinValue };
			}

			var delta = false;

			for (var i = 0; i < m_Resistances.Length; ++i)
			{
				if (m_Resistances[i] != Int32.MinValue)
				{
					m_Resistances[i] = Int32.MinValue;
					delta = true;
				}
			}

			if (delta)
			{
				Delta(MobileDelta.Resistances);
				ProcessDelta();
			}
		}

		public virtual int GetResistance(ResistanceType type)
		{
			if (m_Resistances == null)
			{
				m_Resistances = new[] { Int32.MinValue, Int32.MinValue, Int32.MinValue, Int32.MinValue, Int32.MinValue };
			}

			var v = (int)type;

			if (v < 0 || v >= m_Resistances.Length)
			{
				return 0;
			}

			var res = m_Resistances[v];

			if (res == Int32.MinValue)
			{
				ComputeResistances();
				res = m_Resistances[v];
			}

			return res;
		}

		public List<ResistanceMod> ResistanceMods { get => m_ResistMods; set => m_ResistMods = value; }

		public virtual void AddResistanceMod(ResistanceMod toAdd)
		{
			if (m_ResistMods == null)
			{
				m_ResistMods = new List<ResistanceMod>();
			}

			m_ResistMods.Add(toAdd);
			UpdateResistances();
		}

		public virtual void RemoveResistanceMod(ResistanceMod toRemove)
		{
			if (m_ResistMods != null)
			{
				m_ResistMods.Remove(toRemove);

				if (m_ResistMods.Count == 0)
				{
					m_ResistMods = null;
				}
			}

			UpdateResistances();
		}

		private static int m_MinPlayerResistance = -70;

		public static int MinPlayerResistance { get => m_MinPlayerResistance; set => m_MinPlayerResistance = value; }

		private static int m_MaxPlayerResistance = 70;

		public static int MaxPlayerResistance { get => m_MaxPlayerResistance; set => m_MaxPlayerResistance = value; }

		public virtual void ComputeResistances()
		{
			if (m_Resistances == null)
			{
				m_Resistances = new[] { Int32.MinValue, Int32.MinValue, Int32.MinValue, Int32.MinValue, Int32.MinValue };
			}

			for (var i = 0; i < m_Resistances.Length; ++i)
			{
				m_Resistances[i] = 0;
			}

			m_Resistances[0] += BasePhysicalResistance;
			m_Resistances[1] += BaseFireResistance;
			m_Resistances[2] += BaseColdResistance;
			m_Resistances[3] += BasePoisonResistance;
			m_Resistances[4] += BaseEnergyResistance;

			for (var i = 0; m_ResistMods != null && i < m_ResistMods.Count; ++i)
			{
				var mod = m_ResistMods[i];
				var v = (int)mod.Type;

				if (v >= 0 && v < m_Resistances.Length)
				{
					m_Resistances[v] += mod.Offset;
				}
			}

			for (var i = 0; i < m_Items.Count; ++i)
			{
				var item = m_Items[i];

				if (item.CheckPropertyConfliction(this))
				{
					continue;
				}

				m_Resistances[0] += item.PhysicalResistance;
				m_Resistances[1] += item.FireResistance;
				m_Resistances[2] += item.ColdResistance;
				m_Resistances[3] += item.PoisonResistance;
				m_Resistances[4] += item.EnergyResistance;
			}

			for (var i = 0; i < m_Resistances.Length; ++i)
			{
				var min = GetMinResistance((ResistanceType)i);
				var max = GetMaxResistance((ResistanceType)i);

				if (max < min)
				{
					max = min;
				}

				if (m_Resistances[i] > max)
				{
					m_Resistances[i] = max;
				}
				else if (m_Resistances[i] < min)
				{
					m_Resistances[i] = min;
				}
			}
		}

		public virtual int GetMinResistance(ResistanceType type)
		{
			if (m_Player)
			{
				return m_MinPlayerResistance;
			}

			return -100;
		}

		public virtual int GetMaxResistance(ResistanceType type)
		{
			if (m_Player)
			{
				return m_MaxPlayerResistance;
			}

			return 100;
		}

		public int GetAOSStatus(int index)
		{
			return (m_AOSStatusHandler == null) ? 0 : m_AOSStatusHandler(this, index);
		}

		public virtual void SendPropertiesTo(Mobile from)
		{
			from.Send(PropertyList);
		}

		public virtual void OnAosSingleClick(Mobile from)
		{
			var opl = PropertyList;

			if (opl.Header > 0)
			{
				int hue;

				if (m_NameHue != -1)
				{
					hue = m_NameHue;
				}
				else if (IsStaff())
				{
					hue = 11;
				}
				else
				{
					hue = Notoriety.GetHue(Notoriety.Compute(from, this));
				}

				from.Send(new MessageLocalized(m_Serial, Body, MessageType.Label, hue, 3, opl.Header, Name, opl.HeaderArgs));
			}
		}

		public virtual string ApplyNameSuffix(string suffix)
		{
			return suffix;
		}

		public virtual void AddNameProperties(ObjectPropertyList list)
		{
			var name = Name;

			if (name == null)
			{
				name = String.Empty;
			}

			var prefix = ""; // still needs to be defined due to cliloc. Only defined in PlayerMobile. BaseCreature and BaseVendor require the suffix for the title and use the same cliloc.

			var suffix = "";

			if (PropertyTitle && !String.IsNullOrEmpty(Title))
			{
				suffix = Title;
			}

			suffix = ApplyNameSuffix(suffix);

			list.Add(1050045, "{0} \t{1}\t {2}", prefix, name, suffix); // ~1_PREFIX~~2_NAME~~3_SUFFIX~           
		}

		public virtual void GetProperties(ObjectPropertyList list)
		{
			AddNameProperties(list);

			if (Spawner != null)
			{
				Spawner.GetSpawnProperties(this, list);
			}
		}

		public virtual void GetChildProperties(ObjectPropertyList list, Item item)
		{ }

		public virtual void GetChildNameProperties(ObjectPropertyList list, Item item)
		{ }

		public List<Mobile> Stabled => m_Stabled;

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public VirtueInfo Virtues { get => m_Virtues; set { } }

		public object Party { get => m_Party; set => m_Party = value; }
		public List<SkillMod> SkillMods => m_SkillMods;

		/// <summary>
		///     Overridable. Virtual event invoked when <paramref name="skill" /> changes in some way.
		/// </summary>
		public virtual void OnSkillInvalidated(Skill skill)
		{ }

		public virtual void UpdateSkillMods()
		{
			ValidateSkillMods();

			for (var i = 0; i < m_SkillMods.Count; ++i)
			{
				var mod = m_SkillMods[i];

				var sk = m_Skills[mod.Skill];

				if (sk != null)
				{
					sk.Update();
				}
			}
		}

		public virtual void ValidateSkillMods()
		{
			for (var i = 0; i < m_SkillMods.Count;)
			{
				var mod = m_SkillMods[i];

				if (mod.CheckCondition())
				{
					++i;
				}
				else
				{
					InternalRemoveSkillMod(mod);
				}
			}
		}

		public virtual void AddSkillMod(SkillMod mod)
		{
			if (mod == null)
			{
				return;
			}

			ValidateSkillMods();

			if (!m_SkillMods.Contains(mod))
			{
				m_SkillMods.Add(mod);
				mod.Owner = this;

				var sk = m_Skills[mod.Skill];

				if (sk != null)
				{
					sk.Update();
				}
			}
		}

		public virtual void RemoveSkillMod(SkillMod mod)
		{
			if (mod == null)
			{
				return;
			}

			ValidateSkillMods();

			InternalRemoveSkillMod(mod);
		}

		private void InternalRemoveSkillMod(SkillMod mod)
		{
			if (m_SkillMods.Contains(mod))
			{
				m_SkillMods.Remove(mod);
				mod.Owner = null;

				var sk = m_Skills[mod.Skill];

				if (sk != null)
				{
					sk.Update();
				}
			}
		}

		private class WarmodeTimer : Timer
		{
			private readonly Mobile m_Mobile;
			private bool m_Value;

			public bool Value { get => m_Value; set => m_Value = value; }

			public WarmodeTimer(Mobile m, bool value)
				: base(WarmodeSpamDelay)
			{
				m_Mobile = m;
				m_Value = value;
			}

			protected override void OnTick()
			{
				m_Mobile.Warmode = m_Value;
				m_Mobile.m_WarmodeChanges = 0;

				m_Mobile.m_WarmodeTimer = null;
			}
		}

		/// <summary>
		///     Overridable. Virtual event invoked when a client, <paramref name="from" />, invokes a 'help request' for the Mobile. Seemingly no longer functional in newer clients.
		/// </summary>
		public virtual void OnHelpRequest(Mobile from)
		{ }

		public void DelayChangeWarmode(bool value)
		{
			if (m_WarmodeTimer != null)
			{
				m_WarmodeTimer.Value = value;
				return;
			}

			if (m_Warmode == value)
			{
				return;
			}

			DateTime now = DateTime.UtcNow, next = m_NextWarmodeChange;

			if (now > next || m_WarmodeChanges == 0)
			{
				m_WarmodeChanges = 1;
				m_NextWarmodeChange = now + WarmodeSpamCatch;
			}
			else if (m_WarmodeChanges == WarmodeCatchCount)
			{
				m_WarmodeTimer = new WarmodeTimer(this, value);
				m_WarmodeTimer.Start();

				return;
			}
			else
			{
				++m_WarmodeChanges;
			}

			Warmode = value;
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int MeleeDamageAbsorb { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int MagicDamageAbsorb { get => m_MagicDamageAbsorb; set => m_MagicDamageAbsorb = value; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int SkillsTotal => m_Skills == null ? 0 : m_Skills.Total;

		[CommandProperty(AccessLevel.GameMaster)]
		public int SkillsCap
		{
			get => m_Skills == null ? 0 : m_Skills.Cap;
			set
			{
				if (m_Skills != null)
				{
					m_Skills.Cap = value;
				}
			}
		}

		public bool InLOS(Mobile target)
		{
			if (m_Deleted || m_Map == null)
			{
				return false;
			}

			if (target == this || IsStaff())
			{
				return true;
			}

			return m_Map.LineOfSight(this, target);
		}

		public bool InLOS(object target)
		{
			if (m_Deleted || m_Map == null)
			{
				return false;
			}

			if (target == this || IsStaff())
			{
				return true;
			}

			if (target is Item item)
			{
				if (item.RootParent == this)
				{
					return true;
				}

				if (item.Parent is Container)
				{
					return InLOS(item.Parent);
				}
			}

			return m_Map.LineOfSight(this, target);
		}

		public bool InLOS(Point3D target)
		{
			if (m_Deleted || m_Map == null)
			{
				return false;
			}

			if (IsStaff())
			{
				return true;
			}

			return m_Map.LineOfSight(this, target);
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int BaseSoundID { get => m_BaseSoundID; set => m_BaseSoundID = value; }

		public bool BeginAction(object toLock)
		{
			if (_actions == null)
			{
				_actions = new List<object>
				{
					toLock
				};

				return true;
			}

			if (!_actions.Contains(toLock))
			{
				_actions.Add(toLock);

				return true;
			}

			return false;
		}

		public bool CanBeginAction(object toLock)
		{
			return _actions == null || !_actions.Contains(toLock);
		}

		public void EndAction(object toLock)
		{
			if (_actions != null)
			{
				_actions.Remove(toLock);

				if (_actions.Count == 0)
				{
					_actions = null;
				}
			}
		}

		[CommandProperty(AccessLevel.Decorator)]
		public int NameHue { get => m_NameHue; set => m_NameHue = value; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Hunger
		{
			get => m_Hunger;
			set
			{
				var oldValue = m_Hunger;

				if (oldValue != value)
				{
					m_Hunger = value;

					EventSink.InvokeHungerChanged(new HungerChangedEventArgs(this, oldValue));
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int Thirst { get => m_Thirst; set => m_Thirst = value; }

		[CommandProperty(AccessLevel.Decorator)]
		public int BAC { get => m_BAC; set => m_BAC = value; }

		public virtual int DefaultBloodHue => 0;

		public virtual bool HasBlood => Alive && BloodHue >= 0 && !Body.IsGhost && !Body.IsEquipment;

		private int m_BloodHue = -1;

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual int BloodHue
		{
			get
			{
				if (m_BloodHue < 0)
				{
					return DefaultBloodHue;
				}

				return m_BloodHue;
			}
			set => m_BloodHue = value;
		}

		private long m_LastMoveTime;

		/// <summary>
		///     Gets or sets the number of steps this player may take when hidden before being revealed.
		/// </summary>
		[CommandProperty(AccessLevel.GameMaster)]
		public int AllowedStealthSteps { get => m_AllowedStealthSteps; set => m_AllowedStealthSteps = value; }

		private StatLockType m_StrLock, m_DexLock, m_IntLock;

		private Item m_Holding;

		public Item Holding
		{
			get => m_Holding;
			set
			{
				if (m_Holding != value)
				{
					if (m_Holding != null)
					{
						UpdateTotal(m_Holding, TotalType.Weight, -(m_Holding.TotalWeight + m_Holding.PileWeight));

						if (m_Holding.HeldBy == this)
						{
							m_Holding.HeldBy = null;
						}
					}

					if (value != null && m_Holding != null)
					{
						DropHolding();
					}

					m_Holding = value;

					if (m_Holding != null)
					{
						UpdateTotal(m_Holding, TotalType.Weight, m_Holding.TotalWeight + m_Holding.PileWeight);

						if (m_Holding.HeldBy == null)
						{
							m_Holding.HeldBy = this;
						}
					}
				}
			}
		}

		public long LastMoveTime { get => m_LastMoveTime; set => m_LastMoveTime = value; }

		private static readonly string _ParaTimerID = "ParalyzeTimer";
		private static readonly string _FrozenTimerID = "FreezeTimer";

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual bool Paralyzed
		{
			get => m_Paralyzed;
			set
			{
				if (m_Paralyzed != value)
				{
					m_Paralyzed = value;
					Delta(MobileDelta.Flags);

					SendLocalizedMessage(m_Paralyzed ? 502381 : 502382);

					TimerRegistry.RemoveFromRegistry(_ParaTimerID, this);
				}
			}
		}

		[CommandProperty(AccessLevel.Decorator)]
		public bool Frozen
		{
			get => m_Frozen;
			set
			{
				if (m_Frozen != value)
				{
					m_Frozen = value;
					Delta(MobileDelta.Flags);

					TimerRegistry.RemoveFromRegistry(_FrozenTimerID, this);
				}
			}
		}

		public void Paralyze(TimeSpan duration)
		{
			if (!m_Paralyzed)
			{
				Paralyzed = true;

				TimerRegistry.Register(_ParaTimerID, this, duration, m => m.Paralyzed = false);
			}
		}

		public void Freeze(TimeSpan duration)
		{
			if (!m_Frozen)
			{
				Frozen = true;

				TimerRegistry.Register(_FrozenTimerID, this, duration, m => m.Frozen = false);
			}
		}

		/// <summary>
		///     Gets or sets the <see cref="StatLockType">lock state</see> for the <see cref="RawStr" /> property.
		/// </summary>
		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public StatLockType StrLock
		{
			get => m_StrLock;
			set
			{
				if (m_StrLock != value)
				{
					m_StrLock = value;

					if (m_NetState != null)
					{
						m_NetState.Send(new StatLockInfo(this));
					}
				}
			}
		}

		/// <summary>
		///     Gets or sets the <see cref="StatLockType">lock state</see> for the <see cref="RawDex" /> property.
		/// </summary>
		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public StatLockType DexLock
		{
			get => m_DexLock;
			set
			{
				if (m_DexLock != value)
				{
					m_DexLock = value;

					if (m_NetState != null)
					{
						m_NetState.Send(new StatLockInfo(this));
					}
				}
			}
		}

		/// <summary>
		///     Gets or sets the <see cref="StatLockType">lock state</see> for the <see cref="RawInt" /> property.
		/// </summary>
		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public StatLockType IntLock
		{
			get => m_IntLock;
			set
			{
				if (m_IntLock != value)
				{
					m_IntLock = value;

					if (m_NetState != null)
					{
						m_NetState.Send(new StatLockInfo(this));
					}
				}
			}
		}

		public override string ToString()
		{
			return String.Format("0x{0:X} \"{1}\"", m_Serial.Value, Name);
		}

		public long NextActionTime { get; set; }

		public long NextActionMessage { get => m_NextActionMessage; set => m_NextActionMessage = value; }

		private static int m_ActionMessageDelay = 125;

		public static int ActionMessageDelay { get => m_ActionMessageDelay; set => m_ActionMessageDelay = value; }

		public virtual void SendSkillMessage()
		{
			if (m_NextActionMessage - Core.TickCount >= 0)
			{
				return;
			}

			m_NextActionMessage = Core.TickCount + m_ActionMessageDelay;

			SendLocalizedMessage(500118); // You must wait a few moments to use another skill.
		}

		public virtual void SendActionMessage()
		{
			if (m_NextActionMessage - Core.TickCount >= 0)
			{
				return;
			}

			m_NextActionMessage = Core.TickCount + m_ActionMessageDelay;

			SendLocalizedMessage(500119); // You must wait to perform another action.
		}

		public virtual void ClearHands()
		{
			ClearHand(FindItemOnLayer(Layer.OneHanded));
			ClearHand(FindItemOnLayer(Layer.TwoHanded));
		}

		public virtual void ClearHand(Item item)
		{
			if (item != null && item.Movable && !item.AllowEquipedCast(this))
			{
				var pack = Backpack;

				if (pack == null)
				{
					AddToBackpack(item);
				}
				else
				{
					pack.DropItem(item);
				}
			}
		}

		#region Timers

		#region Regeneration
		private static bool m_GlobalRegenThroughPoison = true;
		public static bool GlobalRegenThroughPoison { get => m_GlobalRegenThroughPoison; set => m_GlobalRegenThroughPoison = value; }

		public static readonly string _HitsRegenTimerID = "HitsRegenTimer";
		public static readonly string _StamRegenTimerID = "StamRegenTimer";
		public static readonly string _ManaRegenTimerID = "ManaRegenTimer";
		public static readonly string _HitsRegenTimerPlayerID = _HitsRegenTimerID + "Player";
		public static readonly string _StamRegenTimerPlayerID = _StamRegenTimerID + "Player";
		public static readonly string _ManaRegenTimerPlayerID = _ManaRegenTimerID + "Player";

		private bool m_InternalCanRegen;

		public virtual bool RegenThroughPoison => m_GlobalRegenThroughPoison;

		public virtual bool CanRegenHits => Alive && !Deleted && (RegenThroughPoison || !Poisoned) && m_InternalCanRegen;
		public virtual bool CanRegenStam => Alive && !Deleted && m_InternalCanRegen;
		public virtual bool CanRegenMana => Alive && !Deleted && m_InternalCanRegen;

		private void HitsOnTick()
		{
			if (CanRegenHits)
			{
				Hits++;

				if (Hits < HitsMax)
				{
					TimerRegistry.UpdateRegistry(Player ? _HitsRegenTimerPlayerID : _HitsRegenTimerID, this, GetHitsRegenRate(this));
				}
			}
		}

		private void StamOnTick()
		{
			if (CanRegenMana)
			{
				Stam++;

				if (Stam < StamMax)
				{
					TimerRegistry.UpdateRegistry(Player ? _StamRegenTimerPlayerID : _StamRegenTimerID, this, GetStamRegenRate(this));
				}
			}
		}

		private void ManaOnTick()
		{
			if (CanRegenMana)
			{
				Mana++;

				if (Mana < ManaMax)
				{
					TimerRegistry.UpdateRegistry(Player ? _ManaRegenTimerPlayerID : _ManaRegenTimerID, this, GetManaRegenRate(this));
				}
			}
		}
		#endregion

		#region Aggro Timer
		private static readonly string _ExpireAggroTimerID = "ExpireAggroTimer";

		public void UpdateAggrExpire()
		{
			if (m_Deleted || (m_Aggressors.Count == 0 && m_Aggressed.Count == 0))
			{
				StopAggrExpire();
			}
			else if (!TimerRegistry.HasTimer(_ExpireAggroTimerID, this))
			{
				TimerRegistry.Register(_ExpireAggroTimerID, this, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5), false, TimerPriority.OneSecond, m => m.AggroExpireOnTick());
			}
		}

		private void AggroExpireOnTick()
		{
			if (Deleted || (Aggressors.Count == 0 && Aggressed.Count == 0))
			{
				StopAggrExpire();
			}
			else
			{
				CheckAggrExpire();
			}
		}

		private void StopAggrExpire()
		{
			TimerRegistry.RemoveFromRegistry(_ExpireAggroTimerID, this);
		}

		private void CheckAggrExpire()
		{
			for (var i = m_Aggressors.Count - 1; i >= 0; --i)
			{
				if (i >= m_Aggressors.Count)
				{
					continue;
				}

				var info = m_Aggressors[i];

				if (info.Expired)
				{
					var attacker = info.Attacker;
					attacker.RemoveAggressed(this);

					m_Aggressors.RemoveAt(i);
					info.Free();

					if (m_NetState != null && Utility.InUpdateRange(this, attacker) && CanSee(attacker))
					{
						m_NetState.Send(MobileIncoming.Create(m_NetState, this, attacker));
					}
				}
			}

			for (var i = m_Aggressed.Count - 1; i >= 0; --i)
			{
				if (i >= m_Aggressed.Count)
				{
					continue;
				}

				var info = m_Aggressed[i];

				if (info.Expired)
				{
					var defender = info.Defender;
					defender.RemoveAggressor(this);

					m_Aggressed.RemoveAt(i);
					info.Free();

					if (m_NetState != null && Utility.InUpdateRange(this, defender) && CanSee(defender))
					{
						m_NetState.Send(MobileIncoming.Create(m_NetState, this, defender));
					}
				}
			}

			UpdateAggrExpire();
		}
		#endregion

		#region Expire Combatant Timer
		private static readonly string _ExpireCombatantTimerID = "ExpireCombatantTimer";

		private void StartExpireCombatantTimer()
		{
			TimerRegistry.Register(_ExpireCombatantTimerID, this, TimeSpan.FromMinutes(1), false, TimerPriority.FiveSeconds, m => m.CheckExpireCombatant());
		}

		private void RemoveCombatantTimer()
		{
			TimerRegistry.RemoveFromRegistry(_ExpireCombatantTimerID, this);
		}

		private void CheckExpireCombatantTimer()
		{
			if (!TimerRegistry.UpdateRegistry(_ExpireCombatantTimerID, this, TimeSpan.FromMinutes(1)))
			{
				StartExpireCombatantTimer();
			}
		}

		private void CheckExpireCombatant()
		{
			Combatant = null;
		}
		#endregion

		#region Logout
		/* Logout:
        *
        * When a client logs into mobile x
        *  - if ( x is Internalized ) move x to logout location and map
        *
        * When a client attached to a mobile disconnects
        *  - LogoutTimer is started
        *	   - Delay is taken from Region.GetLogoutDelay to allow insta-logout regions.
        *     - OnTick : Location and map are stored, and mobile is internalized
        *
        * Some things to consider:
        *  - An internalized person getting killed (say, by poison). Where does the body go?
        *  - Regions now have a GetLogoutDelay( Mobile m ); virtual function (see above)
        */
		private Point3D m_LogoutLocation;
		private Map m_LogoutMap;

		private static readonly string _LogoutTimerID = "LogoutTimer";

		public virtual TimeSpan GetLogoutDelay()
		{
			return Region.GetLogoutDelay(this);
		}

		private void DoLogout()
		{
			if (m_Map != Map.Internal)
			{
				EventSink.InvokeLogout(new LogoutEventArgs(this));

				m_LogoutLocation = m_Location;
				m_LogoutMap = m_Map;

				Internalize();
			}
		}
		#endregion

		#region Combat Timer
		private static readonly string _CombatTimerPlayerID = "CombatTimerPlayer";
		private static readonly string _CombatTimerID = "CombatTimer";

		private long m_NextCombatTime;
		public long NextCombatTime { get => m_NextCombatTime; set => m_NextCombatTime = value; }

		private bool UsePlayerCombatTimer()
		{
			return Player || m_Dex > 100;
		}

		private void StartCombatTimer()
		{
			var playerTimer = UsePlayerCombatTimer();

			TimerRegistry.Register(
				playerTimer ? _CombatTimerPlayerID : _CombatTimerID,
				this,
				TimeSpan.FromSeconds(0.01),
				false,
				playerTimer ? TimerPriority.EveryTick : TimerPriority.FiftyMS,
				m => m.CombatTimerOnTick());
		}

		private void RemoveCombatTimer()
		{
			TimerRegistry.RemoveFromRegistry(UsePlayerCombatTimer() ? _CombatTimerPlayerID : _CombatTimerID, this);
		}

		private void CombatTimerOnTick()
		{
			if (Core.TickCount - m_NextCombatTime >= 0)
			{
				var combatant = Combatant;

				// If no combatant, wrong map, one of us is a ghost, or cannot see, or deleted, then stop combat
				if (combatant == null || combatant.Deleted || m_Deleted || combatant.Map != m_Map ||
					!combatant.Alive || !Alive || !CanSee(combatant) || combatant is Mobile mobile && mobile.IsDeadBondedPet ||
					IsDeadBondedPet)
				{
					Combatant = null;
					return;
				}

				var weapon = Weapon;

				if (!InRange(combatant, weapon.MaxRange))
				{
					return;
				}

				if (InLOS(combatant))
				{
					weapon.OnBeforeSwing(this, combatant);
					RevealingAction();
					m_NextCombatTime = Core.TickCount + (int)weapon.OnSwing(this, combatant).TotalMilliseconds;
				}
			}
		}
		#endregion

		#region Expire Crimimnal
		private static readonly string _ExpireCrimID = "ExpireCriminalTimer";
		private static TimeSpan _ExpireCriminalDelay = TimeSpan.FromMinutes(2.0);

		public static TimeSpan ExpireCriminalDelay { get => _ExpireCriminalDelay; set => _ExpireCriminalDelay = value; }

		private void StartCrimDelayTimer()
		{
			if (!TimerRegistry.UpdateRegistry(_ExpireCrimID, this, _ExpireCriminalDelay))
			{
				TimerRegistry.Register(_ExpireCrimID, this, _ExpireCriminalDelay, TimerPriority.FiveSeconds, m => m.Criminal = false);
			}
		}

		private void StopCrimDelayTimer()
		{
			TimerRegistry.RemoveFromRegistry(_ExpireCrimID, this);
		}
		#endregion
		#endregion

		[CommandProperty(AccessLevel.GameMaster)]
		public long NextSkillTime { get => m_NextSkillTime; set => m_NextSkillTime = value; }

		public List<AggressorInfo> Aggressors => m_Aggressors;

		public List<AggressorInfo> Aggressed => m_Aggressed;

		private int m_ChangingCombatant;

		public bool ChangingCombatant => m_ChangingCombatant > 0;

		public virtual void Attack(IDamageable e)
		{
			if (CheckAttack(e))
			{
				if (!m_Warmode)
				{
					Warmode = true;
				}

				Combatant = e;
			}
		}

		public virtual bool CheckAttack(IDamageable e)
		{
			return Utility.InUpdateRange(this, e.Location) && CanSee(e) && InLOS(e);
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool GuardImmune { get; set; }

		/// <summary>
		///     Overridable. Gets or sets which Mobile that this Mobile is currently engaged in combat with.
		///     <seealso cref="OnCombatantChange" />
		/// </summary>
		[CommandProperty(AccessLevel.GameMaster)]
		public virtual IDamageable Combatant
		{
			get => m_Combatant;
			set
			{
				if (m_Deleted)
				{
					return;
				}

				if (m_Combatant != value && value != this)
				{
					if (++m_ChangingCombatant > 100)
					{
						m_ChangingCombatant = 0;
						return;
					}

					var old = m_Combatant;

					m_Combatant = value;

					if (!Region.OnCombatantChange(this, old, m_Combatant) || (m_Combatant != null && !CanBeHarmful(m_Combatant, false)))
					{
						m_Combatant = old;
						--m_ChangingCombatant;
						return;
					}

					if (m_NetState != null)
					{
						m_NetState.Send(new ChangeCombatant(m_Combatant));
					}

					if (m_Combatant == null)
					{
						RemoveCombatantTimer();
						RemoveCombatTimer();
					}
					else
					{
						CheckExpireCombatantTimer();
						StartCombatTimer();
					}

					if (m_Combatant != null && CanBeHarmful(m_Combatant, false))
					{
						DoHarmful(m_Combatant);

						if (m_Combatant is Mobile mobile)
						{
							mobile.PlaySound(mobile.GetAngerSound());
						}
					}

					OnCombatantChange();
					--m_ChangingCombatant;
				}
			}
		}

		/// <summary>
		///     Overridable. Virtual event invoked after the <see cref="Combatant" /> property has changed.
		///     <seealso cref="Combatant" />
		/// </summary>
		public virtual void OnCombatantChange()
		{ }

		public double GetDistanceToSqrt(Point3D p)
		{
			var xDelta = m_Location.m_X - p.m_X;
			var yDelta = m_Location.m_Y - p.m_Y;

			return Math.Sqrt((xDelta * xDelta) + (yDelta * yDelta));
		}

		public double GetDistanceToSqrt(Mobile m)
		{
			var xDelta = m_Location.m_X - m.m_Location.m_X;
			var yDelta = m_Location.m_Y - m.m_Location.m_Y;

			return Math.Sqrt((xDelta * xDelta) + (yDelta * yDelta));
		}

		public double GetDistanceToSqrt(IPoint2D p)
		{
			var xDelta = m_Location.m_X - p.X;
			var yDelta = m_Location.m_Y - p.Y;

			return Math.Sqrt((xDelta * xDelta) + (yDelta * yDelta));
		}

		public virtual void AggressiveAction(Mobile aggressor)
		{
			AggressiveAction(aggressor, false);
		}

		public virtual void AggressiveAction(Mobile aggressor, bool criminal)
		{
			if (aggressor == this)
				return;

			var args = AggressiveActionEventArgs.Create(this, aggressor, criminal);

			EventSink.InvokeAggressiveAction(args);

			args.Free();

			if (Combatant == aggressor)
			{
				CheckExpireCombatantTimer();
			}

			var addAggressor = true;

			var list = m_Aggressors;

			for (var i = 0; i < list.Count; ++i)
			{
				var info = list[i];

				if (info.Attacker == aggressor)
				{
					info.Refresh();
					info.CriminalAggression = criminal;
					info.CanReportMurder = criminal;

					addAggressor = false;
				}
			}

			list = aggressor.m_Aggressors;

			for (var i = 0; i < list.Count; ++i)
			{
				var info = list[i];

				if (info.Attacker == this)
				{
					info.Refresh();

					addAggressor = false;
				}
			}

			var addAggressed = true;

			list = m_Aggressed;

			for (var i = 0; i < list.Count; ++i)
			{
				var info = list[i];

				if (info.Defender == aggressor)
				{
					info.Refresh();

					addAggressed = false;
				}
			}

			list = aggressor.m_Aggressed;

			for (var i = 0; i < list.Count; ++i)
			{
				var info = list[i];

				if (info.Defender == this)
				{
					info.Refresh();
					info.CriminalAggression = criminal;
					info.CanReportMurder = criminal;

					addAggressed = false;
				}
			}

			var setCombatant = false;

			if (addAggressor)
			{
				m_Aggressors.Add(AggressorInfo.Create(aggressor, this, criminal));

				if (CanSee(aggressor) && m_NetState != null)
				{
					m_NetState.Send(MobileIncoming.Create(m_NetState, this, aggressor));
				}

				if (Combatant == null)
					setCombatant = true;

				UpdateAggrExpire();
			}

			if (addAggressed)
			{
				aggressor.m_Aggressed.Add(AggressorInfo.Create(aggressor, this, criminal));

				if (CanSee(aggressor) && m_NetState != null)
				{
					m_NetState.Send(MobileIncoming.Create(m_NetState, this, aggressor));
				}

				if (Combatant == null)
					setCombatant = true;

				UpdateAggrExpire();
			}

			if (setCombatant && !Hidden)
				Combatant = aggressor;

			Region.OnAggressed(aggressor, this, criminal);
		}

		public void RemoveAggressed(Mobile aggressed)
		{
			if (m_Deleted)
			{
				return;
			}

			var list = m_Aggressed;

			for (var i = 0; i < list.Count; ++i)
			{
				var info = list[i];

				if (info.Defender == aggressed)
				{
					m_Aggressed.RemoveAt(i);
					info.Free();

					if (m_NetState != null && CanSee(aggressed))
					{
						m_NetState.Send(MobileIncoming.Create(m_NetState, this, aggressed));
					}

					break;
				}
			}

			UpdateAggrExpire();
		}

		public void RemoveAggressor(Mobile aggressor)
		{
			if (m_Deleted)
			{
				return;
			}

			var list = m_Aggressors;

			for (var i = 0; i < list.Count; ++i)
			{
				var info = list[i];

				if (info.Attacker == aggressor)
				{
					m_Aggressors.RemoveAt(i);
					info.Free();

					if (m_NetState != null && CanSee(aggressor))
					{
						m_NetState.Send(MobileIncoming.Create(m_NetState, this, aggressor));
					}

					break;
				}
			}

			UpdateAggrExpire();
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int TotalGold => GetTotal(TotalType.Gold);

		[CommandProperty(AccessLevel.GameMaster)]
		public int TotalItems => GetTotal(TotalType.Items);

		[CommandProperty(AccessLevel.GameMaster)]
		public int TotalWeight => GetTotal(TotalType.Weight);

		[CommandProperty(AccessLevel.GameMaster)]
		public int TithingPoints
		{
			get => m_TithingPoints;
			set
			{
				if (m_TithingPoints != value)
				{
					m_TithingPoints = value;

					Delta(MobileDelta.TithingPoints);
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int Followers
		{
			get => m_Followers;
			set
			{
				if (m_Followers != value)
				{
					m_Followers = value;

					Delta(MobileDelta.Followers);
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int FollowersMax
		{
			get => m_FollowersMax;
			set
			{
				if (m_FollowersMax != value)
				{
					m_FollowersMax = value;

					Delta(MobileDelta.Followers);
				}
			}
		}

		public virtual int GetTotal(TotalType type)
		{
			switch (type)
			{
				case TotalType.Gold:
				return m_TotalGold;

				case TotalType.Items:
				return m_TotalItems;

				case TotalType.Weight:
				return m_TotalWeight;
			}

			return 0;
		}

		public virtual void UpdateTotal(Item sender, TotalType type, int delta)
		{
			if (delta == 0 || sender.IsVirtualItem)
			{
				return;
			}

			switch (type)
			{
				case TotalType.Gold:
				m_TotalGold += delta;
				Delta(MobileDelta.Gold);
				break;

				case TotalType.Items:
				m_TotalItems += delta;
				break;

				case TotalType.Weight:
				m_TotalWeight += delta;
				Delta(MobileDelta.Weight);
				OnWeightChange(m_TotalWeight - delta);
				break;
			}
		}

		public virtual void UpdateTotals()
		{
			if (m_Items == null)
			{
				return;
			}

			var oldWeight = m_TotalWeight;

			m_TotalGold = 0;
			m_TotalItems = 0;
			m_TotalWeight = 0;

			for (var i = 0; i < m_Items.Count; ++i)
			{
				var item = m_Items[i];

				item.UpdateTotals();

				if (item.IsVirtualItem)
				{
					continue;
				}

				m_TotalGold += item.TotalGold;
				m_TotalItems += item.TotalItems + 1;
				m_TotalWeight += item.TotalWeight + item.PileWeight;
			}

			if (m_Holding != null)
			{
				m_TotalWeight += m_Holding.TotalWeight + m_Holding.PileWeight;
			}

			if (m_TotalWeight != oldWeight)
			{
				OnWeightChange(oldWeight);
			}
		}

		public void ClearQuestArrow()
		{
			m_QuestArrow = null;
		}

		public void ClearTarget()
		{
			m_Target = null;
		}

		private bool m_TargetLocked;

		public bool TargetLocked { get => m_TargetLocked; set => m_TargetLocked = value; }

		private class SimpleTarget : Target
		{
			private readonly TargetCallback m_Callback;

			public SimpleTarget(int range, TargetFlags flags, bool allowGround, TargetCallback callback)
				: base(range, allowGround, flags)
			{
				m_Callback = callback;
			}

			protected override void OnTarget(Mobile from, object targeted)
			{
				if (m_Callback != null)
				{
					m_Callback(from, targeted);
				}
			}
		}

		public Target BeginTarget(int range, bool allowGround, TargetFlags flags, TargetCallback callback)
		{
			Target t = new SimpleTarget(range, flags, allowGround, callback);

			Target = t;

			return t;
		}

		private class SimpleStateTarget : Target
		{
			private readonly TargetStateCallback m_Callback;
			private readonly object m_State;

			public SimpleStateTarget(int range, TargetFlags flags, bool allowGround, TargetStateCallback callback, object state)
				: base(range, allowGround, flags)
			{
				m_Callback = callback;
				m_State = state;
			}

			protected override void OnTarget(Mobile from, object targeted)
			{
				if (m_Callback != null)
				{
					m_Callback(from, targeted, m_State);
				}
			}
		}

		public Target BeginTarget(int range, bool allowGround, TargetFlags flags, TargetStateCallback callback, object state)
		{
			Target t = new SimpleStateTarget(range, flags, allowGround, callback, state);

			Target = t;

			return t;
		}

		private class SimpleStateTarget<T> : Target
		{
			private readonly TargetStateCallback<T> m_Callback;
			private readonly T m_State;

			public SimpleStateTarget(int range, TargetFlags flags, bool allowGround, TargetStateCallback<T> callback, T state)
				: base(range, allowGround, flags)
			{
				m_Callback = callback;
				m_State = state;
			}

			protected override void OnTarget(Mobile from, object targeted)
			{
				if (m_Callback != null)
				{
					m_Callback(from, targeted, m_State);
				}
			}
		}

		public Target BeginTarget<T>(int range, bool allowGround, TargetFlags flags, TargetStateCallback<T> callback, T state)
		{
			Target t = new SimpleStateTarget<T>(range, flags, allowGround, callback, state);

			Target = t;

			return t;
		}

		public Target Target
		{
			get => m_Target;
			set
			{
				var oldTarget = m_Target;
				var newTarget = value;

				if (oldTarget == newTarget)
				{
					return;
				}

				m_Target = null;

				if (oldTarget != null && newTarget != null)
				{
					oldTarget.Cancel(this, TargetCancelType.Overriden);
				}

				m_Target = newTarget;

				if (newTarget != null && m_NetState != null && !m_TargetLocked)
				{
					m_NetState.Send(newTarget.GetPacketFor(m_NetState));
				}

				OnTargetChange();
			}
		}

		/// <summary>
		///     Overridable. Virtual event invoked after the <see cref="Target">Target property</see> has changed.
		/// </summary>
		protected virtual void OnTargetChange()
		{ }

		public ContextMenu ContextMenu
		{
			get => m_ContextMenu;
			set
			{
				m_ContextMenu = value;

				if (m_ContextMenu != null)
				{
					Send(new DisplayContextMenu(m_ContextMenu));
				}
			}
		}

		public virtual bool CheckContextMenuDisplay(IEntity target)
		{
			return true;
		}

		#region Prompts
		private class SimplePrompt : Prompt
		{
			private readonly PromptCallback m_Callback;
			private readonly PromptCallback m_CancelCallback;
			private readonly bool m_CallbackHandlesCancel;

			public SimplePrompt(PromptCallback callback, PromptCallback cancelCallback)
			{
				m_Callback = callback;
				m_CancelCallback = cancelCallback;
			}

			public SimplePrompt(PromptCallback callback, bool callbackHandlesCancel)
			{
				m_Callback = callback;
				m_CallbackHandlesCancel = callbackHandlesCancel;
			}

			public SimplePrompt(PromptCallback callback)
				: this(callback, false)
			{ }

			public override void OnResponse(Mobile from, string text)
			{
				if (m_Callback != null)
				{
					m_Callback(from, text);
				}
			}

			public override void OnCancel(Mobile from)
			{
				if (m_CallbackHandlesCancel && m_Callback != null)
				{
					m_Callback(from, "");
				}
				else if (m_CancelCallback != null)
				{
					m_CancelCallback(from, "");
				}
			}
		}

		public Prompt BeginPrompt(PromptCallback callback, PromptCallback cancelCallback)
		{
			Prompt p = new SimplePrompt(callback, cancelCallback);

			Prompt = p;
			return p;
		}

		public Prompt BeginPrompt(PromptCallback callback, bool callbackHandlesCancel)
		{
			Prompt p = new SimplePrompt(callback, callbackHandlesCancel);

			Prompt = p;
			return p;
		}

		public Prompt BeginPrompt(PromptCallback callback)
		{
			return BeginPrompt(callback, false);
		}

		private class SimpleStatePrompt : Prompt
		{
			private readonly PromptStateCallback m_Callback;
			private readonly PromptStateCallback m_CancelCallback;

			private readonly bool m_CallbackHandlesCancel;

			private readonly object m_State;

			public SimpleStatePrompt(PromptStateCallback callback, PromptStateCallback cancelCallback, object state)
			{
				m_Callback = callback;
				m_CancelCallback = cancelCallback;
				m_State = state;
			}

			public SimpleStatePrompt(PromptStateCallback callback, bool callbackHandlesCancel, object state)
			{
				m_Callback = callback;
				m_State = state;
				m_CallbackHandlesCancel = callbackHandlesCancel;
			}

			public SimpleStatePrompt(PromptStateCallback callback, object state)
				: this(callback, false, state)
			{ }

			public override void OnResponse(Mobile from, string text)
			{
				if (m_Callback != null)
				{
					m_Callback(from, text, m_State);
				}
			}

			public override void OnCancel(Mobile from)
			{
				if (m_CallbackHandlesCancel && m_Callback != null)
				{
					m_Callback(from, "", m_State);
				}
				else if (m_CancelCallback != null)
				{
					m_CancelCallback(from, "", m_State);
				}
			}
		}

		public Prompt BeginPrompt(PromptStateCallback callback, PromptStateCallback cancelCallback, object state)
		{
			Prompt p = new SimpleStatePrompt(callback, cancelCallback, state);

			Prompt = p;
			return p;
		}

		public Prompt BeginPrompt(PromptStateCallback callback, bool callbackHandlesCancel, object state)
		{
			Prompt p = new SimpleStatePrompt(callback, callbackHandlesCancel, state);

			Prompt = p;
			return p;
		}

		public Prompt BeginPrompt(PromptStateCallback callback, object state)
		{
			return BeginPrompt(callback, false, state);
		}

		private class SimpleStatePrompt<T> : Prompt
		{
			private readonly PromptStateCallback<T> m_Callback;
			private readonly PromptStateCallback<T> m_CancelCallback;

			private readonly bool m_CallbackHandlesCancel;

			private readonly T m_State;

			public SimpleStatePrompt(PromptStateCallback<T> callback, PromptStateCallback<T> cancelCallback, T state)
			{
				m_Callback = callback;
				m_CancelCallback = cancelCallback;
				m_State = state;
			}

			public SimpleStatePrompt(PromptStateCallback<T> callback, bool callbackHandlesCancel, T state)
			{
				m_Callback = callback;
				m_State = state;
				m_CallbackHandlesCancel = callbackHandlesCancel;
			}

			public SimpleStatePrompt(PromptStateCallback<T> callback, T state)
				: this(callback, false, state)
			{ }

			public override void OnResponse(Mobile from, string text)
			{
				if (m_Callback != null)
				{
					m_Callback(from, text, m_State);
				}
			}

			public override void OnCancel(Mobile from)
			{
				if (m_CallbackHandlesCancel && m_Callback != null)
				{
					m_Callback(from, "", m_State);
				}
				else if (m_CancelCallback != null)
				{
					m_CancelCallback(from, "", m_State);
				}
			}
		}

		public Prompt BeginPrompt<T>(PromptStateCallback<T> callback, PromptStateCallback<T> cancelCallback, T state)
		{
			Prompt p = new SimpleStatePrompt<T>(callback, cancelCallback, state);

			Prompt = p;
			return p;
		}

		public Prompt BeginPrompt<T>(PromptStateCallback<T> callback, bool callbackHandlesCancel, T state)
		{
			Prompt p = new SimpleStatePrompt<T>(callback, callbackHandlesCancel, state);

			Prompt = p;
			return p;
		}

		public Prompt BeginPrompt<T>(PromptStateCallback<T> callback, T state)
		{
			return BeginPrompt(callback, false, state);
		}

		public Prompt Prompt
		{
			get => m_Prompt;
			set
			{
				var oldPrompt = m_Prompt;
				var newPrompt = value;

				if (oldPrompt == newPrompt)
				{
					return;
				}

				m_Prompt = null;

				if (oldPrompt != null && newPrompt != null)
				{
					oldPrompt.OnCancel(this);
				}

				m_Prompt = newPrompt;

				if (newPrompt != null)
				{
					newPrompt.SendTo(this);
					//Send(new UnicodePrompt(newPrompt));
				}
			}
		}
		#endregion

		private bool InternalOnMove(Direction d)
		{
			if (!OnMove(d))
			{
				return false;
			}

			var e = MovementEventArgs.Create(this, d);

			EventSink.InvokeMovement(e);

			var ret = !e.Blocked;

			e.Free();

			return ret;
		}

		/// <summary>
		///     Overridable. Event invoked before the Mobile <see cref="Move">moves</see>.
		/// </summary>
		/// <returns>True if the move is allowed, false if not.</returns>
		protected virtual bool OnMove(Direction d)
		{
			if (m_Hidden && m_AccessLevel == AccessLevel.Player)
			{
				if (m_AllowedStealthSteps-- <= 0 || (d & Direction.Running) != 0 || Mounted)
				{
					RevealingAction();
				}
			}

			return true;
		}

		private static readonly Packet[][] m_MovingPacketCache =
		{
			new Packet[8], new Packet[8]
		};

		private bool m_Pushing;
		private bool m_IgnoreMobiles;
		private bool m_IsStealthing;

		public bool Pushing { get => m_Pushing; set => m_Pushing = value; }

		private static int m_WalkFoot = 400;
		private static int m_RunFoot = 200;
		private static int m_WalkMount = 200;
		private static int m_RunMount = 100;

		public static int WalkFoot { get => m_WalkFoot; set => m_WalkFoot = value; }
		public static int RunFoot { get => m_RunFoot; set => m_RunFoot = value; }
		public static int WalkMount { get => m_WalkMount; set => m_WalkMount = value; }
		public static int RunMount { get => m_RunMount; set => m_RunMount = value; }

		private long m_EndQueue;

		private static readonly List<IEntity> m_MoveList = new List<IEntity>();
		private static readonly List<Mobile> m_MoveClientList = new List<Mobile>();

		private static AccessLevel m_FwdAccessOverride = AccessLevel.Counselor;
		private static bool m_FwdEnabled = true;
		private static bool m_FwdUOTDOverride;
		private static int m_FwdMaxSteps = 4;

		public static AccessLevel FwdAccessOverride { get => m_FwdAccessOverride; set => m_FwdAccessOverride = value; }
		public static bool FwdEnabled { get => m_FwdEnabled; set => m_FwdEnabled = value; }
		public static bool FwdUOTDOverride { get => m_FwdUOTDOverride; set => m_FwdUOTDOverride = value; }
		public static int FwdMaxSteps { get => m_FwdMaxSteps; set => m_FwdMaxSteps = value; }

		public virtual void ClearFastwalkStack()
		{
			if (m_MoveRecords != null && m_MoveRecords.Count > 0)
			{
				m_MoveRecords.Clear();
			}

			m_EndQueue = Core.TickCount;
		}

		public virtual bool CheckMovement(Direction d, out int newZ)
		{
			return Movement.Movement.CheckMovement(this, Map, Location, d, out newZ);
		}

		public virtual bool Move(Direction d)
		{
			if (m_Deleted)
			{
				return false;
			}

			var box = FindBankNoCreate();

			if (box != null && box.Opened)
			{
				box.Close();
			}

			var newLocation = m_Location;
			var oldLocation = newLocation;

			if ((m_Direction & Direction.Mask) == (d & Direction.Mask))
			{
				// We are actually moving (not just a direction change)

				if (m_Paralyzed || m_Frozen || (m_Spell != null && !m_Spell.CheckMovement(this)))
				{
					SendLocalizedMessage(500111); // You are frozen and can not move.

					return false;
				}


				if (CheckMovement(d, out var newZ))
				{
					int x = oldLocation.m_X, y = oldLocation.m_Y;
					int oldX = x, oldY = y;
					var oldZ = oldLocation.m_Z;

					switch (d & Direction.Mask)
					{
						case Direction.North:
						--y;
						break;
						case Direction.Right:
						++x;
						--y;
						break;
						case Direction.East:
						++x;
						break;
						case Direction.Down:
						++x;
						++y;
						break;
						case Direction.South:
						++y;
						break;
						case Direction.Left:
						--x;
						++y;
						break;
						case Direction.West:
						--x;
						break;
						case Direction.Up:
						--x;
						--y;
						break;
					}

					newLocation.m_X = x;
					newLocation.m_Y = y;
					newLocation.m_Z = newZ;

					m_Pushing = false;

					var map = m_Map;

					if (map != null)
					{
						var oldSector = map.GetSector(oldX, oldY);
						var newSector = map.GetSector(x, y);

						if (oldSector != newSector)
						{
							for (var i = 0; i < oldSector.Mobiles.Count; ++i)
							{
								var m = oldSector.Mobiles[i];

								if (m != this && m.X == oldX && m.Y == oldY && (m.Z + 15) > oldZ && (oldZ + 15) > m.Z && !m.OnMoveOff(this))
								{
									return false;
								}
							}

							for (var i = 0; i < oldSector.Items.Count; ++i)
							{
								var item = oldSector.Items[i];

								if (item.AtWorldPoint(oldX, oldY) &&
									(item.Z == oldZ || ((item.Z + item.ItemData.Height) > oldZ && (oldZ + 15) > item.Z)) && !item.OnMoveOff(this))
								{
									return false;
								}
							}

							for (var i = 0; i < newSector.Mobiles.Count; ++i)
							{
								var m = newSector.Mobiles[i];

								if (m.X == x && m.Y == y && (m.Z + 15) > newZ && (newZ + 15) > m.Z && !m.OnMoveOver(this))
								{
									return false;
								}
							}

							for (var i = 0; i < newSector.Items.Count; ++i)
							{
								var item = newSector.Items[i];

								if (item.AtWorldPoint(x, y) &&
									(item.Z == newZ || ((item.Z + item.ItemData.Height) >= newZ && (newZ + 15) > item.Z)) && !item.OnMoveOver(this))
								{
									return false;
								}
							}
						}
						else
						{
							for (var i = 0; i < oldSector.Mobiles.Count; ++i)
							{
								var m = oldSector.Mobiles[i];

								if (m != this && m.X == oldX && m.Y == oldY && (m.Z + 15) > oldZ && (oldZ + 15) > m.Z && !m.OnMoveOff(this))
								{
									return false;
								}

								if (m.X == x && m.Y == y && (m.Z + 15) > newZ && (newZ + 15) > m.Z && !m.OnMoveOver(this))
								{
									return false;
								}
							}

							for (var i = 0; i < oldSector.Items.Count; ++i)
							{
								var item = oldSector.Items[i];

								if (item.AtWorldPoint(oldX, oldY) &&
									(item.Z == oldZ || ((item.Z + item.ItemData.Height) > oldZ && (oldZ + 15) > item.Z)) && !item.OnMoveOff(this))
								{
									return false;
								}

								if (item.AtWorldPoint(x, y) &&
									(item.Z == newZ || ((item.Z + item.ItemData.Height) >= newZ && (newZ + 15) > item.Z)) && !item.OnMoveOver(this))
								{
									return false;
								}
							}
						}

						if (!Region.CanMove(this, d, newLocation, oldLocation, m_Map))
						{
							return false;
						}
					}
					else
					{
						return false;
					}

					if (!InternalOnMove(d))
					{
						return false;
					}

					if (m_FwdEnabled && m_NetState != null && m_AccessLevel < m_FwdAccessOverride &&
						(!m_FwdUOTDOverride || !m_NetState.IsUOTDClient))
					{
						if (m_MoveRecords == null)
						{
							m_MoveRecords = new Queue<MovementRecord>(6);
						}

						while (m_MoveRecords.Count > 0)
						{
							var r = m_MoveRecords.Peek();

							if (r.Expired())
							{
								m_MoveRecords.Dequeue();
							}
							else
							{
								break;
							}
						}

						if (m_MoveRecords.Count >= m_FwdMaxSteps)
						{
							var fw = new FastWalkEventArgs(m_NetState);
							EventSink.InvokeFastWalk(fw);

							if (fw.Blocked)
							{
								return false;
							}
						}

						var delay = ComputeMovementSpeed(d);

						long end;

						if (m_MoveRecords.Count > 0)
						{
							end = m_EndQueue + delay;
						}
						else
						{
							end = Core.TickCount + delay;
						}

						m_MoveRecords.Enqueue(MovementRecord.NewInstance(end));

						m_EndQueue = end;
					}

					m_LastMoveTime = Core.TickCount;
				}
				else
				{
					return false;
				}

				DisruptiveAction();
			}

			if (m_NetState != null)
			{
				m_NetState.Send(MovementAck.Instantiate(m_NetState.Sequence, this));
			}

			SetLocation(newLocation, false);
			SetDirection(d);

			if (m_Map != null)
			{
				var eable = m_Map.GetObjectsInRange(m_Location, Core.GlobalMaxUpdateRange);

				foreach (var o in eable)
				{
					if (o == this)
					{
						continue;
					}

					if (o is Mobile mob)
					{
						if (mob.NetState != null)
						{
							m_MoveClientList.Add(mob);
						}
						m_MoveList.Add(mob);
					}
					else if (o is Item item && item.HandlesOnMovement)
					{
						m_MoveList.Add(item);
					}
				}

				eable.Free();

				var cache = m_MovingPacketCache;

				foreach (var m in m_MoveClientList)
				{
					var ns = m.NetState;

					if (ns != null && m.InUpdateRange(m_Location) && m.CanSee(this))
					{
						var noto = Notoriety.Compute(m, this);
						var p = cache[0][noto];

						if (p == null)
						{
							cache[0][noto] = p = Packet.Acquire(new MobileMoving(this, noto));
						}

						ns.Send(p);
					}
				}

				for (var i = 0; i < cache.Length; ++i)
				{
					for (var j = 0; j < cache[i].Length; ++j)
					{
						Packet.Release(ref cache[i][j]);
					}
				}

				for (var i = 0; i < m_MoveList.Count; ++i)
				{
					var o = m_MoveList[i];

					if (o is Mobile mobile)
					{
						mobile.OnMovement(this, oldLocation);
					}
					else if (o is Item item)
					{
						item.OnMovement(this, oldLocation);
					}
				}

				if (m_MoveList.Count > 0)
				{
					m_MoveList.Clear();
				}

				if (m_MoveClientList.Count > 0)
				{
					m_MoveClientList.Clear();
				}
			}

			OnAfterMove(oldLocation);
			return true;
		}

		public virtual void OnAfterMove(Point3D oldLocation)
		{ }

		public int ComputeMovementSpeed()
		{
			return ComputeMovementSpeed(Direction, false);
		}

		public int ComputeMovementSpeed(Direction dir)
		{
			return ComputeMovementSpeed(dir, true);
		}

		public virtual int ComputeMovementSpeed(Direction dir, bool checkTurning)
		{
			int delay;

			if (Mounted)
			{
				delay = (dir & Direction.Running) != 0 ? m_RunMount : m_WalkMount;
			}
			else
			{
				delay = (dir & Direction.Running) != 0 ? m_RunFoot : m_WalkFoot;
			}

			return delay;
		}

		/// <summary>
		///     Overridable. Virtual event invoked when a Mobile <paramref name="m" /> moves off this Mobile.
		/// </summary>
		/// <returns>True if the move is allowed, false if not.</returns>
		public virtual bool OnMoveOff(Mobile m)
		{
			return true;
		}

		public virtual bool IsDeadBondedPet => false;

		/// <summary>
		///     Overridable. Event invoked when a Mobile <paramref name="m" /> moves over this Mobile.
		/// </summary>
		/// <returns>True if the move is allowed, false if not.</returns>
		public virtual bool OnMoveOver(Mobile m)
		{
			if (m_Map == null || m_Deleted)
			{
				return true;
			}

			return m.CheckShove(this);
		}

		public virtual bool CheckShove(Mobile shoved)
		{
			if (!m_IgnoreMobiles && (m_Map.Rules & MapRules.FreeMovement) == 0)
			{
				if (!shoved.Alive || !Alive || shoved.IsDeadBondedPet || IsDeadBondedPet)
				{
					return true;
				}

				if (shoved.m_Hidden && shoved.IsStaff())
				{
					return true;
				}

				if (!m_Pushing)
				{
					m_Pushing = true;

					int number;

					if (IsStaff())
					{
						number = shoved.m_Hidden ? 1019041 : 1019040;
					}
					else
					{
						if (Stam == StamMax)
						{
							number = shoved.m_Hidden ? 1019043 : 1019042;
							Stam -= 10;

							RevealingAction();
						}
						else
						{
							return false;
						}
					}

					SendLocalizedMessage(number);
				}
			}
			return true;
		}

		/// <summary>
		///     Overridable. Virtual event invoked when the Mobile sees another Mobile, <paramref name="m" />, move.
		/// </summary>
		public virtual void OnMovement(Mobile m, Point3D oldLocation)
		{ }

		public ISpell Spell { get => m_Spell; set => m_Spell = value; }

		[CommandProperty(AccessLevel.Administrator)]
		public bool AutoPageNotify { get => m_AutoPageNotify; set => m_AutoPageNotify = value; }

		public virtual void CriminalAction(bool message)
		{
			if (m_Deleted)
			{
				return;
			}

			Criminal = true;

			Region.OnCriminalAction(this, message);
		}

		public virtual bool IsPlayer()
		{
			return AccessLevel < AccessLevel.Counselor;
		}

		public virtual bool IsStaff()
		{
			return AccessLevel >= AccessLevel.Counselor;
		}

		public virtual bool IsSnoop(Mobile from)
		{
			return from != this;
		}

		/// <summary>
		///     Overridable. Any call to <see cref="Resurrect" /> will silently fail if this method returns false.
		///     <seealso cref="Resurrect" />
		/// </summary>
		public virtual bool CheckResurrect()
		{
			return true;
		}

		/// <summary>
		///     Overridable. Event invoked before the Mobile is <see cref="Resurrect">resurrected</see>.
		///     <seealso cref="Resurrect" />
		/// </summary>
		public virtual void OnBeforeResurrect()
		{ }

		/// <summary>
		///     Overridable. Event invoked after the Mobile is <see cref="Resurrect">resurrected</see>.
		///     <seealso cref="Resurrect" />
		/// </summary>
		public virtual void OnAfterResurrect()
		{ }

		public virtual void Resurrect()
		{
			if (!Alive)
			{
				if (!Region.OnResurrect(this))
				{
					return;
				}

				if (!CheckResurrect())
				{
					return;
				}

				OnBeforeResurrect();

				var box = FindBankNoCreate();

				if (box != null && box.Opened)
				{
					box.Close();
				}

				Poison = null;

				Warmode = false;

				Hits = 10;
				Stam = StamMax;
				Mana = 0;

				BodyMod = 0;
				Body = Race.AliveBody(this);

				ProcessDeltaQueue();

				for (var i = m_Items.Count - 1; i >= 0; --i)
				{
					if (i >= m_Items.Count)
					{
						continue;
					}

					var item = m_Items[i];

					if (item.ItemID == 8270)
					{
						item.Delete();
					}
				}

				SendIncomingPacket();
				SendIncomingPacket();

				OnAfterResurrect();
			}
		}

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
		public IAccount Account { get; set; }

		private bool m_Deleted;

		public bool Deleted => m_Deleted;

		[CommandProperty(AccessLevel.GameMaster)]
		public int VirtualArmor
		{
			get => m_VirtualArmor;
			set
			{
				if (m_VirtualArmor != value)
				{
					m_VirtualArmor = value;

					Delta(MobileDelta.Armor);
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual double ArmorRating => 0.0;

		public void DropHolding()
		{
			var holding = m_Holding;

			if (holding != null)
			{
				if (!holding.Deleted && holding.HeldBy == this && holding.Map == Map.Internal)
				{
					AddToBackpack(holding);
				}

				Holding = null;
				holding.ClearBounce();
			}
		}

		public virtual void Delete()
		{
			if (m_Deleted)
			{
				return;
			}

			if (!World.OnDelete(this))
			{
				return;
			}

			if (m_NetState != null)
			{
				m_NetState.CancelAllTrades();
			}

			if (m_NetState != null)
			{
				m_NetState.Dispose();
			}

			DropHolding();

			Region.OnRegionChange(this, m_Region, null);

			m_Region = null;
			//Is the above line REALLY needed?  The old Region system did NOT have said line
			//and worked fine, because of this a LOT of extra checks have to be done everywhere...
			//I guess this should be there for Garbage collection purposes, but, still, is it /really/ needed?

			OnDelete();

			for (var i = m_Items.Count - 1; i >= 0; --i)
			{
				if (i < m_Items.Count)
				{
					m_Items[i].OnParentDeleted(this);
				}
			}

			for (var i = 0; i < m_Stabled.Count; i++)
			{
				m_Stabled[i].Delete();
			}

			SendRemovePacket();

			if (m_Guild != null)
			{
				m_Guild.OnDelete(this);
			}

			m_Deleted = true;

			if (m_Map != null)
			{
				m_Map.OnLeave(this);
				m_Map = null;
			}

			m_Hair = null;
			m_FacialHair = null;
			m_MountItem = null;
			m_Face = null;

			World.RemoveMobile(this);

			OnAfterDelete();

			FreeCache();
		}

		/// <summary>
		///     Overridable. Virtual event invoked before the Mobile is deleted.
		/// </summary>
		public virtual void OnDelete()
		{
			if (m_Spawner != null)
			{
				m_Spawner.Remove(this);
				m_Spawner = null;
			}
		}

		/// <summary>
		///     Overridable. Returns true if the player is alive, false if otherwise. By default, this is computed by: <c>!Deleted &amp;&amp; (!Player || !Body.IsGhost)</c>
		/// </summary>
		[CommandProperty(AccessLevel.Counselor)]
		public virtual bool Alive => !m_Deleted && (!m_Player || !m_Body.IsGhost);

		public virtual bool CheckSpellCast(ISpell spell)
		{
			return true;
		}

		/// <summary>
		///     Overridable. Virtual event invoked when the Mobile casts a <paramref name="spell" />.
		/// </summary>
		/// <param name="spell"></param>
		public virtual void OnSpellCast(ISpell spell)
		{ }

		/// <summary>
		///     Overridable. Virtual event invoked after <see cref="TotalWeight" /> changes.
		/// </summary>
		public virtual void OnWeightChange(int oldValue)
		{ }

		/// <summary>
		///     Overridable. Virtual event invoked when the <see cref="Skill.Base" /> or <see cref="Skill.BaseFixedPoint" /> property of
		///     <paramref
		///         name="skill" />
		///     changes.
		/// </summary>
		public virtual void OnSkillChange(SkillName skill, double oldBase)
		{ }

		/// <summary>
		///     Overridable. Invoked after the mobile is deleted. When overriden, be sure to call the base method.
		/// </summary>
		public virtual void OnAfterDelete()
		{
			StopAggrExpire();

			CheckAggrExpire();

			if (m_PoisonTimer != null)
			{
				m_PoisonTimer.Stop();
			}

			if (m_WarmodeTimer != null)
			{
				m_WarmodeTimer.Stop();
			}

			if (m_AutoManifestTimer != null)
			{
				m_AutoManifestTimer.Stop();
			}

			Timer.DelayCall(EventSink.InvokeMobileDeleted, new MobileDeletedEventArgs(this));
		}

		public virtual bool AllowSkillUse(SkillName name)
		{
			return true;
		}

		public virtual bool UseSkill(SkillName name)
		{
			return Skills.UseSkill(this, name);
		}

		public virtual bool UseSkill(int skillID)
		{
			return Skills.UseSkill(this, skillID);
		}

		private static CreateCorpseHandler m_CreateCorpse;

		public static TimeSpan DefaultCorpseDecay => _DefaultCorpseDecay;
		public static readonly TimeSpan _DefaultCorpseDecay = TimeSpan.FromMinutes(7);

		public static CreateCorpseHandler CreateCorpseHandler { get => m_CreateCorpse; set => m_CreateCorpse = value; }

		public virtual TimeSpan CorpseDecayTime => _DefaultCorpseDecay;

		public virtual DeathMoveResult GetParentMoveResultFor(Item item)
		{
			return item.OnParentDeath(this);
		}

		public virtual DeathMoveResult GetInventoryMoveResultFor(Item item)
		{
			return item.OnInventoryDeath(this);
		}

		public virtual bool RetainPackLocsOnDeath => true;

		public virtual void Kill()
		{
			m_LastKilled = DateTime.UtcNow;

			if (!CanBeDamaged())
			{
				return;
			}

			if (!Alive || IsDeadBondedPet)
			{
				return;
			}

			if (m_Deleted)
			{
				return;
			}

			if (!Region.OnBeforeDeath(this))
			{
				return;
			}

			if (!OnBeforeDeath())
			{
				return;
			}

			var box = FindBankNoCreate();

			if (box != null && box.Opened)
			{
				box.Close();
			}

			if (m_NetState != null)
			{
				m_NetState.CancelAllTrades();
			}

			if (m_Spell != null)
			{
				m_Spell.OnCasterKilled();
			}

			if (m_Target != null)
			{
				m_Target.Cancel(this, TargetCancelType.Canceled);
			}

			DisruptiveAction();

			Warmode = false;

			DropHolding();

			Hits = 0;
			Stam = 0;
			Mana = 0;

			Poison = null;
			Combatant = null;

			if (Paralyzed)
			{
				Paralyzed = false;

				TimerRegistry.RemoveFromRegistry(_ParaTimerID, this);
			}

			if (Frozen)
			{
				Frozen = false;

				TimerRegistry.RemoveFromRegistry(_FrozenTimerID, this);
			}

			var content = new List<Item>();
			var equip = new List<Item>();
			var moveToPack = new List<Item>();

			var itemsCopy = new List<Item>(m_Items);

			var pack = Backpack;

			for (var i = 0; i < itemsCopy.Count; ++i)
			{
				var item = itemsCopy[i];

				if (item == pack)
				{
					continue;
				}

				if ((item.Insured || item.LootType == LootType.Blessed) && item.Parent == this && item.Layer != Layer.Mount)
				{
					equip.Add(item);
				}

				var res = GetParentMoveResultFor(item);

				switch (res)
				{
					case DeathMoveResult.MoveToCorpse:
						{
							content.Add(item);
							equip.Add(item);
							break;
						}
					case DeathMoveResult.MoveToBackpack:
						{
							moveToPack.Add(item);
							break;
						}
				}
			}

			if (pack != null)
			{
				var packCopy = new List<Item>(pack.Items);

				for (var i = 0; i < packCopy.Count; ++i)
				{
					var item = packCopy[i];

					var res = GetInventoryMoveResultFor(item);

					if (res == DeathMoveResult.MoveToCorpse)
					{
						content.Add(item);
					}
					else
					{
						moveToPack.Add(item);
					}
				}

				for (var i = 0; i < moveToPack.Count; ++i)
				{
					var item = moveToPack[i];

					if (RetainPackLocsOnDeath && item.Parent == pack)
					{
						continue;
					}

					pack.DropItem(item);
				}
			}

			HairInfo hair = null;
			if (m_Hair != null)
			{
				hair = new HairInfo(m_Hair.ItemID, m_Hair.Hue);
			}

			FacialHairInfo facialhair = null;
			if (m_FacialHair != null)
			{
				facialhair = new FacialHairInfo(m_FacialHair.ItemID, m_FacialHair.Hue);
			}

			var c = m_CreateCorpse == null ? null : m_CreateCorpse(this, hair, facialhair, content, equip);

			if (m_Map != null)
			{
				Packet animPacket = null;

				var eable = m_Map.GetClientsInRange(m_Location);

				foreach (var state in eable)
				{
					if (state != m_NetState)
					{
						if (animPacket == null)
						{
							animPacket = Packet.Acquire(new DeathAnimation(this, c));
						}

						state.Send(animPacket);

						if (!state.Mobile.CanSee(this))
						{
							state.Send(RemovePacket);
						}
					}
				}

				Packet.Release(animPacket);

				eable.Free();
			}

			Region.OnDeath(this);
			OnDeath(c);
		}

		private Container m_Corpse;

		[CommandProperty(AccessLevel.GameMaster)]
		public Container Corpse { get => m_Corpse; set => m_Corpse = value; }

		/// <summary>
		///     Overridable. Event invoked before the Mobile is <see cref="Kill">killed</see>.
		///     <seealso cref="Kill" />
		///     <seealso cref="OnDeath" />
		/// </summary>
		/// <returns>True to continue with death, false to override it.</returns>
		public virtual bool OnBeforeDeath()
		{
			return true;
		}

		/// <summary>
		///     Overridable. Event invoked after the Mobile is <see cref="Kill">killed</see>. Primarily, this method is responsible for deleting an NPC or turning a PC into a ghost.
		///     <seealso cref="Kill" />
		///     <seealso cref="OnBeforeDeath" />
		/// </summary>
		public virtual void OnDeath(Container c)
		{
			if (LastKiller != null)
			{
				var items = LastKiller.Items;

				var i = items.Count;

				while (--i >= 0)
				{
					if (i >= items.Count)
					{
						continue;
					}

					var o = items[i];

					if (o != null)
					{
						o.OnParentKill(this, c);
					}
				}
			}

			var sound = GetDeathSound();

			if (sound >= 0)
			{
				Effects.PlaySound(this, Map, sound);
			}

			RevealingAction();

			if (!m_Player)
			{
				Delete();
			}
			else
			{
				Send(DeathStatus.Instantiate(true));

				Warmode = false;

				BodyMod = 0;
				//Body = this.Female ? 0x193 : 0x192;
				Body = Race.GhostBody(this);

				var deathShroud = new Item(0x204E)
				{
					Movable = false,
					Layer = Layer.OuterTorso
				};

				AddItem(deathShroud);

				m_Items.Remove(deathShroud);
				m_Items.Insert(0, deathShroud);

				Poison = null;
				Combatant = null;

				Hits = 0;
				Stam = 0;
				Mana = 0;

				EventSink.InvokePlayerDeath(new PlayerDeathEventArgs(this, LastKiller, c));

				ProcessDeltaQueue();

				Send(DeathStatus.Instantiate(false));

				CheckStatTimers();
			}
		}

		#region Get*Sound
		public virtual int GetAngerSound()
		{
			if (m_BaseSoundID != 0)
			{
				return m_BaseSoundID;
			}

			return -1;
		}

		public virtual int GetIdleSound()
		{
			if (m_BaseSoundID != 0)
			{
				return m_BaseSoundID + 1;
			}

			return -1;
		}

		public virtual int GetAttackSound()
		{
			if (m_BaseSoundID != 0)
			{
				return m_BaseSoundID + 2;
			}

			return -1;
		}

		public virtual int GetHurtSound()
		{
			if (m_BaseSoundID != 0)
			{
				return m_BaseSoundID + 3;
			}

			return -1;
		}

		public virtual int GetDeathSound()
		{
			if (m_BaseSoundID != 0)
			{
				return m_BaseSoundID + 4;
			}

			if (m_Body.IsHuman)
			{
				return Utility.Random(m_Female ? 0x314 : 0x423, m_Female ? 4 : 5);
			}

			return -1;
		}
		#endregion

		private static char[] m_GhostChars = { 'o', 'O' };

		public static char[] GhostChars { get => m_GhostChars; set => m_GhostChars = value; }

		private static bool m_NoSpeechLOS;

		public static bool NoSpeechLOS { get => m_NoSpeechLOS; set => m_NoSpeechLOS = value; }

		private static TimeSpan m_AutoManifestTimeout = TimeSpan.FromSeconds(5.0);

		public static TimeSpan AutoManifestTimeout { get => m_AutoManifestTimeout; set => m_AutoManifestTimeout = value; }

		private Timer m_AutoManifestTimer;

		private class AutoManifestTimer : Timer
		{
			private readonly Mobile m_Mobile;

			public AutoManifestTimer(Mobile m, TimeSpan delay)
				: base(delay)
			{
				m_Mobile = m;
			}

			protected override void OnTick()
			{
				if (!m_Mobile.Alive)
				{
					m_Mobile.Warmode = false;
				}
			}
		}

		public virtual bool CheckTarget(Mobile from, Target targ, object targeted)
		{
			return true;
		}

		public static bool InsuranceEnabled { get; set; }

		public virtual void Use(Item item)
		{
			if (item == null || item.Deleted || item.QuestItem || Deleted)
			{
				return;
			}

			DisruptiveAction();

			if (m_Spell != null && !m_Spell.OnCasterUsingObject(item))
			{
				return;
			}

			var root = item.RootParent;
			var okay = false;

			if (!Utility.InUpdateRange(this, item.GetWorldLocation()))
			{
				item.OnDoubleClickOutOfRange(this);
			}
			else if (!CanSee(item))
			{
				item.OnDoubleClickCantSee(this);
			}
			else if (!item.IsAccessibleTo(this))
			{
				var reg = Region.Find(item.GetWorldLocation(), item.Map);

				if (reg == null || !reg.SendInaccessibleMessage(item, this))
				{
					item.OnDoubleClickNotAccessible(this);
				}
			}
			else if (!CheckAlive(false))
			{
				item.OnDoubleClickDead(this);
			}
			else if (item.InSecureTrade)
			{
				item.OnDoubleClickSecureTrade(this);
			}
			else if (!AllowItemUse(item))
			{
				okay = false;
			}
			else if (!item.CheckItemUse(this, item))
			{
				okay = false;
			}
			else if (root is Mobile mobile && mobile.IsSnoop(this))
			{
				item.OnSnoop(this);
			}
			else if (Region.OnDoubleClick(this, item))
			{
				okay = true;
			}

			if (okay)
			{
				if (!item.Deleted)
				{
					item.OnItemUsed(this, item);
				}

				if (!item.Deleted)
				{
					item.OnDoubleClick(this);
				}
			}
		}

		public virtual void Use(Mobile m)
		{
			if (m == null || m.Deleted || Deleted)
			{
				return;
			}

			DisruptiveAction();

			if (m_Spell != null && !m_Spell.OnCasterUsingObject(m))
			{
				return;
			}

			if (!Utility.InUpdateRange(this, m))
			{
				m.OnDoubleClickOutOfRange(this);
			}
			else if (!CanSee(m))
			{
				m.OnDoubleClickCantSee(this);
			}
			else if (!CheckAlive(false))
			{
				m.OnDoubleClickDead(this);
			}
			else if (Region.OnDoubleClick(this, m) && !m.Deleted)
			{
				m.OnDoubleClick(this);
			}
		}

		public static int ActionDelay { get; set; } = 750;

		public virtual void Lift(Item item, int amount, out bool rejected, out LRReason reject)
		{
			rejected = true;
			reject = LRReason.Inspecific;

			if (item == null)
			{
				return;
			}

			var from = this;
			var state = m_NetState;

			if (from.IsStaff() || Core.TickCount - from.NextActionTime >= 0)
			{
				if (from.CheckAlive())
				{
					from.DisruptiveAction();

					if (from.Holding != null)
					{
						reject = LRReason.AreHolding;
					}
					else if (from.AccessLevel < AccessLevel.GameMaster && !from.InRange(item.GetWorldLocation(), 2))
					{
						reject = LRReason.OutOfRange;
					}
					else if (!from.CanSee(item) || !from.InLOS(item))
					{
						reject = LRReason.OutOfSight;
					}
					else if (!item.VerifyMove(from))
					{
						reject = LRReason.CannotLift;
					}
					else if (item.QuestItem && amount != item.Amount && !from.IsStaff())
					{
						reject = LRReason.Inspecific;
						from.SendLocalizedMessage(1074868); // Stacks of quest items cannot be unstacked.
					}
					else if (!item.IsAccessibleTo(from))
					{
						reject = LRReason.CannotLift;
					}
					else if (item.Nontransferable && amount != item.Amount)
					{
						if (item.QuestItem)
						{
							from.SendLocalizedMessage(1074868); // Stacks of quest items cannot be unstacked.
						}

						reject = LRReason.CannotLift;
					}
					else if (!item.CheckLift(from, item, ref reject))
					{ }
					else
					{
						var root = item.RootParent;

						if (root is Mobile mobile && !mobile.CheckNonlocalLift(from, item))
						{
							reject = LRReason.TryToSteal;
						}
						else if (!from.OnDragLift(item) || !item.OnDragLift(from))
						{
							reject = LRReason.Inspecific;
						}
						else if (!from.CheckAlive())
						{
							reject = LRReason.Inspecific;
						}
						else
						{
							item.SetLastMoved();

							var itemGrid = item.GridLocation;

							if (item.Spawner != null)
							{
								item.Spawner.Remove(item);
								item.Spawner = null;
							}

							if (amount < 1)
							{
								amount = 1;
							}

							if (amount > item.Amount)
							{
								amount = item.Amount;
							}

							var oldAmount = item.Amount;

							Item oldStack = null;

							if (amount < oldAmount)
							{
								oldStack = LiftItemDupe(item, amount);
							}

							var map = from.Map;

							if (m_DragEffects && map != null && (root == null || root is Item))
							{
								var eable = map.GetClientsInRange(from.Location);
								Packet p = null;

								foreach (var ns in eable)
								{
									if (ns.Mobile != from && ns.Mobile.CanSee(from) && ns.Mobile.InLOS(from) && ns.Mobile.CanSee(root))
									{
										if (p == null)
										{
											IEntity src;

											if (root == null)
											{
												src = new Entity(Serial.Zero, item.Location, map);
											}
											else
											{
												src = new Entity(((Item)root).Serial, ((Item)root).Location, map);
											}

											p = Packet.Acquire(new DragEffect(src, from, item.ItemID, item.Hue, amount));
										}

										ns.Send(p);
									}
								}

								Packet.Release(p);

								eable.Free();
							}

							var fixLoc = item.Location;
							var fixMap = item.Map;
							var shouldFix = item.Parent == null;

							item.RecordBounce(this, oldStack);
							item.OnItemLifted(from, item);
							item.Internalize();

							from.Holding = item;
							item.GridLocation = 0;

							if (oldStack != null)
							{
								oldStack.GridLocation = itemGrid;
							}

							var liftSound = item.GetLiftSound(from);

							if (liftSound != -1)
							{
								from.Send(new PlaySound(liftSound, from));
							}

							from.NextActionTime = Core.TickCount + ActionDelay;

							if (fixMap != null && shouldFix)
							{
								fixMap.FixColumn(fixLoc.m_X, fixLoc.m_Y);
							}

							reject = LRReason.Inspecific;
							rejected = false;
						}
					}
				}
				else
				{
					reject = LRReason.Inspecific;
				}
			}
			else
			{
				SendActionMessage();
				reject = LRReason.Inspecific;
			}

			if (rejected && state != null)
			{
				state.Send(new LiftRej(reject));

				if (item.Deleted)
				{
					return;
				}

				if (item.Parent is Item)
				{
					state.Send(new ContainerContentUpdate(item));
				}
				else if (item.Parent is Mobile)
				{
					state.Send(new EquipUpdate(item));
				}
				else
				{
					item.SendInfoTo(state);
				}

				if (item.Parent != null)
				{
					state.Send(item.OPLPacket);
				}
			}
		}

		public static Item LiftItemDupe(Item oldItem, int amount)
		{
			Item item;
			try
			{
				item = (Item)Activator.CreateInstance(oldItem.GetType());
			}
			catch
			{
				Console.WriteLine(
					"Warning: 0x{0:X}: Item must have a zero paramater constructor to be separated from a stack. '{1}'.",
					oldItem.Serial.Value,
					oldItem.GetType().Name);
				return null;
			}

			item.Visible = oldItem.Visible;
			item.Movable = oldItem.Movable;
			item.LootType = oldItem.LootType;
			item.Direction = oldItem.Direction;
			item.Hue = oldItem.Hue;
			item.ItemID = oldItem.ItemID;
			item.Location = oldItem.Location;
			item.Layer = oldItem.Layer;
			item.Name = oldItem.Name;
			item.Weight = oldItem.Weight;

			item.Amount = oldItem.Amount - amount;
			item.Map = oldItem.Map;

			oldItem.Amount = amount;
			oldItem.OnAfterDuped(item);

			if (oldItem.Parent is Mobile mobile)
			{
				mobile.AddItem(item);
			}
			else if (oldItem.Parent is Item parent)
			{
				parent.AddItem(item);
			}

			item.Delta(ItemDelta.Update);

			return item;
		}

		public virtual bool Drop(Item to, Point3D loc)
		{
			var from = this;
			var item = from.Holding;

			var valid = item != null && item.HeldBy == from && item.Map == Map.Internal;

			from.Holding = null;

			if (!valid)
			{
				return false;
			}

			var bounced = true;

			item.SetLastMoved();

			if (to == null || !item.DropToItem(from, to, loc))
			{
				item.Bounce(from);
			}
			else
			{
				bounced = false;
			}

			item.ClearBounce();

			return !bounced;
		}

		public virtual bool Drop(Point3D loc)
		{
			var from = this;
			var item = from.Holding;

			var valid = item != null && item.HeldBy == from && item.Map == Map.Internal;

			from.Holding = null;

			if (!valid)
			{
				return false;
			}

			var bounced = true;

			item.SetLastMoved();

			if (!item.DropToWorld(from, loc))
			{
				item.Bounce(from);
			}
			else
			{
				bounced = false;
			}

			item.ClearBounce();

			return !bounced;
		}

		public virtual bool Drop(Mobile to, Point3D loc)
		{
			var from = this;
			var item = from.Holding;

			var valid = item != null && item.HeldBy == from && item.Map == Map.Internal;

			from.Holding = null;

			if (!valid)
			{
				return false;
			}

			var bounced = true;

			item.SetLastMoved();

			if (to == null || !item.DropToMobile(from, to, loc))
			{
				item.Bounce(from);
			}
			else
			{
				bounced = false;
			}

			item.ClearBounce();

			return !bounced;
		}

		private static readonly object m_GhostMutateContext = new object();

		public virtual bool MutateSpeech(List<Mobile> hears, ref string text, ref object context)
		{
			if (Alive)
			{
				return false;
			}

			var sb = new StringBuilder(text.Length, text.Length);

			for (var i = 0; i < text.Length; ++i)
			{
				if (text[i] != ' ')
				{
					sb.Append(m_GhostChars[Utility.Random(m_GhostChars.Length)]);
				}
				else
				{
					sb.Append(' ');
				}
			}

			text = sb.ToString();
			context = m_GhostMutateContext;
			return true;
		}

		public virtual void Manifest(TimeSpan delay)
		{
			Warmode = true;

			if (m_AutoManifestTimer == null)
			{
				m_AutoManifestTimer = new AutoManifestTimer(this, delay);
			}
			else
			{
				m_AutoManifestTimer.Stop();
			}

			m_AutoManifestTimer.Start();
		}

		public virtual bool CheckSpeechManifest()
		{
			if (Alive)
			{
				return false;
			}

			var delay = m_AutoManifestTimeout;

			if (delay > TimeSpan.Zero && (!Warmode || m_AutoManifestTimer != null))
			{
				Manifest(delay);
				return true;
			}

			return false;
		}

		public virtual bool CheckHearsMutatedSpeech(Mobile m, object context)
		{
			if (context == m_GhostMutateContext)
			{
				return m.Alive && !m.CanHearGhosts;
			}

			return true;
		}

		private void AddSpeechItemsFrom(List<IEntity> list, Container cont)
		{
			for (var i = 0; i < cont.Items.Count; ++i)
			{
				var item = cont.Items[i];

				if (item.HandlesOnSpeech)
				{
					list.Add(item);
				}

				if (item is Container container)
				{
					AddSpeechItemsFrom(list, container);
				}
			}
		}

		private class LocationComparer : IComparer<IEntity>
		{
			private static LocationComparer m_Instance;

			public static LocationComparer GetInstance(IEntity relativeTo)
			{
				if (m_Instance == null)
				{
					m_Instance = new LocationComparer(relativeTo);
				}
				else
				{
					m_Instance.m_RelativeTo = relativeTo;
				}

				return m_Instance;
			}

			private IEntity m_RelativeTo;

			public IEntity RelativeTo { get => m_RelativeTo; set => m_RelativeTo = value; }

			public LocationComparer(IEntity relativeTo)
			{
				m_RelativeTo = relativeTo;
			}

			private int GetDistance(IEntity p)
			{
				var x = m_RelativeTo.X - p.X;
				var y = m_RelativeTo.Y - p.Y;
				var z = m_RelativeTo.Z - p.Z;

				x *= 11;
				y *= 11;

				return (x * x) + (y * y) + (z * z);
			}

			public int Compare(IEntity x, IEntity y)
			{
				return GetDistance(x) - GetDistance(y);
			}
		}

		#region Get*InRange
		public IPooledEnumerable<Item> GetItemsInRange(int range)
		{
			var map = m_Map;

			if (map == null)
			{
				return Map.NullEnumerable<Item>.Instance;
			}

			return map.GetItemsInRange(m_Location, range);
		}

		public IPooledEnumerable<IEntity> GetObjectsInRange(int range)
		{
			var map = m_Map;

			if (map == null)
			{
				return Map.NullEnumerable<IEntity>.Instance;
			}

			return map.GetObjectsInRange(m_Location, range);
		}

		public IPooledEnumerable<Mobile> GetMobilesInRange(int range)
		{
			var map = m_Map;

			if (map == null)
			{
				return Map.NullEnumerable<Mobile>.Instance;
			}

			return map.GetMobilesInRange(m_Location, range);
		}

		public IPooledEnumerable<NetState> GetClientsInRange(int range)
		{
			var map = m_Map;

			if (map == null)
			{
				return Map.NullEnumerable<NetState>.Instance;
			}

			return map.GetClientsInRange(m_Location, range);
		}
		#endregion

		private static readonly List<Mobile> m_Hears = new List<Mobile>();
		private static readonly List<IEntity> m_OnSpeech = new List<IEntity>();

		public virtual void DoSpeech(string text, int[] keywords, MessageType type, int hue)
		{
			if (m_Deleted || CommandSystem.Handle(this, text, type))
			{
				return;
			}

			var range = 15;

			switch (type)
			{
				case MessageType.Regular:
				m_SpeechHue = hue;
				break;
				case MessageType.Emote:
				m_EmoteHue = hue;
				break;
				case MessageType.Whisper:
				m_WhisperHue = hue;
				range = 1;
				break;
				case MessageType.Yell:
				m_YellHue = hue;
				range = 18;
				break;
				default:
				type = MessageType.Regular;
				break;
			}

			var regArgs = new SpeechEventArgs(this, text, type, hue, keywords);

			EventSink.InvokeSpeech(regArgs);
			Region.OnSpeech(regArgs);
			OnSaid(regArgs);

			if (regArgs.Blocked)
			{
				return;
			}

			text = regArgs.Speech;

			if (String.IsNullOrEmpty(text))
			{
				return;
			}

			var hears = m_Hears;
			var onSpeech = m_OnSpeech;

			if (m_Map != null)
			{
				var eable = m_Map.GetObjectsInRange(m_Location, range);

				foreach (var o in eable)
				{
					if (o is Mobile heard)
					{
						if (heard.CanSee(this) && (m_NoSpeechLOS || !heard.Player || heard.InLOS(this)))
						{
							if (heard.m_NetState != null)
							{
								hears.Add(heard);
							}

							if (heard.HandlesOnSpeech(this))
							{
								onSpeech.Add(heard);
							}

							for (var i = 0; i < heard.Items.Count; ++i)
							{
								var item = heard.Items[i];

								if (item.HandlesOnSpeech)
								{
									onSpeech.Add(item);
								}

								if (item is Container container)
								{
									AddSpeechItemsFrom(onSpeech, container);
								}
							}
						}
					}
					else if (o is Item item)
					{
						if (item.HandlesOnSpeech)
						{
							onSpeech.Add(item);
						}

						if (item is Container container)
						{
							AddSpeechItemsFrom(onSpeech, container);
						}
					}
				}

				eable.Free();

				object mutateContext = null;
				var mutatedText = text;
				SpeechEventArgs mutatedArgs = null;

				if (MutateSpeech(hears, ref mutatedText, ref mutateContext))
				{
					mutatedArgs = new SpeechEventArgs(this, mutatedText, type, hue, new int[0]);
				}

				CheckSpeechManifest();

				ProcessDelta();

				Packet regp = null;
				Packet mutp = null;

				// TODO: Should this be sorted like onSpeech is below?

				for (var i = 0; i < hears.Count; ++i)
				{
					var heard = hears[i];

					if (mutatedArgs == null || !CheckHearsMutatedSpeech(heard, mutateContext))
					{
						heard.OnSpeech(regArgs);

						var ns = heard.NetState;

						if (ns != null)
						{
							if (regp == null)
							{
								regp = Packet.Acquire(new UnicodeMessage(m_Serial, Body, type, hue, 3, m_Language, Name, text));
							}

							ns.Send(regp);
						}
					}
					else
					{
						heard.OnSpeech(mutatedArgs);

						var ns = heard.NetState;

						if (ns != null)
						{
							if (mutp == null)
							{
								mutp = Packet.Acquire(new UnicodeMessage(m_Serial, Body, type, hue, 3, m_Language, Name, mutatedText));
							}

							ns.Send(mutp);
						}
					}
				}

				Packet.Release(regp);
				Packet.Release(mutp);

				if (onSpeech.Count > 1)
				{
					onSpeech.Sort(LocationComparer.GetInstance(this));
				}

				for (var i = 0; i < onSpeech.Count; ++i)
				{
					var obj = onSpeech[i];

					if (obj is Mobile heard)
					{
						if (mutatedArgs == null || !CheckHearsMutatedSpeech(heard, mutateContext))
						{
							heard.OnSpeech(regArgs);
						}
						else
						{
							heard.OnSpeech(mutatedArgs);
						}
					}
					else
					{
						var item = (Item)obj;

						item.OnSpeech(regArgs);
					}
				}

				if (m_Hears.Count > 0)
				{
					m_Hears.Clear();
				}

				if (m_OnSpeech.Count > 0)
				{
					m_OnSpeech.Clear();
				}
			}
		}

		private static VisibleDamageType m_VisibleDamageType;

		public static VisibleDamageType VisibleDamageType { get => m_VisibleDamageType; set => m_VisibleDamageType = value; }

		private List<DamageEntry> m_DamageEntries;

		public List<DamageEntry> DamageEntries => m_DamageEntries;

		public static Mobile GetDamagerFrom(DamageEntry de)
		{
			return de == null ? null : de.Damager;
		}

		public Mobile FindMostRecentDamager(bool allowSelf)
		{
			return GetDamagerFrom(FindMostRecentDamageEntry(allowSelf));
		}

		public DamageEntry FindMostRecentDamageEntry(bool allowSelf)
		{
			for (var i = m_DamageEntries.Count - 1; i >= 0; --i)
			{
				if (i >= m_DamageEntries.Count)
				{
					continue;
				}

				var de = m_DamageEntries[i];

				if (de.HasExpired)
				{
					m_DamageEntries.RemoveAt(i);
				}
				else if (allowSelf || de.Damager != this)
				{
					return de;
				}
			}

			return null;
		}

		public Mobile FindLeastRecentDamager(bool allowSelf)
		{
			return GetDamagerFrom(FindLeastRecentDamageEntry(allowSelf));
		}

		public DamageEntry FindLeastRecentDamageEntry(bool allowSelf)
		{
			for (var i = 0; i < m_DamageEntries.Count; ++i)
			{
				if (i < 0)
				{
					continue;
				}

				var de = m_DamageEntries[i];

				if (de.HasExpired)
				{
					m_DamageEntries.RemoveAt(i);
					--i;
				}
				else if (allowSelf || de.Damager != this)
				{
					return de;
				}
			}

			return null;
		}

		public Mobile FindMostTotalDamager(bool allowSelf)
		{
			return GetDamagerFrom(FindMostTotalDamageEntry(allowSelf));
		}

		public DamageEntry FindMostTotalDamageEntry(bool allowSelf)
		{
			DamageEntry mostTotal = null;

			for (var i = m_DamageEntries.Count - 1; i >= 0; --i)
			{
				if (i >= m_DamageEntries.Count)
				{
					continue;
				}

				var de = m_DamageEntries[i];

				if (de.HasExpired)
				{
					m_DamageEntries.RemoveAt(i);
				}
				else if ((allowSelf || de.Damager != this) && (mostTotal == null || de.DamageGiven > mostTotal.DamageGiven))
				{
					mostTotal = de;
				}
			}

			return mostTotal;
		}

		public Mobile FindLeastTotalDamager(bool allowSelf)
		{
			return GetDamagerFrom(FindLeastTotalDamageEntry(allowSelf));
		}

		public DamageEntry FindLeastTotalDamageEntry(bool allowSelf)
		{
			DamageEntry mostTotal = null;

			for (var i = m_DamageEntries.Count - 1; i >= 0; --i)
			{
				if (i >= m_DamageEntries.Count)
				{
					continue;
				}

				var de = m_DamageEntries[i];

				if (de.HasExpired)
				{
					m_DamageEntries.RemoveAt(i);
				}
				else if ((allowSelf || de.Damager != this) && (mostTotal == null || de.DamageGiven < mostTotal.DamageGiven))
				{
					mostTotal = de;
				}
			}

			return mostTotal;
		}

		public DamageEntry FindDamageEntryFor(Mobile m)
		{
			for (var i = m_DamageEntries.Count - 1; i >= 0; --i)
			{
				if (i >= m_DamageEntries.Count)
				{
					continue;
				}

				var de = m_DamageEntries[i];

				if (de.HasExpired)
				{
					m_DamageEntries.RemoveAt(i);
				}
				else if (de.Damager == m)
				{
					return de;
				}
			}

			return null;
		}

		public virtual Mobile GetDamageMaster(Mobile damagee)
		{
			return null;
		}

		public virtual DamageEntry RegisterDamage(int amount, Mobile from)
		{
			var de = FindDamageEntryFor(from);

			if (de == null)
			{
				de = new DamageEntry(from);
			}

			de.DamageGiven += amount;
			de.LastDamage = DateTime.UtcNow;

			m_DamageEntries.Remove(de);
			m_DamageEntries.Add(de);

			var master = from.GetDamageMaster(this);

			if (master != null)
			{
				var list = de.Responsible;

				if (list == null)
				{
					de.Responsible = list = new List<DamageEntry>();
				}

				DamageEntry resp = null;

				for (var i = 0; i < list.Count; ++i)
				{
					var check = list[i];

					if (check.Damager == master)
					{
						resp = check;
						break;
					}
				}

				if (resp == null)
				{
					list.Add(resp = new DamageEntry(master));
				}

				resp.DamageGiven += amount;
				resp.LastDamage = DateTime.UtcNow;
			}

			return de;
		}

		private Mobile m_LastKiller;
		private DateTime m_LastKilled;

		[CommandProperty(AccessLevel.GameMaster)]
		public Mobile LastKiller { get => m_LastKiller; set => m_LastKiller = value; }

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime LastKilled { get => m_LastKilled; set => m_LastKilled = value; }

		/// <summary>
		///     Overridable. Virtual event invoked when the Mobile is <see cref="OnDamage">damaged</see>. It is called before
		///     <see
		///         cref="Hits">
		///         hit points
		///     </see>
		///     are lowered or the Mobile is <see cref="Kill">killed</see>.
		///     <seealso cref="OnDamage" />
		///     <seealso cref="Hits" />
		///     <seealso cref="Kill" />
		/// </summary>
		public virtual void OnDamage(int amount, Mobile from, bool willKill)
		{ }

		public virtual bool CanBeDamaged()
		{
			return !m_Blessed;
		}

		public virtual int Damage(int amount)
		{
			return Damage(amount, null);
		}

		public virtual int Damage(int amount, Mobile from)
		{
			return Damage(amount, from, true);
		}

		public virtual int Damage(int amount, Mobile from, bool informMount)
		{
			return Damage(amount, from, informMount, true);
		}

		public virtual int Damage(int amount, Mobile from, bool informMount, bool checkDisrupt)
		{
			if (!CanBeDamaged() || m_Deleted)
			{
				return 0;
			}

			if (!Region.OnDamage(this, ref amount))
			{
				return 0;
			}

			if (amount > 0)
			{
				var oldHits = Hits;
				var newHits = oldHits - amount;

				if (checkDisrupt && m_Spell != null)
				{
					m_Spell.OnCasterHurt();
				}

				if (from != null)
				{
					RegisterDamage(amount, from);
				}

				DisruptiveAction();

				Paralyzed = false;

				var m = Mount;

				if (m != null && informMount)
				{
					var temp = amount;

					m.OnRiderDamaged(from, ref amount, newHits < 0);

					if (temp > amount)
					{
						var absorbed = temp - amount;
						newHits += absorbed;
					}
				}

				SendDamagePacket(from, amount);
				OnDamage(amount, from, newHits < 0);

				if (newHits < 0)
				{
					m_LastKiller = from;

					m_InternalCanRegen = false;

					Hits = 0;

					if (oldHits >= 0)
					{
						Kill();
					}

					m_InternalCanRegen = true;
				}
				else
				{
					FatigueHandler(this, amount, DFA);

					Hits = newHits;
				}
			}

			return amount;
		}

		public virtual void SendDamagePacket(Mobile from, int amount)
		{
			switch (m_VisibleDamageType)
			{
				case VisibleDamageType.Related:
					{
						NetState ourState = m_NetState, theirState = from == null ? null : from.m_NetState;

						if (ourState == null)
						{
							var master = GetDamageMaster(from);

							if (master != null)
							{
								ourState = master.m_NetState;
							}
						}

						if (theirState == null && from != null)
						{
							var master = from.GetDamageMaster(this);

							if (master != null)
							{
								theirState = master.m_NetState;
							}
						}

						if (amount > 0 && (ourState != null || theirState != null))
						{
							Packet p = Packet.Acquire(new DamagePacket(this, amount));

							if (ourState != null)
							{
								ourState.Send(p);
							}

							if (theirState != null && theirState != ourState)
							{
								theirState.Send(p);
							}

							Packet.Release(p);
						}

						break;
					}
				case VisibleDamageType.Everyone:
					{
						SendDamageToAll(amount);
						break;
					}
			}
		}

		public virtual void SendDamageToAll(int amount)
		{
			if (amount < 0)
			{
				return;
			}

			var map = m_Map;

			if (map == null)
			{
				return;
			}

			var eable = map.GetClientsInRange(m_Location);

			Packet p = Packet.Acquire(new DamagePacket(this, amount));

			foreach (var ns in eable)
			{
				if (ns.Mobile.CanSee(this))
				{
					ns.Send(p);
				}
			}

			Packet.Release(p);

			eable.Free();
		}

		public virtual int Heal(int amount)
		{
			return Heal(amount, this, true);
		}

		public virtual int Heal(int amount, Mobile from)
		{
			return Heal(amount, from, true);
		}

		public virtual int Heal(int amount, Mobile from, bool message)
		{
			if (!Alive || IsDeadBondedPet)
			{
				return 0;
			}

			if (!Region.OnHeal(this, ref amount))
			{
				return 0;
			}

			if ((Hits + amount) > HitsMax)
			{
				amount = HitsMax - Hits;
			}

			OnHeal(ref amount, from);

			Hits += amount;

			if (message && amount > 0 && m_NetState != null)
			{
				m_NetState.Send(
					new MessageLocalizedAffix(
						m_NetState,
						Serial.MinusOne,
						-1,
						MessageType.Label,
						0x3B2,
						3,
						1008158,
						"",
						AffixType.Append | AffixType.System,
						amount.ToString(),
						""));
			}

			return amount;
		}

		public virtual void OnHeal(ref int amount, Mobile from)
		{ }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Squelched { get => m_Squelched; set => m_Squelched = value; }

		public virtual void Deserialize(GenericReader reader)
		{
			var version = reader.ReadInt();

			switch (version)
			{
				case 38:
				case 37:
					{
						m_DisplayGuildAbbr = reader.ReadBool();

						goto case 36;
					}
				case 36:
					{
						m_BloodHue = reader.ReadInt();
						m_Deaths = reader.ReadInt();
					}
					goto case 35;
				case 35:
				GuardImmune = reader.ReadBool();
				goto case 34;
				case 34:
					{
						m_StrCap = reader.ReadInt();
						m_DexCap = reader.ReadInt();
						m_IntCap = reader.ReadInt();
						m_StrMaxCap = reader.ReadInt();
						m_DexMaxCap = reader.ReadInt();
						m_IntMaxCap = reader.ReadInt();

						goto case 33;
					}
				case 33:
					{
						m_SpecialSlayerMechanics = reader.ReadBool();

						if (reader.ReadBool())
						{
							var length = reader.ReadInt();

							for (var i = 0; i < length; i++)
							{
								m_SlayerVulnerabilities.Add(reader.ReadString());
							}

						}
						else
						{
							m_SlayerVulnerabilities = new List<string>();
						}

						goto case 32;
					}
				case 32:
					{
						m_IgnoreMobiles = reader.ReadBool();

						goto case 31;
					}
				case 31:
					{
						m_LastStrGain = reader.ReadDeltaTime();
						m_LastIntGain = reader.ReadDeltaTime();
						m_LastDexGain = reader.ReadDeltaTime();

						goto case 30;
					}
				case 30:
					{
						var hairflag = reader.ReadByte();

						if ((hairflag & 0x01) != 0)
						{
							m_Hair = new HairInfo(reader);
						}

						if ((hairflag & 0x02) != 0)
						{
							m_FacialHair = new FacialHairInfo(reader);
						}

						if ((hairflag & 0x04) != 0)
						{
							m_Face = new FaceInfo(reader);
						}

						goto case 29;
					}
				case 29:
					{
						m_Race = reader.ReadRace();
						goto case 28;
					}
				case 28:
					{
						if (version <= 30)
						{
							LastStatGain = reader.ReadDeltaTime();
						}

						goto case 27;
					}
				case 27:
					{
						m_TithingPoints = reader.ReadInt();

						goto case 26;
					}
				case 26:
				case 25:
				case 24:
					{
						m_Corpse = reader.ReadItem() as Container;

						goto case 23;
					}
				case 23:
					{
						m_CreationTime = reader.ReadDateTime();

						goto case 22;
					}
				case 22: // Just removed followers
				case 21:
					{
						m_Stabled = reader.ReadStrongMobileList();

						goto case 20;
					}
				case 20:
					{
						m_CantWalk = reader.ReadBool();

						goto case 19;
					}
				case 19: // Just removed variables
				case 18:
					{
						m_Virtues = new VirtueInfo(reader);

						goto case 17;
					}
				case 17:
					{
						m_Thirst = reader.ReadInt();
						m_BAC = reader.ReadInt();

						goto case 16;
					}
				case 16:
					{
						m_ShortTermMurders = reader.ReadInt();

						if (version <= 24)
						{
							reader.ReadDateTime();
							reader.ReadDateTime();
						}

						goto case 15;
					}
				case 15:
					{
						if (version < 22)
						{
							reader.ReadInt(); // followers
						}

						m_FollowersMax = reader.ReadInt();

						goto case 14;
					}
				case 14:
					{
						m_MagicDamageAbsorb = reader.ReadInt();

						goto case 13;
					}
				case 13:
					{
						m_GuildFealty = reader.ReadMobile();

						goto case 12;
					}
				case 12:
					{
						m_Guild = reader.ReadGuild();

						goto case 11;
					}
				case 11:
					{
						m_DisplayGuildTitle = reader.ReadBool();

						goto case 10;
					}
				case 10:
					{
						m_CanSwim = reader.ReadBool();

						goto case 9;
					}
				case 9:
					{
						m_Squelched = reader.ReadBool();

						goto case 8;
					}
				case 8:
					{
						m_Holding = reader.ReadItem();

						goto case 7;
					}
				case 7:
					{
						m_VirtualArmor = reader.ReadInt();

						goto case 6;
					}
				case 6:
					{
						m_BaseSoundID = reader.ReadInt();

						goto case 5;
					}
				case 5:
					{
						if (version < 38)
						{
							reader.ReadBool();
							reader.ReadBool();
						}

						goto case 4;
					}
				case 4:
					{
						if (version <= 25)
						{
							Poison.Deserialize(reader);
						}

						goto case 3;
					}
				case 3:
					{
						m_StatCap = reader.ReadInt();

						goto case 2;
					}
				case 2:
					{
						m_NameHue = reader.ReadInt();

						goto case 1;
					}
				case 1:
					{
						m_Hunger = reader.ReadInt();

						goto case 0;
					}
				case 0:
					{
						if (version < 37)
						{
							m_DisplayGuildAbbr = true;
						}

						if (version < 34)
						{
							m_StrCap = Config.Get("PlayerCaps.StrCap", 125);
							m_DexCap = Config.Get("PlayerCaps.DexCap", 125);
							m_IntCap = Config.Get("PlayerCaps.IntCap", 125);
							m_StrMaxCap = Config.Get("PlayerCaps.StrMaxCap", 150);
							m_DexMaxCap = Config.Get("PlayerCaps.DexMaxCap", 150);
							m_IntMaxCap = Config.Get("PlayerCaps.IntMaxCap", 150);
						}

						if (version < 21)
						{
							m_Stabled = new List<Mobile>();
						}

						if (version < 18)
						{
							m_Virtues = new VirtueInfo();
						}

						if (version < 11)
						{
							m_DisplayGuildTitle = true;
						}

						if (version < 3)
						{
							m_StatCap = Config.Get("PlayerCaps.TotalStatCap", 225);
						}

						if (version < 15)
						{
							m_Followers = 0;
							m_FollowersMax = 5;
						}

						m_Location = reader.ReadPoint3D();
						m_Body = new Body(reader.ReadInt());
						m_Name = reader.ReadString();
						m_GuildTitle = reader.ReadString();
						m_Criminal = reader.ReadBool();
						m_Kills = reader.ReadInt();
						m_SpeechHue = reader.ReadInt();
						m_EmoteHue = reader.ReadInt();
						m_WhisperHue = reader.ReadInt();
						m_YellHue = reader.ReadInt();
						m_Language = reader.ReadString();
						m_Female = reader.ReadBool();
						m_Warmode = reader.ReadBool();
						m_Hidden = reader.ReadBool();
						m_Direction = (Direction)reader.ReadByte();
						m_Hue = reader.ReadInt();
						m_Str = reader.ReadInt();
						m_Dex = reader.ReadInt();
						m_Int = reader.ReadInt();
						m_Hits = reader.ReadInt();
						m_Stam = reader.ReadInt();
						m_Mana = reader.ReadInt();
						m_Map = reader.ReadMap();
						m_Blessed = reader.ReadBool();
						m_Fame = reader.ReadInt();
						m_Karma = reader.ReadInt();
						m_AccessLevel = (AccessLevel)reader.ReadByte();

						m_Skills = new Skills(this, reader);

						m_Items = reader.ReadStrongItemList();

						m_Player = reader.ReadBool();
						m_Title = reader.ReadString();
						m_Profile = reader.ReadString();
						m_ProfileLocked = reader.ReadBool();

						if (version <= 18)
						{
							reader.ReadInt();
							reader.ReadInt();
							reader.ReadInt();
						}

						m_AutoPageNotify = reader.ReadBool();

						m_LogoutLocation = reader.ReadPoint3D();
						m_LogoutMap = reader.ReadMap();

						m_StrLock = (StatLockType)reader.ReadByte();
						m_DexLock = (StatLockType)reader.ReadByte();
						m_IntLock = (StatLockType)reader.ReadByte();

						m_StatMods = new List<StatMod>();
						m_SkillMods = new List<SkillMod>();

						if (reader.ReadBool())
						{
							m_StuckMenuUses = new DateTime[reader.ReadInt()];

							for (var i = 0; i < m_StuckMenuUses.Length; ++i)
							{
								m_StuckMenuUses[i] = reader.ReadDateTime();
							}
						}
						else
						{
							m_StuckMenuUses = null;
						}

						if (m_Player && m_Map != Map.Internal)
						{
							m_LogoutLocation = m_Location;
							m_LogoutMap = m_Map;

							m_Map = Map.Internal;
						}

						if (m_Map != null)
						{
							m_Map.OnEnter(this);
						}

						if (m_Criminal)
						{
							StartCrimDelayTimer();
						}

						m_InternalCanRegen = true;

						if (ShouldCheckStatTimers)
						{
							CheckStatTimers();
						}

						UpdateRegion();

						UpdateResistances();

						break;
					}
			}

			if (!m_Player)
			{
				Utility.Intern(ref m_Name);
			}

			Utility.Intern(ref m_Title);
			Utility.Intern(ref m_Language);

			/*	//Moved into cleanup in scripts.
			if( version < 30 )
			Timer.DelayCall( TimeSpan.Zero, new TimerCallback( ConvertHair ) );
			* */
		}

		public void ConvertHair()
		{
			Item hair;

			if ((hair = FindItemOnLayer(Layer.Hair)) != null)
			{
				HairItemID = hair.ItemID;
				HairHue = hair.Hue;
				hair.Delete();
			}

			if ((hair = FindItemOnLayer(Layer.FacialHair)) != null)
			{
				FacialHairItemID = hair.ItemID;
				FacialHairHue = hair.Hue;
				hair.Delete();
			}
		}

		public virtual bool ShouldCheckStatTimers => true;

		public virtual void CheckStatTimers()
		{
			if (m_Deleted)
			{
				return;
			}

			if (Hits != HitsMax)
			{
				Hits = m_Hits;
			}

			if (Stam != StamMax)
			{
				Stam = m_Stam;
			}

			if (Mana != ManaMax)
			{
				Mana = m_Mana;
			}
		}

		public virtual void ResetStatTimers()
		{
			TimerRegistry.RemoveFromRegistry(Player ? _HitsRegenTimerPlayerID : _HitsRegenTimerID, this);
			TimerRegistry.RemoveFromRegistry(Player ? _StamRegenTimerPlayerID : _StamRegenTimerID, this);
			TimerRegistry.RemoveFromRegistry(Player ? _ManaRegenTimerPlayerID : _ManaRegenTimerID, this);

			if (!m_Deleted)
			{
				Hits = m_Hits;
				Stam = m_Stam;
				Mana = m_Mana;
			}
		}

		private DateTime m_CreationTime;

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime CreationTime => m_CreationTime;

		int ISerializable.TypeReference => m_TypeRef;

		int ISerializable.SerialIdentity => m_Serial;

		public virtual void Serialize(GenericWriter writer)
		{
			writer.Write(38); // version

			// 38 - Removed Disarm/Stun Ready

			// 37
			writer.Write(m_DisplayGuildAbbr);

			// 36
			writer.Write(m_BloodHue);
			writer.Write(m_Deaths);

			// 35
			writer.Write(GuardImmune);

			// 34
			writer.Write(m_StrCap);
			writer.Write(m_DexCap);
			writer.Write(m_IntCap);
			writer.Write(m_StrMaxCap);
			writer.Write(m_DexMaxCap);
			writer.Write(m_IntMaxCap);

			writer.Write(m_SpecialSlayerMechanics);

			if (m_SlayerVulnerabilities != null && m_SlayerVulnerabilities.Count > 0)
			{
				writer.Write(true);

				writer.Write(m_SlayerVulnerabilities.Count);

				for (var i = 0; i < m_SlayerVulnerabilities.Count; i++)
				{
					writer.Write(m_SlayerVulnerabilities[i]);
				}
			}
			else
			{
				writer.Write(false);
			}

			writer.Write(m_IgnoreMobiles);

			writer.WriteDeltaTime(m_LastStrGain);
			writer.WriteDeltaTime(m_LastIntGain);
			writer.WriteDeltaTime(m_LastDexGain);

			byte hairflag = 0x00;

			if (m_Hair != null)
			{
				hairflag |= 0x01;
			}

			if (m_FacialHair != null)
			{
				hairflag |= 0x02;
			}

			if (m_Face != null)
			{
				hairflag |= 0x04;
			}

			writer.Write(hairflag);

			if ((hairflag & 0x01) != 0)
			{
				m_Hair.Serialize(writer);
			}

			if ((hairflag & 0x02) != 0)
			{
				m_FacialHair.Serialize(writer);
			}

			if ((hairflag & 0x04) != 0)
			{
				if (m_Face != null)
				{
					m_Face.Serialize(writer);
				}
			}

			writer.Write(Race);

			writer.Write(m_TithingPoints);

			writer.Write(m_Corpse);

			writer.Write(m_CreationTime);

			writer.Write(m_Stabled, true);

			writer.Write(m_CantWalk);

			VirtueInfo.Serialize(writer, m_Virtues);

			writer.Write(m_Thirst);
			writer.Write(m_BAC);

			writer.Write(m_ShortTermMurders);
			//writer.Write( m_ShortTermElapse );
			//writer.Write( m_LongTermElapse );

			//writer.Write( m_Followers );
			writer.Write(m_FollowersMax);

			writer.Write(m_MagicDamageAbsorb);

			writer.Write(m_GuildFealty);

			writer.Write(m_Guild);

			writer.Write(m_DisplayGuildTitle);

			writer.Write(m_CanSwim);

			writer.Write(m_Squelched);

			writer.Write(m_Holding);

			writer.Write(m_VirtualArmor);

			writer.Write(m_BaseSoundID);

			writer.Write(m_StatCap);

			writer.Write(m_NameHue);

			writer.Write(m_Hunger);

			writer.Write(m_Location);
			writer.Write(m_Body);
			writer.Write(m_Name);
			writer.Write(m_GuildTitle);
			writer.Write(m_Criminal);
			writer.Write(m_Kills);
			writer.Write(m_SpeechHue);
			writer.Write(m_EmoteHue);
			writer.Write(m_WhisperHue);
			writer.Write(m_YellHue);
			writer.Write(m_Language);
			writer.Write(m_Female);
			writer.Write(m_Warmode);
			writer.Write(m_Hidden);
			writer.Write((byte)m_Direction);
			writer.Write(m_Hue);
			writer.Write(m_Str);
			writer.Write(m_Dex);
			writer.Write(m_Int);
			writer.Write(m_Hits);
			writer.Write(m_Stam);
			writer.Write(m_Mana);

			writer.Write(m_Map);

			writer.Write(m_Blessed);
			writer.Write(m_Fame);
			writer.Write(m_Karma);
			writer.Write((byte)m_AccessLevel);
			m_Skills.Serialize(writer);

			writer.Write(m_Items);

			writer.Write(m_Player);
			writer.Write(m_Title);
			writer.Write(m_Profile);
			writer.Write(m_ProfileLocked);
			writer.Write(m_AutoPageNotify);

			writer.Write(m_LogoutLocation);
			writer.Write(m_LogoutMap);

			writer.Write((byte)m_StrLock);
			writer.Write((byte)m_DexLock);
			writer.Write((byte)m_IntLock);

			if (m_StuckMenuUses != null)
			{
				writer.Write(true);

				writer.Write(m_StuckMenuUses.Length);

				for (var i = 0; i < m_StuckMenuUses.Length; ++i)
				{
					writer.Write(m_StuckMenuUses[i]);
				}
			}
			else
			{
				writer.Write(false);
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int LightLevel
		{
			get => m_LightLevel;
			set
			{
				if (m_LightLevel != value)
				{
					m_LightLevel = value;

					CheckLightLevels(false);

					/*if ( m_NetState != null )
					m_NetState.Send( new PersonalLightLevel( this ) );*/
				}
			}
		}

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public string Profile { get => m_Profile; set => m_Profile = value; }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public bool ProfileLocked { get => m_ProfileLocked; set => m_ProfileLocked = value; }

		[CommandProperty(AccessLevel.GameMaster, AccessLevel.Administrator)]
		public bool Player
		{
			get => m_Player;
			set
			{
				m_Player = value;
				InvalidateProperties();

				CheckStatTimers();
			}
		}

		[CommandProperty(AccessLevel.Decorator)]
		public string Title
		{
			get => m_Title;
			set
			{
				m_Title = value;
				InvalidateProperties();
			}
		}

		private static readonly string[] m_AccessLevelNames =
		{
			"Player", "VIP", "Counselor", //
			"Decorator", "Spawner", "Game Master", //
			"Seer", "Administrator", "Developer", //
			"Owner", "Owner"
		};

		public static string GetAccessLevelName(AccessLevel level)
		{
			return m_AccessLevelNames[(int)level];
		}

		private static readonly string[] m_AccessLevelShortNames =
		{
			"Player", "VIP", "Counselor", //
			"Decorator", "Spawner", "GM", //
			"Seer", "Admin", "Dev", //
			"Owner", "Owner"
		};

		public static string GetAccessLevelShortName(AccessLevel level)
		{
			return m_AccessLevelShortNames[(int)level];
		}

		public virtual bool CanPaperdollBeOpenedBy(Mobile from)
		{
			return Body.IsHuman || Body.IsGhost || IsBodyMod || from == this;
		}

		public virtual void GetChildContextMenuEntries(Mobile from, List<ContextMenuEntry> list, Item item)
		{ }

		public virtual void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
		{
			if (m_Deleted)
			{
				return;
			}

			if (CanPaperdollBeOpenedBy(from))
			{
				list.Add(new PaperdollEntry(this));
			}

			if (from == this && Backpack != null && CanSee(Backpack) && CheckAlive(false))
			{
				list.Add(new OpenBackpackEntry(this));
			}

			if (Spawner != null)
			{
				Spawner.GetSpawnContextEntries(this, from, list);
			}
		}

		public virtual bool DisplayContextMenu(Mobile from)
		{
			return ContextMenu.Display(from, this);
		}

		public void Internalize()
		{
			Map = Map.Internal;
		}

		public List<Item> Items => m_Items;

		/// <summary>
		///     Overridable. Virtual event invoked when <paramref name="item" /> is <see cref="AddItem">added</see> from the Mobile, such as when it is equiped.
		///     <seealso cref="Items" />
		///     <seealso cref="OnItemRemoved" />
		/// </summary>
		public virtual void OnItemAdded(Item item)
		{ }

		/// <summary>
		///     Overridable. Virtual event invoked when <paramref name="item" /> is <see cref="RemoveItem">removed</see> from the Mobile.
		///     <seealso cref="Items" />
		///     <seealso cref="OnItemAdded" />
		/// </summary>
		public virtual void OnItemRemoved(Item item)
		{ }

		/// <summary>
		///     Overridable. Virtual event invoked when <paramref name="item" /> is becomes a child of the Mobile; it's worn or contained at some level of the Mobile's
		///     <see
		///         cref="Mobile.Backpack">
		///         backpack
		///     </see>
		///     or <see cref="Mobile.BankBox">bank box</see>
		///     <seealso cref="OnSubItemRemoved" />
		///     <seealso cref="OnItemAdded" />
		/// </summary>
		public virtual void OnSubItemAdded(Item item)
		{ }

		/// <summary>
		///     Overridable. Virtual event invoked when <paramref name="item" /> is removed from the Mobile, its
		///     <see
		///         cref="Mobile.Backpack">
		///         backpack
		///     </see>
		///     , or its <see cref="Mobile.BankBox">bank box</see>.
		///     <seealso cref="OnSubItemAdded" />
		///     <seealso cref="OnItemRemoved" />
		/// </summary>
		public virtual void OnSubItemRemoved(Item item)
		{ }

		public virtual void OnItemBounceCleared(Item item)
		{ }

		public virtual void OnSubItemBounceCleared(Item item)
		{ }

		public virtual int MaxWeight => Int32.MaxValue;

		public virtual void Obtained(Item item)
		{
			if (item != m_Backpack && item != m_BankBox)
			{
				EventSink.InvokeOnItemObtained(new OnItemObtainedEventArgs(this, item));
			}
		}

		public void AddItem(Item item)
		{
			if (item == null || item.Deleted)
			{
				return;
			}

			if (item.Parent == this)
			{
				return;
			}

			if (item.Parent is Mobile mobileParent)
			{
				mobileParent.RemoveItem(item);
			}
			else if (item.Parent is Item itemParent)
			{
				itemParent.RemoveItem(item);
			}
			else
			{
				item.SendRemovePacket();
			}

			var equipped = FindItemOnLayer(item.Layer);

			if (equipped != null && equipped != item)
			{
				try
				{
					using (var op = new StreamWriter("LayerConflict.log", true))
					{
						op.WriteLine("# {0}", DateTime.UtcNow);
						op.WriteLine("Offending Mobile: {0} [{1}]", GetType(), this);
						op.WriteLine("Offending Item: {0} [{1}]", item, item.GetType());
						op.WriteLine("Equipped Item: {0} [{1}]", equipped, equipped.GetType());
						op.WriteLine("Layer: {0}", item.Layer.ToString());
						op.WriteLine();
					}

					Utility.WriteConsoleColor(ConsoleColor.Red, String.Format("Offending Mobile: {0} [{1}]", GetType(), this));
					Utility.WriteConsoleColor(ConsoleColor.Red, String.Format("Offending Item: {0} [{1}]", item, item.GetType()));
					Utility.WriteConsoleColor(ConsoleColor.Red, String.Format("Equipped Item: {0} [{1}]", equipped, equipped.GetType()));
					Utility.WriteConsoleColor(ConsoleColor.Red, String.Format("Layer: {0}", item.Layer.ToString()));
				}
				catch (Exception e)
				{
					Diagnostics.ExceptionLogging.LogException(e);
				}
			}

			item.Parent = this;
			item.Map = m_Map;

			m_Items.Add(item);

			if (!item.IsVirtualItem)
			{
				UpdateTotal(item, TotalType.Gold, item.TotalGold);
				UpdateTotal(item, TotalType.Items, item.TotalItems + 1);
				UpdateTotal(item, TotalType.Weight, item.TotalWeight + item.PileWeight);
			}

			item.Delta(ItemDelta.Update);

			item.OnAdded(this);
			OnItemAdded(item);

			if (item.PhysicalResistance != 0 || item.FireResistance != 0 || item.ColdResistance != 0 ||
				item.PoisonResistance != 0 || item.EnergyResistance != 0)
			{
				UpdateResistances();
			}
		}

		private static IWeapon m_DefaultWeapon;

		public static IWeapon DefaultWeapon { get => m_DefaultWeapon; set => m_DefaultWeapon = value; }

		public void RemoveItem(Item item)
		{
			if (item == null || m_Items == null)
			{
				return;
			}

			if (m_Items.Contains(item))
			{
				item.SendRemovePacket();

				//int oldCount = m_Items.Count;

				m_Items.Remove(item);

				if (!item.IsVirtualItem)
				{
					UpdateTotal(item, TotalType.Gold, -item.TotalGold);
					UpdateTotal(item, TotalType.Items, -(item.TotalItems + 1));
					UpdateTotal(item, TotalType.Weight, -(item.TotalWeight + item.PileWeight));
				}

				item.Parent = null;

				item.OnRemoved(this);
				OnItemRemoved(item);

				if (item.PhysicalResistance != 0 || item.FireResistance != 0 || item.ColdResistance != 0 ||
					item.PoisonResistance != 0 || item.EnergyResistance != 0)
				{
					UpdateResistances();
				}
			}
		}

		public virtual void Animate(AnimationType type, int action)
		{
			var map = m_Map;

			if (map != null)
			{
				ProcessDelta();

				Packet p = null;

				var eable = map.GetClientsInRange(m_Location);

				foreach (var state in eable)
				{
					if (state.Mobile.CanSee(this))
					{
						state.Mobile.ProcessDelta();

						p = Packet.Acquire(new NewMobileAnimation(this, type, action, Utility.Random(0, 60)));

						state.Send(p);
					}
				}

				Packet.Release(p);

				eable.Free();
			}
		}

		public virtual void Animate(int action, int frameCount, int repeatCount, bool forward, bool repeat, int delay)
		{
			var map = m_Map;

			if (map != null)
			{
				ProcessDelta();

				Packet p = null;

				var eable = map.GetClientsInRange(m_Location);

				foreach (var state in eable)
				{
					if (state.Mobile.CanSee(this))
					{
						state.Mobile.ProcessDelta();

						if (p == null)
						{
							#region SA
							if (Body.IsGargoyle)
							{
								frameCount = 10;

								if (Flying)
								{
									if (action >= 200 && action <= 270)
									{
										action = 75;
									}
									else
									{
										switch (action)
										{
											case 9:
											case 10:
											case 11:
											action = 71;
											break;
											case 12:
											case 13:
											case 14:
											action = 72;
											break;
											case 18:
											case 19:
											action = 71;
											break;
											case 20:
											action = 77;
											break;
											case 31:
											action = 71;
											break;
											case 34:
											action = 78;
											break;
										}
									}
								}
								else
								{
									if (action >= 260 && action <= 270)
									{
										action = 16;
									}
									else if (action >= 200 && action < 260)
									{
										action = 17;
									}
									else
									{
										switch (action)
										{
											case 9:
											action = 13;
											break;
											case 10:
											action = 14;
											break;
											case 11:
											action = 13;
											break;
											case 12:
											case 13:
											case 14:
											action = 12;
											break;
											case 18:
											case 19:
											action = 9;
											break;
										}
									}
								}
							}
							#endregion

							p = Packet.Acquire(new MobileAnimation(this, action, frameCount, repeatCount, forward, repeat, delay));
						}

						state.Send(p);
					}
				}

				Packet.Release(p);

				eable.Free();
			}
		}

		public void SendSound(int soundID)
		{
			if (soundID != -1 && m_NetState != null)
			{
				Send(new PlaySound(soundID, this));
			}
		}

		public void SendSound(int soundID, IPoint3D p)
		{
			if (soundID != -1 && m_NetState != null)
			{
				Send(new PlaySound(soundID, p));
			}
		}

		public void PlaySound(int soundID)
		{
			if (soundID == -1)
			{
				return;
			}

			if (m_Map != null)
			{
				Packet p = Packet.Acquire(new PlaySound(soundID, this));

				var eable = m_Map.GetClientsInRange(m_Location);

				foreach (var state in eable)
				{
					if (state.Mobile.CanSee(this))
					{
						state.Send(p);
					}
				}

				Packet.Release(p);

				eable.Free();
			}
		}

		[CommandProperty(AccessLevel.Counselor)]
		public Skills Skills { get => m_Skills; set { } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool IgnoreMobiles
		{
			get => m_IgnoreMobiles;
			set
			{
				if (m_IgnoreMobiles != value)
				{
					m_IgnoreMobiles = value;
					Delta(MobileDelta.Flags);
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool IsStealthing
		{
			get => m_IsStealthing;
			set => m_IsStealthing = value;
		}

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
		public AccessLevel AccessLevel
		{
			get => m_AccessLevel;
			set
			{
				var oldValue = m_AccessLevel;

				if (oldValue != value)
				{
					m_AccessLevel = value;
					Delta(MobileDelta.Noto);
					InvalidateProperties();

					SendMessage("Your access level has been changed. You are now {0}.", GetAccessLevelName(value));

					ClearScreen();
					SendEverything();

					OnAccessLevelChanged(oldValue);
				}
			}
		}

		public virtual void OnAccessLevelChanged(AccessLevel oldLevel)
		{ }

		[CommandProperty(AccessLevel.Decorator)]
		public int Fame
		{
			get => m_Fame;
			set
			{
				var oldValue = m_Fame;

				if (oldValue != value)
				{
					m_Fame = value;

					if (ShowFameTitle && (m_Player || m_Body.IsHuman) && (oldValue >= 10000) != (value >= 10000))
					{
						InvalidateProperties();
					}

					OnFameChange(oldValue);
					EventSink.InvokeFameChange(new FameChangeEventArgs(this, oldValue, m_Fame));
				}
			}
		}

		public virtual void OnFameChange(int oldValue)
		{ }

		[CommandProperty(AccessLevel.Decorator)]
		public int Karma
		{
			get => m_Karma;
			set
			{
				var old = m_Karma;

				if (old != value)
				{
					m_Karma = value;
					OnKarmaChange(old);
					EventSink.InvokeKarmaChange(new KarmaChangeEventArgs(this, old, m_Karma));
				}
			}
		}

		public virtual void OnKarmaChange(int oldValue)
		{ }

		// Mobile did something which should unhide him
		public virtual void RevealingAction()
		{
			if (m_Hidden && IsPlayer())
			{
				Hidden = false;
			}

			m_IsStealthing = false;

			DisruptiveAction(); // Anything that unhides you will also distrupt meditation
		}

		#region Say/SayTo/Emote/Whisper/Yell
		public void SayTo(Mobile to, bool ascii, string text)
		{
			PrivateOverheadMessage(MessageType.Regular, m_SpeechHue, ascii, text, to.NetState);
		}

		public void SayTo(Mobile to, string text)
		{
			SayTo(to, false, text);
		}

		public void SayTo(Mobile to, string format, params object[] args)
		{
			SayTo(to, false, String.Format(format, args));
		}

		public void SayTo(Mobile to, bool ascii, string format, params object[] args)
		{
			SayTo(to, ascii, String.Format(format, args));
		}

		public void SayTo(Mobile to, int number)
		{
			to.Send(new MessageLocalized(m_Serial, Body, MessageType.Regular, m_SpeechHue, 3, number, Name, ""));
		}

		public void SayTo(Mobile to, int number, string args)
		{
			to.Send(new MessageLocalized(m_Serial, Body, MessageType.Regular, m_SpeechHue, 3, number, Name, args));
		}

		public void SayTo(Mobile to, int number, int hue)
		{
			PrivateOverheadMessage(MessageType.Regular, hue, number, to.NetState);
		}

		public void SayTo(Mobile to, int number, string args, int hue)
		{
			PrivateOverheadMessage(MessageType.Regular, hue, number, args, to.NetState);
		}

		public void SayTo(Mobile to, int hue, string text, string args)
		{
			SayTo(to, text, args, hue, false);
		}

		public void SayTo(Mobile to, int hue, string text, string args, bool ascii)
		{
			PrivateOverheadMessage(MessageType.Regular, hue, ascii, String.Format(text, args), to.NetState);
		}

		public void Say(int number, int hue)
		{
			PublicOverheadMessage(MessageType.Regular, hue, number);
		}

		public void Say(int number, string args, int hue)
		{
			PublicOverheadMessage(MessageType.Regular, hue, number, args);
		}

		public void Say(string text, int hue, bool ascii = false)
		{
			PublicOverheadMessage(MessageType.Regular, hue, ascii, text);
		}

		public void Say(bool ascii, string text)
		{
			PublicOverheadMessage(MessageType.Regular, m_SpeechHue, ascii, text);
		}

		public void Say(string text)
		{
			PublicOverheadMessage(MessageType.Regular, m_SpeechHue, false, text);
		}

		public void Say(string format, params object[] args)
		{
			Say(String.Format(format, args));
		}

		public void Say(int number, AffixType type, string affix, string args)
		{
			PublicOverheadMessage(MessageType.Regular, m_SpeechHue, number, type, affix, args);
		}

		public void Say(int number)
		{
			Say(number, "");
		}

		public void Say(int number, string args)
		{
			PublicOverheadMessage(MessageType.Regular, m_SpeechHue, number, args);
		}

		public void Emote(string text)
		{
			PublicOverheadMessage(MessageType.Emote, m_EmoteHue, false, text);
		}

		public void Emote(string format, params object[] args)
		{
			Emote(String.Format(format, args));
		}

		public void Emote(int number)
		{
			Emote(number, "");
		}

		public void Emote(int number, string args)
		{
			PublicOverheadMessage(MessageType.Emote, m_EmoteHue, number, args);
		}

		public void Whisper(string text)
		{
			PublicOverheadMessage(MessageType.Whisper, m_WhisperHue, false, text);
		}

		public void Whisper(string format, params object[] args)
		{
			Whisper(String.Format(format, args));
		}

		public void Whisper(int number)
		{
			Whisper(number, "");
		}

		public void Whisper(int number, string args)
		{
			PublicOverheadMessage(MessageType.Whisper, m_WhisperHue, number, args);
		}

		public void Yell(string text)
		{
			PublicOverheadMessage(MessageType.Yell, m_YellHue, false, text);
		}

		public void Yell(string format, params object[] args)
		{
			Yell(String.Format(format, args));
		}

		public void Yell(int number)
		{
			Yell(number, "");
		}

		public void Yell(int number, string args)
		{
			PublicOverheadMessage(MessageType.Yell, m_YellHue, number, args);
		}
		#endregion

		[CommandProperty(AccessLevel.Decorator)]
		public bool Blessed
		{
			get => m_Blessed;
			set
			{
				if (m_Blessed != value)
				{
					m_Blessed = value;
					Delta(MobileDelta.HealthbarYellow);
				}
			}
		}

		public void SendRemovePacket()
		{
			SendRemovePacket(true);
		}

		public void SendRemovePacket(bool everyone)
		{
			if (m_Map != null)
			{
				var eable = m_Map.GetClientsInRange(m_Location);

				foreach (var state in eable)
				{
					if (state != m_NetState && (everyone || !state.Mobile.CanSee(this)))
					{
						state.Send(RemovePacket);
					}
				}

				eable.Free();
			}
		}

		public void ClearScreen()
		{
			var ns = m_NetState;

			if (m_Map != null && ns != null)
			{
				var eable = m_Map.GetObjectsInRange(m_Location, Core.GlobalRadarRange - 4);

				foreach (var o in eable)
				{
					if (o is Mobile m)
					{
						if (m != this && Utility.InUpdateRange(m, m_Location, m.m_Location))
						{
							ns.Send(m.RemovePacket);
						}
					}
					else if (o is Item item && InRange(item.Location, item.GetUpdateRange(this)))
					{
						ns.Send(item.RemovePacket);
					}
				}

				eable.Free();
			}
		}

		public virtual bool SendSpeedControl(SpeedControlType type)
		{
			return Send(new SpeedControl(type));
		}

		public bool Send(Packet p)
		{
			return Send(p, false);
		}

		public bool Send(Packet p, bool throwOnOffline)
		{
			if (m_NetState != null)
			{
				m_NetState.Send(p);
				return true;
			}

			if (throwOnOffline)
			{
				throw new MobileNotConnectedException(this, "Packet could not be sent.");
			}

			return false;
		}

		#region Gumps/Menus
		public bool SendHuePicker(HuePicker p)
		{
			return SendHuePicker(p, false);
		}

		public bool SendHuePicker(HuePicker p, bool throwOnOffline)
		{
			if (m_NetState != null)
			{
				p.SendTo(m_NetState);
				return true;
			}

			if (throwOnOffline)
			{
				throw new MobileNotConnectedException(this, "Hue picker could not be sent.");
			}

			return false;
		}

		public Gump FindGump(Type type)
		{
			var ns = m_NetState;

			if (ns != null)
			{
				foreach (var gump in ns.Gumps)
				{
					if (type.IsAssignableFrom(gump.GetType()))
					{
						return gump;
					}
				}
			}

			return null;
		}

		public TGump FindGump<TGump>() where TGump : Gump
		{
			return FindGump(typeof(TGump)) as TGump;
		}

		public bool CloseGump(Type type)
		{
			if (m_NetState != null)
			{
				var gump = FindGump(type);

				if (gump != null)
				{
					m_NetState.Send(new CloseGump(gump.TypeID, 0));

					m_NetState.RemoveGump(gump);

					gump.OnServerClose(m_NetState);
				}

				return true;
			}

			return false;
		}

		[Obsolete("Use CloseGump( Type ) instead.")]
		public bool CloseGump(Type type, int buttonID)
		{
			return CloseGump(type);
		}

		[Obsolete("Use CloseGump( Type ) instead.")]
		public bool CloseGump(Type type, int buttonID, bool throwOnOffline)
		{
			return CloseGump(type);
		}

		public bool CloseAllGumps()
		{
			var ns = m_NetState;

			if (ns != null)
			{
				var gumps = new List<Gump>(ns.Gumps);

				ns.ClearGumps();

				foreach (var gump in gumps)
				{
					ns.Send(new CloseGump(gump.TypeID, 0));

					gump.OnServerClose(ns);
				}

				return true;
			}

			return false;
		}

		[Obsolete("Use CloseAllGumps() instead.", false)]
		public bool CloseAllGumps(bool throwOnOffline)
		{
			return CloseAllGumps();
		}

		public bool HasGump(Type type)
		{
			return FindGump(type) != null;
		}

		[Obsolete("Use HasGump( Type ) instead.", false)]
		public bool HasGump(Type type, bool throwOnOffline)
		{
			return HasGump(type);
		}

		public bool SendGump(Gump g)
		{
			return SendGump(g, false);
		}

		public bool SendGump(Gump g, bool throwOnOffline)
		{
			if (m_NetState != null)
			{
				g.SendTo(m_NetState);
				return true;
			}

			if (throwOnOffline)
			{
				throw new MobileNotConnectedException(this, "Gump could not be sent.");
			}

			return false;
		}

		public bool SendMenu(IMenu m)
		{
			return SendMenu(m, false);
		}

		public bool SendMenu(IMenu m, bool throwOnOffline)
		{
			if (m_NetState != null)
			{
				m.SendTo(m_NetState);
				return true;
			}

			if (throwOnOffline)
			{
				throw new MobileNotConnectedException(this, "Menu could not be sent.");
			}

			return false;
		}
		#endregion

		/// <summary>
		///     Overridable. Event invoked before the Mobile says something.
		///     <seealso cref="DoSpeech" />
		/// </summary>
		public virtual void OnSaid(SpeechEventArgs e)
		{
			if (m_Squelched)
			{
				SendLocalizedMessage(500168); // You can not say anything, you have been muted.
				e.Blocked = true;
			}

			if (!e.Blocked)
			{
				RevealingAction();
			}
		}

		public virtual bool HandlesOnSpeech(Mobile from)
		{
			return false;
		}

		/// <summary>
		///     Overridable. Virtual event invoked when the Mobile hears speech. This event will only be invoked if
		///     <see
		///         cref="HandlesOnSpeech" />
		///     returns true.
		///     <seealso cref="DoSpeech" />
		/// </summary>
		public virtual void OnSpeech(SpeechEventArgs e)
		{ }

		public void SendEverything()
		{
			var ns = m_NetState;

			if (m_Map != null && ns != null)
			{
				var eable = m_Map.GetObjectsInRange(m_Location, Core.GlobalRadarRange);

				foreach (var o in eable)
				{
					if (o is Item item)
					{
						if (InRange(item.GetWorldLocation(), item.GetUpdateRange(this)) && CanSee(item))
						{
							item.SendInfoTo(ns);
						}
					}
					else if (o is Mobile m && Utility.InUpdateRange(this, m) && CanSee(m))
					{
						ns.Send(MobileIncoming.Create(ns, this, m));

						if (ns.IsEnhancedClient)
						{
							ns.Send(new HealthbarPoisonEC(m));
							ns.Send(new HealthbarYellowEC(m));
						}
						else
						{
							ns.Send(new HealthbarPoison(m));
							ns.Send(new HealthbarYellow(m));
						}

						if (m.IsDeadBondedPet)
						{
							ns.Send(new BondedStatus(0, m.m_Serial, 1));
						}

						ns.Send(m.OPLPacket);
					}
				}

				eable.Free();
			}
		}

		public virtual void OnUpdateRangeChanged(int oldRange, int newRange)
		{
			ClearScreen();
			SendEverything();
		}

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Decorator)]
		public Map Map
		{
			get => m_Map;
			set
			{
				if (m_Deleted)
				{
					return;
				}

				if (m_Map != value)
				{
					if (m_NetState != null)
					{
						m_NetState.ValidateAllTrades();
					}

					var oldMap = m_Map;

					if (m_Map != null)
					{
						m_Map.OnLeave(this);

						ClearScreen();
						SendRemovePacket();
					}

					for (var i = 0; i < m_Items.Count; ++i)
					{
						m_Items[i].Map = value;
					}

					m_Map = value;

					UpdateRegion();

					if (m_Map != null)
					{
						m_Map.OnEnter(this);
					}

					var ns = m_NetState;

					if (ns != null && m_Map != null)
					{
						ns.Sequence = 0;
						ns.Send(new MapChange(this));
						ns.Send(new MapPatches());
						ns.Send(SeasonChange.Instantiate(GetSeason(), true));

						ns.Send(new MobileUpdate(this));

						ClearFastwalkStack();
					}

					if (ns != null)
					{
						if (m_Map != null)
						{
							ns.Send(new ServerChange(this, m_Map));
						}

						ns.Sequence = 0;
						ClearFastwalkStack();

						ns.Send(MobileIncoming.Create(ns, this, this));

						ns.Send(new MobileUpdate(this));
						CheckLightLevels(true);
						ns.Send(new MobileUpdate(this));
					}

					SendEverything();
					SendIncomingPacket();

					if (ns != null)
					{
						ns.Sequence = 0;
						ClearFastwalkStack();

						ns.Send(MobileIncoming.Create(ns, this, this));

						ns.Send(SupportedFeatures.Instantiate(ns));
						ns.Send(new MobileUpdate(this));
						ns.Send(new MobileAttributes(this));
					}

					OnMapChange(oldMap);
				}
			}
		}

		public void UpdateRegion()
		{
			if (m_Deleted)
			{
				return;
			}

			var newRegion = Region.Find(m_Location, m_Map);
			var oldRegion = m_Region;

			if (newRegion != oldRegion)
			{
				m_Region = newRegion;

				Region.OnRegionChange(this, oldRegion, newRegion);

				OnRegionChange(oldRegion, newRegion);
			}
		}

		/// <summary>
		///     Overridable. Virtual event invoked when <see cref="Map" /> changes.
		/// </summary>
		protected virtual void OnMapChange(Map oldMap)
		{ }

		#region Beneficial Checks/Actions
		public virtual bool CanBeBeneficial(Mobile target)
		{
			return CanBeBeneficial(target, true, false);
		}

		public virtual bool CanBeBeneficial(Mobile target, bool message)
		{
			return CanBeBeneficial(target, message, false);
		}

		public virtual bool CanBeBeneficial(Mobile target, bool message, bool allowDead)
		{
			if (target == null)
			{
				return false;
			}

			if (m_Deleted || target.m_Deleted || !Alive || IsDeadBondedPet ||
				(!allowDead && (!target.Alive || target.IsDeadBondedPet)))
			{
				if (message)
				{
					SendLocalizedMessage(1001017); // You can not perform beneficial acts on your target.
				}

				return false;
			}

			if (target == this)
			{
				return true;
			}

			if ( /*m_Player &&*/ !Region.AllowBeneficial(this, target))
			{
				// TODO: Pets
				//if ( !(target.m_Player || target.Body.IsHuman || target.Body.IsAnimal) )
				//{
				if (message)
				{
					SendLocalizedMessage(1001017); // You can not perform beneficial acts on your target.
				}

				return false;
				//}
			}

			return true;
		}

		public virtual bool IsBeneficialCriminal(Mobile target)
		{
			if (this == target)
			{
				return false;
			}

			var n = Notoriety.Compute(this, target);

			return n == Notoriety.Criminal || n == Notoriety.Murderer;
		}

		/// <summary>
		///     Overridable. Event invoked when the Mobile <see cref="DoBeneficial">does a beneficial action</see>.
		/// </summary>
		public virtual void OnBeneficialAction(Mobile target, bool isCriminal)
		{
			if (isCriminal)
			{
				CriminalAction(false);
			}
		}

		public virtual void DoBeneficial(Mobile target)
		{
			if (target == null)
			{
				return;
			}

			OnBeneficialAction(target, IsBeneficialCriminal(target));

			Region.OnBeneficialAction(this, target);
			target.Region.OnGotBeneficialAction(this, target);
		}

		public virtual bool BeneficialCheck(Mobile target)
		{
			if (CanBeBeneficial(target, true))
			{
				DoBeneficial(target);
				return true;
			}

			return false;
		}
		#endregion

		#region Harmful Checks/Actions
		public virtual bool CanBeHarmful(IDamageable target)
		{
			return CanBeHarmful(target, true);
		}

		public virtual bool CanBeHarmful(IDamageable target, bool message)
		{
			return CanBeHarmful(target, message, false);
		}

		public virtual bool CanBeHarmful(IDamageable target, bool message, bool ignoreOurBlessedness)
		{
			return CanBeHarmful(target, message, ignoreOurBlessedness, false);
		}

		public virtual bool CanBeHarmful(IDamageable target, bool message, bool ignoreOurBlessedness, bool ignorePeaceCheck)
		{
			if (target == null)
			{
				return false;
			}

			if (m_Deleted || (!ignoreOurBlessedness && m_Blessed) || !Alive || IsDeadBondedPet || target.Deleted)
			{
				if (message)
				{
					SendLocalizedMessage(1001018); // You can not perform negative acts on your target.
				}

				return false;
			}

			if (target is Mobile mobile)
			{
				if (mobile.m_Blessed || !mobile.Alive || mobile.IsDeadBondedPet)
				{
					if (message)
					{
						SendLocalizedMessage(1001018); // You can not perform negative acts on your target.
					}

					return false;
				}

				if (!mobile.CanBeHarmedBy(this, message))
				{
					return false;
				}
			}

			if (target == this)
			{
				return true;
			}

			// TODO: Pets
			if ( /*m_Player &&*/ !Region.AllowHarmful(this, target))
			//(target.m_Player || target.Body.IsHuman) && !Region.AllowHarmful( this, target )  )
			{
				if (message)
				{
					SendLocalizedMessage(1001018); // You can not perform negative acts on your target.
				}

				return false;
			}

			return true;
		}

		public virtual bool CanBeHarmedBy(Mobile from, bool message)
		{
			return true;
		}

		public virtual bool IsHarmfulCriminal(IDamageable target)
		{
			if (this == target)
			{
				return false;
			}

			return Notoriety.Compute(this, target) == Notoriety.Innocent;
		}

		/// <summary>
		///     Overridable. Event invoked when the Mobile <see cref="OnHarmfulAction">does a harmful action</see>.
		/// </summary>
		public virtual void OnHarmfulAction(IDamageable target, bool isCriminal)
		{
			if (isCriminal)
			{
				CriminalAction(false);
			}
		}

		public virtual void DoHarmful(IDamageable target)
		{
			DoHarmful(target, false);
		}

		public virtual void DoHarmful(IDamageable target, bool indirect)
		{
			if (target == null || m_Deleted)
			{
				return;
			}

			var isCriminal = IsHarmfulCriminal(target);

			OnHarmfulAction(target, isCriminal);

			if (target is Mobile mobile)
			{
				mobile.AggressiveAction(this, isCriminal);
			}

			Region.OnDidHarmful(this, target);

			if (target is Mobile harmed)
			{
				harmed.Region.OnGotHarmful(this, harmed);
			}
			else if (target is Item)
			{
				Region.Find(target.Location, target.Map).OnGotHarmful(this, target);
			}

			if (!indirect)
			{
				Combatant = target;
			}

			CheckExpireCombatantTimer();

		}

		public virtual bool HarmfulCheck(IDamageable target)
		{
			if (CanBeHarmful(target))
			{
				DoHarmful(target);
				return true;
			}

			return false;
		}
		#endregion

		#region Stats
		/// <summary>
		///     Gets a list of all <see cref="StatMod">StatMod's</see> currently active for the Mobile.
		/// </summary>
		public List<StatMod> StatMods => m_StatMods;

		public bool RemoveStatMod(string name)
		{
			for (var i = 0; i < m_StatMods.Count; ++i)
			{
				var check = m_StatMods[i];

				if (check.Name == name)
				{
					m_StatMods.RemoveAt(i);
					CheckStatTimers();
					Delta(MobileDelta.Stat | GetStatDelta(check.Type));
					ProcessDelta();
					return true;
				}
			}

			return false;
		}

		public StatMod GetStatMod(string name)
		{
			for (var i = 0; i < m_StatMods.Count; ++i)
			{
				var check = m_StatMods[i];

				if (check.Name == name)
				{
					return check;
				}
			}

			return null;
		}

		public void AddStatMod(StatMod mod)
		{
			for (var i = 0; i < m_StatMods.Count; ++i)
			{
				var check = m_StatMods[i];

				if (check.Name == mod.Name)
				{
					Delta(MobileDelta.Stat | GetStatDelta(check.Type));
					m_StatMods.RemoveAt(i);
					break;
				}
			}

			m_StatMods.Add(mod);
			Delta(MobileDelta.Stat | GetStatDelta(mod.Type));
			CheckStatTimers();
		}

		private MobileDelta GetStatDelta(StatType type)
		{
			MobileDelta delta = 0;

			if ((type & StatType.Str) != 0)
			{
				delta |= MobileDelta.Hits;
			}

			if ((type & StatType.Dex) != 0)
			{
				delta |= MobileDelta.Stam;
			}

			if ((type & StatType.Int) != 0)
			{
				delta |= MobileDelta.Mana;
			}

			return delta;
		}

		/// <summary>
		///     Computes the total modified offset for the specified stat type. Expired <see cref="StatMod" /> instances are removed.
		/// </summary>
		public int GetStatOffset(StatType type)
		{
			var offset = 0;

			for (var i = 0; i < m_StatMods.Count; ++i)
			{
				var mod = m_StatMods[i];

				if (mod.HasElapsed())
				{
					m_StatMods.RemoveAt(i);
					Delta(MobileDelta.Stat | GetStatDelta(mod.Type));
					CheckStatTimers();

					--i;
				}
				else if ((mod.Type & type) != 0)
				{
					offset += mod.Offset;
				}
			}

			return offset;
		}

		/// <summary>
		///     Overridable. Virtual event invoked when the <see cref="RawStr" /> changes.
		///     <seealso cref="RawStr" />
		///     <seealso cref="OnRawStatChange" />
		/// </summary>
		public virtual void OnRawStrChange(int oldValue)
		{ }

		/// <summary>
		///     Overridable. Virtual event invoked when <see cref="RawDex" /> changes.
		///     <seealso cref="RawDex" />
		///     <seealso cref="OnRawStatChange" />
		/// </summary>
		public virtual void OnRawDexChange(int oldValue)
		{ }

		/// <summary>
		///     Overridable. Virtual event invoked when the <see cref="RawInt" /> changes.
		///     <seealso cref="RawInt" />
		///     <seealso cref="OnRawStatChange" />
		/// </summary>
		public virtual void OnRawIntChange(int oldValue)
		{ }

		/// <summary>
		///     Overridable. Virtual event invoked when the <see cref="RawStr" />, <see cref="RawDex" />, or <see cref="RawInt" /> changes.
		///     <seealso cref="OnRawStrChange" />
		///     <seealso cref="OnRawDexChange" />
		///     <seealso cref="OnRawIntChange" />
		/// </summary>
		public virtual void OnRawStatChange(StatType stat, int oldValue)
		{ }

		/// <summary>
		///     Gets or sets the base, unmodified, strength of the Mobile. Ranges from 1 to 65000, inclusive.
		///     <seealso cref="Str" />
		///     <seealso cref="StatMod" />
		///     <seealso cref="OnRawStrChange" />
		///     <seealso cref="OnRawStatChange" />
		/// </summary>
		[CommandProperty(AccessLevel.GameMaster)]
		public int RawStr
		{
			get => m_Str;
			set
			{
				if (value < 1)
				{
					value = 1;
				}
				else if (value > 65000)
				{
					value = 65000;
				}

				if (m_Str != value)
				{
					var oldValue = m_Str;

					m_Str = value;
					Delta(MobileDelta.Stat | MobileDelta.Hits);

					if (CanRegenHits)
					{
						if (Hits < HitsMax)
						{
							TimerRegistry.Register(
								Player ? _HitsRegenTimerPlayerID : _HitsRegenTimerID,
								this,
								GetHitsRegenRate(this),
								Player ? TimeSpan.FromMilliseconds(50) : TimeSpan.FromMilliseconds(250),
								false, TimerPriority.TenMS,
								mobile => mobile.HitsOnTick());
						}
						else if (Hits > HitsMax)
						{
							Hits = HitsMax;
						}
					}
					else
					{
						TimerRegistry.RemoveFromRegistry(Player ? _HitsRegenTimerPlayerID : _HitsRegenTimerID, this);
					}

					OnRawStrChange(oldValue);
					OnRawStatChange(StatType.Str, oldValue);
				}
			}
		}

		/// <summary>
		///     Gets or sets the effective strength of the Mobile. This is the sum of the <see cref="RawStr" /> plus any additional modifiers. Any attempts to set this value when under the influence of a
		///     <see
		///         cref="StatMod" />
		///     will result in no change. It ranges from 1 to 65000, inclusive.
		///     <seealso cref="RawStr" />
		///     <seealso cref="StatMod" />
		/// </summary>
		[CommandProperty(AccessLevel.GameMaster)]
		public virtual int Str
		{
			get
			{
				var value = m_Str + GetStatOffset(StatType.Str);

				if (value < 1)
				{
					value = 1;
				}
				else if (value > 65000)
				{
					value = 65000;
				}

				return value;
			}
			set
			{
				if (m_StatMods.Count == 0)
				{
					RawStr = value;
				}
			}
		}

		/// <summary>
		///     Gets or sets the base, unmodified, dexterity of the Mobile. Ranges from 1 to 65000, inclusive.
		///     <seealso cref="Dex" />
		///     <seealso cref="StatMod" />
		///     <seealso cref="OnRawDexChange" />
		///     <seealso cref="OnRawStatChange" />
		/// </summary>
		[CommandProperty(AccessLevel.GameMaster)]
		public int RawDex
		{
			get => m_Dex;
			set
			{
				if (value < 1)
				{
					value = 1;
				}
				else if (value > 65000)
				{
					value = 65000;
				}

				if (m_Dex != value)
				{
					var oldValue = m_Dex;

					m_Dex = value;
					Delta(MobileDelta.Stat | MobileDelta.Stam);

					if (CanRegenStam)
					{
						if (Stam < StamMax)
						{
							TimerRegistry.Register(Player ? _StamRegenTimerPlayerID : _StamRegenTimerID,
								this,
								GetStamRegenRate(this),
								Player ? TimeSpan.FromMilliseconds(50) : TimeSpan.FromMilliseconds(250),
								false, TimerPriority.TenMS,
								mobile => mobile.StamOnTick());
						}
						else if (Stam > StamMax)
						{
							Stam = StamMax;
						}
					}
					else
					{
						TimerRegistry.RemoveFromRegistry(Player ? _StamRegenTimerPlayerID : _StamRegenTimerID, this);
					}

					OnRawDexChange(oldValue);
					OnRawStatChange(StatType.Dex, oldValue);
				}
			}
		}

		/// <summary>
		///     Gets or sets the effective dexterity of the Mobile. This is the sum of the <see cref="RawDex" /> plus any additional modifiers. Any attempts to set this value when under the influence of a
		///     <see
		///         cref="StatMod" />
		///     will result in no change. It ranges from 1 to 65000, inclusive.
		///     <seealso cref="RawDex" />
		///     <seealso cref="StatMod" />
		/// </summary>
		[CommandProperty(AccessLevel.GameMaster)]
		public virtual int Dex
		{
			get
			{
				var value = m_Dex + GetStatOffset(StatType.Dex);

				if (value < 1)
				{
					value = 1;
				}
				else if (value > 65000)
				{
					value = 65000;
				}

				return value;
			}
			set
			{
				if (m_StatMods.Count == 0)
				{
					RawDex = value;
				}
			}
		}

		/// <summary>
		///     Gets or sets the base, unmodified, intelligence of the Mobile. Ranges from 1 to 65000, inclusive.
		///     <seealso cref="Int" />
		///     <seealso cref="StatMod" />
		///     <seealso cref="OnRawIntChange" />
		///     <seealso cref="OnRawStatChange" />
		/// </summary>
		[CommandProperty(AccessLevel.GameMaster)]
		public int RawInt
		{
			get => m_Int;
			set
			{
				if (value < 1)
				{
					value = 1;
				}
				else if (value > 65000)
				{
					value = 65000;
				}

				if (m_Int != value)
				{
					var oldValue = m_Int;

					m_Int = value;
					Delta(MobileDelta.Stat | MobileDelta.Mana);

					if (CanRegenMana)
					{
						if (Mana < ManaMax)
						{
							TimerRegistry.Register(
								Player ? _ManaRegenTimerPlayerID : _ManaRegenTimerID,
								this,
								GetManaRegenRate(this), Player ? TimeSpan.FromMilliseconds(50) : TimeSpan.FromMilliseconds(250),
								false, TimerPriority.TenMS,
								mobile => mobile.ManaOnTick());
						}
						else if (Mana > ManaMax)
						{
							Mana = ManaMax;
						}
					}
					else
					{
						TimerRegistry.RemoveFromRegistry(Player ? _ManaRegenTimerPlayerID : _ManaRegenTimerID, this);
					}

					OnRawIntChange(oldValue);
					OnRawStatChange(StatType.Int, oldValue);
				}
			}
		}

		/// <summary>
		///     Gets or sets the effective intelligence of the Mobile. This is the sum of the <see cref="RawInt" /> plus any additional modifiers. Any attempts to set this value when under the influence of a
		///     <see
		///         cref="StatMod" />
		///     will result in no change. It ranges from 1 to 65000, inclusive.
		///     <seealso cref="RawInt" />
		///     <seealso cref="StatMod" />
		/// </summary>
		[CommandProperty(AccessLevel.GameMaster)]
		public virtual int Int
		{
			get
			{
				var value = m_Int + GetStatOffset(StatType.Int);

				if (value < 1)
				{
					value = 1;
				}
				else if (value > 65000)
				{
					value = 65000;
				}

				return value;
			}
			set
			{
				if (m_StatMods.Count == 0)
				{
					RawInt = value;
				}
			}
		}

		public virtual void OnHitsChange(int oldValue)
		{ }

		public virtual void OnStamChange(int oldValue)
		{ }

		public virtual void OnManaChange(int oldValue)
		{ }

		/// <summary>
		///     Gets or sets the current hit point of the Mobile. This value ranges from 0 to <see cref="HitsMax" />, inclusive. When set to the value of
		///     <see
		///         cref="HitsMax" />
		///     , the <see cref="AggressorInfo.CanReportMurder">CanReportMurder</see> flag of all aggressors is reset to false, and the list of damage entries is cleared.
		/// </summary>
		[CommandProperty(AccessLevel.GameMaster)]
		public int Hits
		{
			get => m_Hits;
			set
			{
				if (m_Deleted)
				{
					return;
				}

				if (value < 0)
				{
					value = 0;
				}
				else if (value >= HitsMax)
				{
					value = HitsMax;

					for (var i = 0; i < m_Aggressors.Count; i++) //reset reports on full HP
					{
						m_Aggressors[i].CanReportMurder = false;
					}

					if (m_DamageEntries.Count > 0)
					{
						m_DamageEntries.Clear(); // reset damage entries on full HP
					}
				}

				if (m_Hits != value)
				{
					var oldValue = m_Hits;
					m_Hits = value;
					Delta(MobileDelta.Hits);
					OnHitsChange(oldValue);
				}

				if (m_Hits < HitsMax)
				{
					if (CanRegenHits)
					{
						TimerRegistry.Register(
							Player ? _HitsRegenTimerPlayerID : _HitsRegenTimerID,
							this, GetHitsRegenRate(this),
							Player ? TimeSpan.FromMilliseconds(50) : TimeSpan.FromMilliseconds(250),
							false,
							TimerPriority.TenMS,
							mobile => mobile.HitsOnTick());
					}
					else
					{
						TimerRegistry.RemoveFromRegistry(Player ? _HitsRegenTimerPlayerID : _HitsRegenTimerID, this);
					}
				}
				else
				{
					TimerRegistry.RemoveFromRegistry(Player ? _HitsRegenTimerPlayerID : _HitsRegenTimerID, this);
				}
			}
		}

		/// <summary>
		///     Overridable. Gets the maximum hit point of the Mobile. By default, this returns:
		///     <c>
		///         50 + (<see cref="Str" /> / 2)
		///     </c>
		/// </summary>
		[CommandProperty(AccessLevel.GameMaster)]
		public virtual int HitsMax => 50 + (Str / 2);

		/// <summary>
		///     Gets or sets the current stamina of the Mobile. This value ranges from 0 to <see cref="StamMax" />, inclusive.
		/// </summary>
		[CommandProperty(AccessLevel.GameMaster)]
		public int Stam
		{
			get => m_Stam;
			set
			{
				if (m_Deleted)
				{
					return;
				}

				if (value < 0)
				{
					value = 0;
				}
				else if (value >= StamMax)
				{
					value = StamMax;
				}

				if (m_Stam != value)
				{
					var oldValue = m_Stam;
					m_Stam = value;
					Delta(MobileDelta.Stam);
					OnStamChange(oldValue);
				}

				if (m_Stam < StamMax)
				{
					if (CanRegenStam)
					{
						TimerRegistry.Register(
							Player ? _StamRegenTimerPlayerID : _StamRegenTimerID,
							this,
							GetStamRegenRate(this),
							Player ? TimeSpan.FromMilliseconds(50) : TimeSpan.FromMilliseconds(250),
							false,
							TimerPriority.TenMS,
							mobile => mobile.StamOnTick());
					}
					else
					{
						TimerRegistry.RemoveFromRegistry(Player ? _StamRegenTimerPlayerID : _StamRegenTimerID, this);
					}
				}
				else
				{
					TimerRegistry.RemoveFromRegistry(Player ? _StamRegenTimerPlayerID : _StamRegenTimerID, this);
				}
			}
		}

		/// <summary>
		///     Overridable. Gets the maximum stamina of the Mobile. By default, this returns:
		///     <c>
		///         <see cref="Dex" />
		///     </c>
		/// </summary>
		[CommandProperty(AccessLevel.GameMaster)]
		public virtual int StamMax => Dex;

		/// <summary>
		///     Gets or sets the current stamina of the Mobile. This value ranges from 0 to <see cref="ManaMax" />, inclusive.
		/// </summary>
		[CommandProperty(AccessLevel.GameMaster)]
		public int Mana
		{
			get => m_Mana;
			set
			{
				if (m_Deleted)
				{
					return;
				}

				if (value < 0)
				{
					value = 0;
				}
				else if (value >= ManaMax)
				{
					value = ManaMax;

					if (Meditating)
					{
						Meditating = false;
						SendLocalizedMessage(501846); // You are at peace.
					}
				}

				if (m_Mana != value)
				{
					var oldValue = m_Mana;
					m_Mana = value;
					Delta(MobileDelta.Mana);
					OnManaChange(oldValue);
				}

				if (m_Mana < ManaMax)
				{
					if (CanRegenMana)
					{
						TimerRegistry.Register(
							Player ? _ManaRegenTimerPlayerID : _ManaRegenTimerID,
							this, GetManaRegenRate(this),
							Player ? TimeSpan.FromMilliseconds(50) : TimeSpan.FromMilliseconds(250),
							false,
							TimerPriority.TenMS,
							mobile => mobile.ManaOnTick());
					}
					else
					{
						TimerRegistry.RemoveFromRegistry(Player ? _ManaRegenTimerPlayerID : _ManaRegenTimerID, this);
					}
				}
				else
				{
					TimerRegistry.RemoveFromRegistry(Player ? _ManaRegenTimerPlayerID : _ManaRegenTimerID, this);
				}
			}
		}

		/// <summary>
		///     Overridable. Gets the maximum mana of the Mobile. By default, this returns:
		///     <c>
		///         <see cref="Int" />
		///     </c>
		/// </summary>
		[CommandProperty(AccessLevel.GameMaster)]
		public virtual int ManaMax => Int;
		#endregion

		public virtual int Luck => 0;

		public virtual int HuedItemID => m_Female ? 0x2107 : 0x2106;

		private int m_HueMod = -1;

		[Hue, CommandProperty(AccessLevel.Decorator)]
		public int HueMod
		{
			get => m_HueMod;
			set
			{
				if (m_HueMod != value)
				{
					m_HueMod = value;

					Delta(MobileDelta.Hue);
				}
			}
		}

		[Hue, CommandProperty(AccessLevel.Decorator)]
		public virtual int Hue
		{
			get
			{
				if (m_HueMod != -1)
				{
					return m_HueMod;
				}

				return m_Hue;
			}
			set
			{
				var oldHue = m_Hue;

				if (oldHue != value)
				{
					m_Hue = value;

					Delta(MobileDelta.Hue);
				}
			}
		}

		public void SetDirection(Direction dir)
		{
			m_Direction = dir;
		}

		[CommandProperty(AccessLevel.Decorator)]
		public Direction Direction
		{
			get => m_Direction;
			set
			{
				if (m_Direction != value)
				{
					m_Direction = value;

					Delta(MobileDelta.Direction);
					//ProcessDelta();
				}
			}
		}

		public virtual int GetSeason()
		{
			if (m_Map != null)
			{
				return m_Map.Season;
			}

			return 1;
		}

		public virtual int GetPacketFlags()
		{
			var flags = 0x0;

			if (m_Paralyzed || m_Frozen || (m_Spell != null && !m_Spell.CheckMovement(this)))
			{
				flags |= 0x01;
			}

			if (m_Female)
			{
				flags |= 0x02;
			}

			if (m_Flying)
			{
				flags |= 0x04;
			}

			if (m_Blessed || m_YellowHealthbar)
			{
				flags |= 0x08;
			}

			if (m_Warmode)
			{
				flags |= 0x40;
			}

			if (m_Hidden)
			{
				flags |= 0x80;
			}

			if (m_IgnoreMobiles)
			{
				flags |= 0x10;
			}

			return flags;
		}

		// Pre-7.0.0.0 Packet Flags
		public virtual int GetOldPacketFlags()
		{
			var flags = 0x0;

			if (m_Paralyzed || m_Frozen)
			{
				flags |= 0x01;
			}

			if (m_Female)
			{
				flags |= 0x02;
			}

			if (m_Poison != null)
			{
				flags |= 0x04;
			}

			if (m_Blessed || m_YellowHealthbar)
			{
				flags |= 0x08;
			}

			if (m_Warmode)
			{
				flags |= 0x40;
			}

			if (m_Hidden)
			{
				flags |= 0x80;
			}

			if (m_IgnoreMobiles)
			{
				flags |= 0x10;
			}

			return flags;
		}

		[CommandProperty(AccessLevel.Decorator)]
		public bool Female
		{
			get => m_Female;
			set
			{
				if (m_Female != value)
				{
					m_Female = value;
					Delta(MobileDelta.Flags);
					OnGenderChanged(!m_Female);
				}
			}
		}

		public virtual void OnGenderChanged(bool oldFemale)
		{ }

		[CommandProperty(AccessLevel.Decorator)]
		public bool Flying
		{
			get => m_Flying;
			set
			{
				if (m_Flying != value)
				{
					m_Flying = value;
					Delta(MobileDelta.Flags);
				}
			}
		}

		#region Stygian Abyss
		public virtual void ToggleFlying()
		{ }
		#endregion

		[CommandProperty(AccessLevel.Decorator)]
		public bool Warmode
		{
			get => m_Warmode;
			set
			{
				if (m_Deleted)
				{
					return;
				}

				if (m_Warmode != value)
				{
					if (m_AutoManifestTimer != null)
					{
						m_AutoManifestTimer.Stop();
						m_AutoManifestTimer = null;
					}

					m_Warmode = value;
					Delta(MobileDelta.Flags);

					if (m_NetState != null)
					{
						Send(SetWarMode.Instantiate(value));
					}

					if (!m_Warmode)
					{
						Combatant = null;
					}

					if (!Alive)
					{
						if (value)
						{
							Delta(MobileDelta.GhostUpdate);
						}
						else
						{
							SendRemovePacket(false);
						}
					}

					NextActionTime = Core.TickCount + ActionDelay;

					OnWarmodeChanged();
				}
			}
		}

		/// <summary>
		///     Overridable. Virtual event invoked after the Warmode property has changed.
		/// </summary>
		public virtual void OnWarmodeChanged()
		{ }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Hidden
		{
			get => m_Hidden;
			set
			{
				if (m_Hidden != value)
				{
					m_Hidden = value;

					if (m_Hidden)
					{
						if (Warmode)
						{
							Warmode = false;
						}
						else
						{
							Combatant = null;
						}
					}

					OnHiddenChanged();
				}
			}
		}

		public virtual void OnHiddenChanged()
		{
			m_AllowedStealthSteps = 0;

			if (m_Map != null)
			{
				var eable = m_Map.GetClientsInRange(m_Location);

				foreach (var state in eable)
				{
					if (!state.Mobile.CanSee(this))
					{
						state.Send(RemovePacket);
					}
					else
					{
						state.Send(MobileIncoming.Create(state, state.Mobile, this));

						if (IsDeadBondedPet)
						{
							state.Send(new BondedStatus(0, m_Serial, 1));
						}

						state.Send(OPLPacket);
					}
				}

				eable.Free();
			}
		}

		public virtual void OnConnected()
		{ }

		public virtual void OnDisconnected()
		{ }

		public virtual void OnNetStateChanged()
		{ }

		[CommandProperty(AccessLevel.GameMaster, AccessLevel.Administrator)]
		public NetState NetState
		{
			get
			{
				if (m_NetState != null && m_NetState.Socket == null && !m_NetState.IsDisposing)
				{
					NetState = null;
				}

				return m_NetState;
			}
			set
			{
				if (m_NetState != value)
				{
					if (m_Map != null)
					{
						m_Map.OnClientChange(m_NetState, value, this);
					}

					if (m_Target != null)
					{
						m_Target.Cancel(this, TargetCancelType.Disconnected);
					}

					if (m_QuestArrow != null)
					{
						QuestArrow = null;
					}

					if (m_Spell != null)
					{
						m_Spell.OnConnectionChanged();
					}

					//if ( m_Spell != null )
					//	m_Spell.FinishSequence();

					if (m_NetState != null)
					{
						m_NetState.CancelAllTrades();
					}

					var box = FindBankNoCreate();

					if (box != null && box.Opened)
					{
						box.Close();
					}

					// REMOVED:
					//m_Actions.Clear();

					m_NetState = value;

					if (m_NetState == null)
					{
						OnDisconnected();
						EventSink.InvokeDisconnected(new DisconnectedEventArgs(this));

						// Disconnected, start the logout timer
						var logoutDelay = GetLogoutDelay();
						if (!TimerRegistry.UpdateRegistry(_LogoutTimerID, this, logoutDelay))
						{
							TimerRegistry.Register(_LogoutTimerID, this, logoutDelay, TimerPriority.OneSecond, m => m.DoLogout());
						}
					}
					else
					{
						OnConnected();
						EventSink.InvokeConnected(new ConnectedEventArgs(this));

						// Connected, stop the logout timer and if needed, move to the world

						if (TimerRegistry.HasTimer(_LogoutTimerID, this))
						{
							TimerRegistry.RemoveFromRegistry(_LogoutTimerID, this);
						}

						if (m_Map == Map.Internal && m_LogoutMap != null)
						{
							CharacterOut = true;
							Map = m_LogoutMap;
							Location = m_LogoutLocation;
						}
						else
						{
							CharacterOut = false;
						}
					}

					for (var i = m_Items.Count - 1; i >= 0; --i)
					{
						if (i >= m_Items.Count)
						{
							continue;
						}

						var item = m_Items[i];

						if (item is SecureTradeContainer)
						{
							for (var j = item.Items.Count - 1; j >= 0; --j)
							{
								if (j < item.Items.Count)
								{
									item.Items[j].OnSecureTrade(this, this, this, false);
									AddToBackpack(item.Items[j]);
								}
							}

							Timer.DelayCall(TimeSpan.Zero, delegate { item.Delete(); });
						}
					}

					DropHolding();
					OnNetStateChanged();
				}
			}
		}

		public virtual bool CanSee(object o)
		{
			if (o is Item item)
			{
				return CanSee(item);
			}

			if (o is Mobile mobile)
			{
				return CanSee(mobile);
			}

			return true;
		}

		public virtual bool CanSee(Item item)
		{
			if (m_Map == Map.Internal)
			{
				return false;
			}

			if (item.Map == Map.Internal)
			{
				return false;
			}

			if (item.Parent != null)
			{
				if (item.Parent is Item itemParent)
				{
					if (!(CanSee(itemParent) && itemParent.IsChildVisibleTo(this, item)))
					{
						return false;
					}
				}
				else if (item.Parent is Mobile mobileParent && !CanSee(mobileParent))
				{
					return false;
				}
			}

			if (item is BankBox box)
			{
				if (IsPlayer() && (box.Owner != this || !box.Opened))
				{
					return false;
				}
			}
			else if (item is SecureTradeContainer container)
			{
				var trade = container.Trade;

				if (trade != null && trade.From.Mobile != this && trade.To.Mobile != this)
				{
					return false;
				}
			}

			return !item.Deleted && item.Map == m_Map && (item.Visible || IsStaff());
		}

		public virtual bool CanSee(Mobile m)
		{
			if (m_Deleted || m.m_Deleted || m_Map == Map.Internal || m.m_Map == Map.Internal)
			{
				return false;
			}

			return this == m ||
				   (m.m_Map == m_Map && (!m.Hidden || (IsStaff() && m_AccessLevel >= m.AccessLevel)) &&
					(m.Alive || (Skills.SpiritSpeak.Value >= 100.0) || !Alive || IsStaff() || m.Warmode));
		}

		public virtual bool CanBeRenamedBy(Mobile from)
		{
			return from.AccessLevel >= AccessLevel.Decorator && from.m_AccessLevel > m_AccessLevel;
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public string Language
		{
			get => m_Language;
			set
			{
				if (m_Language != value)
				{
					m_Language = value;
				}
			}
		}

		[CommandProperty(AccessLevel.Decorator)]
		public int SpeechHue { get => m_SpeechHue; set => m_SpeechHue = value; }

		[CommandProperty(AccessLevel.Decorator)]
		public int EmoteHue { get => m_EmoteHue; set => m_EmoteHue = value; }

		[CommandProperty(AccessLevel.Decorator)]
		public int WhisperHue { get => m_WhisperHue; set => m_WhisperHue = value; }

		[CommandProperty(AccessLevel.Decorator)]
		public int YellHue { get => m_YellHue; set => m_YellHue = value; }

		[CommandProperty(AccessLevel.Decorator)]
		public string GuildTitle
		{
			get => m_GuildTitle;
			set
			{
				var old = m_GuildTitle;

				if (old != value)
				{
					m_GuildTitle = value;

					if (m_Guild != null && !m_Guild.Disbanded && m_GuildTitle != null)
					{
						SendLocalizedMessage(1018026, true, m_GuildTitle); // Your guild title has changed :
					}

					InvalidateProperties();

					OnGuildTitleChange(old);
				}
			}
		}

		public virtual void OnGuildTitleChange(string oldTitle)
		{ }

		[CommandProperty(AccessLevel.Decorator)]
		public bool DisplayGuildAbbr
		{
			get => m_DisplayGuildAbbr;
			set
			{
				m_DisplayGuildAbbr = value;
				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.Decorator)]
		public bool DisplayGuildTitle
		{
			get => m_DisplayGuildTitle;
			set
			{
				m_DisplayGuildTitle = value;
				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.Decorator)]
		public Mobile GuildFealty { get => m_GuildFealty; set => m_GuildFealty = value; }

		private string m_NameMod;

		[CommandProperty(AccessLevel.Decorator)]
		public string NameMod
		{
			get => m_NameMod;
			set
			{
				if (m_NameMod != value)
				{
					m_NameMod = value;
					Delta(MobileDelta.Name);
					InvalidateProperties();
				}
			}
		}

		private bool m_YellowHealthbar;

		[CommandProperty(AccessLevel.Decorator)]
		public bool YellowHealthbar
		{
			get => m_YellowHealthbar;
			set
			{
				m_YellowHealthbar = value;
				Delta(MobileDelta.HealthbarYellow);
			}
		}

		[CommandProperty(AccessLevel.Decorator)]
		public string RawName { get => m_Name; set => Name = value; }

		[CommandProperty(AccessLevel.Decorator)]
		public virtual string TitleName => m_Name;

		[CommandProperty(AccessLevel.Decorator)]
		public string Name
		{
			get
			{
				if (m_NameMod != null)
				{
					return m_NameMod;
				}

				return m_Name;
			}
			set
			{
				if (m_Name != value) // I'm leaving out the && m_NameMod == null
				{
					var oldName = m_Name;
					m_Name = value;
					OnAfterNameChange(oldName, m_Name);
					Delta(MobileDelta.Name);
					InvalidateProperties();
				}
			}
		}

		public virtual void OnAfterNameChange(string oldName, string newName)
		{ }

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime LastStrGain { get => m_LastStrGain; set => m_LastStrGain = value; }

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime LastIntGain { get => m_LastIntGain; set => m_LastIntGain = value; }

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime LastDexGain { get => m_LastDexGain; set => m_LastDexGain = value; }

		public DateTime LastStatGain
		{
			get
			{
				var d = m_LastStrGain;

				if (m_LastIntGain > d)
				{
					d = m_LastIntGain;
				}

				if (m_LastDexGain > d)
				{
					d = m_LastDexGain;
				}

				return d;
			}
			set
			{
				m_LastStrGain = value;
				m_LastIntGain = value;
				m_LastDexGain = value;
			}
		}

		public BaseGuild Guild
		{
			get => m_Guild;
			set
			{
				var old = m_Guild;

				if (old != value)
				{
					if (value == null)
					{
						GuildTitle = null;
					}

					m_Guild = value;

					Delta(MobileDelta.Noto);
					InvalidateProperties();

					OnGuildChange(old);
				}
			}
		}

		public virtual void OnGuildChange(BaseGuild oldGuild)
		{ }

		#region Poison/Curing
		public Timer PoisonTimer => m_PoisonTimer;

		[CommandProperty(AccessLevel.GameMaster)]
		public Poison Poison
		{
			get => m_Poison;
			set
			{
				/*if ( m_Poison != value && (m_Poison == null || value == null || m_Poison.Level < value.Level) )
				{*/
				m_Poison = value;
				Delta(MobileDelta.HealthbarPoison);

				if (m_PoisonTimer != null)
				{
					m_PoisonTimer.Stop();
					m_PoisonTimer = null;
				}

				if (m_Poison != null)
				{
					m_PoisonTimer = m_Poison.ConstructTimer(this);

					if (m_PoisonTimer != null)
					{
						m_PoisonTimer.Start();
					}
				}

				CheckStatTimers();
				/*}*/
			}
		}

		/// <summary>
		///     Overridable. Event invoked when a call to <see cref="ApplyPoison" /> failed because <see cref="CheckPoisonImmunity" /> returned false: the Mobile was resistant to the poison. By default, this broadcasts an overhead message: * The poison seems to have no effect. *
		///     <seealso cref="CheckPoisonImmunity" />
		///     <seealso cref="ApplyPoison" />
		///     <seealso cref="Poison" />
		/// </summary>
		public virtual void OnPoisonImmunity(Mobile from, Poison poison)
		{
			PublicOverheadMessage(MessageType.Emote, 0x3B2, 1005534); // * The poison seems to have no effect. *
		}

		/// <summary>
		///     Overridable. Virtual event invoked when a call to <see cref="ApplyPoison" /> failed because
		///     <see
		///         cref="CheckHigherPoison" />
		///     returned false: the Mobile was already poisoned by an equal or greater strength poison.
		///     <seealso cref="CheckHigherPoison" />
		///     <seealso cref="ApplyPoison" />
		///     <seealso cref="Poison" />
		/// </summary>
		public virtual void OnHigherPoison(Mobile from, Poison poison)
		{ }

		/// <summary>
		///     Overridable. Event invoked when a call to <see cref="ApplyPoison" /> succeeded. By default, this broadcasts an overhead message varying by the level of the poison. Example: * Zippy begins to spasm uncontrollably. *
		///     <seealso cref="ApplyPoison" />
		///     <seealso cref="Poison" />
		/// </summary>
		public virtual void OnPoisoned(Mobile from, Poison poison, Poison oldPoison)
		{
			if (poison != null)
			{
				LocalOverheadMessage(MessageType.Regular, 0x21, 1042857 + (poison.RealLevel * 2));
				NonlocalOverheadMessage(MessageType.Regular, 0x21, 1042858 + (poison.RealLevel * 2), Name);
			}
		}

		/// <summary>
		///     Overridable. Called from <see cref="ApplyPoison" />, this method checks if the Mobile is immune to some
		///     <see
		///         cref="Poison" />
		///     . If true, <see cref="OnPoisonImmunity" /> will be invoked and
		///     <see
		///         cref="ApplyPoisonResult.Immune" />
		///     is returned.
		///     <seealso cref="OnPoisonImmunity" />
		///     <seealso cref="ApplyPoison" />
		///     <seealso cref="Poison" />
		/// </summary>
		public virtual bool CheckPoisonImmunity(Mobile from, Poison poison)
		{
			return false;
		}

		/// <summary>
		///     Overridable. Called from <see cref="ApplyPoison" />, this method checks if the Mobile is already poisoned by some
		///     <see
		///         cref="Poison" />
		///     of equal or greater strength. If true, <see cref="OnHigherPoison" /> will be invoked and
		///     <see
		///         cref="ApplyPoisonResult.HigherPoisonActive" />
		///     is returned.
		///     <seealso cref="OnHigherPoison" />
		///     <seealso cref="ApplyPoison" />
		///     <seealso cref="Poison" />
		/// </summary>
		public virtual bool CheckHigherPoison(Mobile from, Poison poison)
		{
			#region Mondain's Legacy
			return m_Poison != null && m_Poison.RealLevel >= poison.RealLevel;
			#endregion
		}

		/// <summary>
		///     Overridable. Attempts to apply poison to the Mobile. Checks are made such that no <see cref="CheckHigherPoison">higher poison is active</see> and that the Mobile is not
		///     <see
		///         cref="CheckPoisonImmunity">
		///         immune to the poison
		///     </see>
		///     . Provided those assertions are true, the
		///     <paramref
		///         name="poison" />
		///     is applied and <see cref="OnPoisoned" /> is invoked.
		///     <seealso cref="Poison" />
		///     <seealso cref="CurePoison" />
		/// </summary>
		/// <returns>
		///     One of four possible values:
		///     <list type="table">
		///         <item>
		///             <term>
		///                 <see cref="ApplyPoisonResult.Cured">Cured</see>
		///             </term>
		///             <description>
		///                 The <paramref name="poison" /> parameter was null and so <see cref="CurePoison" /> was invoked.
		///             </description>
		///         </item>
		///         <item>
		///             <term>
		///                 <see cref="ApplyPoisonResult.HigherPoisonActive">HigherPoisonActive</see>
		///             </term>
		///             <description>
		///                 The call to <see cref="CheckHigherPoison" /> returned false.
		///             </description>
		///         </item>
		///         <item>
		///             <term>
		///                 <see cref="ApplyPoisonResult.Immune">Immune</see>
		///             </term>
		///             <description>
		///                 The call to <see cref="CheckPoisonImmunity" /> returned false.
		///             </description>
		///         </item>
		///         <item>
		///             <term>
		///                 <see cref="ApplyPoisonResult.Poisoned">Poisoned</see>
		///             </term>
		///             <description>
		///                 The <paramref name="poison" /> was successfully applied.
		///             </description>
		///         </item>
		///     </list>
		/// </returns>
		public virtual ApplyPoisonResult ApplyPoison(Mobile from, Poison poison)
		{
			if (poison == null)
			{
				CurePoison(from);
				return ApplyPoisonResult.Cured;
			}

			if (CheckHigherPoison(from, poison))
			{
				OnHigherPoison(from, poison);
				return ApplyPoisonResult.HigherPoisonActive;
			}

			if (CheckPoisonImmunity(from, poison))
			{
				OnPoisonImmunity(from, poison);
				return ApplyPoisonResult.Immune;
			}

			var oldPoison = m_Poison;
			Poison = poison;

			OnPoisoned(from, poison, oldPoison);

			return ApplyPoisonResult.Poisoned;
		}

		/// <summary>
		///     Overridable. Called from <see cref="CurePoison" />, this method checks to see that the Mobile can be cured of
		///     <see
		///         cref="Poison" />
		///     <seealso cref="CurePoison" />
		///     <seealso cref="Poison" />
		/// </summary>
		public virtual bool CheckCure(Mobile from)
		{
			return true;
		}

		/// <summary>
		///     Overridable. Virtual event invoked when a call to <see cref="CurePoison" /> succeeded.
		///     <seealso cref="CurePoison" />
		///     <seealso cref="CheckCure" />
		///     <seealso cref="Poison" />
		/// </summary>
		public virtual void OnCured(Mobile from, Poison oldPoison)
		{ }

		/// <summary>
		///     Overridable. Virtual event invoked when a call to <see cref="CurePoison" /> failed.
		///     <seealso cref="CurePoison" />
		///     <seealso cref="CheckCure" />
		///     <seealso cref="Poison" />
		/// </summary>
		public virtual void OnFailedCure(Mobile from)
		{ }

		/// <summary>
		///     Overridable. Attempts to cure any poison that is currently active.
		/// </summary>
		/// <returns>True if poison was cured, false if otherwise.</returns>
		public virtual bool CurePoison(Mobile from)
		{
			if (CheckCure(from))
			{
				var oldPoison = m_Poison;
				Poison = null;

				OnCured(from, oldPoison);

				return true;
			}

			OnFailedCure(from);

			return false;
		}
		#endregion

		private ISpawner m_Spawner;

		public ISpawner Spawner { get => m_Spawner; set => m_Spawner = value; }

		public Region WalkRegion { get; set; }

		public virtual void OnBeforeSpawn(Point3D location, Map m)
		{ }

		public virtual void OnAfterSpawn()
		{ }

		protected virtual void OnCreate()
		{ }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Poisoned => m_Poison != null;

		[CommandProperty(AccessLevel.GameMaster)]
		public bool IsBodyMod => m_BodyMod.BodyID != 0;

		[CommandProperty(AccessLevel.Decorator)]
		public Body BodyMod
		{
			get => m_BodyMod;
			set
			{
				if (m_BodyMod != value)
				{
					m_BodyMod = value;

					Delta(MobileDelta.Body);
					InvalidateProperties();

					CheckStatTimers();
				}
			}
		}

		private static readonly int[] m_InvalidBodies =
		{
			//32,		// Dunno why is blocked
			//95,		// Used for Turkey
			//156,		// Dunno why is blocked
			//197,		// ML Dragon
			//198,		// ML Dragon
		};

		[Body, CommandProperty(AccessLevel.GameMaster)]
		public Body RawBody
		{
			get => m_Body;
			set
			{
				if (m_Body != value)
				{
					m_Body = SafeBody(value);

					if (!IsBodyMod)
					{
						Delta(MobileDelta.Body);
						InvalidateProperties();

						CheckStatTimers();
					}
				}
			}
		}

		[Body, CommandProperty(AccessLevel.Decorator)]
		public Body Body
		{
			get
			{
				if (IsBodyMod)
				{
					return m_BodyMod;
				}

				return m_Body;
			}
			set
			{
				if (m_Body != value && !IsBodyMod)
				{
					m_Body = SafeBody(value);

					Delta(MobileDelta.Body);
					InvalidateProperties();

					CheckStatTimers();
				}
			}
		}

		public virtual int SafeBody(int body)
		{
			var delta = -1;

			for (var i = 0; delta < 0 && i < m_InvalidBodies.Length; ++i)
			{
				delta = m_InvalidBodies[i] - body;
			}

			if (delta != 0)
			{
				return body;
			}

			return 0;
		}

		[Body, CommandProperty(AccessLevel.Decorator)]
		public int BodyValue { get => Body.BodyID; set => Body = value; }

		[CommandProperty(AccessLevel.Counselor)]
		public Serial Serial => m_Serial;

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Decorator)]
		public Point3D Location { get => m_Location; set => SetLocation(value, true); }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public Point3D LogoutLocation { get => m_LogoutLocation; set => m_LogoutLocation = value; }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public Map LogoutMap { get => m_LogoutMap; set => m_LogoutMap = value; }

		public Region Region
		{
			get
			{
				if (m_Region == null)
				{
					if (Map == null)
					{
						return Map.Internal.DefaultRegion;
					}

					return Map.DefaultRegion;
				}

				return m_Region;
			}
		}

		public void FreeCache()
		{
			Packet.Release(ref m_RemovePacket);
			Packet.Release(ref m_PropertyList);
			Packet.Release(ref m_OPLPacket);
		}

		private Packet m_RemovePacket;
		private readonly object rpLock = new object();

		public Packet RemovePacket
		{
			get
			{
				if (m_RemovePacket == null)
				{
					lock (rpLock)
					{
						if (m_RemovePacket == null)
						{
							m_RemovePacket = new RemoveMobile(this);
							m_RemovePacket.SetStatic();
						}
					}
				}

				return m_RemovePacket;
			}
		}

		private Packet m_OPLPacket;
		private readonly object oplLock = new object();

		public Packet OPLPacket
		{
			get
			{
				if (m_OPLPacket == null)
				{
					lock (oplLock)
					{
						if (m_OPLPacket == null)
						{
							m_OPLPacket = new OPLInfo(PropertyList);
							m_OPLPacket.SetStatic();
						}
					}
				}

				return m_OPLPacket;
			}
		}

		private ObjectPropertyList m_PropertyList;

		public ObjectPropertyList PropertyList
		{
			get
			{
				if (m_PropertyList == null)
				{
					m_PropertyList = new ObjectPropertyList(this);

					GetProperties(m_PropertyList);

					m_PropertyList.Terminate();
					m_PropertyList.SetStatic();
				}

				return m_PropertyList;
			}
		}

		public void ClearProperties()
		{
			Packet.Release(ref m_PropertyList);
			Packet.Release(ref m_OPLPacket);
		}

		public void InvalidateProperties()
		{
			if (m_Map != null && m_Map != Map.Internal && !World.Loading)
			{
				var oldList = m_PropertyList;
				Packet.Release(ref m_PropertyList);
				var newList = PropertyList;

				if (oldList == null || oldList.Hash != newList.Hash)
				{
					Packet.Release(ref m_OPLPacket);
					Delta(MobileDelta.Properties);
				}
			}
			else
			{
				ClearProperties();
			}
		}

		private int m_SolidHueOverride = -1;

		[CommandProperty(AccessLevel.Decorator)]
		public int SolidHueOverride
		{
			get => m_SolidHueOverride;
			set
			{
				if (m_SolidHueOverride == value)
				{
					return;
				}
				m_SolidHueOverride = value;
				Delta(MobileDelta.Hue | MobileDelta.Body);
			}
		}

		public virtual void MoveToWorld(Point3D newLocation, Map map)
		{
			if (m_Deleted)
			{
				return;
			}

			if (m_Map == map)
			{
				SetLocation(newLocation, true);
				return;
			}

			var box = FindBankNoCreate();

			if (box != null && box.Opened)
			{
				box.Close();
			}

			var oldLocation = m_Location;
			var oldMap = m_Map;

			if (oldMap != null)
			{
				oldMap.OnLeave(this);

				ClearScreen();
				SendRemovePacket();
			}

			foreach (var o in m_Items)
			{
				o.Map = map;
			}

			m_Map = map;

			m_Location = newLocation;

			var ns = m_NetState;

			if (ns != null)
			{
				ns.Sequence = 0;
				ClearFastwalkStack();
			}

			if (m_Map != null)
			{
				m_Map.OnEnter(this);
			}

			UpdateRegion();

			if (m_Map != null && ns != null)
			{
				ns.Send(new MapChange(this));
				ns.Send(new MapPatches());

				ns.Send(SeasonChange.Instantiate(GetSeason(), true));

				Send(new ServerChange(this, m_Map));

				ns.Send(MobileIncoming.Create(ns, this, this));

				ns.Send(new MobileUpdate(this));

				ns.Send(new MobileAttributes(this));

				CheckLightLevels(true);

				ns.Send(SupportedFeatures.Instantiate(ns));
			}

			SendEverything();
			SendIncomingPacket();

			OnMapChange(oldMap);
			OnLocationChange(oldLocation);

			if (m_Region != null)
			{
				m_Region.OnLocationChanged(this, oldLocation);
			}
		}

		public virtual void SetLocation(Point3D newLocation, bool isTeleport)
		{
			if (m_Deleted)
			{
				return;
			}

			var oldLocation = m_Location;

			if (oldLocation != newLocation)
			{
				m_Location = newLocation;
				UpdateRegion();

				if (AccessLevel <= AccessLevel.Counselor)
				{
					var box = FindBankNoCreate();

					if (box != null && box.Opened)
					{
						box.Close();
					}
				}

				if (m_NetState != null)
				{
					m_NetState.ValidateAllTrades();
				}

				if (m_Map != null)
				{
					m_Map.OnMove(oldLocation, this);
				}

				if (isTeleport && m_NetState != null && !NoMoveHS)
				{
					m_NetState.Sequence = 0;

					m_NetState.Send(new MobileUpdate(this));

					ClearFastwalkStack();

					EventSink.InvokeTeleportMovement(new TeleportMovementEventArgs(this, oldLocation, newLocation));
				}

				var map = m_Map;

				if (map != null)
				{
					// First, send a remove message to everyone who can no longer see us. (inOldRange && !inNewRange)

					var eable = map.GetClientsInRange(oldLocation);

					foreach (var ns in eable)
					{
						if (ns != m_NetState && !Utility.InUpdateRange(ns.Mobile, newLocation, ns.Mobile))
						{
							ns.Send(RemovePacket);
						}
					}

					eable.Free();

					Packet hbpPacket = Packet.Acquire(new HealthbarPoison(this)),
						   hbyPacket = Packet.Acquire(new HealthbarYellow(this));

					Packet hbpKRPacket = Packet.Acquire(new HealthbarPoisonEC(this)),
						   hbyKRPacket = Packet.Acquire(new HealthbarYellowEC(this));

					var ourState = m_NetState;

					// Check to see if we are attached to a client
					if (ourState != null)
					{
						var eeable = map.GetObjectsInRange(newLocation, Core.GlobalRadarRange);

						// We are attached to a client, so it's a bit more complex. We need to send new items and people to ourself, and ourself to other clients
						foreach (var o in eeable)
						{
							if (o is Item item)
							{
								var range = item.GetUpdateRange(this);
								var loc = item.GetWorldLocation();

								if (!Utility.InRange(oldLocation, loc, range) && Utility.InRange(newLocation, loc, range) && CanSee(item))
								{
									item.SendInfoTo(ourState);
								}
							}
							else if (o != this && o is Mobile m)
							{
								// Will we enter their update range? (Y: Update)
								var update = Utility.InUpdateRange(m, newLocation, m);

								// Were we already in their update range? (Y: Cancel Update)
								if (update && Utility.InUpdateRange(m, oldLocation, m))
								{
									update = false;
								}

								if (m.m_NetState != null && (update || (isTeleport && !NoMoveHS)) && m.CanSee(this))
								{
									m.m_NetState.Send(MobileIncoming.Create(m.m_NetState, m, this));

									if (m.m_NetState.IsEnhancedClient)
									{
										m.m_NetState.Send(hbpKRPacket);
										m.m_NetState.Send(hbyKRPacket);
									}
									else
									{
										m.m_NetState.Send(hbpPacket);
										m.m_NetState.Send(hbyPacket);
									}

									if (IsDeadBondedPet)
									{
										m.m_NetState.Send(new BondedStatus(0, m_Serial, 1));
									}

									m.m_NetState.Send(OPLPacket);
								}

								// Will they enter in our update range? (Y: Update)
								update = Utility.InUpdateRange(this, newLocation, m);

								// Were they already in our update range? (Y: Cancel Update)
								if (update && Utility.InUpdateRange(this, oldLocation, m))
								{
									update = false;
								}

								if (update && CanSee(m))
								{
									ourState.Send(MobileIncoming.Create(ourState, this, m));

									if (ourState.IsEnhancedClient)
									{
										ourState.Send(new HealthbarPoisonEC(m));
										ourState.Send(new HealthbarYellowEC(m));
									}
									else
									{
										ourState.Send(new HealthbarPoison(m));
										ourState.Send(new HealthbarYellow(m));
									}

									if (m.IsDeadBondedPet)
									{
										ourState.Send(new BondedStatus(0, m.m_Serial, 1));
									}

									ourState.Send(m.OPLPacket);
								}
							}
						}

						eeable.Free();
					}
					else
					{
						eable = map.GetClientsInRange(newLocation);

						// We're not attached to a client, so simply send an Incoming
						foreach (var ns in eable)
						{
							var update = Utility.InUpdateRange(ns.Mobile, newLocation, ns.Mobile);

							if (update && Utility.InUpdateRange(ns.Mobile, oldLocation, ns.Mobile))
							{
								update = false;
							}

							if ((update || (isTeleport && !NoMoveHS)) && ns.Mobile.CanSee(this))
							{
								ns.Send(MobileIncoming.Create(ns, ns.Mobile, this));

								if (ns.IsEnhancedClient)
								{
									ns.Send(hbpKRPacket);
									ns.Send(hbyKRPacket);
								}
								else
								{
									ns.Send(hbpPacket);
									ns.Send(hbyPacket);
								}

								if (IsDeadBondedPet)
								{
									ns.Send(new BondedStatus(0, m_Serial, 1));
								}

								ns.Send(OPLPacket);
							}
						}

						eable.Free();
					}

					Packet.Release(hbpKRPacket);
					Packet.Release(hbyKRPacket);
					Packet.Release(hbpPacket);
					Packet.Release(hbyPacket);
				}

				OnLocationChange(oldLocation);

				Region.OnLocationChanged(this, oldLocation);
			}
		}

		/// <summary>
		///     Overridable. Virtual event invoked when <see cref="Location" /> changes.
		/// </summary>
		protected virtual void OnLocationChange(Point3D oldLocation)
		{ }

		#region Hair & Face
		private HairInfo m_Hair;
		private FacialHairInfo m_FacialHair;
		private FaceInfo m_Face;

		[CommandProperty(AccessLevel.Decorator)]
		public int HairItemID
		{
			get
			{
				if (m_Hair == null)
				{
					return 0;
				}

				return m_Hair.ItemID;
			}
			set
			{
				if (m_Hair == null && value > 0)
				{
					m_Hair = new HairInfo(value);
				}
				else if (value <= 0)
				{
					m_Hair = null;
				}
				else
				{
					m_Hair.ItemID = value;
				}

				Delta(MobileDelta.Hair);
			}
		}

		[CommandProperty(AccessLevel.Decorator)]
		public int FacialHairItemID
		{
			get
			{
				if (m_FacialHair == null)
				{
					return 0;
				}

				return m_FacialHair.ItemID;
			}
			set
			{
				if (m_FacialHair == null && value > 0)
				{
					m_FacialHair = new FacialHairInfo(value);
				}
				else if (value <= 0)
				{
					m_FacialHair = null;
				}
				else
				{
					m_FacialHair.ItemID = value;
				}

				Delta(MobileDelta.FacialHair);
			}
		}

		[CommandProperty(AccessLevel.Decorator)]
		public int HairHue
		{
			get
			{
				if (m_Hair == null)
				{
					return 0;
				}
				return m_Hair.Hue;
			}
			set
			{
				if (m_Hair != null)
				{
					m_Hair.Hue = value;
					Delta(MobileDelta.Hair);
				}
			}
		}

		[CommandProperty(AccessLevel.Decorator)]
		public int FacialHairHue
		{
			get
			{
				if (m_FacialHair == null)
				{
					return 0;
				}

				return m_FacialHair.Hue;
			}
			set
			{
				if (m_FacialHair != null)
				{
					m_FacialHair.Hue = value;
					Delta(MobileDelta.FacialHair);
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int FaceItemID
		{
			get
			{
				if (m_Face == null)
				{
					return 0;
				}

				return m_Face.ItemID;
			}
			set
			{
				if (m_Face == null && value > 0)
				{
					m_Face = new FaceInfo(value);
				}
				else if (value <= 0)
				{
					m_Face = null;
				}
				else
				{
					m_Face.ItemID = value;
				}

				Delta(MobileDelta.Face);
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int FaceHue
		{
			get
			{
				if (m_Face == null)
				{
					return Hue;
				}

				return m_Face.Hue;
			}
			set
			{
				if (m_Face != null)
				{
					m_Face.Hue = value;
					Delta(MobileDelta.Face);
				}
			}
		}
		#endregion

		public bool HasFreeHand()
		{
			return FindItemOnLayer(Layer.TwoHanded) == null;
		}

		private IWeapon m_Weapon;

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual IWeapon Weapon
		{
			get
			{
				var item = m_Weapon as Item;

				if (item != null && !item.Deleted && item.Parent == this && CanSee(item))
				{
					return m_Weapon;
				}

				m_Weapon = null;

				item = FindItemOnLayer(Layer.OneHanded);

				if (item == null)
				{
					item = FindItemOnLayer(Layer.TwoHanded);
				}

				if (item is IWeapon weapon)
				{
					return m_Weapon = weapon;
				}

				return GetDefaultWeapon();
			}
		}

		public virtual IWeapon GetDefaultWeapon()
		{
			return m_DefaultWeapon;
		}

		private BankBox m_BankBox;

		[CommandProperty(AccessLevel.GameMaster)]
		public BankBox BankBox
		{
			get
			{
				if (m_BankBox != null && !m_BankBox.Deleted && m_BankBox.Parent == this)
				{
					return m_BankBox;
				}

				m_BankBox = FindItemOnLayer(Layer.Bank) as BankBox;

				if (m_BankBox == null)
				{
					AddItem(m_BankBox = new BankBox(this));
				}

				return m_BankBox;
			}
		}

		public BankBox FindBankNoCreate()
		{
			if (m_BankBox != null && !m_BankBox.Deleted && m_BankBox.Parent == this)
			{
				return m_BankBox;
			}

			m_BankBox = FindItemOnLayer(Layer.Bank) as BankBox;

			return m_BankBox;
		}

		private Container m_Backpack;

		[CommandProperty(AccessLevel.GameMaster)]
		public Container Backpack
		{
			get
			{
				if (m_Backpack != null && !m_Backpack.Deleted && m_Backpack.Parent == this)
				{
					return m_Backpack;
				}

				return m_Backpack = FindItemOnLayer(Layer.Backpack) as Container;
			}
		}

		public virtual bool KeepsItemsOnDeath => IsStaff();

		public Item FindItemOnLayer(Layer layer)
		{
			var eq = m_Items;
			var count = eq.Count;

			for (var i = 0; i < count; ++i)
			{
				var item = eq[i];

				if (!item.Deleted && item.Layer == layer)
				{
					return item;
				}
			}

			return null;
		}

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Decorator)]
		public int X { get => m_Location.m_X; set => Location = new Point3D(value, m_Location.m_Y, m_Location.m_Z); }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Decorator)]
		public int Y { get => m_Location.m_Y; set => Location = new Point3D(m_Location.m_X, value, m_Location.m_Z); }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Decorator)]
		public int Z { get => m_Location.m_Z; set => Location = new Point3D(m_Location.m_X, m_Location.m_Y, value); }

		#region Effects & Particles
		public void MovingEffect(
			IEntity to, int itemID, int speed, int duration, bool fixedDirection, bool explodes, int hue, int renderMode)
		{
			Effects.SendMovingEffect(this, to, itemID, speed, duration, fixedDirection, explodes, hue, renderMode);
		}

		public void MovingEffect(IEntity to, int itemID, int speed, int duration, bool fixedDirection, bool explodes)
		{
			Effects.SendMovingEffect(this, to, itemID, speed, duration, fixedDirection, explodes, 0, 0);
		}

		public void MovingParticles(
			IEntity to,
			int itemID,
			int speed,
			int duration,
			bool fixedDirection,
			bool explodes,
			int hue,
			int renderMode,
			int effect,
			int explodeEffect,
			int explodeSound,
			EffectLayer layer,
			int unknown)
		{
			Effects.SendMovingParticles(
				this,
				to,
				itemID,
				speed,
				duration,
				fixedDirection,
				explodes,
				hue,
				renderMode,
				effect,
				explodeEffect,
				explodeSound,
				layer,
				unknown);
		}

		public void MovingParticles(
			IEntity to,
			int itemID,
			int speed,
			int duration,
			bool fixedDirection,
			bool explodes,
			int hue,
			int renderMode,
			int effect,
			int explodeEffect,
			int explodeSound,
			int unknown)
		{
			Effects.SendMovingParticles(
				this,
				to,
				itemID,
				speed,
				duration,
				fixedDirection,
				explodes,
				hue,
				renderMode,
				effect,
				explodeEffect,
				explodeSound,
				(EffectLayer)255,
				unknown);
		}

		public void MovingParticles(
			IEntity to,
			int itemID,
			int speed,
			int duration,
			bool fixedDirection,
			bool explodes,
			int effect,
			int explodeEffect,
			int explodeSound,
			int unknown)
		{
			Effects.SendMovingParticles(
				this, to, itemID, speed, duration, fixedDirection, explodes, effect, explodeEffect, explodeSound, unknown);
		}

		public void MovingParticles(
			IEntity to,
			int itemID,
			int speed,
			int duration,
			bool fixedDirection,
			bool explodes,
			int effect,
			int explodeEffect,
			int explodeSound)
		{
			Effects.SendMovingParticles(
				this, to, itemID, speed, duration, fixedDirection, explodes, 0, 0, effect, explodeEffect, explodeSound, 0);
		}

		public void FixedEffect(int itemID, int speed, int duration, int hue, int renderMode)
		{
			Effects.SendTargetEffect(this, itemID, speed, duration, hue, renderMode);
		}

		public void FixedEffect(int itemID, int speed, int duration)
		{
			Effects.SendTargetEffect(this, itemID, speed, duration, 0, 0);
		}

		public void FixedParticles(
			int itemID, int speed, int duration, int effect, int hue, int renderMode, EffectLayer layer, int unknown)
		{
			Effects.SendTargetParticles(this, itemID, speed, duration, hue, renderMode, effect, layer, unknown);
		}

		public void FixedParticles(
			int itemID, int speed, int duration, int effect, int hue, int renderMode, EffectLayer layer)
		{
			Effects.SendTargetParticles(this, itemID, speed, duration, hue, renderMode, effect, layer, 0);
		}

		public void FixedParticles(int itemID, int speed, int duration, int effect, EffectLayer layer, int unknown)
		{
			Effects.SendTargetParticles(this, itemID, speed, duration, 0, 0, effect, layer, unknown);
		}

		public void FixedParticles(int itemID, int speed, int duration, int effect, EffectLayer layer)
		{
			Effects.SendTargetParticles(this, itemID, speed, duration, 0, 0, effect, layer, 0);
		}

		public void BoltEffect(int hue)
		{
			Effects.SendBoltEffect(this, true, hue);
		}
		#endregion

		public void SendIncomingPacket()
		{
			if (m_Map != null)
			{
				var eable = m_Map.GetClientsInRange(m_Location);

				foreach (var state in eable)
				{
					if (state.Mobile.CanSee(this))
					{
						state.Send(MobileIncoming.Create(state, state.Mobile, this));

						if (m_Poison != null)
						{
							if (state.IsEnhancedClient)
							{
								state.Send(new HealthbarPoisonEC(this));
							}
							else
							{
								state.Send(new HealthbarPoison(this));
							}
						}

						if (m_Blessed || m_YellowHealthbar)
						{
							if (state.IsEnhancedClient)
							{
								state.Send(new HealthbarYellowEC(this));
							}
							else
							{
								state.Send(new HealthbarYellow(this));
							}
						}

						if (IsDeadBondedPet)
						{
							state.Send(new BondedStatus(0, m_Serial, 1));
						}

						state.Send(OPLPacket);
					}
				}

				eable.Free();
			}
		}

		public bool PlaceInBackpack(Item item)
		{
			if (item.Deleted)
			{
				return false;
			}

			var pack = Backpack;

			return pack != null && pack.TryDropItem(this, item, false);
		}

		public bool AddToBackpack(Item item)
		{
			if (item.Deleted)
			{
				return false;
			}

			if (!PlaceInBackpack(item))
			{
				var loc = m_Location;
				var map = m_Map;

				if ((map == null || map == Map.Internal) && m_LogoutMap != null)
				{
					loc = m_LogoutLocation;
					map = m_LogoutMap;
				}

				item.MoveToWorld(loc, map);
				return false;
			}

			return true;
		}

		public virtual bool CheckLift(Mobile from, Item item, ref LRReason reject)
		{
			return true;
		}

		public virtual bool CheckNonlocalLift(Mobile from, Item item)
		{
			if (from == this || (from.AccessLevel > AccessLevel && from.AccessLevel >= AccessLevel.GameMaster))
			{
				return true;
			}

			return false;
		}

		public bool HasTrade
		{
			get
			{
				if (m_NetState != null)
				{
					return m_NetState.Trades.Count > 0;
				}

				return false;
			}
		}

		public virtual bool CheckTrade(
			Mobile to, Item item, SecureTradeContainer cont, bool message, bool checkItems, int plusItems, int plusWeight)
		{
			return true;
		}

		public bool OpenTrade(Mobile from)
		{
			return OpenTrade(from, null);
		}

		public virtual bool OpenTrade(Mobile from, Item offer)
		{
			if (!from.Player || !Player || !from.Alive || !Alive)
			{
				return false;
			}

			var ourState = m_NetState;
			var theirState = from.m_NetState;

			if (ourState == null || theirState == null)
			{
				return false;
			}

			var cont = theirState.FindTradeContainer(this);

			if (!from.CheckTrade(this, offer, cont, true, true, 0, 0))
			{
				return false;
			}

			if (cont == null)
			{
				cont = theirState.AddTrade(ourState);
			}

			if (offer != null)
			{
				cont.DropItem(offer);
			}

			return true;
		}

		/// <summary>
		///     Overridable. Event invoked when a Mobile (<paramref name="from" />) drops an
		///     <see cref="Item">
		///         <paramref name="dropped" />
		///     </see>
		///     onto the Mobile.
		/// </summary>
		public virtual bool OnDragDrop(Mobile from, Item dropped)
		{
			if (from == this)
			{
				var pack = Backpack;

				if (pack != null)
				{
					dropped.GridLocation = 0x0;
					return dropped.DropToItem(from, pack, new Point3D(-1, -1, 0));
				}

				return false;
			}

			if (from.InRange(Location, 2))
			{
				return OpenTrade(from, dropped);
			}

			return false;
		}

		public virtual bool CheckEquip(Item item)
		{
			for (var i = 0; i < m_Items.Count; ++i)
			{
				if (m_Items[i].CheckConflictingLayer(this, item, item.Layer) ||
					item.CheckConflictingLayer(this, m_Items[i], m_Items[i].Layer))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		///     Overridable. Virtual event invoked when the Mobile attempts to wear <paramref name="item" />.
		/// </summary>
		/// <returns>True if the request is accepted, false if otherwise.</returns>
		public virtual bool OnEquip(Item item)
		{
			// For some reason OSI allows equipping quest items, but they are unmarked in the process
			if (item.QuestItem)
			{
				item.QuestItem = false;
				SendLocalizedMessage(1074769);
				// An item must be in your backpack (and not in a container within) to be toggled as a quest item.
			}

			return true;
		}

		/// <summary>
		///     Overridable. Virtual event invoked when the Mobile attempts to lift <paramref name="item" />.
		/// </summary>
		/// <returns>True if the lift is allowed, false if otherwise.</returns>
		/// <example>
		///     The following example demonstrates usage. It will disallow any attempts to pick up a pick axe if the Mobile does not have enough strength.
		///     <code>
		///  public override bool OnDragLift( Item item )
		///  {
		/// 		if ( item is Pickaxe &amp;&amp; this.Str &lt; 60 )
		/// 		{
		/// 			SendMessage( "That is too heavy for you to lift." );
		/// 			return false;
		/// 		}
		///
		/// 		return base.OnDragLift( item );
		///  }</code>
		/// </example>
		public virtual bool OnDragLift(Item item)
		{
			return true;
		}

		/// <summary>
		///     Overridable. Virtual event invoked when the Mobile attempts to drop <paramref name="item" /> into a
		///     <see cref="Container">
		///         <paramref name="container" />
		///     </see>
		///     .
		/// </summary>
		/// <returns>True if the drop is allowed, false if otherwise.</returns>
		public virtual bool OnDroppedItemInto(Item item, Container container, Point3D loc)
		{
			return true;
		}

		/// <summary>
		///     Overridable. Virtual event invoked when the Mobile attempts to drop <paramref name="item" /> directly onto another
		///     <see
		///         cref="Item" />
		///     , <paramref name="target" />. This is the case of stacking items.
		/// </summary>
		/// <returns>True if the drop is allowed, false if otherwise.</returns>
		public virtual bool OnDroppedItemOnto(Item item, Item target)
		{
			return true;
		}

		/// <summary>
		///     Overridable. Virtual event invoked when the Mobile attempts to drop <paramref name="item" /> into another
		///     <see
		///         cref="Item" />
		///     , <paramref name="target" />. The target item is most likely a <see cref="Container" />.
		/// </summary>
		/// <returns>True if the drop is allowed, false if otherwise.</returns>
		public virtual bool OnDroppedItemToItem(Item item, Item target, Point3D loc)
		{
			return true;
		}

		/// <summary>
		///     Overridable. Virtual event invoked when the Mobile attempts to give <paramref name="item" /> to a Mobile (
		///     <paramref
		///         name="target" />
		///     ).
		/// </summary>
		/// <returns>True if the drop is allowed, false if otherwise.</returns>
		public virtual bool OnDroppedItemToMobile(Item item, Mobile target)
		{
			return true;
		}

		/// <summary>
		///     Overridable. Virtual event invoked when the Mobile attempts to drop <paramref name="item" /> to the world at a
		///     <see cref="Point3D">
		///         <paramref name="location" />
		///     </see>
		///     .
		/// </summary>
		/// <returns>True if the drop is allowed, false if otherwise.</returns>
		public virtual bool OnDroppedItemToWorld(Item item, Point3D location)
		{
			return true;
		}

		/// <summary>
		///     Overridable. Virtual event when <paramref name="from" /> successfully uses <paramref name="item" /> while it's on this Mobile.
		///     <seealso cref="Item.OnItemUsed" />
		/// </summary>
		public virtual void OnItemUsed(Mobile from, Item item)
		{
			EventSink.InvokeOnItemUse(new OnItemUseEventArgs(from, item));
		}

		public virtual bool CheckHasTradeDrop(Mobile from, Item item, Item target)
		{
			return true;
		}

		public virtual bool CheckNonlocalDrop(Mobile from, Item item, Item target)
		{
			if (from == this || (from.AccessLevel > AccessLevel && from.AccessLevel >= AccessLevel.GameMaster))
			{
				return true;
			}

			return false;
		}

		public virtual bool CheckItemUse(Mobile from, Item item)
		{
			return true;
		}

		/// <summary>
		///     Overridable. Virtual event invoked when <paramref name="from" /> successfully lifts <paramref name="item" /> from this Mobile.
		///     <seealso cref="Item.OnItemLifted" />
		/// </summary>
		public virtual void OnItemLifted(Mobile from, Item item)
		{ }

		public virtual bool AllowItemUse(Item item)
		{
			return true;
		}

		public virtual bool AllowEquipFrom(Mobile mob)
		{
			return mob == this || (mob.AccessLevel >= AccessLevel.Decorator && mob.AccessLevel > AccessLevel);
		}

		public virtual bool EquipItem(Item item)
		{
			if (item == null || item.Deleted || !item.CanEquip(this))
			{
				return false;
			}

			if (CheckEquip(item) && OnEquip(item) && item.OnEquip(this))
			{
				if (m_Spell != null && !m_Spell.OnCasterEquiping(item))
				{
					return false;
				}

				//if ( m_Spell != null && m_Spell.State == SpellState.Casting )
				//	m_Spell.Disturb( DisturbType.EquipRequest );

				AddItem(item);
				return true;
			}

			return false;
		}

		internal int m_TypeRef;

		public Mobile(Serial serial)
		{
			m_Region = Map.Internal.DefaultRegion;
			m_Serial = serial;
			m_Aggressors = new List<AggressorInfo>();
			m_Aggressed = new List<AggressorInfo>();
			m_NextSkillTime = Core.TickCount;
			m_DamageEntries = new List<DamageEntry>();

			var ourType = GetType();
			m_TypeRef = World.m_MobileTypes.IndexOf(ourType);

			if (m_TypeRef == -1)
			{
				World.m_MobileTypes.Add(ourType);
				m_TypeRef = World.m_MobileTypes.Count - 1;
			}
		}

		public Mobile()
		{
			m_Region = Map.Internal.DefaultRegion;
			m_Serial = Serial.NewMobile;

			DefaultMobileInit();

			World.AddMobile(this);

			var ourType = GetType();
			m_TypeRef = World.m_MobileTypes.IndexOf(ourType);

			if (m_TypeRef == -1)
			{
				World.m_MobileTypes.Add(ourType);
				m_TypeRef = World.m_MobileTypes.Count - 1;
			}

			Timer.DelayCall(() =>
			{
				if (!m_Deleted)
				{
					EventSink.InvokeMobileCreated(new MobileCreatedEventArgs(this));
					if (!m_Deleted)
					{
						m_InternalCanRegen = true;
						OnCreate();
					}
				}
			});
		}

		public void DefaultMobileInit()
		{
			m_StatCap = Config.Get("PlayerCaps.TotalStatCap", 225);
			m_StrCap = Config.Get("PlayerCaps.StrCap", 125);
			m_DexCap = Config.Get("PlayerCaps.DexCap", 125);
			m_IntCap = Config.Get("PlayerCaps.IntCap", 125);
			m_StrMaxCap = Config.Get("PlayerCaps.StrMaxCap", 150);
			m_DexMaxCap = Config.Get("PlayerCaps.DexMaxCap", 150);
			m_IntMaxCap = Config.Get("PlayerCaps.IntMaxCap", 150);
			m_FollowersMax = 5;
			m_Skills = new Skills(this);
			m_Items = new List<Item>();
			m_StatMods = new List<StatMod>();
			m_SkillMods = new List<SkillMod>();
			Map = Map.Internal;
			m_AutoPageNotify = true;
			m_Aggressors = new List<AggressorInfo>();
			m_Aggressed = new List<AggressorInfo>();
			m_Virtues = new VirtueInfo();
			m_Stabled = new List<Mobile>();
			m_DamageEntries = new List<DamageEntry>();

			m_NextSkillTime = Core.TickCount;
			m_CreationTime = DateTime.UtcNow;
		}

		private static readonly List<Mobile> m_DeltaQueue = new List<Mobile>();

		private bool m_InDeltaQueue;
		private MobileDelta m_DeltaFlags;

		public virtual void Delta(MobileDelta flag)
		{
			if (m_Map == null || m_Map == Map.Internal || m_Deleted)
			{
				return;
			}

			m_DeltaFlags |= flag;

			if (!m_InDeltaQueue)
			{
				m_InDeltaQueue = true;

				m_DeltaQueue.Add(this);
			}

			Core.Set();
		}

		private bool m_NoMoveHS;

		public bool NoMoveHS { get => m_NoMoveHS; set => m_NoMoveHS = value; }

		#region GetDirectionTo[..]
		public Direction GetDirectionTo(int x, int y)
		{
			var dx = m_Location.m_X - x;
			var dy = m_Location.m_Y - y;

			var rx = (dx - dy) * 44;
			var ry = (dx + dy) * 44;

			var ax = Math.Abs(rx);
			var ay = Math.Abs(ry);

			Direction ret;

			if (((ay >> 1) - ax) >= 0)
			{
				ret = (ry > 0) ? Direction.Up : Direction.Down;
			}
			else if (((ax >> 1) - ay) >= 0)
			{
				ret = (rx > 0) ? Direction.Left : Direction.Right;
			}
			else if (rx >= 0 && ry >= 0)
			{
				ret = Direction.West;
			}
			else if (rx >= 0 && ry < 0)
			{
				ret = Direction.South;
			}
			else if (rx < 0 && ry < 0)
			{
				ret = Direction.East;
			}
			else
			{
				ret = Direction.North;
			}

			return ret;
		}

		public Direction GetDirectionTo(Point2D p)
		{
			return GetDirectionTo(p.m_X, p.m_Y);
		}

		public Direction GetDirectionTo(Point3D p)
		{
			return GetDirectionTo(p.m_X, p.m_Y);
		}

		public Direction GetDirectionTo(IPoint2D p)
		{
			if (p == null)
			{
				return Direction.North;
			}

			return GetDirectionTo(p.X, p.Y);
		}
		#endregion

		public virtual void ProcessDelta()
		{
			var m = this;
			MobileDelta delta;

			delta = m.m_DeltaFlags;

			if (delta == MobileDelta.None)
			{
				return;
			}

			var attrs = delta & MobileDelta.Attributes;

			m.m_DeltaFlags = MobileDelta.None;
			m.m_InDeltaQueue = false;

			bool sendHits = false, sendStam = false, sendMana = false, sendAll = false, sendAny = false;
			bool sendIncoming = false, sendNonlocalIncoming = false;
			bool sendUpdate = false, sendRemove = false;
			bool sendPublicStats = false, sendPrivateStats = false;
			bool sendMoving = false, sendNonlocalMoving = false;
			var sendOPLUpdate = (delta & MobileDelta.Properties) != 0;

			bool sendHair = false, sendFacialHair = false, removeHair = false, removeFacialHair = false, sendFace = false, removeFace = false;

			bool sendHealthbarPoison = false, sendHealthbarYellow = false;

			if (attrs != MobileDelta.None)
			{
				sendAny = true;

				if (attrs == MobileDelta.Attributes)
				{
					sendAll = true;
				}
				else
				{
					sendHits = (attrs & MobileDelta.Hits) != 0;
					sendStam = (attrs & MobileDelta.Stam) != 0;
					sendMana = (attrs & MobileDelta.Mana) != 0;
				}
			}

			if ((delta & MobileDelta.GhostUpdate) != 0)
			{
				sendNonlocalIncoming = true;
			}

			if ((delta & MobileDelta.Hue) != 0)
			{
				sendNonlocalIncoming = true;
				sendUpdate = true;
				sendRemove = true;
			}

			if ((delta & MobileDelta.Direction) != 0)
			{
				sendNonlocalMoving = true;
				sendUpdate = true;
			}

			if ((delta & MobileDelta.Body) != 0)
			{
				sendUpdate = true;
				sendIncoming = true;
			}

			if ((delta & (MobileDelta.Flags | MobileDelta.Noto)) != 0)
			{
				sendMoving = true;
			}

			if ((delta & MobileDelta.HealthbarPoison) != 0)
			{
				sendHealthbarPoison = true;
			}

			if ((delta & MobileDelta.HealthbarYellow) != 0)
			{
				sendHealthbarYellow = true;
			}

			if ((delta & MobileDelta.Name) != 0)
			{
				sendAll = false;
				sendHits = false;
				sendAny = sendStam || sendMana;
				sendPublicStats = true;
			}

			if ((delta &
				 (MobileDelta.WeaponDamage | MobileDelta.Resistances | MobileDelta.Stat | MobileDelta.Weight | MobileDelta.Gold |
				  MobileDelta.Armor | MobileDelta.StatCap | MobileDelta.Followers | MobileDelta.TithingPoints | MobileDelta.Race)) !=
				0)
			{
				sendPrivateStats = true;
			}

			if ((delta & MobileDelta.Hair) != 0)
			{
				if (m.HairItemID <= 0)
				{
					removeHair = true;
				}

				sendHair = true;
			}

			if ((delta & MobileDelta.FacialHair) != 0)
			{
				if (m.FacialHairItemID <= 0)
				{
					removeFacialHair = true;
				}

				sendFacialHair = true;
			}

			if ((delta & MobileDelta.Face) != 0)
			{
				if (m.FaceItemID <= 0)
				{
					removeFace = true;
				}

				sendFace = true;
			}

			Packet[][] cache = { new Packet[8], new Packet[8] };

			var ourState = m.m_NetState;

			if (ourState != null)
			{
				if (sendUpdate)
				{
					ourState.Sequence = 0;

					ourState.Send(new MobileUpdate(m));

					ClearFastwalkStack();
				}

				if (sendIncoming)
				{
					ourState.Send(MobileIncoming.Create(ourState, m, m));
				}

				if (sendMoving)
				{
					var noto = Notoriety.Compute(m, m);
					ourState.Send(cache[0][noto] = Packet.Acquire(new MobileMoving(m, noto)));
				}

				if (sendHealthbarPoison)
				{
					ourState.Send(new HealthbarPoison(m));
					ourState.Send(new HealthbarPoisonEC(m));
				}

				if (sendHealthbarYellow)
				{
					ourState.Send(new HealthbarYellow(m));
					ourState.Send(new HealthbarYellowEC(m));
				}

				if (sendPublicStats || sendPrivateStats)
				{
					ourState.Send(new MobileStatus(m));
				}
				else if (sendAll)
				{
					ourState.Send(new MobileAttributes(m));
				}
				else if (sendAny)
				{
					if (sendHits)
					{
						ourState.Send(new MobileHits(m));
					}

					if (sendStam)
					{
						ourState.Send(new MobileStam(m));
					}

					if (sendMana)
					{
						ourState.Send(new MobileMana(m));
					}
				}

				if (sendStam || sendMana)
				{
					var ip = m_Party as IParty;

					if (ip != null && sendStam)
					{
						ip.OnStamChanged(this);
					}

					if (ip != null && sendMana)
					{
						ip.OnManaChanged(this);
					}
				}

				if (sendHair)
				{
					if (removeHair)
					{
						ourState.Send(new RemoveHair(m));
					}
					else
					{
						ourState.Send(new HairEquipUpdate(m));
					}
				}

				if (sendFacialHair)
				{
					if (removeFacialHair)
					{
						ourState.Send(new RemoveFacialHair(m));
					}
					else
					{
						ourState.Send(new FacialHairEquipUpdate(m));
					}
				}

				if (sendFace && ourState.IsEnhancedClient)
				{
					if (removeFace)
					{
						ourState.Send(new RemoveFace(m));
					}
					else
					{
						ourState.Send(new RemoveFace(m));
						ourState.Send(new FaceEquipUpdate(m));
					}
				}

				if (sendOPLUpdate)
				{
					ourState.Send(OPLPacket);
				}
			}

			sendMoving = sendMoving || sendNonlocalMoving;
			sendIncoming = sendIncoming || sendNonlocalIncoming;
			sendHits = sendHits || sendAll;

			if (m.m_Map != null &&
				(sendRemove || sendIncoming || sendPublicStats || sendHits || sendMoving || sendOPLUpdate || sendHair ||
				 sendFacialHair || sendHealthbarPoison || sendHealthbarYellow || sendFace))
			{
				Mobile beholder;

				Packet hitsPacket = null;
				Packet statPacketTrue = null;
				Packet statPacketFalse = null;
				Packet deadPacket = null;
				Packet hairPacket = null;
				Packet facialhairPacket = null;
				Packet hbpPacket = null;
				Packet hbyPacket = null;
				Packet hbpPacketEC = null;
				Packet hbyPacketEC = null;
				Packet faceRemovePacket = null;
				Packet faceSendPacket = null;

				var eable = m.Map.GetClientsInRange(m.m_Location);

				foreach (var state in eable)
				{
					beholder = state.Mobile;

					if (beholder != m && Utility.InUpdateRange(beholder, m) && beholder.CanSee(m))
					{
						if (sendRemove)
						{
							state.Send(RemovePacket);
						}

						if (sendIncoming)
						{
							state.Send(MobileIncoming.Create(state, beholder, m));

							if (m.IsDeadBondedPet)
							{
								if (deadPacket == null)
								{
									deadPacket = Packet.Acquire(new BondedStatus(0, m.m_Serial, 1));
								}

								state.Send(deadPacket);
							}
						}

						if (sendMoving)
						{
							var noto = Notoriety.Compute(beholder, m);

							var p = cache[0][noto];

							if (p == null)
							{
								cache[0][noto] = p = Packet.Acquire(new MobileMoving(m, noto));
							}

							state.Send(p);
						}

						if (sendHealthbarPoison)
						{
							if (hbpPacket == null)
							{
								hbpPacket = Packet.Acquire(new HealthbarPoison(m));
								hbpPacketEC = Packet.Acquire(new HealthbarPoisonEC(m));
							}

							state.Send(hbpPacket);
							state.Send(hbpPacketEC);
						}

						if (sendHealthbarYellow)
						{
							if (hbyPacket == null)
							{
								hbyPacket = Packet.Acquire(new HealthbarYellow(m));
								hbyPacketEC = Packet.Acquire(new HealthbarYellowEC(m));
							}

							state.Send(hbyPacket);
							state.Send(hbyPacketEC);
						}

						if (sendPublicStats)
						{
							if (m.CanBeRenamedBy(beholder))
							{
								if (statPacketTrue == null)
								{
									statPacketTrue = Packet.Acquire(new MobileStatusCompact(true, m));
								}

								state.Send(statPacketTrue);
							}
							else
							{
								if (statPacketFalse == null)
								{
									statPacketFalse = Packet.Acquire(new MobileStatusCompact(false, m));
								}

								state.Send(statPacketFalse);
							}
						}
						else if (sendHits)
						{
							if (hitsPacket == null)
							{
								hitsPacket = Packet.Acquire(new MobileHitsN(m));
							}

							state.Send(hitsPacket);
						}

						if (sendHair)
						{
							if (hairPacket == null)
							{
								if (removeHair)
								{
									hairPacket = Packet.Acquire(new RemoveHair(m));
								}
								else
								{
									hairPacket = Packet.Acquire(new HairEquipUpdate(m));
								}
							}

							state.Send(hairPacket);
						}

						if (sendFacialHair)
						{
							if (facialhairPacket == null)
							{
								if (removeFacialHair)
								{
									facialhairPacket = Packet.Acquire(new RemoveFacialHair(m));
								}
								else
								{
									facialhairPacket = Packet.Acquire(new FacialHairEquipUpdate(m));
								}
							}

							state.Send(facialhairPacket);
						}

						if (sendFace && state.IsEnhancedClient)
						{
							if (faceRemovePacket == null)
							{
								faceRemovePacket = Packet.Acquire(new RemoveFace(m));

								if (!removeFace)
								{
									faceSendPacket = Packet.Acquire(new FaceEquipUpdate(m));
								}
							}

							state.Send(faceRemovePacket);

							if (!removeFace)
							{
								state.Send(faceSendPacket);
							}
						}

						if (sendOPLUpdate)
						{
							state.Send(OPLPacket);
						}
					}
				}

				Packet.Release(hitsPacket);
				Packet.Release(statPacketTrue);
				Packet.Release(statPacketFalse);
				Packet.Release(deadPacket);
				Packet.Release(hairPacket);
				Packet.Release(facialhairPacket);
				Packet.Release(hbpPacket);
				Packet.Release(hbyPacket);
				Packet.Release(hbpPacketEC);
				Packet.Release(hbyPacketEC);
				Packet.Release(faceRemovePacket);
				Packet.Release(faceSendPacket);

				eable.Free();
			}

			if (sendMoving || sendNonlocalMoving || sendHealthbarPoison || sendHealthbarYellow)
			{
				for (var i = 0; i < cache.Length; ++i)
				{
					for (var j = 0; j < cache[i].Length; ++j)
					{
						Packet.Release(ref cache[i][j]);
					}
				}
			}
		}

		private static bool _Processing;

		public static void ProcessDeltaQueue()
		{
			if (_Processing)
			{
				return;
			}

			_Processing = true;

			var i = m_DeltaQueue.Count;

			while (--i >= 0)
			{
				if (i < m_DeltaQueue.Count)
				{
					m_DeltaQueue[i].ProcessDelta();
					m_DeltaQueue.RemoveAt(i);
				}
			}

			_Processing = false;
		}

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public int Deaths
		{
			get => m_Deaths;
			set
			{
				if (m_Deaths != value)
				{
					var oldValue = m_Deaths;

					m_Deaths = Math.Max(0, value);

					OnDeathsChange(oldValue);
				}
			}
		}

		public virtual void OnDeathsChange(int oldValue)
		{ }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Decorator)]
		public int Kills
		{
			get => m_Kills;
			set
			{
				var oldValue = m_Kills;

				if (m_Kills != value)
				{
					m_Kills = value;

					if (m_Kills < 0)
					{
						m_Kills = 0;
					}

					if ((oldValue >= 5) != (m_Kills >= 5))
					{
						Delta(MobileDelta.Noto);
						InvalidateProperties();
					}

					OnKillsChange(oldValue);
				}
			}
		}

		public virtual void OnKillsChange(int oldValue)
		{ }

		[CommandProperty(AccessLevel.GameMaster)]
		public int ShortTermMurders
		{
			get => m_ShortTermMurders;
			set
			{
				if (m_ShortTermMurders != value)
				{
					m_ShortTermMurders = value;

					if (m_ShortTermMurders < 0)
					{
						m_ShortTermMurders = 0;
					}
				}
			}
		}

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Decorator)]
		public virtual bool Criminal
		{
			get => m_Criminal;
			set
			{
				if (m_Criminal != value)
				{
					m_Criminal = value;
					Delta(MobileDelta.Noto);
					InvalidateProperties();
				}

				if (m_Criminal)
				{
					StartCrimDelayTimer();
				}
				else
				{
					StopCrimDelayTimer();
				}
			}
		}

		[CommandProperty(AccessLevel.Counselor)]
		public virtual bool Murderer => m_Kills >= 5;

		public bool CheckAlive()
		{
			return CheckAlive(true);
		}

		public bool CheckAlive(bool message)
		{
			if (!Alive)
			{
				if (message)
				{
					LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019048); // I am dead and cannot do that.
				}

				return false;
			}

			return true;
		}

		#region Overhead messages
		public void PublicOverheadMessage(MessageType type, int hue, bool ascii, string text)
		{
			PublicOverheadMessage(type, hue, ascii, text, true);
		}

		public void PublicOverheadMessage(MessageType type, int hue, bool ascii, string text, bool noLineOfSight)
		{
			if (m_Map != null)
			{
				Packet p = null;

				if (ascii)
				{
					p = new AsciiMessage(m_Serial, Body, type, hue, 3, Name, text);
				}
				else
				{
					p = new UnicodeMessage(m_Serial, Body, type, hue, 3, m_Language, Name, text);
				}

				p.Acquire();

				var eable = m_Map.GetClientsInRange(m_Location);

				foreach (var state in eable)
				{
					if (state.Mobile.CanSee(this) && (noLineOfSight || state.Mobile.InLOS(this)))
					{
						state.Send(p);
					}
				}

				Packet.Release(p);

				eable.Free();
			}
		}

		public void PublicOverheadMessage(MessageType type, int hue, int number)
		{
			PublicOverheadMessage(type, hue, number, "", true);
		}

		public void PublicOverheadMessage(MessageType type, int hue, int number, string args)
		{
			PublicOverheadMessage(type, hue, number, args, true);
		}

		public void PublicOverheadMessage(MessageType type, int hue, int number, string args, bool noLineOfSight)
		{
			if (m_Map != null)
			{
				Packet p = Packet.Acquire(new MessageLocalized(m_Serial, Body, type, hue, 3, number, Name, args));

				var eable = m_Map.GetClientsInRange(m_Location);

				foreach (var state in eable)
				{
					if (state.Mobile.CanSee(this) && (noLineOfSight || state.Mobile.InLOS(this)))
					{
						state.Send(p);
					}
				}

				Packet.Release(p);

				eable.Free();
			}
		}

		public void PublicOverheadMessage(
			MessageType type, int hue, int number, AffixType affixType, string affix, string args)
		{
			PublicOverheadMessage(type, hue, number, affixType, affix, args, true);
		}

		public void PublicOverheadMessage(
			MessageType type, int hue, int number, AffixType affixType, string affix, string args, bool noLineOfSight)
		{
			if (m_Map != null)
			{
				Packet cp = null;
				Packet ep = null;

				var eable = m_Map.GetClientsInRange(m_Location);

				foreach (var state in eable)
				{
					if (state.Mobile.CanSee(this) && (noLineOfSight || state.Mobile.InLOS(this)))
					{
						if (state.IsEnhancedClient)
						{
							if (ep == null)
							{
								ep = Packet.Acquire(new MessageLocalizedAffix(state, m_Serial, Body, type, hue, 3, number, Name, affixType, affix, args));
							}

							state.Send(ep);
						}
						else
						{
							if (cp == null)
							{
								cp = Packet.Acquire(new MessageLocalizedAffix(m_Serial, Body, type, hue, 3, number, Name, affixType, affix, args));
							}

							state.Send(cp);
						}
					}
				}

				Packet.Release(ep);
				Packet.Release(cp);

				eable.Free();
			}
		}

		public void PrivateOverheadMessage(MessageType type, int hue, bool ascii, string text, NetState state)
		{
			if (state == null)
			{
				return;
			}

			if (ascii)
			{
				state.Send(new AsciiMessage(m_Serial, Body, type, hue, 3, Name, text));
			}
			else
			{
				state.Send(new UnicodeMessage(m_Serial, Body, type, hue, 3, m_Language, Name, text));
			}
		}

		public void PrivateOverheadMessage(MessageType type, int hue, int number, NetState state)
		{
			PrivateOverheadMessage(type, hue, number, "", state);
		}

		public void PrivateOverheadMessage(MessageType type, int hue, int number, AffixType affixType, string affix, string args, NetState state)
		{
			Send(new MessageLocalizedAffix(m_NetState, Serial, Body, type, hue, 3, number, Name, affixType, affix, args));
		}

		public void PrivateOverheadMessage(MessageType type, int hue, int number, string args, NetState state)
		{
			if (state == null)
			{
				return;
			}

			state.Send(new MessageLocalized(m_Serial, Body, type, hue, 3, number, Name, args));
		}

		public void LocalOverheadMessage(MessageType type, int hue, bool ascii, string text)
		{
			var ns = m_NetState;

			if (ns != null)
			{
				if (ascii)
				{
					ns.Send(new AsciiMessage(m_Serial, Body, type, hue, 3, Name, text));
				}
				else
				{
					ns.Send(new UnicodeMessage(m_Serial, Body, type, hue, 3, m_Language, Name, text));
				}
			}
		}

		public void LocalOverheadMessage(MessageType type, int hue, int number)
		{
			LocalOverheadMessage(type, hue, number, "");
		}

		public void LocalOverheadMessage(MessageType type, int hue, int number, string args)
		{
			var ns = m_NetState;

			if (ns != null)
			{
				ns.Send(new MessageLocalized(m_Serial, Body, type, hue, 3, number, Name, args));
			}
		}

		public void NonlocalOverheadMessage(MessageType type, int hue, int number)
		{
			NonlocalOverheadMessage(type, hue, number, "");
		}

		public void NonlocalOverheadMessage(MessageType type, int hue, int number, string args)
		{
			if (m_Map != null)
			{
				Packet p = Packet.Acquire(new MessageLocalized(m_Serial, Body, type, hue, 3, number, Name, args));

				var eable = m_Map.GetClientsInRange(m_Location);

				foreach (var state in eable)
				{
					if (state != m_NetState && state.Mobile.CanSee(this))
					{
						state.Send(p);
					}
				}

				Packet.Release(p);

				eable.Free();
			}
		}

		public void NonlocalOverheadMessage(MessageType type, int hue, bool ascii, string text)
		{
			if (m_Map != null)
			{
				Packet p = null;

				if (ascii)
				{
					p = new AsciiMessage(m_Serial, Body, type, hue, 3, Name, text);
				}
				else
				{
					p = new UnicodeMessage(m_Serial, Body, type, hue, 3, Language, Name, text);
				}

				p.Acquire();

				var eable = m_Map.GetClientsInRange(m_Location);

				foreach (var state in eable)
				{
					if (state != m_NetState && state.Mobile.CanSee(this))
					{
						state.Send(p);
					}
				}

				Packet.Release(p);

				eable.Free();
			}
		}
		#endregion

		#region SendLocalizedMessage
		public void SendLocalizedMessage(int number)
		{
			var ns = m_NetState;

			if (ns != null)
			{
				ns.Send(MessageLocalized.InstantiateGeneric(number));
			}
		}

		public void SendLocalizedMessage(int number, string args)
		{
			SendLocalizedMessage(number, args, 0x3B2);
		}

		public void SendLocalizedMessage(int number, string args, int hue)
		{
			if (hue == 0x3B2 && String.IsNullOrEmpty(args))
			{
				var ns = m_NetState;

				if (ns != null)
				{
					ns.Send(MessageLocalized.InstantiateGeneric(number));
				}
			}
			else
			{
				var ns = m_NetState;

				if (ns != null)
				{
					ns.Send(new MessageLocalized(Serial.MinusOne, -1, MessageType.Regular, hue, 3, number, "System", args));
				}
			}
		}

		public void SendLocalizedMessage(int number, bool append, string affix)
		{
			SendLocalizedMessage(number, append, affix, "", 0x3B2);
		}

		public void SendLocalizedMessage(int number, bool append, string affix, string args)
		{
			SendLocalizedMessage(number, append, affix, args, 0x3B2);
		}

		public void SendLocalizedMessage(int number, bool append, string affix, string args, int hue)
		{
			var ns = m_NetState;

			if (ns != null)
			{
				ns.Send(
					new MessageLocalizedAffix(
						ns,
						Serial.MinusOne,
						-1,
						MessageType.Regular,
						hue,
						3,
						number,
						"System",
						(append ? AffixType.Append : AffixType.Prepend) | AffixType.System,
						affix,
						args));
			}
		}
		#endregion

		public void LaunchBrowser(string url)
		{
			if (m_NetState != null)
			{
				m_NetState.LaunchBrowser(url);
			}
		}

		#region Send[ASCII]Message
		public event Action<int, string, bool> OnSendMessage;

		public void SendMessage(string text)
		{
			SendMessage(0x3B2, text);
		}

		public void SendMessage(string format, params object[] args)
		{
			SendMessage(0x3B2, String.Format(format, args));
		}

		public void SendMessage(int hue, string text)
		{
			if (OnSendMessage != null)
			{
				OnSendMessage(hue, text, false);
			}

			var ns = m_NetState;

			if (ns != null)
			{
				ns.Send(new UnicodeMessage(Serial.MinusOne, -1, MessageType.Regular, hue, 3, "ENU", "System", text));
			}
		}

		public void SendMessage(int hue, string format, params object[] args)
		{
			SendMessage(hue, String.Format(format, args));
		}

		public void SendAsciiMessage(string text)
		{
			SendAsciiMessage(0x3B2, text);
		}

		public void SendAsciiMessage(string format, params object[] args)
		{
			SendAsciiMessage(0x3B2, String.Format(format, args));
		}

		public void SendAsciiMessage(int hue, string text)
		{
			if (OnSendMessage != null)
			{
				OnSendMessage(hue, text, true);
			}

			var ns = m_NetState;

			if (ns != null)
			{
				ns.Send(new AsciiMessage(Serial.MinusOne, -1, MessageType.Regular, hue, 3, "System", text));
			}
		}

		public void SendAsciiMessage(int hue, string format, params object[] args)
		{
			SendAsciiMessage(hue, String.Format(format, args));
		}
		#endregion

		#region InRange
		public bool InRange(Point2D p, int range)
		{
			return (p.m_X >= (m_Location.m_X - range)) && (p.m_X <= (m_Location.m_X + range)) &&
				   (p.m_Y >= (m_Location.m_Y - range)) && (p.m_Y <= (m_Location.m_Y + range));
		}

		public bool InRange(Point3D p, int range)
		{
			return (p.m_X >= (m_Location.m_X - range)) && (p.m_X <= (m_Location.m_X + range)) &&
				   (p.m_Y >= (m_Location.m_Y - range)) && (p.m_Y <= (m_Location.m_Y + range));
		}

		public bool InRange(IPoint2D p, int range)
		{
			return (p.X >= (m_Location.m_X - range)) && (p.X <= (m_Location.m_X + range)) && (p.Y >= (m_Location.m_Y - range)) &&
				   (p.Y <= (m_Location.m_Y + range));
		}

		public bool InUpdateRange(IPoint2D p)
		{
			if (m_NetState == null)
				return false;

			return InRange(p, m_NetState.UpdateRange);
		}
		#endregion

		public void InitStats(int str, int dex, int intel)
		{
			m_Str = str;
			m_Dex = dex;
			m_Int = intel;

			Hits = HitsMax;
			Stam = StamMax;
			Mana = ManaMax;

			Delta(MobileDelta.Stat | MobileDelta.Hits | MobileDelta.Stam | MobileDelta.Mana);
		}

		public virtual void DisplayPaperdollTo(Mobile to)
		{
			EventSink.InvokePaperdollRequest(new PaperdollRequestEventArgs(to, this));
		}

		private static bool m_DisableDismountInWarmode;

		public static bool DisableDismountInWarmode { get => m_DisableDismountInWarmode; set => m_DisableDismountInWarmode = value; }

		#region OnDoubleClick[..]
		/// <summary>
		///     Overridable. Event invoked when the Mobile is double clicked. By default, this method can either dismount or open the paperdoll.
		///     <seealso cref="CanPaperdollBeOpenedBy" />
		///     <seealso cref="DisplayPaperdollTo" />
		/// </summary>
		public virtual void OnDoubleClick(Mobile from)
		{
			if (this == from && (!m_DisableDismountInWarmode || !m_Warmode))
			{
				var mount = Mount;

				if (mount != null)
				{
					mount.Rider = null;
					return;
				}
			}

			if (CanPaperdollBeOpenedBy(from))
			{
				DisplayPaperdollTo(from);
			}
		}

		/// <summary>
		///     Overridable. Virtual event invoked when the Mobile is double clicked by someone who is over 18 tiles away.
		///     <seealso cref="OnDoubleClick" />
		/// </summary>
		public virtual void OnDoubleClickOutOfRange(Mobile from)
		{ }

		/// <summary>
		///     Overridable. Virtual event invoked when the Mobile is double clicked by someone who can no longer see the Mobile. This may happen, for example, using 'Last Object' after the Mobile has hidden.
		///     <seealso cref="OnDoubleClick" />
		/// </summary>
		public virtual void OnDoubleClickCantSee(Mobile from)
		{ }

		/// <summary>
		///     Overridable. Event invoked when the Mobile is double clicked by someone who is not alive. Similar to
		///     <see
		///         cref="OnDoubleClick" />
		///     , this method will show the paperdoll. It does not, however, provide any dismount functionality.
		///     <seealso cref="OnDoubleClick" />
		/// </summary>
		public virtual void OnDoubleClickDead(Mobile from)
		{
			if (CanPaperdollBeOpenedBy(from))
			{
				DisplayPaperdollTo(from);
			}
		}
		#endregion

		/// <summary>
		///     Overridable. Event invoked when the Mobile requests to open his own paperdoll via the 'Open Paperdoll' macro.
		/// </summary>
		public virtual void OnPaperdollRequest()
		{
			if (CanPaperdollBeOpenedBy(this))
			{
				DisplayPaperdollTo(this);
			}
		}

		private static int m_BodyWeight = 14;

		public static int BodyWeight { get => m_BodyWeight; set => m_BodyWeight = value; }

		/// <summary>
		///     Overridable. Event invoked when <paramref name="from" /> wants to see this Mobile's stats.
		/// </summary>
		/// <param name="from"></param>
		public virtual void OnStatsQuery(Mobile from)
		{
			if (from.Map == Map && Utility.InUpdateRange(from, this) && from.CanSee(this))
			{
				from.Send(new MobileStatus(from, this));
			}

			if (from == this)
			{
				Send(new StatLockInfo(this));
			}

			var ip = m_Party as IParty;

			if (ip != null)
			{
				ip.OnStatsQuery(from, this);
			}
		}

		/// <summary>
		///     Overridable. Event invoked when <paramref name="from" /> wants to see this Mobile's skills.
		/// </summary>
		public virtual void OnSkillsQuery(Mobile from)
		{
			if (from == this)
			{
				Send(new SkillUpdate(m_Skills));
			}
		}

		/// <summary>
		///     Overridable. Virtual event invoked when <see cref="Region" /> changes.
		/// </summary>
		public virtual void OnRegionChange(Region Old, Region New)
		{ }

		private Item m_MountItem;

		[CommandProperty(AccessLevel.Decorator)]
		public IMount Mount
		{
			get
			{
				IMountItem mountItem = null;

				if (m_MountItem != null && !m_MountItem.Deleted && m_MountItem.Parent == this)
				{
					mountItem = (IMountItem)m_MountItem;
				}

				if (mountItem == null)
				{
					m_MountItem = (mountItem = FindItemOnLayer(Layer.Mount) as IMountItem) as Item;
				}

				return mountItem == null ? null : mountItem.Mount;
			}
		}

		[CommandProperty(AccessLevel.Decorator)]
		public bool Mounted => Mount != null;

		private QuestArrow m_QuestArrow;

		public QuestArrow QuestArrow
		{
			get => m_QuestArrow;
			set
			{
				if (m_QuestArrow != value)
				{
					if (m_QuestArrow != null)
					{
						m_QuestArrow.Stop();
					}

					m_QuestArrow = value;
				}
			}
		}

		public virtual bool CanTarget => true;
		public virtual bool ClickTitle => true;

		public virtual bool PropertyTitle => true;

		public virtual bool ShowFameTitle => true;
		public virtual bool ShowAccessTitle => false;

		public bool CheckSkill(SkillName skill, double minSkill, double maxSkill)
		{
			if (m_SkillCheckLocationHandler == null)
			{
				return false;
			}

			return m_SkillCheckLocationHandler(this, skill, minSkill, maxSkill);
		}

		public bool CheckSkill(SkillName skill, double chance)
		{
			if (m_SkillCheckDirectLocationHandler == null)
			{
				return false;
			}

			return m_SkillCheckDirectLocationHandler(this, skill, chance);
		}

		public bool CheckTargetSkill(SkillName skill, object target, double minSkill, double maxSkill)
		{
			if (m_SkillCheckTargetHandler == null)
			{
				return false;
			}

			return m_SkillCheckTargetHandler(this, skill, target, minSkill, maxSkill);
		}

		public bool CheckTargetSkill(SkillName skill, object target, double chance)
		{
			if (m_SkillCheckDirectTargetHandler == null)
			{
				return false;
			}

			return m_SkillCheckDirectTargetHandler(this, skill, target, chance);
		}

		public virtual void DisruptiveAction()
		{
			if (Meditating)
			{
				Meditating = false;
				SendLocalizedMessage(500134); // You stop meditating.
			}
		}

		#region Armor
		public Item ShieldArmor => FindItemOnLayer(Layer.TwoHanded);

		public Item NeckArmor => FindItemOnLayer(Layer.Neck);

		public Item HandArmor => FindItemOnLayer(Layer.Gloves);

		public Item HeadArmor => FindItemOnLayer(Layer.Helm);

		public Item ArmsArmor => FindItemOnLayer(Layer.Arms);

		public Item LegsArmor
		{
			get
			{
				var ar = FindItemOnLayer(Layer.InnerLegs);

				if (ar == null)
				{
					ar = FindItemOnLayer(Layer.Pants);
				}

				return ar;
			}
		}

		public Item ChestArmor
		{
			get
			{
				var ar = FindItemOnLayer(Layer.InnerTorso);

				if (ar == null)
				{
					ar = FindItemOnLayer(Layer.Shirt);
				}

				return ar;
			}
		}

		public Item Talisman => FindItemOnLayer(Layer.Talisman);

		public Item Ring => FindItemOnLayer(Layer.Ring);

		public Item Bracelet => FindItemOnLayer(Layer.Bracelet);
		#endregion

		/// <summary>
		///     Gets or sets the maximum attainable value for <see cref="RawStr" />, <see cref="RawDex" />, and <see cref="RawInt" />.
		/// </summary>
		[CommandProperty(AccessLevel.GameMaster)]
		public int StatCap
		{
			get => m_StatCap;
			set
			{
				if (m_StatCap != value)
				{
					var old = m_StatCap;

					m_StatCap = value;

					if (old != m_StatCap)
					{
						EventSink.InvokeStatCapChange(new StatCapChangeEventArgs(this, old, m_StatCap));
					}

					Delta(MobileDelta.StatCap);
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int StrCap
		{
			get => m_StrCap;
			set => m_StrCap = value;
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int DexCap
		{
			get => m_DexCap;
			set => m_DexCap = value;
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int IntCap
		{
			get => m_IntCap;
			set => m_IntCap = value;
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int StrMaxCap
		{
			get => m_StrMaxCap;
			set => m_StrMaxCap = value;
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int DexMaxCap
		{
			get => m_DexMaxCap;
			set => m_DexMaxCap = value;
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int IntMaxCap
		{
			get => m_IntMaxCap;
			set => m_IntMaxCap = value;
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual bool Meditating { get; set; }

		[CommandProperty(AccessLevel.Decorator)]
		public bool CanSwim { get => m_CanSwim; set => m_CanSwim = value; }

		[CommandProperty(AccessLevel.Decorator)]
		public bool CantWalk { get => m_CantWalk; set => m_CantWalk = value; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool CanHearGhosts { get => m_CanHearGhosts || IsStaff(); set => m_CanHearGhosts = value; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int RawStatTotal => RawStr + RawDex + RawInt;

		[CommandProperty(AccessLevel.GameMaster)]
		public int StatTotal => Str + Dex + Int;

		public long NextSpellTime { get; set; }

		/// <summary>
		///     Overridable. Virtual event invoked when the sector this Mobile is in gets <see cref="Sector.Activate">activated</see>.
		/// </summary>
		public virtual void OnSectorActivate()
		{ }

		/// <summary>
		///     Overridable. Virtual event invoked when the sector this Mobile is in gets <see cref="Sector.Deactivate">deactivated</see>.
		/// </summary>
		public virtual void OnSectorDeactivate()
		{ }
	}
}

