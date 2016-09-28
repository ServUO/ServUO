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
            this.Weight = 8.0;
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
        public override int AosStrengthReq
        {
            get
            {
                return 20;
            }
        }
        public override int AosMinDamage
        {
            get
            {
                return 11;
            }
        }
        public override int AosMaxDamage
        {
            get
            {
                return 15;
            }
        }
        public override int AosSpeed
        {
            get
            {
                return 42;
            }
        }
        public override float MlSpeed
        {
            get
            {
                return 2.75f;
            }
        }
        public override int OldStrengthReq
        {
            get
            {
                return 20;
            }
        }
        public override int OldMinDamage
        {
            get
            {
                return 13;
            }
        }
        public override int OldMaxDamage
        {
            get
            {
                return 15;
            }
        }
        public override int OldSpeed
        {
            get
            {
                return 42;
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
        }// TODO
        public override int InitMaxHits
        {
            get
            {
                return 60;
            }
        }// TODO
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