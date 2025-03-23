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

namespace VitaNex.Modules.AutoPvP
{
	[PropertyObject]
	public class PvPBattleSkillRestrictions : PvPBattleRestrictionsBase<int>
	{
		private static readonly string[] _SkillNames = Enum.GetNames(typeof(SkillName));

		private static int FindSkill(string name, bool ignoreCase = true)
		{
			return _SkillNames.IndexOf(
				_SkillNames.FirstOrDefault(s => ignoreCase ? Insensitive.Equals(s, name) : String.Equals(s, name)));
		}

		public PvPBattleSkillRestrictions()
		{ }

		public PvPBattleSkillRestrictions(GenericReader reader)
			: base(reader)
		{ }

		public override string ToString()
		{
			return "Skill Restrictions";
		}

		public override void Invalidate()
		{
			foreach (var info in SkillInfo.Table)
			{
				SetRestricted(info, false);
			}
		}

		public virtual void SetRestricted(Skill skill, bool restrict)
		{
			if (skill != null)
			{
				SetRestricted(skill.Info, restrict);
			}
		}

		public virtual void SetRestricted(SkillInfo skill, bool restrict)
		{
			if (skill != null)
			{
				SetRestricted(skill.SkillID, restrict);
			}
		}

		public virtual void SetRestricted(string skill, bool restrict)
		{
			if (!String.IsNullOrWhiteSpace(skill))
			{
				SetRestricted(FindSkill(skill), restrict);
			}
		}

		public virtual void SetRestricted(SkillName skill, bool restrict)
		{
			SetRestricted((int)skill, restrict);
		}

		public override void SetRestricted(int skill, bool restrict)
		{
			if (skill >= 0)
			{
				base.SetRestricted(skill, restrict);
			}
		}

		public virtual bool IsRestricted(Skill skill)
		{
			return skill != null && IsRestricted(skill.Info);
		}

		public virtual bool IsRestricted(SkillInfo skill)
		{
			return skill != null && IsRestricted(skill.SkillID);
		}

		public virtual bool IsRestricted(string skill)
		{
			return !String.IsNullOrEmpty(skill) && IsRestricted(FindSkill(skill));
		}

		public virtual bool IsRestricted(SkillName skill)
		{
			return IsRestricted((int)skill);
		}

		public override bool IsRestricted(int key)
		{
			return key >= 0 && base.IsRestricted(key);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();
		}

		public override void SerializeEntry(GenericWriter writer, int key, bool val)
		{
			writer.Write(key);
			writer.Write(val);
		}

		public override KeyValuePair<int, bool> DeserializeEntry(GenericReader reader)
		{
			var k = reader.ReadInt();
			var v = reader.ReadBool();
			return new KeyValuePair<int, bool>(k, v);
		}
	}
}