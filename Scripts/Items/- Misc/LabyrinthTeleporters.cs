using System;
using Server.Mobiles;

namespace Server.Items
{
    public class LabyrinthIslandTele : Item
    { 
        [Constructable]
        public LabyrinthIslandTele()
            : base(0x2FD4)
        { 
            this.Movable = false;
        }

        public LabyrinthIslandTele(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        { 
            if (MondainsLegacy.Labyrinth && from.InRange(this.Location, 2))
                from.MoveToWorld(new Point3D(1731, 978, -80), this.Map);
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

    public class LabyrinthTele : Item
    { 
        [Constructable]
        public LabyrinthTele()
            : base(0x248B)
        { 
            this.Movable = false;
        }

        public LabyrinthTele(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        { 
            if (from.NetState == null || !from.NetState.SupportsExpansion(Expansion.ML))
            {
                from.SendLocalizedMessage(1072608); // You must upgrade to the Mondain's Legacy expansion in order to enter here.				
                return;
            }
            else if (!MondainsLegacy.Labyrinth && (int)from.AccessLevel < (int)AccessLevel.GameMaster)
            {
                from.SendLocalizedMessage(1042753, "Labyrinth"); // ~1_SOMETHING~ has been temporarily disabled.
                return;
            }
		
            if (from.InRange(this.Location, 2))
            {
                Point3D p = new Point3D(330, 1973, 0);
				
                BaseCreature.TeleportPets(from, p, this.Map);
                from.MoveToWorld(p, this.Map);
            }
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