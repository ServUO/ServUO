#region Header
// **********
// ServUO - Mobile.cs
// **********
#endregion

#region References
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CustomsFramework;

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
			return (DateTime.UtcNow < m_Expire);
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
			return (!m_Item.Deleted && !m_Mobile.Deleted && m_Item.Parent == m_Mobile);
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
			get { return m_ObeyCap; }
			set
			{
				m_ObeyCap = value;

				if (m_Owner != null)
				{
					Skill sk = m_Owner.Skills[m_Skill];

					if (sk != null)
					{
						sk.Update();
					}
				}
			}
		}

		public Mobile Owner
		{
			get { return m_Owner; }
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
			get { return m_Skill; }
			set
			{
				if (m_Skill != value)
				{
					Skill oldUpdate = (m_Owner != null ? m_Owner.Skills[m_Skill] : null);

					m_Skill = value;

					if (m_Owner != null)
					{
						Skill sk = m_Owner.Skills[m_Skill];

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
			get { return m_Relative; }
			set
			{
				if (m_Relative != value)
				{
					m_Relative = value;

					if (m_Owner != null)
					{
						Skill sk = m_Owner.Skills[m_Skill];

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
			get { return !m_Relative; }
			set
			{
				if (m_Relative == value)
				{
					m_Relative = !value;

					if (m_Owner != null)
					{
						Skill sk = m_Owner.Skills[m_Skill];

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
			get { return m_Value; }
			set
			{
				if (m_Value != value)
				{
					m_Value = value;

					if (m_Owner != null)
					{
						Skill sk = m_Owner.Skills[m_Skill];

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

		public Mobile Owner { get { return m_Owner; } set { m_Owner = value; } }

		public ResistanceType Type
		{
			get { return m_Type; }
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
			get { return m_Offset; }
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

		public StatType Type { get { return m_Type; } }
		public string Name { get { return m_Name; } }
		public int Offset { get { return m_Offset; } }

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

		public Mobile Damager { get { return m_Damager; } }
		public int DamageGiven { get; set; }
		public DateTime LastDamage { get { return m_LastDamage; } set { m_LastDamage = value; } }
		public bool HasExpired { get { return (DateTime.UtcNow > (m_LastDamage + m_ExpireDelay)); } }
		public List<DamageEntry> Responsible { get; set; }

		private static TimeSpan m_ExpireDelay = TimeSpan.FromMinutes(2.0);

		public static TimeSpan ExpireDelay { get { return m_ExpireDelay; } set { m_ExpireDelay = value; } }

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

	[CustomEnum(new[] {"North", "Right", "East", "Down", "South", "Left", "West", "Up"})]
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

	public delegate bool AllowHarmfulHandler(Mobile from, Mobile target);

	public delegate Container CreateCorpseHandler(
		Mobile from, HairInfo hair, FacialHairInfo facialhair, List<Item> initialContent, List<Item> equipedItems);

	public delegate int AOSStatusHandler(Mobile from, int index);
	#endregion

	/// <summary>
	///     Base class representing players, npcs, and creatures.
	/// </summary>
    [System.Runtime.InteropServices.ComVisible(true)]
	public class Mobile : IEntity, IHued, IComparable<Mobile>, ISerializable, ISpawnable
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

		#region Customs Framework
		private List<BaseModule> m_Modules = new List<BaseModule>();

		[CommandProperty(AccessLevel.Developer)]
		public List<BaseModule> Modules { get { return m_Modules; } set { m_Modules = value; } }

		//public List<BaseModule> Modules { get; private set; }

		public BaseModule GetModule(string name)
		{
			return Modules.FirstOrDefault(mod => mod.Name == name);
		}

		public BaseModule GetModule(Type type)
		{
			return Modules.FirstOrDefault(mod => mod.GetType() == type);
		}

		public List<BaseModule> GetModules(string name)
		{
			return Modules.Where(mod => mod.Name == name).ToList();
		}

		public List<BaseModule> SearchModules(string search)
		{
			var keywords = search.ToLower().Split(' ');
			var modules = new List<BaseModule>();

			foreach (BaseModule mod in Modules)
			{
				bool match = true;
				string name = mod.Name.ToLower();

				foreach (string keyword in keywords)
				{
					if (name.IndexOf(keyword, StringComparison.Ordinal) == -1)
					{
						match = false;
					}
				}

				if (match)
				{
					modules.Add(mod);
				}
			}

			return modules;
		}
		#endregion

		private static bool m_DragEffects = true;

		public static bool DragEffects { get { return m_DragEffects; } set { m_DragEffects = value; } }

		#region Handlers
		public static AllowBeneficialHandler AllowBeneficialHandler { get; set; }

		public static AllowHarmfulHandler AllowHarmfulHandler { get; set; }

		private static SkillCheckTargetHandler m_SkillCheckTargetHandler;
		private static SkillCheckLocationHandler m_SkillCheckLocationHandler;
		private static SkillCheckDirectTargetHandler m_SkillCheckDirectTargetHandler;
		private static SkillCheckDirectLocationHandler m_SkillCheckDirectLocationHandler;

		public static SkillCheckTargetHandler SkillCheckTargetHandler { get { return m_SkillCheckTargetHandler; } set { m_SkillCheckTargetHandler = value; } }

		public static SkillCheckLocationHandler SkillCheckLocationHandler { get { return m_SkillCheckLocationHandler; } set { m_SkillCheckLocationHandler = value; } }

		public static SkillCheckDirectTargetHandler SkillCheckDirectTargetHandler { get { return m_SkillCheckDirectTargetHandler; } set { m_SkillCheckDirectTargetHandler = value; } }

		public static SkillCheckDirectLocationHandler SkillCheckDirectLocationHandler { get { return m_SkillCheckDirectLocationHandler; } set { m_SkillCheckDirectLocationHandler = value; } }

		private static AOSStatusHandler m_AOSStatusHandler;

		public static AOSStatusHandler AOSStatusHandler { get { return m_AOSStatusHandler; } set { m_AOSStatusHandler = value; } }
		#endregion

		#region Regeneration
		private static RegenRateHandler m_HitsRegenRate, m_StamRegenRate, m_ManaRegenRate;
		private static TimeSpan m_DefaultHitsRate, m_DefaultStamRate, m_DefaultManaRate;

		public static RegenRateHandler HitsRegenRateHandler { get { return m_HitsRegenRate; } set { m_HitsRegenRate = value; } }

		public static TimeSpan DefaultHitsRate { get { return m_DefaultHitsRate; } set { m_DefaultHitsRate = value; } }

		public static RegenRateHandler StamRegenRateHandler { get { return m_StamRegenRate; } set { m_StamRegenRate = value; } }

		public static TimeSpan DefaultStamRate { get { return m_DefaultStamRate; } set { m_DefaultStamRate = value; } }

		public static RegenRateHandler ManaRegenRateHandler { get { return m_ManaRegenRate; } set { m_ManaRegenRate = value; } }

		public static TimeSpan DefaultManaRate { get { return m_DefaultManaRate; } set { m_DefaultManaRate = value; } }

		public static TimeSpan GetHitsRegenRate(Mobile m)
		{
			if (m_HitsRegenRate == null)
			{
				return m_DefaultHitsRate;
			}
			else
			{
				return m_HitsRegenRate(m);
			}
		}

		public static TimeSpan GetStamRegenRate(Mobile m)
		{
			if (m_StamRegenRate == null)
			{
				return m_DefaultStamRate;
			}
			else
			{
				return m_StamRegenRate(m);
			}
		}

		public static TimeSpan GetManaRegenRate(Mobile m)
		{
			if (m_ManaRegenRate == null)
			{
				return m_DefaultManaRate;
			}
			else
			{
				return m_ManaRegenRate(m);
			}
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
				bool v = (Core.TickCount - m_End >= 0);

				if (v)
				{
					m_InstancePool.Enqueue(this);
				}

				return v;
			}
		}

		#region Var declarations
		private Serial m_Serial;
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
		private int m_Kills, m_ShortTermMurders;
		private int m_SpeechHue, m_EmoteHue, m_WhisperHue, m_YellHue;
		private string m_Language;
		private NetState m_NetState;
		private bool m_Female, m_Warmode, m_Hidden, m_Blessed, m_Flying;
		private int m_StatCap;
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
		private Mobile m_Combatant;
		private List<Mobile> m_Stabled;
		private bool m_AutoPageNotify;
		private bool m_CanHearGhosts;
		private bool m_CanSwim, m_CantWalk;
		private int m_TithingPoints;
		private bool m_DisplayGuildTitle;
		private Mobile m_GuildFealty;
		private DateTime[] m_StuckMenuUses;
		private Timer m_ExpireCombatant;
		private Timer m_ExpireCriminal;
		private Timer m_ExpireAggrTimer;
		private Timer m_LogoutTimer;
		private Timer m_CombatTimer;
		private Timer m_ManaTimer, m_HitsTimer, m_StamTimer;
		private long m_NextSkillTime;
		private long m_NextActionMessage;
		private bool m_Paralyzed;
		private ParalyzedTimer m_ParaTimer;
		private bool _Sleep;
		private SleepTimer _SleepTimer;
		private bool m_Frozen;
		private FrozenTimer m_FrozenTimer;
		private int m_AllowedStealthSteps;
		private int m_Hunger;
		private int m_NameHue = -1;
		private Region m_Region;
		private bool m_DisarmReady, m_StunReady;
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
		private int m_VirtualArmorMod;
		private VirtueInfo m_Virtues;
		private object m_Party;
		private List<SkillMod> m_SkillMods;
		private Body m_BodyMod;
		private DateTime m_LastStrGain;
		private DateTime m_LastIntGain;
		private DateTime m_LastDexGain;
		private Race m_Race;
		#endregion

		private static readonly TimeSpan WarmodeSpamCatch = TimeSpan.FromSeconds((Core.SE ? 1.0 : 0.5));
		private static readonly TimeSpan WarmodeSpamDelay = TimeSpan.FromSeconds((Core.SE ? 4.0 : 2.0));

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
				Race oldRace = Race;

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

		protected virtual void OnRaceChange(Race oldRace)
		{ }

		public virtual double RacialSkillBonus { get { return 0; } }

        public virtual double GetRacialSkillBonus(SkillName skill)
        {
            return RacialSkillBonus;
        }

		private List<ResistanceMod> m_ResistMods;

		private int[] m_Resistances;

        protected List<string> m_SlayerVulnerabilities = new List<string>();
        protected bool m_SpecialSlayerMechanics = false;

        public List<String> SlayerVulnerabilities { get { return m_SlayerVulnerabilities; } }

        [CommandProperty(AccessLevel.Decorator)]
        public bool SpecialSlayerMechanics { get { return m_SpecialSlayerMechanics; } }
  
		public int[] Resistances { get { return m_Resistances; } }

		public virtual int BasePhysicalResistance { get { return 0; } }
		public virtual int BaseFireResistance { get { return 0; } }
		public virtual int BaseColdResistance { get { return 0; } }
		public virtual int BasePoisonResistance { get { return 0; } }
		public virtual int BaseEnergyResistance { get { return 0; } }

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
		public virtual int PhysicalResistance { get { return GetResistance(ResistanceType.Physical); } }

		[CommandProperty(AccessLevel.Counselor)]
		public virtual int FireResistance { get { return GetResistance(ResistanceType.Fire); } }

		[CommandProperty(AccessLevel.Counselor)]
		public virtual int ColdResistance { get { return GetResistance(ResistanceType.Cold); } }

		[CommandProperty(AccessLevel.Counselor)]
		public virtual int PoisonResistance { get { return GetResistance(ResistanceType.Poison); } }

		[CommandProperty(AccessLevel.Counselor)]
		public virtual int EnergyResistance { get { return GetResistance(ResistanceType.Energy); } }

		public virtual void UpdateResistances()
		{
			if (m_Resistances == null)
			{
				m_Resistances = new int[5] {int.MinValue, int.MinValue, int.MinValue, int.MinValue, int.MinValue};
			}

			bool delta = false;

			for (int i = 0; i < m_Resistances.Length; ++i)
			{
				if (m_Resistances[i] != int.MinValue)
				{
					m_Resistances[i] = int.MinValue;
					delta = true;
				}
			}

			if (delta)
			{
				Delta(MobileDelta.Resistances);
			}
		}

		public virtual int GetResistance(ResistanceType type)
		{
			if (m_Resistances == null)
			{
				m_Resistances = new int[5] {int.MinValue, int.MinValue, int.MinValue, int.MinValue, int.MinValue};
			}

			int v = (int)type;

			if (v < 0 || v >= m_Resistances.Length)
			{
				return 0;
			}

			int res = m_Resistances[v];

			if (res == int.MinValue)
			{
				ComputeResistances();
				res = m_Resistances[v];
			}

			return res;
		}

		public List<ResistanceMod> ResistanceMods { get { return m_ResistMods; } set { m_ResistMods = value; } }

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

		public static int MinPlayerResistance { get { return m_MinPlayerResistance; } set { m_MinPlayerResistance = value; } }

		private static int m_MaxPlayerResistance = 70;

		public static int MaxPlayerResistance { get { return m_MaxPlayerResistance; } set { m_MaxPlayerResistance = value; } }

		public virtual void ComputeResistances()
		{
			if (m_Resistances == null)
			{
				m_Resistances = new int[5] {int.MinValue, int.MinValue, int.MinValue, int.MinValue, int.MinValue};
			}

			for (int i = 0; i < m_Resistances.Length; ++i)
			{
				m_Resistances[i] = 0;
			}

			m_Resistances[0] += BasePhysicalResistance;
			m_Resistances[1] += BaseFireResistance;
			m_Resistances[2] += BaseColdResistance;
			m_Resistances[3] += BasePoisonResistance;
			m_Resistances[4] += BaseEnergyResistance;

			for (int i = 0; m_ResistMods != null && i < m_ResistMods.Count; ++i)
			{
				ResistanceMod mod = m_ResistMods[i];
				int v = (int)mod.Type;

				if (v >= 0 && v < m_Resistances.Length)
				{
					m_Resistances[v] += mod.Offset;
				}
			}

			for (int i = 0; i < m_Items.Count; ++i)
			{
				Item item = m_Items[i];

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

			for (int i = 0; i < m_Resistances.Length; ++i)
			{
				int min = GetMinResistance((ResistanceType)i);
				int max = GetMaxResistance((ResistanceType)i);

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
			ObjectPropertyList opl = PropertyList;

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
			string name = Name;

			if (name == null)
			{
				name = String.Empty;
			}

			string prefix = "";

			if (ShowFameTitle && (m_Player || m_Body.IsHuman) && m_Fame >= 10000)
			{
				prefix = m_Female ? "Lady" : "Lord";
			}

			string suffix = "";

			if (PropertyTitle && Title != null && Title.Length > 0)
			{
				suffix = Title;
			}

			BaseGuild guild = m_Guild;

			if (guild != null && (m_Player || m_DisplayGuildTitle))
			{
				if (suffix.Length > 0)
				{
					suffix = String.Format("{0} [{1}]", suffix, Utility.FixHtml(guild.Abbreviation));
				}
				else
				{
					suffix = String.Format("[{0}]", Utility.FixHtml(guild.Abbreviation));
				}
			}

			suffix = ApplyNameSuffix(suffix);

			list.Add(1050045, "{0} \t{1}\t {2}", prefix, name, suffix); // ~1_PREFIX~~2_NAME~~3_SUFFIX~

			if (guild != null && (m_DisplayGuildTitle || (m_Player && guild.Type != GuildType.Regular)))
			{
				string type;

				if (guild.Type >= 0 && (int)guild.Type < m_GuildTypes.Length)
				{
					type = m_GuildTypes[(int)guild.Type];
				}
				else
				{
					type = "";
				}

				string title = GuildTitle;

				if (title == null)
				{
					title = "";
				}
				else
				{
					title = title.Trim();
				}

				if (NewGuildDisplay && title.Length > 0)
				{
					list.Add("{0}, {1}", Utility.FixHtml(title), Utility.FixHtml(guild.Name));
				}
				else
				{
					if (title.Length > 0)
					{
						list.Add("{0}, {1} Guild{2}", Utility.FixHtml(title), Utility.FixHtml(guild.Name), type);
					}
					else
					{
						list.Add(Utility.FixHtml(guild.Name));
					}
				}
			}
		}

		public virtual bool NewGuildDisplay { get { return false; } }

		public virtual void GetProperties(ObjectPropertyList list)
		{
			AddNameProperties(list);
		}

		public virtual void GetChildProperties(ObjectPropertyList list, Item item)
		{ }

		public virtual void GetChildNameProperties(ObjectPropertyList list, Item item)
		{ }

		private void UpdateAggrExpire()
		{
			if (m_Deleted || (m_Aggressors.Count == 0 && m_Aggressed.Count == 0))
			{
				StopAggrExpire();
			}
			else if (m_ExpireAggrTimer == null)
			{
				m_ExpireAggrTimer = new ExpireAggressorsTimer(this);
				m_ExpireAggrTimer.Start();
			}
		}

		private void StopAggrExpire()
		{
			if (m_ExpireAggrTimer != null)
			{
				m_ExpireAggrTimer.Stop();
			}

			m_ExpireAggrTimer = null;
		}

		private void CheckAggrExpire()
		{
			for (int i = m_Aggressors.Count - 1; i >= 0; --i)
			{
				if (i >= m_Aggressors.Count)
				{
					continue;
				}

				AggressorInfo info = m_Aggressors[i];

				if (info.Expired)
				{
					Mobile attacker = info.Attacker;
					attacker.RemoveAggressed(this);

					m_Aggressors.RemoveAt(i);
					info.Free();

					if (m_NetState != null && CanSee(attacker) && Utility.InUpdateRange(m_Location, attacker.m_Location))
					{
						m_NetState.Send(MobileIncoming.Create(m_NetState, this, attacker));
					}
				}
			}

			for (int i = m_Aggressed.Count - 1; i >= 0; --i)
			{
				if (i >= m_Aggressed.Count)
				{
					continue;
				}

				AggressorInfo info = m_Aggressed[i];

				if (info.Expired)
				{
					Mobile defender = info.Defender;
					defender.RemoveAggressor(this);

					m_Aggressed.RemoveAt(i);
					info.Free();

					if (m_NetState != null && CanSee(defender) && Utility.InUpdateRange(m_Location, defender.m_Location))
					{
						m_NetState.Send(MobileIncoming.Create(m_NetState, this, defender));
					}
				}
			}

			UpdateAggrExpire();
		}

		public List<Mobile> Stabled { get { return m_Stabled; } }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public VirtueInfo Virtues { get { return m_Virtues; } set { } }

		public object Party { get { return m_Party; } set { m_Party = value; } }
		public List<SkillMod> SkillMods { get { return m_SkillMods; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int VirtualArmorMod
		{
			get { return m_VirtualArmorMod; }
			set
			{
				if (m_VirtualArmorMod != value)
				{
					m_VirtualArmorMod = value;

					Delta(MobileDelta.Armor);
				}
			}
		}

		/// <summary>
		///     Overridable. Virtual event invoked when <paramref name="skill" /> changes in some way.
		/// </summary>
		public virtual void OnSkillInvalidated(Skill skill)
		{ }

		public virtual void UpdateSkillMods()
		{
			ValidateSkillMods();

			for (int i = 0; i < m_SkillMods.Count; ++i)
			{
				SkillMod mod = m_SkillMods[i];

				Skill sk = m_Skills[mod.Skill];

				if (sk != null)
				{
					sk.Update();
				}
			}
		}

		public virtual void ValidateSkillMods()
		{
			for (int i = 0; i < m_SkillMods.Count;)
			{
				SkillMod mod = m_SkillMods[i];

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

				Skill sk = m_Skills[mod.Skill];

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

				Skill sk = m_Skills[mod.Skill];

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

			public bool Value { get { return m_Value; } set { m_Value = value; } }

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
		public int MagicDamageAbsorb { get { return m_MagicDamageAbsorb; } set { m_MagicDamageAbsorb = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int SkillsTotal { get { return m_Skills == null ? 0 : m_Skills.Total; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int SkillsCap
		{
			get { return m_Skills == null ? 0 : m_Skills.Cap; }
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
			else if (target == this || IsStaff())
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
			else if (target == this || IsStaff())
			{
				return true;
			}
			else if (target is Item && ((Item)target).RootParent == this)
			{
				return true;
			}

			return m_Map.LineOfSight(this, target);
		}

		public bool InLOS(Point3D target)
		{
			if (m_Deleted || m_Map == null)
			{
				return false;
			}
			else if (IsStaff())
			{
				return true;
			}

			return m_Map.LineOfSight(this, target);
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int BaseSoundID { get { return m_BaseSoundID; } set { m_BaseSoundID = value; } }

		public long NextCombatTime { get { return m_NextCombatTime; } set { m_NextCombatTime = value; } }

		public bool BeginAction(object toLock)
		{
			if (_actions == null)
			{
				_actions = new List<object>();

				_actions.Add(toLock);

				return true;
			}
			else if (!_actions.Contains(toLock))
			{
				_actions.Add(toLock);

				return true;
			}

			return false;
		}

		public bool CanBeginAction(object toLock)
		{
			return (_actions == null || !_actions.Contains(toLock));
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
		public int NameHue { get { return m_NameHue; } set { m_NameHue = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Hunger
		{
			get { return m_Hunger; }
			set
			{
				int oldValue = m_Hunger;

				if (oldValue != value)
				{
					m_Hunger = value;

					EventSink.InvokeHungerChanged(new HungerChangedEventArgs(this, oldValue));
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int Thirst { get { return m_Thirst; } set { m_Thirst = value; } }

		[CommandProperty(AccessLevel.Decorator)]
		public int BAC { get { return m_BAC; } set { m_BAC = value; } }

		private long m_LastMoveTime;

		/// <summary>
		///     Gets or sets the number of steps this player may take when hidden before being revealed.
		/// </summary>
		[CommandProperty(AccessLevel.GameMaster)]
		public int AllowedStealthSteps { get { return m_AllowedStealthSteps; } set { m_AllowedStealthSteps = value; } }

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

		public virtual TimeSpan GetLogoutDelay()
		{
			return Region.GetLogoutDelay(this);
		}

		private StatLockType m_StrLock, m_DexLock, m_IntLock;

		private Item m_Holding;

		public Item Holding
		{
			get { return m_Holding; }
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

		public long LastMoveTime { get { return m_LastMoveTime; } set { m_LastMoveTime = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual bool Paralyzed
		{
			get { return m_Paralyzed; }
			set
			{
				if (m_Paralyzed != value)
				{
					m_Paralyzed = value;
					Delta(MobileDelta.Flags);

					SendLocalizedMessage(m_Paralyzed ? 502381 : 502382);

					if (m_ParaTimer != null)
					{
						m_ParaTimer.Stop();
						m_ParaTimer = null;
					}
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual bool Asleep
		{
			get { return _Sleep; }
			set
			{
				if (_Sleep != value)
				{
					_Sleep = value;

					if (_SleepTimer != null)
					{
						Send(SpeedControl.Disable);
						_SleepTimer.Stop();
						_SleepTimer = null;
					}
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool DisarmReady
		{
			get { return m_DisarmReady; }
			set
			{
				m_DisarmReady = value;
				//SendLocalizedMessage( value ? 1019013 : 1019014 );
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool StunReady
		{
			get { return m_StunReady; }
			set
			{
				m_StunReady = value;
				//SendLocalizedMessage( value ? 1019011 : 1019012 );
			}
		}

		[CommandProperty(AccessLevel.Decorator)]
		public bool Frozen
		{
			get { return m_Frozen; }
			set
			{
				if (m_Frozen != value)
				{
					m_Frozen = value;
					Delta(MobileDelta.Flags);

					if (m_FrozenTimer != null)
					{
						m_FrozenTimer.Stop();
						m_FrozenTimer = null;
					}
				}
			}
		}

		public void Paralyze(TimeSpan duration)
		{
			if (!m_Paralyzed)
			{
				Paralyzed = true;

				m_ParaTimer = new ParalyzedTimer(this, duration);
				m_ParaTimer.Start();
			}
		}

		public void Sleep(TimeSpan duration)
		{
			if (!_Sleep)
			{
				Asleep = true;
				Send(SpeedControl.WalkSpeed);

				_SleepTimer = new SleepTimer(this, duration);
				_SleepTimer.Start();
			}
		}

		public void Freeze(TimeSpan duration)
		{
			if (!m_Frozen)
			{
				Frozen = true;

				m_FrozenTimer = new FrozenTimer(this, duration);
				m_FrozenTimer.Start();
			}
		}

		/// <summary>
		///     Gets or sets the <see cref="StatLockType">lock state</see> for the <see cref="RawStr" /> property.
		/// </summary>
		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public StatLockType StrLock
		{
			get { return m_StrLock; }
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
			get { return m_DexLock; }
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
			get { return m_IntLock; }
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

		public long NextActionMessage { get { return m_NextActionMessage; } set { m_NextActionMessage = value; } }

		private static int m_ActionMessageDelay = 125;

		public static int ActionMessageDelay { get { return m_ActionMessageDelay; } set { m_ActionMessageDelay = value; } }

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
				Container pack = Backpack;

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

		private static bool m_GlobalRegenThroughPoison = true;

		public static bool GlobalRegenThroughPoison { get { return m_GlobalRegenThroughPoison; } set { m_GlobalRegenThroughPoison = value; } }

		public virtual bool RegenThroughPoison { get { return m_GlobalRegenThroughPoison; } }

		public virtual bool CanRegenHits { get { return Alive && (RegenThroughPoison || !Poisoned); } }
		public virtual bool CanRegenStam { get { return Alive; } }
		public virtual bool CanRegenMana { get { return Alive; } }

		#region Timers
		private class ManaTimer : Timer
		{
			private readonly Mobile m_Owner;

			public ManaTimer(Mobile m)
				: base(GetManaRegenRate(m), GetManaRegenRate(m))
			{
				Priority = TimerPriority.FiftyMS;
				m_Owner = m;
			}

			protected override void OnTick()
			{
				if (m_Owner.CanRegenMana) // m_Owner.Alive )
				{
					m_Owner.Mana++;
				}

				Delay = Interval = GetManaRegenRate(m_Owner);
			}
		}

		private class HitsTimer : Timer
		{
			private readonly Mobile m_Owner;

			public HitsTimer(Mobile m)
				: base(GetHitsRegenRate(m), GetHitsRegenRate(m))
			{
				Priority = TimerPriority.FiftyMS;
				m_Owner = m;
			}

			protected override void OnTick()
			{
				if (m_Owner.CanRegenHits) // m_Owner.Alive && !m_Owner.Poisoned )
				{
					m_Owner.Hits++;
				}

				Delay = Interval = GetHitsRegenRate(m_Owner);
			}
		}

		private class StamTimer : Timer
		{
			private readonly Mobile m_Owner;

			public StamTimer(Mobile m)
				: base(GetStamRegenRate(m), GetStamRegenRate(m))
			{
				Priority = TimerPriority.FiftyMS;
				m_Owner = m;
			}

			protected override void OnTick()
			{
				if (m_Owner.CanRegenStam) // m_Owner.Alive )
				{
					m_Owner.Stam++;
				}

				Delay = Interval = GetStamRegenRate(m_Owner);
			}
		}

		private class LogoutTimer : Timer
		{
			private readonly Mobile m_Mobile;

			public LogoutTimer(Mobile m)
				: base(TimeSpan.FromDays(1.0))
			{
				Priority = TimerPriority.OneSecond;
				m_Mobile = m;
			}

			protected override void OnTick()
			{
				if (m_Mobile.m_Map != Map.Internal)
				{
					EventSink.InvokeLogout(new LogoutEventArgs(m_Mobile));

					m_Mobile.m_LogoutLocation = m_Mobile.m_Location;
					m_Mobile.m_LogoutMap = m_Mobile.m_Map;

					m_Mobile.Internalize();
				}
			}
		}

		private class ParalyzedTimer : Timer
		{
			private readonly Mobile m_Mobile;

			public ParalyzedTimer(Mobile m, TimeSpan duration)
				: base(duration)
			{
				Priority = TimerPriority.TwentyFiveMS;
				m_Mobile = m;
			}

			protected override void OnTick()
			{
				m_Mobile.Paralyzed = false;
			}
		}

		private class SleepTimer : Timer
		{
			private readonly Mobile _Mobile;

			public SleepTimer(Mobile m, TimeSpan duration)
				: base(duration)
			{
				Priority = TimerPriority.TwentyFiveMS;
				_Mobile = m;
			}

			protected override void OnTick()
			{
				_Mobile.Asleep = false;
			}
		}

		private class FrozenTimer : Timer
		{
			private readonly Mobile m_Mobile;

			public FrozenTimer(Mobile m, TimeSpan duration)
				: base(duration)
			{
				Priority = TimerPriority.TwentyFiveMS;
				m_Mobile = m;
			}

			protected override void OnTick()
			{
				m_Mobile.Frozen = false;
			}
		}

		private class CombatTimer : Timer
		{
			private readonly Mobile m_Mobile;

			public CombatTimer(Mobile m)
				: base(TimeSpan.FromSeconds(0.0), TimeSpan.FromSeconds(0.01), 0)
			{
				m_Mobile = m;

				if (!m_Mobile.m_Player && m_Mobile.m_Dex <= 100)
				{
					Priority = TimerPriority.FiftyMS;
				}
			}

			protected override void OnTick()
			{
				if (Core.TickCount - m_Mobile.m_NextCombatTime >= 0)
				{
					Mobile combatant = m_Mobile.Combatant;

					// If no combatant, wrong map, one of us is a ghost, or cannot see, or deleted, then stop combat
					if (combatant == null || combatant.m_Deleted || m_Mobile.m_Deleted || combatant.m_Map != m_Mobile.m_Map ||
						!combatant.Alive || !m_Mobile.Alive || !m_Mobile.CanSee(combatant) || combatant.IsDeadBondedPet ||
						m_Mobile.IsDeadBondedPet)
					{
						m_Mobile.Combatant = null;
						return;
					}

					IWeapon weapon = m_Mobile.Weapon;

					if (!m_Mobile.InRange(combatant, weapon.MaxRange))
					{
						return;
					}

					if (m_Mobile.InLOS(combatant))
					{
						weapon.OnBeforeSwing(m_Mobile, combatant); //OnBeforeSwing for checking in regards to being hidden and whatnot
						m_Mobile.RevealingAction();
						m_Mobile.m_NextCombatTime = Core.TickCount + (int)weapon.OnSwing(m_Mobile, combatant).TotalMilliseconds;
					}
				}
			}
		}

		private class ExpireCombatantTimer : Timer
		{
			private readonly Mobile m_Mobile;

			public ExpireCombatantTimer(Mobile m)
				: base(TimeSpan.FromMinutes(1.0))
			{
				Priority = TimerPriority.FiveSeconds;
				m_Mobile = m;
			}

			protected override void OnTick()
			{
				m_Mobile.Combatant = null;
			}
		}

		private static TimeSpan m_ExpireCriminalDelay = TimeSpan.FromMinutes(2.0);

		public static TimeSpan ExpireCriminalDelay { get { return m_ExpireCriminalDelay; } set { m_ExpireCriminalDelay = value; } }

		private class ExpireCriminalTimer : Timer
		{
			private readonly Mobile m_Mobile;

			public ExpireCriminalTimer(Mobile m)
				: base(m_ExpireCriminalDelay)
			{
				Priority = TimerPriority.FiveSeconds;
				m_Mobile = m;
			}

			protected override void OnTick()
			{
				m_Mobile.Criminal = false;
			}
		}

		private class ExpireAggressorsTimer : Timer
		{
			private readonly Mobile m_Mobile;

			public ExpireAggressorsTimer(Mobile m)
				: base(TimeSpan.FromSeconds(5.0), TimeSpan.FromSeconds(5.0))
			{
				m_Mobile = m;
				Priority = TimerPriority.FiveSeconds;
			}

			protected override void OnTick()
			{
				if (m_Mobile.Deleted || (m_Mobile.Aggressors.Count == 0 && m_Mobile.Aggressed.Count == 0))
				{
					m_Mobile.StopAggrExpire();
				}
				else
				{
					m_Mobile.CheckAggrExpire();
				}
			}
		}
		#endregion

		private long m_NextCombatTime;

		public long NextSkillTime { get { return m_NextSkillTime; } set { m_NextSkillTime = value; } }

		public List<AggressorInfo> Aggressors { get { return m_Aggressors; } }

		public List<AggressorInfo> Aggressed { get { return m_Aggressed; } }

		private int m_ChangingCombatant;

		public bool ChangingCombatant { get { return (m_ChangingCombatant > 0); } }

		public virtual void Attack(Mobile m)
		{
			if (CheckAttack(m))
			{
				Combatant = m;
			}
		}

		public virtual bool CheckAttack(Mobile m)
		{
			return (Utility.InUpdateRange(this, m) && CanSee(m) && InLOS(m));
		}

		/// <summary>
		///     Overridable. Gets or sets which Mobile that this Mobile is currently engaged in combat with.
		///     <seealso cref="OnCombatantChange" />
		/// </summary>
		[CommandProperty(AccessLevel.GameMaster)]
		public virtual Mobile Combatant
		{
			get { return m_Combatant; }
			set
			{
				if (m_Deleted)
				{
					return;
				}

				if (m_Combatant != value && value != this)
				{
					Mobile old = m_Combatant;

					++m_ChangingCombatant;
					m_Combatant = value;

					if ((m_Combatant != null && !CanBeHarmful(m_Combatant, false)) || !Region.OnCombatantChange(this, old, m_Combatant))
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
						if (m_ExpireCombatant != null)
						{
							m_ExpireCombatant.Stop();
						}

						if (m_CombatTimer != null)
						{
							m_CombatTimer.Stop();
						}

						m_ExpireCombatant = null;
						m_CombatTimer = null;
					}
					else
					{
						if (m_ExpireCombatant == null)
						{
							m_ExpireCombatant = new ExpireCombatantTimer(this);
						}

						m_ExpireCombatant.Start();

						if (m_CombatTimer == null)
						{
							m_CombatTimer = new CombatTimer(this);
						}

						m_CombatTimer.Start();
					}

					if (m_Combatant != null && CanBeHarmful(m_Combatant, false))
					{
						DoHarmful(m_Combatant);

						if (m_Combatant != null)
						{
							m_Combatant.PlaySound(m_Combatant.GetAngerSound());
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
			int xDelta = m_Location.m_X - p.m_X;
			int yDelta = m_Location.m_Y - p.m_Y;

			return Math.Sqrt((xDelta * xDelta) + (yDelta * yDelta));
		}

		public double GetDistanceToSqrt(Mobile m)
		{
			int xDelta = m_Location.m_X - m.m_Location.m_X;
			int yDelta = m_Location.m_Y - m.m_Location.m_Y;

			return Math.Sqrt((xDelta * xDelta) + (yDelta * yDelta));
		}

		public double GetDistanceToSqrt(IPoint2D p)
		{
			int xDelta = m_Location.m_X - p.X;
			int yDelta = m_Location.m_Y - p.Y;

			return Math.Sqrt((xDelta * xDelta) + (yDelta * yDelta));
		}

		public virtual void AggressiveAction(Mobile aggressor)
		{
			AggressiveAction(aggressor, false);
		}

		public virtual void AggressiveAction(Mobile aggressor, bool criminal)
		{
			if (aggressor == this)
			{
				return;
			}

			AggressiveActionEventArgs args = AggressiveActionEventArgs.Create(this, aggressor, criminal);

			EventSink.InvokeAggressiveAction(args);

			args.Free();

			if (Combatant == aggressor)
			{
				if (m_ExpireCombatant == null)
				{
					m_ExpireCombatant = new ExpireCombatantTimer(this);
				}
				else
				{
					m_ExpireCombatant.Stop();
				}

				m_ExpireCombatant.Start();
			}

			bool addAggressor = true;

			var list = m_Aggressors;

			for (int i = 0; i < list.Count; ++i)
			{
				AggressorInfo info = list[i];

				if (info.Attacker == aggressor)
				{
					info.Refresh();
					info.CriminalAggression = criminal;
					info.CanReportMurder = criminal;

					addAggressor = false;
				}
			}

			list = aggressor.m_Aggressors;

			for (int i = 0; i < list.Count; ++i)
			{
				AggressorInfo info = list[i];

				if (info.Attacker == this)
				{
					info.Refresh();

					addAggressor = false;
				}
			}

			bool addAggressed = true;

			list = m_Aggressed;

			for (int i = 0; i < list.Count; ++i)
			{
				AggressorInfo info = list[i];

				if (info.Defender == aggressor)
				{
					info.Refresh();

					addAggressed = false;
				}
			}

			list = aggressor.m_Aggressed;

			for (int i = 0; i < list.Count; ++i)
			{
				AggressorInfo info = list[i];

				if (info.Defender == this)
				{
					info.Refresh();
					info.CriminalAggression = criminal;
					info.CanReportMurder = criminal;

					addAggressed = false;
				}
			}

			bool setCombatant = false;

			if (addAggressor)
			{
				m_Aggressors.Add(AggressorInfo.Create(aggressor, this, criminal));
				// new AggressorInfo( aggressor, this, criminal, true ) );

				if (CanSee(aggressor) && m_NetState != null)
				{
					m_NetState.Send(MobileIncoming.Create(m_NetState, this, aggressor));
				}

				if (Combatant == null)
				{
					setCombatant = true;
				}

				UpdateAggrExpire();
			}

			if (addAggressed)
			{
				aggressor.m_Aggressed.Add(AggressorInfo.Create(aggressor, this, criminal));
				// new AggressorInfo( aggressor, this, criminal, false ) );

				if (CanSee(aggressor) && m_NetState != null)
				{
					m_NetState.Send(MobileIncoming.Create(m_NetState, this, aggressor));
				}

				if (Combatant == null)
				{
					setCombatant = true;
				}

				UpdateAggrExpire();
			}

			if (setCombatant)
			{
				Combatant = aggressor;
			}

			Region.OnAggressed(aggressor, this, criminal);
		}

		public void RemoveAggressed(Mobile aggressed)
		{
			if (m_Deleted)
			{
				return;
			}

			var list = m_Aggressed;

			for (int i = 0; i < list.Count; ++i)
			{
				AggressorInfo info = list[i];

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

			for (int i = 0; i < list.Count; ++i)
			{
				AggressorInfo info = list[i];

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
		public int TotalGold { get { return GetTotal(TotalType.Gold); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int TotalItems { get { return GetTotal(TotalType.Items); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int TotalWeight { get { return GetTotal(TotalType.Weight); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int TithingPoints
		{
			get { return m_TithingPoints; }
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
			get { return m_Followers; }
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
			get { return m_FollowersMax; }
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

			int oldWeight = m_TotalWeight;

			m_TotalGold = 0;
			m_TotalItems = 0;
			m_TotalWeight = 0;

			for (int i = 0; i < m_Items.Count; ++i)
			{
				Item item = m_Items[i];

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

		public bool TargetLocked { get { return m_TargetLocked; } set { m_TargetLocked = value; } }

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
			get { return m_Target; }
			set
			{
				Target oldTarget = m_Target;
				Target newTarget = value;

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
			get { return m_ContextMenu; }
			set
			{
				m_ContextMenu = value;

				if (m_ContextMenu != null && m_NetState != null)
				{
					// Old packet is preferred until assistants catch up
					if (m_NetState.NewHaven && m_ContextMenu.RequiresNewPacket)
					{
						Send(new DisplayContextMenu(m_ContextMenu));
					}
					else
					{
						Send(new DisplayContextMenuOld(m_ContextMenu));
					}
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
			get { return m_Prompt; }
			set
			{
				Prompt oldPrompt = m_Prompt;
				Prompt newPrompt = value;

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
					Send(new UnicodePrompt(newPrompt));
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

			MovementEventArgs e = MovementEventArgs.Create(this, d);

			EventSink.InvokeMovement(e);

			bool ret = !e.Blocked;

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

		private static readonly Packet[][] m_MovingPacketCache = new Packet[2][] {new Packet[8], new Packet[8]};

		private bool m_Pushing;
        private bool m_IgnoreMobiles;
        private bool m_IsStealthing;

		public bool Pushing { get { return m_Pushing; } set { m_Pushing = value; } }

		private static int m_WalkFoot = 400;
		private static int m_RunFoot = 200;
		private static int m_WalkMount = 200;
		private static int m_RunMount = 100;

		public static int WalkFoot { get { return m_WalkFoot; } set { m_WalkFoot = value; } }
		public static int RunFoot { get { return m_RunFoot; } set { m_RunFoot = value; } }
		public static int WalkMount { get { return m_WalkMount; } set { m_WalkMount = value; } }
		public static int RunMount { get { return m_RunMount; } set { m_RunMount = value; } }

		private long m_EndQueue;

		private static readonly List<IEntity> m_MoveList = new List<IEntity>();
		private static readonly List<Mobile> m_MoveClientList = new List<Mobile>();

		private static AccessLevel m_FwdAccessOverride = AccessLevel.Counselor;
		private static bool m_FwdEnabled = true;
		private static bool m_FwdUOTDOverride;
		private static int m_FwdMaxSteps = 4;

		public static AccessLevel FwdAccessOverride { get { return m_FwdAccessOverride; } set { m_FwdAccessOverride = value; } }
		public static bool FwdEnabled { get { return m_FwdEnabled; } set { m_FwdEnabled = value; } }
		public static bool FwdUOTDOverride { get { return m_FwdUOTDOverride; } set { m_FwdUOTDOverride = value; } }
		public static int FwdMaxSteps { get { return m_FwdMaxSteps; } set { m_FwdMaxSteps = value; } }

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
			return Movement.Movement.CheckMovement(this, d, out newZ);
		}

		public virtual bool Move(Direction d)
		{
			if (m_Deleted)
			{
				return false;
			}

			BankBox box = FindBankNoCreate();

			if (box != null && box.Opened)
			{
				box.Close();
			}

			Point3D newLocation = m_Location;
			Point3D oldLocation = newLocation;

			if ((m_Direction & Direction.Mask) == (d & Direction.Mask))
			{
				// We are actually moving (not just a direction change)

				if (m_Spell != null && !m_Spell.OnCasterMoving(d))
				{
					return false;
				}

				if (m_Paralyzed || m_Frozen || _Sleep)
				{
					SendLocalizedMessage(500111); // You are frozen and can not move.

					return false;
				}

				int newZ;

				if (CheckMovement(d, out newZ))
				{
					int x = oldLocation.m_X, y = oldLocation.m_Y;
					int oldX = x, oldY = y;
					int oldZ = oldLocation.m_Z;

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

					Map map = m_Map;

					if (map != null)
					{
						Sector oldSector = map.GetSector(oldX, oldY);
						Sector newSector = map.GetSector(x, y);

						if (oldSector != newSector)
						{
							for (int i = 0; i < oldSector.Mobiles.Count; ++i)
							{
								Mobile m = oldSector.Mobiles[i];

								if (m != this && m.X == oldX && m.Y == oldY && (m.Z + 15) > oldZ && (oldZ + 15) > m.Z && !m.OnMoveOff(this))
								{
									return false;
								}
							}

							for (int i = 0; i < oldSector.Items.Count; ++i)
							{
								Item item = oldSector.Items[i];

								if (item.AtWorldPoint(oldX, oldY) &&
									(item.Z == oldZ || ((item.Z + item.ItemData.Height) > oldZ && (oldZ + 15) > item.Z)) && !item.OnMoveOff(this))
								{
									return false;
								}
							}

							for (int i = 0; i < newSector.Mobiles.Count; ++i)
							{
								Mobile m = newSector.Mobiles[i];

								if (m.X == x && m.Y == y && (m.Z + 15) > newZ && (newZ + 15) > m.Z && !m.OnMoveOver(this))
								{
									return false;
								}
							}

							for (int i = 0; i < newSector.Items.Count; ++i)
							{
								Item item = newSector.Items[i];

								if (item.AtWorldPoint(x, y) &&
									(item.Z == newZ || ((item.Z + item.ItemData.Height) > newZ && (newZ + 15) > item.Z)) && !item.OnMoveOver(this))
								{
									return false;
								}
							}
						}
						else
						{
							for (int i = 0; i < oldSector.Mobiles.Count; ++i)
							{
								Mobile m = oldSector.Mobiles[i];

								if (m != this && m.X == oldX && m.Y == oldY && (m.Z + 15) > oldZ && (oldZ + 15) > m.Z && !m.OnMoveOff(this))
								{
									return false;
								}
								else if (m.X == x && m.Y == y && (m.Z + 15) > newZ && (newZ + 15) > m.Z && !m.OnMoveOver(this))
								{
									return false;
								}
							}

							for (int i = 0; i < oldSector.Items.Count; ++i)
							{
								Item item = oldSector.Items[i];

								if (item.AtWorldPoint(oldX, oldY) &&
									(item.Z == oldZ || ((item.Z + item.ItemData.Height) > oldZ && (oldZ + 15) > item.Z)) && !item.OnMoveOff(this))
								{
									return false;
								}
								else if (item.AtWorldPoint(x, y) &&
										 (item.Z == newZ || ((item.Z + item.ItemData.Height) > newZ && (newZ + 15) > item.Z)) && !item.OnMoveOver(this))
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
							MovementRecord r = m_MoveRecords.Peek();

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
							FastWalkEventArgs fw = new FastWalkEventArgs(m_NetState);
							EventSink.InvokeFastWalk(fw);

							if (fw.Blocked)
							{
								return false;
							}
						}

						int delay = ComputeMovementSpeed(d);

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
				//new MovementAck( m_NetState.Sequence, this ) );
			}

			SetLocation(newLocation, false);
			SetDirection(d);

			if (m_Map != null)
			{
				var eable = m_Map.GetObjectsInRange(m_Location, Core.GlobalMaxUpdateRange);

				foreach (IEntity o in eable)
				{
					if (o == this)
					{
						continue;
					}

					if (o is Mobile)
					{
						Mobile mob = o as Mobile;
						if (mob.NetState != null)
						{
							m_MoveClientList.Add(mob);
						}
						m_MoveList.Add(o);
					}
					else if (o is Item)
					{
						Item item = (Item)o;

						if (item.HandlesOnMovement)
						{
							m_MoveList.Add(item);
						}
					}
				}

				eable.Free();

				var cache = m_MovingPacketCache;

				/*for( int i = 0; i < cache.Length; ++i )
					for (int j = 0; j < cache[i].Length; ++j)
						Packet.Release( ref cache[i][j] );*/

				foreach (Mobile m in m_MoveClientList)
				{
					NetState ns = m.NetState;

					if (ns != null && Utility.InUpdateRange(m_Location, m.m_Location) && m.CanSee(this))
					{
						if (ns.StygianAbyss)
						{
							Packet p;
							int noto = Notoriety.Compute(m, this);
							p = cache[0][noto];

							if (p == null)
							{
								cache[0][noto] = p = Packet.Acquire(new MobileMoving(this, noto));
							}

							ns.Send(p);
						}
						else
						{
							Packet p;
							int noto = Notoriety.Compute(m, this);
							p = cache[1][noto];

							if (p == null)
							{
								cache[1][noto] = p = Packet.Acquire(new MobileMovingOld(this, noto));
							}

							ns.Send(p);
						}
					}
				}

				for (int i = 0; i < cache.Length; ++i)
				{
					for (int j = 0; j < cache[i].Length; ++j)
					{
						Packet.Release(ref cache[i][j]);
					}
				}

				for (int i = 0; i < m_MoveList.Count; ++i)
				{
					IEntity o = m_MoveList[i];

					if (o is Mobile)
					{
						((Mobile)o).OnMovement(this, oldLocation);
					}
					else if (o is Item)
					{
						((Item)o).OnMovement(this, oldLocation);
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

		public virtual bool IsDeadBondedPet { get { return false; } }

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
				else if (shoved.m_Hidden && shoved.IsStaff())
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

		public ISpell Spell
		{
			get { return m_Spell; }
			set
			{
				//if( m_Spell != null && value != null )
				//	Console.WriteLine("Warning: Spell has been overwritten");

				m_Spell = value;
			}
		}

		[CommandProperty(AccessLevel.Administrator)]
		public bool AutoPageNotify { get { return m_AutoPageNotify; } set { m_AutoPageNotify = value; } }

		public virtual void CriminalAction(bool message)
		{
			if (m_Deleted)
			{
				return;
			}

			Criminal = true;

			Region.OnCriminalAction(this, message);
		}

		public virtual bool CanUseStuckMenu()
		{
			if (m_StuckMenuUses == null)
			{
				return true;
			}
			else
			{
				for (int i = 0; i < m_StuckMenuUses.Length; ++i)
				{
					if ((DateTime.UtcNow - m_StuckMenuUses[i]) > TimeSpan.FromDays(1.0))
					{
						return true;
					}
				}

				return false;
			}
		}

		public virtual bool IsPlayer()
		{
			return Utilities.IsPlayer(this);
		}

		public virtual bool IsStaff()
		{
			return Utilities.IsStaff(this);
		}

		public virtual bool IsOwner()
		{
			return Utilities.IsOwner(this);
		}

		public virtual bool IsSnoop(Mobile from)
		{
			return (from != this);
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

				BankBox box = FindBankNoCreate();

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

				for (int i = m_Items.Count - 1; i >= 0; --i)
				{
					if (i >= m_Items.Count)
					{
						continue;
					}

					Item item = m_Items[i];

					if (item.ItemID == 0x204E)
					{
						item.Delete();
					}
				}

				SendIncomingPacket();
				SendIncomingPacket();

				OnAfterResurrect();

				//Send( new DeathStatus( false ) );
			}
		}

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
		public IAccount Account { get; set; }

		private bool m_Deleted;

		public bool Deleted { get { return m_Deleted; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int VirtualArmor
		{
			get { return m_VirtualArmor; }
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
		public virtual double ArmorRating { get { return 0.0; } }

		public void DropHolding()
		{
			Item holding = m_Holding;

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
			else if (!World.OnDelete(this))
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

			for (int i = m_Items.Count - 1; i >= 0; --i)
			{
				if (i < m_Items.Count)
				{
					m_Items[i].OnParentDeleted(this);
				}
			}

			for (int i = 0; i < m_Stabled.Count; i++)
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
		public virtual bool Alive { get { return !m_Deleted && (!m_Player || !m_Body.IsGhost); } }

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

			if (m_HitsTimer != null)
			{
				m_HitsTimer.Stop();
			}

			if (m_StamTimer != null)
			{
				m_StamTimer.Stop();
			}

			if (m_ManaTimer != null)
			{
				m_ManaTimer.Stop();
			}

			if (m_CombatTimer != null)
			{
				m_CombatTimer.Stop();
			}

			if (m_ExpireCombatant != null)
			{
				m_ExpireCombatant.Stop();
			}

			if (m_LogoutTimer != null)
			{
				m_LogoutTimer.Stop();
			}

			if (m_ExpireCriminal != null)
			{
				m_ExpireCriminal.Stop();
			}

			if (m_WarmodeTimer != null)
			{
				m_WarmodeTimer.Stop();
			}

			if (m_ParaTimer != null)
			{
				m_ParaTimer.Stop();
			}

			if (_SleepTimer != null)
			{
				_SleepTimer.Stop();
			}

			if (m_FrozenTimer != null)
			{
				m_FrozenTimer.Stop();
			}

			if (m_AutoManifestTimer != null)
			{
				m_AutoManifestTimer.Stop();
			}

			foreach (BaseModule module in World.GetModules(this))
			{
				if (module != null)
				{
					module.Delete();
				}
			}
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

		public static CreateCorpseHandler CreateCorpseHandler { get { return m_CreateCorpse; } set { m_CreateCorpse = value; } }

		public virtual DeathMoveResult GetParentMoveResultFor(Item item)
		{
			return item.OnParentDeath(this);
		}

		public virtual DeathMoveResult GetInventoryMoveResultFor(Item item)
		{
			return item.OnInventoryDeath(this);
		}

		public virtual bool RetainPackLocsOnDeath { get { return Core.AOS; } }

		public virtual void Kill()
		{
			if (!CanBeDamaged())
			{
				return;
			}
			else if (!Alive || IsDeadBondedPet)
			{
				return;
			}
			else if (m_Deleted)
			{
				return;
			}
			else if (!Region.OnBeforeDeath(this))
			{
				return;
			}
			else if (!OnBeforeDeath())
			{
				return;
			}

			BankBox box = FindBankNoCreate();

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
			//m_Spell.Disturb( DisturbType.Kill );

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

				if (m_ParaTimer != null)
				{
					m_ParaTimer.Stop();
				}
			}

			if (Frozen)
			{
				Frozen = false;

				if (m_FrozenTimer != null)
				{
					m_FrozenTimer.Stop();
				}
			}

			if (Asleep)
			{
				Asleep = false;
				Send(SpeedControl.Disable);

				if (_SleepTimer != null)
				{
					_SleepTimer.Stop();
				}
			}

			var content = new List<Item>();
			var equip = new List<Item>();
			var moveToPack = new List<Item>();

			var itemsCopy = new List<Item>(m_Items);

			Container pack = Backpack;

			for (int i = 0; i < itemsCopy.Count; ++i)
			{
				Item item = itemsCopy[i];

				if (item == pack)
				{
					continue;
				}

				if ((item.Insured || item.LootType == LootType.Blessed) && item.Parent == this && item.Layer != Layer.Mount)
				{
					equip.Add(item);
				}

				DeathMoveResult res = GetParentMoveResultFor(item);

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

				for (int i = 0; i < packCopy.Count; ++i)
				{
					Item item = packCopy[i];

					DeathMoveResult res = GetInventoryMoveResultFor(item);

					if (res == DeathMoveResult.MoveToCorpse)
					{
						content.Add(item);
					}
					else
					{
						moveToPack.Add(item);
					}
				}

				for (int i = 0; i < moveToPack.Count; ++i)
				{
					Item item = moveToPack[i];

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

			Container c = (m_CreateCorpse == null ? null : m_CreateCorpse(this, hair, facialhair, content, equip));

			/*m_Corpse = c;

			for ( int i = 0; c != null && i < content.Count; ++i )
			c.DropItem( (Item)content[i] );

			if ( c != null )
			c.MoveToWorld( this.Location, this.Map );*/

			if (m_Map != null)
			{
				Packet animPacket = null;

				var eable = m_Map.GetClientsInRange(m_Location);

				foreach (NetState state in eable)
				{
					if (state != m_NetState)
					{
						if (animPacket == null)
						{
							animPacket = Packet.Acquire(new DeathAnimation(this, c));
						}
						;

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
		public Container Corpse { get { return m_Corpse; } set { m_Corpse = value; } }

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
			int sound = GetDeathSound();

			if (sound >= 0)
			{
				Effects.PlaySound(this, Map, sound);
			}

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

				Item deathShroud = new Item(0x204E);

				deathShroud.Movable = false;
				deathShroud.Layer = Layer.OuterTorso;

				AddItem(deathShroud);

				m_Items.Remove(deathShroud);
				m_Items.Insert(0, deathShroud);

				Poison = null;
				Combatant = null;

				Hits = 0;
				Stam = 0;
				Mana = 0;

				EventSink.InvokePlayerDeath(new PlayerDeathEventArgs(this));

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
			else if (m_Body.IsHuman)
			{
				return Utility.Random(m_Female ? 0x314 : 0x423, m_Female ? 4 : 5);
			}
			else
			{
				return -1;
			}
		}
		#endregion

		private static char[] m_GhostChars = new char[2] {'o', 'O'};

		public static char[] GhostChars { get { return m_GhostChars; } set { m_GhostChars = value; } }

		private static bool m_NoSpeechLOS;

		public static bool NoSpeechLOS { get { return m_NoSpeechLOS; } set { m_NoSpeechLOS = value; } }

		private static TimeSpan m_AutoManifestTimeout = TimeSpan.FromSeconds(5.0);

		public static TimeSpan AutoManifestTimeout { get { return m_AutoManifestTimeout; } set { m_AutoManifestTimeout = value; } }

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

			object root = item.RootParent;
			bool okay = false;

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
				Region reg = Region.Find(item.GetWorldLocation(), item.Map);

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
			else if (root != null && root is Mobile && ((Mobile)root).IsSnoop(this))
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

		private static int m_ActionDelay = 500;

		public static int ActionDelay { get { return m_ActionDelay; } set { m_ActionDelay = value; } }

		public virtual void Lift(Item item, int amount, out bool rejected, out LRReason reject)
		{
			rejected = true;
			reject = LRReason.Inspecific;

			if (item == null)
			{
				return;
			}

			Mobile from = this;
			NetState state = m_NetState;

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
					#region Mondain's Legacy
					else if (item.QuestItem && amount != item.Amount && !from.IsStaff())
					{
						reject = LRReason.Inspecific;
						from.SendLocalizedMessage(1074868); // Stacks of quest items cannot be unstacked.
					}
					#endregion

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
						object root = item.RootParent;

						if (root != null && root is Mobile && !((Mobile)root).CheckNonlocalLift(from, item))
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

							if (item.Spawner != null)
							{
								item.Spawner.Remove(item);
								item.Spawner = null;
							}

							if (amount == 0)
							{
								amount = 1;
							}

							if (amount > item.Amount)
							{
								amount = item.Amount;
							}

							int oldAmount = item.Amount;

                            Item oldStack = null;

                            if (amount < oldAmount)
                            {
                                oldStack = LiftItemDupe(item, amount);
                            }

							Map map = from.Map;

							if (m_DragEffects && map != null && (root == null || root is Item))
							{
								var eable = map.GetClientsInRange(from.Location);
								Packet p = null;

								foreach (NetState ns in eable)
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

							Point3D fixLoc = item.Location;
							Map fixMap = item.Map;
							bool shouldFix = (item.Parent == null);

							item.RecordBounce(oldStack);
							item.OnItemLifted(from, item);
							item.Internalize();

							from.Holding = item;

							int liftSound = item.GetLiftSound(from);

							if (liftSound != -1)
							{
								from.Send(new PlaySound(liftSound, from));
							}

							from.NextActionTime = Core.TickCount + m_ActionDelay;

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
					if (state.ContainerGridLines)
					{
						state.Send(new ContainerContentUpdate6017(item));
					}
					else
					{
						state.Send(new ContainerContentUpdate(item));
					}
				}
				else if (item.Parent is Mobile)
				{
					state.Send(new EquipUpdate(item));
				}
				else
				{
					item.SendInfoTo(state);
				}

				if (ObjectPropertyList.Enabled && item.Parent != null)
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

			if (oldItem.Parent is Mobile)
			{
				((Mobile)oldItem.Parent).AddItem(item);
			}
			else if (oldItem.Parent is Item)
			{
				((Item)oldItem.Parent).AddItem(item);
			}

			item.Delta(ItemDelta.Update);

			return item;
		}

		public virtual void SendDropEffect(Item item)
		{
			if (m_DragEffects && !item.Deleted)
			{
				Map map = m_Map;
				object root = item.RootParent;

				if (map != null && (root == null || root is Item))
				{
					var eable = map.GetClientsInRange(m_Location);
					Packet p = null;

					foreach(NetState ns in eable) {
						if (ns.StygianAbyss)
								continue;

						if( ns.Mobile != this && ns.Mobile.CanSee( this ) && ns.Mobile.InLOS( this ) && ns.Mobile.CanSee( root ) ) {
							if (p == null) {
								IEntity trg;

								if (root == null)
									trg = new Entity(Serial.Zero, item.Location, map);
								else
									trg = new Entity(((Item)root).Serial, ((Item)root).Location, map);

								p = Packet.Acquire(new DragEffect(this, trg, item.ItemID, item.Hue, item.Amount));
							}

							ns.Send(p);
						}
					}

					Packet.Release(p);

					eable.Free();
				}
			}
		}

		public virtual bool Drop(Item to, Point3D loc)
		{
			Mobile from = this;
			Item item = from.Holding;

			bool valid = (item != null && item.HeldBy == from && item.Map == Map.Internal);

			from.Holding = null;

			if (!valid)
			{
				return false;
			}

			bool bounced = true;

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

			if (!bounced)
			{
				SendDropEffect(item);
			}

			return !bounced;
		}

		public virtual bool Drop(Point3D loc)
		{
			Mobile from = this;
			Item item = from.Holding;

			bool valid = (item != null && item.HeldBy == from && item.Map == Map.Internal);

			from.Holding = null;

			if (!valid)
			{
				return false;
			}

			bool bounced = true;

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

			if (!bounced)
			{
				SendDropEffect(item);
			}

			return !bounced;
		}

		public virtual bool Drop(Mobile to, Point3D loc)
		{
			Mobile from = this;
			Item item = from.Holding;

			bool valid = (item != null && item.HeldBy == from && item.Map == Map.Internal);

			from.Holding = null;

			if (!valid)
			{
				return false;
			}

			bool bounced = true;

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

			if (!bounced)
			{
				SendDropEffect(item);
			}

			return !bounced;
		}

		private static readonly object m_GhostMutateContext = new object();

		public virtual bool MutateSpeech(List<Mobile> hears, ref string text, ref object context)
		{
			if (Alive)
			{
				return false;
			}

			StringBuilder sb = new StringBuilder(text.Length, text.Length);

			for (int i = 0; i < text.Length; ++i)
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

			TimeSpan delay = m_AutoManifestTimeout;

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
				return (m.Alive && !m.CanHearGhosts);
			}

			return true;
		}

		private void AddSpeechItemsFrom(List<IEntity> list, Container cont)
		{
			for (int i = 0; i < cont.Items.Count; ++i)
			{
				Item item = cont.Items[i];

				if (item.HandlesOnSpeech)
				{
					list.Add(item);
				}

				if (item is Container)
				{
					AddSpeechItemsFrom(list, (Container)item);
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

			public IEntity RelativeTo { get { return m_RelativeTo; } set { m_RelativeTo = value; } }

			public LocationComparer(IEntity relativeTo)
			{
				m_RelativeTo = relativeTo;
			}

			private int GetDistance(IEntity p)
			{
				int x = m_RelativeTo.X - p.X;
				int y = m_RelativeTo.Y - p.Y;
				int z = m_RelativeTo.Z - p.Z;

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
			Map map = m_Map;

			if (map == null)
			{
				return Map.NullEnumerable<Item>.Instance;
			}

			return map.GetItemsInRange(m_Location, range);
		}

		public IPooledEnumerable<IEntity> GetObjectsInRange(int range)
		{
			Map map = m_Map;

			if (map == null)
			{
				return Map.NullEnumerable<IEntity>.Instance;
			}

			return map.GetObjectsInRange(m_Location, range);
		}

		public IPooledEnumerable<Mobile> GetMobilesInRange(int range)
		{
			Map map = m_Map;

			if (map == null)
			{
				return Map.NullEnumerable<Mobile>.Instance;
			}

			return map.GetMobilesInRange(m_Location, range);
		}

		public IPooledEnumerable<NetState> GetClientsInRange(int range)
		{
			Map map = m_Map;

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

			int range = 15;

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

			SpeechEventArgs regArgs = new SpeechEventArgs(this, text, type, hue, keywords);

			EventSink.InvokeSpeech(regArgs);
			Region.OnSpeech(regArgs);
			OnSaid(regArgs);

			if (regArgs.Blocked)
			{
				return;
			}

			text = regArgs.Speech;

			if (string.IsNullOrEmpty(text))
			{
				return;
			}

			var hears = m_Hears;
			var onSpeech = m_OnSpeech;

			if (m_Map != null)
			{
				var eable = m_Map.GetObjectsInRange(m_Location, range);

				foreach (IEntity o in eable)
				{
					if (o is Mobile)
					{
						Mobile heard = (Mobile)o;

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

							for (int i = 0; i < heard.Items.Count; ++i)
							{
								Item item = heard.Items[i];

								if (item.HandlesOnSpeech)
								{
									onSpeech.Add(item);
								}

								if (item is Container)
								{
									AddSpeechItemsFrom(onSpeech, (Container)item);
								}
							}
						}
					}
					else if (o is Item)
					{
						if (((Item)o).HandlesOnSpeech)
						{
							onSpeech.Add(o);
						}

						if (o is Container)
						{
							AddSpeechItemsFrom(onSpeech, (Container)o);
						}
					}
				}

				eable.Free();

				object mutateContext = null;
				string mutatedText = text;
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

				for (int i = 0; i < hears.Count; ++i)
				{
					Mobile heard = hears[i];

					if (mutatedArgs == null || !CheckHearsMutatedSpeech(heard, mutateContext))
					{
						heard.OnSpeech(regArgs);

						NetState ns = heard.NetState;

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

						NetState ns = heard.NetState;

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

				for (int i = 0; i < onSpeech.Count; ++i)
				{
					IEntity obj = onSpeech[i];

					if (obj is Mobile)
					{
						Mobile heard = (Mobile)obj;

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
						Item item = (Item)obj;

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

		public static VisibleDamageType VisibleDamageType { get { return m_VisibleDamageType; } set { m_VisibleDamageType = value; } }

		private List<DamageEntry> m_DamageEntries;

		public List<DamageEntry> DamageEntries { get { return m_DamageEntries; } }

		public static Mobile GetDamagerFrom(DamageEntry de)
		{
			return (de == null ? null : de.Damager);
		}

		public Mobile FindMostRecentDamager(bool allowSelf)
		{
			return GetDamagerFrom(FindMostRecentDamageEntry(allowSelf));
		}

		public DamageEntry FindMostRecentDamageEntry(bool allowSelf)
		{
			for (int i = m_DamageEntries.Count - 1; i >= 0; --i)
			{
				if (i >= m_DamageEntries.Count)
				{
					continue;
				}

				DamageEntry de = m_DamageEntries[i];

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
			for (int i = 0; i < m_DamageEntries.Count; ++i)
			{
				if (i < 0)
				{
					continue;
				}

				DamageEntry de = m_DamageEntries[i];

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

			for (int i = m_DamageEntries.Count - 1; i >= 0; --i)
			{
				if (i >= m_DamageEntries.Count)
				{
					continue;
				}

				DamageEntry de = m_DamageEntries[i];

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

			for (int i = m_DamageEntries.Count - 1; i >= 0; --i)
			{
				if (i >= m_DamageEntries.Count)
				{
					continue;
				}

				DamageEntry de = m_DamageEntries[i];

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
			for (int i = m_DamageEntries.Count - 1; i >= 0; --i)
			{
				if (i >= m_DamageEntries.Count)
				{
					continue;
				}

				DamageEntry de = m_DamageEntries[i];

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
			DamageEntry de = FindDamageEntryFor(from);

			if (de == null)
			{
				de = new DamageEntry(from);
			}

			de.DamageGiven += amount;
			de.LastDamage = DateTime.UtcNow;

			m_DamageEntries.Remove(de);
			m_DamageEntries.Add(de);

			Mobile master = from.GetDamageMaster(this);

			if (master != null)
			{
				var list = de.Responsible;

				if (list == null)
				{
					de.Responsible = list = new List<DamageEntry>();
				}

				DamageEntry resp = null;

				for (int i = 0; i < list.Count; ++i)
				{
					DamageEntry check = list[i];

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

		[CommandProperty(AccessLevel.GameMaster)]
		public Mobile LastKiller { get { return m_LastKiller; } set { m_LastKiller = value; } }

		/// <summary>
		///     Overridable. Virtual event invoked when the Mobile is <see cref="Damage">damaged</see>. It is called before
		///     <see
		///         cref="Hits">
		///         hit points
		///     </see>
		///     are lowered or the Mobile is <see cref="Kill">killed</see>.
		///     <seealso cref="Damage" />
		///     <seealso cref="Hits" />
		///     <seealso cref="Kill" />
		/// </summary>
		public virtual void OnDamage(int amount, Mobile from, bool willKill)
		{ }

		public virtual void Damage(int amount)
		{
			Damage(amount, null);
		}

		public virtual bool CanBeDamaged()
		{
			return !m_Blessed;
		}

		public virtual void Damage(int amount, Mobile from)
		{
			Damage(amount, from, true);
		}

        public virtual void Damage(int amount, Mobile from, bool informMount)
        {
            Damage(amount, from, informMount, true);
        }

		public virtual void Damage(int amount, Mobile from, bool informMount, bool checkDisrupt)
		{
			if (!CanBeDamaged() || m_Deleted)
			{
				return;
			}

			if (!Region.OnDamage(this, ref amount))
			{
				return;
			}

			if (amount > 0)
			{
				int oldHits = Hits;
				int newHits = oldHits - amount;

                if (checkDisrupt && m_Spell != null)
				{
					m_Spell.OnCasterHurt();
				}

				//if ( m_Spell != null && m_Spell.State == SpellState.Casting )
				//	m_Spell.Disturb( DisturbType.Hurt, false, true );

				if (from != null)
				{
					RegisterDamage(amount, from);
				}

				DisruptiveAction();

				Paralyzed = false;

				if (Asleep)
				{
					Asleep = false;

					if (from != null)
					{
						from.Send(SpeedControl.Disable);
					}
				}

				switch (m_VisibleDamageType)
				{
					case VisibleDamageType.Related:
						{
							NetState ourState = m_NetState, theirState = (from == null ? null : from.m_NetState);

							if (ourState == null)
							{
								Mobile master = GetDamageMaster(from);

								if (master != null)
								{
									ourState = master.m_NetState;
								}
							}

							if (theirState == null && from != null)
							{
								Mobile master = from.GetDamageMaster(this);

								if (master != null)
								{
									theirState = master.m_NetState;
								}
							}

							if (amount > 0 && (ourState != null || theirState != null))
							{
								Packet p = null; // = new DamagePacket( this, amount );

								if (ourState != null)
								{
									if (ourState.DamagePacket)
									{
										p = Packet.Acquire(new DamagePacket(this, amount));
									}
									else
									{
										p = Packet.Acquire(new DamagePacketOld(this, amount));
									}

									ourState.Send(p);
								}

								if (theirState != null && theirState != ourState)
								{
									bool newPacket = theirState.DamagePacket;

									if (newPacket && (p == null || !(p is DamagePacket)))
									{
										Packet.Release(p);
										p = Packet.Acquire(new DamagePacket(this, amount));
									}
									else if (!newPacket && (p == null || !(p is DamagePacketOld)))
									{
										Packet.Release(p);
										p = Packet.Acquire(new DamagePacketOld(this, amount));
									}

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

				OnDamage(amount, from, newHits < 0);

				IMount m = Mount;
				if (m != null && informMount)
				{
					m.OnRiderDamaged(amount, from, newHits < 0);
				}

				if (newHits < 0)
				{
					m_LastKiller = from;

					Hits = 0;

					if (oldHits >= 0)
					{
						Kill();
					}
				}
				else
				{
					Hits = newHits;
				}
			}
		}

		public virtual void SendDamageToAll(int amount)
		{
			if (amount < 0)
			{
				return;
			}

			Map map = m_Map;

			if (map == null)
			{
				return;
			}

			var eable = map.GetClientsInRange(m_Location);

			Packet pNew = null;
			Packet pOld = null;

			foreach (NetState ns in eable)
			{
				if (ns.Mobile.CanSee(this))
				{
					if (ns.DamagePacket)
					{
						if (pNew == null)
						{
							pNew = Packet.Acquire(new DamagePacket(this, amount));
						}

						ns.Send(pNew);
					}
					else
					{
						if (pOld == null)
						{
							pOld = Packet.Acquire(new DamagePacketOld(this, amount));
						}

						ns.Send(pOld);
					}
				}
			}

			Packet.Release(pNew);
			Packet.Release(pOld);

			eable.Free();
		}

		public void Heal(int amount)
		{
			Heal(amount, this, true);
		}

		public void Heal(int amount, Mobile from)
		{
			Heal(amount, from, true);
		}

		public void Heal(int amount, Mobile from, bool message)
		{
			if (!Alive || IsDeadBondedPet)
			{
				return;
			}

			if (!Region.OnHeal(this, ref amount))
			{
				return;
			}

			OnHeal(ref amount, from);

			if ((Hits + amount) > HitsMax)
			{
				amount = HitsMax - Hits;
			}

			Hits += amount;

			if (message && amount > 0 && m_NetState != null)
			{
				m_NetState.Send(
					new MessageLocalizedAffix(
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
		}

		public virtual void OnHeal(ref int amount, Mobile from)
		{ }

		public void UsedStuckMenu()
		{
			if (m_StuckMenuUses == null)
			{
				m_StuckMenuUses = new DateTime[2];
			}

			for (int i = 0; i < m_StuckMenuUses.Length; ++i)
			{
				if ((DateTime.UtcNow - m_StuckMenuUses[i]) > TimeSpan.FromDays(1.0))
				{
					m_StuckMenuUses[i] = DateTime.UtcNow;
					return;
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Squelched { get { return m_Squelched; } set { m_Squelched = value; } }

		public virtual void Deserialize(GenericReader reader)
		{
			int version = reader.ReadInt();

			switch (version)
			{
                case 33:
                    {
                        m_SpecialSlayerMechanics = reader.ReadBool();

                        if (reader.ReadBool())
                        {
                            int length = reader.ReadInt();

                            for (int i = 0; i < length; i++)
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
						byte hairflag = reader.ReadByte();

						if ((hairflag & 0x01) != 0)
						{
							m_Hair = new HairInfo(reader);
						}

						if ((hairflag & 0x02) != 0)
						{
							m_FacialHair = new FacialHairInfo(reader);
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
						m_DisarmReady = reader.ReadBool();
						m_StunReady = reader.ReadBool();

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
							m_StatCap = 225;
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

							for (int i = 0; i < m_StuckMenuUses.Length; ++i)
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
							if (m_ExpireCriminal == null)
							{
								m_ExpireCriminal = new ExpireCriminalTimer(this);
							}

							m_ExpireCriminal.Start();
						}

						if (ShouldCheckStatTimers)
						{
							CheckStatTimers();
						}

						if (!m_Player && m_Dex <= 100 && m_CombatTimer != null)
						{
							m_CombatTimer.Priority = TimerPriority.FiftyMS;
						}
						else if (m_CombatTimer != null)
						{
							m_CombatTimer.Priority = TimerPriority.EveryTick;
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

		public virtual bool ShouldCheckStatTimers { get { return true; } }

		public virtual void CheckStatTimers()
		{
			if (m_Deleted)
			{
				return;
			}

			if (Hits < HitsMax)
			{
				if (CanRegenHits)
				{
					if (m_HitsTimer == null)
					{
						m_HitsTimer = new HitsTimer(this);
					}

					m_HitsTimer.Start();
				}
				else if (m_HitsTimer != null)
				{
					m_HitsTimer.Stop();
				}
			}
			else
			{
				Hits = HitsMax;
			}

			if (Stam < StamMax)
			{
				if (CanRegenStam)
				{
					if (m_StamTimer == null)
					{
						m_StamTimer = new StamTimer(this);
					}

					m_StamTimer.Start();
				}
				else if (m_StamTimer != null)
				{
					m_StamTimer.Stop();
				}
			}
			else
			{
				Stam = StamMax;
			}

			if (Mana < ManaMax)
			{
				if (CanRegenMana)
				{
					if (m_ManaTimer == null)
					{
						m_ManaTimer = new ManaTimer(this);
					}

					m_ManaTimer.Start();
				}
				else if (m_ManaTimer != null)
				{
					m_ManaTimer.Stop();
				}
			}
			else
			{
				Mana = ManaMax;
			}
		}

		private DateTime m_CreationTime;

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime CreationTime { get { return m_CreationTime; } }

		int ISerializable.TypeReference { get { return m_TypeRef; } }

		int ISerializable.SerialIdentity { get { return m_Serial; } }

		public virtual void Serialize(GenericWriter writer)
		{
			writer.Write(33); // version

            writer.Write(m_SpecialSlayerMechanics);

            if (m_SlayerVulnerabilities != null && m_SlayerVulnerabilities.Count > 0)
            {
                writer.Write(true);

                writer.Write(m_SlayerVulnerabilities.Count);

                for (int i = 0; i < m_SlayerVulnerabilities.Count; i++)
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

			writer.Write(hairflag);

			if ((hairflag & 0x01) != 0)
			{
				m_Hair.Serialize(writer);
			}

			if ((hairflag & 0x02) != 0)
			{
				m_FacialHair.Serialize(writer);
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

			writer.Write(m_DisarmReady);
			writer.Write(m_StunReady);

			//Poison.Serialize( m_Poison, writer );

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

				for (int i = 0; i < m_StuckMenuUses.Length; ++i)
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
			get { return m_LightLevel; }
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
		public string Profile { get { return m_Profile; } set { m_Profile = value; } }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public bool ProfileLocked { get { return m_ProfileLocked; } set { m_ProfileLocked = value; } }

		[CommandProperty(AccessLevel.GameMaster, AccessLevel.Administrator)]
		public bool Player
		{
			get { return m_Player; }
			set
			{
				m_Player = value;
				InvalidateProperties();

				if (!m_Player && m_Dex <= 100 && m_CombatTimer != null)
				{
					m_CombatTimer.Priority = TimerPriority.FiftyMS;
				}
				else if (m_CombatTimer != null)
				{
					m_CombatTimer.Priority = TimerPriority.EveryTick;
				}

				CheckStatTimers();
			}
		}

		[CommandProperty(AccessLevel.Decorator)]
		public string Title
		{
			get { return m_Title; }
			set
			{
				m_Title = value;
				InvalidateProperties();
			}
		}

		private static readonly string[] m_AccessLevelNames = new[]
		{
			"Player", "VIP Player", "Counselor", "Decorator", "Spawner", "Game Master", "Seer", "Administrator", "Developer",
			"Co-Owner", "Owner"
		};

		public static string GetAccessLevelName(AccessLevel level)
		{
			return m_AccessLevelNames[(int)level];
		}

		public virtual bool CanPaperdollBeOpenedBy(Mobile from)
		{
			return (Body.IsHuman || Body.IsGhost || IsBodyMod);
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
		}

		public void Internalize()
		{
			Map = Map.Internal;
		}

		public List<Item> Items { get { return m_Items; } }

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

		public virtual int MaxWeight { get { return int.MaxValue; } }

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
			else if (item.Parent is Mobile)
			{
				((Mobile)item.Parent).RemoveItem(item);
			}
			else if (item.Parent is Item)
			{
				((Item)item.Parent).RemoveItem(item);
			}
			else
			{
				item.SendRemovePacket();
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

		public static IWeapon DefaultWeapon { get { return m_DefaultWeapon; } set { m_DefaultWeapon = value; } }

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

		public virtual void Animate(int action, int frameCount, int repeatCount, bool forward, bool repeat, int delay)
		{
			Map map = m_Map;

			if (map != null)
			{
				ProcessDelta();

				Packet p = null;
				//Packet pNew = null;

				var eable = map.GetClientsInRange(m_Location);

				foreach (NetState state in eable)
				{
					if (state.Mobile.CanSee(this))
					{
						state.Mobile.ProcessDelta();

						//if ( state.StygianAbyss ) {
						//if( pNew == null )
						//pNew = Packet.Acquire( new NewMobileAnimation( this, action, frameCount, delay ) );

						//state.Send( pNew );
						//} else {
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
						//}
					}
				}

				Packet.Release(p);
				//Packet.Release( pNew );

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

				foreach (NetState state in eable)
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
		public Skills Skills { get { return m_Skills; } set { } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IgnoreMobiles
        {
            get { return m_IgnoreMobiles; }
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
            get { return m_IsStealthing; }
            set { m_IsStealthing = value; }
        }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
		public AccessLevel AccessLevel
		{
			get { return m_AccessLevel; }
			set
			{
				AccessLevel oldValue = m_AccessLevel;

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
			get { return m_Fame; }
			set
			{
				int oldValue = m_Fame;

				if (oldValue != value)
				{
					m_Fame = value;

					if (ShowFameTitle && (m_Player || m_Body.IsHuman) && (oldValue >= 10000) != (value >= 10000))
					{
						InvalidateProperties();
					}

					OnFameChange(oldValue);
				}
			}
		}

		public virtual void OnFameChange(int oldValue)
		{ }

		[CommandProperty(AccessLevel.Decorator)]
		public int Karma
		{
			get { return m_Karma; }
			set
			{
				int old = m_Karma;

				if (old != value)
				{
					m_Karma = value;
					OnKarmaChange(old);
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
			get { return m_Blessed; }
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

				foreach (NetState state in eable)
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
			NetState ns = m_NetState;

			if (m_Map != null && ns != null)
			{
				var eable = m_Map.GetObjectsInRange(m_Location, Core.GlobalMaxUpdateRange);

				foreach (IEntity o in eable)
				{
					if (o is Mobile)
					{
						Mobile m = (Mobile)o;

						if (m != this && Utility.InUpdateRange(m_Location, m.m_Location))
						{
							ns.Send(m.RemovePacket);
						}
					}
					else if (o is Item)
					{
						Item item = (Item)o;

						if (InRange(item.Location, item.GetUpdateRange(this)))
						{
							ns.Send(item.RemovePacket);
						}
					}
				}

				eable.Free();
			}
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
			else if (throwOnOffline)
			{
				throw new MobileNotConnectedException(this, "Packet could not be sent.");
			}
			else
			{
				return false;
			}
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
			else if (throwOnOffline)
			{
				throw new MobileNotConnectedException(this, "Hue picker could not be sent.");
			}
			else
			{
				return false;
			}
		}

		public Gump FindGump(Type type)
		{
			NetState ns = m_NetState;

			if (ns != null)
			{
				foreach (Gump gump in ns.Gumps)
				{
					if (type.IsAssignableFrom(gump.GetType()))
					{
						return gump;
					}
				}
			}

			return null;
		}

		public bool CloseGump(Type type)
		{
			if (m_NetState != null)
			{
				Gump gump = FindGump(type);

				if (gump != null)
				{
					m_NetState.Send(new CloseGump(gump.TypeID, 0));

					m_NetState.RemoveGump(gump);

					gump.OnServerClose(m_NetState);
				}

				return true;
			}
			else
			{
				return false;
			}
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
			NetState ns = m_NetState;

			if (ns != null)
			{
				var gumps = new List<Gump>(ns.Gumps);

				ns.ClearGumps();

				foreach (Gump gump in gumps)
				{
					ns.Send(new CloseGump(gump.TypeID, 0));

					gump.OnServerClose(ns);
				}

				return true;
			}
			else
			{
				return false;
			}
		}

		[Obsolete("Use CloseAllGumps() instead.", false)]
		public bool CloseAllGumps(bool throwOnOffline)
		{
			return CloseAllGumps();
		}

		public bool HasGump(Type type)
		{
			return (FindGump(type) != null);
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
			else if (throwOnOffline)
			{
				throw new MobileNotConnectedException(this, "Gump could not be sent.");
			}
			else
			{
				return false;
			}
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
			else if (throwOnOffline)
			{
				throw new MobileNotConnectedException(this, "Menu could not be sent.");
			}
			else
			{
				return false;
			}
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
				if (Core.ML)
				{
					SendLocalizedMessage(500168); // You can not say anything, you have been muted.
				}
				else
				{
					SendMessage("You can not say anything, you have been squelched."); //Cliloc ITSELF changed during ML.
				}

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
			NetState ns = m_NetState;

			if (m_Map != null && ns != null)
			{
				var eable = m_Map.GetObjectsInRange(m_Location, Core.GlobalMaxUpdateRange);

				foreach (IEntity o in eable)
				{
					if (o is Item)
					{
						Item item = (Item)o;

						if (CanSee(item) && InRange(item.Location, item.GetUpdateRange(this)))
						{
							item.SendInfoTo(ns);
						}
					}
					else if (o is Mobile)
					{
						Mobile m = (Mobile)o;

						if (CanSee(m) && Utility.InUpdateRange(m_Location, m.m_Location))
						{
							ns.Send(MobileIncoming.Create(ns, this, m));

							if (ns.StygianAbyss)
							{
								if (m.Poisoned)
								{
									ns.Send(new HealthbarPoison(m));
								}

								if (m.Blessed || m.YellowHealthbar)
								{
									ns.Send(new HealthbarYellow(m));
								}
							}

							if (m.IsDeadBondedPet)
							{
								ns.Send(new BondedStatus(0, m.m_Serial, 1));
							}

							if (ObjectPropertyList.Enabled)
							{
								ns.Send(m.OPLPacket);

								//foreach ( Item item in m.m_Items )
								//	ns.Send( item.OPLPacket );
							}
						}
					}
				}

				eable.Free();
			}
		}

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Decorator)]
		public Map Map
		{
			get { return m_Map; }
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

					Map oldMap = m_Map;

					if (m_Map != null)
					{
						m_Map.OnLeave(this);

						ClearScreen();
						SendRemovePacket();
					}

					for (int i = 0; i < m_Items.Count; ++i)
					{
						m_Items[i].Map = value;
					}

					m_Map = value;

					UpdateRegion();

					if (m_Map != null)
					{
						m_Map.OnEnter(this);
					}

					NetState ns = m_NetState;

					if (ns != null && m_Map != null)
					{
						ns.Sequence = 0;
						ns.Send(new MapChange(this));
						ns.Send(new MapPatches());
						ns.Send(SeasonChange.Instantiate(GetSeason(), true));

						if (ns.StygianAbyss)
						{
							ns.Send(new MobileUpdate(this));
						}
						else
						{
							ns.Send(new MobileUpdateOld(this));
						}

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

						if (ns.StygianAbyss)
						{
							ns.Send(new MobileUpdate(this));
							CheckLightLevels(true);
							ns.Send(new MobileUpdate(this));
						}
						else
						{
							ns.Send(new MobileUpdateOld(this));
							CheckLightLevels(true);
							ns.Send(new MobileUpdateOld(this));
						}
					}

					SendEverything();
					SendIncomingPacket();

					if (ns != null)
					{
						ns.Sequence = 0;
						ClearFastwalkStack();

						ns.Send(MobileIncoming.Create(ns, this, this));

						if (ns.StygianAbyss)
						{
							ns.Send(SupportedFeatures.Instantiate(ns));
							ns.Send(new MobileUpdate(this));
							ns.Send(new MobileAttributes(this));
						}
						else
						{
							ns.Send(SupportedFeatures.Instantiate(ns));
							ns.Send(new MobileUpdateOld(this));
							ns.Send(new MobileAttributes(this));
						}
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

			Region newRegion = Region.Find(m_Location, m_Map);
			Region oldRegion = m_Region;
			
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

			int n = Notoriety.Compute(this, target);

			return (n == Notoriety.Criminal || n == Notoriety.Murderer);
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
		public virtual bool CanBeHarmful(Mobile target)
		{
			return CanBeHarmful(target, true);
		}

		public virtual bool CanBeHarmful(Mobile target, bool message)
		{
			return CanBeHarmful(target, message, false);
		}

		public virtual bool CanBeHarmful(Mobile target, bool message, bool ignoreOurBlessedness)
		{
			if (target == null)
			{
				return false;
			}

			if (m_Deleted || (!ignoreOurBlessedness && m_Blessed) || target.m_Deleted || target.m_Blessed || !Alive ||
				IsDeadBondedPet || !target.Alive || target.IsDeadBondedPet)
			{
				if (message)
				{
					SendLocalizedMessage(1001018); // You can not perform negative acts on your target.
				}

				return false;
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

		public virtual bool IsHarmfulCriminal(Mobile target)
		{
			if (this == target)
			{
				return false;
			}

			return (Notoriety.Compute(this, target) == Notoriety.Innocent);
		}

		/// <summary>
		///     Overridable. Event invoked when the Mobile <see cref="DoHarmful">does a harmful action</see>.
		/// </summary>
		public virtual void OnHarmfulAction(Mobile target, bool isCriminal)
		{
			if (isCriminal)
			{
				CriminalAction(false);
			}
		}

		public virtual void DoHarmful(Mobile target)
		{
			DoHarmful(target, false);
		}

		public virtual void DoHarmful(Mobile target, bool indirect)
		{
			if (target == null || m_Deleted)
			{
				return;
			}

			bool isCriminal = IsHarmfulCriminal(target);

			OnHarmfulAction(target, isCriminal);
			target.AggressiveAction(this, isCriminal);

			Region.OnDidHarmful(this, target);
			target.Region.OnGotHarmful(this, target);

			if (!indirect)
			{
				Combatant = target;
			}

			if (m_ExpireCombatant == null)
			{
				m_ExpireCombatant = new ExpireCombatantTimer(this);
			}
			else
			{
				m_ExpireCombatant.Stop();
			}

			m_ExpireCombatant.Start();
		}

		public virtual bool HarmfulCheck(Mobile target)
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
		public List<StatMod> StatMods { get { return m_StatMods; } }

		public bool RemoveStatMod(string name)
		{
			for (int i = 0; i < m_StatMods.Count; ++i)
			{
				StatMod check = m_StatMods[i];

				if (check.Name == name)
				{
					m_StatMods.RemoveAt(i);
					CheckStatTimers();
					Delta(MobileDelta.Stat | GetStatDelta(check.Type));
					return true;
				}
			}

			return false;
		}

		public StatMod GetStatMod(string name)
		{
			for (int i = 0; i < m_StatMods.Count; ++i)
			{
				StatMod check = m_StatMods[i];

				if (check.Name == name)
				{
					return check;
				}
			}

			return null;
		}

		public void AddStatMod(StatMod mod)
		{
			for (int i = 0; i < m_StatMods.Count; ++i)
			{
				StatMod check = m_StatMods[i];

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
			int offset = 0;

			for (int i = 0; i < m_StatMods.Count; ++i)
			{
				StatMod mod = m_StatMods[i];

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
			get { return m_Str; }
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
					int oldValue = m_Str;

					m_Str = value;
					Delta(MobileDelta.Stat | MobileDelta.Hits);

					if (Hits < HitsMax)
					{
						if (m_HitsTimer == null)
						{
							m_HitsTimer = new HitsTimer(this);
						}

						m_HitsTimer.Start();
					}
					else if (Hits > HitsMax)
					{
						Hits = HitsMax;
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
				int value = m_Str + GetStatOffset(StatType.Str);

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
			get { return m_Dex; }
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
					int oldValue = m_Dex;

					m_Dex = value;
					Delta(MobileDelta.Stat | MobileDelta.Stam);

					if (Stam < StamMax)
					{
						if (m_StamTimer == null)
						{
							m_StamTimer = new StamTimer(this);
						}

						m_StamTimer.Start();
					}
					else if (Stam > StamMax)
					{
						Stam = StamMax;
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
				int value = m_Dex + GetStatOffset(StatType.Dex);

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
			get { return m_Int; }
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
					int oldValue = m_Int;

					m_Int = value;
					Delta(MobileDelta.Stat | MobileDelta.Mana);

					if (Mana < ManaMax)
					{
						if (m_ManaTimer == null)
						{
							m_ManaTimer = new ManaTimer(this);
						}

						m_ManaTimer.Start();
					}
					else if (Mana > ManaMax)
					{
						Mana = ManaMax;
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
				int value = m_Int + GetStatOffset(StatType.Int);

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
			get { return m_Hits; }
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

					if (m_HitsTimer != null)
					{
						m_HitsTimer.Stop();
					}

					for (int i = 0; i < m_Aggressors.Count; i++) //reset reports on full HP
					{
						m_Aggressors[i].CanReportMurder = false;
					}

					if (m_DamageEntries.Count > 0)
					{
						m_DamageEntries.Clear(); // reset damage entries on full HP
					}
				}

				if (value < HitsMax)
				{
					if (CanRegenHits)
					{
						if (m_HitsTimer == null)
						{
							m_HitsTimer = new HitsTimer(this);
						}

						m_HitsTimer.Start();
					}
					else if (m_HitsTimer != null)
					{
						m_HitsTimer.Stop();
					}
				}

				if (m_Hits != value)
				{
					int oldValue = m_Hits;
					m_Hits = value;
					Delta(MobileDelta.Hits);
					OnHitsChange(oldValue);
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
		public virtual int HitsMax { get { return 50 + (Str / 2); } }

		/// <summary>
		///     Gets or sets the current stamina of the Mobile. This value ranges from 0 to <see cref="StamMax" />, inclusive.
		/// </summary>
		[CommandProperty(AccessLevel.GameMaster)]
		public int Stam
		{
			get { return m_Stam; }
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

					if (m_StamTimer != null)
					{
						m_StamTimer.Stop();
					}
				}

				if (value < StamMax)
				{
					if (CanRegenStam)
					{
						if (m_StamTimer == null)
						{
							m_StamTimer = new StamTimer(this);
						}

						m_StamTimer.Start();
					}
					else if (m_StamTimer != null)
					{
						m_StamTimer.Stop();
					}
				}

				if (m_Stam != value)
				{
					int oldValue = m_Stam;
					m_Stam = value;
					Delta(MobileDelta.Stam);
					OnStamChange(oldValue);
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
		public virtual int StamMax { get { return Dex; } }

		/// <summary>
		///     Gets or sets the current stamina of the Mobile. This value ranges from 0 to <see cref="ManaMax" />, inclusive.
		/// </summary>
		[CommandProperty(AccessLevel.GameMaster)]
		public int Mana
		{
			get { return m_Mana; }
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

					if (m_ManaTimer != null)
					{
						m_ManaTimer.Stop();
					}

					if (Meditating)
					{
						Meditating = false;
						SendLocalizedMessage(501846); // You are at peace.
					}
				}

				if (value < ManaMax)
				{
					if (CanRegenMana)
					{
						if (m_ManaTimer == null)
						{
							m_ManaTimer = new ManaTimer(this);
						}

						m_ManaTimer.Start();
					}
					else if (m_ManaTimer != null)
					{
						m_ManaTimer.Stop();
					}
				}

				if (m_Mana != value)
				{
					int oldValue = m_Mana;
					m_Mana = value;
					Delta(MobileDelta.Mana);
					OnManaChange(oldValue);
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
		public virtual int ManaMax { get { return Int; } }
		#endregion

		public virtual int Luck { get { return 0; } }

		public virtual int HuedItemID { get { return (m_Female ? 0x2107 : 0x2106); } }

		private int m_HueMod = -1;

		[Hue, CommandProperty(AccessLevel.Decorator)]
		public int HueMod
		{
			get { return m_HueMod; }
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
				int oldHue = m_Hue;

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
			get { return m_Direction; }
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
			int flags = 0x0;

			if (m_Paralyzed || m_Frozen)
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
			int flags = 0x0;

			if (m_Paralyzed || m_Frozen || _Sleep)
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
			get { return m_Female; }
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
			get { return m_Flying; }
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
			get { return m_Warmode; }
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
			get { return m_Hidden; }
			set
			{
				if (m_Hidden != value)
				{
					m_Hidden = value;
					//Delta( MobileDelta.Flags );

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

				foreach (NetState state in eable)
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

						if (ObjectPropertyList.Enabled)
						{
							state.Send(OPLPacket);

							//foreach ( Item item in m_Items )
							//	state.Send( item.OPLPacket );
						}
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

		[CommandProperty(AccessLevel.GameMaster, AccessLevel.Owner)]
		public NetState NetState
		{
			get
			{
				if (m_NetState != null && m_NetState.Socket == null)
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

					BankBox box = FindBankNoCreate();

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

						if (m_LogoutTimer == null)
						{
							m_LogoutTimer = new LogoutTimer(this);
						}
						else
						{
							m_LogoutTimer.Stop();
						}

						m_LogoutTimer.Delay = GetLogoutDelay();
						m_LogoutTimer.Start();
					}
					else
					{
						OnConnected();
						EventSink.InvokeConnected(new ConnectedEventArgs(this));

						// Connected, stop the logout timer and if needed, move to the world

						if (m_LogoutTimer != null)
						{
							m_LogoutTimer.Stop();
						}

						m_LogoutTimer = null;

						if (m_Map == Map.Internal && m_LogoutMap != null)
						{
							Map = m_LogoutMap;
							Location = m_LogoutLocation;
						}
					}

					for (int i = m_Items.Count - 1; i >= 0; --i)
					{
						if (i >= m_Items.Count)
						{
							continue;
						}

						Item item = m_Items[i];

						if (item is SecureTradeContainer)
						{
							for (int j = item.Items.Count - 1; j >= 0; --j)
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
			if (o is Item)
			{
				return CanSee((Item)o);
			}
			else if (o is Mobile)
			{
				return CanSee((Mobile)o);
			}
			else
			{
				return true;
			}
		}

		public virtual bool CanSee(Item item)
		{
			if (m_Map == Map.Internal)
			{
				return false;
			}
			else if (item.Map == Map.Internal)
			{
				return false;
			}

			if (item.Parent != null)
			{
				if (item.Parent is Item)
				{
					Item parent = item.Parent as Item;

					if (!(CanSee(parent) && parent.IsChildVisibleTo(this, item)))
					{
						return false;
					}
				}
				else if (item.Parent is Mobile)
				{
					if (!CanSee((Mobile)item.Parent))
					{
						return false;
					}
				}
			}

			if (item is BankBox)
			{
				BankBox box = item as BankBox;

				if (box != null && IsPlayer() && (box.Owner != this || !box.Opened))
				{
					return false;
				}
			}
			else if (item is SecureTradeContainer)
			{
				SecureTrade trade = ((SecureTradeContainer)item).Trade;

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
					((m.Alive || (Core.SE && Skills.SpiritSpeak.Value >= 100.0)) || !Alive || IsStaff() || m.Warmode));
		}

		public virtual bool CanBeRenamedBy(Mobile from)
		{
			return (from.AccessLevel >= AccessLevel.Decorator && from.m_AccessLevel > m_AccessLevel);
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public string Language
		{
			get { return m_Language; }
			set
			{
				if (m_Language != value)
				{
					m_Language = value;
				}
			}
		}

		[CommandProperty(AccessLevel.Decorator)]
		public int SpeechHue { get { return m_SpeechHue; } set { m_SpeechHue = value; } }

		[CommandProperty(AccessLevel.Decorator)]
		public int EmoteHue { get { return m_EmoteHue; } set { m_EmoteHue = value; } }

		[CommandProperty(AccessLevel.Decorator)]
		public int WhisperHue { get { return m_WhisperHue; } set { m_WhisperHue = value; } }

		[CommandProperty(AccessLevel.Decorator)]
		public int YellHue { get { return m_YellHue; } set { m_YellHue = value; } }

		[CommandProperty(AccessLevel.Decorator)]
		public string GuildTitle
		{
			get { return m_GuildTitle; }
			set
			{
				string old = m_GuildTitle;

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
		public bool DisplayGuildTitle
		{
			get { return m_DisplayGuildTitle; }
			set
			{
				m_DisplayGuildTitle = value;
				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.Decorator)]
		public Mobile GuildFealty { get { return m_GuildFealty; } set { m_GuildFealty = value; } }

		private string m_NameMod;

		[CommandProperty(AccessLevel.Decorator)]
		public string NameMod
		{
			get { return m_NameMod; }
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
			get { return m_YellowHealthbar; }
			set
			{
				m_YellowHealthbar = value;
				Delta(MobileDelta.HealthbarYellow);
			}
		}

		[CommandProperty(AccessLevel.Decorator)]
		public string RawName { get { return m_Name; } set { Name = value; } }

		[CommandProperty(AccessLevel.Decorator)]
		public virtual string TitleName
		{
			get
			{
				return m_Name;
			}
		}

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
					string oldName = m_Name;
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
		public DateTime LastStrGain { get { return m_LastStrGain; } set { m_LastStrGain = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime LastIntGain { get { return m_LastIntGain; } set { m_LastIntGain = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime LastDexGain { get { return m_LastDexGain; } set { m_LastDexGain = value; } }

		public DateTime LastStatGain
		{
			get
			{
				DateTime d = m_LastStrGain;

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
			get { return m_Guild; }
			set
			{
				BaseGuild old = m_Guild;

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
		public Timer PoisonTimer { get { return m_PoisonTimer; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public Poison Poison
		{
			get { return m_Poison; }
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
				#region Mondain's Legacy
				LocalOverheadMessage(MessageType.Regular, 0x21, 1042857 + (poison.RealLevel * 2));
				NonlocalOverheadMessage(MessageType.Regular, 0x21, 1042858 + (poison.RealLevel * 2), Name);
				#endregion
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
			return (m_Poison != null && m_Poison.RealLevel >= poison.RealLevel);
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

			Poison oldPoison = m_Poison;
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
				Poison oldPoison = m_Poison;
				Poison = null;

				OnCured(from, oldPoison);

				return true;
			}

			OnFailedCure(from);

			return false;
		}
		#endregion

		private ISpawner m_Spawner;

		public ISpawner Spawner { get { return m_Spawner; } set { m_Spawner = value; } }

		public Region WalkRegion { get; set; }

		public virtual void OnBeforeSpawn(Point3D location, Map m)
		{ }

		public virtual void OnAfterSpawn()
		{ }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Poisoned { get { return (m_Poison != null); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool IsBodyMod { get { return (m_BodyMod.BodyID != 0); } }

		[CommandProperty(AccessLevel.Decorator)]
		public Body BodyMod
		{
			get { return m_BodyMod; }
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

		private static readonly int[] m_InvalidBodies = new int[]
		{
			//32,		// Dunno why is blocked
			//95,		// Used for Turkey
			//156,		// Dunno why is blocked
			//197,		// ML Dragon
			//198,		// ML Dragon
		};

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
			int delta = -1;

			for (int i = 0; delta < 0 && i < m_InvalidBodies.Length; ++i)
			{
				delta = (m_InvalidBodies[i] - body);
			}

			if (delta != 0)
			{
				return body;
			}

			return 0;
		}

		[Body, CommandProperty(AccessLevel.Decorator)]
		public int BodyValue { get { return Body.BodyID; } set { Body = value; } }

		[CommandProperty(AccessLevel.Counselor)]
		public Serial Serial { get { return m_Serial; } }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Decorator)]
		public Point3D Location { get { return m_Location; } set { SetLocation(value, true); } }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public Point3D LogoutLocation { get { return m_LogoutLocation; } set { m_LogoutLocation = value; } }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public Map LogoutMap { get { return m_LogoutMap; } set { m_LogoutMap = value; } }

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
					else
					{
						return Map.DefaultRegion;
					}
				}
				else
				{
					return m_Region;
				}
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
			if (!ObjectPropertyList.Enabled)
			{
				return;
			}

			if (m_Map != null && m_Map != Map.Internal && !World.Loading)
			{
				ObjectPropertyList oldList = m_PropertyList;
				Packet.Release(ref m_PropertyList);
				ObjectPropertyList newList = PropertyList;

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
			get { return m_SolidHueOverride; }
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

			BankBox box = FindBankNoCreate();

			if (box != null && box.Opened)
			{
				box.Close();
			}

			Point3D oldLocation = m_Location;
			Map oldMap = m_Map;

			Region oldRegion = m_Region;

			if (oldMap != null)
			{
				oldMap.OnLeave(this);

				ClearScreen();
				SendRemovePacket();
			}

			for (int i = 0; i < m_Items.Count; ++i)
			{
				m_Items[i].Map = map;
			}

			m_Map = map;

			m_Location = newLocation;

			NetState ns = m_NetState;

			if (m_Map != null)
			{
				m_Map.OnEnter(this);

				UpdateRegion();

				if (ns != null && m_Map != null)
				{
					ns.Sequence = 0;
					ns.Send(new MapChange(this));
					ns.Send(new MapPatches());
					ns.Send(SeasonChange.Instantiate(GetSeason(), true));

					if (ns.StygianAbyss)
					{
						ns.Send(new MobileUpdate(this));
					}
					else
					{
						ns.Send(new MobileUpdateOld(this));
					}

					ClearFastwalkStack();
				}
			}
			else
			{
				UpdateRegion();
			}

			if (ns != null)
			{
				if (m_Map != null)
				{
					Send(new ServerChange(this, m_Map));
				}

				ns.Sequence = 0;
				ClearFastwalkStack();

				ns.Send(MobileIncoming.Create(ns, this, this));

				if (ns.StygianAbyss)
				{
					ns.Send(new MobileUpdate(this));
					CheckLightLevels(true);
					ns.Send(new MobileUpdate(this));
				}
				else
				{
					ns.Send(new MobileUpdateOld(this));
					CheckLightLevels(true);
					ns.Send(new MobileUpdateOld(this));
				}
			}

			SendEverything();
			SendIncomingPacket();

			if (ns != null)
			{
				ns.Sequence = 0;
				ClearFastwalkStack();

				ns.Send(MobileIncoming.Create(ns, this, this));

				if (ns.StygianAbyss)
				{
					ns.Send(SupportedFeatures.Instantiate(ns));
					ns.Send(new MobileUpdate(this));
					ns.Send(new MobileAttributes(this));
				}
				else
				{
					ns.Send(SupportedFeatures.Instantiate(ns));
					ns.Send(new MobileUpdateOld(this));
					ns.Send(new MobileAttributes(this));
				}
			}

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

			Point3D oldLocation = m_Location;

			if (oldLocation != newLocation)
			{
				m_Location = newLocation;
				UpdateRegion();

				BankBox box = FindBankNoCreate();

				if (box != null && box.Opened)
				{
					box.Close();
				}

				if (m_NetState != null)
				{
					m_NetState.ValidateAllTrades();
				}

				if (m_Map != null)
				{
					m_Map.OnMove(oldLocation, this);
				}

				if (isTeleport && m_NetState != null && (!m_NetState.HighSeas || !m_NoMoveHS))
				{
					m_NetState.Sequence = 0;

					if (m_NetState.StygianAbyss)
					{
						m_NetState.Send(new MobileUpdate(this));
					}
					else
					{
						m_NetState.Send(new MobileUpdateOld(this));
					}

					ClearFastwalkStack();
				}

				Map map = m_Map;

				if (map != null)
				{
					// First, send a remove message to everyone who can no longer see us. (inOldRange && !inNewRange)

					var eable = map.GetClientsInRange(oldLocation);

					foreach (NetState ns in eable)
					{
						if (ns != m_NetState && !Utility.InUpdateRange(newLocation, ns.Mobile.Location))
						{
							ns.Send(RemovePacket);
						}
					}

					eable.Free();

					NetState ourState = m_NetState;

					// Check to see if we are attached to a client
					if (ourState != null)
					{
						var eeable = map.GetObjectsInRange(newLocation, Core.GlobalMaxUpdateRange);

						// We are attached to a client, so it's a bit more complex. We need to send new items and people to ourself, and ourself to other clients

						foreach (IEntity o in eeable)
						{
							if (o is Item)
							{
								Item item = (Item)o;

								int range = item.GetUpdateRange(this);
								Point3D loc = item.Location;

								if (!Utility.InRange(oldLocation, loc, range) && Utility.InRange(newLocation, loc, range) && CanSee(item))
								{
									item.SendInfoTo(ourState);
								}
							}
							else if (o != this && o is Mobile)
							{
								Mobile m = (Mobile)o;

								if (!Utility.InUpdateRange(newLocation, m.m_Location))
								{
									continue;
								}

								bool inOldRange = Utility.InUpdateRange(oldLocation, m.m_Location);

								if (m.m_NetState != null && ((isTeleport && (!m.m_NetState.HighSeas || !m_NoMoveHS)) || !inOldRange) &&
									m.CanSee(this))
								{
									m.m_NetState.Send(MobileIncoming.Create(m.m_NetState, m, this));

									if (m.m_NetState.StygianAbyss)
									{
										if (m_Poison != null)
										{
											m.m_NetState.Send(new HealthbarPoison(this));
										}

										if (m_Blessed || m_YellowHealthbar)
										{
											m.m_NetState.Send(new HealthbarYellow(this));
										}
									}

									if (IsDeadBondedPet)
									{
										m.m_NetState.Send(new BondedStatus(0, m_Serial, 1));
									}

									if (ObjectPropertyList.Enabled)
									{
										m.m_NetState.Send(OPLPacket);

										//foreach ( Item item in m_Items )
										//	m.m_NetState.Send( item.OPLPacket );
									}
								}

								if (!inOldRange && CanSee(m))
								{
									ourState.Send(MobileIncoming.Create(ourState, this, m));

									if (ourState.StygianAbyss)
									{
										if (m.Poisoned)
										{
											ourState.Send(new HealthbarPoison(m));
										}

										if (m.Blessed || m.YellowHealthbar)
										{
											ourState.Send(new HealthbarYellow(m));
										}
									}

									if (m.IsDeadBondedPet)
									{
										ourState.Send(new BondedStatus(0, m.m_Serial, 1));
									}

									if (ObjectPropertyList.Enabled)
									{
										ourState.Send(m.OPLPacket);

										//foreach ( Item item in m.m_Items )
										//	ourState.Send( item.OPLPacket );
									}
								}
							}
						}

						eeable.Free();
					}
					else
					{
						eable = map.GetClientsInRange(newLocation);

						// We're not attached to a client, so simply send an Incoming
						foreach (NetState ns in eable)
						{
							if (((isTeleport && (!ns.HighSeas || !m_NoMoveHS)) || !Utility.InUpdateRange(oldLocation, ns.Mobile.Location)) &&
								ns.Mobile.CanSee(this))
							{
								ns.Send(MobileIncoming.Create(ns, ns.Mobile, this));

								if (ns.StygianAbyss)
								{
									if (m_Poison != null)
									{
										ns.Send(new HealthbarPoison(this));
									}

									if (m_Blessed || m_YellowHealthbar)
									{
										ns.Send(new HealthbarYellow(this));
									}
								}

								if (IsDeadBondedPet)
								{
									ns.Send(new BondedStatus(0, m_Serial, 1));
								}

								if (ObjectPropertyList.Enabled)
								{
									ns.Send(OPLPacket);

									//foreach ( Item item in m_Items )
									//	ns.Send( item.OPLPacket );
								}
							}
						}

						eable.Free();
					}
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

		#region Hair
		private HairInfo m_Hair;
		private FacialHairInfo m_FacialHair;

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

		//		[CommandProperty( AccessLevel.GameMaster )]
		//		public int HairSerial { get { return HairInfo.FakeSerial( this ); } }

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

		//		[CommandProperty( AccessLevel.GameMaster )]
		//		public int FacialHairSerial { get { return FacialHairInfo.FakeSerial( this ); } }

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
				Item item = m_Weapon as Item;

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

				if (item is IWeapon)
				{
					return (m_Weapon = (IWeapon)item);
				}
				else
				{
					return GetDefaultWeapon();
				}
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

				return (m_Backpack = (FindItemOnLayer(Layer.Backpack) as Container));
			}
		}

		public virtual bool KeepsItemsOnDeath { get { return IsStaff(); } }

		public Item FindItemOnLayer(Layer layer)
		{
			var eq = m_Items;
			int count = eq.Count;

			for (int i = 0; i < count; ++i)
			{
				Item item = eq[i];

				if (!item.Deleted && item.Layer == layer)
				{
					return item;
				}
			}

			return null;
		}

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Decorator)]
		public int X { get { return m_Location.m_X; } set { Location = new Point3D(value, m_Location.m_Y, m_Location.m_Z); } }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Decorator)]
		public int Y { get { return m_Location.m_Y; } set { Location = new Point3D(m_Location.m_X, value, m_Location.m_Z); } }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Decorator)]
		public int Z { get { return m_Location.m_Z; } set { Location = new Point3D(m_Location.m_X, m_Location.m_Y, value); } }

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

				foreach (NetState state in eable)
				{
					if (state.Mobile.CanSee(this))
					{
						state.Send(MobileIncoming.Create(state, state.Mobile, this));

						if (state.StygianAbyss)
						{
							if (m_Poison != null)
							{
								state.Send(new HealthbarPoison(this));
							}

							if (m_Blessed || m_YellowHealthbar)
							{
								state.Send(new HealthbarYellow(this));
							}
						}

						if (IsDeadBondedPet)
						{
							state.Send(new BondedStatus(0, m_Serial, 1));
						}

						if (ObjectPropertyList.Enabled)
						{
							state.Send(OPLPacket);

							//foreach ( Item item in m_Items )
							//	state.Send( item.OPLPacket );
						}
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

			Container pack = Backpack;

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
				Point3D loc = m_Location;
				Map map = m_Map;

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

			NetState ourState = m_NetState;
			NetState theirState = from.m_NetState;

			if (ourState == null || theirState == null)
			{
				return false;
			}

			SecureTradeContainer cont = theirState.FindTradeContainer(this);

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
				Container pack = Backpack;

				if (pack != null)
				{
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
			for (int i = 0; i < m_Items.Count; ++i)
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
			return (mob == this || (mob.AccessLevel >= AccessLevel.Decorator && mob.AccessLevel > AccessLevel));
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

			Type ourType = GetType();
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

			Type ourType = GetType();
			m_TypeRef = World.m_MobileTypes.IndexOf(ourType);

			if (m_TypeRef == -1)
			{
				World.m_MobileTypes.Add(ourType);
				m_TypeRef = World.m_MobileTypes.Count - 1;
			}
		}

		public void DefaultMobileInit()
		{
            m_StatCap = Config.Get("PlayerCaps.TotalStatCap", 225); ;
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

		private static readonly Queue<Mobile> m_DeltaQueue = new Queue<Mobile>();
		private static readonly Queue<Mobile> m_DeltaQueueR = new Queue<Mobile>();

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

				if (_processing)
				{
					lock (m_DeltaQueueR)
					{
						m_DeltaQueueR.Enqueue(this);

						try
						{
							using (StreamWriter op = new StreamWriter("delta-recursion.log", true))
							{
								op.WriteLine("# {0}", DateTime.UtcNow);
								op.WriteLine(new StackTrace());
								op.WriteLine();
							}
						}
						catch
						{ }
					}
				}
				else
				{
					m_DeltaQueue.Enqueue(this);
				}
			}

			Core.Set();
		}

		private bool m_NoMoveHS;

		public bool NoMoveHS { get { return m_NoMoveHS; } set { m_NoMoveHS = value; } }

		#region GetDirectionTo[..]
		public Direction GetDirectionTo(int x, int y)
		{
			int dx = m_Location.m_X - x;
			int dy = m_Location.m_Y - y;

			int rx = (dx - dy) * 44;
			int ry = (dx + dy) * 44;

			int ax = Math.Abs(rx);
			int ay = Math.Abs(ry);

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
			Mobile m = this;
			MobileDelta delta;

			delta = m.m_DeltaFlags;

			if (delta == MobileDelta.None)
			{
				return;
			}

			MobileDelta attrs = delta & MobileDelta.Attributes;

			m.m_DeltaFlags = MobileDelta.None;
			m.m_InDeltaQueue = false;

			bool sendHits = false, sendStam = false, sendMana = false, sendAll = false, sendAny = false;
			bool sendIncoming = false, sendNonlocalIncoming = false;
			bool sendUpdate = false, sendRemove = false;
			bool sendPublicStats = false, sendPrivateStats = false;
			bool sendMoving = false, sendNonlocalMoving = false;
			bool sendOPLUpdate = ObjectPropertyList.Enabled && (delta & MobileDelta.Properties) != 0;

			bool sendHair = false, sendFacialHair = false, removeHair = false, removeFacialHair = false;

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
					sendHits = ((attrs & MobileDelta.Hits) != 0);
					sendStam = ((attrs & MobileDelta.Stam) != 0);
					sendMana = ((attrs & MobileDelta.Mana) != 0);
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

			/*if ( (delta & MobileDelta.Hue) != 0 )
			{
			sendNonlocalIncoming = true;
			sendUpdate = true;
			}
			else if ( (delta & (MobileDelta.Direction | MobileDelta.Body)) != 0 )
			{
			sendNonlocalMoving = true;
			sendUpdate = true;
			}
			else*/
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

			var cache = new Packet[2][] {new Packet[8], new Packet[8]};

			NetState ourState = m.m_NetState;

			if (ourState != null)
			{
				if (sendUpdate)
				{
					ourState.Sequence = 0;

					if (ourState.StygianAbyss)
					{
						ourState.Send(new MobileUpdate(m));
					}
					else
					{
						ourState.Send(new MobileUpdateOld(m));
					}

					ClearFastwalkStack();
				}

				if (sendIncoming)
				{
					ourState.Send(MobileIncoming.Create(ourState, m, m));
				}

				if (ourState.StygianAbyss)
				{
					if (sendMoving)
					{
						int noto = Notoriety.Compute(m, m);
						ourState.Send(cache[0][noto] = Packet.Acquire(new MobileMoving(m, noto)));
					}

					if (sendHealthbarPoison)
					{
						ourState.Send(new HealthbarPoison(m));
					}

					if (sendHealthbarYellow)
					{
						ourState.Send(new HealthbarYellow(m));
					}
				}
				else
				{
					if (sendMoving || sendHealthbarPoison || sendHealthbarYellow)
					{
						int noto = Notoriety.Compute(m, m);
						ourState.Send(cache[1][noto] = Packet.Acquire(new MobileMovingOld(m, noto)));
					}
				}

				if (sendPublicStats || sendPrivateStats)
				{
					ourState.Send(new MobileStatusExtended(m, m_NetState));
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
					IParty ip = m_Party as IParty;

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
				 sendFacialHair || sendHealthbarPoison || sendHealthbarYellow))
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

				var eable = m.Map.GetClientsInRange(m.m_Location);

				foreach (NetState state in eable)
				{
					beholder = state.Mobile;

					if (beholder != m && beholder.CanSee(m))
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

						if (state.StygianAbyss)
						{
							if (sendMoving)
							{
								int noto = Notoriety.Compute(beholder, m);

								Packet p = cache[0][noto];

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
								}

								state.Send(hbpPacket);
							}

							if (sendHealthbarYellow)
							{
								if (hbyPacket == null)
								{
									hbyPacket = Packet.Acquire(new HealthbarYellow(m));
								}

								state.Send(hbyPacket);
							}
						}
						else
						{
							if (sendMoving || sendHealthbarPoison || sendHealthbarYellow)
							{
								int noto = Notoriety.Compute(beholder, m);

								Packet p = cache[1][noto];

								if (p == null)
								{
									cache[1][noto] = p = Packet.Acquire(new MobileMovingOld(m, noto));
								}

								state.Send(p);
							}
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

				eable.Free();
			}

			if (sendMoving || sendNonlocalMoving || sendHealthbarPoison || sendHealthbarYellow)
			{
				for (int i = 0; i < cache.Length; ++i)
				{
					for (int j = 0; j < cache[i].Length; ++j)
					{
						Packet.Release(ref cache[i][j]);
					}
				}
			}
		}

		private static bool _processing;

		public static void ProcessDeltaQueue()
		{
			_processing = true;

			if (m_DeltaQueue.Count >= 512)
			{
				Parallel.ForEach(m_DeltaQueue, m => m.ProcessDelta());
				m_DeltaQueue.Clear();
			}
			else
			{
				while (m_DeltaQueue.Count > 0)
				{
					m_DeltaQueue.Dequeue().ProcessDelta();
				}
			}

			_processing = false;

			while (m_DeltaQueueR.Count > 0)
			{
				m_DeltaQueueR.Dequeue().ProcessDelta();
			}
		}

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Decorator)]
		public int Kills
		{
			get { return m_Kills; }
			set
			{
				int oldValue = m_Kills;

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
			get { return m_ShortTermMurders; }
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
		public bool Criminal
		{
			get { return m_Criminal; }
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
					if (m_ExpireCriminal == null)
					{
						m_ExpireCriminal = new ExpireCriminalTimer(this);
					}
					else
					{
						m_ExpireCriminal.Stop();
					}

					m_ExpireCriminal.Start();
				}
				else if (m_ExpireCriminal != null)
				{
					m_ExpireCriminal.Stop();
					m_ExpireCriminal = null;
				}
			}
		}

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
			else
			{
				return true;
			}
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

				foreach (NetState state in eable)
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

				foreach (NetState state in eable)
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
				Packet p =
					Packet.Acquire(new MessageLocalizedAffix(m_Serial, Body, type, hue, 3, number, Name, affixType, affix, args));

				var eable = m_Map.GetClientsInRange(m_Location);

				foreach (NetState state in eable)
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
			NetState ns = m_NetState;

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
			NetState ns = m_NetState;

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

				foreach (NetState state in eable)
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

				foreach (NetState state in eable)
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
			NetState ns = m_NetState;

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
			if (hue == 0x3B2 && (args == null || args.Length == 0))
			{
				NetState ns = m_NetState;

				if (ns != null)
				{
					ns.Send(MessageLocalized.InstantiateGeneric(number));
				}
			}
			else
			{
				NetState ns = m_NetState;

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
			NetState ns = m_NetState;

			if (ns != null)
			{
				ns.Send(
					new MessageLocalizedAffix(
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
			NetState ns = m_NetState;

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
			NetState ns = m_NetState;

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

		public static bool DisableDismountInWarmode { get { return m_DisableDismountInWarmode; } set { m_DisableDismountInWarmode = value; } }

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
				IMount mount = Mount;

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

		public static int BodyWeight { get { return m_BodyWeight; } set { m_BodyWeight = value; } }

		/// <summary>
		///     Overridable. Event invoked when <paramref name="from" /> wants to see this Mobile's stats.
		/// </summary>
		/// <param name="from"></param>
		public virtual void OnStatsQuery(Mobile from)
		{
			if (from.Map == Map && Utility.InUpdateRange(this, from) && from.CanSee(this))
			{
				from.Send(new MobileStatus(from, this, m_NetState));
			}

			if (from == this)
			{
				Send(new StatLockInfo(this));
			}

			IParty ip = m_Party as IParty;

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
					m_MountItem = (mountItem = (FindItemOnLayer(Layer.Mount) as IMountItem)) as Item;
				}

				return mountItem == null ? null : mountItem.Mount;
			}
		}

		[CommandProperty(AccessLevel.Decorator)]
		public bool Mounted { get { return (Mount != null); } }

		private QuestArrow m_QuestArrow;

		public QuestArrow QuestArrow
		{
			get { return m_QuestArrow; }
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

		private static readonly string[] m_GuildTypes = new[] {"", " (Chaos)", " (Order)"};

		public virtual bool CanTarget { get { return true; } }
		public virtual bool ClickTitle { get { return true; } }

		public virtual bool PropertyTitle { get { return m_OldPropertyTitles ? ClickTitle : true; } }

		private static bool m_DisableHiddenSelfClick = true;
		private static bool m_AsciiClickMessage = true;
		private static bool m_GuildClickMessage = true;
		private static bool m_OldPropertyTitles;

		public static bool DisableHiddenSelfClick { get { return m_DisableHiddenSelfClick; } set { m_DisableHiddenSelfClick = value; } }
		public static bool AsciiClickMessage { get { return m_AsciiClickMessage; } set { m_AsciiClickMessage = value; } }
		public static bool GuildClickMessage { get { return m_GuildClickMessage; } set { m_GuildClickMessage = value; } }
		public static bool OldPropertyTitles { get { return m_OldPropertyTitles; } set { m_OldPropertyTitles = value; } }

		public virtual bool ShowFameTitle { get { return true; } } //(m_Player || m_Body.IsHuman) && m_Fame >= 10000; } 

		/// <summary>
		///     Overridable. Event invoked when the Mobile is single clicked.
		/// </summary>
		public virtual void OnSingleClick(Mobile from)
		{
			if (m_Deleted)
			{
				return;
			}
			else if (IsPlayer() && DisableHiddenSelfClick && Hidden && from == this)
			{
				return;
			}

			if (m_GuildClickMessage)
			{
				BaseGuild guild = m_Guild;

				if (guild != null && (m_DisplayGuildTitle || (m_Player && guild.Type != GuildType.Regular)))
				{
					string title = GuildTitle;
					string type;

					if (title == null)
					{
						title = "";
					}
					else
					{
						title = title.Trim();
					}

					if (guild.Type >= 0 && (int)guild.Type < m_GuildTypes.Length)
					{
						type = m_GuildTypes[(int)guild.Type];
					}
					else
					{
						type = "";
					}

					string text = String.Format(title.Length <= 0 ? "[{1}]{2}" : "[{0}, {1}]{2}", title, guild.Abbreviation, type);

					PrivateOverheadMessage(MessageType.Regular, SpeechHue, true, text, from.NetState);
				}
			}

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

			string name = Name;

			if (name == null)
			{
				name = String.Empty;
			}

			string prefix = "";

			if (ShowFameTitle && (m_Player || m_Body.IsHuman) && m_Fame >= 10000)
			{
				prefix = (m_Female ? "Lady" : "Lord");
			}

			string suffix = "";

			if (ClickTitle && Title != null && Title.Length > 0)
			{
				suffix = Title;
			}

			suffix = ApplyNameSuffix(suffix);

			string val;

			if (prefix.Length > 0 && suffix.Length > 0)
			{
				val = String.Concat(prefix, " ", name, " ", suffix);
			}
			else if (prefix.Length > 0)
			{
				val = String.Concat(prefix, " ", name);
			}
			else if (suffix.Length > 0)
			{
				val = String.Concat(name, " ", suffix);
			}
			else
			{
				val = name;
			}

			PrivateOverheadMessage(MessageType.Label, hue, m_AsciiClickMessage, val, from.NetState);
		}

		public bool CheckSkill(SkillName skill, double minSkill, double maxSkill)
		{
			if (m_SkillCheckLocationHandler == null)
			{
				return false;
			}
			else
			{
				return m_SkillCheckLocationHandler(this, skill, minSkill, maxSkill);
			}
		}

		public bool CheckSkill(SkillName skill, double chance)
		{
			if (m_SkillCheckDirectLocationHandler == null)
			{
				return false;
			}
			else
			{
				return m_SkillCheckDirectLocationHandler(this, skill, chance);
			}
		}

		public bool CheckTargetSkill(SkillName skill, object target, double minSkill, double maxSkill)
		{
			if (m_SkillCheckTargetHandler == null)
			{
				return false;
			}
			else
			{
				return m_SkillCheckTargetHandler(this, skill, target, minSkill, maxSkill);
			}
		}

		public bool CheckTargetSkill(SkillName skill, object target, double chance)
		{
			if (m_SkillCheckDirectTargetHandler == null)
			{
				return false;
			}
			else
			{
				return m_SkillCheckDirectTargetHandler(this, skill, target, chance);
			}
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
		public Item ShieldArmor { get { return FindItemOnLayer(Layer.TwoHanded); } }

		public Item NeckArmor { get { return FindItemOnLayer(Layer.Neck); } }

		public Item HandArmor { get { return FindItemOnLayer(Layer.Gloves); } }

		public Item HeadArmor { get { return FindItemOnLayer(Layer.Helm); } }

		public Item ArmsArmor { get { return FindItemOnLayer(Layer.Arms); } }

		public Item LegsArmor
		{
			get
			{
				Item ar = FindItemOnLayer(Layer.InnerLegs);

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
				Item ar = FindItemOnLayer(Layer.InnerTorso);

				if (ar == null)
				{
					ar = FindItemOnLayer(Layer.Shirt);
				}

				return ar;
			}
		}

        public Item Talisman
        {
            get
            {
                return FindItemOnLayer(Layer.Talisman) as Item;
            }
        }

        public Item Ring
        {
            get
            {
                return FindItemOnLayer(Layer.Ring) as Item;
            }
        }

        public Item Bracelet
        {
            get
            {
                return FindItemOnLayer(Layer.Bracelet) as Item;
            }
        }
		#endregion

		/// <summary>
		///     Gets or sets the maximum attainable value for <see cref="RawStr" />, <see cref="RawDex" />, and <see cref="RawInt" />.
		/// </summary>
		[CommandProperty(AccessLevel.GameMaster)]
		public int StatCap
		{
			get { return m_StatCap; }
			set
			{
				if (m_StatCap != value)
				{
					m_StatCap = value;

					Delta(MobileDelta.StatCap);
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual bool Meditating { get; set; }

		[CommandProperty(AccessLevel.Decorator)]
		public bool CanSwim { get { return m_CanSwim; } set { m_CanSwim = value; } }

		[CommandProperty(AccessLevel.Decorator)]
		public bool CantWalk { get { return m_CantWalk; } set { m_CantWalk = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool CanHearGhosts { get { return m_CanHearGhosts || IsStaff(); } set { m_CanHearGhosts = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int RawStatTotal { get { return RawStr + RawDex + RawInt; } }

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
