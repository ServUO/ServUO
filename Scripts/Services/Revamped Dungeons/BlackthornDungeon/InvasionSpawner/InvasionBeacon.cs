using System;
using Server;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using Server.Items;
using Server.Engines.CityLoyalty;

namespace Server.Engines.Blackthorn
{
    public class InvasionBeacon : Beacon
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public InvasionController Controller { get; set; }

        public override bool CanDamage { get { return Controller == null || Controller.BeaconVulnerable; } }

        public InvasionBeacon(InvasionController controller)
        {
            Controller = controller;
            Name = "lighthouse";
            Level = ItemLevel.Easy; // Hard
        }

        public override void OnHalfDamage()
        {
            IPooledEnumerable eable = this.Map.GetMobilesInRange(this.Location, 20);

            foreach (Mobile m in eable)
            {
                if (m.NetState != null)
                    m.PrivateOverheadMessage(Server.Network.MessageType.Regular, 1154, 1154551, m.NetState); // *Minax's Beacon surges with energy into an invulnerable state! Defeat her Captains to weaken the Beacon's defenses!*
            }

            eable.Free();

            if (Controller != null)
                Timer.DelayCall(TimeSpan.FromSeconds(1), () => Controller.SpawnWave());
        }

        public override bool OnBeforeDestroyed()
        {
            if (Controller != null)
                Controller.OnBeaconDestroyed();

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
