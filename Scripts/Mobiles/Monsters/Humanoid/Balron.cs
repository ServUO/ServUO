using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a balron corpse")]
    public class Balron : BaseCreature
    {
        [Constructable]
        public Balron()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = NameList.RandomName("balron");
            this.Body = 40;
            this.BaseSoundID = 357;

            this.SetStr(986, 1185);
            this.SetDex(177, 255);
            this.SetInt(151, 250);

            this.SetHits(592, 711);

            this.SetDamage(22, 29);

            this.SetDamageType(ResistanceType.Physical, 50);
            this.SetDamageType(ResistanceType.Fire, 25);
            this.SetDamageType(ResistanceType.Energy, 25);

            this.SetResistance(ResistanceType.Physical, 65, 80);
            this.SetResistance(ResistanceType.Fire, 60, 80);
            this.SetResistance(ResistanceType.Cold, 50, 60);
            this.SetResistance(ResistanceType.Poison, 100);
            this.SetResistance(ResistanceType.Energy, 40, 50);

            this.SetSkill(SkillName.Anatomy, 25.1, 50.0);
            this.SetSkill(SkillName.EvalInt, 90.1, 100.0);
            this.SetSkill(SkillName.Magery, 95.5, 100.0);
            this.SetSkill(SkillName.Meditation, 25.1, 50.0);
            this.SetSkill(SkillName.MagicResist, 100.5, 150.0);
            this.SetSkill(SkillName.Tactics, 90.1, 100.0);
            this.SetSkill(SkillName.Wrestling, 90.1, 100.0);

            this.Fame = 24000;
            this.Karma = -24000;

            this.VirtualArmor = 90;

            this.PackItem(new Longsword());
        }

        public Balron(Serial serial)
            : base(serial)
        {
        }

        public override bool CanRummageCorpses
        {
            get
            {
                return true;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Deadly;
            }
        }
        public override int TreasureMapLevel
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
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich, 2);
            this.AddLoot(LootPack.Rich);
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