using System;

namespace Server.Items
{
    public class ArcanistsWildStaff : WildStaff
    {
        [Constructable]
        public ArcanistsWildStaff()
        {
            this.Attributes.BonusMana = 3;
            this.Attributes.WeaponDamage = 3;
        }

        public ArcanistsWildStaff(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073549;
            }
        }// arcanist's wild staff
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