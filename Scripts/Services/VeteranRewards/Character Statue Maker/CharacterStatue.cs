using System;
using System.Collections;
using System.Collections.Generic;
using Server.Accounting;
using Server.ContextMenus;
using Server.Engines.VeteranRewards;
using Server.Gumps;
using Server.Items;
using Server.Multis;
using Server.Network;
using Server.Spells;
using Server.Targeting;

namespace Server.Mobiles
{
    public enum StatueType
    {
        Marble,
        Jade,
        Bronze
    }

    public enum StatuePose
    {
        Ready,
        Casting,
        Salute,
        AllPraiseMe,
        Fighting,
        HandsOnHips
    }

    public enum StatueMaterial
    {
        Antique,
        Dark,
        Medium,
        Light
    }

    public class CharacterStatue : Mobile, IRewardItem
    {
        private StatueType m_Type;
        private StatuePose m_Pose;
        private StatueMaterial m_Material;
        private Mobile m_SculptedBy;
        private DateTime m_SculptedOn;
        private CharacterStatuePlinth m_Plinth;
        private bool m_IsRewardItem;
        private int m_Animation;
        private int m_Frames;
        public CharacterStatue(Mobile from, StatueType type)
            : base()
        {
            this.m_Type = type;
            this.m_Pose = StatuePose.Ready;
            this.m_Material = StatueMaterial.Antique;

            this.Direction = Direction.South;
            this.AccessLevel = AccessLevel.Counselor;
            this.Hits = this.HitsMax;
            this.Blessed = true;
            this.Frozen = true;

            this.CloneBody(from);
            this.CloneClothes(from);
            this.InvalidateHues();
        }

