using System;

namespace Server.Items
{
    public class BraceletOfPrimalConsumption : GoldBracelet
	{
        public override int LabelNumber { get { return 1157350; } } // bracelet of primal consumption
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public BraceletOfPrimalConsumption()
        {
            AbsorptionAttributes.EaterDamage = 6;
            Attributes.Luck = 200;
            Resistances.Physical = 20;
            Resistances.Fire = 20;
            Resistances.Cold = 20;
            Resistances.Poison = 20;
            Resistances.Energy = 20;
        }

        public BraceletOfPrimalConsumption(Serial serial)
            : base(serial)
        {
        }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

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