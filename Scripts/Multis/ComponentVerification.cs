using System;
using System.Collections.Generic;
using System.IO;

namespace Server.Multis
{
    public class ComponentVerification
    {
        private readonly int[] m_ItemTable;
        private readonly int[] m_MultiTable;
        public ComponentVerification()
        {
            this.m_ItemTable = this.CreateTable(0x10000);
            this.m_MultiTable = this.CreateTable(0x4000);

            this.LoadItems("Data/Components/walls.txt", "South1", "South2", "South3", "Corner", "East1", "East2", "East3", "Post", "WindowS", "AltWindowS", "WindowE", "AltWindowE", "SecondAltWindowS", "SecondAltWindowE");
            this.LoadItems("Data/Components/teleprts.txt", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12", "F13", "F14", "F15", "F16");
            this.LoadItems("Data/Components/stairs.txt", "Block", "North", "East", "South", "West", "Squared1", "Squared2", "Rounded1", "Rounded2");
            this.LoadItems("Data/Components/roof.txt", "North", "East", "South", "West", "NSCrosspiece", "EWCrosspiece", "NDent", "EDent", "SDent", "WDent", "NTPiece", "ETPiece", "STPiece", "WTPiece", "XPiece", "Extra Piece");
            this.LoadItems("Data/Components/floors.txt", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12", "F13", "F14", "F15", "F16");
            this.LoadItems("Data/Components/misc.txt", "Piece1", "Piece2", "Piece3", "Piece4", "Piece5", "Piece6", "Piece7", "Piece8");
            this.LoadItems("Data/Components/doors.txt", "Piece1", "Piece2", "Piece3", "Piece4", "Piece5", "Piece6", "Piece7", "Piece8");

            this.LoadMultis("Data/Components/stairs.txt", "MultiNorth", "MultiEast", "MultiSouth", "MultiWest");
        }

        public bool IsItemValid(int itemID)
        {
            if (itemID <= 0 || itemID >= this.m_ItemTable.Length)
                return false;

            return this.CheckValidity(this.m_ItemTable[itemID]);
        }

        public bool IsMultiValid(int multiID)
        {
            if (multiID <= 0 || multiID >= this.m_MultiTable.Length)
                return false;

            return this.CheckValidity(this.m_MultiTable[multiID]);
        }

        public bool CheckValidity(int val)
        {
            if (val == -1)
                return false;

            return (val == 0 || (ExpansionInfo.CurrentExpansion.CustomHousingFlag & val) != 0);
        }

        private int[] CreateTable(int length)
        {
            int[] table = new int[length];

            for (int i = 0; i < table.Length; ++i)
                table[i] = -1;

            return table;
        }

        private void LoadItems(string path, params string[] itemColumns)
        {
            this.LoadSpreadsheet(this.m_ItemTable, path, itemColumns);
        }

        private void LoadMultis(string path, params string[] multiColumns)
        {
            this.LoadSpreadsheet(this.m_MultiTable, path, multiColumns);
        }

        private void LoadSpreadsheet(int[] table, string path, params string[] tileColumns)
        {
            Spreadsheet ss = new Spreadsheet(path);

            int[] tileCIDs = new int[tileColumns.Length];

            for (int i = 0; i < tileColumns.Length; ++i)
                tileCIDs[i] = ss.GetColumnID(tileColumns[i]);

            int featureCID = ss.GetColumnID("FeatureMask");

            for (int i = 0; i < ss.Records.Length; ++i)
            {
                DataRecord record = ss.Records[i];

                int fid = record.GetInt32(featureCID);

                for (int j = 0; j < tileCIDs.Length; ++j)
                {
                    int itemID = record.GetInt32(tileCIDs[j]);

                    if (itemID <= 0 || itemID >= table.Length)
                        continue;

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
                string[] types = this.ReadLine(ip);
                string[] names = this.ReadLine(ip);

                this.m_Columns = new ColumnInfo[types.Length];

                for (int i = 0; i < this.m_Columns.Length; ++i)
                    this.m_Columns[i] = new ColumnInfo(i, types[i], names[i]);

                List<DataRecord> records = new List<DataRecord>();

                string[] values;

                while ((values = this.ReadLine(ip)) != null)
                {
                    object[] data = new object[this.m_Columns.Length];

                    for (int i = 0; i < this.m_Columns.Length; ++i)
                    {
                        ColumnInfo ci = this.m_Columns[i];

                        switch ( ci.m_Type )
                        {
                            case "int":
                                {
                                    data[i] = Utility.ToInt32(values[ci.m_DataIndex]);
                                    break;
                                }
                            case "string":
                                {
                                    data[i] = values[ci.m_DataIndex];
                                    break;
                                }
                        }
                    }

                    records.Add(new DataRecord(this, data));
                }

                this.m_Records = records.ToArray();
            }
        }

        public DataRecord[] Records
        {
            get
            {
                return this.m_Records;
            }
        }
        public int GetColumnID(string name)
        {
            for (int i = 0; i < this.m_Columns.Length; ++i)
            {
                if (this.m_Columns[i].m_Name == name)
                    return i;
            }

            return -1;
        }

        private string[] ReadLine(StreamReader ip)
        {
            string line;

            while ((line = ip.ReadLine()) != null)
            {
                if (line.Length == 0)
                    continue;

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
                this.m_DataIndex = dataIndex;

                this.m_Type = type;
                this.m_Name = name;
            }
        }
    }

    public class DataRecord
    {
        private readonly Spreadsheet m_Spreadsheet;
        private readonly object[] m_Data;
        public DataRecord(Spreadsheet ss, object[] data)
        {
            this.m_Spreadsheet = ss;
            this.m_Data = data;
        }

        public Spreadsheet Spreadsheet
        {
            get
            {
                return this.m_Spreadsheet;
            }
        }
        public object[] Data
        {
            get
            {
                return this.m_Data;
            }
        }
        public object this[string name]
        {
            get
            {
                return this[this.m_Spreadsheet.GetColumnID(name)];
            }
        }
        public object this[int id]
        {
            get
            {
                if (id < 0)
                    return null;

                return this.m_Data[id];
            }
        }
        public int GetInt32(string name)
        {
            return this.GetInt32(this[name]);
        }

        public int GetInt32(int id)
        {
            return this.GetInt32(this[id]);
        }

        public int GetInt32(object obj)
        {
            if (obj is int)
                return (int)obj;

            return 0;
        }

        public string GetString(string name)
        {
            return this[name] as string;
        }
    }
}