using System;
using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a Swoop corpse")]
    public class Swoop : Eagle
    {
        [Constructable]
        public Swoop()
        {

            Name = "Swoop";
            Hue = 0xE0;

            AI = AIType.AI_Melee;

            SetStr(100, 150);
            SetDex(400, 480);
            SetInt(75, 90);

            SetHits(1350, 1550);

            SetDamage(20, 30);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 75, 90);
            SetResistance(ResistanceType.Fire, 60, 70);
            SetResistance(ResistanceType.Cold, 70, 85);
            SetResistance(ResistanceType.Poison, 55, 60);
            SetResistance(ResistanceType.Energy, 50, 60);

            SetSkill(SkillName.Wrestling, 120.0, 140.0);
            SetSkill(SkillName.Tactics, 120.0, 140.0);
            SetSkill(SkillName.MagicResist, 95.0, 105.0);

            Fame = 18000;
            Karma = 0;

            PackReg(4);

            Tamable = false;

            for (int i = 0; i < Utility.RandomMinMax(0, 1); i++)
            {
                PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }

            SetSpecialAbility(SpecialAbility.GraspingClaw);
        }

		public override bool CanBeParagon { get { return false; } }

        public override void OnDeath( Container c )
        {
            base.OnDeath( c );

            if ( Utility.RandomDouble() < 0.025 )
            {
                switch ( Utility.Random( 18 ) )
                {
                    case 0: c.DropItem( new AssassinChest() ); break;
                    case 1: c.DropItem( new AssassinArms() ); break;
                    case 2: c.DropItem( new DeathChest() ); break;
                    case 3: c.DropItem( new MyrmidonArms() ); break;
                    case 4: c.DropItem( new MyrmidonLegs() ); break;
                    case 5: c.DropItem( new MyrmidonGorget() ); break;
                    case 6: c.DropItem( new LeafweaveGloves() ); break;
                    case 7: c.DropItem( new LeafweaveLegs() ); break;
                    case 8: c.DropItem( new LeafweavePauldrons() ); break;
                    case 9: c.DropItem( new PaladinGloves() ); break;
                    case 10: c.DropItem( new PaladinGorget() ); break;
                    case 11: c.DropItem( new PaladinArms() ); break;
                    case 12: c.DropItem( new HunterArms() ); break;
                    case 13: c.DropItem( new HunterGloves() ); break;
                    case 14: c.DropItem( new HunterLegs() ); break;
                    case 15: c.DropItem( new HunterChest() ); break;
                    case 16: c.DropItem( new GreymistArms() ); break;
                    case 17: c.DropItem( new GreymistGloves() ); break;
                }
            }

            if ( Utility.RandomDouble() < 0.1 )
            c.DropItem( new ParrotItem() );
        }
        
        public Swoop(Serial serial)
            : base(serial)
        {
        }

        public override bool CanFly
        {
            get
            {
                return true;
            }
        }
        public override bool GivesMLMinorArtifact
        {
            get
            {
                return true;
            }
        }
        public override int Feathers
        {
            get
            {
                return 72;
            }
        }
        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 2);
            AddLoot(LootPack.HighScrolls);
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
