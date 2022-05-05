using System;
using System.Xml;

namespace Server.Accounting
{
	public class AccountComment : IAccountComment
	{
		public DateTime LastModified { get; set; }

		public string AddedBy { get; set; }

		private string m_Content;

		public string Content
		{
			get => m_Content;
			set
			{
				m_Content = value;
				LastModified = DateTime.UtcNow;
			}
		}

		public AccountComment(string addedBy, string content)
		{
			AddedBy = addedBy;
			Content = content;
		}

		public AccountComment(XmlElement node)
		{
			Load(node);
		}

		public AccountComment(GenericReader reader)
		{
			Load(reader);
		}

		public virtual void Load(GenericReader reader)
		{
			reader.ReadInt();

			AddedBy = reader.ReadString();
			Content = reader.ReadString();
			LastModified = reader.ReadDateTime();
		}

		public virtual void Save(GenericWriter writer)
		{
			writer.Write(0);

			writer.Write(AddedBy);
			writer.Write(Content);
			writer.Write(LastModified);
		}

		public virtual void Load(XmlElement xml)
		{
			if (!Insensitive.Equals(xml.Name, "comment"))
			{
				foreach (XmlElement sub in xml.GetElementsByTagName("comment"))
				{
					xml = sub;
					break;
				}
			}

			AddedBy = Utility.GetAttribute(xml, "addedBy", "empty");
			Content = Utility.GetText(xml, "");
			LastModified = Utility.GetXMLDateTime(Utility.GetAttribute(xml, "lastModified"), DateTime.UtcNow);
		}

		public virtual void Save(XmlElement xml)
		{
			var parent = xml;

			if (!Insensitive.Equals(xml.Name, "comment"))
			{
				xml = xml.OwnerDocument.CreateElement("comment");
			}

			if (m_Content != null)
			{
				xml.InnerText = m_Content;
			}

			xml.SetAttribute("addedBy", AddedBy);
			xml.SetAttribute("lastModified", XmlConvert.ToString(LastModified, XmlDateTimeSerializationMode.Utc));

			if (xml != parent)
			{
				parent.AppendChild(xml);
			}
		}
	}
}
