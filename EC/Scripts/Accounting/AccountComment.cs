using System;
using System.Xml;

namespace Server.Accounting
{
    public class AccountComment
    {
        private readonly string m_AddedBy;
        private string m_Content;
        private DateTime m_LastModified;
        /// <summary>
        /// Constructs a new AccountComment instance.
        /// </summary>
        /// <param name="addedBy">Initial AddedBy value.</param>
        /// <param name="content">Initial Content value.</param>
        public AccountComment(string addedBy, string content)
        {
            this.m_AddedBy = addedBy;
            this.m_Content = content;
            this.m_LastModified = DateTime.UtcNow;
        }

        /// <summary>
        /// Deserializes an AccountComment instance from an xml element.
        /// </summary>
        /// <param name="node">The XmlElement instance from which to deserialize.</param>
        public AccountComment(XmlElement node)
        {
            this.m_AddedBy = Utility.GetAttribute(node, "addedBy", "empty");
            this.m_LastModified = Utility.GetXMLDateTime(Utility.GetAttribute(node, "lastModified"), DateTime.UtcNow);
            this.m_Content = Utility.GetText(node, "");
        }

        /// <summary>
        /// A string representing who added this comment.
        /// </summary>
        public string AddedBy
        {
            get
            {
                return this.m_AddedBy;
            }
        }
        /// <summary>
        /// Gets or sets the body of this comment. Setting this value will reset LastModified.
        /// </summary>
        public string Content
        {
            get
            {
                return this.m_Content;
            }
            set
            {
                this.m_Content = value;
                this.m_LastModified = DateTime.UtcNow;
            }
        }
        /// <summary>
        /// The date and time when this account was last modified -or- the comment creation time, if never modified.
        /// </summary>
        public DateTime LastModified
        {
            get
            {
                return this.m_LastModified;
            }
        }
        /// <summary>
        /// Serializes this AccountComment instance to an XmlTextWriter.
        /// </summary>
        /// <param name="xml">The XmlTextWriter instance from which to serialize.</param>
        public void Save(XmlTextWriter xml)
        {
            xml.WriteStartElement("comment");

            xml.WriteAttributeString("addedBy", this.m_AddedBy);

            xml.WriteAttributeString("lastModified", XmlConvert.ToString(this.m_LastModified, XmlDateTimeSerializationMode.Utc));

            xml.WriteString(this.m_Content);

            xml.WriteEndElement();
        }
    }
}