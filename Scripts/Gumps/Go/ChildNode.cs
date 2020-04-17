using System.Xml;

namespace Server.Gumps
{
    public class ChildNode
    {
        private readonly ParentNode m_Parent;
        private string m_Name;
        private Point3D m_Location;
        public ChildNode(XmlTextReader xml, ParentNode parent)
        {
            m_Parent = parent;

            Parse(xml);
        }

        public ParentNode Parent => m_Parent;
        public string Name => m_Name;
        public Point3D Location => m_Location;
        private void Parse(XmlTextReader xml)
        {
            if (xml.MoveToAttribute("name"))
                m_Name = xml.Value;
            else
                m_Name = "empty";

            int x = 0, y = 0, z = 0;

            if (xml.MoveToAttribute("x"))
                x = Utility.ToInt32(xml.Value);

            if (xml.MoveToAttribute("y"))
                y = Utility.ToInt32(xml.Value);

            if (xml.MoveToAttribute("z"))
                z = Utility.ToInt32(xml.Value);

            m_Location = new Point3D(x, y, z);
        }
    }
}