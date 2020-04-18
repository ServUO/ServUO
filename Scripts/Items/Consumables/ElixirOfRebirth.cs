using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
    public class ElixirOfRebirth : Item
    {
        [Constructable]
        public ElixirOfRebirth() : base(0x24E2)
        {
            Hue = 0x48E;
        }

        public override int LabelNumber => 1112762;  // elixir of rebirth

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
            else
            {
                from.Target = new ResurrectTarget(this);
                from.SendLocalizedMessage(1112763); // Which pet do you wish to revive?
            }
        }

        private class ResurrectTarget : Target
        {
            private readonly ElixirOfRebirth m_Potion;

            public ResurrectTarget(ElixirOfRebirth potion) : base(12, true, TargetFlags.None)
            {
                m_Potion = potion;
            }

            public ElixirOfRebirth Potion => m_Potion;

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Potion.Deleted)
                    return;

                if (!m_Potion.IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
                }
                else if (targeted is BaseCreature)
                {
                    BaseCreature petPatient = targeted as BaseCreature;

                    if (!petPatient.IsDeadBondedPet)
                        from.SendLocalizedMessage(1112764); // This may only be used to resurrect dead pets.
                    else if (petPatient.Corpse != null && !petPatient.Corpse.Deleted)
                        from.SendLocalizedMessage(1113279); // That creature's spirit lacks cohesion. Try again in a few minutes.
                    else if (!from.InRange(petPatient, 2))
                        from.SendLocalizedMessage(501042); // Target is not close enough.
                    else if (!from.Alive)
                        from.SendLocalizedMessage(501040); // The resurrecter must be alive.
                    else if (!petPatient.IsDeadPet)
                        from.SendLocalizedMessage(1112764); // This may only be used to resurrect dead pets.
                    else if (petPatient.Map == null || !petPatient.Map.CanFit(petPatient.Location, 16, false, false))
                        from.SendLocalizedMessage(501042); // Target can not be resurrected at that location.
                    else
                    {
                        from.PlaySound(0x214);
                        from.FixedEffect(0x376A, 10, 16);
                        petPatient.ResurrectPet();
                        m_Potion.Delete();
                    }
                }
                else
                    from.SendLocalizedMessage(1112764); // This may only be used to resurrect dead pets.
            }
        }

        public ElixirOfRebirth(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }
}
