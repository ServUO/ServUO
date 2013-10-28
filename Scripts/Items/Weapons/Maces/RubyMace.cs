using System;

namespace Server.Items
{
    public class RubyMace : DiamondMace
    {
        [Constructable]
        public RubyMace()
        {
            this.Attributes.WeaponDamage = 5;
        }

        public RubyMace(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073529;
            }
        }// ruby mace
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