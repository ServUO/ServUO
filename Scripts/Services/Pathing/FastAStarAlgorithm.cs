using System;
using System.Collections;
using Server.Mobiles;
using CalcMoves = Server.Movement.Movement;
using MoveImpl = Server.Movement.MovementImpl;

namespace Server.PathAlgorithms.FastAStar
{
    public struct PathNode
    {
        public int cost, total;
        public int parent, next, prev;
        public int z;
    }

    public class FastAStarAlgorithm : PathAlgorithm
    {
        public static PathAlgorithm Instance = new FastAStarAlgorithm();
        private static readonly Direction[] m_Path = new Direction[AreaSize * AreaSize];
        private static readonly PathNode[] m_Nodes = new PathNode[NodeCount];
        private static readonly BitArray m_Touched = new BitArray(NodeCount);
        private static readonly BitArray m_OnOpen = new BitArray(NodeCount);
        private static readonly int[] m_Successors = new int[8];
        private static int m_xOffset, m_yOffset;
        private static int m_OpenList;
        private const int MaxDepth = 300;
        private const int AreaSize = 38;
        private const int NodeCount = AreaSize * AreaSize * PlaneCount;
        private const int PlaneOffset = 128;
        private const int PlaneCount = 13;
        private const int PlaneHeight = 20;
        private Point3D m_Goal;
        public int Heuristic(int x, int y, int z)
        {
            x -= this.m_Goal.X - m_xOffset;
            y -= this.m_Goal.Y - m_yOffset;
            z -= this.m_Goal.Z;

            x *= 11;
            y *= 11;

            return (x * x) + (y * y) + (z * z);
        }

        public override bool CheckCondition(Mobile m, Map map, Point3D start, Point3D goal)
        {
            return Utility.InRange(start, goal, AreaSize);
        }

