using Server.Items;

namespace Server.Mobiles
{
    public class SoulboundPirateCaptain : BaseCreature
    {
        public override bool ClickTitle => false;
        public override bool AlwaysMurderer => true;

        public SoulboundPirateCaptain()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a soulbound pirate captain";
            Body = 0x190;
            Hue = Utility.RandomSkinHue();
            Utility.AssignRandomHair(this);

            SetStr(150, 200);
            SetDex(150);
            SetInt(95, 110);

            SetHits(450, 600);

            SetDamage(20, 28);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 45, 55);
            SetResistance(ResistanceType.Cold, 45, 55);
            SetResistance(ResistanceType.Poison, 45, 55);
            SetResistance(ResistanceType.Energy, 45, 55);

            SetSkill(SkillName.MagicResist, 100.0, 120.0);
            SetSkill(SkillName.Swords, 110.0, 120.0);
            SetSkill(SkillName.Tactics, 110.0, 120.0);
            SetSkill(SkillName.Anatomy, 110.0, 120.0);

            Fame = 8000;
            Karma = -8000;

            AddItem(new TricorneHat(1));
            AddItem(new LeatherArms());
            AddItem(new FancyShirt(1));
            AddItem(new ShortPants(1));
            AddItem(new Cutlass());
            AddItem(new Boots(Utility.RandomNeutralHue()));
            AddItem(new GoldEarrings());

        }

        public override bool OnBeforeDeath()
        {
            if (Region.IsPartOf<Regions.CorgulRegion>())
            {
                CorgulTheSoulBinder.CheckDropSOT(this);
            }

            return base.OnBeforeDeath();
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 3);
        }

        public SoulboundPirateCaptain(Serial serial)
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
