using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a Lurg corpse")]
    public class Lurg : Troglodyte
    {
        [Constructable]
        public Lurg()
        {
            this.Name = "Lurg";
            this.Hue = 0x455;

            this.SetStr(584, 625);
            this.SetDex(163, 176);
            this.SetInt(90, 106);

            this.SetHits(3034, 3189);
            this.SetStam(163, 176);
            this.SetMana(90, 106);

            this.SetDamage(16, 19);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 50, 53);
            this.SetResistance(ResistanceType.Fire, 45, 47);
            this.SetResistance(ResistanceType.Cold, 56, 60);
            this.SetResistance(ResistanceType.Poison, 50, 60);
            this.SetResistance(ResistanceType.Energy, 41, 56);

            this.SetSkill(SkillName.Wrestling, 122.7, 130.5);
            this.SetSkill(SkillName.Tactics, 109.3, 118.5);
            this.SetSkill(SkillName.MagicResist, 72.9, 87.6);
            this.SetSkill(SkillName.Anatomy, 110.5, 124.0);
            this.SetSkill(SkillName.Healing, 84.1, 105.0);

            this.Fame = 10000;
            this.Karma = -10000;

            for (int i = 0; i < Utility.RandomMinMax(0, 1); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }

        public Lurg(Serial serial)
            : base(serial)
        {
        }
        public override bool CanBeParagon { get { return false; } }
        public override bool GivesMLMinorArtifact
        {
            get
            {
                return true;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 4;
            }
        }
        public override bool AllureImmune
        {
            get
            {
                return true;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.UltraRich, 2);
        }

        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.CrushingBlow;
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