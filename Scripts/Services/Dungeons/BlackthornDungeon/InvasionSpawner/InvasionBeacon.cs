using Server.Items;
using System;

namespace Server.Engines.Blackthorn
{
    public class InvasionBeacon : Beacon
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public InvasionController Controller { get; set; }

        public override bool CanDamage => Controller == null || Controller.BeaconVulnerable;

        [Constructable]
        public InvasionBeacon()
            : this(null)
        {
        }

        public InvasionBeacon(InvasionController controller)
        {
            Controller = controller;
            Name = "lighthouse";
            Level = ItemLevel.Easy; // Hard
        }

        public override void OnHalfDamage()
        {
            if (Map == null)
            {
                return;
            }

            IPooledEnumerable eable = Map.GetMobilesInRange(Location, 20);

            foreach (Mobile m in eable)
            {
                if (m.NetState != null)
                    m.PrivateOverheadMessage(Network.MessageType.Regular, 1154, 1154551, m.NetState); // *Minax's Beacon surges with energy into an invulnerable state! Defeat her Captains to weaken the Beacon's defenses!*
            }

            eable.Free();

            if (Controller != null)
                Timer.DelayCall(TimeSpan.FromSeconds(1), () => Controller.SpawnWave());
        }

        public override bool OnBeforeDestroyed()
        {
            if (Controller != null)
            {
                Controller.OnBeaconDestroyed();
            }
            else
            {
                Timer.DelayCall(TimeSpan.FromMinutes(1), Delete);
            }

            return base.OnBeforeDestroyed();
        }

        public InvasionBeacon(Serial serial)
        : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
