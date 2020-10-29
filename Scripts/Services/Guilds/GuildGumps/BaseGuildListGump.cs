using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using System;
using System.Collections.Generic;

namespace Server.Guilds
{
    public struct InfoField<T>
    {
        public InfoField(TextDefinition name, int width, IComparer<T> comparer)
        {
            Name = name;
            Width = width;
            Comparer = comparer;
        }

        public TextDefinition Name { get; }

        public int Width { get; }

        public IComparer<T> Comparer { get; }
    }

    public abstract class BaseGuildListGump<T> : BaseGuildGump
    {
        private const int itemsPerPage = 8;
        readonly IComparer<T> m_Comparer;
        readonly InfoField<T>[] m_Fields;
        readonly string m_Filter;
        List<T> m_List;
        bool m_Ascending;
        int m_StartNumber;
        public BaseGuildListGump(PlayerMobile pm, Guild g, List<T> list, IComparer<T> currentComparer, bool ascending, string filter, int startNumber, InfoField<T>[] fields)
            : base(pm, g)
        {
            m_Filter = filter.Trim();

            m_Comparer = currentComparer;
            m_Fields = fields;
            m_Ascending = ascending;
            m_StartNumber = startNumber;
            m_List = list;
        }

        public virtual bool WillFilter => (m_Filter.Length >= 0);
        public override void PopulateGump()
        {
            base.PopulateGump();

            List<T> list = m_List;
            if (WillFilter)
            {
                m_List = new List<T>();
                for (int i = 0; i < list.Count; i++)
                {
                    if (!IsFiltered(list[i], m_Filter))
                        m_List.Add(list[i]);
                }
            }
            else
            {
                m_List = new List<T>(list);
            }

            m_List.Sort(m_Comparer);
            m_StartNumber = Math.Max(Math.Min(m_StartNumber, m_List.Count - 1), 0);

            AddBackground(130, 75, 385, 30, 0xBB8);
            AddTextEntry(135, 80, 375, 30, 0x481, 1, m_Filter);
            AddButton(520, 75, 0x867, 0x868, 5, GumpButtonType.Reply, 0);	//Filter Button

            int width = 0;
            for (int i = 0; i < m_Fields.Length; i++)
            {
                InfoField<T> f = m_Fields[i];

                AddImageTiled(65 + width, 110, f.Width + 10, 26, 0xA40);
                AddImageTiled(67 + width, 112, f.Width + 6, 22, 0xBBC);
                AddHtmlText(70 + width, 113, f.Width, 20, f.Name, false, false);

                bool isComparer = (m_Fields[i].Comparer.GetType() == m_Comparer.GetType());

                int ButtonID = (isComparer) ? (m_Ascending ? 0x983 : 0x985) : 0x2716;

                AddButton(59 + width + f.Width, 117, ButtonID, ButtonID + (isComparer ? 1 : 0), 100 + i, GumpButtonType.Reply, 0);

                width += (f.Width + 12);
            }

            if (m_StartNumber <= 0)
                AddButton(65, 80, 0x15E3, 0x15E7, 0, GumpButtonType.Page, 0);
            else
                AddButton(65, 80, 0x15E3, 0x15E7, 6, GumpButtonType.Reply, 0);	// Back

            if (m_StartNumber + itemsPerPage > m_List.Count)
                AddButton(95, 80, 0x15E1, 0x15E5, 0, GumpButtonType.Page, 0);
            else
                AddButton(95, 80, 0x15E1, 0x15E5, 7, GumpButtonType.Reply, 0);	// Forward

            int itemNumber = 0;

            if (m_Ascending)
                for (int i = m_StartNumber; i < m_StartNumber + itemsPerPage && i < m_List.Count; i++)
                    DrawEntry(m_List[i], i, itemNumber++);
            else //descending, go from bottom of list to the top
                for (int i = m_List.Count - 1 - m_StartNumber; i >= 0 && i >= (m_List.Count - itemsPerPage - m_StartNumber); i--)
                    DrawEntry(m_List[i], i, itemNumber++);

            DrawEndingEntry(itemNumber);
        }

        public virtual void DrawEndingEntry(int itemNumber)
        {
        }

        public virtual bool HasRelationship(T o)
        {
            return false;
        }

        public virtual void DrawEntry(T o, int index, int itemNumber)
        {
            int width = 0;
            for (int j = 0; j < m_Fields.Length; j++)
            {
                InfoField<T> f = m_Fields[j];

                AddImageTiled(65 + width, 138 + itemNumber * 28, f.Width + 10, 26, 0xA40);
                AddImageTiled(67 + width, 140 + itemNumber * 28, f.Width + 6, 22, 0xBBC);
                AddHtmlText(70 + width, 141 + itemNumber * 28, f.Width, 20, GetValuesFor(o, m_Fields.Length)[j], false, false);

                width += (f.Width + 12);
            }

            if (HasRelationship(o))
                AddButton(40, 143 + itemNumber * 28, 0x8AF, 0x8AF, 200 + index, GumpButtonType.Reply, 0);	//Info Button
            else
                AddButton(40, 143 + itemNumber * 28, 0x4B9, 0x4BA, 200 + index, GumpButtonType.Reply, 0);	//Info Button
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            base.OnResponse(sender, info);

            PlayerMobile pm = sender.Mobile as PlayerMobile;

            if (pm == null || !IsMember(pm, guild))
                return;

            int id = info.ButtonID;

            switch (id)
            {
                case 5:	//Filter
                    {
                        TextRelay t = info.GetTextEntry(1);
                        pm.SendGump(GetResentGump(player, guild, m_Comparer, m_Ascending, (t == null) ? "" : t.Text, 0));
                        break;
                    }
                case 6: //Back
                    {
                        pm.SendGump(GetResentGump(player, guild, m_Comparer, m_Ascending, m_Filter, m_StartNumber - itemsPerPage));
                        break;
                    }
                case 7:	//Forward
                    {
                        pm.SendGump(GetResentGump(player, guild, m_Comparer, m_Ascending, m_Filter, m_StartNumber + itemsPerPage));
                        break;
                    }
            }

            if (id >= 100 && id < (100 + m_Fields.Length))
            {
                IComparer<T> comparer = m_Fields[id - 100].Comparer;

                if (m_Comparer.GetType() == comparer.GetType())
                    m_Ascending = !m_Ascending;

                pm.SendGump(GetResentGump(player, guild, comparer, m_Ascending, m_Filter, 0));
            }
            else if (id >= 200 && id < (200 + m_List.Count))
            {
                pm.SendGump(GetObjectInfoGump(player, guild, m_List[id - 200]));
            }
        }

        public abstract Gump GetResentGump(PlayerMobile pm, Guild g, IComparer<T> comparer, bool ascending, string filter, int startNumber);

        public abstract Gump GetObjectInfoGump(PlayerMobile pm, Guild g, T o);

        public void ResendGump()
        {
            player.SendGump(GetResentGump(player, guild, m_Comparer, m_Ascending, m_Filter, m_StartNumber));
        }

        protected abstract TextDefinition[] GetValuesFor(T o, int aryLength);

        protected abstract bool IsFiltered(T o, string filter);
    }
}
