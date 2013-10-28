using System;

namespace Server.Items
{
    public class EmeraldMace : DiamondMace
    {
        [Constructable]
        public EmeraldMace()
        {
            this.WeaponAttributes.ResistPoisonBonus = 5;
        }

        public EmeraldMace(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073530;
            }
        }// emerald mace
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