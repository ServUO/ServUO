using System;
using Server;

namespace Server.Items
{
    public class Indecency : StuddedChest, ITokunoDyable
    {
	public override int ArtifactRarity{ get{ return 19; } }
	
        public override int BasePhysicalResistance{ get{ return 3; } }
        public override int BaseColdResistance{ get{ return 12; } }
        public override int BaseFireResistance{ get{ return 12; } }
        public override int BaseEnergyResistance{ get{ return 13; } }
        public override int BasePoisonResistance{ get{ return 18; } }
       
	   public override int InitMinHits{ get{ return 255; } }
        public override int InitMaxHits{ get{ return 255; } }

        [Constructable]
        public Indecency()
        {
            Name = "Indecency";
            Hue = 2075;
            Attributes.BonusStr = 5;
            Attributes.BonusInt = 5;
            Attributes.BonusDex = 5;
            Attributes.AttackChance = 10;
            Attributes.DefendChance = 10;
            Attributes.Luck = 205;
            Attributes.SpellDamage = 5;
            ArmorAttributes.MageArmor = 1;
            ArmorAttributes.SelfRepair = 4;
            Attributes.LowerManaCost = 8;
            Attributes.LowerRegCost = 20;
        }

        public Indecency(Serial serial) : base( serial )
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
    } // End Class
} // End Namespace
