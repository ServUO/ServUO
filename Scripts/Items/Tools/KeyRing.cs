using Server.Engines.Craft;
using Server.Targeting;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class KeyRing : Item, IResource, IQuality
    {
        private CraftResource _Resource;
        private Mobile _Crafter;
        private ItemQuality _Quality;

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource { get { return _Resource; } set { _Resource = value; _Resource = value; Hue = CraftResources.GetHue(_Resource); InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Crafter { get { return _Crafter; } set { _Crafter = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public ItemQuality Quality { get { return _Quality; } set { _Quality = value; InvalidateProperties(); } }

        public bool PlayerConstructed => true;

        public static readonly int MaxKeys = 20;
        private List<Key> m_Keys;

        [Constructable]
        public KeyRing()
            : base(0x1011)
        {
            Weight = 1.0; // They seem to have no weight on OSI ?!

            m_Keys = new List<Key>();
        }

        public KeyRing(Serial serial)
            : base(serial)
        {
        }

        public List<Key> Keys => m_Keys;
        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1060640); // The item must be in your backpack to use it.
                return false;
            }

            Key key = dropped as Key;

            if (key == null || key.KeyValue == 0)
            {
                from.SendLocalizedMessage(501689); // Only non-blank keys can be put on a keyring.
                return false;
            }
            else if (Keys.Count >= MaxKeys)
            {
                from.SendLocalizedMessage(1008138); // This keyring is full.
                return false;
            }
            else
            {
                Add(key);
                from.SendLocalizedMessage(501691); // You put the key on the keyring.
                return true;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1060640); // The item must be in your backpack to use it.
                return;
            }

            from.SendLocalizedMessage(501680); // What do you want to unlock?
            from.Target = new InternalTarget(this);
        }

        public override void OnDelete()
        {
            base.OnDelete();

            foreach (Key key in m_Keys)
            {
                key.Delete();
            }

            m_Keys.Clear();
        }

        public void Add(Key key)
        {
            key.Internalize();
            m_Keys.Add(key);

            UpdateItemID();
        }

        public void Open(Mobile from)
        {
            Container cont = Parent as Container;

            if (cont == null)
                return;

            for (int i = m_Keys.Count - 1; i >= 0; i--)
            {
                Key key = m_Keys[i];

                if (!key.Deleted && !cont.TryDropItem(from, key, true))
                    break;

                m_Keys.RemoveAt(i);
            }

            UpdateItemID();
        }

        public void RemoveKeys(uint keyValue)
        {
            for (int i = m_Keys.Count - 1; i >= 0; i--)
            {
                Key key = m_Keys[i];

                if (key.KeyValue == keyValue)
                {
                    key.Delete();
                    m_Keys.RemoveAt(i);
                }
            }

            UpdateItemID();
        }

        public bool ContainsKey(uint keyValue)
        {
            foreach (Key key in m_Keys)
            {
                if (key.KeyValue == keyValue)
                    return true;
            }

            return false;
        }

        public override void AddCraftedProperties(ObjectPropertyList list)
        {
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
                list.Add(1053099, "#{0}\t{1}", CraftResources.GetLocalizationNumber(_Resource), string.Format("#{0}", LabelNumber.ToString())); // ~1_oretype~ ~2_armortype~
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

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(1); // version

            writer.Write((int)_Resource);
            writer.Write(_Crafter);
            writer.Write((int)_Quality);

            writer.WriteItemList(m_Keys);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            switch (version)
            {
                case 1:
                    _Resource = (CraftResource)reader.ReadInt();
                    _Crafter = reader.ReadMobile();
                    _Quality = (ItemQuality)reader.ReadInt();
                    goto case 0;
                case 0:
                    m_Keys = reader.ReadStrongItemList<Key>();
                    break;
            }
        }

        private void UpdateItemID()
        {
            if (Keys.Count < 1)
                ItemID = 0x1011;
            else if (Keys.Count < 3)
                ItemID = 0x1769;
            else if (Keys.Count < 5)
                ItemID = 0x176A;
            else
                ItemID = 0x176B;
        }

        private class InternalTarget : Target
        {
            private readonly KeyRing m_KeyRing;
            public InternalTarget(KeyRing keyRing)
                : base(-1, false, TargetFlags.None)
            {
                m_KeyRing = keyRing;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_KeyRing.Deleted || !m_KeyRing.IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(1060640); // The item must be in your backpack to use it.
                    return;
                }

                if (m_KeyRing == targeted)
                {
                    m_KeyRing.Open(from);
                    from.SendLocalizedMessage(501685); // You open the keyring.
                }
                else if (targeted is ILockable)
                {
                    ILockable o = (ILockable)targeted;

                    foreach (Key key in m_KeyRing.Keys)
                    {
                        if (key.UseOn(from, o))
                            return;
                    }

                    from.SendLocalizedMessage(1008140); // You do not have a key for that.
                }
                else
                {
                    from.SendLocalizedMessage(501666); // You can't unlock that!
                }
            }
        }
    }
}
