using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class IngenuityQuest : BaseQuest
    {
        public IngenuityQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(PowerCrystal), "power crystals", 10, 0x1F1C));

            AddReward(new BaseReward(1074875)); // Another step closer to becoming human.
        }

        public override bool ForceRemember => true;
        /* Ingenuity */
        public override object Title => 1074350;
        /* The best thing about my job is that I do a little bit of everything, every day.  It's what we're good 
        at really.  Just picking up something and making it do something else.  Listen, I'm really low on parts.  
        Are you interested in fetching me some supplies? */
        public override object Description => 1074462;
        /* Okay.  Best of luck with your other endeavors. */
        public override object Refuse => 1074508;
        /* Lord overseers are the best source I know for power crystals of the type I need.  Iron golems too, 
        can have them but they're harder to find. */
        public override object Uncomplete => 1074509;
        /* Do you have those power crystals?  I'm ready to put the finishing touches on my latest experiment. */
        public override object Complete => 1074510;
        public override void GiveRewards()
        {
            base.GiveRewards();

            Owner.SendLocalizedMessage(1074946, null, 0x23); // You have demonstrated your ingenuity!  Humans are jacks of all trades and know a little about a lot of things.  You are one step closer to achieving humanity.
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

    public class HeaveHoQuest : BaseQuest
    {
        public HeaveHoQuest()
            : base()
        {
            AddObjective(new DeliverObjective(typeof(CrateForSledge), "crates for Sledge", 5, typeof(Sledge), "Sledge (Buc's Den)", 3600));

            AddReward(new BaseReward(1074875)); // Another step closer to becoming human.
        }

        public override bool ForceRemember => true;
        /* Heave Ho! */
        public override object Title => 1074351;
        /* Ho there!  There's nothing quite like a day's honest labor to make you appreciate being alive.  Hey, 
        maybe you'd like to help out with this project?  These crates need to be delivered to Sledge.  The only 
        thing is -- it's a bit of a rush job and if you don't make it in time, he won't take them.  Can I trust 
        you to help out? */
        public override object Description => 1074519;
        /* Oh yah, if you're too busy, no problem. */
        public override object Refuse => 1074521;
        /* Sledge can be found in Buc's Den.  Better hurry, he won't take those crates if you take too long with them. */
        public override object Uncomplete => 1074522;
        /* Hey, if you have cargo for me, you can start unloading over here. */
        public override object Complete => 1074523;
        public override void GiveRewards()
        {
            base.GiveRewards();

            Owner.SendLocalizedMessage(1074948, null, 0x23); // You have demonstrated your physical strength!  Humans can carry vast loads without complaint.  You are one step closer to achieving humanity.
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

    public class ResponsibilityQuest : BaseQuest
    {
        public ResponsibilityQuest()
            : base()
        {
            AddObjective(new EscortObjective("Sheep Farm"));

            AddReward(new BaseReward(typeof(TrinketBag), 1072341));
        }

        public override bool ForceRemember => true;
        /* Responsibility */
        public override object Title => 1074352;
        /* Oh!  I just don't know what to do.  My mother is away and my father told me not to talk to strangers ... 
        *worried frown*  But my grandfather has sent word that he has been hurt and needs me to tend his wounds.  
        He has a small farm southeast of here.  Would you ... could you ... escort me there safely? */
        public override object Description => 1074524;
        /* I hope my grandfather will be alright. */
        public override object Refuse => 1074525;
        /* Grandfather's farm is a ways west of the Shrine of Spirituality. So, we're not quite there yet.  Thank 
        you again for keeping me safe. */
        public override object Uncomplete => 1074526;
        public override void GiveRewards()
        {
            base.GiveRewards();

            Owner.SendLocalizedMessage(1074949, null, 0x23); // You have demonstrated your compassion!  Your kind actions have been noted.
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

    public class AllSeasonAdventurerQuest : BaseQuest
    {
        public AllSeasonAdventurerQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(Efreet), "efreets", 5, "Fire"));
            AddObjective(new SlayObjective(typeof(IceFiend), "ice fiends", 5, "Ice"));

            AddReward(new BaseReward(1074875)); // Another step closer to becoming human.
        }

        public override bool ForceRemember => true;
        /* All Season Adventurer */
        public override object Title => 1074353;
        /* It's all about hardship, suffering, struggle and pain.  Without challenges, you've got nothing to test 
        yourself against -- and that's what life is all about.  Self improvement!  Honing your body and mind!  
        Overcoming obstacles ... You'll see what I mean if you take on my challenge. */
        public override object Description => 1074527;
        /* My way of life isn't for everyone, that's true enough. */
        public override object Refuse => 1074528;
        /* You're not making much progress in the honing-mind-and-body department, are you? */
        public override object Uncomplete => 1074529;
        /* Ahhhh!  Don't you feel great?  Struggle is good for the soul. */
        public override object Complete => 1074530;
        public override void GiveRewards()
        {
            base.GiveRewards();

            Owner.SendLocalizedMessage(1074947, null, 0x23); // You have demonstrated your toughness!  Humans are able to endure unimaginable hardships in pursuit of their goals.  You are one step closer to achieving humanity.
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
}