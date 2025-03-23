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
using System.Linq;
using System.Xml;

using Server.Gumps;
#endregion

namespace VitaNex.SuperGumps.Gml
{
	public interface IGmlReader : IDisposable
	{
		Stream Stream { get; }

		GumpPage ReadPage();
		GumpGroup ReadGroup();
		GumpTooltip ReadTooltip();
		GumpAlphaRegion ReadAlphaRegion();
		GumpBackground ReadBackground();
		GumpImage ReadImage();
		GumpImageTiled ReadImageTiled();
		GumpImageTileButton ReadImageTiledButton();
		GumpItem ReadItem();
		GumpLabel ReadLabel();
		GumpLabelCropped ReadLabelCropped();
		GumpHtml ReadHtml();
		GumpHtmlLocalized ReadHtmlLocalized();
		GumpButton ReadButton();
		GumpCheck ReadCheck();
		GumpRadio ReadRadio();
		GumpTextEntry ReadTextEntry();
		GumpTextEntryLimited ReadTextEntryLimited();
	}

	public class GmlReader : IGmlReader
	{
		private readonly XmlDocument _Document;
		private readonly XmlElement _RootNode;

		private XmlElement _LastNode;
		private XmlElement _CurrentNode;

		public Stream Stream { get; private set; }

		public GmlReader(Stream stream)
		{
			_Document = new XmlDocument();

			Stream = stream;

			_Document.Load(Stream);

			_RootNode = _Document["gump"];
		}

		private void MoveNext()
		{
			_LastNode = _CurrentNode ?? _RootNode;

			_CurrentNode = null;

			while (_CurrentNode == null)
			{
				if (_LastNode.HasChildNodes)
				{
					foreach (var e in _LastNode.ChildNodes.OfType<XmlElement>())
					{
						_CurrentNode = e;
						break;
					}
				}
				else if (_LastNode.NextSibling is XmlElement)
				{
					_CurrentNode = (XmlElement)_LastNode.NextSibling;
				}
				else if (_LastNode.ParentNode is XmlElement)
				{
					var parent = (XmlElement)_LastNode.ParentNode;

					if (parent.NextSibling is XmlElement)
					{
						_CurrentNode = (XmlElement)parent.NextSibling;
					}
				}
			}
		}

		public virtual GumpPage ReadPage()
		{
			MoveNext();

			return null;
		}

		public virtual GumpGroup ReadGroup()
		{
			MoveNext();

			return null;
		}

		public virtual GumpTooltip ReadTooltip()
		{
			MoveNext();

			return null;
		}

		public virtual GumpAlphaRegion ReadAlphaRegion()
		{
			MoveNext();

			return null;
		}

		public virtual GumpBackground ReadBackground()
		{
			MoveNext();

			return null;
		}

		public virtual GumpImage ReadImage()
		{
			MoveNext();

			return null;
		}

		public virtual GumpImageTiled ReadImageTiled()
		{
			MoveNext();

			return null;
		}

		public virtual GumpImageTileButton ReadImageTiledButton()
		{
			MoveNext();

			return null;
		}

		public virtual GumpItem ReadItem()
		{
			MoveNext();

			return null;
		}

		public virtual GumpLabel ReadLabel()
		{
			MoveNext();

			return null;
		}

		public virtual GumpLabelCropped ReadLabelCropped()
		{
			MoveNext();

			return null;
		}

		public virtual GumpHtml ReadHtml()
		{
			MoveNext();

			return null;
		}

		public virtual GumpHtmlLocalized ReadHtmlLocalized()
		{
			MoveNext();

			return null;
		}

		public virtual GumpButton ReadButton()
		{
			MoveNext();

			return null;
		}

		public virtual GumpCheck ReadCheck()
		{
			MoveNext();

			return null;
		}

		public virtual GumpRadio ReadRadio()
		{
			MoveNext();

			return null;
		}

		public virtual GumpTextEntry ReadTextEntry()
		{
			MoveNext();

			return null;
		}

		public virtual GumpTextEntryLimited ReadTextEntryLimited()
		{
			MoveNext();

			return null;
		}

		public virtual void Dispose()
		{ }
	}
}