namespace Server.Items
{
    public class MarkOfTravesty : SavageMask
    {
        public override bool IsArtifact => true;
        [Constructable]
        public MarkOfTravesty()
            : base()
        {
            Hue = 0x495;
            Attributes.BonusMana = 8;
            Attributes.RegenHits = 3;
            ClothingAttributes.SelfRepair = 3;

            switch (Utility.Random(15))
            {
                case 0:
                    SkillBonuses.SetValues(0, SkillName.EvalInt, 10);
                    SkillBonuses.SetValues(1, SkillName.Magery, 10);
                    break;
                case 1:
                    SkillBonuses.SetValues(0, SkillName.AnimalLore, 10);
                    SkillBonuses.SetValues(1, SkillName.AnimalTaming, 10);
                    break;
                case 2:
                    SkillBonuses.SetValues(0, SkillName.Swords, 10);
                    SkillBonuses.SetValues(1, SkillName.Tactics, 10);
                    break;
                case 3:
                    SkillBonuses.SetValues(0, SkillName.Discordance, 10);
                    SkillBonuses.SetValues(1, SkillName.Musicianship, 10);
                    break;
                case 4:
                    SkillBonuses.SetValues(0, SkillName.Fencing, 10);
                    SkillBonuses.SetValues(1, SkillName.Tactics, 10);
                    break;
                case 5:
                    SkillBonuses.SetValues(0, SkillName.Chivalry, 10);
                    SkillBonuses.SetValues(1, SkillName.MagicResist, 10);
                    break;
                case 6:
                    SkillBonuses.SetValues(0, SkillName.Anatomy, 10);
                    SkillBonuses.SetValues(1, SkillName.Healing, 10);
                    break;
                case 7:
                    SkillBonuses.SetValues(0, SkillName.Ninjitsu, 10);
                    SkillBonuses.SetValues(1, SkillName.Stealth, 10);
                    break;
                case 8:
                    SkillBonuses.SetValues(0, SkillName.Bushido, 10);
                    SkillBonuses.SetValues(1, SkillName.Parry, 10);
                    break;
                case 9:
                    SkillBonuses.SetValues(0, SkillName.Archery, 10);
                    SkillBonuses.SetValues(1, SkillName.Tactics, 10);
                    break;
                case 10:
                    SkillBonuses.SetValues(0, SkillName.Macing, 10);
                    SkillBonuses.SetValues(1, SkillName.Tactics, 10);
                    break;
                case 11:
                    SkillBonuses.SetValues(0, SkillName.Necromancy, 10);
                    SkillBonuses.SetValues(1, SkillName.SpiritSpeak, 10);
                    break;
                case 12:
                    SkillBonuses.SetValues(0, SkillName.Stealth, 10);
                    SkillBonuses.SetValues(1, SkillName.Stealing, 10);
                    break;
                case 13:
                    SkillBonuses.SetValues(0, SkillName.Peacemaking, 10);
                    SkillBonuses.SetValues(1, SkillName.Musicianship, 10);
                    break;
                case 14:
                    SkillBonuses.SetValues(0, SkillName.Provocation, 10);
                    SkillBonuses.SetValues(1, SkillName.Musicianship, 10);
                    break;
            }
        }

        public MarkOfTravesty(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074493;// Mark of Travesty
        public override int BasePhysicalResistance => 8;
        public override int BaseFireResistance => 5;
        public override int BaseColdResistance => 11;
        public override int BasePoisonResistance => 20;
        public override int BaseEnergyResistance => 15;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}