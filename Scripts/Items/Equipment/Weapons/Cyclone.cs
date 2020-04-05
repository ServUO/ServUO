using System;

namespace Server.Items
{
    public class Cyclone : BaseThrown
    {
        [Constructable]
        public Cyclone()
            : base(0x901)
        {
            Weight = 6.0;
            Layer = Layer.OneHanded;
        }

        public Cyclone(Serial serial)
            : base(serial)
        {
        }

        public override int MinThrowRange { get { return 6; } }

        public override WeaponAbility PrimaryAbility
        {
            get
            {
                return WeaponAbility.MovingShot;
            }
        }
        public override WeaponAbility SecondaryAbility
        {
            get
            {
                return WeaponAbility.InfusedThrow;
            }
        }
        public override int StrengthReq
        {
            get
            {
                return 40;
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
                return 17;
            }
        }
        public override float Speed
        {
            get
            {
                return 3.25f;
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
                return 60;
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
