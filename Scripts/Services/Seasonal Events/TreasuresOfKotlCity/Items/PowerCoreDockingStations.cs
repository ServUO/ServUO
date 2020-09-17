using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;
using System.Linq;

namespace Server.Engines.TreasuresOfKotlCity
{
    public class PowerCoreDockingStation : BaseAddon
    {
        public override BaseAddonDeed Deed => null;

        public static List<PowerCoreDockingStation> Stations { get; set; }

        public InternalContainer Chest1 { get; set; }
        public InternalContainer Chest2 { get; set; }
        public bool Link { get; set; }

        private bool _Active;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active
        {
            get { return _Active; }
            set
            {
                bool active = _Active;
                _Active = value;

                if (!active && _Active)
                {
                    Activate();
                }
                else if (active && !_Active)
                {
                    Deactivate();
                }
            }
        }

        [Constructable]
        public PowerCoreDockingStation() : this(false)
        {
        }

        [Constructable]
        public PowerCoreDockingStation(bool link)
        {
            Link = link;

            if (link)
                AddStation();

            AddComplexComponent(40157, 0, 0, 36, 0, 1124171);// 19
            AddComplexComponent(40157, 1, 0, 36, 0, 1124171);// 20
            AddComplexComponent(40157, 0, 1, 36, 0, 1124171);// 21
            AddComplexComponent(40157, 1, 1, 36, 0, 1124171);// 22
            AddComplexComponent(40146, 0, 0, 11, 2591, 1157023);// 23

            AddComplexComponent(40173, 1, -1, 0, 2591);// 24
            AddComplexComponent(40173, 2, 0, 0, 2591);// 24
            AddComplexComponent(40174, 0, 2, 0, 2591);// 24

            AddComplexComponent(39736, 1, 0, 11, 2591);// 25
            AddComplexComponent(39728, 0, 1, 11, 2591);// 26
            AddComplexComponent(39734, 1, 1, 11, 2591);// 27
            AddComplexComponent(39730, 0, 0, 11, 2591);// 28
            AddComplexComponent(39714, -1, -1, 0, 2591);// 29
            AddComplexComponent(39713, -1, 1, 0, 2591);// 30
            AddComplexComponent(39713, -1, 0, 0, 2591);// 31
            AddComplexComponent(39712, 0, -1, 0, 2591);// 32
            AddComplexComponent(39712, 1, -1, 0, 2591);// 33
            AddComplexComponent(39713, 1, 0, 0, 2591);// 34
            AddComplexComponent(39712, 0, 1, 0, 2591);// 35
            AddComplexComponent(39711, 1, 1, 0, 2591);// 36

            AddComponent(new AddonComponent(0x9CE5), -1, 2, 0);

            Chest1 = new InternalContainer
            {
                Addon = this
            };

            Chest2 = new InternalContainer
            {
                Addon = this
            };
        }

        public void AddStation()
        {
            if (Stations == null)
                Stations = new List<PowerCoreDockingStation>();

            if (!Stations.Contains(this))
                Stations.Add(this);
        }

        public override void OnLocationChange(Point3D oldlocation)
        {
            base.OnLocationChange(oldlocation);

            if (Chest1 != null && !Chest1.Deleted)
            {
                Chest1.MoveToWorld(new Point3D(X + 1, Y + 1, Z + 11), Map);
            }

            if (Chest2 != null && !Chest2.Deleted)
            {
                Chest2.MoveToWorld(new Point3D(X, Y + 1, Z + 11), Map);
            }
        }

        public override void OnMapChange()
        {
            base.OnMapChange();

            if (Chest1 != null)
                Chest1.Map = Map;

            if (Chest2 != null)
                Chest2.Map = Map;
        }

