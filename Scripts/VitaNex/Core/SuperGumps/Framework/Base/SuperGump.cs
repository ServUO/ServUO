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
using System.Linq;

using Server;
using Server.Gumps;
using Server.Network;

using Ultima;

using VitaNex.Collections;
#endregion

namespace VitaNex.SuperGumps
{
	public abstract partial class SuperGump : Gump, IEquatable<SuperGump>, IDisposable
	{
		public static int DefaultX = 250;
		public static int DefaultY = 200;

		public static int DefaultTextHue = 85;
		public static int DefaultErrorHue = 34;
		public static int DefaultHighlightHue = 51;

		public static TimeSpan DefaultRefreshRate = TimeSpan.FromSeconds(30);
		public static Color DefaultHtmlColor = Color.PaleGoldenrod;

		public static TimeSpan PollInterval = TimeSpan.FromMilliseconds(100.0);
		public static TimeSpan DClickInterval = TimeSpan.FromMilliseconds(1000.0);

		private static void InternalClose(Mobile m, Gump g)
		{
			if (m == null || g == null)
			{
				return;
			}

			var ns = m.NetState;

			try
			{
				g.OnServerClose(ns);
			}
			catch
			{ }

			if (ns == null)
			{
				return;
			}

			m.Send(new CloseGump(g.TypeID, 0));
			ns.RemoveGump(g);
		}

		private static void InternalClose(SuperGump g)
		{
			if (g != null)
			{
				InternalClose(g.User, g);
			}
		}

		private static void InternalCloseDupes(SuperGump g)
		{
			if (g == null || g.IsDisposed || g.User == null || g.AllowMultiple)
			{
				return;
			}

			var gumps = Instances.GetValue(g.User);

			if (gumps == null)
			{
				return;
			}

			var t = g.GetType();
			var i = gumps.Count;

			while (--i >= 0)
			{
				if (gumps[i] != g && gumps[i].TypeEquals(t, false))
				{
					InternalClose(gumps[i]);
				}
			}
		}

		private static void InternalSend(SuperGump g)
		{
			if (g == null || g.IsDisposed || g.Modal || g.User == null)
			{
				return;
			}

			var gumps = Instances.GetValue(g.User);

			if (gumps == null)
			{
				return;
			}

			var i = gumps.Count;
			var j = gumps.IndexOf(g);

			while (--i >= 0)
			{
				if ((i >= j && j >= 0) || !gumps.InBounds(i))
				{
					continue;
				}

				var o = gumps[i];

				if (o != g && o.Modal && o.IsOpen && !o.IsDisposed)
				{
					gumps[i].Refresh();
				}
			}
		}

		public static TGump Send<TGump>(TGump gump)
			where TGump : SuperGump
		{
			return gump != null ? (gump.Compiled ? gump.Refresh() : gump.Send()) as TGump : null;
		}

		private Gump _Parent;

		private DateTime _UtcNow = DateTime.UtcNow;

		protected int NextButtonID = 1;
		protected int NextSwitchID;
		protected int NextTextInputID;

		private bool _Modal;
		private bool _EnablePolling;
		private bool _AutoRefresh;

		public DateTime LastButtonClick { get; private set; }
		public bool DoubleClicked { get; private set; }

		public bool IsDisposed { get; private set; }

		public int Identity
		{
			get => TypeID;
			set
			{
				if (!this.SetPropertyValue("TypeID", value))
				{
					this.SetFieldValue("m_TypeID", value);
				}
			}
		}

		public virtual PollTimer InstancePoller { get; protected set; }

		public virtual bool InitPolling => false;

		public bool EnablePolling
		{
			get => _EnablePolling;
			set
			{
				if (_EnablePolling && !value)
				{
					_EnablePolling = false;

					if (InstancePoller == null)
					{
						return;
					}

					InstancePoller.Dispose();
					InstancePoller = null;
				}
				else if (!_EnablePolling && value)
				{
					_EnablePolling = true;

					InitPollTimer();
				}
				else if (value)
				{
					InitPollTimer();
				}
			}
		}

