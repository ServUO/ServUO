using System;
using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefBlacksmithy), typeof(Shortblade))]
    [FlipableAttribute(0x2D21, 0x2D2D)]
    public class AssassinSpike : BaseKnife
    {
        [Constructable]
        public AssassinSpike()
            : base(0x2D21)
        {
            Weight = 4.0;
        }

        public AssassinSpike(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility
        {
            get
            {
                return WeaponAbility.InfectiousStrike;
            }
        }
        public override WeaponAbility SecondaryAbility
        {
            get
            {
                return WeaponAbility.ShadowStrike;
            }
        }
        public override int StrengthReq
        {
            get
            {
                return 15;
            }
        }
        public override int MinDamage
        {
            get
            {
                return 10;
            }
        }
        public override int MaxDamage
        {
            get
            {
                return 12;
            }
        }
        public override float Speed
        {
            get
            {
                return 2.00f;
            }
        }
       
        public override int DefMissSound
        {
            get
            {
                return 0x239;
            }
        }
        public override SkillName DefSkill
        {
            get
            {
                return SkillName.Fencing;
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