        public void Activate()
        {
            if (Link && Stations != null && Stations.Where(s => s.Active).Count() == Stations.Count)
            {
                if (!KotlBattleSimulator.Instance.Active)
                {
                    KotlBattleSimulator.Instance.Active = true;

                    Region r = Region.Find(KotlBattleSimulator.Instance.Location, Map.TerMur);

                    if (r != null)
                    {
                        foreach (PlayerMobile pm in r.GetEnumeratedMobiles().OfType<PlayerMobile>())
                            pm.SendLocalizedMessage(1157026); // [Kotl City Hologenerator]:  Great Battle simulation now active! Proceed to city center!
                    }
                }
            }

            foreach (AddonComponent comp in Components)
            {
                if (comp.ItemID == 40146)
                    comp.ItemID = 40147;

                if (comp.ItemID == 40173)
                    comp.ItemID = 40142;

                if (comp.ItemID == 40174)
                    comp.ItemID = 40169;
            }
        }

        public void Deactivate()
        {
            foreach (AddonComponent comp in Components)
            {
                if (comp.ItemID == 40147)
                    comp.ItemID = 40146;

                if (comp.ItemID == 40142)
                    comp.ItemID = 40173;

                if (comp.ItemID == 40169)
                    comp.ItemID = 40174;
            }

            List<Item> delete = new List<Item>();

            if (Chest1 != null)
            {
                delete.AddRange(Chest1.Items);
            }

            if (Chest2 != null)
            {
                delete.AddRange(Chest2.Items);
            }

            foreach (Item item in delete)
                item.Delete();

            _Active = false;
        }

        private void AddComplexComponent(int item, int xoffset, int yoffset, int zoffset, int hue, int localization = 0)
        {
            AddonComponent ac;

            if (localization == 0)
            {
                ac = new AddonComponent(item);
            }
            else
            {
                ac = new LocalizedAddonComponent(item, localization);
            }

            if (hue != 0)
                ac.Hue = hue;

            AddComponent(ac, xoffset, yoffset, zoffset);
        }

        public override void Delete()
        {
            base.Delete();

            if (Chest1 != null && !Chest1.Deleted)
            {
                Chest1.Delete();
            }

            if (Chest2 != null && !Chest2.Deleted)
            {
                Chest2.Delete();
            }

            if (Stations != null && Stations.Contains(this))
            {
                Stations.Remove(this);
            }
        }

        public PowerCoreDockingStation(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version

            writer.Write(_Active);
            writer.Write(Chest1);
            writer.Write(Chest2);
            writer.Write(Link);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            _Active = reader.ReadBool();
            Chest1 = reader.ReadItem() as InternalContainer;
            Chest2 = reader.ReadItem() as InternalContainer;
            Link = reader.ReadBool();

            if (Link)
                AddStation();

            if (Chest1 != null)
                Chest1.Addon = this;

            if (Chest2 != null)
                Chest2.Addon = this;
        }

        public class InternalContainer : BaseContainer
        {
            public PowerCoreDockingStation Addon { get; set; }

            public InternalContainer() : base(16421)
            {
                Movable = false;
                Hue = 2591;
            }

            public override bool OnDragDrop(Mobile from, Item dropped)
            {
                return CheckDrop(from, dropped);
            }

            public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
            {
                return CheckDrop(from, item);
            }

            private bool CheckDrop(Mobile from, Item dropped)
            {
                if (dropped is StasisChamberPowerCore)
                {
                    if (Addon != null && Addon.Active)
                    {
                        from.SendLocalizedMessage(1157025); // This power station is already active.
                    }
                    else if (Addon != null)
                    {
                        base.OnDragDrop(from, dropped);
                        dropped.Movable = false;

                        Addon.Active = true;
                        return true;
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1157024); // The docking station only accepts power cores.
                }

                return false;
            }

            public override void Delete()
            {
                base.Delete();

                if (Addon != null && !Addon.Deleted)
                {
                    Addon.Delete();
                }
            }

            public InternalContainer(Serial serial)
                : base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
                writer.Write(0); // Version
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
                int version = reader.ReadInt();
            }
        }
    }
}
