using System;
using System.Collections.Generic;
using Server.Gumps;
using Server.Multis;
using Server.Prompts;
using Server.Mobiles;
using Server.ContextMenus;
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
        {
            ID = id;
            RecipeID = rid;
            Expansion = exp;
            Skill = skill;
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
		public override int LabelNumber { get { return 1125598; } } // recipe book

        [CommandProperty(AccessLevel.GameMaster)]
        public string BookName { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level { get; set; }

        public List<RecipeScrollDefinition> Recipes;

        public RecipeScrollFilter Filter { get; set; }

        public void LoadDefinitions()
        {
            Recipes = new List<RecipeScrollDefinition>();

            Recipes.Add(new RecipeScrollDefinition(1, 501, Expansion.ML, RecipeSkillName.Tailoring));
            Recipes.Add(new RecipeScrollDefinition(2, 502, Expansion.ML, RecipeSkillName.Tailoring));
            Recipes.Add(new RecipeScrollDefinition(3, 503, Expansion.ML, RecipeSkillName.Tailoring));
            Recipes.Add(new RecipeScrollDefinition(4, 504, Expansion.ML, RecipeSkillName.Tailoring));
            Recipes.Add(new RecipeScrollDefinition(5, 505, Expansion.ML, RecipeSkillName.Tailoring));
            Recipes.Add(new RecipeScrollDefinition(6, 599, Expansion.ML, RecipeSkillName.Cooking));
            Recipes.Add(new RecipeScrollDefinition(7, 250, Expansion.ML, RecipeSkillName.Fletching));
            Recipes.Add(new RecipeScrollDefinition(8, 251, Expansion.ML, RecipeSkillName.Fletching));
            Recipes.Add(new RecipeScrollDefinition(9, 252, Expansion.ML, RecipeSkillName.Fletching));
            Recipes.Add(new RecipeScrollDefinition(10, 253, Expansion.ML, RecipeSkillName.Fletching));
            Recipes.Add(new RecipeScrollDefinition(11, 254, Expansion.ML, RecipeSkillName.Fletching));
            Recipes.Add(new RecipeScrollDefinition(12, 350, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(13, 351, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(14, 354, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(15, 150, Expansion.ML, RecipeSkillName.Carpentry));
            Recipes.Add(new RecipeScrollDefinition(16, 352, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(17, 353, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(18, 151, Expansion.ML, RecipeSkillName.Carpentry));
            Recipes.Add(new RecipeScrollDefinition(19, 152, Expansion.ML, RecipeSkillName.Carpentry));
            Recipes.Add(new RecipeScrollDefinition(20, 551, Expansion.ML, RecipeSkillName.Tailoring));
            Recipes.Add(new RecipeScrollDefinition(21, 550, Expansion.ML, RecipeSkillName.Tailoring));
            Recipes.Add(new RecipeScrollDefinition(22, 552, Expansion.ML, RecipeSkillName.Tailoring));
            Recipes.Add(new RecipeScrollDefinition(23, 452, Expansion.ML, RecipeSkillName.Tinkering));
            Recipes.Add(new RecipeScrollDefinition(24, 450, Expansion.ML, RecipeSkillName.Tinkering));
            Recipes.Add(new RecipeScrollDefinition(25, 451, Expansion.ML, RecipeSkillName.Tinkering));
            Recipes.Add(new RecipeScrollDefinition(26, 453, Expansion.ML, RecipeSkillName.Inscription));
            Recipes.Add(new RecipeScrollDefinition(27, 116, Expansion.ML, RecipeSkillName.Carpentry));
            Recipes.Add(new RecipeScrollDefinition(28, 106, Expansion.ML, RecipeSkillName.Carpentry));
            Recipes.Add(new RecipeScrollDefinition(29, 108, Expansion.ML, RecipeSkillName.Carpentry));
            Recipes.Add(new RecipeScrollDefinition(30, 109, Expansion.ML, RecipeSkillName.Carpentry));
            Recipes.Add(new RecipeScrollDefinition(31, 100, Expansion.ML, RecipeSkillName.Carpentry));
            Recipes.Add(new RecipeScrollDefinition(32, 102, Expansion.ML, RecipeSkillName.Carpentry));
            Recipes.Add(new RecipeScrollDefinition(33, 111, Expansion.ML, RecipeSkillName.Carpentry));
            Recipes.Add(new RecipeScrollDefinition(34, 118, Expansion.ML, RecipeSkillName.Masonry));
            Recipes.Add(new RecipeScrollDefinition(35, 200, Expansion.ML, RecipeSkillName.Fletching));
            Recipes.Add(new RecipeScrollDefinition(36, 201, Expansion.ML, RecipeSkillName.Fletching));
            Recipes.Add(new RecipeScrollDefinition(37, 202, Expansion.ML, RecipeSkillName.Fletching));
            Recipes.Add(new RecipeScrollDefinition(38, 203, Expansion.ML, RecipeSkillName.Fletching));
            Recipes.Add(new RecipeScrollDefinition(39, 204, Expansion.ML, RecipeSkillName.Fletching));
            Recipes.Add(new RecipeScrollDefinition(40, 205, Expansion.ML, RecipeSkillName.Fletching));
            Recipes.Add(new RecipeScrollDefinition(41, 206, Expansion.ML, RecipeSkillName.Fletching));
            Recipes.Add(new RecipeScrollDefinition(42, 207, Expansion.ML, RecipeSkillName.Fletching));
            Recipes.Add(new RecipeScrollDefinition(43, 300, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(44, 301, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(45, 302, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(46, 303, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(47, 304, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(48, 305, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(49, 306, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(50, 307, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(51, 308, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(52, 309, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(53, 310, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(54, 311, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(55, 312, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(56, 313, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(57, 314, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(58, 315, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(59, 332, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(60, 333, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(61, 334, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(62, 335, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(63, 316, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(64, 317, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(65, 318, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(66, 319, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(67, 320, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(68, 321, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(69, 322, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(70, 323, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(71, 324, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(72, 325, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(73, 326, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(74, 327, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(75, 328, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(76, 329, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(77, 330, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(78, 331, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(79, 112, Expansion.ML, RecipeSkillName.Carpentry));
            Recipes.Add(new RecipeScrollDefinition(80, 113, Expansion.ML, RecipeSkillName.Carpentry));
            Recipes.Add(new RecipeScrollDefinition(81, 114, Expansion.ML, RecipeSkillName.Carpentry));
            Recipes.Add(new RecipeScrollDefinition(82, 115, Expansion.ML, RecipeSkillName.Carpentry));
            Recipes.Add(new RecipeScrollDefinition(83, 400, Expansion.ML, RecipeSkillName.Alchemy));
            Recipes.Add(new RecipeScrollDefinition(84, 402, Expansion.ML, RecipeSkillName.Alchemy));
            Recipes.Add(new RecipeScrollDefinition(85, 401, Expansion.ML, RecipeSkillName.Alchemy));
            Recipes.Add(new RecipeScrollDefinition(86, 117, Expansion.ML, RecipeSkillName.Carpentry));
            Recipes.Add(new RecipeScrollDefinition(87, 107, Expansion.ML, RecipeSkillName.Carpentry));
            Recipes.Add(new RecipeScrollDefinition(88, 120, Expansion.ML, RecipeSkillName.Carpentry));
            Recipes.Add(new RecipeScrollDefinition(89, 110, Expansion.ML, RecipeSkillName.Carpentry));
            Recipes.Add(new RecipeScrollDefinition(90, 101, Expansion.ML, RecipeSkillName.Carpentry));
            Recipes.Add(new RecipeScrollDefinition(91, 103, Expansion.ML, RecipeSkillName.Carpentry));
            Recipes.Add(new RecipeScrollDefinition(92, 105, Expansion.ML, RecipeSkillName.Carpentry));
            Recipes.Add(new RecipeScrollDefinition(93, 119, Expansion.ML, RecipeSkillName.Masonry));
            Recipes.Add(new RecipeScrollDefinition(94, 104, Expansion.ML, RecipeSkillName.Carpentry));
            Recipes.Add(new RecipeScrollDefinition(95, 336, Expansion.ML, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(96, 500, Expansion.ML, RecipeSkillName.Cooking));
            Recipes.Add(new RecipeScrollDefinition(97, 604, Expansion.ML, RecipeSkillName.Cooking));
            Recipes.Add(new RecipeScrollDefinition(98, 603, Expansion.ML, RecipeSkillName.Cooking));
            Recipes.Add(new RecipeScrollDefinition(99, 702, Expansion.ML, RecipeSkillName.Masonry));
            Recipes.Add(new RecipeScrollDefinition(100, 701, Expansion.ML, RecipeSkillName.Masonry));
            Recipes.Add(new RecipeScrollDefinition(101, 560, Expansion.TOL, RecipeSkillName.Tailoring));
            Recipes.Add(new RecipeScrollDefinition(102, 561, Expansion.TOL, RecipeSkillName.Tailoring));
            Recipes.Add(new RecipeScrollDefinition(103, 562, Expansion.TOL, RecipeSkillName.Tailoring));
            Recipes.Add(new RecipeScrollDefinition(104, 563, Expansion.TOL, RecipeSkillName.Tailoring));
            Recipes.Add(new RecipeScrollDefinition(105, 564, Expansion.TOL, RecipeSkillName.Tailoring));
            Recipes.Add(new RecipeScrollDefinition(106, 565, Expansion.TOL, RecipeSkillName.Tailoring));
            Recipes.Add(new RecipeScrollDefinition(107, 566, Expansion.TOL, RecipeSkillName.Tailoring));
            Recipes.Add(new RecipeScrollDefinition(108, 570, Expansion.TOL, RecipeSkillName.Tailoring));
            Recipes.Add(new RecipeScrollDefinition(109, 575, Expansion.TOL, RecipeSkillName.Tailoring));
            Recipes.Add(new RecipeScrollDefinition(110, 573, Expansion.TOL, RecipeSkillName.Tailoring));
            Recipes.Add(new RecipeScrollDefinition(111, 574, Expansion.TOL, RecipeSkillName.Tailoring));
            Recipes.Add(new RecipeScrollDefinition(112, 576, Expansion.TOL, RecipeSkillName.Tailoring));
            Recipes.Add(new RecipeScrollDefinition(113, 577, Expansion.TOL, RecipeSkillName.Tailoring));
            Recipes.Add(new RecipeScrollDefinition(114, 572, Expansion.TOL, RecipeSkillName.Tailoring));
            Recipes.Add(new RecipeScrollDefinition(115, 571, Expansion.TOL, RecipeSkillName.Tailoring));
            Recipes.Add(new RecipeScrollDefinition(116, 581, Expansion.TOL, RecipeSkillName.Tailoring));
            Recipes.Add(new RecipeScrollDefinition(117, 584, Expansion.TOL, RecipeSkillName.Tailoring));
            Recipes.Add(new RecipeScrollDefinition(118, 583, Expansion.TOL, RecipeSkillName.Tailoring));
            Recipes.Add(new RecipeScrollDefinition(119, 582, Expansion.TOL, RecipeSkillName.Tailoring));
            Recipes.Add(new RecipeScrollDefinition(120, 580, Expansion.TOL, RecipeSkillName.Tailoring));
            Recipes.Add(new RecipeScrollDefinition(121, 600, Expansion.ML, RecipeSkillName.Cooking));
            Recipes.Add(new RecipeScrollDefinition(122, 601, Expansion.ML, RecipeSkillName.Cooking));
            Recipes.Add(new RecipeScrollDefinition(123, 602, Expansion.ML, RecipeSkillName.Cooking));
            Recipes.Add(new RecipeScrollDefinition(124, 800, Expansion.ML, RecipeSkillName.Inscription));
            Recipes.Add(new RecipeScrollDefinition(125, 900, Expansion.TOL, RecipeSkillName.Alchemy));
            Recipes.Add(new RecipeScrollDefinition(126, 901, Expansion.TOL, RecipeSkillName.Alchemy));
            Recipes.Add(new RecipeScrollDefinition(127, 902, Expansion.TOL, RecipeSkillName.Alchemy));
            Recipes.Add(new RecipeScrollDefinition(128, 903, Expansion.TOL, RecipeSkillName.Alchemy));
            Recipes.Add(new RecipeScrollDefinition(129, 904, Expansion.TOL, RecipeSkillName.Alchemy));
            Recipes.Add(new RecipeScrollDefinition(130, 905, Expansion.TOL, RecipeSkillName.Alchemy));
            Recipes.Add(new RecipeScrollDefinition(131, 1000, Expansion.TOL, RecipeSkillName.Cartography));
            Recipes.Add(new RecipeScrollDefinition(132, 455, Expansion.TOL, RecipeSkillName.Tinkering));
            Recipes.Add(new RecipeScrollDefinition(133, 461, Expansion.ML, RecipeSkillName.Tinkering));
            Recipes.Add(new RecipeScrollDefinition(134, 462, Expansion.ML, RecipeSkillName.Tinkering));
            Recipes.Add(new RecipeScrollDefinition(135, 460, Expansion.ML, RecipeSkillName.Tinkering));
            Recipes.Add(new RecipeScrollDefinition(136, 459, Expansion.ML, RecipeSkillName.Tinkering));
            Recipes.Add(new RecipeScrollDefinition(137, 170, Expansion.TOL, RecipeSkillName.Carpentry));
            Recipes.Add(new RecipeScrollDefinition(138, 457, Expansion.TOL, RecipeSkillName.Tinkering));
            Recipes.Add(new RecipeScrollDefinition(139, 458, Expansion.TOL, RecipeSkillName.Tinkering));
            Recipes.Add(new RecipeScrollDefinition(140, 355, Expansion.SA, RecipeSkillName.Blacksmith));

            Recipes.Add(new RecipeScrollDefinition(141, 356, Expansion.SA, RecipeSkillName.Blacksmith));
            Recipes.Add(new RecipeScrollDefinition(142, 585, Expansion.ML, RecipeSkillName.Tailoring));
            Recipes.Add(new RecipeScrollDefinition(143, 456, Expansion.SA, RecipeSkillName.Tinkering));
            //Recipes.Add(new RecipeScrollDefinition(144, 205, Expansion.SA, RecipeSkillName.Tinkering)); // Enchanted picnic basket
            //Recipes.Add(new RecipeScrollDefinition(145, 205, Expansion.ML, RecipeSkillName.Tinkering)); // telescope
            Recipes.Add(new RecipeScrollDefinition(146, 605, Expansion.ML, RecipeSkillName.Cooking));  
            Recipes.Add(new RecipeScrollDefinition(147, 606, Expansion.ML, RecipeSkillName.Cooking));
            Recipes.Add(new RecipeScrollDefinition(148, 607, Expansion.ML, RecipeSkillName.Cooking));
            //Recipes.Add(new RecipeScrollDefinition(149, 205, Expansion.ML, RecipeSkillName.Tailoring)); // krampus minion hat
            //Recipes.Add(new RecipeScrollDefinition(150, 205, Expansion.ML, RecipeSkillName.Tailoring)); // krampus minion boots
            //Recipes.Add(new RecipeScrollDefinition(151, 205, Expansion.SA, RecipeSkillName.Tailoring)); // krampus minion talons
            //Recipes.Add(new RecipeScrollDefinition(152, 205, Expansion.SA, RecipeSkillName.Tailoring)); // krampus minion earrings
        }

        [Constructable]
        public RecipeBook()
            : base(0xA266)
        {
            Weight = 1.0;

            LoadDefinitions();
            Filter = new RecipeScrollFilter();
            Level = SecureLevel.CoOwners;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 2))
            {
                from.LocalOverheadMessage(Network.MessageType.Regular, 0x3B2, 1019045);
            }
            else
            {
                from.SendGump(new RecipeBookGump((PlayerMobile)from, this));
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
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1158823); // You must have the book in your backpack to add recipes to it.
                return false;
            }
            else if (dropped is RecipeScroll)
            {
                RecipeScroll recipe = dropped as RecipeScroll;

                Recipes.ForEach(x =>
                {
                    if (x.RecipeID == recipe.RecipeID)
                        x.Amount = x.Amount + 1;
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

        public RecipeBook(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

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

                    break;
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
			
			list.Add(1158849, String.Format("{0}", Recipes.Sum(x => x.Amount))); // Recipes in book: ~1_val~

            if (BookName != null && BookName.Length > 0)
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
            private Mobile m_From;
            private RecipeBook m_Book;

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
            public override int MessageCliloc { get { return 1062479; } }
            private RecipeBook m_Book;

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
