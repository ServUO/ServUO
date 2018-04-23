using System;
using Server;
using Server.Targeting;
using Server.Commands;

namespace Server.Engines.UOArchitect
{
	public delegate void BoundingBoxExCancelled();

	public class BoundingBoxPickerEx
	{
		public BoundingBoxExCancelled OnCancelled;

		public void Begin( Mobile from, BoundingBoxCallback callback, object state )
		{
			from.SendMessage( "Target the first location of the bounding box." );

			PickTarget target = new PickTarget(callback, state);
			target.OnCancelled += new BoundingBoxExCancelled(OnTargetCancelled);
			from.Target = target;
		}

		private void OnTargetCancelled()
		{
			if(OnCancelled != null)
				OnCancelled();
		}

		private class PickTarget : Target
		{
			public BoundingBoxExCancelled OnCancelled;
			private Point3D m_Store;
			private bool m_First;
			private Map m_Map;
			private BoundingBoxCallback m_Callback;
			private object m_State;

			public PickTarget( BoundingBoxCallback callback, object state ) : this( Point3D.Zero, true, null, callback, state )
			{
			}

			public PickTarget( Point3D store, bool first, Map map, BoundingBoxCallback callback, object state ) : base( -1, true, TargetFlags.None )
			{
				m_Store = store;
				m_First = first;
				m_Map = map;
				m_Callback = callback;
				m_State = state;
			}

			protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
			{
				base.OnTargetCancel (from, cancelType);

				if(OnCancelled != null)
					OnCancelled();
			}

			private void OnTargetCancelled()
			{
				if(OnCancelled != null)
					OnCancelled();
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				IPoint3D p = targeted as IPoint3D;

				if ( p == null )
					return;
				else if ( p is Item )
					p = ((Item)p).GetWorldTop();

				if ( m_First )
				{
					from.SendMessage( "Target another location to complete the bounding box." );
					PickTarget target = new PickTarget( new Point3D( p ), false, from.Map, m_Callback, m_State );
					target.OnCancelled += new BoundingBoxExCancelled(OnTargetCancelled);
					from.Target = target;
				}
				else if ( from.Map != m_Map )
				{
					from.SendMessage( "Both locations must reside on the same map." );
				}
				else if ( m_Map != null && m_Map != Map.Internal && m_Callback != null )
				{
					Point3D start = m_Store;
					Point3D end = new Point3D( p );

					Utility.FixPoints( ref start, ref end );

					m_Callback( from, m_Map, start, end, m_State );
				}
			}
		}
	}
}
