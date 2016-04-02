using Server.Ethics;
using Server.Factions;
using Server.Items;
using Server.Services;

namespace Server.Mobiles
{
    [CorpseName("a daemon corpse")]
    public class Daemon : BaseCreature
    {
        [Constructable]
        public Daemon()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = NameList.RandomName("daemon");
            Body = 9;
            BaseSoundID = 357;

            SetStr(476, 505);
            SetDex(76, 95);
            SetInt(301, 325);

            SetHits(286, 303);

            SetDamage(7, 14);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 45, 60);
            SetResistance(ResistanceType.Fire, 50, 60);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 20, 30);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.EvalInt, 70.1, 80.0);
            SetSkill(SkillName.Magery, 70.1, 80.0);
            SetSkill(SkillName.MagicResist, 85.1, 95.0);
            SetSkill(SkillName.Tactics, 70.1, 80.0);
            SetSkill(SkillName.Wrestling, 60.1, 80.0);

            Fame = 15000;
            Karma = -15000;

            VirtualArmor = 58;

            QLPoints = 5;

            switch (Utility.Random(20))
            {
                case 0:
                    PackItem(new LichFormScroll());
                    break;
                case 1:
                    PackItem(new PoisonStrikeScroll());
                    break;
                case 2:
                    PackItem(new StrangleScroll());
                    break;
                case 3:
                    PackItem(new VengefulSpiritScroll());
                    break;
                case 4:
                    PackItem(new WitherScroll());
                    break;
            }


            ControlSlots = Core.SE ? 4 : 5;
        }

        public Daemon(Serial serial)
            : base(serial)
        {
        }

        public override double DispelDifficulty
        {
            get { return 125.0; }
        }

        public override double DispelFocus
        {
            get { return 45.0; }
        }

        public override Faction FactionAllegiance
        {
            get { return Shadowlords.Instance; }
        }

        public override Ethic EthicAllegiance
        {
            get { return Ethic.Evil; }
        }

        public override bool CanRummageCorpses
        {
            get { return true; }
        }

        public override Poison PoisonImmune
        {
            get { return Poison.Regular; }
        }

        public override int TreasureMapLevel
        {
            get { return 4; }
        }

        public override int Meat
        {
            get { return 1; }
        }

        public override bool CanFly
        {
            get { return true; }
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Average, 2);
            AddLoot(LootPack.MedScrolls, 2);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
            SARegionDrops.GetSADrop(c);
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