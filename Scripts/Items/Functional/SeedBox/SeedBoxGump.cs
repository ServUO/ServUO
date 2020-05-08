using Server.Gumps;
using Server.Mobiles;
using Server.Prompts;
using Server.Targeting;
using System;

namespace Server.Engines.Plants
{
    public class SeedBoxGump : BaseGump
    {
        public SeedBox Box { get; set; }
        public int Page { get; set; }

        public int Pages => (int)Math.Ceiling(Box.Entries.Count / 20.0);

        public SeedBoxGump(PlayerMobile user, SeedBox box, int page = 1)
            : base(user, 150, 200)
        {
            Box = box;
            Page = page;

            user.CloseGump(GetType());
        }

        public override void AddGumpLayout()
        {
            AddPage(0);

            AddImage(30, 30, 2172);

            AddPage(1);

            int start = (Page - 1) * 20;
            int index = 0;            

            for (int i = start; i < Box.Entries.Count && i < start + 20; i++)
            {
                SeedEntry entry = Box.Entries[i];

                if (entry == null || entry.Seed == null)
                {
                    index++;
                    continue;
                }

                int x; int y;

                if (index < 4)
                {
                    x = 46 + (index * 70);
                    y = 41;
                }
                else if (index < 8)
                {
                    x = 46 + ((index - 4) * 70);
                    y = 106;
                }
                else if (index < 12)
                {
                    x = 46 + ((index - 8) * 70);
                    y = 171;
                }
                else if (index < 16)
                {
                    x = 46 + ((index - 12) * 70);
                    y = 236;
                }
                else
                {
                    x = 46 + ((index - 16) * 70);
                    y = 301;
                }

                AddImageTiledButton(x, y, entry.Image, entry.Image, i + 100, GumpButtonType.Reply, 0, 0xDCF, entry.Seed.Hue, 3, 30);
                AddTooltip(entry.Seed.GetLabel(out string args), args);

                index++;
            }

            if (Pages > 1)
            {
                AddHtmlLocalized(136, 373, 100, 25, 1151850, string.Format("@{0}@{1}", Page.ToString(), Pages.ToString()), 0x6F7B, false, false);

                if (Page > 1)
                {
                    AddButton(66, 375, 5603, 5603, 1, GumpButtonType.Reply, 0);
                }

                if (Page < Pages)
                {
                    AddButton(276, 375, 5601, 5601, 2, GumpButtonType.Reply, 0);
                }
            }
        }

        public void CheckPage(SeedEntry entry)
        {
            int index = Box.Entries.IndexOf(entry);

            Page = (int)Math.Ceiling((double)(index + 1) / 20);
        }

        public override void OnResponse(RelayInfo info)
        {
            if (Box.Deleted)
                return;

            switch (info.ButtonID)
            {
                case 0: break;
                case 1: // page back
                    Page--;

                    if (Page < 1)
                        Page = 1;

                    Refresh();
                    break;
                case 2: // page forward
                    Page++;

                    if (Page > Pages)
                        Page = Pages;

                    Refresh();
                    break;
                default:
                    int id = info.ButtonID - 100;

                    if (id >= 0 && id < Box.Entries.Count)
                    {
                        SeedEntry entry = Box.Entries[id];

                        if (entry == null)
                            return;

                        Refresh();
                        SendGump(new SeedInfoGump(User, Box, entry, this));
                    }
                    break;
            }
        }
    }

    public class SeedInfoGump : BaseGump
    {
        public SeedBox Box { get; set; }
        public SeedEntry Entry { get; set; }

        public static int TextHue = 0x1CE7;

        public SeedInfoGump(PlayerMobile user, SeedBox box, SeedEntry entry, SeedBoxGump par)
            : base(user, parent: par)
        {
            Box = box;
            Entry = entry;

            user.CloseGump(GetType());
        }

