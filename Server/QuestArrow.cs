#region References
using Server.Network;
#endregion

namespace Server
{
    public class QuestArrow
    {
        private readonly Mobile m_Mobile;
        private readonly IPoint3D m_Target;
        private bool m_Running;

        public Mobile Mobile => m_Mobile;

        public IPoint3D Target => m_Target;

        public bool Running => m_Running;

        public void Update()
        {
            Update(m_Target.X, m_Target.Y);
        }

        public void Update(int x, int y)
        {
            if (!m_Running)
            {
                return;
            }

            var ns = m_Mobile.NetState;

            if (ns != null)
            {
                IPoint2D target = m_Target;

                if (target == null)
                {
                    target = new Point2D(x, y);
                }

                SetArrow.Send(ns, target);
            }
        }

        public void Stop()
        {
            Stop(m_Target.X, m_Target.Y);
        }

        public void Stop(int x, int y)
        {
            if (!m_Running)
            {
                return;
            }

            m_Mobile.ClearQuestArrow();

            var ns = m_Mobile.NetState;

            if (ns != null)
            {
                IPoint2D target = m_Target;

                if (target == null)
                {
                    target = new Point2D(x, y);
                }

                CancelArrow.Send(ns, target);
            }

            m_Running = false;
            OnStop();
        }

        public virtual void OnStop()
        { }

        public virtual void OnClick(bool rightClick)
        { }

        public QuestArrow(Mobile m, IPoint3D t)
        {
            m_Running = true;
            m_Mobile = m;
            m_Target = t;
        }

        public QuestArrow(Mobile m, IPoint3D t, int x, int y)
            : this(m, t)
        {
            Update(x, y);
        }
    }
}
