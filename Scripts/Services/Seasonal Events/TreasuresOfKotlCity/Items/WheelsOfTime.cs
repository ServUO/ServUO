using Server.Items;
using Server.Network;
using System;

namespace Server.Engines.TreasuresOfKotlCity
{
    public class WheelsOfTime : Item
    {
        public static TimeSpan TimeWarpDuration = TimeSpan.FromHours(2); // Duration in which you have to activate spawn to do the spawn

        public static Point3D RockBarrierLocation = new Point3D(584, 2399, 0);
        public static WheelsOfTime Instance { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseAddon RockBarrier { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime TimeWarpEnds { get; set; }

        public Timer Timer { get; set; }

        public override int LabelNumber => 1157032;  // Wheels of Time

        public WheelsOfTime()
            : base(0x9CEF)
        {
            Hue = 2655;
            Movable = false;
            Instance = this;

            RockBarrier = new KotlWallAddon();
            RockBarrier.MoveToWorld(RockBarrierLocation, Map.TerMur);

            TimeWarpEnds = DateTime.MinValue;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 1))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
            else if (TimeWarpEnds != DateTime.MinValue)
            {
                from.SendLocalizedMessage(1157021); // The wheels of time have already been set in motion.
            }
            else
            {
                from.PrivateOverheadMessage(MessageType.Regular, 1154, 1157020, from.NetState);
                from.PlaySound(0x0FF);

                ActivateTimeWarp();
            }
        }

        public void ActivateTimeWarp()
        {
            if (RockBarrier != null)
            {
                RockBarrier.MoveToWorld(new Point3D(RockBarrier.X, RockBarrier.Y, RockBarrier.Z - 50), Map.TerMur);
            }

            TimeWarpEnds = DateTime.UtcNow + TimeWarpDuration;
            Timer = Timer.DelayCall(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), OnTick);
        }

        private void OnTick()
        {
            if (TimeWarpEnds < DateTime.UtcNow)
            {
                if (Timer != null)
                {
                    Timer.Stop();
                    Timer = null;
                }

                Cleanup();
            }
        }

        private void Cleanup()
        {
            TimeWarpEnds = DateTime.MinValue;

            if (KotlBattleSimulator.Instance != null && KotlBattleSimulator.Instance.Active)
            {
                KotlBattleSimulator.Instance.EndSimulation();
            }

            if (RockBarrier != null && RockBarrier.Location != RockBarrierLocation)
            {
                RockBarrier.MoveToWorld(RockBarrierLocation, Map.TerMur);
            }
        }

        public WheelsOfTime(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version

            writer.Write(RockBarrier);
            writer.Write(TimeWarpEnds);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Instance = this;

            RockBarrier = reader.ReadItem() as BaseAddon;
            TimeWarpEnds = reader.ReadDateTime();

            if (TimeWarpEnds != DateTime.MinValue)
            {
                if (TimeWarpEnds > DateTime.UtcNow)
                {
                    Timer = Timer.DelayCall(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), OnTick);
                }
                else
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(60), Cleanup);
                }
            }

            if (RockBarrier == null)
            {
                RockBarrier = new KotlWallAddon();
                RockBarrier.MoveToWorld(RockBarrierLocation, Map.TerMur);
            }
        }
    }
}