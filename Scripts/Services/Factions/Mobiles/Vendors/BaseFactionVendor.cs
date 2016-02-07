using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Factions
{
    public abstract class BaseFactionVendor : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        private Town m_Town;
        private Faction m_Faction;
        public BaseFactionVendor(Town town, Faction faction, string title)
            : base(title)
        {
            this.Frozen = true;
            this.CantWalk = true;
            this.Female = false;
            this.BodyValue = 400;
            this.Name = NameList.RandomName("male");

            this.RangeHome = 0;

            this.m_Town = town;
            this.m_Faction = faction;
            this.Register();
        }

        public BaseFactionVendor(Serial serial)
            : base(serial)
        {
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
                this.Unregister();
                this.m_Town = value;
                this.Register();
            }
        }
        [CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
        public Faction Faction
        {
            get
            {
                return this.m_Faction;
            }
            set
            {
                this.Unregister();
                this.m_Faction = value;
                this.Register();
            }
        }
        protected override List<SBInfo> SBInfos
        {
            get
            {
                return this.m_SBInfos;
            }
        }
        public void Register()
        {
            if (this.m_Town != null && this.m_Faction != null)
                this.m_Town.RegisterVendor(this);
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (Core.ML)
                return true;

            return base.OnMoveOver(m);
        }

        public void Unregister()
        {
            if (this.m_Town != null)
                this.m_Town.UnregisterVendor(this);
        }

        public override void InitSBInfo()
        {
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            this.Unregister();
        }

        public override bool CheckVendorAccess(Mobile from)
        {
            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            Town.WriteReference(writer, this.m_Town);
            Faction.WriteReference(writer, this.m_Faction);
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
                        this.m_Faction = Faction.ReadReference(reader);
                        this.Register();
                        break;
                    }
            }

            this.Frozen = true;
        }
    }
}