        public override void AddGumpLayout()
        {
            if (Entry == null || Entry.Seed == null)
            {
                User.CloseGump(GetType());
                return;
            }

            AddBackground(0, 0, 280, 225, 5170);

            int seedloc = Entry.Seed.GetLabel(out string args);
            int index = Box.Entries.IndexOf(Entry);

            int hue = Entry.Seed.Hue;

            AddHtmlLocalized(25, 25, 200, 20, seedloc, args, TextHue, false, false); // ~1_COLOR~ ~2_TYPE~ seed

            AddPage(1);

            AddImageTiledButton(28, 54, 10740, 10740, 1, GumpButtonType.Reply, 0, 0xDCF, hue, -20, 2);
            AddHtmlLocalized(53, 52, 100, 20, 1151840, TextHue, false, false); // Remove: 1

            AddImageTiledButton(128, 54, 10740, 10740, 2, GumpButtonType.Reply, 0, 0xDCF, hue, -20, 2);
            AddHtmlLocalized(153, 52, 100, 20, 1158411, TextHue, false, false); // Remove Quantity

            AddImageTiledButton(28, 79, 10740, 10740, 3, GumpButtonType.Reply, 0, 0xDCF, hue, -20, 2);
            AddHtmlLocalized(53, 77, 100, 20, 1151841, Entry.Seed.Amount.ToString(), TextHue, false, false); // Remove: ~1_val~
            
            if (index >= 0)
            {
                AddImageTiledButton(28, 104, 10740, 10740, 4, GumpButtonType.Reply, 0, 0xDCF, hue, -20, 2);                
            }

            AddHtmlLocalized(53, 102, 150, 20, 1151851, TextHue, false, false); // Insert Seed Before

            if (Box.Entries.Count < SeedBox.MaxUnique)
            {
                AddImageTiledButton(28, 129, 10740, 10740, 5, GumpButtonType.Reply, 0, 0xDCF, hue, -20, 2);                
            }

            AddHtmlLocalized(53, 127, 150, 20, 1151852, TextHue, false, false); // Insert Seed After

            if (index < Box.Entries.Count && Box.Entries.Count < SeedBox.MaxUnique)
            {
                AddImageTiledButton(28, 154, 10740, 10740, 6, GumpButtonType.Reply, 0, 0xDCF, hue, -20, 2);                
            }

            AddHtmlLocalized(53, 152, 100, 20, 1151842, TextHue, false, false); // Shift Right

            if (index > 0 && Box.Entries[index - 1] == null)
            {
                AddImageTiledButton(28, 179, 10740, 10740, 7, GumpButtonType.Reply, 0, 0xDCF, hue, -20, 2);                
            }

            AddHtmlLocalized(53, 177, 100, 20, 1151843, TextHue, false, false); // Shift Right
        }

        private class QuanitityRemovePrompt : Prompt
        {
            public override int MessageCliloc => 1158424; // How many seeds would you like to remove?
            private readonly SeedBox m_Box;
            private readonly SeedEntry m_Entry;

            public QuanitityRemovePrompt(SeedBox box, SeedEntry entry)
            {
                m_Box = box;
                m_Entry = entry;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (m_Box == null)
                {
                    return;
                }

                var amount = Utility.ToInt32(text);

                if (amount <= 0 || amount > m_Entry.Seed.Amount)
                {
                    from.SendLocalizedMessage(1158425); // You may not remove that quantity of seeds.
                }
                else
                {
                    m_Box.DropSeed(from, m_Entry, amount);

                    from.SendLocalizedMessage(1158426, amount.ToString()); // You remove ~1_quant~ seed(s) from the seedbox.

                    if (from is PlayerMobile pm)
                        SendGump(new SeedBoxGump(pm, m_Box));
                }
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            if (Box.Deleted)
                return;

            int index = Box.Entries.IndexOf(Entry);

            switch (info.ButtonID)
            {
                case 0: break;
                case 1:
                    Box.DropSeed(User, Entry, 1);

                    RefreshParent();
                    break;
                case 2:
                    User.Prompt = new QuanitityRemovePrompt(Box, Entry);
                    break;
                case 3:
                    Box.DropSeed(User, Entry, Entry.Seed.Amount);

                    RefreshParent();
                    break;
                case 4:
                    User.SendLocalizedMessage(1151849); // Click this button and target a seed to add it here.
                    User.BeginTarget(-1, false, TargetFlags.None, (from, targeted) =>
                        {
                            Seed seed = targeted as Seed;

                            if (seed != null)
                            {
                                if (Box != null && !Box.Deleted && index >= 0)
                                {
                                    Box.TryAddSeed(User, seed, index);
                                }
                            }
                            else
                                from.SendLocalizedMessage(1151838); // This item cannot be stored in the seed box.

                            RefreshParent();
                        });
                    break;
                case 5:
                    if (Box.Entries.Count < SeedBox.MaxUnique)
                    {
                        User.SendLocalizedMessage(1151849); // Click this button and target a seed to add it here.
                        User.BeginTarget(-1, false, TargetFlags.None, (from, targeted) =>
                            {
                                Seed seed = targeted as Seed;

                                if (seed != null)
                                {
                                    if (Box != null && !Box.Deleted && index >= 0)
                                    {
                                        Box.TryAddSeed(User, seed, index + 1);
                                    }
                                }
                                else
                                    from.SendLocalizedMessage(1151838); // This item cannot be stored in the seed box.

                                RefreshParent();
                            });
                    }
                    break;
                case 6:  // shift right
                    if (index >= 0 && index < Box.Entries.Count && Box.Entries.Count < SeedBox.MaxUnique)
                    {
                        Box.Entries.Insert(index, null);

                        if (Parent is SeedBoxGump)
                            ((SeedBoxGump)Parent).CheckPage(Entry);

                        RefreshParent(false);
                    }
                    break;
                case 7: // shift left
                    if (index >= 0 && index < Box.Entries.Count && Box.Entries[index - 1] == null)
                    {
                        Box.Entries.RemoveAt(index - 1);

                        Box.TrimEntries();

                        if (Parent is SeedBoxGump)
                            ((SeedBoxGump)Parent).CheckPage(Entry);

                        RefreshParent(false);
                    }
                    break;
            }
        }
    }
}