		public virtual Gump Parent
		{
			get => _Parent;
			set
			{
				if (_Parent == value || value == this)
				{
					return;
				}

				if (!IsDisposed && _Parent is SuperGump)
				{
					((SuperGump)_Parent).RemoveChild(this);
				}

				_Parent = value;

				if (!IsDisposed && _Parent is SuperGump)
				{
					((SuperGump)_Parent).AddChild(this);
				}
			}
		}

		public virtual Mobile User { get; set; }

		public virtual bool Modal
		{
			get => _Modal;
			set
			{
				_Modal = value;

				if (!_Modal && value)
				{
					_Modal = true;

					CanMove = false;
					CanResize = false;
				}
				else if (_Modal && !value)
				{
					_Modal = false;
				}
			}
		}

		public virtual bool AllowMultiple { get; set; }

		public virtual bool ModalSafety { get; set; }

		public virtual bool BlockSpeech { get; set; }
		public virtual bool BlockMovement { get; set; }

		public virtual bool ForceRecompile { get; set; }

		public virtual int TextHue { get; set; }
		public virtual int ErrorHue { get; set; }
		public virtual int HighlightHue { get; set; }

		public virtual Direction Direction { get; set; }

		public virtual TimeSpan AutoRefreshRate { get; set; }
		public virtual DateTime LastAutoRefresh { get; set; }

		private bool _PollingWasDisabled;

		public virtual bool AutoRefresh
		{
			get => _AutoRefresh;
			set
			{
				if (!_AutoRefresh && value)
				{
					_AutoRefresh = true;

					if (!EnablePolling)
					{
						_PollingWasDisabled = EnablePolling = true;
					}
				}
				else if (_AutoRefresh && !value)
				{
					_AutoRefresh = false;

					EnablePolling = !_PollingWasDisabled;
				}
			}
		}

		public bool IsOpen { get; private set; }
		public bool Hidden { get; private set; }
		public bool Compiled { get; private set; }
		public bool Initialized { get; private set; }

		public virtual bool RandomButtonID { get; set; }
		public virtual bool RandomTextEntryID { get; set; }
		public virtual bool RandomSwitchID { get; set; }

		public int IndexedPage => GetEntries<GumpPage>().Aggregate(-1, (max, p) => Math.Max(max, p.Page));

		public bool IsEnhancedClient => User != null && User.NetState != null && User.NetState.IsEnhanced();

		public bool SupportsUltimaStore => User != null && User.NetState != null && User.NetState.SupportsUltimaStore();

		public bool SupportsEndlessJourney => User != null && User.NetState != null && User.NetState.SupportsEndlessJourney();

		public event Action<SuperGump, bool> OnActionSend;
		public event Action<SuperGump, bool> OnActionClose;
		public event Action<SuperGump, bool> OnActionHide;
		public event Action<SuperGump> OnActionRefresh;
		public event Action<SuperGump> OnActionDispose;
		public event Action<SuperGump> OnActionClick;
		public event Action<SuperGump> OnActionDoubleClick;

		public SuperGump(Mobile user, Gump parent = null, int? x = null, int? y = null)
			: base(x ?? DefaultX, y ?? DefaultY)
		{
			AssignCollections();

			TextHue = DefaultTextHue;
			ErrorHue = DefaultErrorHue;
			HighlightHue = DefaultHighlightHue;

			User = user;
			Parent = parent;

			Modal = false;
			ModalSafety = true;
			BlockSpeech = false;
			BlockMovement = false;
			UseSounds = true;

			AutoRefresh = false;
			LastAutoRefresh = DateTime.UtcNow;
			AutoRefreshRate = DefaultRefreshRate;

			AnimationRate = DefaultAnimationRate;

			InitSounds();
			InitAssets();

			RegisterInstance();

			EnablePolling = InitPolling;
		}

		~SuperGump()
		{
			Dispose();
		}

