using System;
using System.Collections;
using System.Xml;

namespace Server.Gumps
{
    public class ParentNode
    {
        private readonly ParentNode m_Parent;
        private object[] m_Children;
        private string m_Name;
        public ParentNode(XmlTextReader xml, ParentNode parent)
        {
            this.m_Parent = parent;

            this.Parse(xml);
        }

        public ParentNode Parent
        {
            get
            {
                return this.m_Parent;
            }
        }
        public object[] Children
        {
            get
            {
                return this.m_Children;
            }
        }
        public string Name
        {
            get
            {
                return this.m_Name;
            }
        }
        private void Parse(XmlTextReader xml)
        {
            if (xml.MoveToAttribute("name"))
                this.m_Name = xml.Value;
            else
                this.m_Name = "empty";

            if (xml.IsEmptyElement)
            {
                this.m_Children = new object[0];
            }
            else
            {
                ArrayList children = new ArrayList();

                while (xml.Read() && (xml.NodeType == XmlNodeType.Element || xml.NodeType == XmlNodeType.Comment))
                {
                    if (xml.NodeType == XmlNodeType.Comment)
                        continue;

                    if (xml.Name == "child")
                    {
                        ChildNode n = new ChildNode(xml, this);

                        children.Add(n);
                    }
                    else
                    {
                        children.Add(new ParentNode(xml, this));
                    }
                }

                this.m_Children = children.ToArray();
            }
        }
    }
}