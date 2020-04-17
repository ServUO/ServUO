namespace Server.Items
{
    public class RobeBearingTheCrestOfBlackthorn3 : Robe
    {
        public override bool IsArtifact => true;

        [Constructable]
        public RobeBearingTheCrestOfBlackthorn3()
            : base()
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            SkillBonuses.SetValues(0, SkillName.Stealth, 10.0);
            Hue = 2130;
        }

        public RobeBearingTheCrestOfBlackthorn3(Serial serial)
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