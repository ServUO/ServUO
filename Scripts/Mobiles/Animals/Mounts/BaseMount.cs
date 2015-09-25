using System;
using System.Collections;

namespace Server.Mobiles
{
    public enum BlockMountType
    {
        None = -1,
        Dazed,
        BolaRecovery,
        DismountRecovery
    }

    public abstract class BaseMount : BaseCreature, IMount
    {
        private static readonly Hashtable m_Table = new Hashtable();
        private Mobile m_Rider;
        private Item m_InternalItem;
        private DateTime m_NextMountAbility;
        public BaseMount(string name, int bodyID, int itemID, AIType aiType, FightMode fightMode, int rangePerception, int rangeFight, double activeSpeed, double passiveSpeed)
            : base(aiType, fightMode, rangePerception, rangeFight, activeSpeed, passiveSpeed)
        {
            this.Name = name;
            this.Body = bodyID;

            this.m_InternalItem = new MountItem(this, itemID);
        }

        public BaseMount(Serial serial)
            : base(serial)
        {
        }

        public virtual TimeSpan MountAbilityDelay
        {
            get
            {
                return TimeSpan.Zero;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextMountAbility
        {
            get
            {
                return this.m_NextMountAbility;
            }
            set
            {
                this.m_NextMountAbility = value;
            }
        }
        public virtual bool AllowMaleRider
        {
            get
            {
                return true;
            }
        }
        public virtual bool AllowFemaleRider
        {
            get
            {
                return true;
            }
        }
        [Hue, CommandProperty(AccessLevel.GameMaster)]
        public override int Hue
        {
            get
            {
                return base.Hue;
            }
            set
            {
                base.Hue = value;

                if (this.m_InternalItem != null)
                    this.m_InternalItem.Hue = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int ItemID
        {
            get
            {
                if (this.m_InternalItem != null)
                    return this.m_InternalItem.ItemID;
                else
                    return 0;
            }
            set
            {
                if (this.m_InternalItem != null)
                    this.m_InternalItem.ItemID = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Rider
        {
            get
            {
                return this.m_Rider;
            }
            set
            {
                if (this.m_Rider != value)
                {
                    if (value == null)
                    {
                        Point3D loc = this.m_Rider.Location;
                        Map map = this.m_Rider.Map;

                        if (map == null || map == Map.Internal)
                        {
                            loc = this.m_Rider.LogoutLocation;
                            map = this.m_Rider.LogoutMap;
                        }

                        this.Direction = this.m_Rider.Direction;
                        this.Location = loc;
                        this.Map = map;

                        if (this.m_InternalItem != null)
                            this.m_InternalItem.Internalize();
                    }
                    else
                    {
                        if (this.m_Rider != null)
                            Dismount(this.m_Rider);

                        Dismount(value);

                        if (this.m_InternalItem != null)
                            value.AddItem(this.m_InternalItem);

                        value.Direction = this.Direction;

                        this.Internalize();
                    }

                    this.m_Rider = value;
                }
            }
        }

        protected Item InternalItem
        {
            get
            {
                return this.m_InternalItem;
            }
        }

        public static void Dismount(Mobile dismounted)
        {
            Dismount(dismounted, dismounted, BlockMountType.None, TimeSpan.FromSeconds(0), false);
        }

        public static void Dismount(Mobile dismounter, Mobile dismounted, BlockMountType blockmounttype, TimeSpan delay)
        {
            Dismount(dismounter, dismounted, blockmounttype, TimeSpan.FromSeconds(0), true);
        }

        public static void Dismount(Mobile dismounter, Mobile dismounted, BlockMountType blockmounttype, TimeSpan delay, bool message)
        {
            if (!dismounted.Mounted)
                return;

            if (dismounted is ChaosDragoonElite)
            {
                dismounter.SendLocalizedMessage(1042047); // You fail to knock the rider from its mount.
            }

            IMount mount = dismounted.Mount;

            if (mount != null)
            {
                mount.Rider = null;
                BaseMount.SetMountPrevention(dismounted, blockmounttype, delay);

                if (message)
                    dismounted.SendLocalizedMessage(1040023); // You have been knocked off of your mount!
            }
            else if (dismounted.Flying)
            {
                if (!OnFlightPath(dismounted))
                {
                    dismounted.Flying = false;
                    dismounted.Freeze(TimeSpan.FromSeconds(1));
                    dismounted.Animate(61, 10, 1, true, false, 0);
                }
            }
            else if (Spells.Ninjitsu.AnimalForm.UnderTransformation(dismounted))
            {
                Spells.Ninjitsu.AnimalForm.RemoveContext(dismounted, true);
            }
        }

        public static bool OnFlightPath(Mobile m)
        {
            if (!m.Flying)
                return false;

            StaticTile[] tiles = m.Map.Tiles.GetStaticTiles(m.X, m.Y, true);
            ItemData itemData;
            bool onpath = false;

            for (int i = 0; i < tiles.Length && !onpath; ++i)
            {
                itemData = TileData.ItemTable[tiles[i].ID & TileData.MaxItemValue];
                onpath = (itemData.Name == "hover over");
            }

            return onpath;
        }

        public static void SetMountPrevention(Mobile mob, BlockMountType type, TimeSpan duration)
        {
            if (mob == null)
                return;

            DateTime expiration = DateTime.UtcNow + duration;

            BlockEntry entry = m_Table[mob] as BlockEntry;

            if (entry != null)
            {
                entry.m_Type = type;
                entry.m_Expiration = expiration;
            }
            else
            {
                m_Table[mob] = entry = new BlockEntry(type, expiration);
            }
        }

        public static void ClearMountPrevention(Mobile mob)
        {
            if (mob != null)
                m_Table.Remove(mob);
        }

        public static BlockMountType GetMountPrevention(Mobile mob)
        {
            if (mob == null)
                return BlockMountType.None;

            BlockEntry entry = m_Table[mob] as BlockEntry;

            if (entry == null)
                return BlockMountType.None;

            if (entry.IsExpired)
            {
                m_Table.Remove(mob);
                return BlockMountType.None;
            }

            return entry.m_Type;
        }

        public static bool CheckMountAllowed(Mobile mob, bool message)
        {
            BlockMountType type = GetMountPrevention(mob);

            if (type == BlockMountType.None)
                return true;

            if (message)
            {
                switch ( type )
                {
                    case BlockMountType.Dazed:
                        {
                            mob.SendLocalizedMessage(1040024); // You are still too dazed from being knocked off your mount to ride!
                            break;
                        }
                    case BlockMountType.BolaRecovery:
                        {
                            mob.SendLocalizedMessage(1062910); // You cannot mount while recovering from a bola throw.
                            break;
                        }
                    case BlockMountType.DismountRecovery:
                        {
                            mob.SendLocalizedMessage(1070859); // You cannot mount while recovering from a dismount special maneuver.
                            break;
                        }
                }
            }

            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write(this.m_NextMountAbility);

            writer.Write(this.m_Rider);
            writer.Write(this.m_InternalItem);
        }

        public override bool OnBeforeDeath()
        {
            this.Rider = null;

            return base.OnBeforeDeath();
        }

        public override void OnAfterDelete()
        {
            if (this.m_InternalItem != null)
                this.m_InternalItem.Delete();

            this.m_InternalItem = null;

            base.OnAfterDelete();
        }

        public override void OnDelete()
        {
            this.Rider = null;

            base.OnDelete();
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                    {
                        this.m_NextMountAbility = reader.ReadDateTime();
                        goto case 0;
                    }
                case 0:
                    {
                        this.m_Rider = reader.ReadMobile();
                        this.m_InternalItem = reader.ReadItem();

                        if (this.m_InternalItem == null)
                            this.Delete();

                        break;
                    }
            }
        }

        public virtual void OnDisallowedRider(Mobile m)
        {
            m.SendMessage("You may not ride this creature.");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.IsDeadPet)
                return;

            if (from.IsBodyMod && !from.Body.IsHuman)
            {
                if (Core.AOS) // You cannot ride a mount in your current form.
                    this.PrivateOverheadMessage(Network.MessageType.Regular, 0x3B2, 1062061, from.NetState);
                else
                    from.SendLocalizedMessage(1061628); // You can't do that while polymorphed.

                return;
            }

            if (!CheckMountAllowed(from, true))
                return;

            if (from.Mounted)
            {
                from.SendLocalizedMessage(1005583); // Please dismount first.
                return;
            }

            if (from.Race == Race.Gargoyle && from.IsPlayer())
            {
                from.SendLocalizedMessage(1112281);
                this.OnDisallowedRider(from);
                return;
            }

            if (from.Female ? !this.AllowFemaleRider : !this.AllowMaleRider)
            {
                this.OnDisallowedRider(from);
                return;
            }

            if (!Multis.DesignContext.Check(from))
                return;

            if (from.HasTrade)
            {
                from.SendLocalizedMessage(1042317, "", 0x41); // You may not ride at this time
                return;
            }

            if (from.InRange(this, 1))
            {
                bool canAccess = (from.AccessLevel >= AccessLevel.GameMaster) ||
                                 (this.Controlled && this.ControlMaster == from) ||
                                 (this.Summoned && this.SummonMaster == from);

                if (canAccess)
                {
                    if (this.Poisoned)
                        this.PrivateOverheadMessage(Network.MessageType.Regular, 0x3B2, 1049692, from.NetState); // This mount is too ill to ride.
                    else
                        this.Rider = from;
                }
                else if (!this.Controlled && !this.Summoned)
                {
                    // That mount does not look broken! You would have to tame it to ride it.
                    this.PrivateOverheadMessage(Network.MessageType.Regular, 0x3B2, 501263, from.NetState);
                }
                else
                {
                    // This isn't your mount; it refuses to let you ride.
                    this.PrivateOverheadMessage(Network.MessageType.Regular, 0x3B2, 501264, from.NetState);
                }
            }
            else
            {
                from.SendLocalizedMessage(500206); // That is too far away to ride.
            }
        }

        public virtual void OnRiderDamaged(int amount, Mobile from, bool willKill)
        {
            if (this.m_Rider == null)
                return;

            Mobile attacker = from;
            if (attacker == null)
                attacker = this.m_Rider.FindMostRecentDamager(true);

            if (!(attacker == this || attacker == this.m_Rider || willKill || DateTime.UtcNow < this.m_NextMountAbility))
            {
                if (this.DoMountAbility(amount, from))
                    this.m_NextMountAbility = DateTime.UtcNow + this.MountAbilityDelay;
            }
        }

        public virtual bool DoMountAbility(int damage, Mobile attacker)
        {
            return false;
        }

        private class BlockEntry
        {
            public BlockMountType m_Type;
            public DateTime m_Expiration;
            public BlockEntry(BlockMountType type, DateTime expiration)
            {
                this.m_Type = type;
                this.m_Expiration = expiration;
            }

            public bool IsExpired
            {
                get
                {
                    return (DateTime.UtcNow >= this.m_Expiration);
                }
            }
        }
    }

    public class MountItem : Item, IMountItem
    {
        private BaseMount m_Mount;
        public MountItem(BaseMount mount, int itemID)
            : base(itemID)
        {
            this.Layer = Layer.Mount;
            this.Movable = false;

            this.m_Mount = mount;
        }

        public MountItem(Serial serial)
            : base(serial)
        {
        }

        public override double DefaultWeight
        {
            get
            {
                return 0;
            }
        }
        public IMount Mount
        {
            get
            {
                return this.m_Mount;
            }
        }
        public override void OnAfterDelete()
        {
            if (this.m_Mount != null)
                this.m_Mount.Delete();

            this.m_Mount = null;

            base.OnAfterDelete();
        }

        public override DeathMoveResult OnParentDeath(Mobile parent)
        {
            if (this.m_Mount != null)
                this.m_Mount.Rider = null;

            return DeathMoveResult.RemainEquiped;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(this.m_Mount);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_Mount = reader.ReadMobile() as BaseMount;

                        if (this.m_Mount == null)
                            this.Delete();

                        break;
                    }
            }
        }
    }
}