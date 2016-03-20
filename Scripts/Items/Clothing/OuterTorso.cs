using System;
using Server.Engines.VeteranRewards;

namespace Server.Items
{
    public abstract class BaseOuterTorso : BaseClothing
    {
        public BaseOuterTorso(int itemID)
            : this(itemID, 0)
        {
        }

        public BaseOuterTorso(int itemID, int hue)
            : base(itemID, Layer.OuterTorso, hue)
        {
        }

        public BaseOuterTorso(Serial serial)
            : base(serial)
        {
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

    public class ZooMemberSkirt : PlainDress
    {
        public override int LabelNumber
        {
            get
            {
                return 1073221;
            }
        }// Britannia Royal Zoo Member

        [Constructable]
        public ZooMemberSkirt()
            : this(0)
        {
        }

        [Constructable]
        public ZooMemberSkirt(int hue)
            : base(hue)
        {
        }

        public ZooMemberSkirt(Serial serial)
            : base(serial)
        {
        }

        public override bool Dye(Mobile from, DyeTub sender)
        {
            from.SendLocalizedMessage(sender.FailMessage);
            return false;
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

    public class ZooMemberBodySash : BodySash
    {
        public override int LabelNumber
        {
            get
            {
                return 1073221;
            }
        }// Britannia Royal Zoo Member

        [Constructable]
        public ZooMemberBodySash()
            : this(0)
        {
        }

        [Constructable]
        public ZooMemberBodySash(int hue)
            : base(hue)
        {
        }

        public ZooMemberBodySash(Serial serial)
            : base(serial)
        {
        }

        public override bool Dye(Mobile from, DyeTub sender)
        {
            from.SendLocalizedMessage(sender.FailMessage);
            return false;
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

    public class ZooMemberRobe : Robe
    {
        public override int LabelNumber
        {
            get
            {
                return 1073221;
            }
        }// Britannia Royal Zoo Member

        [Constructable]
        public ZooMemberRobe()
            : this(0)
        {
        }

        [Constructable]
        public ZooMemberRobe(int hue)
            : base(hue)
        {
        }

        public ZooMemberRobe(Serial serial)
            : base(serial)
        {
        }

        public override bool Dye(Mobile from, DyeTub sender)
        {
            from.SendLocalizedMessage(sender.FailMessage);
            return false;
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

    public class ZooMemberCloak : Cloak
    {
        public override int LabelNumber
        {
            get
            {
                return 1073221;
            }
        }// Britannia Royal Zoo Member

        [Constructable]
        public ZooMemberCloak()
            : this(0)
        {
        }

        [Constructable]
        public ZooMemberCloak(int hue)
            : base(hue)
        {
        }

        public ZooMemberCloak(Serial serial)
            : base(serial)
        {
        }

        public override bool Dye(Mobile from, DyeTub sender)
        {
            from.SendLocalizedMessage(sender.FailMessage);
            return false;
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

    public class LibraryFriendBodySash : BodySash
    {
        public override int LabelNumber
        {
            get
            {
                return 1073346;
            }
        }// Friends of the Library Sash

        [Constructable]
        public LibraryFriendBodySash()
            : this(0)
        {
        }

        [Constructable]
        public LibraryFriendBodySash(int hue)
            : base(hue)
        {
        }

        public LibraryFriendBodySash(Serial serial)
            : base(serial)
        {
        }

        public override bool Dye(Mobile from, DyeTub sender)
        {
            from.SendLocalizedMessage(sender.FailMessage);
            return false;
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

    public class LibraryFriendDoublet : Doublet
    {
        public override int LabelNumber
        {
            get
            {
                return 1073351;
            }
        }// Friends of the Library Doublet

        [Constructable]
        public LibraryFriendDoublet()
            : this(0)
        {
        }

        [Constructable]
        public LibraryFriendDoublet(int hue)
            : base(hue)
        {
        }

        public LibraryFriendDoublet(Serial serial)
            : base(serial)
        {
        }

        public override bool Dye(Mobile from, DyeTub sender)
        {
            from.SendLocalizedMessage(sender.FailMessage);
            return false;
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

    public class LibraryFriendSurcoat : Surcoat
    {
        public override int LabelNumber
        {
            get
            {
                return 1073348;
            }
        }// Friends of the Library Surcoat

        [Constructable]
        public LibraryFriendSurcoat()
            : this(0)
        {
        }

        [Constructable]
        public LibraryFriendSurcoat(int hue)
            : base(hue)
        {
        }

        public LibraryFriendSurcoat(Serial serial)
            : base(serial)
        {
        }

        public override bool Dye(Mobile from, DyeTub sender)
        {
            from.SendLocalizedMessage(sender.FailMessage);
            return false;
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

    public class LibraryFriendCloak : Cloak
    {
        public override int LabelNumber
        {
            get
            {
                return 1073350;
            }
        }// Friends of the Library Cloak

        [Constructable]
        public LibraryFriendCloak()
            : this(0)
        {
        }

        [Constructable]
        public LibraryFriendCloak(int hue)
            : base(hue)
        {
        }

        public LibraryFriendCloak(Serial serial)
            : base(serial)
        {
        }

        public override bool Dye(Mobile from, DyeTub sender)
        {
            from.SendLocalizedMessage(sender.FailMessage);
            return false;
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

    public class Adranath : BodySash
    {
        public override int LabelNumber
        {
            get
            {
                return 1073253;
            }
        }// Adranath - Museum of Vesper Replica

        [Constructable]
        public Adranath()
            : this(0)
        {
        }

        [Constructable]
        public Adranath(int hue)
            : base(hue)
        {
        }

        public Adranath(Serial serial)
            : base(serial)
        {
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

    public class OdricsRobe : Robe
    {
        public override int LabelNumber
        {
            get
            {
                return 1073250;
            }
        }// Odric's Robe - Museum of Vesper Replica

        [Constructable]
        public OdricsRobe()
            : this(0)
        {
        }

        [Constructable]
        public OdricsRobe(int hue)
            : base(hue)
        {
        }

        public OdricsRobe(Serial serial)
            : base(serial)
        {
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

    public class BaronLenshiresCloak : Cloak
    {
        public override int LabelNumber
        {
            get
            {
                return 1073252;
            }
        }// Baron Lenshire's Cloak - Museum of Vesper Replica

        [Constructable]
        public BaronLenshiresCloak()
            : this(0)
        {
        }

        [Constructable]
        public BaronLenshiresCloak(int hue)
            : base(hue)
        {
        }

        public BaronLenshiresCloak(Serial serial)
            : base(serial)
        {
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

    [Flipable(0x230E, 0x230D)]
    public class GildedDress : BaseOuterTorso
    {
        [Constructable]
        public GildedDress()
            : this(0)
        {
        }

        [Constructable]
        public GildedDress(int hue)
            : base(0x230E, hue)
        {
            this.Weight = 3.0;
        }

        public GildedDress(Serial serial)
            : base(serial)
        {
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

    [Flipable(0x1F00, 0x1EFF)]
    public class FancyDress : BaseOuterTorso
    {
        [Constructable]
        public FancyDress()
            : this(0)
        {
        }

        [Constructable]
        public FancyDress(int hue)
            : base(0x1F00, hue)
        {
            this.Weight = 3.0;
        }

        public FancyDress(Serial serial)
            : base(serial)
        {
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

    public class DeathRobe : Robe
    {
        private Timer m_DecayTimer;
        private DateTime m_DecayTime;

        private static readonly TimeSpan m_DefaultDecayTime = TimeSpan.FromMinutes(1.0);

        public override bool DisplayLootType
        {
            get
            {
                return Core.ML;
            }
        }

        [Constructable]
        public DeathRobe()
        {
            this.LootType = LootType.Newbied;
            this.Hue = 2301;
            this.BeginDecay(m_DefaultDecayTime);
        }

        public new bool Scissor(Mobile from, Scissors scissors)
        {
            from.SendLocalizedMessage(502440); // Scissors can not be used on that to produce anything.
            return false;
        }

        public void BeginDecay()
        {
            this.BeginDecay(m_DefaultDecayTime);
        }

        private void BeginDecay(TimeSpan delay)
        {
            if (this.m_DecayTimer != null)
                this.m_DecayTimer.Stop();

            this.m_DecayTime = DateTime.UtcNow + delay;

            this.m_DecayTimer = new InternalTimer(this, delay);
            this.m_DecayTimer.Start();
        }

        public override bool OnDroppedToWorld(Mobile from, Point3D p)
        {
            this.BeginDecay(m_DefaultDecayTime);

            return true;
        }

        public override bool OnDroppedToMobile(Mobile from, Mobile target)
        {
            if (this.m_DecayTimer != null)
            {
                this.m_DecayTimer.Stop();
                this.m_DecayTimer = null;
            }

            return true;
        }

        public override void OnAfterDelete()
        {
            if (this.m_DecayTimer != null)
                this.m_DecayTimer.Stop();

            this.m_DecayTimer = null;
        }

        private class InternalTimer : Timer
        {
            private readonly DeathRobe m_Robe;

            public InternalTimer(DeathRobe c, TimeSpan delay)
                : base(delay)
            {
                this.m_Robe = c;
                this.Priority = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                if (this.m_Robe.Parent != null || this.m_Robe.IsLockedDown)
                    this.Stop();
                else
                    this.m_Robe.Delete();
            }
        }

        public DeathRobe(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)2); // version

            writer.Write(this.m_DecayTimer != null);

            if (this.m_DecayTimer != null)
                writer.WriteDeltaTime(this.m_DecayTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 2:
                    {
                        if (reader.ReadBool())
                        {
                            this.m_DecayTime = reader.ReadDeltaTime();
                            this.BeginDecay(this.m_DecayTime - DateTime.UtcNow);
                        }
                        break;
                    }
                case 1:
                case 0:
                    {
                        if (this.Parent == null)
                            this.BeginDecay(m_DefaultDecayTime);
                        break;
                    }
            }

            if (version < 1 && this.Hue == 0)
                this.Hue = 2301;
        }
    }

    [Flipable]
    public class RewardRobe : BaseOuterTorso, IRewardItem
    {
        private int m_LabelNumber;
        private bool m_IsRewardItem;

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

        [CommandProperty(AccessLevel.GameMaster)]
        public int Number
        {
            get
            {
                return this.m_LabelNumber;
            }
            set
            {
                this.m_LabelNumber = value;
                this.InvalidateProperties();
            }
        }

        public override int LabelNumber
        {
            get
            {
                if (this.m_LabelNumber > 0)
                    return this.m_LabelNumber;

                return base.LabelNumber;
            }
        }

        public override int BasePhysicalResistance
        {
            get
            {
                return 3;
            }
        }

        public override void OnAdded(object parent)
        {
            base.OnAdded(parent);

            if (parent is Mobile)
                ((Mobile)parent).VirtualArmorMod += 2;
        }

        public override void OnRemoved(object parent)
        {
            base.OnRemoved(parent);

            if (parent is Mobile)
                ((Mobile)parent).VirtualArmorMod -= 2;
        }

        public override bool Dye(Mobile from, DyeTub sender)
        {
            from.SendLocalizedMessage(sender.FailMessage);
            return false;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (Core.ML && this.m_IsRewardItem)
                list.Add(RewardSystem.GetRewardYearLabel(this, new object[] { this.Hue, this.m_LabelNumber })); // X Year Veteran Reward
        }

        public override bool CanEquip(Mobile m)
        {
            if (!base.CanEquip(m))
                return false;

            return !this.m_IsRewardItem || RewardSystem.CheckIsUsableBy(m, this, new object[] { this.Hue, this.m_LabelNumber });
        }

        [Constructable]
        public RewardRobe()
            : this(0)
        {
        }

        [Constructable]
        public RewardRobe(int hue)
            : this(hue, 0)
        {
        }

        [Constructable]
        public RewardRobe(int hue, int labelNumber)
            : base(0x1F03, hue)
        {
            this.Weight = 3.0;
            this.LootType = LootType.Blessed;

            this.m_LabelNumber = labelNumber;
        }

        public RewardRobe(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((int)this.m_LabelNumber);
            writer.Write((bool)this.m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_LabelNumber = reader.ReadInt();
                        this.m_IsRewardItem = reader.ReadBool();
                        break;
                    }
            }

            if (this.Parent is Mobile)
                ((Mobile)this.Parent).VirtualArmorMod += 2;
        }
    }

    [Flipable]
    public class RewardDress : BaseOuterTorso, IRewardItem
    {
        private int m_LabelNumber;
        private bool m_IsRewardItem;

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

        [CommandProperty(AccessLevel.GameMaster)]
        public int Number
        {
            get
            {
                return this.m_LabelNumber;
            }
            set
            {
                this.m_LabelNumber = value;
                this.InvalidateProperties();
            }
        }

        public override int LabelNumber
        {
            get
            {
                if (this.m_LabelNumber > 0)
                    return this.m_LabelNumber;

                return base.LabelNumber;
            }
        }

        public override int BasePhysicalResistance
        {
            get
            {
                return 3;
            }
        }

        public override void OnAdded(object parent)
        {
            base.OnAdded(parent);

            if (parent is Mobile)
                ((Mobile)parent).VirtualArmorMod += 2;
        }

        public override void OnRemoved(object parent)
        {
            base.OnRemoved(parent);

            if (parent is Mobile)
                ((Mobile)parent).VirtualArmorMod -= 2;
        }

        public override bool Dye(Mobile from, DyeTub sender)
        {
            from.SendLocalizedMessage(sender.FailMessage);
            return false;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (this.m_IsRewardItem)
                list.Add(RewardSystem.GetRewardYearLabel(this, new object[] { this.Hue, this.m_LabelNumber })); // X Year Veteran Reward
        }

        public override bool CanEquip(Mobile m)
        {
            if (!base.CanEquip(m))
                return false;

            return !this.m_IsRewardItem || RewardSystem.CheckIsUsableBy(m, this, new object[] { this.Hue, this.m_LabelNumber });
        }

        [Constructable]
        public RewardDress()
            : this(0)
        {
        }

        [Constructable]
        public RewardDress(int hue)
            : this(hue, 0)
        {
        }

        [Constructable]
        public RewardDress(int hue, int labelNumber)
            : base(0x1F01, hue)
        {
            this.Weight = 2.0;
            this.LootType = LootType.Blessed;

            this.m_LabelNumber = labelNumber;
        }

        public RewardDress(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((int)this.m_LabelNumber);
            writer.Write((bool)this.m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_LabelNumber = reader.ReadInt();
                        this.m_IsRewardItem = reader.ReadBool();
                        break;
                    }
            }

            if (this.Parent is Mobile)
                ((Mobile)this.Parent).VirtualArmorMod += 2;
        }
    }

    [Flipable]
    public class Robe : BaseOuterTorso, IArcaneEquip
    {
        #region Arcane Impl
        private int m_MaxArcaneCharges, m_CurArcaneCharges;

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxArcaneCharges
        {
            get
            {
                return this.m_MaxArcaneCharges;
            }
            set
            {
                this.m_MaxArcaneCharges = value;
                this.InvalidateProperties();
                this.Update();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CurArcaneCharges
        {
            get
            {
                return this.m_CurArcaneCharges;
            }
            set
            {
                this.m_CurArcaneCharges = value;
                this.InvalidateProperties();
                this.Update();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsArcane
        {
            get
            {
                return (this.m_MaxArcaneCharges > 0 && this.m_CurArcaneCharges >= 0);
            }
        }

        public void Update()
        {
            if (this.IsArcane)
                this.ItemID = 0x26AE;
            else if (this.ItemID == 0x26AE)
                this.ItemID = 0x1F04;

            if (this.IsArcane && this.CurArcaneCharges == 0)
                this.Hue = 0;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (this.IsArcane)
                list.Add(1061837, "{0}\t{1}", this.m_CurArcaneCharges, this.m_MaxArcaneCharges); // arcane charges: ~1_val~ / ~2_val~
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            if (this.IsArcane)
                this.LabelTo(from, 1061837, String.Format("{0}\t{1}", this.m_CurArcaneCharges, this.m_MaxArcaneCharges));
        }

        public void Flip()
        {
            if (this.ItemID == 0x1F03)
                this.ItemID = 0x1F04;
            else if (this.ItemID == 0x1F04)
                this.ItemID = 0x1F03;
        }

        #endregion

        [Constructable]
        public Robe()
            : this(0)
        {
        }

        [Constructable]
        public Robe(int hue)
            : base(0x1F03, hue)
        {
            this.Weight = 3.0;
        }

        public Robe(Serial serial)
            : base(serial)
        {
        }

        public override bool CanBeWornByGargoyles { get { return true; } }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            if (this.IsArcane)
            {
                writer.Write(true);
                writer.Write((int)this.m_CurArcaneCharges);
                writer.Write((int)this.m_MaxArcaneCharges);
            }
            else
            {
                writer.Write(false);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                    {
                        if (reader.ReadBool())
                        {
                            this.m_CurArcaneCharges = reader.ReadInt();
                            this.m_MaxArcaneCharges = reader.ReadInt();

                            if (this.Hue == 2118)
                                this.Hue = ArcaneGem.DefaultArcaneHue;
                        }

                        break;
                    }
            }
        }
    }

    public class MonkRobe : BaseOuterTorso
    {
        [Constructable]
        public MonkRobe()
            : this(0x21E)
        {
        }

        [Constructable]
        public MonkRobe(int hue)
            : base(0x2687, hue)
        {
            this.Weight = 1.0;
            this.StrRequirement = 0;
        }

        public override int LabelNumber
        {
            get
            {
                return 1076584;
            }
        }// A monk's robe
        public override bool CanBeBlessed
        {
            get
            {
                return false;
            }
        }
        public override bool Dye(Mobile from, DyeTub sender)
        {
            from.SendLocalizedMessage(sender.FailMessage);
            return false;
        }

        public MonkRobe(Serial serial)
            : base(serial)
        {
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

    [Flipable(0x1f01, 0x1f02)]
    public class PlainDress : BaseOuterTorso
    {
        [Constructable]
        public PlainDress()
            : this(0)
        {
        }

        [Constructable]
        public PlainDress(int hue)
            : base(0x1F01, hue)
        {
            this.Weight = 2.0;
        }

        public PlainDress(Serial serial)
            : base(serial)
        {
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

            if (this.Weight == 3.0)
                this.Weight = 2.0;
        }
    }

    [Flipable(0x2799, 0x27E4)]
    public class Kamishimo : BaseOuterTorso
    {
        [Constructable]
        public Kamishimo()
            : this(0)
        {
        }

        [Constructable]
        public Kamishimo(int hue)
            : base(0x2799, hue)
        {
            this.Weight = 3.0;
        }

        public Kamishimo(Serial serial)
            : base(serial)
        {
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

    [Flipable(0x279C, 0x27E7)]
    public class HakamaShita : BaseOuterTorso
    {
        [Constructable]
        public HakamaShita()
            : this(0)
        {
        }

        [Constructable]
        public HakamaShita(int hue)
            : base(0x279C, hue)
        {
            this.Weight = 3.0;
        }

        public HakamaShita(Serial serial)
            : base(serial)
        {
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

    [Flipable(0x2782, 0x27CD)]
    public class MaleKimono : BaseOuterTorso
    {
        [Constructable]
        public MaleKimono()
            : this(0)
        {
        }

        [Constructable]
        public MaleKimono(int hue)
            : base(0x2782, hue)
        {
            this.Weight = 3.0;
        }

        public override bool AllowFemaleWearer
        {
            get
            {
                return false;
            }
        }

        public MaleKimono(Serial serial)
            : base(serial)
        {
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

    [Flipable(0x2783, 0x27CE)]
    public class FemaleKimono : BaseOuterTorso
    {
        [Constructable]
        public FemaleKimono()
            : this(0)
        {
        }

        [Constructable]
        public FemaleKimono(int hue)
            : base(0x2783, hue)
        {
            this.Weight = 3.0;
        }

        public override bool AllowMaleWearer
        {
            get
            {
                return false;
            }
        }

        public FemaleKimono(Serial serial)
            : base(serial)
        {
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

    [Flipable(0x2FB9, 0x3173)]
    public class MaleElvenRobe : BaseOuterTorso
    {
        public override Race RequiredRace
        {
            get
            {
                return Race.Elf;
            }
        }

        [Constructable]
        public MaleElvenRobe()
            : this(0)
        {
        }

        [Constructable]
        public MaleElvenRobe(int hue)
            : base(0x2FB9, hue)
        {
            this.Weight = 2.0;
        }

        public MaleElvenRobe(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    [Flipable(0x2FBA, 0x3174)]
    public class FemaleElvenRobe : BaseOuterTorso
    {
        public override Race RequiredRace
        {
            get
            {
                return Race.Elf;
            }
        }
        [Constructable]
        public FemaleElvenRobe()
            : this(0)
        {
        }

        [Constructable]
        public FemaleElvenRobe(int hue)
            : base(0x2FBA, hue)
        {
            this.Weight = 2.0;
        }

        public override bool AllowMaleWearer
        {
            get
            {
                return false;
            }
        }

        public FemaleElvenRobe(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}