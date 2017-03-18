
using System;
using Server.Items;

namespace Server.Engines.Quests
{
    public class KodarsRescueQuest : BaseQuest
    {
        public override bool DoneOnce { get { return true; } }

        public KodarsRescueQuest()
            : base()
        {
            this.AddObjective(new EscortObjective("Paroxysmus Exit"));

            this.AddReward(new BaseReward(typeof(LargeTreasureBag), "A large bag of treasure."));
        }

        /*Kodar's Rescue*/
        public override object Title
        {
            get
            {
                return 1073066;
            }
        }
        /*Please, please I beg of you ... help out of here.  I was trying to find one of my sheep 
           that went missing and I fell down this hole.  Then these ... *hysterical weeping*.  
           Please, get me out of here!*/
        public override object Description
        {
            get
            {
                return 1074704;
            }
        }
        /*Please!  PLEASE!  Don't let me die here.*/
        public override object Refuse
        {
            get
            {
                return 1074705;
            }
        }
        /* *whimper* Please ... hurry.  I am a good climber and could get out the way you came in, 
           if you can just get me there.*/
        public override object Uncomplete
        {
            get
            {
                return 1074706;
            }
        }
        /*You've saved my life!  Oh thank you!  I can't repay you for your kindness but please, take this.*/
        public override object Complete
        {
            get
            {
                return 1074707;
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

    public class Kodar : BaseEscort
    {

     [Constructable]
     public Kodar()
         : base()
     {

         this.Name = "Kodar";
         this.Title = "the lost villager";
     }

         public Kodar(Serial serial)
            : base(serial)
        {
        }

         public override Type[] Quests
         {
             get
             {
                 return new Type[] 
		{ 
			typeof( KodarsRescueQuest )
		};
             }
         }

           public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Race = Race.Human;

            Hue = 33790;
            FacialHairItemID = 0x204D;
            FacialHairHue = 1102;
            HairItemID = 0x204A;
            HairHue = 1102;
        }

        	public override void InitOutfit()
		{
			AddItem( new LongPants(1727) );
			AddItem( new FancyShirt() );
			AddItem( new ThighBoots() );

			PackGold( 100, 200 );
            Blessed = true;
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