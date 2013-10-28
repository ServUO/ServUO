using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a plant corpse")]
    public class Bogling : BaseCreature
    {
        [Constructable]
        public Bogling()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a bogling";
            this.Body = 779;
            this.BaseSoundID = 422;

            this.SetStr(96, 120);
            this.SetDex(91, 115);
            this.SetInt(21, 45);

            this.SetHits(58, 72);

            this.SetDamage(5, 7);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 20, 25);
            this.SetResistance(ResistanceType.Fire, 10, 20);
            this.SetResistance(ResistanceType.Cold, 15, 25);
            this.SetResistance(ResistanceType.Poison, 15, 25);
            this.SetResistance(ResistanceType.Energy, 15, 25);

            this.SetSkill(SkillName.MagicResist, 75.1, 100.0);
            this.SetSkill(SkillName.Tactics, 55.1, 80.0);
            this.SetSkill(SkillName.Wrestling, 55.1, 75.0);

            this.Fame = 450;
            this.Karma = -450;

            this.VirtualArmor = 28;

            this.PackItem(new Log(4));
            this.PackItem(new Engines.Plants.Seed());
        }

        public Bogling(Serial serial)
            : base(serial)
        {
        }

        public override int Hides
        {
            get
            {
                return 6;
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
            this.AddLoot(LootPack.Meager);
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