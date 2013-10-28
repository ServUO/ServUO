using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class Zosilem : MondainQuester
    {
        [Constructable]
        public Zosilem()
            : base("Zosilem", "the Alchemist")
        { 
        }

        public Zosilem(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(DabblingontheDarkSide)
                };
            }
        }
        public override void InitBody()
        { 
            this.HairItemID = 0x2044;//
            this.HairHue = 1153;
            this.FacialHairItemID = 0x204B;
            this.FacialHairHue = 1153;
            this.Body = 666;            
            this.Blessed = true;
        }

        public override void InitOutfit()
        { 
            this.AddItem(new Backpack());		
            this.AddItem(new Boots());
            this.AddItem(new LongPants(0x6C7));
            this.AddItem(new FancyShirt(0x6BB));
            this.AddItem(new Cloak(0x59));		
        }

        public override bool OnDragDrop(Mobile from, Item item1)
        {
            if (item1 is PotionKeg)
            {
                PotionKeg m_Pot1 = item1 as PotionKeg;

                if (m_Pot1.Type == PotionEffect.RefreshTotal)
                {
                    from.SendMessage("OHHHH YESSS !!!");
                      
                    int toConsume = m_Pot1.Amount;

                    if ((m_Pot1.Amount < 2) && (m_Pot1.Amount > 0))
                    {
                        from.SendMessage("You have converted 1 Keg of Total Refreshment Potion in a Inspected Keg of Total Refreshment");
                        m_Pot1.Delete();
                        from.AddToBackpack(new InspectedKegofTotalRefreshment()); 
                        
                        return true;
                    }
                    else
                    {
                        from.SendMessage("You can only convert 1 Keg of Total Refreshment Potion at a time !");
                    }
                }
                else if (m_Pot1.Type == PotionEffect.PoisonGreater)
                {
                    from.SendMessage("OHHHH YESSS !!!");
                      
                    int toConsume = m_Pot1.Amount;

                    if ((m_Pot1.Amount < 2) && (m_Pot1.Amount > 0))
                    {
                        from.SendMessage("You have converted 1 Keg of Greater Poison Potion in a Inspected Keg of Greater Poison");
                        m_Pot1.Delete();
                        from.AddToBackpack(new InspectedKegofGreaterPoison()); 
                        
                        return true;
                    }
                    else
                    {
                        from.SendMessage("You can only convert 1 Keg of Greater Poison Potion at a time !");
                    }
                }
            }
                
            if (item1 is GoldIngot)
            {
                BaseIngot m_Ing1 = item1 as BaseIngot;

                int toConsume = m_Ing1.Amount;

                if ((m_Ing1.Amount > 19) && (m_Ing1.Amount < 21)) 
                {
                    from.SendMessage("You have converted 20 Gold Ingot in a Pile of Inspected Gold Ingots");
                    m_Ing1.Delete();
                    from.AddToBackpack(new PileofInspectedGoldIngots()); 
                        
                    return true;
                }
                else
                {
                    from.SendMessage("You can only convert 20 Gold Ingot at a time !");
                }
            }

            if (item1 is DullCopperIngot)
            {
                BaseIngot m_Ing2 = item1 as BaseIngot;

                int toConsume = m_Ing2.Amount;

                if ((m_Ing2.Amount > 19) && (m_Ing2.Amount < 21)) 
                {
                    from.SendMessage("You have converted 20 DullCopper Ingot in a Pile of Inspected DullCopper Ingots");
                    m_Ing2.Delete();
                    from.AddToBackpack(new PileofInspectedDullCopperIngots()); 
                        
                    return true;
                }
                else
                {
                    from.SendMessage("You can only convert 20 DullCopper Ingot at a time !");
                }
            }
                 
            if (item1 is ShadowIronIngot)
            {
                BaseIngot m_Ing3 = item1 as BaseIngot;

                int toConsume = m_Ing3.Amount;

                if ((m_Ing3.Amount > 19) && (m_Ing3.Amount < 21)) 
                {
                    from.SendMessage("You have converted 20 ShadowIron Ingot in a Pile of Inspected ShadowIron Ingots");
                    m_Ing3.Delete();
                    from.AddToBackpack(new PileofInspectedShadowIronIngots()); 
                        
                    return true;
                }
                else
                {
                    from.SendMessage("You can only convert 20 ShadowIron Ingot at a time !");
                }
            }

            if (item1 is CopperIngot)
            {
                BaseIngot m_Ing4 = item1 as BaseIngot;

                int toConsume = m_Ing4.Amount;

                if ((m_Ing4.Amount > 19) && (m_Ing4.Amount < 21)) 
                {
                    from.SendMessage("You have converted 20 Copper Ingot in a Pile of Inspected Copper Ingots");
                    m_Ing4.Delete();
                    from.AddToBackpack(new PileofInspectedCopperIngots()); 
                        
                    return true;
                }
                else
                {
                    from.SendMessage("You can only convert 20 Copper Ingot at a time !");
                }
            }

            if (item1 is BronzeIngot)
            {
                BaseIngot m_Ing5 = item1 as BaseIngot;

                int toConsume = m_Ing5.Amount;

                if ((m_Ing5.Amount > 19) && (m_Ing5.Amount < 21)) 
                {
                    from.SendMessage("You have converted 20 Bronze Ingot in a Pile of Inspected Bronze Ingots");
                    m_Ing5.Delete();
                    from.AddToBackpack(new PileofInspectedBronzeIngots()); 
                        
                    return true;
                }
                else
                {
                    from.SendMessage("You can only convert 20 Bronze Ingot at a time !");
                }
            }

            if (item1 is AgapiteIngot)
            {
                BaseIngot m_Ing6 = item1 as BaseIngot;

                int toConsume = m_Ing6.Amount;

                if ((m_Ing6.Amount > 19) && (m_Ing6.Amount < 21)) 
                {
                    from.SendMessage("You have converted 20 Agapite Ingot in a Pile of Inspected Bronze Ingots");
                    m_Ing6.Delete();
                    from.AddToBackpack(new PileofInspectedAgapiteIngots()); 
                        
                    return true;
                }
                else
                {
                    from.SendMessage("You can only convert 20 Agapite Ingot at a time !");
                }
            }

            if (item1 is VeriteIngot)
            {
                BaseIngot m_Ing7 = item1 as BaseIngot;

                int toConsume = m_Ing7.Amount;

                if ((m_Ing7.Amount > 19) && (m_Ing7.Amount < 21)) 
                {
                    from.SendMessage("You have converted 20 Verite Ingot in a Pile of Inspected Verite Ingots");
                    m_Ing7.Delete();
                    from.AddToBackpack(new PileofInspectedVeriteIngots()); 
                        
                    return true;
                }
                else
                {
                    from.SendMessage("You can only convert 20 Verite Ingot at a time !");
                }
            }

            if (item1 is ValoriteIngot)
            {
                BaseIngot m_Ing8 = item1 as BaseIngot;

                int toConsume = m_Ing8.Amount;

                if ((m_Ing8.Amount > 19) && (m_Ing8.Amount < 21)) 
                {
                    from.SendMessage("You have converted 20 Valorite Ingot in a Pile of Inspected Valorite Ingots");
                    m_Ing8.Delete();
                    from.AddToBackpack(new PileofInspectedValoriteIngots()); 
                        
                    return true;
                }
                else
                {
                    from.SendMessage("You can only convert 20 Verite Ingot at a time !");
                }
            }
   
            return base.OnDragDrop(from, item1);
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