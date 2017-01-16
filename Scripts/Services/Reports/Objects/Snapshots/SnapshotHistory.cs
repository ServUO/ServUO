using System;
using System.IO;

namespace Server.Engines.Reports
{
    public class SnapshotHistory : PersistableObject
    {
        #region Type Identification
        public static readonly PersistableType ThisTypeID = new PersistableType("sh", new ConstructCallback(Construct));

        private static PersistableObject Construct()
        {
            return new SnapshotHistory();
        }

        public override PersistableType TypeID
        {
            get
            {
                return ThisTypeID;
            }
        }
        #endregion

        private SnapshotCollection m_Snapshots;

        public SnapshotCollection Snapshots
        {
            get
            {
                return this.m_Snapshots;
            }
            set
            {
                this.m_Snapshots = value;
            }
        }

        public SnapshotHistory()
        {
            this.m_Snapshots = new SnapshotCollection();
        }

        public void Save()
        {
            string path = Path.Combine(Core.BaseDirectory, "reportHistory.xml");
            PersistenceWriter pw = new XmlPersistenceWriter(path, "Stats");

            pw.WriteDocument(this);

            pw.Close();
        }

        public void Load()
        {
            string path = Path.Combine(Core.BaseDirectory, "reportHistory.xml");

            if (!File.Exists(path))
                return;

            PersistenceReader pr = new XmlPersistenceReader(path, "Stats");

            pr.ReadDocument(this);

            pr.Close();
        }

        public override void SerializeChildren(PersistenceWriter op)
        {
            for (int i = 0; i < this.m_Snapshots.Count; ++i)
                this.m_Snapshots[i].Serialize(op);
        }

        public override void DeserializeChildren(PersistenceReader ip)
        {
            while (ip.HasChild)
                this.m_Snapshots.Add(ip.GetChild() as Snapshot);
        }
    }
}