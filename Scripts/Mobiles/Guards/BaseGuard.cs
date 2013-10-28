using System;
using Server.Items;

namespace Server.Mobiles
{
    public abstract class BaseGuard : Mobile
    {
        public BaseGuard(Mobile target)
        {
            if (target != null)
            {
                this.Location = target.Location;
                this.Map = target.Map;

                Effects.SendLocationParticles(EffectItem.Create(this.Location, this.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 5023);
            }
        }

        public BaseGuard(Serial serial)
            : base(serial)
        {
        }

        public abstract Mobile Focus { get; set; }
        public static void Spawn(Mobile caller, Mobile target)
        {
            Spawn(caller, target, 1, false);
        }

        public static void Spawn(Mobile caller, Mobile target, int amount, bool onlyAdditional)
        {
            if (target == null || target.Deleted)
                return;

            foreach (Mobile m in target.GetMobilesInRange(15))
            {
                if (m is BaseGuard)
                {
                    BaseGuard g = (BaseGuard)m;

                    if (g.Focus == null) // idling
                    {
                        g.Focus = target;

                        --amount;
                    }
                    else if (g.Focus == target && !onlyAdditional)
                    {
                        --amount;
                    }
                }
            }

            while (amount-- > 0)
                caller.Region.MakeGuard(target);
        }

        public override bool OnBeforeDeath()
        {
            Effects.SendLocationParticles(EffectItem.Create(this.Location, this.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);

            this.PlaySound(0x1FE);

            this.Delete();

            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}