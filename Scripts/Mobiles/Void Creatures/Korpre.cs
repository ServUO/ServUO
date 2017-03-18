using System;
using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    [CorpseName("a korpre corpse")]
    public class Korpre : BaseVoidCreature
    {
        public override VoidEvolution Evolution { get { return VoidEvolution.None; } }
        public override int Stage { get { return 0; } }

        [Constructable]
        public Korpre()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "Korpre";
            this.Body = 51;
            this.BaseSoundID = 456;

            this.Hue = 2071;

            this.SetStr(22, 34);
            this.SetDex(16, 21);
            this.SetInt(16, 20);

            this.SetHits(50, 60);

            this.SetDamage(1, 5);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 5, 10);
            this.SetResistance(ResistanceType.Poison, 15, 20);

            this.SetSkill(SkillName.Poisoning, 36.0, 49.1);
            this.SetSkill(SkillName.Anatomy, 0);
            this.SetSkill(SkillName.MagicResist, 15.9, 18.9);
            this.SetSkill(SkillName.Tactics, 24.6, 26.1);
            this.SetSkill(SkillName.Wrestling, 24.9, 26.1);

            this.Fame = 300;
            this.Karma = -300;

            this.VirtualArmor = 8;
        }

        public Korpre(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Regular;
            }
        }
        public override Poison HitPoison
        {
            get
            {
                return Poison.Regular;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.Fish;
            }
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Poor);
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