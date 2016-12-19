using System;
using Server.Mobiles;
using Server.Network;

namespace Server
{
    public class BuffInfo
    {
        public static bool Enabled
        {
            get
            {
                return Core.ML;
            }
        }

        public static void Initialize()
        {
            if (Enabled)
            {
                EventSink.ClientVersionReceived += new ClientVersionReceivedHandler(delegate(ClientVersionReceivedArgs args)
                {
                    PlayerMobile pm = args.State.Mobile as PlayerMobile;
					
                    if (pm != null)
                        Timer.DelayCall(TimeSpan.Zero, pm.ResendBuffs);
                });
            }
        }

        public static int Blank { get { return 1114057; } } // ~1_val~

        #region Properties
        private readonly BuffIcon m_ID;
        public BuffIcon ID
        {
            get
            {
                return this.m_ID;
            }
        }

        private readonly int m_TitleCliloc;
        public int TitleCliloc
        {
            get
            {
                return this.m_TitleCliloc;
            }
        }

        private readonly int m_SecondaryCliloc;
        public int SecondaryCliloc
        {
            get
            {
                return this.m_SecondaryCliloc;
            }
        }

        private readonly TimeSpan m_TimeLength;
        public TimeSpan TimeLength
        {
            get
            {
                return this.m_TimeLength;
            }
        }

        private readonly DateTime m_TimeStart;
        public DateTime TimeStart
        {
            get
            {
                return this.m_TimeStart;
            }
        }

        private readonly Timer m_Timer;
        public Timer Timer
        {
            get
            {
                return this.m_Timer;
            }
        }

        private readonly bool m_RetainThroughDeath;
        public bool RetainThroughDeath
        {
            get
            {
                return this.m_RetainThroughDeath;
            }
        }

        private readonly TextDefinition m_Args;
        public TextDefinition Args
        {
            get
            {
                return this.m_Args;
            }
        }

        #endregion

        #region Constructors
        public BuffInfo(BuffIcon iconID, int titleCliloc)
            : this(iconID, titleCliloc, titleCliloc + 1)
        {
        }

        public BuffInfo(BuffIcon iconID, int titleCliloc, int secondaryCliloc)
        {
            this.m_ID = iconID;
            this.m_TitleCliloc = titleCliloc;
            this.m_SecondaryCliloc = secondaryCliloc;
        }

        public BuffInfo(BuffIcon iconID, int titleCliloc, TimeSpan length, Mobile m)
            : this(iconID, titleCliloc, titleCliloc + 1, length, m)
        {
        }

        //Only the timed one needs to Mobile to know when to automagically remove it.
        public BuffInfo(BuffIcon iconID, int titleCliloc, int secondaryCliloc, TimeSpan length, Mobile m)
            : this(iconID, titleCliloc, secondaryCliloc)
        {
            this.m_TimeLength = length;
            this.m_TimeStart = DateTime.UtcNow;

            this.m_Timer = Timer.DelayCall(length, new TimerCallback(
                delegate
                {
                    PlayerMobile pm = m as PlayerMobile;

                    if (pm == null)
                        return;

                    pm.RemoveBuff(this);
                }));
        }

        public BuffInfo(BuffIcon iconID, int titleCliloc, TextDefinition args)
            : this(iconID, titleCliloc, titleCliloc + 1, args)
        {
        }

        public BuffInfo(BuffIcon iconID, int titleCliloc, int secondaryCliloc, TextDefinition args)
            : this(iconID, titleCliloc, secondaryCliloc)
        {
            this.m_Args = args;
        }

        public BuffInfo(BuffIcon iconID, int titleCliloc, bool retainThroughDeath)
            : this(iconID, titleCliloc, titleCliloc + 1, retainThroughDeath)
        {
        }

        public BuffInfo(BuffIcon iconID, int titleCliloc, int secondaryCliloc, bool retainThroughDeath)
            : this(iconID, titleCliloc, secondaryCliloc)
        {
            this.m_RetainThroughDeath = retainThroughDeath;
        }

        public BuffInfo(BuffIcon iconID, int titleCliloc, TextDefinition args, bool retainThroughDeath)
            : this(iconID, titleCliloc, titleCliloc + 1, args, retainThroughDeath)
        {
        }

        public BuffInfo(BuffIcon iconID, int titleCliloc, int secondaryCliloc, TextDefinition args, bool retainThroughDeath)
            : this(iconID, titleCliloc, secondaryCliloc, args)
        {
            this.m_RetainThroughDeath = retainThroughDeath;
        }