		public virtual void AssignCollections()
		{
			if (Entries.Capacity < 0x40)
			{
				Entries.Capacity = 0x40;
			}

			if (_Linked == null)
			{
				ObjectPool.Acquire(out _Linked);
			}

			if (_Children == null)
			{
				ObjectPool.Acquire(out _Children);
			}

			if (_Controls == null)
			{
				ObjectPool.Acquire(out _Controls);
			}

			if (_Layout == null)
			{
				ObjectPool.Acquire(out _Layout);
			}

			if (_Buttons == null)
			{
				ObjectPool.Acquire(out _Buttons);
			}

			if (_TileButtons == null)
			{
				ObjectPool.Acquire(out _TileButtons);
			}

			if (_Switches == null)
			{
				ObjectPool.Acquire(out _Switches);
			}

			if (_Radios == null)
			{
				ObjectPool.Acquire(out _Radios);
			}

			if (_TextInputs == null)
			{
				ObjectPool.Acquire(out _TextInputs);
			}

			if (_LimitedTextInputs == null)
			{
				ObjectPool.Acquire(out _LimitedTextInputs);
			}

			if (_TextTooltips == null)
			{
				ObjectPool.Acquire(out _TextTooltips);
			}
		}

		public void InitPollTimer()
		{
			if (InstancePoller != null)
			{
				InstancePoller.Dispose();
				InstancePoller = null;
			}

			if (EnablePolling)
			{
				InstancePoller = PollTimer.CreateInstance(PollInterval, OnInstancePollCheck, CanPollInstance);
			}
		}

		public int NewButtonID()
		{
			return RandomButtonID ? (NextButtonID += Utility.Random(Utility.Dice(6, 6, 6)) + 1) : NextButtonID++;
		}

		public int NewTextEntryID()
		{
			return RandomTextEntryID ? (NextTextInputID += Utility.Random(Utility.Dice(6, 6, 6)) + 1) : NextTextInputID++;
		}

		public int NewSwitchID()
		{
			return RandomSwitchID ? (NextSwitchID += Utility.Random(Utility.Dice(6, 6, 6)) + 1) : NextSwitchID++;
		}

		protected virtual void Compile()
		{
			if (IsDisposed)
			{
				return;
			}

			if (Modal)
			{
				if (X > 0)
				{
					ModalXOffset = X;
					X = 0;
				}

				if (Y > 0)
				{
					ModalYOffset = Y;
					Y = 0;
				}

				CanMove = false;
				CanResize = false;
			}
			else
			{
				if (ModalXOffset > 0)
				{
					X = ModalXOffset;
					ModalXOffset = 0;
				}

				if (ModalYOffset > 0)
				{
					Y = ModalYOffset;
					ModalYOffset = 0;
				}
			}
		}

		protected virtual void CompileLayout(SuperGumpLayout layout)
		{
			if (IsDisposed)
			{
				return;
			}

			if (Modal)
			{
				layout.Add("alpharegion/modal", () => AddModalRegion(0, 0, 2560, 1440));
			}
		}

		protected virtual void OnLayoutApplied()
		{ }

		protected virtual void OnClick()
		{
			if (!DoubleClicked)
			{
				PlayClickSound();
			}

			if (OnActionClick != null)
			{
				OnActionClick(this);
			}
		}

		protected virtual void OnDoubleClick()
		{
			PlayDoubleClickSound();

			if (OnActionDoubleClick != null)
			{
				OnActionDoubleClick(this);
			}
		}

		protected virtual void OnInstancePollCheck()
		{
			_UtcNow = DateTime.UtcNow;

			if (!IsDisposed && CanAutoRefresh())
			{
				OnAutoRefresh();
			}
		}

		protected virtual bool CanPollInstance()
		{
			return !IsDisposed && IsOpen && !Hidden && EnablePolling;
		}

		protected virtual void OnAutoRefresh()
		{
			if (IsDisposed)
			{
				return;
			}

			LastAutoRefresh = _UtcNow;

			Refresh(true);
		}

