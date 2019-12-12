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
            Name = "a moloch";
            Body = 0x311;
            BaseSoundID = 0x300;

            SetStr(331, 360);
            SetDex(66, 85);
            SetInt(41, 65);

            SetHits(171, 200);

            SetDamage(15, 23);

            SetResistance(ResistanceType.Physical, 60, 70);
            SetResistance(ResistanceType.Fire, 60, 70);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 20, 30);
            SetResistance(ResistanceType.Energy, 20, 30);

            SetSkill(SkillName.MagicResist, 65.1, 75.0);
            SetSkill(SkillName.Tactics, 75.1, 90.0);
            SetSkill(SkillName.Wrestling, 70.1, 90.0);

            Fame = 7500;
            Karma = -7500;

            VirtualArmor = 32;

            SetWeaponAbility(WeaponAbility.ConcussionBlow);
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

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
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