using System;
using System.Collections;
using Server.Items;
using Server.Network;

namespace Server.Engines.XmlSpawner2
{
    public class XmlSiege : XmlAttachment
    {
        public bool NeedsEffectsUpdate = true;
        public bool BeingRepaired;
        private const int LightDamageColor = 165;// color at 67-99% of hitsmax
        private const int MediumDamageColor = 53;// color at 34-66% of hitsmax
        private const int HeavyDamageColor = 33;// color at 1-33% of hitsmax
        private const int ShowSiegeColor = 0;// color used to flag items with siege attachments
        private int m_Hits = 1000;// current hits
        private int m_HitsMax = 1000;// max hits
        private int m_ResistFire = 30;// percentage resistance to fire attacks
        private int m_ResistPhysical = 30;// percentage resistance to physical attacks
        private int m_Stone = 1;// amount of stone required per repair
        private int m_Iron = 20;// amount of iron required per repair
        private int m_Wood = 20;// amount of wood required per repair
        private int m_DestroyedItemID = 10984;// itemid used when hits go to zero. 2322=dirt patch, 10984 pulsing pool.  Specifying a value of zero will cause the object to be permanently destroyed.
        private TimeSpan m_AutoRepairTime = TimeSpan.Zero;// autorepair disabled by default
        private bool m_Enabled = true;// allows enabling/disabling siege damage and its effects
        private DateTime m_AutoRepairEnd;
        private AutoRepairTimer m_AutoRepairTimer;
        private ArrayList m_OriginalItemIDList = new ArrayList();// original itemids of parent item
        private ArrayList m_OriginalHueList = new ArrayList();// original hues of parent item
        private EffectsTimer m_EffectsTimer;
        // a serial constructor is REQUIRED
        public XmlSiege(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public XmlSiege()
        {
        }

        [Attachable]
        public XmlSiege(int hitsmax)
        {
            this.HitsMax = hitsmax;
            this.m_Hits = this.HitsMax;
        }

        [Attachable]
        public XmlSiege(int hitsmax, int destroyeditemid)
        {
            this.HitsMax = hitsmax;
            this.m_Hits = this.HitsMax;
            this.DestroyedItemID = destroyeditemid;
        }

        [Attachable]
        public XmlSiege(int hitsmax, int resistfire, int resistphysical)
        {
            this.HitsMax = hitsmax;
            this.m_Hits = this.HitsMax;
            this.ResistPhysical = resistphysical;
            this.ResistFire = resistfire;
        }

        [Attachable]
        public XmlSiege(int hitsmax, int resistfire, int resistphysical, int destroyeditemid)
        {
            this.HitsMax = hitsmax;
            this.m_Hits = this.HitsMax;
            this.ResistPhysical = resistphysical;
            this.ResistFire = resistfire;
            this.DestroyedItemID = destroyeditemid;
        }

        [Attachable]
        public XmlSiege(int hitsmax, int resistfire, int resistphysical, int wood, int iron, int stone)
        {
            this.HitsMax = hitsmax;
            this.m_Hits = this.HitsMax;
            this.ResistPhysical = resistphysical;
            this.ResistFire = resistfire;
            this.Wood = wood;
            this.Iron = iron;
            this.Stone = stone;
        }

        [Attachable]
        public XmlSiege(int hitsmax, int resistfire, int resistphysical, int wood, int iron, int stone, int destroyeditemid)
        {
            this.HitsMax = hitsmax;
            this.m_Hits = this.HitsMax;
            this.ResistPhysical = resistphysical;
            this.ResistFire = resistfire;
            this.Wood = wood;
            this.Iron = iron;
            this.Stone = stone;
            this.DestroyedItemID = destroyeditemid;
        }

        public virtual int LightDamageEffectID
        {
            get
            {
                return 14732;
            }
        }// 14732 = flame effect
        public virtual int MediumDamageEffectID
        {
            get
            {
                return 14732;
            }
        }
        public virtual int HeavyDamageEffectID
        {
            get
            {
                return 14732;
            }
        }
        public virtual int DamagedItemEffectDuration
        {
            get
            {
                return 110;
            }
        }
        public virtual bool UseEffectsDamageIndicator
        {
            get
            {
                return true;
            }
        }// show damage using location effects
        public virtual bool UseColorDamageIndicator
        {
            get
            {
                return false;
            }
        }// show damage using item rehueing
        public virtual int WhenToAutoRepair
        {
            get
            {
                return 0;
            }
        }// 0=never, 1=after any damage, 2=after being destroyed
        public virtual double AutoRepairFactor
        {
            get
            {
                return 0.1;
            }
        }// fraction of HitsMax to repair on each autorepair OnTick. A value of 1 will fully repair.
        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan AutoRepairTime
        {
            get
            {
                return this.m_AutoRepairTime;
            }
            set
            {
                this.m_AutoRepairTime = value;

                // see if the object is already destroyed
                if ((this.Hits == 0 && this.WhenToAutoRepair == 2) || (this.Hits < this.HitsMax && this.WhenToAutoRepair == 1))
                {
                    this.DoAutoRepairTimer(value);
                }
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan NextAutoRepair
        {
            get
            {
                if (this.m_AutoRepairTimer != null && this.m_AutoRepairTimer.Running)
                    return this.m_AutoRepairEnd - DateTime.UtcNow;
                else
                    return TimeSpan.FromSeconds(0);
            }
            set
            {
                this.DoAutoRepairTimer(value);
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool Enabled
        {
            get
            {
                // disable outside of Felucca
                //if (AttachedTo is Item && ((Item)AttachedTo).Map != Map.Felucca) return false;
                return this.m_Enabled; 
            }
            set
            {
                this.m_Enabled = value;
                this.AdjustItemID();
                this.UpdateDamageIndicators();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int DestroyedItemID
        {
            get
            {
                return this.m_DestroyedItemID;
            }
            set
            {
                if (value >= 0)
                {
                    this.m_DestroyedItemID = value;
                    if (this.Hits == 0)
                        this.AdjustItemID();
                }
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int Iron
        {
            get
            {
                return this.m_Iron;
            }
            set
            {
                if (value >= 0)
                    this.m_Iron = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int Stone
        {
            get
            {
                return this.m_Stone;
            }
            set
            {
                if (value >= 0)
                    this.m_Stone = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int Wood
        {
            get
            {
                return this.m_Wood;
            }
            set
            {
                if (value >= 0)
                    this.m_Wood = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int ResistFire
        {
            get
            {
                return this.m_ResistFire;
            }
            set
            {
                if (value >= 0)
                    this.m_ResistFire = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int ResistPhysical
        {
            get
            {
                return this.m_ResistPhysical;
            }
            set
            {
                if (value >= 0)
                    this.m_ResistPhysical = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int HitsMax
        {
            get
            {
                return this.m_HitsMax;
            }
            set
            {
                if (value >= 0 && value != this.HitsMax)
                {
                    this.m_HitsMax = value;
                    if (this.m_Hits > this.m_HitsMax)
                    {
                        this.Hits = this.m_HitsMax;
                    }
                    // recalibrate damage indicators
                    this.UpdateDamageIndicators();
                }
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int Hits
        {
            get
            {
                return this.m_Hits;
            }
            set
            {
                int newvalue = value;
                int oldvalue = this.m_Hits;

                if (newvalue > this.HitsMax)
                    newvalue = this.HitsMax;
                if (newvalue < 0)
                    newvalue = 0;

                if (oldvalue != newvalue)
                {
                    this.m_Hits = newvalue;

                    // it is being destroyed 
                    if (newvalue == 0)
                    {
                        this.OnDestroyed();
                    }
                    else if (oldvalue == 0)
                    {
                        // if autorepair was active then stop it
                        if (this.m_AutoRepairTimer != null)
                            this.m_AutoRepairTimer.Stop();

                        // restore the itemids from the destroyed state
                        this.AdjustItemID();

                        // it is being restored from destroyed state so also refresh nearby mobile locations
                        // which may have to be changed do to the new itemids
                        this.AdjustMobileLocations();
                    }

                    // if it has taken damage and the autorepair feature is set to repair on damage
                    // then start the autorepair timer
                    if (this.WhenToAutoRepair == 1 && this.AutoRepairTime > TimeSpan.Zero && this.m_Hits != this.HitsMax)
                    {
                        this.DoAutoRepairTimer(this.AutoRepairTime);
                    }

                    // adjust the damage indicators on change in hits
                    this.UpdateDamageIndicators();
                }
            }
        }
        public override bool HandlesOnMovement
        {
            get
            {
                return (this.Hits != this.HitsMax) && this.Enabled;
            }
        }
        public static void SendMovingProjectileEffect(IEntity from, IEntity to, int itemID, Point3D fromPoint, Point3D toPoint, int speed, int duration, bool fixedDirection, bool explode, int hue)
        {
            if (from is Mobile)
                ((Mobile)from).ProcessDelta();

            if (to is Mobile)
                ((Mobile)to).ProcessDelta();

            Effects.SendPacket(from.Location, from.Map, new MovingProjectileEffect(from, to, itemID, fromPoint, toPoint, speed, duration, fixedDirection, explode, hue));
        }

        public static void Attack(Mobile from, object target, int firedamage, int physicaldamage)
        {
            // find the XmlSiege attachment on the target
            XmlSiege a = (XmlSiege)XmlAttach.FindAttachment(target, typeof(XmlSiege));

            if (a != null && !a.Deleted)
            {
                a.ApplyScaledDamage(from, firedamage, physicaldamage);
            }
        }

        public static int GetHits(object target)
        {
            // find the XmlSiege attachment on the target
            XmlSiege a = (XmlSiege)XmlAttach.FindAttachment(target, typeof(XmlSiege));

            if (a != null && !a.Deleted)
            {
                return a.Hits;
            }

            return -1;
        }

        public static int GetHitsMax(object target)
        {
            // find the XmlSiege attachment on the target
            XmlSiege a = (XmlSiege)XmlAttach.FindAttachment(target, typeof(XmlSiege));

            if (a != null && !a.Deleted)
            {
                return a.HitsMax;
            }

            return -1;
        }

        public static double GetDistance(Point3D p1, Point3D p2)
        {
            int xDelta = p1.X - p2.X;
            int yDelta = p1.Y - p2.Y;

            return Math.Sqrt((xDelta * xDelta) + (yDelta * yDelta));
        }

        public void DoAutoRepairTimer(TimeSpan delay)
        {
            this.m_AutoRepairEnd = DateTime.UtcNow + delay;

            if (this.m_AutoRepairTimer != null)
                this.m_AutoRepairTimer.Stop();

            this.m_AutoRepairTimer = new AutoRepairTimer(this, delay);
            this.m_AutoRepairTimer.Start();
        }

        public void DoEffectsTimer(TimeSpan delay)
        {
            if (this.m_EffectsTimer != null)
                this.m_EffectsTimer.Stop();

            this.m_EffectsTimer = new EffectsTimer(this, delay);
            this.m_EffectsTimer.Start();
        }

        public override void OnMovement(MovementEventArgs e)
        {
            base.OnMovement(e);

            if (this.UseEffectsDamageIndicator)
            {
                this.NeedsEffectsUpdate = true;

                // if the effects timer is not running
                if (this.m_EffectsTimer == null || !this.m_EffectsTimer.Running)
                {
                    // then update effects damage display
                    this.AdjustDamageEffects();
                }
            }
        }

        public virtual void ApplyScaledDamage(Mobile from, int firedamage, int physicaldamage)
        {
            if (!this.Enabled || this.Hits == 0)
                return;

            int firescale = 100 - this.ResistFire;
            int physicalscale = 100 - this.ResistPhysical;
            if (firescale < 0)
                firescale = 0;
            if (physicalscale < 0)
                physicalscale = 0;

            int scaleddamage = (firedamage * firescale + physicaldamage * physicalscale) / 100;

            // subtract the scaled damage from the current hits
            this.Hits -= scaleddamage;

            Item i = this.AttachedTo as Item;

            if (i != null)
            {
                // display the damage over the target
                // if it is an addon and invisible, then try displaying over a visible component
                if (i is BaseAddon && !i.Visible && ((BaseAddon)i).Components != null && ((BaseAddon)i).Components.Count > 0)
                {
                    foreach (AddonComponent c in ((BaseAddon)i).Components)
                    {
                        if (c != null && c.Visible)
                        {
                            c.PublicOverheadMessage(MessageType.Regular, 33, true, String.Format("{0}", scaleddamage));
                            break;
                        }
                    }
                }
                else
                {
                    i.PublicOverheadMessage(MessageType.Regular, 33, true, String.Format("{0}", scaleddamage));
                }

                if (from != null)
                {
                    from.SendMessage("You deliver {0} siege damage.", scaleddamage);
                }
            }
        }

        public virtual void OnDestroyed()
        {
            // change the itemid to reflect destroyed state
            this.AdjustItemID();

            // if autorepair is enabled, then start the autorepair timer
            if (this.WhenToAutoRepair > 0 && this.AutoRepairTime > TimeSpan.Zero)
            {
                this.DoAutoRepairTimer(this.AutoRepairTime);
            }
        }

        public void RestoreOriginalItemID(Item targetitem)
        {
            if (targetitem == null || targetitem.Deleted)
                return;

            if (this.m_OriginalItemIDList != null && this.m_OriginalItemIDList.Count > 0)
            {
                targetitem.ItemID = (int)this.m_OriginalItemIDList[0];
                if (targetitem is BaseAddon)
                {
                    BaseAddon addon = (BaseAddon)targetitem;
                    if (addon.Components != null)
                    {
                        int j = 1;
                        foreach (AddonComponent i in addon.Components)
                        {
                            if (j < this.m_OriginalItemIDList.Count)
                            {
                                i.ItemID = (int)this.m_OriginalItemIDList[j++];
                            }
                        }
                    }
                }
            }
        }

        public void StoreOriginalItemID(Item targetitem)
        {
            if (targetitem == null || targetitem.Deleted)
                return;

            this.m_OriginalItemIDList = new ArrayList();

            this.m_OriginalItemIDList.Add(targetitem.ItemID);

            if (targetitem is BaseAddon)
            {
                BaseAddon addon = (BaseAddon)targetitem;
                if (addon.Components != null)
                {
                    foreach (AddonComponent i in addon.Components)
                    {
                        this.m_OriginalItemIDList.Add(i.ItemID);
                    }
                }
            }
        }

        public virtual void AssignItemHue(Item targetitem, int hue)
        {
            if (targetitem == null || targetitem.Deleted)
                return;

            // change the target item hue
            targetitem.Hue = hue;

            // deal with addons
            if (targetitem is BaseAddon)
            {
                BaseAddon addon = (BaseAddon)targetitem;
                if (addon.Components != null)
                {
                    // change the ids of all of the components if they dont already have xmlsiege attachments
                    foreach (AddonComponent i in addon.Components)
                    {
                        if (XmlAttach.FindAttachment(i, typeof(XmlSiege)) == null)
                        {
                            i.Hue = hue;
                        }
                    }
                }
            }
        }

        public virtual void AssignItemEffect(Item targetitem, int effectid, int hue, int fraction)
        {
            if (targetitem == null || targetitem.Deleted)
                return;

            // deal with addons
            if (targetitem is BaseAddon)
            {
                BaseAddon addon = (BaseAddon)targetitem;
                if (addon.Components != null)
                {
                    int count = 0;
                    // change the ids of all of the components if they dont already have xmlsiege attachments
                    foreach (AddonComponent i in addon.Components)
                    {
                        if (XmlAttach.FindAttachment(i, typeof(XmlSiege)) == null)
                        {
                            // put the effect on a fraction of the components, but make sure you have at least one
                            if (Utility.Random(100) < fraction || count == 0)
                            {
                                Effects.SendLocationEffect(i.Location, i.Map, effectid, this.DamagedItemEffectDuration, hue, 0);
                                //Effects.SendTargetEffect(i, DamagedItemEffectID, DamagedItemEffectDuration, hue, 0);
                                count++;
                            }
                        }
                    }
                }
            }
            else if (targetitem is BaseMulti)
            {
                // place an effect at the location of the target item
                Effects.SendLocationEffect(targetitem.Location, targetitem.Map, effectid, this.DamagedItemEffectDuration, hue, 0);

                ArrayList tilelist = new ArrayList();
                // go through all of the multi components
                MultiComponentList mcl = ((BaseMulti)targetitem).Components;
                int count = 0;
                if (mcl != null && mcl.List != null)
                {
                    for (int i = 0; i < mcl.List.Length; i++)
                    {
                        MultiTileEntry t = mcl.List[i];

                        int x = t.m_OffsetX + targetitem.X;
                        int y = t.m_OffsetY + targetitem.Y;
                        int z = t.m_OffsetZ + targetitem.Z;
                        int itemID = t.m_ItemID & 0x3FFF;

                        if (Utility.Random(100) < fraction || count == 0)
                        {
                            tilelist.Add(new TileEntry(itemID, x, y, z));
                            count++;
                        }
                    }

                    foreach (TileEntry s in tilelist)
                    {
                        Effects.SendLocationEffect(new Point3D(s.X, s.Y, s.Z), targetitem.Map, effectid, this.DamagedItemEffectDuration, hue, 0);
                    }
                }
            }
            else
            {
                // place an effect at the location of the target item
                Effects.SendLocationEffect(targetitem.Location, targetitem.Map, effectid, this.DamagedItemEffectDuration, hue, 0);
                //Effects.SendTargetEffect(targetitem, DamagedItemEffectID, DamagedItemEffectDuration, hue, 0);
            }
        }

        public void RestoreOriginalHue(Item targetitem)
        {
            if (targetitem == null || targetitem.Deleted)
                return;

            if (this.m_OriginalHueList != null && this.m_OriginalHueList.Count > 0)
            {
                targetitem.Hue = (int)this.m_OriginalHueList[0];
                if (targetitem is BaseAddon)
                {
                    BaseAddon addon = (BaseAddon)targetitem;
                    if (addon.Components != null)
                    {
                        int j = 1;
                        foreach (AddonComponent i in addon.Components)
                        {
                            if (j < this.m_OriginalHueList.Count)
                            {
                                i.Hue = (int)this.m_OriginalHueList[j++];
                            }
                        }
                    }
                }
            }
        }

        public void StoreOriginalHue(Item targetitem)
        {
            if (targetitem == null || targetitem.Deleted)
                return;

            this.m_OriginalHueList = new ArrayList();

            this.m_OriginalHueList.Add(targetitem.Hue);

            if (targetitem is BaseAddon)
            {
                BaseAddon addon = (BaseAddon)targetitem;
                if (addon.Components != null)
                {
                    foreach (AddonComponent i in addon.Components)
                    {
                        this.m_OriginalHueList.Add(i.Hue);
                    }
                }
            }
        }

        public virtual void UpdateDamageIndicators()
        {
            // add colored effects at the item location to reflect damage
            if (this.UseEffectsDamageIndicator)
            {
                this.AdjustDamageEffects();
            }

            // set the item hue to reflect damage
            if (this.UseColorDamageIndicator)
            {
                this.AdjustDamageColor();
            }
        }

        public virtual void AdjustDamageEffects()
        {
            Item targetitem = this.AttachedTo as Item;

            if (targetitem == null)
                return;

            // set the color based on damage
            if (this.Hits == this.HitsMax || !this.Enabled)
            {
                // no effects
            }
            else
            {
                // start the timer and apply effects if a timer is not already running
                if (this.m_EffectsTimer == null || !this.m_EffectsTimer.Running)
                {
                    this.DoEffectsTimer(TimeSpan.FromSeconds(5.0));

                    // linear scaling of effects density 1-30% based on damage
                    int density = 29 * (this.HitsMax - this.Hits) / this.HitsMax + 1;

                    if (this.Hits < this.HitsMax && this.Hits > this.HitsMax * 0.66)
                    {
                        this.AssignItemEffect(targetitem, this.LightDamageEffectID, LightDamageColor, density);
                    }
                    else if (this.Hits <= this.HitsMax * 0.66 && this.Hits > this.HitsMax * 0.33)
                    {
                        this.AssignItemEffect(targetitem, this.MediumDamageEffectID, MediumDamageColor, density);
                    }
                    else if (this.Hits <= this.HitsMax * 0.33)
                    {
                        this.AssignItemEffect(targetitem, this.HeavyDamageEffectID, HeavyDamageColor, density);
                    }
                }
            }
        }

        public virtual void AdjustDamageColor()
        {
            Item targetitem = this.AttachedTo as Item;

            if (targetitem == null)
                return;

            // set the color based on damage
            if (this.Hits == this.HitsMax || !this.Enabled)
            {
                this.RestoreOriginalHue(targetitem);
            }
            else if (this.Hits < this.HitsMax && this.Hits > this.HitsMax * 0.66)
            {
                this.AssignItemHue(targetitem, LightDamageColor);
            }
            else if (this.Hits <= this.HitsMax * 0.66 && this.Hits > this.HitsMax * 0.33)
            {
                this.AssignItemHue(targetitem, MediumDamageColor);
            }
            else if (this.Hits <= this.HitsMax * 0.33)
            {
                this.AssignItemHue(targetitem, HeavyDamageColor);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)2);
            // version 2
            writer.Write(this.m_AutoRepairTime);
            if (this.m_AutoRepairEnd > DateTime.UtcNow)
            {
                writer.Write(this.m_AutoRepairEnd - DateTime.UtcNow);
            }
            else
            {
                writer.Write(TimeSpan.Zero);
            }
            // version 1
            writer.Write(this.m_Enabled);
            // version 0
            writer.Write(this.m_Hits);
            writer.Write(this.m_HitsMax);
            writer.Write(this.m_ResistFire);
            writer.Write(this.m_ResistPhysical);
            writer.Write(this.m_Stone);
            writer.Write(this.m_Iron);
            writer.Write(this.m_Wood);
            writer.Write(this.m_DestroyedItemID);
            if (this.m_OriginalItemIDList != null)
            {
                writer.Write(this.m_OriginalItemIDList.Count);
                for (int i = 0; i < this.m_OriginalItemIDList.Count; i++)
                {
                    writer.Write((int)this.m_OriginalItemIDList[i]);
                }
            }
            else
            {
                writer.Write((int)0);
            }
            if (this.m_OriginalHueList != null)
            {
                writer.Write(this.m_OriginalHueList.Count);
                for (int i = 0; i < this.m_OriginalHueList.Count; i++)
                {
                    writer.Write((int)this.m_OriginalHueList[i]);
                }
            }
            else
            {
                writer.Write((int)0);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 2:
                    this.m_AutoRepairTime = reader.ReadTimeSpan();
                    TimeSpan delay = reader.ReadTimeSpan();
                    if (delay > TimeSpan.Zero)
                    {
                        this.DoAutoRepairTimer(delay);
                    }
                    goto case 1;
                case 1:
                    this.m_Enabled = reader.ReadBool();
                    goto case 0;
                case 0:
                    // version 0
                    this.m_Hits = reader.ReadInt();
                    this.m_HitsMax = reader.ReadInt();
                    this.m_ResistFire = reader.ReadInt();
                    this.m_ResistPhysical = reader.ReadInt();
                    this.m_Stone = reader.ReadInt();
                    this.m_Iron = reader.ReadInt();
                    this.m_Wood = reader.ReadInt();
                    this.m_DestroyedItemID = reader.ReadInt();
                    int count = reader.ReadInt();
                    for (int i = 0; i < count; i++)
                    {
                        this.m_OriginalItemIDList.Add(reader.ReadInt());
                    }
                    count = reader.ReadInt();
                    for (int i = 0; i < count; i++)
                    {
                        this.m_OriginalHueList.Add(reader.ReadInt());
                    }
                    break;
            }

            // force refresh of the Enabled status
            this.Enabled = this.m_Enabled;
        }

        public override void OnDelete()
        {
            base.OnDelete();

            // restore the original itemid and color
            if (this.AttachedTo is Item)
            {
                this.RestoreOriginalItemID((Item)this.AttachedTo);
                this.RestoreOriginalHue((Item)this.AttachedTo);
            }
        }

        public override void OnAttach()
        {
            base.OnAttach();

            if (this.AttachedTo is Item)
            {
                this.StoreOriginalItemID((Item)this.AttachedTo);
                this.StoreOriginalHue((Item)this.AttachedTo);
                // temporarily adjust hue to indicate attachment
                ((Item)this.AttachedTo).Hue = ShowSiegeColor;
            }
            else
                this.Delete();
        }

        public override string OnIdentify(Mobile from)
        {
            if (from == null || from.IsPlayer())
                return null;

            if (this.Expiration > TimeSpan.Zero)
            {
                return String.Format("XmlSiege: Hits {0} expires in {1} mins", this.Hits, this.Expiration.TotalMinutes);
            }
            else
            {
                return String.Format("XmlSiege: Hits {0} out of {1}", this.Hits, this.HitsMax);
            }
        }

        private void AdjustItemID()
        {
            Item targetitem = this.AttachedTo as Item;

            if (targetitem == null || targetitem.Deleted)
                return;

            if (this.m_Hits == 0 && this.Enabled)
            {
                if (this.m_DestroyedItemID == 0)
                {
                    // blow it up
                    Effects.SendLocationEffect(targetitem, targetitem.Map, 0x36B0, 16, 1);
                    Effects.PlaySound(targetitem, targetitem.Map, 0x11D);

                    // and permanently destroy it
                    targetitem.Delete();
                }
                else
                {
                    if (targetitem is BaseMulti)
                    {
                        // map it into a valid multi id
                        targetitem.ItemID = this.m_DestroyedItemID | 0x4000;
                    }
                    else
                    {
                        // change the target item id
                        targetitem.ItemID = this.m_DestroyedItemID;
                    }

                    // deal with addons
                    if (targetitem is BaseAddon)
                    {
                        BaseAddon addon = (BaseAddon)targetitem;
                        if (addon.Components != null)
                        {
                            // change the ids of all of the components
                            foreach (AddonComponent i in addon.Components)
                            {
                                i.ItemID = this.m_DestroyedItemID;
                            }
                        }
                    }
                }
            }
            else
            {
                this.RestoreOriginalItemID(targetitem);
            }
        }

        private void AdjustMobileLocations()
        {
            Item targetitem = this.AttachedTo as Item;

            if (targetitem == null)
                return;

            // make sure nearby mobiles are in valid locations
            ArrayList mobilelist = new ArrayList();
            foreach (Mobile p in targetitem.GetMobilesInRange(0))
            {
                mobilelist.Add(p);
            }

            if (targetitem is BaseAddon)
            {
                BaseAddon addon = (BaseAddon)targetitem;
                if (addon.Components != null)
                {
                    foreach (AddonComponent i in addon.Components)
                    {
                        if (i != null)
                        {
                            foreach (Mobile p in i.GetMobilesInRange(0))
                            {
                                if (!mobilelist.Contains(p))
                                    mobilelist.Add(p);
                            }
                        }
                    }
                }
            }

            if (targetitem is BaseMulti && targetitem.Map != null)
            {
                // check all locations covered by the multi
                // go through all of the multi components
                MultiComponentList mcl = ((BaseMulti)targetitem).Components;

                if (mcl != null && mcl.List != null)
                {
                    for (int i = 0; i < mcl.List.Length; i++)
                    {
                        MultiTileEntry t = mcl.List[i];

                        int x = t.m_OffsetX + targetitem.X;
                        int y = t.m_OffsetY + targetitem.Y;
                        int z = t.m_OffsetZ + targetitem.Z;
                        foreach (Mobile p in targetitem.Map.GetMobilesInRange(new Point3D(x, y, z), 0))
                        {
                            if (!mobilelist.Contains(p))
                                mobilelist.Add(p);
                        }
                    }
                }
            }

            // relocate all mobiles found
            foreach (Mobile p in mobilelist)
            {
                if (p != null && p.Map != null)
                {
                    int x = p.Location.X;
                    int y = p.Location.Y;
                    int z = p.Location.Z;

                    // check the current location
                    if (!p.Map.CanFit(x, y, z, 16, true, false, true))
                    {
                        bool found = false;

                        for (int dx = 0; dx <= 10 && !found; dx++)
                        {
                            for (int dy = 0; dy <= 10 && !found; dy++)
                            {
                                // try moving it up in z to find a valid spot
                                for (int h = 1; h <= 39; h++)
                                {
                                    if (p.Map.CanFit(x + dx, y + dy, z + h, 16, true, false, true))
                                    {
                                        z += h;
                                        x += dx;
                                        y += dy;
                                        found = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    // move them to the new location
                    p.MoveToWorld(new Point3D(x, y, z), p.Map);
                }
            }
        }

        // These are the various ways in which the message attachment can be constructed.  
        // These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
        // Other overloads could be defined to handle other types of arguments
        public class MovingProjectileEffect : HuedEffect
        {
            public MovingProjectileEffect(IEntity from, IEntity to, int itemID, Point3D fromPoint, Point3D toPoint, int speed, int duration, bool fixedDirection, bool explode, int hue)
                : base(EffectType.Moving, from.Serial, to == null ? (Serial)(-1) : to.Serial, itemID, fromPoint, toPoint, speed, duration, fixedDirection, explode, hue, 0)
            {
            }
        }

        private class TileEntry
        {
            public readonly int ID;
            public readonly int X;
            public readonly int Y;
            public readonly int Z;
            public TileEntry(int id, int x, int y, int z)
            {
                this.ID = id;
                this.X = x;
                this.Y = y;
                this.Z = z;
            }
        }

        private class AutoRepairTimer : Timer
        {
            private readonly XmlSiege m_attachment;
            public AutoRepairTimer(XmlSiege attachment, TimeSpan delay)
                : base(delay)
            {
                this.Priority = TimerPriority.FiveSeconds;
                this.m_attachment = attachment;
            }

            protected override void OnTick()
            {
                if (this.m_attachment != null && !this.m_attachment.Deleted)
                {
                    // incrementally repair the object.  This will also restart the timer if not fully repaired and WhenToAutoRepair is set to repair on damage.
                    int repair = (int)(this.m_attachment.HitsMax * this.m_attachment.AutoRepairFactor);
                    if (repair == 0)
                        repair = 1;

                    this.m_attachment.Hits += repair;
                }
            }
        }

        private class EffectsTimer : Timer
        {
            private readonly XmlSiege m_Siege;
            public EffectsTimer(XmlSiege siege, TimeSpan delay)
                : base(delay)
            {
                this.Priority = TimerPriority.OneSecond;

                this.m_Siege = siege;
            }

            protected override void OnTick()
            {
                if (this.m_Siege != null && !this.m_Siege.Deleted && this.m_Siege.NeedsEffectsUpdate)
                {
                    this.m_Siege.AdjustDamageEffects();

                    int nplayers = 0;
                    if (this.m_Siege.AttachedTo is Item)
                    {
                        // check to see if anyone is around
                        foreach (Mobile p in ((Item)this.m_Siege.AttachedTo).GetMobilesInRange(24))
                        {
                            if (p.Player /*&& p.IsPlayer() */)
                            {
                                nplayers++;
                                break;
                            }
                        }
                    }

                    // if not, the no need to update
                    if (nplayers == 0)
                    {
                        this.m_Siege.NeedsEffectsUpdate = false;
                    }
                }
            }
        }
    }
}