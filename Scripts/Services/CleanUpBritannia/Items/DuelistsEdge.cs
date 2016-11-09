using System;
using Server.Mobiles;

namespace Server.Items
{ 
    public class DuelistsEdge : BaseTalisman
    {
        [Constructable]
        public DuelistsEdge()
            : base(0x2F58)
        {
            this.Hue = 1902;
            this.SkillBonuses.SetValues(0, SkillName.Anatomy, 10.0);
            this.Attributes.RegenStam = 2;
            this.Attributes.AttackChance = 5;
            this.Attributes.WeaponDamage = 20;
        }

        public DuelistsEdge(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1154727;
            }
        }// Duelist's Edge
        public override bool ForceShowName
        {
            get
            {
                return true;
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

            int version = reader.ReadInt();
        }
    }
}
