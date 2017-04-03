﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Web.UI;
using CustomsFramework;
using Server.Guilds;

namespace Server
{
	public abstract class GenericWriter
	{
		public abstract void Close();

		public abstract long Position { get; }

		public abstract void Write(string value);
		public abstract void Write(DateTime value);
		public abstract void Write(DateTimeOffset value);
		public abstract void Write(TimeSpan value);
		public abstract void Write(decimal value);
		public abstract void Write(long value);
		public abstract void Write(ulong value);
		public abstract void Write(int value);
		public abstract void Write(uint value);
		public abstract void Write(short value);
		public abstract void Write(ushort value);
		public abstract void Write(double value);
		public abstract void Write(float value);
		public abstract void Write(char value);
		public abstract void Write(byte value);
		public abstract void Write(sbyte value);
		public abstract void Write(bool value);
		public abstract void WriteEncodedInt(int value);
		public abstract void Write(IPAddress value);

		public virtual void WriteDeltaTime(DateTime value)
		{
			var ticks = value.Ticks;
			var now = DateTime.UtcNow.Ticks;

			TimeSpan d;

			try
			{
				d = new TimeSpan(ticks - now);
			}
			catch
			{
				d = TimeSpan.MaxValue;
			}

			Write(d);
		}

		private void WriteEntity(IEntity entity)
		{
			if (entity == null || entity.Deleted)
			{
				Write(Serial.MinusOne);
			}
			else
			{
				Write(entity.Serial);
			}
		}

		private void WriteEntity(ICustomsEntity entity)
		{
			if (entity == null || entity.Deleted)
			{
				Write(Serial.MinusOne);
			}
			else
			{
				Write(entity.Serial);
			}
		}

		public virtual void Write(Point3D value)
		{
			Write(value.m_X);
			Write(value.m_Y);
			Write(value.m_Z);
		}

		public virtual void Write(Point2D value)
		{
			Write(value.m_X);
			Write(value.m_Y);
		}

		public virtual void Write(Rectangle2D value)
		{
			Write(value.Start);
			Write(value.End);
		}

		public virtual void Write(Rectangle3D value)
		{
			Write(value.Start);
			Write(value.End);
		}

		public virtual void Write(Map value)
		{
			if (value != null)
			{
				Write((byte)value.MapIndex);
			}
			else
			{
				Write((byte)0xFF);
			}
		}

		public virtual void Write(Item value) => WriteEntity(value);
		public virtual void Write(Mobile value) => WriteEntity(value);
		public virtual void Write(BaseGuild value) => Write(value?.Id ?? 0);
		public virtual void Write(SaveData value) => WriteEntity(value);
		public virtual void WriteItem<T>(T value) where T : Item => Write(value);
		public virtual void WriteMobile<T>(T value) where T : Mobile => Write(value);
		public virtual void WriteGuild<T>(T value) where T : BaseGuild => Write(value);
		public virtual void WriteData<T>(T value) where T : SaveData => Write(value);

		public virtual void Write(Race value)
		{
			if (value != null)
			{
				Write((byte)value.RaceIndex);
			}
			else
			{
				Write((byte)0xFF);
			}
		}

        protected virtual void WriteListTidy(IList list, Action writerAction, bool tidy = false, Func<int, bool> tidyPredicate = null)
		{
			if (tidy && tidyPredicate == null)
			{
				throw new ArgumentNullException(nameof(tidyPredicate), "tidyPredicate must not be null if tidy is true.");
			}

			if (tidy)
			{
				for (var i = 0; i < list.Count;)
				{
					if (tidyPredicate(i))
					{
						list.RemoveAt(i);
					}
					else
					{
						++i;
					}
				}
			}

			Write(list.Count);
			writerAction();
		}

		protected virtual void WriteSetTidy<T>(HashSet<T> set, Action writerAction, bool tidy = false, Predicate<T> tidyPredicate = null)
		{
			if (tidy && tidyPredicate == null)
			{
				throw new ArgumentNullException(nameof(tidyPredicate), "tidyPredicate must not be null if tidy is true.");
			}

			if (tidy)
			{
				set.RemoveWhere(tidyPredicate);
			}

			Write(set.Count);
			writerAction();
		}

