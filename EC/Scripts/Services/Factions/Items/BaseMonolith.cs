using System;
using System.Collections.Generic;

namespace Server.Factions
{
    public abstract class BaseMonolith : BaseSystemController
    {
        private static List<BaseMonolith> m_Monoliths = new List<BaseMonolith>();
        private Town m_Town;
        private Faction m_Faction;
        private Sigil m_Sigil;
        public BaseMonolith(Town town, Faction faction)
            : base(0x1183)
        {
            this.Movable = false;
            this.Town = town;
            this.Faction = faction;
            m_Monoliths.Add(this);
        }

        public BaseMonolith(Serial serial)
            : base(serial)
        {
            m_Monoliths.Add(this);
        }

        public static List<BaseMonolith> Monoliths
        {
            get
            {
                return m_Monoliths;
            }
            set
            {
                m_Monoliths = value;
            }
        }
        [CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
        public Sigil Sigil
        {
            get
            {
                return this.m_Sigil;
            }
            set
            {
                if (this.m_Sigil == value)
                    return;

                this.m_Sigil = value;

                if (this.m_Sigil != null && this.m_Sigil.LastMonolith != null && this.m_Sigil.LastMonolith != this && this.m_Sigil.LastMonolith.Sigil == this.m_Sigil)
                    this.m_Sigil.LastMonolith.Sigil = null;

                if (this.m_Sigil != null)
                    this.m_Sigil.LastMonolith = this;

                this.UpdateSigil();
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
                this.OnTownChanged();
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
                this.m_Faction = value;
                this.Hue = (this.m_Faction == null ? 0 : this.m_Faction.Definition.HuePrimary);
            }
        }
        public override void OnLocationChange(Point3D oldLocation)
        {
            base.OnLocationChange(oldLocation);
            this.UpdateSigil();
        }

        public override void OnMapChange()
        {
            base.OnMapChange();
            this.UpdateSigil();
        }

        public virtual void UpdateSigil()
        {
            if (this.m_Sigil == null || this.m_Sigil.Deleted)
                return;

            this.m_Sigil.MoveToWorld(new Point3D(this.X, this.Y, this.Z + 18), this.Map);
        }

        public virtual void OnTownChanged()
        {
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();
            m_Monoliths.Remove(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            Town.WriteReference(writer, this.m_Town);
            Faction.WriteReference(writer, this.m_Faction);

            writer.Write((Item)this.m_Sigil);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.Town = Town.ReadReference(reader);
                        this.Faction = Faction.ReadReference(reader);
                        this.m_Sigil = reader.ReadItem() as Sigil;
                        break;
                    }
            }
        }
    }
}