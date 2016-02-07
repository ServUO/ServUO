using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class FirendOfTheFeyQuest : BaseQuest
    { 
        public FirendOfTheFeyQuest()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(Beads), "beads", 1, 0x108B));
            this.AddObjective(new ObtainObjective(typeof(JarHoney), "jar of honey", 1, 0x9EC));
			
            this.AddReward(new BaseReward(1074874)); // The opportunity to prove yourself worthy of learning to Summon Fey. (Sufficient spellweaving skill is required to cast the spell)
        }

        public override QuestChain ChainID
        {
            get
            {
                return QuestChain.SummonFey;
            }
        }
        public override Type NextQuest
        {
            get
            {
                return typeof(TokenOfFriendshipQuest);
            }
        }
        /* Friend of the Fey */
        public override object Title
        {
            get
            {
                return 1074284;
            }
        }
        /* The children of Sosaria understand the dedication and committment of an 
        arcanist -- and will, from time to time offer their friendship. If you would 
        forge such a bond, first seek out a goodwill offering to present.  Pixies 
        enjoy sweets and pretty things. */
        public override object Description
        {
            get
            {
                return 1074286;
            }
        }
        /* There's always time to make new friends. */
        public override object Refuse
        {
            get
            {
                return 1074288;
            }
        }
        /* I think honey and some sparkly beads would please a pixie. */
        public override object Uncomplete
        {
            get
            {
                return 1074290;
            }
        }
        /* What have we here? Oh yes, gifts for a pixie. */
        public override object Complete
        {
            get
            {
                return 1074292;
            }
        }
        public override bool CanOffer()
        {
            return MondainsLegacy.Spellweaving;
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

    public class TokenOfFriendshipQuest : BaseQuest
    { 
        public TokenOfFriendshipQuest()
            : base()
        { 
            this.AddObjective(new DeliverObjective(typeof(GiftForArielle), "gift for Arielle", 1, typeof(Arielle), "Arielle"));
			
            this.AddReward(new BaseReward(1074874)); // The opportunity to prove yourself worthy of learning to Summon Fey. (Sufficient spellweaving skill is required to cast the spell)
        }

        public override QuestChain ChainID
        {
            get
            {
                return QuestChain.SummonFey;
            }
        }
        public override Type NextQuest
        {
            get
            {
                return typeof(AllianceQuest);
            }
        }
        /* Token of Friendship */
        public override object Title
        {
            get
            {
                return 1074293;
            }
        }
        /* I've wrapped your gift suitably to present to a pixie of discriminating 
        taste.  Seek out Arielle and give her your offering. */
        public override object Description
        {
            get
            {
                return 1074297;
            }
        }
        /* I'll hold onto this gift in case you change your mind. */
        public override object Refuse
        {
            get
            {
                return 1074310;
            }
        }
        /* Arielle wanders quite a bit, so I'm not sure exactly where to find her.  
        I'm sure she's going to love your gift. */
        public override object Uncomplete
        {
            get
            {
                return 1074315;
            }
        }
        /* *giggle*  Oooh!  For me? */
        public override object Complete
        {
            get
            {
                return 1074319;
            }
        }
        public override bool CanOffer()
        {
            return MondainsLegacy.Spellweaving;
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

    public class AllianceQuest : BaseQuest
    { 
        public AllianceQuest()
            : base()
        { 
            this.AddObjective(new SlayObjective(typeof(Reaper), "reapers", 20));
			
            this.AddReward(new BaseReward(typeof(SummonFeyScroll), 1071032)); // Summon Fey
        }

        public override QuestChain ChainID
        {
            get
            {
                return QuestChain.SummonFey;
            }
        }
        /* Alliance */
        public override object Title
        {
            get
            {
                return 1074294;
            }
        }
        /* *giggle* Mean reapers make pixies unhappy. *light-hearted giggle*  You 
        could fix them! */
        public override object Description
        {
            get
            {
                return 1074298;
            }
        }
        /* *giggle* Okies! */
        public override object Refuse
        {
            get
            {
                return 1074311;
            }
        }
        /* Mean reapers are all around trees!  *giggle*  You fix them up, please. */
        public override object Uncomplete
        {
            get
            {
                return 1074316;
            }
        }
        /* *giggle* Mean reapers got fixed!  Pixie friend now! *giggle* When mean 
        thingies bother you, a brave pixie will help. */
        public override object Complete
        {
            get
            {
                return 1074320;
            }
        }
        public override bool CanOffer()
        {
            return MondainsLegacy.Spellweaving;
        }

        public override void GiveRewards()
        { 
            /* *giggle* Mean reapers got fixed!  Pixie friend now! *giggle* When mean thingies 
            bother you, a brave pixie will help. */
            this.Owner.SendLocalizedMessage(1074320, null, 0x2A);
			
            base.GiveRewards();
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