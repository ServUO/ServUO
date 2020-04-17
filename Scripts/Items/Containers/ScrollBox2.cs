namespace Server.Items
{
    public class ScrollBox2 : WoodenBox
    {
        [Constructable]
        public ScrollBox2()
            : base()
        {
            Movable = true;
            Hue = 1266;

            DropItem(new PowerScroll(SkillName.Imbuing, 120.0));

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

        public ScrollBox2(Serial serial)
            : base(serial)
        {
        }

        public override string DefaultName => "Reward Scroll Box";
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        private static void PlaceItemIn(Container parent, int x, int y, Item item)
        {
            parent.AddItem(item);
            item.Location = new Point3D(x, y, 0);
        }
    }
}