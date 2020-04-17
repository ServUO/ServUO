using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a wailing banshee corpse")]
    public class WailingBanshee : BaseCreature
    {
        [Constructable]
        public WailingBanshee()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a wailing banshee";
            Body = 310;
            BaseSoundID = 0x482;

            SetStr(126, 150);
            SetDex(76, 100);
            SetInt(86, 110);

            SetHits(76, 90);

            SetDamage(10, 14);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Cold, 60);
            SetDamageType(ResistanceType.Poison, 20);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 25, 30);
            SetResistance(ResistanceType.Cold, 70, 80);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.MagicResist, 70.1, 95.0);
            SetSkill(SkillName.Tactics, 45.1, 70.0);
            SetSkill(SkillName.Wrestling, 50.1, 70.0);

            Fame = 1500;
            Karma = -1500;

            SetWeaponAbility(WeaponAbility.MortalStrike);
        }

        public WailingBanshee(Serial serial)
            : base(serial)
        {
        }

        public override bool BleedImmune => true;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
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