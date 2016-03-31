using System;

namespace Server.Items
{
    [Flipable]
    public class LeafGloves : BaseArmor, IArcaneEquip
    {
        public override Race RequiredRace
        {
            get
            {
                return Race.Elf;
            }
        }
        public override int BasePhysicalResistance
        {
            get
            {
                return 2;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 3;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 2;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 4;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 4;
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
                return 40;
            }
        }

        public override int AosStrReq
        {
            get
            {
                return 10;
            }
        }
        public override int OldStrReq
        {
            get
            {
                return 10;
            }
        }

        public override int ArmorBase
        {
            get
            {
                return 13;
            }
        }

        public override ArmorMaterialType MaterialType
        {
            get
            {
                return ArmorMaterialType.Leather;
            }
        }
        public override CraftResource DefaultResource
        {
            get
            {
                return CraftResource.RegularLeather;
            }
        }

        public override ArmorMeditationAllowance DefMedAllowance
        {
            get
            {
                return ArmorMeditationAllowance.All;
            }
        }

        [Constructable]
        public LeafGloves()
            : base(0x2FC6)
        {
            this.Weight = 2.0;
        }

        public LeafGloves(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            if (this.IsArcane)
            {
                writer.Write(true);
                writer.Write((int)this.m_CurArcaneCharges);
                writer.Write((int)this.m_MaxArcaneCharges);
            }
            else
            {
                writer.Write(false);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            switch ( version )
            {
                case 0:
                    {
                        if (reader.ReadBool())
                        {
                            this.m_CurArcaneCharges = reader.ReadInt();
                            this.m_MaxArcaneCharges = reader.ReadInt();

                            if (this.Hue == 2118)
                                this.Hue = ArcaneGem.DefaultArcaneHue;
                        }

                        break;
                    }
            }
        }

        #region Arcane Impl
        private int m_MaxArcaneCharges, m_CurArcaneCharges;

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxArcaneCharges
        {
            get
            {
                return this.m_MaxArcaneCharges;
            }
            set
            {
                this.m_MaxArcaneCharges = value;
                this.InvalidateProperties();
                this.Update();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CurArcaneCharges
        {
            get
            {
                return this.m_CurArcaneCharges;
            }
            set
            {
                this.m_CurArcaneCharges = value;
                this.InvalidateProperties();
                this.Update();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsArcane
        {
            get
            {
                return (this.m_MaxArcaneCharges > 0 && this.m_CurArcaneCharges >= 0);
            }
        }

        public void Update()
        {
            if (this.IsArcane)
                this.ItemID = 0x26B0; // TODO: Check
            else if (this.ItemID == 0x26B0)
                this.ItemID = 0x2FC6;

            if (this.IsArcane && this.CurArcaneCharges == 0)
                this.Hue = 0;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (this.IsArcane)
                list.Add(1061837, "{0}\t{1}", this.m_CurArcaneCharges, this.m_MaxArcaneCharges); // arcane charges: ~1_val~ / ~2_val~
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            if (this.IsArcane)
                this.LabelTo(from, 1061837, String.Format("{0}\t{1}", this.m_CurArcaneCharges, this.m_MaxArcaneCharges));
        }

        public void Flip()
        {
            if (this.ItemID == 0x2FC6)
                this.ItemID = 0x317C;
            else if (this.ItemID == 0x317C)
                this.ItemID = 0x2FC6;
        }
        #endregion
    }
}