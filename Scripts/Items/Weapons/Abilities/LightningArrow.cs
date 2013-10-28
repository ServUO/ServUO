//----------------------------------------------------------------------------------//
// Created by Vano. Email: vano2006uo@mail.ru      //
//---------------------------------------------------------------------------------//
using System;
using System.Collections;
using Server.Spells;

namespace Server.Items
{
    public class LightningArrow : WeaponAbility
    {
        public LightningArrow()
        {
        }

        public override int BaseMana
        {
            get
            {
                return 20;
            }
        }
        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!this.Validate(attacker))
                return;

            ClearCurrentAbility(attacker);

            Map map = attacker.Map;

            if (map == null)
                return;

            BaseWeapon weapon = attacker.Weapon as BaseWeapon;

            if (weapon == null)
                return;

            if (!this.CheckMana(attacker, true))
                return;

            ArrayList list = new ArrayList();
					
            defender.PlaySound(1471);
            defender.BoltEffect(0);
            attacker.SendMessage("The Lighting Arrow strikes a target");

            foreach (Mobile m in defender.GetMobilesInRange(1))
                list.Add(m);

            ArrayList targets = new ArrayList();

            for (int i = 0; i < list.Count; ++i)
            {
                Mobile m = (Mobile)list[i];

                if (m != defender && m != attacker && SpellHelper.ValidIndirectTarget(attacker, m))
                {
                    if (m == null || m.Deleted || m.Map != attacker.Map || !m.Alive || !attacker.CanSee(m) || !attacker.CanBeHarmful(m))
                        continue;

                    if (!attacker.InRange(m, weapon.MaxRange))
                        continue;

                    if (attacker.InLOS(m))
                        targets.Add(m);
                }
            }

            if (targets.Count > 0)
            {
                double damageBonus = 1.0 + Math.Pow(1, 2) / 100;

                for (int i = 0; i < targets.Count; ++i)
                {
                    Mobile m = (Mobile)targets[i];

                    attacker.SendMessage("The Lighting Arrow strikes around a target");
                    m.PlaySound(1471);
                    m.BoltEffect(0);
                    weapon.OnHit(attacker, m, damageBonus);
                }
            }
        }
    }
}