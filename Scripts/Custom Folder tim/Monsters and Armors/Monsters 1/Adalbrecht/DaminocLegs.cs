// Created by Nept

using System;
using Server;

namespace Server.Items
{
    public class DaminocLegs: ChainLegs
    {
        public override int InitMinHits{ get{ return 255; } }
        public override int InitMaxHits{ get{ return 255; } }

        [Constructable]
        public DaminocLegs()  
        {
            Name = "Legs of the Daminoc Clan";
            Hue = 2065;
            Attributes.NightSight = 1;
	    Attributes.ReflectPhysical = Utility.Random( 1, 35 );
            Attributes.BonusHits = 25;
            Attributes.BonusStam = 25;
            Attributes.BonusMana = 25;
            Attributes.AttackChance = 15;
            Attributes.DefendChance = 15;
            Attributes.WeaponDamage = 20;
	    Attributes.WeaponSpeed = 10;
            ArmorAttributes.MageArmor = 1;
            Attributes.LowerRegCost = Utility.Random( 1, 35 );
        }

        public DaminocLegs(Serial serial) : base( serial )
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
