using System;

namespace Server.Engines.Reports
{
    public abstract class PersistableObject
    {
        public PersistableObject()
        {
        }

        public abstract PersistableType TypeID { get; }
        public virtual void SerializeAttributes(PersistenceWriter op)
        {
        }

        public virtual void SerializeChildren(PersistenceWriter op)
        {
        }

        public void Serialize(PersistenceWriter op)
        {
            op.BeginObject(this.TypeID);
            this.SerializeAttributes(op);
            op.BeginChildren();
            this.SerializeChildren(op);
            op.FinishChildren();
            op.FinishObject();
        }

        public virtual void DeserializeAttributes(PersistenceReader ip)
        {
        }

        public virtual void DeserializeChildren(PersistenceReader ip)
        {
        }

        public void Deserialize(PersistenceReader ip)
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