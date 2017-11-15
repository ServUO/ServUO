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
        public bool InUse { get { return CurrentDual != null; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public ArenaDuel CurrentDual { get; set; }

        public ArenaRegion Region { get; set; }

        public PVPArena(ArenaDefinition definition)
        {
            Definition = definition;
        }

        public void ConfigureArena()
        {
            if (Manager == null)
            {
                Manager = new ArenaManager(this);
                Manager.MoveToWorld(Definition.ManagerLocation, Definition.Map);
            }

            if (Stone == null)
            {
                Stone = new ArenaStone(this);
                Stone.MoveToWorld(Definition.StoneLocation, Defintion.Map);
            }
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

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0);

            writer.Write(Stone);
            writer.Write(Manager);
        }

        public void Deserialize(GenericReader reader)
        {
            int version = reader.ReadInt();

            Stone = reader.ReadItem() as ArenaStone;
            Manager = reader.ReadMobile() as ArenaManager;

            if (Stone != null)
            {
                Stone.Arena = this;
            }

            if (Manager != null)
            {
                Manager.Arena = this;
            }
        }
    }
}