		protected virtual bool CanAutoRefresh()
		{
			if (IsDisposed || !IsOpen || Hidden)
			{
				return false;
			}

			if (!AutoRefresh || LastAutoRefresh + AutoRefreshRate > _UtcNow)
			{
				return false;
			}

			if (Modal && HasOpenChildren)
			{
				return false;
			}

			if (!Modal && EnumerateInstances<SuperGump>(User, true)
					.Any(o => o != this && o.IsOpen && !o.Hidden && (o.Modal || o.IsChildOf(this))))
			{
				return false;
			}

			return true;
		}

		protected virtual void OnRefreshed()
		{
			if (IsDisposed)
			{
				return;
			}

			LastAutoRefresh = DateTime.UtcNow;

			RegisterInstance();

			if (Parent is SuperGump)
			{
				((SuperGump)Parent).AddChild(this);
			}

			if (InstancePoller == null)
			{
				InitPollTimer();
			}
			else
			{
				InstancePoller.Running = EnablePolling;
			}

			Linked.ForEachReverse(g => g.OnLinkRefreshed(this));

			PlayRefreshSound();

			if (OnActionRefresh != null)
			{
				OnActionRefresh(this);
			}
		}

		public virtual void Refresh(GumpButton b)
		{
			if (!IsDisposed)
			{
				Refresh(true);
			}
		}

		public virtual void Refresh(GumpImageTileButton b)
		{
			if (!IsDisposed)
			{
				Refresh(true);
			}
		}

		public SuperGump Refresh()
		{
			return Refresh(false);
		}

		public virtual SuperGump Refresh(bool recompile)
		{
			if (IsDisposed)
			{
				return this;
			}

			if (User == null || !User.IsOnline())
			{
				return this;
			}

			if (IsOpen)
			{
				InternalClose(this);
			}

			if (ForceRecompile || !Compiled || recompile)
			{
				return Send();
			}

			if (Modal && ModalSafety && Buttons.Count == 0 && TileButtons.Count == 0)
			{
				CanClose = true;
				CanDispose = true;
			}

			IsOpen = User.SendGump(this);

			Hidden = false;

			OnRefreshed();

			return this;
		}

		public T Send<T>()
			where T : SuperGump
		{
			return Send() as T;
		}

		private volatile bool _Sending;

		public virtual SuperGump Send()
		{
			if (IsDisposed || _Sending)
			{
				return this;
			}

			if (User == null || !User.IsOnline())
			{
				return this;
			}

			_Sending = true;

			return VitaNexCore.TryCatchGet(
					   () =>
					   {
						   if (IsOpen)
						   {
							   InternalClose(this);
						   }

						   Compile();

						   Clear();

						   AddPage();

						   CompileLayout(Layout);

						   Layout.ApplyTo(this);

						   OnLayoutApplied();

						   InvalidateAnimations();
						   InvalidateOffsets();
						   InvalidateSize();

						   Compiled = true;

						   if (!CanDispose && CanClose)
						   {
							   CanClose = false;
						   }

						   var wasHolding = false;

						   if (Modal)
						   {
							   if (ModalSafety && Buttons.Count == 0 && TileButtons.Count == 0)
							   {
								   CanDispose = true;
								   CanClose = true;
							   }

							   if (User != null && User.Holding != null)
							   {
								   var held = User.Holding;

								   if (held.GetBounce() != null)
								   {
									   held.Bounce(User);
								   }
								   else
								   {
									   User.GiveItem(held, GiveFlags.PackFeet, false);
									   User.Holding = null;
									   held.ClearBounce();
								   }

								   wasHolding = held != null;
							   }
						   }

						   if (OnBeforeSend())
						   {
							   InternalCloseDupes(this);

							   Initialized = true;
							   IsOpen = User.SendGump(this);
							   Hidden = !IsOpen;

							   if (IsOpen)
							   {
								   OnSend();

								   InternalSend(this);
							   }
							   else
							   {
								   OnSendFail();
							   }
						   }
						   else
						   {
							   Close();
						   }

						   _Sending = false;

						   if (wasHolding)
						   {
							   Timer.DelayCall(TimeSpan.FromSeconds(0.5), () => Refresh(true));
						   }

						   return this;
					   },
					   e =>
					   {
						   _Sending = false;

						   Console.WriteLine("SuperGump '{0}' could not be sent, an exception was caught:", GetType().FullName);

						   e.ToConsole();

						   IsOpen = Hidden = false;

						   OnSendFail();
					   }) ?? this;
		}

