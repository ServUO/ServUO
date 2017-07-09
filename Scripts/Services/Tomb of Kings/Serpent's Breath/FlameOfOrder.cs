using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class FlameOfOrder : Item
    {
        public override int LabelNumber { get { return 1112127; } } // Flame of Order

        private List<EnergyBarrier> m_Barriers;
        private List<Blocker> m_Blockers;
        private List<LOSBlocker> m_LOSBlockers;
        private List<SBMessageTrigger> m_MsgTriggers;

        [Constructable]
        public FlameOfOrder(Point3D location, Map map)
            : base(0x19AB)
        {
            Movable = false;
            Light = LightType.Circle225;

            MoveToWorld(location, map);

            m_Barriers = new List<EnergyBarrier>(m_BarrierLocations.Length);
            m_Blockers = new List<Blocker>(m_BarrierLocations.Length);
            m_LOSBlockers = new List<LOSBlocker>(m_BarrierLocations.Length);
            m_MsgTriggers = new List<SBMessageTrigger>(m_MsgTriggerLocations.Length);

            foreach (Point3D loc in m_BarrierLocations)
            {
                m_Barriers.Add(new EnergyBarrier(loc, map));

                Blocker blocker = new Blocker();
                blocker.MoveToWorld(loc, map);
                m_Blockers.Add(blocker);

                LOSBlocker losblocker = new LOSBlocker();
                losblocker.MoveToWorld(loc, map);
                m_LOSBlockers.Add(losblocker);
            }

            foreach (Point3D loc in m_MsgTriggerLocations)
            {
                SBMessageTrigger trigger = new SBMessageTrigger(this);
                trigger.MoveToWorld(loc, map);
                m_MsgTriggers.Add(trigger);
            }
        }

        public override bool HandlesOnSpeech { get { return true; } }

        public override void OnSpeech(SpeechEventArgs e)
        {
            string mantra = e.Speech.ToLower();

            if (Visible && e.Mobile.InRange(this, 2) && mantra == "ord")
            {
                Visible = false;

                foreach (EnergyBarrier barrier in m_Barriers)
                    barrier.Active = false;

                foreach (Blocker blocker in m_Blockers)
                    blocker.Delete();

                foreach (LOSBlocker losblocker in m_LOSBlockers)
                    losblocker.Delete();

                m_Blockers.Clear();
                m_LOSBlockers.Clear();

                Timer.DelayCall(TimeSpan.FromMinutes(2.0), new TimerCallback(RestoreBarrier));
            }
        }

        protected void RestoreBarrier()
        {
            foreach (EnergyBarrier barrier in m_Barriers)
                barrier.Active = true;

            foreach (Point3D loc in m_BarrierLocations)
            {
                Blocker blocker = new Blocker();
                blocker.MoveToWorld(loc, Map);
                m_Blockers.Add(blocker);

                LOSBlocker losblocker = new LOSBlocker();
                losblocker.MoveToWorld(loc, Map);
                m_LOSBlockers.Add(losblocker);
            }

            Visible = true;
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            foreach (Blocker blocker in m_Blockers)
            {
                blocker.Delete();
            }

            foreach (LOSBlocker losblocker in m_LOSBlockers)
            {
                losblocker.Delete();
            }

            foreach (SBMessageTrigger trigger in m_MsgTriggers)
            {
                trigger.Delete();
            }

            foreach (EnergyBarrier barrier in m_Barriers)
            {
                barrier.Delete();
            }
        }

        private static Point3D[] m_BarrierLocations = new Point3D[]
        {
            new Point3D( 33, 205, 0 ),
            new Point3D( 34, 205, 0 ),
            new Point3D( 35, 205, 0 ),
            new Point3D( 36, 205, 0 ),
            new Point3D( 37, 205, 0 ),
        };

        private static Point3D[] m_MsgTriggerLocations = new Point3D[]
        {
            new Point3D( 33, 203, 0 ),
            new Point3D( 34, 203, 0 ),
            new Point3D( 35, 203, 0 ),
            new Point3D( 36, 203, 0 ),
            new Point3D( 37, 203, 0 ),
        };

        public FlameOfOrder(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write((int)m_Barriers.Count);

            for (int i = 0; i < m_Barriers.Count; i++)
                writer.Write((Item)m_Barriers[i]);

            writer.Write((int)m_Blockers.Count);

            for (int i = 0; i < m_Blockers.Count; i++)
                writer.Write((Item)m_Blockers[i]);

            writer.Write((int)m_LOSBlockers.Count);

            for (int i = 0; i < m_LOSBlockers.Count; i++)
                writer.Write((Item)m_LOSBlockers[i]);

            writer.Write((int)m_MsgTriggers.Count);

            for (int i = 0; i < m_MsgTriggers.Count; i++)
                writer.Write((Item)m_MsgTriggers[i]);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            // barrier
            int amount = reader.ReadInt();

            m_Barriers = new List<EnergyBarrier>(amount);

            for (int i = 0; i < amount; i++)
                m_Barriers.Add(reader.ReadItem() as EnergyBarrier);

            // blockers
            amount = reader.ReadInt();

            m_Blockers = new List<Blocker>(amount);

            for (int i = 0; i < amount; i++)
                m_Blockers.Add(reader.ReadItem() as Blocker);

            amount = reader.ReadInt();

            m_LOSBlockers = new List<LOSBlocker>(amount);

            for (int i = 0; i < amount; i++)
                m_LOSBlockers.Add(reader.ReadItem() as LOSBlocker);

            // msg triggers
            amount = reader.ReadInt();

            m_MsgTriggers = new List<SBMessageTrigger>(amount);

            for (int i = 0; i < amount; i++)
                m_MsgTriggers.Add(reader.ReadItem() as SBMessageTrigger);

            if (!Visible)
                Timer.DelayCall(TimeSpan.Zero, new TimerCallback(RestoreBarrier));
        }
    }
}