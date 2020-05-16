using System;
using System.Collections.Generic;

namespace Server.Engines.TombOfKings
{
    public class ChamberInfo
    {
        private Point3D m_BarrierLocation, m_SwitchLocation;
        private readonly int m_SwitchId;

        public Point3D BarrierLocation => m_BarrierLocation;
        public Point3D SwitchLocation => m_SwitchLocation;
        public int SwitchId => m_SwitchId;

        public ChamberInfo(Point3D barrierLoc, Point3D switchLoc, int switchId)
        {
            m_BarrierLocation = barrierLoc;
            m_SwitchLocation = switchLoc;
            m_SwitchId = switchId;
        }
    }

    public class Chamber
    {
        public static void Initialize()
        {
            // we should call it after deserialize the levers
            Timer.DelayCall(TimeSpan.Zero, Generate);
        }

        public static void Generate()
        {
            if (ChamberLever.Levers.Count == 0)
                return;

            foreach (ChamberInfo info in m_ChamberInfos)
                m_Chambers.Add(new Chamber(info));

            // randomize
            List<ChamberLever> levers = new List<ChamberLever>(ChamberLever.Levers);

            foreach (Chamber chamber in m_Chambers)
            {
                int idx = Utility.Random(levers.Count);

                chamber.Lever = levers[idx];
                levers[idx].Chamber = chamber;
                levers.RemoveAt(idx);
            }
        }

        private static readonly List<Chamber> m_Chambers = new List<Chamber>();

        public static List<Chamber> Chambers => m_Chambers;

        private static readonly ChamberInfo[] m_ChamberInfos = new ChamberInfo[]
        {
			// left side
			new ChamberInfo( new Point3D( 15, 200, -5 ), new Point3D( 13, 195, 7 ), 0x1091 ),
            new ChamberInfo( new Point3D( 15, 184, -5 ), new Point3D( 13, 179, 7 ), 0x1091 ),
            new ChamberInfo( new Point3D( 15, 168, -5 ), new Point3D( 13, 163, 7 ), 0x1091 ),
            new ChamberInfo( new Point3D( 15, 152, -5 ), new Point3D( 13, 147, 7 ), 0x1091 ),
            new ChamberInfo( new Point3D( 15, 136, -5 ), new Point3D( 13, 131, 7 ), 0x1091 ),
            new ChamberInfo( new Point3D( 15, 120, -5 ), new Point3D( 13, 115, 7 ), 0x1091 ),

			// right side
			new ChamberInfo( new Point3D( 55, 200, -5 ), new Point3D( 56, 197, 7 ), 0x1090 ),
            new ChamberInfo( new Point3D( 55, 184, -5 ), new Point3D( 56, 181, 7 ), 0x1090 ),
            new ChamberInfo( new Point3D( 55, 168, -5 ), new Point3D( 56, 165, 7 ), 0x1090 ),
            new ChamberInfo( new Point3D( 55, 152, -5 ), new Point3D( 56, 149, 7 ), 0x1090 ),
            new ChamberInfo( new Point3D( 55, 136, -5 ), new Point3D( 56, 133, 7 ), 0x1090 ),
            new ChamberInfo( new Point3D( 55, 120, -5 ), new Point3D( 56, 117, 7 ), 0x1090 ),
        };

        private ChamberSwitch m_Switch;
        private ChamberBarrier m_Barrier;
        private ChamberLever m_Lever;

        public ChamberSwitch Switch
        {
            get { return m_Switch; }
            set { m_Switch = value; }
        }

        public ChamberBarrier Barrier
        {
            get { return m_Barrier; }
            set { m_Barrier = value; }
        }

        public ChamberLever Lever
        {
            get { return m_Lever; }
            set { m_Lever = value; }
        }

        public bool IsOpened()
        {
            return !m_Barrier.Active;
        }

        public void Open()
        {
            m_Barrier.Active = false;
            m_Lever.Switch();

            Timer.DelayCall(TimeSpan.FromMinutes(Utility.RandomMinMax(10, 15)), RestoreBarrier);
        }

        public void RestoreBarrier()
        {
            m_Barrier.Active = true;
            m_Lever.InvalidateProperties();
        }

        public Chamber(ChamberInfo info)
        {
            m_Switch = new ChamberSwitch(this, info.SwitchLocation, info.SwitchId);
            m_Barrier = new ChamberBarrier(info.BarrierLocation);
        }
    }
}
