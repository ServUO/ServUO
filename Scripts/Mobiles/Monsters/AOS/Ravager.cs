using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a ravager corpse")]
    public class Ravager : BaseCreature
    {
        [Constructable]
        public Ravager()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a ravager";
            this.Body = 314;
            this.BaseSoundID = 357;

            this.SetStr(251, 275);
            this.SetDex(101, 125);
            this.SetInt(66, 90);

            this.SetHits(161, 175);

            this.SetDamage(15, 20);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 50, 60);
            this.SetResistance(ResistanceType.Fire, 50, 60);
            this.SetResistance(ResistanceType.Cold, 60, 70);
            this.SetResistance(ResistanceType.Poison, 30, 40);
            this.SetResistance(ResistanceType.Energy, 20, 30);

            this.SetSkill(SkillName.MagicResist, 50.1, 75.0);
            this.SetSkill(SkillName.Tactics, 75.1, 100.0);
            this.SetSkill(SkillName.Wrestling, 70.1, 90.0);

            this.Fame = 3500;
            this.Karma = -3500;

            this.VirtualArmor = 54;
        }

        public Ravager(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility GetWeaponAbility()
        {
            return Utility.RandomBool() ? WeaponAbility.Dismount : WeaponAbility.CrushingBlow;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich);
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