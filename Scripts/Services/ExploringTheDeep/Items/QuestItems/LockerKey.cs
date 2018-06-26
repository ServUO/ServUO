using Server.Network;
using Server.Targeting;
using System;

namespace Server.Items
{
    public class LockerKey : BaseDecayingItem
    {
        public override int LabelNumber { get { return 1154227; } } // Strongbox Key

        private Parts m_Type;

        [CommandProperty(AccessLevel.GameMaster)]
        public Parts Type
        {
            get
            {
                return this.m_Type;
            }
            set
            {
                this.m_Type = value;
                this.InvalidateProperties();
            }
        }

        [Constructable]
        public LockerKey(Parts type)
            : base(0x410B)
        {
            this.Weight = 1.0;
            this.LootType = LootType.Blessed;
            this.Hue = 1359;
            this.m_Type = type;
        }

        public override int Lifespan { get { return 300; } }

        public override void OnDoubleClick(Mobile from)
        {
            if (!this.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(501661); // That key is unreachable.
                return;
            }

            from.SendLocalizedMessage(501662); // What shall I use this key on?
            from.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1154228); // *You insert the key into the mechanism and turn it. To your delight the lock opens with a click and you remove the contents*
            from.Target = new UnlockTarget(this);
        }

        private class UnlockTarget : Target
        {
            private readonly LockerKey m_Key;
            public UnlockTarget(LockerKey key)
                : base(1, false, TargetFlags.None)
            {
                this.m_Key = key;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (this.m_Key.Deleted || !this.m_Key.IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(501661); // That key is unreachable.
                    return;
                }

                if (targeted is StorageLocker)
                {
                    StorageLocker sl = (StorageLocker)targeted;

                    if (sl.Type == this.m_Key.Type)
                    {
                        sl.Locked = false;

                        if (sl.Type == Parts.Flywheel)
                            sl.DropItem(new FlyWheel());
                        else if (sl.Type == Parts.BearingAssembly)
                            sl.DropItem(new BearingAssembly());
                        else if (sl.Type == Parts.PowerCore)
                            sl.DropItem(new PowerCore());
                        else if (sl.Type == Parts.WireSpool)
                            sl.DropItem(new WireSpool());

                        sl.BeginRestart(TimeSpan.FromMinutes(10.0));

                        m_Key.Delete();
                    }
                }
                else
                {
                    from.SendLocalizedMessage(501666); // You can't unlock that!
                }
            }
        }

        public LockerKey(Serial serial)
            : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1154445, String.Format("#{0}", 1154427 + (int)m_Type)); // A Key to the Spare ~1_PART~ Parts Locker
			list.Add(1072351); // Quest Item
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
