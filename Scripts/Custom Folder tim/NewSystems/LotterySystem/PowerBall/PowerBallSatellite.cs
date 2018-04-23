using System; 
using Server; 
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.Gumps;
using System.Collections.Generic; 

namespace Server.Engines.LotterySystem 
{
    public class PowerBallSatellite : Item
    {
        private PowerBall m_PowerBall;

        [CommandProperty(AccessLevel.GameMaster)]
        public PowerBall PowerBall { get { return m_PowerBall; } set { m_PowerBall = value; } }

        [Constructable]
        public PowerBallSatellite() : base(0xED4) 
        {
            Hue = 592;

            Name = "Let's Play Powerball!";
            Movable = false;

            if (PowerBall.PowerBallInstance != null)
            {
                m_PowerBall = PowerBall.PowerBallInstance;
                m_PowerBall.AddToSatList(this);
            }
            else Delete();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_PowerBall == null)
                base.OnDoubleClick(from);

            else if (!m_PowerBall.IsActive && from.AccessLevel == AccessLevel.Player)
                from.SendMessage("Powerball is currenlty inactive at this time.");
            else if (from.InRange(Location, 3))
            {
                if (from.HasGump(typeof(PowerBallStatsGump)))
                    from.CloseGump(typeof(PowerBallStatsGump));

                from.SendGump(new PowerBallStatsGump(m_PowerBall, from));
            }
            else if (from.AccessLevel > AccessLevel.Player)
                from.SendGump(new PropertiesGump(from, m_PowerBall));
            else
                from.SendLocalizedMessage(500446); // That is too far away.
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_PowerBall != null && !m_PowerBall.Deleted)
            {
                if (!m_PowerBall.IsActive)
                    list.Add(1060658, "Status\tOffline");
                else
                    list.Add(1060658, "Status\tActive");

                if (PowerBallStats.JackpotStats.Count > 0)
                {
                    int index = PowerBallStats.JackpotStats.Count - 1;

                    list.Add(1060659, "Last Jackpot\t{0}", PowerBallStats.JackpotStats[index].JackpotWinner.Name);
                    list.Add(1060660, "Date\t{0}", PowerBallStats.JackpotStats[index].JackpotTime);
                    list.Add(1060661, "Amount\t{0}", PowerBallStats.JackpotStats[index].JackpotAmount);
                }
            }
            else
                list.Add("Disabled");

        }

        public override void OnAfterDelete()
        {
            if (m_PowerBall != null)
                m_PowerBall.RemoveFromSatList(this);

            base.OnAfterDelete();
        }

        public PowerBallSatellite(Serial serial) : base (serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); //Version
            writer.Write(m_PowerBall);

        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            m_PowerBall = reader.ReadItem() as PowerBall;

            if (m_PowerBall != null)
                m_PowerBall.AddToSatList(this);
        }
    }
}