using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a moloch corpse")]
    public class Moloch : BaseCreature
    {
        [Constructable]
        public Moloch()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a moloch";
            this.Body = 0x311;
            this.BaseSoundID = 0x300;

            this.SetStr(331, 360);
            this.SetDex(66, 85);
            this.SetInt(41, 65);

            this.SetHits(171, 200);

            this.SetDamage(15, 23);

            this.SetResistance(ResistanceType.Physical, 60, 70);
            this.SetResistance(ResistanceType.Fire, 60, 70);
            this.SetResistance(ResistanceType.Cold, 40, 50);
            this.SetResistance(ResistanceType.Poison, 20, 30);
            this.SetResistance(ResistanceType.Energy, 20, 30);

            this.SetSkill(SkillName.MagicResist, 65.1, 75.0);
            this.SetSkill(SkillName.Tactics, 75.1, 90.0);
            this.SetSkill(SkillName.Wrestling, 70.1, 90.0);

            this.Fame = 7500;
            this.Karma = -7500;

            this.VirtualArmor = 32;
        }

        public Moloch(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Regular;
            }
        }
        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.ConcussionBlow;
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