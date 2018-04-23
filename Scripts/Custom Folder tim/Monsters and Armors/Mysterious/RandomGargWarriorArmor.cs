using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x144e, 0x1453 )]
	public class RandomGargWarriorArmor : BaseArmor
	{

        public override Race RequiredRace { get { return Race.Gargoyle; } }
        public override bool CanBeWornByGargoyles { get { return true; } }

        public override int BasePhysicalResistance{ get{ return 5; } }
		public override int BaseFireResistance{ get{ return 5; } }
		public override int BaseColdResistance{ get{ return 5; } }
		public override int BasePoisonResistance{ get{ return 5; } }
		public override int BaseEnergyResistance{ get{ return 5; } }

		public override int InitMinHits{ get{ return 500; } }
		public override int InitMaxHits{ get{ return 500; } }

		public override int AosStrReq{ get{ return 0; } }
		public override int OldStrReq{ get{ return 0; } }

		public override int OldDexBonus{ get{ return -2; } }

		public override int ArmorBase{ get{ return 30; } }
		public override int RevertArmorBase{ get{ return 4; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Plate; } }
		public override CraftResource DefaultResource{ get{ return CraftResource.Iron; } }

		[Constructable]
		public RandomGargWarriorArmor() : base( 0x144E )
		{
			Weight = 2.0;
            Hue = (Utility.Random(1, 1000));
            Attributes.RegenStam = Utility.RandomMinMax(0, 5);
            Attributes.RegenHits = Utility.RandomMinMax(0, 5);
            Attributes.BonusStr = Utility.RandomMinMax(1, 10);
            Attributes.BonusDex = Utility.RandomMinMax(1, 10);
            Attributes.AttackChance = Utility.RandomMinMax(5, 25);
            Attributes.DefendChance = Utility.RandomMinMax(5, 25);
            PhysicalBonus = Utility.RandomMinMax(1, 10);
            ColdBonus = Utility.RandomMinMax(1, 10);
            FireBonus = Utility.RandomMinMax(1, 10);
            PoisonBonus = Utility.RandomMinMax(1, 10);
            EnergyBonus = Utility.RandomMinMax(1, 10);
            Attributes.Luck = Utility.RandomMinMax(100, 250);
            switch ( Utility.Random( 5) )
			{
                //Plate
                case 0: this.ItemID = 0x0302; this.Name = "mysterious Gargish Arms"; this.Layer = Layer.Arms; break;
                case 1: this.ItemID = 0x0304; this.Name = "mysterious Gargish Chest"; this.Layer = Layer.InnerTorso; break;
                case 2: this.ItemID = 0x0311; this.Name = "mysterious Gargish Kilt"; this.Layer = Layer.OuterLegs; break;
                case 3: this.ItemID = 0x0306; this.Name = "mysterious Gargish Legs"; this.Layer = Layer.InnerLegs; break;
                case 4: this.ItemID = 0x457E; this.Name = "mysterious Gargish Wings"; this.Layer = Layer.Cloak; break;
            }
		}



		public RandomGargWarriorArmor( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );

			if ( Weight == 1.0 )
				Weight = 2.0;
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
