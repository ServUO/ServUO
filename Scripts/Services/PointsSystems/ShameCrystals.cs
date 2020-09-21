using Server.Mobiles;

namespace Server.Engines.Points
{
    public class ShameCrystals : PointsSystem
    {
        public override PointsType Loyalty => PointsType.ShameCrystals;
        public override TextDefinition Name => m_Name;
        public override bool AutoAdd => true;
        public override double MaxPoints => double.MaxValue;

        private readonly TextDefinition m_Name = new TextDefinition(1151673);

        public override void SendMessage(PlayerMobile from, double old, double points, bool quest)
        {
            from.SendLocalizedMessage(1151634, string.Format("{0}\t{1}\t{2}", ((int)points).ToString(), "Shame", ((int)old + points).ToString())); // You gain ~1_AMT~ dungeon points for ~2_NAME~. Your total is now ~3_TOTAL~.
        }

        public override TextDefinition GetTitle(PlayerMobile from)
        {
            return new TextDefinition(1123444);
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
