using Server.Mobiles;

namespace Server.Engines.Points
{
    public class DespiseCrystals : PointsSystem
    {
        public override PointsType Loyalty => PointsType.DespiseCrystals;
        public override TextDefinition Name => m_Name;
        public override bool AutoAdd => true;
        public override double MaxPoints => double.MaxValue;

        private readonly TextDefinition m_Name = new TextDefinition(1151673);

        public override void SendMessage(PlayerMobile from, double old, double points, bool quest)
        {
            from.SendLocalizedMessage(1153423, ((int)points).ToString()); // You have gained ~1_AMT~ Dungeon Crystal Points of Despise.
        }

        public override TextDefinition GetTitle(PlayerMobile from)
        {
            return new TextDefinition(1123418);
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
