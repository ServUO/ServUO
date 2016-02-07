using System;
using CalcMoves = Server.Movement.Movement;

namespace Server
{
    public class PathFollower
    {
        // Should we use pathfinding? 'false' for not
        private static readonly bool Enabled = true;
        private static readonly TimeSpan RepathDelay = TimeSpan.FromSeconds(2.0);
        private readonly Mobile m_From;
        private readonly IPoint3D m_Goal;
        private MovementPath m_Path;
        private int m_Index;
        private Point3D m_Next, m_LastGoalLoc;
        private DateTime m_LastPathTime;
        private MoveMethod m_Mover;
        public PathFollower(Mobile from, IPoint3D goal)
        {
            this.m_From = from;
            this.m_Goal = goal;
        }

        public MoveMethod Mover
        {
            get
            {
                return this.m_Mover;
            }
            set
            {
                this.m_Mover = value;
            }
        }
        public IPoint3D Goal
        {
            get
            {
                return this.m_Goal;
            }
        }
        public MoveResult Move(Direction d)
        {
            if (this.m_Mover == null)
                return (this.m_From.Move(d) ? MoveResult.Success : MoveResult.Blocked);

            return this.m_Mover(d);
        }

        public Point3D GetGoalLocation()
        {
            if (this.m_Goal is Item)
                return ((Item)this.m_Goal).GetWorldLocation();

            return new Point3D(this.m_Goal);
        }

        public void Advance(ref Point3D p, int index)
        {
            if (this.m_Path != null && this.m_Path.Success)
            {
                Direction[] dirs = this.m_Path.Directions;

                if (index >= 0 && index < dirs.Length)
                {
                    int x = p.X, y = p.Y;

                    CalcMoves.Offset(dirs[index], ref x, ref y);

                    p.X = x;
                    p.Y = y;
                }
            }
        }

        public void ForceRepath()
        {
            this.m_Path = null;
        }

        public bool CheckPath()
        {
            if (!Enabled)
                return false;

            bool repath = false;

            Point3D goal = this.GetGoalLocation();

            if (this.m_Path == null)
                repath = true;
            else if ((!this.m_Path.Success || goal != this.m_LastGoalLoc) && (this.m_LastPathTime + RepathDelay) <= DateTime.UtcNow)
                repath = true;
            else if (this.m_Path.Success && this.Check(this.m_From.Location, this.m_LastGoalLoc, 0))
                repath = true;

            if (!repath)
                return false;

            this.m_LastPathTime = DateTime.UtcNow;
            this.m_LastGoalLoc = goal;

            this.m_Path = new MovementPath(this.m_From, goal);

            this.m_Index = 0;
            this.m_Next = this.m_From.Location;

            this.Advance(ref this.m_Next, this.m_Index);

            return true;
        }

        public bool Check(Point3D loc, Point3D goal, int range)
        {
            if (!Utility.InRange(loc, goal, range))
                return false;

            if (range <= 1 && Math.Abs(loc.Z - goal.Z) >= 16)
                return false;

            return true;
        }

        public bool Follow(bool run, int range)
        {
            Point3D goal = this.GetGoalLocation();
            Direction d;

            if (this.Check(this.m_From.Location, goal, range))
                return true;

            bool repathed = this.CheckPath();

            if (!Enabled || !this.m_Path.Success)
            {
                d = this.m_From.GetDirectionTo(goal);

                if (run)
                    d |= Direction.Running;

                this.m_From.SetDirection(d);
                this.Move(d);

                return this.Check(this.m_From.Location, goal, range);
            }

            d = this.m_From.GetDirectionTo(this.m_Next);

            if (run)
                d |= Direction.Running;

            this.m_From.SetDirection(d);

            MoveResult res = this.Move(d);

            if (res == MoveResult.Blocked)
            {
                if (repathed)
                    return false;

                this.m_Path = null;
                this.CheckPath();

                if (!this.m_Path.Success)
                {
                    d = this.m_From.GetDirectionTo(goal);

                    if (run)
                        d |= Direction.Running;

                    this.m_From.SetDirection(d);
                    this.Move(d);

                    return this.Check(this.m_From.Location, goal, range);
                }

                d = this.m_From.GetDirectionTo(this.m_Next);

                if (run)
                    d |= Direction.Running;

                this.m_From.SetDirection(d);

                res = this.Move(d);

                if (res == MoveResult.Blocked)
                    return false;
            }

            if (this.m_From.X == this.m_Next.X && this.m_From.Y == this.m_Next.Y)
            {
                if (this.m_From.Z == this.m_Next.Z)
                {
                    ++this.m_Index;
                    this.Advance(ref this.m_Next, this.m_Index);
                }
                else
                {
                    this.m_Path = null;
                }
            }

            return this.Check(this.m_From.Location, goal, range);
        }
    }
}