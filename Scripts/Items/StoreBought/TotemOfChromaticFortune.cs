using Server.Engines.PartySystem;
using Server.Guilds;
using Server.Mobiles;
using Server.Multis;
using Server.Regions;
using System;

namespace Server.Items
{
    public class TotemOfChromaticFortune : Item
    {
        public override int LabelNumber => 1157606;  // Totem of Chromatic Fortune

        private int _Charges;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges { get => _Charges; set { _Charges = value; InvalidateProperties(); } }

        [Constructable]
        public TotemOfChromaticFortune()
            : base(0x9F1B)
        {
            _Charges = 3;
            Hue = 2721;
            LootType = LootType.Blessed;
        }

        public TotemOfChromaticFortune(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                var b = BaseBoat.FindBoatAt(from, from.Map);

                if (from.Region.IsPartOf(typeof(HouseRegion)) || b != null)
                {
                    from.SendLocalizedMessage(1157483); // You may not use this while inside a house or on a boat.
                    return;
                }

                if (CheckAddon(from.Location))
                {
                    from.SendLocalizedMessage(1157484); // A Totem is already active in this area.
                    return;
                }

                if (CheckSpawn(from.Location))
                {
                    var totem = new TotemOfChromaticFortuneAddon(from);
                    totem.MoveToWorld(from.Location, from.Map);
                    Charges--;
                }
                else
                {
                    from.SendLocalizedMessage(1157548); // You may not use a totem here.
                }
            }
            else
            {
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
            }
        }

        private static readonly Type[] m_Types = {
            typeof(SnowLeopard),    typeof(FrostSpider),
            typeof(Nightmare),   typeof(WhiteWyrm),
            typeof(GiantToad), typeof(StygianDrake)
        };

        private bool CheckAddon(Point3D p)
        {
            IPooledEnumerable<Item> eable = Map.GetItemsInRange(p, 5);

            foreach (Item item in eable)
            {
                if (item is TotemOfChromaticFortuneAddon)
                {
                    eable.Free();
                    return true;
                }
            }

            eable.Free();

            return false;
        }

        private bool CheckSpawn(Point3D p)
        {
            IPooledEnumerable<Item> eable = Map.GetItemsInRange(p, 12);

            foreach (Item item in eable)
            {
                if (item is XmlSpawner xmlspawn)
                {
                    foreach (var so in xmlspawn.SpawnObjects)
                    {
                        foreach (var t in m_Types)
                        {
                            if (t.Name.ToLower() == so.TypeName.ToLower())
                            {
                                eable.Free();
                                return true;
                            }
                        }
                    }
                }

                if (item is Spawner spawn)
                {
                    foreach (var so in spawn.SpawnObjects)
                    {
                        foreach (var t in m_Types)
                        {
                            if (t.Name.ToLower() == so.SpawnName.ToLower())
                            {
                                eable.Free();
                                return true;
                            }
                        }
                    }
                }
            }

            eable.Free();

            return false;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060741, _Charges.ToString());
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(_Charges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            _Charges = reader.ReadInt();
        }
    }

    public class TotemOfChromaticFortuneAddon : BaseDecayingItem
    {
        public override int LabelNumber => 1157606;  // Totem of Chromatic Fortune

        public override bool ForceShowProperties => true;

        public override int Lifespan => 1200;
        public override bool UseSeconds => false;
        private Item m_Sparkle;

        public Mobile Owner { get; set; }

        [Constructable]
        public TotemOfChromaticFortuneAddon(Mobile from)
            : base(0x9F1A)
        {
            LootType = LootType.Regular;
            Movable = false;
            Weight = 0.0;
            Owner = from;

            m_Sparkle = new Static(0x375A);
        }

        public bool IsGuildOrParty(Mobile to)
        {
            return IsGuild(to) || IsParty(to);
        }

        public bool IsGuild(Mobile to)
        {
            return Owner.Guild is Guild fromGuild && to.Guild is Guild toGuild && fromGuild == toGuild;
        }

        public bool IsParty(Mobile to)
        {
            var p = Party.Get(Owner);

            return p != null && p.Contains(to);
        }

        public bool CheckParty(Mobile m)
        {
            Party party = Party.Get(Owner);

            if (party != null)
            {
                foreach (PartyMemberInfo info in party.Members)
                {
                    if (info.Mobile == m)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public TotemOfChromaticFortuneAddon(Serial serial)
            : base(serial)
        {
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);

            if (Owner != null)
            {
                list.Add(1041602, "{0}", Owner.Name); // Owner: ~1_val~
            }
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_Sparkle != null)
            {
                m_Sparkle.Delete();
            }
        }

        public override void OnMapChange()
        {
            if (Deleted)
            {
                return;
            }

            if (m_Sparkle != null)
            {
                m_Sparkle.Map = Map;
            }
        }

        public override void OnLocationChange(Point3D oldLoc)
        {
            if (Deleted)
            {
                return;
            }

            if (m_Sparkle != null)
            {
                m_Sparkle.Location = new Point3D(X , Y, Z + 31);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write(m_Sparkle);
            writer.Write(Owner);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            m_Sparkle = reader.ReadItem();
            Owner = reader.ReadMobile();
        }
    }

}