        public BuffInfo(BuffIcon iconID, int titleCliloc, TimeSpan length, Mobile m, TextDefinition args)
            : this(iconID, titleCliloc, titleCliloc + 1, length, m, args)
        {
        }

        public BuffInfo(BuffIcon iconID, int titleCliloc, int secondaryCliloc, TimeSpan length, Mobile m, TextDefinition args)
            : this(iconID, titleCliloc, secondaryCliloc, length, m)
        {
            this.m_Args = args;
        }

        public BuffInfo(BuffIcon iconID, int titleCliloc, TimeSpan length, Mobile m, TextDefinition args, bool retainThroughDeath)
            : this(iconID, titleCliloc, titleCliloc + 1, length, m, args, retainThroughDeath)
        {
        }

        public BuffInfo(BuffIcon iconID, int titleCliloc, int secondaryCliloc, TimeSpan length, Mobile m, TextDefinition args, bool retainThroughDeath)
            : this(iconID, titleCliloc, secondaryCliloc, length, m)
        {
            this.m_Args = args;
            this.m_RetainThroughDeath = retainThroughDeath;
        }

        #endregion

        #region Convenience Methods
        public static void AddBuff(Mobile m, BuffInfo b)
        {
            PlayerMobile pm = m as PlayerMobile;

            if (pm != null)
                pm.AddBuff(b);
        }

        public static void RemoveBuff(Mobile m, BuffInfo b)
        {
            PlayerMobile pm = m as PlayerMobile;

            if (pm != null)
                pm.RemoveBuff(b);
        }

        public static void RemoveBuff(Mobile m, BuffIcon b)
        {
            PlayerMobile pm = m as PlayerMobile;

            if (pm != null)
                pm.RemoveBuff(b);
        }
        #endregion
    }

    public enum BuffIcon : short
    {
        DismountPrevention = 0x3E9,
        NoRearm = 0x3EA,
        //Currently, no 0x3EB or 0x3EC
        NightSight = 0x3ED,	//*
        DeathStrike,
        EvilOmen,
        UnknownStandingSwirl,	//Which is healing throttle & Stamina throttle?
        UnknownKneelingSword,
        DivineFury,			//*
        EnemyOfOne,			//*
        HidingAndOrStealth,	//*
        ActiveMeditation,	//*
        BloodOathCaster,	//*
        BloodOathCurse,		//*
        CorpseSkin,			//*
        Mindrot,			//*
        PainSpike,			//*
        Strangle,
        GiftOfRenewal,		//*
        AttuneWeapon,		//*
        Thunderstorm,		//*
        EssenceOfWind,		//*
        EtherealVoyage,		//*
        GiftOfLife,			//*
        ArcaneEmpowerment,	//*
        MortalStrike,
        ReactiveArmor,		//*
        Protection,			//*
        ArchProtection,
        MagicReflection,	//*
        Incognito,			//*
        Disguised,
        AnimalForm,
        Polymorph,
        Invisibility,		//*
        Paralyze,			//*
        Poison,
        Bleed,
        Clumsy,				//*
        FeebleMind,			//*
        Weaken,				//*
        Curse,				//*
        MassCurse,
        Agility,			//*
        Cunning,			//*
        Strength,			//*
        Bless,				//*
        Sleep,
        StoneForm,
        SpellPlague,
        Berserk,
        MassSleep,
        Fly,
        Inspire,
        Invigorate,
        Resilience,
        Perseverance,
        TribulationTarget,
        DespairTarget,
        FishPie = 0x426,
        HitLowerAttack,
        HitLowerDefense,
        DualWield,
        Block,
        DefenseMastery,
        DespairCaster,
        Healing,
        SpellFocusingBuff,
        SpellFocusingDebuff,
        RageFocusingDebuff,
        RageFocusingBuff,
        Warding,
        TribulationCaster,
        ForceArrow,
        Disarm,
        Surge,
        Feint,
        TalonStrike,
        PsychicAttack,
        ConsecrateWeapon,
        GrapesOfWrath,
        EnemyOfOneDebuff,
        HorrificBeast,
        LichForm,
        VampiricEmbrace,
        CurseWeapon,
        ReaperForm,
        ImmolatingWeapon,
        Enchant,
        HonorableExecution,
        Confidence,
        Evasion,
        CounterAttack,
        LightningStrike,
        MomentumStrike,
        OrangePetals,
        RoseOfTrinsic,
        PoisonImmunity,
        Veterinary,
        Perfection,
        Honored,
        ManaPhase,
        FanDancerFanFire,
        Rage,
        Webbing,
        MedusaStone,
        TrueFear,
        AuraOfNausea,
        HowlOfCacophony,
        GazeDespair,
        HiryuPhysicalResistance,
        RuneBeetleCorruption,
        BloodwormAnemia,
        RotwormBloodDisease,
        SkillUseDelay,
        FactionStatLoss,
        HeatOfBattleStatus,
        CriminalStatus,
        ArmorPierce,
        SplinteringEffect,
        SwingSpeedDebuff,
        WraithForm,
        CityTradeDeal = 0x466,
        HumilityDebuff = 0x467,
        Spirituality,
        Humility,
        // Skill Masteries
        Rampage,
        Stagger, // Debuff
        Toughness,
        Thrust,
        Pierce,   // Debuff
        PlayingTheOdds,
        FocusedEye,
        Onslaught, // Debuff
        ElementalFury,
        ElementalFuryDebuff, // Debuff
        CalledShot,
        Knockout,
        SavingThrow,
        Conduit,
        EtherealBurst,
        MysticWeapon,
        ManaShield,
        AnticipateHit,
        Warcry,
        Shadow,
        WhiteTigerForm,
        Bodyguard,
        HeightenedSenses,
        Tolerance,
        DeathRay,
        DeathRayDebuff,
        Intuition,
        EnchantedSummoning,
        ShieldBash,
        Whispering,
        CombatTraining,
        InjectedStrikeDebuff,
        InjectedStrike,
        UnknownTomato,
        PlayingTheOddsDebuff,
        DragonTurtleDebuff,
        Boarding,
        Potency,
        ThrustDebuff,
        FistsOfFury, // 1169
        BarrabHemolymphConcentrate,
        JukariBurnPoiltice,
        KurakAmbushersEssence,
        BarakoDraftOfMight,
        UraliTranceTonic,
        SakkhraProphylaxis
    }

