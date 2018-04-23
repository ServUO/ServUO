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
    public class ScratcherLottoSatellite : Item 
    {
        private ScratcherLotto m_Stone;

        [CommandProperty(AccessLevel.GameMaster)]
        public ScratcherLotto LottoStone { get { return m_Stone; } set { m_Stone = value; } }

        [Constructable]
        public ScratcherLottoSatellite()
            : base(0xED4)
        {

            Name = "Lottery Scratch Tickets";
            Hue = Utility.RandomSlimeHue();
            Movable = false;

            if (ScratcherLotto.Stone != null)
            {
                m_Stone = ScratcherLotto.Stone;
                m_Stone.AddToSatList(this);
            }
            else Delete();
        }

        public override void OnDoubleClick(Mobile from) 
        {
            if (m_Stone == null || (!m_Stone.IsActive && from.AccessLevel == AccessLevel.Player))
                from.SendMessage("Scratch tickets are currenlty inactive at this time.");
            else if (from.InRange(Location, 3))
            {
                if (from.HasGump(typeof(ScratcherStoneGump)))
                    from.CloseGump(typeof(ScratcherStoneGump));

                from.SendGump(new ScratcherStoneGump(m_Stone, from));
            }
            else if (from.AccessLevel > AccessLevel.Player)
                from.SendGump( new PropertiesGump( from, m_Stone ) );
            else
                from.SendLocalizedMessage(500446); // That is too far away.
        } 

        public override void GetProperties(ObjectPropertyList list) 
        { 
            base.GetProperties(list); 

            if (m_Stone == null || !m_Stone.IsActive)
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
                    list.Add(1060662, "Game\t{0}", ScratcherLotto.GetGameType(ScratcherStats.Stats[index].Type));
                }
                catch
                {
                }
            }
        }

        public override void OnAfterDelete()
        {
            if (m_Stone != null)
                m_Stone.RemoveFromSatList(this);

            base.OnAfterDelete();
        }

        public ScratcherLottoSatellite(Serial serial) : base( serial )
        { 
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); //Version

            writer.Write(m_Stone);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            m_Stone = (ScratcherLotto)reader.ReadItem();
            if (m_Stone != null)
                m_Stone.AddToSatList(this);
        }
    } 
}