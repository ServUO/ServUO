using System;
using Server.Engines.Harvest;

namespace Server.Items
{
    [FlipableAttribute(0x48C4, 0x48C5)]
    public class GargishScythe : BasePoleArm
    {
        [Constructable]
        public GargishScythe()
            : base(0x48C4)
        {
            Weight = 5.0;
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
                return 16;
            }
        }
        public override int MaxDamage
        {
            get
            {
                return 19;
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