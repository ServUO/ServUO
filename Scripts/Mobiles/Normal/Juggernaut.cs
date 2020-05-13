using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a juggernaut corpse")]
    public class Juggernaut : BaseCreature
    {
        [Constructable]
        public Juggernaut()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.3, 0.6)
        {
            Name = "a blackthorn juggernaut";
            Body = 768;

            SetStr(301, 400);
            SetDex(51, 70);
            SetInt(51, 100);

            SetHits(181, 240);

            SetDamage(12, 19);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Fire, 25);
            SetDamageType(ResistanceType.Energy, 25);

            SetResistance(ResistanceType.Physical, 65, 75);
            SetResistance(ResistanceType.Fire, 35, 45);
            SetResistance(ResistanceType.Cold, 35, 45);
            SetResistance(ResistanceType.Poison, 15, 25);
            SetResistance(ResistanceType.Energy, 10, 20);

            SetSkill(SkillName.Anatomy, 90.1, 100.0);
            SetSkill(SkillName.MagicResist, 140.1, 150.0);
            SetSkill(SkillName.Tactics, 90.1, 100.0);
            SetSkill(SkillName.Wrestling, 90.1, 100.0);

            Fame = 12000;
            Karma = -12000;

            SetSpecialAbility(SpecialAbility.ColossalBlow);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Gems);
            AddLoot(LootPack.LootItem<PowerCrystal>(10.0));
            AddLoot(LootPack.LootItem<ClockworkAssembly>(40.0));
            AddLoot(LootPack.LootItemCallback(Golem.SpawnGears, 5.0, 1, false, false));
        }

        public Juggernaut(Serial serial)
            : base(serial)
        {
        }

        public override bool AlwaysMurderer => true;
        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override int Meat => 1;
        public override int TreasureMapLevel => 5;

        public override int GetDeathSound()
        {
            return 0x423;
        }

        public override int GetAttackSound()
        {
            return 0x23B;
        }

        public override int GetHurtSound()
        {
            return 0x140;
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
