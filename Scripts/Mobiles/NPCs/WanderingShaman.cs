using System;
using Server;
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

            this.Items.ForEach(i =>
                {
                    if (i is BaseClothing)
                        i.Delete();
                });

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

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            Timer.DelayCall(TimeSpan.FromSeconds(10), () =>
            {
                Item robe = FindItemOnLayer(Layer.OuterTorso);
                if (robe != null && robe is Robe)
                    robe.Delete();
            });
		}
	}
}