/*
 * The two lines following this entry specify what RunUO version you are running.
 * In order to switch to RunUO 1.0 Final, remove the '//' in front of that setting
 * and add '//' in front of '#define RunUO_2_RC1'.  Warning:  If you comment both
 * out, many commands in this system will not work.  Enjoy!
 */

#define RunUO_2_RC1
//#define RunUO_1_Final

using System;
using System.Collections;
using Server;
using Server.Multis;
using Server.Network;

#if (RunUO_2_RC1)
    using Server.Commands;
#endif

namespace Knives.TownHouses
{
    public class RUOVersion
    {
        private static Hashtable s_Commands = new Hashtable();

        public static void AddCommand(string com, AccessLevel acc, TownHouseCommandHandler cch)
        {
            s_Commands[com.ToLower()] = cch;

            #if(RunUO_1_Final)
                Server.Commands.Register(com, acc, new CommandEventHandler(OnCommand));
            #elif(RunUO_2_RC1)
                Server.Commands.CommandSystem.Register(com, acc, new CommandEventHandler(OnCommand));
            #endif
        }

        public static void OnCommand(CommandEventArgs e)
        {
            if (s_Commands[e.Command.ToLower()] == null)
                return;

            ((TownHouseCommandHandler)s_Commands[e.Command.ToLower()])(new CommandInfo(e.Mobile, e.Command, e.ArgString, e.Arguments));
        }

        public static void UpdateRegion(TownHouseSign sign)
        {
            if (sign.House == null)
                return;

            #if(RunUO_1_Final)
                sign.House.Region.Coords = new ArrayList(sign.Blocks);
                sign.House.Region.MinZ = sign.MinZ;
                sign.House.Region.MaxZ = sign.MaxZ;
                sign.House.Region.Unregister();
                sign.House.Region.Register();
                sign.House.Region.GoLocation = sign.BanLoc;
            #elif(RunUO_2_RC1)
                sign.House.UpdateRegion();

                Rectangle3D rect = new Rectangle3D(Point3D.Zero, Point3D.Zero);

                for (int i = 0; i < sign.House.Region.Area.Length; ++i)
                {
                    rect = sign.House.Region.Area[i];
                    rect = new Rectangle3D(new Point3D(rect.Start.X - sign.House.X, rect.Start.Y - sign.House.Y, sign.MinZ), new Point3D(rect.End.X - sign.House.X, rect.End.Y - sign.House.Y, sign.MaxZ));
                    sign.House.Region.Area[i] = rect;
                }

                sign.House.Region.Unregister();
                sign.House.Region.Register();
                sign.House.Region.GoLocation = sign.BanLoc;
 
            #endif
        }

        public static bool RegionContains(Region region, Mobile m)
        {
            #if(RunUO_1_Final)
                return region.Mobiles.Contains(m);
            #elif(RunUO_2_RC1)
                return region.GetMobiles().Contains(m);
            #endif
        }

        public static Rectangle3D[] RegionArea(Region region)
        {
            #if(RunUO_1_Final)

                Rectangle3D[] rects = new Rectangle3D[region.Coords.Count];
                Rectangle2D rect = new Rectangle2D(Point2D.Zero, Point2D.Zero);

                for (int i = 0; i < rects.Length && i < region.Coords.Count; ++i)
                {
                    rect = (Rectangle2D)region.Coords[i];
                    rects[i] = new Rectangle3D(new Point3D(rect.Start.X, rect.Start.Y, region.MinZ), new Point3D(rect.End.X, rect.End.Y, region.MaxZ));
                }

                return rects;

            #elif(RunUO_2_RC1)
                return region.Area;
            #endif
        }
    }

    public class VersionHouse : BaseHouse
    {
        public VersionHouse(int id, Mobile m, int locks, int secures)
            : base(id, m, locks, secures)
        {
        }

        public override Rectangle2D[] Area { get { return new Rectangle2D[5]; } }

        #if(RunUO_2_RC1)
        
        public override Point3D BaseBanLocation { get { return Point3D.Zero; } }
        
        #endif

        public VersionHouse(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
        }
    }
}