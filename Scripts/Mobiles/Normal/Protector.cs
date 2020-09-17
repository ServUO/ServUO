using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a human corpse")]
    public class Protector : BaseCreature
    {
        [Constructable]
        public Protector()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 401;
            Female = true;
            Hue = Race.Human.RandomSkinHue();
            HairItemID = Race.Human.RandomHair(this);
            HairHue = Race.Human.RandomHairHue();

            Name = "A Protector";
            Title = "The Mystic Llamaherder";

            SetStr(700, 800);
            SetDex(100, 150);
            SetInt(50, 75);

            SetHits(350, 450);

            SetDamage(6, 12);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 30, 40);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 35, 40);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.Wrestling, 70.0, 100.0);
            SetSkill(SkillName.Tactics, 80.0, 100.0);
            SetSkill(SkillName.MagicResist, 50.0, 70.0);
            SetSkill(SkillName.Anatomy, 70.0, 100.0);

            Fame = 10000;
            Karma = -10000;

            Item boots = new ThighBoots
            {
                Movable = false,
                Hue = Utility.Random(2)
            };

            Item shroud = new Item(0x204E)
            {
                Layer = Layer.OuterTorso,
                Movable = false,
                Hue = Utility.Random(2)
            };

            AddItem(boots);
            AddItem(shroud);
        }

        public Protector(Serial serial)
            : base(serial)
        {
        }

        public override bool AlwaysMurderer => true;
        public override bool PropertyTitle => false;
        public override bool ShowFameTitle => false;
        public override void GenerateLoot(bool spawning)
        {
            if (spawning)
                return; // No loot/backpack on spawn

            base.GenerateLoot(true);
            base.GenerateLoot(false);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.4)
                c.DropItem(new ProtectorsEssence());
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}