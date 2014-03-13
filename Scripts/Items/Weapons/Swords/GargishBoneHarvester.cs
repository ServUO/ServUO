using System;

namespace Server.Items
{
    //Based off Bone Harvester
    [FlipableAttribute(0x48C6, 0x48C7)]
    public class GargishBoneHarvester : BaseSword
    {
        [Constructable]
        public GargishBoneHarvester()
            : base(0x48C6)
        {
            this.Weight = 3.0;
        }

        public GargishBoneHarvester(Serial serial)
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
                return WeaponAbility.MortalStrike;
            }
        }
        public override int AosStrengthReq
        {
            get
            {
                return 25;
            }
        }
        public override int AosMinDamage
        {
            get
            {
                return 12;
            }
        }
        public override int AosMaxDamage
        {
            get
            {
                return 16;
            }
        }
        public override int AosSpeed
        {
            get
            {
                return 36;
            }
        }
        public override float MlSpeed
        {
            get
            {
                return 3.00f;
            }
        }
        public override int OldStrengthReq
        {
            get
            {
                return 25;
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
                return 36;
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
                return 70;
            }
        }
        public override Race RequiredRace
        {
            get
            {
                return Race.Gargoyle;
            }
        }
        public override bool CanBeWornByGargoyles
        {
            get
            {
                return true;
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