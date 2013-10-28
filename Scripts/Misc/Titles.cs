using System;
using System.Text;
using Server.Engines.CannedEvil;
using Server.Items;
using Server.Mobiles;

namespace Server.Misc
{
    public class Titles
    {
        public const int MinFame = 0;
        public const int MaxFame = 15000;

        public static void AwardFame(Mobile m, int offset, bool message)
        {
            if (offset > 0)
            {
                if (m.Fame >= MaxFame)
                    return;

                offset -= m.Fame / 100;

                if (offset < 0)
                    offset = 0;
            }
            else if (offset < 0)
            {
                if (m.Fame <= MinFame)
                    return;

                offset -= m.Fame / 100;

                if (offset > 0)
                    offset = 0;
            }

            if ((m.Fame + offset) > MaxFame)
                offset = MaxFame - m.Fame;
            else if ((m.Fame + offset) < MinFame)
                offset = MinFame - m.Fame;

            m.Fame += offset;

            if (message)
            {
                if (offset > 40)
                    m.SendLocalizedMessage(1019054); // You have gained a lot of fame.
                else if (offset > 20)
                    m.SendLocalizedMessage(1019053); // You have gained a good amount of fame.
                else if (offset > 10)
                    m.SendLocalizedMessage(1019052); // You have gained some fame.
                else if (offset > 0)
                    m.SendLocalizedMessage(1019051); // You have gained a little fame.
                else if (offset < -40)
                    m.SendLocalizedMessage(1019058); // You have lost a lot of fame.
                else if (offset < -20)
                    m.SendLocalizedMessage(1019057); // You have lost a good amount of fame.
                else if (offset < -10)
                    m.SendLocalizedMessage(1019056); // You have lost some fame.
                else if (offset < 0)
                    m.SendLocalizedMessage(1019055); // You have lost a little fame.
            }
        }

        public const int MinKarma = -15000;
        public const int MaxKarma = 15000;

        public static void AwardKarma(Mobile m, int offset, bool message)
        {
            #region Mondain's Legacy
            if (m.Talisman is BaseTalisman)
            {
                BaseTalisman talisman = (BaseTalisman)m.Talisman;

                if (talisman.KarmaLoss > 0)
                    offset *= (1 + (int)(((double)talisman.KarmaLoss) / 100));
                else if (talisman.KarmaLoss < 0)
                    offset *= (1 - (int)(((double)-talisman.KarmaLoss) / 100));
            }
            #endregion

            #region Heritage Items
            int karmaLoss = AosAttributes.GetValue(m, AosAttribute.IncreasedKarmaLoss);

            if (karmaLoss != 0 && offset < 0)
            {
                offset -= (int)(offset * (karmaLoss / 100.0));
            }
            #endregion

            if (offset > 0)
            {
                if (m is PlayerMobile && ((PlayerMobile)m).KarmaLocked)
                    return;

                if (m.Karma >= MaxKarma)
                    return;

                offset -= m.Karma / 100;

                if (offset < 0)
                    offset = 0;
            }
            else if (offset < 0)
            {
                if (m.Karma <= MinKarma)
                    return;

                offset -= m.Karma / 100;

                if (offset > 0)
                    offset = 0;
            }

            if ((m.Karma + offset) > MaxKarma)
                offset = MaxKarma - m.Karma;
            else if ((m.Karma + offset) < MinKarma)
                offset = MinKarma - m.Karma;

            bool wasPositiveKarma = (m.Karma >= 0);

            m.Karma += offset;

            if (message)
            {
                if (offset > 40)
                    m.SendLocalizedMessage(1019062); // You have gained a lot of karma.
                else if (offset > 20)
                    m.SendLocalizedMessage(1019061); // You have gained a good amount of karma.
                else if (offset > 10)
                    m.SendLocalizedMessage(1019060); // You have gained some karma.
                else if (offset > 0)
                    m.SendLocalizedMessage(1019059); // You have gained a little karma.
                else if (offset < -40)
                    m.SendLocalizedMessage(1019066); // You have lost a lot of karma.
                else if (offset < -20)
                    m.SendLocalizedMessage(1019065); // You have lost a good amount of karma.
                else if (offset < -10)
                    m.SendLocalizedMessage(1019064); // You have lost some karma.
                else if (offset < 0)
                    m.SendLocalizedMessage(1019063); // You have lost a little karma.
            }

            if (!Core.AOS && wasPositiveKarma && m.Karma < 0 && m is PlayerMobile && !((PlayerMobile)m).KarmaLocked)
            {
                ((PlayerMobile)m).KarmaLocked = true;
                m.SendLocalizedMessage(1042511, "", 0x22); // Karma is locked.  A mantra spoken at a shrine will unlock it again.
            }
        }

        public static string[] HarrowerTitles = new string[] { "Spite", "Opponent", "Hunter", "Venom", "Executioner", "Annihilator", "Champion", "Assailant", "Purifier", "Nullifier" };

