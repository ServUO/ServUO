using System;
using Server.Engines.PartySystem;
using Server.Targeting;
using System.Linq;
using Server.Mobiles;

namespace Server.Items
{
    public class ExodusSummoningRite : BaseDecayingItem
    {
        public override int LabelNumber { get { return 1153498; } } // exodus summoning rite 

        [Constructable]
        public ExodusSummoningRite() : base(0x2258)
        {
            this.Weight = 1;
            this.Hue = 1910;
            this.LootType = LootType.Regular;
        }

        public override int Lifespan { get { return 604800; } }
        public override bool UseSeconds { get { return false; } }

        public ExodusSummoningRite(Serial serial) : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            RobeofRite robe = from.FindItemOnLayer(Layer.OuterTorso) as RobeofRite;
            ExodusSacrificalDagger dagger = from.FindItemOnLayer(Layer.OneHanded) as ExodusSacrificalDagger;

            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1054107); // This item must be in your backpack.
            }
            else if (Party.Get(from) == null)
            {
                from.SendLocalizedMessage(1153596); // You must join a party with the players you wish to perform the ritual with. 
            }
            else if (robe == null || dagger == null)
            {
                from.SendLocalizedMessage(1153591); // Thou art not properly attired to perform such a ritual.
            }
            else
            {
                from.SendLocalizedMessage(1153600); // Which Summoning Tome do you wish to use this on? 
                from.Target = new RiteTarget(this);
            }
        }

        public class RiteTarget : Target
        {
            private Item m_Deed;

            public RiteTarget(Item deed) : base(2, true, TargetFlags.None)
            {
                m_Deed = deed;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is ExodusTomeAltar)
                {
                    ExodusTomeAltar altar = (ExodusTomeAltar)targeted;

                    if (altar.CheckParty(altar.Owner, from))
                    {
                        if (altar.Rituals.Count(s => s.RitualMobile == from) == 0)
                        {
                            altar.Rituals.Add(new RitualArray { RitualMobile = from, Ritual1 = false, Ritual2 = false });
                        }

                        bool RiteRitual = altar.Rituals.Find(s => s.RitualMobile == from).Ritual1;

                        if (!RiteRitual)
                        {
                            ((PlayerMobile)from).UseSummoningRite = true;
                            from.Say(1153597); // You place the rite within the tome and begin to meditate...
                            altar.Rituals.Find(s => s.RitualMobile == from).Ritual1 = true;
                            m_Deed.Delete();
                            from.SendLocalizedMessage(1153598, from.Name); // ~1_PLAYER~ has read the Summoning Rite! 
                        }
                        else
                        {
                            from.SendLocalizedMessage(1153599); // You've already used this item in another ritual. 
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage(1153595); // You must first join the party of the person who built this altar.
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1153601); // That is not a Summoning Tome. 
                }
            }
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
