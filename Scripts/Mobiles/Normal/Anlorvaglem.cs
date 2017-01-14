using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an anlorvaglem corpse")]
    public class Anlorvaglem : BaseCreature
    {
        [Constructable]
        public Anlorvaglem()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.6, 1.2)
        {
            Name = "Anlorvaglem";
            Hue = 2071;
            Body = 152;

            SetStr(1104);
            SetDex(1076);
            SetInt(1107);

            SetHits(3205);

            SetDamage(15, 28);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Fire, 20);
            SetDamageType(ResistanceType.Cold, 20);
            SetDamageType(ResistanceType.Poison, 20);
            SetDamageType(ResistanceType.Energy, 20);

            SetResistance(ResistanceType.Physical, 36, 40);
            SetResistance(ResistanceType.Fire, 40, 43);
            SetResistance(ResistanceType.Cold, 55, 58);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 40, 45);

            SetSkill(SkillName.Wrestling, 58.8, 60);
            SetSkill(SkillName.Tactics, 94.0, 95.0);
            SetSkill(SkillName.MagicResist, 65, 67);
            SetSkill(SkillName.Anatomy, 27, 30);

            Fame = 8000;
            Karma = -8000;

            VirtualArmor = 48;

            QLPoints = 50;

            PackItem(new DaemonBone(30));
        }

        public Anlorvaglem(Serial serial)
            : base(serial)
        {
        }

        public override bool AlwaysMurderer
        {
            get { return true; }
        }

        public override Poison PoisonImmune
        {
            get { return Poison.Lethal; }
        }

        public override bool Unprovokable
        {
            get { return true; }
        }

        public override bool BardImmune
        {
            get { return true; }
        }

        public override bool ReacquireOnMovement
        {
            get { return true; }
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.AosUltraRich, 3);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.3)
                c.DropItem(new VoidOrb());

            if (Utility.RandomDouble() < 0.10)
            {
                switch (Utility.Random(2))
                {
                    case 0:
                        AddToBackpack(new VoidEssence());
                        break;
                    case 1:
                        AddToBackpack(new VoidCore());
                        break;
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            var version = reader.ReadInt();
        }
    }
}