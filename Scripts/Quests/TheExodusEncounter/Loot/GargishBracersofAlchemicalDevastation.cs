using System;

namespace Server.Items
{
    public class GargishBracersofAlchemicalDevastation : GargishLeatherArms
    {
        public override bool IsArtifact { get { return true; } }

        private AosWeaponAttributes m_AosWeaponAttributes;

        [Constructable]
        public GargishBracersofAlchemicalDevastation()
        {
            this.ItemID = 0x0301;
            this.Weight = 2.0;
            this.Attributes.RegenMana = 4;
            this.Attributes.CastRecovery = 3;
            this.ArmorAttributes.MageArmor = 1;
            this.m_AosWeaponAttributes = new AosWeaponAttributes(this);
            this.m_AosWeaponAttributes.HitFireball = 15;
        }

        public GargishBracersofAlchemicalDevastation(Serial serial) : base(serial)
        {
        }

        private enum SaveFlag
        {
            None = 0x00000000,
            WeaponAttributes = 0x00000001,
        }

        public override int LabelNumber
        {
            get
            {
                return 1153523;
            }
        }//Bracers of Alchemical Devastation [Replica]

        public override int BasePhysicalResistance { get { return 10; } }
        public override int BaseFireResistance { get { return 8; } }
        public override int BaseColdResistance { get { return 8; } }
        public override int BasePoisonResistance { get { return 8; } }
        public override int BaseEnergyResistance { get { return 8; } }
        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public override bool CanFortify { get { return false; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public AosWeaponAttributes WeaponAttributes
        {
            get { return this.m_AosWeaponAttributes; }
            set { }
        }

        public override void AppendChildNameProperties(ObjectPropertyList list)
        {
            base.AppendChildNameProperties(list);

            int prop;

            if ((prop = this.m_AosWeaponAttributes.HitFireball) != 0)
                list.Add(1060420, prop.ToString());
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            SaveFlag flags = SaveFlag.None;

            SetSaveFlag(ref flags, SaveFlag.WeaponAttributes, !this.m_AosWeaponAttributes.IsEmpty);

            writer.Write((int)flags);

            if (GetSaveFlag(flags, SaveFlag.WeaponAttributes))
                this.m_AosWeaponAttributes.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            SaveFlag flags = (SaveFlag)reader.ReadInt();

            if (GetSaveFlag(flags, SaveFlag.WeaponAttributes))
                this.m_AosWeaponAttributes = new AosWeaponAttributes(this, reader);
            else
                this.m_AosWeaponAttributes = new AosWeaponAttributes(this);
        }

        private static void SetSaveFlag(ref SaveFlag flags, SaveFlag toSet, bool setIf)
        {
            if (setIf)
                flags |= toSet;
        }

        private static bool GetSaveFlag(SaveFlag flags, SaveFlag toGet)
        {
            return ((flags & toGet) != 0);
        }
    }
}