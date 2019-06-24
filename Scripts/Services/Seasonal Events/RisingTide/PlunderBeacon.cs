using System;
using System.Linq;
using System.Collections.Generic;

using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Items
{
    public class PlunderBeacon : Beacon
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public PlunderBeaconAddon Controller { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile LastDamager { get; set; }

        public override bool CanDamage { get { return Controller == null || Controller.BeaconVulnerable; } }

        public PlunderBeacon(PlunderBeaconAddon controller)
        {
            Controller = controller;
            Name = "a plunderbeacon";

            ResistBasePhys = 0;
            ResistBaseFire = 0;
            ResistBaseCold = 0;
            ResistBasePoison = 0;
            ResistBaseEnergy = 0;

            HitsMax = 70000;
            Hits = HitsMax;
        }

        public override void OnHalfDamage()
        {
            /*IPooledEnumerable eable = this.Map.GetMobilesInRange(this.Location, 20);

            foreach (Mobile m in eable)
            {
                if (m.NetState != null)
                    m.PrivateOverheadMessage(Server.Network.MessageType.Regular, 1154, 1154551, m.NetState); // *Minax's Beacon surges with energy into an invulnerable state! Defeat her Captains to weaken the Beacon's defenses!*
            }

            eable.Free();

            if (Controller != null)
                Timer.DelayCall(TimeSpan.FromSeconds(1), () => Controller.SpawnWave());*/
        }

        public override bool CheckAreaDamage(Mobile from, int amount)
        {
            if (amount >= 5000)
            {
                return false;
            }

            return base.CheckAreaDamage(from, amount);
        }

        public override void OnDamage(int amount, Mobile from, bool willkill)
        {
            base.OnDamage(amount, from, willkill);

            LastDamager = from;
        }

        public override bool OnBeforeDestroyed()
        {
            if (DamageStore != null)
            {
                var eligables = DamageStore.Keys.Where(m => m.InRange(Location, 20)).ToList();

                if (eligables.Count > 0 && 0.5 > Utility.RandomDouble())
                {
                    var winner = eligables[Utility.Random(eligables.Count)];

                    if (winner != null)
                    {
                        winner.AddToBackpack(new MaritimeCargo(CargoQuality.Mythical));
                        winner.SendLocalizedMessage(1158907); // You recover maritime trade cargo!
                    }
                }
            }

            if (Controller != null)
                Controller.OnBeaconDestroyed();

            return base.OnBeforeDestroyed();
        }

        public override void Delete()
        {
            base.Delete();

            if (Controller != null && !Controller.Deleted)
            {
                Controller.Delete();
            }
        }

        public PlunderBeacon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.WriteItem<PlunderBeaconAddon>(Controller);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            Controller = reader.ReadItem<PlunderBeaconAddon>();
        }
    }
}
