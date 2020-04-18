using Server.Items;
using Server.Mobiles;
using Server.Services.TownCryer;
using System;

namespace Server.Engines.Quests
{
    public class RightingWrongQuest : BaseQuest
    {
        /* Righting Wrong */
        public override object Title => 1158150;

        /*The situation at the Prison Dungeon Wrong seems to have gotten out of control. The article mentioned the Royal 
         * Britannian' Guard has started contracting adventuring groups to handle the situation. Perhaps it would be
         * prudent to inquire about opportunities with the Guard at the Court of Truth in Yew.*/
        public override object Description => 1158151;

        /* You decide against accepting the quest. */
        public override object Refuse => 1158130;

        /* Inquire about the events at Wrong with the Royal Britannian Guard at the Court of Truth in Yew. */
        public override object Uncomplete => 1158152;

        public override object Complete => 1158170;
        /*You have brought Justice to the forgotten prison dungeon Wrong! The Royal Britannian Guard thanks you for your service,
         * and for not leaving the Lieutenant behind. You fought bravely this day and escaped the prison. As a thank you for your
         * service, and a testament to your accomplishment, you have been granted the title Warden of Wrong!*/

        public override int CompleteMessage => 1158291;  // You've found the Royal Guard Captain! Speak to him to learn more!

        public override int AcceptSound => 0x2E8;
        public override bool DoneOnce => true;

        public RightingWrongQuest()
        {
            AddObjective(new InternalObjective());

            AddReward(new BaseReward(1158153)); // A unique opportunity with the Royal Britannian Guard
        }

        public override void GiveRewards()
        {
            base.GiveRewards();

            QuestHelper.Delay(Owner, typeof(RightingWrongQuest), RestartDelay);
        }

        private class InternalObjective : BaseObjective
        {
            public override object ObjectiveDescription => Quest.Uncomplete;

