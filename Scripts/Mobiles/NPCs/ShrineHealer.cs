using Server.ContextMenus;
using Server.Gumps;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public abstract class ShrineHealer : BaseHealer
    {
        public ShrineHealer()
        {
        }

        public override bool CheckResurrect(Mobile m)
        {
            return false;
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (from is PlayerMobile)
            {
                list.Add(new LockKarmaEntry((PlayerMobile)from));
                list.Add(new ResurrectEntry(from, this));
                list.Add(new TitheEntry(from));
            }
        }

        private class ResurrectEntry : ContextMenuEntry
        {
            private readonly Mobile m_Mobile;
            private readonly ShrineHealer m_Vendor;

            public ResurrectEntry(Mobile mobile, ShrineHealer vendor)
                : base(6195, 2)
            {
                m_Mobile = mobile;
                m_Vendor = vendor;

                Enabled = !m_Mobile.Alive;
            }

            public override void OnClick()
            {
                m_Vendor.OfferResurrection(m_Mobile);
            }
        }

        private class LockKarmaEntry : ContextMenuEntry
        {
            private readonly PlayerMobile m_Mobile;

            public LockKarmaEntry(PlayerMobile mobile)
                : base(mobile.KarmaLocked ? 6197 : 6196, 2)
            {
                m_Mobile = mobile;
            }

            public override void OnClick()
            {
                m_Mobile.KarmaLocked = !m_Mobile.KarmaLocked;

                if (m_Mobile.KarmaLocked)
                    m_Mobile.SendLocalizedMessage(1060192); // Your karma has been locked. Your karma can no longer be raised.
                else
                    m_Mobile.SendLocalizedMessage(1060191); // Your karma has been unlocked. Your karma can be raised again.
            }
        }

        private class TitheEntry : ContextMenuEntry
        {
            private readonly Mobile m_Mobile;

            public TitheEntry(Mobile mobile)
                : base(6198, 2)
            {
                m_Mobile = mobile;

                Enabled = m_Mobile.Alive;
            }

            public override void OnClick()
            {
                if (m_Mobile.CheckAlive())
                    m_Mobile.SendGump(new TithingGump(m_Mobile, 0));
            }
        }

        public ShrineHealer(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}