using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class HABPromotionalToken : Item, IAccountRestricted
    {
        public override int LabelNumber => 1070997;  // A promotional token

        [CommandProperty(AccessLevel.GameMaster)]
        public string Account { get; set; }

        [Constructable]
        public HABPromotionalToken()
           : this(null)
        { }

        [Constructable]
        public HABPromotionalToken(string account)
            : base(0x2AAA)
        {
            LootType = LootType.Blessed;
            Light = LightType.Circle300;
            Weight = 5.0;

            Account = account;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1070998, string.Format("#{0}", 1158657)); // Use this to redeem<br>your ~1_PROMO~
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
            }
            else if (Account == null)
            {
                from.SendLocalizedMessage(1158650); // You cannot redeem this promotional hairstyle change token right now.
            }
            else if (from.Account.Username != Account)
            {
                from.SendLocalizedMessage(1116257); // This token does not belong to this character.
            }
            else
            {
                from.CloseGump(typeof(PromotionalTokenGump));
                from.SendGump(new PromotionalTokenGump(this));
            }
        }

        public HABPromotionalToken(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(Account);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Account = reader.ReadString();
        }

        public enum StyleType
        {
            Hair = 0,
            Beard = 1,
        }

        public class ChangeHairstyleEntry
        {
            public int GumpID { get; set; }
            public int Tooltip { get; set; }
            public StyleType Type { get; set; }
            public int ItemID { get; set; }

            public ChangeHairstyleEntry(int gid, int tooltip, StyleType type, int itemid)
            {
                GumpID = gid;
                Tooltip = tooltip;
                Type = type;
                ItemID = itemid;
            }
        }

        public static readonly ChangeHairstyleEntry[] HumanMaleEntries = new[]
            {
                /* Hair */
                new ChangeHairstyleEntry(0xC8F7, 1125404, StyleType.Hair, 0xA1A4),
                new ChangeHairstyleEntry(0xC8F8, 1125405, StyleType.Hair, 0xA1A5),
                new ChangeHairstyleEntry(0xC8F9, 1125406, StyleType.Hair, 0xA1A6),
                new ChangeHairstyleEntry(0xC8FA, 1125407, StyleType.Hair, 0xA1A7),
                /* Beard */
                new ChangeHairstyleEntry(0xC8FB, 1125408, StyleType.Beard, 0xA1A8),
                new ChangeHairstyleEntry(0xC8FC, 1125409, StyleType.Beard, 0xA1A9),
                new ChangeHairstyleEntry(0xC8FD, 1125410, StyleType.Beard, 0xA1AA),
                new ChangeHairstyleEntry(0xC8FE, 1125411, StyleType.Beard, 0xA1AB)
            };

        public static readonly ChangeHairstyleEntry[] HumanFemaleEntries = new[]
        {
                /* Hair */
                new ChangeHairstyleEntry(0xF00F, 1125412, StyleType.Hair, 0xA1AC),
                new ChangeHairstyleEntry(0xF010, 1125413, StyleType.Hair, 0xA1AD),
                new ChangeHairstyleEntry(0xF011, 1125414, StyleType.Hair, 0xA1AE),
                new ChangeHairstyleEntry(0xF012, 1125415, StyleType.Hair, 0xA1AF)
            };

        public static readonly ChangeHairstyleEntry[] ElfMaleEntries = new[]
        {
                /* Hair */
                new ChangeHairstyleEntry(0xC903, 1125416, StyleType.Hair, 0xA1B0),
                new ChangeHairstyleEntry(0xC904, 1125417, StyleType.Hair, 0xA1B1),
                new ChangeHairstyleEntry(0xC905, 1125418, StyleType.Hair, 0xA1B2),
                new ChangeHairstyleEntry(0xC906, 1125419, StyleType.Hair, 0xA1B3)
            };

        public static readonly ChangeHairstyleEntry[] ElfFemaleEntries = new[]
        {
                /* Hair */
                new ChangeHairstyleEntry(0xF017, 1125420, StyleType.Hair, 0xA1B4),
                new ChangeHairstyleEntry(0xF018, 1125421, StyleType.Hair, 0xA1B5),
                new ChangeHairstyleEntry(0xF019, 1125422, StyleType.Hair, 0xA1B6),
                new ChangeHairstyleEntry(0xF01A, 1125423, StyleType.Hair, 0xA1B7)
            };

        public static readonly ChangeHairstyleEntry[] GargoyleMaleEntries = new[]
        {
                /* Hair */
                new ChangeHairstyleEntry(0xC90B, 1125424, StyleType.Hair, 0xA1B8),
                new ChangeHairstyleEntry(0xC90C, 1125425, StyleType.Hair, 0xA1B9),
                new ChangeHairstyleEntry(0xC90D, 1125426, StyleType.Hair, 0xA1BA),
                new ChangeHairstyleEntry(0xC90E, 1125427, StyleType.Hair, 0xA1BB),
                /* Beard */
                new ChangeHairstyleEntry(0xC90F, 1125428, StyleType.Beard, 0xA1BC),
                new ChangeHairstyleEntry(0xC910, 1125429, StyleType.Beard, 0xA1BD),
                new ChangeHairstyleEntry(0xC911, 1125430, StyleType.Beard, 0xA1BE),
                new ChangeHairstyleEntry(0xC912, 1125431, StyleType.Beard, 0xA1BF)
            };

        public static readonly ChangeHairstyleEntry[] GargoyleFemaleEntries = new[]
        {
                /* Hair */
                new ChangeHairstyleEntry(0xF023, 1125432, StyleType.Hair, 0xA1C0),
                new ChangeHairstyleEntry(0xF024, 1125433, StyleType.Hair, 0xA1C1),
                new ChangeHairstyleEntry(0xF025, 1125434, StyleType.Hair, 0xA1C2),
                new ChangeHairstyleEntry(0xF026, 1125435, StyleType.Hair, 0xA1C3)
        };

        private class PromotionalTokenGump : Gump
        {
            private readonly Item Token;

            public PromotionalTokenGump(Item token)
                : base(170, 150)
            {
                Token = token;

                AddPage(0);

                AddBackground(0, 0, 291, 99, 0x13BE);
                AddImageTiled(5, 6, 280, 20, 0xA40);
                AddHtmlLocalized(9, 8, 280, 20, 1158655, 0x7FFF, false, false); // Change your character's hairstyle.
                AddImageTiled(5, 31, 280, 40, 0xA40);
                AddHtmlLocalized(9, 35, 272, 40, 1158651, 0x7FFF, false, false); // Are you absolutely sure you wish to change your character's hairstyle?
                AddButton(125, 73, 0xFB7, 0xFB8, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(160, 75, 155, 20, 1158654, 0x7FFF, false, false); // Make me fabulous! 
                AddButton(5, 73, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
                AddHtmlLocalized(40, 75, 100, 20, 1060051, 0x7FFF, false, false); // CANCEL
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (info.ButtonID != 1)
                    return;

                Mobile from = sender.Mobile;

                if (!Token.IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
                }
                else
                {
                    from.CloseGump(typeof(InternalGump));

                    ChangeHairstyleEntry[] entry = HumanMaleEntries;

                    if (from.Race == Race.Human)
                    {
                        if (from.Female)
                        {
                            entry = HumanFemaleEntries;
                        }
                        else
                        {
                            entry = HumanMaleEntries;
                        }
                    }
                    else if (from.Race == Race.Elf)
                    {
                        if (from.Female)
                        {
                            entry = ElfFemaleEntries;
                        }
                        else
                        {
                            entry = ElfMaleEntries;
                        }
                    }
                    else if (from.Race == Race.Gargoyle)
                    {
                        if (from.Female)
                        {
                            entry = GargoyleFemaleEntries;
                        }
                        else
                        {
                            entry = GargoyleMaleEntries;
                        }
                    }

                    from.SendGump(new InternalGump(from, Token, entry));
                }
            }
        }

        private class InternalGump : Gump
        {
            private readonly Item Token;
            private readonly ChangeHairstyleEntry[] _Entries;

            public InternalGump(Mobile from, Item token, ChangeHairstyleEntry[] entries)
                : base(50, 50)
            {
                Token = token;
                _Entries = entries;

                from.CloseGump(typeof(InternalGump));

                if (_Entries.Length > 4)
                {
                    AddBackground(0, 0, 560, 400, 0xA28);

                    AddButton(117, 345, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(149, 345, 90, 35, 1006044, 0x0, false, false); // OK
                    AddButton(324, 345, 0xFA5, 0xFA7, 0, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(356, 345, 90, 35, 1006045, 0x0, false, false); // Cancel

                    AddHtmlLocalized(50, 25, 460, 20, 1018353, 0x0, false, false); // <center>New Hairstyle</center>
                }
                else
                {
                    AddBackground(0, 0, 300, 250, 0xA28);

                    AddButton(69, 207, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(101, 207, 90, 35, 1006044, 0x0, false, false); // OK
                    AddButton(191, 207, 0xFA5, 0xFA7, 0, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(223, 207, 90, 35, 1006045, 0x0, false, false); // Cancel

                    AddHtmlLocalized(50, 25, 230, 20, 1018353, 0x0, false, false); // <center>New Hairstyle</center>
                }

                for (int i = 0; i < _Entries.Length; ++i)
                {
                    if (i > 3)
                    {
                        AddBackground(70 + (124 * (i - 4)), 135, 60, 64, 0xA3C);
                        AddTooltip(_Entries[i].Tooltip);
                        AddImage(10 + (124 * (i - 4)), 95, _Entries[i].GumpID);
                        AddRadio(30 + (124 * (i - 4)), 155, 0xD0, 0xD1, false, 41400 + i);
                    }
                    else
                    {
                        if (i > 1 && _Entries.Length < 5)
                        {
                            AddBackground(70 + (124 * (i - 2)), 135, 60, 64, 0xA3C);
                            AddTooltip(_Entries[i].Tooltip);
                            AddImage(10 + (124 * (i - 2)), 95, _Entries[i].GumpID);
                            AddRadio(30 + (124 * (i - 2)), 155, 0xD0, 0xD1, false, 41400 + i);
                        }
                        else
                        {
                            AddBackground(70 + (124 * i), 60, 60, 64, 0xA3C);
                            AddTooltip(_Entries[i].Tooltip);
                            AddImage(10 + (124 * i), 20, _Entries[i].GumpID);
                            AddRadio(30 + (124 * i), 80, 0xD0, 0xD1, false, 41400 + i);
                        }
                    }
                }
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                Mobile from = sender.Mobile;

                if (!Token.IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
                    return;
                }

                if (info.ButtonID == 1)
                {
                    int[] switches = info.Switches;

                    if (switches.Length > 0)
                    {
                        int index = switches[0];

                        ChangeHairstyleEntry entry = _Entries[index - 41400];

                        if (entry.Type == StyleType.Hair)
                            from.HairItemID = entry.ItemID;
                        else
                            from.FacialHairItemID = entry.ItemID;

                        from.SendLocalizedMessage(1158661); // You have successfully changed your hairstyle.

                        Token.Delete();
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1013009); // You decide not to change your hairstyle.
                }
            }
        }
    }
}
