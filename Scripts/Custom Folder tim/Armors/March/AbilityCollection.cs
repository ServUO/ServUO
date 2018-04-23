/* Created by M_0_h 
   Please do not redistribute this file without permission
   Feel free to modify the file for your own use as you please */
using System;
using System.Collections;
using Server.Items;
using Server.Mobiles;
using Server.Spells;
using Server.Misc;
using Server.Targeting;


namespace Server
{
    public static class AbilityCollection
    {
        #region ScreamOfHell
        public static void ScreamOfHell(Mobile from)
        {
            ScreamOfHell(from, 12, TimeSpan.FromMinutes(1));
        }
        public static void ScreamOfHell(Mobile from, int range)
        {
            ScreamOfHell(from, range, TimeSpan.FromMinutes(1));
        }
        public static void ScreamOfHell(Mobile from, int range, TimeSpan duration)
        {
            if (from.Map != Map.Internal && from.Map != null && from != null && !from.Blessed && from.Alive)
            {
                IPooledEnumerable eable = from.GetMobilesInRange(range);
                
                foreach (Mobile m in eable)
                {
                    if (m is BaseCreature && !m.Blessed && m.Alive && ((BaseCreature)from).IsEnemy(m) && from.CanBeHarmful(m))
                    {
                        BaseCreature b = m as BaseCreature;

                        if (b.Controlled && b != from)
                        {
                            b.Combatant = from;
                            b.BeginFlee(duration);
                        }
                    }
                }
            }
        }
        #endregion

        #region AreaDmg
        public static void AreaDmg(Mobile from, int dmgMin, int dmgMax, int physDmg, int fireDmg, int nrgyDmg, int coldDmg, int poisDmg, int range)
        {
            if (from == null || from.Map == Map.Internal || from.Map == null || from.Deleted || from.Blessed)
                return;

            IPooledEnumerable eable = from.GetMobilesInRange(range);

            foreach (Mobile m in eable)
            {
                if (m is BaseCreature)
                {
                    BaseCreature b = m as BaseCreature;

                    if (b.Alive && !b.Blessed && !b.IsDeadBondedPet && b.CanBeHarmful(from) && ((BaseCreature)from).IsEnemy(b) && b.Map != null && b.Map != Map.Internal && b != null)
                    {
                        AOS.Damage(b, from, Utility.RandomMinMax(dmgMin, dmgMax), physDmg, fireDmg, coldDmg, poisDmg, nrgyDmg);
                    }
                }
                else if (m is PlayerMobile)
                {
                    PlayerMobile p = m as PlayerMobile;

                    if (p.Alive && !p.Blessed && p.AccessLevel == AccessLevel.Player && p.Map != null && p.Map != Map.Internal && p != null)
                    {
                        AOS.Damage(p, from, Utility.RandomMinMax(dmgMin, dmgMax), physDmg, fireDmg, coldDmg, poisDmg, nrgyDmg);
                    }
                }
            }
        }
        #endregion

        public static void AreaEffect(TimeSpan delayMin, TimeSpan delayMax, double delayBreak, Map map, Point3D loc, int itemID, int duration, int hue, int renderMode)
        {
            AreaEffect(delayMin, delayMax, delayBreak, map, loc, itemID, duration, hue, renderMode, 12, 0, true, false, false);
        }

        public static void AreaEffect(TimeSpan delayMin, TimeSpan delayMax, double delayBreak, Map map, Point3D loc, int itemID, int duration, int hue, int renderMode, int range, int height, bool inToOut, bool ascending, bool descending)
        {
            for (int x = (range * -1); x <= range; ++x)
            {
                for (int y = (range * -1); y <= range; ++y)
                {
                    double dist = Math.Sqrt(x * x + y * y);

                    if (dist <= range)
                    {
                        Point3D loc2;

                        if (inToOut)
                        {
                            if (ascending)
                                loc2 = new Point3D(loc.X + x, loc.Y + y, (int)(loc.Z + (dist * height)));

                            else if (descending)
                                loc2 = new Point3D(loc.X + x, loc.Y + y, (int)(loc.Z + (range - (dist * height))));

                            else
                                loc2 = new Point3D(loc.X + x, loc.Y + y, loc.Z);

                            new EffectTimer(delayMin + TimeSpan.FromSeconds(dist * delayBreak), delayMax + TimeSpan.FromSeconds(dist * delayBreak), map, loc2, itemID, duration, hue, renderMode).Start();
                        }
                        else
                        {
                            if (ascending)
                                loc2 = new Point3D(loc.X + x, loc.Y + y, (int)(loc.Z + (dist * height)));

                            else if (descending)
                                loc2 = new Point3D(loc.X + x, loc.Y + y, (int)(loc.Z + (range - (dist * height))));

                            else
                                loc2 = new Point3D(loc.X + x, loc.Y + y, loc.Z);

                            new EffectTimer(delayMin + TimeSpan.FromSeconds( range - (dist * delayBreak)), delayMax + TimeSpan.FromSeconds(range - (dist * delayBreak)), map, loc2, itemID, duration, hue, renderMode).Start();
                        }
                    }
                }
            }
        }

        public class EffectTimer : Timer
        {
            private int m_ItemID, m_Duration, m_Hue, m_RenderMode;
            private Map m_Map;
            private Point3D m_Location;

            public EffectTimer(TimeSpan delayMin, TimeSpan delayMax, Map map, Point3D loc, int itemID, int duration, int hue, int renderMode)
                : base(delayMin, delayMax)
            {
                m_ItemID = itemID;
                m_Duration = duration;
                m_Hue = hue;
                m_RenderMode = renderMode;
                m_Map = map;
                m_Location = loc;
            }

            public EffectTimer(TimeSpan delayMin, TimeSpan delayMax, Map map, Point3D loc, int itemID, int duration)
                : this(delayMin, delayMax, map, loc, itemID, duration, 0, 0)
            {
            }

            protected override void OnTick()
            {
                Effects.SendLocationEffect(m_Location, m_Map, m_ItemID, m_Duration, m_Hue, m_RenderMode);
                Stop();
            }
        }
    }
}