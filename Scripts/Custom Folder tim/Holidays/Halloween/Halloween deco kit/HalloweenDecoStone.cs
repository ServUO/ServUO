/* Created by Hammerhand */
using System;
using Server.Items;

namespace Server.Items
{
	public class HalloweenDecoStone : Item
	{
		public override string DefaultName
		{
			get { return "a Halloween Decorating Supply Stone"; }
		}

		[Constructable]
		public HalloweenDecoStone() : base( 4476 )
		{
			Movable = false;
			Hue = 2114;
		}

		public override void OnDoubleClick( Mobile from )
		{
			HalloweenDecoBag halloweendecoBag = new HalloweenDecoBag();

            if (!from.AddToBackpack(halloweendecoBag))
                halloweendecoBag.Delete();
		}

        public HalloweenDecoStone(Serial serial)
            : base(serial)
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