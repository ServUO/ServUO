#region References
using System;

using Server.Diagnostics;
using Server.Items;
using Server.Targeting;
#endregion

namespace Server.Commands
{
	public static class Dupe
	{
		public static void Configure()
		{
			Mobile.LiftItemDupeHandler = LiftItemDupe;

			CommandSystem.Register("Dupe", AccessLevel.GameMaster, Dupe_OnCommand);
			CommandSystem.Register("DupeInBag", AccessLevel.GameMaster, DupeInBag_OnCommand);
		}

		private static Item LiftItemDupe(Item oldItem, int amount)
		{
			if (oldItem?.Deleted != false)
			{
				return null;
			}

			var newItem = DupeItem(oldItem);

			if (newItem?.Deleted == false)
			{
				newItem.Amount = oldItem.Amount - amount;
				oldItem.Amount = amount;

				return newItem;
			}

			return null;
		}

		[Usage("Dupe [amount]")]
		[Description("Dupes a targeted item.")]
		private static void Dupe_OnCommand(CommandEventArgs e)
		{
			var amount = 1;

			if (e.Length > 0)
			{
				amount = e.GetInt32(0);
			}

			e.Mobile.Target = new DupeTarget(false, amount);
			e.Mobile.SendMessage("What do you wish to dupe?");
		}

		[Usage("DupeInBag <count>")]
		[Description("Dupes an item at it's current location (count) number of times.")]
		private static void DupeInBag_OnCommand(CommandEventArgs e)
		{
			var amount = 1;

			if (e.Length > 0)
			{
				amount = e.GetInt32(0);
			}

			e.Mobile.Target = new DupeTarget(true, amount);
			e.Mobile.SendMessage("What do you wish to dupe?");
		}

		private class DupeTarget : Target
		{
			private readonly bool _InBag;
			private readonly int _Amount;

			public DupeTarget(bool inbag, int amount)
				: base(15, false, TargetFlags.None)
			{
				_InBag = inbag;
				_Amount = Math.Max(1, amount);
			}

			protected override void OnTarget(Mobile m, object targ)
			{
				if (!(targ is Item item))
				{
					m.SendMessage("You can only dupe items.");
					return;
				}

				CommandLogging.WriteLine(m, "{0} {1} duping {2} (inBag={3}; amount={4})", m.AccessLevel, CommandLogging.Format(m), CommandLogging.Format(targ), _InBag, _Amount);

				Container pack;

				if (_InBag)
				{
					if (item.Parent is Container cp)
					{
						pack = cp;
					}
					else if (item.Parent is Mobile mp)
					{
						pack = mp.Backpack;
					}
					else
					{
						pack = null;
					}
				}
				else
				{
					pack = m.Backpack;
				}

				try
				{
					m.SendMessage("Duping {0}...", _Amount);

					for (var i = 0; i < _Amount; i++)
					{
						var o = Construct(m, item);

						if (o == null)
						{
							m.SendMessage("Unable to dupe.");
							return;
						}

						if (pack != null)
						{
							pack.DropItem(o);
						}
						else
						{
							o.MoveToWorld(m.Location, m.Map);
						}

						o.UpdateTotals();
						o.InvalidateProperties();
						o.Delta(ItemDelta.Update);

						item.Delta(ItemDelta.Update);

						CommandLogging.WriteLine(m, "{0} {1} duped {2} creating {3}", m.AccessLevel, CommandLogging.Format(m), CommandLogging.Format(item), CommandLogging.Format(o));
					}

					m.SendMessage("Done");
				}
				catch (Exception e)
				{
					ExceptionLogging.LogException(e);

					m.SendMessage("Error");
				}
			}
		}

		public static Item DupeItem(Item item)
		{
			return DupeItem(null, item);
		}

		public static Item DupeItem(Mobile m, Item item)
		{
			try
			{
				var o = Construct(m, item);

				if (o == null)
				{
					return null;
				}

				if (m != null)
				{
					o.MoveToWorld(m.Location, m.Map);
				}

				o.UpdateTotals();
				o.InvalidateProperties();
				o.Delta(ItemDelta.Update);

				item.Delta(ItemDelta.Update);

				if (m != null)
				{
					CommandLogging.WriteLine(m, "{0} {1} duped {2} creating {3}", m.AccessLevel, CommandLogging.Format(m), CommandLogging.Format(item), CommandLogging.Format(o));
				}

				return o;
			}
			catch (Exception e)
			{
				ExceptionLogging.LogException(e);

				return null;
			}
		}

		public static void DupeChildren(Container src, Container dest)
		{
			DupeChildren(null, src, dest);
		}

		public static void DupeChildren(Mobile m, Container src, Container dest)
		{
			foreach (var item in src.Items)
			{
				try
				{
					var o = Construct(m, item);

					if (o == null)
					{
						continue;
					}

					dest.DropItem(o);

					o.Location = item.Location;

					o.UpdateTotals();
					o.InvalidateProperties();
					o.Delta(ItemDelta.Update);

					item.Delta(ItemDelta.Update);

					if (m != null)
					{
						CommandLogging.WriteLine(m, "{0} {1} duped {2} creating {3}", m.AccessLevel, CommandLogging.Format(m), CommandLogging.Format(item), CommandLogging.Format(o));
					}
				}
				catch (Exception e)
				{
					ExceptionLogging.LogException(e);
				}
			}
		}

		private static Item Construct(Mobile m, Item item)
		{
			var t = item.GetType();

			if (m != null)
			{
				var ctor = t.GetConstructor(Type.EmptyTypes);

				if (ctor != null)
				{
					var a = ctor.GetCustomAttributes(typeof(ConstructableAttribute), false);

					for (var i = 0; i < a.Length; i++)
					{
						if (((ConstructableAttribute)a[i]).AccessLevel > m.AccessLevel)
						{
							return null;
						}
					}
				}
			}

			Item o;

			try
			{
				o = (Item)Activator.CreateInstance(t, true);
			}
			catch
			{
				o = null;
			}

			if (o == null) // default to serial ctor
			{
				try
				{
					o = (Item)Activator.CreateInstance(t, Serial.NewItem);
				}
				catch
				{
					o = null;
				}

				if (o != null) // invoke functions found in the skipped base item ctor
				{
					World.AddItem(o);

					Timer.DelayCall(obj =>
					{
						if (!obj.Deleted)
						{
							EventSink.InvokeItemCreated(new ItemCreatedEventArgs(obj));
						}
					}, o);
				}
			}

			if (o?.Deleted == false)
			{
				CopyProperties(item, o);

				o.Internalize();

				item.OnAfterDuped(o);

				if (item is Container ic && o is Container oc)
				{
					DupeChildren(m, ic, oc);
				}

				return o;
			}

			return null;
		}

		public static void CopyProperties(object src, object dest)
		{
			var props = src.GetType().GetProperties();

			foreach (var p in props)
			{
				try
				{
					if (p.CanRead && p.CanWrite)
					{
						if (p.GetCustomAttributes(typeof(NoDupeAttribute), true).Length > 0)
						{
							continue;
						}

						p.SetValue(dest, p.GetValue(src, null), null);
					}
				}
				catch (Exception e)
				{
					ExceptionLogging.LogException(e);
				}
			}

			OnCopyProperties?.Invoke(src, dest);
		}

		public static event Action<object, object> OnCopyProperties;
	}
}
