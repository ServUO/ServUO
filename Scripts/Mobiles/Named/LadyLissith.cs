using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a Lady Lissith corpse")]
    public class LadyLissith : GiantBlackWidow
    {
        [Constructable]
        public LadyLissith()
        {

            this.Name = "Lady Lissith";
            this.Hue = 0x452;

            this.SetStr(81, 130);
            this.SetDex(116, 152);
            this.SetInt(44, 100);

            this.SetHits(245, 375);
            this.SetStam(116, 152);
            this.SetMana(44, 100);

            this.SetDamage(15, 22);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 40, 50);
            this.SetResistance(ResistanceType.Fire, 31, 39);
            this.SetResistance(ResistanceType.Cold, 30, 40);
            this.SetResistance(ResistanceType.Poison, 71, 80);
            this.SetResistance(ResistanceType.Energy, 30, 40);

            this.SetSkill(SkillName.Wrestling, 108.6, 123.0);
            this.SetSkill(SkillName.Tactics, 102.7, 119.0);
            this.SetSkill(SkillName.MagicResist, 78.8, 95.6);
            this.SetSkill(SkillName.Anatomy, 68.6, 106.8);
            this.SetSkill(SkillName.Poisoning, 96.6, 112.9);

            this.Fame = 18900;
            this.Karma = -18900;

            for (int i = 0; i < Utility.RandomMinMax(0, 1); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }

        public LadyLissith(Serial serial)
            : base(serial)
        {
        }
		public override bool CanBeParagon { get { return false; } }
        public override void OnDeath( Container c )
        {
            base.OnDeath( c );

            if ( Utility.RandomDouble() < 0.025 )
            c.DropItem( new GreymistChest() );

            if ( Utility.RandomDouble() < 0.45 )
            c.DropItem( new LissithsSilk() );

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
            this.AddLoot(LootPack.UltraRich, 2);
        }

        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.BleedAttack;
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