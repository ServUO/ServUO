using System;
using Server;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Engines.NewMagincia
{
	public class CommodityBrokerEntry 
	{
		private Type m_CommodityType;
        private CommodityBroker m_Broker;
        private int m_ItemID;
		private int m_Label;
		private int m_SellPricePer;
        private int m_BuyPricePer;
		private int m_BuyLimit;
		private int m_SellLimit;
		private int m_Stock;
		
		public Type CommodityType { get { return m_CommodityType; } }
        public CommodityBroker Broker { get { return m_Broker; } }
        public int ItemID { get { return m_ItemID; } }
		public int Label { get { return m_Label; } }
		public int SellPricePer { get { return m_SellPricePer; } set { m_SellPricePer = value; } }
		public int BuyPricePer { get { return m_BuyPricePer; } set { m_BuyPricePer = value; } }
		public int BuyLimit { get { return m_BuyLimit; } set { m_BuyLimit = value; } }
		public int SellLimit { get { return m_SellLimit; } set { m_SellLimit = value; } }
		public int Stock { get { return m_Stock; } set { m_Stock = value; } }

        public int ActualSellLimit
        {
            get 
            {
                if (m_Stock < m_SellLimit)
                    return m_Stock;

                return m_SellLimit;
            }
        }

        public int ActualBuyLimit
        {
            get 
            {
                if(m_Broker != null && m_Broker.BankBalance < m_BuyLimit * m_BuyPricePer)
                    return m_Broker.BankBalance / m_BuyPricePer;

                int limit = m_BuyLimit - m_Stock;

                if (limit <= 0)
                    return 0;

                return limit;
            }
        }
		
		/*	SellPricePer - price per unit the broker is selling to players for
		 *	BuyPricePer - price per unit the borker is buying from players for
		 *	BuyAtLimit - Limit a commodity must go below before it will buy that particular commodity
         *	
		 *	SellAtLimit - Limit a commodty must go above before it will sell that particular commodity
		 *	BuyLimit - Limit (in units) it will buy from players
		 *	SellLimit - Limit (in units) it will sell to players
		 */
		public CommodityBrokerEntry (Item item, CommodityBroker broker, int amount)
		{
			m_CommodityType = item.GetType();
            m_ItemID = item.ItemID;
            m_Broker = broker;
            m_Stock = amount;
			
			if(item is ICommodity)
				m_Label = ((ICommodity)item).DescriptionNumber;
			else
				m_Label = item.LabelNumber;
		}
		
        /// <summary>
        /// Player buys, the vendor is selling
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
		public bool PlayerCanBuy(int amount)
		{
			//return (m_SellAtLimit == 0 || m_Stock >= m_SellAtLimit) && m_Stock > 0 && m_SellPricePer > 0 && m_SellLimit < amount;
            return (m_SellLimit == 0 || amount <= ActualSellLimit) && m_SellPricePer > 0;
        }
		
        /// <summary>
        /// Player sells, the vendor is buying
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
		public bool PlayerCanSell(int amount)
		{
			//(m_BuyAtLimit == 0 || m_Stock <= entry.BuyAtLimit) && m_Stock > 0 && m_BuyPricePer * amount <= m_Broker.BankBalance && m_BuyPricePer > 0 && m_BuyLimit < amount;
		    return (m_BuyLimit == 0 || amount <= ActualBuyLimit) && m_Stock > 0 && m_BuyPricePer > 0 && m_BuyPricePer <= m_Broker.BankBalance;
        }
		
		public CommodityBrokerEntry(GenericReader reader)
		{
			int version = reader.ReadInt();
			
			m_CommodityType = ScriptCompiler.FindTypeByName(reader.ReadString());
            m_Label = reader.ReadInt();
            m_Broker = reader.ReadMobile() as CommodityBroker;
            m_ItemID = reader.ReadInt();
			m_SellPricePer = reader.ReadInt();
			m_BuyPricePer = reader.ReadInt();
			m_BuyLimit = reader.ReadInt();
			m_SellLimit = reader.ReadInt();
			m_Stock = reader.ReadInt();
		}
		
		public void Serialize(GenericWriter writer)
		{
			writer.Write((int)0);

            writer.Write(m_CommodityType.Name);
            writer.Write(m_Label);
            writer.Write(m_Broker);
            writer.Write(m_ItemID);
			writer.Write(m_SellPricePer);
			writer.Write(m_BuyPricePer);
			writer.Write(m_BuyLimit);
			writer.Write(m_SellLimit);
			writer.Write(m_Stock);
		}
	}
	
	public class PetBrokerEntry
	{
		private BaseCreature m_Pet;
		private int m_SalePrice;
		private string m_TypeName;
		
		public BaseCreature Pet { get { return m_Pet; } }
		public int SalePrice { get { return m_SalePrice; } set { m_SalePrice = value; } }
		public string TypeName { get { return m_TypeName; } set { m_TypeName = value; } }
		
		private static Dictionary<Type, string> m_NameBuffer = new Dictionary<Type, string>();
		public static Dictionary<Type, string> NameBuffer { get { return m_NameBuffer; } }
		
		public static readonly int DefaultPrice = 1000;

        public PetBrokerEntry(BaseCreature pet) : this(pet, DefaultPrice)
        {
        }

		public PetBrokerEntry(BaseCreature pet, int price)
		{
			m_Pet = pet;
			m_SalePrice = price;
			m_TypeName = GetOriginalName(pet);
		}
		
		public static string GetOriginalName(BaseCreature bc)
		{
			if(bc == null)
				return null;
				
			Type t = bc.GetType();
			
			if(m_NameBuffer.ContainsKey(t))
				return m_NameBuffer[t];
			
			BaseCreature c = Activator.CreateInstance(t) as BaseCreature;

			if(c != null)
			{
				c.Delete();
				AddToBuffer(t, c.Name);
				return c.Name;
			}
			
			return t.Name;
		}
		
		public static void AddToBuffer(Type type, string s)
		{
			if(s != null && s.Length > 0 && !m_NameBuffer.ContainsKey(type))
				m_NameBuffer[type] = s;
		}

        public void Internalize()
        {
            if (m_Pet.Map == Map.Internal)
                return;

            m_Pet.ControlTarget = null;
            m_Pet.ControlOrder = OrderType.Stay;
            m_Pet.Internalize();

            m_Pet.SetControlMaster(null);
            m_Pet.SummonMaster = null;

            m_Pet.IsStabled = true;
            m_Pet.Loyalty = BaseCreature.MaxLoyalty;

            m_Pet.Home = Point3D.Zero;
            m_Pet.RangeHome = 10;
            m_Pet.Blessed = false;
        }
		
		public PetBrokerEntry(GenericReader reader)
		{
			int version = reader.ReadInt();
			
			m_Pet = reader.ReadMobile() as BaseCreature;
			m_SalePrice = reader.ReadInt();
			m_TypeName = reader.ReadString();

            if (m_Pet != null)
            {
                AddToBuffer(m_Pet.GetType(), m_TypeName);

                m_Pet.IsStabled = true;

                Timer.DelayCall(TimeSpan.FromSeconds(10), new TimerCallback(Internalize));
            }
		}
		
		public void Serialize(GenericWriter writer)
		{
			writer.Write((int)0);
		
			writer.Write(m_Pet);
			writer.Write(m_SalePrice);
			writer.Write(m_TypeName);
		}
	}
}