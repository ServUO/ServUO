using System;
using System.Collections;

namespace Server.Engines.Reports
{
    public delegate PersistableObject ConstructCallback();

    public sealed class PersistableTypeRegistry
    {
        private static Hashtable m_Table;
        static PersistableTypeRegistry()
        {
            m_Table = new Hashtable(StringComparer.OrdinalIgnoreCase);

            Register(Report.ThisTypeID);
            Register(BarGraph.ThisTypeID);
            Register(PieChart.ThisTypeID);
            Register(Snapshot.ThisTypeID);
            Register(ItemValue.ThisTypeID);
            Register(ChartItem.ThisTypeID);
            Register(ReportItem.ThisTypeID);
            Register(ReportColumn.ThisTypeID);
            Register(SnapshotHistory.ThisTypeID);

            Register(PageInfo.ThisTypeID);
            Register(QueueStatus.ThisTypeID);
            Register(StaffHistory.ThisTypeID);
            Register(ResponseInfo.ThisTypeID);
        }

        public static PersistableType Find(string name)
        {
            return m_Table[name] as PersistableType;
        }

        public static void Register(PersistableType type)
        {
            if (type != null)
                m_Table[type.Name] = type;
        }
    }

    public sealed class PersistableType
    {
        private readonly string m_Name;
        private readonly ConstructCallback m_Constructor;
        public PersistableType(string name, ConstructCallback constructor)
        {
            this.m_Name = name;
            this.m_Constructor = constructor;
        }

        public string Name
        {
            get
            {
                return this.m_Name;
            }
        }
        public ConstructCallback Constructor
        {
            get
            {
                return this.m_Constructor;
            }
        }
    }
}