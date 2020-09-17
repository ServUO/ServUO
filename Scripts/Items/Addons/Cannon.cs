using Server.Engines.Quests.Haven;
using Server.Engines.VeteranRewards;
using Server.Gumps;
using Server.Network;
using Server.Targeting;
using System;

namespace Server.Items
{
    public class CannonAddonComponent : AddonComponent
    {
        public CannonAddonComponent(int itemID)
            : base(itemID)
        {
            LootType = LootType.Blessed;
        }

        public CannonAddonComponent(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1076157;// Decorative Cannon
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (Addon is CannonAddon)
            {
                if (((CannonAddon)Addon).IsRewardItem)
                    list.Add(1076223); // 7th Year Veteran Reward

                list.Add(1076207, ((CannonAddon)Addon).Charges.ToString()); // Remaining Charges: ~1_val~
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class CannonAddon : BaseAddon
    {
        private static readonly int[] m_Effects = new int[]
        {
            0x36B0, 0x3728, 0x3709, 0x36FE
        };
        private CannonDirection m_CannonDirection;
        private int m_Charges;
        private bool m_IsRewardItem;
        [Constructable]
        public CannonAddon(CannonDirection direction)
        {
            m_CannonDirection = direction;

            switch (direction)
            {
                case CannonDirection.North:
                    {
                        AddComponent(new CannonAddonComponent(0xE8D), 0, 0, 0);
                        AddComponent(new CannonAddonComponent(0xE8C), 0, 1, 0);
                        AddComponent(new CannonAddonComponent(0xE8B), 0, 2, 0);

                        break;
                    }
                case CannonDirection.East:
                    {
                        AddComponent(new CannonAddonComponent(0xE96), 0, 0, 0);
                        AddComponent(new CannonAddonComponent(0xE95), -1, 0, 0);
                        AddComponent(new CannonAddonComponent(0xE94), -2, 0, 0);

                        break;
                    }
                case CannonDirection.South:
                    {
                        AddComponent(new CannonAddonComponent(0xE91), 0, 0, 0);
                        AddComponent(new CannonAddonComponent(0xE92), 0, -1, 0);
                        AddComponent(new CannonAddonComponent(0xE93), 0, -2, 0);

                        break;
                    }
                default:
                    {
                        AddComponent(new CannonAddonComponent(0xE8E), 0, 0, 0);
                        AddComponent(new CannonAddonComponent(0xE8F), 1, 0, 0);
                        AddComponent(new CannonAddonComponent(0xE90), 2, 0, 0);

                        break;
                    }
            }
        }

        public CannonAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                CannonDeed deed = new CannonDeed
                {
                    Charges = m_Charges,
                    IsRewardItem = m_IsRewardItem
                };

                return deed;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public CannonDirection CannonDirection => m_CannonDirection;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get
            {
                return m_Charges;
            }
            set
            {
                m_Charges = value;

                foreach (AddonComponent c in Components)
                    c.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get
            {
                return m_IsRewardItem;
            }
            set
            {
                m_IsRewardItem = value;

                foreach (AddonComponent c in Components)
                    c.InvalidateProperties();
            }
        }
        public override void OnComponentUsed(AddonComponent c, Mobile from)
        {
            if (from.InRange(Location, 2))
            {
                if (m_Charges > 0)
                {
                    from.Target = new InternalTarget(this);
                }
                else
                {
                    if (from.Backpack != null)
                    {
                        PotionKeg keg = from.Backpack.FindItemByType(typeof(PotionKeg)) as PotionKeg;

                        if (Validate(keg) > 0)
                            from.SendGump(new InternalGump(this, keg));
                        else
                            from.SendLocalizedMessage(1076198); // You do not have a full keg of explosion potions needed to recharge the cannon.
                    }
                }
            }
            else
                from.SendLocalizedMessage(1076766); // That is too far away.
        }

        public int Validate(PotionKeg keg)
        {
            if (keg != null && !keg.Deleted && keg.Held == 100)
            {
                if (keg.Type == PotionEffect.ExplosionLesser)
                    return 5;
                else if (keg.Type == PotionEffect.Explosion)
                    return 10;
                else if (keg.Type == PotionEffect.ExplosionGreater)
                    return 15;
            }

            return 0;
        }

        public void Fill(Mobile from, PotionKeg keg)
        {
            Charges = Validate(keg);

            if (Charges > 0)
            {
                keg.Delete();
                from.SendLocalizedMessage(1076199); // Your cannon is recharged.
            }
            else
                from.SendLocalizedMessage(1076198); // You do not have a full keg of explosion potions needed to recharge the cannon.
        }

        public void DoFireEffect(IPoint3D target)
        {
            Map map = Map;

            if (target == null || map == null)
                return;

            Effects.PlaySound(target, map, Utility.RandomList(0x11B, 0x11C, 0x11D));
            Effects.SendLocationEffect(target, map, Utility.RandomList(m_Effects), 16, 1);

            for (int count = Utility.Random(3); count > 0; count--)
            {
                IPoint3D location = new Point3D(target.X + Utility.RandomMinMax(-1, 1), target.Y + Utility.RandomMinMax(-1, 1), target.Z);
                int effect = Utility.RandomList(m_Effects);
                Effects.SendLocationEffect(location, map, effect, 16, 1);
            }

            Charges -= 1;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.Write((int)m_CannonDirection);
            writer.Write(m_Charges);
            writer.Write(m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            m_CannonDirection = (CannonDirection)reader.ReadInt();
            m_Charges = reader.ReadInt();
            m_IsRewardItem = reader.ReadBool();
        }

        private class InternalTarget : Target
        {
            private readonly CannonAddon m_Cannon;
            public InternalTarget(CannonAddon cannon)
                : base(12, true, TargetFlags.None)
            {
                m_Cannon = cannon;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Cannon == null || m_Cannon.Deleted)
                    return;

                IPoint3D p = targeted as IPoint3D;

                if (p == null)
                    return;

                if (from.InLOS(new Point3D(p)))
                {
                    if (!Utility.InRange(new Point3D(p), m_Cannon.Location, 2))
                    {
                        bool allow = false;

                        int x = p.X - m_Cannon.X;
                        int y = p.Y - m_Cannon.Y;

                        switch (m_Cannon.CannonDirection)
                        {
                            case CannonDirection.North:
                                if (y < 0 && Math.Abs(x) <= -y / 3)
                                    allow = true;

                                break;
                            case CannonDirection.East:
                                if (x > 0 && Math.Abs(y) <= x / 3)
                                    allow = true;

                                break;
                            case CannonDirection.South:
                                if (y > 0 && Math.Abs(x) <= y / 3)
                                    allow = true;

                                break;
                            case CannonDirection.West:
                                if (x < 0 && Math.Abs(y) <= -x / 3)
                                    allow = true;

                                break;
                        }

                        if (allow && Utility.InRange(new Point3D(p), m_Cannon.Location, 14))
                            m_Cannon.DoFireEffect(p);
                        else
                            from.SendLocalizedMessage(1076203); // Target out of range.							
                    }
                    else
                        from.SendLocalizedMessage(1076215); // Cannon must be aimed farther away.
                }
                else
                    from.SendLocalizedMessage(1049630); // You cannot see that target!		
            }

            protected override void OnTargetOutOfRange(Mobile from, object targeted)
            {
                from.SendLocalizedMessage(1076203); // Target out of range.
            }
        }

        private class InternalGump : Gump
        {
            private readonly CannonAddon m_Cannon;
            private readonly PotionKeg m_Keg;
            public InternalGump(CannonAddon cannon, PotionKeg keg)
                : base(50, 50)
            {
                m_Cannon = cannon;
                m_Keg = keg;

                Closable = true;
                Disposable = true;
                Dragable = true;
                Resizable = false;

                AddPage(0);

                AddBackground(0, 0, 291, 133, 0x13BE);
                AddImageTiled(5, 5, 280, 100, 0xA40);

                AddHtmlLocalized(9, 9, 272, 100, 1076196, cannon.Validate(keg).ToString(), 0x7FFF, false, false); // You will need a full keg of explosion potions to recharge the cannon.  Your keg will provide ~1_CHARGES~ charges.

                AddButton(5, 107, 0xFB1, 0xFB2, (int)Buttons.Cancel, GumpButtonType.Reply, 0);
                AddHtmlLocalized(40, 109, 100, 20, 1060051, 0x7FFF, false, false); // CANCEL

                AddButton(160, 107, 0xFB7, 0xFB8, (int)Buttons.Recharge, GumpButtonType.Reply, 0);
                AddHtmlLocalized(195, 109, 120, 20, 1076197, 0x7FFF, false, false); // Recharge
            }

            private enum Buttons
            {
                Cancel,
                Recharge
            }
            public override void OnResponse(NetState state, RelayInfo info)
            {
                if (m_Cannon == null || m_Cannon.Deleted)
                    return;

                if (info.ButtonID == (int)Buttons.Recharge)
                    m_Cannon.Fill(state.Mobile, m_Keg);
            }
        }
    }

    public class CannonDeed : BaseAddonDeed, IRewardItem, IRewardOption
    {
        private CannonDirection m_Direction;
        private int m_Charges;
        private bool m_IsRewardItem;
        [Constructable]
        public CannonDeed()
            : base()
        {
            LootType = LootType.Blessed;
        }

        public CannonDeed(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1076195;// A deed for a cannon
        public override BaseAddon Addon
        {
            get
            {
                CannonAddon addon = new CannonAddon(m_Direction)
                {
                    Charges = m_Charges,
                    IsRewardItem = m_IsRewardItem
                };

                return addon;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get
            {
                return m_Charges;
            }
            set
            {
                m_Charges = value;
                InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get
            {
                return m_IsRewardItem;
            }
            set
            {
                m_IsRewardItem = value;
                InvalidateProperties();
            }
        }
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_IsRewardItem)
                list.Add(1076223); // 7th Year Veteran Reward

            list.Add(1076207, m_Charges.ToString()); // Remaining Charges: ~1_val~
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_IsRewardItem && !RewardSystem.CheckIsUsableBy(from, this, null))
                return;

            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(RewardOptionGump));
                from.SendGump(new RewardOptionGump(this));
            }
            else
                from.SendLocalizedMessage(1042038); // You must have the object in your backpack to use it.          	
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.Write(m_Charges);
            writer.Write(m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            m_Charges = reader.ReadInt();
            m_IsRewardItem = reader.ReadBool();
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add((int)CannonDirection.South, 1075386); // South
            list.Add((int)CannonDirection.East, 1075387); // East
            list.Add((int)CannonDirection.North, 1075389); // North
            list.Add((int)CannonDirection.West, 1075390); // West
        }

        public void OnOptionSelected(Mobile from, int option)
        {
            m_Direction = (CannonDirection)option;

            if (!Deleted)
                base.OnDoubleClick(from);
        }
    }
}