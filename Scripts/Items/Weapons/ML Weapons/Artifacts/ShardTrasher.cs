using System;

namespace Server.Items
{
    public class ShardThrasher : DiamondMace
    {
        [Constructable]
        public ShardThrasher()
        {
            this.Hue = 0x4F2;

            this.WeaponAttributes.HitPhysicalArea = 30;
            this.Attributes.BonusStam = 8;
            this.Attributes.AttackChance = 10;
            this.Attributes.WeaponSpeed = 35;
            this.Attributes.WeaponDamage = 40;
        }

        public ShardThrasher(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1072918;
            }
        }// Shard Thrasher
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}