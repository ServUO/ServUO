// Created by Nept

using System;
using Server;

namespace Server.Items
{
    public class SinChest : PlateChest
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
        public SinChest() 
        {
            Name = "Chest of Gluttony";
            Hue = 2100;
            Attributes.NightSight = 1;
            Attributes.BonusStr = Utility.Random( 1, 30 );
            Attributes.BonusInt = Utility.Random( 1, 30 );
            Attributes.BonusDex = Utility.Random( 1, 30 );
	    Attributes.ReflectPhysical = Utility.Random( 1, 35 );
            Attributes.BonusHits = Utility.Random( 1, 15 );
            Attributes.BonusStam = Utility.Random( 1, 15 );
            Attributes.BonusMana = Utility.Random( 1, 15 );
            Attributes.RegenHits = Utility.Random( 1, 5 );
            Attributes.RegenStam = Utility.Random( 1, 5 );
            Attributes.AttackChance = Utility.Random( 1, 25 );
            Attributes.DefendChance = Utility.Random( 1, 25 );
            Attributes.WeaponDamage = Utility.Random( 1, 60 );
            Attributes.SpellDamage = Utility.Random( 1, 65 );
	    Attributes.WeaponSpeed = Utility.Random( 1, 30 );
            Attributes.LowerManaCost = 10;
            Attributes.LowerRegCost = 20;
        }

        public SinChest(Serial serial) : base( serial )
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
