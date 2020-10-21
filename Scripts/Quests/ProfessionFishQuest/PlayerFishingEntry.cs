using Server.Commands;
using System;
using System.Collections.Generic;

namespace Server.Engines.Quests
{
    public class PlayerFishingEntry
    {
        public static void Initialize()
        {
            CommandSystem.Register("FishMongerStatus", AccessLevel.Player, FishMongerStatus_OnCommand);
        }

        private readonly Mobile m_Player;
        private double m_Reputation;
        private bool m_HasRecievedBritGal;
        private readonly Dictionary<int, int> m_HaveFished = new Dictionary<int, int>();
        private readonly Dictionary<int, int> m_TimesFished = new Dictionary<int, int>();

        public Mobile Player => m_Player;
        public double Reputation { get { return m_Reputation; } set { m_Reputation = value; } }
        public bool HasRecievedBritGal { get { return m_HasRecievedBritGal; } set { m_HasRecievedBritGal = value; } }
        public Dictionary<int, int> HaveFished => m_HaveFished;
        public Dictionary<int, int> TimesFished => m_TimesFished;

        private static readonly Dictionary<Mobile, PlayerFishingEntry> m_FishingEntries = new Dictionary<Mobile, PlayerFishingEntry>();
        public static Dictionary<Mobile, PlayerFishingEntry> FishingEntries => m_FishingEntries;

        public static readonly double RewardAmount = 15000;

        public PlayerFishingEntry(Mobile from)
        {
            m_Player = from;
            m_Reputation = 0.0;

            m_FishingEntries.Add(from, this);

            m_HasRecievedBritGal = false;
        }

        public static PlayerFishingEntry GetEntry(Mobile from)
        {
            return GetEntry(from, false);
        }

        public static PlayerFishingEntry GetEntry(Mobile from, bool create)
        {
            if (m_FishingEntries.ContainsKey(from))
                return m_FishingEntries[from];

            if (create)
            {
                PlayerFishingEntry entry = new PlayerFishingEntry(from);
                return entry;
            }

            return null;
        }

        public double GetPointsAwarded(FishQuestObjective obj)
        {
            double toAward = 0.0;

            foreach (KeyValuePair<Type, int[]> kvp in obj.Line)
            {
                Type type = kvp.Key;
                int value = kvp.Value[1];

                if (FishQuestHelper.IsShallowWaterFish(type))
                    toAward += value;
                else if (FishQuestHelper.IsCrustacean(type) || FishQuestHelper.IsDeepWaterFish(type))
                    toAward += value * 2;
                else if (FishQuestHelper.IsDungeonFish(type))
                    toAward += value * 3;

                m_Reputation += toAward;
            }

            return toAward;
        }

        public void OnAfterReward(double points)
        {
            if (m_Player == null || m_Player.NetState == null)
                return;

            if (points <= 100)
                m_Player.SendMessage("You have gained a fair amount of reputation with the Fish Mongers.");
            else if (points <= 150)
                m_Player.SendMessage("You have gained a good amount of reputation with the Fish Mongers.");
            else
                m_Player.SendMessage("You have gained a large amount of reputation with the Fish Mongers.");
        }

        public int CalculateLines()
        {
            int eligibleIndex = FishQuestHelper.GetIndexForSkillLevel(m_Player);
            double line2 = 0.0; double line3 = 0.0;
            double line4 = 0.0; double line5 = 0.0; double line6 = 0.0;

            for (int i = 0; i < eligibleIndex; i++)
            {
                if (!m_TimesFished.ContainsKey(i))
                    continue;

                int timesDone = Math.Min(6, m_TimesFished[i]);
                int toAdd = 100 / eligibleIndex;

                switch (timesDone)
                {
                    case 6: goto case 5;
                    case 5: line6 += toAdd; goto case 4;
                    case 4: line5 += toAdd; goto case 3;
                    case 3: line4 += toAdd; goto case 2;
                    case 2: line3 += toAdd; goto case 1;
                    case 1: line2 += toAdd; goto case 0;
                    case 0:
                    default: break;
                }
            }

            if (Math.Min(85, line6) >= Utility.Random(100))
                return 6;
            if (Math.Min(85, line5) >= Utility.Random(100))
                return 5;
            if (Math.Min(85, line4) >= Utility.Random(100))
                return 4;
            if (Math.Min(85, line3) >= Utility.Random(100))
                return 3;
            if (Math.Min(85, line2) >= Utility.Random(100))
                return 2;
            return 1;
        }

