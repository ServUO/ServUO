//created by thors hammer//

using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class OrcKingSkull : BaseArmor
    {   
        	public override int ArtifactRarity{ get{ return 333; } }
        	public override int BasePhysicalResistance{ get{ return 50; } }
        	public override int BaseFireResistance{ get{ return 15; } }
        	public override int BaseColdResistance{ get{ return 15; } }
        	public override int BasePoisonResistance{ get{ return 10; } }
        	public override int BaseEnergyResistance{ get{ return 10; } }
	
		public override int AosStrReq{ get{ return 30; } }
		public override int OldStrReq{ get{ return 10; } }

		public override int ArmorBase{ get{ return 20; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Leather; } }
		public override CraftResource DefaultResource{ get{ return CraftResource.RegularLeather; } }

		public override ArmorMeditationAllowance DefMedAllowance{ get{ return ArmorMeditationAllowance.All; } }

        	public override int InitMinHits { get { return 255; } }
        	public override int InitMaxHits { get { return 255; } }
        
        [Constructable]
        public OrcKingSkull() : base( 0x1F0B )
        {
            Name = "Skull of the Orc King";
            Hue = 0x198;
            ArmorAttributes.SelfRepair = 10;
            ArmorAttributes.DurabilityBonus = 255;
            Attributes.AttackChance= 20;
            Attributes.CastRecovery = 5;
            Attributes.CastSpeed = 5;
            Attributes.DefendChance = 20;
            Attributes.ReflectPhysical = 50;
            Attributes.BonusStr = 20;
            Attributes.BonusStam = 15;
            Attributes.BonusDex = 15;
            Attributes.BonusHits = 20;
            Attributes.RegenHits = 20;
            LootType = LootType.Blessed;

            Weight = 1;
        }

        public OrcKingSkull( Serial serial ) : base( serial )
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

