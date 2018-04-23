// Created by Nept

using System;
using Server;

namespace Server.Items
{
    public class DaminocBlade: Scimitar
    {
        public override int InitMinHits{ get{ return 255; } }
        public override int InitMaxHits{ get{ return 255; } }
	public override int AosMinDamage{ get{ return 21; } }
	public override int OldMinDamage{ get{ return 21; } }
	public override int AosMaxDamage{ get{ return 29; } }
	public override int OldMaxDamage{ get{ return 29; } }

        [Constructable]
        public DaminocBlade()  
        {
            Name = "Blade of the Daminoc Clan";
            Hue = 2065;
            Attributes.NightSight = 1;
	    WeaponAttributes.HitLeechStam = Utility.Random( 1, 50 );
            Attributes.BonusHits = 100;
            Attributes.BonusStam = 100;
            Attributes.BonusMana = 100;
            Attributes.AttackChance = 15;
            Attributes.DefendChance = 15;
            Attributes.WeaponDamage = 20;
	    Attributes.WeaponSpeed = 10;
            WeaponAttributes.HitLeechHits = Utility.Random( 1, 50 );
            WeaponAttributes.HitLeechMana = Utility.Random( 1, 50 );
        }

        public DaminocBlade(Serial serial) : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int) 0 );
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    } 
} 
