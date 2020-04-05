using System;
using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefTinkering), typeof(DualShortAxes))]
    [FlipableAttribute(0xF43, 0xF44)]
    public class Hatchet : BaseAxe
    {
        [Constructable]
        public Hatchet()
            : base(0xF43)
        {
            Weight = 4.0;
        }

        public Hatchet(Serial serial)
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
                return WeaponAbility.Disarm;
            }
        }
        public override int StrengthReq
        {
            get
            {
                return 20;
            }
        }
        public override int MinDamage
        {
            get
            {
                return 13;
            }
        }
        public override int MaxDamage
        {
            get
            {
                return 16;
            }
        }
        public override float Speed
        {
            get
            {
                return 2.75f;
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
