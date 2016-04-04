using System;

namespace Server.Items
{
    public class ElixirofValoriteConversion : Item
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ElixirofValoriteConversion()
            : base(0x99B)
        {
            this.Name = "Elixir of Valorite Conversion";  

            this.Hue = 2219;
            this.Movable = true;
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
                    from.SendMessage("You've successfully converted the Metal.");                    
                    from.AddToBackpack(new ValoriteIngot(500)); 
                    this.Delete();
                }
                else if ((m_Ore1.Amount < 500) || (m_Ore1.Amount > 500))
                {
                    from.SendMessage("You can only convert 500 Bronze Ingots at a time.");
                }
            }
            else
            {
                from.SendMessage("There isn't Bronze Ingots in your Backpack.");
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