    public sealed class AddBuffPacket : Packet
    {
        public AddBuffPacket(Mobile m, BuffInfo info)
            : this(m, info.ID, info.TitleCliloc, info.SecondaryCliloc, info.Args, (info.TimeStart != DateTime.MinValue) ? ((info.TimeStart + info.TimeLength) - DateTime.UtcNow) : TimeSpan.Zero)
        {
        }

        public AddBuffPacket(Mobile mob, BuffIcon iconID, int titleCliloc, int secondaryCliloc, TextDefinition args, TimeSpan length)
            : base(0xDF)
        {
            bool hasArgs = (args != null);

            this.EnsureCapacity((hasArgs ? (48 + args.ToString().Length * 2) : 44));
            this.m_Stream.Write((int)mob.Serial);

            this.m_Stream.Write((short)iconID);	//ID
            this.m_Stream.Write((short)0x1);	//Type 0 for removal. 1 for add 2 for Data

            this.m_Stream.Fill(4);

            this.m_Stream.Write((short)iconID);	//ID
            this.m_Stream.Write((short)0x01);	//Type 0 for removal. 1 for add 2 for Data

            this.m_Stream.Fill(4);

            if (length < TimeSpan.Zero)
                length = TimeSpan.Zero;

            this.m_Stream.Write((short)length.TotalSeconds);	//Time in seconds

            this.m_Stream.Fill(3);
            this.m_Stream.Write((int)titleCliloc);
            this.m_Stream.Write((int)secondaryCliloc);

            if (!hasArgs)
            {
                //m_Stream.Fill( 2 );
                this.m_Stream.Fill(10);
            }
            else
            {
                this.m_Stream.Fill(4);
                this.m_Stream.Write((short)0x1);	//Unknown -> Possibly something saying 'hey, I have more data!'?
                this.m_Stream.Fill(2);

                //m_Stream.WriteLittleUniNull( "\t#1018280" );
                this.m_Stream.WriteLittleUniNull(String.Format("\t{0}", args.ToString()));

                this.m_Stream.Write((short)0x1);	//Even more Unknown -> Possibly something saying 'hey, I have more data!'?
                this.m_Stream.Fill(2);
            }
        }
    }

    public sealed class RemoveBuffPacket : Packet
    {
        public RemoveBuffPacket(Mobile mob, BuffInfo info)
            : this(mob, info.ID)
        {
        }

        public RemoveBuffPacket(Mobile mob, BuffIcon iconID)
            : base(0xDF)
        {
            this.EnsureCapacity(13);
            this.m_Stream.Write((int)mob.Serial);

            this.m_Stream.Write((short)iconID);	//ID
            this.m_Stream.Write((short)0x0);	//Type 0 for removal. 1 for add 2 for Data

            this.m_Stream.Fill(4);
        }
    }
}