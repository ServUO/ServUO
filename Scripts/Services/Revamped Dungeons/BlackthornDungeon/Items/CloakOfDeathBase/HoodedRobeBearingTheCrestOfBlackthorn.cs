using Server.Engines.Craft;

namespace Server.Items
{
    [Flipable(0x2684, 0x2683)]
    public class HoodedRobeBearingTheCrestOfBlackthorn6 : BaseOuterTorso, IRepairable
    {
        public CraftSystem RepairSystem => DefTailoring.CraftSystem;
        public override int LabelNumber => 1029863;  // Hooded Robe
        public override bool IsArtifact => true;

        [Constructable]
        public HoodedRobeBearingTheCrestOfBlackthorn6()
            : base(0x2683)
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            Attributes.AttackChance = 3;
            Attributes.DefendChance = 3;
            Attributes.SpellDamage = 3;
            Hue = 2019;
        }

        public HoodedRobeBearingTheCrestOfBlackthorn6(Serial serial)
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