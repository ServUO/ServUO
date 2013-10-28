using System;
using Server.Items;

namespace Server.Engines.Quests.Necro
{
    public class MaabusCoffin : BaseAddon
    {
        private Maabus m_Maabus;
        private Point3D m_SpawnLocation;
        [Constructable]
        public MaabusCoffin()
        {
            this.AddComponent(new MaabusCoffinComponent(0x1C2B, 0x1C2B), -1, -1, 0);

            this.AddComponent(new MaabusCoffinComponent(0x1D16, 0x1C2C), 0, -1, 0);
            this.AddComponent(new MaabusCoffinComponent(0x1D17, 0x1C2D), 1, -1, 0);
            this.AddComponent(new MaabusCoffinComponent(0x1D51, 0x1C2E), 2, -1, 0);

            this.AddComponent(new MaabusCoffinComponent(0x1D4E, 0x1C2A), 0, 0, 0);
            this.AddComponent(new MaabusCoffinComponent(0x1D4D, 0x1C29), 1, 0, 0);
            this.AddComponent(new MaabusCoffinComponent(0x1D4C, 0x1C28), 2, 0, 0);
        }

        public MaabusCoffin(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Maabus Maabus
        {
            get
            {
                return this.m_Maabus;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D SpawnLocation
        {
            get
            {
                return this.m_SpawnLocation;
            }
            set
            {
                this.m_SpawnLocation = value;
            }
        }
        public void Awake(Mobile caller)
        {
            if (this.m_Maabus != null || this.m_SpawnLocation == Point3D.Zero)
                return;

            foreach (MaabusCoffinComponent c in this.Components)
                c.TurnToEmpty();

            this.m_Maabus = new Maabus();

            this.m_Maabus.Location = this.m_SpawnLocation;
            this.m_Maabus.Map = this.Map;

            this.m_Maabus.Direction = this.m_Maabus.GetDirectionTo(caller);

            Timer.DelayCall(TimeSpan.FromSeconds(7.5), new TimerCallback(BeginSleep));
        }

        public void BeginSleep()
        {
            if (this.m_Maabus == null)
                return;

            Effects.PlaySound(this.m_Maabus.Location, this.m_Maabus.Map, 0x48E);

            Timer.DelayCall(TimeSpan.FromSeconds(2.5), new TimerCallback(Sleep));
        }

        public void Sleep()
        {
            if (this.m_Maabus == null)
                return;

            Effects.SendLocationParticles(EffectItem.Create(this.m_Maabus.Location, this.m_Maabus.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 0x7E7);
            Effects.PlaySound(this.m_Maabus.Location, this.m_Maabus.Map, 0x1FE);

            this.m_Maabus.Delete();
            this.m_Maabus = null;

            foreach (MaabusCoffinComponent c in this.Components)
                c.TurnToFull();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((Mobile)this.m_Maabus);
            writer.Write((Point3D)this.m_SpawnLocation);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.m_Maabus = reader.ReadMobile() as Maabus;
            this.m_SpawnLocation = reader.ReadPoint3D();

            this.Sleep();
        }
    }

    public class MaabusCoffinComponent : AddonComponent
    {
        private int m_FullItemID;
        private int m_EmptyItemID;
        public MaabusCoffinComponent(int itemID)
            : this(itemID, itemID)
        {
        }

        public MaabusCoffinComponent(int fullItemID, int emptyItemID)
            : base(fullItemID)
        {
            this.m_FullItemID = fullItemID;
            this.m_EmptyItemID = emptyItemID;
        }

        public MaabusCoffinComponent(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D SpawnLocation
        {
            get
            {
                return this.Addon is MaabusCoffin ? ((MaabusCoffin)this.Addon).SpawnLocation : Point3D.Zero;
            }
            set
            {
                if (this.Addon is MaabusCoffin)
                    ((MaabusCoffin)this.Addon).SpawnLocation = value;
            }
        }
        public void TurnToEmpty()
        {
            this.ItemID = this.m_EmptyItemID;
        }

        public void TurnToFull()
        {
            this.ItemID = this.m_FullItemID;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((int)this.m_FullItemID);
            writer.Write((int)this.m_EmptyItemID);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.m_FullItemID = reader.ReadInt();
            this.m_EmptyItemID = reader.ReadInt();
        }
    }
}