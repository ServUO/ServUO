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

using Server.Gumps;
#endregion

namespace VitaNex.SuperGumps.Gml
{
	public interface IGmlWriter : IDisposable
	{
		void Write(string name, GumpPage e);
		void Write(string name, GumpGroup e);
		void Write(string name, GumpTooltip e);
		void Write(string name, GumpAlphaRegion e);
		void Write(string name, GumpBackground e);
		void Write(string name, GumpImage e);
		void Write(string name, GumpImageTiled e);
		void Write(string name, GumpImageTileButton e);
		void Write(string name, GumpItem e);
		void Write(string name, GumpLabel e);
		void Write(string name, GumpLabelCropped e);
		void Write(string name, GumpHtml e);
		void Write(string name, GumpHtmlLocalized e);
		void Write(string name, GumpButton e);
		void Write(string name, GumpCheck e);
		void Write(string name, GumpRadio e);
		void Write(string name, GumpTextEntry e);
		void Write(string name, GumpTextEntryLimited e);

		void Save(Stream stream);
	}

	public class GmlWriter : IGmlWriter
	{
		private readonly XmlDocument _Document;
		private readonly XmlElement _RootNode;

		private XmlElement _LastNode;
		private XmlElement _CurrentNode;
		private XmlElement _CurrentPageNode;
		private XmlElement _CurrentGroupNode;

		public GmlWriter()
		{
			_Document = new XmlDocument();
			_RootNode = _Document.CreateElement("gump");
		}

		~GmlWriter()
		{
			Dispose();
		}

		private void CreateElement(GumpEntry e)
		{
			_CurrentNode = _Document.CreateElement(e.GetType().Name);
		}

		private void CreatePageElement(GumpPage e)
		{
			_CurrentPageNode = _Document.CreateElement(e.GetType().Name);
		}

		private void CreateGroupElement(GumpGroup e)
		{
			_CurrentGroupNode = _Document.CreateElement(e.GetType().Name);
		}

		private void SetValue(object value)
		{
			_CurrentNode.Value = value != null ? value.ToString() : String.Empty;
		}

		private void SetAttribute(string name, object value)
		{
			_CurrentNode.SetAttribute(name, value != null ? value.ToString() : String.Empty);
		}

		private void SetPageAttribute(string name, object value)
		{
			_CurrentPageNode.SetAttribute(name, value != null ? value.ToString() : String.Empty);
		}

		private void SetGroupAttribute(string name, object value)
		{
			_CurrentPageNode.SetAttribute(name, value != null ? value.ToString() : String.Empty);
		}

		private void Append()
		{
			if (_CurrentNode == null)
			{
				return;
			}

			(_CurrentGroupNode ?? _CurrentPageNode ?? _RootNode).AppendChild(_CurrentNode);

			_LastNode = _CurrentNode;
			_CurrentNode = null;
		}

		private void AppendPage()
		{
			if (_CurrentPageNode == null)
			{
				return;
			}

			_RootNode.AppendChild(_CurrentPageNode);

			_CurrentPageNode = null;
		}

		private void AppendGroup()
		{
			if (_CurrentGroupNode == null)
			{
				return;
			}

			(_CurrentPageNode ?? _RootNode).AppendChild(_CurrentGroupNode);

			_CurrentGroupNode = null;
		}

		public virtual void Dispose()
		{ }

		public virtual void Write(string name, GumpPage e)
		{
			AppendPage();
			CreatePageElement(e);
			SetPageAttribute("name", name);
			SetPageAttribute("value", e.Page);
		}

		public virtual void Write(string name, GumpGroup e)
		{
			if (_LastNode != null && _LastNode.Name != e.GetType().Name)
			{
				SetAttribute("group", e.Group);
				return;
			}

			AppendGroup();
			CreateGroupElement(e);
			SetGroupAttribute("name", name);
			SetGroupAttribute("value", e.Group);
		}

		public virtual void Write(string name, GumpTooltip e)
		{
			if (_LastNode != null && _LastNode.Name != e.GetType().Name)
			{
				SetAttribute("tooltip", e.Number);
				return;
			}

			CreateElement(e);
			SetAttribute("name", name);
			SetAttribute("value", e.Number);
			Append();
		}

		public virtual void Write(string name, GumpAlphaRegion e)
		{
			CreateElement(e);
			SetAttribute("name", name);
			SetAttribute("x", e.X);
			SetAttribute("y", e.Y);
			SetAttribute("width", e.Width);
			SetAttribute("height", e.Height);
			Append();
		}

		public virtual void Write(string name, GumpBackground e)
		{
			CreateElement(e);
			SetAttribute("name", name);
			SetAttribute("x", e.X);
			SetAttribute("y", e.Y);
			SetAttribute("width", e.Width);
			SetAttribute("height", e.Height);
			SetAttribute("gumpid", e.GumpID);
			Append();
		}

		public virtual void Write(string name, GumpImage e)
		{
			CreateElement(e);
			SetAttribute("name", name);
			SetAttribute("x", e.X);
			SetAttribute("y", e.Y);
			SetAttribute("hue", e.Hue);
			SetAttribute("gumpid", e.GumpID);
			Append();
		}

