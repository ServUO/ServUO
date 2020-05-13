#region References
using System.Collections.Generic;
using System.IO;
using System.Linq;
#endregion

namespace Server.Multis
{
    public class ComponentVerification
    {
        public int[] ItemTable => m_ItemTable;
        public int[] MultiTable => m_MultiTable;

        private readonly int[] m_ItemTable;
        private readonly int[] m_MultiTable;

        public ComponentVerification()
        {
            m_ItemTable = CreateTable(TileData.MaxItemValue);
            m_MultiTable = CreateTable(0x4000);

            LoadItems(
                "Data/Components/walls.txt",
                //
                "South1",
                "South2",
                "South3",
                "Corner",
                "East1",
                "East2",
                "East3",
                "Post",
                "WindowS",
                "AltWindowS",
                "WindowE",
                "AltWindowE",
                "SecondAltWindowS",
                "SecondAltWindowE");

            LoadItems(
                "Data/Components/teleprts.txt",
                //
                "F1",
                "F2",
                "F3",
                "F4",
                "F5",
                "F6",
                "F7",
                "F8",
                "F9",
                "F10",
                "F11",
                "F12",
                "F13",
                "F14",
                "F15",
                "F16");

            LoadItems(
                "Data/Components/stairs.txt",
                //
                "Block",
                "North",
                "East",
                "South",
                "West",
                "Squared1",
                "Squared2",
                "Rounded1",
                "Rounded2");

            LoadItems(
                "Data/Components/roof.txt",
                //
                "North",
                "East",
                "South",
                "West",
                "NSCrosspiece",
                "EWCrosspiece",
                "NDent",
                "EDent",
                "SDent",
                "WDent",
                "NTPiece",
                "ETPiece",
                "STPiece",
                "WTPiece",
                "XPiece",
                "Extra Piece");

            LoadItems(
                "Data/Components/floors.txt",
                //
                "F1",
                "F2",
                "F3",
                "F4",
                "F5",
                "F6",
                "F7",
                "F8",
                "F9",
                "F10",
                "F11",
                "F12",
                "F13",
                "F14",
                "F15",
                "F16");

            LoadItems(
                "Data/Components/misc.txt",
                //
                "Piece1",
                "Piece2",
                "Piece3",
                "Piece4",
                "Piece5",
                "Piece6",
                "Piece7",
                "Piece8");

            LoadItems(
                "Data/Components/doors.txt",
                //
                "Piece1",
                "Piece2",
                "Piece3",
                "Piece4",
                "Piece5",
                "Piece6",
                "Piece7",
                "Piece8");

            LoadMultis(
                "Data/Components/stairs.txt",
                //
                "MultiNorth",
                "MultiEast",
                "MultiSouth",
                "MultiWest");
        }

        public bool IsItemValid(int itemID)
        {
            if (itemID <= 0 || itemID >= m_ItemTable.Length)
            {
                return false;
            }

            return CheckValidity(m_ItemTable[itemID]);
        }

        public bool IsMultiValid(int multiID)
        {
            if (multiID <= 0 || multiID >= m_MultiTable.Length)
            {
                return false;
            }

            return CheckValidity(m_MultiTable[multiID]);
        }

        public bool CheckValidity(int val)
        {
            if (val == -1)
            {
                return false;
            }

            return (val == 0 || ((int)ExpansionInfo.CoreExpansion.CustomHousingFlag & val) != 0);
        }

        private static int[] CreateTable(int length)
        {
            int[] table = new int[length];

            for (int i = 0; i < table.Length; ++i)
            {
                table[i] = -1;
            }

            return table;
        }

        private void LoadItems(string path, params string[] itemColumns)
        {
            LoadSpreadsheet(m_ItemTable, path, itemColumns);
        }

        private void LoadMultis(string path, params string[] multiColumns)
        {
            LoadSpreadsheet(m_MultiTable, path, multiColumns);
        }

        private static void LoadSpreadsheet(int[] table, string path, params string[] tileColumns)
        {
            Spreadsheet ss = new Spreadsheet(path);

            int[] tileCIDs = new int[tileColumns.Length];

            for (int i = 0; i < tileColumns.Length; ++i)
            {
                tileCIDs[i] = ss.GetColumnID(tileColumns[i]);
            }

            int featureCID = ss.GetColumnID("FeatureMask");

            foreach (DataRecord record in ss.Records)
            {
                int fid = record.GetInt32(featureCID);

                foreach (int itemID in tileCIDs.Select(v => record.GetInt32(v)).Where(id => id > 0 && id < table.Length))
                {
                    table[itemID] = fid;
                }
            }
        }
    }

    public class Spreadsheet
    {
        private readonly ColumnInfo[] m_Columns;
        private readonly DataRecord[] m_Records;

        public Spreadsheet(string path)
        {
            using (StreamReader ip = new StreamReader(path))
            {
                string[] types = ReadLine(ip);
                string[] names = ReadLine(ip);

                m_Columns = new ColumnInfo[types.Length];

                for (int i = 0; i < m_Columns.Length; ++i)
                {
                    m_Columns[i] = new ColumnInfo(i, types[i], names[i]);
                }

                List<DataRecord> records = new List<DataRecord>();

                string[] values;

                while ((values = ReadLine(ip)) != null)
                {
                    object[] data = new object[m_Columns.Length];

                    for (int i = 0; i < m_Columns.Length; ++i)
                    {
                        ColumnInfo ci = m_Columns[i];

                        switch (ci.m_Type)
                        {
                            case "int":
                                data[i] = Utility.ToInt32(values[ci.m_DataIndex]);
                                break;
                            case "string":
                                data[i] = values[ci.m_DataIndex];
                                break;
                        }
                    }

                    records.Add(new DataRecord(this, data));
                }

                m_Records = records.ToArray();
            }
        }

        public DataRecord[] Records => m_Records;

        public int GetColumnID(string name)
        {
            for (int i = 0; i < m_Columns.Length; ++i)
            {
                if (m_Columns[i].m_Name == name)
                {
                    return i;
                }
            }

            return -1;
        }

        private static string[] ReadLine(StreamReader ip)
        {
            string line;

            while ((line = ip.ReadLine()) != null)
            {
                if (line.Length == 0)
                {
                    continue;
                }

                return line.Split('\t');
            }

            return null;
        }

        private class ColumnInfo
        {
            public readonly int m_DataIndex;
            public readonly string m_Type;
            public readonly string m_Name;

            public ColumnInfo(int dataIndex, string type, string name)
            {
                m_DataIndex = dataIndex;

                m_Type = type;
                m_Name = name;
            }
        }
    }

    public class DataRecord
    {
        private readonly Spreadsheet m_Spreadsheet;
        private readonly object[] m_Data;

        public DataRecord(Spreadsheet ss, object[] data)
        {
            m_Spreadsheet = ss;
            m_Data = data;
        }

        public Spreadsheet Spreadsheet => m_Spreadsheet;
        public object[] Data => m_Data;
        public object this[string name] => this[m_Spreadsheet.GetColumnID(name)];

        public object this[int id]
        {
            get
            {
                if (id < 0)
                {
                    return null;
                }

                return m_Data[id];
            }
        }

        public int GetInt32(string name)
        {
            return GetInt32(this[name]);
        }

        public int GetInt32(int id)
        {
            return GetInt32(this[id]);
        }

        public int GetInt32(object obj)
        {
            if (obj is int)
            {
                return (int)obj;
            }

            return 0;
        }

        public string GetString(string name)
        {
            return this[name] as string;
        }
    }
}
