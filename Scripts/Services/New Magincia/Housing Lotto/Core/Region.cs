using Server.Regions;
using System.Collections.Generic;
using System.Xml;

namespace Server.Engines.NewMagincia
{
    public class NewMaginciaRegion : TownRegion
    {
        public NewMaginciaRegion(XmlElement xml, Map map, Region parent) : base(xml, map, parent)
        {
        }

        public override bool AllowHousing(Mobile from, Point3D p)
        {
            MaginciaLottoSystem system = MaginciaLottoSystem.Instance;

            if (system != null && system.Enabled && from.Backpack != null)
            {
                List<Item> items = new List<Item>();

                Item[] packItems = from.Backpack.FindItemsByType(typeof(WritOfLease));
                Item[] bankItems = from.BankBox.FindItemsByType(typeof(WritOfLease));

                if (packItems != null && packItems.Length > 0)
                    items.AddRange(packItems);

                if (bankItems != null && bankItems.Length > 0)
                    items.AddRange(bankItems);

                foreach (Item item in items)
                {
                    WritOfLease lease = item as WritOfLease;

                    if (lease != null && !lease.Expired && lease.Plot != null && lease.Plot.Bounds.Contains(p) && from.Map == lease.Plot.Map)
                        return true;
                }
            }

            return MaginciaLottoSystem.IsFreeHousingZone(p, Map);
        }
    }
}