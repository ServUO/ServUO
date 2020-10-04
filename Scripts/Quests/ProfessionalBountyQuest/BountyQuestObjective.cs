using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Quests
{
    public class BountyQuestObjective : BaseObjective
    {
        private bool m_Captured;
        private Mobile m_CapturedCaptain;

        public bool Captured { get { return m_Captured; } set { m_Captured = value; } }
        public Mobile CapturedCaptain { get { return m_CapturedCaptain; } set { m_CapturedCaptain = value; } }

        public override bool Update(object obj)
        {
            Mobile from = (Mobile)obj;
            Mobile captain = ((ProfessionalBountyQuest)Quest).Captain;
            Mobile quester = Quest.Quester as Mobile;

            if (from == null || captain == null)
                return false;

            Container pack = from.Backpack;
            bool inRange = quester != null && quester.InRange(captain.Location, 75);

            if (m_Captured && inRange)
                return true;

            Item item = pack.FindItemByType(typeof(DeathCertificate));
            if (item != null && Quest is ProfessionalBountyQuest && ((DeathCertificate)item).Owner != null)
            {
                item.Delete();
                return true;
            }
            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(1); // version

            writer.Write(m_Captured);
            writer.Write(m_CapturedCaptain);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();

            m_Captured = reader.ReadBool();
            m_CapturedCaptain = reader.ReadMobile();

            Timer.DelayCall(TimeSpan.FromSeconds(10), ValidateCaught);
        }

        private void ValidateCaught()
        {
            if (m_CapturedCaptain != null && m_CapturedCaptain is PirateCaptain && Quest is ProfessionalBountyQuest)
                ((PirateCaptain)m_CapturedCaptain).Quest = Quest as ProfessionalBountyQuest;
        }
    }
}
