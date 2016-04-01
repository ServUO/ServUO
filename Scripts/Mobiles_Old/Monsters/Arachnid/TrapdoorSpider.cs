using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a trapdoor spider corpse")]
    public class TrapdoorSpider : BaseCreature
    {
        [Constructable]
        public TrapdoorSpider() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a trapdoor spider";
            Body = 737;

            SetStr(100, 104);
            SetDex(162, 165);
            SetInt(29, 50);

            SetHits(125, 144);

            SetDamage(15, 18);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Poison, 80);

            SetResistance(ResistanceType.Physical, 0);
            SetResistance(ResistanceType.Fire, 30, 35);
            SetResistance(ResistanceType.Cold, 30, 35);
            SetResistance(ResistanceType.Poison, 40, 45);
            SetResistance(ResistanceType.Energy, 95, 100);

            SetSkill(SkillName.Anatomy, 2.0, 3.8);
            SetSkill(SkillName.MagicResist, 47.5, 57.9);
            SetSkill(SkillName.Poisoning, 70.5, 73.5);
            SetSkill(SkillName.Tactics, 73.3, 78.9);
            SetSkill(SkillName.Wrestling, 92.5, 94.6);
            SetSkill(SkillName.Hiding, 110.3, 119.9);
            SetSkill(SkillName.Stealth, 110.5, 119.6);

            QLPoints = 5;
        }

        public TrapdoorSpider(Serial serial) : base(serial)
        {
        }

        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.ShadowStrike;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.12)
                c.DropItem(new BottleIchor());

            if (Utility.RandomDouble() < 0.05)
            {
                switch (Utility.Random(2))
                {
                    case 0:
                        c.DropItem(new SpiderCarapace());
                        break;
                    case 1:
                        c.DropItem(new TatteredAncientScroll());
                        break;
                }
            }
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
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            var version = reader.ReadInt();
        }
    }
}