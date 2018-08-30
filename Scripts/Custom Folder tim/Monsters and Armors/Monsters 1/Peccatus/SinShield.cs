// Created by Nept

using System;
using Server;

namespace Server.Items
{
    public class SinShield : BronzeShield
    {
        public override int BasePhysicalResistance{ get{ return 20; } }
        public override int BaseColdResistance{ get{ return 20; } }
        public override int BaseFireResistance{ get{ return 20; } }
        public override int BaseEnergyResistance{ get{ return 20; } }
        public override int BasePoisonResistance{ get{ return 20; } }
        public override int ArtifactRarity{ get{ return 23; } }
        public override int InitMinHits{ get{ return 255; } }
        public override int InitMaxHits{ get{ return 255; } }

        [Constructable]
        public SinShield() : base( 11024 ) 
        {
            Name = "Shield of Sloth";
            Hue = 2100;
            Attributes.NightSight = 1;
            Attributes.BonusStr = Utility.Random( 1, 30 );
            Attributes.BonusInt = Utility.Random( 1, 30 );
            Attributes.BonusDex = Utility.Random( 1, 30 );
	    Attributes.ReflectPhysical = Utility.Random( 1, 35 );
            Attributes.BonusHits = Utility.Random( 1, 20 );
            Attributes.BonusStam = Utility.Random( 1, 20 );
            Attributes.BonusMana = Utility.Random( 1, 20 );
            Attributes.AttackChance = Utility.Random( 1, 25 );
            Attributes.DefendChance = Utility.Random( 1, 25 );
            Attributes.WeaponDamage = Utility.Random( 1, 50 );
            Attributes.SpellDamage = Utility.Random( 1, 40 );
	    Attributes.WeaponSpeed = Utility.Random( 1, 75 );
        }

        public SinShield(Serial serial) : base( serial )
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
