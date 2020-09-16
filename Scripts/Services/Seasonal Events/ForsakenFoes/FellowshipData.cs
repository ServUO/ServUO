using Server.Engines.Points;
using Server.Mobiles;

namespace Server.Engines.Fellowship
{
    public class FellowshipData : PointsSystem
    {
        public override PointsType Loyalty => PointsType.FellowshipData;
        public override TextDefinition Name => "Fellowship Event";
        public override bool AutoAdd => true;
        public override double MaxPoints => double.MaxValue;

        public override bool ShowOnLoyaltyGump => false;

        public override void SendMessage(PlayerMobile from, double old, double points, bool quest)
        {
            from.SendLocalizedMessage(1159189, string.Format("{0}", ((int)points).ToString())); // The soul has been cleansed and you have been awarded ~1_SILVER~ Fellowship Silver for your efforts!
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(2);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 2:
                case 1:
                    if (version == 1)
                    {
                        reader.ReadBool();
                        var questGenerated = reader.ReadBool();

                        Timer.DelayCall(() =>
                        {
                            var jolly = SeasonalEvents.SeasonalEventSystem.GetEvent<ForsakenFoesEvent>();

                            if (jolly != null)
                            {
                                jolly.QuestContentGenerated = questGenerated;
                            }
                        });

                    }
                    break;
            }
        }
    }
}
