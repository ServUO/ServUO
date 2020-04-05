using System;
using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefBlacksmithy), typeof(GargishMaul))]
    [FlipableAttribute(0x143B, 0x143A)]
    public class Maul : BaseBashing
    {
        [Constructable]
        public Maul()
            : base(0x143B)
        {
            Weight = 10.0;
        }

        public Maul(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility
        {
            get
            {
                return WeaponAbility.DoubleStrike;
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
                return 45;
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
                return 70;
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
