//###########################################################//
// CREATED BY FOREST CONDON AKA FCONDON AKA EXALE
// MYSTICALNIGHTS.ORG
// PLEASE LEAVE HEADER INTACT.
//###########################################################//

using System;
using Server.AllHues;

namespace Server.Items
{
    public class RareDyeTub : DyeTub
    {
        [Constructable]
        public RareDyeTub()
        {
			Name = "Rare Colored Dyeing Tub";
            Hue = DyedHue = AllHuesInfo.Rare;
            Redyable = false;
        }

        public RareDyeTub( Serial serial ) : base( serial )
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
 