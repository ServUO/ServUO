using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an effete putrid gargoyle corpse")]
    public class EffetePutridGargoyle : BaseCreature
    {
        [Constructable]
        public EffetePutridGargoyle()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "an Effete putrid gargoyle";
            this.Body = 4;
            this.BaseSoundID = 372;

            this.SetStr(215, 220);
            this.SetDex(90, 95);
            this.SetInt(40, 45);

            this.SetHits(110, 111);

            this.SetDamage(8, 18);

            this.SetDamageType(ResistanceType.Physical, 60);
            this.SetDamageType(ResistanceType.Cold, 40);

            this.SetResistance(ResistanceType.Physical, 30, 35);
            this.SetResistance(ResistanceType.Fire, 25, 35);
            this.SetResistance(ResistanceType.Cold, 5, 10);
            this.SetResistance(ResistanceType.Poison, 15, 25);

            this.SetSkill(SkillName.Anatomy, 6.0, 8.0);
            this.SetSkill(SkillName.MagicResist, 60.5, 65);
            this.SetSkill(SkillName.Tactics, 65.7, 66);
            this.SetSkill(SkillName.Wrestling, 69.6, 70.0);

            this.Fame = 3500;
            this.Karma = -3500;

            this.VirtualArmor = 32;

            if (0.25 > Utility.RandomDouble())
                this.PackItem(new UndyingFlesh());
        }

        public EffetePutridGargoyle(Serial serial)
            : base(serial)
        {
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
            this.AddLoot(LootPack.Average, 2);
            this.AddLoot(LootPack.MedScrolls);
            this.AddLoot(LootPack.Gems, Utility.RandomMinMax(1, 4));
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