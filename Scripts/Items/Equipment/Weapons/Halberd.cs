using System;
using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefBlacksmithy), typeof(GargishTalwar))]
    [FlipableAttribute(0x143E, 0x143F)]
    public class Halberd : BasePoleArm
    {
        [Constructable]
        public Halberd()
            : base(0x143E)
        {
            Weight = 16.0;
        }

        public Halberd(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility
        {
            get
            {
                return WeaponAbility.WhirlwindAttack;
            }
        }
        public override WeaponAbility SecondaryAbility
        {
            get
            {
                return WeaponAbility.ConcussionBlow;
            }
        }
        public override int StrengthReq
        {
            get
            {
                return 95;
            }
        }
        public override int MinDamage
        {
            get
            {
                return 18;
            }
        }
        public override int MaxDamage
        {
            get
            {
                return 21;
            }
        }
        public override float Speed
        {
            get
            {
                return 4.00f;
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
                return 80;
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