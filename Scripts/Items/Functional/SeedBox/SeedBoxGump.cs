using System;
using Server;
using Server.Targeting;
using Server.Engines.Plants;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Engines.Plants
{
    public class SeedBoxGump : BaseGump
    {
        public SeedBox Box { get; set; }
        public int Page { get; set; }

        public int Pages { get { return (int)Math.Ceiling((double)Box.Entries.Count / 20.0); } }

        public SeedBoxGump(PlayerMobile user, SeedBox box, int page = 1) : base(user, 100, 100)
        {
            Box = box;
            Page = page;

            user.CloseGump(this.GetType());
        }

        public override void AddGumpLayout()
        {
            AddImage(0, 0, 2172);

            int start = (Page - 1) * 20;
            int index = 0;

            AddHtmlLocalized(100, 345, 300, 20, 1151850, String.Format("{0}\t{1}", Page.ToString(), Pages.ToString()), 0xFFFF, false, false);

            if (Page > 1)
            {
                AddButton(45, 345, 5603, 5603, 1, GumpButtonType.Reply, 0);
            }

            if (Page < Pages)
            {
                AddButton(235, 345, 5601, 5601, 2, GumpButtonType.Reply, 0);
            }

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
                    x = 15 + (index * 70);
                    y = 15;
                }
                else if (index < 8)
                {
                    x = 15 + ((index - 4) * 70);
                    y = 82;
                }
                else if (index < 12)
                {
                    x = 15 + ((index - 8) * 70);
                    y = 149;
                }
                else if (index < 16)
                {
                    x = 15 + ((index - 12) * 70);
                    y = 216;
                }
                else
                {
                    x = 15 + ((index - 16) * 70);
                    y = 283;
                }

                AddButton(x, y, entry.Image, entry.Image, i + 100, GumpButtonType.Reply, 0);
                AddItem(x, y + 30, 0xDCF, entry.Seed.Hue);

                AddItemProperty(entry.Seed.Serial);

                index++;
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
                        BaseGump.SendGump(new SeedInfoGump(User, Box, entry, this));
                    }
                    break;
            }
        }
    }

    public class SeedInfoGump : BaseGump
    {
        public SeedBox Box { get; set; }
        public SeedEntry Entry { get; set; }

        public static int TextHue = 0x696969;

        public SeedInfoGump(PlayerMobile user, SeedBox box, SeedEntry entry, SeedBoxGump par) : base(user, parent: par)
        {
            Box = box;
            Entry = entry;

            user.CloseGump(this.GetType());
        }

        public override void AddGumpLayout()
        {
            if (Entry == null || Entry.Seed == null)
            {
                User.CloseGump(this.GetType());
                return;
            }

            AddBackground(0, 0, 300, 280, 5170);

            string args;
            int seedloc = Entry.Seed.GetLabel(out args);
            int index = Box.Entries.IndexOf(Entry);

            AddHtmlLocalized(30, 25, 270, 20, seedloc, args, C32216(TextHue), false, false);

            AddHtmlLocalized(50, 60, 150, 20, 1151840, C32216(TextHue), false, false); // Remove: 1
            AddButton(30, 60, 10740, 10740, 1, GumpButtonType.Reply, 0);

            AddHtmlLocalized(50, 90, 150, 20, 1151841, Entry.Seed.Amount.ToString(), C32216(TextHue), false, false); // Remove: ~1_val~
            AddButton(30, 90, 10740, 10740, 2, GumpButtonType.Reply, 0);

            if (index > 0)
            {
                AddHtmlLocalized(50, 120, 150, 20, 1151851, C32216(TextHue), false, false); // Insert Seed Before
                AddButton(30, 120, 10740, 10740, 3, GumpButtonType.Reply, 0);
            }
            else
                AddImage(30, 120, 10740);

            if (Box.Entries.Count < SeedBox.MaxUnique)
            {
                AddHtmlLocalized(50, 150, 150, 20, 1151852, C32216(TextHue), false, false); // Insert Seed After
                AddButton(30, 150, 10740, 10740, 4, GumpButtonType.Reply, 0);
            }
            else
                AddImage(30, 150, 10740);

            if (index < Box.Entries.Count && Box.Entries.Count < SeedBox.MaxUnique)
            {
                AddHtmlLocalized(50, 180, 150, 20, 1151842, C32216(TextHue), false, false); // Shift Right
                AddButton(30, 180, 10740, 10740, 5, GumpButtonType.Reply, 0);
            }
            else
                AddImage(30, 180, 10740);

            if (index > 0 && Box.Entries[index - 1] == null)
            {
                AddHtmlLocalized(50, 210, 150, 20, 1151843, C32216(TextHue), false, false); // Shift Left
                AddButton(30, 210, 10740, 10740, 6, GumpButtonType.Reply, 0);
            }
            else
                AddImage(30, 210, 10740);
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
                    Box.DropSeed(User, Entry, Entry.Seed.Amount);

                    RefreshParent();
                    break;
                case 3:
                    User.SendLocalizedMessage(1151849); // Click this button and target a seed to add it here.
                    User.BeginTarget(-1, false, TargetFlags.None, (from, targeted) =>
                        {
                            Seed seed = targeted as Seed;

                            if (seed != null)
                            {
                                if (Box != null && !Box.Deleted && index > 0)
                                {
                                    Box.TryAddSeed(User, seed, index);
                                }
                            }
                            else
                                from.SendLocalizedMessage(1151838); // This item cannot be stored in the seed box.

                            RefreshParent();
                        });
                    break;
                case 4:
                    if (Box.Entries.Count < SeedBox.MaxUnique)
                    {
                        User.SendLocalizedMessage(1151849); // Click this button and target a seed to add it here.
                        User.BeginTarget(-1, false, TargetFlags.None, (from, targeted) =>
                            {
                                Seed seed = targeted as Seed;

                                if (seed != null)
                                {
                                    if (Box != null && !Box.Deleted && index > 0)
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
                case 5:  // shift right
                    if (index >= 0 && index < Box.Entries.Count && Box.Entries.Count < SeedBox.MaxUnique)
                    {
                        Box.Entries.Insert(index, null);

                        if (index + 2 < Box.Entries.Count && Box.Entries[index + 2] == null)
                            Box.Entries.RemoveAt(index + 2);

                        if (Parent is SeedBoxGump)
                            ((SeedBoxGump)Parent).CheckPage(Entry);

                        RefreshParent(true);
                    }
                    break;
                case 6: // shift left
                    if (index >= 0 && index < Box.Entries.Count && Box.Entries[index - 1] == null)
                    {
                        Box.Entries.Remove(Entry);
                        Box.Entries.Insert(index - 1, Entry);
                        Box.TrimEntries();
                        if(Parent is SeedBoxGump)
                            ((SeedBoxGump)Parent).CheckPage(Entry);
                        RefreshParent(true);
                    }
                    break;
            }
        }
    }
}