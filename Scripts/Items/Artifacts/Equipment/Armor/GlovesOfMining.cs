using System;

namespace Server.Items
{
    [FlipableAttribute(0x13c6, 0x13ce)]
    public class LeatherGlovesOfMining : BaseGlovesOfMining
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public LeatherGlovesOfMining(int bonus)
            : base(bonus, 0x13C6)
        {
            this.Weight = 1;
        }

        public LeatherGlovesOfMining(Serial serial)
            : base(serial)
        {
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
                return 4;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 3;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 3;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 3;
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
                return 20;
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
        public override int LabelNumber
        {
            get
            {
                return 1045122;
            }
        }// leather blacksmith gloves of mining
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [FlipableAttribute(0x13d5, 0x13dd)]
    public class StuddedGlovesOfMining : BaseGlovesOfMining
    {
        [Constructable]
        public StuddedGlovesOfMining(int bonus)
            : base(bonus, 0x13D5)
        {
            this.Weight = 2;
        }

        public StuddedGlovesOfMining(Serial serial)
            : base(serial)
        {
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
                return 4;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 3;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 3;
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
                return 35;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 45;
            }
        }
        public override int AosStrReq
        {
            get
            {
                return 25;
            }
        }
        public override int OldStrReq
        {
            get
            {
                return 25;
            }
        }
        public override int ArmorBase
        {
            get
            {
                return 16;
            }
        }
        public override ArmorMaterialType MaterialType
        {
            get
            {
                return ArmorMaterialType.Studded;
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
                return 1045123;
            }
        }// studded leather blacksmith gloves of mining
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [FlipableAttribute(0x13eb, 0x13f2)]
    public class RingmailGlovesOfMining : BaseGlovesOfMining
    {
        [Constructable]
        public RingmailGlovesOfMining(int bonus)
            : base(bonus, 0x13EB)
        {
            this.Weight = 1;
        }

        public RingmailGlovesOfMining(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance
        {
            get
            {
                return 3;
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
                return 1;
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
                return 3;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 40;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 50;
            }
        }
        public override int AosStrReq
        {
            get
            {
                return 40;
            }
        }
        public override int OldStrReq
        {
            get
            {
                return 20;
            }
        }
        public override int OldDexBonus
        {
            get
            {
                return -1;
            }
        }
        public override int ArmorBase
        {
            get
            {
                return 22;
            }
        }
        public override ArmorMaterialType MaterialType
        {
            get
            {
                return ArmorMaterialType.Ringmail;
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1045124;
            }
        }// ringmail blacksmith gloves of mining
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public abstract class BaseGlovesOfMining : BaseArmor
    {
        private int m_Bonus;
        private SkillMod m_SkillMod;
        public BaseGlovesOfMining(int bonus, int itemID)
            : base(itemID)
        {
            this.m_Bonus = bonus;

            this.Hue = CraftResources.GetHue((CraftResource)Utility.RandomMinMax((int)CraftResource.DullCopper, (int)CraftResource.Valorite));
        }

        public BaseGlovesOfMining(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Bonus
        {
            get
            {
                return this.m_Bonus;
            }
            set
            {
                this.m_Bonus = value;
                this.InvalidateProperties();

                if (this.m_Bonus == 0)
                {
                    if (this.m_SkillMod != null)
                        this.m_SkillMod.Remove();

                    this.m_SkillMod = null;
                }
                else if (this.m_SkillMod == null && this.Parent is Mobile)
                {
                    this.m_SkillMod = new DefaultSkillMod(SkillName.Mining, true, this.m_Bonus);
                    ((Mobile)this.Parent).AddSkillMod(this.m_SkillMod);
                }
                else if (this.m_SkillMod != null)
                {
                    this.m_SkillMod.Value = this.m_Bonus;
                }
            }
        }
        public override void OnAdded(object parent)
        {
            base.OnAdded(parent);

            if (this.m_Bonus != 0 && parent is Mobile)
            {
                if (this.m_SkillMod != null)
                    this.m_SkillMod.Remove();

                this.m_SkillMod = new DefaultSkillMod(SkillName.Mining, true, this.m_Bonus);
                ((Mobile)parent).AddSkillMod(this.m_SkillMod);
            }
        }

        public override void OnRemoved(object parent)
        {
            base.OnRemoved(parent);

            if (this.m_SkillMod != null)
                this.m_SkillMod.Remove();

            this.m_SkillMod = null;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (this.m_Bonus != 0)
                list.Add(1062005, this.m_Bonus.ToString()); // mining bonus +~1_val~
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((int)this.m_Bonus);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_Bonus = reader.ReadInt();
                        break;
                    }
            }

            if (this.m_Bonus != 0 && this.Parent is Mobile)
            {
                if (this.m_SkillMod != null)
                    this.m_SkillMod.Remove();

                this.m_SkillMod = new DefaultSkillMod(SkillName.Mining, true, this.m_Bonus);
                ((Mobile)this.Parent).AddSkillMod(this.m_SkillMod);
            }
        }
    }
}