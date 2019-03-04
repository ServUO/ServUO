using System;
using Server;
using Server.Mobiles;
using Server.Items;
using System.Linq;

namespace Server.Engines.TreasuresOfKotlCity
{
    public class EnergyTileAddon : BaseAddon
    {
        public DateTime m_NextDeactivation;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextDeactivation { get { return m_NextDeactivation; } set { m_NextDeactivation = value; } }

        public override BaseAddonDeed Deed { get { return null; } }

        [Constructable]
        public EnergyTileAddon(int count, Direction direction)
        {
            int x = 0;
            int y = 0;

            for (int i = 0; i < count; i++)
            {
                if (i == 0)
                {
                    AddComponent(new EnergyTileComponent(), x, y, 0);
                }
                else
                {
                    Movement.Movement.Offset(direction, ref x, ref y);
                    AddComponent(new EnergyTileComponent(), x, y, 0);
                }
            }

            DeactivateRandom();
        }

        private void DeactivateRandom()
        {
            EnergyTileComponent comp = Components[Utility.Random(Components.Count)] as EnergyTileComponent;

            foreach (EnergyTileComponent component in Components.OfType<EnergyTileComponent>())
            {
                if (comp == component)
                {
                    comp.Active = false;
                    m_NextDeactivation = DateTime.UtcNow + TimeSpan.FromMinutes(120);
                }
                else if (!component.Active)
                {
                    component.Active = true;
                }
            }
        }

        public EnergyTileAddon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version

            writer.Write(m_NextDeactivation);

            Timer.DelayCall(TimeSpan.FromSeconds(10), () =>
                {
                    if (m_NextDeactivation < DateTime.UtcNow)
                        DeactivateRandom();
                });
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_NextDeactivation = reader.ReadDateTime();
        }
    }

    public class EnergyTileComponent : AddonComponent
    {
        private bool m_Active;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active { get { return m_Active; } set { m_Active = value; } }

        public EnergyTileComponent() : base(0x9B3A)
        {
            m_Active = true;
            Hue = 2591;
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (!m_Active)
                return base.OnMoveOver(m);

            if (m is PlayerMobile || (m is BaseCreature && ((BaseCreature)m).GetMaster() is PlayerMobile))
            {
                AOS.Damage(m, Utility.RandomMinMax(100, 150), false, 0, 0, 0, 0, 100);

                m.FixedParticles(0x3818, 1, 11, 0x13A8, 0, 0, EffectLayer.Waist);
                m.PlaySound(0x665);
            }

            return base.OnMoveOver(m);
        }

        public EnergyTileComponent(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version

            writer.Write(m_Active);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Active = reader.ReadBool();
        }
    }
}