using Server;
using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.ArenaSystem
{
    public class PVPArena
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public ArenaManager Manager { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public ArenaStone Stone { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public ArenaDefinition Defintion { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool InUse { get; private set; }

        public ArenaRegion Region { get; set; }

        public PVPArena(ArenaDefinition definition)
        {
            Definition = definition;
        }

        public void Register()
        {
            Region = new ArenaRegion(this);
            Region.Register();
        }

        public void Unregister()
        {
            if (Region != null)
            {
                Region.Unregister();
                Region = null;
            }
        }
    }
}