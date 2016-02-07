using System;

namespace Server.Items
{
    public class SanctuaryTele : Teleporter
    { 
        [Constructable]
        public SanctuaryTele()
            : base(new Point3D(6172, 22, 0), Map.Trammel)
        {
        }

        public SanctuaryTele(Serial serial)
            : base(serial)
        {
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (m.NetState == null || !m.NetState.SupportsExpansion(Expansion.ML))
            {
                m.SendLocalizedMessage(1072608); // You must upgrade to the Mondain's Legacy expansion in order to enter here.				
                return true;
            }
            else if (!MondainsLegacy.Sanctuary && (int)m.AccessLevel < (int)AccessLevel.GameMaster)
            {
                m.SendLocalizedMessage(1042753, "Sanctuary"); // ~1_SOMETHING~ has been temporarily disabled.
                return true;
            }
			
            return base.OnMoveOver(m);
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