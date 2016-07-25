using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a wolf spider spider corpse")]
    public class WolfSpider : BaseCreature
    {
        [Constructable]
        public WolfSpider()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a wolf spider";
            this.Body = 736;

            this.SetStr(225, 268);
            this.SetDex(145, 165);
            this.SetInt(285, 310);

            this.SetHits(150, 160);
            this.SetMana(285, 310);
            this.SetStam(145, 165);

            this.SetDamage(15, 18);

            this.SetDamageType(ResistanceType.Physical, 70);
            this.SetDamageType(ResistanceType.Poison, 30);

            this.SetResistance(ResistanceType.Physical, 30, 35);
            this.SetResistance(ResistanceType.Fire, 20, 30);
            this.SetResistance(ResistanceType.Cold, 25, 35);
            this.SetResistance(ResistanceType.Poison, 100);
            this.SetResistance(ResistanceType.Energy, 25, 35);

            this.SetSkill(SkillName.Anatomy, 80.0, 90.0);
            this.SetSkill(SkillName.MagicResist, 60.0, 75.0);
            this.SetSkill(SkillName.Poisoning, 62.3, 77.2);
            this.SetSkill(SkillName.Tactics, 84.1, 95.9);
            this.SetSkill(SkillName.Wrestling, 80.2, 90.0);

            this.Tamable = true;
            this.ControlSlots = 2;
            this.MinTameSkill = 59.1;

            this.Fame = 5000;
            this.Karma = -5000;

            this.QLPoints = 5;
        }

        public WolfSpider(Serial serial)
            : base(serial)
        {
        }

        public override FoodType FavoriteFood { get { return FoodType.Meat; } }
        public override PackInstinct PackInstinct { get { return PackInstinct.Arachnid; } }
        public override Poison PoisonImmune { get { return Poison.Deadly; } }
        public override Poison HitPoison { get { return (0.8 >= Utility.RandomDouble() ? Poison.Greater : Poison.Deadly); } }

        public override void GenerateLoot()
        {
            PackItem(new SpidersSilk(8));
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Gems, 2);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.15)
                c.DropItem(new BottleIchor());

            if (Utility.RandomDouble() < 0.01)
                c.DropItem(new LuckyCoin());
        }

        public override int GetIdleSound()
        {
            return 1605;
        }

        public override int GetAngerSound()
        {
            return 1602;
        }

        public override int GetHurtSound()
        {
            return 1604;
        }

        public override int GetDeathSound()
        {
            return 1603;
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