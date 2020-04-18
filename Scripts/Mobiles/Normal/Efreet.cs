using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an efreet corpse")]
    public class Efreet : BaseCreature
    {
        [Constructable]
        public Efreet()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an efreet";
            Body = 131;
            BaseSoundID = 768;

            SetStr(326, 355);
            SetDex(266, 285);
            SetInt(171, 195);

            SetHits(196, 213);

            SetDamage(11, 13);

            SetDamageType(ResistanceType.Physical, 0);
            SetDamageType(ResistanceType.Fire, 50);
            SetDamageType(ResistanceType.Energy, 50);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 60, 70);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.EvalInt, 60.1, 75.0);
            SetSkill(SkillName.Magery, 60.1, 75.0);
            SetSkill(SkillName.MagicResist, 60.1, 75.0);
            SetSkill(SkillName.Tactics, 60.1, 80.0);
            SetSkill(SkillName.Wrestling, 60.1, 80.0);

            Fame = 10000;
            Karma = -10000;
        }

        public Efreet(Serial serial)
            : base(serial)
        {
        }

        public override int TreasureMapLevel => 4;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Gems);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (0.02 > Utility.RandomDouble())
            {

                switch (Utility.Random(5))
                {
                    case 0:
                        c.DropItem(new DaemonArms());
                        break;
                    case 1:
                        c.DropItem(new DaemonChest());
                        break;
                    case 2:
                        c.DropItem(new DaemonGloves());
                        break;
                    case 3:
                        c.DropItem(new DaemonLegs());
                        break;
                    case 4:
                        c.DropItem(new DaemonHelm());
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