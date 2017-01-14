using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a tormented minotaur corpse")]
    public class TormentedMinotaur : BaseCreature
    {
        [Constructable]
        public TormentedMinotaur()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "Tormented Minotaur";
            this.Body = 262;

            this.SetStr(822, 930);
            this.SetDex(401, 415);
            this.SetInt(128, 138);

            this.SetHits(4000, 4200);

            this.SetDamage(16, 30);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 62);
            this.SetResistance(ResistanceType.Fire, 74);
            this.SetResistance(ResistanceType.Cold, 54);
            this.SetResistance(ResistanceType.Poison, 56);
            this.SetResistance(ResistanceType.Energy, 54);

            this.SetSkill(SkillName.Wrestling, 110.1, 111.0);
            this.SetSkill(SkillName.Tactics, 100.7, 102.8);
            this.SetSkill(SkillName.MagicResist, 104.3, 116.3);

            this.Fame = 20000;
            this.Karma = -20000;
        }

        public TormentedMinotaur(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Deadly;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 3;
            }
        }
        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.Dismount;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich, 10);
        }

        public override int GetDeathSound()
        {
            return 0x596;
        }

        public override int GetAttackSound()
        {
            return 0x597;
        }

        public override int GetIdleSound()
        {
            return 0x598;
        }

        public override int GetAngerSound()
        {
            return 0x599;
        }

        public override int GetHurtSound()
        {
            return 0x59A;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}