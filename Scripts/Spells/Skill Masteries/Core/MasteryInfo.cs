using Server.Items;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Spells.SkillMasteries
{
    public enum Volume
    {
        None,
        One,
        Two,
        Three
    }

    public enum PassiveSpell
    {
        None,
        EnchantedSummoning,
        Intuition,
        SavingThrow,
        Potency,
        Knockout,
        Boarding,
        AnticipateHit
    }

    public class MasteryInfo
    {
        public static readonly int MinSkillRequirement = 90;

        public static void Configure()
        {
            Infos = new List<MasteryInfo>();

            EventSink.Login += OnLogin;

            Infos.Add(new MasteryInfo(typeof(InspireSpell), 700, SkillName.Provocation));
            Infos.Add(new MasteryInfo(typeof(InvigorateSpell), 701, SkillName.Provocation));

            Infos.Add(new MasteryInfo(typeof(ResilienceSpell), 702, SkillName.Peacemaking));
            Infos.Add(new MasteryInfo(typeof(PerseveranceSpell), 703, SkillName.Peacemaking));

            Infos.Add(new MasteryInfo(typeof(TribulationSpell), 704, SkillName.Discordance));
            Infos.Add(new MasteryInfo(typeof(DespairSpell), 705, SkillName.Discordance));

            Infos.Add(new MasteryInfo(typeof(DeathRaySpell), 706, SkillName.Magery));
            Infos.Add(new MasteryInfo(typeof(EtherealBurstSpell), 707, SkillName.Magery));

            Infos.Add(new MasteryInfo(typeof(NetherBlastSpell), 708, SkillName.Mysticism));
            Infos.Add(new MasteryInfo(typeof(MysticWeaponSpell), 709, SkillName.Mysticism));

            Infos.Add(new MasteryInfo(typeof(CommandUndeadSpell), 710, SkillName.Necromancy));
            Infos.Add(new MasteryInfo(typeof(ConduitSpell), 711, SkillName.Necromancy));

            Infos.Add(new MasteryInfo(typeof(ManaShieldSpell), 712, SkillName.Spellweaving));
            Infos.Add(new MasteryInfo(typeof(SummonReaperSpell), 713, SkillName.Spellweaving));

            Infos.Add(new MasteryInfo(typeof(WarcrySpell), 716, SkillName.Bushido));

            Infos.Add(new MasteryInfo(typeof(RejuvinateSpell), 718, SkillName.Chivalry));
            Infos.Add(new MasteryInfo(typeof(HolyFistSpell), 719, SkillName.Chivalry));

            Infos.Add(new MasteryInfo(typeof(ShadowSpell), 720, SkillName.Ninjitsu));
            Infos.Add(new MasteryInfo(typeof(WhiteTigerFormSpell), 721, SkillName.Ninjitsu));

            Infos.Add(new MasteryInfo(typeof(FlamingShotSpell), 722, SkillName.Archery));
            Infos.Add(new MasteryInfo(typeof(PlayingTheOddsSpell), 723, SkillName.Archery));

            Infos.Add(new MasteryInfo(typeof(ThrustSpell), 724, SkillName.Fencing));
            Infos.Add(new MasteryInfo(typeof(PierceSpell), 725, SkillName.Fencing));

            Infos.Add(new MasteryInfo(typeof(StaggerSpell), 726, SkillName.Macing));
            Infos.Add(new MasteryInfo(typeof(ToughnessSpell), 727, SkillName.Macing));

            Infos.Add(new MasteryInfo(typeof(OnslaughtSpell), 728, SkillName.Swords));
            Infos.Add(new MasteryInfo(typeof(FocusedEyeSpell), 729, SkillName.Swords));

            Infos.Add(new MasteryInfo(typeof(ElementalFurySpell), 730, SkillName.Throwing));
            Infos.Add(new MasteryInfo(typeof(CalledShotSpell), 731, SkillName.Throwing));

            Infos.Add(new MasteryInfo(typeof(ShieldBashSpell), 733, SkillName.Parry));
            Infos.Add(new MasteryInfo(typeof(BodyGuardSpell), 734, SkillName.Parry));
            Infos.Add(new MasteryInfo(typeof(HeightenedSensesSpell), 735, SkillName.Parry));

            Infos.Add(new MasteryInfo(typeof(ToleranceSpell), 736, SkillName.Poisoning));
            Infos.Add(new MasteryInfo(typeof(InjectedStrikeSpell), 737, SkillName.Poisoning));

            Infos.Add(new MasteryInfo(typeof(RampageSpell), 739, SkillName.Wrestling));
            Infos.Add(new MasteryInfo(typeof(FistsOfFurySpell), 740, SkillName.Wrestling));

            Infos.Add(new MasteryInfo(typeof(WhisperingSpell), 742, SkillName.AnimalTaming));
            Infos.Add(new MasteryInfo(typeof(CombatTrainingSpell), 743, SkillName.AnimalTaming));

            //Passive Masteries
            Infos.Add(new MasteryInfo(null, 714, SkillName.Magery, PassiveSpell.EnchantedSummoning));            // Enchanted Summoning
            Infos.Add(new MasteryInfo(null, 714, SkillName.Necromancy, PassiveSpell.EnchantedSummoning));        // Enchanted Summoning
            Infos.Add(new MasteryInfo(null, 714, SkillName.Spellweaving, PassiveSpell.EnchantedSummoning));      // Enchanted Summoning
            Infos.Add(new MasteryInfo(null, 714, SkillName.Mysticism, PassiveSpell.EnchantedSummoning));         // Enchanted Summoning

            Infos.Add(new MasteryInfo(null, 715, SkillName.Bushido, PassiveSpell.AnticipateHit));           // Anticipate Hit

            Infos.Add(new MasteryInfo(null, 717, SkillName.Bushido, PassiveSpell.Intuition));           // Intuition
            Infos.Add(new MasteryInfo(null, 717, SkillName.Ninjitsu, PassiveSpell.Intuition));          // Intuition
            Infos.Add(new MasteryInfo(null, 717, SkillName.Chivalry, PassiveSpell.Intuition));          // Intuition

            Infos.Add(new MasteryInfo(null, 732, SkillName.Archery, PassiveSpell.SavingThrow));           // Saving Throw
            Infos.Add(new MasteryInfo(null, 732, SkillName.Fencing, PassiveSpell.SavingThrow));           // Saving Throw
            Infos.Add(new MasteryInfo(null, 732, SkillName.Swords, PassiveSpell.SavingThrow));            // Saving Throw
            Infos.Add(new MasteryInfo(null, 732, SkillName.Macing, PassiveSpell.SavingThrow));            // Saving Throw
            Infos.Add(new MasteryInfo(null, 732, SkillName.Throwing, PassiveSpell.SavingThrow));          // Saving Throw

            Infos.Add(new MasteryInfo(null, 738, SkillName.Poisoning, PassiveSpell.Potency));         // Potency
            Infos.Add(new MasteryInfo(null, 741, SkillName.Wrestling, PassiveSpell.Knockout));         // Knockout
            Infos.Add(new MasteryInfo(null, 744, SkillName.AnimalTaming, PassiveSpell.Boarding));      // Boarding
        }

        public static List<MasteryInfo> Infos { get; set; }

        public Type SpellType { get; set; }
        public int SpellID { get; set; }
        public SkillName MasterySkill { get; set; }
        public int NameLocalization { get; set; }

        public bool Passive => PassiveSpell != PassiveSpell.None;
        public PassiveSpell PassiveSpell { get; set; }

        public MasteryInfo(Type skillType, int spellID, SkillName masterySkill, PassiveSpell passive = PassiveSpell.None)
        {
            SpellType = skillType;
            SpellID = spellID;

            MasterySkill = masterySkill;
            PassiveSpell = passive;

            NameLocalization = GetLocalization(masterySkill);
        }

        public static MasteryInfo GetInfo(Type spell, SkillName skill)
        {
            return Infos.FirstOrDefault(info => info.SpellType == spell && info.MasterySkill == skill);
        }

        public static MasteryInfo GetInfo(int spellID)
        {
            return Infos.FirstOrDefault(info => info.SpellID == spellID);
        }

        public static MasteryInfo GetInfo(int spellID, SkillName name)
        {
            return Infos.FirstOrDefault(info => info.SpellID == spellID && info.MasterySkill == name);
        }

        /// <summary>
        /// Obsolete, used to serialize old types
        /// </summary>
        /// <param name="spellID"></param>
        /// <param name="skill"></param>
        /// <returns></returns>
        public static int GetVolume(int spellID, SkillName skill)
        {
            if (IsPassiveMastery(spellID) || spellID == 733)
                return 1;
            else if (spellID <= 715)
            {
                if (spellID % 2 == 0)
                    return 3;

                return 2;
            }
            else if (spellID <= 731 || (spellID >= 736 && spellID <= 743))
            {
                if (spellID % 2 == 0)
                    return 2;

                return 3;
            }
            else if (spellID == 734)
                return 2;
            else if (spellID == 735)
                return 3;

            return 1;
        }

        public static SkillName GetSkillForID(int spellID)
        {
            MasteryInfo info = GetInfo(spellID);

            if (info != null)
                return info.MasterySkill;

            return SkillName.Archery;
        }

        public static bool HasLearned(Mobile m, SkillName skill)
        {
            return m.Skills[skill].HasLearnedMastery();
        }

        public static bool HasLearned(Mobile m, SkillName skill, int volume)
        {
            return m.Skills[skill].HasLearnedVolume(volume);
        }

        public static bool HasLearned(Mobile m, Type spell, SkillName skill)
        {
            MasteryInfo info = GetInfo(spell, skill);

            return info != null && m.Skills[info.MasterySkill].HasLearnedMastery();
        }

        public static int GetMasteryLevel(Mobile m, SkillName name)
        {
            return m.Skills[name].VolumeLearned;
        }

        public static bool CanLearn(Mobile m, int spellID, SkillName skill)
        {
            MasteryInfo info = GetInfo(spellID, skill);

            if (info != null)
            {
                return m.Skills[info.MasterySkill].Base >= MinSkillRequirement;
            }

            return false;
        }

        public static bool LearnMastery(Mobile m, SkillName skill, int volume)
        {
            if (GetMasteryLevel(m, skill) < volume)
            {
                m.Skills[skill].LearnMastery(volume);
                return true;
            }

            return false;
        }

        public static bool IsPassiveMastery(int spellID)
        {
            return spellID == 714 || spellID == 715 || spellID == 716 || spellID == 732 || spellID == 738 || spellID == 741 || spellID == 744;
        }

        public static void OnMasteryChanged(Mobile m, SkillName oldMastery)
        {
            PassiveSpell passive = GetActivePassive(m);
            SkillName newMastery = m.Skills.CurrentMastery;

            if (oldMastery != newMastery)
            {
                List<SkillMasterySpell> list = SkillMasterySpell.GetSpells(m);

                if (list != null)
                {
                    list.ForEach(spell =>
                    {
                        spell.Expire();
                    });

                    ColUtility.Free(list);
                }

                if (m is PlayerMobile && oldMastery == SkillName.Necromancy)
                {
                    ((PlayerMobile)m).AllFollowers.IterateReverse(mob =>
                    {
                        if (mob is BaseCreature && CommandUndeadSpell.ValidateTarget((BaseCreature)mob))
                        {
                            ((BaseCreature)mob).SetControlMaster(null);
                        }
                    });
                }

                SpecialMove move = SpecialMove.GetCurrentMove(m);

                if (move is SkillMasteryMove)
                    SpecialMove.ClearCurrentMove(m);

                m.RemoveStatMod("SavingThrow_Str");

                ColUtility.Free(list);
                RemovePassiveBuffs(m);
            }

            if (passive != PassiveSpell.None && passive != PassiveSpell.AnticipateHit)
            {
                switch (passive)
                {
                    case PassiveSpell.EnchantedSummoning:
                        BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.EnchantedSummoning, 1155904, 1156090, string.Format("{0}\t{0}", EnchantedSummoningBonus(m).ToString()), true)); // +~1_STAMINA~ Stamina Regeneration and +~2_HP~% Hit Points for summoned pets.<br>Increased difficulty for summoned pets to be dispelled.
                        break;
                    case PassiveSpell.Intuition:
                        BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Intuition, 1155907, 1156089, IntuitionBonus(m).ToString(), true)); // Mana Increase ~1_VAL~
                        break;
                    case PassiveSpell.SavingThrow:
                        {
                            string args = null;

                            switch (GetMasteryLevel(m, newMastery))
                            {
                                default: args = "5\t0\t0\t0"; break;
                                case 2: args = "5\t5\t0\t0"; break;
                                case 3: args = "5\t5\t5\t5"; break;
                            }

                            m.AddStatMod(new StatMod(StatType.Str, "SavingThrow_Str", 5, TimeSpan.Zero));
                            BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.SavingThrow, 1156031, 1156032, args, true)); // Provides a chance to block disarm attempts based on Mastery level, weapon skill level and tactics skill level.
                        }
                        break;
                    case PassiveSpell.Potency:
                        BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Potency, 1155928, 1156195, NonPoisonConsumeChance(m).ToString(), true)); // ~1_VAL~% chance to not consume poison charges when using infecting strike or injected strike.
                        break;
                    case PassiveSpell.Knockout:
                        BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Knockout, 1155931, 1156030, string.Format("{0}\t{1}", GetKnockoutModifier(m).ToString(), GetKnockoutModifier(m, true).ToString(), true))); // Wrestling Damage Bonus:<br>+~1_VAL~% PvM<br>+~2_VAL~% PvP
                        break;
                    case PassiveSpell.Boarding:
                        BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Boarding, 1155934, 1156194, BoardingSlotIncrease(m).ToString(), true)); // Your number of stable slots has been increased by ~1_VAL~.
                        break;
                }

                m.Delta(MobileDelta.WeaponDamage);
                m.UpdateResistances();

                if (m.Mana > m.ManaMax)
                    m.Mana = m.ManaMax;
            }

            if (m.Backpack != null)
            {
                foreach (Item item in m.Backpack.FindItemsByType(typeof(BookOfMasteries)))
                {
                    BookOfMasteries book = item as BookOfMasteries;

                    if (book != null)
                        book.InvalidateProperties();
                }
            }

            foreach (Item item in m.Items.Where(i => i is BookOfMasteries))
            {
                BookOfMasteries book = item as BookOfMasteries;

                if (book != null)
                    book.InvalidateProperties();
            }
        }

        public static PassiveSpell GetActivePassive(Mobile m)
        {
            if (m == null || m.Skills == null || Infos == null)
                return PassiveSpell.None;

            SkillName mastery = m.Skills.CurrentMastery;

            MasteryInfo info = Infos.FirstOrDefault(i => i.Passive && i.MasterySkill == mastery && i.PassiveSpell != PassiveSpell.AnticipateHit);

            if (info != null)
                return info.PassiveSpell;

            return PassiveSpell.None;
        }

        public static bool IsActivePassive(Mobile m, PassiveSpell spell)
        {
            if (spell == PassiveSpell.AnticipateHit)
                return m.Skills.CurrentMastery == SkillName.Bushido;

            return GetActivePassive(m) == spell;
        }

        public static void RemovePassiveBuffs(Mobile m)
        {
            BuffInfo.RemoveBuff(m, BuffIcon.EnchantedSummoning);
            BuffInfo.RemoveBuff(m, BuffIcon.AnticipateHit);
            BuffInfo.RemoveBuff(m, BuffIcon.Intuition);
            BuffInfo.RemoveBuff(m, BuffIcon.SavingThrow);
            BuffInfo.RemoveBuff(m, BuffIcon.Potency);
            BuffInfo.RemoveBuff(m, BuffIcon.Knockout);
            BuffInfo.RemoveBuff(m, BuffIcon.Boarding);
        }

        public static void OnLogin(LoginEventArgs e)
        {
            Mobile m = e.Mobile;

            if (m.Skills.CurrentMastery > 0)
                OnMasteryChanged(m, m.Skills.CurrentMastery);
        }

        public static int GetSpellID(PassiveSpell spell)
        {
            switch (spell)
            {
                case PassiveSpell.EnchantedSummoning: return 714;
                case PassiveSpell.AnticipateHit: return 715;
                case PassiveSpell.Intuition: return 717;
                case PassiveSpell.SavingThrow: return 732;
                case PassiveSpell.Potency: return 738;
                case PassiveSpell.Knockout: return 741;
                case PassiveSpell.Boarding: return 744;
            }

            return -1;
        }

        public static int GetSpellID(SkillName name)
        {
            MasteryInfo info = Infos.FirstOrDefault(i => i.MasterySkill == name && i.Passive);

            if (info == null)
                return -1;

            return info.SpellID;
        }

        public static int GetLocalization(SkillName name)
        {
            switch (name)
            {
                default:
                case SkillName.Discordance: return 1151945;
                case SkillName.Provocation: return 1151946;
                case SkillName.Peacemaking: return 1151947;
                case SkillName.Magery: return 1155771;
                case SkillName.Mysticism: return 1155772;
                case SkillName.Necromancy: return 1155773;
                case SkillName.Spellweaving: return 1155774;
                case SkillName.Bushido: return 1155775;
                case SkillName.Chivalry: return 1155776;
                case SkillName.Ninjitsu: return 1155777;
                case SkillName.Archery: return 1155786;
                case SkillName.Fencing: return 1155778;
                case SkillName.Macing: return 1155779;
                case SkillName.Swords: return 1155780;
                case SkillName.Throwing: return 1155781;
                case SkillName.Parry: return 1155782;
                case SkillName.Poisoning: return 1155783;
                case SkillName.Wrestling: return 1155784;
                case SkillName.AnimalTaming: return 1155785;
            }
        }

        #region Passive Bonuses/Maluses
        public static int EnchantedSummoningBonus(BaseCreature bc)
        {
            if (bc.Summoned)
                return EnchantedSummoningBonus(bc.SummonMaster);

            return 0;
        }

        public static int EnchantedSummoningBonus(Mobile m)
        {
            if (IsActivePassive(m, PassiveSpell.EnchantedSummoning))
            {
                SkillName sk = m.Skills.CurrentMastery;

                return (int)((m.Skills[sk].Value + (GetMasteryLevel(m, sk) * 40)) / 16);
            }

            return 0;
        }

        public static int AnticipateHitBonus(Mobile m)
        {
            if (IsActivePassive(m, PassiveSpell.AnticipateHit))
            {
                return (int)(m.Skills[SkillName.Bushido].Value * .67);
            }

            return 0;
        }

        public static int IntuitionBonus(Mobile m)
        {
            if (IsActivePassive(m, PassiveSpell.Intuition))
            {
                SkillName sk = m.Skills.CurrentMastery;

                return (GetMasteryLevel(m, sk) * 40) / 8;
            }

            return 0;
        }

        public static int NonPoisonConsumeChance(Mobile m)
        {
            if (IsActivePassive(m, PassiveSpell.Potency))
            {
                double skill = m.Skills[SkillName.Poisoning].Value + m.Skills[SkillName.Anatomy].Value;
                skill += GetMasteryLevel(m, SkillName.Poisoning) * 20;

                return (int)(skill / 4.375);
            }

            return 0;
        }

        public static int GetKnockoutModifier(Mobile m, bool pvp = false)
        {
            if (IsActivePassive(m, PassiveSpell.Knockout))
            {
                switch (GetMasteryLevel(m, SkillName.Wrestling))
                {
                    default: return 0;
                    case 3: return pvp ? 50 : 100;
                    case 2: return pvp ? 25 : 50;
                    case 1: return pvp ? 10 : 25;
                }
            }

            return 0;
        }

        public static int BoardingSlotIncrease(Mobile m)
        {
            if (IsActivePassive(m, PassiveSpell.Boarding))
            {
                return GetMasteryLevel(m, SkillName.AnimalTaming);
            }

            return 0;
        }

        public static int SavingThrowChance(Mobile m, AosAttribute attr)
        {
            if (IsActivePassive(m, PassiveSpell.SavingThrow))
            {
                int level = GetMasteryLevel(m, m.Skills.CurrentMastery);

                if (level <= 0)
                    return 0;

                switch (attr)
                {
                    case AosAttribute.AttackChance: return 5;
                    case AosAttribute.DefendChance: return level > 1 ? 5 : 0;
                    case AosAttribute.BonusStr: return level > 2 ? 5 : 0;
                    case AosAttribute.WeaponDamage: return level > 2 ? 5 : 0;
                }
            }

            return 0;
        }
        #endregion

        #region Mastery Skills
        public static SkillName[] Skills => _Skills;

        private static readonly SkillName[] _Skills =
        {
            SkillName.Peacemaking,
            SkillName.Provocation,
            SkillName.Discordance,
            SkillName.Magery,
            SkillName.Mysticism,
            SkillName.Necromancy,
            SkillName.Spellweaving,
            SkillName.Bushido,
            SkillName.Chivalry,
            SkillName.Ninjitsu,
            SkillName.Fencing,
            SkillName.Macing,
            SkillName.Swords,
            SkillName.Throwing,
            SkillName.Parry,
            SkillName.Poisoning,
            SkillName.Wrestling,
            SkillName.AnimalTaming,
            SkillName.Archery
        };

        private static readonly int[] _Descriptions =
        {
            1115707,
            1115706,
            1115708,
            1156303,
            1156316,
            1156311,
            1156315,
            1156313,
            1156312,
            1156314,
            1156309,
            1156308,
            1156307,
            1156317,
            1156302,
            1156304,
            1156310,
            1156306,
            1156305
        };

        private static readonly string[] _Titles =
        {
            "the Galvanizer",
            "the Exhilarator",
            "the Desponder",
            "the Marvelous",
            "the Enigmatic",
            "the Undying",
            "the Mysterious",
            "the Disciplined",
            "the Courageous",
            "the Unseen",
            "the Needle",
            "the Crushing",
            "the Blade",
            "the Precise",
            "the Deflector",
            "the Lethal",
            "the Champion",
            "the Beastmaster",
            "the Exact"
        };

        public static int GetDescription(Mobile m)
        {
            SkillName sk = m.Skills.CurrentMastery;

            for (int i = 0; i < _Skills.Length; i++)
            {
                if (_Skills[i] == sk)
                    return _Descriptions[i];
            }

            return -1;
        }

        public static string GetTitle(Mobile m)
        {
            SkillName sk = m.Skills.CurrentMastery;

            for (int i = 0; i < _Skills.Length; i++)
            {
                if (_Skills[i] == sk)
                    return _Titles[i];
            }

            return null;
        }
        #endregion
    }
}
