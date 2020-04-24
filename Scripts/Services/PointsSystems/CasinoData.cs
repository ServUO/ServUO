using Server.Mobiles;

namespace Server.Engines.Points
{
    public class CasinoData : PointsSystem
    {
        public override PointsType Loyalty => PointsType.CasinoData;
        public override TextDefinition Name => m_Name;
        public override bool AutoAdd => true;
        public override double MaxPoints => double.MaxValue;
        public override bool ShowOnLoyaltyGump => false;

        public static readonly int ChipCost = 100;

        private readonly TextDefinition m_Name = new TextDefinition(1153485); // Fortune's Fire Resort & Casino

        public override void SendMessage(PlayerMobile from, double old, double points, bool quest)
        {
            //from.SendLocalizedMessage(1153189, ((int)points).ToString());
        }

        public override TextDefinition GetTitle(PlayerMobile from)
        {
            return new TextDefinition(1153485); // Fortune's Fire Resort & Casino
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            if (Version >= 2)
            {
                int version = reader.ReadInt();

                // all deserialize code in here
            }
        }
    }
}
