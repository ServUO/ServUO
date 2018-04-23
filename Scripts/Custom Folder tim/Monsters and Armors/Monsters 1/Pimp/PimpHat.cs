/*
*   Scripter : 
*   Author : Lord Mashadow // Avalon Team
*   Generator : Avalon Script Creator
*   Created at :  1/13/2008 6:57:45 PM
*   Thank you for using this tool, feel free to visit our web site  www.avalon.gen.tr
*/
using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class PimpHat : FeatheredHat
    {
        [Constructable]
        public PimpHat()
        {
            this.Weight = 10;
            this.Hue = 18;
            this.Name = "Hat of the Pimp";
	    
            this.Attributes.AttackChance = 15;
            this.Attributes.BonusDex = 15;
            this.Attributes.BonusHits = 10;
            this.Attributes.BonusInt = 15;
            this.Attributes.BonusMana = 10;
            this.Attributes.BonusStam = 10;
            this.Attributes.CastRecovery = 5;
            this.Attributes.CastSpeed = 5;
            this.Attributes.DefendChance = 15;
            this.Attributes.EnhancePotions = 15;
            this.Attributes.LowerManaCost = 15;
            this.Attributes.LowerRegCost = 15;
            this.Attributes.ReflectPhysical = 15;
            this.Attributes.SpellDamage = 15;
            this.Attributes.WeaponDamage = 15;
            this.Attributes.BonusStr = 15;

            this.LootType = LootType.Regular;
        }

        public PimpHat( Serial serial ) : base( serial )
        {
        }

		public override int ArtifactRarity{ get{ return 76; } }
             
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