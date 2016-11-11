using Server;
using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
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
            PassiveTable = new Dictionary<Mobile, int>();

            EventSink.Login += new LoginEventHandler(OnLogin);

            Infos.Add(new MasteryInfo(typeof(SkillMasteries.InspireSpell),          700, SkillName.Provocation, Volume.Two));
            Infos.Add(new MasteryInfo(typeof(SkillMasteries.InvigorateSpell),       701, SkillName.Provocation, Volume.Three));

            Infos.Add(new MasteryInfo(typeof(SkillMasteries.ResilienceSpell),       702, SkillName.Peacemaking, Volume.Two));
            Infos.Add(new MasteryInfo(typeof(SkillMasteries.PerseveranceSpell),     703, SkillName.Peacemaking, Volume.Three));

            Infos.Add(new MasteryInfo(typeof(SkillMasteries.TribulationSpell),      704, SkillName.Discordance, Volume.Two));
            Infos.Add(new MasteryInfo(typeof(SkillMasteries.DespairSpell),          705, SkillName.Discordance, Volume.Three));

            /*Infos.Add(new MasteryInfo(typeof(SkillMasteries.DeathRaySpell),         706, SkillName.Magery,      Volume.Two));
            Infos.Add(new MasteryInfo(typeof(SkillMasteries.EtherealBurstSpell),    707, SkillName.Magery,      Volume.Three));

            Infos.Add(new MasteryInfo(typeof(SkillMasteries.NetherBlastSpell),      708, SkillName.Mysticism,   Volume.Two));
            Infos.Add(new MasteryInfo(typeof(SkillMasteries.MysticWeaponSpell),     709, SkillName.Mysticism,   Volume.Three));

            Infos.Add(new MasteryInfo(typeof(SkillMasteries.CommandUndeadSpell),    710, SkillName.Necromancy,  Volume.Two));
            Infos.Add(new MasteryInfo(typeof(SkillMasteries.ConduitSpell),          711, SkillName.Necromancy,  Volume.Three));

            Infos.Add(new MasteryInfo(typeof(SkillMasteries.ManaShieldSpell),       712, SkillName.Spellweaving, Volume.Two));
            Infos.Add(new MasteryInfo(typeof(SkillMasteries.SummonReaperSpell),     713, SkillName.Spellweaving, Volume.Three));

            Infos.Add(new MasteryInfo(typeof(SkillMasteries.WarcrySpell),           716, SkillName.Bushido,     Volume.Three));

            Infos.Add(new MasteryInfo(typeof(SkillMasteries.RejuvinateSpell),       718, SkillName.Chivalry,    Volume.Two));
            Infos.Add(new MasteryInfo(typeof(SkillMasteries.HolyFistSpell),         719, SkillName.Chivalry,    Volume.Three));

            Infos.Add(new MasteryInfo(typeof(SkillMasteries.ShadowSpell),           720, SkillName.Ninjitsu,    Volume.Two));
            Infos.Add(new MasteryInfo(typeof(SkillMasteries.WhiteTigerFormSpell),   721, SkillName.Ninjitsu,    Volume.Three));

            Infos.Add(new MasteryInfo(typeof(SkillMasteries.FlamingShotSpell),      722, SkillName.Archery,     Volume.Two));
            Infos.Add(new MasteryInfo(typeof(SkillMasteries.PlayingTheOddsSpell),   723, SkillName.Archery,     Volume.Three));

            Infos.Add(new MasteryInfo(typeof(SkillMasteries.ThrustSpell),           724, SkillName.Fencing,     Volume.Two));
            Infos.Add(new MasteryInfo(typeof(SkillMasteries.PierceSpell),           725, SkillName.Fencing,     Volume.Three));

            Infos.Add(new MasteryInfo(typeof(SkillMasteries.StaggerSpell),          726, SkillName.Macing,      Volume.Two));
            Infos.Add(new MasteryInfo(typeof(SkillMasteries.ToughnessSpell),        727, SkillName.Macing,      Volume.Three));

            Infos.Add(new MasteryInfo(typeof(SkillMasteries.OnslaughtSpell),        728, SkillName.Swords,      Volume.Two));
            Infos.Add(new MasteryInfo(typeof(SkillMasteries.FocusedEyeSpell),       729, SkillName.Swords,      Volume.Three));

            Infos.Add(new MasteryInfo(typeof(SkillMasteries.ElementalFurySpell),    730, SkillName.Throwing,    Volume.Two));
            Infos.Add(new MasteryInfo(typeof(SkillMasteries.CalledShotSpell),       731, SkillName.Throwing,    Volume.Three));

            Infos.Add(new MasteryInfo(typeof(SkillMasteries.ShieldBashSpell),       733, SkillName.Parry,       Volume.One));
            Infos.Add(new MasteryInfo(typeof(SkillMasteries.BodyGuardSpell),        734, SkillName.Parry,       Volume.Two));
            Infos.Add(new MasteryInfo(typeof(SkillMasteries.HeightenedSensesSpell), 735, SkillName.Parry,       Volume.Three));

            Infos.Add(new MasteryInfo(typeof(SkillMasteries.ToleranceSpell),        736, SkillName.Poisoning,   Volume.Two));
            Infos.Add(new MasteryInfo(typeof(SkillMasteries.InjectedStrikeSpell),   737, SkillName.Poisoning,   Volume.Three));
 
            Infos.Add(new MasteryInfo(typeof(SkillMasteries.RampageSpell),          739, SkillName.Wrestling,   Volume.Two));
            Infos.Add(new MasteryInfo(typeof(SkillMasteries.FistsOfFurySpell),      740, SkillName.Wrestling,   Volume.Three));
 
            Infos.Add(new MasteryInfo(typeof(SkillMasteries.WhisperingSpell),       742, SkillName.AnimalTaming, Volume.Two));
            Infos.Add(new MasteryInfo(typeof(SkillMasteries.CombatTrainingSpell),   743, SkillName.AnimalTaming, Volume.Three));

            //Passive Masteries
            Infos.Add(new MasteryInfo(null, 714, SkillName.Magery,          Volume.One, PassiveSpell.EnchantedSummoning));            // Enchanted Summoning
            Infos.Add(new MasteryInfo(null, 714, SkillName.Necromancy,      Volume.One, PassiveSpell.EnchantedSummoning));        // Enchanted Summoning
            Infos.Add(new MasteryInfo(null, 714, SkillName.Spellweaving,    Volume.One, PassiveSpell.EnchantedSummoning));      // Enchanted Summoning
            Infos.Add(new MasteryInfo(null, 714, SkillName.Mysticism,       Volume.One, PassiveSpell.EnchantedSummoning));         // Enchanted Summoning

            Infos.Add(new MasteryInfo(null, 715, SkillName.Bushido,         Volume.Two, PassiveSpell.AnticipateHit));           // Anticipate Hit

            Infos.Add(new MasteryInfo(null, 717, SkillName.Bushido,         Volume.One, PassiveSpell.Intuition));           // Intuition
            Infos.Add(new MasteryInfo(null, 717, SkillName.Ninjitsu,        Volume.One, PassiveSpell.Intuition));          // Intuition
            Infos.Add(new MasteryInfo(null, 717, SkillName.Chivalry,        Volume.One, PassiveSpell.Intuition));          // Intuition

            Infos.Add(new MasteryInfo(null, 732, SkillName.Archery,         Volume.One, PassiveSpell.SavingThrow));           // Saving Throw
            Infos.Add(new MasteryInfo(null, 732, SkillName.Fencing,         Volume.One, PassiveSpell.SavingThrow));           // Saving Throw
            Infos.Add(new MasteryInfo(null, 732, SkillName.Swords,          Volume.One, PassiveSpell.SavingThrow));            // Saving Throw
            Infos.Add(new MasteryInfo(null, 732, SkillName.Macing,          Volume.One, PassiveSpell.SavingThrow));            // Saving Throw
            Infos.Add(new MasteryInfo(null, 732, SkillName.Throwing,        Volume.One, PassiveSpell.SavingThrow));          // Saving Throw

            Infos.Add(new MasteryInfo(null, 738, SkillName.Poisoning,       Volume.One, PassiveSpell.Potency));         // Potency
            Infos.Add(new MasteryInfo(null, 741, SkillName.Wrestling,       Volume.One, PassiveSpell.Knockout));         // Knockout
            Infos.Add(new MasteryInfo(null, 744, SkillName.AnimalTaming,    Volume.One, PassiveSpell.Boarding));      // Boarding*/
        }

        public static List<MasteryInfo> Infos { get; set; }
        public static Dictionary<Mobile, int> PassiveTable { get; set; }

        public Type SpellType { get; set; }
        public int SpellID { get; set; }
        public SkillName MasterySkill { get; set; }
        public Volume Volume { get; set; }
        public int NameLocalization { get; set; }

        public bool Passive { get; set; }
        public SkillName SkillGroup { get; set; }
        public PassiveSpell PassiveSpell { get; set; }

        public MasteryInfo(Type skillType, int spellID, SkillName masterySkill, Volume volume, PassiveSpell passive = PassiveSpell.None)
        {
            SpellType = skillType;
            SpellID = spellID;
            MasterySkill = masterySkill;
            Volume = volume;

            if (skillType == null && passive != PassiveSpell.None)
            {
                PassiveSpell = passive;
                Passive = true;
            }

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

        public static Volume GetVolume(int spellID, SkillName skill)
        {
            MasteryInfo info = GetInfo(spellID, skill);

            if (info != null)
                return info.Volume;

            return Volume.None;
        }

        public static SkillName GetSkillForID(int spellID)
        {
            MasteryInfo info = GetInfo(spellID);

            if (info != null)
                return info.MasterySkill;

            return SkillName.Archery;
        }

        public static int GetLocalization(int spellID, SkillName skill)
        {
            MasteryInfo info = GetInfo(spellID, skill);

            if (info != null)
                return info.NameLocalization;

            return 0;
        }

        public static bool HasLearned(Mobile m, int spellID, SkillName skill)
        {
            MasteryInfo info = GetInfo(spellID, skill);

            return info != null && m.Skills[skill].HasLearnedMastery((int)info.Volume);
        }

        public static bool HasLearned(Mobile m, Type spell, SkillName skill)
        {
            MasteryInfo info = GetInfo(spell, skill);

            return info != null && m.Skills[info.MasterySkill].HasLearnedMastery((int)info.Volume);
        }

        public static int GetMasteryLevel(Mobile m, SkillName name)
        {
            return m.Skills[name].KnownMasteries;
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

        public static void LearnMastery(Mobile m, SkillName skill, Volume v)
        {
            m.Skills[skill].LearnMastery((int)v);
        }

        public static bool LearnMastery(Mobile m, int spellID, SkillName skill)
        {
            MasteryInfo info = GetInfo(spellID, skill);

            if (info != null && !HasLearned(m, spellID, skill))
            {
                m.Skills[info.MasterySkill].LearnMastery((int)info.Volume);
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
            List<SkillMasterySpell> list = SkillMasterySpell.GetSpells(m);

            if (list != null)
            {
                list.ForEach(spell =>
                {
                    spell.Expire();
                });

                list.Clear();
                list.TrimExcess();
            }

            /*RemovePassiveBuffs(m);

            foreach (MasteryInfo info in Infos.Where(i => i.Passive))
            {
                if (IsActivePassive(m, info.PassiveSpell))
                {
                    if (info.PassiveSpell == PassiveSpell.AnticipateHit)
                        continue;

                    //Console.WriteLine("Toggling {0} passive spell", info.PassiveSpell.ToString());

                    switch (info.PassiveSpell)
                    {
                        case PassiveSpell.EnchantedSummoning:
                            BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.EnchantedSummoning, 1155904, 1156090, String.Format("{0}\t{0}", EnchantedSummoningBonus(m).ToString()), true)); // +~1_STAMINA~ Stamina Regeneration and +~2_HP~% Hit Points for summoned pets.<br>Increased difficulty for summoned pets to be dispelled.
                            break;
                        case PassiveSpell.Intuition:
                            BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Intuition, 1155907, 1156089, IntuitionBonus(m).ToString(), true)); // Mana Increase ~1_VAL~
                            break;
                        case PassiveSpell.SavingThrow:
                            BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.SavingThrow, 1155922, 1156032, true)); // Provides a chance to block disarm attempts based on Mastery level, weapon skill level and tactics skill level.
                            break;
                        case PassiveSpell.Potency:
                            BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Potency, 1155928, 1156195, NonPoisonConsumeChance(m).ToString(), true)); // ~1_VAL~% chance to not consume poison charges when using infecting strike or injected strike.
                            break;
                        case PassiveSpell.Knockout:
                            BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Knockout, 1155931, 1156030, String.Format("{0}\t{1}", GetKnockoutModifier(m).ToString(), GetKnockoutModifier(m, true).ToString(), true))); // Wrestling Damage Bonus:<br>+~1_VAL~% PvM<br>+~2_VAL~% PvP
                            break;
                        case PassiveSpell.Boarding:
                            BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Boarding, 1155934, 1156194, BoardingSlotIncrease(m).ToString(), true)); // Your number of stable slots has been increased by ~1_VAL~.
                            break;
                    }

                    m.UpdateResistances();
                    PassiveTable[m] = info.SpellID;

                    if (m.Mana > m.ManaMax)
                        m.Mana = m.ManaMax;

                    break;
                }
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
            }*/
        }

        public static bool IsActivePassive(Mobile m, PassiveSpell spell)
        {
            if (m == null || m.Skills == null)
                return false;

            int id = GetSpellID(spell);

            foreach (MasteryInfo info in Infos)
            {
                SkillName current = m.Skills.CurrentMastery;

                if (id == info.SpellID && current == info.MasterySkill && HasLearned(m, id, current))
                {
                    return true;
                }
            }

            return false;
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
            /*Mobile m = e.Mobile;

            if ((int)m.Skills.CurrentMastery > 0)
                OnMasteryChanged(m, m.Skills.CurrentMastery);*/
        }

        public static int GetSpellID(PassiveSpell spell)
        {
            switch (spell)
            {
                case PassiveSpell.EnchantedSummoning: return 714;
                case PassiveSpell.AnticipateHit: return 715;
                case PassiveSpell.Intuition: return 716;
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
        /*public static int EnchantedSummoningBonus(BaseCreature bc)
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
                return (int)(m.Skills[SkillName.Bushido].Value / 2);
            }

            return 0;
        }

        public static int IntuitionBonus(Mobile m)
        {
            if (IsActivePassive(m, PassiveSpell.Intuition))
            {
                SkillName sk = m.Skills.CurrentMastery;

                return (int)((GetMasteryLevel(m, sk) * 40) / 8);
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

        public static int SavingThrowChance(Mobile m)
        {
            if (IsActivePassive(m, PassiveSpell.SavingThrow))
            {
                BaseWeapon wep = m.Weapon as BaseWeapon;

                if (wep != null && wep.DefSkill != SkillName.Wrestling)
                {
                    return (int)((m.Skills[wep.DefSkill].Value + m.Skills[SkillName.Tactics].Value) / 4.8); // 50% at 120/120
                }
            }

            return 0;
        }*/
        #endregion

        #region Mastery Skills
        public static SkillName[] Skills { get { return _Skills; } }

        private static SkillName[] _Skills =
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

        private static int[] _Descriptions =
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

        private static string[] _Titles =
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

            for(int i = 0; i < _Skills.Length; i++)
            {
                if (_Skills[i] == sk)
                    return _Titles[i];
            }

            return null;
        }
        #endregion
    }
}