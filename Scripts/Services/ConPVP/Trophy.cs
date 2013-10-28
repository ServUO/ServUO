using System;

namespace Server.Items
{
    public enum TrophyRank
    {
        Bronze,
        Silver,
        Gold
    }

    [Flipable(5020, 4647)]
    public class Trophy : Item
    {
        private string m_Title;
        private TrophyRank m_Rank;
        private Mobile m_Owner;
        private DateTime m_Date;
        [Constructable]
        public Trophy(string title, TrophyRank rank)
            : base(5020)
        {
            this.m_Title = title;
            this.m_Rank = rank;
            this.m_Date = DateTime.UtcNow;

            this.LootType = LootType.Blessed;

            this.UpdateStyle();
        }

        public Trophy(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Title
        {
            get
            {
                return this.m_Title;
            }
            set
            {
                this.m_Title = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public TrophyRank Rank
        {
            get
            {
                return this.m_Rank;
            }
            set
            {
                this.m_Rank = value;
                this.UpdateStyle();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner
        {
            get
            {
                return this.m_Owner;
            }
            set
            {
                this.m_Owner = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime Date
        {
            get
            {
                return this.m_Date;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write((string)this.m_Title);
            writer.Write((int)this.m_Rank);
            writer.Write((Mobile)this.m_Owner);
            writer.Write((DateTime)this.m_Date);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.m_Title = reader.ReadString();
            this.m_Rank = (TrophyRank)reader.ReadInt();
            this.m_Owner = reader.ReadMobile();
            this.m_Date = reader.ReadDateTime();

            if (version == 0)
                this.LootType = LootType.Blessed;
        }

        public override void OnAdded(object parent)
        {
            base.OnAdded(parent);

            if (this.m_Owner == null)
                this.m_Owner = this.RootParent as Mobile;
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            if (this.m_Owner != null)
                this.LabelTo(from, "{0} -- {1}", this.m_Title, this.m_Owner.RawName);
            else if (this.m_Title != null)
                this.LabelTo(from, this.m_Title);

            if (this.m_Date != DateTime.MinValue)
                this.LabelTo(from, this.m_Date.ToString("d"));
        }

        public void UpdateStyle()
        {
            this.Name = String.Format("{0} trophy", this.m_Rank.ToString().ToLower());

            switch ( this.m_Rank )
            {
                case TrophyRank.Gold:
                    this.Hue = 2213;
                    break;
                case TrophyRank.Silver:
                    this.Hue = 0;
                    break;
                case TrophyRank.Bronze:
                    this.Hue = 2206;
                    break;
            }
        }
    }
}