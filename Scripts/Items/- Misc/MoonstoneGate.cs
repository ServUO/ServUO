using System;
using Server.Engines.PartySystem;

namespace Server.Items
{
    public class MoonstoneGate : Moongate
    {
        private readonly Mobile m_Caster;
        public MoonstoneGate(Point3D loc, Map map, Map targetMap, Mobile caster, int hue)
            : base(loc, targetMap)
        {
            this.MoveToWorld(loc, map);
            this.Dispellable = false;
            this.Hue = hue;

            this.m_Caster = caster;

            new InternalTimer(this).Start();

            Effects.PlaySound(loc, map, 0x20E);
        }

        public MoonstoneGate(Serial serial)
            : base(serial)
        {
        }

        public override void CheckGate(Mobile m, int range)
        {
            if (m.Kills >= 5)
                return;

            Party casterParty = Party.Get(this.m_Caster);
            Party userParty = Party.Get(m);

            if (m == this.m_Caster || (casterParty != null && userParty == casterParty))
                base.CheckGate(m, range);
        }

        public override void UseGate(Mobile m)
        {
            if (m.Kills >= 5)
                return;

            Party casterParty = Party.Get(this.m_Caster);
            Party userParty = Party.Get(m);

            if (m == this.m_Caster || (casterParty != null && userParty == casterParty))
                base.UseGate(m);
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

            this.Delete();
        }

        private class InternalTimer : Timer
        {
            private readonly Item m_Item;
            public InternalTimer(Item item)
                : base(TimeSpan.FromSeconds(30.0))
            {
                this.m_Item = item;
                this.Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                this.m_Item.Delete();
            }
        }
    }
}