using Server.Engines.Quests;
using Server.Mobiles;
using System.Collections;
using System.Xml;

namespace Server.Regions
{
    public class ApprenticeRegion : BaseRegion
    {
        private readonly Hashtable m_Table = new Hashtable();
        public ApprenticeRegion(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        {
        }

        public Hashtable Table => m_Table;
        public override void OnEnter(Mobile m)
        {
            base.OnEnter(m);

            if (m is PlayerMobile)
            {
                PlayerMobile player = (PlayerMobile)m;

                for (int i = 0; i < player.Quests.Count; i++)
                {
                    BaseQuest quest = player.Quests[i];

                    for (int j = 0; j < quest.Objectives.Count; j++)
                    {
                        BaseObjective objective = quest.Objectives[j];

                        if (objective is ApprenticeObjective && !objective.Completed)
                        {
                            ApprenticeObjective apprentice = (ApprenticeObjective)objective;

                            if (IsPartOf(apprentice.Region))
                            {
                                if (apprentice.Enter is int)
                                    player.SendLocalizedMessage((int)apprentice.Enter);
                                else if (apprentice.Enter is string)
                                    player.SendMessage((string)apprentice.Enter);

                                BuffInfo info = new BuffInfo(BuffIcon.ArcaneEmpowerment, 1078511, 1078512, apprentice.Skill.ToString()); // Accelerated Skillgain Skill: ~1_val~
                                BuffInfo.AddBuff(m, info);
                                m_Table[m] = info;
                            }
                        }
                    }
                }
            }
        }

        public override void OnExit(Mobile m)
        {
            base.OnExit(m);

            if (m is PlayerMobile)
            {
                PlayerMobile player = (PlayerMobile)m;

                for (int i = 0; i < player.Quests.Count; i++)
                {
                    BaseQuest quest = player.Quests[i];

                    for (int j = 0; j < quest.Objectives.Count; j++)
                    {
                        BaseObjective objective = quest.Objectives[j];

                        if (objective is ApprenticeObjective && !objective.Completed)
                        {
                            ApprenticeObjective apprentice = (ApprenticeObjective)objective;

                            if (IsPartOf(apprentice.Region))
                            {
                                if (apprentice.Leave is int)
                                    player.SendLocalizedMessage((int)apprentice.Leave);
                                else if (apprentice.Leave is string)
                                    player.SendMessage((string)apprentice.Leave);

                                if (m_Table[m] is BuffInfo)
                                    BuffInfo.RemoveBuff(m, (BuffInfo)m_Table[m]);
                            }
                        }
                    }
                }
            }
        }
    }
}