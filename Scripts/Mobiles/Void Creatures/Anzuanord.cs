using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an anzuanord corpse")]
    public class Anzuanord : BaseVoidCreature
    {
        public override VoidEvolution Evolution { get { return VoidEvolution.Survival; } }
        public override int Stage { get { return 1; } }

        [Constructable]
        public Anzuanord()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "an Anzuanord";
            this.Body = 74;
            this.Hue = 2071;
            this.BaseSoundID = 422;

            this.SetStr(705);
            this.SetDex(900, 910);
            this.SetInt(900, 1000);

            this.SetHits(180);

            this.SetDamage(8, 10);

            this.SetDamageType(ResistanceType.Physical, 20);
            this.SetDamageType(ResistanceType.Fire, 20);
            this.SetDamageType(ResistanceType.Cold, 20);
            this.SetDamageType(ResistanceType.Poison, 20);
            this.SetDamageType(ResistanceType.Energy, 20);

            this.SetResistance(ResistanceType.Physical, 0, 10);
            this.SetResistance(ResistanceType.Poison, 15, 20);
            this.SetResistance(ResistanceType.Physical, 15, 10);
            this.SetResistance(ResistanceType.Poison, 0, 20);
            this.SetResistance(ResistanceType.Physical, 100);

            this.SetSkill(SkillName.Anatomy, 5.0, 10.0);
            this.SetSkill(SkillName.MagicResist, 40.0, 50.0);
            this.SetSkill(SkillName.Tactics, 40.0, 50.0);
            this.SetSkill(SkillName.Wrestling, 40.0, 50.0);
            this.SetSkill(SkillName.Magery, 70.0, 80.0);
            this.SetSkill(SkillName.EvalInt, 80.0, 90.0);
            this.SetSkill(SkillName.Meditation, 50.0, 60.0);

            this.Fame = 2500;
            this.Karma = -2500;

            this.PackItem(new DaemonBone(5));

            this.VirtualArmor = 50;
        }

        public Anzuanord(Serial serial)
            : base(serial)
        {
        }

        public override bool BardImmune
        {
            get
            {
                return true;
            }
        }
        public override int Meat
        {
            get
            {
                return 1;
            }
        }
        public override int Hides
        {
            get
            {
                return 7;
            }
        }
        public override HideType HideType
        {
            get
            {
                return HideType.Spined;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.Meat;
            }
        }
        public override PackInstinct PackInstinct
        {
            get
            {
                return PackInstinct.Daemon;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Meager);
            this.AddLoot(LootPack.MedScrolls, 2);
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