using System;
using System.IO;
using System.Xml;

namespace Server.Engines.Reports
{
    public abstract class PersistenceWriter
    {
        public PersistenceWriter()
        {
        }

        public abstract void SetInt32(string key, int value);

        public abstract void SetBoolean(string key, bool value);

        public abstract void SetString(string key, string value);

        public abstract void SetDateTime(string key, DateTime value);

        public abstract void BeginObject(PersistableType typeID);

        public abstract void BeginChildren();

        public abstract void FinishChildren();

        public abstract void FinishObject();

        public abstract void WriteDocument(PersistableObject root);

        public abstract void Close();
    }

    public sealed class XmlPersistenceWriter : PersistenceWriter
    {
        private readonly string m_RealFilePath;
        private readonly string m_TempFilePath;
        private readonly StreamWriter m_Writer;
        private readonly XmlTextWriter m_Xml;
        private readonly string m_Title;
        public XmlPersistenceWriter(string filePath, string title)
        {
            this.m_RealFilePath = filePath;
            this.m_TempFilePath = Path.ChangeExtension(filePath, ".tmp");

            this.m_Writer = new StreamWriter(this.m_TempFilePath);
            this.m_Xml = new XmlTextWriter(this.m_Writer);

            this.m_Title = title;
        }

        public override void SetInt32(string key, int value)
        {
            this.m_Xml.WriteAttributeString(key, XmlConvert.ToString(value));
        }

        public override void SetBoolean(string key, bool value)
        {
            this.m_Xml.WriteAttributeString(key, XmlConvert.ToString(value));
        }

        public override void SetString(string key, string value)
        {
            if (value != null)
                this.m_Xml.WriteAttributeString(key, value);
        }

        public override void SetDateTime(string key, DateTime value)
        {
            if (value != DateTime.MinValue)
                this.m_Xml.WriteAttributeString(key, XmlConvert.ToString(value, XmlDateTimeSerializationMode.Utc));
        }

        public override void BeginObject(PersistableType typeID)
        {
            this.m_Xml.WriteStartElement(typeID.Name);
        }

        public override void BeginChildren()
        {
        }

        public override void FinishChildren()
        {
        }

        public override void FinishObject()
        {
            this.m_Xml.WriteEndElement();
        }

        public override void WriteDocument(PersistableObject root)
        {
            Console.WriteLine("Reports: {0}: Save started", this.m_Title);

            this.m_Xml.Formatting = Formatting.Indented;
            this.m_Xml.IndentChar = '\t';
            this.m_Xml.Indentation = 1;

            this.m_Xml.WriteStartDocument(true);

            root.Serialize(this);

            Console.WriteLine("Reports: {0}: Save complete", this.m_Title);
        }

        public override void Close()
        {
            this.m_Xml.Close();
            this.m_Writer.Close();

            try
            {
                string renamed = null;

                if (File.Exists(this.m_RealFilePath))
                {
                    renamed = Path.ChangeExtension(this.m_RealFilePath, ".rem");
                    File.Move(this.m_RealFilePath, renamed);
                    File.Move(this.m_TempFilePath, this.m_RealFilePath);
                    File.Delete(renamed);
                }
                else
                {
                    File.Move(this.m_TempFilePath, this.m_RealFilePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}