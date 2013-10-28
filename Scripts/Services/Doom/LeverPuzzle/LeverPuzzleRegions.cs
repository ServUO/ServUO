using System;
using Server.Mobiles;
using Server.Regions;

namespace Server.Engines.Doom
{
    public class LampRoomRegion : BaseRegion
    {
        private readonly LeverPuzzleController Controller;
        public LampRoomRegion(LeverPuzzleController controller)
            : base(null, Map.Malas, Region.Find(LeverPuzzleController.lr_Enter, Map.Malas), LeverPuzzleController.lr_Rect)
        {
            this.Controller = controller;
            this.Register();
        }

        public static void Initialize()
        {
            EventSink.Login += new LoginEventHandler(OnLogin);
        }

        public static void OnLogin(LoginEventArgs e)
        {
            Mobile m = e.Mobile;
            Rectangle2D rect = LeverPuzzleController.lr_Rect;
            if (m.X >= rect.X && m.X <= (rect.X + 10) && m.Y >= rect.Y && m.Y <= (rect.Y + 10) && m.Map == Map.Internal)
            {
                Timer kick = new LeverPuzzleController.LampRoomKickTimer(m);
                kick.Start();
            }
        }

        public override void OnEnter(Mobile m)
        {
            if (m == null || m is WandererOfTheVoid)
                return;

            if (m.IsStaff())
                return;

            if (this.Controller.Successful != null)
            {
                if (m is PlayerMobile)
                {
                    if (m == this.Controller.Successful)
                    {
                        return;
                    }
                }
                else if (m is BaseCreature)
                {
                    BaseCreature bc = (BaseCreature)m;
                    if ((bc.Controlled && bc.ControlMaster == this.Controller.Successful) || bc.Summoned)
                    {
                        return;
                    }
                }
            }
            Timer kick = new LeverPuzzleController.LampRoomKickTimer(m);
            kick.Start();
        }

        public override void OnExit(Mobile m)
        {
            if (m != null && m == this.Controller.Successful)
                this.Controller.RemoveSuccessful();
        }

        public override void OnDeath(Mobile m)
        {
            if (m != null && !m.Deleted && !(m is WandererOfTheVoid))
            {
                Timer kick = new LeverPuzzleController.LampRoomKickTimer(m);
                kick.Start();
            }
        }

        public override bool OnSkillUse(Mobile m, int Skill) /* just in case */
        {
            if ((this.Controller.Successful == null) || (m.IsStaff() && m != this.Controller.Successful))
            {
                return false;
            }
            return true;
        }
    }

    public class LeverPuzzleRegion : BaseRegion
    {
        public Mobile m_Occupant;
        private readonly LeverPuzzleController Controller;
        public LeverPuzzleRegion(LeverPuzzleController controller, int[] loc)
            : base(null, Map.Malas, Region.Find(LeverPuzzleController.lr_Enter, Map.Malas), new Rectangle2D(loc[0],loc[1],1,1))
        {
            this.Controller = controller;
            this.Register();
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Occupant
        {
            get
            {
                if (this.m_Occupant != null && this.m_Occupant.Alive)
                    return this.m_Occupant;
                return null;
            }
        }
        public override void OnEnter(Mobile m)
        {
            if (m != null && this.m_Occupant == null && m is PlayerMobile && m.Alive)
                this.m_Occupant = m;
        }

        public override void OnExit(Mobile m)
        {
            if (m != null && m == this.m_Occupant)
                this.m_Occupant = null;
        }
    }
}