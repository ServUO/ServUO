using System;
using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefBlacksmithy), typeof(GargishBardiche))]
    [FlipableAttribute(0xF4D, 0xF4E)]
    public class Bardiche : BasePoleArm
    {
        [Constructable]
        public Bardiche()
            : base(0xF4D)
        {
            Weight = 7.0;
        }

        public Bardiche(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility
        {
            get
            {
                return WeaponAbility.ParalyzingBlow;
            }
        }
        public override WeaponAbility SecondaryAbility
        {
            get
            {
                return WeaponAbility.Dismount;
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
                return 17;
            }
        }
        public override int MaxDamage
        {
            get
            {
                return 20;
            }
        }
        public override float Speed
        {
            get
            {
                return 3.75f;
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
