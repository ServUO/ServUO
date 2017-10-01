using System;
using System.Collections;
using Server.Network;
using Server.Gumps;

namespace Server.Items
{
    [FlipableAttribute(0x100A/*East*/, 0x100B/*South*/)]
    public class ArcheryButte : AddonComponent
    {
        private static readonly TimeSpan UseDelay = TimeSpan.FromSeconds(2.0);
        private double m_MinSkill;
        private double m_MaxSkill;
        private int m_Arrows, m_Bolts;
        private DateTime m_LastUse;
        private Hashtable m_Entries;
        [Constructable]
        public ArcheryButte()
            : this(0x100A)
        {
        }

        public ArcheryButte(int itemID)
            : base(itemID)
        {
            this.m_MinSkill = -25.0;
            this.m_MaxSkill = +25.0;
        }

        public ArcheryButte(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double MinSkill
        {
            get
            {
                return this.m_MinSkill;
            }
            set
            {
                this.m_MinSkill = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public double MaxSkill
        {
            get
            {
                return this.m_MaxSkill;
            }
            set
            {
                this.m_MaxSkill = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime LastUse
        {
            get
            {
                return this.m_LastUse;
            }
            set
            {
                this.m_LastUse = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool FacingEast
        {
            get
            {
                return (this.ItemID == 0x100A);
            }
            set
            {
                this.ItemID = value ? 0x100A : 0x100B;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Arrows
        {
            get
            {
                return this.m_Arrows;
            }
            set
            {
                this.m_Arrows = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Bolts
        {
            get
            {
                return this.m_Bolts;
            }
            set
            {
                this.m_Bolts = value;
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if ((from.Weapon is Boomerang || from.Weapon is Cyclone || from.Weapon is BaseThrown) && from.InRange(this.GetWorldLocation(), 1))
                this.Fire(from);
            if ((this.m_Arrows > 0 || this.m_Bolts > 0) && from.InRange(this.GetWorldLocation(), 1))
                this.Gather(from);
            else
                this.Fire(from);
        }

        public void Gather(Mobile from)
        {
            from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 500592); // You gather the arrows and bolts.

            if (this.m_Arrows > 0)
                from.AddToBackpack(new Arrow(this.m_Arrows));

            if (this.m_Bolts > 0)
                from.AddToBackpack(new Bolt(this.m_Bolts));

            this.m_Arrows = 0;
            this.m_Bolts = 0;

            this.m_Entries = null;
        }

        public void Fire(Mobile from)
        {
            BaseRanged bow = from.Weapon as BaseRanged;
            BaseThrown trow = from.Weapon as BaseThrown;

            if (bow == null && trow == null)
            {
                this.SendLocalizedMessageTo(from, 500593); // You must practice with ranged weapons on this.
                return;
            }

            if (DateTime.UtcNow < (this.m_LastUse + UseDelay))
                return;

            Point3D worldLoc = this.GetWorldLocation();

            if (this.FacingEast ? from.X <= worldLoc.X : from.Y <= worldLoc.Y)
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 500596); // You would do better to stand in front of the archery butte.
                return;
            }

            if (this.FacingEast ? from.Y != worldLoc.Y : from.X != worldLoc.X)
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 500597); // You aren't properly lined up with the archery butte to get an accurate shot.
                return;
            }

            if (!from.InRange(worldLoc, 6))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 500598); // You are too far away from the archery butte to get an accurate shot.
                return;
            }
            else if (from.InRange(worldLoc, 4))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 500599); // You are too close to the target.
                return;
            }

            Container pack = from.Backpack;
            Type ammoType = bow.AmmoType;

            bool isArrow = (ammoType == typeof(Arrow));
            bool isBolt = (ammoType == typeof(Bolt));
            bool isKnown = (isArrow || isBolt);

            if (from.Weapon != trow && (pack == null || !pack.ConsumeTotal(ammoType, 1)))
            {
                if (isArrow)
                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 500594); // You do not have any arrows with which to practice.
                else if (isBolt)
                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 500595); // You do not have any crossbow bolts with which to practice.
                else
                    this.SendLocalizedMessageTo(from, 500593); // You must practice with ranged weapons on this.

                return;
            }

            this.m_LastUse = DateTime.UtcNow;

            if (from.Weapon == trow)
            {
                from.MovingEffect(this, trow.EffectID, 18, 1, false, false);
                from.Direction = from.GetDirectionTo(this.GetWorldLocation());
                trow.PlaySwingAnimation(from);
            }

            else
            {
                from.Direction = from.GetDirectionTo(this.GetWorldLocation());
                bow.PlaySwingAnimation(from);
                from.MovingEffect(this, bow.EffectID, 18, 1, false, false);
            }

            ScoreEntry se = this.GetEntryFor(from);

            if (from.Weapon == trow)
            {
                if (!from.CheckSkill(trow.Skill, this.m_MinSkill, this.m_MaxSkill))
                {
                    from.PlaySound(trow.MissSound);

                    this.PublicOverheadMessage(MessageType.Regular, 0x3B2, 500604, from.Name); // You miss the target altogether.

                    se.Record(0);

                    if (se.Count == 1)
                        this.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1062719, se.Total.ToString());
                    else
                        this.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1042683, String.Format("{0}\t{1}", se.Total, se.Count));

                    return;
                }
            }
            else if (from.Weapon == bow)
            {
                if (!from.CheckSkill(bow.Skill, this.m_MinSkill, this.m_MaxSkill))
                {
                    from.PlaySound(bow.MissSound);

                    this.PublicOverheadMessage(MessageType.Regular, 0x3B2, 500604, from.Name); // You miss the target altogether.

                    se.Record(0);

                    if (se.Count == 1)
                        this.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1062719, se.Total.ToString());
                    else
                        this.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1042683, String.Format("{0}\t{1}", se.Total, se.Count));

