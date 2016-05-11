using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a Lady Sabrix corpse")]
    public class LadySabrix : GiantBlackWidow
    {
        [Constructable]
        public LadySabrix()
        {
            this.Name = "Lady Sabrix";
            this.Hue = 0x497;

            this.SetStr(82, 130);
            this.SetDex(117, 146);
            this.SetInt(50, 98);

            this.SetHits(233, 361);
            this.SetStam(117, 146);
            this.SetMana(50, 98);

            this.SetDamage(15, 22);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 40, 50);
            this.SetResistance(ResistanceType.Fire, 30, 40);
            this.SetResistance(ResistanceType.Cold, 30, 39);
            this.SetResistance(ResistanceType.Poison, 70, 80);
            this.SetResistance(ResistanceType.Energy, 35, 44);

            this.SetSkill(SkillName.Wrestling, 109.8, 122.8);
            this.SetSkill(SkillName.Tactics, 102.8, 120.0);
            this.SetSkill(SkillName.MagicResist, 79.4, 95.1);
            this.SetSkill(SkillName.Anatomy, 68.8, 105.1);
            this.SetSkill(SkillName.Poisoning, 97.8, 116.7);

            this.Fame = 18900;
            this.Karma = -18900;

            for (int i = 0; i < Utility.RandomMinMax(0, 1); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }

        public LadySabrix(Serial serial)
            : base(serial)
        {
        }
		public override bool CanBeParagon { get { return false; } }
        public override void OnDeath( Container c )
        {
            base.OnDeath( c );

            if ( Utility.RandomDouble() < 0.2 )
            c.DropItem( new SabrixsEye() );

            if ( Utility.RandomDouble() < 0.25 )
            {
                switch ( Utility.Random( 2 ) )
                {
                    case 0: AddToBackpack( new PaladinArms() ); break;
                    case 1: AddToBackpack( new HunterLegs() ); break;
                }
            }

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
            return WeaponAbility.ArmorIgnore;
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