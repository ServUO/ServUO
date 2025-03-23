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

using Server;
using Server.Network;
using Server.Targeting;
#endregion

namespace VitaNex.Targets
{
	public enum TargetResult
	{
		Success,
		Cancelled,
		NoAccess,
		CantSee,
		OutOfSight,
		OutOfRange,
		Invalid
	}

	public interface ISelectTarget
	{
		Mobile User { get; }
		TargetResult Result { get; }
	}

	public interface ISelectTarget<TObj> : ISelectTarget
	{
		Action<Mobile, TObj> SuccessHandler { get; set; }
		Action<Mobile> FailHandler { get; set; }
	}

	/// <summary>
	///     Provides methods for selecting specific Items of the given Type
	/// </summary>
	/// <typeparam name="TObj">Type of the TObj to be selected</typeparam>
	public class GenericSelectTarget<TObj> : Target, ISelectTarget<TObj>
	{
		/// <summary>
		///     Begin targeting for the specified Mobile with definded handlers
		/// </summary>
		/// <param name="m">Mobile owner of the new GenericSelectTarget instance</param>
		/// <param name="success">Success callback</param>
		/// <param name="fail">Failure callback</param>
		/// <param name="range">Maximum distance allowed</param>
		/// <param name="allowGround">Allow ground as valid target</param>
		/// <param name="flags">Target flags determine the target action</param>
		public static void Begin(
			Mobile m,
			Action<Mobile, TObj> success,
			Action<Mobile> fail,
			int range = -1,
			bool allowGround = false,
			TargetFlags flags = TargetFlags.None)
		{
			if (m != null)
			{
				m.Target = new GenericSelectTarget<TObj>(success, fail, range, allowGround, flags);
			}
		}

		public Mobile User { get; protected set; }
		public TargetResult Result { get; protected set; }

		/// <summary>
		///     Gets or sets the Multi display ID
		/// </summary>
		public int MultiID { get; set; }

		/// <summary>
		///     Gets or sets the Multi display Hue
		/// </summary>
		public int MultiHue { get; set; }

		/// <summary>
		///     Gets or sets the Multi display Offset
		/// </summary>
		public Point3D MultiOffset { get; set; }

		/// <summary>
		///     Gets or sets the current Success callback
		/// </summary>
		public Action<Mobile, TObj> SuccessHandler { get; set; }

		/// <summary>
		///     Gets or sets the current Failure callback
		/// </summary>
		public Action<Mobile> FailHandler { get; set; }

		/// <summary>
		///     Create an instance of GenericSelectTarget with handlers
		/// </summary>
		/// <param name="success">Success handler</param>
		/// <param name="fail">Failure handler</param>
		public GenericSelectTarget(Action<Mobile, TObj> success, Action<Mobile> fail)
			: base(-1, false, TargetFlags.None)
		{
			MultiID = -1;
			SuccessHandler = success;
			FailHandler = fail;
		}

		public GenericSelectTarget(
			Action<Mobile, TObj> success,
			Action<Mobile> fail,
			int range,
			bool allowGround,
			TargetFlags flags)
			: base(range, allowGround, flags)
		{
			MultiID = -1;
			SuccessHandler = success;
			FailHandler = fail;
		}

		public sealed class MultiTargetReq : Packet
		{
			public MultiTargetReq(GenericSelectTarget<TObj> t, bool hs)
				: base(0x99, hs ? 30 : 26)
			{
				m_Stream.Write(t.AllowGround);
				m_Stream.Write(t.TargetID);
				m_Stream.Write((byte)t.Flags);

				m_Stream.Fill(11);

				m_Stream.Write((short)t.MultiID);
				m_Stream.Write((short)t.MultiOffset.X);
				m_Stream.Write((short)t.MultiOffset.Y);
				m_Stream.Write((short)t.MultiOffset.Z);

				if (hs)
				{
					m_Stream.Write(t.MultiHue);
				}
			}
		}

		public override Packet GetPacketFor(NetState ns)
		{
			User = ns.Mobile;

			if (MultiID > -1)
			{
				return new MultiTargetReq(this, ns.HighSeas);
			}

			return base.GetPacketFor(ns);
		}

		protected virtual void OnTargetFail()
		{ }

		/// <summary>
		///     Called when this instance of GenericSelectTarget is cancelled
		/// </summary>
		/// <param name="from">Mobile owner of the current GenericSelectTarget instance</param>
		/// <param name="cancelType">CancelType</param>
		protected override sealed void OnTargetCancel(Mobile from, TargetCancelType cancelType)
		{
			base.OnTargetCancel(from, cancelType);

			Result = TargetResult.Cancelled;

			OnTargetFail();

			if (FailHandler != null)
			{
				FailHandler(from);
			}
		}

		/// <summary>
		///     Called when this instance of GenericSelectTarget is checked
		/// </summary>
		/// <param name="from">Mobile owner of the current GenericSelectTarget instance</param>
		/// <param name="targeted">The targeted TObj object</param>
		protected override sealed void OnTarget(Mobile from, object targeted)
		{
			base.OnTarget(from, targeted);

			if (targeted is TObj)
			{
				if (typeof(TObj).IsEqual<IPoint2D>() || typeof(TObj).IsEqual<IPoint3D>())
				{
					if (targeted is Item)
					{
						targeted = ((Item)targeted).GetWorldLocation();
					}
					else if (targeted is Mobile)
					{
						targeted = ((Mobile)targeted).Location;
					}
				}

				Result = TargetResult.Success;

				OnTarget(from, (TObj)targeted);
			}
			else
			{
				Result = TargetResult.Invalid;

				OnTargetFail();

				if (FailHandler != null)
				{
					FailHandler(from);
				}
			}
		}

		/// <summary>
		///     Called when this instance of GenericSelectTarget is successful
		/// </summary>
		/// <param name="from">Mobile owner of the current GenericSelectTarget instance</param>
		/// <param name="targeted">The targeted TObj object</param>
		protected virtual void OnTarget(Mobile from, TObj targeted)
		{
			if (SuccessHandler != null)
			{
				SuccessHandler(from, targeted);
			}
		}

		protected override void OnTargetNotAccessible(Mobile from, object targeted)
		{
			base.OnTargetNotAccessible(from, targeted);

			Result = TargetResult.NoAccess;
		}

		protected override void OnTargetInSecureTrade(Mobile from, object targeted)
		{
			base.OnTargetInSecureTrade(from, targeted);

			Result = TargetResult.NoAccess;
		}

		protected override void OnTargetUntargetable(Mobile from, object targeted)
		{
			base.OnTargetUntargetable(from, targeted);

			Result = TargetResult.Invalid;
		}

		protected override void OnTargetOutOfLOS(Mobile from, object targeted)
		{
			base.OnTargetOutOfLOS(from, targeted);

			Result = TargetResult.OutOfSight;
		}

		protected override void OnTargetOutOfRange(Mobile from, object targeted)
		{
			base.OnTargetOutOfRange(from, targeted);

			Result = TargetResult.OutOfRange;
		}

		protected override void OnCantSeeTarget(Mobile from, object targeted)
		{
			base.OnCantSeeTarget(from, targeted);

			Result = TargetResult.CantSee;
		}
	}
}