		protected virtual bool OnBeforeSend()
		{
			return !IsDisposed && User != null && User.IsOnline();
		}

		protected virtual void OnSend()
		{
			if (IsDisposed)
			{
				return;
			}

			LastAutoRefresh = DateTime.UtcNow;

			RegisterInstance();

			if (Parent is SuperGump)
			{
				((SuperGump)Parent).AddChild(this);
			}

			if (InstancePoller == null)
			{
				InitPollTimer();
			}
			else
			{
				InstancePoller.Running = EnablePolling;
			}

			Linked.ForEachReverse(g => g.OnLinkSend(this));

			PlaySendSound();

			if (OnActionSend != null)
			{
				OnActionSend(this, true);
			}
		}

		protected virtual void OnSendFail()
		{
			if (IsDisposed)
			{
				return;
			}

			UnregisterInstance();

			if (Parent is SuperGump)
			{
				((SuperGump)Parent).RemoveChild(this);
			}

			if (InstancePoller != null)
			{
				InstancePoller.Dispose();
				InstancePoller = null;
			}

			Linked.ForEachReverse(g => g.OnLinkSendFail(this));

			if (OnActionSend != null)
			{
				OnActionSend(this, false);
			}
		}

		protected virtual void OnHidden(bool all)
		{
			if (IsDisposed)
			{
				return;
			}

			Linked.ForEachReverse(g => g.OnLinkHidden(this));

			PlayHideSound();

			if (OnActionHide != null)
			{
				OnActionHide(this, all);
			}
		}

		public virtual void Hide(GumpButton b)
		{
			if (!IsDisposed)
			{
				Hide(false);
			}
		}

		public virtual void Hide(GumpImageTileButton b)
		{
			if (!IsDisposed)
			{
				Hide(false);
			}
		}

		public SuperGump Hide()
		{
			return Hide(false);
		}

		public virtual SuperGump Hide(bool all)
		{
			if (IsDisposed)
			{
				return this;
			}

			Hidden = true;

			if (IsOpen)
			{
				InternalClose(this);
			}

			if (Parent != null && all)
			{
				if (Parent is SuperGump)
				{
					((SuperGump)Parent).Hide(true);
				}
				else
				{
					InternalClose(User, Parent);
				}
			}

			OnHidden(all);
			return this;
		}

		public void Show()
		{
			Refresh(true);
		}

		protected virtual void OnClosed(bool all)
		{
			if (IsDisposed)
			{
				return;
			}

			UnregisterInstance();

			if (InstancePoller != null)
			{
				InstancePoller.Dispose();
				InstancePoller = null;
			}

			Linked.ForEachReverse(g => g.OnLinkClosed(this));

			PlayCloseSound();

			if (OnActionClose != null)
			{
				OnActionClose(this, all);
			}
		}

		public virtual void Close(GumpButton b)
		{
			if (!IsDisposed)
			{
				Close(false);
			}
		}

		public virtual void Close(GumpImageTileButton b)
		{
			if (!IsDisposed)
			{
				Close(false);
			}
		}

		public void Close()
		{
			Close(false);
		}

		public virtual void Close(bool all)
		{
			if (IsDisposed)
			{
				return;
			}

			if (IsOpen || Hidden)
			{
				InternalClose(this);
			}

			IsOpen = Hidden = false;

			if (Parent != null)
			{
				if (all)
				{
					if (Parent is SuperGump)
					{
						((SuperGump)Parent).Close(true);
					}
					else
					{
						InternalClose(User, Parent);
					}
				}
				else
				{
					if (Parent is SuperGump)
					{
						((SuperGump)Parent).Refresh(true);
					}
					else
					{
						User.SendGump(Parent);
					}
				}
			}

			OnClosed(all);
		}

