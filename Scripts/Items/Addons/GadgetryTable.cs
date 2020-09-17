using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Engines.VeteranRewards
{
    public class GadgetryTableComponent : LocalizedAddonComponent
    {
        public override bool ForceShowProperties => true;

        public GadgetryTableComponent(int id)
            : base(id, 1098558) // gadgetry table
        {
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            SetSecureLevelEntry.AddTo(from, Addon, list);
        }

        public GadgetryTableComponent(Serial serial)
            : base(serial)
        {
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

    public class GadgetryTableAddon : BaseAddon, ISecurable
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem { get; set; }

        public override BaseAddonDeed Deed
        {
            get
            {
                GadgetryTableAddonDeed deed = new GadgetryTableAddonDeed
                {
                    IsRewardItem = IsRewardItem
                };

                return deed;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextGolemTime { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level { get; set; }

        [Constructable]
        public GadgetryTableAddon(DirectionType type)
        {
            NextGolemTime = DateTime.UtcNow + TimeSpan.FromDays(1);

            switch (type)
            {
                case DirectionType.South:
                    AddComponent(new GadgetryTableComponent(20372), 1, 0, 0);
                    AddComponent(new GadgetryTableComponent(20362), 0, 0, 0);
                    AddComponent(new GadgetryTableComponent(20352), -1, 0, 0);
                    AddComponent(new GadgetryTableComponent(19687), -1, 1, 0);
                    break;
                case DirectionType.East:
                    AddComponent(new GadgetryTableComponent(19692), 1, 1, 0);
                    AddComponent(new GadgetryTableComponent(20382), 0, 1, 0);
                    AddComponent(new GadgetryTableComponent(20392), 0, 0, 0);
                    AddComponent(new GadgetryTableComponent(20402), 0, -1, 0);
                    break;
            }
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

        public double Scalar(Mobile m)
        {
            double scalar;

            double skill = m.Skills[SkillName.Tinkering].Value;

            if (skill >= 100.0)
                scalar = 1.0;
            else if (skill >= 90.0)
                scalar = 0.9;
            else if (skill >= 80.0)
                scalar = 0.8;
            else if (skill >= 70.0)
                scalar = 0.7;
            else
                scalar = 0.6;

            return scalar;
        }

        public override void OnComponentUsed(AddonComponent c, Mobile from)
        {
            if (from.InRange(c.Location, 3))
            {
                BaseHouse house = BaseHouse.FindHouseAt(from);

                if (house != null)
                {
                    if (CheckAccessible(from, this))
                    {
                        if ((from.Followers + 4) > from.FollowersMax)
                        {
                            from.SendLocalizedMessage(1049607); // You have too many followers to control that creature.
                        }
                        else if (((PlayerMobile)from).AllFollowers.Any(x => x is GadgetryTableGolem))
                        {
                            from.SendLocalizedMessage(1154576); // You cannot build another golem at this time because you are currently in control of one.
                        }
                        else if (from.Stabled.Any(x => x is GadgetryTableGolem))
                        {
                            from.SendLocalizedMessage(1154577); // You cannot build another golem at this time because you have one in the stables.
                        }
                        else if (DateTime.UtcNow > NextGolemTime)
                        {
                            Golem g = new GadgetryTableGolem(Scalar(from));

                            if (g.SetControlMaster(from))
                            {
                                NextGolemTime = DateTime.UtcNow + TimeSpan.FromDays(1);
                                g.MoveToWorld(from.Location, from.Map);
                                from.PlaySound(0x241);
                            }
                        }
                        else
                        {
                            from.SendLocalizedMessage(1154578); // A golem can only be built once every 24 hours.
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage(1061637); // You are not allowed to access this.
                    }
                }
                else
                {
                    from.SendLocalizedMessage(502092); // You must be in your house to do this.
                }
            }
            else
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
        }


        public GadgetryTableAddon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // Version

            writer.Write(NextGolemTime);
            writer.Write((int)Level);
            writer.Write(IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        NextGolemTime = reader.ReadDateTime();
                        Level = (SecureLevel)reader.ReadInt();
                        IsRewardItem = reader.ReadBool();
                        break;
                    }
                case 0:
                    {
                        IsRewardItem = reader.ReadBool();
                        break;
                    }
            }
        }
    }

    public class GadgetryTableAddonDeed : BaseAddonDeed, IRewardItem, IRewardOption
    {
        public override int LabelNumber => 1154583;  // Deed for a Gadgetry Table

        public override BaseAddon Addon
        {
            get
            {
                GadgetryTableAddon addon = new GadgetryTableAddon(_Direction)
                {
                    IsRewardItem = m_IsRewardItem
                };

                return addon;
            }
        }

        private DirectionType _Direction;

        private bool m_IsRewardItem;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get { return m_IsRewardItem; }
            set
            {
                m_IsRewardItem = value;
                InvalidateProperties();
            }
        }

        [Constructable]
        public GadgetryTableAddonDeed()
            : base()
        {
            LootType = LootType.Blessed;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_IsRewardItem)
                list.Add(1076224); // 8th Year Veteran Reward		
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(AddonOptionGump));
                from.SendGump(new AddonOptionGump(this, 1154194)); // Choose a Facing:
            }
            else
            {
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
            }
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add((int)DirectionType.South, 1075386); // South
            list.Add((int)DirectionType.East, 1075387); // East
        }

        public void OnOptionSelected(Mobile from, int choice)
        {
            _Direction = (DirectionType)choice;

            if (!Deleted)
                base.OnDoubleClick(from);
        }

        public GadgetryTableAddonDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version

            writer.Write(m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_IsRewardItem = reader.ReadBool();
        }
    }

    [CorpseName("a golem corpse")]
    public class GadgetryTableGolem : Golem, IRepairableMobile
    {
        [Constructable]
        public GadgetryTableGolem(double scalar)
            : base(true, 0)
        {
            Name = "a golem";
            Body = 752;
            Hue = 1110;

            SetStr((int)(550 * scalar), (int)(600 * scalar));
            SetDex((int)(125 * scalar), (int)(150 * scalar));
            SetInt((int)(200 * scalar), (int)(225 * scalar));

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 40, 60);
            SetResistance(ResistanceType.Fire, 50, 65);
            SetResistance(ResistanceType.Cold, 20, 30);
            SetResistance(ResistanceType.Poison, 75, 85);
            SetResistance(ResistanceType.Energy, 30, 45);

            SetSkill(SkillName.MagicResist, 190.0);
            SetSkill(SkillName.Tactics, 100.0);
            SetSkill(SkillName.Wrestling, 100.0);
            SetSkill(SkillName.Parry, 100.0);
            SetSkill(SkillName.DetectHidden, 48.4);

            SetDamage(13, 24);

            Fame = 10;
            Karma = 10;

            ControlSlots = 3;
        }

        public override bool CanTransfer(Mobile m)
        {
            PrivateOverheadMessage(MessageType.Regular, 0x3B2, 502036, m.NetState); // I cannot be transferred

            return false;
        }

        public GadgetryTableGolem(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
