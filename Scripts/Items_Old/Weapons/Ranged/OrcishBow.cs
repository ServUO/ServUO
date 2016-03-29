using System;

namespace Server.Items
{
    [FlipableAttribute(0x13B2, 0x13B1)]
    public class OrcishBow : Bow
    {
        [Constructable]
        public OrcishBow()
        {
            this.Hue = 0x497;

            this.Attributes.WeaponDamage = 25;

            this.WeaponAttributes.DurabilityBonus = 70;

            this.Name = "an orcish bow";
        }

        public OrcishBow(Serial serial)
            : base(serial)
        {
        }

        public override int AosStrengthReq
        {
            get
            {
                return 80;
            }
        }
        public override int AosDexterityReq
        {
            get
            {
                return 80;
            }
        }
        public override int OldStrengthReq
        {
            get
            {
                return 80;
            }
        }
        public override int OldDexterityReq
        {
            get
            {
                return 80;
            }
        }
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060410, 70.ToString()); // durability ~1_val~%
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