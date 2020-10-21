using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class PerfectTimingQuest : BaseQuest
    {
        /* Perfect Timing */
        public override object Title => 1112870;

        /* Presumptuous, are we? You think i will just let you get your grubby hands on
		 * my clever inventions! I think not! If you want to learn how to create these
		 * wonders of mechanical life, you will have to prove yourself. Correctly combine
		 * the required ingredients to build one of my inventions in a timely manner and
		 * I might share my secrets with you. */
        public override object Description => 1112873;

        /* I'm not surprised. *disdainful snort*  People with both manual and mental
		 * dexterity come in short supply. Move along then. Science does not wait for
		 * anyone. */
        public override object Refuse => 1112875;

        /* Give your assembly the material it requests. You'll find everything lying
		 * around here. Just use it. But be quick! */
        public override object Uncomplete => 1112877;

        /* There's more to you than meets the eye after all! Well done! You should enjoy
		 * this copy of my manual. */
        public override object Complete => 1112878;

        public PerfectTimingQuest()
        {
            AddObjective(new ObtainObjective(typeof(CompletedClockworkAssembly), "Completed Clockwork Assembly", 1));

            AddReward(new BaseReward(typeof(MechanicalLifeManual), 1112874)); // Mechanical Life Manual
            AddReward(new BaseReward(typeof(SuteksDirtyGear), 1115722)); // Sutek's Dirty Gear
        }

        public override void OnAccept()
        {
            base.OnAccept();

            Owner.AddToBackpack(new ClockworkMechanism());
        }

        public override void OnResign(bool resignChain)
        {
            base.OnResign(resignChain);

            Owner.DropHolding();

            Item item = Owner.Backpack.FindItemByType(typeof(ClockworkMechanism));

            if (item != null)
            {
                item.Delete();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class Sutek : MondainQuester
    {
        private static readonly Type[] m_Quests = { typeof(PerfectTimingQuest) };
        public override Type[] Quests => m_Quests;

        [Constructable]
        public Sutek()
            : base("Sutek", "the Mage")
        {
            TalkTimer();
        }

        public Sutek(Serial serial)
            : base(serial)
        {
        }

        public override void Advertise()
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Race = Race.Human;

            Hue = 0x840D;

            HairItemID = 0x203C; // Long Hair
            HairHue = 0x835;
            FacialHairItemID = 0x2040; // goatee
            FacialHairHue = 0x835;
        }

        public override void InitOutfit()
        {
            AddItem(new Sandals());
            AddItem(new TattsukeHakama(0x528));
            AddItem(new WizardsHat(0x528));
            AddItem(new Tunic(0x528));
        }

        public void TalkTimer()
        {
            Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(15.0), delegate
            {
                Say(1113224 + Utility.Random(15));
            });
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            TalkTimer();
        }
    }
}
