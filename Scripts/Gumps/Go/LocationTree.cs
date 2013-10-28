using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Server.Gumps
{
    public class LocationTree
    {
        private readonly Map m_Map;
        private readonly ParentNode m_Root;
        private readonly Dictionary<Mobile, ParentNode> m_LastBranch;
        public LocationTree(string fileName, Map map)
        {
            this.m_LastBranch = new Dictionary<Mobile, ParentNode>();
            this.m_Map = map;

            string path = Path.Combine("Data/Locations/", fileName);

            if (File.Exists(path))
            {
                XmlTextReader xml = new XmlTextReader(new StreamReader(path));

                xml.WhitespaceHandling = WhitespaceHandling.None;

                this.m_Root = this.Parse(xml);

                xml.Close();
            }
        }

        public Dictionary<Mobile, ParentNode> LastBranch
        {
            get
            {
                return this.m_LastBranch;
            }
        }
        public Map Map
        {
            get
            {
                return this.m_Map;
            }
        }
        public ParentNode Root
        {
            get
            {
                return this.m_Root;
            }
        }
        private ParentNode Parse(XmlTextReader xml)
        {
            xml.Read();
            xml.Read();
            xml.Read();

            return new ParentNode(xml, null);
        }
    }
}