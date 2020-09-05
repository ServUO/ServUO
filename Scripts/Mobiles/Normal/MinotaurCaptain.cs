using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a minotaur corpse")]
    public class MinotaurCaptain : BaseCreature
    {
        [Constructable]
        public MinotaurCaptain()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)// NEED TO CHECK
        {
            Name = "a minotaur captain";
            Body = 280;
            BaseSoundID = 1270;

            SetStr(401, 425);
            SetDex(91, 110);
            SetInt(31, 50);

            SetHits(401, 440);

            SetDamage(11, 20);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 65, 75);
            SetResistance(ResistanceType.Fire, 35, 45);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.Meditation, 0);
            SetSkill(SkillName.EvalInt, 0);
            SetSkill(SkillName.Magery, 0);
            SetSkill(SkillName.Poisoning, 0);
            SetSkill(SkillName.Anatomy, 0, 6.3);
            SetSkill(SkillName.MagicResist, 66.1, 73.6);
            SetSkill(SkillName.Tactics, 93.0, 109.9);
            SetSkill(SkillName.Wrestling, 92.6, 107.2);

            Fame = 7000;
            Karma = -7000;

            SetWeaponAbility(WeaponAbility.ParalyzingBlow);
        }

        public MinotaurCaptain(Serial serial)
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