		protected virtual void Clear()
		{
			_Layout.Clear();

			_Buttons.Clear();
			_TileButtons.Clear();
			_Switches.Clear();
			_Radios.Clear();
			_TextInputs.Clear();
			_LimitedTextInputs.Clear();

			_TextTooltips.Values.ForEach(Spoof.Free);
			_TextTooltips.Clear();

			Entries.Clear();

			NextButtonID = 1;
			NextSwitchID = 0;
			NextTextInputID = 0;
		}

		protected void ClearCache()
		{
			if (this.GetFieldValue("m_Strings", out List<string> strings) || this.GetFieldValue("_Strings", out strings))
			{
				strings.Clear();
			}
		}

		protected virtual void OnSpeech(SpeechEventArgs e)
		{
			if (BlockSpeech && !IsDisposed && IsOpen && !Hidden && !e.Blocked && User.AccessLevel < AccessLevel.Counselor)
			{
				e.Blocked = true;
			}
		}

		protected virtual void OnMovement(MovementEventArgs e)
		{
			if (IsDisposed || !IsOpen || Hidden)
			{
				return;
			}

			var d = e.Direction & Direction.Mask;

			GetDirections(d, out var up, out var right, out var down, out var left);

			var blocked = e.Blocked;

			OnMovement(ref blocked, d, up, right, down, left);

			Direction = d;

			e.Blocked = blocked;
		}

		protected virtual void OnMovement(ref bool blocked, Direction d, bool up, bool right, bool down, bool left)
		{
			if (BlockMovement)
			{
				blocked = true;
			}
		}

		public override void OnServerClose(NetState owner)
		{
			if (IsDisposed)
			{
				return;
			}

			IsOpen = false;

			UnregisterInstance();

			if (!Hidden && InstancePoller != null)
			{
				InstancePoller.Dispose();
				InstancePoller = null;
			}

			base.OnServerClose(owner);
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			if (IsDisposed)
			{
				return;
			}

			foreach (var o in Switches.Keys)
			{
				HandleSwitch(o, info.IsSwitched(o.SwitchID));
			}

			foreach (var o in Radios.Keys)
			{
				HandleRadio(o, info.IsSwitched(o.SwitchID));
			}

			foreach (var o in TextInputs.Keys)
			{
				var r = info.GetTextEntry(o.EntryID);

				if (r != null)
				{
					HandleTextInput(o, r.Text ?? String.Empty);
				}
			}

			foreach (var o in LimitedTextInputs.Keys)
			{
				var r = info.GetTextEntry(o.EntryID);

				if (r != null)
				{
					HandleLimitedTextInput(o, r.Text ?? String.Empty);
				}
			}

			var button1 = GetButtonEntry(info.ButtonID);
			var button2 = GetTileButtonEntry(info.ButtonID);

			if (button1 == null && button2 == null)
			{
				Close();
			}
			else
			{
				Hide();

				if (button1 != null)
				{
					HandleButtonClick(button1);
				}

				if (button2 != null)
				{
					HandleTileButtonClick(button2);
				}
			}

			base.OnResponse(sender, info);
		}

		public virtual void GetDirections(Direction d, out bool up, out bool right, out bool down, out bool left)
		{
			up = right = down = left = false;

			switch (Direction)
			{
				case Direction.Up:
					up = true;
					break;
				case Direction.North:
					up = right = true;
					break;
				case Direction.Right:
					right = true;
					break;
				case Direction.East:
					right = down = true;
					break;
				case Direction.Down:
					down = true;
					break;
				case Direction.South:
					down = left = true;
					break;
				case Direction.Left:
					left = true;
					break;
				case Direction.West:
					left = up = true;
					break;
			}
		}

		public virtual void GetOffset(bool up, bool right, bool down, bool left, ref int x, ref int y)
		{
			if (up)
			{
				--y;
			}

			if (right)
			{
				++x;
			}

			if (down)
			{
				++y;
			}

			if (left)
			{
				--x;
			}
		}

