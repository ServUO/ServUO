using System;
using Server.Engines.Harvest;

namespace Server.Items
{
    //Based Off Scythe
    [FlipableAttribute(0x48C4, 0x48C5)]
    public class GargishScythe : BasePoleArm
    {
        [Constructable]
        public GargishScythe()
            : base(0x48C4)
        {
            this.Weight = 5.0;
        }

        public GargishScythe(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility
        {
            get
            {
                return WeaponAbility.BleedAttack;
            }
        }
        public override WeaponAbility SecondaryAbility
        {
            get
            {
                return WeaponAbility.ParalyzingBlow;
            }
        }
        public override int AosStrengthReq
        {
            get
            {
                return 45;
            }
        }
        public override int AosMinDamage
        {
            get
            {
                return 16;
            }
        }
        public override int AosMaxDamage
        {
            get
            {
                return 19;
            }
        }
        public override int AosSpeed
        {
            get
            {
                return 32;
            }
        }
        public override float MlSpeed
        {
            get
            {
                return 3.50f;
            }
        }
        public override int OldStrengthReq
        {
            get
            {
                return 45;
            }
        }
        public override int OldMinDamage
        {
            get
            {
                return 15;
            }
        }
        public override int OldMaxDamage
        {
            get
            {
                return 18;
            }
        }
        public override int OldSpeed
        {
            get
            {
                return 32;
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
        public override HarvestSystem HarvestSystem
        {
            get
            {
                return null;
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

            if (this.Weight == 15.0)
                this.Weight = 5.0;
        }
    }
}