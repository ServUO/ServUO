using Server.ContextMenus;
using Server.Engines.Craft;
using Server.Gumps;
using Server.Multis;
using System;

namespace Server.Items
{
    public abstract class BaseLight : Item, ICraftable, IResource, IQuality, ISecurable
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
        public CraftResource Resource { get { return _Resource; } set { _Resource = value; _Resource = value; Hue = CraftResources.GetHue(_Resource); InvalidateProperties(); } }

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
        public virtual int UnlitItemID => 0;
        public virtual int BurntOutItemID => 0;
        public virtual int LitSound => 0x47;
        public virtual int UnlitSound => 0x3be;
        public virtual int BurntOutSound => 0x4b8;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Burning
        {
            get { return m_Burning; }
            set
            {
                if (m_Burning == value) return;
                if (value)
                    Ignite();
                else
                    Douse();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool BurntOut
        {
            get
            {
                return m_BurntOut;
            }
            set
            {
                m_BurntOut = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Protected
        {
            get
            {
                return m_Protected;
            }
            set
            {
                m_Protected = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan Duration
        {
            get
            {
                if (m_Duration != TimeSpan.Zero && m_Burning)
                {
                    return m_End - DateTime.UtcNow;
                }
                else
                    return m_Duration;
            }

            set
            {
                m_Duration = value;
            }
        }
        public virtual void PlayLitSound()
        {
            if (LitSound != 0)
            {
                Point3D loc = GetWorldLocation();
                Effects.PlaySound(loc, Map, LitSound);
            }
        }

        public virtual void PlayUnlitSound()
        {
            int sound = UnlitSound;

            if (m_BurntOut && BurntOutSound != 0)
                sound = BurntOutSound;

            if (sound != 0)
            {
                Point3D loc = GetWorldLocation();
                Effects.PlaySound(loc, Map, sound);
            }
        }

        public virtual void Ignite()
        {
            if (!m_BurntOut)
            {
                PlayLitSound();

                m_Burning = true;
                ItemID = LitItemID;
                DoTimer(m_Duration);
            }
        }

        public virtual void Douse()
        {
            m_Burning = false;

            if (m_BurntOut && BurntOutItemID != 0)
                ItemID = BurntOutItemID;
            else
                ItemID = UnlitItemID;

            if (m_BurntOut)
                m_Duration = TimeSpan.Zero;
            else if (m_Duration != TimeSpan.Zero)
                m_Duration = m_End - DateTime.UtcNow;

            if (m_Timer != null)
                m_Timer.Stop();

            PlayUnlitSound();
        }

        public virtual void Burn()
        {
            m_BurntOut = true;
            Douse();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_BurntOut)
                return;

            if (m_Protected && from.IsPlayer())
                return;

            if (!from.InRange(GetWorldLocation(), 2))
                return;

            if (m_Burning)
            {
                if (UnlitItemID != 0)
                    Douse();
            }
            else
            {
                Ignite();
            }
        }

        public override void AddCraftedProperties(ObjectPropertyList list)
        {
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
                list.Add(1053099, "#{0}\t{1}", CraftResources.GetLocalizationNumber(_Resource), string.Format("#{0}", LabelNumber.ToString())); // ~1_oretype~ ~2_armortype~
            }
            else
            {
                base.AddNameProperty(list);
            }
        }

        public virtual int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            Quality = (ItemQuality)quality;

            PlayerConstructed = true;

            if (makersMark)
                Crafter = from;

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

            writer.Write(3);

            writer.Write((int)Level);

            writer.Write(_PlayerConstructed);

            writer.Write((int)_Resource);
            writer.Write(_Crafter);
            writer.Write((int)_Quality);

            writer.Write(m_BurntOut);
            writer.Write(m_Burning);
            writer.Write(m_Duration);
            writer.Write(m_Protected);

            if (m_Burning && m_Duration != TimeSpan.Zero)
                writer.WriteDeltaTime(m_End);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
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
                        m_BurntOut = reader.ReadBool();
                        m_Burning = reader.ReadBool();
                        m_Duration = reader.ReadTimeSpan();
                        m_Protected = reader.ReadBool();

                        if (m_Burning && m_Duration != TimeSpan.Zero)
                            DoTimer(reader.ReadDeltaTime() - DateTime.UtcNow);

                        break;
                    }
            }

            if (version == 2)
                Level = SecureLevel.Friends;
        }

        private void DoTimer(TimeSpan delay)
        {
            m_Duration = delay;

            if (m_Timer != null)
                m_Timer.Stop();

            if (delay == TimeSpan.Zero)
                return;

            m_End = DateTime.UtcNow + delay;

            m_Timer = new InternalTimer(this, delay);
            m_Timer.Start();
        }

        private class InternalTimer : Timer
        {
            private readonly BaseLight m_Light;
            public InternalTimer(BaseLight light, TimeSpan delay)
                : base(delay)
            {
                m_Light = light;
                Priority = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                if (m_Light != null && !m_Light.Deleted)
                    m_Light.Burn();
            }
        }
    }
}
