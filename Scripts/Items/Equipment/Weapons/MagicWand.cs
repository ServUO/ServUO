using System;
using Server.Engines.Craft;

namespace Server.Items
{
    public class MagicWand : BaseBashing, IRepairable
    {
        public CraftSystem RepairSystem { get { return DefCarpentry.CraftSystem; } }

        [Constructable]
        public MagicWand()
            : base(0xDF2)
        {
            this.Weight = 1.0;
        }

        public MagicWand(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility
        {
            get
            {
                return WeaponAbility.Dismount;
            }
        }
        public override WeaponAbility SecondaryAbility
        {
            get
            {
                return WeaponAbility.Disarm;
            }
        }
        public override int StrengthReq
        {
            get
            {
                return 5;
            }
        }
        public override int MinDamage
        {
            get
            {
                return 9;
            }
        }
        public override int MaxDamage
        {
            get
            {
                return 11;
            }
        }
        public override float Speed
        {
            get
            {
                return 2.75f;
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