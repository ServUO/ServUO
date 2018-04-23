// Created by Nept

using System;
using Server;

namespace Server.Items
{
    public class ManiacTailorGorget : LeatherGorget
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
        public ManiacTailorGorget()
        {
            Name = "Gorget of a Maniacal Tailor";
            Hue = 2075;
            Attributes.NightSight = 1;
            Attributes.BonusStr = Utility.Random( 5, 35 );
            Attributes.BonusInt = Utility.Random( 5, 35 );
            Attributes.BonusDex = Utility.Random( 5, 35 );
	    Attributes.ReflectPhysical = Utility.Random( 5, 35 );
            Attributes.BonusHits = Utility.Random( 5, 35 );
            Attributes.BonusStam = Utility.Random( 5, 35 );
            Attributes.BonusMana = Utility.Random( 5, 35 );
            Attributes.RegenHits = Utility.Random( 5, 35 );
            Attributes.RegenStam = Utility.Random( 5, 35 );
            Attributes.AttackChance = Utility.Random( 5, 35 );
            Attributes.DefendChance = Utility.Random( 5, 35 );
            Attributes.WeaponDamage = Utility.Random( 5, 35 );
            Attributes.SpellDamage = Utility.Random( 5, 35 );
	    Attributes.WeaponSpeed = Utility.Random( 5, 35 );
            ArmorAttributes.MageArmor = 1;
            Attributes.CastSpeed = 5;
            Attributes.CastRecovery = 5;
            Attributes.LowerManaCost = Utility.Random( 5, 35 );
            Attributes.LowerRegCost = Utility.Random( 5, 35 );
        }

        public ManiacTailorGorget(Serial serial) : base( serial )
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
