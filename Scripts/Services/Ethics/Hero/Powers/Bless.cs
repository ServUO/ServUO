using System;
using Server.Spells;

namespace Server.Ethics.Hero
{
    public sealed class Bless : Power
    {
        public Bless()
        {
            this.m_Definition = new PowerDefinition(
                15,
                "Bless",
                "Erstok Ontawl",
                "");
        }

        public override void BeginInvoke(Player from)
        {
            from.Mobile.BeginTarget(12, true, Targeting.TargetFlags.None, new TargetStateCallback(Power_OnTarget), from);
            from.Mobile.SendMessage("Where do you wish to bless?");
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

            foreach (Mobile mob in from.Mobile.GetMobilesInRange(6))
            {
                if (mob != from.Mobile && SpellHelper.ValidIndirectTarget(from.Mobile, mob))
                    continue;

                if (mob.GetStatMod("Holy Bless") != null)
                    continue;

                if (!from.Mobile.CanBeBeneficial(mob, false))
                    continue;

                from.Mobile.DoBeneficial(mob);

                mob.AddStatMod(new StatMod(StatType.All, "Holy Bless", 10, TimeSpan.FromMinutes(30.0)));

                mob.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
                mob.PlaySound(0x1EA);

                powerFunctioned = true;
            }

            if (powerFunctioned)
            {
                SpellHelper.Turn(from.Mobile, p);

                Effects.PlaySound(p, from.Mobile.Map, 0x299);

                from.Mobile.LocalOverheadMessage(Server.Network.MessageType.Regular, 0x3B2, false, "You consecrate the area.");

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