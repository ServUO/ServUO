using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a skeletal corpse")]
    public class Skeleton : BaseCreature
    {
        [Constructable]
        public Skeleton()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a skeleton";
            Body = Utility.RandomList(50, 56);
            BaseSoundID = 0x48D;

            SetStr(56, 80);
            SetDex(56, 75);
            SetInt(16, 40);

            SetHits(34, 48);

            SetDamage(3, 7);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 15, 20);
            SetResistance(ResistanceType.Fire, 5, 10);
            SetResistance(ResistanceType.Cold, 25, 40);
            SetResistance(ResistanceType.Poison, 25, 35);
            SetResistance(ResistanceType.Energy, 5, 15);

            SetSkill(SkillName.MagicResist, 45.1, 60.0);
            SetSkill(SkillName.Tactics, 45.1, 60.0);
            SetSkill(SkillName.Wrestling, 45.1, 55.0);

            Fame = 450;
            Karma = -450;
        }

        public Skeleton(Serial serial)
            : base(serial)
        {
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lesser;
        public override TribeType Tribe => TribeType.Undead;

        public override bool IsEnemy(Mobile m)
        {
            if (Region.IsPartOf("Haven Island"))
            {
                return false;
            }

            return base.IsEnemy(m);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Poor);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (!Controlled)
            {
                switch (Utility.Random(5))
                {
                    case 0:
                        c.DropItem(new BoneArms());
                        break;
                    case 1:
                        c.DropItem(new BoneChest());
                        break;
                    case 2:
                        c.DropItem(new BoneGloves());
                        break;
                    case 3:
                        c.DropItem(new BoneLegs());
                        break;
                    case 4:
                        c.DropItem(new BoneHelm());
                        break;
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
