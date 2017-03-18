using System;
using Server;
using System.Collections.Generic;
using Server.Mobiles;
using System.Linq;
using Server.Network;
using Server.Regions;
using Server.Spells;

namespace Server.Items
{
	public class Volcano
    {
        public static readonly Rectangle2D LavaStart = new Rectangle2D(927, 1615, 2, 2);
        public static readonly Rectangle2D[] FlameZone = new Rectangle2D[] { new Rectangle2D(903, 1591, 50, 50) };
        public static readonly int MaxFlames = 12;
        public static readonly int LastLavaStage = 31;

        public static TimeSpan FlameRespawn { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(1, 3)); } }
        public static TimeSpan LavaRespawn { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(20, 30)); } }
        public static TimeSpan LavaAdvance { get { return TimeSpan.FromSeconds(2); } }

        public static Volcano Instance { get; set; }

        private Timer _LavaTimer;
        private DateTime _NextLava;
        private long _NextLavaAdvance;
        private int _LavaStage;
        private Rectangle2D _CurrentLava;
        private Region _Region;

        public static void Initialize()
        {
            Instance = new Volcano();
        }

        public Volcano()
        {
            _Region = new VolcanoRegion(this);
            Timer.DelayCall(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10), OnTick);
        }

        public void OnTick()
        {
            if (_Region == null)
                return;

            int count = _Region.GetPlayerCount();

            if (count == 0 && _LavaTimer == null && _NextLava < DateTime.UtcNow)
            {
                _NextLava = DateTime.UtcNow + LavaRespawn;
                return;
            }

            if (count > 0 && _LavaTimer == null && _NextLava < DateTime.UtcNow)
            {
                List<Mobile> players = _Region.GetPlayers();
                players.ForEach(m => m.PrivateOverheadMessage(MessageType.Regular, 0x21, 1156506, m.NetState)); // *The Volcano is becoming unstable!*
                ColUtility.Free(players);

                _CurrentLava = LavaStart;
                _NextLavaAdvance = Core.TickCount + 1000;

                _LavaTimer = Timer.DelayCall(TimeSpan.FromMilliseconds(250), TimeSpan.FromMilliseconds(250), () =>
                {
                    if (Core.TickCount - _NextLavaAdvance >= 0)
                    {
                        _CurrentLava = new Rectangle2D(_CurrentLava.X - 2, _CurrentLava.Y - 2, _CurrentLava.Width + 4, _CurrentLava.Height + 4);
                        _NextLavaAdvance = Core.TickCount + 1000;

                        AddLava();
                    }
                });

                _LavaTimer.Start();
            }
        }

        public void AddLava()
        {
            _LavaStage++;

            for (int x = _CurrentLava.X; x <= _CurrentLava.X + _CurrentLava.Width; x++)
            {
                for (int y = _CurrentLava.Y; y <= _CurrentLava.Y + _CurrentLava.Height; y++)
                {
                    if (x <= _CurrentLava.X + 2 || x >= (_CurrentLava.X + _CurrentLava.Width) - 2 ||
                       y <= _CurrentLava.Y + 2 || y >= (_CurrentLava.Y + _CurrentLava.Height) - 2)
                    {
                        if (Map.TerMur.CanFit(x, y, 0, 16, false, false, true))
                        {
                            Point3D p = new Point3D(x, y, 0);
                            Effects.SendLocationEffect(p, Map.TerMur, 4847, (int)LavaAdvance.TotalSeconds * 30);

                            IPooledEnumerable eable = Map.TerMur.GetMobilesInRange(p, 0);

                            foreach (Mobile m in eable)
                            {
                                if (m.Alive && m.AccessLevel == AccessLevel.Player && (m is PlayerMobile || (m is BaseCreature && ((BaseCreature)m).GetMaster() is PlayerMobile)))
                                    DoLavaDamageDelayed(m);
                            }

                            eable.Free();
                        }
                    }
                }
            }

            if (_LavaStage >= LastLavaStage)
            {
                if (_LavaTimer != null)
                {
                    _LavaTimer.Stop();
                    _LavaTimer = null;
                }

                _NextLava = DateTime.UtcNow + LavaRespawn;
                _LavaStage = 0;
            }
        }

        public void CheckMovement(Mobile m)
        {
            if (!m.Alive || m.AccessLevel > AccessLevel.Player || (m is BaseCreature && ((BaseCreature)m).GetMaster() == null))
                return;

            if (_LavaTimer != null && _CurrentLava.Contains(m) && m.Alive && m.AccessLevel == AccessLevel.Player)
            {
                DoLavaDamageDelayed(m);
            }
        }

        public void DoLavaDamage(Mobile m)
        {
            m.PrivateOverheadMessage(MessageType.Regular, 0x22, 1156497, m.NetState); // *The flowing lava scorches you!*
            AOS.Damage(m, null, Utility.RandomMinMax(200, 300), 0, 100, 0, 0, 0);
        }

        public void DoLavaDamageDelayed(Mobile m)
        {
            Timer.DelayCall(TimeSpan.FromSeconds(.25), new TimerStateCallback<Mobile>(DoLavaDamage), m);
        }
    }

    public class VolcanoRegion : BaseRegion
    {
        public Volcano Volcano { get; private set; }

        public VolcanoRegion(Volcano volcano)
            : base("Eodon_Volcano", Map.TerMur, Region.DefaultPriority, new Rectangle2D[] { new Rectangle2D(832, 1502, 255, 217)})
        {
            Volcano = volcano;
            Register();
        }

        public override bool CheckTravel(Mobile m, Point3D newLocation, TravelCheckType travelType)
        {
            return travelType >= (TravelCheckType)4; // teleport only
        }

        public override void OnLocationChanged(Mobile m, Point3D oldLocation)
        {
            if(Volcano != null)
                Volcano.CheckMovement(m);

            base.OnLocationChanged(m, oldLocation);
        }
    }
}