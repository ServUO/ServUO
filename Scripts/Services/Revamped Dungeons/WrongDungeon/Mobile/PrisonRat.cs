using System;

namespace Server.Mobiles
{
    [CorpseName("a rat corpse")]
    public class PrisonRat : BaseCreature
    {
        [Constructable]
        public PrisonRat()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a prison rat";
            this.Body = 0xEE;
            this.Hue = 443;
            this.BaseSoundID = 0xCC;

            this.SetStr(9);
            this.SetDex(35);
            this.SetInt(7, 10);

            this.SetHits(50);
            this.SetStam(25);
            this.SetMana(7, 10);

            this.SetDamage(5, 8);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 5, 10);
            this.SetResistance(ResistanceType.Poison, 15, 25);
            this.SetResistance(ResistanceType.Energy, 5, 10);

            this.SetSkill(SkillName.MagicResist, 3.7, 20.7);
            this.SetSkill(SkillName.Tactics, 6.7, 17.0);
            this.SetSkill(SkillName.Wrestling, 9.1, 19.5);

            this.Fame = 150;
            this.Karma = -150;

            this.VirtualArmor = 6;
        }

        public PrisonRat(Serial serial)
            : base(serial)
        {
        }

        public override int Meat { get { return 1; } }
        public override FoodType FavoriteFood { get { return FoodType.Fish; } }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Poor);
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