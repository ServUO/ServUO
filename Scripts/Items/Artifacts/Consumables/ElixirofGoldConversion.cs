using System;

namespace Server.Items
{
    public class ElixirofGoldConversion : Item
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ElixirofGoldConversion()
            : base(0x99B)
        {
            this.Hue = 2213;
            this.Movable = true;
        }

        public ElixirofGoldConversion(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113007;
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            Container backpack = from.Backpack;

            DullCopperIngot item1 = (DullCopperIngot)backpack.FindItemByType(typeof(DullCopperIngot));   
               
            if (item1 != null)                
            { 
                BaseIngot m_Ore1 = item1 as BaseIngot;

                int toConsume = m_Ore1.Amount;

                if ((m_Ore1.Amount > 499) && (m_Ore1.Amount < 501)) 
                {
                    m_Ore1.Delete();
                    from.SendMessage("You've successfully converted the Metal.");                    
                    from.AddToBackpack(new GoldIngot(500)); 
                    this.Delete();
                }
                else if ((m_Ore1.Amount < 500) || (m_Ore1.Amount > 500))
                {
                    from.SendMessage("You can only convert 500 Dull Copper Ingots at a time.");
                }
            }
            else
            {
                from.SendMessage("There isn't DullCopper Ingots in your Backpack.");
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