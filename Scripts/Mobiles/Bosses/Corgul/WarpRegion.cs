using Server.Items;
using Server.Mobiles;
using Server.Multis;
using System.Collections.Generic;
using System.Linq;

namespace Server.Regions
{
    public class CorgulWarpRegion : Region
    {
        private Rectangle2D m_Bounds;
        private List<Item> m_Markers;

        public Rectangle2D Bounds => m_Bounds;

        public CorgulWarpRegion(CorgulAltar ped, Rectangle2D rec)
            : base("Corgul Warp Region", ped.Map, DefaultPriority, rec)
        {
            m_Bounds = rec;
        }

        public void MarkBounds(Rectangle2D rec)
        {
            m_Markers = new List<Item>();

            int w = rec.X + rec.Width;
            int h = rec.Y + rec.Height;
            int t = 0;

            for (int x = rec.X; x <= w; x++)
            {
                for (int y = rec.Y; y <= h; y++)
                {
                    if (x == rec.X || x == rec.X + rec.Width || y == rec.Y || y == rec.Y + rec.Height)
                    {
                        if (t >= 10)
                        {
                            MarkerItem i = new MarkerItem(14089);
                            i.MoveToWorld(new Point3D(x, y, -5), Map);
                            m_Markers.Add(i);
                            t = 0;
                        }
                        else
                            t++;
                    }
                }
            }
        }

        public override void OnUnregister()
        {
            if (m_Markers == null)
                return;

            foreach (Item i in m_Markers)
                i.Delete();

            m_Markers.Clear();
        }

        public void CheckEnter(BaseBoat boat)
        {
            if (boat == null || Map == null || Map == Map.Internal)
                return;

            //Do not enter corgul region if we aren't in this region anymore
            Region r = Find(boat.Location, boat.Map);
            if (r != null && !r.IsPartOf(this))
                return;

            Map map = Map;

            List<PlayerMobile> pms = new List<PlayerMobile>();
            bool hasMap = false;

            foreach (PlayerMobile i in boat.GetEntitiesOnBoard().OfType<PlayerMobile>().Where(pm => pm.NetState != null))
            {
                pms.Add(i);
                PlayerMobile pm = i;

                if (pm.Backpack == null)
                    continue;

                Item item = pm.Backpack.FindItemByType(typeof(CorgulIslandMap));
                if (item != null && item is CorgulIslandMap && Contains(((CorgulIslandMap)item).DestinationPoint))
                {
                    hasMap = true;
                    break;
                }
            }

            if (hasMap)
            {
                int x = boat.X - m_Bounds.X;
                int y = boat.Y - m_Bounds.Y;
                int z = map.GetAverageZ(x, y);

                Point3D ePnt = new Point3D(CorgulAltar.CorgulBounds.X + x, CorgulAltar.CorgulBounds.Y + y, 0);

                int offsetX = ePnt.X - boat.X;
                int offsetY = ePnt.Y - boat.Y;
                int offsetZ = map.GetAverageZ(ePnt.X, ePnt.Y) - boat.Z;

                if (boat.CanFit(ePnt, Map, boat.ItemID))
                {
                    boat.Teleport(offsetX, offsetY, offsetZ);

                    //int z = this.Map.GetAverageZ(boat.X, boat.Y);
                    if (boat.Z != 0)
                        boat.Z = 0;

                    if (boat.TillerMan != null)
                        boat.TillerManSay(501425); //Ar, turbulent water!
                }
                else
                {
                    boat.StopMove(true);
                    boat.SendMessageToAllOnBoard("The boat has struck a coral reef!");
                }

            }
        }
    }

    public class MarkerItem : Static
    {
        public MarkerItem(int itemID) : base(itemID)
        {
            Hue = 1234;
        }

        public MarkerItem(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Delete();
        }
    }
}
