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

            if (m is PlayerMobile player)
            {
                for (int i = 0; i < player.Quests.Count; i++)
                {
                    BaseQuest quest = player.Quests[i];

                    for (int j = 0; j < quest.Objectives.Count; j++)
                    {
                        BaseObjective objective = quest.Objectives[j];

                        if (objective is ApprenticeObjective apprentice && !apprentice.Completed && IsPartOf(apprentice.Region))
                        {
                            if (apprentice.Enter is int iEnter)
                            {
                                player.SendLocalizedMessage(iEnter);
                            }
                            else if (apprentice.Enter is string sEnter)
                            {
                                player.SendMessage(sEnter);
                            }

                            BuffInfo info = new BuffInfo(BuffIcon.ArcaneEmpowerment, 1078511, 1078512,
                                apprentice.Skill.ToString()); // Accelerated Skillgain Skill: ~1_val~
                            BuffInfo.AddBuff(player, info);
                            m_Table[m] = info;
                        }
                    }
                }
            }
        }

        public override void OnExit(Mobile m)
        {
            base.OnExit(m);

            if (m is PlayerMobile player)
            {
                for (int i = 0; i < player.Quests.Count; i++)
                {
                    BaseQuest quest = player.Quests[i];

                    for (int j = 0; j < quest.Objectives.Count; j++)
                    {
                        BaseObjective objective = quest.Objectives[j];

                        if (objective is ApprenticeObjective apprentice && !apprentice.Completed && IsPartOf(apprentice.Region))
                        {
                            if (apprentice.Leave is int iLeave)
                            {
                                player.SendLocalizedMessage(iLeave);
                            }
                            else if (apprentice.Leave is string sLeave)
                            {
                                player.SendMessage(sLeave);
                            }

                            if (m_Table[m] is BuffInfo)
                            {
                                BuffInfo.RemoveBuff(player, (BuffInfo) m_Table[m]);
                            }
                        }
                    }
                }
            }
        }
    }
}
