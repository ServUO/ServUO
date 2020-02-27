using System;
using System.Linq;
using Server.Gumps;
using Server.Mobiles;
using Server.Multis;
using Server.ContextMenus;
using System.Collections.Generic;

namespace Server.Items
{
    public class TeleporterComponent : AddonComponent
    {
        public override bool ForceShowProperties { get { return true; } }

        public TeleporterComponent(int itemID)
            : base(itemID)
        {
        }

        public TeleporterComponent(Serial serial)
            : base(serial)
        {
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            SetSecureLevelEntry.AddTo(from, Addon, list);
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (m is PlayerMobile && ((SpiralStaircaseAddon)Addon).CheckAccessible(m, this))
            {
                Timer.DelayCall(TimeSpan.FromSeconds(0.5), () => DoTeleport(m));
            }

            return base.OnMoveOver(m);
        }

        public virtual void DoTeleport(Mobile m)
        {
            Map map = Map;

            if (map == null || map == Map.Internal)
            {
                map = m.Map;
            }

            Point3D p = Addon.Components.FirstOrDefault(x => x != this).Location;

            if (p == Point3D.Zero)
            {
                p = m.Location;
            }

            BaseCreature.TeleportPets(m, p, map);

            m.MoveToWorld(new Point3D(p.X, p.Y, p.Z + 20), map);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class SpiralStaircaseAddon : BaseAddon, ISecurable, IDyable
    {
        public override BaseAddonDeed Deed { get { return new SpiralStaircaseDeed(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level { get; set; }

        [Constructable]
        public SpiralStaircaseAddon(bool topper)
        {
            if (topper)
            {
                AddComponent(new AddonComponent(42319), 0, 1, 0);
                AddComponent(new AddonComponent(42320), 1, 0, 0);
                AddComponent(new AddonComponent(42333), 1, 1, 20);
                AddComponent(new AddonComponent(42334), 0, 1, 20);
                AddComponent(new AddonComponent(42335), 1, 0, 20);
                AddComponent(new TeleporterComponent(42336), 0, 0, 20);
                AddComponent(new TeleporterComponent(42318), 1, 1, 0);
            }
            else
            {
                AddComponent(new TeleporterComponent(42321), 0, 0, 0);
                AddComponent(new AddonComponent(42319), 0, 1, 0);
                AddComponent(new AddonComponent(42320), 1, 0, 0);
                AddComponent(new TeleporterComponent(42318), 1, 1, 0);
            }
        }

        public virtual bool Dye(Mobile from, DyeTub sender)
        {
            if (Deleted)
                return false;

            Hue = sender.DyedHue;
            return true;
        }

        public bool CheckAccessible(Mobile from, Item item)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
                return true; // Staff can access anything

            BaseHouse house = BaseHouse.FindHouseAt(item);

            if (house == null)
                return false;

            switch (Level)
            {
                case SecureLevel.Owner: return house.IsOwner(from);
                case SecureLevel.CoOwners: return house.IsCoOwner(from);
                case SecureLevel.Friends: return house.IsFriend(from);
                case SecureLevel.Anyone: return true;
                case SecureLevel.Guild: return house.IsGuildMember(from);
            }

            return false;
        }

        public SpiralStaircaseAddon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version

            writer.Write((int)Level);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Level = (SecureLevel)reader.ReadInt();
        }
    }

    public class SpiralStaircaseDeed : BaseAddonDeed, IRewardOption
    {
        public override BaseAddon Addon { get { return new SpiralStaircaseAddon(m_Topper); } }
        public override int LabelNumber { get { return 1159429; } } // spiral staircase

        private bool m_Topper;

        [Constructable]
        public SpiralStaircaseDeed()
        {
            LootType = LootType.Blessed;
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add(0, 1159431); // Without Topper
            list.Add(1, 1159432); // With Topper
        }

        public void OnOptionSelected(Mobile from, int choice)
        {
            m_Topper = choice == 1;

            if (!Deleted)
                base.OnDoubleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(AddonOptionGump));
                from.SendGump(new AddonOptionGump(this, 1159430, 300, 180)); // Choose an option:
            }
            else
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.       	
        }

        public SpiralStaircaseDeed(Serial serial)
            : base(serial)
        {
        }

        private void SendTarget(Mobile m)
        {
            base.OnDoubleClick(m);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
