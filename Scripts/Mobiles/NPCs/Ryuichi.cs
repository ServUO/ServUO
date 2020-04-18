using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Quests
{
    public class TheArtOfStealthQuest : BaseQuest
    {
        public override bool DoneOnce => true;

        /* The Art of Stealth */
        public override object Title => 1078154;

        /* Head East out of town and go to Old Haven. While wielding your fencing weapon, battle monsters with focus attack 
        and summon mirror images up to 40 Ninjitsu skill, and continue practicing focus attack on monsters until 50 Ninjitsu 
        skill. Welcome, young one. You seek to learn Ninjitsu. With it, and the book of Ninjitsu, a Ninja can envoke a number 
        of special abilites including transforming into a variety of creatures that give unique bonuses, using stealth to 
        attack unsuspecting opponents or just plain disappear into thin air! If you do not have a book of Ninjitsu , you can 
        purchase one from me. I have an assignment for you. Head East out of town and go to Old Haven. While wielding your 
        fencing weapon, battle monsters with focus attack and summon mirror images up to the Novice rank, and continue focusing 
        your attack for greater damage on monsters until you become an Apprentice Ninja. Each image will absorb one attack. The 
        art of deception is a strong defense. Use it wisely. Come back to me once you have ahcived the rank of Apprentice Ninja, 
        and i shall reward you with something useful. */
        public override object Description => 1078158;

        /* Come back to me if you want to learn Ninjitsu in the future.. */
        public override object Refuse => 1078159;

        /* You have not ahcived the rank of Apprentice Ninja. Come back to me once you have done so. */
        public override object Uncomplete => 1078160;

        /* You have done well, young one. Please accept this kryss as a gift. It is called the "silver Serpant Blade". With it, 
        you will strike with precision and power. This should aid you in your journey as a Ninja. Farewell. */
        public override object Complete => 1078162;

        public TheArtOfStealthQuest()
            : base()
        {
            AddObjective(new ApprenticeObjective(SkillName.Ninjitsu, 50, "Old Haven Training", 1078156, 1078157));

            // 1078156 You feel a greater sense of awareness here. Your ability to hone your Ninjitsu skill is enhanced in this area.
            // 1078157 You feel your sense of awareness is normal here. Your Ninjitsu learning potential is no longer enhanced.

            AddReward(new BaseReward(typeof(SilverSerpentBlade), 1078163));
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
                return Owner.Skills.Ninjitsu.Base < 50;
        }

        public override void OnCompleted()
        {
            Owner.SendLocalizedMessage(1078161, null, 0x23); // You have achieved the rank of Apprentice Ninja. Return to Ryuichi in New Haven to see what kind of reward he has waiting for you.
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

    public class Ryuichi : MondainQuester
    {
        public override Type[] Quests => new Type[]
                {
                    typeof(TheArtOfStealthQuest)
                };

        public override bool IsActiveVendor => true;
        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBKeeperOfNinjitsu());
        }

        [Constructable]
        public Ryuichi()
            : base("Ryuichi", "The Ninjitsu Instructor")
        {
            SetSkill(SkillName.Hiding, 120.0, 120.0);
            SetSkill(SkillName.Tracking, 120.0, 120.0);
            SetSkill(SkillName.Healing, 120.0, 120.0);
            SetSkill(SkillName.Tactics, 120.0, 120.0);
            SetSkill(SkillName.Fencing, 120.0, 120.0);
            SetSkill(SkillName.Stealth, 120.0, 120.0);
            SetSkill(SkillName.Ninjitsu, 120.0, 120.0);
        }

        public Ryuichi(Serial serial)
            : base(serial)
        {
        }

        public override void Advertise()
        {
            Say(1078155); // I can teach you Ninjitsu. The Art of Stealth.
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
            AddItem(new SamuraiTabi());
            AddItem(new LeatherNinjaPants());
            AddItem(new LeatherNinjaHood());
            AddItem(new LeatherNinjaBelt());
            AddItem(new LeatherNinjaMitts());
            AddItem(new LeatherNinjaJacket());
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