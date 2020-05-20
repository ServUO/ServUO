using System.Linq;

namespace Server.Items
{
    public class PlunderBeacon : Beacon
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public PlunderBeaconAddon Controller { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile LastDamager { get; set; }

        public override bool CanDamage => Controller == null || Controller.BeaconVulnerable;

        public PlunderBeacon(PlunderBeaconAddon controller)
        {
            Controller = controller;
            Name = "a plunderbeacon";

            ResistBasePhys = 0;
            ResistBaseFire = 0;
            ResistBaseCold = 0;
            ResistBasePoison = 0;
            ResistBaseEnergy = 0;

            HitsMax = 65001;
            Hits = HitsMax;
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
                System.Collections.Generic.List<Mobile> eligables = DamageStore.Keys.Where(m => m.InRange(Location, 20)).ToList();

                for (int i = 0; i < eligables.Count; i++)
                {
                    Mobile winner = eligables[i];

                    winner.AddToBackpack(new MaritimeCargo(CargoQuality.Mythical));
                    winner.SendLocalizedMessage(1158907); // You recover maritime trade cargo!
                }
            }

            if (Controller != null)
                Controller.OnBeaconDestroyed();

            return base.OnBeforeDestroyed();
        }

        public PlunderBeacon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.WriteItem(Controller);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            Controller = reader.ReadItem<PlunderBeaconAddon>();
        }
    }
}
