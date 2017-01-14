using System;
using Server;
using Server.Targeting;
using Server.Engines.Plants;

namespace Server.Items
{
	public class Hoe : Item
	{
        public override int LabelNumber { get { return 1150482; } } // hoe

		[Constructable]
		public Hoe() : base( 3897 )
		{
		}

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.Target = new InternalTarget(this);
                from.SendMessage("Target the land you would like to till.");
            }
        }

        private class InternalTarget : Target
        {
            private Hoe m_Hoe;

            public InternalTarget(Hoe hoe) : base(2, true, TargetFlags.None)
            {
                m_Hoe = hoe;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                Map map = from.Map;

                if (targeted is LandTarget && map != null)
                {
                    LandTarget lt = (LandTarget)targeted;
                    Region r = Region.Find(lt.Location, map);

                    if (r != null && r.IsPartOf("Magincia") && (lt.Name == "dirt" || lt.Name == "grass"))
                    {
                        if (!MaginciaPlantSystem.Enabled)
                            from.SendMessage("Magincia plant placement is currently disabled.");
                        else if (MaginciaPlantSystem.CanAddPlant(from, lt.Location))
                        {
                            int fertileDirt = from.Backpack == null ? 0 : from.Backpack.GetAmount(typeof(FertileDirt), false);

                            if (fertileDirt > 0)
                                from.SendGump(new FertileDirtGump(null, fertileDirt, lt));
                            else
                            {
                                if (from.Body.IsHuman && !from.Mounted)
                                    from.Animate(11, 5, 1, true, false, 0);

                                from.PlaySound(0x125);

                                MaginciaPlantItem dirt = new MaginciaPlantItem();
                                dirt.StartTimer();

                                Timer.DelayCall(TimeSpan.FromSeconds(.7), new TimerStateCallback(MoveItem_Callback), new object[] { dirt, lt.Location, map } );
                            }
                        }
                    }
                    else
                        from.SendLocalizedMessage(1150457); // The ground here is not good for gardening.
                    
                }
            }

            private void MoveItem_Callback(object o)
            {
                object[] objs = o as object[];

                if (objs != null)
                {
                    Item dirt = objs[0] as Item;
                    Point3D p = (Point3D)objs[1];
                    Map map = objs[2] as Map;

                    if (dirt != null)
                        dirt.MoveToWorld(p, map);
                }
            }
        }

		public Hoe( Serial serial ) : base( serial )
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