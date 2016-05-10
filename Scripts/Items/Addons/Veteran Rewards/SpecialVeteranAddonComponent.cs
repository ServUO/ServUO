using Server.ContextMenus;
using Server.Engines.Craft;
using Server.Multis;
using System;
using System.Collections.Generic;

namespace Server.Items
{

    public class SpecialVeteranAddonComponent : AddonComponent
    {
        private SpecialVeteranCraftAddon m_Addon;

        [Constructable]
        public SpecialVeteranAddonComponent()
            : base(1)
        {
        }
        [Constructable]
        public SpecialVeteranAddonComponent(int itemID)
            : base(itemID)
        {
        }

        public SpecialVeteranAddonComponent(Serial serial)
            : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            if (SpAddon == null)
            {
                m_Addon = Addon as SpecialVeteranCraftAddon;
                if(!(this is SpecialVeteranAddonComponentBox))
                    SpAddon.RevalidateComponents();
            }
            if (SpAddon.ShowSecurity)
            {
                list.Add(SpAddon.Level.ToString());
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
            SetSecureLevelEntry.AddTo(from, SpAddon, list);
            SetPowerToolAnimationEntry.AddTo(from, SpAddon, list);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this, 2))
            {
                from.SendLocalizedMessage(501975);
            }
            else if (!from.CanSee(this))
            {
                from.SendLocalizedMessage(501977);
            }
            else
            {
                if (SpAddon.IsAccessibleTo(from))
                {
                    CraftSystem system = SpAddon.CraftSystem;

                    CraftContext context = system.GetContext(from);
                    from.SendGump(new CraftGump(from, system, Addon as IUsesRemaining, null));
                }
            }
        }

        public override int LabelNumber
        {
            get
            {
                return 1123577;
            }
        }// smithing press

        public virtual int[] ItemIDs
        {
            get { 
                return null;
            }
        }

        public void PlayAnimation(bool play)
        {
            if (SpAddon == null)
            {
                m_Addon = Addon as SpecialVeteranCraftAddon;
            }
            if ( ItemIDs.Length >= 4)
            {
                if (SpAddon.AddonDirection == Direction.East)
                    this.ItemID = play ? ItemIDs[2] : ItemIDs[0];
                else
                    this.ItemID = play ? ItemIDs[3] : ItemIDs[1];
            }
            else if (ItemIDs.Length >= 2)
            {
                if (SpAddon.AddonDirection == Direction.East)
                    this.ItemID = ItemIDs[0];
                else
                    this.ItemID = ItemIDs[1];
            }
        }

        public override bool IsAccessibleTo(Mobile check)
        {
            if (check == null  || !base.IsAccessibleTo(check))
            {
                return false;
            }

            if (this is SpecialVeteranAddonComponentBox && SpAddon.AlwaysAllowToolDrop)
            {
                return true;
            }

            bool accessable = false;
            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house == null)
            {
                return true;
            }

            switch (SpAddon.Level)
            {
                case SecureLevel.Owner:
                    accessable = house.IsOwner(check);
                    break;
                case SecureLevel.CoOwners:
                    accessable = house.IsCoOwner(check);
                    break;
                case SecureLevel.Friends:
                    accessable = house.IsFriend(check);
                    break;
                case SecureLevel.Guild:
                    accessable = house.IsGuildMember(check);
                    break;
                case SecureLevel.Anyone: return true;
            }

            if (!accessable)
            {
                LabelTo(check, 1061637);
                // Not sure what to use since I prefer not to use public message here
                // PublicOverheadMessage(Server.Network.MessageType.Label, 0x3E9, 1061637); // You are not allowed to access this.
            }

            return accessable;
        }

        public SpecialVeteranCraftAddon SpAddon
        {
            get
            {
                return m_Addon;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get
            {
                return SpAddon.UsesRemaining;
            }
            set
            {
                SpAddon.UsesRemaining = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ShowUsesRemaining
        {
            get
            {
                return SpAddon.ShowUsesRemaining;
            }
            set
            {
                SpAddon.ShowUsesRemaining = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ShowSecurity
        {
            get
            {
                return SpAddon.ShowSecurity;
            }
            set
            {
                SpAddon.ShowSecurity = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ShowAnimation
        {
            get
            {
                return SpAddon.ShowAnimation;
            }
            set
            {
                SpAddon.ShowAnimation = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool AlwaysAllowToolDrop
        {
            get
            {
                return SpAddon.AlwaysAllowToolDrop;
            }
            set
            {
                SpAddon.AlwaysAllowToolDrop = value;
            }
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
    }
}
