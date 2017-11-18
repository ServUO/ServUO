using Server;
using System;
using Server.Items;

namespace Server.Engines.ArenaSystem
{
    public class ArenaExitBanner : Item
    {
        public override bool ForceShowProperties { get { return true; } }
        public override int LabelNumber { get { return 1116111; } } // Arena Exit Banner

        [CommandProperty(AccessLevel.GameMaster)]
        public PVPArena Arena { get; set; }

        [Constructable]
        public ArenaExitBanner(int itemid, PVPArena arena)
            : base(itemid)
        {
            Arena = arena;
            Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(Location, 3))
            {
                from.SendGump(new WarningGump(1115969, C32216(0xB22222), 1115970, 0xFFFF, 348, 222, (from, okeedokee, state) =>
                {
                    var duel = Arena.CurrentDuel;

                    if (duel != null && from is PlayerMobile)
                    {
                        duel.RemovePlayer((PlayerMobile)from);
                        duel.OnPlayerLeave((PlayerMobile)from);
                    }
                }, null, true));
            }
        }

        public ArenaExitBanner(Serial serial)
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