#region Header
//               _,-'/-'/
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2023  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #                                       #
#endregion

#region References
using System;
using System.IO;
using System.Xml;
#endregion

namespace VitaNex.IO
{
	public class XmlDataStore<T1, T2> : DataStore<T1, T2>
	{
		public FileInfo Document { get; set; }

		public Func<XmlDocument, bool> OnSerialize { get; set; }
		public Func<XmlDocument, bool> OnDeserialize { get; set; }

		public XmlDataStore(string root, string name)
			: this(IOUtility.EnsureDirectory(root), name)
		{ }

		public XmlDataStore(DirectoryInfo root, string name)
			: base(root, name)
		{
			Document = IOUtility.EnsureFile(root.FullName + "/" + name + ((name.IndexOf('.') != -1) ? String.Empty : ".xml"));
		}

		protected override void OnExport()
		{
			using (var stream = Document.Open(FileMode.Truncate, FileAccess.ReadWrite, FileShare.ReadWrite))
			{
				var doc = new XmlDocument();
				var handled = false;

				if (OnSerialize != null)
				{
					handled = OnSerialize(doc);
				}

				if (!handled)
				{
					WriteXml(doc);
				}

				doc.Save(stream);
			}
		}

		protected override void OnImport()
		{
			if (!Document.Exists || Document.Length == 0)
			{
				return;
			}

			using (var stream = Document.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
			{
				var doc = new XmlDocument();
				doc.Load(stream);
				var handled = false;

				if (OnDeserialize != null)
				{
					handled = OnDeserialize(doc);
				}

				if (!handled)
				{
					ReadXml(doc);
				}
			}
		}

		protected virtual void WriteXml(XmlDocument doc)
		{ }

		protected virtual void ReadXml(XmlDocument doc)
		{ }
	}
}