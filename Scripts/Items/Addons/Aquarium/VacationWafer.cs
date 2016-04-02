using System;

namespace Server.Items
{
    public class VacationWafer : Item
    {
        public const int VacationDays = 7;
        [Constructable]
        public VacationWafer()
            : base(0x973)
        {
        }

        public VacationWafer(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074431;
            }
        }// An aquarium flake sphere
        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);

            list.Add(1074432, VacationDays.ToString()); // Vacation days: ~1_DAYS~
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version < 1 && this.ItemID == 0x971)
                this.ItemID = 0x973;
        }
    }
}