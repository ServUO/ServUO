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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

using Server;
using Server.Gumps;

using VitaNex.IO;
#endregion

namespace VitaNex.SuperGumps.UI
{
	public class FileExplorerGump : TreeGump
	{
		public static void Initialize()
		{
			CommandUtility.Register("explorer", AccessLevel.Owner, e => new FileExplorerGump(e.Mobile, null, null).Send());
		}

		public DirectoryInfo RootDirectory { get; set; }

		public DirectoryInfo SelectedDirectory { get; set; }
		public FileInfo SelectedFile { get; set; }

		public FileExplorerGump(Mobile user, Gump parent, DirectoryInfo rootDirectory)
			: base(user, parent)
		{
			RootDirectory = SelectedDirectory = rootDirectory;

			Width = 800;
			Height = 600;
		}

		protected override void Compile()
		{
			if (RootDirectory == null)
			{
				RootDirectory = new DirectoryInfo(Core.BaseDirectory);
			}

			while (!RootDirectory.Exists && RootDirectory.Parent != null)
			{
				RootDirectory = RootDirectory.Parent;
			}

			if (!RootDirectory.Exists)
			{
				RootDirectory = new DirectoryInfo(Core.BaseDirectory);
			}

			if (SelectedFile != null)
			{
				if (SelectedFile.Exists)
				{
					SelectedDirectory = SelectedFile.Directory;
				}
				else
				{
					SelectedFile = null;
				}
			}

			if (SelectedDirectory == null)
			{
				SelectedDirectory = RootDirectory;
			}

			while (!SelectedDirectory.Exists && SelectedDirectory.Parent != null)
			{
				SelectedDirectory = SelectedDirectory.Parent;
			}

			if (!SelectedDirectory.Exists)
			{
				SelectedDirectory = RootDirectory;
			}

			if (!Insensitive.StartsWith(SelectedDirectory.FullName, RootDirectory.FullName))
			{
				SelectedDirectory = RootDirectory;
			}

			base.Compile();
		}

		private string GetPath(FileSystemInfo info)
		{
			if (RootDirectory.Parent != null)
			{
				return info.FullName.Replace(RootDirectory.Parent.FullName, String.Empty)
						   .Replace(IOUtility.PathSeparator, TreeGumpNode.Separator)
						   .Trim(TreeGumpNode.Separator);
			}

			return info.FullName.Replace(RootDirectory.FullName, String.Empty)
					   .Replace(IOUtility.PathSeparator, TreeGumpNode.Separator)
					   .Trim(TreeGumpNode.Separator);
		}

		private string GetPath(TreeGumpNode node)
		{
			if (RootDirectory.Parent != null)
			{
				return Path.Combine(
							   RootDirectory.Parent.FullName,
							   node.FullName.Replace(TreeGumpNode.Separator, IOUtility.PathSeparator))
						   .Trim(IOUtility.PathSeparator);
			}

			return Path.Combine(RootDirectory.FullName, node.FullName.Replace(TreeGumpNode.Separator, IOUtility.PathSeparator))
					   .Trim(IOUtility.PathSeparator);
		}

		protected override void CompileNodes(Dictionary<TreeGumpNode, Action<Rectangle, int, TreeGumpNode>> list)
		{
			base.CompileNodes(list);

			list.Clear();

			MapTree(list, RootDirectory);
		}

		private void MapTree(Dictionary<TreeGumpNode, Action<Rectangle, int, TreeGumpNode>> list, DirectoryInfo dir)
		{
			TreeGumpNode node = GetPath(dir);

			list[node] = RenderPanel;

			var dirs = dir.EnumerateDirectories();

			foreach (var d in dirs.OrderByNatural(d => d.Name))
			{
				node = GetPath(dir);

				if (node >= SelectedNode)
				{
					MapTree(list, d);
				}
				else
				{
					list[node] = RenderPanel;
				}
			}
		}

		protected void RenderPanel(Rectangle panel, int index, TreeGumpNode node)
		{
			if (SelectedFile != null)
			{
				RenderFilePanel(panel, index, node);
			}
			else
			{
				RenderDirectoryPanel(panel, index, node);
			}
		}

		// 310 x 350
		protected virtual void RenderDirectoryPanel(Rectangle panel, int index, TreeGumpNode node)
		{
			var x = panel.X + 20;
			var y = panel.Y + 20;

			var w = panel.Width - 40;
			var h = panel.Height - 20;

			var xx = x;
			var yy = y;

			var subIndex = 0;

			var xMax = w / 65;
			var yMax = h / 110;

			var max = xMax * yMax;

			var range = Enumerable.Empty<FileSystemInfo>()
								  .Union(SelectedDirectory.EnumerateDirectories().OrderByNatural(d => d.Name))
								  .Union(SelectedDirectory.EnumerateFiles().OrderByNatural(f => f.Name));

			foreach (var info in range.Take(max))
			{
				// 65 x 110
				if (info is DirectoryInfo)
				{
					var dir = (DirectoryInfo)info;

					// 56 x 80
					AddButton(xx, yy, 9810, 9810, b => SelectDirectory(dir)); // 56 x 50
					AddHtml(xx, yy + 50, 56, 40, info.Name.WrapUOHtmlCenter(), false, false); // 56 x 40
				}
				else if (info is FileInfo)
				{
					var file = (FileInfo)info;

					// 56 x 80
					AddButton(xx + 5, yy, 2234, 2234, b => SelectFile(file)); // 46 x 50
					AddHtml(xx, yy + 50, 56, 40, info.Name.WrapUOHtmlCenter(), false, false); // 56 x 40
				}

				if (++subIndex % xMax == 0)
				{
					xx = x;
					yy += 110;
				}
				else
				{
					xx += 65;
				}
			}
		}

		// 310 x 350
		protected virtual void RenderFilePanel(Rectangle panel, int index, TreeGumpNode node)
		{
			if (FileMime.IsCommonText(SelectedFile))
			{
				var content = File.ReadAllText(SelectedFile.FullName);

				AddHtml(panel.X, panel.Y, panel.Width, panel.Height, content, HtmlColor, Color.Black);
			}
		}

		protected override void OnSelected(TreeGumpNode oldNode, TreeGumpNode newNode)
		{
			var path = GetPath(newNode);

			if (Directory.Exists(path))
			{
				SelectedDirectory = new DirectoryInfo(path);
			}

			SelectedFile = null;

			base.OnSelected(oldNode, newNode);
		}

		public void SelectDirectory(DirectoryInfo dir)
		{
			var path = GetPath(dir);

			var node = List.FirstOrDefault(n => n == path);

			if (node != null)
			{
				SelectedNode = node;
			}
			else
			{
				SelectedNode = path;
			}

			SelectedDirectory = dir;
			SelectedFile = null;

			Refresh(true);
		}

		public void SelectFile(FileInfo file)
		{
			var path = GetPath(file.Directory);

			var node = List.FirstOrDefault(n => n == path);

			if (node != null)
			{
				SelectedNode = node;
			}
			else
			{
				SelectedNode = path;
			}

			SelectedDirectory = file.Directory;
			SelectedFile = file;

			Refresh(true);
		}
	}
}