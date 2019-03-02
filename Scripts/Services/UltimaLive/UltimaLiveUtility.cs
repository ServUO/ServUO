/* Copyright (C) 2013 Ian Karlinsey
 * 
 * UltimeLive is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * UltimaLive is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with UltimaLive.  If not, see <http://www.gnu.org/licenses/>. 
 */

using Server;
using System;
using System.Collections.Generic;

namespace UltimaLive
{
    public sealed class UltimaLiveUtility
    {
        public static List<Point2D> rasterCircle(Point2D center, int radius)
        {
            int x0 = center.X;
            int y0 = center.Y;

            List<Point2D> pointList = new List<Point2D>();
            int f = 1 - radius;
            int ddF_x = 1;
            int ddF_y = -2 * radius;
            int x = 0;
            int y = radius;

            pointList.Add(new Point2D(x0, y0 - radius));
            pointList.Add(new Point2D(x0, y0 + radius));
            pointList.Add(new Point2D(x0 - radius, y0));
            pointList.Add(new Point2D(x0 + radius, y0));

            while (x < y)
            {
                if (f >= 0)
                {
                    y--;
                    ddF_y += 2;
                    f += ddF_y;
                }
                x++;
                ddF_x += 2;
                f += ddF_x;

                Point2D p = new Point2D(x0 + x, y0 + y);
                if (!(pointList.Contains(p)))
                {
                    pointList.Add(p);
                }
                p = new Point2D(x0 - x, y0 + y);
                if (!(pointList.Contains(p)))
                {
                    pointList.Add(p);
                }

                p = new Point2D(x0 + x, y0 - y);
                if (!(pointList.Contains(p)))
                {
                    pointList.Add(p);
                }

                p = new Point2D(x0 - x, y0 - y);
                if (!(pointList.Contains(p)))
                {
                    pointList.Add(p);
                }

                p = new Point2D(x0 + y, y0 + x);
                if (!(pointList.Contains(p)))
                {
                    pointList.Add(p);
                }

                p = new Point2D(x0 - y, y0 + x);
                if (!(pointList.Contains(p)))
                {
                    pointList.Add(p);
                }

                p = new Point2D(x0 + y, y0 - x);
                if (!(pointList.Contains(p)))
                {
                    pointList.Add(p);
                }

                p = new Point2D(x0 - y, y0 - x);
                if (!(pointList.Contains(p)))
                {
                    pointList.Add(p);
                }

            }

            foreach (Point2D p in pointList)
            {
                Console.WriteLine("" + p.X + "," + p.Y);
            }

            return pointList;
        }

        //http://en.wikipedia.org/wiki/Midpoint_circle_algorithm
        public static List<Point2D> rasterFilledCircle(Point2D center, int radius)
        {
            int x0 = center.X;
            int y0 = center.Y;

            List<Point2D> pointList = new List<Point2D>();
            int f = 1 - radius;
            int ddF_x = 1;
            int ddF_y = -2 * radius;
            int x = 0;
            int y = radius;

            for (int h = y0 - radius; h <= y0 + radius; h++)
            {
                Point2D p = new Point2D(x0, h);
                if (!(pointList.Contains(p)))
                {
                    pointList.Add(p);
                }
            }

            for (int h = x0 - radius; h <= x0 + radius; h++)
            {
                Point2D p = new Point2D(h, y0);
                if (!(pointList.Contains(p)))
                {
                    pointList.Add(p);
                }
            }

            while (x < y)
            {
                if (f >= 0)
                {
                    y--;
                    ddF_y += 2;
                    f += ddF_y;
                }
                x++;
                ddF_x += 2;
                f += ddF_x;

                for (int h = x0 - x; h <= x0 + x; h++)
                {
                    Point2D p = new Point2D(h, y0 + y);
                    if (!(pointList.Contains(p)))
                    {
                        pointList.Add(p);
                    }
                }

                for (int h = x0 - x; h <= x0 + x; h++)
                {
                    Point2D p = new Point2D(h, y0 - y);
                    if (!(pointList.Contains(p)))
                    {
                        pointList.Add(p);
                    }
                }

                for (int h = x0 - y; h <= x0 + y; h++)
                {
                    Point2D p = new Point2D(h, y0 + x);
                    if (!(pointList.Contains(p)))
                    {
                        pointList.Add(p);
                    }
                }

                for (int h = x0 - y; h <= x0 + y; h++)
                {
                    Point2D p = new Point2D(h, y0 - x);
                    if (!(pointList.Contains(p)))
                    {
                        pointList.Add(p);
                    }
                }
            }
            return pointList;
        }
    }
}