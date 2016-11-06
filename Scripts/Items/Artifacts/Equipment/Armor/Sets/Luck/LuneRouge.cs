using System;
using Server.Items;

namespace Server.Items
{
	public class LuneRouge : GoldRing
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber{ get{ return 1154372; } } // Lune Rouge
        public override SetItem SetID{ get{ return SetItem.Luck2; } }
		public override int Pieces{ get{ return 2; } }
		[Constructable]
		public LuneRouge() : base()
		{
			this.Weight = 1.0;
            this.Hue = 1166;

            this.Attributes.Luck = 150;
            this.Attributes.AttackChance = 10;
            this.Attributes.WeaponDamage = 20;

            this.SetHue = 1166;
            this.SetAttributes.Luck = 100;
            this.SetAttributes.AttackChance = 10;
            this.SetAttributes.WeaponDamage = 20;
            this.SetAttributes.WeaponSpeed = 10;
            this.SetAttributes.RegenHits = 2;
            this.SetAttributes.RegenStam = 3;
        }

		public LuneRouge( Serial serial ) : base( serial )
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
