using System;
using Server.Mobiles;
using CalcMoves = Server.Movement.Movement;
using MoveImpl = Server.Movement.MovementImpl;

namespace Server.PathAlgorithms.SlowAStar
{
    public struct PathNode
    {
        public int x, y, z;
        public int g, h;
        public int px, py, pz;
        public int dir;
    }

    public class SlowAStarAlgorithm : PathAlgorithm
    {
        public static PathAlgorithm Instance = new SlowAStarAlgorithm();
        private static readonly PathNode[] m_Closed = new PathNode[MaxNodes];
        private static readonly PathNode[] m_Open = new PathNode[MaxNodes];
        private static readonly PathNode[] m_Successors = new PathNode[8];
        private static readonly Direction[] m_Path = new Direction[MaxNodes];
        private const int MaxDepth = 300;
        private const int MaxNodes = MaxDepth * 16;
        private Point3D m_Goal;
        public int Heuristic(int x, int y, int z)
        {
            x -= this.m_Goal.X;
            y -= this.m_Goal.Y;
            z -= this.m_Goal.Z;

            x *= 11;
            y *= 11;

            return (x * x) + (y * y) + (z * z);
        }

        public override bool CheckCondition(IPoint3D p, Map map, Point3D start, Point3D goal)
        {
            return false;
        }

        public override Direction[] Find(IPoint3D p, Map map, Point3D start, Point3D goal)
        {
            this.m_Goal = goal;

            BaseCreature bc = p as BaseCreature;

            PathNode curNode;

            PathNode goalNode = new PathNode();
            goalNode.x = goal.X;
            goalNode.y = goal.Y;
            goalNode.z = goal.Z;

            PathNode startNode = new PathNode();
            startNode.x = start.X;
            startNode.y = start.Y;
            startNode.z = start.Z;
            startNode.h = this.Heuristic(startNode.x, startNode.y, startNode.z);

            PathNode[] closed = m_Closed, open = m_Open, successors = m_Successors;
            Direction[] path = m_Path;

            int closedCount = 0, openCount = 0, sucCount = 0, pathCount = 0;
            int popIndex, curF;
            int x, y, z;
            int depth = 0;

            int xBacktrack, yBacktrack, zBacktrack, iBacktrack = 0;

            open[openCount++] = startNode;

            while (openCount > 0)
            {
                curNode = open[0];
                curF = curNode.g + curNode.h;
                popIndex = 0;

                for (int i = 1; i < openCount; ++i)
                {
                    if ((open[i].g + open[i].h) < curF)
                    {
                        curNode = open[i];
                        curF = curNode.g + curNode.h;
                        popIndex = i;
                    }
                }

                if (curNode.x == goalNode.x && curNode.y == goalNode.y && Math.Abs(curNode.z - goalNode.z) < 16)
                {
                    if (closedCount == MaxNodes)
                        break;

                    closed[closedCount++] = curNode;

                    xBacktrack = curNode.px;
                    yBacktrack = curNode.py;
                    zBacktrack = curNode.pz;

                    if (pathCount == MaxNodes)
                        break;

                    path[pathCount++] = (Direction)curNode.dir;

                    while (xBacktrack != startNode.x || yBacktrack != startNode.y || zBacktrack != startNode.z)
                    {
                        bool found = false;

                        for (int j = 0; !found && j < closedCount; ++j)
                        {
                            if (closed[j].x == xBacktrack && closed[j].y == yBacktrack && closed[j].z == zBacktrack)
                            {
                                if (pathCount == MaxNodes)
                                    break;

                                curNode = closed[j];
                                path[pathCount++] = (Direction)curNode.dir;
                                xBacktrack = curNode.px;
                                yBacktrack = curNode.py;
                                zBacktrack = curNode.pz;
                                found = true;
                            }
                        }

                        if (!found)
                        {
                            Console.WriteLine("bugaboo..");
                            return null;
                        }

                        if (pathCount == MaxNodes)
                            break;
                    }

                    if (pathCount == MaxNodes)
                        break;

                    Direction[] dirs = new Direction[pathCount];

                    while (pathCount > 0)
                        dirs[iBacktrack++] = path[--pathCount];

                    return dirs;
                }

                --openCount;

                for (int i = popIndex; i < openCount; ++i)
                    open[i] = open[i + 1];

                sucCount = 0;

                if (bc != null)
                {
                    MoveImpl.AlwaysIgnoreDoors = bc.CanOpenDoors;
                    MoveImpl.IgnoreMovableImpassables = bc.CanMoveOverObstacles;
                }

                MoveImpl.Goal = goal;

                for (int i = 0; i < 8; ++i)
                {
                    switch ( i )
                    {
                        default:
                        case 0:
                            x = 0;
                            y = -1;
                            break;
                        case 1:
                            x = 1;
                            y = -1;
                            break;
                        case 2:
                            x = 1;
                            y = 0;
                            break;
                        case 3:
                            x = 1;
                            y = 1;
                            break;
                        case 4:
                            x = 0;
                            y = 1;
                            break;
                        case 5:
                            x = -1;
                            y = 1;
                            break;
                        case 6:
                            x = -1;
                            y = 0;
                            break;
                        case 7:
                            x = -1;
                            y = -1;
                            break;
                    }

                    if (CalcMoves.CheckMovement(p, map, new Point3D(curNode.x, curNode.y, curNode.z), (Direction)i, out z))
                    {
                        successors[sucCount].x = x + curNode.x;
                        successors[sucCount].y = y + curNode.y;
                        successors[sucCount++].z = z;
                    }
                }

                MoveImpl.AlwaysIgnoreDoors = false;
                MoveImpl.IgnoreMovableImpassables = false;
                MoveImpl.Goal = Point3D.Zero;

                if (sucCount == 0 || ++depth > MaxDepth)
                    break;

                for (int i = 0; i < sucCount; ++i)
                {
                    x = successors[i].x;
                    y = successors[i].y;
                    z = successors[i].z;

                    successors[i].g = curNode.g + 1;

                    int openIndex = -1, closedIndex = -1;

                    for (int j = 0; openIndex == -1 && j < openCount; ++j)
                    {
                        if (open[j].x == x && open[j].y == y && open[j].z == z)
                            openIndex = j;
                    }

                    if (openIndex >= 0 && open[openIndex].g < successors[i].g)
                        continue;

                    for (int j = 0; closedIndex == -1 && j < closedCount; ++j)
                    {
                        if (closed[j].x == x && closed[j].y == y && closed[j].z == z)
                            closedIndex = j;
                    }

                    if (closedIndex >= 0 && closed[closedIndex].g < successors[i].g)
                        continue;

                    if (openIndex >= 0)
                    {
                        --openCount;

                        for (int j = openIndex; j < openCount; ++j)
                            open[j] = open[j + 1];
                    }

                    if (closedIndex >= 0)
                    {
                        --closedCount;

                        for (int j = closedIndex; j < closedCount; ++j)
                            closed[j] = closed[j + 1];
                    }

                    successors[i].px = curNode.x;
                    successors[i].py = curNode.y;
                    successors[i].pz = curNode.z;
                    successors[i].dir = (int)this.GetDirection(curNode.x, curNode.y, x, y);
                    successors[i].h = this.Heuristic(x, y, z);

                    if (openCount == MaxNodes)
                        break;

                    open[openCount++] = successors[i];
                }

                if (openCount == MaxNodes || closedCount == MaxNodes)
                    break;

                closed[closedCount++] = curNode;
            }

            return null;
        }
    }
}