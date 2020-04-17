using System.Xml;

namespace Server.Accounting
{
    public class AccountTag
    {
        private string m_Name, m_Value;
        /// <summary>
        /// Constructs a new AccountTag instance with a specific name and value.
        /// </summary>
        /// <param name="name">Initial name.</param>
        /// <param name="value">Initial value.</param>
        public AccountTag(string name, string value)
        {
            m_Name = name;
            m_Value = value;
        }

        /// <summary>
        /// Deserializes an AccountTag instance from an xml element.
        /// </summary>
        /// <param name="node">The XmlElement instance from which to deserialize.</param>
        public AccountTag(XmlElement node)
        {
            m_Name = Utility.GetAttribute(node, "name", "empty");
            m_Value = Utility.GetText(node, "");
        }

        /// <summary>
        /// Gets or sets the name of this tag.
        /// </summary>
        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
            }
        }
        /// <summary>
        /// Gets or sets the value of this tag.
        /// </summary>
        public string Value
        {
            get
            {
                return m_Value;
            }
            set
            {
                m_Value = value;
            }
        }
        /// <summary>
        /// Serializes this AccountTag instance to an XmlTextWriter.
        /// </summary>
        /// <param name="xml">The XmlTextWriter instance from which to serialize.</param>
        public void Save(XmlTextWriter xml)
        {
            xml.WriteStartElement("tag");
            xml.WriteAttributeString("name", m_Name);
            xml.WriteString(m_Value);
            xml.WriteEndElement();
        }
    }
}