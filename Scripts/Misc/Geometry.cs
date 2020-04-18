using System;

namespace Server.Misc
{
    public delegate void DoEffect_Callback(Point3D p, Map map);

    public static class Geometry
    {
        public static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        public static double RadiansToDegrees(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        public static double DegreesToRadians(double angle)
        {
            return angle * (Math.PI / 180.0);
        }

        public static Point2D ArcPoint(Point3D loc, int radius, int angle)
        {
            int sideA, sideB;

            if (angle < 0)
                angle = 0;

            if (angle > 90)
                angle = 90;

            sideA = (int)Math.Round(radius * Math.Sin(DegreesToRadians(angle)));
            sideB = (int)Math.Round(radius * Math.Cos(DegreesToRadians(angle)));

            return new Point2D(loc.X - sideB, loc.Y - sideA);
        }

        public static void Circle2D(Point3D loc, Map map, int radius, DoEffect_Callback effect)
        {
            Circle2D(loc, map, radius, effect, 0, 360);
        }

        public static void Circle2D(Point3D loc, Map map, int radius, DoEffect_Callback effect, int angleStart, int angleEnd)
        {
            if (angleStart < 0 || angleStart > 360)
                angleStart = 0;

            if (angleEnd > 360 || angleEnd < 0)
                angleEnd = 360;

            if (angleStart == angleEnd)
                return;

            bool opposite = angleStart > angleEnd;

            int startQuadrant = angleStart / 90;
            int endQuadrant = angleEnd / 90;

            Point2D start = ArcPoint(loc, radius, angleStart % 90);
            Point2D end = ArcPoint(loc, radius, angleEnd % 90);

            if (opposite)
            {
                Swap(ref start, ref end);
                Swap(ref startQuadrant, ref endQuadrant);
            }

            CirclePoint startPoint = new CirclePoint(start, angleStart, startQuadrant);
            CirclePoint endPoint = new CirclePoint(end, angleEnd, endQuadrant);

            int error = -radius;
            int x = radius;
            int y = 0;

            while (x > y)
            {
                plot4points(loc, map, x, y, startPoint, endPoint, effect, opposite);
                plot4points(loc, map, y, x, startPoint, endPoint, effect, opposite);

                error += (y * 2) + 1;
                ++y;

                if (error >= 0)
                {
                    --x;
                    error -= x * 2;
                }
            }

            plot4points(loc, map, x, y, startPoint, endPoint, effect, opposite);
        }

        public static void plot4points(Point3D loc, Map map, int x, int y, CirclePoint start, CirclePoint end, DoEffect_Callback effect, bool opposite)
        {
            Point2D pointA = new Point2D(loc.X - x, loc.Y - y);
            Point2D pointB = new Point2D(loc.X - y, loc.Y - x);

            int quadrant = 2;

            if (x == 0 && start.Quadrant == 3)
                quadrant = 3;

            if (WithinCircleBounds(quadrant == 3 ? pointB : pointA, quadrant, loc, start, end, opposite))
                effect(new Point3D(loc.X + x, loc.Y + y, loc.Z), map);

            quadrant = 3;

            if (y == 0 && start.Quadrant == 0)
                quadrant = 0;

            if (x != 0 && WithinCircleBounds(quadrant == 0 ? pointA : pointB, quadrant, loc, start, end, opposite))
                effect(new Point3D(loc.X - x, loc.Y + y, loc.Z), map);
            if (y != 0 && WithinCircleBounds(pointB, 1, loc, start, end, opposite))
                effect(new Point3D(loc.X + x, loc.Y - y, loc.Z), map);
            if (x != 0 && y != 0 && WithinCircleBounds(pointA, 0, loc, start, end, opposite))
                effect(new Point3D(loc.X - x, loc.Y - y, loc.Z), map);
        }

        public static bool WithinCircleBounds(Point2D pointLoc, int pointQuadrant, Point3D center, CirclePoint start, CirclePoint end, bool opposite)
        {
            if (start.Angle == 0 && end.Angle == 360)
                return true;

            int startX = start.Point.X;
            int startY = start.Point.Y;
            int endX = end.Point.X;
            int endY = end.Point.Y;

            int x = pointLoc.X;
            int y = pointLoc.Y;

            if (pointQuadrant < start.Quadrant || pointQuadrant > end.Quadrant)
                return opposite;

            if (pointQuadrant > start.Quadrant && pointQuadrant < end.Quadrant)
                return !opposite;

            bool withinBounds = true;

            if (start.Quadrant == end.Quadrant)
            {
                if (startX == endX && (x > startX || y > startY || y < endY))
                    withinBounds = false;
                else if (startY == endY && (y < startY || x < startX || x > endX))
                    withinBounds = false;
                else if (x < startX || x > endX || y > startY || y < endY)
                    withinBounds = false;
            }
            else if (pointQuadrant == start.Quadrant && (x < startX || y > startY))
                withinBounds = false;
            else if (pointQuadrant == end.Quadrant && (x > endX || y < endY))
                withinBounds = false;

            return opposite ? !withinBounds : withinBounds;
        }

        public static void Line2D(Point3D start, Point3D end, Map map, DoEffect_Callback effect)
        {
            bool steep = Math.Abs(end.Y - start.Y) > Math.Abs(end.X - start.X);

            int x0 = start.X;
            int x1 = end.X;
            int y0 = start.Y;
            int y1 = end.Y;

            if (steep)
            {
                Swap(ref x0, ref y0);
                Swap(ref x1, ref y1);
            }

            if (x0 > x1)
            {
                Swap(ref x0, ref x1);
                Swap(ref y0, ref y1);
            }

            int deltax = x1 - x0;
            int deltay = Math.Abs(y1 - y0);
            int error = deltax / 2;
            int ystep = y0 < y1 ? 1 : -1;
            int y = y0;

            for (int x = x0; x <= x1; x++)
            {
                if (steep)
                    effect(new Point3D(y, x, start.Z), map);
                else
                    effect(new Point3D(x, y, start.Z), map);

                error -= deltay;

                if (error < 0)
                {
                    y += ystep;
                    error += deltax;
                }
            }
        }

        public class CirclePoint
        {
            private readonly Point2D point;
            private readonly int angle;
            private readonly int quadrant;
            public CirclePoint(Point2D point, int angle, int quadrant)
            {
                this.point = point;
                this.angle = angle;
                this.quadrant = quadrant;
            }

            public Point2D Point => point;

            public int Angle => angle;

            public int Quadrant => quadrant;
        }
    }
}