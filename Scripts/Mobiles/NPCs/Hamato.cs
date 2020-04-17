using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Quests
{
    public class TheWayOfTheSamuraiQuest : BaseQuest
    {
        public override bool DoneOnce => true;

        /* The Way of the Samurai */
        public override object Title => 1078007;

        /* Head East out of town and go to Old Haven, use the confidence defensive stance and attempt to honorably execute 
        monsters there until you have raised your Bushido skill to 50. Greetings. I see you wish to learn the Way of the 
        Samurai. Wielding a blade is easy. Anyone can grasp a sword'd hilt. Learning how to fight properly and skillfully, 
        is to become an Armsman Learning how to master weapons, and even more importantly when not to use them, this is the 
        Way of the Warrior. The Way of the Samurai. The Code of Bushido. That is why you are here. Adventure East to Old Haven. 
        Use the Confidence defensive stance and attempt to honorably execute the undead that inhabit there. You will need a 
        book of Bushido. If you do not possess a book of Bushido, you can purchase one from me.	If you fail to honorably execute 
        the undead, your defenses will be greatly weakened. Resistances will suffer and Resisting Spells will suffer. A 
        successful parry instantly ends the weakness. If you succeed, however, you will be infused with stength and healing. 
        Your swing speed will also be boosted for a short duration. With practice, you will learn how to master your Bushido 
        abilities. Return to me once you feel that you have become an Apprentice Samurai. */
        public override object Description => 1078010;

        /* Good journey to you. Return to me if you wish to live the life of a Samurai. */
        public override object Refuse => 1078011;

        /* You have not ready to become an Apprentice Samurai. There are still alot more undead to lay to rest. Return to me 
        once you have done so. */
        public override object Uncomplete => 1078012;

        /* You have proven yourself young one. You will continue to improve as your skills are honed with age. You are an 
        honorable warrior, worthy of the rank Apprentice Samurai. Please accept this no-dachi as a gift. It is called 
        "The Dragon's Tail". Upon a successful strike in combat, there is a chance this mnighty weapon will replenish your 
        stamina equal to the damage of your attack. I hope " The Dragon's Tail" serves you well. You have earned it. Fare 
        for now. */
        public override object Complete => 1078014;

        public TheWayOfTheSamuraiQuest()
            : base()
        {
            AddObjective(new ApprenticeObjective(SkillName.Bushido, 50, "Old Haven Training", 1078008, 1078009));

            // 1078008 Your Bushido potential is greatly enhanced while questing in this area.
            // 1078009 You are not in the quest area for Apprentice Samurai. Your Bushido potential is not enhanced here.

            AddReward(new BaseReward(typeof(TheDragonsTail), 1078015));
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
                return Owner.Skills.Bushido.Base < 50;
        }

        public override void OnCompleted()
        {
            Owner.SendLocalizedMessage(1078013, null, 0x23); // You have achieved the rank of Apprentice Samurai. Return to Hamato in New Haven to report your progress.
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

    public class Hamato : MondainQuester
    {
        public override Type[] Quests => new Type[]
                {
                    typeof(TheWayOfTheSamuraiQuest)
                };

        public override bool IsActiveVendor => true;
        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBKeeperOfBushido());
        }

        [Constructable]
        public Hamato()
            : base("Hamato", "The Bushido Instructor")
        {
            SetSkill(SkillName.Anatomy, 120.0, 120.0);
            SetSkill(SkillName.Parry, 120.0, 120.0);
            SetSkill(SkillName.Healing, 120.0, 120.0);
            SetSkill(SkillName.Tactics, 120.0, 120.0);
            SetSkill(SkillName.Swords, 120.0, 120.0);
            SetSkill(SkillName.Bushido, 120.0, 120.0);
        }

        public Hamato(Serial serial)
            : base(serial)
        {
        }

        public override void Advertise()
        {
            Say(1078134); // Seek me to learn the way of the samurai.
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
            AddItem(new NoDachi());
            AddItem(new NinjaTabi());
            AddItem(new PlateSuneate());
            AddItem(new LightPlateJingasa());
            AddItem(new LeatherDo());
            AddItem(new LeatherHiroSode());
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