		public virtual void WriteItemList(ArrayList list, bool tidy = false) => 
			WriteListTidy(list, () => list.ForEach<Item>(Write), tidy, i => ((Item) list[i]).Deleted);

		public virtual void WriteMobileList(ArrayList list, bool tidy = false) => 
			WriteListTidy(list, () => list.ForEach<Mobile>(Write), tidy, i => ((Mobile)list[i]).Deleted);

		public virtual void WriteGuildList(ArrayList list, bool tidy = false) =>
			WriteListTidy(list, () => list.ForEach<BaseGuild>(Write), tidy, i => ((BaseGuild) list[i]).Disbanded);

		public virtual void WriteDataList(ArrayList list, bool tidy = false) =>
			WriteListTidy(list, () => list.ForEach<SaveData>(Write), tidy, i => ((SaveData)list[i]).Deleted);

		public virtual void Write(List<Item> list, bool tidy = false) =>
			WriteListTidy(list, () => list.ForEach(Write), tidy, i => list[i].Deleted);

		public virtual void WriteItemList<T>(List<T> list, bool tidy = false) where T : Item =>
			WriteListTidy(list, () => list.ForEach(Write), tidy, i => list[i].Deleted);

		public virtual void Write(HashSet<Item> set, bool tidy = false) =>
			WriteSetTidy(set, () => set.ForEach(Write), tidy, item => item.Deleted);

		public virtual void WriteItemSet<T>(HashSet<T> set, bool tidy) where T : Item =>
			WriteSetTidy(set, () => set.ForEach(Write), tidy, item => item.Deleted);

		public virtual void Write(List<Mobile> list, bool tidy = false) =>
			WriteListTidy(list, () => list.ForEach(Write), tidy, i => list[i].Deleted);

		public virtual void WriteMobileList<T>(List<T> list, bool tidy = false) where T : Mobile =>
			WriteListTidy(list, () => list.ForEach(Write), tidy, i => list[i].Deleted);

		public virtual void Write(HashSet<Mobile> set, bool tidy = false) =>
			WriteSetTidy(set, () => set.ForEach(Write), tidy, item => item.Deleted);

		public virtual void WriteMobileSet<T>(HashSet<T> set, bool tidy = false) where T : Mobile =>
			WriteSetTidy(set, () => set.ForEach(Write), tidy, item => item.Deleted);

		public virtual void Write(List<BaseGuild> list, bool tidy = false) =>
			WriteListTidy(list, () => list.ForEach(Write), tidy, i => list[i].Disbanded);

		public virtual void WriteGuildList<T>(List<T> list, bool tidy = false) where T : BaseGuild =>
			WriteListTidy(list, () => list.ForEach(Write), tidy, i => list[i].Disbanded);

		public virtual void Write(HashSet<BaseGuild> set, bool tidy = false) =>
			WriteSetTidy(set, () => set.ForEach(Write), tidy, item => item.Disbanded);

		public virtual void WriteGuildSet<T>(HashSet<T> set, bool tidy = false) where T : BaseGuild =>
			WriteSetTidy(set, () => set.ForEach(Write), tidy, item => item.Disbanded);

		public virtual void Write(List<SaveData> list, bool tidy = false) =>
			WriteListTidy(list, () => list.ForEach(Write), tidy, i => list[i].Deleted);

		public virtual void WriteDataList<T>(List<T> list, bool tidy = false) where T : SaveData =>
			WriteListTidy(list, () => list.ForEach(Write), tidy, i => list[i].Deleted);

		public virtual void Write(HashSet<SaveData> set, bool tidy = false) =>
			WriteSetTidy(set, () => set.ForEach(Write), tidy, item => item.Deleted);

		public virtual void WriteDataSet<T>(HashSet<T> set, bool tidy = false) where T : SaveData =>
			WriteSetTidy(set, () => set.ForEach(Write), tidy, item => item.Deleted);
	}
}