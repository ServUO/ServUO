using System;
using System.Collections;
using Server.Engines.Plants;
using Server.Engines.Quests;
using Server.Engines.Quests.Hag;
using Server.Engines.Quests.Matriarch;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
    public enum BeverageType
    {
        Ale,
        Cider,
        Liquor,
        Milk,
        Wine,
        Water
    }

    public interface IHasQuantity
    {
        int Quantity { get; set; }
    }

    public interface IWaterSource : IHasQuantity
    {
    }

    // TODO: Flipable attributes

    [TypeAlias("Server.Items.BottleAle", "Server.Items.BottleLiquor", "Server.Items.BottleWine")]
    public class BeverageBottle : BaseBeverage
    {
        public override int BaseLabelNumber
        {
            get
            {
                return 1042959;
            }
        }// a bottle of Ale
        public override int MaxQuantity
        {
            get
            {
                return 5;
            }
        }
        public override bool Fillable
        {
            get
            {
                return false;
            }
        }

        public override int ComputeItemID()
        {
            if (!this.IsEmpty)
            {
                switch( this.Content )
                {
                    case BeverageType.Ale:
                        return 0x99F;
                    case BeverageType.Cider:
                        return 0x99F;
                    case BeverageType.Liquor:
                        return 0x99B;
                    case BeverageType.Milk:
                        return 0x99B;
                    case BeverageType.Wine:
                        return 0x9C7;
                    case BeverageType.Water:
                        return 0x99B;
                }
            }

            return 0;
        }

        [Constructable]
        public BeverageBottle(BeverageType type)
            : base(type)
        {
            this.Weight = 1.0;
        }

        public BeverageBottle(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    {
                        if (this.CheckType("BottleAle"))
                        {
                            this.Quantity = this.MaxQuantity;
                            this.Content = BeverageType.Ale;
                        }
                        else if (this.CheckType("BottleLiquor"))
                        {
                            this.Quantity = this.MaxQuantity;
                            this.Content = BeverageType.Liquor;
                        }
                        else if (this.CheckType("BottleWine"))
                        {
                            this.Quantity = this.MaxQuantity;
                            this.Content = BeverageType.Wine;
                        }
                        else
                        {
                            throw new Exception(World.LoadingType);
                        }

                        break;
                    }
            }
        }
    }

    public class Jug : BaseBeverage
    {
        public override int BaseLabelNumber
        {
            get
            {
                return 1042965;
            }
        }// a jug of Ale
        public override int MaxQuantity
        {
            get
            {
                return 10;
            }
        }
        public override bool Fillable
        {
            get
            {
                return false;
            }
        }

        public override int ComputeItemID()
        {
            if (!this.IsEmpty)
                return 0x9C8;

            return 0;
        }

        [Constructable]
        public Jug(BeverageType type)
            : base(type)
        {
            this.Weight = 1.0;
        }

        public Jug(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class CeramicMug : BaseBeverage
    {
        public override int BaseLabelNumber
        {
            get
            {
                return 1042982;
            }
        }// a ceramic mug of Ale
        public override int MaxQuantity
        {
            get
            {
                return 1;
            }
        }

        public override int ComputeItemID()
        {
            if (this.ItemID >= 0x995 && this.ItemID <= 0x999)
                return this.ItemID;
            else if (this.ItemID == 0x9CA)
                return this.ItemID;

            return 0x995;
        }

        [Constructable]
        public CeramicMug()
        {
            this.Weight = 1.0;
        }

        [Constructable]
        public CeramicMug(BeverageType type)
            : base(type)
        {
            this.Weight = 1.0;
        }

        public CeramicMug(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class PewterMug : BaseBeverage
    {
        public override int BaseLabelNumber
        {
            get
            {
                return 1042994;
            }
        }// a pewter mug with Ale
        public override int MaxQuantity
        {
            get
            {
                return 1;
            }
        }

        public override int ComputeItemID()
        {
            if (this.ItemID >= 0xFFF && this.ItemID <= 0x1002)
                return this.ItemID;

            return 0xFFF;
        }

        [Constructable]
        public PewterMug()
        {
            this.Weight = 1.0;
        }

        [Constructable]
        public PewterMug(BeverageType type)
            : base(type)
        {
            this.Weight = 1.0;
        }

        public PewterMug(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class Goblet : BaseBeverage
    {
        public override int BaseLabelNumber
        {
            get
            {
                return 1043000;
            }
        }// a goblet of Ale
        public override int MaxQuantity
        {
            get
            {
                return 1;
            }
        }

        public override int ComputeItemID()
        {
            if (this.ItemID == 0x99A || this.ItemID == 0x9B3 || this.ItemID == 0x9BF || this.ItemID == 0x9CB)
                return this.ItemID;

            return 0x99A;
        }

        [Constructable]
        public Goblet()
        {
            this.Weight = 1.0;
        }

        [Constructable]
        public Goblet(BeverageType type)
            : base(type)
        {
            this.Weight = 1.0;
        }

        public Goblet(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    [TypeAlias("Server.Items.MugAle", "Server.Items.GlassCider", "Server.Items.GlassLiquor",
        "Server.Items.GlassMilk", "Server.Items.GlassWine", "Server.Items.GlassWater")]
    public class GlassMug : BaseBeverage
    {
        public override int EmptyLabelNumber
        {
            get
            {
                return 1022456;
            }
        }// mug
        public override int BaseLabelNumber
        {
            get
            {
                return 1042976;
            }
        }// a mug of Ale
        public override int MaxQuantity
        {
            get
            {
                return 5;
            }
        }

        public override int ComputeItemID()
        {
            if (this.IsEmpty)
                return (this.ItemID >= 0x1F81 && this.ItemID <= 0x1F84 ? this.ItemID : 0x1F81);

            switch( this.Content )
            {
                case BeverageType.Ale:
                    return (this.ItemID == 0x9EF ? 0x9EF : 0x9EE);
                case BeverageType.Cider:
                    return (this.ItemID >= 0x1F7D && this.ItemID <= 0x1F80 ? this.ItemID : 0x1F7D);
                case BeverageType.Liquor:
                    return (this.ItemID >= 0x1F85 && this.ItemID <= 0x1F88 ? this.ItemID : 0x1F85);
                case BeverageType.Milk:
                    return (this.ItemID >= 0x1F89 && this.ItemID <= 0x1F8C ? this.ItemID : 0x1F89);
                case BeverageType.Wine:
                    return (this.ItemID >= 0x1F8D && this.ItemID <= 0x1F90 ? this.ItemID : 0x1F8D);
                case BeverageType.Water:
                    return (this.ItemID >= 0x1F91 && this.ItemID <= 0x1F94 ? this.ItemID : 0x1F91);
            }

            return 0;
        }

        [Constructable]
        public GlassMug()
        {
            this.Weight = 1.0;
        }

        [Constructable]
        public GlassMug(BeverageType type)
            : base(type)
        {
            this.Weight = 1.0;
        }

        public GlassMug(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    {
                        if (this.CheckType("MugAle"))
                        {
                            this.Quantity = this.MaxQuantity;
                            this.Content = BeverageType.Ale;
                        }
                        else if (this.CheckType("GlassCider"))
                        {
                            this.Quantity = this.MaxQuantity;
                            this.Content = BeverageType.Cider;
                        }
                        else if (this.CheckType("GlassLiquor"))
                        {
                            this.Quantity = this.MaxQuantity;
                            this.Content = BeverageType.Liquor;
                        }
                        else if (this.CheckType("GlassMilk"))
                        {
                            this.Quantity = this.MaxQuantity;
                            this.Content = BeverageType.Milk;
                        }
                        else if (this.CheckType("GlassWine"))
                        {
                            this.Quantity = this.MaxQuantity;
                            this.Content = BeverageType.Wine;
                        }
                        else if (this.CheckType("GlassWater"))
                        {
                            this.Quantity = this.MaxQuantity;
                            this.Content = BeverageType.Water;
                        }
                        else
                        {
                            throw new Exception(World.LoadingType);
                        }

                        break;
                    }
            }
        }
    }

    [TypeAlias("Server.Items.PitcherAle", "Server.Items.PitcherCider", "Server.Items.PitcherLiquor",
        "Server.Items.PitcherMilk", "Server.Items.PitcherWine", "Server.Items.PitcherWater",
        "Server.Items.GlassPitcher")]
    public class Pitcher : BaseBeverage
    {
        public override int BaseLabelNumber
        {
            get
            {
                return 1048128;
            }
        }// a Pitcher of Ale
        public override int MaxQuantity
        {
            get
            {
                return Content == BeverageType.Water ? 5 : 5;
            }
        }

        public override int ComputeItemID()
        {
            if (this.IsEmpty)
            {
                if (this.ItemID == 0x9A7 || this.ItemID == 0xFF7)
                    return this.ItemID;

                return 0xFF6;
            }

            switch( this.Content )
            {
                case BeverageType.Ale:
                    {
                        if (this.ItemID == 0x1F96)
                            return this.ItemID;

                        return 0x1F95;
                    }
                case BeverageType.Cider:
                    {
                        if (this.ItemID == 0x1F98)
                            return this.ItemID;

                        return 0x1F97;
                    }
                case BeverageType.Liquor:
                    {
                        if (this.ItemID == 0x1F9A)
                            return this.ItemID;

                        return 0x1F99;
                    }
                case BeverageType.Milk:
                    {
                        if (this.ItemID == 0x9AD)
                            return this.ItemID;

                        return 0x9F0;
                    }
                case BeverageType.Wine:
                    {
                        if (this.ItemID == 0x1F9C)
                            return this.ItemID;

                        return 0x1F9B;
                    }
                case BeverageType.Water:
                    {
                        if (this.ItemID == 0xFF8 || this.ItemID == 0xFF9 || this.ItemID == 0x1F9E)
                            return this.ItemID;

                        return 0x1F9D;
                    }
            }

            return 0;
        }

        [Constructable]
        public Pitcher()
        {
            this.Weight = 2.0;
        }

        [Constructable]
        public Pitcher(BeverageType type)
            : base(type)
        {
            this.Weight = 2.0;
        }

        public Pitcher(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            if (this.CheckType("PitcherWater") || this.CheckType("GlassPitcher"))
                base.InternalDeserialize(reader, false);
            else
                base.InternalDeserialize(reader, true);

            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    {
                        if (this.CheckType("PitcherAle"))
                        {
                            this.Quantity = this.MaxQuantity;
                            this.Content = BeverageType.Ale;
                        }
                        else if (this.CheckType("PitcherCider"))
                        {
                            this.Quantity = this.MaxQuantity;
                            this.Content = BeverageType.Cider;
                        }
                        else if (this.CheckType("PitcherLiquor"))
                        {
                            this.Quantity = this.MaxQuantity;
                            this.Content = BeverageType.Liquor;
                        }
                        else if (this.CheckType("PitcherMilk"))
                        {
                            this.Quantity = this.MaxQuantity;
                            this.Content = BeverageType.Milk;
                        }
                        else if (this.CheckType("PitcherWine"))
                        {
                            this.Quantity = this.MaxQuantity;
                            this.Content = BeverageType.Wine;
                        }
                        else if (this.CheckType("PitcherWater"))
                        {
                            this.Quantity = this.MaxQuantity;
                            this.Content = BeverageType.Water;
                        }
                        else if (this.CheckType("GlassPitcher"))
                        {
                            this.Quantity = 0;
                            this.Content = BeverageType.Water;
                        }
                        else
                        {
                            throw new Exception(World.LoadingType);
                        }

                        break;
                    }
            }
        }
    }

    public abstract class BaseBeverage : Item, IHasQuantity
    {
        private BeverageType m_Content;
        private int m_Quantity;
        private Mobile m_Poisoner;
        private Poison m_Poison;

        public override int LabelNumber
        {
            get
            {
                int num = this.BaseLabelNumber;

                if (this.IsEmpty || num == 0)
                    return this.EmptyLabelNumber;

                return this.BaseLabelNumber + (int)this.m_Content;
            }
        }

        public virtual bool ShowQuantity
        {
            get
            {
                return (this.MaxQuantity > 1);
            }
        }
        public virtual bool Fillable
        {
            get
            {
                return true;
            }
        }
        public virtual bool Pourable
        {
            get
            {
                return true;
            }
        }

        public virtual int EmptyLabelNumber
        {
            get
            {
                return base.LabelNumber;
            }
        }
        public virtual int BaseLabelNumber
        {
            get
            {
                return 0;
            }
        }

        public abstract int MaxQuantity { get; }

        public abstract int ComputeItemID();

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsEmpty
        {
            get
            {
                return (this.m_Quantity <= 0);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ContainsAlchohol
        {
            get
            {
                return (!this.IsEmpty && this.m_Content != BeverageType.Milk && this.m_Content != BeverageType.Water);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsFull
        {
            get
            {
                return (this.m_Quantity >= this.MaxQuantity);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Poison Poison
        {
            get
            {
                return this.m_Poison;
            }
            set
            {
                this.m_Poison = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Poisoner
        {
            get
            {
                return this.m_Poisoner;
            }
            set
            {
                this.m_Poisoner = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public BeverageType Content
        {
            get
            {
                return this.m_Content;
            }
            set
            {
                this.m_Content = value;

                this.InvalidateProperties();

                int itemID = this.ComputeItemID();

                if (itemID > 0)
                    this.ItemID = itemID;
                else
                    this.Delete();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Quantity
        {
            get
            {
                return this.m_Quantity;
            }
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > this.MaxQuantity)
                    value = this.MaxQuantity;

                this.m_Quantity = value;

                this.QuantityChanged();
                this.InvalidateProperties();

                int itemID = this.ComputeItemID();

                if (itemID > 0)
                    this.ItemID = itemID;
                else
                    this.Delete();
            }
        }

        public virtual int GetQuantityDescription()
        {
            int perc = (this.m_Quantity * 100) / this.MaxQuantity;

            if (perc <= 0)
                return 1042975; // It's empty.
            else if (perc <= 33)
                return 1042974; // It's nearly empty.
            else if (perc <= 66)
                return 1042973; // It's half full.
            else
                return 1042972; // It's full.
        }

        public virtual void QuantityChanged()
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (this.ShowQuantity)
                list.Add(this.GetQuantityDescription());
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            if (this.ShowQuantity)
                this.LabelTo(from, this.GetQuantityDescription());
        }

        public virtual bool ValidateUse(Mobile from, bool message)
        {
            if (this.Deleted)
                return false;

            if (!this.Movable && !this.Fillable)
            {
                Multis.BaseHouse house = Multis.BaseHouse.FindHouseAt(this);

                if (house == null || !house.IsLockedDown(this))
                {
                    if (message)
                        from.SendLocalizedMessage(502946, "", 0x59); // That belongs to someone else.

                    return false;
                }
            }

            if (from.Map != this.Map || !from.InRange(this.GetWorldLocation(), 2) || !from.InLOS(this))
            {
                if (message)
                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.

                return false;
            }

            return true;
        }

        public virtual void Fill_OnTarget(Mobile from, object targ)
        {
            if (!this.IsEmpty || !this.Fillable || !this.ValidateUse(from, false))
                return;

            if (targ is BaseBeverage)
            {
                BaseBeverage bev = (BaseBeverage)targ;

                if (bev.IsEmpty || !bev.ValidateUse(from, true))
                    return;

                this.Content = bev.Content;
                this.Poison = bev.Poison;
                this.Poisoner = bev.Poisoner;

                if (bev.Quantity > this.MaxQuantity)
                {
                    this.Quantity = this.MaxQuantity;
                    bev.Quantity -= this.MaxQuantity;
                }
                else
                {
                    this.Quantity += bev.Quantity;
                    bev.Quantity = 0;
                }
            }
            else if (targ is BaseWaterContainer)
            {
                BaseWaterContainer bwc = targ as BaseWaterContainer;

                if (this.Quantity == 0 || (this.Content == BeverageType.Water && !this.IsFull))
                {
                    this.Content = BeverageType.Water;

                    int iNeed = Math.Min((this.MaxQuantity - this.Quantity), bwc.Quantity);

                    if (iNeed > 0 && !bwc.IsEmpty && !this.IsFull)
                    {
                        bwc.Quantity -= iNeed;
                        this.Quantity += iNeed;

                        from.PlaySound(0x4E);
                    }
                }
            }
            else if (targ is Item)
            {
                Item item = (Item)targ;
                IWaterSource src;

                src = (item as IWaterSource);

                if (src == null && item is AddonComponent)
                    src = (((AddonComponent)item).Addon as IWaterSource);

                if (src == null || src.Quantity <= 0)
                    return;

                if (from.Map != item.Map || !from.InRange(item.GetWorldLocation(), 2) || !from.InLOS(item))
                {
                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                    return;
                }

                this.Content = BeverageType.Water;
                this.Poison = null;
                this.Poisoner = null;

                if (src.Quantity > this.MaxQuantity)
                {
                    this.Quantity = this.MaxQuantity;
                    src.Quantity -= this.MaxQuantity;
                }
                else
                {
                    this.Quantity += src.Quantity;
                    src.Quantity = 0;
                }

                from.SendLocalizedMessage(1010089); // You fill the container with water.
            }
            else if (targ is Cow)
            {
                Cow cow = (Cow)targ;

                if (cow.TryMilk(from))
                {
                    this.Content = BeverageType.Milk;
                    this.Quantity = this.MaxQuantity;
                    from.SendLocalizedMessage(1080197); // You fill the container with milk.
                }
            }
            else if (targ is LandTarget)
            {
                int tileID = ((LandTarget)targ).TileID;

                PlayerMobile player = from as PlayerMobile;

                if (player != null)
                {
                    QuestSystem qs = player.Quest;

                    if (qs is WitchApprenticeQuest)
                    {
                        FindIngredientObjective obj = qs.FindObjective(typeof(FindIngredientObjective)) as FindIngredientObjective;

                        if (obj != null && !obj.Completed && obj.Ingredient == Ingredient.SwampWater)
                        {
                            bool contains = false;

                            for (int i = 0; !contains && i < m_SwampTiles.Length; i += 2)
                                contains = (tileID >= m_SwampTiles[i] && tileID <= m_SwampTiles[i + 1]);

                            if (contains)
                            {
                                this.Delete();

                                player.SendLocalizedMessage(1055035); // You dip the container into the disgusting swamp water, collecting enough for the Hag's vile stew.
                                obj.Complete();
                            }
                        }
                    }
                }
            }
        }

        private static readonly int[] m_SwampTiles = new int[]
        {
            0x9C4, 0x9EB,
            0x3D65, 0x3D65,
            0x3DC0, 0x3DD9,
            0x3DDB, 0x3DDC,
            0x3DDE, 0x3EF0,
            0x3FF6, 0x3FF6,
            0x3FFC, 0x3FFE,
        };

        #region Effects of achohol
        private static readonly Hashtable m_Table = new Hashtable();

        public static void Initialize()
        {
            EventSink.Login += new LoginEventHandler(EventSink_Login);
        }

        private static void EventSink_Login(LoginEventArgs e)
        {
            CheckHeaveTimer(e.Mobile);
        }

        public static void CheckHeaveTimer(Mobile from)
        {
            if (from.BAC > 0 && from.Map != Map.Internal && !from.Deleted)
            {
                Timer t = (Timer)m_Table[from];

                if (t == null)
                {
                    if (from.BAC > 60)
                        from.BAC = 60;

                    t = new HeaveTimer(from);
                    t.Start();

                    m_Table[from] = t;
                }
            }
            else
            {
                Timer t = (Timer)m_Table[from];

                if (t != null)
                {
                    t.Stop();
                    m_Table.Remove(from);

                    from.SendLocalizedMessage(500850); // You feel sober.
                }
            }
        }

        private class HeaveTimer : Timer
        {
            private readonly Mobile m_Drunk;

            public HeaveTimer(Mobile drunk)
                : base(TimeSpan.FromSeconds(5.0), TimeSpan.FromSeconds(5.0))
            {
                this.m_Drunk = drunk;

                this.Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                if (this.m_Drunk.Deleted || this.m_Drunk.Map == Map.Internal)
                {
                    this.Stop();
                    m_Table.Remove(this.m_Drunk);
                }
                else if (this.m_Drunk.Alive)
                {
                    if (this.m_Drunk.BAC > 60)
                        this.m_Drunk.BAC = 60;

                    // chance to get sober
                    if (10 > Utility.Random(100))
                        --this.m_Drunk.BAC;

                    // lose some stats
                    this.m_Drunk.Stam -= 1;
                    this.m_Drunk.Mana -= 1;

                    if (Utility.Random(1, 4) == 1)
                    {
                        if (!this.m_Drunk.Mounted)
                        {
                            // turn in a random direction
                            this.m_Drunk.Direction = (Direction)Utility.Random(8);

                            // heave
                            this.m_Drunk.Animate(32, 5, 1, true, false, 0);
                        }

                        // *hic*
                        this.m_Drunk.PublicOverheadMessage(Network.MessageType.Regular, 0x3B2, 500849);
                    }

                    if (this.m_Drunk.BAC <= 0)
                    {
                        this.Stop();
                        m_Table.Remove(this.m_Drunk);

                        this.m_Drunk.SendLocalizedMessage(500850); // You feel sober.
                    }
                }
            }
        }

        #endregion

        public virtual void Pour_OnTarget(Mobile from, object targ)
        {
            if (this.IsEmpty || !this.Pourable || !this.ValidateUse(from, false))
                return;

            if (targ is BaseBeverage)
            {
                BaseBeverage bev = (BaseBeverage)targ;

                if (!bev.ValidateUse(from, true))
                    return;

                if (bev.IsFull && bev.Content == this.Content)
                {
                    from.SendLocalizedMessage(500848); // Couldn't pour it there.  It was already full.
                }
                else if (!bev.IsEmpty)
                {
                    from.SendLocalizedMessage(500846); // Can't pour it there.
                }
                else
                {
                    bev.Content = this.Content;
                    bev.Poison = this.Poison;
                    bev.Poisoner = this.Poisoner;

                    if (this.Quantity > bev.MaxQuantity)
                    {
                        bev.Quantity = bev.MaxQuantity;
                        this.Quantity -= bev.MaxQuantity;
                    }
                    else
                    {
                        bev.Quantity += this.Quantity;
                        this.Quantity = 0;
                    }

                    from.PlaySound(0x4E);
                }
            }
            else if (from == targ)
            {
                if (from.Thirst < 20)
                    from.Thirst += 1;

                if (this.ContainsAlchohol)
                {
                    int bac = 0;

                    switch( this.Content )
                    {
                        case BeverageType.Ale:
                            bac = 1;
                            break;
                        case BeverageType.Wine:
                            bac = 2;
                            break;
                        case BeverageType.Cider:
                            bac = 3;
                            break;
                        case BeverageType.Liquor:
                            bac = 4;
                            break;
                    }

                    from.BAC += bac;

                    if (from.BAC > 60)
                        from.BAC = 60;

                    CheckHeaveTimer(from);
                }

                from.PlaySound(Utility.RandomList(0x30, 0x2D6));

                if (this.m_Poison != null)
                    from.ApplyPoison(this.m_Poisoner, this.m_Poison);

                --this.Quantity;
            }
            else if (targ is BaseWaterContainer)
            {
                BaseWaterContainer bwc = targ as BaseWaterContainer;
				
                if (this.Content != BeverageType.Water)
                {
                    from.SendLocalizedMessage(500842); // Can't pour that in there.
                }
                else if (bwc.Items.Count != 0)
                {
                    from.SendLocalizedMessage(500841); // That has something in it.
                }
                else
                { 
                    int itNeeds = Math.Min((bwc.MaxQuantity - bwc.Quantity), this.Quantity);

                    if (itNeeds > 0)
                    {
                        bwc.Quantity += itNeeds;
                        this.Quantity -= itNeeds;

                        from.PlaySound(0x4E);
                    }
                }
            }
            else if (targ is PlantItem)
            {
                ((PlantItem)targ).Pour(from, this);
            }
            else if (targ is ChickenLizardEgg)
            {
                ((ChickenLizardEgg)targ).Pour(from, this);
            }
            else if (targ is AddonComponent &&
                     (((AddonComponent)targ).Addon is WaterVatEast || ((AddonComponent)targ).Addon is WaterVatSouth) &&
                     this.Content == BeverageType.Water)
            {
                PlayerMobile player = from as PlayerMobile;

                if (player != null)
                {
                    SolenMatriarchQuest qs = player.Quest as SolenMatriarchQuest;

                    if (qs != null)
                    {
                        QuestObjective obj = qs.FindObjective(typeof(GatherWaterObjective));

                        if (obj != null && !obj.Completed)
                        {
                            BaseAddon vat = ((AddonComponent)targ).Addon;

                            if (vat.X > 5784 && vat.X < 5814 && vat.Y > 1903 && vat.Y < 1934 &&
                                ((qs.RedSolen && vat.Map == Map.Trammel) || (!qs.RedSolen && vat.Map == Map.Felucca)))
                            {
                                if (obj.CurProgress + this.Quantity > obj.MaxProgress)
                                {
                                    int delta = obj.MaxProgress - obj.CurProgress;

                                    this.Quantity -= delta;
                                    obj.CurProgress = obj.MaxProgress;
                                }
                                else
                                {
                                    obj.CurProgress += this.Quantity;
                                    this.Quantity = 0;
                                }
                            }
                        }
                    }
                }
            }
            else if (targ is WaterElemental)
            {
                if (this is Pitcher && this.Content == BeverageType.Water)
                {
                    EndlessDecanter.HandleThrow(this, (WaterElemental)targ, from);
                }
            }
            else
            {
                from.SendLocalizedMessage(500846); // Can't pour it there.
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.IsEmpty)
            {
                if (!this.Fillable || !this.ValidateUse(from, true))
                    return;

                from.BeginTarget(-1, true, TargetFlags.None, new TargetCallback(Fill_OnTarget));
                this.SendLocalizedMessageTo(from, 500837); // Fill from what?
            }
            else if (this.Pourable && this.ValidateUse(from, true))
            {
                from.BeginTarget(-1, true, TargetFlags.None, new TargetCallback(Pour_OnTarget));
                from.SendLocalizedMessage(1010086); // What do you want to use this on?
            }
        }

        public static bool ConsumeTotal(Container pack, BeverageType content, int quantity)
        {
            return ConsumeTotal(pack, typeof(BaseBeverage), content, quantity);
        }

        public static bool ConsumeTotal(Container pack, Type itemType, BeverageType content, int quantity)
        {
            Item[] items = pack.FindItemsByType(itemType);

            // First pass, compute total
            int total = 0;

            for (int i = 0; i < items.Length; ++i)
            {
                BaseBeverage bev = items[i] as BaseBeverage;

                if (bev != null && bev.Content == content && !bev.IsEmpty)
                    total += bev.Quantity;
            }

            if (total >= quantity)
            {
                // We've enough, so consume it
                int need = quantity;

                for (int i = 0; i < items.Length; ++i)
                {
                    BaseBeverage bev = items[i] as BaseBeverage;

                    if (bev == null || bev.Content != content || bev.IsEmpty)
                        continue;

                    int theirQuantity = bev.Quantity;

                    if (theirQuantity < need)
                    {
                        bev.Quantity = 0;
                        need -= theirQuantity;
                    }
                    else
                    {
                        bev.Quantity -= need;
                        return true;
                    }
                }
            }

            return false;
        }

        public BaseBeverage()
        {
            this.ItemID = this.ComputeItemID();
        }

        public BaseBeverage(BeverageType type)
        {
            this.m_Content = type;
            this.m_Quantity = this.MaxQuantity;
            this.ItemID = this.ComputeItemID();
        }

        public BaseBeverage(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write((Mobile)this.m_Poisoner);

            Poison.Serialize(this.m_Poison, writer);
            writer.Write((int)this.m_Content);
            writer.Write((int)this.m_Quantity);
        }

        protected bool CheckType(string name)
        {
            return (World.LoadingType == String.Format("Server.Items.{0}", name));
        }

        public override void Deserialize(GenericReader reader)
        {
            this.InternalDeserialize(reader, true);
        }

        protected void InternalDeserialize(GenericReader reader, bool read)
        {
            base.Deserialize(reader);

            if (!read)
                return;

            int version = reader.ReadInt();

            switch( version )
            {
                case 1:
                    {
                        this.m_Poisoner = reader.ReadMobile();
                        goto case 0;
                    }
                case 0:
                    {
                        this.m_Poison = Poison.Deserialize(reader);
                        this.m_Content = (BeverageType)reader.ReadInt();
                        this.m_Quantity = reader.ReadInt();
                        break;
                    }
            }
        }
    }
}