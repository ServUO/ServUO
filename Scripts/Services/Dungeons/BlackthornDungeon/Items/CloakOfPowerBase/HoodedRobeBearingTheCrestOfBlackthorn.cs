using Server.Engines.Craft;

namespace Server.Items
{
    [Flipable(0x2684, 0x2683)]
    public class HoodedRobeBearingTheCrestOfBlackthorn4 : BaseOuterTorso, IRepairable
    {
        public CraftSystem RepairSystem => DefTailoring.CraftSystem;
        public override int LabelNumber => 1029863;  // Hooded Robe
        public override bool IsArtifact => true;

        [Constructable]
        public HoodedRobeBearingTheCrestOfBlackthorn4()
            : base(0x2683)
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            Attributes.BonusStr = 2;
            Attributes.BonusDex = 2;
            Attributes.BonusInt = 2;
            Hue = 2107;
        }

        public HoodedRobeBearingTheCrestOfBlackthorn4(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0)
            {
                MaxHitPoints = 0;
                HitPoints = 0;
            }
        }
    }
}