        public CharacterStatue(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public StatueType StatueType
        {
            get
            {
                return this.m_Type;
            }
            set
            {
                this.m_Type = value;
                this.InvalidateHues();
                this.InvalidatePose();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public StatuePose Pose
        {
            get
            {
                return this.m_Pose;
            }
            set
            {
                this.m_Pose = value;
                this.InvalidatePose();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public StatueMaterial Material
        {
            get
            {
                return this.m_Material;
            }
            set
            {
                this.m_Material = value;
                this.InvalidateHues();
                this.InvalidatePose();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile SculptedBy
        {
            get
            {
                return this.m_SculptedBy;
            }
            set
            {
                this.m_SculptedBy = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime SculptedOn
        {
            get
            {
                return this.m_SculptedOn;
            }
            set
            {
                this.m_SculptedOn = value;
            }
        }
        public CharacterStatuePlinth Plinth
        {
            get
            {
                return this.m_Plinth;
            }
            set
            {
                this.m_Plinth = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get
            {
                return this.m_IsRewardItem;
            }
            set
            {
                this.m_IsRewardItem = value;
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            this.DisplayPaperdollTo(from);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (this.m_SculptedBy != null)
            {
                if (this.m_SculptedBy.ShowFameTitle && (this.m_SculptedBy.Player || this.m_SculptedBy.Body.IsHuman) && this.m_SculptedBy.Fame >= 10000)
                    list.Add(1076202, String.Format("{0} {1}", this.m_SculptedBy.Female ? "Lady" : "Lord", this.m_SculptedBy.Name)); // Sculpted by ~1_Name~
                else
                    list.Add(1076202, this.m_SculptedBy.Name); // Sculpted by ~1_Name~
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (from.Alive && this.m_SculptedBy != null)
            {
                BaseHouse house = BaseHouse.FindHouseAt(this);

                if ((house != null && house.IsCoOwner(from)) || (int)from.AccessLevel > (int)AccessLevel.Counselor)
                    list.Add(new DemolishEntry(this));
            }
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (this.m_Plinth != null && !this.m_Plinth.Deleted)
                this.m_Plinth.Delete();
        }

        public override bool CanBeRenamedBy(Mobile from)
        {
            return false;
        }

        public override bool CanBeDamaged()
        {
            return false;
        }

        public void OnRequestedAnimation(Mobile from)
        {
            from.Send(new UpdateStatueAnimation(this, 1, this.m_Animation, this.m_Frames));
        }

        public override void OnAosSingleClick(Mobile from)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version

            writer.Write((int)this.m_Type);
            writer.Write((int)this.m_Pose);
            writer.Write((int)this.m_Material);

            writer.Write((Mobile)this.m_SculptedBy);
            writer.Write((DateTime)this.m_SculptedOn);

            writer.Write((Item)this.m_Plinth);
            writer.Write((bool)this.m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            this.m_Type = (StatueType)reader.ReadInt();
            this.m_Pose = (StatuePose)reader.ReadInt();
            this.m_Material = (StatueMaterial)reader.ReadInt();

            this.m_SculptedBy = reader.ReadMobile();
            this.m_SculptedOn = reader.ReadDateTime();

            this.m_Plinth = reader.ReadItem() as CharacterStatuePlinth;
            this.m_IsRewardItem = reader.ReadBool();

            this.InvalidatePose();

            this.Frozen = true;

            if (this.m_SculptedBy == null || this.Map == Map.Internal) // Remove preview statues
            {
                Timer.DelayCall(TimeSpan.Zero, new TimerCallback(Delete));
            }
        }

        public void Sculpt(Mobile by)
        {
            this.m_SculptedBy = by;
            this.m_SculptedOn = DateTime.UtcNow;

            this.InvalidateProperties();
        }

        public bool Demolish(Mobile by)
        {
            CharacterStatueDeed deed = new CharacterStatueDeed(null);

            if (by.PlaceInBackpack(deed))
            {
                this.Delete();

                deed.Statue = this;
                deed.StatueType = this.m_Type;
                deed.IsRewardItem = this.m_IsRewardItem;

                if (this.m_Plinth != null)
                    this.m_Plinth.Delete();

                return true;
            }
            else
            {
                by.SendLocalizedMessage(500720); // You don't have enough room in your backpack!
                deed.Delete();

                return false;
            }
        }

        public void Restore(CharacterStatue from)
        {
            this.m_Material = from.Material;
            this.m_Pose = from.Pose;

            this.Direction = from.Direction;

            this.CloneBody(from);
            this.CloneClothes(from);

            this.InvalidateHues();
            this.InvalidatePose();
        }

        public void CloneBody(Mobile from)
        {
            this.Name = from.Name;
            this.BodyValue = from.BodyValue;
            this.Female = from.Female;
            this.HairItemID = from.HairItemID;
            this.FacialHairItemID = from.FacialHairItemID;
        }

        public void CloneClothes(Mobile from)
        {
            for (int i = this.Items.Count - 1; i >= 0; i --)
                this.Items[i].Delete();

            for (int i = from.Items.Count - 1; i >= 0; i --)
            {
                Item item = from.Items[i];

                if (item.Layer != Layer.Backpack && item.Layer != Layer.Mount && item.Layer != Layer.Bank)
                    this.AddItem(this.CloneItem(item));
            }
        }

        public Item CloneItem(Item item)
        {
            Item cloned = new Item(item.ItemID);
            cloned.Layer = item.Layer;
            cloned.Name = item.Name;
            cloned.Hue = item.Hue;
            cloned.Weight = item.Weight;
            cloned.Movable = false;

            return cloned;
        }

        public void InvalidateHues()
        {
            this.Hue = 0xB8F + (int)this.m_Type * 4 + (int)this.m_Material;

            this.HairHue = this.Hue;

            if (this.FacialHairItemID > 0)
                this.FacialHairHue = this.Hue;

            for (int i = this.Items.Count - 1; i >= 0; i --)
                this.Items[i].Hue = this.Hue;

            if (this.m_Plinth != null)
                this.m_Plinth.InvalidateHue();
        }

        public void InvalidatePose()
        {
            switch ( this.m_Pose )
            {
                case StatuePose.Ready:
                    this.m_Animation = 4;
                    this.m_Frames = 0;
                    break;
                case StatuePose.Casting:
                    this.m_Animation = 16;
                    this.m_Frames = 2;
                    break;
                case StatuePose.Salute:
                    this.m_Animation = 33;
                    this.m_Frames = 1;
                    break;
                case StatuePose.AllPraiseMe:
                    this.m_Animation = 17;
                    this.m_Frames = 4;
                    break;
                case StatuePose.Fighting:
                    this.m_Animation = 31;
                    this.m_Frames = 5;
                    break;
                case StatuePose.HandsOnHips:
                    this.m_Animation = 6;
                    this.m_Frames = 1;
                    break;
            }

            if (this.Map != null)
            {
                this.ProcessDelta();

                Packet p = null;

                IPooledEnumerable eable = this.Map.GetClientsInRange(this.Location);

                foreach (NetState state in eable)
                {
                    state.Mobile.ProcessDelta();

                    if (p == null)
                        p = Packet.Acquire(new UpdateStatueAnimation(this, 1, this.m_Animation, this.m_Frames));

                    state.Send(p);
                }

                Packet.Release(p);

                eable.Free();
            }
        }

        protected override void OnMapChange(Map oldMap)
        {
            this.InvalidatePose();

            if (this.m_Plinth != null)
                this.m_Plinth.Map = this.Map;
        }

        protected override void OnLocationChange(Point3D oldLocation)
        {
            this.InvalidatePose();

            if (this.m_Plinth != null)
                this.m_Plinth.Location = new Point3D(this.X, this.Y, this.Z - 5);
        }

        private class DemolishEntry : ContextMenuEntry
        {
            private readonly CharacterStatue m_Statue;
            public DemolishEntry(CharacterStatue statue)
                : base(6275, 2)
            {
                this.m_Statue = statue;
            }

            public override void OnClick()
            {
                if (this.m_Statue.Deleted)
                    return;

                this.m_Statue.Demolish(this.Owner.From);
            }
        }
    }

    public class CharacterStatueDeed : Item, IRewardItem
    {
        private CharacterStatue m_Statue;
        private StatueType m_Type;
        private bool m_IsRewardItem;
        public CharacterStatueDeed(CharacterStatue statue)
            : base(0x14F0)
        {
            this.m_Statue = statue;

            if (statue != null)
            {
                this.m_Type = statue.StatueType;
                this.m_IsRewardItem = statue.IsRewardItem;
            }

            this.LootType = LootType.Blessed;
            this.Weight = 1.0;
        }

        public CharacterStatueDeed(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                StatueType t = this.m_Type;

                if (this.m_Statue != null)
                {
                    t = this.m_Statue.StatueType;
                }

                switch ( t )
                {
                    case StatueType.Marble:
                        return 1076189;
                    case StatueType.Jade:
                        return 1076188;
                    case StatueType.Bronze:
                        return 1076190;
                    default:
                        return 1076173;
                }
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public CharacterStatue Statue
        {
            get
            {
                return this.m_Statue;
            }
            set
            {
                this.m_Statue = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public StatueType StatueType
        {
            get
            {
                if (this.m_Statue != null)
                    return this.m_Statue.StatueType;

                return this.m_Type;
            }
            set
            {
                this.m_Type = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get
            {
                return this.m_IsRewardItem;
            }
            set
            {
                this.m_IsRewardItem = value;
                this.InvalidateProperties();
            }
        }
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (this.m_IsRewardItem)
                list.Add(1076222); // 6th Year Veteran Reward

            if (this.m_Statue != null)
                list.Add(1076231, this.m_Statue.Name); // Statue of ~1_Name~
        }

        public override void OnDoubleClick(Mobile from)
        {
            Account acct = from.Account as Account;

            if (acct != null && from.IsPlayer())
            {
                TimeSpan time = TimeSpan.FromDays(RewardSystem.RewardInterval.TotalDays * 6) - (DateTime.UtcNow - acct.Created);

                if (time > TimeSpan.Zero)
                {
                    from.SendLocalizedMessage(1008126, true, Math.Ceiling(time.TotalDays / RewardSystem.RewardInterval.TotalDays).ToString()); // Your account is not old enough to use this item. Months until you can use this item :
                    return;
                }
            }

            if (this.IsChildOf(from.Backpack))
            {
                if (!from.IsBodyMod)
                {
                    from.SendLocalizedMessage(1076194); // Select a place where you would like to put your statue.
                    from.Target = new CharacterStatueTarget(this, this.StatueType);
                }
                else
                    from.SendLocalizedMessage(1073648); // You may only proceed while in your original state...
            }
            else
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
        }

        public override void OnDelete()
        {
            base.OnDelete();

            if (this.m_Statue != null)
                this.m_Statue.Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)1); // version

            writer.Write((int)this.m_Type);

            writer.Write((Mobile)this.m_Statue);
            writer.Write((bool)this.m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            if (version >= 1)
            {
                this.m_Type = (StatueType)reader.ReadInt();
            }

            this.m_Statue = reader.ReadMobile() as CharacterStatue;
            this.m_IsRewardItem = reader.ReadBool();
        }
    }

    public class CharacterStatueTarget : Target
    {
        private readonly Item m_Maker;
        private readonly StatueType m_Type;
        public CharacterStatueTarget(Item maker, StatueType type)
            : base(-1, true, TargetFlags.None)
        {
            this.m_Maker = maker;
            this.m_Type = type;
        }

        public static AddonFitResult CouldFit(Point3D p, Map map, Mobile from, ref BaseHouse house)
        {
            if (!map.CanFit(p.X, p.Y, p.Z, 20, true, true, true))
                return AddonFitResult.Blocked;
            else if (!BaseAddon.CheckHouse(from, p, map, 20, ref house))
                return AddonFitResult.NotInHouse;
            else
                return CheckDoors(p, 20, house);
        }

        public static AddonFitResult CheckDoors(Point3D p, int height, BaseHouse house)
        {
            List<Item> doors = house.Doors;

            for (int i = 0; i < doors.Count; i ++)
            {
                BaseDoor door = doors[i] as BaseDoor;

                Point3D doorLoc = door.GetWorldLocation();
                int doorHeight = door.ItemData.CalcHeight;

                if (Utility.InRange(doorLoc, p, 1) && (p.Z == doorLoc.Z || ((p.Z + height) > doorLoc.Z && (doorLoc.Z + doorHeight) > p.Z)))
                    return AddonFitResult.DoorTooClose;
            }

            return AddonFitResult.Valid;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            IPoint3D p = targeted as IPoint3D;
            Map map = from.Map;

            if (p == null || map == null || this.m_Maker == null || this.m_Maker.Deleted)
                return;

            if (this.m_Maker.IsChildOf(from.Backpack))
            {
                SpellHelper.GetSurfaceTop(ref p);
                BaseHouse house = null;
                Point3D loc = new Point3D(p);

                if (targeted is Item && !((Item)targeted).IsLockedDown && !((Item)targeted).IsSecure && !(targeted is AddonComponent))
                {
                    from.SendLocalizedMessage(1076191); // Statues can only be placed in houses.
                    return;
                }
                else if (from.IsBodyMod)
                {
                    from.SendLocalizedMessage(1073648); // You may only proceed while in your original state...
                    return;
                }

                AddonFitResult result = CouldFit(loc, map, from, ref house);

                if (result == AddonFitResult.Valid)
                {
                    CharacterStatue statue = new CharacterStatue(from, this.m_Type);
                    CharacterStatuePlinth plinth = new CharacterStatuePlinth(statue);

                    house.Addons[plinth] = from;

                    if (this.m_Maker is IRewardItem)
                        statue.IsRewardItem = ((IRewardItem)this.m_Maker).IsRewardItem;

                    statue.Plinth = plinth;
                    plinth.MoveToWorld(loc, map);
                    statue.InvalidatePose();

                    /*
                    * TODO: Previously the maker wasn't deleted until after statue
                    * customization, leading to redeeding issues. Exact OSI behavior
                    * needs looking into.
                    */
                    this.m_Maker.Delete();
                    statue.Sculpt(from);

                    from.CloseGump(typeof(CharacterStatueGump));
                    from.SendGump(new CharacterStatueGump(this.m_Maker, statue, from));
                }
                else if (result == AddonFitResult.Blocked)
                    from.SendLocalizedMessage(500269); // You cannot build that there.
                else if (result == AddonFitResult.NotInHouse)
                    from.SendLocalizedMessage(1076192); // Statues can only be placed in houses where you are the owner or co-owner.
                else if (result == AddonFitResult.DoorTooClose)
                    from.SendLocalizedMessage(500271); // You cannot build near the door.
            }
            else
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
        }
    }
}