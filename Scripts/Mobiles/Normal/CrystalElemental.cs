using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a crystal elemental corpse")]
    public class CrystalElemental : BaseCreature
    {
        [Constructable]
        public CrystalElemental()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a crystal elemental";
            this.Body = 300;
            this.BaseSoundID = 278;

            this.SetStr(136, 160);
            this.SetDex(51, 65);
            this.SetInt(86, 110);

            this.SetHits(150);

            this.SetDamage(10, 15);

            this.SetDamageType(ResistanceType.Physical, 80);
            this.SetDamageType(ResistanceType.Energy, 20);

            this.SetResistance(ResistanceType.Physical, 50, 60);
            this.SetResistance(ResistanceType.Fire, 40, 50);
            this.SetResistance(ResistanceType.Cold, 40, 50);
            this.SetResistance(ResistanceType.Poison, 100);
            this.SetResistance(ResistanceType.Energy, 55, 70);

            this.SetSkill(SkillName.EvalInt, 70.1, 75.0);
            this.SetSkill(SkillName.Magery, 70.1, 75.0);
            this.SetSkill(SkillName.Meditation, 65.1, 75.0);
            this.SetSkill(SkillName.MagicResist, 80.1, 90.0);
            this.SetSkill(SkillName.Tactics, 75.1, 85.0);
            this.SetSkill(SkillName.Wrestling, 65.1, 75.0);

            this.Fame = 6500;
            this.Karma = -6500;

            this.VirtualArmor = 54;
        }

        public CrystalElemental(Serial serial)
            : base(serial)
        {
        }

        public override bool BleedImmune
        {
            get
            {
                return true;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 1;
            }
        }
        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.BleedAttack;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich);
            this.AddLoot(LootPack.Average);
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
        }
    }
}