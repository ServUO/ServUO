using Server.Items;

namespace Server.Mobiles
{
	public class WanderingShaman : WanderingHealer
	{
        public override bool ChangeRace { get { return false; } }

		[Constructable]
		public WanderingShaman()
		{
            Name = NameList.RandomName("savage");
			Title = "the wandering shaman";
            Hue = 0;

            HairItemID = 0;
            FacialHairItemID = 0;

			var i = Items.Count;

			while (--i >= 0)
			{
				if (i < Items.Count && Items[i] is BaseClothing)
					Items[i].Delete();
			}

			SetWearable( new TribalMask(), 2500 );
            SetWearable( new BoneArms() );
            SetWearable( new BoneLegs() );
		}

		public WanderingShaman( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			var v = reader.ReadInt();

			if (v > 0)
			{
				return;
			}

			var robe = FindItemOnLayer(Layer.OuterTorso) as Robe;

			if (robe != null)
			{
				Timer.DelayCall(robe.Delete);
			}
		}
	}
}