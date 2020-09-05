using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a minotaur corpse")]
    public class MinotaurScout : BaseCreature
    {
        [Constructable]
        public MinotaurScout()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)// NEED TO CHECK
        {
            Name = "a minotaur scout";
            Body = 281;

            SetStr(353, 375);
            SetDex(111, 130);
            SetInt(34, 50);
            BaseSoundID = 1270;

            SetHits(354, 383);

            SetDamage(11, 20);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 25, 35);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.Anatomy, 0);
            SetSkill(SkillName.MagicResist, 60.6, 67.5);
            SetSkill(SkillName.Tactics, 86.9, 103.6);
            SetSkill(SkillName.Wrestling, 85.6, 104.5);

            Fame = 5000;
            Karma = -5000;

            SetWeaponAbility(WeaponAbility.ParalyzingBlow);
        }

        public MinotaurScout(Serial serial)
            : base(serial)
        {
        }

        public override int TreasureMapLevel => 3;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.ArcanistScrolls, 0, 1);
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