        public void GetRandomFish(ref int index, ref int amount, List<int> chosen)
        {
            int eligibleIndex = FishQuestHelper.GetIndexForSkillLevel(m_Player);

            while (true)
            {
                index = Utility.Random(eligibleIndex);

                if (!chosen.Contains(index))
                    break;
            }

            if (m_HaveFished.ContainsKey(index))
                amount = m_HaveFished[index];
            else
                amount = 10;
        }

        public void OnQuestResign(Type type)
        {
            int index = FishQuestHelper.GetIndexForType(type);

            if (index < 0)
                return;

            m_Reputation -= 20;

            if (m_HaveFished.ContainsKey(index))
            {
                if (m_HaveFished[index] > 10)
                    m_HaveFished[index] -= 5;
                else
                    m_HaveFished[index] = 5;
            }
            else
                m_HaveFished.Add(index, 10);

        }

        public void OnQuestComplete(FishQuestObjective obj)
        {
            foreach (KeyValuePair<Type, int[]> kvp in obj.Line)
            {
                Type type = kvp.Key;

                int index = FishQuestHelper.GetIndexForType(type);

                if (m_HaveFished.ContainsKey(index))
                {
                    if (m_HaveFished[index] >= 20)
                        m_HaveFished[index] = 10;
                    else
                        m_HaveFished[index] += 5;
                }
                else
                    m_HaveFished.Add(index, 15);

                if (!m_TimesFished.ContainsKey(index))
                    m_TimesFished[index] = 0;

                m_TimesFished[index]++;
            }
        }

        public PlayerFishingEntry(GenericReader reader)
        {
            int version = reader.ReadInt();

            if (version > 0)
                m_HasRecievedBritGal = reader.ReadBool();
            else
                m_HasRecievedBritGal = false;

            m_Player = reader.ReadMobile();
            m_Reputation = reader.ReadDouble();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                int index = reader.ReadInt();
                int finished = reader.ReadInt();

                if (index >= 0 && index < FishQuestHelper.Fish.Length)
                    m_HaveFished.Add(index, finished);
            }

            count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                int index = reader.ReadInt();
                int fished = reader.ReadInt();

                if (index >= 0 && index < FishQuestHelper.Fish.Length)
                    m_TimesFished.Add(index, fished);
            }

            if (m_Player != null)
                m_FishingEntries.Add(m_Player, this);
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(1);

            writer.Write(m_HasRecievedBritGal);

            writer.Write(m_Player);
            writer.Write(m_Reputation);

            writer.Write(m_HaveFished.Count);
            foreach (KeyValuePair<int, int> kvp in m_HaveFished)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value);
            }

            writer.Write(m_TimesFished.Count);
            foreach (KeyValuePair<int, int> kvp in m_TimesFished)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value);
            }
        }

        public void GetFishMongerReputation(Mobile from)
        {
            if (from == null)
                return;

            from.SendMessage("You have earned {0} amount of reputation with the Fish Mongers.", GetStatus());
        }

        private string GetStatus()
        {
            double points = m_Reputation;

            if (points < 500)
                return m_Status[0];
            if (points < 1000)
                return m_Status[1];
            if (points < 5000)
                return m_Status[2];
            if (points < 15000)
                return m_Status[3];
            if (points < 50000)
                return m_Status[4];
            return m_Status[5];
        }

        private readonly string[] m_Status =
        {
            "a small",
            "a fair",
            "a moderate",
            "a tremendous",
            "an outstanding",
            "an enormous",
        };

        public static void FishMongerStatus_OnCommand(CommandEventArgs e)
        {
            PlayerFishingEntry entry = GetEntry(e.Mobile);

            if (entry != null)
                entry.GetFishMongerReputation(e.Mobile);
            else
                e.Mobile.SendMessage("You have no reputation with the Fish Mongers.");
        }
    }
}
