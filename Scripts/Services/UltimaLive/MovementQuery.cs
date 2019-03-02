/* Copyright (C) 2013 Ian Karlinsey
 * 
 * UltimeLive is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * UltimaLive is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with UltimaLive.  If not, see <http://www.gnu.org/licenses/>. 
 */

using Server;
using Server.Mobiles;
using Server.Network;

namespace UltimaLive
{
    public sealed class MovementQuery
    {
        public static void Initialize()
        {
            PlayerMobile.BlockQuery = new MovementQuery();
        }

        public MovementQuery()
        {
        }

        public int QueryMobile(Mobile m, int previousMapBlock)
        {
            if (m.Map == null)
            {
                return 0;
            }

            int blocknum = (((m.Location.X >> 3) * m.Map.Tiles.BlockHeight) + (m.Location.Y >> 3));

            if (blocknum != previousMapBlock)
            {
                m.Send(new QueryClientHash(m));
            }

            return blocknum;
        }
    }
}