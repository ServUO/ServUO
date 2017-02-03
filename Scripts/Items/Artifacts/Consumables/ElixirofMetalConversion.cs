using System;

namespace Server.Items
{
    public class ElixirofMetalConversion : Item
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ElixirofMetalConversion()
            : base(0x99B)
        {
            this.Hue = 1159;
            this.Movable = true;
        }

        public ElixirofMetalConversion(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113011;
            }
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

                    switch ( Utility.Random(4) )
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

                    from.SendMessage("You've successfully converted the Metal.");    
                    this.Delete();
                }
                else if ((m_Ore1.Amount < 500) || (m_Ore1.Amount > 500))
                {
                    from.SendMessage("You can only convert 500 Iron Ingots at a time.");
                }
            }
            else
            {
                from.SendMessage("There isn't Iron Ingots in your Backpack.");
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