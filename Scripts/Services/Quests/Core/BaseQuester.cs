using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class TalkEntry : ContextMenuEntry
    {
        private readonly BaseQuester m_Quester;
        public TalkEntry(BaseQuester quester)
            : base(quester.TalkNumber)
        {
            this.m_Quester = quester;
        }

        public override void OnClick()
        {
            Mobile from = this.Owner.From;

            if (from.CheckAlive() && from is PlayerMobile && this.m_Quester.CanTalkTo((PlayerMobile)from))
                this.m_Quester.OnTalk((PlayerMobile)from, true);
        }
    }

    public abstract class BaseQuester : BaseVendor
    {
        protected List<SBInfo> m_SBInfos = new List<SBInfo>();
        public BaseQuester()
            : this(null)
        {
        }

        public BaseQuester(string title)
            : base(title)
        {
        }

        public BaseQuester(Serial serial)
            : base(serial)
        {
        }

        public override void CheckMorph()
        {
            // Don't morph me!
        }

        public override bool IsActiveVendor
        {
            get
            {
                return false;
            }
        }
        public override bool IsInvulnerable
        {
            get
            {
                return true;
            }
        }
        public override bool DisallowAllMoves
        {
            get
            {
                return true;
            }
        }
        public override bool ClickTitle
        {
            get
            {
                return false;
            }
        }
        public override bool CanTeach
        {
            get
            {
                return false;
            }
        }
        public virtual int TalkNumber
        {
            get
            {
                return 6146;
            }
        }// Talk
        protected override List<SBInfo> SBInfos
        {
            get
            {
                return this.m_SBInfos;
            }
        }
        public static Container GetNewContainer()
        {
            Bag bag = new Bag();
            bag.Hue = QuestSystem.RandomBrightHue();
            return bag;
        }

        public override void InitSBInfo()
        {
        }

        public abstract void OnTalk(PlayerMobile player, bool contextMenu);

        public virtual bool CanTalkTo(PlayerMobile to)
        {
            return true;
        }

        public virtual int GetAutoTalkRange(PlayerMobile m)
        {
            return -1;
        }

        public override bool CanBeDamaged()
        {
            return false;
        }

        public override void AddCustomContextEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.AddCustomContextEntries(from, list);

            if (from.Alive && from is PlayerMobile && this.TalkNumber > 0 && this.CanTalkTo((PlayerMobile)from))
                list.Add(new TalkEntry(this));
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m.Alive && m is PlayerMobile)
            {
                PlayerMobile pm = (PlayerMobile)m;

                int range = this.GetAutoTalkRange(pm);

                if (m.Alive && range >= 0 && this.InRange(m, range) && !this.InRange(oldLocation, range) && this.CanTalkTo(pm))
                    this.OnTalk(pm, false);
            }
        }

        public void FocusTo(Mobile to)
        {
            QuestSystem.FocusTo(this, to);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        protected Item SetHue(Item item, int hue)
        {
            item.Hue = hue;
            return item;
        }
    }
}