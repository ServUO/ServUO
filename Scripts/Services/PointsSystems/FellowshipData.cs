using Server.Engines.SeasonalEvents;
using Server.Mobiles;

namespace Server.Engines.Points
{
    public class FellowshipData : PointsSystem
    {
        public override PointsType Loyalty => PointsType.FellowshipData;
        public override TextDefinition Name => "Fellowship Event";
        public override bool AutoAdd => true;
        public override double MaxPoints => double.MaxValue;

        public override bool ShowOnLoyaltyGump => false;
        public bool InSeason => SeasonalEventSystem.IsActive(EventType.Fellowship);

        public bool Enabled { get; set; }
        public bool QuestContentGenerated { get; set; }

        public override void SendMessage(PlayerMobile from, double old, double points, bool quest)
        {
            from.SendLocalizedMessage(1159189, string.Format("{0}", ((int)points).ToString())); // The soul has been cleansed and you have been awarded ~1_SILVER~ Fellowship Silver for your efforts!
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);

            writer.Write(Enabled);
            writer.Write(QuestContentGenerated);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    Enabled = reader.ReadBool();
                    QuestContentGenerated = reader.ReadBool();
                    break;
            }
        }
    }
}
