using System;
using Server;
using Server.Multis;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Engines.UOArchitect
{
	public class UOAR_ObjectTarget : Target
	{
		public delegate void TargetCancelEvent();
		public delegate void TargetObjectEvent(object targeted);

		public TargetObjectEvent OnTargetObject;
		public TargetCancelEvent OnCancelled;

		public UOAR_ObjectTarget() : base( -1, true, TargetFlags.None )
		{
		}

		protected override void OnTarget( Mobile from, object targeted )
		{
			if(OnTargetObject != null)
				OnTargetObject(targeted);
		}

		protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
		{
			if(OnCancelled != null)
				OnCancelled();

			base.OnTargetCancel (from, cancelType);
		}

	}
}
