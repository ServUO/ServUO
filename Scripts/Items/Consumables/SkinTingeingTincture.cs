using Server.Gumps;
using Server.Mobiles;

namespace Server.Items
{
    public class SkinTingeingTincture : Item
    {
        public override int LabelNumber => 1114770;  //Skin Tingeing Tincture

        [Constructable]
        public SkinTingeingTincture()
            : base(0xEFF)
        {
            Hue = 90;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1114771); // Apply Directly to Forehead
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (IsChildOf(m.Backpack) && m is PlayerMobile)
            {
                if (!m.HasGump(typeof(InternalGump)))
                {
                    BaseGump.SendGump(new InternalGump((PlayerMobile)m, this));
                }
            }
        }

        public SkinTingeingTincture(Serial serial)
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
        }

        private class InternalGump : BaseGump
        {
            public override int GetTypeID()
            {
                return 0xF3EA1;
            }

            public SkinTingeingTincture Item { get; set; }
            public int SelectedHue { get; set; }

            public InternalGump(PlayerMobile pm, SkinTingeingTincture item)
                : base(pm, 50, 50)
            {
                Item = item;
            }

            public override void AddGumpLayout()
            {
                AddBackground(0, 0, 460, 300, 2620);

                int[] list = GetHueList();

                int rows = User.Race == Race.Human ? 8 : 6;
                int start = User.Race == Race.Human ? 40 : 80;
                bool elf = User.Race == Race.Elf;

                int x = start;
                int y = start;
                int displayHue;

                for (int i = 0; i < list.Length; i++)
                {
                    if (i > 0 && i % rows == 0)
                    {
                        x = start;
                        y += 22;
                    }

                    displayHue = elf ? list[i] - 1 : list[i];

                    AddImage(x, y, 210, displayHue);
                    AddButton(x, y, 212, 212, i + 100, GumpButtonType.Reply, 0);

                    x += 21;
                }

                displayHue = SelectedHue != 0 ? SelectedHue : User.Hue ^ 0x8000;

                if (elf)
                    displayHue--;

                AddImage(240, 0, GetPaperdollImage(), displayHue);

                AddButton(250, 260, 239, 238, 1, GumpButtonType.Reply, 0);
                AddButton(50, 260, 242, 241, 0, GumpButtonType.Reply, 0);
            }

            public override void OnResponse(RelayInfo info)
            {
                int button = info.ButtonID;

                if (button >= 100)
                {
                    button -= 100;

                    int[] list = GetHueList();

                    if (button >= 0 && button < list.Length)
                    {
                        SelectedHue = list[button];
                        Refresh(true, false);
                    }
                }
                else if (button == 1 && Item != null)
                {
                    if (SelectedHue != 0)
                    {
                        User.Hue = User.Race.ClipSkinHue(SelectedHue & 0x3FFF) | 0x8000;
                        Item.Delete();
                    }
                }
            }

            private int GetPaperdollImage()
            {
                if (User.Race == Race.Human)
                {
                    return User.Female ? 13 : 12;
                }

                if (User.Race == Race.Elf)
                {
                    return User.Female ? 15 : 14;
                }

                if (User.Race == Race.Gargoyle)
                {
                    return User.Female ? 665 : 666;
                }

                return 0;
            }

            private int[] GetHueList()
            {
                if (User.Race == Race.Human)
                {
                    return HumanSkinHues;
                }

                if (User.Race == Race.Elf)
                {
                    return ElfSkinHues;
                }

                if (User.Race == Race.Gargoyle)
                {
                    return GargoyleSkinHues;
                }

                return new int[0];
            }

            private static int[] _HumanSkinHues;
            private static int[] _ElfSkinHues;
            private static int[] _GargoyleSkinHues;

            public static int[] HumanSkinHues
            {
                get
                {
                    if (_HumanSkinHues == null)
                    {
                        _HumanSkinHues = new int[57];

                        for (int i = 0; i < _HumanSkinHues.Length; i++)
                        {
                            _HumanSkinHues[i] = i + 1001;
                        }
                    }

                    return _HumanSkinHues;
                }
            }

            public static int[] ElfSkinHues
            {
                get
                {
                    if (_ElfSkinHues == null)
                    {
                        _ElfSkinHues = new int[]
                        {
                            0x4DE, 0x76C, 0x835, 0x430, 0x24D, 0x24E, 0x24F, 0x0BF,
                            0x4A7, 0x361, 0x375, 0x367, 0x3E8, 0x3DE, 0x353, 0x903,
                            0x76D, 0x384, 0x579, 0x3E9, 0x374, 0x389, 0x385, 0x376,
                            0x53F, 0x381, 0x382, 0x383, 0x76B, 0x3E5, 0x51D, 0x3E6
                        };
                    }

                    return _ElfSkinHues;
                }
            }

            public static int[] GargoyleSkinHues
            {
                get
                {
                    if (_GargoyleSkinHues == null)
                    {
                        _GargoyleSkinHues = new int[25];

                        for (int i = 0; i < _GargoyleSkinHues.Length; i++)
                        {
                            _GargoyleSkinHues[i] = i + 1754;
                        }
                    }

                    return _GargoyleSkinHues;
                }
            }
        }
    }
}