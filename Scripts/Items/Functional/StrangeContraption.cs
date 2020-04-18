using Server.ContextMenus;
using Server.Engines.Quests.Collector;
using Server.Engines.Quests.Hag;
using Server.Gumps;
using Server.Network;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class StrangeContraptionComponent : AddonComponent
    {
        public override bool ForceShowProperties => true;

        public StrangeContraptionComponent(int id, int hue = 0)
            : base(id)
        {
            Name = "a strange contraption";
            Hue = hue;
        }

        public StrangeContraptionComponent(Serial serial)
            : base(serial)
        {
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (ItemID == 6434)
            {
                list.Add(new DeviceEntry(from, this));
            }
        }

        private class DeviceEntry : ContextMenuEntry
        {
            private readonly Mobile m_Mobile;
            private readonly AddonComponent m_Component;

            public DeviceEntry(Mobile mobile, AddonComponent c)
                : base(3006190, 16) // Appraise for Cleanup
            {
                m_Mobile = mobile;
                m_Component = c;
            }

            public bool CanDrop(Mobile m)
            {
                if (!StrangeContraptionAddon.Table.ContainsKey(m))
                    return true;

                if (StrangeContraptionAddon.Table[m] < DateTime.UtcNow)
                {
                    StrangeContraptionAddon.Table.Remove(m);
                    return true;
                }
                else
                {
                    m.SendLocalizedMessage(1060001); // You throw the switch, but the mechanism cannot be engaged again so soon.
                }

                return false;
            }

            public override void OnClick()
            {
                if (m_Mobile.InRange(m_Component.Location, 2))
                {
                    if (CanDrop(m_Mobile))
                    {
                        Container pack = m_Mobile.Backpack as Container;

                        if (pack == null)
                            return;

                        Item pmc = pack.FindItemByType(typeof(PlagueBeastMutationCore));
                        Item obs = pack.FindItemByType(typeof(Obsidian));
                        Item mb = pack.FindItemByType(typeof(MoonfireBrew));

                        if (pmc != null && obs != null && mb != null)
                        {
                            m_Mobile.PlaySound(0x21E);
                            m_Mobile.SendLocalizedMessage(1055143); // You add the required ingredients and activate the contraption. It rumbles and smokes and then falls silent. The water shines for a brief moment, and you feel confident that it is now much less tainted then before. 
                            m_Mobile.Karma += 500;

                            pmc.Delete();
                            obs.Delete();
                            mb.Delete();

                            StrangeContraptionAddon.Table[m_Mobile] = DateTime.UtcNow + TimeSpan.FromHours(1);
                        }
                        else
                        {
                            m_Mobile.SendLocalizedMessage(1055142, "", 0x59); // You do not have the necessary ingredients. The contraptions rumbles angrily but does nothing.
                        }
                    }
                }
                else
                {
                    m_Mobile.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                }
            }
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

    public class StrangeContraptionAddon : BaseAddon
    {
        public static StrangeContraptionAddon InstanceTram { get; set; }
        public static StrangeContraptionAddon InstanceFel { get; set; }

        public static void Initialize()
        {
            if (InstanceTram == null)
            {
                StrangeContraptionAddon item = new StrangeContraptionAddon();
                item.MoveToWorld(new Point3D(5668, 1925, 0), Map.Trammel);
            }

            if (InstanceFel == null)
            {
                StrangeContraptionAddon item = new StrangeContraptionAddon();
                item.MoveToWorld(new Point3D(5668, 1925, 0), Map.Felucca);
            }
        }

        public static Dictionary<Mobile, DateTime> Table = new Dictionary<Mobile, DateTime>();

        public StrangeContraptionAddon()
        {
            // Table
            AddComponent(new AddonComponent(2923), 1, 2, 1);
            AddComponent(new AddonComponent(2924), 1, 1, 1);

            // Book
            AddComponent(new AddonComponent(0x0FF4), 1, 2, 7);

            // Water
            AddComponent(new AddonComponent(6063), -1, 0, 1);
            AddComponent(new AddonComponent(6066), 0, 0, 1);
            AddComponent(new AddonComponent(6064), -1, 1, 1);
            AddComponent(new AddonComponent(6065), 0, 1, 1);

            AddComponent(new StrangeContraptionComponent(4758), 0, -1, 1);
            AddComponent(new StrangeContraptionComponent(4758), 0, 0, 1);
            AddComponent(new StrangeContraptionComponent(2813), 2, 0, 19);
            AddComponent(new StrangeContraptionComponent(2815), 1, 0, 19);
            AddComponent(new StrangeContraptionComponent(2816), 2, 0, 16);
            AddComponent(new StrangeContraptionComponent(2818), 1, 0, 16);
            AddComponent(new StrangeContraptionComponent(4715), 1, 0, 1);
            AddComponent(new StrangeContraptionComponent(2643), 1, 0, 3);
            AddComponent(new StrangeContraptionComponent(6434), 2, 0, 1);
            AddComponent(new StrangeContraptionComponent(4758, 1545), -1, 0, 0);
            AddComponent(new StrangeContraptionComponent(4272, 1545), -1, 0, 1);
            AddComponent(new StrangeContraptionComponent(6039, 999), 2, 0, 20);
            AddComponent(new StrangeContraptionComponent(6040, 999), 1, 0, 20);
        }

        public override void OnComponentUsed(AddonComponent comp, Mobile from)
        {
            if (comp.ItemID == 0x0FF4)
            {
                if (!from.InRange(comp.GetWorldLocation(), 2))
                {
                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                }
                else
                {
                    Gump g = new Gump(35, 70);
                    g.AddImage(0, 0, 500);
                    g.AddHtmlLocalized(40, 17, 150, 220, 1055141, false, false);
                    from.SendGump(g);
                }
            }
        }

        public StrangeContraptionAddon(Serial serial)
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

            if (Map == Map.Trammel)
            {
                InstanceTram = this;
            }
            else if (Map == Map.Felucca)
            {
                InstanceFel = this;
            }
        }
    }
}
