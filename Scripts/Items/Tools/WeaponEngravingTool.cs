using System;

namespace Server.Items
{
    public class WeaponEngravingTool : BaseEngravingTool
    {
        public override int LabelNumber => 1076158;  // Weapon Engraving Tool

        public override bool DeletedItem => false;
        public override int LowSkillMessage => 1076178;  // // Your tinkering skill is too low to fix this yourself.  An NPC tinkerer can help you repair this for a fee.
        public override int VeteranRewardCliloc => 1076224;  // 8th Year Veteran Reward

        [Constructable]
        public WeaponEngravingTool()
            : base(0x32F8, 10)
        {
            Hue = 0;
        }

        public WeaponEngravingTool(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Engraves => new Type[]
                {
                    typeof(BaseWeapon)
                };

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = InheritsItem ? 0 : reader.ReadInt();
        }
    }
}
