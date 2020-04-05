using System;
using Server.Engines.Harvest;
using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefBlacksmithy), typeof(GargishScythe))]
    [FlipableAttribute(0x26BA, 0x26C4)]
    public class Scythe : BasePoleArm
    {
        [Constructable]
        public Scythe()
            : base(0x26BA)
        {
            Weight = 5.0;
        }

        public Scythe(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility
        {
            get
            {
                return WeaponAbility.BleedAttack;
            }
        }
        public override WeaponAbility SecondaryAbility
        {
            get
            {
                return WeaponAbility.ParalyzingBlow;
            }
        }
        public override int StrengthReq
        {
            get
            {
                return 45;
            }
        }
        public override int MinDamage
        {
            get
            {
                return 16;
            }
        }
        public override int MaxDamage
        {
            get
            {
                return 19;
            }
        }
        public override float Speed
        {
            get
            {
                return 3.50f;
            }
        }
      
        public override int InitMinHits
        {
            get
            {
                return 31;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 100;
            }
        }
        public override HarvestSystem HarvestSystem
        {
            get
            {
                return null;
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
