using Server.Gumps;

namespace Server.Items
{
    public class PeerlessTeleporter : Teleporter
    {
        [Constructable]
        public PeerlessTeleporter()
            : this(null)
        {
        }

        public PeerlessTeleporter(PeerlessAltar altar)
            : base()
        {
            Altar = altar;
        }

        public PeerlessTeleporter(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public PeerlessAltar Altar { get; set; }

        public override bool OnMoveOver(Mobile m)
        {
            if (m.Alive)
            {
                m.CloseGump(typeof(ConfirmExitGump));
                m.SendGump(new ConfirmExitGump(Altar));
            }
            else if (Altar != null)
            {
                Altar.Exit(m);
                return false;
            }

            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write(Altar);

            if (Altar != null && Altar.Map != Map)
                Map = Altar.Map;
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Altar = reader.ReadItem() as PeerlessAltar;
        }
    }
}
