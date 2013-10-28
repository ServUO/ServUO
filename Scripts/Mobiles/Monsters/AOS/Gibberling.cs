using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a gibberling corpse")]
    public class Gibberling : BaseCreature
    {
        [Constructable]
        public Gibberling()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a gibberling";
            this.Body = 307;
            this.BaseSoundID = 422;

            this.SetStr(141, 165);
            this.SetDex(101, 125);
            this.SetInt(56, 80);

            this.SetHits(85, 99);

            this.SetDamage(12, 17);

            this.SetDamageType(ResistanceType.Physical, 0);
            this.SetDamageType(ResistanceType.Fire, 40);
            this.SetDamageType(ResistanceType.Energy, 60);

            this.SetResistance(ResistanceType.Physical, 45, 55);
            this.SetResistance(ResistanceType.Fire, 25, 35);
            this.SetResistance(ResistanceType.Cold, 25, 35);
            this.SetResistance(ResistanceType.Poison, 10, 20);
            this.SetResistance(ResistanceType.Energy, 30, 40);

            this.SetSkill(SkillName.MagicResist, 45.1, 70.0);
            this.SetSkill(SkillName.Tactics, 67.6, 92.5);
            this.SetSkill(SkillName.Wrestling, 60.1, 80.0);

            this.Fame = 1500;
            this.Karma = -1500;

            this.VirtualArmor = 27;
        }

        public Gibberling(Serial serial)
            : base(serial)
        {
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
            return WeaponAbility.Dismount;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Meager);
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