using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Engines.Quests
{
    public interface ITierQuest
    {
        TierQuestInfo TierInfo { get; }
        TimeSpan RestartDelay { get; }
    }

    public interface ITierQuester
    {
        TierQuestInfo TierInfo { get; }
    }

    public class TierQuestInfo
    {
        public Type Quester { get; set; }
        public TierInfo[] Tiers { get; set; }

        public TierQuestInfo(Type quester, params TierInfo[] tiers)
        {
            Quester = quester;
            Tiers = tiers;
        }

        public static TierQuestInfo Percolem { get; set; }
        public static TierQuestInfo Thepem { get; set; }
        public static TierQuestInfo Zosilem { get; set; }

        static TierQuestInfo()
        {
            Percolem = new TierQuestInfo(typeof(Percolem),
                new TierInfo(0, TimeSpan.FromMinutes(30), typeof(BouraBouraQuest), typeof(RaptorliciousQuest), typeof(TheSlithWarsQuest)),
                new TierInfo(5, TimeSpan.FromMinutes(120), typeof(AmbushingTheAmbushersQuest), typeof(BouraBouraAndMoreBouraQuest), typeof(RevengeOfTheSlithQuest), typeof(WeveGotAnAntProblemQuest)),
                new TierInfo(10, TimeSpan.FromMinutes(1440), typeof(ItMakesMeSickQuest), typeof(ItsAMadMadWorldQuest), typeof(TheDreamersQuest)));

            Thepem = new TierQuestInfo(typeof(Thepem),
                new TierInfo(0, TimeSpan.FromMinutes(30), typeof(AllThatGlitters), typeof(TastyTreats)),
                new TierInfo(5, TimeSpan.FromMinutes(120), typeof(MetalHead), typeof(PinkistheNewBlack)));

            Zosilem = new TierQuestInfo(typeof(Zosilem),
                new TierInfo(0, TimeSpan.FromMinutes(30), typeof(DabblingontheDarkSide), typeof(TheBrainyAlchemist)),
                new TierInfo(5, TimeSpan.FromMinutes(120), typeof(ArmorUp), typeof(ToTurnBaseMetalIntoVerite)),
                new TierInfo(10, TimeSpan.FromMinutes(1440), typeof(PureValorite), typeof(TheForbiddenFruit)));
        }

        public static TimeSpan GetCooldown(TierQuestInfo tierInfo, Type questType)
        {
            TierInfo info = tierInfo.Tiers.FirstOrDefault(i => i.Quests.Any(q => q == questType));

            if (info != null)
            {
                return info.Cooldown;
            }

            return TimeSpan.Zero;
        }

        public static int GetCompleteReq(TierQuestInfo tierInfo, Type questType)
        {
            TierInfo info = tierInfo.Tiers.FirstOrDefault(i => i.Quests.Any(q => q == questType));

            if (info != null)
            {
                return info.ToComplete;
            }

            return 0;
        }

        public static Dictionary<PlayerMobile, Dictionary<Type, int>> PlayerTierInfo { get; set; } = new Dictionary<PlayerMobile, Dictionary<Type, int>>();

        public static void CompleteQuest(PlayerMobile pm, ITierQuest quest)
        {
            Type type = quest.GetType();

            if (!PlayerTierInfo.ContainsKey(pm))
            {
                PlayerTierInfo[pm] = new Dictionary<Type, int>();
            }

            if (PlayerTierInfo[pm].ContainsKey(type))
            {
                PlayerTierInfo[pm][type]++;
            }
            else
            {
                PlayerTierInfo[pm][type] = 1;
            }
        }

        public static int HasCompleted(PlayerMobile pm, Type questType, TierQuestInfo info)
        {
            if (!PlayerTierInfo.ContainsKey(pm))
            {
                return 0;
            }

            int completed = 0;

            foreach (KeyValuePair<Type, int> kvp in PlayerTierInfo[pm])
            {
                if (questType == kvp.Key)
                {
                    completed += kvp.Value;
                }
            }
            /*foreach (var tier in info.Tiers)
            {
                if (tier.Quests.Any(q => q == questType))
                {
                    foreach (var q in tier.Quests)
                    {
                        foreach (var kvp in PlayerTierInfo[pm])
                        {
                            if (q == kvp.Key)
                            {
                                completed += kvp.Value;
                            }
                        }
                    }
                }
            }*/

            return completed;
        }

        public static BaseQuest RandomQuest(PlayerMobile pm, ITierQuester quester)
        {
            TierQuestInfo info = quester.TierInfo;

            if (info != null)
            {
                List<Type> list = new List<Type>();
                int lastTierComplete = 0;

                for (int i = 0; i < info.Tiers.Length; i++)
                {
                    TierInfo tier = info.Tiers[i];

                    if (lastTierComplete >= tier.ToComplete)
                    {
                        list.AddRange(tier.Quests);
                    }

                    lastTierComplete = 0;

                    foreach (Type quest in tier.Quests)
                    {
                        lastTierComplete += HasCompleted(pm, quest, info);
                    }
                }

                if (list.Count > 0)
                {
                    return QuestHelper.Construct(list[Utility.Random(list.Count)]);
                }
            }

            return null;
        }

        public static void Save(GenericWriter writer)
        {
            writer.Write(0);

            writer.Write(PlayerTierInfo.Count);

            foreach (KeyValuePair<PlayerMobile, Dictionary<Type, int>> kvp in PlayerTierInfo)
            {
                writer.WriteMobile(kvp.Key);
                writer.Write(kvp.Value.Count);

                foreach (KeyValuePair<Type, int> kvp2 in kvp.Value)
                {
                    writer.Write(kvp2.Key.FullName);
                    writer.Write(kvp2.Value);
                }
            }
        }

        public static void Load(GenericReader reader)
        {
            reader.ReadInt();

            int count = reader.ReadInt();

            for (int i = 0; i < count; i++)
            {
                PlayerMobile pm = reader.ReadMobile<PlayerMobile>();
                int c = reader.ReadInt();
                Dictionary<Type, int> list = new Dictionary<Type, int>();

                for (int j = 0; j < c; j++)
                {
                    Type type = ScriptCompiler.FindTypeByFullName(reader.ReadString());
                    int completed = reader.ReadInt();

                    list[type] = completed;
                }

                if (pm != null)
                {
                    PlayerTierInfo[pm] = list;
                }
            }
        }
    }

    public class TierInfo
    {
        public Type[] Quests { get; set; }
        public TimeSpan Cooldown { get; set; }
        public int ToComplete { get; set; }

        public TierInfo(int toComplete, TimeSpan cooldown, params Type[] quests)
        {
            Quests = quests;
            Cooldown = cooldown;
            ToComplete = toComplete;
        }
    }
}
