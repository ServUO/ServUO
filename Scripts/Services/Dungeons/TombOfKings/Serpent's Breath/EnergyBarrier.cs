using System;

namespace Server.Items
{
    public class EnergyBarrier : Item
    {
        private static readonly string _TimerID = "EnergyBarrierTimer";

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active
        {
            get { return Visible; }
            set
            {
                if (Active != value)
                {
                    Visible = value;

                    if (Active)
                    {
                        ActivateTimer();
                    }
                    else
                    {
                        DeactivateTimer();
                    }
                }
            }
        }

        public EnergyBarrier(Point3D location, Map map)
            : base(0x3967)
        {
            Movable = false;
            Light = LightType.Circle150;

            MoveToWorld(location, map);
        }

        protected void PlaySound()
        {
            if (Active)
            {
                Effects.PlaySound(Location, Map, Utility.RandomList(0x1DC, 0x210, 0x2F4));
                TimerRegistry.UpdateRegistry(_TimerID, this, TimeSpan.FromSeconds(Utility.RandomMinMax(5, 25)));
            }
            else
            {
                TimerRegistry.RemoveFromRegistry(_TimerID, this);
            }
        }

        private void ActivateTimer()
        {
            TimerRegistry.Register(_TimerID, this, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), false, barrier => barrier.PlaySound());
        }

        private void DeactivateTimer()
        {
            TimerRegistry.RemoveFromRegistry(_TimerID, this);
        }

        public override void OnSectorActivate()
        {
            ActivateTimer();
        }

        public override void OnSectorDeactivate()
        {
            DeactivateTimer();
        }

        public EnergyBarrier(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
