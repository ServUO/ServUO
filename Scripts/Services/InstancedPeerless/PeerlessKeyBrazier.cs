using Server.Items;
using Server.Targeting;

namespace Server.Engines.InstancedPeerless
{
    public class PeerlessKeyBrazier : AddonComponent
    {
        public override int LabelNumber => 1024622; // altar

        private PeerlessPlatform m_Platform;
        private Item m_Key;

        [CommandProperty(AccessLevel.GameMaster)]
        public PeerlessPlatform Platform => m_Platform;

        [CommandProperty(AccessLevel.GameMaster)]
        public Item Key { get { return m_Key; } set { m_Key = value; } }

        public PeerlessKeyBrazier(PeerlessPlatform platform)
            : base(0x19BB)
        {
            Hue = 0x15F;
            Light = LightType.Circle300;

            m_Platform = platform;
        }

        public PeerlessKeyBrazier(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.Map != Map || !from.InRange(Location, 2))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
            }
            else if (m_Platform.Summoner != null && m_Platform.Summoner != from)
            {
                SendLocalizedMessageTo(from, 502221); // Someone else is already using this item.
            }
            else if (m_Key != null && !m_Key.Deleted)
            {
                SendLocalizedMessageTo(from, 1112701); // I've already accepted your offering.
            }
            else
            {
                from.SendAsciiMessage("What would you like to sacrifice?");
                from.BeginTarget(10, false, TargetFlags.None, Sacrifice_Callback);
            }
        }

        public void Sacrifice_Callback(Mobile from, object state)
        {
            Item targeted = state as Item;

            if (targeted == null || targeted.Deleted)
                return;

            if (from.Map != Map || !from.InRange(Location, 2))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
            }
            else if (m_Key != null && !m_Key.Deleted)
            {
                SendLocalizedMessageTo(from, 502221); // Someone else is already using this item.
            }
            else if (!targeted.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1049486); // You can only sacrifice items that are in your backpack!
            }
            else if (targeted.GetType() != m_Platform.KeyType)
            {
                from.SendAsciiMessage("I do not want that!");
                Effects.SendBoltEffect(from);
            }
            else
            {
                targeted.Movable = false;
                targeted.MoveToWorld(new Point3D(X, Y, Z + 1), Map);

                Effects.SendLocationEffect(targeted.Location, targeted.Map, 0x3728, 10);
                Effects.PlaySound(targeted.Location, targeted.Map, 0x29);

                m_Key = targeted;

                m_Platform.Summoner = from;
                m_Platform.Validate();
            }
        }

        public override void OnAfterDelete()
        {
            if (m_Key != null)
                m_Key.Delete();

            base.OnAfterDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write(m_Platform);
            writer.Write(m_Key);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_Platform = reader.ReadItem() as PeerlessPlatform;
                        m_Key = reader.ReadItem();

                        if (m_Platform == null)
                            Delete();

                        if (m_Key != null)
                        {
                            m_Key.Delete();
                            m_Key = null;
                        }

                        break;
                    }
            }
        }
    }
}
