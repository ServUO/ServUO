/***************************************************************************
 *                                ECHandleInput.cs
 *                            -------------------
 *   begin                : May 1, 2002
 *   copyright            : (C) The RunUO Software Team
 *   email                : info@runuo.com
 *
 *   $Id$
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System;
using Server.Network;

namespace Server.Gumps
{
    public class ECHandleInput : GumpEntry
    {
        public ECHandleInput()
        {
        }

        public override string Compile()
        {
            return String.Format("{{ echandleinput }}");
        }

        private static byte[] m_LayoutName = Gump.StringToBuffer("echandleinput");

        public override void AppendTo(IGumpWriter disp)
        {
            disp.AppendLayout(m_LayoutName);
        }
    }
}
