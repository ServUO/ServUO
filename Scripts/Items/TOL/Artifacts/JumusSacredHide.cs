using System;

namespace Server.Items
{
    public class JumusSacredHide : FurCape
    {
        [Constructable]
        public JumusSacredHide()
        {
            Attributes.SpellDamage = 5;
            Attributes.CastRecovery = 1;
            AbsorptionAttributes.EaterPoison = 15;
            Attributes.WeaponDamage = 20;
        }

        public JumusSacredHide(Serial serial)
            : base(serial)
        {
        }

        public override int BaseFireResistance
        {
            get
            {
                return 5;
            }
        }

        public override int LabelNumber
        {
            get
            {
                return 1156130;
            }
        }// Jumu's Sacred Hide

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