namespace Server.Items
{
    public class ElixirofMetalConversion : Item
    {
        public override int LabelNumber => 1113011;  // Elixir of Metal Conversion

        [Constructable]
        public ElixirofMetalConversion()
            : base(0x99B)
        {
            Hue = 1159;
            Movable = true;
        }

        public ElixirofMetalConversion(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {

            Container backpack = from.Backpack;
            IronIngot item1 = (IronIngot)backpack.FindItemByType(typeof(IronIngot));

            if (item1 != null)
            {
                BaseIngot m_Ore1 = item1 as BaseIngot;

                int toConsume = m_Ore1.Amount;

                if ((m_Ore1.Amount > 499) && (m_Ore1.Amount < 501))
                {
                    m_Ore1.Delete();

                    switch (Utility.Random(4))
                    {
                        case 0:
                            from.AddToBackpack(new DullCopperIngot(500));
                            break;
                        case 2:
                            from.AddToBackpack(new ShadowIronIngot(500));
                            break;
                        case 1:
                            from.AddToBackpack(new CopperIngot(500));
                            break;
                        case 3:
                            from.AddToBackpack(new BronzeIngot(500));
                            break;
                    }

                    from.SendLocalizedMessage(1113048); // You've successfully converted the metal.
                    Delete();
                }
                else if ((m_Ore1.Amount < 500) || (m_Ore1.Amount > 500))
                {
                    from.SendLocalizedMessage(1113046); // You can only convert five hundred ingots at a time.
                }
            }
            else
            {
                from.SendLocalizedMessage(1078618); // The item must be in your backpack to be exchanged.
            }
        }

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
    }
}