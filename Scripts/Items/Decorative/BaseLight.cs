using System;
using Server.Engines.Craft;
using Server.Gumps;
using Server.Multis;
using Server.ContextMenus;

namespace Server.Items
{
    public abstract class BaseLight : Item, ICraftable, IResource, ISecurable
    {
        public static readonly bool Burnout = false;
        private Timer m_Timer;
        private DateTime m_End;
        private bool m_BurntOut = false;
        private bool m_Burning = false;
        private bool m_Protected = false;
        private TimeSpan m_Duration = TimeSpan.Zero;
        private CraftResource _Resource;
        private Mobile _Crafter;
        private ItemQuality _Quality;
        private bool _PlayerConstructed;

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource { get { return _Resource; } set { _Resource = value; _Resource = value; Hue = CraftResources.GetHue(this._Resource); InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Crafter { get { return _Crafter; } set { _Crafter = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public ItemQuality Quality { get { return _Quality; } set { _Quality = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool PlayerConstructed { get { return _PlayerConstructed; } set { _PlayerConstructed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level { get; set; }

        [Constructable]
        public BaseLight(int itemID)
            : base(itemID)
        {
            Level = SecureLevel.Friends;
        }

        public BaseLight(Serial serial)
            : base(serial)
        {
        }

        public abstract int LitItemID { get; }
        public virtual int UnlitItemID
        {
            get
            {
                return 0;
            }
        }
        public virtual int BurntOutItemID
        {
            get
            {
                return 0;
            }
        }
        public virtual int LitSound
        {
            get
            {
                return 0x47;
            }
        }
        public virtual int UnlitSound
        {
            get
            {
                return 0x3be;
            }
        }
        public virtual int BurntOutSound
        {
            get
            {
                return 0x4b8;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Burning
        {
            get
            {
                return this.m_Burning;
            }
            set
            {
                if (this.m_Burning != value)
                {
                    this.m_Burning = true;
                    this.DoTimer(this.m_Duration);
                }
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool BurntOut
        {
            get
            {
                return this.m_BurntOut;
            }
            set
            {
                this.m_BurntOut = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Protected
        {
            get
            {
                return this.m_Protected;
            }
            set
            {
                this.m_Protected = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan Duration
        {
            get
            {
                if (this.m_Duration != TimeSpan.Zero && this.m_Burning)
                {
                    return this.m_End - DateTime.UtcNow;
                }
                else
                    return this.m_Duration;
            }

            set
            {
                this.m_Duration = value;
            }
        }
        public virtual void PlayLitSound()
        {
            if (this.LitSound != 0)
            {
                Point3D loc = this.GetWorldLocation();
                Effects.PlaySound(loc, this.Map, this.LitSound);
            }
        }

        public virtual void PlayUnlitSound()
        {
            int sound = this.UnlitSound;

            if (this.m_BurntOut && this.BurntOutSound != 0)
                sound = this.BurntOutSound;

            if (sound != 0)
            {
                Point3D loc = this.GetWorldLocation();
                Effects.PlaySound(loc, this.Map, sound);
            }
        }

        public virtual void Ignite()
        {
            if (!this.m_BurntOut)
            {
                this.PlayLitSound();

                this.m_Burning = true;
                this.ItemID = this.LitItemID;
                this.DoTimer(this.m_Duration);
            }
        }

        public virtual void Douse()
        {
            this.m_Burning = false;
			
            if (this.m_BurntOut && this.BurntOutItemID != 0)
                this.ItemID = this.BurntOutItemID;
            else
                this.ItemID = this.UnlitItemID;

            if (this.m_BurntOut)
                this.m_Duration = TimeSpan.Zero;
            else if (this.m_Duration != TimeSpan.Zero)
                this.m_Duration = this.m_End - DateTime.UtcNow;

            if (this.m_Timer != null)
                this.m_Timer.Stop();

            this.PlayUnlitSound();
        }

        public virtual void Burn()
        {
            this.m_BurntOut = true;
            this.Douse();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.m_BurntOut)
                return;

            if (this.m_Protected && from.IsPlayer())
                return;

            if (!from.InRange(this.GetWorldLocation(), 2))
                return;

            if (this.m_Burning)
            {
                if (this.UnlitItemID != 0)
                    this.Douse();
            }
            else
            {
                this.Ignite();
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (_PlayerConstructed && _Crafter != null)
            {
                list.Add(1050043, _Crafter.TitleName); // crafted by ~1_NAME~
            }

            if (_Quality == ItemQuality.Exceptional)
            {
                list.Add(1060636); // Exceptional
            }
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (_Resource > CraftResource.Iron)
            {
                list.Add(1053099, "#{0}\t{1}", CraftResources.GetLocalizationNumber(_Resource), String.Format("#{0}", LabelNumber.ToString())); // ~1_oretype~ ~2_armortype~
            }
            else
            {
                base.AddNameProperty(list);
            }
        }

        public virtual int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            this.Quality = (ItemQuality)quality;

            PlayerConstructed = true;

            if (makersMark)
                this.Crafter = from;

            if (!craftItem.ForceNonExceptional)
            {
                if (typeRes == null)
                    typeRes = craftItem.Resources.GetAt(0).ItemType;

                Resource = CraftResources.GetFromType(typeRes);
            }

            return quality;
        }

        public override void GetContextMenuEntries(Mobile from, System.Collections.Generic.List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
            SetSecureLevelEntry.AddTo(from, this, list);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)3);

            writer.Write((int)Level);

            writer.Write(_PlayerConstructed);

            writer.Write((int)_Resource);
            writer.Write(_Crafter);
            writer.Write((int)_Quality);

            writer.Write(this.m_BurntOut);
            writer.Write(this.m_Burning);
            writer.Write(this.m_Duration);
            writer.Write(this.m_Protected);

            if (this.m_Burning && this.m_Duration != TimeSpan.Zero)
                writer.WriteDeltaTime(this.m_End);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 3:
                    {
                        Level = (SecureLevel)reader.ReadInt();
                        goto case 2;
                    }
                case 2:
                    {
                        _PlayerConstructed = reader.ReadBool();
                        goto case 1;
                    }
                case 1:
                    {
                        _Resource = (CraftResource)reader.ReadInt();
                        _Crafter = reader.ReadMobile();
                        _Quality = (ItemQuality)reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
                        this.m_BurntOut = reader.ReadBool();
                        this.m_Burning = reader.ReadBool();
                        this.m_Duration = reader.ReadTimeSpan();
                        this.m_Protected = reader.ReadBool();

                        if (this.m_Burning && this.m_Duration != TimeSpan.Zero)
                            this.DoTimer(reader.ReadDeltaTime() - DateTime.UtcNow);

                        break;
                    }
            }

            if(version == 2)
                Level = SecureLevel.Friends;
        }

        private void DoTimer(TimeSpan delay)
        {
            this.m_Duration = delay;
			
            if (this.m_Timer != null)
                this.m_Timer.Stop();

            if (delay == TimeSpan.Zero)
                return;

            this.m_End = DateTime.UtcNow + delay;

            this.m_Timer = new InternalTimer(this, delay);
            this.m_Timer.Start();
        }

        private class InternalTimer : Timer
        {
            private readonly BaseLight m_Light;
            public InternalTimer(BaseLight light, TimeSpan delay)
                : base(delay)
            {
                this.m_Light = light;
                this.Priority = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                if (this.m_Light != null && !this.m_Light.Deleted)
                    this.m_Light.Burn();
            }
        }
    }
}