                    return;
                }
            }
            Effects.PlaySound(this.Location, this.Map, 0x2B1);

            double rand = Utility.RandomDouble();

            int area, score, splitScore;

            if (0.10 > rand)
            {
                area = 0; // bullseye
                score = 50;
                splitScore = 100;
            }
            else if (0.25 > rand)
            {
                area = 1; // inner ring
                score = 10;
                splitScore = 20;
            }
            else if (0.50 > rand)
            {
                area = 2; // middle ring
                score = 5;
                splitScore = 15;
            }
            else
            {
                area = 3; // outer ring
                score = 2;
                splitScore = 5;
            }

            bool split = (isKnown && ((this.m_Arrows + this.m_Bolts) * 0.02) > Utility.RandomDouble());

            if (split)
            {
                this.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1010027 + area, String.Format("{0}\t{1}", from.Name, isArrow ? "arrow" : "bolt"));
            }
            else
            {
                this.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1010035 + area, from.Name);

                if (isArrow)
                    ++this.m_Arrows;
                else if (isBolt)
                    ++this.m_Bolts;
            }

            se.Record(split ? splitScore : score);

            if (se.Count == 1)
                this.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1062719, se.Total.ToString());
            else
                this.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1042683, String.Format("{0}\t{1}", se.Total, se.Count));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);

            writer.Write(this.m_MinSkill);
            writer.Write(this.m_MaxSkill);
            writer.Write(this.m_Arrows);
            writer.Write(this.m_Bolts);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        this.m_MinSkill = reader.ReadDouble();
                        this.m_MaxSkill = reader.ReadDouble();
                        this.m_Arrows = reader.ReadInt();
                        this.m_Bolts = reader.ReadInt();

                        if (this.m_MinSkill == 0.0 && this.m_MaxSkill == 30.0)
                        {
                            this.m_MinSkill = -25.0;
                            this.m_MaxSkill = +25.0;
                        }

                        break;
                    }
            }
        }

        private ScoreEntry GetEntryFor(Mobile from)
        {
            if (this.m_Entries == null)
                this.m_Entries = new Hashtable();

            ScoreEntry e = (ScoreEntry)this.m_Entries[from];

            if (e == null)
                this.m_Entries[from] = e = new ScoreEntry();

            return e;
        }

        private class ScoreEntry
        {
            private int m_Total;
            private int m_Count;
            public ScoreEntry()
            {
            }

            public int Total
            {
                get
                {
                    return this.m_Total;
                }
                set
                {
                    this.m_Total = value;
                }
            }
            public int Count
            {
                get
                {
                    return this.m_Count;
                }
                set
                {
                    this.m_Count = value;
                }
            }
            public void Record(int score)
            {
                this.m_Total += score;
                this.m_Count += 1;
            }
        }
    }

    public class ArcheryButteAddon : BaseAddon
    {
        [Constructable]
        public ArcheryButteAddon(AddonFacing facing)
        {
            switch (facing)
            {
                case AddonFacing.East:
                    AddComponent(new ArcheryButte(0x100A), 0, 0, 0);
                    break;
                case AddonFacing.South:
                    AddComponent(new ArcheryButte(0x100B), 0, 0, 0);
                    break;
            }
        }

        public ArcheryButteAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new ArcheryButteDeed();
            }
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

    public class ArcheryButteDeed : BaseAddonDeed, IRewardOption
    {
        private AddonFacing Facing { get; set; }

        [Constructable]
        public ArcheryButteDeed()
        {
        }

        public ArcheryButteDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new ArcheryButteAddon(Facing);
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1080205;
            }
        }// archery butte

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(RewardOptionGump));
                from.SendGump(new RewardOptionGump(this));
            }
            else
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.       	
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

        public void GetOptions(RewardOptionList list)
        {
            list.Add((int)AddonFacing.South, 1080204);
            list.Add((int)AddonFacing.East, 1080203);
        }


        public void OnOptionSelected(Mobile from, int choice)
        {
            Facing = (AddonFacing)choice;

            if (!Deleted)
                base.OnDoubleClick(from);
        }
    }
}