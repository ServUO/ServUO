#region References
using Server.ContextMenus;
using Server.Engines.PartySystem;
using Server.Engines.Quests.Doom;
using Server.Engines.VvV;
using Server.Items;
using Server.Misc;
using Server.Multis;
using Server.Network;
using Server.Prompts;
using Server.Regions;
using Server.Services.Virtues;
using Server.SkillHandlers;
using Server.Spells;
using Server.Spells.Bushido;
using Server.Spells.Necromancy;
using Server.Spells.Sixth;
using Server.Spells.SkillMasteries;
using Server.Spells.Spellweaving;
using Server.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        Metal = 0x0040,
        BlackrockStew = 0x0080
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
        Rotworm,
        DinoRibs,
        SeaSerpentSteak
    }

    public enum HideType
    {
        Regular,
        Spined,
        Horned,
        Barbed
    }

    public enum FurType
    {
        None,
        Green,
        LightBrown,
        Yellow,
        Brown
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

    public enum LootStage
    {
        Spawning,
        Stolen,
        Death
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

        public TextDefinition FriendlyName => m_FriendlyName;

        public FriendlyNameAttribute(TextDefinition friendlyName)
        {
            m_FriendlyName = friendlyName;
        }

        public static TextDefinition GetFriendlyNameFor(Type t)
        {
            if (t.IsDefined(typeof(FriendlyNameAttribute), false))
            {
                object[] objs = t.GetCustomAttributes(typeof(FriendlyNameAttribute), false);

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

        private bool _LockDirection;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool LockDirection
        {
            get
            {
                if (AIObject == null)
                {
                    return _LockDirection;
                }

                return AIObject.DirectionLocked = _LockDirection;
            }
            set
            {
                _LockDirection = value;

                if (AIObject != null)
                {
                    AIObject.DirectionLocked = value;
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool CanMove { get; set; }

        public virtual bool CanCallGuards => !Deleted && Alive && !AlwaysMurderer && Kills < 5 && (Player || Body.IsHuman);

        #region Var declarations
        private BaseAI m_AI; // THE AI

        private AIType m_CurrentAI; // The current AI
        private AIType m_DefaultAI; // The default AI

        private FightMode m_FightMode; // The style the mob uses

        private int m_iRangePerception; // The view area
        private int m_iRangeFight; // The fight distance

        private bool m_bDebugAI; // Show debug AI messages

        private int m_iTeam; // Monster Team

        private double m_ForceActiveSpeed;
        private double m_ForcePassiveSpeed;

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

        public virtual InhumanSpeech SpeechType => null;

        public virtual bool ForceStayHome => false;

        public int FollowRange { get; set; }

        public virtual bool CanBeParagon => true;

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

        public DateTime SummonEnd { get { return m_SummonEnd; } set { m_SummonEnd = value; } }

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

        public virtual bool IsBondable => (BondingEnabled && !Summoned && !m_Allured && !(this is IRepairableMobile));
        public virtual TimeSpan BondingDelay => TimeSpan.FromDays(7.0);
        public virtual TimeSpan BondingAbandonDelay => TimeSpan.FromDays(1.0);

        public override bool CanRegenHits => !m_IsDeadPet && !Summoned && base.CanRegenHits;
        public override bool CanRegenStam => !IsParagon && !m_IsDeadPet && base.CanRegenStam;
        public override bool CanRegenMana => !m_IsDeadPet && base.CanRegenMana;

        public override TimeSpan CorpseDecayTime { get { return IsChampionSpawn ? TimeSpan.FromMinutes(1) : base.CorpseDecayTime; } }

        public override bool IsDeadBondedPet => m_IsDeadPet;

        private bool m_IsBonded;
        private bool m_IsDeadPet;
        private DateTime m_BondingBegin;
        private DateTime m_OwnerAbandonTime;
        private DateTime m_DeleteTime;

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

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime DeleteTime
        {
            get { return m_DeleteTime; }
            set
            {
                m_DeleteTime = value;

                if (m_DeleteTime != DateTime.MinValue)
                {
                    CreatureDeleteTimer.RegisterTimer(this);
                }
                else
                {
                    CreatureDeleteTimer.RemoveFromTimer(this);
                }
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
        public static double MaxTameRequirement = 108.0;

        private AbilityProfile _Profile;
        private TrainingProfile _TrainingProfile;

        [CommandProperty(AccessLevel.GameMaster)]
        public AbilityProfile AbilityProfile { get { return _Profile; } set { _Profile = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public TrainingProfile TrainingProfile { get { return _TrainingProfile; } set { _TrainingProfile = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public double BardingDifficulty => BaseInstrument.GetBaseDifficulty(this);

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

        public virtual TrainingDefinition TrainingDefinition => null;

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

            if (HealChance > 0.0 && HealChance >= Utility.RandomDouble())
            {
                SetSpecialAbility(SpecialAbility.Heal);
            }

            if (Skills[SkillName.Focus].Value == 0)
                SetSkill(SkillName.Focus, 2, 20);

            if (Skills[SkillName.DetectHidden].Value == 0 && !(this is BaseVendor))
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

        public void RemoveMagicalAbility(MagicalAbility ability)
        {
            PetTrainingHelper.GetAbilityProfile(this, true).RemoveAbility(ability);
        }

        public void RemoveSpecialAbility(SpecialAbility ability)
        {
            PetTrainingHelper.GetAbilityProfile(this, true).RemoveAbility(ability);
        }

        public void RemoveAreaEffect(AreaEffect ability)
        {
            PetTrainingHelper.GetAbilityProfile(this, true).RemoveAbility(ability);
        }

        public void RemoveWeaponAbility(WeaponAbility ability)
        {
            PetTrainingHelper.GetAbilityProfile(this, true).RemoveAbility(ability);
        }

        public bool HasAbility(object o)
        {
            return PetTrainingHelper.GetAbilityProfile(this, true).HasAbility(o);
        }

        public virtual double AverageThreshold => 0.33;

        public List<double> _InitAverage;

        private void SetAverage(double min, double max, double value)
        {
            if (CanLowerSlot() && max > min)
            {
                if (_InitAverage == null)
                    _InitAverage = new List<double>();

                _InitAverage.Add((value - min) / (max - min));
            }
        }

        public static Type[] SlotLowerables => _SlotLowerables;
        private static readonly Type[] _SlotLowerables =
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
            TrainingDefinition def = PetTrainingHelper.GetTrainingDefinition(this);

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

            if (total / _InitAverage.Count <= AverageThreshold)
            {
                ControlSlotsMin = Math.Max(1, ControlSlotsMin - 1);
            }

            ColUtility.Free(_InitAverage);
            _InitAverage = null;
        }

        public void AdjustTameRequirements()
        {
            if (ControlSlots <= ControlSlotsMin)
            {
                CurrentTameSkill = MinTameSkill;
            }
            else
            {
                CurrentTameSkill = ((ControlSlots - ControlSlotsMin) * 21) + 1;
            }

            if (CurrentTameSkill > MaxTameRequirement)
            {
                CurrentTameSkill = MaxTameRequirement;
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
                SkillName old = _Mastery;
                _Mastery = value;

                if (old != _Mastery)
                {
                    UpdateMasteryInfo();
                }
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
                MasteryInfo[] masteries = MasteryInfo.Infos.Where(i => i.MasterySkill == _Mastery && !i.Passive && (i.SpellType != typeof(BodyGuardSpell) || Controlled)).ToArray();

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
                MasteryInfo info = Masteries[Utility.Random(Masteries.Length)];

                if (info != null)
                {
                    if (info.SpellType.IsSubclassOf(typeof(SkillMasteryMove)))
                    {
                        SpecialMove move = SpellRegistry.GetSpecialMove(info.SpellID);

                        if (move != null)
                        {
                            SpecialMove.SetCurrentMove(this, move);
                            NextMastery = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(10, 60));
                        }
                    }
                    else
                    {
                        Spell spell = SpellRegistry.NewSpell(info.SpellID, this, null);

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

        #region Soulbound
        private bool _IsSoulBound;

        public bool IsSoulBound
        {
            get
            {
                if (!IsSoulboundEnemies)
                {
                    return false;
                }

                return _IsSoulBound || _SoulboundCreatures.Any(c => c == GetType());
            }
            set
            {
                if (IsSoulboundEnemies)
                {
                    _IsSoulBound = value;
                }
            }
        }

        public static bool IsSoulboundEnemies => Engines.Fellowship.ForsakenFoesEvent.Instance.Running;

        public static Type[] _SoulboundCreatures =
        {
            typeof(MerchantCaptain), typeof(PirateCrew), typeof(PirateCaptain), typeof(MerchantCrew), typeof(Osiredon), typeof(Charydbis), typeof(CorgulTheSoulBinder),
        };
        #endregion

        public virtual double WeaponAbilityChance => 0.4;

        public virtual WeaponAbility GetWeaponAbility()
        {
            return null;
        }

        #region Elemental Resistance/Damage
        public override int BasePhysicalResistance => m_PhysicalResistance;
        public override int BaseFireResistance => m_FireResistance;
        public override int BaseColdResistance => m_ColdResistance;
        public override int BasePoisonResistance => m_PoisonResistance;
        public override int BaseEnergyResistance => m_EnergyResistance;

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
                    Paragon.Convert(this);
                }
                else
                {
                    Paragon.UnConvert(this);
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
                if (m_IsChampionSpawn != value)
                {
                    if (!m_IsChampionSpawn && value)
                        SetToChampionSpawn();

                    m_IsChampionSpawn = value;

                    OnChampionSpawnChange();
                }
            }
        }

        protected virtual void OnChampionSpawnChange()
        { }

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
                if (Deleted)
                    return;

                var c = base.Combatant;

                if (c == value)
                    return;

                if (AttacksFocus)
                {
                    Mobile focus = InitialFocus;

                    if (c != null)
                    {
                        if (focus != null && focus != value && InRange(focus.Location, RangePerception) && CanSee(focus))
                            value = focus;
                    }
                    else
                    {
                        if (focus == null && value is Mobile m)
                            InitialFocus = m;
                    }
                }
                else
                    InitialFocus = null;

                base.Combatant = value;

                if (Controlled)
                {
                    AdjustSpeeds();
                }
            }
        }

        public bool IsAmbusher { get; set; }

        public virtual FoodType FavoriteFood => FoodType.Meat;
        public virtual PackInstinct PackInstinct => PackInstinct.None;

        public List<Mobile> Owners => m_Owners;

        public virtual bool AllowMaleTamer => true;
        public virtual bool AllowFemaleTamer => true;
        public virtual bool SubdueBeforeTame => false;
        public virtual bool StatLossAfterTame => SubdueBeforeTame;
        public virtual bool ReduceSpeedWithDamage => true;
        public virtual bool IsSubdued => SubdueBeforeTame && (Hits < ((double)HitsMax / 10));

        public virtual bool Commandable => true;

        public virtual Poison HitPoison => null;
        public virtual double HitPoisonChance => 0.5;
        public virtual Poison PoisonImmune => null;

        public virtual bool BardImmune => false;
        public virtual bool Unprovokable => BardImmune || m_IsDeadPet;
        public virtual bool Uncalmable => BardImmune || m_IsDeadPet;
        public virtual bool AreaPeaceImmune => BardImmune || m_IsDeadPet;

        public virtual bool BleedImmune => false;
        public virtual double BonusPetDamageScalar => 1.0;
        public virtual bool AllureImmune => false;

        public virtual bool DeathAdderCharmable => false;

        public virtual bool GivesFameAndKarmaAward => true;

        //TODO: Find the pub 31 tweaks to the DispelDifficulty and apply them of course.
        public virtual double DispelDifficulty => 0.0; // at this skill level we dispel 50% chance
        public virtual double DispelFocus => 20.0;
        // at difficulty - focus we have 0%, at difficulty + focus we have 100%
        public virtual bool DisplayWeight => Backpack is StrongBackpack;

        public virtual double TeleportChance => 0.05;
        public virtual bool AttacksFocus => false;
        public virtual bool ShowSpellMantra => false;
        public virtual bool FreezeOnCast => ShowSpellMantra;
        public virtual bool CanFly => false;

        public virtual bool CanAutoStable
        {
            get
            {
                if (!(ControlMaster is PlayerMobile))
                    return false;

                if (Allured || Summoned)
                    return false;

                if (this is IMount && ((IMount)this).Rider != null)
                    return false;

                return true;
            }
        }

        public bool IsGolem => this is IRepairableMobile && GetMaster() != null;

        public virtual bool TaintedLifeAura => false;
        public virtual bool BreathImmune => false;

        #region Spill Acid
        public void SpillAcid(int Amount)
        {
            SpillAcid(null, Amount);
        }

        public void SpillAcid(Mobile target, int amount)
        {
            if ((target != null && target.Map == null) || Map == null)
            {
                return;
            }

            for (int i = 0; i < amount; ++i)
            {
                Point3D loc = Location;
                Map map = Map;
                Item acid = NewHarmfulItem();

                if (target != null && target.Map != null && amount == 1)
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

                        if (!map.CanFit(loc, 16, false, false))
                        {
                            SpellHelper.AdjustField(ref loc, map, 16, true);
                        }

                        validLocation = map.CanFit(loc, 16, false, false);
                    }
                }

                acid.MoveToWorld(loc, map);
            }
        }

        public virtual Item NewHarmfulItem()
        {
            return new PoolOfAcid(TimeSpan.FromSeconds(10), 30, 30);
        }
        #endregion

        public virtual void OnDrainLife(Mobile victim)
        {
        }

        #region Flee!!!
        public virtual bool CanFlee => !m_Paragon && !GivesMLMinorArtifact && !SlayerGroup.GetEntryByName(SlayerName.Silver).Slays(this);
        public virtual double FleeChance => 0.25;
        public virtual double BreakFleeChance => 0.85;

        public long NextFleeCheck { get; set; }
        public DateTime ForceFleeUntil { get; set; }

        public bool CheckCanFlee()
        {
            if (ForceFleeUntil != DateTime.MinValue)
            {
                if (ForceFleeUntil < DateTime.UtcNow)
                {
                    ForceFleeUntil = DateTime.MinValue;
                }
                else
                {
                    return true;
                }
            }

            if (!CanFlee || NextFleeCheck > Core.TickCount)
            {
                return false;
            }

            NextFleeCheck = Core.TickCount + 1000;

            return CheckFlee() && FleeChance > Utility.RandomDouble();
        }

        public virtual bool CheckBreakFlee()
        {
            if ((ForceFleeUntil != DateTime.MinValue && ForceFleeUntil > DateTime.UtcNow) || Hits < HitsMax / 2)
            {
                return false;
            }

            bool caster = AI == AIType.AI_Mage || AI == AIType.AI_NecroMage || AI == AIType.AI_Spellbinder || AI == AIType.AI_Spellweaving || AI == AIType.AI_Mystic;

            return !caster || Mana > 20 || Mana == ManaMax;
        }

        public virtual bool CheckFlee()
        {
            return Hits <= (HitsMax * 16) / 100;
        }

        public virtual bool BreakFlee()
        {
            NextFleeCheck = Core.TickCount + Utility.RandomMinMax(2500, 10000);

            return true;
        }
        #endregion

        public virtual bool IsInvulnerable => false;

        public BaseAI AIObject => m_AI;

        public const int MaxOwners = 5;

        // Tribe Opposition (Replaces Opposition Group
        public virtual TribeType Tribe => TribeType.None; // What opposition list am I in?

        public virtual bool IsTribeEnemy(Mobile m)
        {
            // Target must be BaseCreature
            if (!(m is BaseCreature))
            {
                return false;
            }

            BaseCreature c = (BaseCreature)m;

            switch (Tribe)
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
        public List<Mobile> Friends => m_Friends;

        public virtual bool AllowNewPetFriend => (m_Friends == null || m_Friends.Count < 5);

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
            if (Tribe != TribeType.None && IsTribeEnemy(m))
            {
                return false;
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

            return ((m_bSummoned || m_bControlled) == (c.m_bSummoned || c.m_bControlled));
        }
        #endregion

        public virtual bool IsEnemy(Mobile m)
        {
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

            if (Tribe != TribeType.None && IsTribeEnemy(m))
            {
                return true;
            }

            BaseCreature c = m as BaseCreature;

            // Are we a non-aggressive FightMode or are they an uncontrolled Summon?
            if (FightMode == FightMode.Aggressor || FightMode == FightMode.Evil || FightMode == FightMode.Good ||
                (c != null && c.m_bSummoned && !c.m_bControlled && c.SummonMaster != null))
            {
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

            if (c == null)
            {
                return true;
            }
            else
            {
                Mobile master = c.GetMaster();

                if (master != null && !(master is BaseCreature))
                {
                    return true;
                }
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

            // If I'm summoned/controlled and they aren't summoned/controlled, they are my enemy
            // If I'm not summoned/controlled and they are summoned/controlled, they are my enemy
            // Summoned creatures must have masters to count as summoned here
            return (((t.m_bSummoned && t.SummonMaster != null) || t.m_bControlled) !=
                ((c.m_bSummoned && c.SummonMaster != null) || c.m_bControlled));
        }

        public override string ApplyNameSuffix(string suffix)
        {
            if (IsParagon && !GivesMLMinorArtifact)
            {
                if (suffix.Length == 0)
                {
                    suffix = "(Paragon)";
                }
                else
                {
                    suffix = string.Concat(suffix, " (Paragon)");
                }
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

            Animate(AnimationType.Alert, 0);

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

            if (dMinTameSkill > -24.9 && DarkWolfFamiliar.CheckMastery(m, this))
            {
                dMinTameSkill = -24.9;
            }

            int taming = (int)((useBaseSkill ? m.Skills[SkillName.AnimalTaming].Base : m.Skills[SkillName.AnimalTaming].Value) * 10);
            int lore = (int)((useBaseSkill ? m.Skills[SkillName.AnimalLore].Base : m.Skills[SkillName.AnimalLore].Value) * 10);
            int bonus = 0, chance = 700;

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

            return ((double)chance / 1000);
        }

        public static readonly TimeSpan DeleteTimeSpan = TimeSpan.FromDays(3);

        public virtual void BeginDeleteTimer()
        {
            DeleteTime = DateTime.UtcNow + DeleteTimeSpan;
        }

        public virtual void StopDeleteTimer()
        {
            DeleteTime = DateTime.MinValue;
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

        public virtual bool IsAnimatedDead => Summoned && m_AnimateDeadTypes.Any(t => t == GetType());

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

        public override int Damage(int amount, Mobile from)
        {
            return Damage(amount, from, false, false);
        }

        public override int Damage(int amount, Mobile from, bool informMount)
        {
            return Damage(amount, from, informMount, false);
        }

        public override int Damage(int amount, Mobile from, bool informMount, bool checkDisrupt)
        {
            int oldHits = Hits;

            if (Controlled && from is BaseCreature && !((BaseCreature)from).Controlled && !((BaseCreature)from).Summoned)
                amount = (int)(amount * ((BaseCreature)from).BonusPetDamageScalar);

            amount = base.Damage(amount, from, informMount, checkDisrupt);

            if (SubdueBeforeTame && !Controlled)
            {
                if ((oldHits > ((double)HitsMax / 10)) && (Hits <= ((double)HitsMax / 10)))
                {
                    PublicOverheadMessage(MessageType.Regular, 0x3B2, false, "* The creature has been beaten into subjugation! *");
                }
            }

            return amount;
        }

        public virtual bool DeleteCorpseOnDeath => false;

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
            if (Paragon.CheckConvert(this, location, m))
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
                if (_NavPoints == null)
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

        public virtual Mobile ConstantFocus => null;

        public virtual bool DisallowAllMoves => false;

        public virtual bool InitialInnocent => false;
        public virtual bool AlwaysInnocent => false;

        public virtual bool AlwaysMurderer => false;

        public virtual bool AlwaysAttackable => false;

        public virtual bool ForceNotoriety => false;

        public virtual bool HoldSmartSpawning => IsParagon;
        public virtual bool UseSmartAI => false;

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int DamageMin { get { return m_DamageMin; } set { m_DamageMin = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int DamageMax { get { return m_DamageMax; } set { m_DamageMax = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public override int HitsMax
        {
            get
            {
                int value = Str;

                if (m_HitsMax > 0)
                {
                    value = m_HitsMax + GetStatOffset(StatType.Str);

                    if (value < 1)
                    {
                        value = 1;
                    }
                    else if (value > 1000000)
                    {
                        value = 1000000;
                    }
                }

                // Skill Masteries
                value += ToughnessSpell.GetHPBonus(this);
                value += InvigorateSpell.GetHPBonus(this);

                return value;
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

        public virtual bool CanOpenDoors => !Body.IsAnimal && !Body.IsSea;

        public virtual bool CanMoveOverObstacles => Body.IsMonster;

        public virtual bool CanDestroyObstacles =>
                // to enable breaking of furniture, 'return CanMoveOverObstacles;'
                false;

        public void Unpacify()
        {
            BardEndTime = DateTime.UtcNow;
            BardPacified = false;
        }

        private HonorContext m_ReceivedHonorContext;

        public HonorContext ReceivedHonorContext { get { return m_ReceivedHonorContext; } set { m_ReceivedHonorContext = value; } }

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
            if (from != null && from.Player)
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

            InhumanSpeech speechType = SpeechType;

            if (speechType != null && !willKill)
            {
                speechType.OnDamage(this, amount);
            }

            if (m_ReceivedHonorContext != null)
            {
                m_ReceivedHonorContext.OnTargetDamaged(from, amount);
            }

            if (from is PlayerMobile)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(10), ((PlayerMobile)@from).RecoverAmmo);
            }

            base.OnDamage(amount, from, willKill);
        }

        public virtual void OnDamagedBySpell(Mobile from)
        {
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
            int fur = Fur;

            bool special = with is HarvestersBlade;

            if ((feathers == 0 && wool == 0 && meat == 0 && hides == 0 && scales == 0 && fur == 0) || Summoned || IsBonded || corpse.Animated)
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
                if (from.Race == Race.Human)
                {
                    hides = (int)Math.Ceiling(hides * 1.1); // 10% bonus only applies to hides, ore & logs
                }

                if (corpse.Map == Map.Felucca && !Siege.SiegeShard)
                {
                    feathers *= 2;
                    wool *= 2;
                    hides *= 2;
                    fur *= 2;
                    meat *= 2;
                    scales *= 2;
                }

                if (special)
                {
                    feathers = (int)Math.Ceiling(feathers * 1.1);
                    wool = (int)Math.Ceiling(wool * 1.1);
                    hides = (int)Math.Ceiling(hides * 1.1);
                    meat = (int)Math.Ceiling(meat * 1.1);
                    scales = (int)Math.Ceiling(scales * 1.1);
                }

                new Blood(0x122D).MoveToWorld(corpse.Location, corpse.Map);

                if (feathers != 0)
                {
                    Item feather = new Feather(feathers);

                    if (!special || !from.AddToBackpack(feather))
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

                    if (!special || !from.AddToBackpack(w))
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
                        case MeatType.DinoRibs: m = new RawDinoRibs(meat); break;
                        case MeatType.SeaSerpentSteak: m = new RawSeaSerpentSteak(meat); break;
                    }

                    if (!special || !from.AddToBackpack(m))
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
                    bool cutHides = (with is SkinningKnife && from.FindItemOnLayer(Layer.OneHanded) == with) || special || with is ButchersWarCleaver;

                    switch (HideType)
                    {
                        default:
                        case HideType.Regular:
                            if (cutHides) leather = new Leather(hides);
                            else leather = new Hides(hides);
                            break;
                        case HideType.Spined:
                            if (cutHides) leather = new SpinedLeather(hides);
                            else leather = new SpinedHides(hides);
                            break;
                        case HideType.Horned:
                            if (cutHides) leather = new HornedLeather(hides);
                            else leather = new HornedHides(hides);
                            break;
                        case HideType.Barbed:
                            if (cutHides) leather = new BarbedLeather(hides);
                            else leather = new BarbedHides(hides);
                            break;
                    }

                    if (!cutHides || !from.AddToBackpack(leather))
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

                    if (special)
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

                        if (!allPack)
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

                    if (!special || !from.AddToBackpack(dblood))
                    {
                        corpse.AddCarvedItem(dblood, from);
                        from.SendLocalizedMessage(1094946); // Some blood is left on the corpse.
                    }
                    else
                    {
                        from.SendLocalizedMessage(1114100); // You take some blood off the corpse and put it in your backpack.
                    }
                }

                if (fur != 0)
                {
                    Item _fur = new Fur(FurType, fur);

                    corpse.AddCarvedItem(_fur, from);
                    from.SendLocalizedMessage(1112765); // You shear it, and the fur is now on the corpse.
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

        public BaseCreature(AIType ai, FightMode mode, int iRangePerception, int iRangeFight)
            : this(ai, mode, iRangePerception, iRangeFight, .2, .4)
        {
        }

        public BaseCreature(
            AIType ai, FightMode mode, int iRangePerception, int iRangeFight, double dActiveSpeed, double dPassiveSpeed)
        {
            PhysicalDamage = 100;

            CanMove = true;

            ApproachWait = false;
            ApproachRange = 10;

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

            InitializeAbilities();
        }

        public BaseCreature(Serial serial)
            : base(serial)
        {
            m_arSpellAttack = new List<Type>();
            m_arSpellDefense = new List<Type>();

            m_bDebugAI = false;
        }

        protected override void OnCreate()
        {
            GenerateLoot(LootStage.Spawning);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(32); // version

            writer.Write(StealPackGenerated);
            writer.Write(HasBeenStolen);

            writer.Write(m_ForceActiveSpeed);
            writer.Write(m_ForcePassiveSpeed);

            writer.Write(CanMove);
            writer.Write(_LockDirection);
            writer.Write(ApproachWait);
            writer.Write(ApproachRange);

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
                writer.Write(DateTime.MinValue);
            }
            else
            {
                writer.Write(m_DeleteTime);
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

            switch (version)
            {
                case 32:
                case 31:
                    StealPackGenerated = reader.ReadBool();
                    HasBeenStolen = reader.ReadBool();
                    goto case 28;
                case 30:
                    goto case 28;
                case 29:
                    reader.ReadBool();
                    goto case 28;
                case 28:
                    m_ForceActiveSpeed = reader.ReadDouble();
                    m_ForcePassiveSpeed = reader.ReadDouble();
                    goto case 27;
                case 27: // Pet Slot Fix
                case 26:
                    {
                        CanMove = reader.ReadBool();
                        _LockDirection = reader.ReadBool();

                        ApproachWait = reader.ReadBool();
                        ApproachRange = reader.ReadInt();
                    }
                    break;
            }

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
                    TimerRegistry.Register<BaseCreature>("UnsummonTimer", this, m_SummonEnd - DateTime.UtcNow, c => c.Delete()); 
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

            m_dActiveSpeed = activeSpeed;
            m_dPassiveSpeed = passiveSpeed;

            if (version >= 14)
            {
                m_RemoveIfUntamed = reader.ReadBool();
                m_RemoveStep = reader.ReadInt();
            }

            if (version >= 17)
            {
                if (version < 32)
                {
                    var span = reader.ReadTimeSpan();

                    if (span > TimeSpan.Zero)
                    {
                        DeleteTime = DateTime.UtcNow + span;
                    }
                }
                else
                {
                    DeleteTime = reader.ReadDateTime();
                }
            }

            if (version >= 18)
            {
                m_CorpseNameOverride = reader.ReadString();
            }

            if (version >= 19)
            {
                m_Allured = reader.ReadBool();
            }

            if (version == 20)
            {
                reader.ReadInt();
            }

            if (version < 26)
            {
                CanMove = true;

                ApproachWait = false;
                ApproachRange = 10;
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

                if (Controlled && version == 26)
                {
                    AdjustTameRequirements();
                }
                else if (Controlled && CurrentTameSkill > MaxTameRequirement)
                {
                    CurrentTameSkill = MaxTameRequirement;
                }
            }
            else
            {
                AdjustTameRequirements();
            }

            if (version <= 14 && m_Paragon && Hue == 0x31)
            {
                Hue = Paragon.Hue; //Paragon hue fixed, should now be 0x501.
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

            if (AI == AIType.AI_UNUSED1 || AI == AIType.AI_UNUSED2)
                AI = AIType.AI_Melee; // Can be safely removed on 1/1/2021 - Dan
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

        public override bool ShouldCheckStatTimers => false;

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

        private static readonly Type[] m_Gold = new[]
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

        private static readonly Type[] m_BlackrockStew =
        {
            typeof(BowlOfBlackrockStew)
        };

        public virtual bool CheckFoodPreference(Item f)
        {
            if (FavoriteFood == FoodType.None)
            {
                return false;
            }

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

            if (CheckFoodPreference(f, FoodType.BlackrockStew, m_BlackrockStew))
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

            return types.Any(t => t == fed.GetType() || fed.GetType().IsSubclassOf(t));
        }

        public virtual bool CheckFeed(Mobile from, Item dropped)
        {
            if (!IsDeadPet && Controlled && (ControlMaster == from || IsPetFriend(from)))
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

                        if (m_Loyalty < MaxLoyalty)
                        {
                            m_Loyalty = MaxLoyalty;
                            happier = true;
                        }

                        if (happier)
                        {
                            SayTo(from, 502060); // Your pet looks happier.
                        }

                        Animate(AnimationType.Eat, 0);

                        if (IsBondable && !IsBonded)
                        {
                            Mobile master = m_ControlMaster;

                            if (master != null && master == from) //So friends can't start the bonding process
                            {
                                if (m_CurrentTameSkill <= 29.1 || master.Skills[SkillName.AnimalTaming].Base >= m_CurrentTameSkill ||
                                    OverrideBondingReqs() || (master.Skills[SkillName.AnimalTaming].Value >= m_CurrentTameSkill))
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
                                else
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

        public virtual bool CanAngerOnTame => false;

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
            bool canDrop = false;

            if (CheckFeed(from, dropped))
            {
                canDrop = true;
            }
            
            if (!canDrop && CheckGold(from, dropped))
            {
                canDrop = true;
            }
            
            if (!canDrop && !from.InRange(Location, 2) && base.OnDragDrop(from, dropped))
            {
                return true;
            }

            if (!canDrop)
            {
                PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1043257, from.NetState); // The animal shies away.
            }

            return canDrop;
        }

        protected virtual BaseAI ForcedAI => null;

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
        public double ForceActiveSpeed { get { return m_ForceActiveSpeed; } set { m_ForceActiveSpeed = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public double ForcePassiveSpeed { get { return m_ForcePassiveSpeed; } set { m_ForcePassiveSpeed = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public double ActiveSpeed { get { return m_ForceActiveSpeed != 0.0 ? m_ForceActiveSpeed : m_dActiveSpeed; } set { m_dActiveSpeed = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public double PassiveSpeed { get { return m_ForcePassiveSpeed != 0.0 ? m_ForcePassiveSpeed : m_dPassiveSpeed; } set { m_dPassiveSpeed = value; } }

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
        public Mobile CharmMaster => m_CharmMaster;

        [CommandProperty(AccessLevel.GameMaster)]
        public Point2D CharmTarget => m_CharmTarget;

        private class CharmTimer : Timer
        {
            private readonly BaseCreature m_Owner;
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

                    if (KhaldunTastyTreat.UnderInfluence(this))
                    {
                        Caddellite.UpdateBuff(m_ControlMaster);
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

                    if (KhaldunTastyTreat.UnderInfluence(this))
                    {
                        Caddellite.UpdateBuff(m_ControlMaster);
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

                if (m_Allured && m_ControlOrder != OrderType.None)
                {
                    Say(1079120); // Very well.
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

                if (skill != value)
                {
                    m_dMinTameSkill = value;
                    double adjusted = CurrentTameSkill - skill;

                    if (adjusted > 0)
                    {
                        m_CurrentTameSkill = value + adjusted;
                    }
                    else
                    {
                        m_CurrentTameSkill = value;
                    }
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
                if (ControlSlotsMin == 0 && ControlSlotsMax == 0)
                {
                    m_iControlSlots = value;

                    CalculateSlots(value);

                    if (m_iControlSlots != ControlSlotsMin)
                    {
                        m_iControlSlots = ControlSlotsMin;
                    }
                    else if (m_iControlSlots > ControlSlotsMax)
                    {
                        m_iControlSlots = ControlSlotsMax;
                    }
                }
                else
                {
                    m_iControlSlots = value;
                }
            }
        }

        [CommandProperty(AccessLevel.Administrator)]
        public int ControlSlotsMax { get; set; }

        [CommandProperty(AccessLevel.Administrator)]
        public int ControlSlotsMin { get; set; }

        public virtual bool NoHouseRestrictions => false;
        public virtual bool IsHouseSummonable => false;

        #region Corpse Resources
        public virtual int Feathers => 0;
        public virtual int Wool => 0;

        public virtual int Fur => 0;
        public virtual FurType FurType => FurType.Green;

        public virtual MeatType MeatType => MeatType.Ribs;
        public virtual int Meat => 0;

        public virtual int Hides => 0;
        public virtual HideType HideType => HideType.Regular;

        public virtual int Scales => 0;
        public virtual ScaleType ScaleType => ScaleType.Red;

        public virtual int DragonBlood => 0;
        #endregion

        public virtual bool AutoDispel => false;
        public virtual double AutoDispelChance => 0.1;

        public virtual bool IsScaryToPets => false;
        public virtual bool IsScaredOfScaryThings => true;

        public virtual bool CanRummageCorpses => false;

        public virtual void OnGotMeleeAttack(Mobile attacker)
        {
            if (AutoDispel && attacker is BaseCreature && ((BaseCreature)attacker).IsDispellable &&
                AutoDispelChance > Utility.RandomDouble())
            {
                Dispel(attacker);
            }
        }

        public virtual void Dispel(Mobile m)
        {
            Effects.SendLocationParticles(EffectItem.Create(m.Location, m.Map, EffectItem.DefaultDuration), 0x3728, 8, 20, 5042);
            Effects.PlaySound(m, m.Map, 0x201);

            m.Delete();
        }

        public virtual bool DeleteOnRelease => m_bSummoned || m_Allured;

        public virtual void OnGaveMeleeAttack(Mobile defender)
        {
            Poison p = GetHitPoison();

            if (m_Paragon)
            {
                p = PoisonImpl.IncreaseLevel(p);
            }

            if (p != null)
            {
                if (TryHitPoison())
                {
                    defender.FixedEffect(0x3779, 1, 10, 1271, 0);
                    defender.ApplyPoison(this, p);
                }

                if (Controlled)
                {
                    if (AbilityProfile != null && AbilityProfile.HasAbility(MagicalAbility.Poisoning))
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

            if (ColossalRage.HasRage(this) && 0.33 >= Utility.RandomDouble())
            {
                DoRageHit(defender);
            }
        }

        public virtual Poison GetHitPoison()
        {
            if (!Controlled)
                return HitPoison;

            int current = 0;

            if (HitPoison != null)
                current = HitPoison.Level;

            AbilityProfile profile = AbilityProfile;

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
            if (!Controlled)
                return HitPoisonChance >= Utility.RandomDouble();

            AbilityProfile profile = AbilityProfile;

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

            StopDeleteTimer();

            FocusMob = null;

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
                PublicOverheadMessage(MessageType.Regular, 41, false, string.Format(format, args));
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
                AnimalTaming.DeferredTarget = false;

                if (Owner.From.UseSkill(SkillName.AnimalTaming) && Owner.From.Target != null)
                {
                    Owner.From.Target.Invoke(Owner.From, m_Mobile);
                }

                AnimalTaming.DeferredTarget = true;
                AnimalTaming.DisableMessage = false;
                Owner.From.TargetLocked = false;
            }
        }

        private class RenameEntry : ContextMenuEntry
        {
            private readonly Mobile m_From;
            private readonly BaseCreature m_Creature;

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
            public override int MessageCliloc => 1115558; // Enter a new name for your pet.

            private readonly BaseCreature m_Creature;

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
        public virtual bool CanTeach => false;

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
                            Say(1019077, AffixType.Append, string.Format(" {0}", pointsToLearn), "");
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
            if (ControlMaster != null && ControlMaster != aggressor)
            {
                var master = ControlMaster;
                AggressorInfo info = master.Aggressors.FirstOrDefault(i => i.Attacker == aggressor);

                if (info != null)
                {
                    // already in the list, so we're refreshing it
                    info.Refresh();
                    info.CriminalAggression = criminal;
                }
                else
                {
                    // not in the list, so we're adding it
                    master.Aggressors.Add(AggressorInfo.Create(aggressor, master, criminal));

                    if (CanSee(aggressor) && NetState != null)
                    {
                        master.NetState.Send(MobileIncoming.Create(NetState, master, aggressor));
                    }

                    master.UpdateAggrExpire();
                }

                // Now, if the master is in the aggressor list, it needs to be refreshed
                info = aggressor.Aggressors.FirstOrDefault(i => i.Attacker == master);

                if (info != null)
                {
                    info.Refresh();
                }

                info = master.Aggressed.FirstOrDefault(i => i.Defender == aggressor);

                if (info != null)
                {
                    info.Refresh();
                }

                // next lets find out if our master is on the aggressors aggressed list
                info = aggressor.Aggressed.FirstOrDefault(i => i.Defender == master);

                if (info != null)
                {
                    // already in the list, so we're refreshing it
                    info.Refresh();
                    info.CriminalAggression = criminal;
                }
                else
                {
                    // not in the list, so we're adding it
                    aggressor.Aggressed.Add(AggressorInfo.Create(aggressor, master, criminal));

                    if (CanSee(aggressor) && NetState != null)
                    {
                        master.NetState.Send(MobileIncoming.Create(NetState, master, aggressor));
                    }

                    master.UpdateAggrExpire();
                }

                if (aggressor is PlayerMobile || (aggressor is BaseCreature && !((BaseCreature)aggressor).IsMonster))
                {
                    BuffInfo.AddBuff(master, new BuffInfo(BuffIcon.HeatOfBattleStatus, 1153801, 1153827, Aggression.CombatHeatDelay, master, true));
                    BuffInfo.AddBuff(aggressor, new BuffInfo(BuffIcon.HeatOfBattleStatus, 1153801, 1153827, Aggression.CombatHeatDelay, aggressor, true));
                }
            }
            else if (aggressor is BaseCreature)
            {
                var pm = ((BaseCreature)aggressor).GetMaster() as PlayerMobile;

                if (pm != null)
                {
                    AggressiveAction(pm, criminal);
                }
            }

            base.AggressiveAction(aggressor, criminal);

            OrderType ct = m_ControlOrder;

            if (m_AI != null)
            {
                if (ct != OrderType.Follow && ct != OrderType.Stop && ct != OrderType.Stay)
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

            //StopFlee();
            ForceReacquire();

            if (aggressor.ChangingCombatant && (m_bControlled || m_bSummoned) &&
                (ct == OrderType.Come || ct == OrderType.Stay || ct == OrderType.Stop || ct == OrderType.None ||
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

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (CanBeRenamedBy(from) && m_bControlled && m_ControlMaster == from && !m_bSummoned)
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

            if (ViceVsVirtueSystem.Enabled && Map == ViceVsVirtueSystem.Facet)
            {
                ViceVsVirtueSystem.CheckHarmful(this, target);
            }
        }

        public override void DoBeneficial(Mobile target)
        {
            base.DoBeneficial(target);

            if (ViceVsVirtueSystem.Enabled && Map == ViceVsVirtueSystem.Facet && target != null)
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

            Animate(AnimationType.Fidget, 0);

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
                Animate(AnimationType.Alert, 0);

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

        public virtual bool CanStealth => false;
        public virtual bool SupportsRunAnimation => true;

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
                        Stealth.OnUse(this);
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

            SpecialAbility.CheckApproachTrigger(this, m, oldLocation);

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
                        Animate(AnimationType.Pillage, 0);
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
            m_HitsMax = val;
            Hits = HitsMax;
        }

        public void SetHits(int min, int max)
        {
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
                SkillsCap += (Skills[name].BaseFixedPoint - Skills[name].CapFixedPoint);

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
                SkillsCap += (Skills[name].BaseFixedPoint - Skills[name].CapFixedPoint);

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

        public override void OnRawDexChange(int oldDex)
        {
            if (oldDex != RawDex)
            {
                AdjustSpeeds();
            }
        }

        public void AdjustSpeeds()
        {
            double activeSpeed = 0.0;
            double passiveSpeed = 0.0;

            SpeedInfo.GetSpeeds(this, ref activeSpeed, ref passiveSpeed);

            m_dActiveSpeed = activeSpeed;
            m_dPassiveSpeed = passiveSpeed;
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

                    List<Item> list = new List<Item>(Backpack.Items);
                    foreach (Item item in list)
                    {
                        if (item.Movable)
                            b.DropItem(item);
                        else
                            item.Delete();
                    }

                    BaseHouse house = BaseHouse.FindHouseAt(this);
                    if (house != null)
                    {
                        if (Backpack.Items.Count == 0)
                            b.Delete();
                        else
                            b.MoveToWorld(house.BanLocation, house.Map);
                    }
                    else
                    {
                        if (Backpack.Items.Count == 0)
                            b.Delete();
                        else
                            b.MoveToWorld(Location, Map);
                    }
                }
            }
        }

        public LootStage LootStage { get; protected set; }
        public int KillersLuck { get; protected set; }
        public bool StealPackGenerated { get; protected set; }
        public bool HasBeenStolen { get; set; }

        public virtual void GenerateLoot(bool spawning)
        {
            GenerateLoot(spawning ? LootStage.Spawning : LootStage.Death);
        }

        public virtual void GenerateLoot(LootStage stage)
        {
            if (m_NoLootOnDeath || m_Allured)
                return;

            LootStage = stage;

            switch (stage)
            {
                case LootStage.Stolen:
                    StealPackGenerated = true;
                    break;
                case LootStage.Death:
                    KillersLuck = LootPack.GetLuckChanceForKiller(this);
                    break;
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

            KillersLuck = 0;
        }

        public virtual void GenerateLoot()
        { }

        public virtual void AddLoot(LootPack pack, int min, int max)
        {
            AddLoot(pack, Utility.RandomMinMax(min, max), 100.0);
        }

        public virtual void AddLoot(LootPack pack, int min, int max, double chance)
        {
            if (min > max)
                min = max;

            AddLoot(pack, Utility.RandomMinMax(min, max), chance);
        }

        public virtual void AddLoot(LootPack pack, int amount)
        {
            AddLoot(pack, amount, 100.0);
        }

        public virtual void AddLoot(LootPack pack, int amount, double chance)
        {
            for (int i = 0; i < amount; ++i)
            {
                AddLoot(pack, chance);
            }
        }

        public virtual void AddLoot(LootPack pack)
        {
            AddLoot(pack, 100.0);
        }

        public virtual void AddLoot(LootPack pack, double chance)
        {
            if (Summoned || pack == null || (chance < 100.0 && Utility.RandomDouble() > chance / 100))
            {
                return;
            }

            Container backpack = Backpack;

            if (backpack == null)
            {
                backpack = new Backpack
                {
                    Movable = false
                };

                AddItem(backpack);
            }

            pack.Generate(this);
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

        public void PackItem(Item item)
        {
            if ((Summoned && item.Movable) || item == null)
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
                pack = new Backpack
                {
                    Movable = false
                };

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

            if (Controlled && !string.IsNullOrEmpty(EngravedText))
            {
                list.Add(1157315, EngravedText); // <BASEFONT COLOR=#668cff>Branded: ~1_VAL~<BASEFONT COLOR=#FFFFFF>
            }

            if (DisplayWeight)
            {
                list.Add(TotalWeight == 1 ? 1072788 : 1072789, TotalWeight.ToString()); // Weight: ~1_WEIGHT~ stones
            }

            if (m_ControlOrder == OrderType.Guard)
            {
                list.Add(1080078); // guarding
            }

            if (IsGolem)
                list.Add(1113697); // (Golem)

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

            if (IsSoulBound)
            {
                list.Add(1159188); // <BASEFONT COLOR=#FF8300>Soulbound<BASEFONT COLOR=#FFFFFF>
            }

            if (IsAmbusher)
                list.Add(1155480); // Ambusher
        }

        public virtual double TreasureMapChance => TreasureMap.LootChance;
        public virtual int TreasureMapLevel => -1;

        public virtual bool IgnoreYoungProtection => false;

        public override bool OnBeforeDeath()
        {
            int treasureLevel = TreasureMapInfo.ConvertLevel(TreasureMapLevel);
            GetLootingRights();

            if (!Summoned && !NoKillAwards && !IsBonded && !NoLootOnDeath)
            {
                if (treasureLevel >= 0)
                {
                    if (m_Paragon && Paragon.ChestChance > Utility.RandomDouble())
                    {
                        PackItem(new ParagonChest(Name, treasureLevel));
                    }
                    else if (TreasureMapChance >= Utility.RandomDouble())
                    {
                        Map map = Map;

                        if (map == Map.Trammel && Siege.SiegeShard)
                            map = Map.Felucca;

                        PackItem(new TreasureMap(treasureLevel, map, SpellHelper.IsEodon(map, Location)));
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
                GenerateLoot(LootStage.Death);
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

                Mobile master = GetMaster();

                return master == null || (master is BaseCreature && !((BaseCreature)master).Controlled);
            }
        }

        public virtual bool IsAggressiveMonster => IsMonster 
        										&&
        										 ( m_FightMode == FightMode.Closest 
        										|| m_FightMode == FightMode.Strongest 
        										|| m_FightMode == FightMode.Weakest 
        										|| m_FightMode == FightMode.Good
        										 );

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

        public Mobile RandomPlayerWithLootingRights()
        {
            var rights = GetLootingRights();

            if (rights == null)
            {
                return null;
            }

            for (int i = rights.Count - 1; i >= 0; --i)
            {
                var ds = rights[i];

                if (!ds.m_HasRight)
                {
                    rights.RemoveAt(i);
                }
            }

            if (rights.Count > 0)
            {
                return rights[Utility.Random(rights.Count)].m_Mobile;
            }

            return null;
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

                List<DamageEntry> respList = de.Responsible;

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

                minDamage = (int)(topDamage * 0.06);

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
        public bool Allured
        {
            get { return m_Allured; }
            set
            {
                m_Allured = value;

                if (value && Backpack != null)
                {
                    ColUtility.SafeDelete(Backpack.Items);
                }
            }
        }
        #endregion

        public virtual bool GivesMLMinorArtifact => false;

        public override void OnItemLifted(Mobile from, Item item)
        {
            base.OnItemLifted(from, item);

            InvalidateProperties();
        }

        public virtual void OnKilledBy(Mobile mob)
        {
            if (m_Paragon && Paragon.CheckArtifactChance(mob, this))
            {
                Paragon.GiveArtifactTo(mob);
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

                List<AggressorInfo> aggressors = Aggressors;

                for (int i = 0; i < aggressors.Count; ++i)
                {
                    AggressorInfo info = aggressors[i];

                    if (info.Attacker.Combatant == this)
                    {
                        info.Attacker.Combatant = null;
                    }
                }

                List<AggressorInfo> aggressed = Aggressed;

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
                LootingRights = null;

                if (!Summoned && !m_NoKillAwards)
                {
                    int totalFame = Fame / 100;
                    int totalKarma = -Karma / 100;

                    if (Map == Map.Felucca)
                    {
                        totalFame += ((totalFame / 10) * 3);
                        totalKarma += ((totalKarma / 10) * 3);
                    }

                    List<DamageStore> list = GetLootingRights();
                    List<Mobile> titles = new List<Mobile>();
                    List<int> fame = new List<int>();
                    List<int> karma = new List<int>();

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
                                if (ds.m_Mobile is PlayerMobile)
                                {
                                    foreach (Mobile pet in ((PlayerMobile)ds.m_Mobile).AllFollowers.Where(p => DamageEntries.Any(de => de.Damager == p)))
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

                        OnKilledBy(ds.m_Mobile);

                        if (HumilityVirtue.IsInHunt(ds.m_Mobile) && Karma < 0)
                            HumilityVirtue.RegisterKill(ds.m_Mobile, this, list.Count);
                    }

                    for (int i = 0; i < titles.Count; ++i)
                    {
                        Titles.AwardFame(titles[i], fame[i], true);
                        Titles.AwardKarma(titles[i], karma[i], true);
                    }
                }

                CreatureDeathEventArgs e = new CreatureDeathEventArgs(this, LastKiller, c);

                EventSink.InvokeCreatureDeath(e);

                if (!c.Deleted)
                {
                    int i;

                    if (e.ClearCorpse)
                    {
                        i = c.Items.Count;

                        while (--i >= 0)
                        {
                            if (i >= c.Items.Count)
                            {
                                continue;
                            }

                            Item o = c.Items[i];

                            if (o != null && !o.Deleted)
                            {
                                o.Delete();
                            }
                        }
                    }

                    i = e.ForcedLoot.Count;

                    while (--i >= 0)
                    {
                        if (i >= e.ForcedLoot.Count)
                        {
                            continue;
                        }

                        Item o = e.ForcedLoot[i];

                        if (o != null && !o.Deleted)
                        {
                            c.DropItem(o);
                        }
                    }

                    e.ClearLoot(false);
                }
                else
                {
                    int i = e.ForcedLoot.Count;

                    while (--i >= 0)
                    {
                        if (i >= e.ForcedLoot.Count)
                        {
                            continue;
                        }

                        Item o = e.ForcedLoot[i];

                        if (o != null && !o.Deleted)
                        {
                            o.Delete();
                        }
                    }

                    e.ClearLoot(true);
                }

                base.OnDeath(c);

                if (e.PreventDefault)
                {
                    return;
                }

                if (DeleteCorpseOnDeath && !e.PreventDelete)
                {
                    c.Delete();
                }
            }
        }

        public bool GivenSpecialArtifact { get; set; }

        /* To save on cpu usage, RunUO creatures only reacquire creatures under the following circumstances:
        *  - 10 seconds have elapsed since the last time it tried
        *  - The creature was attacked
        *  - Some creatures, like dragons, will reacquire when they see someone move
        *
        * This functionality appears to be implemented on OSI as well
        */

        private long m_NextReacquireTime;

        public long NextReacquireTime { get { return m_NextReacquireTime; } set { m_NextReacquireTime = value; } }

        public virtual TimeSpan ReacquireDelay => TimeSpan.FromSeconds(10.0);
        public virtual bool ReacquireOnMovement => false;

        public virtual bool AcquireOnApproach => m_Paragon || ApproachWait;
        public virtual int AcquireOnApproachRange => ApproachRange;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ApproachWait { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ApproachRange { get; set; }

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

            if (_NavPoints != null)
            {
                _NavPoints.Clear();
                _NavPoints = null;
            }
        }

        public override bool CanBeHarmful(IDamageable damageable, bool message, bool ignoreOurBlessedness)
        {
            Mobile target = damageable as Mobile;

            if (RecentSetControl && GetMaster() == target)
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

            if (Controlled && from == ControlMaster && !from.Region.IsPartOf<Jail>() && !Allured)
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

                UpdateMasteryInfo();

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

                UpdateMasteryInfo();

                AdjustSpeeds();
                CurrentSpeed = m_dActiveSpeed;

                StopDeleteTimer();

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
            if (StatLossAfterTame && Owners.Count == 0)
            {
                AnimalTaming.ScaleStats(this, 0.5);
            }
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

        public virtual double GetDispelDifficulty()
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

            creature.m_SummonEnd = DateTime.UtcNow + duration;
            TimerRegistry.Register<BaseCreature>("UnsummonTimer", creature, duration, c => c.Delete());

            creature.MoveToWorld(p, caster.Map);

            Effects.PlaySound(p, creature.Map, sound);

            m_Summoning = false;

            // Skill Masteries
            creature.HitsMaxSeed += MasteryInfo.EnchantedSummoningBonus(creature);
            creature.Hits = creature.HitsMaxSeed;

            return true;
        }

        private static readonly bool EnableRummaging = true;

        private const double ChanceToRummage = 0.5; // 50%

        private const double MinutesToNextRummageMin = 1.0;
        private const double MinutesToNextRummageMax = 4.0;

        private const double MinutesToNextChanceMin = 0.25;
        private const double MinutesToNextChanceMax = 0.75;

        private long m_NextRummageTime;

        public virtual bool IsDispellable => Summoned && !IsAnimatedDead;

        #region Healing
        public virtual double HealChance => 0.0;

        private long m_NextHealTime = Core.TickCount;
        private long m_NextHealOwnerTime = Core.TickCount;
        private Timer m_HealTimer;

        public bool IsHealing => (m_HealTimer != null);

        public virtual bool CheckHeal()
        {
            long tc = Core.TickCount;

            if (Alive && !IsHealing && !BardPacified)
            {
                Mobile owner = ControlMaster;

                if (owner != null && tc >= m_NextHealOwnerTime && CanBeBeneficial(owner, true, true) &&
                    owner.Map == Map && InRange(owner, 2) && InLOS(owner) && (owner.Poisoned || owner.Hits < .78 * owner.HitsMax))
                {
                    HealStart(owner);
                    m_NextHealOwnerTime = tc + (int)TimeSpan.FromSeconds(30).TotalMilliseconds;

                    return true;
                }
                else if (tc >= m_NextHealTime && CanBeBeneficial(this) && (Hits < .78 * HitsMax || Poisoned))
                {
                    HealStart(this);
                    m_NextHealTime = tc + (int)TimeSpan.FromSeconds(1.0).TotalMilliseconds;

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

            double seconds = 6.5 + (patient.Alive ? 0.0 : 5.0);

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
                !InRange(patient, RangePerception))
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

                    patient.Heal((int)toHeal, this);

                    CheckSkill(SkillName.Healing, 0.0, Skills[SkillName.Healing].Cap);
                    CheckSkill(SkillName.Anatomy, 0.0, Skills[SkillName.Anatomy].Cap);
                }
                else if (Controlled)
                {
                    CheckSkill(SkillName.Healing, 0.0, 10);
                    CheckSkill(SkillName.Anatomy, 0.0, 10);
                }
            }

            HealEffect(patient);

            StopHeal();

            if ((onSelf && Hits >= .78 * HitsMax && Hits < HitsMax) ||
                (!onSelf && patient.Hits >= .78 * patient.HitsMax && patient.Hits < patient.HitsMax))
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
            patient.PlaySound(0x57);
        }
        #endregion

        public override void OnHeal(ref int amount, Mobile from)
        {
            base.OnHeal(ref amount, from);

            // Removed until we can figure out the endless loop, or do a complete refactor
            /*
            if (from == null)
                return;

            if (amount > 0 && from != null && from != this)
            {
                for (int i = Aggressed.Count - 1; i >= 0; i--)
                {
                    AggressorInfo info = Aggressed[i];

                    if (info.Defender.InRange(Location, Core.GlobalMaxUpdateRange) && info.Defender.DamageEntries.Any(de => de.Damager == this))
                    {
                        info.Defender.RegisterDamage(amount, from);
                    }

                    if (info.Defender.Player && from.CanBeHarmful(info.Defender))
                    {
                        from.DoHarmful(info.Defender, true);
                    }
                }

                for (int i = Aggressors.Count - 1; i >= 0; i--)
                {
                    AggressorInfo info = Aggressors[i];

                    if (info.Attacker.InRange(Location, Core.GlobalMaxUpdateRange) && info.Attacker.DamageEntries.Any(de => de.Damager == this))
                    {
                        info.Attacker.RegisterDamage(amount, from);
                    }

                    if (info.Attacker.Player && from.CanBeHarmful(info.Attacker))
                    {
                        from.DoHarmful(info.Attacker, true);
                    }
                }
            }*/
        }

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
                int x = from.X + Utility.RandomMinMax(-range, range);
                int y = from.Y + Utility.RandomMinMax(-range, range);
                int z = map.GetAverageZ(x, y);

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
        public virtual void DoRageHit(Mobile defender)
        {
            if (defender != null && defender.Alive)
            {
                int damage = 0;

                SpecialAbility.ColossalBlow.DoEffects(this, defender, ref damage);
            }
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

        public virtual bool CanPeace => false;
        public virtual bool CanProvoke => false;

        public virtual bool PlayInstrumentSound => true;

        public virtual bool DoDiscord()
        {
            Mobile target = GetBardTarget(Controlled);

            if (target == null || !target.InLOS(this) || !InRange(target.Location, BaseInstrument.GetBardRange(this, SkillName.Discordance)) || CheckInstrument() == null)
            {
                return false;
            }

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
            {
                return false;
            }

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
                inst = Backpack == null ? null : Backpack.FindItemByType(typeof(BaseInstrument)) as BaseInstrument;

                if (inst == null)
                {
                    inst = new Harp
                    {
                        SuccessSound = PlayInstrumentSound ? 0x58B : 0,
                        FailureSound = PlayInstrumentSound ? 0x58C : 0,
                        Movable = false,
                        Quality = ItemQuality.Exceptional
                    };

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
            {
                m = GetMaster().Combatant as Mobile;
            }

            if (creaturesOnly && m is PlayerMobile)
            {
                return null;
            }

            if (m == null || m == this || !CanBeHarmful(m, false) || (creaturesOnly && !(m is BaseCreature)))
            {
                List<AggressorInfo> list = new List<AggressorInfo>();
                list.AddRange(Aggressors.Where(info => !creaturesOnly || info.Attacker is PlayerMobile));

                if (list.Count > 0)
                {
                    m = list[Utility.Random(list.Count)].Attacker;
                }
                else
                {
                    m = null;
                }

                ColUtility.Free(list);
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

        public virtual bool TeleportsTo => false;
        public virtual TimeSpan TeleportDuration => TimeSpan.FromSeconds(5);
        public virtual int TeleportRange => 16;
        public virtual double TeleportProb => 0.25;

        public virtual bool TeleportsPets => false;

        private static readonly int[] m_Offsets = new int[]
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

                    SpellHelper.Turn(this, toTeleport);
                    SpellHelper.Turn(toTeleport, this);

                    toTeleport.ProcessDelta();

                    Effects.SendLocationParticles(EffectItem.Create(from, Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);
                    Effects.SendLocationParticles(EffectItem.Create(to, Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 5023);

                    toTeleport.PlaySound(0x1FE);

                    Combatant = toTeleport;

                    OnAfterTeleport(toTeleport);
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

        public virtual void OnAfterTeleport(Mobile m)
        {
        }
        #endregion

        #region Detect Hidden
        private long _NextDetect;

        public virtual bool CanDetectHidden => Controlled && Skills.DetectHidden.Value > 0;

        public virtual int FindPlayerDelayBase => (15000 / Int);
        public virtual int FindPlayerDelayMax => 60;
        public virtual int FindPlayerDelayMin => 5;
        public virtual int FindPlayerDelayHigh => 10;
        public virtual int FindPlayerDelayLow => 9;

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

            if (Paralyzed || Frozen)
            {
                return;
            }

            if (!Summoned && _Profile != null)
            {
                SpecialAbility.CheckThinkTrigger(this);
                AreaEffect.CheckThinkTrigger(this);
            }

            if (Combatant != null)
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
                DoProvoke();
                m_NextProvoke = tc + Utility.RandomMinMax(5000, 12500);
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
            if (Map == null)
                return false;

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

            List<Item> items = toRummage.Items;

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

            string[] wordsString = str.Split(' ');
            string[] wordsName = name.Split(' ');

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
            List<Mobile> move = new List<Mobile>();

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
        public virtual bool PlayerRangeSensitive => CurrentWayPoint == null && (_NavPoints == null || _NavPoints.Count == 0);
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

        public virtual bool ReturnsToHome => (m_SeeksHome && (Home != Point3D.Zero) && !m_ReturnQueued && !Controlled && !Summoned);

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

            List<BaseCreature> toRelease = new List<BaseCreature>();

            // added array for wild creatures in house regions to be removed
            List<BaseCreature> toRemove = new List<BaseCreature>();

            Parallel.ForEach(
                World.Mobiles.Values,
                m =>
                {
                    if (m is BaseMount && ((BaseMount)m).Rider != null)
                    {
                        ((BaseCreature)m).OwnerAbandonTime = DateTime.MinValue;
                    }
                    else if (m is BaseCreature)
                    {
                        BaseCreature c = (BaseCreature)m;

                        if (c.IsDeadPet)
                        {
                            Mobile owner = c.ControlMaster;

                            if (!c.IsStabled && !(c is BaseVendor) &&
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

                if (c.AIObject != null)
                {
                    c.AIObject.DoOrderRelease();
                }

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

    #region Delete Previously Tamed Timer
    public class CreatureDeleteTimer : Timer
    {
        public static CreatureDeleteTimer Instance { get; set; }

        public List<BaseCreature> ToDelete { get; set; } = new List<BaseCreature>();

        public CreatureDeleteTimer()
            : base(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5))
        {
            Priority = TimerPriority.OneMinute;
        }

        protected override void OnTick()
        {
            var toDelete = ToDelete.Where(bc => bc.Deleted || bc.DeleteTime < DateTime.UtcNow).ToList();

            for (int i = 0; i < toDelete.Count; i++)
            {
                var bc = toDelete[i];

                if (!bc.Summoned && !bc.Deleted && !bc.IsStabled && bc.DeleteTime != DateTime.MinValue)
                {
                    bc.Delete();
                }

                RemoveFromTimer(bc);
            }

            ColUtility.Free(toDelete);
        }

        public static void RegisterTimer(BaseCreature bc)
        {
            if (Instance == null)
            {
                Instance = new CreatureDeleteTimer();
            }

            if (!Instance.Running)
            {
                Instance.Start();
            }

            if (!Instance.ToDelete.Contains(bc) && !bc.Summoned && !bc.Deleted && !bc.IsStabled)
            {
                Instance.ToDelete.Add(bc);
            }
        }

        public static void RemoveFromTimer(BaseCreature bc)
        {
            if (Instance == null)
            {
                return;
            }

            if (Instance.ToDelete.Contains(bc))
            {
                Instance.ToDelete.Remove(bc);

                if (Instance.ToDelete.Count == 0)
                {
                    Instance.Stop();
                }
            }
        }
    }
    #endregion

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
