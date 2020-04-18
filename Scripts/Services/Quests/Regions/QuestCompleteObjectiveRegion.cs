using Server.Mobiles;
using Server.Regions;
using System;
using System.Xml;

namespace Server.Engines.Quests
{
    public class QuestCompleteObjectiveRegion : BaseRegion
    {
        private readonly Type m_Quest;
        private readonly Type m_Objective;
        public QuestCompleteObjectiveRegion(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        {
            XmlElement questEl = xml["quest"];

            ReadType(questEl, "type", ref m_Quest);
            ReadType(questEl, "complete", ref m_Objective);
        }

        public Type Quest => m_Quest;
        public Type Objective => m_Objective;
        public override void OnEnter(Mobile m)
        {
            base.OnEnter(m);

            if (m_Quest != null && m_Objective != null)
            {
                PlayerMobile player = m as PlayerMobile;

                if (player != null && player.Quest != null && player.Quest.GetType() == m_Quest)
                {
                    QuestObjective obj = player.Quest.FindObjective(m_Objective);

                    if (obj != null && !obj.Completed)
                        obj.Complete();
                }
            }
        }
    }
}