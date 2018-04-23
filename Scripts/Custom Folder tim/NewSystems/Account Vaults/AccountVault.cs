#region References

using System;
using System.Linq;
using Server.Network;

#endregion

namespace Server.Items
{
    public class AccountVault : BaseContainer
    {
        private string m_BoxAccount;
        private DateTime m_LastSummoned;

        [CommandProperty(AccessLevel.GameMaster)]
        public string BoxAccount
        {
            get { return m_BoxAccount; }
            set
            {
                m_BoxAccount = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime LastSummoned
        {
            get { return m_LastSummoned; }
            set { m_LastSummoned = value; InvalidateProperties(); }
        }

        [Constructable]
        public AccountVault(string account)
            : base(0x9AB)
        {
            m_BoxAccount = account;
            Movable = false;
            Visible = false;
            Name = "Account Vault";
            LiftOverride = true;
            Hue = 1157;
        }


        public AccountVault(Serial serial) : base(serial)
        {
        }

        public override bool IsAccessibleTo(Mobile m)
        {
            return BoxAccount == m.Account.ToString() || m.AccessLevel >= AccessLevel.GameMaster;
        }

        public override bool TryDropItem(Mobile from, Item dropped, bool sendFullMessage)
        {
            if (this.BoxAccount != @from.Account.ToString())
            {
                return false;
            }

            var list = this.Items;

            if (list.Any(item => !(item is Container) && item.StackWith(@from, dropped, false)))
            {
                return true;
            }

            DropItem(dropped);

            return true;
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            if (this.BoxAccount != @from.Account.ToString())
            {
                return false;
            }

            item.Location = new Point3D(p.X, p.Y, 0);
            AddItem(item);

            @from.SendSound(GetDroppedSound(item), GetWorldLocation());

            return true;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.BoxAccount == from.Account.ToString())
            {
                Open(from);
            }
            else
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
        }

        public virtual void DoContainerHideTimer(Item vault)
        {
            Timer.DelayCall(TimeSpan.FromSeconds(20.0), new TimerStateCallback(HideContainer_Callback), vault);
        }

        private static void HideContainer_Callback(object state)
        {
            HideContainer((Item) state);
        }

        private static void HideContainer(Item vault)
        {
            if (vault == null)
            {
                return;
            }

            vault.Visible = false;
            vault.Map = Map.Internal;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version

            writer.Write(m_BoxAccount);
            writer.Write(m_LastSummoned);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_BoxAccount = reader.ReadString();
            m_LastSummoned = reader.ReadDateTime();
        }
    }
}