using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an archaeosaurus corpse")]
    public class Archaeosaurus : BaseCreature
    {
        public override bool AttacksFocus { get { return true; } }

        [Constructable]
        public Archaeosaurus()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, .2, .4)
        {
            this.Name = "an Archaeosaurus";
            this.Body = 1287;
            this.BaseSoundID = 422;

            this.SetStr(405, 421);
            this.SetDex(301, 320);
            this.SetInt(201, 224);

            this.SetDamage(14, 16);

            this.SetHits(1818, 2500);

            this.SetResistance(ResistanceType.Physical, 2, 3);
            this.SetResistance(ResistanceType.Fire, 4, 5);
            this.SetResistance(ResistanceType.Cold, 2, 3);
            this.SetResistance(ResistanceType.Poison, 3, 4);
            this.SetResistance(ResistanceType.Energy, 3);

            this.SetDamageType(ResistanceType.Poison, 50);
            this.SetDamageType(ResistanceType.Fire, 50);

            this.SetSkill(SkillName.MagicResist, 100.0, 115.0);
            this.SetSkill(SkillName.Tactics, 90.0, 110.0);
            this.SetSkill(SkillName.Wrestling, 90.0, 110.0);
            this.SetSkill(SkillName.DetectHidden, 60.0, 70.0);
            this.SetSkill(SkillName.EvalInt, 95.0, 105.0);
            this.SetSkill(SkillName.Ninjitsu, 120.0);

            this.Fame = 8100;
            this.Karma = -8100;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
        }

        public override int Meat { get { return 1; } }
        public override int Hides { get { return 7; } }
        public override int DragonBlood { get { return 6; } }

        public override WeaponAbility GetWeaponAbility()
        {
            if (Utility.RandomBool())
                return WeaponAbility.BleedAttack;

            return WeaponAbility.TalonStrike;
        }

        public Archaeosaurus(Serial serial)
            : base(serial)
        {
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