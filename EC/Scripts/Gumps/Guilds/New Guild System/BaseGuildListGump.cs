using System;
using System.Collections.Generic;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Guilds
{
    public struct InfoField<T>
    {
        private readonly TextDefinition m_Name;
        private readonly int m_Width;
        private readonly IComparer<T> m_Comparer;
        public InfoField(TextDefinition name, int width, IComparer<T> comparer)
        {
            this.m_Name = name;
            this.m_Width = width;
            this.m_Comparer = comparer;
        }

        public TextDefinition Name
        {
            get
            {
                return this.m_Name;
            }
        }
        public int Width
        {
            get
            {
                return this.m_Width;
            }
        }
        public IComparer<T> Comparer
        {
            get
            {
                return this.m_Comparer;
            }
        }
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
            this.m_Filter = filter.Trim();

            this.m_Comparer = currentComparer;
            this.m_Fields = fields;
            this.m_Ascending = ascending;
            this.m_StartNumber = startNumber;
            this.m_List = list;
        }

        public virtual bool WillFilter
        {
            get
            {
                return (this.m_Filter.Length >= 0);
            }
        }
        public override void PopulateGump()
        {
            base.PopulateGump();

            List<T> list = this.m_List;
            if (this.WillFilter)
            {
                this.m_List = new List<T>();
                for (int i = 0; i < list.Count; i++)
                {
                    if (!this.IsFiltered(list[i], this.m_Filter))
                        this.m_List.Add(list[i]);
                }
            }
            else
            {
                this.m_List = new List<T>(list);
            }

            this.m_List.Sort(this.m_Comparer);
            this.m_StartNumber = Math.Max(Math.Min(this.m_StartNumber, this.m_List.Count - 1), 0);

            this.AddBackground(130, 75, 385, 30, 0xBB8);
            this.AddTextEntry(135, 80, 375, 30, 0x481, 1, this.m_Filter);
            this.AddButton(520, 75, 0x867, 0x868, 5, GumpButtonType.Reply, 0);	//Filter Button

            int width = 0;
            for (int i = 0; i < this.m_Fields.Length; i++)
            {
                InfoField<T> f = this.m_Fields[i];

                this.AddImageTiled(65 + width, 110, f.Width + 10, 26, 0xA40);
                this.AddImageTiled(67 + width, 112, f.Width + 6, 22, 0xBBC);
                this.AddHtmlText(70 + width, 113, f.Width, 20, f.Name, false, false);

                bool isComparer = (this.m_Fields[i].Comparer.GetType() == this.m_Comparer.GetType());

                int ButtonID = (isComparer) ? (this.m_Ascending ? 0x983 : 0x985) : 0x2716;

                this.AddButton(59 + width + f.Width, 117, ButtonID, ButtonID + (isComparer ? 1 : 0), 100 + i, GumpButtonType.Reply, 0);

                width += (f.Width + 12);
            }

            if (this.m_StartNumber <= 0)
                this.AddButton(65, 80, 0x15E3, 0x15E7, 0, GumpButtonType.Page, 0);
            else
                this.AddButton(65, 80, 0x15E3, 0x15E7, 6, GumpButtonType.Reply, 0);	// Back

            if (this.m_StartNumber + itemsPerPage > this.m_List.Count)
                this.AddButton(95, 80, 0x15E1, 0x15E5, 0, GumpButtonType.Page, 0);	
            else
                this.AddButton(95, 80, 0x15E1, 0x15E5, 7, GumpButtonType.Reply, 0);	// Forward

            int itemNumber = 0;

            if (this.m_Ascending)
                for (int i = this.m_StartNumber; i < this.m_StartNumber + itemsPerPage && i < this.m_List.Count; i++)
                    this.DrawEntry(this.m_List[i], i, itemNumber++);
            else //descending, go from bottom of list to the top
                for (int i = this.m_List.Count - 1 - this.m_StartNumber; i >= 0 && i >= (this.m_List.Count - itemsPerPage - this.m_StartNumber); i--)
                    this.DrawEntry(this.m_List[i], i, itemNumber++);

            this.DrawEndingEntry(itemNumber);
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
            for (int j = 0; j < this.m_Fields.Length; j++)
            {
                InfoField<T> f = this.m_Fields[j];

                this.AddImageTiled(65 + width, 138 + itemNumber * 28, f.Width + 10, 26, 0xA40);
                this.AddImageTiled(67 + width, 140 + itemNumber * 28, f.Width + 6, 22, 0xBBC);
                this.AddHtmlText(70 + width, 141 + itemNumber * 28, f.Width, 20, this.GetValuesFor(o, this.m_Fields.Length)[j], false, false);

                width += (f.Width + 12);
            }

            if (this.HasRelationship(o))
                this.AddButton(40, 143 + itemNumber * 28, 0x8AF, 0x8AF, 200 + index, GumpButtonType.Reply, 0);	//Info Button
            else
                this.AddButton(40, 143 + itemNumber * 28, 0x4B9, 0x4BA, 200 + index, GumpButtonType.Reply, 0);	//Info Button
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            base.OnResponse(sender, info);

            PlayerMobile pm = sender.Mobile as PlayerMobile;

            if (pm == null || !IsMember(pm, this.guild))
                return;

            int id = info.ButtonID;

            switch( id )
            {
                case 5:	//Filter
                    {
                        TextRelay t = info.GetTextEntry(1);
                        pm.SendGump(this.GetResentGump(this.player, this.guild, this.m_Comparer, this.m_Ascending, (t == null) ? "" : t.Text, 0));
                        break;
                    }
                case 6: //Back
                    {
                        pm.SendGump(this.GetResentGump(this.player, this.guild, this.m_Comparer, this.m_Ascending, this.m_Filter, this.m_StartNumber - itemsPerPage));
                        break;
                    }
                case 7:	//Forward
                    {
                        pm.SendGump(this.GetResentGump(this.player, this.guild, this.m_Comparer, this.m_Ascending, this.m_Filter, this.m_StartNumber + itemsPerPage));
                        break;
                    }
            }

            if (id >= 100 && id < (100 + this.m_Fields.Length))
            {
                IComparer<T> comparer = this.m_Fields[id - 100].Comparer;

                if (this.m_Comparer.GetType() == comparer.GetType())
                    this.m_Ascending = !this.m_Ascending;

                pm.SendGump(this.GetResentGump(this.player, this.guild, comparer, this.m_Ascending, this.m_Filter, 0));
            }
            else if (id >= 200 && id < (200 + this.m_List.Count))
            {
                pm.SendGump(this.GetObjectInfoGump(this.player, this.guild, this.m_List[id - 200]));
            }
        }

        public abstract Gump GetResentGump(PlayerMobile pm, Guild g, IComparer<T> comparer, bool ascending, string filter, int startNumber);

        public abstract Gump GetObjectInfoGump(PlayerMobile pm, Guild g, T o);

        public void ResendGump()
        {
            this.player.SendGump(this.GetResentGump(this.player, this.guild, this.m_Comparer, this.m_Ascending, this.m_Filter, this.m_StartNumber));
        }

        protected abstract TextDefinition[] GetValuesFor(T o, int aryLength);

        protected abstract bool IsFiltered(T o, string filter);
    }
}