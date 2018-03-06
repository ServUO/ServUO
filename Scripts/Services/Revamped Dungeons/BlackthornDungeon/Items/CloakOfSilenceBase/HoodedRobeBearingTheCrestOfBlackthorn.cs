using Server;
using System;
using Server.Engines.Craft;

namespace Server.Items
{
    [Flipable(0x2684, 0x2683)]
    public class HoodedRobeBearingTheCrestOfBlackthorn3 : BaseOuterTorso, IRepairable
    {
        public CraftSystem RepairSystem { get { return DefTailoring.CraftSystem; } }
        public override int LabelNumber { get { return 1029863; } } // Hooded Robe
        public override bool IsArtifact { get { return true; } }        
        
        [Constructable]
        public HoodedRobeBearingTheCrestOfBlackthorn3()
            : base(0x2683)
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            SkillBonuses.SetValues(0, SkillName.Stealth, 10.0);
            Hue = 2130;
        }

        public HoodedRobeBearingTheCrestOfBlackthorn3(Serial serial)
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
