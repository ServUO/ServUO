using Server.Targeting;

namespace Server
{
    public delegate void BoundingBoxCallback(Mobile from, Map map, Point3D start, Point3D end, object state);

    public class BoundingBoxPicker
    {
        public static void Begin(Mobile from, BoundingBoxCallback callback, object state)
        {
            from.SendMessage("Target the first location of the bounding box.");
            from.Target = new PickTarget(callback, state);
        }

        private class PickTarget : Target
        {
            private readonly Point3D m_Store;
            private readonly bool m_First;
            private readonly Map m_Map;
            private readonly BoundingBoxCallback m_Callback;
            private readonly object m_State;
            public PickTarget(BoundingBoxCallback callback, object state)
                : this(Point3D.Zero, true, null, callback, state)
            {
            }

            public PickTarget(Point3D store, bool first, Map map, BoundingBoxCallback callback, object state)
                : base(-1, true, TargetFlags.None)
            {
                m_Store = store;
                m_First = first;
                m_Map = map;
                m_Callback = callback;
                m_State = state;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                IPoint3D p = targeted as IPoint3D;

                if (p == null)
                    return;
                else if (p is Item)
                    p = ((Item)p).GetWorldTop();

                if (m_First)
                {
                    from.SendMessage("Target another location to complete the bounding box.");
                    from.Target = new PickTarget(new Point3D(p), false, from.Map, m_Callback, m_State);
                }
                else if (from.Map != m_Map)
                {
                    from.SendMessage("Both locations must reside on the same map.");
                }
                else if (m_Map != null && m_Map != Map.Internal && m_Callback != null)
                {
                    Point3D start = m_Store;
                    Point3D end = new Point3D(p);

                    Utility.FixPoints(ref start, ref end);

                    m_Callback(from, m_Map, start, end, m_State);
                }
            }
        }
    }
}