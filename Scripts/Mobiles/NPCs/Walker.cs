using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Quests
{
    public class EyesOfRangerQuest : BaseQuest
    {
        public override bool DoneOnce => true;

        /* Eyes of a Ranger */
        public override object Title => 1078211;

        /* Track animals, monsters, and people on Haven Island until you have raised your Tracking skill to 50.	Hello Friend. I am 
        Walker, Grandmaster Ranger. An adventurer need to keep alive in the wilderness. Being able to track those around you is 
        essential to surviving in dangerous places.Certain Ninja abilites are more potent when the Ninja possesses Tracking knowledge. 
        If you want to be a Ninja, or if you simply want to get a leg up on the creatures that habits these parts. I advise you to 
        learn how to track them. You can track any animals, monsters, or people on Haven Island. Clear your mind, focus, and note 
        any tracks in the ground or sounds in the air that can help you find your mark. You can do it, friend. I have faith in you.
        Come back to me once you have achibed the rank of Apprentice Ranger (for Tracking), and i will give you something that may 
        help in your travels. Take Care, friend. */
        public override object Description => 1078217;

        /* Farewell, friend. Be careful out here. If you change your mind and want to learn Tracking come back and talk to me.*/
        public override object Refuse => 1078218;

        /* So far so good, kid. You are still alive, and you are getting the hang of Tracking. There are many more animals, monsters, 
        and people to track. Come back to me once you have tracked them. */
        public override object Uncomplete => 1078219;

        /* I knew you could do it! You have become a fine Ranger. Just keep practicing and one day you will bceome a Grandmaster 
        Ranger. Just like me. I have a little something for you that will hopefully aid you in your journeys. These leggings offer 
        some resistances that will hopefully protect you from harm. I hope these server you well. Farewell, friend. */
        public override object Complete => 1078221;

        public EyesOfRangerQuest()
            : base()
        {
            AddObjective(new ApprenticeObjective(SkillName.Tracking, 50, "Enhanced Tracking Skill", 1078215, 1078216));

            // 1078215 You feel you can track creatures here with ease. Your Tracking skill is enhanced in this area.
            // 1078216 You feel it is more difficult to track creatures here. Your Tracking skill is no longer enhanced.

            AddReward(new BaseReward(typeof(WalkersLeggings), 1078222));
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
                return Owner.Skills.Tracking.Base < 50;
        }

        public override void OnCompleted()
        {
            Owner.SendLocalizedMessage(1078220, null, 0x23); // You have achieved the rank of Apprentice Ranger (for Tracking). Return to Walker in New Haven to claim your reward.
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

    public class Walker : MondainQuester
    {
        public override Type[] Quests => new Type[]
                {
                    typeof(EyesOfRangerQuest)
                };

        [Constructable]
        public Walker()
            : base("Walker", "The Tracking Instructor")
        {
            SetSkill(SkillName.Hiding, 120.0, 120.0);
            SetSkill(SkillName.Tactics, 120.0, 120.0);
            SetSkill(SkillName.Tracking, 120.0, 120.0);
            SetSkill(SkillName.Fencing, 120.0, 120.0);
            SetSkill(SkillName.Wrestling, 120.0, 120.0);
            SetSkill(SkillName.Stealth, 120.0, 120.0);
            SetSkill(SkillName.Ninjitsu, 120.0, 120.0);
        }

        public Walker(Serial serial)
            : base(serial)
        {
        }

        public override void Advertise()
        {
            Say(Utility.RandomMinMax(1078212, 1078214));
        }

        public override void OnOfferFailed()
        {
            Say(1077772); // I cannot teach you, for you know all I can teach!
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            CantWalk = true;
            Race = Race.Human;

            base.InitBody();
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());
            AddItem(new Boots(0x455));
            AddItem(new LongPants(0x455));
            AddItem(new FancyShirt(0x47D));
            AddItem(new FloppyHat(0x455));
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