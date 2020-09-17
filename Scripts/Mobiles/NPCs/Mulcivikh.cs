using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Quests
{
    public class TheAllureOfDarkMagicQuest : BaseQuest
    {
        public override bool DoneOnce => true;

        /* The Allure of Dark Magic */
        public override object Title => 1078036;

        /* Head East out of town and go to Old Haven. Cast Evil Omen and Pain Spike against monsters there until you have raised your 
        Necromancy skill to 50.<br><center>------</center><br>Welcome! I see you are allured by the dark magic of Necromancy. First, 
        you must prove yourself worthy of such knowledge. Undead currently occupy the town of Old Haven. Practice your harmful Necromancy 
        spells on them such as Evil Omen and Pain Spike.<br><br>Make sure you have plenty of reagents before embarking on your journey. 
        Reagents are required to cast Necromancy spells. You can purchase extra reagents from me, or you can find reagents growing in 
        the nearby wooded areas. You can see which reagents are required for each spell by looking in your spellbook.<br><br>Come back 
        to me once you feel that you are worthy of the rank of Apprentice Necromancer and I will reward you with the knowledge you desire. */
        public override object Description => 1078039;

        /* You are weak after all. Come back to me when you are ready to practice Necromancy. */
        public override object Refuse => 1078040;

        /* You have not achieved the rank of Apprentice Necromancer. Come back to me once you feel that you are worthy of the rank of 
        Apprentice Necromancer and I will reward you with the knowledge you desire. */
        public override object Uncomplete => 1078041;

        /* You have done well, my young apprentice. Behold! I now present to you the knowledge you desire. This spellbook contains all 
        the Necromancer spells. The power is intoxicating, isn't it? */
        public override object Complete => 1078043;

        public TheAllureOfDarkMagicQuest()
            : base()
        {
            AddObjective(new ApprenticeObjective(SkillName.Necromancy, 50, "Old Haven Training", 1078037, 1078038));

            // 1078037 Your Necromancy potential is greatly enhanced while questing in this area.
            // 1078038 You are not in the quest area for Apprentice Necromancer. Your Necromancy potential is not enhanced here.

            AddReward(new BaseReward(typeof(CompleteNecromancerSpellbook), 1078052));
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
                return Owner.Skills.Necromancy.Base < 50;
        }

        public override void OnCompleted()
        {
            Owner.SendLocalizedMessage(1078042, null, 0x23); // You have achieved the rank of Apprentice Necromancer. Return to Mulcivikh in New Haven to receive the knowledge you desire.
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

    public class Mulcivikh : MondainQuester
    {
        public override Type[] Quests => new Type[]
                {
                    typeof(TheAllureOfDarkMagicQuest)
                };

        public override void InitSBInfo()
        {
            SBInfos.Add(new SBMage());
        }

        [Constructable]
        public Mulcivikh()
            : base("Mulcivikh", "The Necromancy Instructor")
        {
            SetSkill(SkillName.Magery, 120.0, 120.0);
            SetSkill(SkillName.MagicResist, 120.0, 120.0);
            SetSkill(SkillName.SpiritSpeak, 120.0, 120.0);
            SetSkill(SkillName.Swords, 120.0, 120.0);
            SetSkill(SkillName.Meditation, 120.0, 120.0);
            SetSkill(SkillName.Necromancy, 120.0, 120.0);
        }

        public Mulcivikh(Serial serial)
            : base(serial)
        {
        }

        public override void Advertise()
        {
            Say(1078131); // Allured by dark magic, aren't you?
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
            AddItem(new Sandals(0x8FD));
            AddItem(new BoneHelm());

            Item item;

            item = new LeatherLegs
            {
                Hue = 0x2C3
            };
            AddItem(item);

            item = new LeatherGloves
            {
                Hue = 0x2C3
            };
            AddItem(item);

            item = new LeatherGorget
            {
                Hue = 0x2C3
            };
            AddItem(item);

            item = new LeatherChest
            {
                Hue = 0x2C3
            };
            AddItem(item);

            item = new LeatherArms
            {
                Hue = 0x2C3
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