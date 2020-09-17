using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Quests
{
    public class EnGuardeQuest : BaseQuest
    {
        public override bool DoneOnce => true;

        /* En Guarde! */
        public override object Title => 1078186;

        /* Head East out of town and go to Old Haven. Battle monster there until you have raised your Fecning skill to 50.
        Well hello there, lad. Fighting with elegance and percision is far more enriching than slugging an enemy with a 
        club or butchering an enemy with a sword. Learn the art of Fencing if you want to master combat and look good 
        doing it! The key to being a successful fencer is to be the complement and not the opposition to your opponent's 
        strength. Watch for your opponent to become off balance. Then finish him off with finesses and flair. There are 
        some undead that need cleansing out in Old Haven towards the East. Head over there and slay them, but remember, 
        do it with style! Come back to me once you have achived the rank of Apprentice Fencer, and i will reward you 
        with a prize. */
        public override object Description => 1078190;

        /* I understand, lad. Being a hero isn't for eeryone. Run along, then. Come back to me if you change your mind. */
        public override object Refuse => 1078191;

        /* You're doing well so far, but you're not quite ready yet. Head back to Old Haven, to the East, and kill some 
        more undead. */
        public override object Uncomplete => 1078192;

        /* Excellent! You are beginning to appreciate the art of Fencing. I told you fighting with elegance and precision 
        is more enriching than fighting like an orge. Since you have returned victorious, please take this war fork and 
        use it well. The war fork is a finesse weapon, and this one is magical! I call it "Recaro's Riposte". With it, 
        you will be able to parry and counterstrike with ease! your enemies will bask in your greatness and glory! Good 
        luck to you, lad, abd keep practicing! */
        public override object Complete => 1078194;

        public EnGuardeQuest()
            : base()
        {
            AddObjective(new ApprenticeObjective(SkillName.Fencing, 50, "Old Haven Training", 1078188, 1078189));

            // 1078188 You feel more dexterous and quick witted while practicing combat here. Your ability to raise your Fencing skill is enhanced in this area.
            // 1078189 You feel less dexterous here. Your Fencing learning potential is no longer enhanced.

            AddReward(new BaseReward(typeof(RecarosRiposte), 1078195));
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
                return Owner.Skills.Fencing.Base < 50;
        }

        public override void OnCompleted()
        {
            Owner.SendLocalizedMessage(1078193, null, 0x23); // You have achieved the rank of Apprentice Fencer. Return to Recaro in New Haven to claim your reward.
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

    public class Recaro : MondainQuester
    {
        public override Type[] Quests => new Type[]
                {
                    typeof(EnGuardeQuest)
                };

        [Constructable]
        public Recaro()
            : base("Recaro", "The Fencer Instructor")
        {
            SetSkill(SkillName.Anatomy, 120.0, 120.0);
            SetSkill(SkillName.Parry, 120.0, 120.0);
            SetSkill(SkillName.Healing, 120.0, 120.0);
            SetSkill(SkillName.Tactics, 120.0, 120.0);
            SetSkill(SkillName.Fencing, 120.0, 120.0);
            SetSkill(SkillName.Focus, 120.0, 120.0);
        }

        public Recaro(Serial serial)
            : base(serial)
        {
        }

        public override void Advertise()
        {
            Say(1078187); // The art of fencing requires a dexterous hand, a quick wit and fleet feet.
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
            AddItem(new Shoes(0x455));
            AddItem(new WarFork());

            Item item;

            item = new StuddedLegs
            {
                Hue = 0x455
            };
            AddItem(item);

            item = new StuddedGloves
            {
                Hue = 0x455
            };
            AddItem(item);

            item = new StuddedGorget
            {
                Hue = 0x455
            };
            AddItem(item);

            item = new StuddedChest
            {
                Hue = 0x455
            };
            AddItem(item);

            item = new StuddedArms
            {
                Hue = 0x455
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