using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a reapers corpse")]
    public class Reaper : BaseCreature
    {
        [Constructable]
        public Reaper()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a reaper";
            this.Body = 47;
            this.BaseSoundID = 442;

            this.SetStr(66, 215);
            this.SetDex(66, 75);
            this.SetInt(101, 250);

            this.SetHits(40, 129);
            this.SetStam(0);

            this.SetDamage(9, 11);

            this.SetDamageType(ResistanceType.Physical, 80);
            this.SetDamageType(ResistanceType.Poison, 20);

            this.SetResistance(ResistanceType.Physical, 35, 45);
            this.SetResistance(ResistanceType.Fire, 15, 25);
            this.SetResistance(ResistanceType.Cold, 10, 20);
            this.SetResistance(ResistanceType.Poison, 40, 50);
            this.SetResistance(ResistanceType.Energy, 30, 40);

            this.SetSkill(SkillName.EvalInt, 90.1, 100.0);
            this.SetSkill(SkillName.Magery, 90.1, 100.0);
            this.SetSkill(SkillName.MagicResist, 100.1, 125.0);
            this.SetSkill(SkillName.Tactics, 45.1, 60.0);
            this.SetSkill(SkillName.Wrestling, 50.1, 60.0);

            this.Fame = 3500;
            this.Karma = -3500;

            this.VirtualArmor = 40;

            this.PackItem(new Log(10));
            this.PackItem(new MandrakeRoot(5));
        }

        public Reaper(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Greater;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 2;
            }
        }
        public override bool DisallowAllMoves
        {
            get
            {
                return true;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Average);
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