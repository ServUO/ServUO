using System; 
using Server; 
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.Gumps;
using Server.Network;
using System.Collections.Generic; 

namespace Server.Engines.LotterySystem 
{ 
    public class ScratcherLotto : Item 
    {
        private bool m_DeleteTicket;     //Deletes ticket on quick scratch if it is a losing ticket
        private bool m_Active;
        private int m_GoldSink;          //Eye candy for GM's!!!
        private int m_SkiesProgressive;  //Progressive for Skies the Limit
        private DateTime m_StatStart;
        private TimeSpan m_WipeStats;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool DeleteTicketOnLoss { get { return m_DeleteTicket; } set { m_DeleteTicket = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsActive { get { return m_Active; } set { m_Active = value; InvalidateProperties(); UpdateSatellites(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int GoldSink { get { return m_GoldSink; } set { m_GoldSink = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SkiesProgressive { get { return m_SkiesProgressive; } set { m_SkiesProgressive = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime StatStart { get { return m_StatStart; } set { m_StatStart = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan WipeStats { get { return m_WipeStats; } set { m_WipeStats = value; } }

        public bool CanWipe { get { return m_StatStart + m_WipeStats < DateTime.Now; } }

        private static ScratcherLotto m_Stone; //Instanced Stone for easy access
        public static ScratcherLotto Stone{ get { return m_Stone; } set { m_Stone = value; } }

        private static List<ScratcherLottoSatellite> m_SatList = new List<ScratcherLottoSatellite>();
        public static List<ScratcherLottoSatellite> SatList { get { return m_SatList; } }

        [Constructable]
        public ScratcherLotto()
            : base(0xED4)
        {
            if (CheckForScratcherStone())
            {
                Console.WriteLine("You can only have one Lotto Scratcher Stone Item.");
                Delete();
                return;
            }

            Name = "Lottery Scratch Tickets";
            Hue = Utility.RandomSlimeHue();
            Movable = false;
            m_Active = true;
            m_Stone = this;
            m_SkiesProgressive = 500000;
            m_DeleteTicket = true;
            m_WipeStats = TimeSpan.FromDays(90);
            m_StatStart = DateTime.Now;
        }

        public override void OnDoubleClick(Mobile from) 
        {
            if (!m_Active && from.AccessLevel == AccessLevel.Player)
                from.SendMessage("Scratch tickets are currenlty inactive at this time.");
            else if (from.InRange(Location, 3))
            {
                if (from.HasGump(typeof(ScratcherStoneGump)))
                    from.CloseGump(typeof(ScratcherStoneGump));

                from.SendGump(new ScratcherStoneGump(this, from));
            }
            else if (from.AccessLevel > AccessLevel.Player)
                from.SendGump( new PropertiesGump( from, this ) );
            else
                from.SendLocalizedMessage(500446); // That is too far away.
        } 

        public override void GetProperties(ObjectPropertyList list) 
        { 
            base.GetProperties(list); 

            if (!m_Active)
                list.Add(1060658, "Status\tOffline");
            else
                list.Add(1060658, "Status\tActive");

            if (ScratcherStats.Stats.Count > 0)
            {
                try
                {
                    int index = ScratcherStats.Stats.Count - 1;
                    string jackpotAmount = String.Format("{0:##,###,###}", ScratcherStats.Stats[index].Payout);

                    list.Add(1060659, "Last Big Win\t{0}", ScratcherStats.Stats[index].Winner.Name);
                    list.Add(1060660, "Date\t{0}", ScratcherStats.Stats[index].WinTime);
                    list.Add(1060661, "Amount\t{0}", jackpotAmount);
                    list.Add(1060662, "Game\t{0}", GetGameType(ScratcherStats.Stats[index].Type));
                }
                catch
                {
                }
            }
        
        }

        public static string GetGameType(TicketType type)
        {
            switch (type)
            {
                default: return "";
                case TicketType.GoldenTicket: return "Golden Ticket";
                case TicketType.CrazedCrafting: return "Crazed Crafting";
                case TicketType.SkiesTheLimit: return "Skies the Limit";
                case TicketType.Powerball: return "Powerball";
            }
        }

        public static void DoProgressiveMessage(Mobile winner, int amount)
        {
            string name = "Somebody";

            if (winner != null)
                name = winner.Name;

            foreach (NetState netState in NetState.Instances)
            {
                Mobile m = netState.Mobile;
                if (m != null)
                {
                    m.PlaySound(1460);
                    m.SendMessage(33, "{0} has won {1} gold in the Skies the Limit Progressive Scratcher!", name, amount.ToString());
                }
            }
        }

        private void DoWipe()
        {
            if (ScratcherStats.Stats != null)
                ScratcherStats.Stats.Clear();

            UpdateSatellites();
            InvalidateProperties();
            m_StatStart = DateTime.Now;
        }

        public void UpdateSatellites()
        {
            foreach (ScratcherLottoSatellite sat in m_SatList)
            {
                if (sat != null && !sat.Deleted)
                    sat.InvalidateProperties();
            }
        }

        public ScratcherLotto(Serial serial) : base( serial )
        { 
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)2); //Version

            writer.Write(m_WipeStats);
            writer.Write(m_StatStart);
            writer.Write(m_DeleteTicket);

            writer.Write(m_Active);
            writer.Write(m_GoldSink);
            writer.Write(m_SkiesProgressive);

            writer.Write(ScratcherStats.Stats.Count);
            for (int i = 0; i < ScratcherStats.Stats.Count; ++i)
            {
                ScratcherStats.Stats[i].Serialize(writer);
            }

            if (CanWipe)
                Timer.DelayCall(TimeSpan.FromSeconds(5), new TimerCallback(DoWipe));

            InvalidateProperties();
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 2:
                    {
                        m_WipeStats = reader.ReadTimeSpan();
                        m_StatStart = reader.ReadDateTime();
                        goto case 1;
                    }
                case 1:
                    {
                        m_DeleteTicket = reader.ReadBool();
                        goto case 0;
                    }
                case 0:
                    {
                        m_Active = reader.ReadBool();
                        m_GoldSink = reader.ReadInt();
                        m_SkiesProgressive = reader.ReadInt();

                        int statsCount = reader.ReadInt();
                        for (int i = 0; i < statsCount; i++)
                        {
                            new ScratcherStats(reader);
                        }
                        break;
                    }
            }
            m_Stone = this;
        }

        public override void OnAfterDelete()
        {
            for (int i = 0; i < m_SatList.Count; ++i)
            {
                if (m_SatList[i] != null && !m_SatList[i].Deleted)
                    m_SatList[i].Delete();
            }
        }

        private bool CheckForScratcherStone()
        {
            foreach (Item item in World.Items.Values)
            {
                if (item is ScratcherLotto && !item.Deleted && item != this)
                    return true;
            }

            return false;
        }

        public void AddToSatList(ScratcherLottoSatellite satellite)
        {
            if (m_SatList != null && !m_SatList.Contains(satellite))
                m_SatList.Add(satellite);
        }

        public void RemoveFromSatList(ScratcherLottoSatellite satellite)
        {
            if (m_SatList != null && m_SatList.Contains(satellite))
                m_SatList.Remove(satellite);
        }
    } 
}