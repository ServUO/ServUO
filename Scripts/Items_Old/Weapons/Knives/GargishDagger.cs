using System;

namespace Server.Items
{
    // Based off a Dagger
    [FlipableAttribute(0x902, 0x406A)]
    public class GargishDagger : BaseKnife
    {
        [Constructable]
        public GargishDagger()
            : base(0x902)
        {
            //Weight = 1.0;
        }

        public GargishDagger(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility
        {
            get
            {
                return WeaponAbility.ShadowStrike;
            }
        }
        public override WeaponAbility SecondaryAbility
        {
            get
            {
                return WeaponAbility.InfectiousStrike;
            }
        }
        public override int AosStrengthReq
        {
            get
            {
                return 10;
            }
        }
        public override int AosMinDamage
        {
            get
            {
                return 10;
            }
        }
        public override int AosMaxDamage
        {
            get
            {
                return 12;
            }
        }
        public override int AosSpeed
        {
            get
            {
                return 56;
            }
        }
        public override float MlSpeed
        {
            get
            {
                return 2.00f;
            }
        }
        public override int OldStrengthReq
        {
            get
            {
                return 1;
            }
        }
        public override int OldMinDamage
        {
            get
            {
                return 3;
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
                return 55;
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
                return 40;
            }
        }
        public override SkillName DefSkill
        {
            get
            {
                return SkillName.Fencing;
            }
        }
        public override WeaponType DefType
        {
            get
            {
                return WeaponType.Piercing;
            }
        }
        public override WeaponAnimation DefAnimation
        {
            get
            {
                return WeaponAnimation.Pierce1H;
            }
        }

        public override Race RequiredRace { get { return Race.Gargoyle; } }
        public override bool CanBeWornByGargoyles { get { return true; } }
        
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