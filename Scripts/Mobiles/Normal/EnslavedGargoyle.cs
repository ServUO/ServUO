using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an enslaved gargoyle corpse")]
    public class EnslavedGargoyle : BaseCreature
    {
        [Constructable]
        public EnslavedGargoyle()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "an enslaved gargoyle";
            this.Body = 0x2F1;
            this.BaseSoundID = 0x174;

            this.SetStr(302, 360);
            this.SetDex(76, 95);
            this.SetInt(81, 105);

            this.SetHits(186, 212);

            this.SetDamage(7, 14);

            this.SetResistance(ResistanceType.Physical, 30, 40);
            this.SetResistance(ResistanceType.Fire, 50, 70);
            this.SetResistance(ResistanceType.Cold, 15, 25);
            this.SetResistance(ResistanceType.Poison, 25, 30);
            this.SetResistance(ResistanceType.Energy, 25, 30);

            this.SetSkill(SkillName.MagicResist, 70.1, 85.0);
            this.SetSkill(SkillName.Tactics, 50.1, 70.0);
            this.SetSkill(SkillName.Wrestling, 40.1, 80.0);

            this.Fame = 3500;
            this.Karma = 0;

            this.VirtualArmor = 35;

            if (0.2 > Utility.RandomDouble())
                this.PackItem(new GargoylesPickaxe());
        }

        public EnslavedGargoyle(Serial serial)
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
        public override int TreasureMapLevel
        {
            get
            {
                return 1;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Average, 2);
            this.AddLoot(LootPack.Gems);
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