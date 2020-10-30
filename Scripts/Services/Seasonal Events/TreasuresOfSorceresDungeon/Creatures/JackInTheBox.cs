using Server.Items;
using Server.Mobiles;

namespace Server.Engines.SorcerersDungeon
{
    [CorpseName("a jack in the box corpse")]
    public class JackInTheBox : BaseCreature
    {
        [Constructable]
        public JackInTheBox()
            : base(AIType.AI_NecroMage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "jack in the box";
            Body = 1428;

            SetStr(200, 300);
            SetDex(150, 200);
            SetInt(700, 800);

            SetHits(8000);

            SetDamage(21, 27);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 70, 80);
            SetResistance(ResistanceType.Fire, 70, 80);
            SetResistance(ResistanceType.Cold, 70, 80);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.Anatomy, 25);
            SetSkill(SkillName.MagicResist, 120);
            SetSkill(SkillName.Tactics, 100, 110);
            SetSkill(SkillName.Wrestling, 130, 140);
            SetSkill(SkillName.Parry, 20, 30);

            SetSkill(SkillName.Magery, 120);
            SetSkill(SkillName.EvalInt, 120);
            SetSkill(SkillName.Necromancy, 120);
            SetSkill(SkillName.SpiritSpeak, 120);
            SetSkill(SkillName.Meditation, 120);
            SetSkill(SkillName.Focus, 70, 80);

            Fame = 16000;
            Karma = -16000;

            SetWeaponAbility(WeaponAbility.ArmorIgnore);
            SetWeaponAbility(WeaponAbility.BleedAttack);
        }

        public JackInTheBox(Serial serial)
            : base(serial)
        {
        }

        public override bool CanFlee => false;
        public override bool AlwaysMurderer => true;
        public override Poison PoisonImmune => Poison.Deadly;

        public override int GetIdleSound() { return 0x218; }
        public override int GetAngerSound() { return 0x26C; }
        public override int GetDeathSound() { return 0x211; }
        public override int GetAttackSound() { return 0x232; }
        public override int GetHurtSound() { return 0x140; }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 2);
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
