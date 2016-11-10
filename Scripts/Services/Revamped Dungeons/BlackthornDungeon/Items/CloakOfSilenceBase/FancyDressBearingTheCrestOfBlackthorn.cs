using Server;
using System;

namespace Server.Items
{
    public class FancyDressBearingTheCrestOfBlackthorn3 : FancyDress
    {
        public override bool IsArtifact { get { return true; } }
                
        [Constructable]
        public FancyDressBearingTheCrestOfBlackthorn3()
            : base()
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            SkillBonuses.SetValues(0, SkillName.Stealth, 10.0);
            Hue = 2130;
        }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public FancyDressBearingTheCrestOfBlackthorn3(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}