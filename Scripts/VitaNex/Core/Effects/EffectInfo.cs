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

using VitaNex.Network;
#endregion

namespace VitaNex.FX
{
	public class EffectInfo : IDisposable
	{
		protected Timer DelayTimer { get; set; }

		public virtual event Action Callback;

		public bool IsDisposed { get; private set; }
		public bool Sending { get; protected set; }

		/// <summary>
		///     Represents the queue position of this effect.
		///     The value is usually incremented based on the number of effect queues to be processed.
		///     Currently set by BaseEffect during queue processing.
		/// </summary>
		public int QueueIndex { get; set; }

		/// <summary>
		///     Represents the process index of this effect.
		///     The value is usually incremented based on the number of effects on a single point.
		///     Currently set by BaseEffect during queue processing.
		/// </summary>
		public int ProcessIndex { get; set; }

		public virtual IEntity Source { get; set; }
		public virtual Map Map { get; set; }
		public virtual int EffectID { get; set; }
		public virtual int Hue { get; set; }
		public virtual int Speed { get; set; }
		public virtual int Duration { get; set; }
		public virtual EffectRender Render { get; set; }
		public virtual TimeSpan Delay { get; set; }

		public virtual int SoundID { get; set; }

		public EffectInfo(
			IPoint3D source,
			Map map,
			int effectID,
			int hue = 0,
			int speed = 10,
			int duration = 10,
			EffectRender render = EffectRender.Normal,
			TimeSpan? delay = null,
			Action callback = null)
		{
			Map = map;
			EffectID = effectID;
			Hue = hue;
			Speed = speed;
			Duration = duration;
			Render = render;
			Delay = delay ?? TimeSpan.Zero;

			if (callback != null)
			{
				Callback += callback;
			}

			SetSource(source);
		}

		~EffectInfo()
		{
			Dispose();
		}

		public virtual void SetSource(IPoint3D source)
		{
			if (IsDisposed || Source == source)
			{
				return;
			}

			if (source == null)
			{
				Source = null;
				Map = null;
				return;
			}

			if (source is IEntity)
			{
				Source = (IEntity)source;
				Map = Source.Map;
				return;
			}

			Source = new Entity(Serial.Zero, source.Clone3D(), Source != null ? Source.Map : Map);
		}

		public virtual void Send()
		{
			if (IsDisposed || Source == null || Sending)
			{
				return;
			}

			Sending = true;

			if (Delay > TimeSpan.Zero)
			{
				DelayTimer = Timer.DelayCall(
					Delay,
					() =>
					{
						if (DelayTimer != null)
						{
							DelayTimer.Stop();
							DelayTimer = null;
						}

						if (OnSend())
						{
							OnAfterSend();
						}
					});
			}
			else
			{
				if (OnSend())
				{
					OnAfterSend();
				}
			}
		}

		protected virtual bool OnSend()
		{
			if (IsDisposed || Source == null || Map == null)
			{
				return false;
			}

			if (SoundID > 0)
			{
				Effects.PlaySound(Source, Map, SoundID);
			}

			if (Source.Map == Map && (Source is Mobile || Source is Item))
			{
				Effects.SendTargetEffect(Source, EffectID, Speed, Duration, Hue, (int)Render);
			}
			else
			{
				Effects.SendLocationEffect(Source, Map, EffectID, Duration, Speed, Hue, (int)Render);
			}

			return true;
		}

		protected virtual void OnAfterSend()
		{
			if (IsDisposed)
			{
				return;
			}

			if (Callback != null)
			{
				Callback.Invoke();
			}

			Sending = false;
		}

		public virtual void Dispose()
		{
			if (IsDisposed)
			{
				return;
			}

			IsDisposed = true;

			if (DelayTimer != null)
			{
				DelayTimer.Stop();
				DelayTimer = null;
			}

			Source = null;
			Map = null;
			EffectID = -1;
			Hue = 0;
			Speed = 0;
			Duration = 0;
			Render = EffectRender.Normal;
			Delay = TimeSpan.Zero;
			Callback = null;
			Sending = false;
		}
	}

	public class MovingEffectInfo : EffectInfo
	{
		protected Timer ImpactTimer { get; set; }

		public virtual event Action ImpactCallback;

		public virtual IEntity Target { get; set; }

		public override int Duration
		{
			get => (int)(GetTravelTime().TotalMilliseconds / 100.0);
			set => base.Duration = value;
		}

		public MovingEffectInfo(
			IPoint3D source,
			IPoint3D target,
			Map map,
			int effectID,
			int hue = 0,
			int speed = 10,
			EffectRender render = EffectRender.Normal,
			TimeSpan? delay = null,
			Action callback = null)
			: base(source, map, effectID, hue, speed, 0, render, delay, callback)
		{
			SetTarget(target);
		}

		public virtual void SetTarget(IPoint3D target)
		{
			if (IsDisposed || Target == target)
			{
				return;
			}

			if (target == null)
			{
				Target = null;
				return;
			}

			if (target is IEntity)
			{
				Target = (IEntity)target;
				return;
			}

			if (Target is Mobile)
			{
				((Mobile)Target).Location = target.Clone3D();
				return;
			}

			if (Target is Item)
			{
				((Item)Target).Location = target.Clone3D();
				return;
			}

			Target = new Entity(Serial.Zero, target.Clone3D(), Map);
		}

		public override void Send()
		{
			if (!IsDisposed && Source != null && Target != null && Map != null)
			{
				base.Send();
			}
		}

		protected override bool OnSend()
		{
			if (IsDisposed || Source == null || Target == null || Map == null)
			{
				return false;
			}

			if (SoundID > 0)
			{
				Effects.PlaySound(Source, Map, SoundID);
			}

			Effects.SendMovingEffect(Source, Target, EffectID, Speed, Duration, false, false, Hue, (int)Render);
			return true;
		}

		protected override void OnAfterSend()
		{
			if (IsDisposed)
			{
				return;
			}

			var delay = GetTravelTime();

			if (delay > TimeSpan.Zero)
			{
				ImpactTimer = Timer.DelayCall(
					delay,
					() =>
					{
						if (ImpactTimer != null)
						{
							ImpactTimer.Stop();
							ImpactTimer = null;
						}

						if (ImpactCallback != null)
						{
							ImpactCallback.Invoke();
							ImpactCallback = null;
						}

						base.OnAfterSend();
					});
			}
			else
			{
				if (ImpactCallback != null)
				{
					ImpactCallback.Invoke();
					ImpactCallback = null;
				}

				base.OnAfterSend();
			}
		}

		public void MovingImpact(Action callback)
		{
			if (IsDisposed || Sending || Source == null || Target == null || Map == null)
			{
				return;
			}

			if (callback != null)
			{
				ImpactCallback += callback;
			}

			Send();
		}

		public void MovingImpact(Action<MovingEffectInfo> callback)
		{
			if (IsDisposed || Sending || Source == null || Target == null || Map == null)
			{
				return;
			}

			if (callback != null)
			{
				ImpactCallback += () => callback(this);
			}

			Send();
		}

		public virtual TimeSpan GetTravelTime()
		{
			return Source.GetTravelTime(Target, Speed);
		}

		public override void Dispose()
		{
			if (IsDisposed)
			{
				return;
			}

			if (ImpactTimer != null)
			{
				ImpactTimer.Stop();
				ImpactTimer = null;
			}

			base.Dispose();

			Target = null;
		}
	}
}