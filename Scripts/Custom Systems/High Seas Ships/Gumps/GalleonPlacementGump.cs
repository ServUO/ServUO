using System;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Network;

namespace Server.Multis
{
    public class GalleonPlacementGump : Gump
    {
        private Mobile _from;
        private BaseGalleonDeed _deed;
		private BaseDockedGalleon _dockedGalleon;

        public GalleonPlacementGump(Mobile from, BaseGalleonDeed deed, BaseDockedGalleon dockedGalleon)
            : base(150, 200)
        {
            _from = from;
			_deed = deed;
			_dockedGalleon = dockedGalleon;

            AddPage(0);

			AddBackground(0, 0, 270, 200, 5054);
			
            AddImageTiled(14, 14, 4, 4, 0x2486);
            AddImageTiled(18, 14, 240, 4, 0x2487);
			AddImageTiled(256, 14, 4, 4, 0x2488);
			AddImageTiled(14, 18, 4, 170, 0x2489);
			AddImageTiled(18, 18, 240, 170, 0x248A);
			AddImageTiled(256, 18, 4, 170, 0x248B);
			AddImageTiled(14, 184, 4, 4, 0x248C);
			AddImageTiled(18, 184, 240, 4, 0x248D);
			AddImageTiled(256, 184, 4, 4, 0x248E);

            AddLabel(40, 40, 0x480, "Select the ship direction");
			AddLabel(40, 60, 0x480, "for placement.");

            AddButton(20, 120, 0xFA5, 0xFA6, 1, GumpButtonType.Reply, 0);
			AddLabel(60, 120, 0x480, "WEST");
            AddButton(120, 120, 0xFA5, 0xFA6, 2, GumpButtonType.Reply, 0);
			AddLabel(160, 120, 0x480, "NORTH");
			AddButton(20, 160, 0xFA5, 0xFA6, 3, GumpButtonType.Reply, 0);
			AddLabel(60, 160, 0x480, "SOUTH");
			AddButton(120, 160, 0xFA5, 0xFA6, 4, GumpButtonType.Reply, 0);
			AddLabel(160, 160, 0x480, "EAST");
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (info.ButtonID == 1)
			{
				if (_deed != null)
				{
					_deed.PlacementDirection(_from, Direction.West);
				}
				else if (_dockedGalleon != null)
				{
					_dockedGalleon.PlacementDirection(_from, Direction.West);
				}
			}
			else if (info.ButtonID == 2)
			{
				if (_deed != null)
				{
					_deed.PlacementDirection(_from, Direction.North);
				}
				else if (_dockedGalleon != null)
				{
					_dockedGalleon.PlacementDirection(_from, Direction.North);
				}
			}
            else if (info.ButtonID == 3)
			{
				if (_deed != null)
				{
					_deed.PlacementDirection(_from, Direction.South);
				}
				else if (_dockedGalleon != null)
				{
					_dockedGalleon.PlacementDirection(_from, Direction.South);
				}
			}
            else if (info.ButtonID == 4)
			{
				if (_deed != null)
				{
					_deed.PlacementDirection(_from, Direction.East);
				}
				else if (_dockedGalleon != null)
				{
					_dockedGalleon.PlacementDirection(_from, Direction.East);
				}
			}
        }
    }
}