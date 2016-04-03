using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("Pixie [Renowned] corpse")]
    public class PixieRenowned : BaseRenowned
    {
        [Constructable]
        public PixieRenowned()
            : base(AIType.AI_Mage)
        {
            this.Name = "Pixie";
            this.Title = "[Renowned]";
            this.Body = 128;
            this.BaseSoundID = 0x467;

            this.SetStr(-350, 380);
            this.SetDex(450, 600);
            this.SetInt(700, 8500);

            this.SetHits(9100, 9200);
            this.SetStam(450, 600);
            this.SetMana(700, 800);

            this.SetDamage(9, 15);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 70, 90);
            this.SetResistance(ResistanceType.Fire, 60, 70);
            this.SetResistance(ResistanceType.Cold, 70, 80);
            this.SetResistance(ResistanceType.Poison, 60, 70);
            this.SetResistance(ResistanceType.Energy, 60, 70);

            this.SetSkill(SkillName.EvalInt, 100.0, 100.0);
            this.SetSkill(SkillName.Magery, 90.1, 110.0);
            this.SetSkill(SkillName.Meditation, 100.0, 100.0);
            this.SetSkill(SkillName.MagicResist, 110.5, 150.0);
            this.SetSkill(SkillName.Tactics, 100.1, 120.0);
            this.SetSkill(SkillName.Wrestling, 100.1, 120.0);

            this.Fame = 7000;
            this.Karma = 7000;

            this.VirtualArmor = 100;

            this.PackItem(new EssenceFeeling());

            if (0.02 > Utility.RandomDouble())
                this.PackStatue();				
        }

        public PixieRenowned(Serial serial)
            : base(serial)
        {
        }

        public override Type[] UniqueSAList
        {
            get
            {
                return new Type[] { typeof(DemonHuntersStandard), typeof(DragonJadeEarrings) };
            }
        }
        public override Type[] SharedSAList
        {
            get
            {
                return new Type[] { typeof(SwordOfShatteredHopes), typeof(PillarOfStrength) };
            }
        }
        public override bool InitialInnocent
        {
            get
            {
                return true;
            }
        }
        public override HideType HideType
        {
            get
            {
                return HideType.Spined;
            }
        }
        public override int Hides
        {
            get
            {
                return 5;
            }
        }
        public override int Meat
        {
            get
            {
                return 1;
            }
        }
        public override OppositionGroup OppositionGroup
        {
            get
            {
                return OppositionGroup.FeyAndUndead;
            }
        }
        public override void GenerateLoot()
        {
			AddLoot(LootPack.UltraRich, 2);
        }

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
}