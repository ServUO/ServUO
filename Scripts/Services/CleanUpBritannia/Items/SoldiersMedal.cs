using System;
using Server.Mobiles;

namespace Server.Items
{
    public class SoldiersMedal : BaseTalisman, ITokunoDyable
    {
        [Constructable]
        public SoldiersMedal()
            : base(0x2F5B)
        {
            this.Hue = 1902;
            this.SkillBonuses.SetValues(0, SkillName.Tactics, 10.0);
            this.Attributes.AttackChance = 5;
            this.Attributes.RegenStam = 2;
            this.Attributes.WeaponDamage = 20;
        }

        public SoldiersMedal(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1154726;
            }
        }// Soldier's Medal
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
