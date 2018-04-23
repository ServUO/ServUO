// Created by Nept

using System;
using Server;

namespace Server.Items
{
    public class MegamiScepter: Scepter
    {
        public override int InitMinHits{ get{ return 255; } }
        public override int InitMaxHits{ get{ return 255; } }
	public override int AosMinDamage{ get{ return 28; } }
	public override int OldMinDamage{ get{ return 28; } }
	public override int AosMaxDamage{ get{ return 34; } }
	public override int OldMaxDamage{ get{ return 34; } }

        [Constructable]
        public MegamiScepter()  
        {
            Name = "Scepter of the Reborn";
            Hue = 2946;
            	WeaponAttributes.HitLeechHits = Utility.Random( 1, 20 );
      		WeaponAttributes.HitLeechMana = Utility.Random( 1, 20 );
      		WeaponAttributes.HitLeechStam = Utility.Random( 1, 20 );
      		WeaponAttributes.SelfRepair = Utility.Random( 1, 20 );
      		Attributes.AttackChance = Utility.Random( 1, 20 );
      		Attributes.BonusDex = Utility.Random( 1, 20 );
      		Attributes.BonusHits = Utility.Random( 1, 10 );
      		Attributes.BonusInt = Utility.Random( 1, 20 );
      		Attributes.BonusMana = Utility.Random( 1, 10 );
      		Attributes.BonusStam = Utility.Random( 1, 20 );
      		Attributes.DefendChance = Utility.Random( 1, 20 );
      		Attributes.WeaponDamage = Utility.Random( 1, 100 );
      		Attributes.WeaponSpeed = Utility.Random( 1, 75 );
        }

        public MegamiScepter(Serial serial) : base( serial )
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
