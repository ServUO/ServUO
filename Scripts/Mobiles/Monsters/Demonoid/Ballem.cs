using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a Ballem corpse")]
    public class Ballem : BaseCreature
    {
        [Constructable]
        public Ballem()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a Ballem";
            Body = 304;
            Hue = 2071;
            BaseSoundID = 684;

            SetStr(376, 300);
            SetDex(151, 175);
            SetInt(46, 70);

            SetHits(1106, 1120);

            SetDamage(18, 22);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 25, 35);
            SetResistance(ResistanceType.Cold, 15, 25);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 50.1, 75.0);
            SetSkill(SkillName.Tactics, 55.1, 80.0);
            SetSkill(SkillName.Wrestling, 60.1, 70.0);

            Fame = 1000;
            Karma = -1800;

            VirtualArmor = 54;

            PackItem(new DaemonBone(15));

            QLPoints = 20;
        }

        public Ballem(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune
        {
            get { return Poison.Lethal; }
        }

        public override bool Unprovokable
        {
            get { return true; }
        }

        public override bool AlwaysMurderer
        {
            get { return true; }
        }

        public override bool BardImmune
        {
            get { return true; }
        }

        public override bool CanRummageCorpses
        {
            get { return true; }
        }

        public override bool BleedImmune
        {
            get { return true; }
        }

        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.CrushingBlow;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Average);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.5)
                c.DropItem(new VoidOrb());

            if (Utility.RandomDouble() < 0.10)
            {
                switch (Utility.Random(2))
                {
                    case 0:
                        AddToBackpack(new VoidEssence());
                        break;
                    case 1:
                        AddToBackpack(new AncientPotteryFragments());
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
            var version = reader.ReadInt();
        }
    }
}