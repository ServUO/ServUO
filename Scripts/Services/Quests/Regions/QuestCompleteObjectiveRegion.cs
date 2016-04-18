using System;
using System.Xml;
using Server.Mobiles;
using Server.Regions;

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

            ReadType(questEl, "type", ref this.m_Quest);
            ReadType(questEl, "complete", ref this.m_Objective);
        }

        public Type Quest
        {
            get
            {
                return this.m_Quest ;
            }
        }
        public Type Objective
        {
            get
            {
                return this.m_Objective;
            }
        }
        public override void OnEnter(Mobile m)
        {
            base.OnEnter(m);

            if (this.m_Quest != null && this.m_Objective != null)
            {
                PlayerMobile player = m as PlayerMobile;

                if (player != null && player.Quest != null && player.Quest.GetType() == this.m_Quest)
                {
                    QuestObjective obj = player.Quest.FindObjective(this.m_Objective);

                    if (obj != null && !obj.Completed)
                        obj.Complete();
                }
            }
        }
    }
}