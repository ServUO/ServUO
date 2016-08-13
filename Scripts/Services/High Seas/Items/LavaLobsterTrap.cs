using Server;
using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
    public class LavaLobsterTrap : LobsterTrap
    {
        [Constructable]
        public LavaLobsterTrap()
        {
        }

        public override int[] UseableTiles { get { return Server.Engines.Harvest.Fishing.LavaTiles; } }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add(1116474); //lava lobster trap
        }

        public LavaLobsterTrap(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}