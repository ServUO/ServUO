using System;
using System.Collections.Generic;
using Server.Targeting;

namespace Server.Items
{
    public class KeyRing : Item
    {
        public static readonly int MaxKeys = 20;
        private List<Key> m_Keys;
        [Constructable]
        public KeyRing()
            : base(0x1011)
        {
            this.Weight = 1.0; // They seem to have no weight on OSI ?!

            this.m_Keys = new List<Key>();
        }

        public KeyRing(Serial serial)
            : base(serial)
        {
        }

        public List<Key> Keys
        {
            get
            {
                return this.m_Keys;
            }
        }
        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (!this.IsChildOf(from.Backpack))
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
            else if (this.Keys.Count >= MaxKeys)
            {
                from.SendLocalizedMessage(1008138); // This keyring is full.
                return false;
            }
            else
            {
                this.Add(key);
                from.SendLocalizedMessage(501691); // You put the key on the keyring.
                return true;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!this.IsChildOf(from.Backpack))
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

            foreach (Key key in this.m_Keys)
            {
                key.Delete();
            }

            this.m_Keys.Clear();
        }

        public void Add(Key key)
        {
            key.Internalize();
            this.m_Keys.Add(key);

            this.UpdateItemID();
        }

        public void Open(Mobile from)
        {
            Container cont = this.Parent as Container;

            if (cont == null)
                return;

            for (int i = this.m_Keys.Count - 1; i >= 0; i--)
            {
                Key key = this.m_Keys[i];

                if (!key.Deleted && !cont.TryDropItem(from, key, true))
                    break;

                this.m_Keys.RemoveAt(i);
            }

            this.UpdateItemID();
        }

        public void RemoveKeys(uint keyValue)
        {
            for (int i = this.m_Keys.Count - 1; i >= 0; i--)
            {
                Key key = this.m_Keys[i];

                if (key.KeyValue == keyValue)
                {
                    key.Delete();
                    this.m_Keys.RemoveAt(i);
                }
            }

            this.UpdateItemID();
        }

        public bool ContainsKey(uint keyValue)
        {
            foreach (Key key in this.m_Keys)
            {
                if (key.KeyValue == keyValue)
                    return true;
            }

            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.WriteItemList<Key>(this.m_Keys);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            this.m_Keys = reader.ReadStrongItemList<Key>();
        }

        private void UpdateItemID()
        {
            if (this.Keys.Count < 1)
                this.ItemID = 0x1011;
            else if (this.Keys.Count < 3)
                this.ItemID = 0x1769;
            else if (this.Keys.Count < 5)
                this.ItemID = 0x176A;
            else
                this.ItemID = 0x176B;
        }

        private class InternalTarget : Target
        {
            private readonly KeyRing m_KeyRing;
            public InternalTarget(KeyRing keyRing)
                : base(-1, false, TargetFlags.None)
            {
                this.m_KeyRing = keyRing;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (this.m_KeyRing.Deleted || !this.m_KeyRing.IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(1060640); // The item must be in your backpack to use it.
                    return;
                }

                if (this.m_KeyRing == targeted)
                {
                    this.m_KeyRing.Open(from);
                    from.SendLocalizedMessage(501685); // You open the keyring.
                }
                else if (targeted is ILockable)
                {
                    ILockable o = (ILockable)targeted;

                    foreach (Key key in this.m_KeyRing.Keys)
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