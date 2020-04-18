using Server.Network;

namespace Server.Items
{
    public class InvisibleTile : Item
    {
        public override string DefaultName => "Invisible Tile";

        [Constructable]
        public InvisibleTile()
            : base(0x2198)
        {
            Movable = false;
        }

        public InvisibleTile(Serial serial)
            : base(serial)
        {
        }

        protected override Packet GetWorldPacketFor(NetState state)
        {
            Mobile mob = state.Mobile;

            if (mob != null && mob.AccessLevel >= AccessLevel.GameMaster)
            {
                return new LOSBlocker.GMItemPacket(this);
            }

            return base.GetWorldPacketFor(state);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class RevealTile : Item
    {
        [Constructable]
        public RevealTile()
            : base(0x2e46)
        {
            Movable = false;
        }

        public override int GetUpdateRange(Mobile m)
        {
            if (m.Player)
                return 1;

            return base.GetUpdateRange(m);
        }

        public RevealTile(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}