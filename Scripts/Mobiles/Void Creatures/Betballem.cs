using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a betballem corpse")]
    public class Betballem : BaseVoidCreature
    {
        public override VoidEvolution Evolution => VoidEvolution.Killing;
        public override int Stage => 1;

        [Constructable]
        public Betballem()
            : base(AIType.AI_Melee, 10, 1, 0.2, 0.4)
        {
            Name = "betballem";
            Body = 776;
            Hue = 2071;
            BaseSoundID = 357;

            SetStr(270);
            SetDex(890);
            SetInt(80);

            SetHits(90, 100);
            SetDamage(5, 10);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Fire, 20);
            SetDamageType(ResistanceType.Cold, 20);
            SetDamageType(ResistanceType.Poison, 20);
            SetDamageType(ResistanceType.Energy, 20);

            SetResistance(ResistanceType.Physical, 30, 40);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Fire, 100);

            SetSkill(SkillName.MagicResist, 40.0, 50.0);
            SetSkill(SkillName.Tactics, 20.1, 30.0);
            SetSkill(SkillName.Wrestling, 30.1, 40.0);
            SetSkill(SkillName.Anatomy, 0.0, 10.0);

            Fame = 500;
            Karma = -500;

            AddItem(new LightSource());
        }

        public Betballem(Serial serial)
            : base(serial)
        {
        }

        public override bool Unprovokable => true;

        public override bool BardImmune => true;

        public override bool CanRummageCorpses => true;

        public override bool BleedImmune => true;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Meager);
            AddLoot(LootPack.Gems);
            AddLoot(LootPack.LootItem<DaemonBone>(5, true));
            AddLoot(LootPack.LootItem<FertileDirt>(1, 4, true));
            AddLoot(LootPack.LootItem<AncientPotteryFragments>(10.0));
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
            reader.ReadInt();
        }
    }
}
