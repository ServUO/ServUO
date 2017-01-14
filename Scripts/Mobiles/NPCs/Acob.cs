using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class OverpopulationQuest : BaseQuest
    { 
        public OverpopulationQuest()
            : base()
        { 
            this.AddObjective(new SlayObjective(typeof(Hind), "hinds", 10));
			
            this.AddReward(new BaseReward(typeof(SmallTrinketBag), 1072268));
        }

        /* Overpopulation */
        public override object Title
        {
            get
            {
                return 1072252;
            }
        }
        /* I just can't bear it any longer.  Sure, it's my job to thin the deer out so 
        they don't overeat the area and starve themselves come winter time.  Sure, I 
        know we killed off the predators that would do this naturally so now we have 
        to make up for it.  But they're so graceful and innocent.  I just can't do it.  
        Will you? */
        public override object Description
        {
            get
            {
                return 1072267;
            }
        }
        /* Well, okay. But if you decide you are up for it after all, c'mon back and see me. */
        public override object Refuse
        {
            get
            {
                return 1072270;
            }
        }
        /* You're not quite done yet.  Get back to work! */
        public override object Uncomplete
        {
            get
            {
                return 1072271;
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

    public class WildBoarCullQuest : BaseQuest
    { 
        public WildBoarCullQuest()
            : base()
        { 
            this.AddObjective(new SlayObjective(typeof(Boar), "boars", 10));
			
            this.AddReward(new BaseReward(typeof(SmallTrinketBag), 1072268));
        }

        /* Wild Boar Cull */
        public override object Title
        {
            get
            {
                return 1072245;
            }
        }
        /* A pity really.  With the balance of nature awry, we have no choice but to accept 
        the responsibility of making it all right.  It's all a part of the circle of life, 
        after all. So, yes, the boars are running rampant. There are far too many in the 
        region.  Will you shoulder your obligations as a higher life form? */
        public override object Description
        {
            get
            {
                return 1072260;
            }
        }
        /* Well, okay. But if you decide you are up for it after all, c'mon back and see me. */
        public override object Refuse
        {
            get
            {
                return 1072270;
            }
        }
        /* You're not quite done yet.  Get back to work! */
        public override object Uncomplete
        {
            get
            {
                return 1072271;
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

    public class NewLeadershipQuest : BaseQuest
    { 
        public NewLeadershipQuest()
            : base()
        { 
            this.AddObjective(new SlayObjective(typeof(SerpentsFangHighExecutioner), "serpent's fang high executioner", 1, "TheCitadel"));
            this.AddObjective(new SlayObjective(typeof(TigersClawThief), "tiger's claw thief", 1, "TheCitadel"));
            this.AddObjective(new SlayObjective(typeof(DragonsFlameGrandMage), "dragon's flame mage", 1, "TheCitadel"));
			
            this.AddReward(new BaseReward(typeof(RewardBox), 1072584));
        }

        /* New Leadership */
        public override object Title
        {
            get
            {
                return 1072905;
            }
        }
        /* I have a task for you ... adventurer.  Will you risk all to win great renown?  The 
        Black Order is organized into three sects, each with their own speciality.  The Dragon's 
        Flame serves the will of the Grand Mage, the Tiger's Claw answers to the Master Thief, 
        and the Serpent's Fang kills at the direction of the High Executioner.  Slay all three 
        and you will strike the order a devastating blow! */
        public override object Description
        {
            get
            {
                return 1072963;
            }
        }
        /* I do not fault your decision. */
        public override object Refuse
        {
            get
            {
                return 1072973;
            }
        }
        /* Once you gain entrance into The Citadel, you will need to move cautiously to find 
        the sect leaders. */
        public override object Uncomplete
        {
            get
            {
                return 1072974;
            }
        }
        public override bool CanOffer()
        {
            return MondainsLegacy.Citadel;
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

    public class ExAssassinsQuest : BaseQuest
    { 
        public ExAssassinsQuest()
            : base()
        { 
            this.AddObjective(new InternalObjective());
			
            this.AddReward(new BaseReward(typeof(TreasureBag), 1072583));
        }

        /* Ex-Assassins */
        public override object Title
        {
            get
            {
                return 1072917;
            }
        }
        /* The Serpent's Fang sect members have gone too far! Express to them my displeasure by slaying 
        ten of them. But remember, I do not condone war on women, so I will only accept the deaths of 
        men, human and elf. */
        public override object Description
        {
            get
            {
                return 1072969;
            }
        }
        /* As you wish. */
        public override object Refuse
        {
            get
            {
                return 1072979;
            }
        }
        /* The Black Order's fortress home is well hidden.  Legend has it that a humble fishing village 
        disguises the magical portal. */
        public override object Uncomplete
        {
            get
            {
                return 1072980;
            }
        }
        public override bool CanOffer()
        {
            return MondainsLegacy.Citadel;
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

        private class InternalObjective : SlayObjective
        {
            public InternalObjective()
                : base(typeof(SerpentsFangAssassin), "male serpent's fang assassins", 10, "TheCitadel")
            {
            }

            public override bool IsObjective(Mobile mob)
            {
                if (mob.Female)
                    return false;
					
                return base.IsObjective(mob);
            }
        }
    }

    public class ExtinguishingTheFlameQuest : BaseQuest
    { 
        public ExtinguishingTheFlameQuest()
            : base()
        { 
            this.AddObjective(new InternalObjective());
			
            this.AddReward(new BaseReward(typeof(TreasureBag), 1072583));
        }

        /* Extinguishing the Flame */
        public override object Title
        {
            get
            {
                return 1072911;
            }
        }
        /* The Dragon's Flame sect members have gone too far! Express to them my displeasure by slaying ten 
        of them. But remember, I do not condone war on women, so I will only accept the deaths of men, 
        human or elf.  Either race will do, I care not for the shape of their ears. Yes, this action will 
        properly make clear my disapproval and has a pleasing harmony. */
        public override object Description
        {
            get
            {
                return 1072966;
            }
        }
        /* As you wish. */
        public override object Refuse
        {
            get
            {
                return 1072979;
            }
        }
        /* The Black Order's fortress home is well hidden.  Legend has it that a humble fishing village 
        disguises the magical portal. */
        public override object Uncomplete
        {
            get
            {
                return 1072980;
            }
        }
        public override bool CanOffer()
        {
            return MondainsLegacy.Citadel;
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

        private class InternalObjective : SlayObjective
        {
            public InternalObjective()
                : base(typeof(DragonsFlameMage), "male dragon's flame mages", 10, "TheCitadel")
            {
            }

            public override bool IsObjective(Mobile mob)
            {
                if (mob.Female)
                    return false;
					
                return base.IsObjective(mob);
            }
        }
    }

    public class DeathToTheNinjaQuest : BaseQuest
    { 
        public DeathToTheNinjaQuest()
            : base()
        { 
            this.AddObjective(new SlayObjective(typeof(EliteNinja), "elite ninjas", 10, "TheCitadel"));
			
            this.AddReward(new BaseReward(typeof(TreasureBag), 1072583));
        }

        /* Death to the Ninja! */
        public override object Title
        {
            get
            {
                return 1072913;
            }
        }
        /* I wish to make a statement of censure against the elite ninjas of the Black Order.  Deliver, in 
        the strongest manner, my disdain.  But do not make war on women, even those that take arms against 
        you.  It is not ... fitting. */
        public override object Description
        {
            get
            {
                return 1072967;
            }
        }
        /* As you wish. */
        public override object Refuse
        {
            get
            {
                return 1072979;
            }
        }
        /* The Black Order's fortress home is well hidden.  Legend has it that a humble fishing village 
        disguises the magical portal. */
        public override object Uncomplete
        {
            get
            {
                return 1072980;
            }
        }
        public override bool CanOffer()
        {
            return MondainsLegacy.Citadel;
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

    public class CrimeAndPunishmentQuest : BaseQuest
    { 
        public CrimeAndPunishmentQuest()
            : base()
        { 
            this.AddObjective(new InternalObjective());
			
            this.AddReward(new BaseReward(typeof(TreasureBag), 1072583));
        }

        /* Crime and Punishment */
        public override object Title
        {
            get
            {
                return 1072914;
            }
        }
        /* The Tiger's Claw sect members have gone too far! Express to them my displeasure by slaying 
        ten of them. But remember, I do not condone war on women, so I will only accept the deaths of 
        men, human and elf. */
        public override object Description
        {
            get
            {
                return 1072968;
            }
        }
        /* As you wish. */
        public override object Refuse
        {
            get
            {
                return 1072979;
            }
        }
        /* The Black Order's fortress home is well hidden.  Legend has it that a humble fishing village 
        disguises the magical portal. */
        public override object Uncomplete
        {
            get
            {
                return 1072980;
            }
        }
        public override bool CanOffer()
        {
            return MondainsLegacy.Citadel;
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

        private class InternalObjective : SlayObjective
        {
            public InternalObjective()
                : base(typeof(TigersClawThief), "male tiger's claw thieves", 10, "TheCitadel")
            {
            }

            public override bool IsObjective(Mobile mob)
            {
                if (mob.Female)
                    return false;
					
                return base.IsObjective(mob);
            }
        }
    }

    public class Acob : MondainQuester
    { 
        [Constructable]
        public Acob()
            : base("Elder Acob", "the wise")
        { 
            this.SetSkill(SkillName.Meditation, 60.0, 83.0);
            this.SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Acob(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(OverpopulationQuest),
                    typeof(WildBoarCullQuest),
                    typeof(NewLeadershipQuest),
                    typeof(ExAssassinsQuest),
                    typeof(ExtinguishingTheFlameQuest),
                    typeof(DeathToTheNinjaQuest),
                    typeof(CrimeAndPunishmentQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.Race = Race.Elf;
			
            this.Hue = 0x8389;
            this.HairItemID = 0x2FCF;
            this.HairHue = 0x389;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());
            this.AddItem(new ElvenBoots(0x73D));
            this.AddItem(new HidePants());
            this.AddItem(new ElvenShirt(0x71));
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