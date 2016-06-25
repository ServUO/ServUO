using Server;
using Server.Mobiles;
using Server.Targeting;
using Server.Gumps;

namespace Server.Items
{
    public class ElixirOfRebirth : Item
    {
        [Constructable]
        public ElixirOfRebirth() : base(0x24E2)
        {
            this.Hue = 0x48E;
        }

        public override int LabelNumber { get { return 1112762; } } // elixir of rebirth
        
        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                return;
            }

            if (Core.AOS && (from.Paralyzed || from.Frozen || (from.Spell != null && from.Spell.IsCasting)))
            {
                from.SendLocalizedMessage(1062725); // You can not use a purple potion while paralyzed.
                return;
            }

            from.Target = new ResurrectTarget(this);

            from.SendLocalizedMessage(1112763); // Which pet do you wish to revive?
        }
        
        private class ResurrectTarget : Target
        {
            private readonly ElixirOfRebirth m_Potion;

            public ResurrectTarget(ElixirOfRebirth potion) : base(12, true, TargetFlags.None)
            {
                m_Potion = potion;
            }

            public ElixirOfRebirth Potion { get { return m_Potion; } }

            protected override void OnTarget(Mobile from, object targeted)
            {
                BaseCreature petPatient = targeted as BaseCreature;

                if (petPatient != null)
                {
                    if (!from.InRange(petPatient, 2))
                    {
                        from.SendLocalizedMessage(501042); // Target is not close enough.
                    }
                    else if (!from.Alive)
                    {
                        from.SendLocalizedMessage(501040); // The resurrecter must be alive.
                    }
                    else if (!petPatient.IsDeadPet)
                    {
                        from.SendLocalizedMessage(1112764); // This may only be used to resurrect dead pets.
                    }
                    else if (petPatient.Map == null || !petPatient.Map.CanFit(petPatient.Location, 16, false, false))
                    {
                        from.SendLocalizedMessage(501042); // Target can not be resurrected at that location.
                    }
                    else if (petPatient.Region != null && petPatient.Region.IsPartOf("Khaldun"))
                    {
                        from.SendLocalizedMessage(1010395); // The veil of death in this area is too strong and resists thy efforts to restore life.
                    }
                    else
                    {
                        Mobile master = petPatient.ControlMaster;

                        from.PlaySound(0x214);
                        from.FixedEffect(0x376A, 10, 16);
                        this.m_Potion.Delete();

                        if (master != null && from == master)
                        {
                            petPatient.ResurrectPet();                            

                            for (int i = 0; i < petPatient.Skills.Length; ++i)
                            {
                                petPatient.Skills[i].Base -= 0.1;
                            }
                        }
                        else if (master != null && master.InRange(petPatient, 3))
                        {
                            from.SendLocalizedMessage(503255); // You are able to resurrect the creature.

                            master.CloseGump(typeof(PetResurrectGump));
                            master.SendGump(new PetResurrectGump(from, petPatient));
                        }
                        else
                        {
                            bool found = false;

                            var friends = petPatient.Friends;

                            for (int i = 0; friends != null && i < friends.Count; ++i)
                            {
                                Mobile friend = friends[i];

                                if (friend.InRange(petPatient, 3))
                                {
                                    from.SendLocalizedMessage(503255); // You are able to resurrect the creature.

                                    friend.CloseGump(typeof(PetResurrectGump));
                                    friend.SendGump(new PetResurrectGump(from, petPatient));

                                    found = true;
                                    break;
                                }
                            }

                            if (!found)
                            {
                                from.SendLocalizedMessage(1049670); // The pet's owner must be nearby to attempt resurrection.
                            }
                        }
                    }
                }
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