		public virtual void GetOffset(Direction d, ref int x, ref int y)
		{
			switch (d & Direction.Mask)
			{
				case Direction.Up:
					--y;
					break;
				case Direction.North:
				{
					++x;
					--y;
				}
				break;
				case Direction.Right:
					++x;
					break;
				case Direction.East:
				{
					++x;
					++y;
				}
				break;
				case Direction.Down:
					++y;
					break;
				case Direction.South:
				{
					--x;
					++y;
				}
				break;
				case Direction.Left:
					--x;
					break;
				case Direction.West:
				{
					--x;
					--y;
				}
				break;
			}
		}

		public virtual Size GetImageSize(int imageID)
		{
			return GumpsExtUtility.GetImageSize(imageID);
		}

		public virtual Size GetItemSize(int itemID)
		{
			return ArtExtUtility.GetImageSize(itemID);
		}

		public virtual Point GetItemOffset(int itemID)
		{
			return ArtExtUtility.GetImageOffset(itemID);
		}

		public virtual IEnumerable<T> GetEntries<T>()
			where T : GumpEntry
		{
			return Entries.OfType<T>();
		}

		public override int GetHashCode()
		{
			return Serial;
		}

		public override bool Equals(object other)
		{
			return Equals(other as SuperGump);
		}

		public virtual bool Equals(SuperGump other)
		{
			return !ReferenceEquals(other, null) && other.Serial == Serial;
		}

		private void DisposeEntry(GumpEntry e)
		{
			if (e == null || e.Parent != this)
			{
				return;
			}

			if (e is IDisposable)
			{
				((IDisposable)e).Dispose();
			}
			else
			{
				e.Parent = null;
			}

			Entries.Remove(e);
		}

		public void Dispose()
		{
			if (IsDisposed)
			{
				return;
			}

			IsDisposed = true;
			IsOpen = Hidden = false;

			if (OnActionDispose != null)
			{
				VitaNexCore.TryCatch(OnActionDispose, this);
			}

			VitaNexCore.TryCatch(OnDispose);
			VitaNexCore.TryCatch(UnregisterInstance);

			Linked.ForEachReverse(g => VitaNexCore.TryCatch(Unlink, g));
			Children.ForEachReverse(g => VitaNexCore.TryCatch(RemoveChild, g));
			Entries.ForEachReverse(e => VitaNexCore.TryCatch(DisposeEntry, e));

			if (InstancePoller != null)
			{
				VitaNexCore.TryCatch(InstancePoller.Dispose);
			}

			VitaNexCore.TryCatch(OnDisposed);

			ObjectPool.Free(ref _Linked);
			ObjectPool.Free(ref _Children);

			ObjectPool.Free(ref _Controls);
			ObjectPool.Free(ref _Layout);

			ObjectPool.Free(ref _Buttons);
			ObjectPool.Free(ref _TileButtons);
			ObjectPool.Free(ref _Switches);
			ObjectPool.Free(ref _Radios);
			ObjectPool.Free(ref _TextInputs);
			ObjectPool.Free(ref _LimitedTextInputs);

			ObjectPool.Free(ref _TextTooltips);

			Entries.Free(true);

			NextButtonID = 1;
			NextSwitchID = 0;
			NextTextInputID = 0;

			OnActionSend = null;
			OnActionClose = null;
			OnActionHide = null;
			OnActionRefresh = null;
			OnActionDispose = null;
			OnActionClick = null;
			OnActionDoubleClick = null;

			LastButtonClicked = null;
			LastTileButtonClicked = null;

			ButtonHandler = null;
			TileButtonHandler = null;
			SwitchHandler = null;
			RadioHandler = null;
			TextInputHandler = null;
			LimitedTextInputHandler = null;

			Parent = null;
			User = null;

			InstancePoller = null;

			VitaNexCore.TryCatch(OnAfterDisposed);
		}

		protected virtual void OnDispose()
		{ }

		protected virtual void OnDisposed()
		{ }

		protected virtual void OnAfterDisposed()
		{ }

		public override string ToString()
		{
			return String.Format("0x{0:X} \"{1}\"", Serial, this.GetTypeName(false));
		}
	}
}
