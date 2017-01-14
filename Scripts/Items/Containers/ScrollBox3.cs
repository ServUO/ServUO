using System;

namespace Server.Items
{
    public class ScrollBox3 : WoodenBox
    {
        [Constructable]	
        public ScrollBox3()
            : base()
        {
            this.Movable = true;
            this.Hue = 1159;

            DropItem(new PowerScroll(SkillName.Imbuing, 115.0));

            if (0.05 >= Utility.RandomDouble())
            {
                double runictype = Utility.RandomDouble();
                CraftResource res;
                int charges;

                if (runictype <= .25)
                {
                    res = CraftResource.DullCopper;
                    charges = 50;
                }
                else if (runictype <= .40)
                {
                    res = CraftResource.ShadowIron;
                    charges = 45;
                }
                else if (runictype <= .55)
                {
                    res = CraftResource.Copper;
                    charges = 40;
                }
                else if (runictype <= .65)
                {
                    res = CraftResource.Bronze;
                    charges = 35;
                }
                else if (runictype <= .75)
                {
                    res = CraftResource.Gold;
                    charges = 30;
                }
                else if (runictype <= .85)
                {
                    res = CraftResource.Agapite;
                    charges = 25;
                }
                else if (runictype <= .98)
                {
                    res = CraftResource.Verite;
                    charges = 20;
                }
                else
                {
                    res = CraftResource.Valorite;
                    charges = 15;
                }

                DropItem(new RunicMalletAndChisel(res, charges));
            }
        }

        public ScrollBox3(Serial serial)
            : base(serial)
        {
        }

        public override string DefaultName
        {
            get
            {
                return "Reward Scroll Box";
            }
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