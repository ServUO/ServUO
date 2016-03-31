using System;

namespace Server.Items
{
    public class OrcishMachete : ElvenMachete
    {
        [Constructable]
        public OrcishMachete()
        {
            this.Attributes.BonusInt = -5;
            this.Attributes.WeaponDamage = 10;
        }

        public OrcishMachete(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073534;
            }
        }// Orcish Machete
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}