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
using System.Linq;

using Server;
#endregion

namespace VitaNex.SuperGumps.UI
{
	public class TreeGumpNode : IEquatable<TreeGumpNode>, IEquatable<string>
	{
		public static char Separator = '|';

		public static readonly TreeGumpNode Empty = new TreeGumpNode(String.Empty);

		public TreeGumpNode Parent { get; private set; }

		public TreeGumpNode RootParent
		{
			get
			{
				if (!HasParent)
				{
					return null;
				}

				var p = Parent;

				while (p.HasParent)
				{
					p = p.Parent;
				}

				return p;
			}
		}

		public string Name { get; private set; }
		public string FullName { get; private set; }

		public bool HasParent => Parent != null;

		public bool IsRoot => !HasParent;
		public bool IsEmpty => String.IsNullOrWhiteSpace(FullName);

		public int Depth => IsEmpty ? 0 : GetParents().Count();

		public TreeGumpNode(string path)
		{
			FullName = path ?? String.Empty;

			var parents = FullName.Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries);

			if (parents.Length == 0)
			{
				Name = FullName;
				return;
			}

			Name = parents.LastOrDefault() ?? FullName;

			if (parents.Length > 1)
			{
				Parent = new TreeGumpNode(String.Join(Separator.ToString(), parents.Take(parents.Length - 1)));
			}
		}

		public bool IsChildOf(string d)
		{
			if (String.IsNullOrWhiteSpace(d) || IsEmpty)
			{
				return false;
			}

			var p = Parent;

			while (p != null)
			{
				if (p.FullName == d)
				{
					return true;
				}

				p = p.Parent;
			}

			return false;
		}

		public bool IsChildOf(TreeGumpNode d)
		{
			if (d == null || d.IsEmpty || IsEmpty)
			{
				return false;
			}

			var p = Parent;

			while (p != null)
			{
				if (p == d)
				{
					return true;
				}

				p = p.Parent;
			}

			return false;
		}

		public bool IsParentOf(TreeGumpNode d)
		{
			return d != null && d.IsChildOf(this);
		}

		public IEnumerable<TreeGumpNode> GetParents()
		{
			if (IsEmpty)
			{
				yield break;
			}

			var c = this;

			while (c.HasParent)
			{
				c = c.Parent;

				yield return c;
			}
		}

		public override int GetHashCode()
		{
			unchecked
			{
				if (IsEmpty)
				{
					return 0;
				}

				var hash = FullName.Length;
				hash = (hash * 397) ^ FullName.ToLower().GetHashCode();
				return hash;
			}
		}

		public override bool Equals(object obj)
		{
			return (obj is string && Equals((string)obj)) || (obj is TreeGumpNode && Equals((TreeGumpNode)obj));
		}

		public virtual bool Equals(TreeGumpNode other)
		{
			return !ReferenceEquals(other, null) && Equals(other.FullName);
		}

		public virtual bool Equals(string other)
		{
			return Insensitive.Equals(FullName, other);
		}

		public static bool operator ==(TreeGumpNode l, TreeGumpNode r)
		{
			return ReferenceEquals(l, null) ? ReferenceEquals(r, null) : l.Equals(r);
		}

		public static bool operator !=(TreeGumpNode l, TreeGumpNode r)
		{
			return ReferenceEquals(l, null) ? !ReferenceEquals(r, null) : !l.Equals(r);
		}

		public static bool operator >(TreeGumpNode l, TreeGumpNode r)
		{
			return !ReferenceEquals(l, null) && !ReferenceEquals(r, null) && r.IsChildOf(l);
		}

		public static bool operator <(TreeGumpNode l, TreeGumpNode r)
		{
			return !ReferenceEquals(l, null) && !ReferenceEquals(r, null) && l.IsChildOf(r);
		}

		public static bool operator >=(TreeGumpNode l, TreeGumpNode r)
		{
			return l == r || l > r;
		}

		public static bool operator <=(TreeGumpNode l, TreeGumpNode r)
		{
			return l == r || l < r;
		}

		public static implicit operator TreeGumpNode(string path)
		{
			return String.IsNullOrWhiteSpace(path) ? Empty : new TreeGumpNode(path);
		}

		public static implicit operator string(TreeGumpNode node)
		{
			return ReferenceEquals(node, null) ? String.Empty : node.FullName;
		}
	}
}