        public static string ComputeTitle(Mobile beholder, Mobile beheld)
        {
            StringBuilder title = new StringBuilder();

            int fame = beheld.Fame;
            int karma = beheld.Karma;

            bool showSkillTitle = beheld.ShowFameTitle && ((beholder == beheld) || (fame >= 5000));

            /*if ( beheld.Kills >= 5 )
            {
            title.AppendFormat( beheld.Fame >= 10000 ? "The Murderer {1} {0}" : "The Murderer {0}", beheld.Name, beheld.Female ? "Lady" : "Lord" );
            }
            else*/if (beheld.ShowFameTitle || (beholder == beheld))
            {
                for (int i = 0; i < m_FameEntries.Length; ++i)
                {
                    FameEntry fe = m_FameEntries[i];

                    if (fame <= fe.m_Fame || i == (m_FameEntries.Length - 1))
                    {
                        KarmaEntry[] karmaEntries = fe.m_Karma;

                        for (int j = 0; j < karmaEntries.Length; ++j)
                        {
                            KarmaEntry ke = karmaEntries[j];

                            if (karma <= ke.m_Karma || j == (karmaEntries.Length - 1))
                            {
                                title.AppendFormat(ke.m_Title, beheld.Name, beheld.Female ? "Lady" : "Lord");
                                break;
                            }
                        }

                        break;
                    }
                }
            }
            else
            {
                title.Append(beheld.Name);
            }

            if (beheld is PlayerMobile && ((PlayerMobile)beheld).DisplayChampionTitle)
            {
                PlayerMobile.ChampionTitleInfo info = ((PlayerMobile)beheld).ChampionTitles;

                if (info.Harrower > 0)
                    title.AppendFormat(": {0} of Evil", HarrowerTitles[Math.Min(HarrowerTitles.Length, info.Harrower) - 1]);
                else
                {
                    int highestValue = 0, highestType = 0;
                    for (int i = 0; i < ChampionSpawnInfo.Table.Length; i++)
                    {
                        int v = info.GetValue(i);

                        if (v > highestValue)
                        {
                            highestValue = v;
                            highestType = i;
                        }
                    }

                    int offset = 0;
                    if (highestValue > 800)
                        offset = 3;
                    else if (highestValue > 300)
                        offset = (int)(highestValue / 300);

                    if (offset > 0)
                    {
                        ChampionSpawnInfo champInfo = ChampionSpawnInfo.GetInfo((ChampionSpawnType)highestType);
                        title.AppendFormat(": {0} of the {1}", champInfo.LevelNames[Math.Min(offset, champInfo.LevelNames.Length) - 1], champInfo.Name);
                    }
                }
            }

            string customTitle = beheld.Title;

            if (customTitle != null && (customTitle = customTitle.Trim()).Length > 0)
            {
                title.AppendFormat(" {0}", customTitle);
            }
            else if (showSkillTitle && beheld.Player)
            {
                string skillTitle = GetSkillTitle(beheld);

                if (skillTitle != null)
                {
                    title.Append(", ").Append(skillTitle);
                }
            }

            return title.ToString();
        }

        public static string GetSkillTitle(Mobile mob)
        {
            Skill highest = GetHighestSkill(mob);// beheld.Skills.Highest;

            if (highest != null && highest.BaseFixedPoint >= 300)
            {
                string skillLevel = GetSkillLevel(highest);
                string skillTitle = highest.Info.Title;

                if (mob.Female && skillTitle.EndsWith("man"))
                    skillTitle = skillTitle.Substring(0, skillTitle.Length - 3) + "woman";

                return String.Concat(skillLevel, " ", skillTitle);
            }

            return null;
        }

        private static Skill GetHighestSkill(Mobile m)
        {
            Skills skills = m.Skills;

            if (!Core.AOS)
                return skills.Highest;

            Skill highest = null;

            for (int i = 0; i < m.Skills.Length; ++i)
            {
                Skill check = m.Skills[i];

                if (highest == null || check.BaseFixedPoint > highest.BaseFixedPoint)
                    highest = check;
                else if (highest != null && highest.Lock != SkillLock.Up && check.Lock == SkillLock.Up && check.BaseFixedPoint == highest.BaseFixedPoint)
                    highest = check;
            }

            return highest;
        }

        private static readonly string[,] m_Levels = new string[,]
        {
            { "Neophyte", "Neophyte", "Neophyte" },
            { "Novice", "Novice", "Novice" },
            { "Apprentice", "Apprentice", "Apprentice" },
            { "Journeyman", "Journeyman", "Journeyman" },
            { "Expert", "Expert", "Expert" },
            { "Adept", "Adept", "Adept" },
            { "Master", "Master", "Master" },
            { "Grandmaster", "Grandmaster", "Grandmaster" },
            { "Elder", "Tatsujin", "Shinobi" },
            { "Legendary", "Kengo", "Ka-ge" }
        };

