using System.Xml;

namespace Server.Accounting
{
	public class AccountTag : IAccountTag
	{
		public string Name { get; set; }
		public string Value { get; set; }

		public AccountTag(string name, string value)
		{
			Name = name;
			Value = value;
		}

		public AccountTag(XmlElement node)
		{
			Load(node);
		}

		public AccountTag(GenericReader reader)
		{
			Load(reader);
		}

		public virtual void Load(GenericReader reader)
		{
			reader.ReadInt();

			Name = reader.ReadString();
			Value = reader.ReadString();
		}

		public virtual void Save(GenericWriter writer)
		{
			writer.Write(0);

			writer.Write(Name);
			writer.Write(Value);
		}

		public virtual void Load(XmlElement node)
		{
			if (!Insensitive.Equals(node.Name, "tag"))
			{
				foreach (XmlElement sub in node.GetElementsByTagName("tag"))
				{
					node = sub;
					break;
				}
			}

			Name = Utility.GetAttribute(node, "name", "empty");
			Value = Utility.GetText(node, "");
		}

		public virtual void Save(XmlElement node)
		{
			var parent = node;

			if (!Insensitive.Equals(node.Name, "tag"))
			{
				node = node.OwnerDocument.CreateElement("tag");
			}

			if (Value != null)
			{
				node.Value = Value;
			}

			node.SetAttribute("name", Name);

			if (node != parent)
			{
				parent.AppendChild(node);
			}
		}
	}
}
