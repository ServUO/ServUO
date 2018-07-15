using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a black order mage corpse")]
    public class BlackOrderMage : BaseCreature
    {
        [Constructable]
        public BlackOrderMage()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Black Order Mage";
            Title = "of the Dragons's Flame Sect";
            Race = Utility.RandomBool() ? Race.Human : Race.Elf;
            Body = Race == Race.Elf ? 605 : 400;
            Hue = Utility.RandomSkinHue();

            Utility.AssignRandomHair(this);

            if (Utility.RandomBool())
                Utility.AssignRandomFacialHair(this, HairHue);

            SetStr(125, 175);
            SetDex(90, 110);
            SetInt(285, 305);

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
            SetSkill(SkillName.Magery, 100.0, 110.0);
            SetSkill(SkillName.EvalInt, 100.0, 110.0);

            /* Equip */
            Item item = null;

            AddItem(new Waraji());

            item = new FancyShirt();
            item.Hue = 1309;
            AddItem(item);

            item = new Kasa();
            item.Hue = 1309;
            AddItem(item);

            item = new Hakama();
            item.Hue = 1309;
            AddItem(item);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (0.25 > Utility.RandomDouble())
                c.DropItem(new DragonFlameSectBadge());
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
        }

        public override bool AlwaysMurderer { get { return true; } }
        public override bool CanRummageCorpses { get { return true; } }

        public BlackOrderMage(Serial serial)
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
