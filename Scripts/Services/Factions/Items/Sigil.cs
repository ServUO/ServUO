using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Factions
{
    public class Sigil : BaseSystemController
    {
        public const int OwnershipHue = 0xB;

        // ?? time corrupting faction has to return the sigil before corruption time resets ?
        public static readonly TimeSpan CorruptionGrace = TimeSpan.FromMinutes((Core.SE) ? 30.0 : 15.0);

        // Sigil must be held at a stronghold for this amount of time in order to become corrupted
        public static readonly TimeSpan CorruptionPeriod = ((Core.SE) ? TimeSpan.FromHours(10.0) : TimeSpan.FromHours(24.0)); 

        // After a sigil has been corrupted it must be returned to the town within this period of time
        public static readonly TimeSpan ReturnPeriod = TimeSpan.FromHours(1.0);

        // Once it's been returned the corrupting faction owns the town for this period of time
        public static readonly TimeSpan PurificationPeriod = TimeSpan.FromDays(3.0);

        private BaseMonolith m_LastMonolith;

        private Town m_Town;
        private Faction m_Corrupted;
        private Faction m_Corrupting;

        private DateTime m_LastStolen;
        private DateTime m_GraceStart;
        private DateTime m_CorruptionStart;
        private DateTime m_PurificationStart;

        [CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
        public DateTime LastStolen
        {
            get
            {
                return this.m_LastStolen;
            }
            set
            {
                this.m_LastStolen = value;
            }
        }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
        public DateTime GraceStart
        {
            get
            {
                return this.m_GraceStart;
            }
            set
            {
                this.m_GraceStart = value;
            }
        }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
        public DateTime CorruptionStart
        {
            get
            {
                return this.m_CorruptionStart;
            }
            set
            {
                this.m_CorruptionStart = value;
            }
        }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
        public DateTime PurificationStart
        {
            get
            {
                return this.m_PurificationStart;
            }
            set
            {
                this.m_PurificationStart = value;
            }
        }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
        public Town Town
        {
            get
            {
                return this.m_Town;
            }
            set
            {
                this.m_Town = value;
                this.Update();
            }
        }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
        public Faction Corrupted
        {
            get
            {
                return this.m_Corrupted;
            }
            set
            {
                this.m_Corrupted = value;
                this.Update();
            }
        }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
        public Faction Corrupting
        {
            get
            {
                return this.m_Corrupting;
            }
            set
            {
                this.m_Corrupting = value;
                this.Update();
            }
        }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
        public BaseMonolith LastMonolith
        {
            get
            {
                return this.m_LastMonolith;
            }
            set
            {
                this.m_LastMonolith = value;
            }
        }

        [CommandProperty(AccessLevel.Counselor)]
        public bool IsBeingCorrupted
        {
            get
            {
                return (this.m_LastMonolith is StrongholdMonolith && this.m_LastMonolith.Faction == this.m_Corrupting && this.m_Corrupting != null);
            }
        }

        [CommandProperty(AccessLevel.Counselor)]
        public bool IsCorrupted
        {
            get
            {
                return (this.m_Corrupted != null);
            }
        }

        [CommandProperty(AccessLevel.Counselor)]
        public bool IsPurifying
        {
            get
            {
                return (this.m_PurificationStart != DateTime.MinValue);
            }
        }

        [CommandProperty(AccessLevel.Counselor)]
        public bool IsCorrupting
        {
            get
            {
                return (this.m_Corrupting != null && this.m_Corrupting != this.m_Corrupted);
            }
        }

        public void Update()
        {
            this.ItemID = (this.m_Town == null ? 0x1869 : this.m_Town.Definition.SigilID);

            if (this.m_Town == null)
                this.AssignName(null);
            else if (this.IsCorrupted || this.IsPurifying)
                this.AssignName(this.m_Town.Definition.CorruptedSigilName);
            else
                this.AssignName(this.m_Town.Definition.SigilName);

            this.InvalidateProperties();
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (this.IsCorrupted)
                TextDefinition.AddTo(list, this.m_Corrupted.Definition.SigilControl);
            else
                list.Add(1042256); // This sigil is not corrupted.

            if (this.IsCorrupting)
                list.Add(1042257); // This sigil is in the process of being corrupted.
            else if (this.IsPurifying)
                list.Add(1042258); // This sigil has recently been corrupted, and is undergoing purification.
            else
                list.Add(1042259); // This sigil is not in the process of being corrupted.
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            if (this.IsCorrupted)
            {
                if (this.m_Corrupted.Definition.SigilControl.Number > 0)
                    this.LabelTo(from, this.m_Corrupted.Definition.SigilControl.Number);
                else if (this.m_Corrupted.Definition.SigilControl.String != null)
                    this.LabelTo(from, this.m_Corrupted.Definition.SigilControl.String);
            }
            else
            {
                this.LabelTo(from, 1042256); // This sigil is not corrupted.
            }

            if (this.IsCorrupting)
                this.LabelTo(from, 1042257); // This sigil is in the process of being corrupted.
            else if (this.IsPurifying)
                this.LabelTo(from, 1042258); // This sigil has been recently corrupted, and is undergoing purification.
            else
                this.LabelTo(from, 1042259); // This sigil is not in the process of being corrupted.
        }

        public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
        {
            from.SendLocalizedMessage(1005225); // You must use the stealing skill to pick up the sigil
            return false;
        }

        private Mobile FindOwner(object parent)
        {
            if (parent is Item)
                return ((Item)parent).RootParent as Mobile;

            if (parent is Mobile)
                return (Mobile)parent;

            return null;
        }

        public override void OnAdded(object parent)
        {
            base.OnAdded(parent);

            Mobile mob = this.FindOwner(parent);

            if (mob != null)
                mob.SolidHueOverride = OwnershipHue;
        }

        public override void OnRemoved(object parent)
        {
            base.OnRemoved(parent);

            Mobile mob = this.FindOwner(parent);

            if (mob != null)
                mob.SolidHueOverride = -1;
        }

        public Sigil(Town town)
            : base(0x1869)
        {
            this.Movable = false;
            this.Town = town;

            m_Sigils.Add(this);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.IsChildOf(from.Backpack))
            {
                from.BeginTarget(1, false, Targeting.TargetFlags.None, new TargetCallback(Sigil_OnTarget));
                from.SendLocalizedMessage(1042251); // Click on a sigil monolith or player
            }
        }

        public static bool ExistsOn(Mobile mob, bool vvvOnly = false)
        {
            Container pack = mob.Backpack;

            if(vvvOnly && pack.FindItemByType(typeof(Server.Engines.VvV.VvVSigil)) != null)
                return true;

            return (pack != null && (pack.FindItemByType(typeof(Sigil)) != null || pack.FindItemByType(typeof(Server.Engines.VvV.VvVSigil)) != null));
        }

        private void BeginCorrupting(Faction faction)
        {
            this.m_Corrupting = faction;
            this.m_CorruptionStart = DateTime.UtcNow;
        }

        private void ClearCorrupting()
        {
            this.m_Corrupting = null;
            this.m_CorruptionStart = DateTime.MinValue;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan TimeUntilCorruption
        {
            get
            {
                if (!this.IsBeingCorrupted)
                    return TimeSpan.Zero;

                TimeSpan ts = (this.m_CorruptionStart + CorruptionPeriod) - DateTime.UtcNow;

                if (ts < TimeSpan.Zero)
                    ts = TimeSpan.Zero;

                return ts;
            }
        }

        private void Sigil_OnTarget(Mobile from, object obj)
        {
            if (this.Deleted || !this.IsChildOf(from.Backpack))
                return;

            #region Give To Mobile
            if (obj is Mobile)
            {
                if (obj is PlayerMobile)
                {
                    PlayerMobile targ = (PlayerMobile)obj;

                    Faction toFaction = Faction.Find(targ);
                    Faction fromFaction = Faction.Find(from);

                    if (toFaction == null)
                        from.SendLocalizedMessage(1005223); // You cannot give the sigil to someone not in a faction
                    else if (fromFaction != toFaction)
                        from.SendLocalizedMessage(1005222); // You cannot give the sigil to someone not in your faction
                    else if (Sigil.ExistsOn(targ))
                        from.SendLocalizedMessage(1005220); // You cannot give this sigil to someone who already has a sigil
                    else if (!targ.Alive)
                        from.SendLocalizedMessage(1042248); // You cannot give a sigil to a dead person.
                    else if (from.NetState != null && targ.NetState != null)
                    {
                        Container pack = targ.Backpack;

                        if (pack != null)
                            pack.DropItem(this);
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1005221); //You cannot give the sigil to them
                }
            }
            #endregion
            else if (obj is BaseMonolith)
            {
                #region Put in Stronghold
                if (obj is StrongholdMonolith)
                {
                    StrongholdMonolith m = (StrongholdMonolith)obj;

                    if (m.Faction == null || m.Faction != Faction.Find(from))
                        from.SendLocalizedMessage(1042246); // You can't place that on an enemy monolith
                    else if (m.Town == null || m.Town != this.m_Town)
                        from.SendLocalizedMessage(1042247); // That is not the correct faction monolith
                    else
                    {
                        m.Sigil = this;

                        Faction newController = m.Faction;
                        Faction oldController = this.m_Corrupting;

                        if (oldController == null)
                        {
                            if (this.m_Corrupted != newController)
                                this.BeginCorrupting(newController);
                        }
                        else if (this.m_GraceStart > DateTime.MinValue && (this.m_GraceStart + CorruptionGrace) < DateTime.UtcNow)
                        {
                            if (this.m_Corrupted != newController)
                                this.BeginCorrupting(newController); // grace time over, reset period
                            else
                                this.ClearCorrupting();

                            this.m_GraceStart = DateTime.MinValue;
                        }
                        else if (newController == oldController)
                        {
                            this.m_GraceStart = DateTime.MinValue; // returned within grace period
                        }
                        else if (this.m_GraceStart == DateTime.MinValue)
                        {
                            this.m_GraceStart = DateTime.UtcNow;
                        }

                        this.m_PurificationStart = DateTime.MinValue;
                    }
                }
                #endregion
                #region Put in Town
                else if (obj is TownMonolith)
                {
                    TownMonolith m = (TownMonolith)obj;

                    if (m.Town == null || m.Town != this.m_Town)
                        from.SendLocalizedMessage(1042245); // This is not the correct town sigil monolith
                    else if (this.m_Corrupted == null || this.m_Corrupted != Faction.Find(from))
                        from.SendLocalizedMessage(1042244); // Your faction did not corrupt this sigil.  Take it to your stronghold.
                    else
                    {
                        m.Sigil = this;

                        this.m_Corrupting = null;
                        this.m_PurificationStart = DateTime.UtcNow;
                        this.m_CorruptionStart = DateTime.MinValue;

                        this.m_Town.Capture(this.m_Corrupted);
                        this.m_Corrupted = null;
                    }
                }
                #endregion
            }
            else
            {
                from.SendLocalizedMessage(1005224);	//	You can't use the sigil on that 
            }

            this.Update();
        }

        public Sigil(Serial serial)
            : base(serial)
        {
            m_Sigils.Add(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            Town.WriteReference(writer, this.m_Town);
            Faction.WriteReference(writer, this.m_Corrupted);
            Faction.WriteReference(writer, this.m_Corrupting);

            writer.Write((Item)this.m_LastMonolith);

            writer.Write(this.m_LastStolen);
            writer.Write(this.m_GraceStart);
            writer.Write(this.m_CorruptionStart);
            writer.Write(this.m_PurificationStart);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_Town = Town.ReadReference(reader);
                        this.m_Corrupted = Faction.ReadReference(reader);
                        this.m_Corrupting = Faction.ReadReference(reader);

                        this.m_LastMonolith = reader.ReadItem() as BaseMonolith;

                        this.m_LastStolen = reader.ReadDateTime();
                        this.m_GraceStart = reader.ReadDateTime();
                        this.m_CorruptionStart = reader.ReadDateTime();
                        this.m_PurificationStart = reader.ReadDateTime();

                        this.Update();

                        Mobile mob = this.RootParent as Mobile;

                        if (mob != null)
                            mob.SolidHueOverride = OwnershipHue;

                        break;
                    }
            }
        }

        public bool ReturnHome()
        {
            BaseMonolith monolith = this.m_LastMonolith;

            if (monolith == null && this.m_Town != null)
                monolith = this.m_Town.Monolith;

            if (monolith != null && !monolith.Deleted)
                monolith.Sigil = this;

            return (monolith != null && !monolith.Deleted);
        }

        public override void OnParentDeleted(object parent)
        {
            base.OnParentDeleted(parent);

            this.ReturnHome();
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            m_Sigils.Remove(this);
        }

        public override void Delete()
        {
            if (this.ReturnHome())
                return;

            base.Delete();
        }

        private static readonly List<Sigil> m_Sigils = new List<Sigil>();

        public static List<Sigil> Sigils
        {
            get
            {
                return m_Sigils;
            }
        }
    }
}