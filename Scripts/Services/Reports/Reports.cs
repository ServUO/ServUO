using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Server.Accounting;
using Server.Commands;
using Server.Mobiles;
using Server.Network;

namespace Server.Engines.Reports
{
    public class Reports
    {
        public static bool Enabled = Config.Get("Reports.AutoGenerate", false);
        private static DateTime m_GenerateTime;
        private static SnapshotHistory m_StatsHistory;
        private static StaffHistory m_StaffHistory;
        public static StaffHistory StaffHistory
        {
            get
            {
                return m_StaffHistory;
            }
        }
        public static void Initialize()
        {
            CommandSystem.Register("GenReports", AccessLevel.Administrator, new CommandEventHandler(GenReports_OnCommand));

            m_StatsHistory = new SnapshotHistory();
            m_StatsHistory.Load();

            m_StaffHistory = new StaffHistory();
            m_StaffHistory.Load();

            DateTime now = DateTime.UtcNow;
            
            if (!Enabled)
                return;

            DateTime date = now.Date;
            TimeSpan timeOfDay = now.TimeOfDay;

            m_GenerateTime = date + TimeSpan.FromHours(Math.Ceiling(timeOfDay.TotalHours));

            Timer.DelayCall(TimeSpan.FromMinutes(0.5), TimeSpan.FromMinutes(0.5), new TimerCallback(CheckRegenerate));
        }

        public static void CheckRegenerate()
        {
            if (DateTime.UtcNow < m_GenerateTime)
                return;

            Generate();
            m_GenerateTime += TimeSpan.FromHours(1.0);
        }
        
        [Usage("GenReports")]
        [Description("Generates Reports on Command.")]
        public static void GenReports_OnCommand(CommandEventArgs e)
        {
			Generate();
        }

        public static void Generate()
        {
            Snapshot ss = new Snapshot();

            ss.TimeStamp = DateTime.UtcNow;

            FillSnapshot(ss);

            m_StatsHistory.Snapshots.Add(ss);
            m_StaffHistory.QueueStats.Add(new QueueStatus(Engines.Help.PageQueue.List.Count));

            ThreadPool.QueueUserWorkItem(new WaitCallback(UpdateOutput), ss);
        }

        public static void FillSnapshot(Snapshot ss)
        {
            ss.Children.Add(CompileGeneralStats());
            ss.Children.Add(CompileStatChart());

            PersistableObject[] obs = CompileSkillReports();

            for (int i = 0; i < obs.Length; ++i)
                ss.Children.Add(obs[i]);
        }

        public static Report CompileGeneralStats()
        {
            Report report = new Report("General Stats", "200");

            report.Columns.Add("50%", "left");
            report.Columns.Add("50%", "left");

            int npcs = 0, players = 0;

            foreach (Mobile mob in World.Mobiles.Values)
            {
                if (mob.Player)
                    ++players;
                else
                    ++npcs;
            }

            report.Items.Add("NPCs", npcs, "N0");
            report.Items.Add("Players", players, "N0");
            report.Items.Add("Clients", NetState.Instances.Count, "N0");
            report.Items.Add("Accounts", Accounts.Count, "N0");
            report.Items.Add("Items", World.Items.Count, "N0");

            return report;
        }

        public static Chart CompileStatChart()
        {
            PieChart chart = new PieChart("Stat Distribution", "graphs_strdexint_distrib", true);

            ChartItem strItem = new ChartItem("Strength", 0);
            ChartItem dexItem = new ChartItem("Dexterity", 0);
            ChartItem intItem = new ChartItem("Intelligence", 0);

            foreach (Mobile mob in World.Mobiles.Values)
            {
                if (mob.RawStatTotal == mob.StatCap && mob is PlayerMobile)
                {
                    strItem.Value += mob.RawStr;
                    dexItem.Value += mob.RawDex;
                    intItem.Value += mob.RawInt;
                }
            }

            chart.Items.Add(strItem);
            chart.Items.Add(dexItem);
            chart.Items.Add(intItem);

            return chart;
        }

        public static SkillDistribution[] GetSkillDistribution()
        {
            SkillDistribution[] distribs = new SkillDistribution[SkillInfo.Table.Length];

            for (int i = 0; i < distribs.Length; ++i)
                distribs[i] = new SkillDistribution(SkillInfo.Table[i]);

            foreach (Mobile mob in World.Mobiles.Values)
            {
                if (mob.SkillsTotal >= 1500 && mob.SkillsTotal <= (Config.Get("PlayerCaps.TotalSkillCap", 7000) + 200) && mob is PlayerMobile)
                {
                    Skills skills = mob.Skills;

                    for (int i = 0; i < skills.Length; ++i)
                    {
                        Skill skill = skills[i];

                        if (skill.BaseFixedPoint >= 1000)
                            distribs[i].m_NumberOfGMs++;
                    }
                }
            }

            return distribs;
        }

        public static PersistableObject[] CompileSkillReports()
        {
            SkillDistribution[] distribs = GetSkillDistribution();

            Array.Sort(distribs);

            return new PersistableObject[] { CompileSkillChart(distribs), CompileSkillReport(distribs) };
        }

        public static Report CompileSkillReport(SkillDistribution[] distribs)
        {
            Report report = new Report("Skill Report", "300");

            report.Columns.Add("70%", "left", "Name");
            report.Columns.Add("30%", "center", "GMs");

            for (int i = 0; i < distribs.Length; ++i)
                report.Items.Add(distribs[i].m_Skill.Name, distribs[i].m_NumberOfGMs, "N0");

            return report;
        }

        public static Chart CompileSkillChart(SkillDistribution[] distribs)
        {
            PieChart chart = new PieChart("GM Skill Distribution", "graphs_skill_distrib", true);

            for (int i = 0; i < 12; ++i)
                chart.Items.Add(distribs[i].m_Skill.Name, distribs[i].m_NumberOfGMs);

            int rem = 0;

            for (int i = 12; i < distribs.Length; ++i)
                rem += distribs[i].m_NumberOfGMs;

            chart.Items.Add("Other", rem);

            return chart;
        }

        private static void UpdateOutput(object state)
        {
            m_StatsHistory.Save();
            m_StaffHistory.Save();

            HtmlRenderer renderer = new HtmlRenderer("stats", (Snapshot)state, m_StatsHistory);
            renderer.Render();
            renderer.Upload();

            renderer = new HtmlRenderer("staff", m_StaffHistory);
            renderer.Render();
            renderer.Upload();
        }
                
        public class SkillDistribution : IComparable
        {
            public SkillInfo m_Skill;
            public int m_NumberOfGMs;
            public SkillDistribution(SkillInfo skill)
            {
                this.m_Skill = skill;
            }

            public int CompareTo(object obj)
            {
                return (((SkillDistribution)obj).m_NumberOfGMs - this.m_NumberOfGMs);
            }
        }
    }
}
