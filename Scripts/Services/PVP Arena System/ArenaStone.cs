using Server;
using System;
using Server.Items;

namespace Server.Engines.ArenaSystem
{
    public class ArenaStone : Item
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public PVPArena Arena { get; set; }

        [Constructable]
        public ArenaStone(PVPArena arena)
            : base(0xEDD)
        {
            Arena = arena;

            Movable = false;
            Hue = 1194;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(Location, 10))
            {
                BaseGump.Send(new ArenaStoneGump(from as PlayerMobile, Arena));
            }
            else
            {
                from.SendLocalizedMessage(502138); // That is too far away for you to use.
            }
        }

        public ArenaStone(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}