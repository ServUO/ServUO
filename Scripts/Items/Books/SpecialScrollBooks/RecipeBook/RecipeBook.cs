using Server.ContextMenus;
using Server.Gumps;
using Server.Mobiles;
using Server.Multis;
using Server.Prompts;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    public enum RecipeSkillName
    {
        Alchemy = 0,
        Blacksmith = 1,
        Carpentry = 2,
        Cartography = 3,
        Cooking = 4,
        Fletching = 5,
        Inscription = 6,
        Masonry = 7,
        Tailoring = 8,
        Tinkering = 9,
    }

    public class RecipeScrollDefinition
    {
        public int ID { get; set; }
        public int RecipeID { get; set; }
        public Expansion Expansion { get; set; }
        public RecipeSkillName Skill { get; set; }
        public int Amount { get; set; }
        public int Price { get; set; }

        public RecipeScrollDefinition(int id, int rid, Expansion exp, RecipeSkillName skill)
            : this(id, rid, exp, skill, 0, 0)
        {
        }

        public RecipeScrollDefinition(int id, int rid, Expansion exp, RecipeSkillName skill, int amount, int price)
        {
            ID = id;
            RecipeID = rid;
            Expansion = exp;
            Skill = skill;
            Amount = amount;
            Price = price;
        }
    }

    public class RecipeBook : Item, ISecurable
    {
        public override int LabelNumber => 1125598;  // recipe book

        [CommandProperty(AccessLevel.GameMaster)]
        public string BookName { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Using { get; set; }

        public List<RecipeScrollDefinition> Recipes { get; set; }

        public RecipeScrollFilter Filter { get; set; }

        public RecipeScrollDefinition[] Definitions = new RecipeScrollDefinition[]
        {
            new RecipeScrollDefinition(1, 501, Expansion.ML, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(2, 502, Expansion.ML, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(3, 503, Expansion.ML, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(4, 504, Expansion.ML, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(5, 505, Expansion.ML, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(6, 599, Expansion.ML, RecipeSkillName.Cooking),
            new RecipeScrollDefinition(7, 250, Expansion.ML, RecipeSkillName.Fletching),
            new RecipeScrollDefinition(8, 251, Expansion.ML, RecipeSkillName.Fletching),
            new RecipeScrollDefinition(9, 252, Expansion.ML, RecipeSkillName.Fletching),
            new RecipeScrollDefinition(10, 253, Expansion.ML, RecipeSkillName.Fletching),
            new RecipeScrollDefinition(11, 254, Expansion.ML, RecipeSkillName.Fletching),
            new RecipeScrollDefinition(12, 350, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(13, 351, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(14, 354, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(15, 150, Expansion.ML, RecipeSkillName.Carpentry),
            new RecipeScrollDefinition(16, 352, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(17, 353, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(18, 151, Expansion.ML, RecipeSkillName.Carpentry),
            new RecipeScrollDefinition(19, 152, Expansion.ML, RecipeSkillName.Carpentry),
            new RecipeScrollDefinition(20, 551, Expansion.ML, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(21, 550, Expansion.ML, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(22, 552, Expansion.ML, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(23, 452, Expansion.ML, RecipeSkillName.Tinkering),
            new RecipeScrollDefinition(24, 450, Expansion.ML, RecipeSkillName.Tinkering),
            new RecipeScrollDefinition(25, 451, Expansion.ML, RecipeSkillName.Tinkering),
            new RecipeScrollDefinition(26, 453, Expansion.ML, RecipeSkillName.Inscription),
            new RecipeScrollDefinition(27, 116, Expansion.ML, RecipeSkillName.Carpentry),
            new RecipeScrollDefinition(28, 106, Expansion.ML, RecipeSkillName.Carpentry),
            new RecipeScrollDefinition(29, 108, Expansion.ML, RecipeSkillName.Carpentry),
            new RecipeScrollDefinition(30, 109, Expansion.ML, RecipeSkillName.Carpentry),
            new RecipeScrollDefinition(31, 100, Expansion.ML, RecipeSkillName.Carpentry),
            new RecipeScrollDefinition(32, 102, Expansion.ML, RecipeSkillName.Carpentry),
            new RecipeScrollDefinition(33, 111, Expansion.ML, RecipeSkillName.Carpentry),
            new RecipeScrollDefinition(34, 118, Expansion.ML, RecipeSkillName.Masonry),
            new RecipeScrollDefinition(35, 200, Expansion.ML, RecipeSkillName.Fletching),
            new RecipeScrollDefinition(36, 201, Expansion.ML, RecipeSkillName.Fletching),
            new RecipeScrollDefinition(37, 202, Expansion.ML, RecipeSkillName.Fletching),
            new RecipeScrollDefinition(38, 203, Expansion.ML, RecipeSkillName.Fletching),
            new RecipeScrollDefinition(39, 204, Expansion.ML, RecipeSkillName.Fletching),
            new RecipeScrollDefinition(40, 205, Expansion.ML, RecipeSkillName.Fletching),
            new RecipeScrollDefinition(41, 206, Expansion.ML, RecipeSkillName.Fletching),
            new RecipeScrollDefinition(42, 207, Expansion.ML, RecipeSkillName.Fletching),
            new RecipeScrollDefinition(43, 300, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(44, 301, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(45, 302, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(46, 303, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(47, 304, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(48, 305, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(49, 306, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(50, 307, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(51, 308, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(52, 309, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(53, 310, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(54, 311, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(55, 312, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(56, 313, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(57, 314, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(58, 315, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(59, 332, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(60, 333, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(61, 334, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(62, 335, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(63, 316, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(64, 317, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(65, 318, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(66, 319, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(67, 320, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(68, 321, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(69, 322, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(70, 323, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(71, 324, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(72, 325, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(73, 326, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(74, 327, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(75, 328, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(76, 329, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(77, 330, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(78, 331, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(79, 112, Expansion.ML, RecipeSkillName.Carpentry),
            new RecipeScrollDefinition(80, 113, Expansion.ML, RecipeSkillName.Carpentry),
            new RecipeScrollDefinition(81, 114, Expansion.ML, RecipeSkillName.Carpentry),
            new RecipeScrollDefinition(82, 115, Expansion.ML, RecipeSkillName.Carpentry),
            new RecipeScrollDefinition(83, 400, Expansion.ML, RecipeSkillName.Alchemy),
            new RecipeScrollDefinition(84, 402, Expansion.ML, RecipeSkillName.Alchemy),
            new RecipeScrollDefinition(85, 401, Expansion.ML, RecipeSkillName.Alchemy),
            new RecipeScrollDefinition(86, 117, Expansion.ML, RecipeSkillName.Carpentry),
            new RecipeScrollDefinition(87, 107, Expansion.ML, RecipeSkillName.Carpentry),
            new RecipeScrollDefinition(88, 120, Expansion.ML, RecipeSkillName.Carpentry),
            new RecipeScrollDefinition(89, 110, Expansion.ML, RecipeSkillName.Carpentry),
            new RecipeScrollDefinition(90, 101, Expansion.ML, RecipeSkillName.Carpentry),
            new RecipeScrollDefinition(91, 103, Expansion.ML, RecipeSkillName.Carpentry),
            new RecipeScrollDefinition(92, 105, Expansion.ML, RecipeSkillName.Carpentry),
            new RecipeScrollDefinition(93, 119, Expansion.ML, RecipeSkillName.Masonry),
            new RecipeScrollDefinition(94, 104, Expansion.ML, RecipeSkillName.Carpentry),
            new RecipeScrollDefinition(95, 336, Expansion.ML, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(96, 500, Expansion.ML, RecipeSkillName.Cooking),
            new RecipeScrollDefinition(97, 604, Expansion.ML, RecipeSkillName.Cooking),
            new RecipeScrollDefinition(98, 603, Expansion.ML, RecipeSkillName.Cooking),
            new RecipeScrollDefinition(99, 702, Expansion.ML, RecipeSkillName.Masonry),
            new RecipeScrollDefinition(100, 701, Expansion.ML, RecipeSkillName.Masonry),
            new RecipeScrollDefinition(101, 560, Expansion.TOL, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(102, 561, Expansion.TOL, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(103, 562, Expansion.TOL, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(104, 563, Expansion.TOL, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(105, 564, Expansion.TOL, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(106, 565, Expansion.TOL, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(107, 566, Expansion.TOL, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(108, 570, Expansion.TOL, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(109, 575, Expansion.TOL, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(110, 573, Expansion.TOL, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(111, 574, Expansion.TOL, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(112, 576, Expansion.TOL, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(113, 577, Expansion.TOL, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(114, 572, Expansion.TOL, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(115, 571, Expansion.TOL, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(116, 581, Expansion.TOL, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(117, 584, Expansion.TOL, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(118, 583, Expansion.TOL, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(119, 582, Expansion.TOL, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(120, 580, Expansion.TOL, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(121, 600, Expansion.ML, RecipeSkillName.Cooking),
            new RecipeScrollDefinition(122, 601, Expansion.ML, RecipeSkillName.Cooking),
            new RecipeScrollDefinition(123, 602, Expansion.ML, RecipeSkillName.Cooking),
            new RecipeScrollDefinition(124, 800, Expansion.ML, RecipeSkillName.Inscription),
            new RecipeScrollDefinition(125, 900, Expansion.TOL, RecipeSkillName.Alchemy),
            new RecipeScrollDefinition(126, 901, Expansion.TOL, RecipeSkillName.Alchemy),
            new RecipeScrollDefinition(127, 902, Expansion.TOL, RecipeSkillName.Alchemy),
            new RecipeScrollDefinition(128, 903, Expansion.TOL, RecipeSkillName.Alchemy),
            new RecipeScrollDefinition(129, 904, Expansion.TOL, RecipeSkillName.Alchemy),
            new RecipeScrollDefinition(130, 905, Expansion.TOL, RecipeSkillName.Alchemy),
            new RecipeScrollDefinition(131, 1000, Expansion.TOL, RecipeSkillName.Cartography),
            new RecipeScrollDefinition(132, 455, Expansion.TOL, RecipeSkillName.Tinkering),
            new RecipeScrollDefinition(133, 461, Expansion.ML, RecipeSkillName.Tinkering),
            new RecipeScrollDefinition(134, 462, Expansion.ML, RecipeSkillName.Tinkering),
            new RecipeScrollDefinition(135, 460, Expansion.ML, RecipeSkillName.Tinkering),
            new RecipeScrollDefinition(136, 459, Expansion.ML, RecipeSkillName.Tinkering),
            new RecipeScrollDefinition(137, 170, Expansion.TOL, RecipeSkillName.Carpentry),
            new RecipeScrollDefinition(138, 457, Expansion.TOL, RecipeSkillName.Tinkering),
            new RecipeScrollDefinition(139, 458, Expansion.TOL, RecipeSkillName.Tinkering),
            new RecipeScrollDefinition(140, 355, Expansion.SA, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(141, 356, Expansion.SA, RecipeSkillName.Blacksmith),
            new RecipeScrollDefinition(142, 585, Expansion.ML, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(143, 456, Expansion.SA, RecipeSkillName.Tinkering),
            new RecipeScrollDefinition(144, 464, Expansion.SA, RecipeSkillName.Tinkering),
            new RecipeScrollDefinition(145, 465, Expansion.ML, RecipeSkillName.Tinkering),
            new RecipeScrollDefinition(146, 605, Expansion.ML, RecipeSkillName.Cooking),
            new RecipeScrollDefinition(147, 606, Expansion.ML, RecipeSkillName.Cooking),
            new RecipeScrollDefinition(148, 607, Expansion.ML, RecipeSkillName.Cooking),
            new RecipeScrollDefinition(149, 586, Expansion.ML, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(150, 587, Expansion.ML, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(151, 588, Expansion.SA, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(152, 463, Expansion.SA, RecipeSkillName.Tinkering),
            new RecipeScrollDefinition(153, 153, Expansion.HS, RecipeSkillName.Carpentry),
            new RecipeScrollDefinition(154, 154, Expansion.HS, RecipeSkillName.Carpentry),
            new RecipeScrollDefinition(155, 155, Expansion.HS, RecipeSkillName.Carpentry),
            new RecipeScrollDefinition(156, 608, Expansion.ML, RecipeSkillName.Cooking),
            new RecipeScrollDefinition(157, 609, Expansion.ML, RecipeSkillName.Cooking),
            new RecipeScrollDefinition(158, 610, Expansion.ML, RecipeSkillName.Cooking),
            new RecipeScrollDefinition(159, 1100, Expansion.HS, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(160, 1101, Expansion.HS, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(161, 1102, Expansion.HS, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(162, 1103, Expansion.HS, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(163, 1108, Expansion.HS, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(164, 1109, Expansion.HS, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(165, 1104, Expansion.HS, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(166, 1105, Expansion.HS, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(167, 1106, Expansion.HS, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(168, 1107, Expansion.HS, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(169, 1110, Expansion.HS, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(170, 1111, Expansion.HS, RecipeSkillName.Tailoring),
            new RecipeScrollDefinition(171, 466, Expansion.ML, RecipeSkillName.Tinkering),
            new RecipeScrollDefinition(172, 467, Expansion.ML, RecipeSkillName.Tinkering),
            new RecipeScrollDefinition(173, 468, Expansion.ML, RecipeSkillName.Tinkering),
        };

        [Constructable]
        public RecipeBook()
            : base(0xA266)
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
            LoadDefinitions();
            Filter = new RecipeScrollFilter();
            Level = SecureLevel.CoOwners;
        }

        public void LoadDefinitions()
        {
            Recipes = new List<RecipeScrollDefinition>();

            Definitions.ToList().ForEach(x =>
            {
                Recipes.Add(x);
            });
        }

        public void ReLoadDefinitions()
        {
            Definitions.Where(n => !Recipes.Any(o => o.RecipeID == n.RecipeID)).ToList().ForEach(x =>
            {
                Recipes.Add(x);
            });
        }

        public bool CheckAccessible(Mobile from, Item item)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
                return true; // Staff can access anything

            BaseHouse house = BaseHouse.FindHouseAt(item);

            if (house == null)
                return false;

            switch (Level)
            {
                case SecureLevel.Owner: return house.IsOwner(from);
                case SecureLevel.CoOwners: return house.IsCoOwner(from);
                case SecureLevel.Friends: return house.IsFriend(from);
                case SecureLevel.Anyone: return true;
                case SecureLevel.Guild: return house.IsGuildMember(from);
            }

            return false;
        }


        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 2))
            {
                from.LocalOverheadMessage(Network.MessageType.Regular, 0x3B2, 1019045);
            }
            else
            {
                if (from.HasGump(typeof(RecipeBookGump)))
                    return;

                if (!Using)
                {
                    Using = true;
                    from.SendGump(new RecipeBookGump((PlayerMobile)from, this));
                }
                else
                {
                    from.SendLocalizedMessage(1062456); // The book is currently in use.
                }
            }
        }

        public override void OnDoubleClickSecureTrade(Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 2))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
            }
            else
            {
                from.SendGump(new RecipeBookGump(from, this));

                SecureTradeContainer cont = GetSecureTradeCont();

                if (cont != null)
                {
                    SecureTrade trade = cont.Trade;

                    if (trade != null && trade.From.Mobile == from)
                        trade.To.Mobile.SendGump(new RecipeBookGump(trade.To.Mobile, this));
                    else if (trade != null && trade.To.Mobile == from)
                        trade.From.Mobile.SendGump(new RecipeBookGump(trade.From.Mobile, this));
                }
            }
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (!IsChildOf(from.Backpack) && !IsLockedDown)
            {
                from.SendLocalizedMessage(1158823); // You must have the book in your backpack to add recipes to it.
                return false;
            }
            else if (dropped is RecipeScroll)
            {
                RecipeScroll recipe = dropped as RecipeScroll;

                if (Recipes.Any(x => x.RecipeID == recipe.RecipeID))
                {
                    Recipes.ForEach(x =>
                    {
                        if (x.RecipeID == recipe.RecipeID)
                            x.Amount += 1;
                    });

                    InvalidateProperties();

                    from.SendLocalizedMessage(1158826); // Recipe added to the book.

                    if (from is PlayerMobile)
                        from.SendGump(new RecipeBookGump((PlayerMobile)from, this));

                    dropped.Delete();
                    return true;
                }
                else
                {
                    from.SendLocalizedMessage(1158825); // That is not a recipe.
                    return false;
                }
            }
            else
            {
                from.SendLocalizedMessage(1158825); // That is not a recipe.
                return false;
            }
        }

        public RecipeBook(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);

            writer.Write((int)Level);
            writer.Write(BookName);
            Filter.Serialize(writer);

            writer.Write(Recipes.Count);

            Recipes.ForEach(s =>
            {
                writer.Write(s.ID);
                writer.Write(s.RecipeID);
                writer.Write((int)s.Expansion);
                writer.Write((int)s.Skill);
                writer.Write(s.Amount);
                writer.Write(s.Price);
            });

        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                case 0:
                    Level = (SecureLevel)reader.ReadInt();
                    BookName = reader.ReadString();
                    Filter = new RecipeScrollFilter(reader);

                    int count = reader.ReadInt();

                    Recipes = new List<RecipeScrollDefinition>();

                    for (int i = count; i > 0; i--)
                    {
                        int id = reader.ReadInt();
                        int rid = reader.ReadInt();
                        Expansion ex = (Expansion)reader.ReadInt();
                        RecipeSkillName skill = (RecipeSkillName)reader.ReadInt();
                        int amount = reader.ReadInt();
                        int price = reader.ReadInt();

                        Recipes.Add(new RecipeScrollDefinition(id, rid, ex, skill, amount, price));
                    }

                    ReLoadDefinitions();

                    break;
            }

            if (version == 0)
                LootType = LootType.Blessed;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1158849, string.Format("{0}", Recipes.Sum(x => x.Amount))); // Recipes in book: ~1_val~

            if (!string.IsNullOrEmpty(BookName))
                list.Add(1062481, BookName);
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (from.CheckAlive() && IsChildOf(from.Backpack))
            {
                list.Add(new NameBookEntry(from, this));
            }

            SetSecureLevelEntry.AddTo(from, this, list);
        }

        private class NameBookEntry : ContextMenuEntry
        {
            private readonly Mobile m_From;
            private readonly RecipeBook m_Book;

            public NameBookEntry(Mobile from, RecipeBook book)
                : base(6216)
            {
                m_From = from;
                m_Book = book;
            }

            public override void OnClick()
            {
                if (m_From.CheckAlive() && m_Book.IsChildOf(m_From.Backpack))
                {
                    m_From.Prompt = new NameBookPrompt(m_Book);
                    m_From.SendLocalizedMessage(1062479); // Type in the new name of the book:
                }
            }
        }

        private class NameBookPrompt : Prompt
        {
            public override int MessageCliloc => 1062479;
            private readonly RecipeBook m_Book;

            public NameBookPrompt(RecipeBook book)
            {
                m_Book = book;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (text.Length > 40)
                    text = text.Substring(0, 40);

                if (from.CheckAlive() && m_Book.IsChildOf(from.Backpack))
                {
                    m_Book.BookName = Utility.FixHtml(text.Trim());

                    from.SendLocalizedMessage(1158827); // The recipe book's name has been changed.
                }
            }

            public override void OnCancel(Mobile from)
            {
            }
        }
    }
}