		public virtual void Write(string name, GumpImageTiled e)
		{
			CreateElement(e);
			SetAttribute("name", name);
			SetAttribute("x", e.X);
			SetAttribute("y", e.Y);
			SetAttribute("width", e.Width);
			SetAttribute("height", e.Height);
			SetAttribute("gumpid", e.GumpID);
			Append();
		}

		public virtual void Write(string name, GumpImageTileButton e)
		{
			CreateElement(e);
			SetAttribute("name", name);
			SetAttribute("x", e.X);
			SetAttribute("y", e.Y);
			SetAttribute("width", e.Width);
			SetAttribute("height", e.Height);
			SetAttribute("itemid", e.ItemID);
			SetAttribute("buttonid", e.ButtonID);
			SetAttribute("normalid", e.NormalID);
			SetAttribute("pressedid", e.PressedID);
			SetAttribute("hue", e.Hue);
			SetAttribute("tooltip", e.LocalizedTooltip);
			Append();
		}

		public virtual void Write(string name, GumpItem e)
		{
			CreateElement(e);
			SetAttribute("name", name);
			SetAttribute("x", e.X);
			SetAttribute("y", e.Y);
			SetAttribute("itemid", e.ItemID);
			SetAttribute("hue", e.Hue);
			Append();
		}

		public virtual void Write(string name, GumpLabel e)
		{
			CreateElement(e);
			SetAttribute("name", name);
			SetAttribute("x", e.X);
			SetAttribute("y", e.Y);
			SetAttribute("hue", e.Hue);
			SetValue(e.Text);
			Append();
		}

		public virtual void Write(string name, GumpLabelCropped e)
		{
			CreateElement(e);
			SetAttribute("name", name);
			SetAttribute("x", e.X);
			SetAttribute("y", e.Y);
			SetAttribute("width", e.Width);
			SetAttribute("height", e.Height);
			SetAttribute("hue", e.Hue);
			SetValue(e.Text);
			Append();
		}

		public virtual void Write(string name, GumpHtml e)
		{
			CreateElement(e);
			SetAttribute("name", name);
			SetAttribute("x", e.X);
			SetAttribute("y", e.Y);
			SetAttribute("width", e.Width);
			SetAttribute("height", e.Height);
			SetAttribute("scrollbar", e.Scrollbar);
			SetAttribute("background", e.Background);
			SetValue(e.Text);
			Append();
		}

		public virtual void Write(string name, GumpHtmlLocalized e)
		{
			CreateElement(e);
			SetAttribute("name", name);
			SetAttribute("x", e.X);
			SetAttribute("y", e.Y);
			SetAttribute("width", e.Width);
			SetAttribute("height", e.Height);
			SetAttribute("scrollbar", e.Scrollbar);
			SetAttribute("background", e.Background);
			SetAttribute("value", e.Number);
			SetAttribute("args", e.Args);
			Append();
		}

		public virtual void Write(string name, GumpButton e)
		{
			CreateElement(e);
			SetAttribute("name", name);
			SetAttribute("x", e.X);
			SetAttribute("y", e.Y);
			SetAttribute("buttonid", e.ButtonID);
			SetAttribute("normalid", e.NormalID);
			SetAttribute("pressedid", e.PressedID);
			SetAttribute("type", e.Type);
			SetAttribute("param", e.Param);
			Append();
		}

		public virtual void Write(string name, GumpCheck e)
		{
			CreateElement(e);
			SetAttribute("name", name);
			SetAttribute("x", e.X);
			SetAttribute("y", e.Y);
			SetAttribute("switchid", e.SwitchID);
			SetAttribute("onid", e.ActiveID);
			SetAttribute("offid", e.InactiveID);
			SetAttribute("state", e.InitialState);
			Append();
		}

		public virtual void Write(string name, GumpRadio e)
		{
			CreateElement(e);
			SetAttribute("name", name);
			SetAttribute("x", e.X);
			SetAttribute("y", e.Y);
			SetAttribute("switchid", e.SwitchID);
			SetAttribute("onid", e.ActiveID);
			SetAttribute("offid", e.InactiveID);
			SetAttribute("state", e.InitialState);
			Append();
		}

		public virtual void Write(string name, GumpTextEntry e)
		{
			CreateElement(e);
			SetAttribute("name", name);
			SetAttribute("x", e.X);
			SetAttribute("y", e.Y);
			SetAttribute("width", e.Width);
			SetAttribute("height", e.Height);
			SetAttribute("entryid", e.EntryID);
			SetAttribute("hue", e.Hue);
			SetValue(e.InitialText);
			Append();
		}

		public virtual void Write(string name, GumpTextEntryLimited e)
		{
			CreateElement(e);
			SetAttribute("name", name);
			SetAttribute("x", e.X);
			SetAttribute("y", e.Y);
			SetAttribute("width", e.Width);
			SetAttribute("height", e.Height);
			SetAttribute("entryid", e.EntryID);
			SetAttribute("limit", e.Size);
			SetAttribute("hue", e.Hue);
			SetValue(e.InitialText);
			Append();
		}

		public void Save(Stream stream)
		{
			Append();
			AppendGroup();
			AppendPage();

			_Document.AppendChild(_RootNode);
			_Document.Save(stream);
		}
	}
}