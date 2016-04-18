using System;
using Server.Mobiles;

namespace Server.Items
{
    public class HalloweenPumpkin : Item
    {
        private static readonly string[] m_Staff =
        {
            "Ryan", "Mark", "Eos", "Athena", "Xavier", "Krrios", "Zippy"
        };
        [Constructable]
        public HalloweenPumpkin()
            : base()
        {
            this.Weight = Utility.RandomMinMax(3, 20);
            this.ItemID = (Utility.RandomDouble() <= .02) ? Utility.RandomList(0x4694, 0x4698) : Utility.RandomList(0xc6a, 0xc6b, 0xc6c);
        }

        public HalloweenPumpkin(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this.GetWorldLocation(), 2))
                return;

            bool douse = false;

            switch (this.ItemID)
            {
                case 0x4694: 
                    this.ItemID = 0x4691; 
                    break;
                case 0x4691: 
                    this.ItemID = 0x4694; 
                    douse = true; 
                    break;
                case 0x4698: 
                    this.ItemID = 0x4695; 
                    break;
                case 0x4695: 
                    this.ItemID = 0x4698; 
                    douse = true; 
                    break;
                default: 
                    return;
            }
            from.SendLocalizedMessage(douse ? 1113988 : 1113987); // You extinguish/light the Jack-O-Lantern
            Effects.PlaySound(this.GetWorldLocation(), this.Map, douse ? 0x3be : 0x47);
        }

        private void AssignRandomName()
        {
            this.Name = String.Format("{0}'s Jack-O-Lantern", m_Staff[Utility.Random(m_Staff.Length)]);
        }

        public override bool OnDragLift(Mobile from)
        {
            if (this.Name == null && (this.ItemID == 0x4694 || this.ItemID == 0x4691 || this.ItemID == 0x4698 || this.ItemID == 0x4695))
            {
                if (Utility.RandomBool())
                {
                    new PumpkinHead().MoveToWorld(this.GetWorldLocation(), this.Map);

                    this.Delete();
                    return false;
                }
                this.AssignRandomName();
            }
            return true;
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

            if (version == 0 && this.Name == null && this.ItemID == 0x4698)
                this.AssignRandomName();
        }
    }
}