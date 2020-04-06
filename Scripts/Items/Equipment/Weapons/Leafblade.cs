using System;
using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefBlacksmithy), typeof(BloodBlade))]
    [FlipableAttribute(0x2D22, 0x2D2E)]
    public class Leafblade : BaseKnife
    {
        [Constructable]
        public Leafblade()
            : base(0x2D22)
        {
            Weight = 8.0;
        }

        public Leafblade(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility
        {
            get
            {
                return WeaponAbility.Feint;
            }
        }
        public override WeaponAbility SecondaryAbility
        {
            get
            {
                return WeaponAbility.ArmorIgnore;
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
                return 11;
            }
        }
        public override int MaxDamage
        {
            get
            {
                return 15;
            }
        }
        public override float Speed
        {
            get
            {
                return 2.75f;
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