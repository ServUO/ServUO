using System;
using Server.Network;
using Server.Prompts;
using Server.Targeting;
using Server.Engines.Craft;

namespace Server.Items
{
    public enum KeyType
    {
        Copper = 0x100E,
        Gold = 0x100F,
        Iron = 0x1010,
        Rusty = 0x1013
    }

    public interface ILockable
    {
        bool Locked { get; set; }
        uint KeyValue { get; set; }
    }

    public class Key : Item, IResource
    {
        private string m_Description;
        private uint m_KeyVal;
        private Item m_Link;
        private int m_MaxRange;

        private CraftResource _Resource;
        private Mobile _Crafter;
        private ItemQuality _Quality;

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource { get { return _Resource; } set { _Resource = value; _Resource = value; Hue = CraftResources.GetHue(_Resource); InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Crafter { get { return _Crafter; } set { _Crafter = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public ItemQuality Quality { get { return _Quality; } set { _Quality = value; InvalidateProperties(); } }

        public bool PlayerConstructed { get { return true; } }

        [Constructable]
        public Key()
            : this(KeyType.Iron, 0)
        {
        }

        [Constructable]
        public Key(KeyType type)
            : this(type, 0)
        {
        }

        [Constructable]
        public Key(uint val)
            : this(KeyType.Iron, val)
        {
        }

        [Constructable]
        public Key(KeyType type, uint LockVal)
            : this(type, LockVal, null)
        {
            m_KeyVal = LockVal;
        }

        public Key(KeyType type, uint LockVal, Item link)
            : base((int)type)
        {
            Weight = 1.0;

            m_MaxRange = 3;
            m_KeyVal = LockVal;
            m_Link = link;
        }

        public Key(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Description
        {
            get
            {
                return m_Description;
            }
            set
            {
                m_Description = value;
                InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxRange
        {
            get
            {
                return m_MaxRange;
            }

            set
            {
                m_MaxRange = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public uint KeyValue
        {
            get
            {
                return m_KeyVal;
            }

            set
            {
                m_KeyVal = value;
                InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Item Link
        {
            get
            {
                return m_Link;
            }

            set
            {
                m_Link = value;
            }
        }
        public static uint RandomValue()
        {
            return (uint)(0xFFFFFFFE * Utility.RandomDouble()) + 1;
        }

        public static void RemoveKeys(Mobile m, uint keyValue)
        {
            if (keyValue == 0)
                return;

            RemoveKeys(m.Backpack, keyValue);
            RemoveKeys(m.BankBox, keyValue);
        }

        public static void RemoveKeys(Container cont, uint keyValue)
        {
            if (cont == null || keyValue == 0)
                return;

            Item[] items = cont.FindItemsByType(new Type[] { typeof(Key), typeof(KeyRing) });

            foreach (Item item in items)
            {
                if (item is Key)
                {
                    Key key = (Key)item;

                    if (key.KeyValue == keyValue)
                        key.Delete();
                }
                else
                {
                    KeyRing keyRing = (KeyRing)item;

                    keyRing.RemoveKeys(keyValue);
                }
            }
        }

        public static bool ContainsKey(Container cont, uint keyValue)
        {
            if (cont == null)
                return false;

            Item[] items = cont.FindItemsByType(new Type[] { typeof(Key), typeof(KeyRing) });

            foreach (Item item in items)
            {
                if (item is Key)
                {
                    Key key = (Key)item;

                    if (key.KeyValue == keyValue)
                        return true;
                }
                else
                {
                    KeyRing keyRing = (KeyRing)item;

                    if (keyRing.ContainsKey(keyValue))
                        return true;
                }
            }

            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)3); // version

            writer.Write((int)_Resource);
            writer.Write(_Crafter);
            writer.Write((int)_Quality);

            writer.Write((int)m_MaxRange);

            writer.Write((Item)m_Link);

            writer.Write((string)m_Description);
            writer.Write((uint)m_KeyVal);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 3:
                    {
                        _Resource = (CraftResource)reader.ReadInt();
                        _Crafter = reader.ReadMobile();
                        _Quality = (ItemQuality)reader.ReadInt();

                        goto case 2;
                    }
                case 2:
                    {
                        m_MaxRange = reader.ReadInt();

                        goto case 1;
                    }
                case 1:
                    {
                        m_Link = reader.ReadItem();

                        goto case 0;
                    }
                case 0:
                    {
                        if (version < 2 || m_MaxRange == 0)
                            m_MaxRange = 3;

                        m_Description = reader.ReadString();

                        m_KeyVal = reader.ReadUInt();

                        break;
                    }
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(501661); // That key is unreachable.
                return;
            }

            Target t;
            int number;

            if (m_KeyVal != 0)
            {
                number = 501662; // What shall I use this key on?
                t = new UnlockTarget(this);
            }
            else
            {
                number = 501663; // This key is a key blank. Which key would you like to make a copy of?
                t = new CopyTarget(this);
            }

            from.SendLocalizedMessage(number);
            from.Target = t;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            string desc;

            if (m_KeyVal == 0)
                desc = "(blank)";
            else if ((desc = m_Description) == null || (desc = desc.Trim()).Length <= 0)
                desc = null;

            if (desc != null)
                list.Add(desc);

            if (_Crafter != null)
            {
                list.Add(1050043, _Crafter.TitleName); // crafted by ~1_NAME~
            }

            if (_Quality == ItemQuality.Exceptional)
            {
                list.Add(1060636); // Exceptional
            }
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (_Resource > CraftResource.Iron)
            {
                list.Add(1053099, "#{0}\t{1}", CraftResources.GetLocalizationNumber(_Resource), String.Format("#{0}", LabelNumber.ToString())); // ~1_oretype~ ~2_armortype~
            }
            else
            {
                base.AddNameProperty(list);
            }
        }

        public virtual int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            Quality = (ItemQuality)quality;

            if (makersMark)
                Crafter = from;

            if (!craftItem.ForceNonExceptional)
            {
                if (typeRes == null)
                    typeRes = craftItem.Resources.GetAt(0).ItemType;

                Resource = CraftResources.GetFromType(typeRes);
            }

            return quality;
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            string desc;

            if (m_KeyVal == 0)
                desc = "(blank)";
            else if ((desc = m_Description) == null || (desc = desc.Trim()).Length <= 0)
                desc = "";

            if (desc.Length > 0)
                from.Send(new UnicodeMessage(Serial, ItemID, MessageType.Regular, 0x3B2, 3, "ENU", "", desc));
        }

        public bool UseOn(Mobile from, ILockable o)
        {
            if (o.KeyValue == KeyValue)
            {
                if (o is BaseDoor && !((BaseDoor)o).UseLocks())
                {
                    return false;
                }
                else
                {
                    o.Locked = !o.Locked;

                    if (o is LockableContainer)
                    {
                        LockableContainer cont = (LockableContainer)o;

                        if (cont.LockLevel == -255)
                            cont.LockLevel = cont.RequiredSkill - 10;
                    }

                    if (o is Item)
                    {
                        Item item = (Item)o;

                        if (o.Locked)
                            item.SendLocalizedMessageTo(from, 1048000); // You lock it.
                        else
                            item.SendLocalizedMessageTo(from, 1048001); // You unlock it.

                        if (item is LockableContainer)
                        {
                            LockableContainer cont = (LockableContainer)item;

                            if (cont.TrapType != TrapType.None && cont.TrapOnLockpick)
                            {
                                if (o.Locked)
                                    item.SendLocalizedMessageTo(from, 501673); // You re-enable the trap.
                                else
                                    item.SendLocalizedMessageTo(from, 501672); // You disable the trap temporarily.  Lock it again to re-enable it.
                            }
                        }
                    }

                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        private class RenamePrompt : Prompt
        {
            public override int MessageCliloc { get { return 501665; } }
            private readonly Key m_Key;
            public RenamePrompt(Key key)
            {
                m_Key = key;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (m_Key.Deleted || !m_Key.IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(501661); // That key is unreachable.
                    return;
                }

                m_Key.Description = Utility.FixHtml(text);
            }
        }

        private class UnlockTarget : Target
        {
            private readonly Key m_Key;
            public UnlockTarget(Key key)
                : base(key.MaxRange, false, TargetFlags.None)
            {
                m_Key = key;
                CheckLOS = false;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Key.Deleted || !m_Key.IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(501661); // That key is unreachable.
                    return;
                }

                int number;

                if (targeted == m_Key)
                {
                    number = 501665; // Enter a description for this key.

                    from.Prompt = new RenamePrompt(m_Key);
                }
                else if (targeted is ILockable)
                {
                    if (m_Key.UseOn(from, (ILockable)targeted))
                        number = -1;
                    else
                        number = 501668; // This key doesn't seem to unlock that.
                }
                else
                {
                    number = 501666; // You can't unlock that!
                }

                if (number != -1)
                {
                    from.SendLocalizedMessage(number);
                }
            }
        }

        private class CopyTarget : Target
        {
            private readonly Key m_Key;
            public CopyTarget(Key key)
                : base(3, false, TargetFlags.None)
            {
                m_Key = key;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Key.Deleted || !m_Key.IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(501661); // That key is unreachable.
                    return;
                }

                int number;

                if (targeted is Key)
                {
                    Key k = (Key)targeted;

                    if (k.m_KeyVal == 0)
                    {
                        number = 501675; // This key is also blank.
                    }
                    else if (from.CheckTargetSkill(SkillName.Tinkering, k, 0, 75.0))
                    {
                        number = 501676; // You make a copy of the key.

                        m_Key.Description = k.Description;
                        m_Key.KeyValue = k.KeyValue;
                        m_Key.Link = k.Link;
                        m_Key.MaxRange = k.MaxRange;
                    }
                    else if (Utility.RandomDouble() <= 0.1) // 10% chance to destroy the key
                    {
                        from.SendLocalizedMessage(501677); // You fail to make a copy of the key.

                        number = 501678; // The key was destroyed in the attempt.

                        m_Key.Delete();
                    }
                    else
                    {
                        number = 501677; // You fail to make a copy of the key.
                    }
                }
                else
                {
                    number = 501688; // Not a key.
                }

                from.SendLocalizedMessage(number);
            }
        }
    }
}