#region Header
// **********
// ServUO - ToolbarModule.cs
// **********
#endregion

#region References
using System;

using CustomsFramework;

using Server;

using Services.Toolbar.Core;
#endregion

namespace Services.Toolbar
{
	public class ToolbarModule : BaseModule
	{
		private ToolbarInfo _ToolbarInfo;

		public ToolbarModule(Mobile from)
			: base(from)
		{
			_ToolbarInfo = ToolbarInfo.CreateNew(from);
		}

		public ToolbarModule(CustomSerial serial)
			: base(serial)
		{ }

		public override string Name
		{
			get { return LinkedMobile != null ? String.Format(@"Toolbar Module - {0}", LinkedMobile.Name) : @"Unlinked Toolbar Module"; }
		}

		public override string Version { get { return ToolbarCore.SystemVersion; } }

		public override AccessLevel EditLevel { get { return AccessLevel.Developer; } }

		[CommandProperty(AccessLevel.Developer)]
		public ToolbarInfo ToolbarInfo { get { return _ToolbarInfo; } set { _ToolbarInfo = value; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteVersion(0);

			// Version 0
			_ToolbarInfo.Serialize(writer);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 0:
					{
						_ToolbarInfo = new ToolbarInfo(reader);
						break;
					}
			}
		}
	}
}