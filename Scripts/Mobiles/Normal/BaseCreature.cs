#region Header
// **********
// ServUO - BaseCreature.cs
// **********
#endregion

#region References
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Server.ContextMenus;
using Server.Engines.PartySystem;
using Server.Engines.Quests;
using Server.Engines.Quests.Doom;
using Server.Engines.Quests.Haven;
using Server.Engines.XmlSpawner2;
using Server.Ethics;
using Server.Factions;
using Server.Items;
using Server.Misc;
using Server.Multis;
using Server.Network;
using Server.Regions;
using Server.SkillHandlers;
using Server.Spells;
using Server.Spells.Bushido;
using Server.Spells.Necromancy;
using Server.Spells.Sixth;
using Server.Spells.Spellweaving;
using Server.Targeting;
using System.Linq;
using Server.Spells.SkillMasteries;
using Server.Prompts;
using Server.Engines.VvV;
#endregion

namespace Server.Mobiles
{

    #region Enums
    /// <summary>
    ///     Summary description for MobileAI.
    /// </summary>
    public enum FightMode
    {
        None, // Never focus on others
        Aggressor, // Only attack aggressors
        Strongest, // Attack the strongest
        Weakest, // Attack the weakest
        Closest, // Attack the closest
        Evil, // Only attack aggressor -or- negative karma
        Good // Only attack aggressor -or- positive karma
    }

    public enum OrderType
    {
        None, //When no order, let's roam
        Come, //"(All/Name) come"  Summons all or one pet to your location.
        Drop, //"(Name) drop"  Drops its loot to the ground (if it carries any).
        Follow, //"(Name) follow"  Follows targeted being.
                //"(All/Name) follow me"  Makes all or one pet follow you.
        Friend, //"(Name) friend"  Allows targeted player to confirm resurrection.
        Unfriend, // Remove a friend
        Guard, //"(Name) guard"  Makes the specified pet guard you. Pets can only guard their owner.
               //"(All/Name) guard me"  Makes all or one pet guard you.
        Attack, //"(All/Name) kill",
                //"(All/Name) attack"  All or the specified pet(s) currently under your control attack the target.
        Patrol, //"(Name) patrol"  Roves between two or more guarded targets.
        Release, //"(Name) release"  Releases pet back into the wild (removes "tame" status).
        Stay, //"(All/Name) stay" All or the specified pet(s) will stop and stay in current spot.
        Stop, //"(All/Name) stop Cancels any current orders to attack, guard or follow.
        Transfer //"(Name) transfer" Transfers complete ownership to targeted player.
    }

    [Flags]
    public enum FoodType
    {
        None = 0x0000,
        Meat = 0x0001,
        FruitsAndVegies = 0x0002,
        GrainsAndHay = 0x0004,
        Fish = 0x0008,
        Eggs = 0x0010,
        Gold = 0x0020,
        Metal = 0x0040
    }

    [Flags]
    public enum PackInstinct
    {
        None = 0x0000,
        Canine = 0x0001,
        Ostard = 0x0002,
        Feline = 0x0004,
        Arachnid = 0x0008,
        Daemon = 0x0010,
        Bear = 0x0020,
        Equine = 0x0040,
        Bull = 0x0080
    }

    public enum ScaleType
    {
        Red,
        Yellow,
        Black,
        Green,
        White,
        Blue,
        MedusaLight,
        MedusaDark,
        All
    }

    public enum MeatType
    {
        Ribs,
        Bird,
        LambLeg,
        Rotworm
    }

    public enum HideType
    {
        Regular,
        Spined,
        Horned,
        Barbed,
        Fur
    }

    public enum TribeType
    {
        None,
        Terathan,
        Ophidian,
        Savage,
        Orc,
        Fey,
        Undead,
        GrayGoblin,
        GreenGoblin
    }
    #endregion

    public class DamageStore : IComparable
    {
        public Mobile m_Mobile;
        public int m_Damage;
        public bool m_HasRight;

        public DamageStore(Mobile m, int damage)
        {
            m_Mobile = m;
            m_Damage = damage;
        }

