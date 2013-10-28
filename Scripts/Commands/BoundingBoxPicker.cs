using System;
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
                this.m_Store = store;
                this.m_First = first;
                this.m_Map = map;
                this.m_Callback = callback;
                this.m_State = state;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                IPoint3D p = targeted as IPoint3D;

                if (p == null)
                    return;
                else if (p is Item)
                    p = ((Item)p).GetWorldTop();

                if (this.m_First)
                {
                    from.SendMessage("Target another location to complete the bounding box.");
                    from.Target = new PickTarget(new Point3D(p), false, from.Map, this.m_Callback, this.m_State);
                }
                else if (from.Map != this.m_Map)
                {
                    from.SendMessage("Both locations must reside on the same map.");
                }
                else if (this.m_Map != null && this.m_Map != Map.Internal && this.m_Callback != null)
                {
                    Point3D start = this.m_Store;
                    Point3D end = new Point3D(p);

                    Utility.FixPoints(ref start, ref end);

                    this.m_Callback(from, this.m_Map, start, end, this.m_State);
                }
            }
        }
    }
}