        public override Direction[] Find(Mobile m, Map map, Point3D start, Point3D goal)
        {
            if (!Utility.InRange(start, goal, AreaSize))
                return null;

            m_Touched.SetAll(false);

            this.m_Goal = goal;

            m_xOffset = (start.X + goal.X - AreaSize) / 2;
            m_yOffset = (start.Y + goal.Y - AreaSize) / 2;

            int fromNode = this.GetIndex(start.X, start.Y, start.Z);
            int destNode = this.GetIndex(goal.X, goal.Y, goal.Z);

            m_OpenList = fromNode;

            m_Nodes[m_OpenList].cost = 0;
            m_Nodes[m_OpenList].total = this.Heuristic(start.X - m_xOffset, start.Y - m_yOffset, start.Z);
            m_Nodes[m_OpenList].parent = -1;
            m_Nodes[m_OpenList].next = -1;
            m_Nodes[m_OpenList].prev = -1;
            m_Nodes[m_OpenList].z = start.Z;

            m_OnOpen[m_OpenList] = true;
            m_Touched[m_OpenList] = true;

            BaseCreature bc = m as BaseCreature;

            int pathCount, parent;
            int backtrack = 0, depth = 0;

            Direction[] path = m_Path;

            while (m_OpenList != -1)
            {
                int bestNode = this.FindBest(m_OpenList);

                if (++depth > MaxDepth)
                    break;

                if (bc != null)
                {
                    MoveImpl.AlwaysIgnoreDoors = bc.CanOpenDoors;
                    MoveImpl.IgnoreMovableImpassables = bc.CanMoveOverObstacles;
                }

                MoveImpl.Goal = goal;

                int[] vals = m_Successors;
                int count = this.GetSuccessors(bestNode, m, map);

                MoveImpl.AlwaysIgnoreDoors = false;
                MoveImpl.IgnoreMovableImpassables = false;
                MoveImpl.Goal = Point3D.Zero;

                if (count == 0)
                    break;

                for (int i = 0; i < count; ++i)
                {
                    int newNode = vals[i];

                    bool wasTouched = m_Touched[newNode];

                    if (!wasTouched)
                    {
                        int newCost = m_Nodes[bestNode].cost + 1;
                        int newTotal = newCost + this.Heuristic(newNode % AreaSize, (newNode / AreaSize) % AreaSize, m_Nodes[newNode].z);

                        if (!wasTouched || m_Nodes[newNode].total > newTotal)
                        {
                            m_Nodes[newNode].parent = bestNode;
                            m_Nodes[newNode].cost = newCost;
                            m_Nodes[newNode].total = newTotal;

                            if (!wasTouched || !m_OnOpen[newNode])
                            {
                                this.AddToChain(newNode);

                                if (newNode == destNode)
                                {
                                    pathCount = 0;
                                    parent = m_Nodes[newNode].parent;

                                    while (parent != -1)
                                    {
                                        path[pathCount++] = this.GetDirection(parent % AreaSize, (parent / AreaSize) % AreaSize, newNode % AreaSize, (newNode / AreaSize) % AreaSize);
                                        newNode = parent;
                                        parent = m_Nodes[newNode].parent;

                                        if (newNode == fromNode)
                                            break;
                                    }

                                    Direction[] dirs = new Direction[pathCount];

                                    while (pathCount > 0)
                                        dirs[backtrack++] = path[--pathCount];

                                    return dirs;
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        public int GetSuccessors(int p, Mobile m, Map map)
        {
            int px = p % AreaSize;
            int py = (p / AreaSize) % AreaSize;
            int pz = m_Nodes[p].z;
            int x, y, z;

            Point3D p3D = new Point3D(px + m_xOffset, py + m_yOffset, pz);

            int[] vals = m_Successors;
            int count = 0;

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

                x += px;
                y += py;

                if (x < 0 || x >= AreaSize || y < 0 || y >= AreaSize)
                    continue;

                if (CalcMoves.CheckMovement(m, map, p3D, (Direction)i, out z))
                {
                    int idx = this.GetIndex(x + m_xOffset, y + m_yOffset, z);

                    if (idx >= 0 && idx < NodeCount)
                    {
                        m_Nodes[idx].z = z;
                        vals[count++] = idx;
                    }
                }
            }

            return count;
        }

        private void RemoveFromChain(int node)
        {
            if (node < 0 || node >= NodeCount)
                return;

            if (!m_Touched[node] || !m_OnOpen[node])
                return;

            int prev = m_Nodes[node].prev;
            int next = m_Nodes[node].next;

            if (m_OpenList == node)
                m_OpenList = next;

            if (prev != -1)
                m_Nodes[prev].next = next;

            if (next != -1)
                m_Nodes[next].prev = prev;

            m_Nodes[node].prev = -1;
            m_Nodes[node].next = -1;
        }

        private void AddToChain(int node)
        {
            if (node < 0 || node >= NodeCount)
                return;

            this.RemoveFromChain(node);

            if (m_OpenList != -1)
                m_Nodes[m_OpenList].prev = node;

            m_Nodes[node].next = m_OpenList;
            m_Nodes[node].prev = -1;

            m_OpenList = node;

            m_Touched[node] = true;
            m_OnOpen[node] = true;
        }

        private int GetIndex(int x, int y, int z)
        {
            x -= m_xOffset;
            y -= m_yOffset;
            z += PlaneOffset;
            z /= PlaneHeight;

            return x + (y * AreaSize) + (z * AreaSize * AreaSize);
        }

        private int FindBest(int node)
        {
            int least = m_Nodes[node].total;
            int leastNode = node;

            while (node != -1)
            {
                if (m_Nodes[node].total < least)
                {
                    least = m_Nodes[node].total;
                    leastNode = node;
                }

                node = m_Nodes[node].next;
            }

            this.RemoveFromChain(leastNode);

            m_Touched[leastNode] = true;
            m_OnOpen[leastNode] = false;

            return leastNode;
        }
    }
}