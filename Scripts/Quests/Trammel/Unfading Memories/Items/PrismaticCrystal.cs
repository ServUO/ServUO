using System;

namespace Server.Items
{
    public class PrismaticCrystal : Item
    {
        [Constructable]
        public PrismaticCrystal()
            : base(0x2DA)
        {
            this.Movable = false;
            this.Hue = 0x32;
        }

        public PrismaticCrystal(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074269;
            }
        }// prismatic crystal
        public override void OnDoubleClick(Mobile from)
        {
            if (from.Backpack == null)
                return;
		
            if (from.InRange(this.Location, 2))
            {
                if (from.Backpack.FindItemByType(typeof(PrismaticAmber), true) == null)
                {
                    if (from.PlaceInBackpack(new PrismaticAmber()))
                        this.Delete();
                    else
                        from.SendLocalizedMessage(1077971); // Make room in your backpack first!
                }
                else
                    from.SendLocalizedMessage(1075464); // You already have as many of those as you need.
            }
            else
                from.SendLocalizedMessage(1076766); // That is too far away.
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