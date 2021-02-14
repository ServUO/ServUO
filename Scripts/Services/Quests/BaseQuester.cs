using Server.ContextMenus;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Engines.Quests
{
    public class TalkEntry : ContextMenuEntry
    {
        private readonly BaseQuester m_Quester;
        public TalkEntry(BaseQuester quester)
            : base(quester.TalkNumber)
        {
            m_Quester = quester;
        }

        public override void OnClick()
        {
            Mobile from = Owner.From;

            if (from.CheckAlive() && from is PlayerMobile pm && m_Quester.CanTalkTo(pm))
            {
                m_Quester.OnTalk(pm, true);
            }
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

        public override bool IsActiveVendor => false;
        public override bool IsInvulnerable => true;
        public override bool DisallowAllMoves => true;
        public override bool ClickTitle => false;
        public override bool CanTeach => false;
        public virtual int TalkNumber => 6146;// Talk
        protected override List<SBInfo> SBInfos => m_SBInfos;
        public static Container GetNewContainer()
        {
            Bag bag = new Bag
            {
                Hue = QuestSystem.RandomBrightHue()
            };
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

            if (from.Alive && from is PlayerMobile pm && TalkNumber > 0 && CanTalkTo(pm))
            {
                list.Add(new TalkEntry(this));
            }
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m.Alive && m is PlayerMobile pm)
            {
                int range = GetAutoTalkRange(pm);

                if (pm.Alive && range >= 0 && InRange(m, range) && !InRange(oldLocation, range) && CanTalkTo(pm))
                {
                    OnTalk(pm, false);
                }
            }
        }

        public void FocusTo(Mobile to)
        {
            QuestSystem.FocusTo(this, to);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }

        protected Item SetHue(Item item, int hue)
        {
            item.Hue = hue;
            return item;
        }
    }
}
