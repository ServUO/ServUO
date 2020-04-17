using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class DoneInTheNameOfTinkeringQuest : BaseQuest
    {
        public override object Title => 1094983;         // Done in the Name of Tinkering

        public override object Description => 1094985;   /*Travel into the Abyss and find five floor traps using 
                                                                         * the Detect Hidden skill.  Collect the floor traps by 
                                                                         * using the Remove Trap skill.  Bring the components of
                                                                         * the trap to Tobin for your reward.
                                                                         * <br><center>-----</center><br>The name's Tobin, friend.
                                                                         * Specialist in all forms of tinkerin' that involves traps.
                                                                         * Only private contract stuff of course, royal projects an'
                                                                         * the like.  I've joined this Society of Ariel Haven so 
                                                                         * that I might find any new devices that might be in this 
                                                                         * ancient place.<br><br>Say then, this old bird Dugan has 
                                                                         * been holding us up with a bunch of bureaucratic mumbo 
                                                                         * jumbo.  Something about dangerous creatures or some such.
                                                                         * Since you ain't under her thumb, why don't you go look 
                                                                         * around and bring ol' Tobin back any traps ye discover.  
                                                                         * I'll need several specimens to work out the particulars. 
                                                                         * In exchange, I will share with ye any tinkeries I 
                                                                         * discover from it.  What do you say?*/

        public override object Refuse => 1094986;        // So be it, friend.  I can understand if ye want to go it alone.  Let me know if ye change yer mind.

        public override object Uncomplete => 1094987;    // Hmm... Looks like traps are findin' you more than your're findin' the traps.  Keep it up though, 
                                                         // by the looks of your clothes these traps will be a rare find!

        public override object Complete => 1094988;      /*Well done. These here are as fine a specimen of a trap if
                                                                         * I've ever seen!  I've figured out how this thing works...
                                                                         * I think.  Here I've made some notes so you can have these
                                                                         * back.  I've fixed it up so it is easier to deploy.*/

        public DoneInTheNameOfTinkeringQuest()
        {
            AddObjective(new ObtainObjective(typeof(FloorTrapComponent), "Floor Trap Component", 5, 3117));
            AddReward(new BaseReward(typeof(GoblinFloorTrapKit), 1113293));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }

    public class Tobin : MondainQuester
    {
        [Constructable]
        public Tobin()
            : base("Fiddling Tobin", "the Tinkerer")
        {
        }

        public override void Advertise()
        {
            Say(1094984); // Hail traveler.  Come here a moment.  I want to ask you somethin'.
        }

        public Tobin(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[]
                {
                    typeof(DoneInTheNameOfTinkeringQuest)
                };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Race = Race.Human;

            Hue = 0x8418;
            HairItemID = 0x2046;
            HairHue = 0x466;
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());
            AddItem(new Shoes(0x743));
            AddItem(new Shirt(0x743));
            AddItem(new ShortPants(0x485));
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