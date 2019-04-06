using System;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Engines.Quests
{
    public class TamingPetQuest : BaseQuest
    {
        public TamingPetQuest()
            : base()
        {
            AddObjective(new InternalObjective());

            AddReward(new BaseReward(1157538)); // A step closer to mastering Animal Training.
        }

        public override QuestChain ChainID{get {return QuestChain.AnimalTraining; } }
        public override Type NextQuest { get { return typeof(UsingAnimalLoreQuest); } }

        /* Discovering Animal Training */
        public override object Title { get { return 1157527; } }

        /*Years of patience and meticulous study have paid off!  New Animal Training techniques have been discovered!  
         * Animal tamers may now train their pets, teaching them new ways to fight and survive!  The first step is to 
         * tame a creature.  Find a creature in the wild, and using your animal taming skill - tame it!*/
        public override object Description { get { return 1157528; } }

        /* The life of an animal trainer is not for everyone, return to an Animal Trainer if you wish to try again. */
        public override object Refuse { get { return 1157530; } }

        /* Find a creature in the wild, and using the animal taming skill - tame it! */
        public override object Uncomplete { get { return 1157531; } }

        /* Well done!  Now that you have your pet it is time to start training! */
        public override object Complete { get { return 1157532; } }

        public override int AcceptSound { get { return 0x2E8; } }
        public override int CompleteMessage { get { return 1157539; } } // // You've completed an Animal Training quest! Visit an Animal Trainer to continue!				

        public static void CheckTame(PlayerMobile pm)
        {
            if (pm == null)
                return;

            var quest = QuestHelper.GetQuest(pm, typeof(TamingPetQuest));

            if (quest != null && !quest.Completed)
            {
                quest.Objectives[0].CurProgress++;
                quest.OnCompleted();
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

        public class InternalObjective : BaseObjective
        {
            public override object ObjectiveDescription { get { return 1157529; } } // Tame a Creature

            public InternalObjective()
                : base(1)
            {
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

    public class UsingAnimalLoreQuest : BaseQuest
    {
        public UsingAnimalLoreQuest()
            : base()
        {
            AddObjective(new InternalObjective());

            AddReward(new BaseReward(1157538)); // A step closer to mastering Animal Training.
        }

        public override QuestChain ChainID { get { return QuestChain.AnimalTraining; } }
        public override Type NextQuest { get { return typeof(LeadingIntoBattleQuest); } }
        //public override bool DoneOnce { get { return true; } }

        /* Discovering Animal Training */
        public override object Title { get { return 1157527; } }

        /*Now that your pet is tame, you must begin the training process.  Pets will train while they are engaged in 
         * combat, and will progress as they battle other creatures.  Pets train best against wild creatures, and will
         * learn the most from the fiercest creatures in the realm!  There is a limit to how much a pet can learn from 
         * a single foe, so make sure your pet has fresh adversaries!<br><br>When you are ready to begin the training 
         * process, use the Animal Lore skill on your pet and select "Begin Animal Training."  When your pet has 
         * completed the training process you can teach them new ways to fight and survive!*/
        public override object Description { get { return 1157533; } }

        /* The life of an animal trainer is not for everyone, return to an Animal Trainer if you wish to try again. */
        public override object Refuse { get { return 1157530; } }

        /* When you are ready to begin the training process, use the Animal Lore skill on your pet and select "Begin Animal Training." */
        public override object Uncomplete { get { return 1157535; } }

        /* Well done!  Now that your pet has begun the Animal Training process return to the Animal Trainer to learn more about the next steps. */
        public override object Complete { get { return 1157536; } }

        public override void OnCompleted()
        {
            Owner.SendLocalizedMessage(1157536, null, 0x23); // Well done!  Now that your pet has begun the Animal Training process return to the Animal Trainer to learn more about the next steps.					
            Owner.PlaySound(CompleteSound);
        }

        public static void CheckComplete(PlayerMobile pm)
        {
            if (pm == null)
                return;

            var quest = QuestHelper.GetQuest(pm, typeof(UsingAnimalLoreQuest));

            if (quest != null && !quest.Completed)
            {
                quest.Objectives[0].CurProgress++;
                quest.OnCompleted();
            }
        }

        public class InternalObjective : BaseObjective
        {
            public override object ObjectiveDescription { get { return 1157534; } }
            /*Use the Animal Lore Skill on your pet and select "Begin Animal Training."<br><br> */

            public InternalObjective()
                : base(1)
            {
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

    public class LeadingIntoBattleQuest : BaseQuest
    {
        public LeadingIntoBattleQuest()
            : base()
        {
            AddObjective(new InternalObjective());

            AddReward(new BaseReward(1157538)); // A step closer to mastering Animal Training.
        }

        public override QuestChain ChainID { get { return QuestChain.AnimalTraining; } }
        public override Type NextQuest { get { return typeof(TeachingSomethingNewQuest); } }
        //public override bool DoneOnce { get { return true; } }

        /* Discovering Animal Training */
        public override object Title { get { return 1157527; } }

        /*Now that you have started the training process it is time to lead your pet into battle!  Pets will train while 
         * they are engaged in combat, and will progress as they battle other creatures.  Pets train best against wild 
         * creatures, and will learn the most from the fiercest creatures in the realm!  There is a limit to how much a 
         * pet can learn from a single foe, so make sure your pet has fresh adversaries!  When the "Pet Training Progress"
         * bar is full, your pet is ready to learn new ways to fight and survive.  <br><br>Now you must lead your pet into 
         * the wild and battle it against other creatures!*/
        public override object Description { get { return 1157540; } }

        /* The life of an animal trainer is not for everyone, return to an Animal Trainer if you wish to try again. */
        public override object Refuse { get { return 1157530; } }

        /* Lead your pet into the wild and battle it against other creatures until the "Pet Training Progress" bar is full. 
         * Remember Pets train best against wild creatures, and will learn the most from the fiercest creatures in the realm!  
         * There is a limit to how much a pet can learn from a single foe, so make sure your pet has fresh adversaries! */
        public override object Uncomplete { get { return 1157542; } }

        /* Well done!  Now that your pet has begun the Animal Training process return to the Animal Trainer to learn more about the next steps. */
        public override object Complete { get { return 1157536; } }

        public override void OnCompleted()
        {
            Owner.SendLocalizedMessage(1157536, null, 0x23); // Well done!  Now that your pet has begun the Animal Training process return to the Animal Trainer to learn more about the next steps.	
            Owner.PlaySound(CompleteSound);
        }

        public static void CheckComplete(PlayerMobile pm)
        {
            if (pm == null)
                return;

            var quest = QuestHelper.GetQuest(pm, typeof(LeadingIntoBattleQuest));

            if (quest != null && !quest.Completed)
            {
                quest.Objectives[0].CurProgress++;
                quest.OnCompleted();
            }
        }

        public class InternalObjective : BaseObjective
        {
            public override object ObjectiveDescription { get { return 1157541; } }
            /*Lead your pet into the wild and battle it against other creatures until the "Pet Training Progress" bar is full.*/

            public InternalObjective()
                : base(1)
            {
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

    public class TeachingSomethingNewQuest : BaseQuest
    {
        public TeachingSomethingNewQuest()
            : base()
        {
            AddObjective(new InternalObjective());

            AddReward(new BaseReward(1157538)); // A step closer to mastering Animal Training.
        }

        public override QuestChain ChainID { get { return QuestChain.AnimalTraining; } }
        public override Type NextQuest { get { return null; } }
        public override bool DoneOnce { get { return true; } }

        /* Discovering Animal Training */
        public override object Title { get { return 1157527; } }

        /*Now that your pet has fully trained, it is time to teach it something new!  Use the Animal Lore skill on your pet and select 
         * "Pet Training Options."  The Animal Training menu lists all of the available training properties you can apply to the pet.
         * <br><br>The Categories pane shows the category of available properties:<br><br>Stats and Resists allow you to increase
         * individual Stat and Resist properties for the pet.<br><br>Increase Magic Skill Caps and Increase Combat Skill Caps allow
         * you to increase skill caps for various magical and combat related skills.  This process requires the use of Power Scrolls
         * and only increases the skill cap, you will still need to train the pet in the specific skill through traditional pet skill 
         * training. <br><br>Magical Abilities allow you to give the pet magical abilities in one of several spell schools<br><br>Special
         * Abilities allow you to give the pet special abilities, different than those traditionally found as weapon special moves.
         * <br><br>Special Moves allow you to give the pet special moves, similar to those traditionally found as weapon special moves. 
         * <br><br>Area Effect Abilities allow you to give the pet an area attack, targeting multiple adversaries within an area.<br><br>
         * When you train your pet, the number of control slots the pet requires will increase.  The maximum number of control slots any 
         * pet can have is 5, however individual pets have maximum control slots they can be trained to.<br><br>A pet can only learn so
         * much during each training level.  As you mix and match properties from the Animal Training menu, the amount of available training
         * points will decrease based on your selections.  Different property selections have different training point costs.  When you are
         * ready to apply a new property to your pet, select "Train Pet" and confirm you are ready to do so!*/
        public override object Description { get { return 1157545; } }

        /* The life of an animal trainer is not for everyone, return to an Animal Trainer if you wish to try again. */
        public override object Refuse { get { return 1157530; } }

        /*Use the Animal Lore skill on your pet and select "Pet Training Options" to mix and match which properties 
             * you will train your pet.  When you are satisfied with the property you have chosen select "Train Pet" 
             * and confirm the training!.*/
        public override object Uncomplete { get { return 1157546; } }

        /* You have Discovered Animal Training!  Train new pets and mix and match properties to create unique variations of pets to take into 
         * battle!  Good Luck, Animal Trainer! */
        public override object Complete { get { return 1157547; } }

        public override void OnCompleted()
        {
            Owner.SendLocalizedMessage(1157539, null, 0x23); // You've completed an Animal Training quest! Visit an Animal Trainer to continue!						
            Owner.PlaySound(CompleteSound);
        }

        public static void CheckComplete(PlayerMobile pm)
        {
            if (pm == null)
                return;

            var quest = QuestHelper.GetQuest(pm, typeof(TeachingSomethingNewQuest));

            if (quest != null && !quest.Completed)
            {
                quest.Objectives[0].CurProgress++;
                quest.OnCompleted();
            }
        }

        public override void GiveRewards()
        {
            base.GiveRewards();

            Owner.AddToBackpack(new EthologistTitleDeed());
        }

        public class InternalObjective : BaseObjective
        {
            public override object ObjectiveDescription { get { return 1157546; } }
            /*Use the Animal Lore skill on your pet and select "Pet Training Options" to mix and match which properties 
            * you will train your pet.  When you are satisfied with the property you have chosen select "Train Pet" 
            * and confirm the training!.*/

            public InternalObjective()
                : base(1)
            {
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