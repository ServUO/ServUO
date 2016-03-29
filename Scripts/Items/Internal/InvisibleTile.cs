using System;
using Server;
using Server.Network;

namespace Server.Items
{
    public class InvisibleTile : Item
    {
        public override string DefaultName
        {
            get { return "Invisible Tile"; }
        }

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

            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}