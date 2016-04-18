using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a chaos daemon corpse")]
    public class ChaosDaemon : BaseCreature
    {
        [Constructable]
        public ChaosDaemon()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a chaos daemon";
            this.Body = 792;
            this.BaseSoundID = 0x3E9;

            this.SetStr(106, 130);
            this.SetDex(171, 200);
            this.SetInt(56, 80);

            this.SetHits(91, 110);

            this.SetDamage(12, 17);

            this.SetDamageType(ResistanceType.Physical, 85);
            this.SetDamageType(ResistanceType.Fire, 15);

            this.SetResistance(ResistanceType.Physical, 50, 60);
            this.SetResistance(ResistanceType.Fire, 60, 70);
            this.SetResistance(ResistanceType.Cold, 40, 50);
            this.SetResistance(ResistanceType.Poison, 20, 30);
            this.SetResistance(ResistanceType.Energy, 20, 30);

            this.SetSkill(SkillName.MagicResist, 85.1, 95.0);
            this.SetSkill(SkillName.Tactics, 70.1, 80.0);
            this.SetSkill(SkillName.Wrestling, 95.1, 100.0);

            this.Fame = 3000;
            this.Karma = -4000;

            this.VirtualArmor = 15;
        }

        public ChaosDaemon(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.CrushingBlow;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Average);
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