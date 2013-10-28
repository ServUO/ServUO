using Server;
using System;
using System.Collections.Generic;
using Services.Toolbar.Core;
using CustomsFramework;

namespace Services.Toolbar
{
    public partial class ToolbarModule : BaseModule
    {
        private ToolbarInfo _ToolbarInfo;

        public ToolbarModule(Mobile from) : base(from)
        {
            _ToolbarInfo = ToolbarInfo.CreateNew(from);
        }

        public ToolbarModule(CustomSerial serial) : base(serial)
        {
        }

        public override string Name
        {
            get
            {
                if (this.LinkedMobile != null)
                    return String.Format(@"Toolbar Module - {0}", this.LinkedMobile.Name);
                else
                    return @"Unlinked Toolbar Module";
            }
        }

        public override string Version
        {
            get
            {
                return ToolbarCore.SystemVersion;
            }
        }

        public override AccessLevel EditLevel
        {
            get
            {
                return AccessLevel.Developer;
            }
        }

        [CommandProperty(AccessLevel.Developer)]
        public ToolbarInfo ToolbarInfo
        {
            get
            {
                return _ToolbarInfo;
            }
            set
            {
                _ToolbarInfo = value;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            Utilities.WriteVersion(writer, 0);

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
