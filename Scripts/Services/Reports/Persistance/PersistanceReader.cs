using System;
using System.IO;
using System.Xml;

namespace Server.Engines.Reports
{
    public abstract class PersistanceReader
    {
        public PersistanceReader()
        {
        }

        public abstract bool HasChild { get; }
        public abstract int GetInt32(string key);

        public abstract bool GetBoolean(string key);

        public abstract string GetString(string key);

        public abstract DateTime GetDateTime(string key);

        public abstract bool BeginChildren();

        public abstract void FinishChildren();

        public abstract PersistableObject GetChild();

        public abstract void ReadDocument(PersistableObject root);

        public abstract void Close();
    }

    public class XmlPersistanceReader : PersistanceReader
    {
        private readonly StreamReader m_Reader;
        private readonly XmlTextReader m_Xml;
        private readonly string m_Title;
        private bool m_HasChild;
        private bool m_WasEmptyElement;
        public XmlPersistanceReader(string filePath, string title)
        {
            this.m_Reader = new StreamReader(filePath);
            this.m_Xml = new XmlTextReader(this.m_Reader);
            this.m_Xml.WhitespaceHandling = WhitespaceHandling.None;
            this.m_Title = title;
        }

        public override bool HasChild
        {
            get
            {
                return this.m_HasChild;
            }
        }
        public override int GetInt32(string key)
        {
            return XmlConvert.ToInt32(this.m_Xml.GetAttribute(key));
        }

        public override bool GetBoolean(string key)
        {
            return XmlConvert.ToBoolean(this.m_Xml.GetAttribute(key));
        }

        public override string GetString(string key)
        {
            return this.m_Xml.GetAttribute(key);
        }

        public override DateTime GetDateTime(string key)
        {
            string val = this.m_Xml.GetAttribute(key);

            if (val == null)
                return DateTime.MinValue;

            return XmlConvert.ToDateTime(val, XmlDateTimeSerializationMode.Utc);
        }

        public override bool BeginChildren()
        {
            this.m_HasChild = !this.m_WasEmptyElement;

            this.m_Xml.Read();

            return this.m_HasChild;
        }

        public override void FinishChildren()
        {
            this.m_Xml.Read();
        }

        public override PersistableObject GetChild()
        {
            PersistableType type = PersistableTypeRegistry.Find(this.m_Xml.Name);
            PersistableObject obj = type.Constructor();

            this.m_WasEmptyElement = this.m_Xml.IsEmptyElement;

            obj.Deserialize(this);

            this.m_HasChild = (this.m_Xml.NodeType == XmlNodeType.Element);

            return obj;
        }

        public override void ReadDocument(PersistableObject root)
        {
            Console.Write("Reports: {0}: Loading...", this.m_Title);
            this.m_Xml.Read();
            this.m_Xml.Read();
            this.m_HasChild = !this.m_Xml.IsEmptyElement;
            root.Deserialize(this);
            Console.WriteLine("done");
        }

        public override void Close()
        {
            this.m_Xml.Close();
            this.m_Reader.Close();
        }
    }
}