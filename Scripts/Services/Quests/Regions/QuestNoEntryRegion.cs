using System;
using System.Xml;
using Server.Mobiles;
using Server.Regions;

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

            ReadType(questEl, "type", ref this.m_Quest);
            ReadType(questEl, "min", ref this.m_MinObjective, false);
            ReadType(questEl, "max", ref this.m_MaxObjective, false);
            ReadInt32(questEl, "message", ref this.m_Message, false);
        }

        public Type Quest
        {
            get
            {
                return this.m_Quest;
            }
        }
        public Type MinObjective
        {
            get
            {
                return this.m_MinObjective;
            }
        }
        public Type MaxObjective
        {
            get
            {
                return this.m_MaxObjective;
            }
        }
        public int Message
        {
            get
            {
                return this.m_Message;
            }
        }
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

            if (this.m_Quest == null)
                return true;

            PlayerMobile player = m as PlayerMobile;

            if (player != null && player.Quest != null && player.Quest.GetType() == this.m_Quest &&
                (this.m_MinObjective == null || player.Quest.FindObjective(this.m_MinObjective) != null) &&
                (this.m_MaxObjective == null || player.Quest.FindObjective(this.m_MaxObjective) == null))
            {
                return true;
            }
            else
            {
                if (this.m_Message != 0)
                    m.SendLocalizedMessage(this.m_Message);

                return false;
            }
        }
    }
}