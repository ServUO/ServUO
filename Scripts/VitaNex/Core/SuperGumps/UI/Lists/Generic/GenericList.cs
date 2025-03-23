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

using Server;
using Server.Gumps;
#endregion

namespace VitaNex.SuperGumps.UI
{
	public abstract class GenericListGump<T> : ListGump<T>
	{
		public bool ChangesPending { get; protected set; }

		public virtual bool CanAdd { get; set; }
		public virtual bool CanRemove { get; set; }
		public virtual bool CanClear { get; set; }

		public virtual Action<T> AddCallback { get; set; }
		public virtual Action<T> RemoveCallback { get; set; }
		public virtual Action<List<T>> ApplyCallback { get; set; }
		public virtual Action ClearCallback { get; set; }

		public GenericListGump(
			Mobile user,
			Gump parent = null,
			int? x = null,
			int? y = null,
			IEnumerable<T> list = null,
			string emptyText = null,
			string title = null,
			IEnumerable<ListGumpEntry> opts = null,
			bool canAdd = true,
			bool canRemove = true,
			bool canClear = true,
			Action<T> addCallback = null,
			Action<T> removeCallback = null,
			Action<List<T>> applyCallback = null,
			Action clearCallback = null)
			: base(user, parent, x, y, list, emptyText, title, opts)
		{
			CanAdd = canAdd;
			CanRemove = canRemove;
			CanClear = canClear;

			AddCallback = addCallback;
			RemoveCallback = removeCallback;
			ApplyCallback = applyCallback;
			ClearCallback = clearCallback;

			ForceRecompile = true;
		}

		public override string GetSearchKeyFor(T key)
		{
			return key != null ? key.ToString() : base.GetSearchKeyFor(default(T));
		}

		protected override string GetLabelText(int index, int pageIndex, T entry)
		{
			return entry != null ? entry.ToString() : base.GetLabelText(index, pageIndex, default(T));
		}

		protected override void Compile()
		{
			if (ChangesPending)
			{
				if (!Title.EndsWith(" *"))
				{
					Title += " *";
				}
			}
			else
			{
				if (Title.EndsWith(" *"))
				{
					Title = Title.Substring(0, Title.Length - 2);
				}
			}

			base.Compile();
		}

		protected override void CompileMenuOptions(MenuGumpOptions list)
		{
			if (ChangesPending)
			{
				list.Insert(0, new ListGumpEntry("Apply Changes", HandleApplyChanges, HighlightHue));
				list.Insert(1, new ListGumpEntry("Clear Changes", HandleClearChanges, HighlightHue));
			}
			else
			{
				list.RemoveEntry("Apply Changes");
				list.RemoveEntry("Clear Changes");
			}

			if (CanAdd)
			{
				list.Insert(2, new ListGumpEntry("Add", HandleAdd, HighlightHue));
			}
			else
			{
				list.RemoveEntry("Add");
			}

			if (CanClear)
			{
				list.Insert(3, new ListGumpEntry("Clear", HandleClear, ErrorHue));
			}
			else
			{
				list.RemoveEntry("Clear");
			}

			base.CompileMenuOptions(list);
		}

		protected override void SelectEntry(GumpButton button, T entry)
		{
			base.SelectEntry(button, entry);

			var opts = new MenuGumpOptions();

			CompileEntryOptions(opts, entry);

			new MenuGump(User, Refresh(), opts, button).Send();
		}

		protected virtual void CompileEntryOptions(MenuGumpOptions opts, T entry)
		{
			if (CanRemove)
			{
				opts.AppendEntry("Remove", b => HandleRemove(entry), ErrorHue);
			}
		}

		protected virtual void HandleAdd()
		{
			if (!OnBeforeListAdd())
			{
				return;
			}

			var obj = GetListAddObject();

			AddToList(obj);
		}

		protected virtual void HandleRemove(T entry)
		{
			if (OnBeforeListRemove())
			{
				RemoveFromList(entry);
			}
		}

		protected virtual void HandleClear()
		{
			if (OnBeforeListClear())
			{
				ClearList();
			}
		}

		protected virtual void HandleApplyChanges()
		{
			if (!OnBeforeApplyChanges())
			{
				return;
			}

			var destList = GetExternalList();

			ApplyChangesToList(destList);
		}

		protected virtual void HandleClearChanges()
		{
			if (!OnBeforeClearChanges())
			{
				return;
			}

			var restoList = GetExternalList();

			ClearChanges(restoList);
		}

		public abstract List<T> GetExternalList();
		public abstract T GetListAddObject();

		public virtual bool Validate(T obj)
		{
			return obj != null;
		}

		public virtual void AddToList(T obj)
		{
			if (!Validate(obj))
			{
				Refresh();
				return;
			}

			List.Add(obj);

			if (AddCallback != null)
			{
				AddCallback(obj);
			}

			ChangesPending = true;
			Refresh(true);
		}

		public virtual void RemoveFromList(T obj)
		{
			if (obj != null && List.Contains(obj))
			{
				List.Remove(obj);
			}

			if (RemoveCallback != null)
			{
				RemoveCallback(obj);
			}

			ChangesPending = true;
			Refresh(true);
		}

		public virtual void ClearList()
		{
			if (List != null)
			{
				List.Clear();
			}

			if (ClearCallback != null)
			{
				ClearCallback();
			}

			ChangesPending = true;
			Refresh(true);
		}

		public virtual void ClearChanges(List<T> resto)
		{
			if (resto != null && resto != List)
			{
				List.Clear();
				List.AddRange(resto);
			}

			ChangesPending = false;
			Refresh(true);
		}

		public virtual void ApplyChangesToList(List<T> list)
		{
			if (list != null && list != List)
			{
				list.Clear();
				list.AddRange(List);
			}

			if (ApplyCallback != null)
			{
				ApplyCallback(list);
			}

			ChangesPending = false;
			Refresh(true);
		}

		protected virtual bool OnBeforeClearChanges()
		{
			return true;
		}

		protected virtual bool OnBeforeApplyChanges()
		{
			return true;
		}

		protected virtual bool OnBeforeListAdd()
		{
			return true;
		}

		protected virtual bool OnBeforeListRemove()
		{
			return true;
		}

		protected virtual bool OnBeforeListClear()
		{
			return true;
		}

		protected override void OnClosed(bool all)
		{
			base.OnClosed(all);

			if (!ChangesPending)
			{
				return;
			}

			new ConfirmDialogGump(User)
			{
				Title = "Apply Changes?",
				Html = "There are changes waiting that have not been applied.\nDo you want to apply them now?",
				AcceptHandler = b =>
				{
					HandleApplyChanges();
					Close(all);
				}
			}.Send();
		}
	}
}