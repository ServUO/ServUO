using Server;
using System;
using Server.Items;
using Server.Gumps;
using Server.Mobiles;

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
            if (from is PlayerMobile && from.InRange(Location, 3))
            {
                from.SendGump(new ConfirmCallbackGump((PlayerMobile)from, 1115969, 1115970, null, null, (m, state) =>
                {
                    var duel = Arena.CurrentDuel;

                    Arena.RemovePlayer((PlayerMobile)m);

                    if (duel != null && !duel.Complete)
                    {
                        duel.OnPlayerLeave((PlayerMobile)m);
                    }
                }));
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