using Server.Mobiles;
using Server.Regions;

namespace Server.Engines.Doom
{
    public class LampRoomRegion : BaseRegion
    {
        private readonly LeverPuzzleController Controller;
        public LampRoomRegion(LeverPuzzleController controller)
            : base(null, Map.Malas, Find(LeverPuzzleController.lr_Enter, Map.Malas), LeverPuzzleController.lr_Rect)
        {
            Controller = controller;
            Register();
        }

        public static void Initialize()
        {
            EventSink.Login += OnLogin;
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

            if (Controller.Successful != null)
            {
                if (m is PlayerMobile)
                {
                    if (m == Controller.Successful)
                    {
                        return;
                    }
                }
                else if (m is BaseCreature)
                {
                    BaseCreature bc = (BaseCreature)m;
                    if ((bc.Controlled && bc.ControlMaster == Controller.Successful) || bc.Summoned)
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
            if (m != null && m == Controller.Successful)
                Controller.RemoveSuccessful();
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
            if ((Controller.Successful == null) || (m.IsStaff() && m != Controller.Successful))
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
            : base(null, Map.Malas, Find(LeverPuzzleController.lr_Enter, Map.Malas), new Rectangle2D(loc[0], loc[1], 1, 1))
        {
            Controller = controller;
            Register();
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Occupant
        {
            get
            {
                if (m_Occupant != null && m_Occupant.Alive)
                    return m_Occupant;
                return null;
            }
        }
        public override void OnEnter(Mobile m)
        {
            if (m != null && m_Occupant == null && m is PlayerMobile && m.Alive)
                m_Occupant = m;
        }

        public override void OnExit(Mobile m)
        {
            if (m != null && m == m_Occupant)
                m_Occupant = null;
        }
    }
}
