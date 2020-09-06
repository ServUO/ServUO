using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a minotaur corpse")]
    public class Minotaur : BaseCreature
    {
        [Constructable]
        public Minotaur()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)// NEED TO CHECK
        {
            Name = "a minotaur";
            Body = 263;
            BaseSoundID = 1270;

            SetStr(301, 340);
            SetDex(91, 110);
            SetInt(31, 50);

            SetHits(301, 340);

            SetDamage(11, 20);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 25, 35);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.Meditation, 0);
            SetSkill(SkillName.EvalInt, 0);
            SetSkill(SkillName.Magery, 0);
            SetSkill(SkillName.Poisoning, 0);
            SetSkill(SkillName.Anatomy, 0);
            SetSkill(SkillName.MagicResist, 56.1, 64.0);
            SetSkill(SkillName.Tactics, 93.3, 97.8);
            SetSkill(SkillName.Wrestling, 90.4, 92.1);

            Fame = 5000;
            Karma = -5000;

            SetWeaponAbility(WeaponAbility.ParalyzingBlow);
        }

        public Minotaur(Serial serial)
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
