using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Quests
{
    public class CleansingOldHavenQuest : BaseQuest
    {
        public override bool DoneOnce => true;

        /* Cleansing Old Haven */
        public override object Title => 1077719;

        /* Head East out of town to Old Haven. Consecrate your weapon, cast Divine Fury, and battle monsters there until 
        you have raised your Chivalry skill to 50.<br><center>------</center><br>Hail, friend. The life of a Paladin is 
        a life of much sacrifice, humility, bravery, and righteousness. If you wish to pursue such a life, I have an 
        assignment for you. Adventure east to Old Haven, consecrate your weapon, and lay to rest the undead that inhabit 
        there.<br><br>Each ability a Paladin wishes to invoke will require a certain amount of "tithing points" to use. 
        A Paladin can earn these tithing points by donating gold at a shrine or holy place. You may tithe at this shrine.
        <br><br>Return to me once you feel that you are worthy of the rank of Apprentice Paladin. */
        public override object Description => 1077722;

        /* Farewell to you my friend. Return to me if you wish to live the life of a Paladin. */
        public override object Refuse => 1077723;

        /* There are still more undead to lay to rest. You still have more to learn. Return to me once you have done so. */
        public override object Uncomplete => 1077724;

        /* Well done, friend. While I know you understand Chivalry is its own reward, I would like to reward you with 
        something that will protect you in battle. It was passed down to me when I was a lad. Now, I am passing it on you. 
        It is called the Bulwark Leggings. Thank you for your service. */
        public override object Complete => 1077726;

        public CleansingOldHavenQuest()
            : base()
        {
            AddObjective(new ApprenticeObjective(SkillName.Chivalry, 50, "Old Haven Training", 1077720, 1077721));

            // 1077720 Your Chivalry potential is greatly enhanced while questing in this area.
            // 1077721 You are not in the quest area for Apprentice Paladin. Your Chivalry potential is not enhanced here.

            AddReward(new BaseReward(typeof(BulwarkLeggings), 1077727));
        }

        public override bool CanOffer()
        {
            #region Scroll of Alacrity
            PlayerMobile pm = Owner as PlayerMobile;
            if (pm.AcceleratedStart > DateTime.UtcNow)
            {
                Owner.SendLocalizedMessage(1077951); // You are already under the effect of an accelerated skillgain scroll.
                return false;
            }
            #endregion
            else
                return Owner.Skills.Chivalry.Base < 50;
        }

        public override void OnCompleted()
        {
            Owner.SendLocalizedMessage(1077725, null, 0x23); // You have achieved the rank of Apprentice Paladin. Return to Aelorn in New Haven to report your progress.						
            Owner.PlaySound(CompleteSound);
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

    public class Aelorn : MondainQuester
    {
        public override Type[] Quests => new Type[]
                {
                    typeof(CleansingOldHavenQuest)
                };

        public override bool IsActiveVendor => true;
        public override void InitSBInfo()
        {
            SBInfos.Add(new SBKeeperOfChivalry());
        }

        [Constructable]
        public Aelorn()
            : base("Aelorn", "The Chivalry Instructor")
        {
            SetSkill(SkillName.Anatomy, 120.0, 120.0);
            SetSkill(SkillName.MagicResist, 120.0, 120.0);
            SetSkill(SkillName.Tactics, 120.0, 120.0);
            SetSkill(SkillName.Swords, 120.0, 120.0);
            SetSkill(SkillName.Meditation, 120.0, 120.0);
            SetSkill(SkillName.Focus, 120.0, 120.0);
            SetSkill(SkillName.Chivalry, 120.0, 120.0);
        }

        public Aelorn(Serial serial)
            : base(serial)
        {
        }

        public override void Advertise()
        {
            Say(1078133); // Hail, friend. Want to live the life of a paladin?
        }

        public override void OnOfferFailed()
        {
            Say(1077772); // I cannot teach you, for you know all I can teach!
        }

        public override void InitBody()
        {
            Female = false;
            CantWalk = true;
            Race = Race.Human;

            base.InitBody();
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());
            AddItem(new VikingSword());
            AddItem(new PlateChest());
            AddItem(new PlateLegs());
            AddItem(new PlateGloves());
            AddItem(new PlateArms());
            AddItem(new PlateGorget());
            AddItem(new OrderShield());
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