        private static string GetSkillLevel(Skill skill)
        {
            return m_Levels[GetTableIndex(skill), GetTableType(skill)];
        }

        private static int GetTableType(Skill skill)
        {
            switch ( skill.SkillName )
            {
                default:
                    return 0;
                case SkillName.Bushido:
                    return 1;
                case SkillName.Ninjitsu:
                    return 2;
            }
        }

        private static int GetTableIndex(Skill skill)
        {
            int fp = Math.Min(skill.BaseFixedPoint, 1200);

            return (fp - 300) / 100;
        }

        private static readonly FameEntry[] m_FameEntries = new FameEntry[]
        {
            new FameEntry(1249, new KarmaEntry[]
            {
                new KarmaEntry(-10000, "The Outcast {0}"),
                new KarmaEntry(-5000, "The Despicable {0}"),
                new KarmaEntry(-2500, "The Scoundrel {0}"),
                new KarmaEntry(-1250, "The Unsavory {0}"),
                new KarmaEntry(-625, "The Rude {0}"),
                new KarmaEntry(624, "{0}"),
                new KarmaEntry(1249, "The Fair {0}"),
                new KarmaEntry(2499, "The Kind {0}"),
                new KarmaEntry(4999, "The Good {0}"),
                new KarmaEntry(9999, "The Honest {0}"),
                new KarmaEntry(10000, "The Trustworthy {0}")
            }),
            new FameEntry(2499, new KarmaEntry[]
            {
                new KarmaEntry(-10000, "The Wretched {0}"),
                new KarmaEntry(-5000, "The Dastardly {0}"),
                new KarmaEntry(-2500, "The Malicious {0}"),
                new KarmaEntry(-1250, "The Dishonorable {0}"),
                new KarmaEntry(-625, "The Disreputable {0}"),
                new KarmaEntry(624, "The Notable {0}"),
                new KarmaEntry(1249, "The Upstanding {0}"),
                new KarmaEntry(2499, "The Respectable {0}"),
                new KarmaEntry(4999, "The Honorable {0}"),
                new KarmaEntry(9999, "The Commendable {0}"),
                new KarmaEntry(10000, "The Estimable {0}")
            }),
            new FameEntry(4999, new KarmaEntry[]
            {
                new KarmaEntry(-10000, "The Nefarious {0}"),
                new KarmaEntry(-5000, "The Wicked {0}"),
                new KarmaEntry(-2500, "The Vile {0}"),
                new KarmaEntry(-1250, "The Ignoble {0}"),
                new KarmaEntry(-625, "The Notorious {0}"),
                new KarmaEntry(624, "The Prominent {0}"),
                new KarmaEntry(1249, "The Reputable {0}"),
                new KarmaEntry(2499, "The Proper {0}"),
                new KarmaEntry(4999, "The Admirable {0}"),
                new KarmaEntry(9999, "The Famed {0}"),
                new KarmaEntry(10000, "The Great {0}")
            }),
            new FameEntry(9999, new KarmaEntry[]
            {
                new KarmaEntry(-10000, "The Dread {0}"),
                new KarmaEntry(-5000, "The Evil {0}"),
                new KarmaEntry(-2500, "The Villainous {0}"),
                new KarmaEntry(-1250, "The Sinister {0}"),
                new KarmaEntry(-625, "The Infamous {0}"),
                new KarmaEntry(624, "The Renowned {0}"),
                new KarmaEntry(1249, "The Distinguished {0}"),
                new KarmaEntry(2499, "The Eminent {0}"),
                new KarmaEntry(4999, "The Noble {0}"),
                new KarmaEntry(9999, "The Illustrious {0}"),
                new KarmaEntry(10000, "The Glorious {0}")
            }),
            new FameEntry(10000, new KarmaEntry[]
            {
                new KarmaEntry(-10000, "The Dread {1} {0}"),
                new KarmaEntry(-5000, "The Evil {1} {0}"),
                new KarmaEntry(-2500, "The Dark {1} {0}"),
                new KarmaEntry(-1250, "The Sinister {1} {0}"),
                new KarmaEntry(-625, "The Dishonored {1} {0}"),
                new KarmaEntry(624, "{1} {0}"),
                new KarmaEntry(1249, "The Distinguished {1} {0}"),
                new KarmaEntry(2499, "The Eminent {1} {0}"),
                new KarmaEntry(4999, "The Noble {1} {0}"),
                new KarmaEntry(9999, "The Illustrious {1} {0}"),
                new KarmaEntry(10000, "The Glorious {1} {0}")
            })
        };
    }

    public class FameEntry
    {
        public int m_Fame;
        public KarmaEntry[] m_Karma;

        public FameEntry(int fame, KarmaEntry[] karma)
        {
            this.m_Fame = fame;
            this.m_Karma = karma;
        }
    }

    public class KarmaEntry
    {
        public int m_Karma;
        public string m_Title;

        public KarmaEntry(int karma, string title)
        {
            this.m_Karma = karma;
            this.m_Title = title;
        }
    }
}