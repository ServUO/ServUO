using System;
using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefBlacksmithy), typeof(GargishTalwar))]
    [FlipableAttribute(0x2D32, 0x2D26)]
    public class RuneBlade : BaseSword
    {
        [Constructable]
        public RuneBlade()
            : base(0x2D32)
        {
            Weight = 7.0;
            Layer = Layer.TwoHanded;
        }

        public RuneBlade(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility
        {
            get
            {
                return WeaponAbility.Disarm;
            }
        }
        public override WeaponAbility SecondaryAbility
        {
            get
            {
                return WeaponAbility.Bladeweave;
            }
        }
        public override int StrengthReq
        {
            get
            {
                return 30;
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
                return 17;
            }
        }
        public override float Speed
        {
            get
            {
                return 3.00f;
            }
        }
       
        public override int DefHitSound
        {
            get
            {
                return 0x23B;
            }
        }
        public override int DefMissSound
        {
            get
            {
                return 0x239;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 30;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 60;
            }
        }
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