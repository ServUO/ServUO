using System;
using System.Xml;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Regions;

namespace Server.Engines.Quests
{
    public class DoneInTheNameOfTinkering : BaseQuest
    {
        /* Done in the name of Tinkering */
        public override object Title { get { return 1094983; } }

        /* Description
		 * Travel into the Abyss and find five floor traps using the Detect Hidden skill. Collect the
		 * floor traps by using the Remove Trap skill. Bring the components of the trap to Tobin for your reward.
		 * -----------------------------
		 * The name's Tobin, friend. Specialist in all forms of tinkerin' that involves traps. Only private contract stuff of course, royal projects an' the like. I've joined this Society of
		 * Ariel Haven so that I might find any new devices that might be in this ancient place.
		 * Say then, this old bird Dugan has been holding us up with a bunch of bureaucratic mumbo jumbo. Something about dangerous creatures or some such. Since you ain't under her thumb, why
		 * don't you go look around and bring ol' Tobin back any traps ye discover. I'll nedd several
		 * specimens to work out the particulars. In exchange, I will share with ye any tinkeries I discover from it. What do you say?
		 */
        public override object Description { get { return 1094985; } }

        /* So be it, friend. I can understand if ye want to go it alone. 
		 * Let me know if ye change yer mind.
		 */
        public override object Refuse { get { return 1094986; } }

        /* Hmm... Looks like traps are findin' you more than your're findin' the traps. 
		 * Keep it up though, by the looks of your clothes these traps will be a rare find!
         */
        public override object Uncomplete { get { return 1094987; } }

        /* Well done. These here are as fine a specimen of a trap if I've ever seen! 
		 * I've figured out how this thing works... I think. Here I've made some notes so you can have these back. 
		 * I've fixed it up so it is easier to deploy.
		 */
        public override object Complete { get { return 1094988; } }

        public DoneInTheNameOfTinkering()
        {
            this.AddObjective(new ObtainObjective(typeof(FloorTrapComponents), "Floor Trap Components", 5, 0xC2F));

            this.AddReward(new BaseReward(typeof(FloorTrapKit), 5, 1113293)); // Floor Trap Kit
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

    public class FiddlingTobin : MondainQuester
    {
        public override Type[] Quests
        {
            get
            {
                return new Type[]
                {
                    typeof(DoneInTheNameOfTinkering)
                };
            }
        }

        public override void Advertise()
        {
            Say(1094984); // Hail traveler.  Come here a moment.  I want to ask you somethin'.
        }

        [Constructable]
        public FiddlingTobin()
            : base("Fiddling Tobin", "the Tinkerer")
        {
        }

        public FiddlingTobin(Serial serial)
            : base(serial)
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            CantWalk = true;
            Race = Race.Human;

            Hue = 0x83EA;
            HairItemID = 0x2047;
            HairHue = 2114;
            FacialHairItemID = 0x204B;
            FacialHairHue = 2114;
        }

        public override void InitOutfit()
        {
            AddItem(new Shoes(0x72D));
            AddItem(new LongPants(0x1BB));
            AddItem(new FancyShirt(0x8FD));
            AddItem(new Backpack());
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