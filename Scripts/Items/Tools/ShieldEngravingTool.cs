using System;

namespace Server.Items
{
    public class ShieldEngravingTool : BaseEngravingTool
    {
        public override int LabelNumber { get { return 1159004; } } // Weapon Engraving Tool

        public override bool DeletedItem { get { return false; } }
        public override int LowSkillMessage { get { return 1076178; } } // // Your tinkering skill is too low to fix this yourself.  An NPC tinkerer can help you repair this for a fee.
        public override int VeteranRewardCliloc { get { return 0; } } 

        [Constructable]
        public ShieldEngravingTool()
            : base(0x1EB8, 10)
        {
            Hue = 1165;
        }

        public ShieldEngravingTool(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Engraves
        {
            get
            {
                return new Type[]
                {
                    typeof(BaseShield)
                };
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
