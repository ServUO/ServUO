using System;
using System.Collections;
using Server.Items;
using Server.Network;
using System.Collections.Generic;
using Server.Spells.Mystic;
using Server.Spells;

namespace Server.Gumps
{
    public class EnchantSpellGump : Gump
    {
        private Mobile m_Caster;
        private Item m_Scroll;
        private BaseWeapon m_Weapon;

        public EnchantSpellGump(Mobile caster, Item scroll, BaseWeapon weapon)
            : base(20, 20)
        {
            m_Caster = caster;
            m_Scroll = scroll;
            m_Weapon = weapon;

            int font = 0x7FFF;

            AddBackground(0, 0, 170, 200, 9270);
            AddAlphaRegion(10, 10, 150, 180);

            AddHtmlLocalized(20, 22, 150, 16, 1080133, font, false, false); //Select Enchant

            AddButton(20, 50, 9702, 9703, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(45, 50, 200, 16, 1079705, font, false, false); //Hit Lighting

            AddButton(20, 75, 9702, 9703, 2, GumpButtonType.Reply, 0);
            AddHtmlLocalized(45, 75, 200, 16, 1079703, font, false, false); //Hit Fireball

            AddButton(20, 100, 9702, 9703, 3, GumpButtonType.Reply, 0);
            AddHtmlLocalized(45, 100, 200, 16, 1079704, font, false, false); //Hit Harm

            AddButton(20, 125, 9702, 9703, 4, GumpButtonType.Reply, 0);
            AddHtmlLocalized(45, 125, 200, 16, 1079706, font, false, false); //Hit Magic Arrow

            AddButton(20, 150, 9702, 9703, 5, GumpButtonType.Reply, 0);
            AddHtmlLocalized(45, 150, 200, 16, 1079702, font, false, false); //Hit Dispel
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            AosWeaponAttribute attr = AosWeaponAttribute.HitLightning;

            switch (info.ButtonID)
            {
                default:
                    m_Caster.SendLocalizedMessage(1080132); //You decide not to enchant your weapon.
                    return;
                case 1: //Hit Lightning
                    {
                        attr = AosWeaponAttribute.HitLightning;
                        break;
                    }
                case 2: //Hit Fireball
                    {
                        attr = AosWeaponAttribute.HitFireball;
                        break;
                    }
                case 3: //Hit Harm
                    {
                        attr = AosWeaponAttribute.HitHarm;
                        break;
                    }
                case 4: //Hit Magic Arrow
                    {
                        attr = AosWeaponAttribute.HitMagicArrow;
                        break;
                    }
                case 5: //Hit Dispel
                    {
                        attr = AosWeaponAttribute.HitDispel;
                        break;
                    }
            }

            Spell spell = new EnchantSpell(m_Caster, m_Scroll, m_Weapon, attr);
            spell.Cast();
        }
    }
}