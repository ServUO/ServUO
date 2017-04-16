using System;
using Server;
using Server.Items;
using Server.Network;

namespace Server.Engines.TreasuresOfKotlCity
{
    public class WheelsOfTime : Item
    {
        public static TimeSpan TimeWarpDuration = TimeSpan.FromHours(2); // Duration in which you have to activate spawn to do the spawn

        public static BaseAddon RockBarrier { get; set; }
        public static Point3D RockBarrierLocation = new Point3D(584, 2399, 0);

        public DateTime TimeWarpEnds { get; set; }
        public Timer Timer { get; set; }

        public override int LabelNumber { get { return 1157032; } } // Wheels of Time

        public WheelsOfTime()
            : base(0x9CEF)
        {
            Hue = 2655;
            Movable = false;

            RockBarrier = new KotlWallAddon();
            RockBarrier.MoveToWorld(RockBarrierLocation, Map.TerMur);

            TimeWarpEnds = DateTime.MinValue;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this.GetWorldLocation(), 1))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
            else if (TimeWarpEnds != DateTime.MinValue)
            {
                from.SendLocalizedMessage(1157021); // The wheels of time have already been set in motion.
            }
            else
            {
                from.PrivateOverheadMessage(Server.Network.MessageType.Regular, 1154, 1157020, from.NetState);
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

                TimeWarpEnds = DateTime.MinValue;
                Cleanup();
            }
        }

        private void Cleanup()
        {
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
            }
        }
    }
}