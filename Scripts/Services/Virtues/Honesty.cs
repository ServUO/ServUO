using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Items;

namespace Server.Services.Virtues
{
    class Honesty
    {
        private static List<Item> _HonestyItems = new List<Item>();
        private static List<Item> _Items = new List<Item>();
        private static List<Mobile> _Mobiles = new List<Mobile>();
        private static bool _GenRunning = false;

        public static void Initialize()
        {
            if (Core.SA)
            {
                VirtueGump.Register(106, OnVirtueUsed);


                EventSink.WorldSave += (eventArgs) =>
                {
                    _Items = World.Items.Values.Where(i => i.HonestyItem).ToList();
                    _Mobiles = World.Mobiles.Values.Where(m => (m.BodyValue == 400 || m.BodyValue == 401) && !m.Player).ToList();
                };


                EventSink.AfterWorldSave += (eventArgs) =>
            {

                if (!_GenRunning && World.Saving == false)
                    Task.Factory.StartNew(() => GenerateHonestyItems(_Mobiles, _Items));
            };
            }
        }


        public static void OnVirtueUsed(Mobile from)
        {
            from.SendLocalizedMessage(1053001); // This virtue is not activated through the virtue menu.
        }

        private static void GenerateHonestyItems(List<Mobile> mobiles, List<Item> items)
        {
            _GenRunning = true;

            try
            {
                _HonestyItems.Clear();
                _HonestyItems.AddRange(items);

                string[] regions =
                {
                    "Britain", "Minoc", "Magincia", "Trinsic", "Jhelom", "Moonglow", "Skara Brae", "Yew"
                };

                var index = _HonestyItems.Count;

                while (--index >= 0)
                {
                    if (index >= _HonestyItems.Count)
                        continue;

                    if (ItemFlags.GetTaken(_HonestyItems[index]))
                        _HonestyItems.RemoveAt(index);
                }

                if (_HonestyItems.Count < 1000 && World.Saving == false)
                {
                    int spawnamount = _HonestyItems.Count + 50 > 1000 ? _HonestyItems.Count%50 : 50;

                    for (int i = 0; i < spawnamount; i++)
                    {
                        Item toSpawn = Loot.RandomArmorOrShieldOrWeapon();
                        ItemFlags.SetTaken(toSpawn, false);
                        PlaceItemOnWorld(toSpawn);
                        _HonestyItems.Add(toSpawn);
                    }
                }

                for (int i = 0; i < _HonestyItems.Count; i++)
                {
                    _HonestyItems[i].HonestyItem = true;
                    _HonestyItems[i].HonestyRegion = regions[Utility.Random(regions.Length - 1)];
                    while (true)
                    {
                        var m = mobiles[Utility.Random(mobiles.Count - 1)];
                        if (m == null || m.Region.Name != _HonestyItems[i].HonestyRegion)
                            continue;
                        _HonestyItems[i].HonestyOwner = m;
                        break;
                    }
                }


                _GenRunning = false;
            }
            catch (Exception e)
            {
                //Shouldn't happen but it requires a bit more real world testing

                _GenRunning = false;
            }
        }

        private static void PlaceItemOnWorld(Item item)
        {
            Rectangle2D rect = new Rectangle2D(0, 0, 5119, 4095);

            var placeCoords = new Point3D();

            Map map = Utility.RandomBool() ? Map.Trammel : Map.Felucca;

            while (true)
            {
                var x = Utility.Random(rect.X, rect.Width);
                var y = Utility.Random(rect.Y, rect.Height);

                if (!TreasureMap.ValidateLocation(x, y, map)) continue;
                placeCoords.X = x;
                placeCoords.Y = y;
                placeCoords.Z = map.GetAverageZ(x, y);

                break;
            }

            item.MoveToWorld(placeCoords, map);
        }

    }

    public class HonestyChest : Container
    {
        [Constructable]
        public HonestyChest() : base(0x9A9)
        {
            Name = "Lost and Found box";
        }

        public HonestyChest(Serial serial) : base(serial)
        {

        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (!dropped.HonestyItem)
            {
                from.SendLocalizedMessage(1151530);
                return false;
            }
            Region reg = Region.Find(Location, Map);

            bool gainedPath = false;
            if (dropped.HonestyRegion == reg.Name)
            {
                VirtueHelper.Award(from, VirtueName.Honesty, 60, ref gainedPath);
            }
            else
            {
                VirtueHelper.Award(from, VirtueName.Honesty, 30, ref gainedPath);
            }

            from.SendMessage(gainedPath ? "You have gained a path in Honesty!" : "You have gained in Honesty.");

            dropped.Delete();
            return true;
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            if (!item.HonestyItem) return false;
            Region reg = Region.Find(Location, Map);

            bool gainedPath = false;
            if (item.HonestyRegion == reg.Name)
            {
                VirtueHelper.Award(from, VirtueName.Honesty, 60, ref gainedPath);
            }
            else
            {
                VirtueHelper.Award(from, VirtueName.Honesty, 30, ref gainedPath);
            }

            from.SendMessage(gainedPath ? "You have gained a path in Honesty!" : "You have gained in Honesty.");

            item.Delete();
            return true;
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