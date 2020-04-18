using Server.Mobiles;
using Server.Regions;
using System;
using System.Xml;

namespace Server.Engines.Quests
{
    public class QuestNoEntryRegion : BaseRegion
    {
        private readonly Type m_Quest;
        private readonly Type m_MinObjective;
        private readonly Type m_MaxObjective;
        private readonly int m_Message;
        public QuestNoEntryRegion(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        {
            XmlElement questEl = xml["quest"];

            ReadType(questEl, "type", ref m_Quest);
            ReadType(questEl, "min", ref m_MinObjective, false);
            ReadType(questEl, "max", ref m_MaxObjective, false);
            ReadInt32(questEl, "message", ref m_Message, false);
        }

        public Type Quest => m_Quest;
        public Type MinObjective => m_MinObjective;
        public Type MaxObjective => m_MaxObjective;
        public int Message => m_Message;
        public override bool OnMoveInto(Mobile m, Direction d, Point3D newLocation, Point3D oldLocation)
        {
            if (!base.OnMoveInto(m, d, newLocation, oldLocation))
                return false;

            if (m.IsStaff())
                return true;

            if (m is BaseCreature)
            {
                BaseCreature bc = m as BaseCreature;

                if (!bc.Controlled && !bc.Summoned)
                    return true;
            }

            if (m_Quest == null)
                return true;

            PlayerMobile player = m as PlayerMobile;

            if (player != null && player.Quest != null && player.Quest.GetType() == m_Quest &&
                (m_MinObjective == null || player.Quest.FindObjective(m_MinObjective) != null) &&
                (m_MaxObjective == null || player.Quest.FindObjective(m_MaxObjective) == null))
            {
                return true;
            }
            else
            {
                if (m_Message != 0)
                    m.SendLocalizedMessage(m_Message);

                return false;
            }
        }
    }
}