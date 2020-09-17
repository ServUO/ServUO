using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Quests
{
    public class ThouAndThineShieldQuest : BaseQuest
    {
        public override bool DoneOnce => true;

        /* Thou and Thine Shield */
        public override object Title => 1077704;

        /* Head East out of town and go to Old Haven. Battle monsters, or simply let them hit you, while holding a shield or a weapon 
        until you have raised your Parrying skill to 50. Oh, hello. You probably want me to teach you how to parry, don't you? Very 
        Well. First, you'll need a weapon or a shield. Obviously shields work best of all, but you can parry with a 2-handed weapon. 
        Or if you're feeling particularly brave, a 1-handed weapon will do in a pinch, I'd advise you to go to Old Haven, which you'll 
        find to the East, and practice blocking incoming blows from the undead there. You'll learn quickly if you have more than one 
        opponent attacking you at the same time to practice parrying lots of blows at once. That's the quickest way to master the art 
        of parrying. If you manage to improve your skill enough, i have a shield that you might find useful. Come back to me when you've 
        trained to an apprentice level. */
        public override object Description => 1077707;

        /* It's your choice, obviously, but I'd highly suggest that you learn to parry before adventuring out into the world. Come talk 
        to me again when you get tired of being beat on by your opponents */
        public override object Refuse => 1077708;

        /* You're doing well, but in my opinion, I Don't think you really want to continue on without improving your parrying skill a bit 
        more. Go to Old Haven, to the East, and practice blocking blows with a shield. */
        public override object Uncomplete => 1077709;

        /* Well done! You're much better at parrying blows than you were when we first met. You should be proud of your new ability and I 
        bet your body is greatful to you aswell. *Tyl Ariadne laughs loudly at his ownn (mostly lame) joke*	Oh yes, I did promise you a 
        shield if I thought you were worthy of having it, so here you go. My father made these shields for the guards who served my father 
        faithfully for many years, and I just happen to have obe that i can part with. You should find it useful as you explore the lands.
        Good luck, and may the Virtues be your guide. */
        public override object Complete => 1077711;

        public ThouAndThineShieldQuest()
            : base()
        {
            AddObjective(new ApprenticeObjective(SkillName.Parry, 50, "Old Haven Training", 1077705, 1077706));

            // 1077705 You feel as light as a butterfly, as if you could block incoming blows easily. Your ability to hone your Parrying skill is enhanced in this area. 
            // 1077706 Your inner butterfly is tired. You're not particularly able to block incoming blows well. Your Parrying learning potential is no longer enhanced.

            AddReward(new BaseReward(typeof(EscutcheonDeAriadne), 1077694));
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
                return Owner.Skills.Parry.Base < 50;
        }

        public override void OnCompleted()
        {
            Owner.SendLocalizedMessage(1077710, null, 0x23); // You have achieved the rank of Apprentice Warrior (for Parrying). Return to Tyl Ariadne in New Haven as soon as you can to claim your reward.
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

    public class TylAriadne : MondainQuester
    {
        public override Type[] Quests => new Type[]
                {
                    typeof(ThouAndThineShieldQuest)
                };

        [Constructable]
        public TylAriadne()
            : base("Tyl Ariadne", "The Parrying Instructor")
        {
            SetSkill(SkillName.Anatomy, 120.0, 120.0);
            SetSkill(SkillName.Parry, 120.0, 120.0);
            SetSkill(SkillName.Healing, 120.0, 120.0);
            SetSkill(SkillName.Tactics, 120.0, 120.0);
            SetSkill(SkillName.Swords, 120.0, 120.0);
            SetSkill(SkillName.Meditation, 120.0, 120.0);
            SetSkill(SkillName.Focus, 120.0, 120.0);
        }

        public TylAriadne(Serial serial)
            : base(serial)
        {
        }

        public override void Advertise()
        {
            Say(1078140); // Want to learn how to parry blows?
        }

        public override void OnOfferFailed()
        {
            Say(1077772); // I cannot teach you, for you know all I can teach!
        }

        public override void InitBody()
        {
            Female = false;
            CantWalk = true;
            Race = Race.Elf;

            base.InitBody();
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());
            AddItem(new ElvenBoots(0x96D));

            Item item;

            item = new StuddedLegs
            {
                Hue = 0x96D
            };
            AddItem(item);

            item = new StuddedGloves
            {
                Hue = 0x96D
            };
            AddItem(item);

            item = new StuddedGorget
            {
                Hue = 0x96D
            };
            AddItem(item);

            item = new StuddedChest
            {
                Hue = 0x96D
            };
            AddItem(item);

            item = new StuddedArms
            {
                Hue = 0x96D
            };
            AddItem(item);

            item = new DiamondMace
            {
                Hue = 0x96D
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