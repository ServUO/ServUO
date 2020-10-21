using Server.Items;
using Server.Mobiles;
using Server.Multis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Regions
{
    public class CorgulRegion : BaseRegion
    {
        public static void Initialize()
        {
            EventSink.Login += OnLogin;

            Timer.DelayCall(TimeSpan.FromSeconds(30), () =>
            {
                foreach (CorgulRegion reg in Regions.OfType<CorgulRegion>())
                {
                    if (reg.Altar != null && reg.Altar.Activated)
                        continue;

                    foreach (BaseMulti multi in reg.GetEnumeratedMultis())
                    {
                        if (multi is BaseBoat)
                            reg.RemoveBoat((BaseBoat)multi);
                    }
                }
            });
        }

        private List<Item> m_Markers;
        private readonly CorgulAltar m_Altar;
        private Rectangle2D m_Bounds;

        public CorgulAltar Altar => m_Altar;

        public CorgulRegion(Rectangle2D rec, CorgulAltar altar)
            : base("Corgul Boss Region", altar.Map, DefaultPriority, rec)
        {
            //MarkBounds(rec);
            m_Altar = altar;
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
                            i.MoveToWorld(new Point3D(x, y, 0), Map);
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

        public override bool OnBeginSpellCast(Mobile m, ISpell s)
        {
            if (m.AccessLevel == AccessLevel.Player)
            {
                if (s is Spells.Sixth.MarkSpell || s is Spells.Fourth.RecallSpell || s is Spells.Seventh.GateTravelSpell
                || s is Spells.Chivalry.SacredJourneySpell)
                    return false;
            }

            return true;
        }

        public override bool CheckTravel(Mobile m, Point3D newLocation, Spells.TravelCheckType travelType)
        {
            return false;
        }

        public void CheckExit(BaseBoat boat)
        {
            if (boat != null)
                Timer.DelayCall(TimeSpan.FromSeconds(1), new TimerStateCallback(RemoveBoat_Callback), boat);
        }

        public void RemovePlayers(bool message)
        {
            List<Mobile> list = GetMobiles();

            foreach (Mobile m in list)
            {
                if (message && m is PlayerMobile)
                    m.SendMessage("You have failed to meet the deadline.");

                if (BaseBoat.FindBoatAt(m, m.Map) != null)
                    continue;

                if (m is PlayerMobile || (m is BaseCreature && ((BaseCreature)m).Controlled || ((BaseCreature)m).Summoned))
                {
                    Point3D go = CorgulAltar.GetRandomPoint(CorgulAltar.LandKickLocation, Map);
                    BaseCreature.TeleportPets(m, go, Map);
                    m.MoveToWorld(go, Map);
                }

            }

            foreach (BaseBoat b in GetEnumeratedMultis().OfType<BaseBoat>())
            {
                RemoveBoat(b);
            }
        }

        public void RemoveBoat_Callback(object o)
        {
            if (o is BaseBoat)
                RemoveBoat((BaseBoat)o);
        }

        public void RemoveBoat(BaseBoat boat)
        {
            if (boat == null)
                return;

            //First, we'll try and put the boat in the cooresponding location where it warped in
            if (boat.Map != null && boat.Map != Map.Internal && m_Altar != null && m_Altar.WarpRegion != null)
            {
                Map map = boat.Map;
                Rectangle2D rec = m_Altar.WarpRegion.Bounds;

                int x = boat.X - m_Bounds.X;
                int y = boat.Y - m_Bounds.Y;
                int z = map.GetAverageZ(x, y);

                Point3D ePnt = new Point3D(rec.X + x, rec.Y + y, -5);

                int offsetX = ePnt.X - boat.X;
                int offsetY = ePnt.Y - boat.Y;
                int offsetZ = map.GetAverageZ(ePnt.X, ePnt.Y) - boat.Z;

                if (boat.CanFit(ePnt, Map, boat.ItemID))
                {
                    boat.Teleport(offsetX, offsetY, offsetZ);

                    //int z = this.Map.GetAverageZ(boat.X, boat.Y);
                    if (boat.Z != -5)
                        boat.Z = -5;

                    if (boat.TillerMan != null)
                        boat.TillerManSay(501425); //Ar, turbulent water!
                    return;
                }
            }

            //Plan B, lets kick to some random location who-knows-where
            for (int i = 0; i < 25; i++)
            {
                Rectangle2D rec = CorgulAltar.BoatKickLocation;
                Point3D ePnt = CorgulAltar.GetRandomPoint(rec, Map);

                int offsetX = ePnt.X - boat.X;
                int offsetY = ePnt.Y - boat.Y;
                int offsetZ = ePnt.Z - boat.Z;

                if (boat.CanFit(ePnt, Map, boat.ItemID))
                {
                    boat.Teleport(offsetX, offsetY, -5);
                    boat.SendMessageToAllOnBoard("A rough patch of sea has disoriented the crew!");

                    //int z = this.Map.GetAverageZ(boat.X, boat.Y);
                    if (boat.Z != -5)
                        boat.Z = -5;

                    if (boat.TillerMan != null)
                        boat.TillerManSay(501425); //Ar, turbulent water!
                    break;
                }
            }
        }

        public static void OnLogin(LoginEventArgs e)
        {
            Mobile from = e.Mobile;

            Region reg = Find(from.Location, from.Map);

            if (reg is CorgulRegion)
            {
                CorgulAltar altar = ((CorgulRegion)reg).Altar;

                if (altar != null && !altar.Activated)
                {
                    Point3D pnt = CorgulAltar.GetRandomPoint(CorgulAltar.LandKickLocation, from.Map);

                    BaseCreature.TeleportPets(from, pnt, from.Map);
                    from.MoveToWorld(pnt, from.Map);
                }
            }
        }
    }
}
