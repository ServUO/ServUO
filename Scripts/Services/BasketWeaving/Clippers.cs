#region References
using System;
using System.Collections.Generic;
using Server.Gumps;
using Server.Network;
using Server.ContextMenus;
using Server.Engines.Craft;
using Server.Engines.Plants;
using Server.Mobiles;
using Server.Targeting;
#endregion

namespace Server.Items
{
    [Flipable(0x0DFC, 0x0DFD)]
    public class Clippers : BaseTool
    {
        public override int LabelNumber { get { return 1112117; } } // clippers

        [Constructable]
        public Clippers()
            : base(0x0DFC)
        {
            this.Weight = 1.0;
            this.Hue = 1168;
        }

        [Constructable]
        public Clippers(int uses)
            : base(uses, 0x0DFC)
        {
            this.Weight = 1.0;
            this.Hue = 1168;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            //Makers mark not displayed on OSI
            if (Crafter != null)
            {
                list.Add(1050043, Crafter.TitleName); // crafted by ~1_NAME~
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            AddContextMenuEntries(from, this, list);
        }

        public static void AddContextMenuEntries(Mobile from, Item item, List<ContextMenuEntry> list)
        {
            if (!item.IsChildOf(from.Backpack) && item.Parent != from)
                return;

            PlayerMobile pm = from as PlayerMobile;

            if (pm == null)
                return;

            list.Add(new ToggleClippings(pm, true, false, false, 1112282)); //Set to clip plants
            list.Add(new ToggleClippings(pm, false, true, false, 1112283)); //Set to cut reeds
            list.Add(new ToggleClippings(pm, false, false, true, 1150660)); //Set to cut topiaries
        }

        private class ToggleClippings : ContextMenuEntry
        {
            private readonly PlayerMobile m_Mobile;
            private readonly bool m_Valueclips;
            private readonly bool m_Valuereeds;
            private readonly bool m_Valuetopiaries;

            public ToggleClippings(PlayerMobile mobile, bool valueclips, bool valuereeds, bool valuetopiaries, int number)
                : base(number)
            {
                m_Mobile = mobile;
                m_Valueclips = valueclips;
                m_Valuereeds = valuereeds;
                m_Valuetopiaries = valuetopiaries;
            }

            public override void OnClick()
            {
                bool oldValueclips = m_Mobile.ToggleCutClippings;
                bool oldValuereeds = m_Mobile.ToggleCutReeds;
                bool oldValuetopiaries = m_Mobile.ToggleCutTopiaries;

                if (m_Valueclips)
                {
                    if (oldValueclips)
                    {
                        m_Mobile.ToggleCutClippings = true;
                        m_Mobile.ToggleCutReeds = false;
                        m_Mobile.ToggleCutTopiaries = false;
                        m_Mobile.SendLocalizedMessage(1112284); // You are already set to make plant clippings 
                    }
                    else
                    {
                        m_Mobile.ToggleCutClippings = true;
                        m_Mobile.ToggleCutReeds = false;
                        m_Mobile.ToggleCutTopiaries = false;
                        m_Mobile.SendLocalizedMessage(1112285); // You are now set to make plant clippings
                    }
                }
                else if (m_Valuereeds)
                {
                    if (oldValuereeds)
                    {
                        m_Mobile.ToggleCutReeds = true;
                        m_Mobile.ToggleCutClippings = false;
                        m_Mobile.ToggleCutTopiaries = false;
                        m_Mobile.SendLocalizedMessage(1112287); // You are already set to cut reeds. 
                    }
                    else
                    {
                        m_Mobile.ToggleCutReeds = true;
                        m_Mobile.ToggleCutClippings = false;
                        m_Mobile.ToggleCutTopiaries = false;
                        m_Mobile.SendLocalizedMessage(1112286); // You are now set to cut reeds.
                    }
                }
                else if (m_Valuetopiaries)
                {
                    if (oldValuetopiaries)
                    {
                        m_Mobile.ToggleCutTopiaries = true;
                        m_Mobile.ToggleCutReeds = false;
                        m_Mobile.ToggleCutClippings = false;
                        m_Mobile.SendLocalizedMessage(1150653); // You are already set to cut topiaries! 
                    }
                    else
                    {
                        m_Mobile.ToggleCutTopiaries = true;
                        m_Mobile.ToggleCutReeds = false;
                        m_Mobile.ToggleCutClippings = false;
                        m_Mobile.SendLocalizedMessage(1150652); // You are now set to cut topiaries.
                    }
                }
            }
        }

        public Clippers(Serial serial)
            : base(serial)
        { }

        public virtual PlantHue PlantHue { get { return PlantHue.None; } }

        public override CraftSystem CraftSystem
        {
            get
            {
                return DefTinkering.CraftSystem;
            }
        }

        public void ConsumeUse(Mobile from)
        {
            if (this.UsesRemaining > 1)
            {
                --this.UsesRemaining;
            }
            else
            {
                if (from != null)
                    from.SendLocalizedMessage(1112126); // Your clippers break as you use up the last charge..

                this.Delete();
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1112118); // What plant do you wish to use these clippers on?
                from.Target = new InternalTarget(this);
            }
        }

