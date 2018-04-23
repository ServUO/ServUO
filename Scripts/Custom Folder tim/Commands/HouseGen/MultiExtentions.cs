using Server.Items;
using System.Collections.Generic;

namespace Server.Multis
{
    public static class MultiExtentions
    {
        public static List<Rectangle2D> Rects(this BaseMulti multi)
        {
            MultiComponentList m = multi.Components;
            var rects = new List<Rectangle2D>();
            var filledRect = FillFrom(m);
            Rectangle2D r;

            while (Area(r = GetLargestRect(filledRect)) > 0)
            {
                Subtract(filledRect, r);
                r = new Rectangle2D(
                    new Point2D(r.Start.X - m.Center.X, r.Start.Y - m.Center.Y),
                    new Point2D(r.End.X - m.Center.X, r.End.Y - m.Center.Y));
                rects.Add(r);
            }
            return rects;
        }

        private static void Subtract(bool[][] filledRect, Rectangle2D r)
        {
            for (int x = r.X; x < r.X + r.Width; x++)
                for (int y = r.Y; y < r.Y + r.Height; y++)
                    filledRect[x][y] = false;
        }

        private static Rectangle2D GetLargestRect(bool[][] filledRect)
        {
            var best = new Rectangle2D(0, 0, -1, -1);
            var N = filledRect.Length;
            var M = filledRect[0].Length;
            var c = new int[M + 1];
            var stack = new Stack<Line>();
            Line l = new Line(0, 0);
            for (var x = N - 1; x >= 0; x--)
            {
                Update_Cache(filledRect, c, x);
                var width = 0;
                for (var y = 0; y < M + 1; y++)
                {
                    if (c[y] > width)
                    {
                        stack.Push(new Line(y, width));
                        width = c[y];
                    }
                    if (c[y] < width)
                    {
                        while (c[y] < width)
                        {
                            if (stack.Count < 1)
                                break;

                            l = stack.Pop();
                            if (width * (y - l.Y) > Area(best))
                                best = new Rectangle2D(new Point2D(x, l.Y), new Point2D(x + width - 1, y - 1));
                            width = l.Width;
                        }
                        width = c[y];
                        if (width != 0)
                            stack.Push(new Line(l.Y, width));
                    }
                }
            }

            var fixOffByOneWH = new Rectangle2D(best.X, best.Y, best.Width + 1, best.Height + 1);

            return fixOffByOneWH;
        }

        private class Line
        {
            public int Y;
            public int Width;
            public Line(int y, int width) { Y = y; Width = width; }
        }

        private static int Area(Rectangle2D r)
        {
            if (r.Start.X > r.End.X || r.Start.Y > r.End.Y)
                return 0;
            return (r.Width) * (r.Height);
        }

        private static void Update_Cache(bool[][] filledRect, int[] c, int x)
        {
            for (var y = 0; y < filledRect[0].Length; y++)
            {
                if (filledRect[x][y])
                    c[y] = c[y] + 1;
                else
                    c[y] = 0;
            }
        }

        private static bool[][] FillFrom(MultiComponentList m)
        {
            var filled = new bool[m.Width][];
            for (int i = 0; i < m.Width; i++)
                filled[i] = new bool[m.Height];

            foreach (var c in m.List)
                filled[c.m_OffsetX + m.Center.X][c.m_OffsetY + m.Center.Y] = true;

            return filled;
        }
    }
}