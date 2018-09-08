using System;
using Server.Engines.CannedEvil;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a Khal Ankur corpse")]
    public class KhalAnkur : BaseChampion
    {
        [Constructable]
        public KhalAnkur()
            : base(AIType.AI_Necro)
        {
            Name = "Khal Ankur";
            Body = 0x5C7;
            BaseSoundID = 0x301;

            SetStr(700, 800);
            SetDex(500, 600);
            SetInt(800, 900);

            SetHits(30000);

            SetDamage(28, 35);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Cold, 25);
            SetDamageType(ResistanceType.Poison, 25);

            SetResistance(ResistanceType.Physical, 90);
            SetResistance(ResistanceType.Fire, 70, 80);
            SetResistance(ResistanceType.Cold, 70, 80);
            SetResistance(ResistanceType.Poison, 80, 90);
            SetResistance(ResistanceType.Energy, 80, 90);

            SetSkill(SkillName.Wrestling, 120.0);
            SetSkill(SkillName.Tactics, 80.0, 100.0);
            SetSkill(SkillName.MagicResist, 150.0);
            SetSkill(SkillName.DetectHidden, 100.0);
            SetSkill(SkillName.Parry, 80.0, 100.0);
            SetSkill(SkillName.Necromancy, 120.0);
            SetSkill(SkillName.SpiritSpeak, 120.0);           

            Fame = 28000;
            Karma = -28000;

            VirtualArmor = 80;

            SetWeaponAbility(WeaponAbility.Disarm);
            SetMagicalAbility(MagicalAbility.WrestlingMastery);
        }

        public KhalAnkur(Serial serial)
            : base(serial)
        {
        }

        public override bool CanRummageCorpses { get { return true; } }
        public override bool BleedImmune { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override bool ShowFameTitle { get { return false; } }
        public override bool ClickTitle { get { return false; } }
        public override bool AlwaysMurderer { get { return true; } }
        public override bool AutoDispel { get { return true; } }
        public override double AutoDispelChance { get { return 1.0; } }

        public override ChampionSkullType SkullType { get { return ChampionSkullType.None; } }

        public override Type[] UniqueList
        {
            get
            {
                return new Type[] { };
            }
        }
        public override Type[] SharedList
        {
            get
            {
                return new Type[] { };
            }
        }
        public override Type[] DecorativeList
        {
            get
            {
                return new Type[] { };
            }
        }
        public override MonsterStatuetteType[] StatueTypes
        {
            get
            {
                return new MonsterStatuetteType[] { };
            }
        }        

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 3);
            AddLoot(LootPack.Meager);
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
