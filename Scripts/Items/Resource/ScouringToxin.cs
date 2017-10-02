using System;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Items
{
    public class ScouringToxin : Item, IUsesRemaining
    {
        public override int LabelNumber { get { return 1112292; } } // scouring toxin

        private int m_UsesRemaining;

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining { get { return m_UsesRemaining; } set { m_UsesRemaining = value; if (m_UsesRemaining <= 0) Delete(); else InvalidateProperties(); } }

        public bool ShowUsesRemaining { get { return false; } set { { } } }

        [Constructable]
        public ScouringToxin()
            : this(10)
        {
        }

        [Constructable]
        public ScouringToxin(int amount)
            : base(0x1848)
        {
            m_UsesRemaining = amount;
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (m_UsesRemaining <= 1)
                list.Add(LabelNumber);
            else
                list.Add(1050039, "{0}\t#{1}", m_UsesRemaining, LabelNumber); // ~1_NUMBER~ ~2_ITEMNAME~
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.SendMessage("What would you like to scour?");
                from.BeginTarget(-1, false, Server.Targeting.TargetFlags.None, new TargetCallback(OnTarget));
            }
        }

        public void OnTarget(Mobile from, object targeted)
        {
            if (!(from is PlayerMobile) || !((PlayerMobile)from).BasketWeaving)
                from.SendLocalizedMessage(1112253); //You haven't learned basket weaving. Perhaps studying a book would help!
            else if (targeted is DryReeds)
            {
                DryReeds reed1 = (DryReeds)targeted;
                Container cont = from.Backpack;

                Server.Engines.Plants.PlantHue hue = reed1.PlantHue;

                if (!reed1.IsChildOf(from.Backpack))
                    from.SendLocalizedMessage(1116249); //That must be in your backpack for you to use it.
                else if (cont != null)
                {
                    Item[] items = cont.FindItemsByType(typeof(DryReeds));
                    List<Item> list = new List<Item>();
                    int total = 0;

                    foreach (Item item in items)
                    {
                        if (item is DryReeds)
                        {
                            DryReeds check = (DryReeds)item;

                            if (reed1.PlantHue == check.PlantHue)
                            {
                                total += item.Amount;
                                list.Add(item);
                            }
                        }
                    }

                    int toConsume = 2;

                    if (list.Count > 0 && total > 1)
                    {
                        foreach (Item item in list)
                        {
                            if (item.Amount >= toConsume)
                            {
                                item.Consume(toConsume);
                                toConsume = 0;
                            }
                            else if (item.Amount < toConsume)
                            {
                                item.Delete();
                                toConsume -= item.Amount;
                            }

                            if (toConsume <= 0)
                                break;
                        }

                        SoftenedReeds sReed = new SoftenedReeds(hue);

                        if (!from.Backpack.TryDropItem(from, sReed, false))
                            sReed.MoveToWorld(from.Location, from.Map);

                        m_UsesRemaining--;

                        if (m_UsesRemaining <= 0)
                            Delete();
                        else
                            InvalidateProperties();

                        from.PlaySound(0x23E);
                    }
                    else
                        from.SendLocalizedMessage(1112250); //You don't have enough of this type of dry reeds to make that.
                }

                return;
            }

            if (targeted is Item && BasePigmentsOfTokuno.IsValidItem((Item)targeted))
            {
                from.PlaySound(0x23E);
                from.SendMessage("You scour any color from the item.");

                ((Item)targeted).Hue = 0;
                this.Consume();
            }
            else
                from.SendMessage("You cannot scour that!");
        }

        private static bool IsInTypeList(Type t, Type[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i] == t) return true;
            }

            return false;
        }

        public ScouringToxin(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)2); // version
            writer.Write(m_UsesRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version > 0)
                m_UsesRemaining = reader.ReadInt();

            if (version == 1)
                ItemID = 0x1848;
        }
    }
}