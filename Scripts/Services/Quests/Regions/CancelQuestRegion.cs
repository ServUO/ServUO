using Server.Mobiles;
using Server.Regions;
using System;
using System.Xml;

namespace Server.Engines.Quests
{
    public class CancelQuestRegion : BaseRegion
    {
        private readonly Type m_Quest;
        public CancelQuestRegion(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        {
            ReadType(xml["quest"], "type", ref m_Quest);
        }

        public Type Quest => m_Quest;
        public override bool OnMoveInto(Mobile m, Direction d, Point3D newLocation, Point3D oldLocation)
        {
            if (!base.OnMoveInto(m, d, newLocation, oldLocation))
                return false;

            if (m.IsStaff())
                return true;

            if (m_Quest == null)
                return true;

            PlayerMobile player = m as PlayerMobile;

            if (player != null && player.Quest != null && player.Quest.GetType() == m_Quest)
            {
                if (!player.HasGump(typeof(QuestCancelGump)))
                    player.Quest.BeginCancelQuest();

                return false;
            }

            return true;
        }
    }
}