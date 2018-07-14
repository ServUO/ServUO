using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a black order thief corpse")]
    public class BlackOrderThief : BaseCreature
    {
        [Constructable]
        public BlackOrderThief()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Black Order Thief";
            Title = "of the Tiger's Claw Sect";
            Race = Utility.RandomBool() ? Race.Human : Race.Elf;
            Body = Race == Race.Elf ? 605 : 400;
            Hue = Utility.RandomSkinHue();

            Utility.AssignRandomHair(this);

            if (Utility.RandomBool())
                Utility.AssignRandomFacialHair(this, HairHue);

            SetStr(325, 375);
            SetDex(90, 110);
            SetInt(85, 105);

            SetHits(900, 1100);

            SetDamage(12, 26);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 40, 65);
            SetResistance(ResistanceType.Fire, 50, 70);
            SetResistance(ResistanceType.Cold, 30, 50);
            SetResistance(ResistanceType.Poison, 45, 65);
            SetResistance(ResistanceType.Energy, 45, 65);

            Fame = 10000;
            Karma = -10000;

            SetSkill(SkillName.MagicResist, 80.0, 100.0);
            SetSkill(SkillName.Tactics, 115.0, 130.0);
            SetSkill(SkillName.Wrestling, 95.0, 120.0);
            SetSkill(SkillName.Anatomy, 105.0, 120.0);
            SetSkill(SkillName.Swords, 100.0, 110.0);
            SetSkill(SkillName.Parry, 100.0, 110.0);

            /* Equip */
            AddItem(new Wakizashi());

            Item item = null;

            item = new LeatherNinjaPants();
            item.Hue = 1309;
            AddItem(item);

            item = new FancyShirt();
            item.Hue = 1309;
            AddItem(item);

            item = new StuddedGloves();
            item.Hue = 105;
            AddItem(item);

            item = new JinBaori();
            item.Hue = 105;
            AddItem(item);

            item = new LightPlateJingasa();
            item.Hue = 1309;
            AddItem(item);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (0.25 > Utility.RandomDouble())
                c.DropItem(new TigerClawSectBadge());
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
        }

        public override bool AlwaysMurderer { get { return true; } }
        public override bool CanRummageCorpses { get { return true; } }

        public BlackOrderThief(Serial serial)
            : base(serial)
        {
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
