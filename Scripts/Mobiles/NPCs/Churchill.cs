using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Quests
{
    public class CrushingBonesAndTakingNamesQuest : BaseQuest
    {
        public override bool DoneOnce => true;

        /* Crushing Bones and Taking Names */
        public override object Title => 1078070;

        /* Head East out of town and go to Old Haven. While wielding your mace,battle monster there until you have 
        raised your Mace Fighting skill to 50. I see you want to learn a real weapon skill and not that toothpick 
        training Jockles hasto offer. Real warriors are called Armsmen, and they wield mace weapons. No doubt about 
        it. Nothing is more satisfying than knocking the wind out of your enemies, smashing there armor, crushing 
        their bones, and taking there names. Want to learn how to wield a mace? Well i have an assignment for you. 
        Head East out of town and go to Old Haven. Undead have plagued the town, so there are plenty of bones for 
        you to smash there.	Come back to me after you have ahcived the rank of Apprentice Armsman, and i will 
        reward you with a real weapon.*/
        public override object Description => 1078065;

        /* I thought you wanted to be an Armsman and really make something of yourself. You have potential, kid, 
        but if you want to play with toothpicks, run to Jockles and he will teach you how to clean your teeth 
        with a sword. If you change your mind, come back to me, and i will show you how to wield a real weapon. */
        public override object Refuse => 1078068;

        /* Listen kid. There are a lot of undead in Old Haven, and you haven't smashed enough of them yet. So get 
        back there and do some more cleansing. */
        public override object Uncomplete => 1078067;

        /* Now that's what I'm talking about! Well done! Don't you like crushing bones and taking names? As i promised, 
        here is a war mace for you. It hits hard. It swings fast. It hits often. What more do you need? Now get out of 
        here and crush some more enemies! */
        public override object Complete => 1078069;

        public CrushingBonesAndTakingNamesQuest()
            : base()
        {
            AddObjective(new ApprenticeObjective(SkillName.Macing, 50, "Old Haven Training", 1078063, 1078064));

            // 1078063 You feel much more attuned to your mace. Your ability to hone your Mace Fighting skill is enhanced in this area.
            // 1078064 You feel less attuned to your mace. Your Mace Fighting learning potential is no longer enhanced.

            AddReward(new BaseReward(typeof(ChurchillsWarMace), 1078062));
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
                return Owner.Skills.Macing.Base < 50;
        }

        public override void OnCompleted()
        {
            Owner.SendLocalizedMessage(1078068, null, 0x23); // You have achieved the rank of Apprentice Armsman. Return to Churchill in New Haven to claim your reward.
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

    public class Churchill : MondainQuester
    {
        public override Type[] Quests => new Type[]
                {
                    typeof(CrushingBonesAndTakingNamesQuest)
                };

        [Constructable]
        public Churchill()
            : base("Churchill", "The Mace Fighting Instructor")
        {
            SetSkill(SkillName.Anatomy, 120.0, 120.0);
            SetSkill(SkillName.Parry, 120.0, 120.0);
            SetSkill(SkillName.Healing, 120.0, 120.0);
            SetSkill(SkillName.Tactics, 120.0, 120.0);
            SetSkill(SkillName.Macing, 120.0, 120.0);
            SetSkill(SkillName.Focus, 120.0, 120.0);
        }

        public Churchill(Serial serial)
            : base(serial)
        {
        }

        public override void Advertise()
        {
            Say(1078141); // Don't listen to Jockles. Real warriors wield mace weapons!
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
            Direction = Direction.South;

            base.InitBody();
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());
            AddItem(new OrderShield());
            AddItem(new WarMace());

            Item item;

            item = new PlateLegs
            {
                Hue = 0x966
            };
            AddItem(item);

            item = new PlateGloves
            {
                Hue = 0x966
            };
            AddItem(item);

            item = new PlateGorget
            {
                Hue = 0x966
            };
            AddItem(item);

            item = new PlateChest
            {
                Hue = 0x966
            };
            AddItem(item);

            item = new PlateArms
            {
                Hue = 0x966
            };
            AddItem(item);
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