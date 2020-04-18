using System;
using System.Collections.Generic;

namespace Server.Items
{

    public class ExperimentalRoomChest : MetalBox
    {
        private Dictionary<Item, Mobile> m_Instancing;

        public override bool DisplayWeight => false;
        public override bool DisplaysContent => false;
        public override bool Decays => true;
        public override TimeSpan DecayTime => TimeSpan.FromMinutes(10.0);

        [Constructable]
        public ExperimentalRoomChest()
        {
            Movable = false;
            m_Instancing = new Dictionary<Item, Mobile>();
            LiftOverride = true;

        }

        public override void OnDoubleClick(Mobile from)
        {
            Container pack = from.Backpack;

            if (pack != null)
            {
                Item item = pack.FindItemByType(typeof(ExperimentalGem));

                if (item != null && item is ExperimentalGem && ((ExperimentalGem)item).Complete)
                {
                    item.Delete();

                    Item toDrop = GetRandomDrop();

                    if (toDrop != null)
                        AddItemFor(toDrop, from);
                }
            }

            base.OnDoubleClick(from);
        }

        public override bool TryDropItem(Mobile from, Item dropped, bool message)
        {
            if (dropped is ExperimentalGem && ((ExperimentalGem)dropped).Complete && from.InRange(Location, 2))
            {
                dropped.Delete();

                Item toDrop = GetRandomDrop();

                if (toDrop != null)
                    AddItemFor(toDrop, from);

                base.OnDoubleClick(from);
            }

            return false;
        }

        public void AddItemFor(Item item, Mobile mob)
        {
            if (item == null || mob == null)
                return;

            DropItem(item);
            item.SetLastMoved();

            if (m_Instancing == null)
                m_Instancing = new Dictionary<Item, Mobile>();

            m_Instancing[item] = mob;
        }

        public override bool IsChildVisibleTo(Mobile m, Item child)
        {
            if (m.AccessLevel > AccessLevel.Player)
                return true;

            if (m_Instancing != null)
            {
                if (!m_Instancing.ContainsKey(child))
                    return true;

                if (m_Instancing[child] == m)
                    return true;
            }
            else
            {
                return true;
            }

            return false;
        }

        public override bool OnDecay()
        {
            List<Item> items = new List<Item>(Items);

            foreach (Item i in items)
            {
                if (i.Decays && i.LastMoved.Add(DecayTime) < DateTime.UtcNow)
                {
                    i.Delete();

                    if (m_Instancing.ContainsKey(i))
                        m_Instancing.Remove(i);
                }
            }

            return false;
        }

        public override void RemoveItem(Item item)
        {
            if (m_Instancing != null && m_Instancing.ContainsKey(item))
                m_Instancing.Remove(item);

            base.RemoveItem(item);
        }

        public Item GetRandomDrop()
        {
            Item item = null;

            switch (Utility.Random(17))
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    item = new Stalagmite();
                    break;
                case 7:
                case 8:
                case 9:
                case 10:
                    item = new Flowstone();
                    break;
                case 11:
                    item = new CanvaslessEasel();
                    break;
                case 12:
                    item = new HangingChainmailLegs();
                    break;
                case 13:
                    item = new HangingRingmailTunic();
                    break;
                case 14:
                    item = new PluckedChicken();
                    break;
                case 15:
                    item = new ColorfulTapestry();
                    break;
                case 16:
                    item = new TwoStoryBanner();
                    break;
            }

            return item;
        }

        public ExperimentalRoomChest(Serial serial) : base(serial)
        {

        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // ver
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Instancing = new Dictionary<Item, Mobile>();
        }
    }
}