using System;
using Server.Multis;
using Server.Engines.VeteranRewards;
using Server.Targeting;

namespace Server.Items
{
	public class BackpackDyes : Item
	{
        [Constructable]
        public BackpackDyes()
            : base(0xFA9)
        {
            Name = "Backpack Dyes";
            LootType = LootType.Blessed;
        }

		[Constructable]
		public BackpackDyes( int hue )
            : base(0xFA9)
		{
		    Name = "Backpack Dyes";
		    Hue = hue;
			LootType = LootType.Blessed;
		}

		public override void OnDoubleClick( Mobile from )
		{
		    if (RootParent == from)
		    {
		        from.Target = new InternalTarget(this);
		    }

		    base.OnDoubleClick( from );
		}

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from, "Hue: " + Hue, Hue);
        }

        public BackpackDyes(Serial serial)
            : base(serial)
		{
		}

        private class InternalTarget : Target
        {
            private readonly BackpackDyes _Dyes;

            public InternalTarget(BackpackDyes dyes)
                : base(1, false, TargetFlags.None)
            {
                _Dyes = dyes;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is Container && targeted == from.Backpack)
                {
                    from.Backpack.Hue = _Dyes.Hue;
                    _Dyes.Delete();
                    from.SendMessage(54, "You have dyed your backpack.");
                }
                else
                {
                    from.SendMessage(54, "You must target your backpack with these dyes!");
                }
            }
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