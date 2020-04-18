using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
    public class SorcerersRewardChest : Item
    {
        public override int LabelNumber => 1023712;  // strong box

        [Constructable]
        public SorcerersRewardChest() : base(0x9AA)
        {
            Movable = false;
            Hue = 1912;
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1154226); // *It's an unassuming strong box. You examine the lock more closely and determine there is no way to pick it. You'll need to find a key.*

            base.OnDoubleClick(from);
        }

        public SorcerersRewardChest(Serial serial) : base(serial)
        {
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

    public class StrongboxKey : BaseDecayingItem
    {
        public override int LabelNumber => 1154227;  // Strongbox Key        

        [Constructable]
        public StrongboxKey() : base(0x410A)
        {
            Stackable = false;
            Weight = 0.01;
            Hue = 2721;
            LootType = LootType.Blessed;
        }

        public override int Lifespan => 3600;
        public override bool UseSeconds => false;

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072351); // Quest Item
        }

        public StrongboxKey(Serial serial) : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!(from is PlayerMobile))
                return;

            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1054107); // This item must be in your backpack.
                return;
            }

            from.Target = new ChestTarget(from, this);
            from.SendLocalizedMessage(1010086); // What do you want to use this on?
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

    public class ChestTarget : Target
    {
        private static Mobile m_From;
        private static StrongboxKey m_Key;

        public ChestTarget(Mobile from, StrongboxKey key) : base(2, false, TargetFlags.None)
        {
            m_From = from;
            m_Key = key;
        }

        protected override void OnTarget(Mobile from, object o)
        {
            if (o is SorcerersRewardChest)
            {
                Item item = new SalvagerSuitPlans();
                Container pack = from.Backpack;

                if (pack == null || !pack.TryDropItem(from, item, false))
                    from.BankBox.DropItem(item);

                m_From.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1154228); // *You insert the key into the mechanism and turn it. To your delight the lock opens with a click and you remove the contents*

                m_Key.Delete();
            }
            else
            {
                from.SendLocalizedMessage(501668); // This key doesn't seem to unlock that.
            }
        }
    }
}