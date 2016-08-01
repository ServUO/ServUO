using Server;
using System;
using Server.Items;
using Server.Mobiles;
using Server.Accounting;

namespace Server.Engines.NewMagincia
{
	[PropertyObject]
	public class MaginciaBazaarPlot
	{
		private PlotDef m_Definition;
		private Mobile m_Owner;
		private string m_ShopName;
		private BaseBazaarMulti m_PlotMulti;
		private BaseBazaarBroker m_Merchant;
		private PlotSign m_Sign;
		private MaginciaPlotAuction m_Auction;
		
		[CommandProperty(AccessLevel.GameMaster)]
		public PlotDef PlotDef { get { return m_Definition; } set { } }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public Mobile Owner { get { return m_Owner; } set { m_Owner = value; } }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public string ShopName { get { return m_ShopName; } set { m_ShopName = value; } }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public BaseBazaarMulti PlotMulti 
		{ 
			get { return m_PlotMulti; } 
			set 
			{
                if (m_PlotMulti != null && m_PlotMulti != value && value != null)
                {
                    m_PlotMulti.Delete();
                    m_PlotMulti = null;
                }
					
				m_PlotMulti = value;
				
				if(m_PlotMulti != null)
					m_PlotMulti.MoveToWorld(m_Definition.MultiLocation, m_Definition.Map);
			} 
		}
		
		[CommandProperty(AccessLevel.GameMaster)]
        public BaseBazaarBroker Merchant 
		{ 
			get { return m_Merchant; } 
			set 
			{ 
				m_Merchant = value;
				
				if(m_Merchant != null)
				{
					m_Merchant.Plot = this;
					
					Point3D p = m_Definition.Location;
					p.X++;
					p.Y++;
                    p.Z = 27;

                    if (m_PlotMulti != null && m_PlotMulti.Fillers.Count > 0)
                    {
                        p = m_PlotMulti.Fillers[0].Location;
                        p.Z = m_PlotMulti.Fillers[0].Z + TileData.ItemTable[m_PlotMulti.Fillers[0].ItemID & TileData.MaxItemValue].CalcHeight;
                    }

					m_Merchant.MoveToWorld(p, m_Definition.Map);
				}
			} 
		}
		
