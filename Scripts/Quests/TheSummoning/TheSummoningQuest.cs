using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Doom
{
    public class TheSummoningQuest : QuestSystem
    {
        private static readonly Type[] m_TypeReferenceTable = new Type[]
        {
            typeof(Doom.AcceptConversation),
            typeof(Doom.CollectBonesObjective),
            typeof(Doom.VanquishDaemonConversation),
            typeof(Doom.VanquishDaemonObjective)
        };
        private Victoria m_Victoria;
        private bool m_WaitForSummon;
        public TheSummoningQuest(Victoria victoria, PlayerMobile from)
            : base(from)
        {
            this.m_Victoria = victoria;
        }

        public TheSummoningQuest()
        {
        }

        public override Type[] TypeReferenceTable
        {
            get
            {
                return m_TypeReferenceTable;
            }
        }
        public Victoria Victoria
        {
            get
            {
                return this.m_Victoria;
            }
        }
        public bool WaitForSummon
        {
            get
            {
                return this.m_WaitForSummon;
            }
            set
            {
                this.m_WaitForSummon = value;
            }
        }
        public override object Name
        {
            get
            {
                // The Summoning
                return 1050025;
            }
        }
        public override object OfferMessage
        {
            get
            {
                /* <I>Victoria turns to you and smiles...</I><BR><BR>
                * 
                * Chyloth, eh?  He is the ferry man of lake <I>Mortis</I>, beyond which lies
                * the nest of the <I>The Dark Father</I> - the fountainhead of all the evil
                * that you see around you here.<BR><BR>
                * 
                * 800 and some years ago, my sisters and I persuaded the ferry man Chyloth
                * to take us across the lake to battle the <I>The Dark Father</I>.
                * My party was utterly destroyed, except for me.  For my insolence, I was
                * cursed by the <I>The Dark Father</I> to wander these halls for eternity,
                * unable to die - unable to leave.<BR><BR>
                * 
                * Chyloth usually only crosses over the souls of the undead, but he can be
                * persuaded otherwise...with a token of gold, in the form of a human skull.
                * Such a gem can be found only in the belly of the hellspawn known as
                * <I>the devourer</I>.<BR><BR>
                * 
                * I can help you summon the beast from the depths of the abyss, but I require
                * 1000 Daemon bones to do so.  If you accept my help, I will store the Daemon
                * bones for you until you have collected all 1000 of them.  Once the bones
                * are collected in full, I will summon the beast for you, which you must
                * slay to claim your prize.<BR><BR>
                * 
                * Do you accept?
                */
                return 1050020;
            }
        }
        public override bool IsTutorial
        {
            get
            {
                return false;
            }
        }
        public override TimeSpan RestartDelay
        {
            get
            {
                return TimeSpan.Zero;
            }
        }
        public override int Picture
        {
            get
            {
                return 0x15B5;
            }
        }
        public static int GetDaemonBonesFor(BaseCreature creature)
        {
            if (creature == null || creature.Controlled || creature.Summoned)
                return 0;

            if (creature is DarkGuardian)
                return 30;

            int fame = creature.Fame;

            if (fame < 1500)
                return Utility.Dice(2, 5, -1);
            else if (fame < 20000)
                return Utility.Dice(2, 4, 8);
            else
                return 50;
        }

        // NOTE: Quest not entirely OSI-accurate: some changes made to prevent numerous OSI bugs
        public override void Slice()
        {
            if (this.m_WaitForSummon && this.m_Victoria != null)
            {
                SummoningAltar altar = this.m_Victoria.Altar;

                if (altar != null && (altar.Daemon == null || !altar.Daemon.Alive))
                {
                    if (this.From.Map == this.m_Victoria.Map && this.From.InRange(this.m_Victoria, 8))
                    {
                        this.m_WaitForSummon = false;

                        this.AddConversation(new VanquishDaemonConversation());
                    }
                }
            }

            base.Slice();
        }

        public override void Cancel()
        {
            base.Cancel();

            QuestObjective obj = this.FindObjective(typeof(CollectBonesObjective));

            if (obj != null && obj.CurProgress > 0)
            {
                this.From.BankBox.DropItem(new DaemonBone(obj.CurProgress));

                this.From.SendLocalizedMessage(1050030); // The Daemon bones that you have thus far given to Victoria have been returned to you.
            }
        }

        public override void Accept()
        {
            base.Accept();

            this.AddConversation(new AcceptConversation());
        }

        public override void ChildDeserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            this.m_Victoria = reader.ReadMobile() as Victoria;
            this.m_WaitForSummon = reader.ReadBool();
        }

        public override void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)0); // version

            writer.Write((Mobile)this.m_Victoria);
            writer.Write((bool)this.m_WaitForSummon);
        }
    }
}