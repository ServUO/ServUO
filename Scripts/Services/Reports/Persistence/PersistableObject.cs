namespace Server.Engines.Reports
{
    public abstract class PersistableObject
    {
        public abstract PersistableType TypeID { get; }
        public virtual void SerializeAttributes(PersistenceWriter op)
        {
        }

        public virtual void SerializeChildren(PersistenceWriter op)
        {
        }

        public void Serialize(PersistenceWriter op)
        {
            op.BeginObject(TypeID);
            SerializeAttributes(op);
            op.BeginChildren();
            SerializeChildren(op);
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
            DeserializeAttributes(ip);

            if (ip.BeginChildren())
            {
                DeserializeChildren(ip);
                ip.FinishChildren();
            }
        }
    }
}
