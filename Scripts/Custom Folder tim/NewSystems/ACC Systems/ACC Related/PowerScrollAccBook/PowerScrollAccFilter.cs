/*

$Id: 

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

*/

using System;

namespace Server.Items
{
    public enum SkillCategory
    {
        None,
        Miscellaneous,
        CombatRatings,
        Actions,
        LoreKnowledge,
        Magical,
        CraftingHarvesting
    }

    public class PowerScrollFilter
    {
        private SkillCategory m_SkillCat;
        private int m_SkillValue;

        public bool IsDefault
        {
            get { return (m_SkillCat == SkillCategory.None && m_SkillValue == 0); }
        }

        public void Clear()
        {
            m_SkillCat = SkillCategory.None;
            m_SkillValue = 0;
        }

        public SkillCategory SkillCat
        {
            get { return m_SkillCat; }
            set { m_SkillCat = value; }
        }

        public int SkillValue
        {
            get { return m_SkillValue; }
            set { m_SkillValue = value; }
        }

        public PowerScrollFilter()
        {
        }

        public PowerScrollFilter(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            switch (version)
            {
                case 1:
                    m_SkillCat = (SkillCategory)reader.ReadEncodedInt();
                    m_SkillValue = reader.ReadEncodedInt();
                    break;
            }
        }

        public void Serialize(GenericWriter writer)
        {
            if (IsDefault)
            {
                writer.WriteEncodedInt(0);
            }
            else
            {
                writer.WriteEncodedInt(1);

                writer.WriteEncodedInt((int)m_SkillCat);
                writer.WriteEncodedInt((int)m_SkillValue);
            }
        }
    }
}