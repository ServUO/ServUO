/**************************************
*Script Name: Staff Runebook          *
*Author: Joeku                        *
*For use with RunUO 2.0 RC2           *
*Client Tested with: 6.0.9.2          *
*Version: 1.10                        *
*Initial Release: 11/25/07            *
*Revision Date: 02/04/09              *
**************************************/

using System;
using Server;
using Server.Accounting;
using Server.Gumps;

namespace Joeku.SR
{
    public class SR_Utilities
    {
        public static bool FindItem(Type type, Point3D p, Map map)
        {
            return FindEntity(type, p, map, false);
        }

        public static bool FindMobile(Type type, Point3D p, Map map)
        {
            return FindEntity(type, p, map, true);
        }
		
        public static bool FindEntity(Type type, Point3D p, Map map, bool mob)
        {
            IPooledEnumerable loc;
            Rectangle2D rect = new Rectangle2D(p.X, p.Y, 1, 1);
            if (mob)
                loc = map.GetMobilesInBounds(rect);
            else
                loc = map.GetItemsInBounds(rect);

            bool found = false;

            try
            {
                foreach (object o in loc)
                    if (o != null && o.GetType() == type || o.GetType().IsSubclassOf(type))
                    {
                        found = true;
                        break;
                    }
            }
            catch
            {
            }

            loc.Free();

            return found;
        }

        public static SR_RuneAccount FetchInfo(IAccount acc)
        {
            return FetchInfo(acc as Account);
        }

        public static SR_RuneAccount FetchInfo(Account acc)
        {
            return FetchInfo(acc.Username);
        }

        public static SR_RuneAccount FetchInfo(string username)
        {
            SR_RuneAccount runeAcc = null;

            for (int i = 0; i < SR_Main.Count; i++)
                if (SR_Main.Info[i].Username == username)
                {
                    runeAcc = SR_Main.Info[i];
                    break;
                }

            if (runeAcc == null)
            {
                runeAcc = new SR_RuneAccount(username);
                NewRuneAcc(runeAcc);
            }

            return runeAcc;
        }

        public static int RunebookID = 8901;
        public static int RuneID = 7956;

        public static int ItemOffsetY(SR_Rune rune)
        {
            if (rune.IsRunebook)
                return -1;
            return 3;
        }

        public static int ItemOffsetX(SR_Rune rune)
        {
            if (rune.IsRunebook)
                return -1;
            return -2;
        }

        public static int ItemHue(SR_Rune rune)
        {
            int hue = 0;

            if (rune.IsRunebook)
                hue = 1121;
            else
                hue = RuneHues[MapInt(rune.TargetMap) /*+ (rune.House != null) ? 5 : 0*/];

            return hue;
        }

        private static readonly int[] RuneHues = new int[] { 0, 50, 1102, 1102, 1154, 0x66D, 0x47F, 0x55F, 0x55F, 0x47F };

        // To do: check for valid Z (?)
        public static bool CheckValid(Point3D loc, Map map)
        {
            Point2D dim = MapDimensions[MapInt(map)];

            if (loc.X < 0 || loc.Y < 0 || loc.X > dim.X || loc.Y > dim.Y)
                return false;

            return true;
        }

        private static readonly Point2D[] MapDimensions = new Point2D[]
        {
            new Point2D(7168, 4096), // Felucca
            new Point2D(7168, 4096), // Trammel
            new Point2D(2304, 1600), // Ilshenar
            new Point2D(2560, 2048), // Malas
            new Point2D(1448, 1448), // Tokuno
            #region SA
            new Point2D(1280, 4096)// TerMur
            #endregion
        };

        public static int MapInt(Map map)
        {
            int i = 0;

            if (map == Map.Felucca)
                i = 0;
            else if (map == Map.Trammel)
                i = 1;
            else if (map == Map.Ilshenar)
                i = 2;
            else if (map == Map.Malas)
                i = 3;
            else if (map == Map.Tokuno)
                i = 4;
            #region SA
            else if (map == Map.TerMur)
                i = 5;
            #endregion

            return i;
        }

        public static void NewRuneAcc(SR_RuneAccount acc)
        {
            acc.Clear();

            acc.AddRune(AddTree(GoGump.Felucca, Map.Felucca));
            acc.AddRune(AddTree(GoGump.Trammel, Map.Trammel));
            acc.AddRune(AddTree(GoGump.Ilshenar, Map.Ilshenar));
            acc.AddRune(AddTree(GoGump.Malas, Map.Malas));
            acc.AddRune(AddTree(GoGump.Tokuno, Map.Tokuno));
            #region SA
            acc.AddRune(AddTree(GoGump.TerMur, Map.TerMur));
            #endregion
        }

        private static SR_Rune AddTree(LocationTree tree, Map map)
        {
            SR_Rune runeBook = new SR_Rune(map.ToString(), true);

            for (int i = 0; i < tree.Root.Children.Length; i++)
                runeBook.AddRune(AddNode(tree.Root.Children[i], map));

            return runeBook;
        }

        private static SR_Rune AddNode(object o, Map map)
        {
            if (o is ParentNode)
            {
                ParentNode parentNode = o as ParentNode;
                SR_Rune runeBook = new SR_Rune(parentNode.Name, true);

                for (int i = 0; i < parentNode.Children.Length; i++)
                    runeBook.AddRune(AddNode(parentNode.Children[i], map));

                return runeBook;
            }

            ChildNode childNode = o as ChildNode;

            return new SR_Rune(childNode.Name, map, childNode.Location);
        }
    }
}