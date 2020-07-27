using System;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
    public class FelineBlessedStatue : BaseContainer, IFlipable
    {
        public override int LabelNumber => 1075494; // Blessed Statue

        private static readonly Type[] m_ResourceTypes = new Type[]
        {
            typeof(BlackPearl),
            typeof(Bloodmoss),
            typeof(Garlic),
            typeof(Ginseng),
            typeof(MandrakeRoot),
            typeof(Nightshade),
            typeof(SpidersSilk),
            typeof(SulfurousAsh)
        };

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextReagentTime  { get; set; }

        [Constructable]
        public FelineBlessedStatue()
            : base(0x1947)
        {
            Weight = 12;            
            LootType = LootType.Blessed;
        }

        public override int DefaultGumpID => 0x116;

        public virtual void OnFlip(Mobile m)
        {
            if (ItemID == 0x1947)
            {
                ItemID = 0x1948;
            }
            else if (ItemID == 0x1948)
            {
                ItemID = 0x1947;
            }
            else if (ItemID == 0x1949)
            {
                ItemID = 0x194A;
            }
            else if (ItemID == 0x194A)
            {
                ItemID = 0x1949;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house != null && (house.IsLockedDown(this) || house.IsSecure(this)))
            {
                if (!from.InRange(GetWorldLocation(), 1))
                {
                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                }
                else
                {
                    base.OnDoubleClick(from);
                }                
            }
            else
            {
                from.SendLocalizedMessage(501717); // This isn't secure...
            }
        }

        private Timer Timer { get; set; }

        public void StartTimer()
        {
            if (NextReagentTime == DateTime.MinValue)
            {
                NextReagentTime = DateTime.UtcNow + TimeSpan.FromDays(1);
            }

            if (Timer == null)
            {
                Timer = new InternalTimer(this);
            }

            Timer.Start();
        }

        public void StopTimer()
        {
            if (Timer != null)
            {
                Timer.Stop();
                Timer = null;
                NextReagentTime = DateTime.MinValue;
            }
        }

        public void OnTick()
        {
            if (Movable)
            {                
                StopTimer();
                return;
            }

            if (NextReagentTime < DateTime.UtcNow)
            {
                DropResource();
                NextReagentTime = DateTime.UtcNow + TimeSpan.FromDays(1.0);
            }
        }

        private class InternalTimer : Timer
        {
            private readonly FelineBlessedStatue _Item;

            public InternalTimer(FelineBlessedStatue item)
                : base(TimeSpan.FromHours(1.0), TimeSpan.FromHours(1.0))
            {
                Priority = TimerPriority.OneSecond;
                _Item = item;
            }

            protected override void OnTick()
            {
                if (_Item == null || _Item.Deleted)
                {
                    Stop();                    
                    return;
                }

                _Item.OnTick();
            }
        }

        public override bool TryDropItem(Mobile from, Item dropped, bool sendFullMessage)
        {
            return false;
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            return false;
        }

        public override void OnItemRemoved(Item item)
        {
            base.OnItemRemoved(item);

            CheckContainer();
            StartTimer();
        }

        public override void OnSecureChange()
        {
            if (IsSecure)
            {
                StartTimer();
            }
        }

        public override void OnLockDownChange()
        {
            if (IsLockedDown)
            {                
                StartTimer();
            }
        }

        public void CheckContainer()
        {
            if (Items.Count == 0)
            {
                if (ItemID == 0x1949 || ItemID == 0x194A)
                {
                    ItemID -= 2;
                }                
            }
            else
            {
                if (ItemID == 0x1947 || ItemID == 0x1948)
                {
                    ItemID += 2;
                }
            }
        }

        private void DropResource()
        {            
            var item = Activator.CreateInstance(m_ResourceTypes[Utility.Random(m_ResourceTypes.Length)]) as Item;           

            if (item != null)
            {
                item.Amount = Utility.RandomMinMax(10, 20);

                if (TotalItems + 1 > MaxItems)
                {
                    item.Delete();
                    StopTimer();
                }
                else
                {
                    DropItem(item);

                    CheckContainer();                    
                }
            }
        }

        public FelineBlessedStatue(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(NextReagentTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            NextReagentTime  = reader.ReadDateTime();

            if (!Movable)
            {
                StartTimer();
            }
        }
    }
}
