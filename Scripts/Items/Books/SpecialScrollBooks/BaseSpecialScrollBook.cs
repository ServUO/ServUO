using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.ContextMenus;
using System.Collections.Generic;
using Server.Multis;
using System.Linq;

namespace Server.Items
{
    [Flipable(0x9A95, 0x9AA7)]
    public abstract class BaseSpecialScrollBook : Container, ISecurable
    {
        public const int MaxScrolls = 300;

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level { get; set; }

        public override bool DisplaysContent { get { return false; } }

        public abstract Type ScrollType { get; }

        public abstract int BadDropMessage { get; }
        public abstract int DropMessage { get; }
        public abstract int RemoveMessage { get; }
        public abstract int GumpTitle { get; }

        public BaseSpecialScrollBook(int id) 
            : base(id)
        {
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile m)
        {
            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (m is PlayerMobile && m.InRange(GetWorldLocation(), 2) && (house == null || house.HasSecureAccess(m, Level)))
            {
                BaseGump.SendGump(new SpecialScrollBookGump((PlayerMobile)m, this));
            }
            else if (m.AccessLevel > AccessLevel.Player)
            {
                base.OnDoubleClick(m);
            }
        }


        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1151797, String.Format("{0}\t{1}", Items.Count, MaxScrolls.ToString())); // Scrolls in book: ~1_val~/~2_val~
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
            SetSecureLevelEntry.AddTo(from, this, list);
        }

        public override bool OnDragDrop(Mobile m, Item dropped)
        {
            if (m.InRange(GetWorldLocation(), 2))
            {
                BaseHouse house = BaseHouse.FindHouseAt(this);

                if (dropped.GetType() != ScrollType)
                {
                    m.SendLocalizedMessage(BadDropMessage);
                }
                else if (house == null || !IsLockedDown)
                {
                    m.SendLocalizedMessage(1151765); // You must lock this book down in a house to add scrolls to it.
                }
                else if (!house.HasSecureAccess(m, Level))
                {
                    m.SendLocalizedMessage(1155693); // This item is impermissible and can not be added to the book.
                }
                else if (Items.Count < MaxScrolls) // TODO: Message for overfilled?
                {
                    DropItem(dropped);
                    m.SendLocalizedMessage(DropMessage);

                    dropped.Movable = false;
                    m.CloseGump(typeof(SpecialScrollBookGump));

                    return true;
                }
            }

            return false;
        }

        public virtual void Construct(Mobile m, SkillName sk, double value)
        {
            var scroll = Items.OfType<SpecialScroll>().FirstOrDefault(s => s.Skill == sk && s.Value == value);

            if (scroll != null)
            {
                if (m.Backpack == null || !m.Backpack.TryDropItem(m, scroll, false))
                {
                    m.SendLocalizedMessage(502868); // Your backpack is too full.
                }
                else
                {
                    BaseHouse house = BaseHouse.FindHouseAt(this);

                    if (house != null && house.LockDowns.ContainsKey(scroll))
                    {
                        house.LockDowns.Remove(scroll);
                    }

                    if (!scroll.Movable)
                    {
                        scroll.Movable = true;
                    }

                    if (scroll.IsLockedDown)
                    {
                        scroll.IsLockedDown = false;
                    }

                    m.SendLocalizedMessage(RemoveMessage);
                }
            }
        }

        public BaseSpecialScrollBook(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)2); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if(version < 2)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(10), () =>
                    {
                        BaseHouse house = BaseHouse.FindHouseAt(this);

                        foreach (var item in Items)
                        {
                            if (house != null && house.LockDowns.ContainsKey(item))
                            {
                                house.LockDowns.Remove(item);
                            }

                            item.IsLockedDown = false;
                            item.Movable = false;
                        }
                    });
            }
        }

        public virtual Dictionary<SkillCat, List<SkillName>> SkillInfo { get { return null; } }
        public virtual Dictionary<int, double> ValueInfo { get { return null; } }

        public static int GetCategoryLocalization(SkillCat category)
        {
            switch (category)
            {
                default:
                case SkillCat.None: return 0;
                case SkillCat.Miscellaneous: return 1078596;
                case SkillCat.Combat: return 1078592;
                case SkillCat.TradeSkills: return 1078591;
                case SkillCat.Magic: return 1078593;
                case SkillCat.Wilderness: return 1078595;
                case SkillCat.Thievery: return 1078594;
                case SkillCat.Bard: return 1078590;
            }
        }
    }
}