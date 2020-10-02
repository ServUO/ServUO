using System;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
    public class FelineBlessedStatue : BaseContainer, IFlipable
    {
        public static readonly TimeSpan RespawnDuration = TimeSpan.FromDays(1);
        public static readonly string TimerID = "FelineStatueTimer";

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

        public void StartTimer()
        {
            if (NextReagentTime == DateTime.MinValue)
            {
                NextReagentTime = DateTime.UtcNow + RespawnDuration;
            }
            else if (NextReagentTime < DateTime.UtcNow)
            {
                NextReagentTime = DateTime.UtcNow + TimeSpan.FromSeconds(10);
            }

            TimerRegistry.Register(TimerID, this, NextReagentTime - DateTime.UtcNow, false, statue => OnTick(statue));
        }

        public void StopTimer()
        {
            TimerRegistry.RemoveFromRegistry(TimerID, this);
        }

        public static void OnTick(FelineBlessedStatue statue)
        {
            if (statue.Movable)
            {
                statue.StopTimer();
                return;
            }

            statue.DropResource();

            TimerRegistry.UpdateRegistry(TimerID, statue, RespawnDuration);
            statue.NextReagentTime = DateTime.UtcNow + RespawnDuration;
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
            if (!World.Loading && IsSecure)
            {
                StartTimer();
            }
        }

        public override void OnLockDownChange()
        {
            if (!World.Loading && IsLockedDown)
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
                    DropItemStacked(item);

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

            if (IsLockedDown || IsSecure)
            {
                StartTimer();
            }
        }
    }
}
