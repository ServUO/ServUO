using Server.Items;

namespace Server.Mobiles
{
    public class SoulboundSwashbuckler : BaseCreature
    {
        public override bool ClickTitle => false;
        public override bool AlwaysMurderer => true;

        [Constructable]
        public SoulboundSwashbuckler()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a soulbound swashbuckler";
            Body = 0x190;
            Hue = Utility.RandomSkinHue();
            Utility.AssignRandomHair(this);

            SetStr(120, 130);
            SetDex(105, 115);
            SetInt(95, 110);

            SetHits(100, 125);

            SetDamage(12, 18);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 30, 40);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 25.0, 47.5);
            SetSkill(SkillName.Swords, 65.0, 87.5);
            SetSkill(SkillName.Tactics, 65.0, 87.5);
            SetSkill(SkillName.Anatomy, 65.0, 87.5);

            Fame = 2000;
            Karma = -2000;

            AddItem(new Bandana());
            AddItem(new LeatherArms());
            AddItem(new FancyShirt());
            AddItem(new ShortPants());
            AddItem(new Cutlass());
            AddItem(new Boots(Utility.RandomNeutralHue()));
            AddItem(new SilverEarrings());

        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 1);
        }

        public override bool OnBeforeDeath()
        {
            if (Region.IsPartOf<Regions.CorgulRegion>())
            {
                CorgulTheSoulBinder.CheckDropSOT(this);
            }

            return base.OnBeforeDeath();
        }

        public SoulboundSwashbuckler(Serial serial)
            : base(serial)
        {
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
