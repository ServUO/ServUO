using System;
using Server.Spells;

namespace Server.Ethics.Evil
{
    public sealed class BlightPower : Power
    {
        public BlightPower()
        {
            this.m_Definition = new PowerDefinition(
                15,
                "Blight",
                "Velgo Ontawl",
                "");
        }

        public override void BeginInvoke(Player from)
        {
            from.Mobile.BeginTarget(12, true, Targeting.TargetFlags.None, new TargetStateCallback(Power_OnTarget), from);
            from.Mobile.SendMessage("Where do you wish to blight?");
        }

        private void Power_OnTarget(Mobile fromMobile, object obj, object state)
        {
            Player from = state as Player;

            IPoint3D p = obj as IPoint3D;

            if (p == null)
                return;

            if (!this.CheckInvoke(from))
                return;

            bool powerFunctioned = false;

            SpellHelper.GetSurfaceTop(ref p);
            IPooledEnumerable eable = from.Mobile.GetMobilesInRange(6);

            foreach (Mobile mob in eable)
            {
                if (mob == from.Mobile || !SpellHelper.ValidIndirectTarget(from.Mobile, mob))
                    continue;

                if (mob.GetStatMod("Holy Curse") != null)
                    continue;

                if (!from.Mobile.CanBeHarmful(mob, false))
                    continue;

                from.Mobile.DoHarmful(mob, true);

                mob.AddStatMod(new StatMod(StatType.All, "Holy Curse", -10, TimeSpan.FromMinutes(30.0)));

                mob.FixedParticles(0x374A, 10, 15, 5028, EffectLayer.Waist);
                mob.PlaySound(0x1FB);

                powerFunctioned = true;
            }

            eable.Free();

            if (powerFunctioned)
            {
                SpellHelper.Turn(from.Mobile, p);

                Effects.PlaySound(p, from.Mobile.Map, 0x1FB);

                from.Mobile.LocalOverheadMessage(Server.Network.MessageType.Regular, 0x3B2, false, "You curse the area.");

                this.FinishInvoke(from);
            }
            else
            {
                from.Mobile.FixedEffect(0x3735, 6, 30);
                from.Mobile.PlaySound(0x5C);
            }
        }
    }
}