using Server.Network;
using Server.Spells;
using Server.Targeting;
using System;
using System.Linq;

namespace Server.Items
{
    public class FireHorn : Item
    {
        [Constructable]
        public FireHorn()
            : base(0xFC7)
        {
            Hue = 0x466;
            Weight = 1.0;
        }

        public FireHorn(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1060456;// fire horn
        public override void OnDoubleClick(Mobile from)
        {
            if (CheckUse(from))
            {
                from.SendLocalizedMessage(1049620); // Select an area to incinerate.
                from.Target = new InternalTarget(this);
            }
        }

        public void Use(Mobile from, IPoint3D loc)
        {
            if (!CheckUse(from))
                return;

            from.BeginAction(typeof(FireHorn));
            Timer.DelayCall(TimeSpan.FromSeconds(6.0), new TimerStateCallback(EndAction), from);

            int music = from.Skills[SkillName.Musicianship].Fixed;

            int sucChance = 500 + (music - 775) * 2;
            double dSucChance = sucChance / 1000.0;

            if (!from.CheckSkill(SkillName.Musicianship, dSucChance))
            {
                from.SendLocalizedMessage(1049618); // The horn emits a pathetic squeak.
                from.PlaySound(0x18A);
                return;
            }

            from.Backpack.ConsumeUpTo(typeof(SulfurousAsh), 4);

            from.PlaySound(0x15F);
            Effects.SendPacket(from, from.Map, new HuedEffect(EffectType.Moving, from.Serial, Serial.Zero, 0x36D4, from.Location, loc, 5, 0, false, true, 0, 0));

            System.Collections.Generic.List<Mobile> targets = SpellHelper.AcquireIndirectTargets(from, loc, from.Map, 2).OfType<Mobile>().ToList();
            int count = targets.Count;
            bool playerVsPlayer = targets.Any(t => t.Player);

            if (count > 0)
            {
                int prov = from.Skills[SkillName.Provocation].Fixed;
                int disc = from.Skills[SkillName.Discordance].Fixed;
                int peace = from.Skills[SkillName.Peacemaking].Fixed;

                int minDamage, maxDamage;

                int musicScaled = music + Math.Max(0, music - 900) * 2;
                int provScaled = prov + Math.Max(0, prov - 900) * 2;
                int discScaled = disc + Math.Max(0, disc - 900) * 2;
                int peaceScaled = peace + Math.Max(0, peace - 900) * 2;

                int weightAvg = (musicScaled + provScaled * 3 + discScaled * 3 + peaceScaled) / 80;

                int avgDamage;
                if (playerVsPlayer)
                    avgDamage = weightAvg / 3;
                else
                    avgDamage = weightAvg / 2;

                minDamage = (avgDamage * 9) / 10;
                maxDamage = (avgDamage * 10) / 9;

                double damage = Utility.RandomMinMax(minDamage, maxDamage);

                if (count > 1)
                    damage = (damage * 2) / count;

                foreach (Mobile m in targets)
                {
                    double toDeal = damage;

                    from.DoHarmful(m);
                    SpellHelper.Damage(TimeSpan.Zero, m, from, toDeal, 0, 100, 0, 0, 0);

                    Effects.SendTargetEffect(m, 0x3709, 10, 30);
                }
            }

            ColUtility.Free(targets);

            if (Utility.RandomDouble() < 0.01)
            {
                from.SendLocalizedMessage(1049619); // The fire horn crumbles in your hands.
                Delete();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }

        private static void EndAction(object state)
        {
            Mobile m = (Mobile)state;

            m.EndAction(typeof(FireHorn));
            m.SendLocalizedMessage(1049621); // You catch your breath.
        }

        private bool CheckUse(Mobile from)
        {
            if (!IsAccessibleTo(from))
                return false;

            if (from.Map != Map || !from.InRange(GetWorldLocation(), 2))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                return false;
            }

            if (!from.CanBeginAction(typeof(FireHorn)))
            {
                from.SendLocalizedMessage(1049615); // You must take a moment to catch your breath.
                return false;
            }

            if (from.Backpack == null || from.Backpack.GetAmount(typeof(SulfurousAsh)) < 4)
            {
                from.SendLocalizedMessage(1049617); // You do not have enough sulfurous ash.
                return false;
            }

            return true;
        }

        private class InternalTarget : Target
        {
            private readonly FireHorn m_Horn;
            public InternalTarget(FireHorn horn)
                : base(3, true, TargetFlags.Harmful)
            {
                m_Horn = horn;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Horn.Deleted)
                    return;

                IPoint3D loc;
                if (targeted is Item)
                    loc = ((Item)targeted).GetWorldLocation();
                else
                    loc = targeted as IPoint3D;

                m_Horn.Use(from, loc);
            }
        }
    }
}
