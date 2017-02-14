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
            this.AbsorptionAttributes.EaterDamage = 6;
            this.Attributes.Luck = 200;
            this.Resistances.Physical = 20;
            this.Resistances.Fire = 20;
            this.Resistances.Cold = 20;
            this.Resistances.Poison = 20;
            this.Resistances.Energy = 20;
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