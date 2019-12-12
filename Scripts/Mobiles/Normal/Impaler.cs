using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an impaler corpse")]
    public class Impaler : BaseCreature
    {
        [Constructable]
        public Impaler()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = NameList.RandomName("impaler");
            Body = 306;
            BaseSoundID = 0x2A7;

            SetStr(190);
            SetDex(45);
            SetInt(190);

            SetHits(5000);

            SetDamage(31, 35);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 90);
            SetResistance(ResistanceType.Fire, 60);
            SetResistance(ResistanceType.Cold, 75);
            SetResistance(ResistanceType.Poison, 60);
            SetResistance(ResistanceType.Energy, 100);

            SetSkill(SkillName.Wrestling, 80.0, 120.0);
            SetSkill(SkillName.Tactics, 100.0);
            SetSkill(SkillName.MagicResist, 100.0);
            SetSkill(SkillName.Poisoning, 160.0);
            SetSkill(SkillName.DetectHidden, 100.0);
            SetSkill(SkillName.Necromancy, 110.0, 120.0);
            SetSkill(SkillName.SpiritSpeak, 100.0, 110.0);

            Fame = 24000;
            Karma = -24000;

            VirtualArmor = 49;

            SetWeaponAbility(WeaponAbility.BleedAttack);
            SetWeaponAbility(WeaponAbility.MortalStrike);
            SetWeaponAbility(WeaponAbility.ArmorIgnore);

            ForceActiveSpeed = 0.38;
            ForcePassiveSpeed = 0.66;
        }

        public Impaler(Serial serial)
            : base(serial)
        {
        }

        public override bool CanFlee { get { return false; } }

        public override bool IgnoreYoungProtection
        {
            get
            {
                return Core.ML;
            }
        }
        public override bool AutoDispel
        {
            get
            {
                return true;
            }
        }
        public override bool BardImmune
        {
            get
            {
                return !Core.SE;
            }
        }
        public override bool Unprovokable
        {
            get
            {
                return Core.SE;
            }
        }
        public override bool AreaPeaceImmune
        {
            get
            {
                return Core.SE;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override Poison HitPoison
        {
            get
            {
                return (0.8 >= Utility.RandomDouble() ? Poison.Greater : Poison.Deadly);
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 1;
            }
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 2);
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