		[CommandProperty(AccessLevel.GameMaster)]
		public PlotSign Sign { get { return m_Sign; } set { m_Sign = value; } }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public MaginciaPlotAuction Auction 
		{ 
			get 
			{ 
				/*if(m_Auction == null)
				{
					TimeSpan ts;
					if(m_Owner == null)
						ts = MaginciaBazaar.GetLongAuctionTime;
					else
						ts = MaginciaBazaar.GetShortAuctionTime;
						
					m_Auction = new MaginciaPlotAuction(this, ts);
				}*/
				
				return m_Auction; 
			} 
			set { m_Auction = value; } 
		}

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active { get { return MaginciaBazaar.IsActivePlot(this); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime AuctionEnds
        {
            get
            {
                if (Auction == null)
                    return DateTime.MinValue;
                return Auction.AuctionEnd;
            }

        }
		
		public MaginciaBazaarPlot(PlotDef definition)
		{
			m_Definition = definition;
			m_Owner = null;
			m_PlotMulti = null;
			m_Merchant = null;
			m_ShopName = null;
		}
		
		public bool IsOwner(Mobile from)
		{
			if(from == null || m_Owner == null)
				return false;
				
			if(from == m_Owner)
				return true;
				
			Account acct1 = from.Account as Account;
			Account acct2 = m_Owner.Account as Account;
			
			return acct1 != null && acct2 != null && acct1 == acct2;
		}
		
		public void AddPlotSign()
		{
			m_Sign = new PlotSign(this);
            m_Sign.MoveToWorld(m_Definition.SignLoc, m_Definition.Map);
		}
		
		public void Reset()
		{
			if(m_PlotMulti != null)
                Timer.DelayCall(TimeSpan.FromMinutes(2), new TimerCallback(DeleteMulti_Callback));

            EndTempMultiTimer();

			if(m_Merchant != null)
				m_Merchant.Dismiss();

            m_Owner = null;
            m_ShopName = null;
			m_Merchant = null;	
			m_ShopName = null;
		}

        public void NewAuction(TimeSpan time)
        {
            m_Auction = new MaginciaPlotAuction(this, time);

            if (m_Sign != null)
                m_Sign.InvalidateProperties();
        }

        private void DeleteMulti_Callback()
        {
            if (m_PlotMulti != null)
                m_PlotMulti.Delete();

            m_PlotMulti = null;
        }
		
		public void OnTick()
		{
            if (m_Auction != null)
                m_Auction.OnTick();
				
			if(m_Merchant != null)
				m_Merchant.OnTick();

            if (m_Sign != null)
                m_Sign.InvalidateProperties();
		}
		
		#region Stall Style Multis
		private Timer m_Timer;
		
		public void AddTempMulti(int idx1, int idx2)
		{
            if (m_PlotMulti != null)
            {
                m_PlotMulti.Delete();
                m_PlotMulti = null;
            }

            BaseBazaarMulti multi = null;

            if (idx1 == 0)
            {
                switch (idx2)
                {
                    case 0: multi = new CommodityStyle1(); break;
                    case 1: multi = new CommodityStyle2(); break;
                    case 2: multi = new CommodityStyle3(); break;
                }
            }
            else
            {
                switch (idx2)
                {
                    case 0: multi = new PetStyle1(); break;
                    case 1: multi = new PetStyle2(); break;
                    case 2: multi = new PetStyle3(); break;
                }
            }

            if (multi != null)
            {
                PlotMulti = multi;
                BeginTempMultiTimer();
            }
		}
		
		public void ConfirmMulti(bool commodity)
		{
			EndTempMultiTimer();
			
			if(commodity)
				Merchant = new CommodityBroker(this);
			else
				Merchant = new PetBroker(this);
		}
		
		public void RemoveTempPlot()
		{
			EndTempMultiTimer();

            if (m_PlotMulti != null)
            {
                m_PlotMulti.Delete();
                m_PlotMulti = null;
            }
		}
		
		public void BeginTempMultiTimer()
		{
			if(m_Timer != null)
			{
				m_Timer.Stop();
				m_Timer = null;
			}
			
			m_Timer = new InternalTimer(this);
			m_Timer.Start();
		}
		
		public void EndTempMultiTimer()
		{
			if(m_Timer != null)
			{
				m_Timer.Stop();
				m_Timer = null;
			}
		}
		
		public bool HasTempMulti()
		{
			return m_Timer != null;
		}
		
		private class InternalTimer : Timer
		{
			private MaginciaBazaarPlot m_Plot;
			
			public InternalTimer(MaginciaBazaarPlot plot) : base(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1))
			{
				m_Plot = plot;
			}
			
			protected override void OnTick()
			{
				if(m_Plot != null)
					m_Plot.RemoveTempPlot();
			}
		}
		
		#endregion
		
		public override string ToString()
		{
			return "...";
		}
		
		public bool TrySetShopName(Mobile from, string text)
		{
			if (text == null || !Server.Guilds.BaseGuildGump.CheckProfanity(text) || text.Length == 0 || text.Length > 40)
                return false;
				
			m_ShopName = text;
			
			if(m_Merchant != null)
				m_Merchant.InvalidateProperties();
				
			if(m_Sign != null)
				m_Sign.InvalidateProperties();
				
			from.SendLocalizedMessage(1150333); // Your shop has been renamed.
				
			return true;
		}
		
		public void FireBroker()
		{
			if(m_Merchant != null)
			{
				m_Merchant.Delete();
				m_Merchant = null;

                if (m_PlotMulti != null)
                {
                    m_PlotMulti.Delete();
                    m_PlotMulti = null;
                }
			}
		}
		
		public void Abandon()
		{
            Reset();

            if (m_Auction != null)
                m_Auction.ChangeAuctionTime(MaginciaBazaar.GetShortAuctionTime);
		}

        public int GetBid(Mobile from)
        {
            if (m_Auction != null && m_Auction.Auctioners.ContainsKey(from))
                return m_Auction.Auctioners[from].Amount;
            return 0;
        }

		public void Serialize(GenericWriter writer)
		{
			writer.Write((int)0);

            m_Definition.Serialize(writer);

			writer.Write(m_Owner);
			writer.Write(m_ShopName);
			writer.Write(m_Merchant);
			writer.Write(m_Sign);
			writer.Write(m_PlotMulti);
			
			if(m_Auction != null)
			{
				writer.Write((bool)true);
				m_Auction.Serialize(writer);
			}
			else
				writer.Write((bool)false);
		}
		
		public MaginciaBazaarPlot(GenericReader reader)
		{
			int version = reader.ReadInt();

            m_Definition = new PlotDef(reader);

			m_Owner = reader.ReadMobile();
			m_ShopName = reader.ReadString();
			m_Merchant = reader.ReadMobile() as BaseBazaarBroker;
			m_Sign = reader.ReadItem() as PlotSign;
			m_PlotMulti = reader.ReadItem() as BaseBazaarMulti;
			
			if(reader.ReadBool())
				m_Auction = new MaginciaPlotAuction(reader, this);
				
			if(m_Merchant != null)
				m_Merchant.Plot = this;

            if (m_Sign != null)
                m_Sign.Plot = this;
		}
	}
	
	[PropertyObject]
	public class PlotDef
	{
		private string m_ID;
		private Point3D m_Location;
		private Map m_Map;
		
		[CommandProperty(AccessLevel.GameMaster)]
		public string ID { get { return m_ID; } set { m_ID = value; } }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public Point3D Location { get { return m_Location; } set { m_Location = value; } }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public Map Map { get { return m_Map; } }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public Point3D SignLoc { get { return new Point3D(m_Location.X + 1, m_Location.Y - 2, m_Location.Z); } }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public Point3D MultiLocation { get { return new Point3D(m_Location.X, m_Location.Y, m_Location.Z + 2); } }
		
		public PlotDef(string id, Point3D pnt, int mapID)
		{
			m_ID = id;
			m_Location = pnt;
            m_Map = Server.Map.Maps[mapID];
		}
		
		public override string ToString()
		{
			return "...";
		}

        public PlotDef(GenericReader reader)
        {
            int version = reader.ReadInt();

            m_ID = reader.ReadString();
            m_Location = reader.ReadPoint3D();
            m_Map = reader.ReadMap();
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write((int)0);

            writer.Write(m_ID);
            writer.Write(m_Location);
            writer.Write(m_Map);
        }
	}
}