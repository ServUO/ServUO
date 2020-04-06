using System;
using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefBlacksmithy), typeof(GargishWarHammer))]
    [FlipableAttribute(0x1439, 0x1438)]
    public class WarHammer : BaseBashing
    {
        [Constructable]
        public WarHammer()
            : base(0x1439)
        {
            Weight = 10.0;
            Layer = Layer.TwoHanded;
        }

        public WarHammer(Serial serial)
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
                return WeaponAbility.CrushingBlow;
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
                return 110;
            }
        }
        public override WeaponAnimation DefAnimation
        {
            get
            {
                return WeaponAnimation.Bash2H;
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