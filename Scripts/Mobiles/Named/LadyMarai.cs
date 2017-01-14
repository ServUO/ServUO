using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a Lady Marai corpse")]
    public class LadyMarai : SkeletalKnight
    {
        [Constructable]
        public LadyMarai()
        {
            this.Name = "Lady Marai";
            this.Hue = 0x21;

            this.SetStr(221, 304);
            this.SetDex(98, 138);
            this.SetInt(54, 99);

            this.SetHits(694, 846);

            this.SetDamage(15, 25);

            this.SetDamageType(ResistanceType.Physical, 40);
            this.SetDamageType(ResistanceType.Cold, 60);

            this.SetResistance(ResistanceType.Physical, 55, 65);
            this.SetResistance(ResistanceType.Fire, 40, 50);
            this.SetResistance(ResistanceType.Cold, 70, 80);
            this.SetResistance(ResistanceType.Poison, 40, 50);
            this.SetResistance(ResistanceType.Energy, 50, 60);

            this.SetSkill(SkillName.Wrestling, 126.6, 137.2);
            this.SetSkill(SkillName.Tactics, 128.7, 134.5);
            this.SetSkill(SkillName.MagicResist, 102.1, 119.1);
            this.SetSkill(SkillName.Anatomy, 126.2, 136.5);

            this.Fame = 18000;
            this.Karma = -18000;

            for (int i = 0; i < Utility.RandomMinMax(0, 1); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }

        public LadyMarai(Serial serial)
            : base(serial)
        {
        }
		public override bool CanBeParagon { get { return false; } }
        public override void OnDeath( Container c )
        {
            base.OnDeath( c );

            if ( Utility.RandomDouble() < 0.15 )
            c.DropItem( new DisintegratingThesisNotes() );

            if ( Utility.RandomDouble() < 0.1 )
            c.DropItem( new ParrotItem() );
        }

        public override bool GivesMLMinorArtifact
        {
            get
            {
                return true;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.UltraRich, 3);
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