            public InternalObjective()
                : base(1)
            {
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

    public class RightingWrongQuest2 : BaseQuest
    {
        public override QuestChain ChainID => QuestChain.RightingWrong;
        public override Type NextQuest => typeof(RightingWrongQuest3);

        /* Righting Wrong */
        public override object Title => 1158150;

        /*Another wanna-be guardsman expecting to wrangle this mess? You should know that inside Wrong there are many terrors.
         * Lizardman have squatted in the entire prison, eaten most of the prisoners and staff. The few staff that are left have
         * gone mad. If you expect to make it as a Guardsman you are going to need to thin out the heard. Head inside and kill 
         * the creatures within!*/
        public override object Description => 1158155;

        /* You decide against accepting the quest. */
        public override object Refuse => 1158130;

        /* Go inside the prison dungeon Wrong and slay the creatures within! */
        public override object Uncomplete => 1158156;

        /* Well then, I guess you aren't as useless as you look. Made quick work of the Lizards, that's fine work indeed! 
         * Your next test is going to require you travel deeper into the prison. Seems there is an ogre who has taken to mastering 
         * his cooking skills at the expense of the former prisoners. The Guard can't have that, so you need to go in there and make 
         * sure this cook is prepping his last meal! */
        public override object Complete => 1158157;

        public override int AcceptSound => 0x2E8;
        public override bool DoneOnce => true;

        public RightingWrongQuest2()
        {
            AddObjective(new SlayObjective(typeof(LizardmanDefender), "lizardman defenders", 5));
            AddObjective(new SlayObjective(typeof(LizardmanSquatter), "lizardman squatters", 5));
            AddObjective(new SlayObjective(typeof(CaveTrollWrong), "cave trolls", 5));
            AddObjective(new SlayObjective(typeof(HungryOgre), "hungry ogres", 5));

            AddReward(new BaseReward(1158167)); // A step closer to righting Wrong
        }
    }

    public class RightingWrongQuest3 : BaseQuest
    {
        public override QuestChain ChainID => QuestChain.RightingWrong;
        public override Type NextQuest => typeof(RightingWrongQuest4);

        /* Righting Wrong */
        public override object Title => 1158150;

        /* Well then, I guess you aren't as useless as you look. Made quick work of the Lizards, that's fine work indeed! 
         * Your next test is going to require you travel deeper into the prison. Seems there is an ogre who has taken to mastering 
         * his cooking skills at the expense of the former prisoners. The Guard can't have that, so you need to go in there and make 
         * sure this cook is prepping his last meal! */
        public override object Description => 1158157;

        /* You decide against accepting the quest. */
        public override object Refuse => 1158130;

        /* Go inside the prison dungeon Wrong and slay Fezzik the Ogre Cook */
        public override object Uncomplete => 1158158;

        /* Looks like Fezzik won't be making anymore stew! Hah! Well done! Your final task is going to require you to learn the
         * most important lesson of being a Guardsman - we never leave a man behind. One of our comrades was captured by the 
         * demonic jailers and taken to the prison. You need to get yourself captured by the jailers and taken inside the deepest
         * part of the prison. Once inside, find our fallen comrade and escape. Here's a copy of his orders to help you find his corpse. */
        public override object Complete => 1158163;

        //public override int CompleteMessage { get { return 1158291; } } // You've found the Royal Guard Captain! Speak to him to learn more!

        public override int AcceptSound => 0x2E8;

        public RightingWrongQuest3()
        {
            AddObjective(new SlayObjective(typeof(Fezzik), "fezzik the ogre cook", 1));

            AddReward(new BaseReward(1158167)); // A step closer to righting Wrong
        }
    }

    public class RightingWrongQuest4 : BaseQuest
    {
        public override QuestChain ChainID => QuestChain.RightingWrong;

        private RoyalBritannianGuardOrders Orders { get; set; }

        /* Righting Wrong */
        public override object Title => 1158150;

        /* Looks like Fezzik won't be making anymore stew! Hah! Well done! Your final task is going to require you to learn the
         * most important lesson of being a Guardsman - we never leave a man behind. One of our comrades was captured by the 
         * demonic jailers and taken to the prison. You need to get yourself captured by the jailers and taken inside the deepest
         * part of the prison. Once inside, find our fallen comrade and escape. Here's a copy of his orders to help you find his corpse. */
        public override object Description => 1158163;

        /* You decide against accepting the quest. */
        public override object Refuse => 1158130;

        /* Go inside the prison dungeon Wrong and get yourself captured by the Demonic Jailors. Once inside, find the fallen Guard and escape 
         * with his corpse. */
        public override object Uncomplete => 1158166;

        /* You have brought Justice to the forgotten prison dungeon Wrong! The Royal Britannian Guard thanks you for your service, 
         * and for not leaving the Lieutenant behind. You fought bravely this day and escaped the prison. As a thank you for your
         * service, and a testament to your accomplishment, you have been granted the title Warden of Wrong! */
        public override object Complete => 1158170;

        public override int CompleteMessage => 1158169;
        /*You have found the corpse of Lieutenant Bennet Yardley of the Royal Britannian Guard. Unfortunately, it seems there 
         * is not much left to return to the Field Commander. Regardless, you hold back your urge to become sick and gather his
         * remains. Escape the prison and return to the Field Commander.*/

        public override int AcceptSound => 0x2E8;

        public RightingWrongQuest4()
        {
            AddObjective(new InternalObjective());

            AddReward(new BaseReward(typeof(RightingWrongRewardTitleDeed), 1158165)); // A Unique Honor from the Royal Britannian Guard
        }

        public override void OnAccept()
        {
            base.OnAccept();

            Orders = new RoyalBritannianGuardOrders();
            Owner.Backpack.DropItem(Orders);

            Owner.SendLocalizedMessage(1154489); // You received a Quest Item!
        }

        public void CompleteQuest()
        {
            TownCryerSystem.CompleteQuest(Owner, new RightingWrongQuest());

            OnCompleted();
            GiveRewards();

            QuestHelper.Delay(Owner, typeof(RightingWrongQuest2), RestartDelay);
        }

        public override void RemoveQuest(bool removeChain)
        {
            base.RemoveQuest(removeChain);

            if (Orders != null && !Orders.Deleted)
            {
                Orders.Delete();
            }
        }

        private class InternalObjective : BaseObjective
        {
            public override object ObjectiveDescription => 1158164;  // Find the fallen Guard's corpse and escape the prison.

            public InternalObjective()
                : base(1)
            {
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

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(Orders);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            Orders = reader.ReadItem() as RoyalBritannianGuardOrders;
        }
    }

    public class Arnold : MondainQuester
    {
        public override Type[] Quests => new Type[] { typeof(RightingWrongQuest2) };

        public static Arnold TramInstance { get; set; }
        public static Arnold FelInstance { get; set; }

        public static void Initialize()
        {
            if (TramInstance == null)
            {
                TramInstance = new Arnold();
                TramInstance.MoveToWorld(new Point3D(363, 913, 0), Map.Trammel);
                TramInstance.Direction = Direction.East;
            }

            if (FelInstance == null)
            {
                FelInstance = new Arnold();
                FelInstance.MoveToWorld(new Point3D(363, 913, 0), Map.Felucca);
                FelInstance.Direction = Direction.East;
            }
        }

        public Arnold()
            : base("Arnold", "the Royal Britannian Guard")
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            CantWalk = true;

            Body = 0x190;
            Hue = Race.RandomSkinHue();
            HairItemID = 0x203C;
            FacialHairItemID = 0x204C;

            HairHue = 0x8A8;
            FacialHairHue = 0x8A8;
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());

            SetWearable(new ChainChest());
            SetWearable(new ThighBoots());
            SetWearable(new BodySash(), 1157);
            SetWearable(new Epaulette(), 1157);
            SetWearable(new ChaosShield());
            SetWearable(new Broadsword());
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m is PlayerMobile && m.InRange(Location, 5))
            {
                RightingWrongQuest4 quest = QuestHelper.GetQuest<RightingWrongQuest4>((PlayerMobile)m);

                if (quest != null && quest.Completed)
                {
                    quest.CompleteQuest();
                }
                else
                {
                    base.OnDoubleClick(m);
                }
            }
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m is PlayerMobile && InRange(m.Location, 5) && !InRange(oldLocation, 5))
            {
                RightingWrongQuest quest = QuestHelper.GetQuest<RightingWrongQuest>((PlayerMobile)m);

                if (quest != null)
                {
                    quest.OnCompleted();
                    quest.GiveRewards();
                }
            }
        }

        public Arnold(Serial serial)
            : base(serial)
        {
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

            if (Map == Map.Trammel)
            {
                TramInstance = this;
            }
            else if (Map == Map.Felucca)
            {
                FelInstance = this;
            }
        }
    }
}
