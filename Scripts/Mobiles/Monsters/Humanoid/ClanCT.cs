using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a clan scratch tinkerer corpse")]
    public class ClanCT : BaseCreature
    {
        //public override InhumanSpeech SpeechType{ get{ return InhumanSpeech.Ratman; } }
        [Constructable]
        public ClanCT()
            : base(AIType.AI_Archer, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "Clan Scratch Tinkerer";
            this.Body = 0x8E;
            this.BaseSoundID = 437;

            this.SetStr(300, 330);
            this.SetDex(220, 240);
            this.SetInt(240, 275);

            this.SetHits(2025, 2068);

            this.SetDamage(4, 10);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 20, 30);
            this.SetResistance(ResistanceType.Fire, 20, 30);
            this.SetResistance(ResistanceType.Cold, 35, 50);
            this.SetResistance(ResistanceType.Poison, 10, 20);
            this.SetResistance(ResistanceType.Energy, 10, 20);

            this.SetSkill(SkillName.Anatomy, 62.5, 82.6);
            this.SetSkill(SkillName.Archery, 80.1, 90.0);
            this.SetSkill(SkillName.MagicResist, 76.8, 99.3);
            this.SetSkill(SkillName.Tactics, 64.2, 84.4);
            this.SetSkill(SkillName.Wrestling, 62.8, 85.0);

            this.Fame = 6500;
            this.Karma = -6500;

            this.VirtualArmor = 56;

            this.AddItem(new Bow());
            this.PackItem(new Arrow(Utility.RandomMinMax(50, 70)));
        }

        public ClanCT(Serial serial)
            : base(serial)
        {
        }

        public override bool CanRummageCorpses
        {
            get
            {
                return true;
            }
        }
        public override int Hides
        {
            get
            {
                return 8;
            }
        }
        public override HideType HideType
        {
            get
            {
                return HideType.Spined;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich, 2);
        }

        public override void OnDeath(Container c)
        {

            base.OnDeath(c);
            Region reg = Region.Find(c.GetWorldLocation(), c.Map);
            if (0.25 > Utility.RandomDouble() && reg.Name == "Cavern of the Discarded")
            {
                switch (Utility.Random(10))
                {
                    case 0: c.DropItem(new AbyssalCloth()); break;
                    case 1: c.DropItem(new PowderedIron()); break;
                    case 2: c.DropItem(new CrystallineBlackrock()); break;
                    case 3: c.DropItem(new EssenceBalance()); break;
                    case 4: c.DropItem(new CrystalShards()); break;
                    case 5: c.DropItem(new ArcanicRuneStone()); break;
                    case 6: c.DropItem(new DelicateScales()); break;
                    case 7: c.DropItem(new SeedRenewal()); break;
                    case 8: c.DropItem(new CrushedGlass()); break;
                    case 9: c.DropItem(new ElvenFletchings()); break;
                }
            }
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

            if (this.Body == 42)
            {
                this.Body = 0x8E;
                this.Hue = 0;
            }
        }
    }
}