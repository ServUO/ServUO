// Created by Nept

using System;
using Server;

namespace Server.Items
{
    public class VirtueCloak : LeatherGloves
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
        public VirtueCloak()  
        {
            Name = "Cloak of Humility";
            Hue = 2040;
	    ItemID = 11013;
	    Layer = Layer.Cloak;
            Attributes.NightSight = 1;
            Attributes.BonusStr = Utility.Random( 1, 25 );
            Attributes.BonusInt = Utility.Random( 1, 25 );
            Attributes.BonusDex = Utility.Random( 1, 25 );
	    Attributes.ReflectPhysical = Utility.Random( 1, 25 );
            Attributes.BonusHits = Utility.Random( 1, 25 );
            Attributes.BonusStam = Utility.Random( 1, 25 );
            Attributes.BonusMana = Utility.Random( 1, 25 );
            Attributes.RegenHits = Utility.Random( 1, 25 );
            Attributes.RegenStam = Utility.Random( 1, 25 );
            Attributes.AttackChance = Utility.Random( 1, 25 );
            Attributes.DefendChance = Utility.Random( 1, 25 );
            Attributes.WeaponDamage = Utility.Random( 1, 25 );
            Attributes.SpellDamage = Utility.Random( 1, 25 );
	    Attributes.WeaponSpeed = Utility.Random( 1, 25 );
            ArmorAttributes.MageArmor = 1;
            Attributes.CastSpeed = 5;
            Attributes.CastRecovery = 5;
            Attributes.LowerManaCost = Utility.Random( 1, 25 );
            Attributes.LowerRegCost = Utility.Random( 1, 25 );
        }

        public VirtueCloak(Serial serial) : base( serial )
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
