using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a betballem corpse")]
    public class Betballem : BaseCreature
    {
        [Constructable]
        public Betballem()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a Betballem";
            Body = 776;
            Hue = 2071;
            BaseSoundID = 357;

            SetStr(16, 40);
            SetDex(31, 60);
            SetInt(11, 25);

            SetHits(5010, 5250);

            SetDamage(21, 25);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 15, 20);
            SetResistance(ResistanceType.Fire, 5, 10);

            SetSkill(SkillName.MagicResist, 10.0);
            SetSkill(SkillName.Tactics, 0.1, 15.0);
            SetSkill(SkillName.Wrestling, 25.1, 40.0);

            Fame = 500;
            Karma = -500;

            VirtualArmor = 38;

            QLPoints = 10;

            AddItem(new LightSource());

            PackItem(new FertileDirt(Utility.RandomMinMax(1, 4)));
            PackItem(new DaemonBone(5)); // TODO: Five small iron ore
        }

        public Betballem(Serial serial)
            : base(serial)
        {
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

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Meager);
            AddLoot(LootPack.Gems);
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
                        AddToBackpack(new AncientPotteryFragments());
                        break;
                }
            }
        }

        public override int GetIdleSound()
        {
            return 338;
        }

        public override int GetAngerSound()
        {
            return 338;
        }

        public override int GetDeathSound()
        {
            return 338;
        }

        public override int GetAttackSound()
        {
            return 406;
        }

        public override int GetHurtSound()
        {
            return 194;
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