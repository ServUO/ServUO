using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a Miasma corpse")]
    public class Miasma : Scorpion
    {
        [Constructable]
        public Miasma()
        {

            this.Name = "Miasma";
            this.Hue = 0x8FD;

            this.SetStr(255, 847);
            this.SetDex(145, 428);
            this.SetInt(26, 380);

            this.SetHits(272, 2000);
            this.SetMana(5, 60);

            this.SetDamage(20, 30);

            this.SetDamageType(ResistanceType.Physical, 60);
            this.SetDamageType(ResistanceType.Poison, 40);

            this.SetResistance(ResistanceType.Physical, 50, 54);
            this.SetResistance(ResistanceType.Fire, 40, 45);
            this.SetResistance(ResistanceType.Cold, 50, 55);
            this.SetResistance(ResistanceType.Poison, 70, 80);
            this.SetResistance(ResistanceType.Energy, 40, 45);

            this.SetSkill(SkillName.Wrestling, 64.9, 73.3);
            this.SetSkill(SkillName.Tactics, 98.4, 110.6);
            this.SetSkill(SkillName.MagicResist, 74.4, 77.7);
            this.SetSkill(SkillName.Poisoning, 128.5, 143.6);

            this.Fame = 21000;
            this.Karma = -21000;

            this.Tamable = false;

            for (int i = 0; i < Utility.RandomMinMax(0, 1); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }

        public Miasma(Serial serial)
            : base(serial)
        {
        }
		public override bool CanBeParagon { get { return false; } }
        public override void OnDeath( Container c )
        {
            base.OnDeath( c );

            if ( Paragon.ChestChance > Utility.RandomDouble() )
            c.DropItem( new ParagonChest( Name, TreasureMapLevel ) );

            if ( Utility.RandomDouble() < 0.025 )
            {
                switch ( Utility.Random( 16 ) )
                {
                    case 0: c.DropItem( new MyrmidonGloves() ); break;
                    case 1: c.DropItem( new MyrmidonGorget() ); break;
                    case 2: c.DropItem( new MyrmidonLegs() ); break;
                    case 3: c.DropItem( new MyrmidonArms() ); break;
                    case 4: c.DropItem( new PaladinArms() ); break;
                    case 5: c.DropItem( new PaladinGorget() ); break;
                    case 6: c.DropItem( new LeafweaveLegs() ); break;
                    case 7: c.DropItem( new DeathChest() ); break;
                    case 8: c.DropItem( new DeathGloves() ); break;
                    case 9: c.DropItem( new DeathLegs() ); break;
                    case 10: c.DropItem( new GreymistGloves() ); break;
                    case 11: c.DropItem( new GreymistArms() ); break;
                    case 12: c.DropItem( new AssassinChest() ); break;
                    case 13: c.DropItem( new AssassinArms() ); break;
                    case 14: c.DropItem( new HunterGloves() ); break;
                    case 15: c.DropItem( new HunterLegs() ); break;
                }
            }
        }

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
                return 5;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.UltraRich, 2);
        }

        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.MortalStrike;
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