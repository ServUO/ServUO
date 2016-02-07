using System;

namespace Server.Items
{
    public class SilverSerpentBlade : Kryss
    {
        [Constructable]
        public SilverSerpentBlade()
        {
            this.LootType = LootType.Blessed;

            this.Attributes.AttackChance = 5;
            this.Attributes.WeaponSpeed = 10;
            this.Attributes.WeaponDamage = 25;
        }

        public SilverSerpentBlade(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1078163;
            }
        }// Silver Serpent Blade
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