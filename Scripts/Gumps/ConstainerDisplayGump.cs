using Server.Mobiles;
using Server.Items;
using Server.Network;
using System.Collections.Generic;

namespace Server.Gumps
{
    public class ContainerDisplayGump : BaseGump
    {
        public int Page { get; private set; } = 0;

        public TextDefinition Title { get; }
        public Container Container { get; }
        public List<Item> Contents { get; }
        public bool CanTake { get; set; }

        public int Pages => Contents.Count / 50 + 1;

        public ContainerDisplayGump(PlayerMobile pm, Container c, TextDefinition title, bool canTake = false)
            : base(pm, 120, 50)
        {
            Container = c;
            Title = title;
            CanTake = canTake;
            Contents = AllContents();
        }

        public override void AddGumpLayout()
        {
            AddImage(0, 0, 0x9CDF);

            if (Title != null)
            {
                if (Title.Number > 0)
                {
                    AddHtmlLocalized(0, 10, 600, 18, CenterLoc, string.Format("#{0}", Title.Number), 0x6B45, false, false);
                }
                else if (!string.IsNullOrEmpty(Title.String))
                {
                    AddHtml(0, 10, 600, 18, ColorAndCenter(C16232(0x6B45), Title.String), false, false);
                }
            }

            int start = Page * 50;
            var pageIndex = 0;

            int x = 50;
            int y = 60;

            for (int i = start; i < start + 50 && i < Contents.Count; i++)
            {
                var item = Contents[i];

                if (CanTake)
                {
                    AddButton(x, y, 0x931, 0x931, i + 1000, GumpButtonType.Reply, 0);
                }
                else
                {
                    AddImage(x, y, 0x931);
                }

                Rectangle2D b = ItemBounds.Table[item.ItemID];
                AddItem((x + 25) - b.Width / 2 - b.X, (y + 25) - b.Height / 2 - b.Y, item.ItemID, item.Hue);
                AddItemProperty(item);

                pageIndex++;

                if (pageIndex > 0 && pageIndex % 5 == 0)
                {
                    x += 50;
                    y = 60;
                }
                else
                {
                    y += 50;
                }
            }

            if (Pages > 1)
            {
                AddHtmlLocalized(263, 346, 100, 18, 1153561, string.Format("{0}\t{1}", Page + 1, Pages), 0x6B45, false, false); // Page ~1_CUR~ of ~2_MAX~
            }
            else
            {
                AddHtmlLocalized(0, 346, 600, 18, 1153562, 0x6B45, false, false); // Page
            }

            AddButton(200, 346, 4014, 4016, 1, GumpButtonType.Reply, 0);
            AddButton(370, 346, 4005, 4007, 2, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case 0: break;
                case 1:
                    if (Page > 0)
                    {
                        Page--;
                    }
                    Refresh();
                    break;
                case 2:
                    if (Page < Pages - 1)
                    {
                        Page++;
                    }
                    Refresh();
                    break;
                default:
                    if (CanTake)
                    {
                        var index = info.ButtonID - 1000;

                        if (index >= 0 && index < Contents.Count)
                        {
                            var item = Contents[index];

                            bool rejected;
                            LRReason reject;

                            // TODO: We will need a server side gump to break apart amount for stacked items
                            User.Lift(item, item.Amount, out rejected, out reject);
                        }
                    }
                    Refresh();
                    break;
            }
        }

        public List<Item> AllContents()
        {
            var list = new List<Item>();
            AllContents(list, Container);

            return list;
        }

        public void AllContents(List<Item> list, Container c)
        {
            for (int i = 0; i < c.Items.Count; i++)
            {
                var item = c.Items[i];

                if (!list.Contains(item))
                {
                    list.Add(item);
                }

                var cont = item as Container;

                if (cont != null)
                {
                    AllContents(list, cont);
                }
            }
        }
    }
}
