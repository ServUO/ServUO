using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Quests
{
    public class ScholarlyTaskQuest : BaseQuest
    {
        public override bool DoneOnce => true;

        /* A Scholarly Task */
        public override object Title => 1077603;

        /* Head East out of town and go to Old Haven.Use Evaluating Intelligence on all creatures you see there. You can also 
        cast Magery spells as well to raise Evaluating Intelligence. Do these activities until you have raised your Evaluating 
        Intelligence skill to 50. Hello. Truly knowing your opponent is essential for landing you offnesive spells with precision. 
        I can teach you how to enhance the effectiveness of you offensive spells, but first you must learn how to size up your 
        opponents intellectually. I have a scholarly task for you. Head East out of town and go to Old Haven.Use Evaluating 
        Intelligence on all creatures you see there. You can also cast Magery spells as well to raise Evaluating Intelligence.
        Come back to me once you feel that you are worthy of the rank Apprentice Scholar and i will reward you with a arcane prize. */
        public override object Description => 1077604;

        /* Return to me if you reconsider and wish to become an Apprentice Scholar. */
        public override object Refuse => 1077605;

        /* You have not achived the rank of Apprentice Scholar. Come back to me once you feel that you are worthy of the rank 
        Apprentice Scholar and i will reward you with a arcane prize. */
        public override object Uncomplete => 1077629;

        /* You have completed the task. Well Done. On behalf of the New Haven Mage Council i wish to present you with this ring. 
        When worn the Ring of the Savant enhances your inellectual aptitude and increases your mana pool. Your spell castng 
        abilities will take less time to invoke and recovering from such spell casting will be bastened. I hope the Ring of the 
        Savant serves you well. */
        public override object Complete => 1077607;

        public ScholarlyTaskQuest()
            : base()
        {
            AddObjective(new ApprenticeObjective(SkillName.EvalInt, 50, "Old Haven Training", 1077491, 1077585));

            // 1077491 Your Evaluating Intelligence potential is greatly enhanced while questing in this area.
            // 1077585 You are not in the quest area for Apprentice Scholar. Your Evaluating Intelligence potential is not enhanced here.

            AddReward(new BaseReward(typeof(RingOfTheSavant), 1077608));
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
                return Owner.Skills.EvalInt.Base < 50;
        }

        public override void OnCompleted()
        {
            Owner.SendLocalizedMessage(1077606, null, 0x23); // You have achieved the rank of Apprentice Scholar. Return to Mithneral in New Haven to receive your arcane prize.
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

    public class Mithneral : MondainQuester
    {
        public override Type[] Quests => new Type[]
                {
                    typeof(ScholarlyTaskQuest)
                };

        [Constructable]
        public Mithneral()
            : base("Mithneral", "The Evaluating Intelligence Instructor")
        {
            SetSkill(SkillName.EvalInt, 120.0, 120.0);
            SetSkill(SkillName.Inscribe, 120.0, 120.0);
            SetSkill(SkillName.Magery, 120.0, 120.0);
            SetSkill(SkillName.MagicResist, 120.0, 120.0);
            SetSkill(SkillName.Wrestling, 120.0, 120.0);
            SetSkill(SkillName.Meditation, 120.0, 120.0);
        }

        public Mithneral(Serial serial)
            : base(serial)
        {
        }

        public override void Advertise()
        {
            Say(1078127); // Want to maximize your spell damage? I have a scholarly task for you!
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
            AddItem(new HoodedShroudOfShadows(0x51C));
            AddItem(new Sandals());
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