        private class InternalTarget : Target
        {
            private readonly Clippers m_Item;

            public InternalTarget(Clippers item)
                : base(2, false, TargetFlags.None)
            {
                m_Item = item;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                PlayerMobile pm = from as PlayerMobile;

                if (pm == null || m_Item == null || m_Item.Deleted)
                {
                    return;
                }

                PlantItem plant = targeted as PlantItem;

                if (null == plant || PlantStatus.DecorativePlant != plant.PlantStatus)
                {
                    from.SendLocalizedMessage(1112119); // You may only use these clippers on decorative plants.
                    return;
                }

                if (pm.ToggleCutClippings)
                {
                    from.PlaySound(0x248);
                    from.AddToBackpack(
                        new PlantClippings
                        {
                            Hue = ((PlantItem)targeted).Hue,
                            PlantHue = plant.PlantHue
                        });
                    plant.Delete();
                    m_Item.ConsumeUse(from);
                }
                else if (pm.ToggleCutReeds)
                {
                    from.PlaySound(0x248);
                    from.AddToBackpack(
                        new DryReeds
                        {
                            Hue = ((PlantItem)targeted).Hue,
                            PlantHue = plant.PlantHue
                        });
                    plant.Delete();
                    m_Item.ConsumeUse(from);
                }
                else if (pm.ToggleCutTopiaries)
                {
                    if (plant.PlantType == PlantType.HedgeTall || plant.PlantType == PlantType.HedgeShort || plant.PlantType == PlantType.JuniperBush)
                    {
                        from.CloseGump(typeof(TopiaryGump));
                        from.SendGump(new TopiaryGump(plant, m_Item));
                    }
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class TopiaryGump : Gump
    {
        PlantItem m_plant;
        Clippers m_clippers;

        public TopiaryGump(PlantItem plant, Clippers clippers) : base(0, 0)
        {
            m_plant = plant;
            m_clippers = clippers;

            AddPage(0);

            AddBackground(50, 89, 508, 195, 2600);

            AddLabel(103, 114, 0, @"Choose a Topiary:");

            AddButton(92, 155, 1209, 1210, 1, GumpButtonType.Reply, 0);
            AddItem(75, 178, 18713);

            AddButton(133, 155, 1209, 1210, 2, GumpButtonType.Reply, 0);
            AddItem(119, 178, 18714);

            AddButton(177, 155, 1209, 1210, 3, GumpButtonType.Reply, 0);
            AddItem(165, 182, 18715);

            AddButton(217, 155, 1209, 1210, 4, GumpButtonType.Reply, 0);
            AddItem(205, 182, 18736);

            AddButton(267, 155, 1209, 1210, 5, GumpButtonType.Reply, 0);
            AddItem(220, 133, 18813);

            AddButton(333, 155, 1209, 1210, 6, GumpButtonType.Reply, 0);
            AddItem(272, 133, 18814);

            AddButton(388, 155, 1209, 1210, 7, GumpButtonType.Reply, 0);
            AddItem(374, 178, 18784);

            AddButton(426, 155, 1209, 1210, 8, GumpButtonType.Reply, 0);
            AddItem(413, 175, 18713);

            AddButton(480, 155, 1209, 1210, 9, GumpButtonType.Reply, 0);
            AddItem(463, 176, 19369);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            switch (info.ButtonID)
            {
                case 0:
                    {
                        break;
                    }
                case 1:
                    {
                        from.PlaySound(0x248);
                        m_plant.ItemID = 18713;
                        m_clippers.ConsumeUse(from);
                        break;
                    }
                case 2:
                    {
                        from.PlaySound(0x248);
                        m_plant.ItemID = 18714;
                        m_clippers.ConsumeUse(from);
                        break;
                    }
                case 3:
                    {
                        from.PlaySound(0x248);
                        m_plant.ItemID = 18715;
                        m_clippers.ConsumeUse(from);
                        break;
                    }
                case 4:
                    {
                        from.PlaySound(0x248);
                        m_plant.ItemID = 18736;
                        m_clippers.ConsumeUse(from);
                        break;
                    }
                case 5:
                    {
                        from.PlaySound(0x248);
                        m_plant.ItemID = 18813;
                        m_clippers.ConsumeUse(from);
                        break;
                    }
                case 6:
                    {
                        from.PlaySound(0x248);
                        m_plant.ItemID = 18814;
                        m_clippers.ConsumeUse(from);
                        break;
                    }
                case 7:
                    {
                        from.PlaySound(0x248);
                        m_plant.ItemID = 18814;
                        m_clippers.ConsumeUse(from);
                        break;
                    }
                case 8:
                    {
                        from.PlaySound(0x248);
                        m_plant.ItemID = 18713;
                        m_clippers.ConsumeUse(from);
                        break;
                    }
                case 9:
                    {
                        from.PlaySound(0x248);
                        m_plant.ItemID = 19369;
                        m_clippers.ConsumeUse(from);
                        break;
                    }
            }
        }
    }
}