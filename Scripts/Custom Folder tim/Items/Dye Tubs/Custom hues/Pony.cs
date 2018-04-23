//###########################################################//
// CREATED BY FOREST CONDON AKA FCONDON AKA EXALE
// MYSTICALNIGHTS.ORG
// PLEASE LEAVE HEADER INTACT.
//###########################################################//

using System;
using Server;
using System.Collections;
using Server.Items;
using Server.Mobiles;
using Server.AllHues;

namespace Server.Mobiles
{
    [CorpseName( "a pony corpse" )]
    [TypeAlias( "Server.Mobiles.BrownHorse", "Server.Mobiles.DirtyHorse", "Server.Mobiles.GrayHorse", "Server.Mobiles.TanHorse" )]
    public class Pony : BaseMount
    {
        private static int[] m_IDs = new int[]
            {
                0xC8, 0x3E9F,
                0xE2, 0x3EA0,
                0xE4, 0x3EA1,
                0xCC, 0x3EA2
            };

        [Constructable]
        public Pony() : this( "a pony" )
        {
        }

        [Constructable]
        public Pony( string name ) : base( name, 0xE2, 0x3EA0, AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
        {
            int random = Utility.Random( 4 );

            Body = m_IDs[random * 2];
            ItemID = m_IDs[random * 2 + 1];
            BaseSoundID = 0xA8;
            Hue=AllHuesInfo.Rare;

            SetStr( 100, 150 );
            SetDex( 80, 100 );
            SetInt( 80, 100 );

            SetHits( 100, 150 );

            SetDamage( 6, 12 );

            SetDamageType( ResistanceType.Physical, 100 );

            SetResistance( ResistanceType.Physical, 30, 40 );
            SetResistance( ResistanceType.Fire, 10, 20 );
            SetResistance( ResistanceType.Cold, 10, 20 );
            SetResistance( ResistanceType.Poison, 10, 20 );
            SetResistance( ResistanceType.Energy, 10, 20 );

            SetSkill( SkillName.MagicResist, 30.0, 50.0 );
            SetSkill( SkillName.Tactics, 50.0, 70.0 );
            SetSkill( SkillName.Wrestling, 60.0, 80.0 );

            Fame = 5000;
            Karma = 5000;

            VirtualArmor = 40;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 95.1;

        }

        public override int Meat{ get{ return 3; } }
        public override int Hides{ get{ return 10; } }
        public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

        public Pony( Serial serial ) : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int) 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }
}