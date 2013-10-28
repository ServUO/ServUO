using System;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public abstract class SpecialScroll : Item
    {
        private SkillName m_Skill;
        private double m_Value;

        #region Old Item Serialization Vars
        /* DO NOT USE! Only used in serialization of special scrolls that originally derived from Item */
        private bool m_InheritsItem;

        protected bool InheritsItem
        {
            get
            {
                return this.m_InheritsItem;
            }
        }
        #endregion

        public abstract int Message { get; }
        public virtual int Title
        {
            get
            {
                return 0;
            }
        }
        public abstract string DefaultTitle { get; }

        public SpecialScroll(SkillName skill, double value)
            : base(0x14F0)
        {
            this.LootType = LootType.Cursed;
            this.Weight = 1.0;

            this.m_Skill = skill;
            this.m_Value = value;
        }

        public SpecialScroll(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SkillName Skill
        {
            get
            {
                return this.m_Skill;
            }
            set
            {
                this.m_Skill = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double Value
        {
            get
            {
                return this.m_Value;
            }
            set
            {
                this.m_Value = value;
            }
        }

        public virtual string GetNameLocalized()
        {
            return String.Concat("#", AosSkillBonuses.GetLabel(this.m_Skill).ToString());
        }

        public virtual string GetName()
        {
            int index = (int)this.m_Skill;
            SkillInfo[] table = SkillInfo.Table;

            if (index >= 0 && index < table.Length)
                return table[index].Name.ToLower();
            else
                return "???";
        }

        public virtual bool CanUse(Mobile from)
        {
            if (this.Deleted)
                return false;

            if (!this.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                return false;
            }

            return true;
        }

        public virtual void Use(Mobile from)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!this.CanUse(from))
                return;

            from.CloseGump(typeof(SpecialScroll.InternalGump));
            from.SendGump(new InternalGump(from, this));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write((int)this.m_Skill);
            writer.Write((double)this.m_Value);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                    {
                        this.m_Skill = (SkillName)reader.ReadInt();
                        this.m_Value = reader.ReadDouble();
                        break;
                    }
                case 0:
                    {
                        this.m_InheritsItem = true;

                        if (!(this is StatCapScroll))
                            this.m_Skill = (SkillName)reader.ReadInt();
                        else
                            this.m_Skill = SkillName.Alchemy;

                        if (this is ScrollofAlacrity)
                            this.m_Value = 0.0;
                        else if (this is StatCapScroll)
                            this.m_Value = (double)reader.ReadInt();
                        else
                            this.m_Value = reader.ReadDouble();

                        break;
                    }
            }
        }

        public class InternalGump : Gump
        {
            private readonly Mobile m_Mobile;
            private readonly SpecialScroll m_Scroll;

            public InternalGump(Mobile mobile, SpecialScroll scroll)
                : base(25, 50)
            {
                this.m_Mobile = mobile;
                this.m_Scroll = scroll;

                this.AddPage(0);

                this.AddBackground(25, 10, 420, 200, 5054);

                this.AddImageTiled(33, 20, 401, 181, 2624);
                this.AddAlphaRegion(33, 20, 401, 181);

                this.AddHtmlLocalized(40, 48, 387, 100, this.m_Scroll.Message, true, true);

                this.AddHtmlLocalized(125, 148, 200, 20, 1049478, 0xFFFFFF, false, false); // Do you wish to use this scroll?

                this.AddButton(100, 172, 4005, 4007, 1, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(135, 172, 120, 20, 1046362, 0xFFFFFF, false, false); // Yes

                this.AddButton(275, 172, 4005, 4007, 0, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(310, 172, 120, 20, 1046363, 0xFFFFFF, false, false); // No

                if (this.m_Scroll.Title != 0)
                    this.AddHtmlLocalized(40, 20, 260, 20, this.m_Scroll.Title, 0xFFFFFF, false, false);
                else
                    this.AddHtml(40, 20, 260, 20, this.m_Scroll.DefaultTitle, false, false);

                if (this.m_Scroll is StatCapScroll)
                    this.AddHtmlLocalized(310, 20, 120, 20, 1038019, 0xFFFFFF, false, false); // Power
                else
                    this.AddHtmlLocalized(310, 20, 120, 20, AosSkillBonuses.GetLabel(this.m_Scroll.Skill), 0xFFFFFF, false, false);
            }

            public override void OnResponse(NetState state, RelayInfo info)
            {
                if (info.ButtonID == 1)
                    this.m_Scroll.Use(this.m_Mobile);
            }
        }
    }
}