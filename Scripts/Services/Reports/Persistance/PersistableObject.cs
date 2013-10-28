using System;

namespace Server.Engines.Reports
{
    public abstract class PersistableObject
    {
        public PersistableObject()
        {
        }

        public abstract PersistableType TypeID { get; }
        public virtual void SerializeAttributes(PersistanceWriter op)
        {
        }

        public virtual void SerializeChildren(PersistanceWriter op)
        {
        }

        public void Serialize(PersistanceWriter op)
        {
            op.BeginObject(this.TypeID);
            this.SerializeAttributes(op);
            op.BeginChildren();
            this.SerializeChildren(op);
            op.FinishChildren();
            op.FinishObject();
        }

        public virtual void DeserializeAttributes(PersistanceReader ip)
        {
        }

        public virtual void DeserializeChildren(PersistanceReader ip)
        {
        }

        public void Deserialize(PersistanceReader ip)
        {
            this.DeserializeAttributes(ip);

            if (ip.BeginChildren())
            {
                this.DeserializeChildren(ip);
                ip.FinishChildren();
            }
        }
    }
}