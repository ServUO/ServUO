using System;

namespace Server.Items
{
    [FlipableAttribute(0x144f, 0x1454)]
    public class DaemonChest : BaseArmor
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public DaemonChest()
            : base(0x144F)
        {
            this.Weight = 6.0;
            this.Hue = 0x648;

            this.ArmorAttributes.SelfRepair = 1;
        }

        public DaemonChest(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance
        {
            get
            {
                return 6;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 6;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 7;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 5;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 7;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
            }
        }
        public override int AosStrReq
        {
            get
            {
                return 60;
            }
        }
        public override int OldStrReq
        {
            get
            {
                return 40;
            }
        }
        public override int OldDexBonus
        {
            get
            {
                return -6;
            }
        }
        public override int ArmorBase
        {
            get
            {
                return 46;
            }
        }
        public override ArmorMaterialType MaterialType
        {
            get
            {
                return ArmorMaterialType.Bone;
            }
        }
        public override CraftResource DefaultResource
        {
            get
            {
                return CraftResource.RegularLeather;
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1041372;
            }
        }// daemon bone armor
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (this.Weight == 1.0)
                this.Weight = 6.0;

            if (this.ArmorAttributes.SelfRepair == 0)
                this.ArmorAttributes.SelfRepair = 1;
        }
    }
}