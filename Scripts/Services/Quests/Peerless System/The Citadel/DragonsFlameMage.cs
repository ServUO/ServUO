using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a black order mage corpse")]
    public class DragonsFlameMage : BaseCreature
    {
        [Constructable]
        public DragonsFlameMage()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "Black Order Mage";
            this.Title = "of the Dragon's Flame Sect";
            this.Female = Utility.RandomBool();
            this.Race = Race.Human;
            this.Hue = this.Race.RandomSkinHue();
            this.HairItemID = this.Race.RandomHair(this.Female);
            this.HairHue = this.Race.RandomHairHue();
            this.Race.RandomFacialHair(this);

            this.AddItem(new NinjaTabi());
            this.AddItem(new FancyShirt(0x51D));
            this.AddItem(new Hakama(0x51D));
            this.AddItem(new Kasa(0x51D));

            this.SetStr(340, 360);
            this.SetDex(200, 215);
            this.SetInt(400, 415);

            this.SetHits(600, 615);

            this.SetDamage(13, 15);

            this.SetDamageType(ResistanceType.Physical, 10);
            this.SetDamageType(ResistanceType.Fire, 20);
            this.SetDamageType(ResistanceType.Cold, 20);
            this.SetDamageType(ResistanceType.Energy, 50);

            this.SetResistance(ResistanceType.Physical, 40, 50);
            this.SetResistance(ResistanceType.Fire, 30, 50);
            this.SetResistance(ResistanceType.Cold, 55, 60);
            this.SetResistance(ResistanceType.Poison, 50, 60);
            this.SetResistance(ResistanceType.Energy, 60, 70);

            this.SetSkill(SkillName.EvalInt, 70.1, 80.0);
            this.SetSkill(SkillName.Magery, 90.1, 100.0);
            this.SetSkill(SkillName.MagicResist, 85.1, 95.0);
            this.SetSkill(SkillName.Tactics, 70.1, 80.0);
            this.SetSkill(SkillName.Wrestling, 60.1, 80.0);

            this.Fame = 13000;
            this.Karma = -13000;

            this.VirtualArmor = 58;
        }

        public DragonsFlameMage(Serial serial)
            : base(serial)
        {
        }

        public override bool AlwaysMurderer
        {
            get
            {
                return true;
            }
        }
        public override bool ShowFameTitle
        {
            get
            {
                return false;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.AosFilthyRich, 4);
        }

        public override void AlterSpellDamageFrom(Mobile from, ref int damage)
        {
            if (from != null)
                from.Damage(damage / 2, from);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.3)
                c.DropItem(new DragonFlameSectBadge());

            if (Utility.RandomDouble() < 0.1)
                c.DropItem(new ParrotItem());

            if (Utility.RandomDouble() < 0.05)
                c.DropItem(new DragonFlameKey());
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}