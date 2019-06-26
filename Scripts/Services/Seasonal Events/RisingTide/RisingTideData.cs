using System;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Engines.SeasonalEvents;

namespace Server.Engines.Points
{
    public class RisingTide : PointsSystem
    {
        public override PointsType Loyalty { get { return PointsType.RisingTide; } }
        public override TextDefinition Name { get { return m_Name; } }
        public override bool AutoAdd { get { return true; } }
        public override double MaxPoints { get { return double.MaxValue; } }
        public override bool ShowOnLoyaltyGump { get { return false; } }

        public bool InSeason { get { return SeasonalEventSystem.IsActive(EventType.RisingTide); } }
        private TextDefinition m_Name = null;

        public static readonly double CargoChance = 0.1;

        public RisingTide()
        {
        }

        public override void SendMessage(PlayerMobile from, double old, double points, bool quest)
        {
            from.SendLocalizedMessage(1158910, ((int)points).ToString()); // You have ~1_COUNT~ doubloons!
        }

        public override void ProcessKill(Mobile victim, Mobile damager)
        {
            if (Enabled && victim is BaseCreature && damager is PlayerMobile)
            {
                var bc = victim as BaseCreature;
                var beacon = GetPlunderBeaconAt(bc);

                if (beacon != null)
                {
                    if (CargoChance > Utility.RandomDouble())
                    {
                        damager.AddToBackpack(new MaritimeCargo());
                        damager.SendLocalizedMessage(1158907); // You recover maritime trade cargo!
                    }
                }
                else if (CargoDropsTypes.Any(type => type == bc.GetType()))
                {
                    double chance = 0.1;

                    if (bc is BaseShipCaptain)
                    {
                        chance = 0.33;
                    }

                    if (chance > Utility.RandomDouble())
                    {
                        var corpse = victim.Corpse;

                        if (corpse != null)
                        {
                            corpse.DropItem(new MaritimeCargo());
                        }
                    }
                }
            }
        }

        private Type[] CargoDropsTypes =
        {
            typeof(PirateCaptain), typeof(MerchantCaptain), typeof(PirateCrew), typeof(MerchantCrew)
        };

        public static PlunderBeaconAddon GetPlunderBeaconAt(IEntity e)
        {
            if (e == null || e.Map == null || e.Map == Map.Internal)
                return null;

            IPooledEnumerable eable = e.Map.GetItemsInRange(e.Location, 0);

            foreach (Item item in eable)
            {
                if (item is PlunderBeaconAddon)
                {
                    eable.Free();
                    return (PlunderBeaconAddon)item;
                }
                else if (item is AddonComponent && ((AddonComponent)item).Addon is PlunderBeaconAddon)
                {
                    eable.Free();
                    return (PlunderBeaconAddon)((AddonComponent)item).Addon;
                }
            }

            eable.Free();
            return null;
        }

        public bool Enabled { get; set; }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(Enabled);

            if (PlunderBeaconSpawner.Spawner != null)
            {
                writer.Write(0);
                PlunderBeaconSpawner.Spawner.Serialize(writer);
            }
            else
            {
                writer.Write(1);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            Enabled = reader.ReadBool();

            if (reader.ReadInt() == 0)
            {
                var spawner = new PlunderBeaconSpawner();
                spawner.Deserialize(reader);
            }
        }
    }
}
