
using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Engines.Quests
{
    public class KodarsRescueQuest : BaseQuest
    {
        public override bool DoneOnce { get { return true; } }

        public KodarsRescueQuest()
            : base()
        {
            AddObjective(new EscortObjective(1074780, "Paroxysmus Exit")); // Palace Entrance

            AddReward(new BaseReward(typeof(LargeTreasureBag), "A large bag of treasure."));
        }

        /* Kodar's Rescue */
        public override object Title { get { return 1073066; } }

        /* Please, please I beg of you ... help out of here. I was trying to find one of my sheep 
           that went missing and I fell down this hole.  Then these ... *hysterical weeping*.  
           Please, get me out of here!
        */
        public override object Description { get { return 1074704; } }

        /* Please!  PLEASE!  Don't let me die here. */
        public override object Refuse { get { return 1074705; } }

        /* *whimper* Please ... hurry.  I am a good climber and could get out the way you came in, 
           if you can just get me there.
        */
        public override object Uncomplete { get { return 1074706; } }

        /* You've saved my life!  Oh thank you!  I can't repay you for your kindness but please, take */
        public override object Complete { get { return 1074707; } }

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
        public static void Initialize()
        {
            if (Core.ML)
                Spawn();
        }

        public static Point3D HomeLocation { get { return new Point3D(6311, 449, -50); } }
        public static int HomeRange { get { return 5; } }

        public override Type[] Quests { get { return new Type[] { typeof(KodarsRescueQuest) }; } }

        public static List<Kodar> FelInstances { get; set; }
        public static List<Kodar> TramInstances { get; set; }

        [Constructable]
        public Kodar()
            : base()
        {
            Name = "Kodar";
            Title = "the lost villager";
        }

        public Kodar(Serial serial)
           : base(serial)
        {
        }

        public override void Advertise()
        {
            Say(1074202); // It’s you!   I’m saved, you are just in time.
        }
        
        public override void OnDelete()
        {
            if (Map == Map.Felucca && FelInstances != null && FelInstances.Contains(this))
            {
                FelInstances.Remove(this);
                FelInstances = null;
            }

            if (Map == Map.Trammel && TramInstances != null && TramInstances.Contains(this))
            {
                TramInstances.Remove(this);
                TramInstances = null;
            }

            Timer.DelayCall(TimeSpan.FromSeconds(3), new TimerCallback(
                delegate
                {
                    Spawn();
                }));

            base.OnDelete();
        }

        public static void Spawn()
        {
            if (FelInstances == null)
            {
                Kodar creature = new Kodar();
                creature.Home = HomeLocation;
                creature.RangeHome = HomeRange;
                creature.MoveToWorld(HomeLocation, Map.Felucca);

                FelInstances = new List<Kodar>();
                FelInstances.Add(creature);
            }

            if (TramInstances == null)
            {
                Kodar creature = new Kodar();
                creature.Home = HomeLocation;
                creature.RangeHome = HomeRange;
                creature.MoveToWorld(HomeLocation, Map.Trammel);

                TramInstances = new List<Kodar>();
                TramInstances.Add(creature);
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
            SetWearable(new LongPants(1727));
            SetWearable(new FancyShirt());
            SetWearable(new ThighBoots());
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

            if (Map == Map.Felucca && FelInstances == null)
            {
                FelInstances = new List<Kodar>();
                FelInstances.Add(this);
            }

            if (Map == Map.Trammel && TramInstances == null)
            {
                TramInstances = new List<Kodar>();
                TramInstances.Add(this);
            }
        }
    }
}
