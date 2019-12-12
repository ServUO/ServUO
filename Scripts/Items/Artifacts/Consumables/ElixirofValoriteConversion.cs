using System;

namespace Server.Items
{
    public class ElixirofValoriteConversion : Item
    {
		public override int LabelNumber { get { return 1113010; } } // Elixir of Valorite Conversion
		
        [Constructable]
        public ElixirofValoriteConversion()
            : base(0x99B)
        { 
            Hue = 2219;
            Movable = true;
        }

        public ElixirofValoriteConversion(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
			
            Container backpack = from.Backpack;
            BronzeIngot item1 = (BronzeIngot)backpack.FindItemByType(typeof(BronzeIngot));   
     
            if (item1 != null)                
            { 
                BaseIngot m_Ore1 = item1 as BaseIngot;

                int toConsume = m_Ore1.Amount;

                if ((m_Ore1.Amount > 499) && (m_Ore1.Amount < 501)) 
                {
                    m_Ore1.Delete();
                    from.SendLocalizedMessage(1113048); // You've successfully converted the metal.               
                    from.AddToBackpack(new ValoriteIngot(500)); 
                    this.Delete();
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
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}