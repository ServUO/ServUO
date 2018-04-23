using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class StarterArmor : Bag
	{
		[Constructable]
		public StarterArmor() : this(50)
		{
		}

		[Constructable]
		public StarterArmor( int amount )
		{
            //DropItem(new WornExplorerCap());
            //DropItem(new WornExplorerArms());
            //DropItem(new WornExplorerGorget());
            //DropItem(new WornExplorerGloves());
            //DropItem(new WornExplorerChest());
            //DropItem(new WornExplorerLegs());
            //DropItem(new ApprenticeEarrings());
            //DropItem(new ApprenticeRing());
            //DropItem(new ApprenticeBracelet());
            //DropItem(new ApprenticeSword());
            //DropItem(new ApprenticeShield());
            //DropItem(new EXGargishLeatherArms());
           // DropItnm(new EXGargishLeatherChest());
            //DropItem(new EXGargishLeatherKilt());
           // DropItem(new EXGargishLeatherLegs());
            //DropItem(new EXGargishLeatherWingArmor());
		}

		public StarterArmor( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}