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
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public class PvPBattleSuddenDeath : PropertyObject
	{
		private bool _Active;
		private int _CapacityRequired = 2;
		private int _DamageRange;
		private DateTime _EndedWhen;
		private int _MaxDamage = 40;
		private int _MinDamage = 20;

		private DateTime _StartedWhen;

		[CommandProperty(AutoPvP.Access)]
		public virtual bool Enabled { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual bool Active => _Active;

		[CommandProperty(AutoPvP.Access)]
		public virtual DateTime StartedWhen => _StartedWhen;

		[CommandProperty(AutoPvP.Access)]
		public virtual DateTime EndedWhen => _EndedWhen;

		[CommandProperty(AutoPvP.Access)]
		public virtual int CapacityRequired
		{
			get => _CapacityRequired;
			set => _CapacityRequired = Math.Max(2, value);
		}

		[CommandProperty(AutoPvP.Access)]
		public virtual TimeSpan Delay { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual bool Damages { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual int DamageRange { get => _DamageRange; set => _DamageRange = Math.Max(0, value); }

		[CommandProperty(AutoPvP.Access)]
		public virtual int MinDamage
		{
			get => _MinDamage;
			set => _MinDamage = Math.Max(0, Math.Min(_MaxDamage, value));
		}

		[CommandProperty(AutoPvP.Access)]
		public virtual int MaxDamage { get => _MaxDamage; set => _MaxDamage = Math.Max(_MinDamage, value); }

		public PvPBattleSuddenDeath()
		{
			_StartedWhen = DateTime.UtcNow;
			_EndedWhen = DateTime.UtcNow;
			CapacityRequired = 2;
			Delay = TimeSpan.FromSeconds(30.0);
			Damages = true;
			DamageRange = 1;
			MinDamage = 20;
			MaxDamage = 40;
		}

		public PvPBattleSuddenDeath(GenericReader reader)
			: base(reader)
		{ }

		public override string ToString()
		{
			return "Sudden Death";
		}

		public override void Clear()
		{
			Enabled = false;

			_Active = false;
			_StartedWhen = DateTime.UtcNow;
			_EndedWhen = DateTime.UtcNow;

			CapacityRequired = 0;

			Delay = TimeSpan.Zero;

			Damages = false;
			DamageRange = 0;
			MinDamage = 0;
			MaxDamage = 0;
		}

		public override void Reset()
		{
			Enabled = false;

			_Active = false;
			_StartedWhen = DateTime.UtcNow;
			_EndedWhen = DateTime.UtcNow;

			CapacityRequired = 2;

			Delay = TimeSpan.FromSeconds(30.0);

			Damages = true;
			DamageRange = 1;
			MinDamage = 20;
			MaxDamage = 40;
		}

		public virtual void Damage(Mobile target)
		{
			if (target == null || target.Deleted)
			{
				return;
			}

			if (Damages)
			{
				target.Damage(Utility.RandomMinMax(_MinDamage, _MaxDamage));
			}
		}

		public virtual void Damage(Mobile from, Mobile target)
		{
			if (target == null || target.Deleted)
			{
				return;
			}

			if (Damages)
			{
				target.Damage(Utility.RandomMinMax(_MinDamage, _MaxDamage), from);
			}
		}

		public virtual void Start()
		{
			if (!Enabled)
			{
				return;
			}

			_StartedWhen = DateTime.UtcNow;
			_Active = true;
		}

		public virtual void End()
		{
			_EndedWhen = DateTime.UtcNow;
			_Active = false;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
				{
					writer.Write(Enabled);
					writer.Write(_Active);
					writer.Write(_StartedWhen);
					writer.Write(_EndedWhen);
					writer.Write(CapacityRequired);
					writer.Write(Delay);
					writer.Write(Damages);
					writer.Write(MinDamage);
					writer.Write(MaxDamage);
					writer.Write(DamageRange);
				}
				break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.GetVersion();

			switch (version)
			{
				case 0:
				{
					Enabled = reader.ReadBool();
					_Active = reader.ReadBool();
					_StartedWhen = reader.ReadDateTime();
					_EndedWhen = reader.ReadDateTime();
					CapacityRequired = reader.ReadInt();
					Delay = reader.ReadTimeSpan();
					Damages = reader.ReadBool();
					MinDamage = reader.ReadInt();
					MaxDamage = reader.ReadInt();
					DamageRange = reader.ReadInt();
				}
				break;
			}
		}
	}
}