        public int CompareTo(object obj)
        {
            DamageStore ds = (DamageStore)obj;

            return ds.m_Damage - m_Damage;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class FriendlyNameAttribute : Attribute
    {
        //future use: Talisman 'Protection/Bonus vs. Specific Creature
        private readonly TextDefinition m_FriendlyName;

        public TextDefinition FriendlyName { get { return m_FriendlyName; } }

        public FriendlyNameAttribute(TextDefinition friendlyName)
        {
            m_FriendlyName = friendlyName;
        }

        public static TextDefinition GetFriendlyNameFor(Type t)
        {
            if (t.IsDefined(typeof(FriendlyNameAttribute), false))
            {
                var objs = t.GetCustomAttributes(typeof(FriendlyNameAttribute), false);

                if (objs != null && objs.Length > 0)
                {
                    FriendlyNameAttribute friendly = objs[0] as FriendlyNameAttribute;

                    return friendly.FriendlyName;
                }
            }

            return t.Name;
        }
    }

    public class BaseCreature : Mobile, IHonorTarget, IEngravable
    {
        public const int MaxLoyalty = 100;

        #region Var declarations
        private BaseAI m_AI; // THE AI

        private AIType m_CurrentAI; // The current AI
        private AIType m_DefaultAI; // The default AI

        private FightMode m_FightMode; // The style the mob uses

        private int m_iRangePerception; // The view area
        private int m_iRangeFight; // The fight distance

        private bool m_bDebugAI; // Show debug AI messages

        private int m_iTeam; // Monster Team

        private double m_dActiveSpeed; // Timer speed when active
        private double m_dPassiveSpeed; // Timer speed when not active
        private double m_dCurrentSpeed; // The current speed, lets say it could be changed by something;

        private Point3D m_pHome; // The home position of the creature, used by some AI
        private int m_iRangeHome = 10; // The home range of the creature

        private readonly List<Type> m_arSpellAttack; // List of attack spell/power
        private readonly List<Type> m_arSpellDefense; // List of defensive spell/power

        private bool m_bControlled; // Is controlled
        private Mobile m_ControlMaster; // My master
        private IDamageable m_ControlTarget; // My target mobile
        private Point3D m_ControlDest; // My target destination (patrol)
        private OrderType m_ControlOrder; // My order

        private int m_Loyalty;

        private double m_dMinTameSkill;
        private double m_CurrentTameSkill;
        private bool m_bTamable;

        private bool m_bSummoned;
        private DateTime m_SummonEnd;
        private int m_iControlSlots = 1;

        private bool m_bBardProvoked;
        private bool m_bBardPacified;
        private Mobile m_bBardMaster;
        private Mobile m_bBardTarget;
        private WayPoint m_CurrentWayPoint;
        private IPoint2D m_TargetLocation;

        private int _CurrentNavPoint;
        private Dictionary<Map, List<Point2D>> _NavPoints;

        private Mobile m_SummonMaster;

        private int m_HitsMax = -1;
        private int m_StamMax = -1;
        private int m_ManaMax = -1;
        private int m_DamageMin = -1;
        private int m_DamageMax = -1;

        private int m_PhysicalResistance, m_PhysicalDamage = 100;
        private int m_FireResistance, m_FireDamage;
        private int m_ColdResistance, m_ColdDamage;
        private int m_PoisonResistance, m_PoisonDamage;
        private int m_EnergyResistance, m_EnergyDamage;

        private List<Mobile> m_Owners;
        private List<Mobile> m_Friends;

        private bool m_IsStabled;

        private bool m_HasGeneratedLoot; // have we generated our loot yet?

        private bool m_Paragon;

        private string m_CorpseNameOverride;

        private int m_FailedReturnHome; /* return to home failure counter */

        private bool m_IsChampionSpawn;

        private Mobile m_InitialFocus;
        #endregion

        #region Monster Stealables
        private bool m_HasBeenStolen;

        [CommandProperty(AccessLevel.Administrator)]
        public bool HasBeenStolen
        {
            get { return m_HasBeenStolen; }
            set { m_HasBeenStolen = value; }
        }
        #endregion

        public virtual InhumanSpeech SpeechType { get { return null; } }

        public virtual bool ForceStayHome { get { return false; } }

        public int FollowRange { get; set; }

        public virtual bool CanBeParagon { get { return true; } }

        /* Do not serialize this till the code is finalized */

        private bool m_SeeksHome;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool SeeksHome { get { return m_SeeksHome; } set { m_SeeksHome = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public string CorpseNameOverride { get { return m_CorpseNameOverride; } set { m_CorpseNameOverride = value; } }

        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Administrator)]
        public bool IsStabled
        {
            get { return m_IsStabled; }
            set
            {
                m_IsStabled = value;
                if (m_IsStabled)
                {
                    StopDeleteTimer();
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Administrator)]
        public Mobile StabledBy { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsPrisoner { get; set; }

        protected DateTime SummonEnd { get { return m_SummonEnd; } set { m_SummonEnd = value; } }

        public virtual Faction FactionAllegiance { get { return null; } }
        public virtual int FactionSilverWorth { get { return 30; } }

        public virtual int BaseLootBudget { get { return RandomItemGenerator.GetBaseBudget(this); } }

        public virtual int DefaultHitsRegen 
        {
            get
            {
                int regen = 0;

                if (IsAnimatedDead)
                    regen = 4;

                if (IsParagon)
                    regen += 40;

                regen += HumilityVirtue.GetRegenBonus(this);

                if (AbilityProfile != null)
                    regen += AbilityProfile.RegenHits;

                return regen;
            }
        }

        public virtual int DefaultStamRegen
        {
            get
            {
                int regen = 0;

                regen += MasteryInfo.EnchantedSummoningBonus(this);

                if (IsParagon)
                    regen += 40;

                if (AbilityProfile != null)
                    regen += AbilityProfile.RegenStam;

                return regen;
            }
        }

        public virtual int DefaultManaRegen
        {
            get
            {
                int regen = 0;

                if (IsParagon)
                    regen += 40;

                if (AbilityProfile != null)
                    regen += AbilityProfile.RegenMana;

                return regen;
            }
        }

        #region Bonding
        public const bool BondingEnabled = true;

        public virtual bool IsBondable { get { return (BondingEnabled && !Summoned && !m_Allured && !IsGolem); } }
        public virtual TimeSpan BondingDelay { get { return TimeSpan.FromDays(7.0); } }
        public virtual TimeSpan BondingAbandonDelay { get { return TimeSpan.FromDays(1.0); } }

        public override bool CanRegenHits { get { return !m_IsDeadPet && !Summoned && base.CanRegenHits; } }
        public override bool CanRegenStam { get { return !IsParagon && !m_IsDeadPet && base.CanRegenStam; } }
        public override bool CanRegenMana { get { return !m_IsDeadPet && base.CanRegenMana; } }

        public override bool IsDeadBondedPet { get { return m_IsDeadPet; } }

        private bool m_IsBonded;
        private bool m_IsDeadPet;
        private DateTime m_BondingBegin;
        private DateTime m_OwnerAbandonTime;

        [CommandProperty(AccessLevel.GameMaster)]
        public Spawner MySpawner
        {
            get
            {
                if (Spawner is Spawner)
                {
                    return (Spawner as Spawner);
                }

                return null;
            }
            set { }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile LastOwner
        {
            get
            {
                if (m_Owners == null || m_Owners.Count == 0)
                {
                    return null;
                }

                return m_Owners[m_Owners.Count - 1];
            }
        }

        public bool IsGolem
        {
            get { return this is IRepairableMobile; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsBonded
        {
            get { return m_IsBonded; }
            set
            {
                m_IsBonded = value;
                InvalidateProperties();
            }
        }

        public bool IsDeadPet { get { return m_IsDeadPet; } set { m_IsDeadPet = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime BondingBegin { get { return m_BondingBegin; } set { m_BondingBegin = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime OwnerAbandonTime { get { return m_OwnerAbandonTime; } set { m_OwnerAbandonTime = value; } }
        #endregion

        #region Delete Previously Tamed Timer
        private DeleteTimer m_DeleteTimer;

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan DeleteTimeLeft
        {
            get
            {
                if (m_DeleteTimer != null && m_DeleteTimer.Running)
                {
                    return m_DeleteTimer.Next - DateTime.UtcNow;
                }

                return TimeSpan.Zero;
            }
        }

        private class DeleteTimer : Timer
        {
            private readonly Mobile m;

            public DeleteTimer(Mobile creature, TimeSpan delay)
                : base(delay)
            {
                m = creature;
                Priority = TimerPriority.OneMinute;
            }

            protected override void OnTick()
            {
                m.Delete();
            }
        }

        public void BeginDeleteTimer()
        {
            if (!(this is BaseEscortable) && !Summoned && !Deleted && !IsStabled)
            {
                StopDeleteTimer();
                m_DeleteTimer = new DeleteTimer(this, TimeSpan.FromDays(3.0));
                m_DeleteTimer.Start();
            }
        }

        public void StopDeleteTimer()
        {
            if (m_DeleteTimer != null)
            {
                m_DeleteTimer.Stop();
                m_DeleteTimer = null;
            }
        }
        #endregion

        #region IEngravable Members
        private string m_EngravedText;

        [CommandProperty(AccessLevel.GameMaster)]
        public string EngravedText
        {
            get { return m_EngravedText != null ? Utility.FixHtml(m_EngravedText) : null; }
            set
            {
                m_EngravedText = value;
                InvalidateProperties();
            }
        }
        #endregion

        #region Pet Training
        private AbilityProfile _Profile;
        private TrainingProfile _TrainingProfile;

        [CommandProperty(AccessLevel.GameMaster)]
        public AbilityProfile AbilityProfile { get { return _Profile; } set { _Profile = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public TrainingProfile TrainingProfile { get { return _TrainingProfile; } set { _TrainingProfile = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public double BardingDifficulty { get { return BaseInstrument.GetBaseDifficulty(this); } }

        public virtual WeaponAbility TryGetWeaponAbility()
        {
            if (_Profile != null && _Profile.WeaponAbilities != null && _Profile.WeaponAbilities.Length > 0)
            {
                return _Profile.WeaponAbilities[Utility.Random(_Profile.WeaponAbilities.Length)];
            }
            else
            {
                return GetWeaponAbility();
            }
        }

        public virtual void InitializeAbilities()
        {
            switch (AI)
            {
                case AIType.AI_Mage: SetMagicalAbility(MagicalAbility.Magery); break;
                case AIType.AI_NecroMage: SetMagicalAbility(!Controlled ? MagicalAbility.Necromancy : MagicalAbility.Necromage); break;
                case AIType.AI_Necro: SetMagicalAbility(MagicalAbility.Necromancy); break;
                case AIType.AI_Spellweaving: SetMagicalAbility(MagicalAbility.Spellweaving); break;
                case AIType.AI_Mystic: SetMagicalAbility(MagicalAbility.Mysticism); break;
                case AIType.AI_Samurai: SetMagicalAbility(MagicalAbility.Bushido); break;
                case AIType.AI_Ninja: SetMagicalAbility(MagicalAbility.Ninjitsu); break;
                case AIType.AI_Paladin: SetMagicalAbility(MagicalAbility.Chivalry); break;
            }

            if (HasBreath)
            {
                SetSpecialAbility(SpecialAbility.DragonBreath);
            }

            if (HealChance > 0.0 && HealChance >= Utility.RandomDouble())
            {
                SetSpecialAbility(SpecialAbility.Heal);
            }

            if (PetTrainingHelper.Enabled)
                return;

            if(Skills[SkillName.Focus].Value == 0)
                SetSkill(SkillName.Focus, 2, 20);

            if(Skills[SkillName.DetectHidden].Value == 0 && !(this is BaseVendor))
                SetSkill(SkillName.DetectHidden, Utility.RandomList(10, 60));
        }

        public void SetMagicalAbility(MagicalAbility ability)
        {
            PetTrainingHelper.GetAbilityProfile(this, true).AddAbility(ability, false);
        }

        public void SetSpecialAbility(SpecialAbility ability)
        {
            PetTrainingHelper.GetAbilityProfile(this, true).AddAbility(ability, false);
        }

        public void SetAreaEffect(AreaEffect ability)
        {
            PetTrainingHelper.GetAbilityProfile(this, true).AddAbility(ability, false);
        }

        public void SetWeaponAbility(WeaponAbility ability)
        {
            PetTrainingHelper.GetAbilityProfile(this, true).AddAbility(ability, false);
        }

        public virtual double AverageThreshold { get { return 0.33; } }

        public List<double> _InitAverage;

        private void SetAverage(double min, double max, double value)
        {
            if (PetTrainingHelper.Enabled && CanLowerSlot() && max > min)
            {
                if (_InitAverage == null)
                    _InitAverage = new List<double>();

                _InitAverage.Add((value - min) / (max - min));
            }
        }

        public static Type[] SlotLowerables { get { return _SlotLowerables; } }
        private static Type[] _SlotLowerables =
        {
            typeof(Nightmare), typeof(Najasaurus), typeof(RuneBeetle), typeof(GreaterDragon), typeof(FrostDragon),
            typeof(WhiteWyrm), typeof(Reptalon), typeof(DragonTurtleHatchling), typeof(Phoenix), typeof(FrostMite),
            typeof(DireWolf), typeof(Skree), typeof(HighPlainsBoura), typeof(LesserHiryu), typeof(DragonWolf),
            typeof(BloodFox)
        };

        private bool CanLowerSlot()
        {
            return _SlotLowerables.Any(t => t == GetType());
        }

        public void CalculateSlots(int slots)
        {
            var def = PetTrainingHelper.GetTrainingDefinition(this);

            if (def == null)
            {
                ControlSlotsMin = slots;
                ControlSlotsMax = slots;
                return;
            }
            else
            {
                ControlSlotsMin = def.ControlSlotsMin;
                ControlSlotsMax = def.ControlSlotsMax;
            }

            if (_InitAverage == null)
                return;

            double total = _InitAverage.Sum(d => d);

            if (total / (double)_InitAverage.Count <= AverageThreshold)
            {
                ControlSlotsMin = Math.Max(1, ControlSlotsMin - 1);
            }

            ColUtility.Free(_InitAverage);
            _InitAverage = null;
        }

        public void AdjustTameRequirements()
        {
            // Currently, with increased control slots, taming skill does not seem to pass 108.0
            if(ControlSlots <=ControlSlotsMin)
            {
                CurrentTameSkill = MinTameSkill;
            }
            else if (MinTameSkill < 108)
            {
                double minSkill = Math.Ceiling(MinTameSkill);

                if (MinTameSkill < 0)
                {
                    CurrentTameSkill = Math.Ceiling(Math.Min(108.0, Math.Max(0, CurrentTameSkill) + (Math.Abs(minSkill) * .7)));
                }
                else
                {
                    double level = ControlSlots - ControlSlotsMin;
                    double levelFactor = (double)(1 + (ControlSlotsMax - ControlSlotsMin)) / minSkill;

                    CurrentTameSkill = Math.Ceiling(Math.Min(108.0, minSkill + (minSkill * ((levelFactor * 7) * level))));
                }

                if (CurrentTameSkill < MinTameSkill)
                    CurrentTameSkill = MinTameSkill;
            }
        }
        #endregion

        #region Skill Masteries
        private SkillName _Mastery;

        [CommandProperty(AccessLevel.GameMaster)]
		public SkillName Mastery 
		{
			get { return _Mastery; } 
			set
			{
				var old = _Mastery;
				_Mastery = value;
				
				if(old != _Mastery)
					UpdateMasteryInfo();
			}
		}
		
		public virtual MasteryInfo[] Masteries { get; set; }
		public DateTime NextMastery { get; set; }

        public void UpdateMasteryInfo()
        {
            if (_Mastery == SkillName.Alchemy)
            {
                Masteries = null;
            }
            else
            {
                var masteries = MasteryInfo.Infos.Where(i => i.MasterySkill == _Mastery && !i.Passive && i.SpellType != typeof(BodyGuardSpell)).ToArray();

                if (masteries != null && masteries.Length > 0)
                {
                    Masteries = masteries;
                }
            }
        }

        public virtual void CheckCastMastery()
        {
            if (Spell == null && Masteries != null && Masteries.Length > 0 && NextMastery < DateTime.UtcNow)
            {
                var info = Masteries[Utility.Random(Masteries.Length)];

                if (info != null)
                {
                    if (info.SpellType.IsSubclassOf(typeof(SkillMasteryMove)))
                    {
                        var move = SpellRegistry.GetSpecialMove(info.SpellID);

                        if (move != null)
                        {
                            SpecialMove.SetCurrentMove(this, move);
                            NextMastery = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(10, 60));
                        }
                    }
                    else
                    {
                        var spell = SpellRegistry.NewSpell(info.SpellID, this, null);

                        if (spell != null)
                        {
                            spell.Cast();
                            NextMastery = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(10, 60));
                        }
                    }
                }
            }
        }
        #endregion

        public virtual bool AutoRearms { get { return false; } }

        public virtual double WeaponAbilityChance { get { return 0.4; } }

        public virtual WeaponAbility GetWeaponAbility()
        {
            return null;
        }

        #region Elemental Resistance/Damage
        public override int BasePhysicalResistance { get { return m_PhysicalResistance; } }
        public override int BaseFireResistance { get { return m_FireResistance; } }
        public override int BaseColdResistance { get { return m_ColdResistance; } }
        public override int BasePoisonResistance { get { return m_PoisonResistance; } }
        public override int BaseEnergyResistance { get { return m_EnergyResistance; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PhysicalResistanceSeed
        {
            get { return m_PhysicalResistance; }
            set
            {
                m_PhysicalResistance = value;
                UpdateResistances();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int FireResistSeed
        {
            get { return m_FireResistance; }
            set
            {
                m_FireResistance = value;
                UpdateResistances();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ColdResistSeed
        {
            get { return m_ColdResistance; }
            set
            {
                m_ColdResistance = value;
                UpdateResistances();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PoisonResistSeed
        {
            get { return m_PoisonResistance; }
            set
            {
                m_PoisonResistance = value;
                UpdateResistances();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int EnergyResistSeed
        {
            get { return m_EnergyResistance; }
            set
            {
                m_EnergyResistance = value;
                UpdateResistances();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PhysicalDamage { get { return m_PhysicalDamage; } set { m_PhysicalDamage = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int FireDamage { get { return m_FireDamage; } set { m_FireDamage = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ColdDamage { get { return m_ColdDamage; } set { m_ColdDamage = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PoisonDamage { get { return m_PoisonDamage; } set { m_PoisonDamage = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int EnergyDamage { get { return m_EnergyDamage; } set { m_EnergyDamage = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ChaosDamage { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int DirectDamage { get; set; }
        #endregion

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsParagon
        {
            get { return m_Paragon; }
            set
            {
                if (m_Paragon == value)
                {
                    return;
                }
                else if (value)
                {
                    XmlParagon.Convert(this);
                }
                else
                {
                    XmlParagon.UnConvert(this);
                }

                m_Paragon = value;

                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsChampionSpawn
        {
            get { return m_IsChampionSpawn; }
            set
            {
                if (!m_IsChampionSpawn && value)
                    SetToChampionSpawn();

                m_IsChampionSpawn = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile InitialFocus
        {
            get
            {
                if (m_InitialFocus != null && (!m_InitialFocus.Alive || m_InitialFocus.Deleted))
                {
                    m_InitialFocus = null;
                }

                return m_InitialFocus;
            }
            set { m_InitialFocus = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public override IDamageable Combatant
        {
            get
            {
                return base.Combatant;
            }
            set
            {
                Mobile initialFocus = InitialFocus;

                if (base.Combatant == null)
                {
                    if (value is Mobile && AttacksFocus)
                        InitialFocus = (Mobile)value;
                }
                else if (AttacksFocus && initialFocus != null && value != initialFocus && !initialFocus.Hidden && InRange(initialFocus.Location, RangePerception))
                {
                    //Keeps focus
                    base.Combatant = initialFocus;
                    return;
                }

                base.Combatant = value;
            }
        }

        public bool IsAmbusher { get; set; }

        public virtual bool HasManaOveride { get { return false; } }

        public virtual FoodType FavoriteFood { get { return FoodType.Meat; } }
        public virtual PackInstinct PackInstinct { get { return PackInstinct.None; } }

        public List<Mobile> Owners { get { return m_Owners; } }

        public virtual bool AllowMaleTamer { get { return true; } }
        public virtual bool AllowFemaleTamer { get { return true; } }
        public virtual bool SubdueBeforeTame { get { return false; } }
        public virtual bool StatLossAfterTame { get { return SubdueBeforeTame; } }
        public virtual bool ReduceSpeedWithDamage { get { return true; } }
        public virtual bool IsSubdued { get { return SubdueBeforeTame && (Hits < ((double)HitsMax / 10)); } }

        public virtual bool Commandable { get { return true; } }

        public virtual Poison HitPoison { get { return null; } }
        public virtual double HitPoisonChance { get { return 0.5; } }
        public virtual Poison PoisonImmune { get { return null; } }

        public virtual bool BardImmune { get { return false; } }
        public virtual bool Unprovokable { get { return BardImmune || m_IsDeadPet; } }
        public virtual bool Uncalmable { get { return BardImmune || m_IsDeadPet; } }
        public virtual bool AreaPeaceImmune { get { return BardImmune || m_IsDeadPet; } }

        public virtual bool BleedImmune { get { return false; } }
        public virtual double BonusPetDamageScalar { get { return 1.0; } }
        public virtual bool AllureImmune { get { return false; } }

        public virtual bool DeathAdderCharmable { get { return false; } }

        public virtual bool GivesFameAndKarmaAward { get { return true; } }

        //TODO: Find the pub 31 tweaks to the DispelDifficulty and apply them of course.
        public virtual double DispelDifficulty { get { return 0.0; } } // at this skill level we dispel 50% chance
        public virtual double DispelFocus { get { return 20.0; } }
        // at difficulty - focus we have 0%, at difficulty + focus we have 100%
        public virtual bool DisplayWeight { get { return Backpack is StrongBackpack; } }

        public virtual double TeleportChance { get { return 0.05; } }
        public virtual bool AttacksFocus { get { return false; } }
        public virtual bool ShowSpellMantra { get { return false; } }
        public virtual bool FreezeOnCast { get { return ShowSpellMantra; } }
        public virtual bool CanFly { get { return false; } }

        #region High Seas
        public virtual bool TaintedLifeAura { get { return false; } }
        #endregion

        #region Special Abilities and Area Effects overrides
        public virtual int AreaPoisonDamage { get { return 0; } }
        public virtual Poison HitAreaPoison { get { return Poison.Deadly; } }

        #region Dragon Breath

        // Old way of enabling. Kept for compatibility and construction
        public virtual bool HasBreath { get { return false; } }

        // Base damage given is: CurrentHitPoints * BreathDamageScalar
        public virtual double BreathDamageScalar { get { return (Core.AOS ? 0.16 : 0.05); } }

        // Creature stops moving for 1.0 seconds while breathing
        public virtual double BreathStallTime { get { return 1.0; } }

        // Effect is sent 1.3 seconds after BreathAngerSound and BreathAngerAnimation is played
        public virtual double BreathEffectDelay { get { return 1.3; } }

        // Damage is given 1.0 seconds after effect is sent
        public virtual double BreathDamageDelay { get { return 1.0; } }

        // Damage types
        public virtual int BreathChaosDamage { get { return 0; } }
        public virtual int BreathPhysicalDamage { get { return 0; } }
        public virtual int BreathFireDamage { get { return 100; } }
        public virtual int BreathColdDamage { get { return 0; } }
        public virtual int BreathPoisonDamage { get { return 0; } }
        public virtual int BreathEnergyDamage { get { return 0; } }

        // Is immune to breath damages
        public virtual bool BreathImmune { get { return false; } }

        public virtual double BreathMinDelay { get { return 30.0; } }
        public virtual double BreathMaxDelay { get { return 45.0; } }

        // Effect details and sound
        public virtual int BreathEffectItemID { get { return 0x36D4; } }
        public virtual int BreathEffectSpeed { get { return 5; } }
        public virtual int BreathEffectDuration { get { return 0; } }
        public virtual bool BreathEffectExplodes { get { return false; } }
        public virtual bool BreathEffectFixedDir { get { return false; } }
        public virtual int BreathEffectHue { get { return 0; } }
        public virtual int BreathEffectRenderMode { get { return 0; } }

        public virtual int BreathEffectSound { get { return 0x227; } }

        // Anger sound/animations
        public virtual int BreathAngerSound { get { return GetAngerSound(); } }
        public virtual int BreathAngerAnimation { get { return 12; } }

        public virtual void BreathStart(IDamageable target)
        {
            RevealingAction();
            BreathStallMovement();
            BreathPlayAngerSound();
            BreathPlayAngerAnimation();

            Direction = GetDirectionTo(target);

            Timer.DelayCall(TimeSpan.FromSeconds(BreathEffectDelay), new TimerStateCallback(BreathEffect_Callback), target);
        }

        public virtual void BreathStallMovement()
        {
            if (m_AI != null)
            {
                m_AI.NextMove = Core.TickCount + (int)(BreathStallTime * 1000);
            }
        }

        public virtual void BreathPlayAngerSound()
        {
            PlaySound(BreathAngerSound);
        }

        public virtual void BreathPlayAngerAnimation()
        {
            if (Core.SA)
            {
                Animate(AnimationType.Pillage, 0);
            }
            else
            {
                Animate(BreathAngerAnimation, 5, 1, true, false, 0);
            }
        }

        public virtual void BreathEffect_Callback(object state)
        {
            RevealingAction();
            IDamageable target = (IDamageable)state;

            if (!target.Alive || !CanBeHarmful(target))
            {
                return;
            }

            BreathPlayEffectSound();
            BreathPlayEffect(target);

            Timer.DelayCall(TimeSpan.FromSeconds(BreathDamageDelay), new TimerStateCallback(BreathDamage_Callback), target);
        }

        public virtual void BreathPlayEffectSound()
        {
            PlaySound(BreathEffectSound);
        }

        public virtual void BreathPlayEffect(IDamageable target)
        {
            Effects.SendMovingEffect(
                this,
                target,
                BreathEffectItemID,
                BreathEffectSpeed,
                BreathEffectDuration,
                BreathEffectFixedDir,
                BreathEffectExplodes,
                BreathEffectHue,
                BreathEffectRenderMode);
        }

        public virtual void BreathDamage_Callback(object state)
        {
            IDamageable target = (IDamageable)state;

            if (target is BaseCreature && ((BaseCreature)target).BreathImmune)
            {
                return;
            }

            if (CanBeHarmful(target))
            {
                DoHarmful(target);
                BreathDealDamage(target);
            }
        }

        public virtual void BreathDealDamage(IDamageable target)
        {
            if (!(target is Mobile) || !Evasion.CheckSpellEvasion((Mobile)target))
            {
                int physDamage = BreathPhysicalDamage;
                int fireDamage = BreathFireDamage;
                int coldDamage = BreathColdDamage;
                int poisDamage = BreathPoisonDamage;
                int nrgyDamage = BreathEnergyDamage;

                if (BreathChaosDamage > 0)
                {
                    switch (Utility.Random(5))
                    {
                        case 0: physDamage += BreathChaosDamage; break;
                        case 1: fireDamage += BreathChaosDamage; break;
                        case 2: coldDamage += BreathChaosDamage; break;
                        case 3: poisDamage += BreathChaosDamage; break;
                        case 4: nrgyDamage += BreathChaosDamage; break;
                    }
                }

                if (physDamage == 0 && fireDamage == 0 && coldDamage == 0 && poisDamage == 0 && nrgyDamage == 0)
                {
                    AOS.Damage(target, this, BreathComputeDamage(), 0, 0, 0, 0, 0, 0, 100);
                }
                else
                {
                    AOS.Damage(target, this, BreathComputeDamage(), physDamage, fireDamage, coldDamage, poisDamage, nrgyDamage);
                }
            }
        }

        public virtual int BreathComputeDamage()
        {
            int damage = (int)(Hits * BreathDamageScalar);

            if (IsParagon)
            {
                damage = (int)(damage / XmlParagon.GetHitsBuff(this));
            }

            if (damage > 200)
            {
                damage = 200;
            }

            return damage;
        }
        #endregion

        #endregion

        #region Spill Acid
        public void SpillAcid(int Amount)
        {
            SpillAcid(null, Amount);
        }

        public void SpillAcid(Mobile target, int Amount)
        {
            if ((target != null && target.Map == null) || Map == null)
            {
                return;
            }

            for (int i = 0; i < Amount; ++i)
            {
                Point3D loc = Location;
                Map map = Map;
                Item acid = NewHarmfulItem();

                if (target != null && target.Map != null && Amount == 1)
                {
                    loc = target.Location;
                    map = target.Map;
                }
                else
                {
                    bool validLocation = false;
                    for (int j = 0; !validLocation && j < 10; ++j)
                    {
                        loc = new Point3D(loc.X + (Utility.Random(0, 3) - 2), loc.Y + (Utility.Random(0, 3) - 2), loc.Z);
                        loc.Z = map.GetAverageZ(loc.X, loc.Y);
                        validLocation = map.CanFit(loc, 16, false, false);
                    }
                }
                acid.MoveToWorld(loc, map);
            }
        }

        /*
        Solen Style, override me for other mobiles/items:
        kappa+acidslime, grizzles+whatever, etc.
        */

        public virtual Item NewHarmfulItem()
        {
            return new PoolOfAcid(TimeSpan.FromSeconds(10), 30, 30);
        }
        #endregion

        #region Life Drain
        public virtual bool DrainsLife { get { return false; } }
        public virtual double DrainsLifeChance { get { return 0.1; } }
        public virtual int DrainAmount { get { return Utility.RandomMinMax(10, 40); } }

        public virtual int GetDrainAmount(Mobile target)
        {
            return DrainAmount;
        }

        public virtual void DrainLife()
        {
            List<Mobile> list = new List<Mobile>();

            IPooledEnumerable eable = GetMobilesInRange(2);
            foreach (Mobile m in eable)
            {
                if (m == this || !CanBeHarmful(m))
                    continue;

                if (m is BaseCreature && (((BaseCreature)m).Controlled || ((BaseCreature)m).Summoned || ((BaseCreature)m).Team != Team))
                    list.Add(m);
                else if (m.Player)
                    list.Add(m);
            }

            eable.Free();

            foreach (Mobile m in list)
            {
                DoHarmful(m);

                m.FixedParticles(0x374A, 10, 15, 5013, 0x496, 0, EffectLayer.Waist);
                m.PlaySound(0x231);

                m.SendMessage("You feel the life drain out of you!");

                int toDrain = GetDrainAmount(m);

                //Monster Stealables
                if (m is PlayerMobile)
                {
                    PlayerMobile pm = m as PlayerMobile;
                    toDrain = (int)LifeShieldLotion.HandleLifeDrain(pm, toDrain);
                }
                //end


                Hits += toDrain;
                m.Damage(toDrain, this);
            }

            ColUtility.Free(list);
        }
        #endregion

        #region Colossal Blow
        public virtual bool DoesColossalBlow { get { return false; } }
        public virtual double ColossalBlowChance { get { return 0.3; } }
        public virtual TimeSpan ColossalBlowDuration { get { return TimeSpan.FromSeconds(5); } }

        public bool _Stunning;

        public virtual void DoColossalBlow(Mobile defender)
        {
            _Stunning = true;

            if (Core.SA)
            {
                defender.Animate(AnimationType.Die, 0);
            }
            else
            {
                defender.Animate(21, 6, 1, true, false, 0);
            }

            PlaySound(0xEE);
            defender.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1070696); // You have been stunned by a colossal blow!

            BaseWeapon weapon = Weapon as BaseWeapon;

            if (weapon != null)
                weapon.OnHit(this, defender);

            if (defender.Alive)
            {
                defender.Frozen = true;

                Timer.DelayCall<Mobile>(TimeSpan.FromSeconds(5.0), victim =>
                    {
                        victim.Frozen = false;
                        victim.Combatant = null;
                        victim.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1070695); // You recover your senses.

                        _Stunning = false;

                    }, defender);
            }
        }
        #endregion

        #region Flee!!!
        public virtual bool CanFlee { get { return !m_Paragon; } }

        private DateTime m_EndFlee;

        public DateTime EndFleeTime { get { return m_EndFlee; } set { m_EndFlee = value; } }

        public virtual void StopFlee()
        {
            m_EndFlee = DateTime.MinValue;
        }

        public virtual bool CheckFlee()
        {
            if (HitsMax >= 500)
            {
                return false;
            }

            if (m_EndFlee == DateTime.MinValue)
            {
                return false;
            }

            if (DateTime.UtcNow >= m_EndFlee)
            {
                StopFlee();
                return false;
            }

            return true;
        }

        public virtual void BeginFlee(TimeSpan maxDuration)
        {
            m_EndFlee = DateTime.UtcNow + maxDuration;
        }
        #endregion

        #region True Fear
        public virtual bool CausesTrueFear { get { return false; } }

        private static List<Mobile> m_TrueFearCooldown = new List<Mobile>();

        private const int TrueFearRange = 8;

        public virtual void CauseTrueFear(Mobile m, Point3D oldLocation)
        {
            base.OnMovement(m, oldLocation);

            if (m.Alive && m.Player && InRange(m.Location, TrueFearRange) && !InRange(oldLocation, TrueFearRange))
            {
                if (!m_TrueFearCooldown.Contains(m))
                {
                    int seconds = (int)(13.0 - (m.Skills[SkillName.MagicResist].Value / 10.0));

                    if (seconds < 1)
                        seconds = 1;

                    int number;

                    if (seconds <= 2)
                        number = 1080339; // A sense of discomfort passes through you, but it fades quickly
                    else if (seconds <= 4)
                        number = 1080340; // An unfamiliar fear washes over you, and for a moment you're unable to move
                    else if (seconds <= 7)
                        number = 1080341; // Panic grips you! You're unable to move, to think, to feel anything but fear!
                    else if (seconds <= 10)
                        number = 1080342; // Terror slices into your very being, destroying any chance of resisting ~1_name~ you might have had
                    else
                        number = 1080343; // Everything around you dissolves into darkness as ~1_name~'s burning eyes fill your vision

                    m.SendLocalizedMessage(number, Name, 0x21);

                    m_TrueFearCooldown.Add(m);

                    m.Frozen = true;

                    BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.TrueFear, 1153791, 1153827, TimeSpan.FromSeconds(seconds), m));

                    Timer.DelayCall(TimeSpan.FromSeconds(seconds), new TimerCallback(
                        delegate
                        {
                            m.Frozen = false;
                            m.SendLocalizedMessage(1005603); // You can move again!
                        }));

                    Timer.DelayCall(TimeSpan.FromMinutes(5.0), new TimerCallback(
                        delegate { m_TrueFearCooldown.Remove(m); }));
                }
            }
        }
        #endregion

        public virtual bool IsInvulnerable { get { return false; } }

        public BaseAI AIObject { get { return m_AI; } }

        public const int MaxOwners = 5;

        public virtual OppositionGroup OppositionGroup { get { return null; } }

        // Tribe Opposition stuff
        public virtual TribeType Tribe{ get{ return TribeType.None ; } } // What opposition list am I in?

        public virtual bool IsTribeEnemy(Mobile m)
        {
            // Target must be BaseCreature
            if (!(m is BaseCreature))
            {
                return false;
            }

            BaseCreature c = (BaseCreature)m;

            switch(Tribe)
            {
                case TribeType.Terathan: return (c.Tribe == TribeType.Ophidian);
                case TribeType.Ophidian: return (c.Tribe == TribeType.Terathan);
                case TribeType.Savage: return (c.Tribe == TribeType.Orc);
                case TribeType.Orc: return (c.Tribe == TribeType.Savage);
                case TribeType.Fey: return (c.Tribe == TribeType.Undead);
                case TribeType.Undead: return (c.Tribe == TribeType.Fey);
                case TribeType.GrayGoblin: return (c.Tribe == TribeType.GreenGoblin);
                case TribeType.GreenGoblin: return (c.Tribe == TribeType.GrayGoblin);
                default: return false;
            }
        }

        #region Friends
        public List<Mobile> Friends { get { return m_Friends; } }

        public virtual bool AllowNewPetFriend { get { return (m_Friends == null || m_Friends.Count < 5); } }

        public virtual bool IsPetFriend(Mobile m)
        {
            return (m_Friends != null && m_Friends.Contains(m));
        }

        public virtual void AddPetFriend(Mobile m)
        {
            if (m_Friends == null)
            {
                m_Friends = new List<Mobile>();
            }

            m_Friends.Add(m);
        }

        public virtual void RemovePetFriend(Mobile m)
        {
            if (m_Friends != null)
            {
                m_Friends.Remove(m);
            }
        }

		public virtual bool IsFriend(Mobile m)
		{
			if (Core.TOL)
			{
				if (Tribe != TribeType.None && IsTribeEnemy(m))
				{
					return false;
				}
			}
			else
			{
				OppositionGroup g = OppositionGroup;

				if (g != null && g.IsEnemy(this, m))
				{
					return false;
				}
			}

			if (!(m is BaseCreature))
			{
				return false;
			}

			BaseCreature c = (BaseCreature)m;

			if (m_iTeam != c.m_iTeam)
			{
				return false;
			}
/*
			if (c.Combatant == this)
			{
				return false;
			}
*/
			return ((m_bSummoned || m_bControlled) == (c.m_bSummoned || c.m_bControlled));
		}
        #endregion

        #region Allegiance
        public virtual Ethic EthicAllegiance { get { return null; } }

        public enum Allegiance
        {
            None,
            Ally,
            Enemy
        }

        public virtual Allegiance GetFactionAllegiance(Mobile mob)
        {
            if (mob == null || mob.Map != Faction.Facet || FactionAllegiance == null)
            {
                return Allegiance.None;
            }

            Faction fac = Faction.Find(mob, true);

            if (fac == null)
            {
                return Allegiance.None;
            }

            return (fac == FactionAllegiance ? Allegiance.Ally : Allegiance.Enemy);
        }

        public virtual Allegiance GetEthicAllegiance(Mobile mob)
        {
            if (mob == null || mob.Map != Faction.Facet || EthicAllegiance == null)
            {
                return Allegiance.None;
            }

            Ethic ethic = Ethic.Find(mob, true);

            if (ethic == null)
            {
                return Allegiance.None;
            }

            return (ethic == EthicAllegiance ? Allegiance.Ally : Allegiance.Enemy);
        }
        #endregion

		public virtual bool IsEnemy(Mobile m)
		{
			XmlIsEnemy a = (XmlIsEnemy)XmlAttach.FindAttachment(this, typeof(XmlIsEnemy));

			if (a != null)
			{
				return a.IsEnemy(m);
			}

			if (m is BaseGuard)
			{
				return false;
			}

			if (Combatant != m)
			{
                if (m is PlayerMobile && ((PlayerMobile)m).HonorActive)
                {
                    return false;
                }

                if (TransformationSpellHelper.UnderTransformation(m, typeof(EtherealVoyageSpell)))
                {
                    return false;
                }
			}

			if (Core.TOL)
			{
				if (Tribe != TribeType.None && IsTribeEnemy(m))
				{
					return true;
				}
			}
			else
			{
				OppositionGroup g = OppositionGroup;

				if (g != null && g.IsEnemy(this, m))
				{
					return true;
				}
			}

            BaseCreature c = m as BaseCreature;

            // Are we a non-aggressive FightMode or are they an uncontrolled Summon?
            if (FightMode == FightMode.Aggressor || FightMode == FightMode.Evil || FightMode == FightMode.Good ||
                (c != null && c.m_bSummoned && !c.m_bControlled && c.SummonMaster != null))
            {
                // Faction Opposed Players/Pets are my enemies
                if (GetFactionAllegiance(m) == BaseCreature.Allegiance.Enemy)
                {
                    return true;
                }

                // Ethic Opposed Players/Pets are my enemies
                if (GetEthicAllegiance(m) == BaseCreature.Allegiance.Enemy)
                {
                    return true;
                }

                // Negative Karma are my enemies
                if (FightMode == FightMode.Evil)
                {
                    if (c != null && c.GetMaster() != null)
                    {
                        return (c.GetMaster().Karma < 0);
                    }
                    return (m.Karma < 0);
                }

                // Positive Karma are my enemies
                if (FightMode == FightMode.Good)
                {
                    if (c != null && c.GetMaster() != null)
                    {
                        return (c.GetMaster().Karma > 0);
                    }
                    return (m.Karma > 0);
                }

                // Others are not my enemies
                return false;
            }

			// Faction Allied Players/Pets are not my enemies
			if (GetFactionAllegiance(m) == Allegiance.Ally)
			{
				return false;
			}

			Ethic ourEthic = EthicAllegiance;
			Player pl = Ethics.Player.Find(m, true);

			// Ethic Allied Players/Pets are not my enemies
			if (pl != null && pl.IsShielded && (ourEthic == null || ourEthic == pl.Ethic))
			{
				return false;
			}

			if (!(m is BaseCreature))
			{
				return true;
			}

            if (c is Server.Engines.Quests.Haven.MilitiaFighter)
			{
				return true;
			}

			BaseCreature t = this;

			// Summons should have same rules as their master
			if (c.m_bSummoned && c.SummonMaster != null && c.SummonMaster is BaseCreature)
			{
				c = c.SummonMaster as BaseCreature;
			}

			// Summons should have same rules as their master
			if (t.m_bSummoned && t.SummonMaster != null && t.SummonMaster is BaseCreature)
			{
				t = t.SummonMaster as BaseCreature;
			}

			// Creatures on other teams are my enemies
			if (t.m_iTeam != c.m_iTeam)
			{
				return true;
			}
/*
			// Creatures attacking me are my enemies
			if (c.Combatant == this)
			{
				return true;
			}
*/
			// If I'm summoned/controlled and they aren't summoned/controlled, they are my enemy
			// If I'm not summoned/controlled and they are summoned/controlled, they are my enemy
            // Summoned creatures must have masters to count as summoned here
			return (((t.m_bSummoned && t.SummonMaster != null) || t.m_bControlled) !=
                ((c.m_bSummoned && c.SummonMaster != null) || c.m_bControlled));
		}

        public override string ApplyNameSuffix(string suffix)
        {
            XmlData customtitle = (XmlData)XmlAttach.FindAttachment(this, typeof(XmlData), "ParagonTitle");

            if (customtitle != null)
            {
                suffix = customtitle.Data;
            }
            else if (IsParagon && !GivesMLMinorArtifact)
            {
                if (suffix.Length == 0)
                {
                    suffix = XmlParagon.GetParagonLabel(this);
                }
                else
                {
                    suffix = String.Concat(suffix, " " + XmlParagon.GetParagonLabel(this));
                }

                XmlAttach.AttachTo(this, new XmlData("ParagonTitle", suffix));
            }

            return base.ApplyNameSuffix(suffix);
        }

        public virtual bool CheckControlChance(Mobile m)
        {
            if (GetControlChance(m) > Utility.RandomDouble())
            {
                Loyalty += 1;
                return true;
            }

            PlaySound(GetAngerSound());

            if (Core.SA)
            {
                Animate(AnimationType.Alert, 0);
            }
            else
            {
                Animate(Body.IsAnimal ? 10 : 18, 5, 1, true, false, 0);
            }

            Loyalty -= 3;
            return false;
        }

        public virtual bool CanBeControlledBy(Mobile m)
        {
            return (GetControlChance(m) > 0.0);
        }

        public double GetControlChance(Mobile m)
        {
            return GetControlChance(m, false);
        }

        public virtual double GetControlChance(Mobile m, bool useBaseSkill)
        {
            if (m_CurrentTameSkill <= 29.1 || m_bSummoned || m.AccessLevel >= AccessLevel.GameMaster)
            {
                return 1.0;
            }

            double dMinTameSkill = m_CurrentTameSkill;

            if (dMinTameSkill > -24.9 && AnimalTaming.CheckMastery(m, this))
            {
                dMinTameSkill = -24.9;
            }

            int taming =
                (int)((useBaseSkill ? m.Skills[SkillName.AnimalTaming].Base : m.Skills[SkillName.AnimalTaming].Value) * 10);
            int lore = (int)((useBaseSkill ? m.Skills[SkillName.AnimalLore].Base : m.Skills[SkillName.AnimalLore].Value) * 10);
            int bonus = 0, chance = 700;

            if (Core.ML)
            {
                int SkillBonus = taming - (int)(dMinTameSkill * 10);
                int LoreBonus = lore - (int)(dMinTameSkill * 10);

                int SkillMod = 6, LoreMod = 6;

                if (SkillBonus < 0)
                {
                    SkillMod = 28;
                }
                if (LoreBonus < 0)
                {
                    LoreMod = 14;
                }

                SkillBonus *= SkillMod;
                LoreBonus *= LoreMod;

                bonus = (SkillBonus + LoreBonus) / 2;
            }
            else
            {
                int difficulty = (int)(dMinTameSkill * 10);
                int weighted = ((taming * 4) + lore) / 5;
                bonus = weighted - difficulty;

                if (bonus <= 0)
                {
                    bonus *= 14;
                }
                else
                {
                    bonus *= 6;
                }
            }

            chance += bonus;

            if (chance >= 0 && chance < 200)
            {
                chance = 200;
            }
            else if (chance > 990)
            {
                chance = 990;
            }

            chance -= (MaxLoyalty - m_Loyalty) * 10;

            chance += (int)XmlMobFactions.GetScaledFaction(m, this, -250, 250, 0.001);

            return ((double)chance / 1000);
        }

        public virtual bool CanTransfer(Mobile m)
        {
            return !Allured;
        }

        public virtual bool CanFriend(Mobile m)
        {
            return true;
        }

        private static readonly Type[] m_AnimateDeadTypes = new[]
        {
            typeof(MoundOfMaggots), typeof(HellSteed), typeof(SkeletalMount), typeof(WailingBanshee), typeof(Wraith),
            typeof(SkeletalDragon), typeof(LichLord), typeof(FleshGolem), typeof(Lich), typeof(SkeletalKnight),
            typeof(BoneKnight), typeof(Mummy), typeof(SkeletalMage), typeof(BoneMagi), typeof(PatchworkSkeleton)
        };

        public virtual bool IsAnimatedDead
        {
            get
            {
                if (!Summoned)
                {
                    return false;
                }

                Type type = GetType();

                bool contains = false;

                for (int i = 0; !contains && i < m_AnimateDeadTypes.Length; ++i)
                {
                    contains = (type == m_AnimateDeadTypes[i]);
                }

                return contains;
            }
        }

        public virtual bool IsNecroFamiliar
        {
            get
            {
                if (!Summoned)
                {
                    return false;
                }

                if (m_ControlMaster != null && SummonFamiliarSpell.Table.Contains(m_ControlMaster))
                {
                    return SummonFamiliarSpell.Table[m_ControlMaster] == this;
                }

                return false;
            }
        }

        public override void Damage(int amount, Mobile from)
        {
            Damage(amount, from, false, false);
        }

        public override void Damage(int amount, Mobile from, bool informMount)
        {
            Damage(amount, from, informMount, false);
        }

        public override void Damage(int amount, Mobile from, bool informMount, bool checkDisrupt)
        {
            int oldHits = Hits;

            if (Core.AOS && Controlled && from is BaseCreature && !((BaseCreature)from).Controlled && !((BaseCreature)from).Summoned)
                amount = (int)(amount * ((BaseCreature)from).BonusPetDamageScalar);

            base.Damage(amount, from, informMount, checkDisrupt);

            if (SubdueBeforeTame && !Controlled)
            {
                if ((oldHits > ((double)HitsMax / 10)) && ((double)Hits <= ((double)HitsMax / 10)))
                {
                    PublicOverheadMessage(MessageType.Regular, 0x3B2, false, "* The creature has been beaten into subjugation! *");
                }
            }
        }

        public virtual bool DeleteCorpseOnDeath { get { return !Core.AOS && m_bSummoned; } }

        public override void SetLocation(Point3D newLocation, bool isTeleport)
        {
            base.SetLocation(newLocation, isTeleport);

            if (isTeleport && m_AI != null)
            {
                m_AI.OnTeleported();
            }
        }

        public override void OnBeforeSpawn(Point3D location, Map m)
        {
            if (XmlParagon.CheckConvert(this, location, m))
            {
                IsParagon = true;
            }

            base.OnBeforeSpawn(location, m);
        }

        public override ApplyPoisonResult ApplyPoison(Mobile from, Poison poison)
        {
            if (!Alive || IsDeadPet)
            {
                return ApplyPoisonResult.Immune;
            }

            if (EvilOmenSpell.TryEndEffect(this))
            {
                poison = PoisonImpl.IncreaseLevel(poison);
            }

            ApplyPoisonResult result = base.ApplyPoison(from, poison);

            if (from != null && result == ApplyPoisonResult.Poisoned && PoisonTimer is PoisonImpl.PoisonTimer)
            {
                (PoisonTimer as PoisonImpl.PoisonTimer).From = from;
            }

            return result;
        }

        public override bool CheckPoisonImmunity(Mobile from, Poison poison)
        {
            if (base.CheckPoisonImmunity(from, poison))
            {
                return true;
            }

            Poison p = PoisonImmune;

            XmlPoison xp = (XmlPoison)XmlAttach.FindAttachment(this, typeof(XmlPoison));

            if (xp != null)
            {
                p = xp.PoisonImmune;
            }

            return (p != null && p.RealLevel >= poison.RealLevel);
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Loyalty { get { return m_Loyalty; } set { m_Loyalty = Math.Min(Math.Max(value, 0), MaxLoyalty); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public WayPoint CurrentWayPoint { get { return m_CurrentWayPoint; } set { m_CurrentWayPoint = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CurrentNavPoint
        {
            get
            {
                return _CurrentNavPoint;
            }
            set
            {
                _CurrentNavPoint = value;
            }
        }

        public Dictionary<Map, List<Point2D>> NavPoints
        {
            get
            {
                if(_NavPoints == null)
                    _NavPoints = new Dictionary<Map, List<Point2D>>();

                return _NavPoints;
            }
            set
            {
                _NavPoints = value;
            }
        }

        public List<Point2D> CurrentNavPoints
        {
            get
            {
                if (Map != null && _NavPoints.ContainsKey(Map))
                    return _NavPoints[Map];

                return null;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public IPoint2D TargetLocation { get { return m_TargetLocation; } set { m_TargetLocation = value; } }

        public virtual Mobile ConstantFocus { get { return null; } }

        public virtual bool DisallowAllMoves
        {
            get
            {
                XmlData x = (XmlData)XmlAttach.FindAttachment(this, typeof(XmlData), "NoSpecials");

                return x != null && x.Data == "True";
            }
        }

        public virtual bool InitialInnocent
        {
            get
            {
                XmlData x = (XmlData)XmlAttach.FindAttachment(this, typeof(XmlData), "Notoriety");

                return x != null && x.Data == "blue";
            }
        }

        public virtual bool AlwaysMurderer
        {
            get
            {
                XmlData x = (XmlData)XmlAttach.FindAttachment(this, typeof(XmlData), "Notoriety");

                return x != null && x.Data == "red";
            }
        }

        public virtual bool AlwaysAttackable
        {
            get
            {
                XmlData x = (XmlData)XmlAttach.FindAttachment(this, typeof(XmlData), "Notoriety");

                return x != null && x.Data == "gray";
            }
        }

        public virtual bool ForceNotoriety
        {
            get
            {
                return false;
            }
        }

        public virtual bool HoldSmartSpawning { get { return IsParagon; } }
        public virtual bool UseSmartAI { get { return false; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int DamageMin { get { return m_DamageMin; } set { m_DamageMin = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int DamageMax { get { return m_DamageMax; } set { m_DamageMax = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public override int HitsMax
        {
            get
            {
                if (m_HitsMax > 0)
                {
                    int value = m_HitsMax + GetStatOffset(StatType.Str);

                    if (value < 1)
                    {
                        value = 1;
                    }
                    else if (value > 1000000)
                    {
                        value = 1000000;
                    }

                    return value;
                }

                return Str;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int HitsMaxSeed { get { return m_HitsMax; } set { m_HitsMax = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public override int StamMax
        {
            get
            {
                if (m_StamMax > 0)
                {
                    int value = m_StamMax + GetStatOffset(StatType.Dex);

                    if (value < 1)
                    {
                        value = 1;
                    }
                    else if (value > 1000000)
                    {
                        value = 1000000;
                    }

                    return value;
                }

                return Dex;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int StamMaxSeed { get { return m_StamMax; } set { m_StamMax = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public override int ManaMax
        {
            get
            {
                if (m_ManaMax > 0)
                {
                    int value = m_ManaMax + GetStatOffset(StatType.Int);

                    if (value < 1)
                    {
                        value = 1;
                    }
                    else if (value > 1000000)
                    {
                        value = 1000000;
                    }

                    return value;
                }

                return Int;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ManaMaxSeed { get { return m_ManaMax; } set { m_ManaMax = value; } }

        public virtual bool CanOpenDoors { get { return !Body.IsAnimal && !Body.IsSea; } }

        public virtual bool CanMoveOverObstacles { get { return Core.AOS || Body.IsMonster; } }

        public virtual bool CanDestroyObstacles
        {
            get
            {
                // to enable breaking of furniture, 'return CanMoveOverObstacles;'
                return false;
            }
        }

        public void Unpacify()
        {
            BardEndTime = DateTime.UtcNow;
            BardPacified = false;
        }

        private HonorContext m_ReceivedHonorContext;

        public HonorContext ReceivedHonorContext { get { return m_ReceivedHonorContext; } set { m_ReceivedHonorContext = value; } }

        /*

        Seems this actually was removed on OSI somewhere between the original bug report and now.
        We will call it ML, until we can get better information. I suspect it was on the OSI TC when
        originally it taken out of RunUO, and not implmented on OSIs production shards until more
        recently.  Either way, this is, or was, accurate OSI behavior, and just entirely
        removing it was incorrect.  OSI followers were distracted by being attacked well into
        AoS, at very least.

        */

        public virtual bool CanBeDistracted { get { return !Core.ML; } }

        public virtual void CheckDistracted(Mobile from)
        {
            if (Utility.RandomDouble() < .10)
            {
                ControlTarget = from;
                ControlOrder = OrderType.Attack;
                Combatant = from;
                Warmode = true;
            }
        }

        public virtual void OnBeforeDamage(Mobile from, ref int totalDamage, DamageType type)
        {
            if (type >= DamageType.Spell && RecentSetControl)
            {
                totalDamage = 0;
            }
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            if (BardPacified && (HitsMax - Hits) * 0.001 > Utility.RandomDouble())
            {
                Unpacify();
            }

            int disruptThreshold;
            //NPCs can use bandages too!
            if (!Core.AOS)
            {
                disruptThreshold = 0;
            }
            else if (from != null && from.Player)
            {
                disruptThreshold = 18;
            }
            else
            {
                disruptThreshold = 25;
            }

            if (amount > disruptThreshold)
            {
                BandageContext c = BandageContext.GetContext(this);

                if (c != null)
                {
                    c.Slip();
                }
            }

            if (Confidence.IsRegenerating(this))
            {
                Confidence.StopRegenerating(this);
            }

            WeightOverloading.FatigueOnDamage(this, amount);

            InhumanSpeech speechType = SpeechType;

            if (speechType != null && !willKill)
            {
                speechType.OnDamage(this, amount);
            }

            if (m_ReceivedHonorContext != null)
            {
                m_ReceivedHonorContext.OnTargetDamaged(from, amount);
            }

            if (!willKill)
            {
                if (CanBeDistracted && ControlOrder == OrderType.Follow)
                {
                    CheckDistracted(from);
                }
            }
            else if (from is PlayerMobile)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(10), ((PlayerMobile)@from).RecoverAmmo);
            }

            #region XmlSpawner
            if (!Summoned && willKill && from != null)
            {
                LevelItemManager.CheckItems(from, this);
            }
            #endregion

            base.OnDamage(amount, from, willKill);
        }

        public virtual void OnDamagedBySpell(Mobile from)
        {
            if (CanBeDistracted && ControlOrder == OrderType.Follow)
            {
                CheckDistracted(from);
            }
        }

        public virtual void OnHarmfulSpell(Mobile from)
        { }

        #region Alter[...]Damage From/To
        public virtual void AlterDamageScalarFrom(Mobile caster, ref double scalar)
        { }

        public virtual void AlterDamageScalarTo(Mobile target, ref double scalar)
        { }

        public virtual void AlterSpellDamageFrom(Mobile from, ref int damage)
        {
            if (m_TempDamageAbsorb > 0 && VialofArmorEssence.UnderInfluence(this))
                damage -= damage / m_TempDamageAbsorb;
        }

        public virtual void AlterSpellDamageTo(Mobile to, ref int damage)
        { }

        public virtual void AlterMeleeDamageFrom(Mobile from, ref int damage)
        {
            #region Mondain's Legacy
            if (from != null && from.Talisman is BaseTalisman)
            {
                BaseTalisman talisman = (BaseTalisman)from.Talisman;

                if (talisman.Killer != null && talisman.Killer.Type != null)
                {
                    Type type = talisman.Killer.Type;

                    if (type.IsAssignableFrom(GetType()))
                    {
                        damage = (int)(damage * (1 + (double)talisman.Killer.Amount / 100));
                    }
                }
            }
            #endregion

            if (m_TempDamageAbsorb > 0 && VialofArmorEssence.UnderInfluence(this))
                damage -= damage / m_TempDamageAbsorb;
        }

        public virtual void AlterMeleeDamageTo(Mobile to, ref int damage)
        {
            if (m_TempDamageBonus > 0 && TastyTreat.UnderInfluence(this))
                damage += damage / m_TempDamageBonus;
        }
        #endregion

        #region SA / High Seas Tasty Treats/Vial of Armor Essense
        private int m_TempDamageBonus = 0;
        private int m_TempDamageAbsorb = 0;

        public int TempDamageBonus { get { return m_TempDamageBonus; } set { m_TempDamageBonus = value; } }
        public int TempDamageAbsorb { get { return m_TempDamageAbsorb; } set { m_TempDamageAbsorb = value; } }
        #endregion

        public virtual void CheckReflect(Mobile caster, ref bool reflect)
        { }

        public virtual void OnCarve(Mobile from, Corpse corpse, Item with)
        {
            int feathers = Feathers;
            int wool = Wool;
            int meat = Meat;
            int hides = Hides;
            int scales = Scales;
            int dragonblood = DragonBlood;

            bool special = with is SkinningKnife || with is ButchersWarCleaver || with is HarvestersBlade;

            if ((feathers == 0 && wool == 0 && meat == 0 && hides == 0 && scales == 0) || Summoned || IsBonded || corpse.Animated)
            {
                if (corpse.Animated)
                {
                    corpse.SendLocalizedMessageTo(from, 500464); // Use this on corpses to carve away meat and hide
                }
                else
                {
                    from.SendLocalizedMessage(500485); // You see nothing useful to carve from the corpse.
                }
            }
            else
            {
                if (Core.ML && from.Race == Race.Human)
                {
                    hides = (int)Math.Ceiling(hides * 1.1); // 10% bonus only applies to hides, ore & logs
                }

                if (corpse.Map == Map.Felucca && !Siege.SiegeShard)
                {
                    feathers *= 2;
                    wool *= 2;
                    hides *= 2;

                    if (Core.ML)
                    {
                        meat *= 2;
                        scales *= 2;
                    }
                }

                if (with is HarvestersBlade)
                {
                    feathers = (int)Math.Ceiling((double)feathers * 1.1);
                    wool = (int)Math.Ceiling((double)wool * 1.1);
                    hides = (int)Math.Ceiling((double)hides * 1.1);
                    meat = (int)Math.Ceiling((double)meat * 1.1);
                    scales = (int)Math.Ceiling((double)scales * 1.1);
                }

                new Blood(0x122D).MoveToWorld(corpse.Location, corpse.Map);

                if (feathers != 0)
                {
                    Item feather = new Feather(feathers);

                    if (!Core.AOS || !special || !from.AddToBackpack(feather))
                    {
                        corpse.AddCarvedItem(feather, from);
                        from.SendLocalizedMessage(500479); // You pluck the bird. The feathers are now on the corpse.
                    }
                    else
                    {
                        from.SendLocalizedMessage(1114097); // You pluck the bird and place the feathers in your backpack.
                    }
                }

                if (wool != 0)
                {
                    Item w = new TaintedWool(wool);

                    if (!Core.AOS || !special || !from.AddToBackpack(w))
                    {
                        corpse.AddCarvedItem(w, from);
                        from.SendLocalizedMessage(500483); // You shear it, and the wool is now on the corpse.
                    }
                    else
                    {
                        from.SendLocalizedMessage(1114099); // You shear the creature and put the resources in your backpack.
                    }
                }

                if (meat != 0)
                {
                    Item m = null;

                    switch (MeatType)
                    {
                        default:
                        case MeatType.Ribs: m = new RawRibs(meat); break;
                        case MeatType.Bird: m = new RawBird(meat); break;
                        case MeatType.LambLeg: m = new RawLambLeg(meat); break;
                        case MeatType.Rotworm: m = new RawRotwormMeat(meat); break;
                    }

                    if (!Core.AOS || !special || !from.AddToBackpack(m))
                    {
                        corpse.AddCarvedItem(m, from);
                        from.SendLocalizedMessage(500467); // You carve some meat, which remains on the corpse.
                    }
                    else
                    {
                        from.SendLocalizedMessage(1114101); // You carve some meat and put it in your backpack.
                    }
                }

                if (hides != 0)
                {
                    Item leather = null;

                    switch (HideType)
                    {
                        default:
                        case HideType.Regular: leather = new Leather(hides); break;
                        case HideType.Spined: leather = new SpinedLeather(hides); break;
                        case HideType.Horned: leather = new HornedLeather(hides); break;
                        case HideType.Barbed: leather = new BarbedLeather(hides); break;
                    }

                    if (!Core.AOS || !special || !from.AddToBackpack(leather))
                    {
                        corpse.AddCarvedItem(leather, from);
                        from.SendLocalizedMessage(500471); // You skin it, and the hides are now in the corpse.
                    }
                    else
                    {
                        from.SendLocalizedMessage(1073555); // You skin it and place the cut-up hides in your backpack.
                    }
                }

                if (scales != 0)
                {
                    ScaleType sc = ScaleType;
                    List<Item> list = new List<Item>();

                    switch (sc)
                    {
                        default:
                        case ScaleType.Red: list.Add(new RedScales(scales)); break;
                        case ScaleType.Yellow: list.Add(new YellowScales(scales)); break;
                        case ScaleType.Black: list.Add(new BlackScales(scales)); break;
                        case ScaleType.Green: list.Add(new GreenScales(scales)); break;
                        case ScaleType.White: list.Add(new WhiteScales(scales)); break;
                        case ScaleType.Blue: list.Add(new BlueScales(scales)); break;
                        case ScaleType.All:
                            {
                                list.Add(new RedScales(scales));
                                list.Add(new YellowScales(scales));
                                list.Add(new BlackScales(scales));
                                list.Add(new GreenScales(scales));
                                list.Add(new WhiteScales(scales));
                                list.Add(new BlueScales(scales));
                                break;
                            }
                    }

                    if (Core.AOS && special)
                    {
                        bool allPack = true;
                        bool anyPack = false;

                        foreach (Item s in list)
                        {
                            //corpse.AddCarvedItem(s, from);
                            if (!from.PlaceInBackpack(s))
                            {
                                corpse.AddCarvedItem(s, from);
                                allPack = false;
                            }
                            else if (!anyPack)
                            {
                                anyPack = true;
                            }
                        }

                        if (anyPack)
                            from.SendLocalizedMessage(1114098); // You cut away some scales and put them in your backpack.

                        if(!allPack)
                            from.SendLocalizedMessage(1079284); // You cut away some scales, but they remain on the corpse.
                    }
                    else
                    {
                        foreach (Item s in list)
                        {
                            corpse.AddCarvedItem(s, from);
                        }

                        from.SendLocalizedMessage(1079284); // You cut away some scales, but they remain on the corpse.
                    }

                    ColUtility.Free(list);
                }

                if (dragonblood != 0)
                {
                    Item dblood = new DragonBlood(dragonblood);

                    if (!Core.AOS || !special || !from.AddToBackpack(dblood))
                    {
                        corpse.AddCarvedItem(dblood, from);
                        from.SendLocalizedMessage(1094946); // Some blood is left on the corpse.
                    }
                    else
                    {
                        from.SendLocalizedMessage(1114100); // You take some blood off the corpse and put it in your backpack.
                    }
                }

                corpse.Carved = true;

                if (corpse.IsCriminalAction(from))
                {
                    from.CriminalAction(true);
                }
            }
        }

        public const int DefaultRangePerception = 16;
        public const int OldRangePerception = 10;

        public BaseCreature(
            AIType ai, FightMode mode, int iRangePerception, int iRangeFight, double dActiveSpeed, double dPassiveSpeed)
        {
            if (iRangePerception == OldRangePerception)
            {
                iRangePerception = DefaultRangePerception;
            }

            m_Loyalty = MaxLoyalty; // Wonderfully Happy

            m_CurrentAI = ai;
            m_DefaultAI = ai;

            m_iRangePerception = iRangePerception;
            m_iRangeFight = iRangeFight;

            m_FightMode = mode;

            m_iTeam = 0;

            SpeedInfo.GetSpeeds(this, ref dActiveSpeed, ref dPassiveSpeed);

            m_dActiveSpeed = dActiveSpeed;
            m_dPassiveSpeed = dPassiveSpeed;
            m_dCurrentSpeed = dPassiveSpeed;

            m_bDebugAI = false;

            m_arSpellAttack = new List<Type>();
            m_arSpellDefense = new List<Type>();

            m_bControlled = false;
            m_ControlMaster = null;
            m_ControlTarget = null;
            m_ControlOrder = OrderType.None;

            m_bTamable = false;

            m_Owners = new List<Mobile>();

            m_NextReacquireTime = Core.TickCount + (int)ReacquireDelay.TotalMilliseconds;

            ChangeAIType(AI);

            InhumanSpeech speechType = SpeechType;

            if (speechType != null)
            {
                speechType.OnConstruct(this);
            }

            if (IsInvulnerable && !Core.AOS)
            {
                NameHue = 0x35;
            }

            InitializeAbilities();

            Timer.DelayCall(() =>GenerateLoot(true));
        }

        public BaseCreature(Serial serial)
            : base(serial)
        {
            m_arSpellAttack = new List<Type>();
            m_arSpellDefense = new List<Type>();

            m_bDebugAI = false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(25); // version

            writer.Write((int)m_CurrentAI);
            writer.Write((int)m_DefaultAI);

            writer.Write(m_iRangePerception);
            writer.Write(m_iRangeFight);

            writer.Write(m_iTeam);

            writer.Write(m_dActiveSpeed);
            writer.Write(m_dPassiveSpeed);
            writer.Write(m_dCurrentSpeed);

            writer.Write(m_pHome.X);
            writer.Write(m_pHome.Y);
            writer.Write(m_pHome.Z);

            // Version 1
            writer.Write(m_iRangeHome);

            int i = 0;

            writer.Write(m_arSpellAttack.Count);
            for (i = 0; i < m_arSpellAttack.Count; i++)
            {
                writer.Write(m_arSpellAttack[i].ToString());
            }

            writer.Write(m_arSpellDefense.Count);
            for (i = 0; i < m_arSpellDefense.Count; i++)
            {
                writer.Write(m_arSpellDefense[i].ToString());
            }

            // Version 2
            writer.Write((int)m_FightMode);

            writer.Write(m_bControlled);
            writer.Write(m_ControlMaster);
            writer.Write(m_ControlTarget is Mobile ? (Mobile)m_ControlTarget : null);
            writer.Write(m_ControlDest);
            writer.Write((int)m_ControlOrder);
            writer.Write(m_dMinTameSkill);
            // Removed in version 9
            //writer.Write( (double) m_dMaxTameSkill );
            writer.Write(m_bTamable);
            writer.Write(m_bSummoned);

            if (m_bSummoned)
            {
                writer.WriteDeltaTime(m_SummonEnd);
            }

            writer.Write(m_iControlSlots);

            // Version 3
            writer.Write(m_Loyalty);

            // Version 4
            writer.Write(m_CurrentWayPoint);

            // Verison 5
            writer.Write(m_SummonMaster);

            // Version 6
            writer.Write(m_HitsMax);
            writer.Write(m_StamMax);
            writer.Write(m_ManaMax);
            writer.Write(m_DamageMin);
            writer.Write(m_DamageMax);

            // Version 7
            writer.Write(m_PhysicalResistance);
            writer.Write(m_PhysicalDamage);

            writer.Write(m_FireResistance);
            writer.Write(m_FireDamage);

            writer.Write(m_ColdResistance);
            writer.Write(m_ColdDamage);

            writer.Write(m_PoisonResistance);
            writer.Write(m_PoisonDamage);

            writer.Write(m_EnergyResistance);
            writer.Write(m_EnergyDamage);

            // Version 8
            writer.Write(m_Owners, true);

            // Version 10
            writer.Write(m_IsDeadPet);
            writer.Write(m_IsBonded);
            writer.Write(m_BondingBegin);
            writer.Write(m_OwnerAbandonTime);

            // Version 11
            writer.Write(m_HasGeneratedLoot);

            // Version 12
            writer.Write(m_Paragon);

            // Version 13
            writer.Write((m_Friends != null && m_Friends.Count > 0));

            if (m_Friends != null && m_Friends.Count > 0)
            {
                writer.Write(m_Friends, true);
            }

            // Version 14
            writer.Write(m_RemoveIfUntamed);
            writer.Write(m_RemoveStep);

            // Version 17
            if (IsStabled || (Controlled && ControlMaster != null))
            {
                writer.Write(TimeSpan.Zero);
            }
            else
            {
                writer.Write(DeleteTimeLeft);
            }

            // Version 18
            writer.Write(m_CorpseNameOverride);

            // Mondain's Legacy version 19
            writer.Write(m_Allured);

            // Pet Branding version 22
            writer.Write(m_EngravedText);

            // Version 24 Pet Training
            writer.Write(ControlSlotsMin);
            writer.Write(ControlSlotsMax);

            writer.Write((int)Mastery);

            if (_Profile != null)
            {
                writer.Write(1);
                _Profile.Serialize(writer);
            }
            else
            {
                writer.Write(0);
            }

            if (_TrainingProfile != null)
            {
                writer.Write(1);
                _TrainingProfile.Serialize(writer);
            }
            else
            {
                writer.Write(0);
            }

            // Version 25 Current Tame Skill
            writer.Write(m_CurrentTameSkill);
        }

        private static readonly double[] m_StandardActiveSpeeds = new[] { 0.175, 0.1, 0.15, 0.2, 0.25, 0.3, 0.4, 0.5, 0.6, 0.8 };

        private static readonly double[] m_StandardPassiveSpeeds = new[] { 0.350, 0.2, 0.4, 0.5, 0.6, 0.8, 1.0, 1.2, 1.6, 2.0 };

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_CurrentAI = (AIType)reader.ReadInt();
            m_DefaultAI = (AIType)reader.ReadInt();

            m_iRangePerception = reader.ReadInt();
            m_iRangeFight = reader.ReadInt();

            m_iTeam = reader.ReadInt();

            m_dActiveSpeed = reader.ReadDouble();
            m_dPassiveSpeed = reader.ReadDouble();
            m_dCurrentSpeed = reader.ReadDouble();

            if (m_iRangePerception == OldRangePerception)
            {
                m_iRangePerception = DefaultRangePerception;
            }

            m_pHome.X = reader.ReadInt();
            m_pHome.Y = reader.ReadInt();
            m_pHome.Z = reader.ReadInt();

            if (version >= 1)
            {
                m_iRangeHome = reader.ReadInt();

                int i, iCount;

                iCount = reader.ReadInt();
                for (i = 0; i < iCount; i++)
                {
                    string str = reader.ReadString();
                    Type type = Type.GetType(str);

                    if (type != null)
                    {
                        m_arSpellAttack.Add(type);
                    }
                }

                iCount = reader.ReadInt();
                for (i = 0; i < iCount; i++)
                {
                    string str = reader.ReadString();
                    Type type = Type.GetType(str);

                    if (type != null)
                    {
                        m_arSpellDefense.Add(type);
                    }
                }
            }
            else
            {
                m_iRangeHome = 0;
            }

            if (version >= 2)
            {
                m_FightMode = (FightMode)reader.ReadInt();

                m_bControlled = reader.ReadBool();
                m_ControlMaster = reader.ReadMobile();
                m_ControlTarget = reader.ReadMobile();
                m_ControlDest = reader.ReadPoint3D();
                m_ControlOrder = (OrderType)reader.ReadInt();

                m_dMinTameSkill = reader.ReadDouble();

                if (version < 9)
                {
                    reader.ReadDouble();
                }

                m_bTamable = reader.ReadBool();
                m_bSummoned = reader.ReadBool();

                if (m_bSummoned)
                {
                    m_SummonEnd = reader.ReadDeltaTime();
                    new UnsummonTimer(m_ControlMaster, this, m_SummonEnd - DateTime.UtcNow).Start();
                }

                m_iControlSlots = reader.ReadInt();
            }
            else
            {
                m_FightMode = FightMode.Closest;

                m_bControlled = false;
                m_ControlMaster = null;
                m_ControlTarget = null;
                m_ControlOrder = OrderType.None;
            }

            if (version >= 3)
            {
                m_Loyalty = reader.ReadInt();
            }
            else
            {
                m_Loyalty = MaxLoyalty; // Wonderfully Happy
            }

            if (version >= 4)
            {
                m_CurrentWayPoint = reader.ReadItem() as WayPoint;
            }

            if (version >= 5)
            {
                m_SummonMaster = reader.ReadMobile();
            }

            if (version >= 6)
            {
                m_HitsMax = reader.ReadInt();
                m_StamMax = reader.ReadInt();
                m_ManaMax = reader.ReadInt();
                m_DamageMin = reader.ReadInt();
                m_DamageMax = reader.ReadInt();
            }

            if (version >= 7)
            {
                m_PhysicalResistance = reader.ReadInt();
                m_PhysicalDamage = reader.ReadInt();

                m_FireResistance = reader.ReadInt();
                m_FireDamage = reader.ReadInt();

                m_ColdResistance = reader.ReadInt();
                m_ColdDamage = reader.ReadInt();

                m_PoisonResistance = reader.ReadInt();
                m_PoisonDamage = reader.ReadInt();

                m_EnergyResistance = reader.ReadInt();
                m_EnergyDamage = reader.ReadInt();
            }

            if (version >= 8)
            {
                m_Owners = reader.ReadStrongMobileList();
            }
            else
            {
                m_Owners = new List<Mobile>();
            }

            if (version >= 10)
            {
                m_IsDeadPet = reader.ReadBool();
                m_IsBonded = reader.ReadBool();
                m_BondingBegin = reader.ReadDateTime();
                m_OwnerAbandonTime = reader.ReadDateTime();
            }

            if (version >= 11)
            {
                m_HasGeneratedLoot = reader.ReadBool();
            }
            else
            {
                m_HasGeneratedLoot = true;
            }

            if (version >= 12)
            {
                m_Paragon = reader.ReadBool();
            }
            else
            {
                m_Paragon = false;
            }

            if (version >= 13 && reader.ReadBool())
            {
                m_Friends = reader.ReadStrongMobileList();
            }
            else if (version < 13 && m_ControlOrder >= OrderType.Unfriend)
            {
                ++m_ControlOrder;
            }

            if (version < 16 && Loyalty != MaxLoyalty)
            {
                Loyalty *= 10;
            }

            double activeSpeed = m_dActiveSpeed;
            double passiveSpeed = m_dPassiveSpeed;

            SpeedInfo.GetSpeeds(this, ref activeSpeed, ref passiveSpeed);

            bool isStandardActive = false;
            for (int i = 0; !isStandardActive && i < m_StandardActiveSpeeds.Length; ++i)
            {
                isStandardActive = (m_dActiveSpeed == m_StandardActiveSpeeds[i]);
            }

            bool isStandardPassive = false;
            for (int i = 0; !isStandardPassive && i < m_StandardPassiveSpeeds.Length; ++i)
            {
                isStandardPassive = (m_dPassiveSpeed == m_StandardPassiveSpeeds[i]);
            }

            if (isStandardActive && m_dCurrentSpeed == m_dActiveSpeed)
            {
                m_dCurrentSpeed = activeSpeed;
            }
            else if (isStandardPassive && m_dCurrentSpeed == m_dPassiveSpeed)
            {
                m_dCurrentSpeed = passiveSpeed;
            }

            if (isStandardActive && !m_Paragon)
            {
                m_dActiveSpeed = activeSpeed;
            }

            if (isStandardPassive && !m_Paragon)
            {
                m_dPassiveSpeed = passiveSpeed;
            }

            if (version >= 14)
            {
                m_RemoveIfUntamed = reader.ReadBool();
                m_RemoveStep = reader.ReadInt();
            }

            TimeSpan deleteTime = TimeSpan.Zero;

            if (version >= 17)
            {
                deleteTime = reader.ReadTimeSpan();
            }

            if (deleteTime > TimeSpan.Zero || LastOwner != null && !Controlled && !IsStabled)
            {
                if (deleteTime == TimeSpan.Zero)
                {
                    deleteTime = TimeSpan.FromDays(3.0);
                }

                m_DeleteTimer = new DeleteTimer(this, deleteTime);
                m_DeleteTimer.Start();
            }

            if (version >= 18)
            {
                m_CorpseNameOverride = reader.ReadString();
            }

            if (version >= 19)
            {
                m_Allured = reader.ReadBool();
            }

            if (version <= 20)
            {
                reader.ReadInt();
            }

            if (version >= 22)
            {
                m_EngravedText = reader.ReadString();
            }

            if (version == 23)
            {
                reader.ReadBool();
            }

            if (version >= 24)
            {
                ControlSlotsMin = reader.ReadInt();
                ControlSlotsMax = reader.ReadInt();

                Mastery = (SkillName)reader.ReadInt();

                if (reader.ReadInt() == 1)
                {
                    _Profile = new AbilityProfile(this, reader);
                }

                if (reader.ReadInt() == 1)
                {
                    _TrainingProfile = new TrainingProfile(this, reader);
                }
            }
            else
            {
                if (Tamable)
                {
                    CalculateSlots(m_iControlSlots);

                    if (m_iControlSlots < ControlSlotsMin)
                    {
                        ControlSlotsMin = m_iControlSlots;
                    }

                    ControlSlots = ControlSlotsMin;
                }

                InitializeAbilities();
            }

            if (version >= 25)
            {
                CurrentTameSkill = reader.ReadDouble();
            }
            else
            {
                AdjustTameRequirements();
            }

            if (version <= 14 && m_Paragon && Hue == 0x31)
            {
                Hue = Paragon.Hue; //Paragon hue fixed, should now be 0x501.
            }

            if (Core.AOS && NameHue == 0x35)
            {
                NameHue = -1;
            }

            CheckStatTimers();

            ChangeAIType(m_CurrentAI);

            AddFollowers();

            if (IsAnimatedDead)
            {
                AnimateDeadSpell.Register(m_SummonMaster, this);
            }

            if (Tamable && CurrentTameSkill == 0)
            {
                AdjustTameRequirements();
            }
        }

        public virtual bool IsHumanInTown()
        {
            return (Body.IsHuman && Region.IsPartOf<GuardedRegion>());
        }

        public virtual bool CheckGold(Mobile from, Item dropped)
        {
            if (dropped is Gold)
            {
                return OnGoldGiven(from, (Gold)dropped);
            }

            return false;
        }

        public virtual bool OnGoldGiven(Mobile from, Gold dropped)
        {
            if (CheckTeachingMatch(from))
            {
                if (Teach(m_Teaching, from, dropped.Amount, true))
                {
                    dropped.Delete();
                    return true;
                }
            }
            else if (IsHumanInTown())
            {
                Direction = GetDirectionTo(from);

                int oldSpeechHue = SpeechHue;

                SpeechHue = 0x23F;
                SayTo(from, "Thou art giving me gold?");

                if (dropped.Amount >= 400)
                {
                    SayTo(from, "'Tis a noble gift.");
                }
                else
                {
                    SayTo(from, "Money is always welcome.");
                }

                SpeechHue = 0x3B2;
                SayTo(from, 501548); // I thank thee.

                SpeechHue = oldSpeechHue;

                dropped.Delete();
                return true;
            }

            return false;
        }

        public override bool ShouldCheckStatTimers { get { return false; } }

        #region Food
        private static readonly Type[] m_Eggs = new[] { typeof(FriedEggs), typeof(Eggs) };

        private static readonly Type[] m_Fish = new[] { typeof(FishSteak), typeof(RawFishSteak) };

        private static readonly Type[] m_GrainsAndHay = new[] { typeof(BreadLoaf), typeof(FrenchBread), typeof(SheafOfHay) };

        private static readonly Type[] m_Meat = new[]
        {
			/* Cooked */
			typeof(Bacon), typeof(CookedBird), typeof(Sausage), typeof(Ham), typeof(Ribs), typeof(LambLeg), typeof(ChickenLeg),
			/* Uncooked */
			typeof(RawBird), typeof(RawRibs), typeof(RawLambLeg), typeof(RawChickenLeg), /* Body Parts */
			typeof(Head), typeof(LeftArm), typeof(LeftLeg), typeof(Torso), typeof(RightArm), typeof(RightLeg)
        };

        private static readonly Type[] m_FruitsAndVegies = new[]
        {
            typeof(HoneydewMelon), typeof(YellowGourd), typeof(GreenGourd), typeof(Banana), typeof(Bananas), typeof(Lemon),
            typeof(Lime), typeof(Dates), typeof(Grapes), typeof(Peach), typeof(Pear), typeof(Apple), typeof(Watermelon),
            typeof(Squash), typeof(Cantaloupe), typeof(Carrot), typeof(Cabbage), typeof(Onion), typeof(Lettuce), typeof(Pumpkin)
        };

        private static Type[] m_Gold = new[]
        {
			// white wyrms eat gold..
			typeof(Gold)
        };

        private static readonly Type[] m_Metal = new[]
        {
			// Some Stygian Abyss Monsters eat Metal..
			typeof(IronIngot), typeof(DullCopperIngot), typeof(ShadowIronIngot), typeof(CopperIngot), typeof(BronzeIngot),
            typeof(GoldIngot), typeof(AgapiteIngot), typeof(VeriteIngot), typeof(ValoriteIngot)
        };

        public virtual bool CheckFoodPreference(Item f)
        {
            if (CheckFoodPreference(f, FoodType.Eggs, m_Eggs))
            {
                return true;
            }

            if (CheckFoodPreference(f, FoodType.Fish, m_Fish))
            {
                return true;
            }

            if (CheckFoodPreference(f, FoodType.GrainsAndHay, m_GrainsAndHay))
            {
                return true;
            }

            if (CheckFoodPreference(f, FoodType.Meat, m_Meat))
            {
                return true;
            }

            if (CheckFoodPreference(f, FoodType.FruitsAndVegies, m_FruitsAndVegies))
            {
                return true;
            }

            if (CheckFoodPreference(f, FoodType.Metal, m_Metal))
            {
                return true;
            }

            return false;
        }

        public virtual bool CheckFoodPreference(Item fed, FoodType type, Type[] types)
        {
            if ((FavoriteFood & type) == 0)
            {
                return false;
            }

            Type fedType = fed.GetType();
            bool contains = false;

            for (int i = 0; !contains && i < types.Length; ++i)
            {
                contains = (fedType == types[i]);
            }

            return contains;
        }

        public virtual bool CheckFeed(Mobile from, Item dropped)
        {
            if (!IsDeadPet && Controlled && (ControlMaster == from || IsPetFriend(from)) &&
                (dropped is Food || dropped is Gold || dropped is CookableFood || dropped is Head || dropped is LeftArm ||
                 dropped is LeftLeg || dropped is Torso || dropped is RightArm || dropped is RightLeg || dropped is IronIngot ||
                 dropped is DullCopperIngot || dropped is ShadowIronIngot || dropped is CopperIngot || dropped is BronzeIngot ||
                 dropped is GoldIngot || dropped is AgapiteIngot || dropped is VeriteIngot || dropped is ValoriteIngot))
            {
                Item f = dropped;

                if (CheckFoodPreference(f))
                {
                    int amount = f.Amount;

                    if (amount > 0)
                    {
                        bool happier = false;

                        int stamGain;

                        if (f is Gold)
                        {
                            stamGain = amount - 50;
                        }
                        else
                        {
                            stamGain = (amount * 15) - 50;
                        }

                        if (stamGain > 0)
                        {
                            Stam += stamGain;
                        }

                        if (Core.SE)
                        {
                            if (m_Loyalty < MaxLoyalty)
                            {
                                m_Loyalty = MaxLoyalty;
                                happier = true;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < amount; ++i)
                            {
                                if (m_Loyalty < MaxLoyalty && 0.5 >= Utility.RandomDouble())
                                {
                                    m_Loyalty += 10;
                                    happier = true;
                                }
                            }
                        }

                        if (happier)
                        {
                            SayTo(from, 502060); // Your pet looks happier.
                        }

                        if (Core.SA)
                        {
                            Animate(AnimationType.Eat, 0);
                        }
                        else
                        {
                            Animate(Body.IsAnimal ? 3 : Body.IsHuman ? 34 : 17, 5, 1, true, false, 0);
                        }

                        if (IsBondable && !IsBonded)
                        {
                            Mobile master = m_ControlMaster;

                            if (master != null && master == from) //So friends can't start the bonding process
                            {
                                if (m_CurrentTameSkill <= 29.1 || master.Skills[SkillName.AnimalTaming].Base >= m_CurrentTameSkill ||
                                    OverrideBondingReqs() || (Core.ML && master.Skills[SkillName.AnimalTaming].Value >= m_CurrentTameSkill))
                                {
                                    if (BondingBegin == DateTime.MinValue)
                                    {
                                        BondingBegin = DateTime.UtcNow;
                                    }
                                    else if ((BondingBegin + BondingDelay) <= DateTime.UtcNow)
                                    {
                                        IsBonded = true;
                                        BondingBegin = DateTime.MinValue;
                                        from.SendLocalizedMessage(1049666); // Your pet has bonded with you!
                                    }
                                }
                                else if (Core.ML)
                                {
                                    from.SendLocalizedMessage(1075268);
                                    // Your pet cannot form a bond with you until your animal taming ability has risen.
                                }
                            }
                        }

                        dropped.Delete();
                        return true;
                    }
                }
            }

            return false;
        }
        #endregion

        public virtual bool OverrideBondingReqs()
        {
            return false;
        }

        public virtual bool CanAngerOnTame { get { return false; } }

        #region OnAction[...]
        public virtual void OnActionWander()
        { }

        public virtual void OnActionCombat()
        { }

        public virtual void OnActionGuard()
        { }

        public virtual void OnActionFlee()
        { }

        public virtual void OnActionInteract()
        { }

        public virtual void OnActionBackoff()
        { }
        #endregion

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (CheckFeed(from, dropped))
            {
                return true;
            }
            if (CheckGold(from, dropped))
            {
                return true;
            }
	        if (!from.InRange(Location, 2)) return base.OnDragDrop(from, dropped);
	        bool gainedPath = false;

	        if (dropped.HonestyOwner == this)
		        VirtueHelper.Award(from, VirtueName.Honesty, 120, ref gainedPath);
	        else
		        return false;

	        from.SendMessage(gainedPath ? "You have gained a path in Honesty!" : "You have gained in Honesty.");
	        SayTo(from, 1074582); //Ah!  You found my property.  Thank you for your honesty in returning it to me.
	        dropped.Delete();
	        return true;
        }

        protected virtual BaseAI ForcedAI { get { return null; } }

        public void ChangeAIType(AIType NewAI)
        {
            if (m_AI != null)
            {
                m_AI.m_Timer.Stop();
            }

            if (ForcedAI != null)
            {
                m_AI = ForcedAI;
                return;
            }

            m_AI = null;

            switch (NewAI)
            {
                case AIType.AI_Melee:
                    m_AI = new MeleeAI(this);
                    break;
                case AIType.AI_Animal:
                    m_AI = new AnimalAI(this);
                    break;
                case AIType.AI_Berserk:
                    m_AI = new BerserkAI(this);
                    break;
                case AIType.AI_Archer:
                    m_AI = new ArcherAI(this);
                    break;
                case AIType.AI_Healer:
                    m_AI = new HealerAI(this);
                    break;
                case AIType.AI_Vendor:
                    m_AI = new VendorAI(this);
                    break;
                case AIType.AI_Mage:
                    m_AI = new MageAI(this);
                    break;
                case AIType.AI_Predator:
                    //m_AI = new PredatorAI(this);
                    m_AI = new MeleeAI(this);
                    break;
                case AIType.AI_Thief:
                    m_AI = new ThiefAI(this);
                    break;
                case AIType.AI_NecroMage:
                    m_AI = new NecroMageAI(this);
                    break;
                case AIType.AI_OrcScout:
                    m_AI = new OrcScoutAI(this);
                    break;
                case AIType.AI_Samurai:
                    m_AI = new SamuraiAI(this);
                    break;
                case AIType.AI_Ninja:
                    m_AI = new NinjaAI(this);
                    break;
                case AIType.AI_Spellweaving:
                    m_AI = new SpellweavingAI(this);
                    break;
                case AIType.AI_Mystic:
                    m_AI = new MysticAI(this);
                    break;
                case AIType.AI_Paladin:
                    m_AI = new PaladinAI(this);
                    break;
                case AIType.AI_Spellbinder:
                    m_AI = new SpellbinderAI(this);
                    break;
                case AIType.AI_Necro:
                    m_AI = new NecroAI(this);
                    break;
            }
        }

        public void ChangeAIToDefault()
        {
            ChangeAIType(m_DefaultAI);
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public AIType AI
        {
            get { return m_CurrentAI; }
            set
            {
                m_CurrentAI = value;

                if (m_CurrentAI == AIType.AI_Use_Default)
                {
                    m_CurrentAI = m_DefaultAI;
                }

                ChangeAIType(m_CurrentAI);
            }
        }

        [CommandProperty(AccessLevel.Administrator)]
        public bool Debug { get { return m_bDebugAI; } set { m_bDebugAI = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Team
        {
            get { return m_iTeam; }
            set
            {
                m_iTeam = value;

                OnTeamChange();
            }
        }

        public virtual void OnTeamChange()
        { }

        [CommandProperty(AccessLevel.GameMaster)]
        public IDamageable FocusMob { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public FightMode FightMode { get { return m_FightMode; } set { m_FightMode = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RangePerception { get { return m_iRangePerception; } set { m_iRangePerception = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RangeFight { get { return m_iRangeFight; } set { m_iRangeFight = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RangeHome { get { return m_iRangeHome; } set { m_iRangeHome = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public double ActiveSpeed { get { return m_dActiveSpeed; } set { m_dActiveSpeed = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public double PassiveSpeed { get { return m_dPassiveSpeed; } set { m_dPassiveSpeed = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public double CurrentSpeed
        {
            get
            {
                if (m_TargetLocation != null)
                {
                    return 0.3;
                }

                return m_dCurrentSpeed;
            }
            set
            {
                if (m_dCurrentSpeed != value)
                {
                    m_dCurrentSpeed = value;

                    if (m_AI != null)
                    {
                        m_AI.OnCurrentSpeedChanged();
                    }
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D Home { get { return m_pHome; } set { m_pHome = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Controlled
        {
            get { return m_bControlled; }
            set
            {
                if (m_bControlled == value)
                {
                    return;
                }

                m_bControlled = value;
                Delta(MobileDelta.Noto);

                InvalidateProperties();
            }
        }

        #region Snake Charming
        private Mobile m_CharmMaster;
        private Point2D m_CharmTarget;
        private Timer m_CharmTimer;

        public void BeginCharm(Mobile master, Point2D target)
        {
            m_CharmMaster = master;
            m_CharmTarget = target;

            m_CharmTimer = new CharmTimer(this);
            m_CharmTimer.Start();
        }

        public void EndCharm()
        {
            if (!Deleted && m_CharmMaster != null)
            {
                // The charm seems to wear off.
                PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1112181, m_CharmMaster.NetState);

                Frozen = false;

                m_CharmMaster = null;
                m_CharmTarget = Point2D.Zero;

                if (m_CharmTimer != null)
                {
                    m_CharmTimer.Stop();
                    m_CharmTimer = null;
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile CharmMaster { get { return m_CharmMaster; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point2D CharmTarget { get { return m_CharmTarget; } }

        private class CharmTimer : Timer
        {
            private BaseCreature m_Owner;
            private int m_Count;

            public CharmTimer(BaseCreature owner)
                : base(TimeSpan.Zero, TimeSpan.FromSeconds(2.0))
            {
                m_Owner = owner;
                m_Count = 10;
            }

            protected override void OnTick()
            {
                if (m_Count == 0 || m_Owner.CharmMaster == null || !m_Owner.CharmMaster.InRange(m_Owner.Location, 10))
                {
                    Stop();
                    m_Owner.EndCharm();
                }
                else
                {
                    m_Owner.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);
                    m_Count--;
                }
            }
        }
        #endregion

        public override void RevealingAction()
        {
            InvisibilitySpell.RemoveTimer(this);

            base.RevealingAction();
        }

        public void RemoveFollowers()
        {
            if (m_ControlMaster != null)
            {
                m_ControlMaster.Followers -= ControlSlots;

                if (m_ControlMaster is PlayerMobile)
                {
                    PlayerMobile pm = (PlayerMobile)m_ControlMaster;

                    pm.AllFollowers.Remove(this);

                    if (pm.AutoStabled.Contains(this))
                    {
                        pm.AutoStabled.Remove(this);
                    }

                    NetState ns = m_ControlMaster.NetState;

                    if (ns != null && ns.IsEnhancedClient && Commandable)
                    {
                        ns.Send(new PetWindow(pm, this));
                    }
                }
            }
            else if (m_SummonMaster != null)
            {
                m_SummonMaster.Followers -= ControlSlots;

                if (m_SummonMaster is PlayerMobile)
                {
                    ((PlayerMobile)m_SummonMaster).AllFollowers.Remove(this);
                }
            }

            if (m_ControlMaster != null && m_ControlMaster.Followers < 0)
            {
                m_ControlMaster.Followers = 0;
            }

            if (m_SummonMaster != null && m_SummonMaster.Followers < 0)
            {
                m_SummonMaster.Followers = 0;
            }
        }

        public void AddFollowers()
        {
            if (m_ControlMaster != null)
            {
                m_ControlMaster.Followers += ControlSlots;

                if (m_ControlMaster is PlayerMobile && !(this is PersonalAttendant))
                {
                    ((PlayerMobile)m_ControlMaster).AllFollowers.Add(this);

                    NetState ns = m_ControlMaster.NetState;

                    if (ns != null && ns.IsEnhancedClient && Commandable)
                    {
                        ns.Send(new PetWindow((PlayerMobile)m_ControlMaster, this));
                    }
                }
            }
            else if (m_SummonMaster != null)
            {
                m_SummonMaster.Followers += ControlSlots;
                if (m_SummonMaster is PlayerMobile)
                {
                    ((PlayerMobile)m_SummonMaster).AllFollowers.Add(this);
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile ControlMaster
        {
            get { return m_ControlMaster; }
            set
            {
                if (m_ControlMaster == value || this == value)
                {
                    return;
                }

                RemoveFollowers();
                m_ControlMaster = value;
                AddFollowers();
                if (m_ControlMaster != null)
                {
                    StopDeleteTimer();
                }

                Delta(MobileDelta.Noto);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile SummonMaster
        {
            get { return m_SummonMaster; }
            set
            {
                if (m_SummonMaster == value || this == value)
                {
                    return;
                }

                RemoveFollowers();
                m_SummonMaster = value;
                AddFollowers();

                Delta(MobileDelta.Noto);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public IDamageable ControlTarget { get { return m_ControlTarget; } set { m_ControlTarget = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D ControlDest { get { return m_ControlDest; } set { m_ControlDest = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual OrderType ControlOrder
        {
            get { return m_ControlOrder; }
            set
            {
                m_ControlOrder = value;

                if (m_Allured)
                {
                    if (m_ControlOrder == OrderType.Release)
                    {
                        Say(502003); // Sorry, but no.
                    }
                    else if (m_ControlOrder != OrderType.None)
                    {
                        Say(1079120); // Very well.
                    }
                }

                if (m_AI != null)
                {
                    m_AI.OnCurrentOrderChanged();
                }

                InvalidateProperties();

                if (m_ControlMaster != null)
                {
                    m_ControlMaster.InvalidateProperties();
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool BardProvoked { get { return m_bBardProvoked; } set { m_bBardProvoked = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool BardPacified { get { return m_bBardPacified; } set { m_bBardPacified = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile BardMaster { get { return m_bBardMaster; } set { m_bBardMaster = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile BardTarget { get { return m_bBardTarget; } set { m_bBardTarget = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime BardEndTime { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public double MinTameSkill 
        {
            get { return m_dMinTameSkill; }
            set
            {
                double skill = m_dMinTameSkill;

                m_dMinTameSkill = value;

                if (skill != m_dMinTameSkill)
                {
                    m_CurrentTameSkill = value;
                    AdjustTameRequirements();
                }
            } 
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double CurrentTameSkill { get { return m_CurrentTameSkill; } set { m_CurrentTameSkill = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Tamable { get { return m_bTamable && !m_Paragon; } set { m_bTamable = value; } }

        [CommandProperty(AccessLevel.Administrator)]
        public bool Summoned
        {
            get { return m_bSummoned; }
            set
            {
                if (m_bSummoned == value)
                {
                    return;
                }

                m_NextReacquireTime = Core.TickCount;

                m_bSummoned = value;
                Delta(MobileDelta.Noto);

                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.Administrator)]
        public int ControlSlots
        {
            get { return m_iControlSlots; }
            set
            {
                if (PetTrainingHelper.Enabled && ControlSlotsMin == 0 && ControlSlotsMax == 0)
                {
                    CalculateSlots(value);
                    m_iControlSlots = ControlSlotsMin;
                }
                else
                {
                    m_iControlSlots = value;
                }
            }
        }

        [CommandProperty(AccessLevel.Administrator)]
        public int ControlSlotsMax { get; private set; }

        [CommandProperty(AccessLevel.Administrator)]
        public int ControlSlotsMin { get; private set; }

        public virtual bool NoHouseRestrictions { get { return false; } }
        public virtual bool IsHouseSummonable { get { return false; } }

        #region Corpse Resources
        public virtual int Feathers { get { return 0; } }
        public virtual int Wool { get { return 0; } }

        public virtual int Fur { get { return 0; } }

        public virtual MeatType MeatType { get { return MeatType.Ribs; } }
        public virtual int Meat { get { return 0; } }

        public virtual int Hides { get { return 0; } }
        public virtual HideType HideType { get { return HideType.Regular; } }

        public virtual int Scales { get { return 0; } }
        public virtual ScaleType ScaleType { get { return ScaleType.Red; } }

        public virtual int DragonBlood { get { return 0; } }
        #endregion

        public virtual bool AutoDispel { get { return false; } }
        public virtual double AutoDispelChance { get { return ((Core.SE) ? .10 : 1.0); } }

        public virtual bool IsScaryToPets { get { return false; } }
        public virtual bool IsScaredOfScaryThings { get { return true; } }

        public virtual bool CanRummageCorpses { get { return false; } }

        public virtual void OnGotMeleeAttack(Mobile attacker)
        {
            if (AutoDispel && attacker is BaseCreature && ((BaseCreature)attacker).IsDispellable &&
                AutoDispelChance > Utility.RandomDouble())
            {
                Dispel(attacker);
            }

            if (!m_InRage && CanDoRage)
            {
                DoRage(attacker);
            }

            if (DrainsLife && DrainsLifeChance >= Utility.RandomDouble())
            {
                DrainLife();
            }
        }

        public virtual void Dispel(Mobile m)
        {
            Effects.SendLocationParticles(EffectItem.Create(m.Location, m.Map, EffectItem.DefaultDuration), 0x3728, 8, 20, 5042);
            Effects.PlaySound(m, m.Map, 0x201);

            m.Delete();
        }

        public virtual bool DeleteOnRelease { get { return m_bSummoned; } }

        public virtual void OnGaveMeleeAttack(Mobile defender)
        {
            Poison p = GetHitPoison();

            XmlPoison xp = (XmlPoison)XmlAttach.FindAttachment(this, typeof(XmlPoison));

            if (xp != null)
            {
                p = xp.HitPoison;
            }

            if (m_Paragon)
            {
                p = PoisonImpl.IncreaseLevel(p);
            }

            if (p != null)
            {
                if (TryHitPoison())
                {
                    if (Core.TOL)
                    {
                        defender.FixedEffect(0x3779, 1, 10, 1271, 0);
                    }

                    defender.ApplyPoison(this, p);
                }

                if (Controlled)
                {
                    if (!PetTrainingHelper.Enabled || (AbilityProfile != null && AbilityProfile.HasAbility(MagicalAbility.Poisoning)))
                    {
                        CheckSkill(SkillName.Poisoning, 0, Skills[SkillName.Poisoning].Cap);
                    }
                }
            }

            if (AutoDispel && defender is BaseCreature && ((BaseCreature)defender).IsDispellable &&
                AutoDispelChance > Utility.RandomDouble())
            {
                Dispel(defender);
            }

            if (DrainsLife && DrainsLifeChance >= Utility.RandomDouble())
            {
                DrainLife();
            }

            if (m_InRage && RageProbability >= Utility.RandomDouble())
            {
                DoRageHit(defender);
            }

            if (DoesColossalBlow && !_Stunning && ColossalBlowChance > Utility.RandomDouble())
            {
                DoColossalBlow(defender);
            }
        }

        public Poison GetHitPoison()
        {
            if (!PetTrainingHelper.Enabled || !Controlled)
                return HitPoison;

            int current = 0;

            if (HitPoison != null)
                current = HitPoison.Level;

            var profile = AbilityProfile;

            if (profile == null || !profile.HasAbility(MagicalAbility.Poisoning) || current >= 4)
                return HitPoison;

            int level = 1;
            double total = Skills[SkillName.Poisoning].Value;

            // natural poisoner retains their poison level. Added spell school is capped at level 2.
            if (total >= 100)
                level = 4;
            else if (total > 85)
                level = 3;
            else if (total > 65)
                level = 2;
            else if (total > 35)
                level = 1;
                
            return Poison.GetPoison(Math.Max(current, level));
        }

        private bool TryHitPoison()
        {
            if(!PetTrainingHelper.Enabled || !Controlled)
                return HitPoisonChance >= Utility.RandomDouble();

            var profile = AbilityProfile;

            if (profile == null || !profile.HasAbility(MagicalAbility.Poisoning))
                return false;

            return Skills[SkillName.Poisoning].Value >= Utility.Random(300);
        }

        public override void OnAfterDelete()
        {
            if (m_AI != null)
            {
                if (m_AI.m_Timer != null)
                {
                    m_AI.m_Timer.Stop();
                }

                m_AI = null;
            }

            if (m_DeleteTimer != null)
            {
                m_DeleteTimer.Stop();
                m_DeleteTimer = null;
            }

            FocusMob = null;

            if (IsAnimatedDead)
            {
                AnimateDeadSpell.Unregister(m_SummonMaster, this);
            }

            base.OnAfterDelete();
        }

        public void DebugSay(string text)
        {
            if (m_bDebugAI)
            {
                PublicOverheadMessage(MessageType.Regular, 41, false, text);
            }
        }

        public void DebugSay(string format, params object[] args)
        {
            if (m_bDebugAI)
            {
                PublicOverheadMessage(MessageType.Regular, 41, false, String.Format(format, args));
            }
        }

        /*
        * This function can be overriden.. so a "Strongest" mobile, can have a different definition depending
        * on who check for value
        * -Could add a FightMode.Prefered
        *
        */

        public virtual double GetFightModeRanking(Mobile m, FightMode acqType, bool bPlayerOnly)
        {
            if ((bPlayerOnly && m.Player) || !bPlayerOnly)
            {
                switch (acqType)
                {
                    case FightMode.Strongest:
                        return (m.Skills[SkillName.Tactics].Value + m.Str); //returns strongest mobile

                    case FightMode.Weakest:
                        return -m.Hits; // returns weakest mobile

                    default:
                        return -GetDistanceToSqrt(m); // returns closest mobile
                }
            }
            else
            {
                return double.MinValue;
            }
        }

        // Turn, - for left, + for right
        // Basic for now, needs work
        public virtual void Turn(int iTurnSteps)
        {
            int v = (int)Direction;

            Direction = (Direction)((((v & 0x7) + iTurnSteps) & 0x7) | (v & 0x80));
        }

        public virtual void TurnInternal(int iTurnSteps)
        {
            int v = (int)Direction;

            SetDirection((Direction)((((v & 0x7) + iTurnSteps) & 0x7) | (v & 0x80)));
        }

        public bool IsHurt()
        {
            return (Hits != HitsMax);
        }

        public double GetHomeDistance()
        {
            return GetDistanceToSqrt(m_pHome);
        }

        public virtual int GetTeamSize(int iRange)
        {
            int iCount = 0;

            IPooledEnumerable eable = GetMobilesInRange(iRange);
            foreach (Mobile m in eable)
            {
                if (m is BaseCreature)
                {
                    if (((BaseCreature)m).Team == Team)
                    {
                        if (!m.Deleted)
                        {
                            if (m != this)
                            {
                                if (CanSee(m))
                                {
                                    iCount++;
                                }
                            }
                        }
                    }
                }
            }

            eable.Free();

            return iCount;
        }

        private class TameEntry : ContextMenuEntry
        {
            private readonly BaseCreature m_Mobile;

            public TameEntry(Mobile from, BaseCreature creature)
                : base(6130, 6)
            {
                m_Mobile = creature;

                Enabled = Enabled && (from.Female ? creature.AllowFemaleTamer : creature.AllowMaleTamer);
            }

            public override void OnClick()
            {
                if (!Owner.From.CheckAlive())
                {
                    return;
                }

                Owner.From.TargetLocked = true;
                AnimalTaming.DisableMessage = true;

                if (Owner.From.UseSkill(SkillName.AnimalTaming))
                {
                    Owner.From.Target.Invoke(Owner.From, m_Mobile);
                }

                AnimalTaming.DisableMessage = false;
                Owner.From.TargetLocked = false;
            }
        }

        private class RenameEntry : ContextMenuEntry
        {
            private Mobile m_From;
            private BaseCreature m_Creature;

            public RenameEntry(Mobile from, BaseCreature creature)
                : base(1111680, 6)
            {
                m_From = from;
                m_Creature = creature;
            }

            public override void OnClick()
            {
                if (!m_Creature.Deleted && m_Creature.Controlled && m_Creature.ControlMaster == m_From)
                    m_From.Prompt = new PetRenamePrompt(m_Creature);
            }
        }

        public class PetRenamePrompt : Prompt
        {
            // Enter a new name for your pet.
            public override int MessageCliloc { get { return 1115558; } }

            private BaseCreature m_Creature;

            public PetRenamePrompt(BaseCreature creature)
                : base(creature)
            {
                m_Creature = creature;
            }

            public override void OnCancel(Mobile from)
            {
                from.SendLocalizedMessage(501806); // Request cancelled.
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (!m_Creature.Deleted && m_Creature.Controlled && m_Creature.ControlMaster == from)
                {
                    if (Utility.IsAlpha(text))
                    {
                        m_Creature.Name = text;
                        from.SendLocalizedMessage(1115559); // Pet name changed.
                    }
                    else
                    {
                        from.SendLocalizedMessage(1075246); // That name is not valid.
                    }
                }
            }
        }

        #region Teaching
        public virtual bool CanTeach { get { return false; } }

        public virtual bool CheckTeach(SkillName skill, Mobile from)
        {
            if (!CanTeach || Siege.SiegeShard)
            {
                return false;
            }

            if (skill == SkillName.Stealth && from.Skills[SkillName.Hiding].Base < Stealth.HidingRequirement)
            {
                return false;
            }

            if (skill == SkillName.RemoveTrap &&
                (from.Skills[SkillName.Lockpicking].Base < 50.0 || from.Skills[SkillName.DetectHidden].Base < 50.0))
            {
                return false;
            }

            if (!Core.AOS && (skill == SkillName.Focus || skill == SkillName.Chivalry || skill == SkillName.Necromancy))
            {
                return false;
            }

            return true;
        }

        public enum TeachResult
        {
            Success,
            Failure,
            KnowsMoreThanMe,
            KnowsWhatIKnow,
            SkillNotRaisable,
            NotEnoughFreePoints
        }

        public virtual TeachResult CheckTeachSkills(
            SkillName skill, Mobile m, int maxPointsToLearn, ref int pointsToLearn, bool doTeach)
        {
            if (!CheckTeach(skill, m) || !m.CheckAlive())
            {
                return TeachResult.Failure;
            }

            Skill ourSkill = Skills[skill];
            Skill theirSkill = m.Skills[skill];

            if (ourSkill == null || theirSkill == null)
            {
                return TeachResult.Failure;
            }

            int baseToSet = ourSkill.BaseFixedPoint / 3;

            if (baseToSet > 420)
            {
                baseToSet = 420;
            }
            else if (baseToSet < 200)
            {
                return TeachResult.Failure;
            }

            if (baseToSet > theirSkill.CapFixedPoint)
            {
                baseToSet = theirSkill.CapFixedPoint;
            }

            pointsToLearn = baseToSet - theirSkill.BaseFixedPoint;

            if (maxPointsToLearn > 0 && pointsToLearn > maxPointsToLearn)
            {
                pointsToLearn = maxPointsToLearn;
                baseToSet = theirSkill.BaseFixedPoint + pointsToLearn;
            }

            if (pointsToLearn < 0)
            {
                return TeachResult.KnowsMoreThanMe;
            }

            if (pointsToLearn == 0)
            {
                return TeachResult.KnowsWhatIKnow;
            }

            if (theirSkill.Lock != SkillLock.Up)
            {
                return TeachResult.SkillNotRaisable;
            }

            int freePoints = m.Skills.Cap - m.Skills.Total;
            int freeablePoints = 0;

            if (freePoints < 0)
            {
                freePoints = 0;
            }

            for (int i = 0; (freePoints + freeablePoints) < pointsToLearn && i < m.Skills.Length; ++i)
            {
                Skill sk = m.Skills[i];

                if (sk == theirSkill || sk.Lock != SkillLock.Down)
                {
                    continue;
                }

                freeablePoints += sk.BaseFixedPoint;
            }

            if ((freePoints + freeablePoints) == 0)
            {
                return TeachResult.NotEnoughFreePoints;
            }

            if ((freePoints + freeablePoints) < pointsToLearn)
            {
                pointsToLearn = freePoints + freeablePoints;
                baseToSet = theirSkill.BaseFixedPoint + pointsToLearn;
            }

            if (doTeach)
            {
                int need = pointsToLearn - freePoints;

                for (int i = 0; need > 0 && i < m.Skills.Length; ++i)
                {
                    Skill sk = m.Skills[i];

                    if (sk == theirSkill || sk.Lock != SkillLock.Down)
                    {
                        continue;
                    }

                    if (sk.BaseFixedPoint < need)
                    {
                        need -= sk.BaseFixedPoint;
                        sk.BaseFixedPoint = 0;
                    }
                    else
                    {
                        sk.BaseFixedPoint -= need;
                        need = 0;
                    }
                }

                /* Sanity check */
                if (baseToSet > theirSkill.CapFixedPoint || (m.Skills.Total - theirSkill.BaseFixedPoint + baseToSet) > m.Skills.Cap)
                {
                    // Full refund
                    m.Backpack.TryDropItem(m, new Gold(maxPointsToLearn), false);
                    return TeachResult.NotEnoughFreePoints;
                }

                // Partial refund if needed
                if (maxPointsToLearn > pointsToLearn)
                {
                    m.Backpack.TryDropItem(m, new Gold(maxPointsToLearn - pointsToLearn), false);
                }
                theirSkill.BaseFixedPoint = baseToSet;
            }

            return TeachResult.Success;
        }

        public virtual bool CheckTeachingMatch(Mobile m)
        {
            if (m_Teaching == (SkillName)(-1))
            {
                return false;
            }

            if (m is PlayerMobile)
            {
                return (((PlayerMobile)m).Learning == m_Teaching);
            }

            return true;
        }

        private SkillName m_Teaching = (SkillName)(-1);

        public virtual bool Teach(SkillName skill, Mobile m, int maxPointsToLearn, bool doTeach)
        {
            int pointsToLearn = 0;
            TeachResult res = CheckTeachSkills(skill, m, maxPointsToLearn, ref pointsToLearn, doTeach);

            switch (res)
            {
                case TeachResult.KnowsMoreThanMe:
                    {
                        Say(501508); // I cannot teach thee, for thou knowest more than I!
                        break;
                    }
                case TeachResult.KnowsWhatIKnow:
                    {
                        Say(501509); // I cannot teach thee, for thou knowest all I can teach!
                        break;
                    }
                case TeachResult.NotEnoughFreePoints:
                case TeachResult.SkillNotRaisable:
                    {
                        // Make sure this skill is marked to raise. If you are near the skill cap (700 points) you may need to lose some points in another skill first.
                        m.SendLocalizedMessage(501510, "", 0x22);
                        break;
                    }
                case TeachResult.Success:
                    {
                        if (doTeach)
                        {
                            Say(501539); // Let me show thee something of how this is done.
                            m.SendLocalizedMessage(501540); // Your skill level increases.

                            m_Teaching = (SkillName)(-1);

                            if (m is PlayerMobile)
                            {
                                ((PlayerMobile)m).Learning = (SkillName)(-1);
                            }
                        }
                        else
                        {
                            // I will teach thee all I know, if paid the amount in full.  The price is:
                            Say(1019077, AffixType.Append, String.Format(" {0}", pointsToLearn), "");
                            Say(1043108); // For less I shall teach thee less.

                            m_Teaching = skill;

                            if (m is PlayerMobile)
                            {
                                ((PlayerMobile)m).Learning = skill;
                            }
                        }

                        return true;
                    }
            }

            return false;
        }
        #endregion

        public override void AggressiveAction(Mobile aggressor, bool criminal)
        {
            base.AggressiveAction(aggressor, criminal);

            if (ControlMaster != null && ControlMaster != aggressor)
            {
                if (NotorietyHandlers.CheckAggressor(Aggressors, aggressor))
                {
                    ControlMaster.Aggressors.Add(AggressorInfo.Create(aggressor, ControlMaster, criminal));
                    ControlMaster.Delta(MobileDelta.Noto);

                    if (NotorietyHandlers.CheckAggressed(aggressor.Aggressed, this))
                        aggressor.Aggressed.Add(AggressorInfo.Create(aggressor, ControlMaster, criminal));

                    if (aggressor is PlayerMobile || (aggressor is BaseCreature && !((BaseCreature)aggressor).IsMonster))
                    {
                        BuffInfo.AddBuff(ControlMaster, new BuffInfo(BuffIcon.HeatOfBattleStatus, 1153801, 1153827, AttackMessage.CombatHeatDelay, ControlMaster, true));
                        BuffInfo.AddBuff(aggressor, new BuffInfo(BuffIcon.HeatOfBattleStatus, 1153801, 1153827, AttackMessage.CombatHeatDelay, aggressor, true));
                    }
                }
            }

            OrderType ct = m_ControlOrder;

            if (m_AI != null)
            {
                if (!Core.ML || (ct != OrderType.Follow && ct != OrderType.Stop && ct != OrderType.Stay))
                {
                    m_AI.OnAggressiveAction(aggressor);
                }
                else
                {
                    DebugSay("I'm being attacked but my master told me not to fight.");
                    Warmode = false;
                    return;
                }
            }

            StopFlee();

            ForceReacquire();

            if (!IsEnemy(aggressor))
            {
                Player pl = Ethics.Player.Find(aggressor, true);

                if (pl != null && pl.IsShielded)
                {
                    pl.FinishShield();
                }
            }

            if (aggressor.ChangingCombatant && (m_bControlled || m_bSummoned) &&
                (ct == OrderType.Come || (!Core.ML && ct == OrderType.Stay) || ct == OrderType.Stop || ct == OrderType.None ||
                 ct == OrderType.Follow))
            {
                ControlTarget = aggressor;
                ControlOrder = OrderType.Attack;
            }
            else if (Combatant == null && !m_bBardPacified)
            {
                Warmode = true;
                Combatant = aggressor;
            }
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (m is BaseCreature && !((BaseCreature)m).Controlled)
            {
                return (!Alive || !m.Alive || IsDeadBondedPet || m.IsDeadBondedPet) || (Hidden && IsStaff());
            }

            return base.OnMoveOver(m);
        }

        public virtual void AddCustomContextEntries(Mobile from, List<ContextMenuEntry> list)
        { }

        public virtual bool CanDrop { get { return IsBonded; } }
        public virtual bool OwnerCanRename { get { return true; } }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (m_bControlled && m_ControlMaster == from && !m_bSummoned && OwnerCanRename)
            {
                list.Add(new RenameEntry(from, this));
            }

            if (m_AI != null && Commandable)
            {
                m_AI.GetContextMenuEntries(from, list);
            }

            if (m_bTamable && !m_bControlled && from.Alive)
            {
                list.Add(new TameEntry(from, this));
            }

            AddCustomContextEntries(from, list);

            if (CanTeach && from.Alive)
            {
                Skills ourSkills = Skills;
                Skills theirSkills = from.Skills;

                for (int i = 0; i < ourSkills.Length && i < theirSkills.Length; ++i)
                {
                    Skill skill = ourSkills[i];
                    Skill theirSkill = theirSkills[i];

                    if (skill != null && theirSkill != null && skill.Base >= 60.0 && CheckTeach(skill.SkillName, from))
                    {
                        int toTeach = skill.BaseFixedPoint / 3;

                        if (toTeach > 420)
                        {
                            toTeach = 420;
                        }

                        list.Add(new TeachEntry((SkillName)i, this, from, (toTeach > theirSkill.BaseFixedPoint)));
                    }
                }
            }
        }

        public override bool HandlesOnSpeech(Mobile from)
        {
            InhumanSpeech speechType = SpeechType;

            if (speechType != null && (speechType.Flags & IHSFlags.OnSpeech) != 0 && from.InRange(this, 3))
            {
                return true;
            }

            return (m_AI != null && m_AI.HandlesOnSpeech(from) && from.InRange(this, m_iRangePerception));
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            InhumanSpeech speechType = SpeechType;

            if (speechType != null && speechType.OnSpeech(this, e.Mobile, e.Speech))
            {
                e.Handled = true;
            }
            else if (!e.Handled && m_AI != null && e.Mobile.InRange(this, m_iRangePerception))
            {
                m_AI.OnSpeech(e);
            }
        }

        public override bool IsHarmfulCriminal(IDamageable damageable)
        {
            Mobile target = damageable as Mobile;

            if ((Controlled && target == m_ControlMaster) || (Summoned && target == m_SummonMaster))
            {
                return false;
            }

            if (target is BaseCreature && ((BaseCreature)target).InitialInnocent && !((BaseCreature)target).Controlled)
            {
                return false;
            }

            if (target is PlayerMobile && ((PlayerMobile)target).PermaFlags.Count > 0)
            {
                return false;
            }

            return base.IsHarmfulCriminal(damageable);
        }

        public override void CriminalAction(bool message)
        {
            base.CriminalAction(message);

            if (Controlled || Summoned)
            {
                if (m_ControlMaster != null && m_ControlMaster.Player)
                {
                    m_ControlMaster.CriminalAction(false);
                }
                else if (m_SummonMaster != null && m_SummonMaster.Player)
                {
                    m_SummonMaster.CriminalAction(false);
                }
            }
        }

        public override void DoHarmful(IDamageable damageable, bool indirect)
        {
            if (RecentSetControl && GetMaster() == damageable as Mobile)
            {
                return;
            }

            base.DoHarmful(damageable, indirect);

            Mobile target = damageable as Mobile;

            if (target == null)
                return;

            if (target == this || target == m_ControlMaster || target == m_SummonMaster || (!Controlled && !Summoned))
            {
                return;
            }

            if (ViceVsVirtueSystem.Enabled && Map == Faction.Facet)
            {
                ViceVsVirtueSystem.CheckHarmful(this, target);
            }
        }

        public override void DoBeneficial(Mobile target)
        {
            base.DoBeneficial(target);

            if (ViceVsVirtueSystem.Enabled && Map == Faction.Facet && target != null)
            {
                ViceVsVirtueSystem.CheckBeneficial(this, target);
            }
        }

        private static Mobile m_NoDupeGuards;

        public void ReleaseGuardDupeLock()
        {
            m_NoDupeGuards = null;
        }

        public void ReleaseGuardLock()
        {
            EndAction(typeof(GuardedRegion));
        }

        private DateTime m_IdleReleaseTime;

        public virtual bool CheckIdle()
        {
            if (Combatant != null)
            {
                return false; // in combat.. not idling
            }

            if (m_IdleReleaseTime > DateTime.MinValue)
            {
                // idling...
                if (DateTime.UtcNow >= m_IdleReleaseTime)
                {
                    m_IdleReleaseTime = DateTime.MinValue;
                    return false; // idle is over
                }

                return true; // still idling
            }

            if (95 > Utility.Random(100))
            {
                return false; // not idling, but don't want to enter idle state
            }

            m_IdleReleaseTime = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(15, 25));

            if (Core.SA)
            {
                Animate(AnimationType.Fidget, 0);
            }
            else
            {
                if (Body.IsHuman && !Mounted)
                {
                    if (Flying)
                    {
                        Animate(66, 10, 1, true, false, 1);
                    }
                    else
                    {
                        switch (Utility.Random(2))
                        {
                            case 0:
                                Animate(5, 5, 1, true, true, 1);
                                break;
                            case 1:
                                Animate(6, 5, 1, true, false, 1);
                                break;
                        }
                    }
                }
                else if (Body.IsAnimal)
                {
                    switch (Utility.Random(3))
                    {
                        case 0:
                            Animate(3, 3, 1, true, false, 1);
                            break;
                        case 1:
                            Animate(9, 5, 1, true, false, 1);
                            break;
                        case 2:
                            Animate(10, 5, 1, true, false, 1);
                            break;
                    }
                }
                else if (Body.IsMonster)
                {
                    switch (Utility.Random(2))
                    {
                        case 0:
                            Animate(17, 5, 1, true, false, 1);
                            break;
                        case 1:
                            Animate(18, 5, 1, true, false, 1);
                            break;
                    }
                }


            }

            PlaySound(GetIdleSound());
            return true; // entered idle state
        }

        public override void Animate(int action, int frameCount, int repeatCount, bool forward, bool repeat, int delay)
        {
            base.Animate(action, frameCount, repeatCount, forward, repeat, delay);
        }

        private void CheckAIActive()
        {
            Map map = Map;

            if (PlayerRangeSensitive && m_AI != null && map != null && map.GetSector(Location).Active)
            {
                m_AI.Activate();
            }
        }

        public override void OnCombatantChange()
        {
            base.OnCombatantChange();

            Warmode = (Combatant != null && !Combatant.Deleted && Combatant.Alive);

            if (Warmode)
            {
                if (Core.SA)
                {
                    Animate(AnimationType.Alert, 0);
                }

                if (CanFly)
                {
                    Flying = false;
                }
            }
        }

        protected override void OnMapChange(Map oldMap)
        {
            CheckAIActive();

            base.OnMapChange(oldMap);
        }

        protected override void OnLocationChange(Point3D oldLocation)
        {
            CheckAIActive();

            base.OnLocationChange(oldLocation);
        }

        public virtual void ForceReacquire()
        {
            m_NextReacquireTime = Core.TickCount;
        }

        public virtual bool CanStealth { get { return false; } }

        protected override bool OnMove(Direction d)
        {

            if (Hidden) //Hidden, let's try stealth
            {
                if (!Mounted && Skills.Stealth.Value >= 25.0 && CanStealth)
                {
                    bool running = (d & Direction.Running) != 0;

                    if (running)
                    {
                        if ((AllowedStealthSteps -= 2) <= 0)
                            RevealingAction();
                    }
                    else if (AllowedStealthSteps-- <= 0)
                    {
                        Server.SkillHandlers.Stealth.OnUse(this);
                    }
                }
                else
                {
                    RevealingAction();
                }
            }

            return true;

        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (AcquireOnApproach && (!Controlled && !Summoned) && FightMode == FightMode.Closest && IsEnemy(m))
            {
                if (InRange(m.Location, AcquireOnApproachRange) && !InRange(oldLocation, AcquireOnApproachRange))
                {
                    if (CanBeHarmful(m) && IsEnemy(m))
                    {
                        Combatant = FocusMob = m;

                        if (AIObject != null)
                        {
                            AIObject.MoveTo(m, true, 1);
                        }

                        DoHarmful(m);
                    }
                }
            }
            else if (ReacquireOnMovement)
            {
                ForceReacquire();
            }

            if (CausesTrueFear)
                CauseTrueFear(m, oldLocation);

            InhumanSpeech speechType = SpeechType;

            if (speechType != null)
            {
                speechType.OnMovement(this, m, oldLocation);
            }

            /* Begin notice sound */
            if ((!m.Hidden || m.IsPlayer()) && m.Player && m_FightMode != FightMode.Aggressor && m_FightMode != FightMode.None &&
                Combatant == null && !Controlled && !Summoned)
            {
                // If this creature defends itself but doesn't actively attack (animal) or
                // doesn't fight at all (vendor) then no notice sounds are played..
                // So, players are only notified of aggressive monsters
                // Monsters that are currently fighting are ignored
                // Controlled or summoned creatures are ignored
                if (InRange(m.Location, 18) && !InRange(oldLocation, 18))
                {
                    if (Body.IsMonster)
                    {
                        if (Core.SA)
                        {
                            Animate(AnimationType.Pillage, 0);
                        }
                        else
                        {
                            Animate(11, 5, 1, true, false, 1);
                        }
                    }

                    PlaySound(GetAngerSound());
                }
            }
            /* End notice sound */

            if (m_NoDupeGuards == m)
            {
                return;
            }

            if (!Body.IsHuman || Murderer || AlwaysMurderer || AlwaysAttackable || m.Kills < 5 || !m.InRange(Location, 12) ||
                !m.Alive)
            {
                return;
            }

            GuardedRegion guardedRegion = (GuardedRegion)Region.GetRegion(typeof(GuardedRegion));

            if (guardedRegion != null)
            {
                if (!guardedRegion.IsDisabled() && guardedRegion.IsGuardCandidate(m) && BeginAction(typeof(GuardedRegion)))
                {
                    Say(1013037 + Utility.Random(16));
                    guardedRegion.CallGuards(Location);

                    Timer.DelayCall(TimeSpan.FromSeconds(5.0), ReleaseGuardLock);

                    m_NoDupeGuards = m;
                    Timer.DelayCall(TimeSpan.Zero, ReleaseGuardDupeLock);
                }
            }
        }

        public void AddSpellAttack(Type type)
        {
            m_arSpellAttack.Add(type);
        }

        public void AddSpellDefense(Type type)
        {
            m_arSpellDefense.Add(type);
        }

        public Spell GetAttackSpellRandom()
        {
            if (m_arSpellAttack.Count > 0)
            {
                Type type = m_arSpellAttack[Utility.Random(m_arSpellAttack.Count)];

                object[] args = { this, null };
                return Activator.CreateInstance(type, args) as Spell;
            }
            else
            {
                return null;
            }
        }

        public Spell GetDefenseSpellRandom()
        {
            if (m_arSpellDefense.Count > 0)
            {
                Type type = m_arSpellDefense[Utility.Random(m_arSpellDefense.Count)];

                object[] args = { this, null };
                return Activator.CreateInstance(type, args) as Spell;
            }
            else
            {
                return null;
            }
        }

        public Spell GetSpellSpecific(Type type)
        {
            int i;

            for (i = 0; i < m_arSpellAttack.Count; i++)
            {
                if (m_arSpellAttack[i] == type)
                {
                    object[] args = { this, null };
                    return Activator.CreateInstance(type, args) as Spell;
                }
            }

            for (i = 0; i < m_arSpellDefense.Count; i++)
            {
                if (m_arSpellDefense[i] == type)
                {
                    object[] args = { this, null };
                    return Activator.CreateInstance(type, args) as Spell;
                }
            }

            return null;
        }

        #region Set[...]
        public void SetDamage(int val)
        {
            m_DamageMin = val;
            m_DamageMax = val;
        }

        public void SetDamage(int min, int max)
        {
            m_DamageMin = min;
            m_DamageMax = max;
        }

        public void SetHits(int val)
        {
            if (val < 1000 && !Core.AOS)
            {
                val = (val * 100) / 60;
            }

            m_HitsMax = val;
            Hits = HitsMax;
        }

        public void SetHits(int min, int max)
        {
            if (min < 1000 && !Core.AOS)
            {
                min = (min * 100) / 60;
                max = (max * 100) / 60;
            }

            m_HitsMax = Utility.RandomMinMax(min, max);
            Hits = HitsMax;
            SetAverage(min, max, m_HitsMax);
        }

        public void SetStam(int val)
        {
            m_StamMax = val;
            Stam = StamMax;
        }

        public void SetStam(int min, int max)
        {
            m_StamMax = Utility.RandomMinMax(min, max);
            Stam = StamMax;
            SetAverage(min, max, m_StamMax);
        }

        public void SetMana(int val)
        {
            m_ManaMax = val;
            Mana = ManaMax;
        }

        public void SetMana(int min, int max)
        {
            m_ManaMax = Utility.RandomMinMax(min, max);
            Mana = ManaMax;
            SetAverage(min, max, m_ManaMax);
        }

        public void SetStr(int val)
        {
            RawStr = val;
            Hits = HitsMax;
        }

        public void SetStr(int min, int max)
        {
            RawStr = Utility.RandomMinMax(min, max);
            Hits = HitsMax;
            SetAverage(min, max, RawStr);
        }

        public void SetDex(int val)
        {
            RawDex = val;
            Stam = StamMax;
        }

        public void SetDex(int min, int max)
        {
            RawDex = Utility.RandomMinMax(min, max);
            Stam = StamMax;
            SetAverage(min, max, RawDex);
        }

        public void SetInt(int val)
        {
            RawInt = val;
            Mana = ManaMax;
        }

        public void SetInt(int min, int max)
        {
            RawInt = Utility.RandomMinMax(min, max);
            Mana = ManaMax;
            SetAverage(min, max, RawInt);
        }

        public void SetDamageType(ResistanceType type, int min, int max)
        {
            SetDamageType(type, Utility.RandomMinMax(min, max));
        }

        public void SetDamageType(ResistanceType type, int val)
        {
            switch (type)
            {
                case ResistanceType.Physical:
                    m_PhysicalDamage = val;
                    break;
                case ResistanceType.Fire:
                    m_FireDamage = val;
                    break;
                case ResistanceType.Cold:
                    m_ColdDamage = val;
                    break;
                case ResistanceType.Poison:
                    m_PoisonDamage = val;
                    break;
                case ResistanceType.Energy:
                    m_EnergyDamage = val;
                    break;
            }
        }

        public void SetResistance(ResistanceType type, int value)
        {
            SetResistance(type, value, value);
        }

        public void SetResistance(ResistanceType type, int min, int max)
        {
            int val = min == max ? min : Utility.RandomMinMax(min, max);
            SetAverage(min, max, val);

            switch (type)
            {
                case ResistanceType.Physical: m_PhysicalResistance = val; break;
                case ResistanceType.Fire: m_FireResistance = val; break;
                case ResistanceType.Cold: m_ColdResistance = val; break;
                case ResistanceType.Poison: m_PoisonResistance = val; break;
                case ResistanceType.Energy: m_EnergyResistance = val; break;
            }

            UpdateResistances();
        }

        public void SetSkill(SkillName name, double val)
        {
            Skills[name].BaseFixedPoint = (int)(val * 10);

            if (Skills[name].Base > Skills[name].Cap)
            {
                if (Core.SE)
                {
                    SkillsCap += (Skills[name].BaseFixedPoint - Skills[name].CapFixedPoint);
                }

                Skills[name].Cap = Skills[name].Base;
            }

            if (name == SkillName.Poisoning && Skills[name].Base > 0 && 
                !Controlled &&
                (AbilityProfile == null || !AbilityProfile.HasAbility(MagicalAbility.Poisoning)))
            {
                SetMagicalAbility(MagicalAbility.Poisoning);
            }

            if (!Controlled && name == SkillName.Magery && 
                (AbilityProfile == null || !AbilityProfile.HasAbility(MagicalAbility.Magery)) && 
                Skills[SkillName.Magery].Base > 0 && 
                (AI == AIType.AI_Mage || AI == AIType.AI_Necro || AI == AIType.AI_NecroMage || AI == AIType.AI_Mystic || AI == AIType.AI_Spellweaving))

            {
                SetMagicalAbility(MagicalAbility.Magery);
            }
        }

        public void SetSkill(SkillName name, double min, double max)
        {
            int minFixed = (int)(min * 10);
            int maxFixed = (int)(max * 10);

            Skills[name].BaseFixedPoint = Utility.RandomMinMax(minFixed, maxFixed);

            SetAverage(min, max, Skills[name].BaseFixedPoint / 10);

            if (Skills[name].Base > Skills[name].Cap)
            {
                if (Core.SE)
                {
                    SkillsCap += (Skills[name].BaseFixedPoint - Skills[name].CapFixedPoint);
                }

                Skills[name].Cap = Skills[name].Base;
            }

            if (name == SkillName.Poisoning && Skills[name].Base > 0 && 
                !Controlled &&
                (AbilityProfile == null || !AbilityProfile.HasAbility(MagicalAbility.Poisoning)))
            {
                SetMagicalAbility(MagicalAbility.Poisoning);
            }

            if (!Controlled && name == SkillName.Magery &&
                (AbilityProfile == null || !AbilityProfile.HasAbility(MagicalAbility.Magery)) && 
                Skills[SkillName.Magery].Base > 0 && 
                (AI == AIType.AI_Mage || AI == AIType.AI_Necro || AI == AIType.AI_NecroMage || AI == AIType.AI_Mystic || AI == AIType.AI_Spellweaving))

            {
                SetMagicalAbility(MagicalAbility.Magery);
            }
        }

        public void SetFameLevel(int level)
        {
            switch (level)
            {
                case 1:
                    Fame = Utility.RandomMinMax(0, 1249);
                    break;
                case 2:
                    Fame = Utility.RandomMinMax(1250, 2499);
                    break;
                case 3:
                    Fame = Utility.RandomMinMax(2500, 4999);
                    break;
                case 4:
                    Fame = Utility.RandomMinMax(5000, 9999);
                    break;
                case 5:
                    Fame = Utility.RandomMinMax(10000, 10000);
                    break;
            }
        }

        public void SetKarmaLevel(int level)
        {
            switch (level)
            {
                case 0:
                    Karma = -Utility.RandomMinMax(0, 624);
                    break;
                case 1:
                    Karma = -Utility.RandomMinMax(625, 1249);
                    break;
                case 2:
                    Karma = -Utility.RandomMinMax(1250, 2499);
                    break;
                case 3:
                    Karma = -Utility.RandomMinMax(2500, 4999);
                    break;
                case 4:
                    Karma = -Utility.RandomMinMax(5000, 9999);
                    break;
                case 5:
                    Karma = -Utility.RandomMinMax(10000, 10000);
                    break;
            }
        }
        #endregion

        public static void Cap(ref int val, int min, int max)
        {
            if (val < min)
            {
                val = min;
            }
            else if (val > max)
            {
                val = max;
            }
        }

        public virtual void DropBackpack()
        {
            if (Backpack != null)
            {
                if (Backpack.Items.Count > 0)
                {
                    Backpack b = new CreatureBackpack(Name);

                    var list = new List<Item>(Backpack.Items);
                    foreach (Item item in list)
                    {
                        b.DropItem(item);
                    }

                    BaseHouse house = BaseHouse.FindHouseAt(this);
                    if (house != null)
                    {
                        b.MoveToWorld(house.BanLocation, house.Map);
                    }
                    else
                    {
                        b.MoveToWorld(Location, Map);
                    }
                }
            }
        }

        protected bool m_Spawning;
        protected int m_KillersLuck;

        public virtual void GenerateLoot(bool spawning)
        {
            if (m_NoLootOnDeath)
                return;

            m_Spawning = spawning;

            if (!spawning)
            {
                m_KillersLuck = LootPack.GetLuckChanceForKiller(this);
            }

            GenerateLoot();

            if (m_Paragon)
            {
                if (Fame < 1250)
                {
                    AddLoot(LootPack.Meager);
                }
                else if (Fame < 2500)
                {
                    AddLoot(LootPack.Average);
                }
                else if (Fame < 5000)
                {
                    AddLoot(LootPack.Rich);
                }
                else if (Fame < 10000)
                {
                    AddLoot(LootPack.FilthyRich);
                }
                else
                {
                    AddLoot(LootPack.UltraRich);
                }
            }

            m_Spawning = false;
            m_KillersLuck = 0;
        }

        public virtual void GenerateLoot()
        { }

        public virtual void AddLoot(LootPack pack, int amount)
        {
            for (int i = 0; i < amount; ++i)
            {
                AddLoot(pack);
            }
        }

        public virtual void AddLoot(LootPack pack)
        {
            if (Summoned)
            {
                return;
            }

            Container backpack = Backpack;

            if (backpack == null)
            {
                backpack = new Backpack();

                backpack.Movable = false;

                AddItem(backpack);
            }

            pack.Generate(this, backpack, m_Spawning, m_KillersLuck);
        }

        public static void GetRandomAOSStats(int minLevel, int maxLevel, out int attributeCount, out int min, out int max)
        {
            int v = RandomMinMaxScaled(minLevel, maxLevel);

            if (v >= 5)
            {
                attributeCount = Utility.RandomMinMax(2, 6);
                min = 20;
                max = 70;
            }
            else if (v == 4)
            {
                attributeCount = Utility.RandomMinMax(2, 4);
                min = 20;
                max = 50;
            }
            else if (v == 3)
            {
                attributeCount = Utility.RandomMinMax(2, 3);
                min = 20;
                max = 40;
            }
            else if (v == 2)
            {
                attributeCount = Utility.RandomMinMax(1, 2);
                min = 10;
                max = 30;
            }
            else
            {
                attributeCount = 1;
                min = 10;
                max = 20;
            }
        }

        public static int RandomMinMaxScaled(int min, int max)
        {
            if (min == max)
            {
                return min;
            }

            if (min > max)
            {
                int hold = min;
                min = max;
                max = hold;
            }

            /* Example:
            *    min: 1
            *    max: 5
            *  count: 5
            *
            * total = (5*5) + (4*4) + (3*3) + (2*2) + (1*1) = 25 + 16 + 9 + 4 + 1 = 55
            *
            * chance for min+0 : 25/55 : 45.45%
            * chance for min+1 : 16/55 : 29.09%
            * chance for min+2 :  9/55 : 16.36%
            * chance for min+3 :  4/55 :  7.27%
            * chance for min+4 :  1/55 :  1.81%
            */

            int count = max - min + 1;
            int total = 0, toAdd = count;

            for (int i = 0; i < count; ++i, --toAdd)
            {
                total += toAdd * toAdd;
            }

            int rand = Utility.Random(total);
            toAdd = count;

            int val = min;

            for (int i = 0; i < count; ++i, --toAdd, ++val)
            {
                rand -= toAdd * toAdd;

                if (rand < 0)
                {
                    break;
                }
            }

            return val;
        }

        public void PackGold(int amount)
        {
            if (amount > 0)
            {
                PackItem(new Gold(amount));
            }
        }

        public void PackGold(int min, int max)
        {
            PackGold(Utility.RandomMinMax(min, max));
        }

        public void PackStatue(int min, int max)
        {
            PackStatue(Utility.RandomMinMax(min, max));
        }

        public void PackStatue(int amount)
        {
            for (int i = 0; i < amount; ++i)
            {
                PackStatue();
            }
        }

        public void PackStatue()
        {
            PackItem(Loot.RandomStatue());
        }

        public void PackGem()
        {
            PackGem(1);
        }

        public void PackGem(int min, int max)
        {
            PackGem(Utility.RandomMinMax(min, max));
        }

        public void PackGem(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            Item gem = Loot.RandomGem();

            gem.Amount = amount;

            PackItem(gem);
        }

        public void PackNecroReg(int min, int max)
        {
            PackNecroReg(Utility.RandomMinMax(min, max));
        }

        public void PackNecroReg(int amount)
        {
            for (int i = 0; i < amount; ++i)
            {
                PackNecroReg();
            }
        }

        public void PackNecroReg()
        {
            if (!Core.AOS)
            {
                return;
            }

            PackItem(Loot.RandomNecromancyReagent());
        }

        public void PackReg(int min, int max)
        {
            PackReg(Utility.RandomMinMax(min, max));
        }

        public void PackReg(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            Item reg = Loot.RandomReagent();

            reg.Amount = amount;

            PackItem(reg);
        }

        public void PackBodyPart()
        {
            switch (Utility.Random(5))
            {
                case 0: PackItem(new LeftArm()); break;
                case 1: PackItem(new RightArm()); break;
                case 2: PackItem(new Torso()); break;
                case 3: PackItem(new RightLeg()); break;
                case 4: PackItem(new LeftLeg()); break;
            }
        }

        public void PackBones()
        {
            switch (Utility.Random(6))
            {
                case 0: PackItem(new Bone()); break;
                case 1: PackItem(new RibCage()); break;
                case 2: PackItem(new RibCage()); break;
                case 3: PackItem(new BonePile()); break;
                case 4: PackItem(new BonePile()); break;
                case 5: PackItem(new BonePile()); break;
            }
        }

        public void PackBodyPartOrBones()
        {
            switch (Utility.Random(8))
            {
                case 0: PackItem(new LeftArm()); break;
                case 1: PackItem(new RightArm()); break;
                case 2: PackItem(new Torso()); break;
                case 3: PackItem(new RightLeg()); break;
                case 4: PackItem(new LeftLeg()); break;
                case 5: PackItem(new Bone()); break;
                case 6: PackItem(new RibCage()); break;
                case 7: PackItem(new BonePile()); break;
            }
        }

        public void PackItem(Item item)
        {
            if (Summoned || item == null)
            {
                if (item != null)
                {
                    item.Delete();
                }

                return;
            }

            Container pack = Backpack;

            if (pack == null)
            {
                pack = new Backpack();

                pack.Movable = false;

                AddItem(pack);
            }

            if (!item.Stackable || !pack.TryDropItem(this, item, false)) // try stack
            {
                pack.DropItem(item); // failed, drop it anyway
            }
        }

        public virtual void SetToChampionSpawn()
        {
        }

        public virtual void SetWearable(Item item, int hue = -1, double dropChance = 0.0)
        {
            if (hue > -1)
                item.Hue = hue;

            item.Movable = dropChance > Utility.RandomDouble();

            if (!CheckEquip(item) || !OnEquip(item) || !item.OnEquip(this))
            {
                PackItem(item);
            }
            else
            {
                AddItem(item);
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster && !Body.IsHuman)
            {
                Container pack = Backpack;

                if (pack != null)
                {
                    pack.DisplayTo(from);
                }
            }

            if (DeathAdderCharmable && from.CanBeHarmful(this, false))
            {
                DeathAdder da = SummonFamiliarSpell.Table[from] as DeathAdder;

                if (da != null && !da.Deleted)
                {
                    from.SendAsciiMessage("You charm the snake.  Select a target to attack.");
                    from.Target = new DeathAdderCharmTarget(this);
                }
            }

            base.OnDoubleClick(from);
        }

        private class DeathAdderCharmTarget : Target
        {
            private readonly BaseCreature m_Charmed;

            public DeathAdderCharmTarget(BaseCreature charmed)
                : base(-1, false, TargetFlags.Harmful)
            {
                m_Charmed = charmed;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (!m_Charmed.DeathAdderCharmable || m_Charmed.Combatant != null || !from.CanBeHarmful(m_Charmed, false))
                {
                    return;
                }

                DeathAdder da = SummonFamiliarSpell.Table[from] as DeathAdder;
                if (da == null || da.Deleted)
                {
                    return;
                }

                Mobile targ = targeted as Mobile;
                if (targ == null || !from.CanBeHarmful(targ, false))
                {
                    return;
                }

                from.RevealingAction();
                from.DoHarmful(targ, true);

                m_Charmed.Combatant = targ;

                if (m_Charmed.AIObject != null)
                {
                    m_Charmed.AIObject.Action = ActionType.Combat;
                }
            }
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);

            if (Controlled && !String.IsNullOrEmpty(EngravedText))
            {
                list.Add(1157315, EngravedText); // <BASEFONT COLOR=#668cff>Branded: ~1_VAL~<BASEFONT COLOR=#FFFFFF>
            }

            if (Core.ML)
            {
                if (DisplayWeight)
                {
                    list.Add(TotalWeight == 1 ? 1072788 : 1072789, TotalWeight.ToString()); // Weight: ~1_WEIGHT~ stones
                }

                if (m_ControlOrder == OrderType.Guard)
                {
                    list.Add(1080078); // guarding
                }
            }

            if (Summoned && !IsAnimatedDead && !IsNecroFamiliar && !(this is Clone))
            {
                list.Add(1049646); // (summoned)
            }
            else if (Controlled && Commandable)
            {
                if (this is BaseHire)
                {
                    list.Add(1062030); // (hired)
                }
                else if (IsBonded) //Intentional difference (showing ONLY bonded when bonded instead of bonded & tame)
                {
                    list.Add(1049608); // (bonded)
                }
                else
                {
                    list.Add(502006); // (tame)
                }
            }

            if (IsAmbusher)
                list.Add(1155480); // Ambusher
        }

        public override void OnSingleClick(Mobile from)
        {
            if (Controlled && Commandable)
            {
                int number;

                if (Summoned)
                {
                    number = 1049646; // (summoned)
                }
                else if (IsBonded)
                {
                    number = 1049608; // (bonded)
                }
                else
                {
                    number = 502006; // (tame)
                }

                PrivateOverheadMessage(MessageType.Regular, 0x3B2, number, from.NetState);
            }

            base.OnSingleClick(from);
        }

        public virtual double TreasureMapChance { get { return TreasureMap.LootChance; } }
        public virtual int TreasureMapLevel { get { return -1; } }

        public virtual bool IgnoreYoungProtection { get { return false; } }

        public override bool OnBeforeDeath()
        {
            int treasureLevel = TreasureMapLevel;
            GetLootingRights();

            if (treasureLevel == 1 && Map == Map.Trammel && TreasureMap.IsInHavenIsland(this))
            {
                Mobile killer = LastKiller;

                if (killer is BaseCreature)
                {
                    killer = ((BaseCreature)killer).GetMaster();
                }

                if (killer is PlayerMobile && ((PlayerMobile)killer).Young)
                {
                    treasureLevel = 0;
                }
            }

            if (!Summoned && !NoKillAwards && !IsBonded && !NoLootOnDeath)
            {
                if (treasureLevel >= 0)
                {
                    if (m_Paragon && XmlParagon.GetChestChance(this) > Utility.RandomDouble())
                    {
                        XmlParagon.AddChest(this, treasureLevel);
                    }
                    else if (/*(Map == Map.Felucca || Map == Map.Trammel) &&*/TreasureMapChance >= Utility.RandomDouble())
                    {
                        Map map = Map;

                        if (map == Map.Trammel && Siege.SiegeShard)
                            map = Map.Felucca;

                        PackItem(new TreasureMap(treasureLevel, map));
                    }
                }

                if (m_Paragon && Paragon.ChocolateIngredientChance > Utility.RandomDouble())
                {
                    switch (Utility.Random(4))
                    {
                        case 0:
                            PackItem(new CocoaButter());
                            break;
                        case 1:
                            PackItem(new CocoaLiquor());
                            break;
                        case 2:
                            PackItem(new SackOfSugar());
                            break;
                        case 3:
                            PackItem(new Vanilla());
                            break;
                    }
                }
            }

            if (!Summoned && !NoKillAwards && !m_HasGeneratedLoot && !m_NoLootOnDeath)
            {
                m_HasGeneratedLoot = true;
                GenerateLoot(false);
            }

            if (!NoKillAwards && Region.IsPartOf("Doom"))
            {
                int bones = TheSummoningQuest.GetDaemonBonesFor(this);

                if (bones > 0)
                {
                    PackItem(new DaemonBone(bones));
                }
            }

            if (IsAnimatedDead)
            {
                Effects.SendLocationEffect(Location, Map, 0x3728, 13, 1, 0x461, 4);
            }

            InhumanSpeech speechType = SpeechType;

            if (speechType != null)
            {
                speechType.OnDeath(this);
            }

            if (m_ReceivedHonorContext != null)
            {
                m_ReceivedHonorContext.OnTargetKilled();
            }

            return base.OnBeforeDeath();
        }

        private bool m_NoKillAwards;
        private bool m_NoLootOnDeath;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool NoKillAwards { get { return m_NoKillAwards; } set { m_NoKillAwards = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool NoLootOnDeath { get { return m_NoLootOnDeath; } set { m_NoLootOnDeath = value; } }

        public int ComputeBonusDamage(List<DamageEntry> list, Mobile m)
        {
            int bonus = 0;

            for (int i = list.Count - 1; i >= 0; --i)
            {
                DamageEntry de = list[i];

                if (de.Damager == m || !(de.Damager is BaseCreature))
                {
                    continue;
                }

                BaseCreature bc = (BaseCreature)de.Damager;
                Mobile master = null;

                master = bc.GetMaster();

                if (master == m)
                {
                    bonus += de.DamageGiven;
                }
            }

            return bonus;
        }

        public Mobile GetMaster()
        {
            if (Controlled && ControlMaster != null)
            {
                return ControlMaster;
            }
            else if (Summoned && SummonMaster != null)
            {
                return SummonMaster;
            }

            return null;
        }

        public virtual bool IsMonster
        {
            get
            {
                if (!Controlled)
                    return true;

                var master = GetMaster();

                return master == null || (master is BaseCreature && !((BaseCreature)master).Controlled);
            }
        }

        public virtual bool IsAggressiveMonster
        {
            get
            {
                return IsMonster && (m_FightMode == FightMode.Closest ||
                                     m_FightMode == FightMode.Strongest ||
                                     m_FightMode == FightMode.Weakest ||
                                     m_FightMode == FightMode.Good);
            }
        }

        private class FKEntry
        {
            public Mobile m_Mobile;
            public int m_Damage;

            public FKEntry(Mobile m, int damage)
            {
                m_Mobile = m;
                m_Damage = damage;
            }
        }

        public List<DamageStore> LootingRights { get; set; }

        public bool HasLootingRights(Mobile m)
        {
            if (LootingRights == null)
                return false;

            return LootingRights.FirstOrDefault(ds => ds.m_Mobile == m && ds.m_HasRight) != null;
        }

        public Mobile GetHighestDamager()
        {
            if (LootingRights == null || LootingRights.Count == 0)
                return null;

            return LootingRights[0].m_Mobile;
        }

        public bool IsHighestDamager(Mobile m)
        {
            return LootingRights != null && LootingRights.Count > 0 && LootingRights[0].m_Mobile == m;
        }

        public List<DamageStore> GetLootingRights()
        {
            if (LootingRights != null)
                return LootingRights;

            List<DamageEntry> damageEntries = DamageEntries;
            int hitsMax = HitsMax;

            List<DamageStore> rights = new List<DamageStore>();

            for (int i = damageEntries.Count - 1; i >= 0; --i)
            {
                if (i >= damageEntries.Count)
                {
                    continue;
                }

                DamageEntry de = damageEntries[i];

                if (de.HasExpired)
                {
                    damageEntries.RemoveAt(i);
                    continue;
                }

                int damage = de.DamageGiven;

                var respList = de.Responsible;

                if (respList != null)
                {
                    for (int j = 0; j < respList.Count; ++j)
                    {
                        DamageEntry subEntry = respList[j];
                        Mobile master = subEntry.Damager;

                        if (master == null || master.Deleted || !master.Player)
                        {
                            continue;
                        }

                        bool needNewSubEntry = true;

                        for (int k = 0; needNewSubEntry && k < rights.Count; ++k)
                        {
                            DamageStore ds = rights[k];

                            if (ds.m_Mobile == master)
                            {
                                ds.m_Damage += subEntry.DamageGiven;
                                needNewSubEntry = false;
                            }
                        }

                        if (needNewSubEntry)
                        {
                            rights.Add(new DamageStore(master, subEntry.DamageGiven));
                        }

                        damage -= subEntry.DamageGiven;
                    }
                }

                Mobile m = de.Damager;

                if (m == null || m.Deleted || !m.Player)
                {
                    continue;
                }

                if (damage <= 0)
                {
                    continue;
                }

                bool needNewEntry = true;

                for (int j = 0; needNewEntry && j < rights.Count; ++j)
                {
                    DamageStore ds = rights[j];

                    if (ds.m_Mobile == m)
                    {
                        ds.m_Damage += damage;
                        needNewEntry = false;
                    }
                }

                if (needNewEntry)
                {
                    rights.Add(new DamageStore(m, damage));
                }
            }

            if (rights.Count > 0)
            {
                rights[0].m_Damage = (int)(rights[0].m_Damage * 1.25);
                //This would be the first valid person attacking it.  Gets a 25% bonus.  Per 1/19/07 Five on Friday

                if (rights.Count > 1)
                {
                    rights.Sort(); //Sort by damage
                }

                int topDamage = rights[0].m_Damage;
                int minDamage;

                if (hitsMax >= 3000)
                {
                    minDamage = topDamage / 16;
                }
                else if (hitsMax >= 1000)
                {
                    minDamage = topDamage / 8;
                }
                else if (hitsMax >= 200)
                {
                    minDamage = topDamage / 4;
                }
                else
                {
                    minDamage = topDamage / 2;
                }

                for (int i = 0; i < rights.Count; ++i)
                {
                    DamageStore ds = rights[i];

                    ds.m_HasRight = (ds.m_Damage >= minDamage);
                }
            }

            LootingRights = rights;
            return rights;
        }

        #region Mondain's Legacy
        private bool m_Allured;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Allured { get { return m_Allured; } set { m_Allured = value; } }

        public virtual bool GivesMLMinorArtifact { get { return false; } }
        #endregion

        public virtual void OnRelease(Mobile from)
        {
            if (m_Allured)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(2), Delete);
            }
        }

        public override void OnItemLifted(Mobile from, Item item)
        {
            base.OnItemLifted(from, item);

            InvalidateProperties();
        }

        public virtual void OnKilledBy(Mobile mob)
        {
            if (m_Paragon && XmlParagon.CheckArtifactChance(mob, this))
            {
                XmlParagon.GiveArtifactTo(mob, this);
            }

            EventSink.InvokeOnKilledBy(new OnKilledByEventArgs(this, mob));
        }

        public override void OnDeath(Container c)
        {
            MeerMage.StopEffect(this, false);

            if (IsBonded)
            {
                int sound = GetDeathSound();

                if (sound >= 0)
                {
                    Effects.PlaySound(this, Map, sound);
                }

                Warmode = false;

                Poison = null;
                Combatant = null;

                Hits = 0;
                Stam = 0;
                Mana = 0;

                IsDeadPet = true;
                ControlTarget = ControlMaster;
                ControlOrder = OrderType.Follow;

                ProcessDeltaQueue();
                SendIncomingPacket();
                SendIncomingPacket();

                var aggressors = Aggressors;

                for (int i = 0; i < aggressors.Count; ++i)
                {
                    AggressorInfo info = aggressors[i];

                    if (info.Attacker.Combatant == this)
                    {
                        info.Attacker.Combatant = null;
                    }
                }

                var aggressed = Aggressed;

                for (int i = 0; i < aggressed.Count; ++i)
                {
                    AggressorInfo info = aggressed[i];

                    if (info.Defender.Combatant == this)
                    {
                        info.Defender.Combatant = null;
                    }
                }

                Mobile owner = ControlMaster;

                if (owner == null || owner.Deleted || owner.Map != Map || !owner.InRange(this, 12) || !CanSee(owner) ||
                    !InLOS(owner))
                {
                    if (OwnerAbandonTime == DateTime.MinValue)
                    {
                        OwnerAbandonTime = DateTime.UtcNow;
                    }
                }
                else
                {
                    OwnerAbandonTime = DateTime.MinValue;
                }

                GiftOfLifeSpell.HandleDeath(this);

                CheckStatTimers();
            }
            else
            {
                if (!Summoned && !m_NoKillAwards)
                {
                    int totalFame = Fame / 100;
                    int totalKarma = -Karma / 100;

                    if (Map == Map.Felucca)
                    {
                        totalFame += ((totalFame / 10) * 3);
                        totalKarma += ((totalKarma / 10) * 3);
                    }

                    var list = GetLootingRights();
                    var titles = new List<Mobile>();
                    var fame = new List<int>();
                    var karma = new List<int>();

                    bool givenFactionKill = false;
                    bool givenToTKill = false;
                    bool givenVASKill = false;

                    for (int i = 0; i < list.Count; ++i)
                    {
                        DamageStore ds = list[i];

                        if (!ds.m_HasRight)
                        {
                            continue;
                        }

                        if (GivesFameAndKarmaAward)
                        {
                            Party party = Engines.PartySystem.Party.Get(ds.m_Mobile);

                            if (party != null)
                            {
                                int divedFame = totalFame / party.Members.Count;
                                int divedKarma = totalKarma / party.Members.Count;

                                for (int j = 0; j < party.Members.Count; ++j)
                                {
                                    PartyMemberInfo info = party.Members[j];

                                    if (info != null && info.Mobile != null)
                                    {
                                        int index = titles.IndexOf(info.Mobile);

                                        if (index == -1)
                                        {
                                            titles.Add(info.Mobile);
                                            fame.Add(divedFame);
                                            karma.Add(divedKarma);
                                        }
                                        else
                                        {
                                            fame[index] += divedFame;
                                            karma[index] += divedKarma;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (PetTrainingHelper.Enabled && ds.m_Mobile is PlayerMobile)
                                {
                                    foreach (var pet in ((PlayerMobile)ds.m_Mobile).AllFollowers.Where(p => DamageEntries.Any(de => de.Damager == p)))
                                    {
                                        titles.Add(pet);
                                        fame.Add(totalFame);
                                        karma.Add(totalKarma);
                                    }
                                }

                                titles.Add(ds.m_Mobile);
                                fame.Add(totalFame);
                                karma.Add(totalKarma);
                            }

                        }

                        if (HumilityVirtue.IsInHunt(ds.m_Mobile) && Karma < 0)
                            HumilityVirtue.RegisterKill(ds.m_Mobile, this, list.Count);

                        OnKilledBy(ds.m_Mobile);

                        XmlQuest.RegisterKill(this, ds.m_Mobile);

                        if (!givenFactionKill)
                        {
                            givenFactionKill = true;
                            Faction.HandleDeath(this, ds.m_Mobile);
                        }

                        Region region = ds.m_Mobile.Region;

                        if (!givenToTKill &&
                            (Map == Map.Tokuno || region.IsPartOf("Yomotsu Mines") || region.IsPartOf("Fan Dancer's Dojo")))
                        {
                            givenToTKill = true;
                            TreasuresOfTokuno.HandleKill(this, ds.m_Mobile);
                        }
                        if (!givenVASKill &&
                            (Map == Map.Felucca || region.IsPartOf("Covetous") || region.IsPartOf("Deceit") || region.IsPartOf("Despise")
                            || region.IsPartOf("Destard") || region.IsPartOf("Hythloth") || region.IsPartOf("Shame") || region.IsPartOf("Wrong")))
                        {
                            givenVASKill = true;
                            VirtueArtifactsSystem.HandleKill(this, ds.m_Mobile);
                        }
                        if (region.IsPartOf("Doom Gauntlet") || region.Name == "GauntletRegion")
                        {
                            DemonKnight.HandleKill(this, ds.m_Mobile);
                        }

                        Server.Engines.Points.PointsSystem.HandleKill(this, ds.m_Mobile, i);

                        PlayerMobile pm = ds.m_Mobile as PlayerMobile;

                        if (pm != null)
                        {
                            QuestHelper.CheckCreature(pm, this); // This line moved up...

                            QuestSystem qs = pm.Quest;

                            if (qs != null)
                            {
                                qs.OnKill(this, c);
                            }
                        }
                    }

                    for (int i = 0; i < titles.Count; ++i)
                    {
                        Titles.AwardFame(titles[i], fame[i], true);
                        Titles.AwardKarma(titles[i], karma[i], true);
                    }
                }

                var e = new CreatureDeathEventArgs(this, LastKiller, c);

                EventSink.InvokeCreatureDeath(e);

                if (e.ClearCorpse)
                {
                    var i = c.Items.Count;

                    while (--i >= 0)
                    {
                        if (i < c.Items.Count)
                        {
                            c.Items[i].Delete();
                        }
                    }
                }

                base.OnDeath(c);

                if (DeleteCorpseOnDeath)
                {
                    c.Delete();
                }
            }
        }

        /* To save on cpu usage, RunUO creatures only reacquire creatures under the following circumstances:
        *  - 10 seconds have elapsed since the last time it tried
        *  - The creature was attacked
        *  - Some creatures, like dragons, will reacquire when they see someone move
        *
        * This functionality appears to be implemented on OSI as well
        */

        private long m_NextReacquireTime;

        public long NextReacquireTime { get { return m_NextReacquireTime; } set { m_NextReacquireTime = value; } }

        public virtual TimeSpan ReacquireDelay { get { return TimeSpan.FromSeconds(10.0); } }
        public virtual bool ReacquireOnMovement { get { return false; } }
        public virtual bool AcquireOnApproach { get { return m_Paragon; } }
        public virtual int AcquireOnApproachRange { get { return 10; } }

        public override void OnDelete()
        {
            Mobile m = m_ControlMaster;

            SetControlMaster(null);
            SummonMaster = null;

            if (m_ReceivedHonorContext != null)
            {
                m_ReceivedHonorContext.Cancel();
            }

            base.OnDelete();

            if (m != null)
            {
                m.InvalidateProperties();
            }
        }

        public override bool CanBeHarmful(IDamageable damageable, bool message, bool ignoreOurBlessedness)
        {
            Mobile target = damageable as Mobile;

            if (RecentSetControl && GetMaster() == target)
            {
                return false;
            }

            if (target is BaseFactionGuard)
            {
                return false;
            }

            if ((target is BaseVendor && ((BaseVendor)target).IsInvulnerable) || target is PlayerVendor || target is TownCrier)
            {
                return false;
            }

            if (damageable is IDamageableItem && !((IDamageableItem)damageable).CanDamage)
            {
                return false;
            }

            return base.CanBeHarmful(damageable, message, ignoreOurBlessedness);
        }

        public override bool CanBeRenamedBy(Mobile from)
        {
            bool ret = base.CanBeRenamedBy(from);

            if (Controlled && from == ControlMaster && !from.Region.IsPartOf<Jail>())
            {
                ret = true;
            }

            return ret;
        }

        public bool SetControlMaster(Mobile m)
        {
            if (m == null)
            {
                ControlMaster = null;
                Controlled = false;
                ControlTarget = null;
                ControlOrder = OrderType.None;
                Guild = null;

                Delta(MobileDelta.Noto);
            }
            else
            {
                ISpawner se = Spawner;

                if (se != null && se.UnlinkOnTaming)
                {
                    Spawner.Remove(this);
                    Spawner = null;
                }

                if (m.Followers + ControlSlots > m.FollowersMax)
                {
                    m.SendLocalizedMessage(1049607); // You have too many followers to control that creature.
                    return false;
                }

                CurrentWayPoint = null; //so tamed animals don't try to go back

                Home = Point3D.Zero;

                ControlMaster = m;
                Controlled = true;
                ControlTarget = null;
                ControlOrder = OrderType.Come;
                Guild = null;

                if (m_DeleteTimer != null)
                {
                    m_DeleteTimer.Stop();
                    m_DeleteTimer = null;
                }

                RemoveAggressed(m);
                RemoveAggressor(m);
                m.RemoveAggressed(this);
                m.RemoveAggressor(this);

                if (Combatant != null)
                    Combatant = null;

                if (m.Combatant == this)
                    m.Combatant = null;

                RecentSetControl = true;
                Timer.DelayCall(TimeSpan.FromSeconds(3), () => RecentSetControl = false);

                Delta(MobileDelta.Noto);
            }

            InvalidateProperties();

            return true;
        }

        public bool RecentSetControl { get; set; }

        public virtual void OnAfterTame(Mobile tamer)
        {
        }

        public override void OnRegionChange(Region Old, Region New)
        {
            base.OnRegionChange(Old, New);

            if (Controlled)
            {
                SpawnEntry se = Spawner as SpawnEntry;

                if (se != null && !se.UnlinkOnTaming && (New == null || !New.AcceptsSpawnsFrom(se.Region)))
                {
                    Spawner.Remove(this);
                    Spawner = null;
                }
            }
        }

        public double GetDispelDifficulty()
        {
            double dif = DispelDifficulty;
            if (SummonMaster != null)
                dif += ArcaneEmpowermentSpell.GetDispellBonus(SummonMaster);
            return dif;
        }

        private static bool m_Summoning;

        public static bool Summoning { get { return m_Summoning; } set { m_Summoning = value; } }

        public static bool Summon(BaseCreature creature, Mobile caster, Point3D p, int sound, TimeSpan duration)
        {
            return Summon(creature, true, caster, p, sound, duration);
        }

        public static bool Summon(
            BaseCreature creature, bool controlled, Mobile caster, Point3D p, int sound, TimeSpan duration)
        {
            if (caster.Followers + creature.ControlSlots > caster.FollowersMax)
            {
                caster.SendLocalizedMessage(1049645); // You have too many followers to summon that creature.
                creature.Delete();
                return false;
            }

            m_Summoning = true;

            if (controlled)
            {
                creature.SetControlMaster(caster);
            }

            creature.RangeHome = 10;
            creature.Summoned = true;

            creature.SummonMaster = caster;

            Container pack = creature.Backpack;

            if (pack != null)
            {
                for (int i = pack.Items.Count - 1; i >= 0; --i)
                {
                    if (i >= pack.Items.Count)
                    {
                        continue;
                    }

                    pack.Items[i].Delete();
                }
            }

            creature.SetHits(
                (int)Math.Floor(creature.HitsMax * (1 + ArcaneEmpowermentSpell.GetSpellBonus(caster, false) / 100.0)));

            new UnsummonTimer(caster, creature, duration).Start();
            creature.m_SummonEnd = DateTime.UtcNow + duration;

            creature.MoveToWorld(p, caster.Map);

            Effects.PlaySound(p, creature.Map, sound);

            m_Summoning = false;

            // Skill Masteries
            creature.HitsMaxSeed += MasteryInfo.EnchantedSummoningBonus(creature);
            creature.Hits = creature.HitsMaxSeed;

            return true;
        }

        private static readonly Type[] m_MinorArtifactsMl = new[]
        {
            typeof(AegisOfGrace), typeof(BladeDance), typeof(Bonesmasher), typeof(Boomstick), typeof(FeyLeggings),
            typeof(FleshRipper), typeof(HelmOfSwiftness), typeof(PadsOfTheCuSidhe), typeof(QuiverOfRage),
            typeof(QuiverOfElements), typeof(RaedsGlory), typeof(RighteousAnger), typeof(RobeOfTheEclipse),
            typeof(RobeOfTheEquinox), typeof(SoulSeeker), typeof(TalonBite), typeof(WildfireBow), typeof(Windsong),
			// TODO: Brightsight lenses, Bloodwood spirit, Totem of the void
		};

        public static Type[] MinorArtifactsMl { get { return m_MinorArtifactsMl; } }

        private static bool EnableRummaging = true;

        private const double ChanceToRummage = 0.5; // 50%

        private const double MinutesToNextRummageMin = 1.0;
        private const double MinutesToNextRummageMax = 4.0;

        private const double MinutesToNextChanceMin = 0.25;
        private const double MinutesToNextChanceMax = 0.75;

        private long m_NextRummageTime;

        public virtual bool IsDispellable { get { return Summoned && !IsAnimatedDead; } }

        #region Animate Dead
        public virtual bool CanAnimateDead { get { return false; } }
        public virtual double AnimateChance { get { return 0.05; } }
        public virtual int AnimateScalar { get { return 50; } }
        public virtual TimeSpan AnimateDelay { get { return TimeSpan.FromSeconds(10); } }
        public virtual BaseCreature Animates { get { return null; } }

        private DateTime m_NextAnimateDead = DateTime.UtcNow;

        public virtual void AnimateDead()
        {
            Corpse best = null;

            foreach (Item item in Map.GetItemsInRange(Location, 12))
            {
                Corpse c = null;

                if (item is Corpse)
                {
                    c = (Corpse)item;
                }
                else
                {
                    continue;
                }

                if (c.ItemID != 0x2006 || c.Channeled || c.Owner.GetType() == typeof(PlayerMobile) || c.Owner.GetType() == null ||
                    (c.Owner != null && c.Owner.Fame < 100) ||
                    ((c.Owner != null) && (c.Owner is BaseCreature) &&
                     (((BaseCreature)c.Owner).Summoned || ((BaseCreature)c.Owner).IsBonded)))
                {
                    continue;
                }

                best = c;
                break;
            }

            if (best != null)
            {
                BaseCreature animated = Animates;

                if (animated != null)
                {
                    animated.Tamable = false;
                    animated.MoveToWorld(best.Location, Map);
                    Scale(animated, AnimateScalar);
                    Effects.PlaySound(best.Location, Map, 0x1FB);
                    Effects.SendLocationParticles(
                        EffectItem.Create(best.Location, Map, EffectItem.DefaultDuration), 0x3789, 1, 40, 0x3F, 3, 9907, 0);
                }

                best.ProcessDelta();
                best.SendRemovePacket();
                best.ItemID = Utility.Random(0xECA, 9); // bone graphic
                best.Hue = 0;
                best.ProcessDelta();
            }

            m_NextAnimateDead = DateTime.UtcNow + AnimateDelay;
        }

        public static void Scale(BaseCreature bc, int scalar)
        {
            int toScale;

            toScale = bc.RawStr;
            bc.RawStr = AOS.Scale(toScale, scalar);

            toScale = bc.HitsMaxSeed;

            if (toScale > 0)
            {
                bc.HitsMaxSeed = AOS.Scale(toScale, scalar);
            }

            bc.Hits = bc.Hits; // refresh hits
        }
        #endregion

        #region Healing
        public virtual double HealChance { get { return 0.0; } }
        public virtual bool CanHealOwner { get { return PetTrainingHelper.Enabled; } }
        public virtual double HealScalar { get { return 1.0; } }

        public virtual int HealSound { get { return 0x57; } }
        public virtual int HealStartRange { get { return 2; } }
        public virtual int HealEndRange { get { return RangePerception; } }
        public virtual double HealTrigger { get { return 0.78; } }
        public virtual double HealDelay { get { return 6.5; } }
        public virtual double HealInterval { get { return 0.0; } }
        public virtual bool HealFully { get { return true; } }
        public virtual double HealOwnerTrigger { get { return 0.78; } }
        public virtual double HealOwnerDelay { get { return 6.5; } }
        public virtual double HealOwnerInterval { get { return 30.0; } }
        public virtual bool HealOwnerFully { get { return PetTrainingHelper.Enabled; } }

        private long m_NextHealTime = Core.TickCount;
        private long m_NextHealOwnerTime = Core.TickCount;
        private Timer m_HealTimer;

        public bool IsHealing { get { return (m_HealTimer != null); } }

        public virtual bool CheckHeal()
        {
            long tc = Core.TickCount;

            if (Alive && !IsHealing && !BardPacified)
            {
                Mobile owner = ControlMaster;

                if (owner != null && CanHealOwner && tc >= m_NextHealOwnerTime && CanBeBeneficial(owner, true, true) &&
                    owner.Map == Map && InRange(owner, HealStartRange) && InLOS(owner) && owner.Hits < HealOwnerTrigger * owner.HitsMax)
                {
                    HealStart(owner);
                    m_NextHealOwnerTime = tc + (int)TimeSpan.FromSeconds(HealOwnerInterval).TotalMilliseconds;

                    return true;
                }
                else if (tc >= m_NextHealTime && CanBeBeneficial(this) && (Hits < HealTrigger * HitsMax || Poisoned))
                {
                    HealStart(this);
                    m_NextHealTime = tc + (int)TimeSpan.FromSeconds(HealInterval).TotalMilliseconds;

                    return true;
                }
            }

            return false;
        }

        public virtual void HealStart(Mobile patient)
        {
            bool onSelf = (patient == this);

            //DoBeneficial( patient );

            RevealingAction();

            if (!onSelf)
            {
                patient.RevealingAction();
                patient.SendLocalizedMessage(1008078, false, Name); //  : Attempting to heal you.
            }

            double seconds = (onSelf ? HealDelay : HealOwnerDelay) + (patient.Alive ? 0.0 : 5.0);

            m_HealTimer = Timer.DelayCall(TimeSpan.FromSeconds(seconds), new TimerStateCallback(Heal_Callback), patient);
        }

        private void Heal_Callback(object state)
        {
            if (state is Mobile)
            {
                Heal((Mobile)state);
            }
        }

        public virtual void Heal(Mobile patient)
        {
            if (!Alive || Map == Map.Internal || !CanBeBeneficial(patient, true, true) || patient.Map != Map ||
                !InRange(patient, HealEndRange))
            {
                StopHeal();
                return;
            }

            bool onSelf = (patient == this);

            if (!patient.Alive)
            { }
            else if (patient.Poisoned)
            {
                int poisonLevel = patient.Poison.RealLevel;

                double healing = Skills.Healing.Value;
                double anatomy = Skills.Anatomy.Value;
                double chance = (healing - 30.0) / 50.0 - poisonLevel * 0.1;

                if ((healing >= 60.0 && anatomy >= 60.0) && chance > Utility.RandomDouble())
                {
                    if (patient.CurePoison(this))
                    {
                        patient.SendLocalizedMessage(1010059); // You have been cured of all poisons.

                        CheckSkill(SkillName.Healing, 0.0, 60.0 + poisonLevel * 10.0); // TODO: Verify formula
                        CheckSkill(SkillName.Anatomy, 0.0, Skills[SkillName.Anatomy].Cap);
                    }
                }
            }
            else if (BleedAttack.IsBleeding(patient))
            {
                patient.SendLocalizedMessage(1060167); // The bleeding wounds have healed, you are no longer bleeding!
                BleedAttack.EndBleed(patient, false);
            }
            else
            {
                double healing = Skills.Healing.Value;
                double anatomy = Skills.Anatomy.Value;
                double chance = (healing + 10.0) / 100.0;

                if (chance > Utility.RandomDouble())
                {
                    double min, max;

                    min = (anatomy / 10.0) + (healing / 6.0) + 4.0;
                    max = (anatomy / 8.0) + (healing / 3.0) + 4.0;

                    if (onSelf)
                    {
                        max += 10;
                    }

                    double toHeal = min + (Utility.RandomDouble() * (max - min));

                    toHeal *= HealScalar;

                    patient.Heal((int)toHeal);

                    CheckSkill(SkillName.Healing, 0.0, Skills[SkillName.Healing].Cap);
                    CheckSkill(SkillName.Anatomy, 0.0, Skills[SkillName.Anatomy].Cap);
                }
                else if (PetTrainingHelper.Enabled && Controlled)
                {
                    CheckSkill(SkillName.Healing, 0.0, 10);
                    CheckSkill(SkillName.Anatomy, 0.0, 10);
                }
            }

            HealEffect(patient);

            StopHeal();

            if ((onSelf && HealFully && Hits >= HealTrigger * HitsMax && Hits < HitsMax) ||
                (!onSelf && HealOwnerFully && patient.Hits >= HealOwnerTrigger * patient.HitsMax && patient.Hits < patient.HitsMax))
            {
                HealStart(patient);
            }
        }

        public virtual void StopHeal()
        {
            if (m_HealTimer != null)
            {
                m_HealTimer.Stop();
            }

            m_HealTimer = null;
        }

        public virtual void HealEffect(Mobile patient)
        {
            patient.PlaySound(HealSound);
        }
        #endregion

        #region Damaging Aura
        private long m_NextAura;

        public virtual bool HasAura { get { return false; } }
        public virtual TimeSpan AuraInterval { get { return TimeSpan.FromSeconds(5); } }
        public virtual int AuraRange { get { return 4; } }

        public virtual int AuraBaseDamage { get { return 5; } }
        public virtual int AuraPhysicalDamage { get { return 0; } }
        public virtual int AuraFireDamage { get { return 100; } }
        public virtual int AuraColdDamage { get { return 0; } }
        public virtual int AuraPoisonDamage { get { return 0; } }
        public virtual int AuraEnergyDamage { get { return 0; } }
        public virtual int AuraChaosDamage { get { return 0; } }

        public virtual int GetAuraDamage(Mobile from)
        {
            if(from is PlayerMobile)
                return (int)BalmOfProtection.HandleDamage((PlayerMobile)from, AuraBaseDamage);

            return AuraBaseDamage;
        }

        public virtual void AuraDamage()
        {
            if (!Alive || IsDeadBondedPet)
            {
                return;
            }

            var list = new List<Mobile>();

            IPooledEnumerable eable = GetMobilesInRange(AuraRange);

            foreach (Mobile m in eable)
            {
                if (m != this && SpellHelper.ValidIndirectTarget(this, m) && CanBeHarmful(m, false) && (!Core.AOS || InLOS(m)))
                {
                    list.Add(m);
                }
            }

            eable.Free();

            foreach (Mobile m in list)
            {
                int damage = GetAuraDamage(m);

                AOS.Damage(
                    m,
                    this,
                    damage,
                    AuraPhysicalDamage,
                    AuraFireDamage,
                    AuraColdDamage,
                    AuraPoisonDamage,
                    AuraEnergyDamage,
                    AuraChaosDamage,
                    0,
                    DamageType.SpellAOE);

                m.RevealingAction();
                AuraEffect(m);
            }

            ColUtility.Free(list);
        }

        public virtual void AuraEffect(Mobile m)
        { }
        #endregion

        #region Spawn Position
        public virtual Point3D GetSpawnPosition(int range)
        {
            return GetSpawnPosition(Location, Map, range);
        }

        public static Point3D GetSpawnPosition(Point3D from, Map map, int range)
        {
            if (map == null)
                return from;

            for (int i = 0; i < 10; i++)
            {
                int x = from.X + Utility.Random(range);
                int y = from.Y + Utility.Random(range);
                int z = map.GetAverageZ(x, y);

                if (Utility.RandomBool())
                    x *= -1;

                if (Utility.RandomBool())
                    y *= -1;

                Point3D p = new Point3D(x, y, from.Z);

                if (map.CanSpawnMobile(p) && map.LineOfSight(from, p))
                    return p;

                p = new Point3D(x, y, z);

                if (map.CanSpawnMobile(p) && map.LineOfSight(from, p))
                    return p;
            }

            return from;
        }
        #endregion

        #region Rage
        public virtual bool CanDoRage { get { return false; } }
        public virtual TimeSpan RageDuration { get { return TimeSpan.FromSeconds(5); } }
        public virtual double RageProbability { get { return 0.20; } }
        public virtual int RageHue { get { return 1157; } }

        private bool m_InRage;

        public virtual void DoRage(Mobile attacker)
        {
            m_InRage = true;

            HueMod = RageHue;
            Stam = StamMax;

            Timer.DelayCall(TimeSpan.FromSeconds(.25), DoRageMessage);
        }

        public virtual void DoRageHit(Mobile attacker)
        {
            if (attacker != null && attacker.Alive)
            {
                if (Core.SA)
                {
                    attacker.Animate(AnimationType.Pillage, 0);
                }
                else
                {
                    attacker.Animate(21, 6, 1, true, false, 0);
                }

                PlaySound(0xEE);
                attacker.LocalOverheadMessage(MessageType.Regular, 0x20, 1070696); // You have been stunned by a colossal blow!

                attacker.Frozen = true;
                Timer.DelayCall(RageDuration, () =>
                {
                    attacker.Frozen = false;
                    attacker.Combatant = null;
                    attacker.LocalOverheadMessage(MessageType.Regular, 0x20, 1070695); // You recover your senses.
                    HueMod = -1;
                });
            }
        }

        public virtual void DoRageMessage()
        {
            PublicOverheadMessage(MessageType.Regular, 0x20, 1113587); // The creature goes into a frenzied rage!
        }
        #endregion

        #region Barding Skills
        private long m_NextDiscord;
        private long m_NextPeace;
        private long m_NextProvoke;

        public virtual bool CanDiscord 
        { 
            get 
            {
                if (Controlled && AbilityProfile != null)
                {
                    return AbilityProfile.HasAbility(MagicalAbility.Discordance);
                }

                return false; 
            } 
        }

        public virtual bool CanPeace { get { return false; } }
        public virtual bool CanProvoke { get { return false; } }

        public virtual bool PlayInstrumentSound { get { return true; } }

        public virtual TimeSpan DiscordInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(60, 120)); } }
        public virtual TimeSpan PeaceInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(60, 120)); } }
        public virtual TimeSpan ProvokeInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(60, 120)); } }

        public virtual bool DoDiscord()
        {
            Mobile target = GetBardTarget();

            if (target == null || !target.InLOS(this) || !InRange(target.Location, BaseInstrument.GetBardRange(this, SkillName.Discordance)) || CheckInstrument() == null)
                return false;

            // TODO: get mana
            if (AbilityProfile != null && AbilityProfile.HasAbility(MagicalAbility.Discordance) && Mana < 25)
            {
                return false;
            }
            else
            {
                Mana -= 25;
            }

            if (Spell != null)
                Spell = null;

            if (!UseSkill(SkillName.Discordance))
                return false;

            if (Target is Discordance.DiscordanceTarget)
            {
                Target.Invoke(this, target);
                return true;
            }

            return false;
        }

        public virtual bool DoPeace()
        {
            Mobile target = GetBardTarget();
            if (target == null || !target.InLOS(this) || !InRange(target.Location, BaseInstrument.GetBardRange(this, SkillName.Peacemaking)) || CheckInstrument() == null)
                return false;

            if (Spell != null)
                Spell = null;

            if (!UseSkill(SkillName.Peacemaking))
                return false;

            if (Target is Peacemaking.InternalTarget)
            {
                Target.Invoke(this, target);
                return true;
            }

            return false;
        }

        public virtual bool DoProvoke()
        {
            Mobile target = GetBardTarget();

            if (target == null || !target.InLOS(this) || !InRange(target.Location, BaseInstrument.GetBardRange(this, SkillName.Provocation)) || CheckInstrument() == null || !(target is BaseCreature))
                return false;

            if (Spell != null)
                Spell = null;

            if (!UseSkill(SkillName.Provocation))
                return false;

            if (Target is Provocation.InternalFirstTarget)
            {
                Target.Invoke(this, target);

                if (Target is Provocation.InternalSecondTarget)
                {
                    Mobile second = GetSecondTarget((BaseCreature)target);

                    if (second != null)
                        Target.Invoke(this, second);

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Auto Checks creature for an instrument. Creates if none in pack, and sets for barding skills.
        /// </summary>
        /// <returns></returns>
        public BaseInstrument CheckInstrument()
        {
            BaseInstrument inst = BaseInstrument.GetInstrument(this);

            if (inst == null)
            {
                if (Backpack == null)
                    return null;

                inst = Backpack.FindItemByType(typeof(BaseInstrument)) as BaseInstrument;

                if (inst == null)
                {
                    inst = new Harp();
                    inst.SuccessSound = PlayInstrumentSound ? 0x58B : 0;
                    inst.FailureSound = PlayInstrumentSound ? 0x58C : 0;
                    inst.Movable = false;
                    PackItem(inst);
                }
            }

            BaseInstrument.SetInstrument(this, inst);

            return inst;
        }

        /// <summary>
        /// Default Method to get bard target. Simplisticly gets combatant. Override for a more dynamic way to choosing target
        /// </summary>
        /// <returns></returns>
        public virtual Mobile GetBardTarget(bool creaturesOnly = false)
        {
            Mobile m = Combatant as Mobile;

            if (m == null && GetMaster() is PlayerMobile)
                m = GetMaster().Combatant as Mobile;

            if (m == null || m == this || !CanBeHarmful(m, false) || (creaturesOnly && !(m is BaseCreature)))
            {
                List<AggressorInfo> list = new List<AggressorInfo>();
                list.AddRange(Aggressors.Where(info => !creaturesOnly || info.Attacker is PlayerMobile));

                if (list.Count > 0)
                    m = list[Utility.Random(list.Count)].Attacker;
                else
                    m = null;

                list.Clear();
                list.TrimExcess();
            }

            return m;
        }

        /// <summary>
        /// Used for second Provocation target.
        /// </summary>
        /// <param name="first"></param>
        /// <returns></returns>
        public virtual Mobile GetSecondTarget(BaseCreature first)
        {
            if (first == null)
                return null;

            int range = BaseInstrument.GetBardRange(this, SkillName.Provocation);

            IPooledEnumerable eable = Map.GetMobilesInRange(Location, range);
            List<Mobile> possibles = new List<Mobile>();

            foreach (Mobile m in eable)
            {
                if (m != first && m != this && first.InRange(m.Location, range))
                {
                    if (CanBeHarmful(m, false) && first.CanBeHarmful(m, false))
                        possibles.Add(m);
                }
            }
            eable.Free();

            Mobile t = null;

            if (possibles.Count > 0)
                t = possibles[Utility.Random(possibles.Count)];

            ColUtility.Free(possibles);

            return t;
        }
        #endregion

        #region TeleportTo
        private long m_NextTeleport;

        public virtual bool TeleportsTo { get { return false; } }
        public virtual TimeSpan TeleportDuration { get { return TimeSpan.FromSeconds(5); } }
        public virtual int TeleportRange { get { return 16; } }
        public virtual double TeleportProb { get { return 0.25; } }
        public virtual bool TeleportsPets { get { return false; } }

        private static int[] m_Offsets = new int[]
			{
				-1, -1,
				-1,  0,
				-1,  1,
				0, -1,
				0,  1,
				1, -1,
				1,  0,
				1,  1
			};

        public void TryTeleport()
        {
            if (Deleted)
                return;

            if (TeleportProb > Utility.RandomDouble())
            {
                Mobile toTeleport = GetTeleportTarget();

                if (toTeleport != null)
                {
                    int offset = Utility.Random(8) * 2;

                    Point3D to = Location;

                    for (int i = 0; i < m_Offsets.Length; i += 2)
                    {
                        int x = X + m_Offsets[(offset + i) % m_Offsets.Length];
                        int y = Y + m_Offsets[(offset + i + 1) % m_Offsets.Length];

                        if (Map.CanSpawnMobile(x, y, Z))
                        {
                            to = new Point3D(x, y, Z);
                            break;
                        }
                        else
                        {
                            int z = Map.GetAverageZ(x, y);

                            if (Map.CanSpawnMobile(x, y, z))
                            {
                                to = new Point3D(x, y, z);
                                break;
                            }
                        }
                    }

                    Point3D from = toTeleport.Location;
                    toTeleport.MoveToWorld(to, Map);

                    Server.Spells.SpellHelper.Turn(this, toTeleport);
                    Server.Spells.SpellHelper.Turn(toTeleport, this);

                    toTeleport.ProcessDelta();

                    Effects.SendLocationParticles(EffectItem.Create(from, Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);
                    Effects.SendLocationParticles(EffectItem.Create(to, Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 5023);

                    toTeleport.PlaySound(0x1FE);

                    Combatant = toTeleport;
                }
            }
        }

        public virtual Mobile GetTeleportTarget()
        {
            IPooledEnumerable eable = GetMobilesInRange(TeleportRange);
            List<Mobile> list = new List<Mobile>();

            foreach (Mobile m in eable)
            {
                bool isPet = m is BaseCreature && ((BaseCreature)m).GetMaster() is PlayerMobile;

                if (m != this && (m.Player || (TeleportsPets && isPet)) && CanBeHarmful(m) && CanSee(m))
                {
                    list.Add(m);
                }
            }

            eable.Free();
            Mobile mob = null;

            if (list.Count > 0)
                mob = list[Utility.Random(list.Count)];

            ColUtility.Free(list);
            return mob;
        }
        #endregion

        #region Detect Hidden
        private long _NextDetect;

        public virtual bool CanDetectHidden { get { return Skills[SkillName.DetectHidden].Value > 0; } }

        public virtual int FindPlayerDelayBase { get { return (15000 / Int); } }
        public virtual int FindPlayerDelayMax { get { return 60; } }
        public virtual int FindPlayerDelayMin { get { return 5; } }
        public virtual int FindPlayerDelayHigh { get { return 10; } }
        public virtual int FindPlayerDelayLow { get { return 9; } }

        // This does NOT actually use the skill. Just uses the skill values for a distinct ability.
        public virtual void TryFindPlayer()
        {
            if (Deleted || Map == null)
            {
                return;
            }

            double srcSkill = Skills[SkillName.DetectHidden].Value;

            if (srcSkill <= 0)
            {
                return;
            }

            DetectHidden.OnUse(this);

            if (Target is DetectHidden.InternalTarget)
            {
                Target.Invoke(this, this);
                DebugSay("Checking for hidden players");
            }
            else
            {
                DebugSay("Failed Checking for hidden players");
            }
        }
        #endregion

        public virtual void OnThink()
        {
            long tc = Core.TickCount;

            if (Combatant != null && HasAura && tc >= m_NextAura)
            {
                AuraDamage();
                m_NextAura = tc + (int)AuraInterval.TotalMilliseconds;
            }

            if (Paralyzed || Frozen)
            {
                return;
            }

            if (PetTrainingHelper.Enabled && !Summoned && _Profile != null)
            {
                SpecialAbility.CheckThinkTrigger(this);
                AreaEffect.CheckThinkTrigger(this);
            }

            if (Combatant != null && Core.TOL)
            {
                CheckCastMastery();
            }

            if (EnableRummaging && CanRummageCorpses && !Summoned && !Controlled && tc >= m_NextRummageTime)
            {
                double min, max;

                if (ChanceToRummage > Utility.RandomDouble() && Rummage())
                {
                    min = MinutesToNextRummageMin;
                    max = MinutesToNextRummageMax;
                }
                else
                {
                    min = MinutesToNextChanceMin;
                    max = MinutesToNextChanceMax;
                }

                double delay = min + (Utility.RandomDouble() * (max - min));
                m_NextRummageTime = tc + (int)TimeSpan.FromMinutes(delay).TotalMilliseconds;
            }

            if (ReturnsToHome && IsSpawnerBound() && !InRange(Home, RangeHome))
            {
                if ((Combatant == null) && (Warmode == false) && Utility.RandomDouble() < .10) /* some throttling */
                {
                    m_FailedReturnHome = !Move(GetDirectionTo(Home.X, Home.Y)) ? m_FailedReturnHome + 1 : 0;

                    if (m_FailedReturnHome > 5)
                    {
                        SetLocation(Home, true);

                        m_FailedReturnHome = 0;
                    }
                }
            }
            else
            {
                m_FailedReturnHome = 0;
            }

            Mobile combatant = Combatant as Mobile;

            if (combatant != null && CanDiscord && !Discordance.UnderEffects(combatant) && tc >= m_NextDiscord && 0.33 > Utility.RandomDouble())
            {
                DoDiscord();
                m_NextDiscord = tc + Utility.RandomMinMax(5000, 12500);
            }
            else if (combatant != null && CanPeace && !Peacemaking.UnderEffects(combatant) && tc >= m_NextPeace && 0.33 > Utility.RandomDouble())
            {
                DoPeace();
                m_NextPeace = tc + Utility.RandomMinMax(5000, 12500);
            }
            else if (combatant != null && CanProvoke && tc >= m_NextProvoke && 0.33 > Utility.RandomDouble())
            {
                if (DoProvoke())
                    m_NextProvoke = tc + (int)ProvokeInterval.TotalMilliseconds;
                else
                    m_NextProvoke = tc + (int)TimeSpan.FromSeconds(15).TotalMilliseconds;
            }

            if (combatant != null && TeleportsTo && tc >= m_NextTeleport)
            {
                TryTeleport();
                m_NextTeleport = tc + (int)TeleportDuration.TotalMilliseconds;
            }

            if (CanDetectHidden && Core.TickCount >= _NextDetect)
            {
                TryFindPlayer();

                // Not exactly OSI style, approximation.
                int delay = FindPlayerDelayBase;

                if (delay > FindPlayerDelayMax)
                {
                    delay = FindPlayerDelayMax; // 60s max at 250 int
                }
                else if (delay < FindPlayerDelayMin)
                {
                    delay = FindPlayerDelayMin; // 5s min at 3000 int
                }

                int min = delay * (FindPlayerDelayLow / FindPlayerDelayHigh); // 13s at 1000 int, 33s at 400 int, 54s at <250 int
                int max = delay * (FindPlayerDelayHigh / FindPlayerDelayLow); // 16s at 1000 int, 41s at 400 int, 66s at <250 int

                _NextDetect = Core.TickCount +
                    (int)TimeSpan.FromSeconds(Utility.RandomMinMax(min, max)).TotalMilliseconds;
            }
        }

        public virtual bool Rummage()
        {
            Corpse toRummage = null;

            IPooledEnumerable eable = Map.GetItemsInRange(Location, 2);
            foreach (Item item in eable)
            {
                if (item is Corpse && ((Corpse)item).Items.Count > 0)
                {
                    toRummage = (Corpse)item;
                    break;
                }
            }
            eable.Free();

            if (toRummage == null)
            {
                return false;
            }

            Container pack = Backpack;

            if (pack == null)
            {
                return false;
            }

            var items = toRummage.Items;

            bool rejected;
            LRReason reason;

            for (int i = 0; i < items.Count; ++i)
            {
                Item item = items[Utility.Random(items.Count)];

                Lift(item, item.Amount, out rejected, out reason);

                if (!rejected && Drop(pack, new Point3D(-1, -1, 0)))
                {
                    // *rummages through a corpse and takes an item*
                    PublicOverheadMessage(MessageType.Emote, 0x3B2, 1008086);
                    //TODO: Instancing of Rummaged stuff.
                    return true;
                }
            }

            return false;
        }

        public void Pacify(Mobile master, DateTime endtime)
        {
            BardPacified = true;
            BardEndTime = endtime;
        }

        public override Mobile GetDamageMaster(Mobile damagee)
        {
            if (m_bBardProvoked && damagee == m_bBardTarget)
            {
                return m_bBardMaster;
            }
            else if (m_bControlled && m_ControlMaster != null)
            {
                return m_ControlMaster;
            }
            else if (m_bSummoned && m_SummonMaster != null)
            {
                return m_SummonMaster;
            }

            return base.GetDamageMaster(damagee);
        }

        public void Provoke(Mobile master, Mobile target, bool bSuccess)
        {
            BardProvoked = true;

            if (!Core.ML)
            {
                PublicOverheadMessage(MessageType.Emote, EmoteHue, false, "*looks furious*");
            }

            if (bSuccess)
            {
                PlaySound(GetIdleSound());

                BardMaster = master;
                BardTarget = target;
                Combatant = target;
                BardEndTime = DateTime.UtcNow + TimeSpan.FromSeconds(30.0);

                if (target is BaseCreature)
                {
                    BaseCreature t = (BaseCreature)target;

                    if (t.Unprovokable || (t.IsParagon && BaseInstrument.GetBaseDifficulty(t) >= 160.0))
                    {
                        return;
                    }

                    t.BardProvoked = true;

                    t.BardMaster = master;
                    t.BardTarget = this;
                    t.Combatant = this;
                    t.BardEndTime = DateTime.UtcNow + TimeSpan.FromSeconds(30.0);
                }
                else if (target is PlayerMobile)
                {
                    ((PlayerMobile)target).Combatant = this;
                    Combatant = target;
                }
            }
            else
            {
                PlaySound(GetAngerSound());

                BardMaster = master;
                BardTarget = target;
            }
        }

        public bool FindMyName(string str, bool bWithAll)
        {
            int i, j;

            string name = Name;

            if (name == null || str.Length < name.Length)
            {
                return false;
            }

            var wordsString = str.Split(' ');
            var wordsName = name.Split(' ');

            for (j = 0; j < wordsName.Length; j++)
            {
                string wordName = wordsName[j];

                bool bFound = false;
                for (i = 0; i < wordsString.Length; i++)
                {
                    string word = wordsString[i];

                    if (Insensitive.Equals(word, wordName))
                    {
                        bFound = true;
                    }

                    if (bWithAll && Insensitive.Equals(word, "all"))
                    {
                        return true;
                    }
                }

                if (!bFound)
                {
                    return false;
                }
            }

            return true;
        }

        public static void TeleportPets(Mobile master, Point3D loc, Map map)
        {
            TeleportPets(master, loc, map, false);
        }

        public static void TeleportPets(Mobile master, Point3D loc, Map map, bool onlyBonded)
        {
            var move = new List<Mobile>();

            IPooledEnumerable eable = master.GetMobilesInRange(3);
            foreach (Mobile m in eable)
            {
                if (m is BaseCreature)
                {
                    BaseCreature pet = (BaseCreature)m;

                    if (pet.Controlled && pet.ControlMaster == master)
                    {
                        if (!onlyBonded || pet.IsBonded)
                        {
                            if (pet.ControlOrder == OrderType.Guard || pet.ControlOrder == OrderType.Follow ||
                                pet.ControlOrder == OrderType.Come)
                            {
                                move.Add(pet);
                            }
                        }
                    }
                }
            }

            eable.Free();

            foreach (Mobile m in move)
            {
                m.MoveToWorld(loc, map);
            }

            ColUtility.Free(move);
        }

        public virtual void ResurrectPet()
        {
            if (!IsDeadPet)
            {
                return;
            }

            OnBeforeResurrect();

            Poison = null;

            Warmode = false;

            Hits = 10;
            Stam = StamMax;
            Mana = 0;

            ProcessDeltaQueue();

            IsDeadPet = false;

            Effects.SendPacket(Location, Map, new BondedStatus(0, Serial, 0));

            SendIncomingPacket();
            SendIncomingPacket();

            OnAfterResurrect();

            Mobile owner = ControlMaster;

            if (owner == null || owner.Deleted || owner.Map != Map || !owner.InRange(this, 12) || !CanSee(owner) || !InLOS(owner))
            {
                if (OwnerAbandonTime == DateTime.MinValue)
                {
                    OwnerAbandonTime = DateTime.UtcNow;
                }
            }
            else
            {
                OwnerAbandonTime = DateTime.MinValue;
            }

            CheckStatTimers();
        }

        public override bool CanBeDamaged()
        {
            if (IsDeadPet || IsInvulnerable)
            {
                return false;
            }

            return base.CanBeDamaged();
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool PlayerRangeSensitive { get { return CurrentWayPoint == null && (_NavPoints == null || _NavPoints.Count == 0); } }
        //If they are following a waypoint, they'll continue to follow it even if players aren't around

        /* until we are sure about who should be getting deleted, move them instead */
        /* On OSI, they despawn */

        private bool m_ReturnQueued;

        private bool IsSpawnerBound()
        {
            if ((Map != null) && (Map != Map.Internal))
            {
                if (FightMode != FightMode.None && (RangeHome >= 0))
                {
                    if (!Controlled && !Summoned)
                    {
                        if (Spawner != null && Spawner is Spawner && ((Spawner as Spawner).Map) == Map)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public virtual bool ReturnsToHome { get { return (m_SeeksHome && (Home != Point3D.Zero) && !m_ReturnQueued && !Controlled && !Summoned); } }

        public override void OnSectorDeactivate()
        {
            if (!Deleted && ReturnsToHome && IsSpawnerBound() && !InRange(Home, (RangeHome + 5)))
            {
                Timer.DelayCall(TimeSpan.FromSeconds((Utility.Random(45) + 15)), GoHome_Callback);

                m_ReturnQueued = true;
            }
            else if (PlayerRangeSensitive && m_AI != null)
            {
                m_AI.Deactivate();
            }

            base.OnSectorDeactivate();
        }

        public void GoHome_Callback()
        {
            if (m_ReturnQueued && IsSpawnerBound())
            {
                if (!((Map.GetSector(X, Y)).Active))
                {
                    SetLocation(Home, true);

                    if (!((Map.GetSector(X, Y)).Active) && m_AI != null)
                    {
                        m_AI.Deactivate();
                    }
                }
            }

            m_ReturnQueued = false;
        }

        public override void OnSectorActivate()
        {
            if (PlayerRangeSensitive && m_AI != null)
            {
                m_AI.Activate();
            }

            base.OnSectorActivate();
        }

        private bool m_RemoveIfUntamed;

        // used for deleting untamed creatures [in houses]
        private int m_RemoveStep;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool RemoveIfUntamed { get { return m_RemoveIfUntamed; } set { m_RemoveIfUntamed = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RemoveStep { get { return m_RemoveStep; } set { m_RemoveStep = value; } }

        // used for deleting untamed creatures [on save]
        private bool m_RemoveOnSave;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool RemoveOnSave { get { return m_RemoveOnSave; } set { m_RemoveOnSave = value; } }
    }

    public class LoyaltyTimer : Timer
    {
        private static readonly TimeSpan InternalDelay = TimeSpan.FromMinutes(5.0);

        public static void Initialize()
        {
            new LoyaltyTimer().Start();
        }

        public LoyaltyTimer()
            : base(InternalDelay, InternalDelay)
        {
            m_NextHourlyCheck = DateTime.UtcNow + TimeSpan.FromHours(1.0);
            Priority = TimerPriority.FiveSeconds;
        }

        private DateTime m_NextHourlyCheck;

        protected override void OnTick()
        {
            if (DateTime.UtcNow >= m_NextHourlyCheck)
            {
                m_NextHourlyCheck = DateTime.UtcNow + TimeSpan.FromHours(1.0);
            }
            else
            {
                return;
            }

            var toRelease = new List<BaseCreature>();

            // added array for wild creatures in house regions to be removed
            var toRemove = new List<BaseCreature>();

            Parallel.ForEach(
                World.Mobiles.Values,
                m =>
                {
                    if (m is BaseMount && ((BaseMount)m).Rider != null)
                    {
                        ((BaseCreature)m).OwnerAbandonTime = DateTime.MinValue;

                        return;
                    }

                    if (m is BaseCreature)
                    {
                        BaseCreature c = (BaseCreature)m;

                        if (c.IsDeadPet)
                        {
                            Mobile owner = c.ControlMaster;

                            if (!c.IsStabled &&
                                (owner == null || owner.Deleted || owner.Map != c.Map || !owner.InRange(c, 12) || !c.CanSee(owner) ||
                                 !c.InLOS(owner)))
                            {
                                if (c.OwnerAbandonTime == DateTime.MinValue)
                                {
                                    c.OwnerAbandonTime = DateTime.UtcNow;
                                }
                                else if ((c.OwnerAbandonTime + c.BondingAbandonDelay) <= DateTime.UtcNow)
                                {
                                    toRemove.Add(c);
                                }
                            }
                            else
                            {
                                c.OwnerAbandonTime = DateTime.MinValue;
                            }
                        }
                        else if (c.Controlled && c.Commandable)
                        {
                            c.OwnerAbandonTime = DateTime.MinValue;

                            if (c.Map != Map.Internal)
                            {
                                c.Loyalty -= (BaseCreature.MaxLoyalty / 10);

                                if (c.Loyalty < (BaseCreature.MaxLoyalty / 10))
                                {
                                    c.Say(1043270, c.Name); // * ~1_NAME~ looks around desperately *
                                    c.PlaySound(c.GetIdleSound());
                                }

                                if (c.Loyalty <= 0)
                                {
                                    toRelease.Add(c);
                                }
                            }
                        }

                        // added lines to check if a wild creature in a house region has to be removed or not
                        if (!c.Controlled && !c.IsStabled &&
                            ((c.Region.IsPartOf<HouseRegion>() && c.CanBeDamaged()) || (c.RemoveIfUntamed && c.Spawner == null)))
                        {
                            c.RemoveStep++;

                            if (c.RemoveStep >= 20)
                            {
                                lock (toRemove)
                                    toRemove.Add(c);
                            }
                        }
                        else
                        {
                            c.RemoveStep = 0;
                        }
                    }
                });

            foreach (BaseCreature c in toRelease.Where(c => c != null))
            {
                if (c.IsDeadBondedPet)
                {
                    c.Delete();
                    continue;
                }

                c.Say(1043255, c.Name); // ~1_NAME~ appears to have decided that is better off without a master!
                c.Loyalty = BaseCreature.MaxLoyalty; // Wonderfully Happy
                c.IsBonded = false;
                c.BondingBegin = DateTime.MinValue;
                c.OwnerAbandonTime = DateTime.MinValue;
                c.ControlTarget = null;
                //c.ControlOrder = OrderType.Release;
                c.AIObject.DoOrderRelease();
                // this will prevent no release of creatures left alone with AI disabled (and consequent bug of Followers)
                c.DropBackpack();
                c.RemoveOnSave = true;
            }

            // added code to handle removing of wild creatures in house regions
            foreach (BaseCreature c in toRemove)
            {
                c.Delete();
            }

            ColUtility.Free(toRelease);
            ColUtility.Free(toRemove);
        }
    }

    public sealed class PetWindow : Packet
    {
        public PetWindow(PlayerMobile owner, Mobile pet)
            : base(0x31)
        {
            int count = owner.AllFollowers.Count;

            EnsureCapacity(6 + (6 * count));

            m_Stream.Write(owner.Serial);
            m_Stream.Write((byte)count);

            for (int i = 0; i < owner.AllFollowers.Count; i++)
            {
                m_Stream.Write(owner.AllFollowers[i].Serial);
                m_Stream.Write((byte)0x01);
            }
        }
    }
}
