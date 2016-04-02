/* Based on Gargoyle, still no infos on Undead Gargoyle... Have to get also the correct body ID */
using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an undead gargoyle corpse")]
    public class UndeadGargoyle : BaseCreature
    {
        [Constructable]
        public UndeadGargoyle()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "an Undead Gargoyle";
            this.Body = 722;
            this.BaseSoundID = 372;

            this.SetStr(250, 350);
            this.SetDex(120, 140);
            this.SetInt(250, 350);

            this.SetHits(200, 300);

            this.SetDamage(15, 27);

            this.SetDamageType(ResistanceType.Physical, 10);
            this.SetDamageType(ResistanceType.Cold, 50);
            this.SetDamageType(ResistanceType.Energy, 40);

            this.SetResistance(ResistanceType.Physical, 45, 55);
            this.SetResistance(ResistanceType.Fire, 30, 40);
            this.SetResistance(ResistanceType.Cold, 40, 55);
            this.SetResistance(ResistanceType.Poison, 55, 65);
            this.SetResistance(ResistanceType.Energy, 40, 50);

            this.SetSkill(SkillName.EvalInt, 90.1, 110.0);
            this.SetSkill(SkillName.Magery, 120);
            this.SetSkill(SkillName.MagicResist, 100.1, 120.0);
            this.SetSkill(SkillName.Tactics, 60.1, 70.0);
            this.SetSkill(SkillName.Wrestling, 60.1, 70.0);

            this.Fame = 3500;
            this.Karma = -3500;

            this.VirtualArmor = 32;

            if (0.025 > Utility.RandomDouble())
                this.PackItem(new GargoylesPickaxe());
        }

        public UndeadGargoyle(Serial serial)
            : base(serial)
        {
        }

        public override int TreasureMapLevel
        {
            get
            {
                return 1;
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
            this.AddLoot(LootPack.Average);
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