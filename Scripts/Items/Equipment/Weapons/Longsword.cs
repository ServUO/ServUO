using System;
using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefBlacksmithy), typeof(DreadSword))]
    [FlipableAttribute(0xF61, 0xF60)]
    public class Longsword : BaseSword
    {
        [Constructable]
        public Longsword()
            : base(0xF61)
        {
            Weight = 7.0;
        }

        public Longsword(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility
        {
            get
            {
                return WeaponAbility.ArmorIgnore;
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
                return 35;
            }
        }
        public override int MinDamage
        {
            get
            {
                return 14;
            }
        }
        public override int MaxDamage
        {
            get
            {
                return 18;
            }
        }
        public override float Speed
        {
            get
            {
                return 3.50f;
            }
        }
        
        public override int DefHitSound
        {
            get
            {
                return 0x237;
            }
        }
        public override int DefMissSound
        {
            get
            {
                return 0x23A;
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
                return 110;
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