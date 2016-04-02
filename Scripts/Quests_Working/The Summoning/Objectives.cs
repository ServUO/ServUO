using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Doom
{
    public class CollectBonesObjective : QuestObjective
    {
        public CollectBonesObjective()
        {
        }

        public override object Message
        {
            get
            {
                /* Find 1000 Daemon bones and hand them
                * to Victoria as you find them.
                */
                return 1050026; 
            }
        }
        public override int MaxProgress
        {
            get
            {
                return 1000;
            }
        }
        public override void OnComplete()
        {
            Victoria victoria = ((TheSummoningQuest)this.System).Victoria;

            if (victoria == null)
            {
                this.System.From.SendMessage("Internal error: unable to find Victoria. Quest unable to continue.");
                this.System.Cancel();
            }
            else
            {
                SummoningAltar altar = victoria.Altar;

                if (altar == null)
                {
                    this.System.From.SendMessage("Internal error: unable to find summoning altar. Quest unable to continue.");
                    this.System.Cancel();
                }
                else if (altar.Daemon == null || !altar.Daemon.Alive)
                {
                    this.System.AddConversation(new VanquishDaemonConversation());
                }
                else
                {
                    victoria.SayTo(this.System.From, "The devourer has already been summoned. Return when the devourer has been slain and I will summon it for you.");
                    ((TheSummoningQuest)this.System).WaitForSummon = true;
                }
            }
        }

        public override void RenderMessage(BaseQuestGump gump)
        {
            if (this.CurProgress > 0 && this.CurProgress < this.MaxProgress)
                gump.AddHtmlObject(70, 130, 300, 100, 1050028, BaseQuestGump.Blue, false, false); // Victoria has accepted the Daemon bones, but the requirement is not yet met.
            else
                base.RenderMessage(gump);
        }

        public override void RenderProgress(BaseQuestGump gump)
        {
            if (this.CurProgress > 0 && this.CurProgress < this.MaxProgress)
            {
                gump.AddHtmlObject(70, 260, 270, 100, 1050019, BaseQuestGump.Blue, false, false); // Number of bones collected:

                gump.AddLabel(70, 280, 100, this.CurProgress.ToString());
                gump.AddLabel(100, 280, 100, "/");
                gump.AddLabel(130, 280, 100, this.MaxProgress.ToString());
            }
            else
            {
                base.RenderProgress(gump);
            }
        }
    }

    public class VanquishDaemonObjective : QuestObjective
    {
        private BoneDemon m_Daemon;
        private Corpse m_CorpseWithSkull;
        public VanquishDaemonObjective(BoneDemon daemon)
        {
            this.m_Daemon = daemon;
        }

        // Serialization
        public VanquishDaemonObjective()
        {
        }

        public Corpse CorpseWithSkull
        {
            get
            {
                return this.m_CorpseWithSkull;
            }
            set
            {
                this.m_CorpseWithSkull = value;
            }
        }
        public override object Message
        {
            get
            {
                /* Go forth and vanquish the devourer that has been summoned!
                */
                return 1050037;
            }
        }
        public override void CheckProgress()
        {
            if (this.m_Daemon == null || !this.m_Daemon.Alive)
                this.Complete();
        }

        public override void OnComplete()
        {
            Victoria victoria = ((TheSummoningQuest)this.System).Victoria;

            if (victoria != null)
            {
                SummoningAltar altar = victoria.Altar;

                if (altar != null)
                    altar.CheckDaemon();
            }

            PlayerMobile from = this.System.From;

            if (!from.Alive)
            {
                from.SendLocalizedMessage(1050033); // The devourer lies dead, unfortunately so do you.  You cannot claim your reward while dead.  You will need to face him again.
                ((TheSummoningQuest)this.System).WaitForSummon = true;
            }
            else
            {
                bool hasRights = true;

                if (this.m_Daemon != null)
                {
                    List<DamageStore> lootingRights = BaseCreature.GetLootingRights(this.m_Daemon.DamageEntries, this.m_Daemon.HitsMax);

                    for (int i = 0; i < lootingRights.Count; ++i)
                    {
                        DamageStore ds = lootingRights[i];

                        if (ds.m_HasRight && ds.m_Mobile == from)
                        {
                            hasRights = true;
                            break;
                        }
                    }
                }

                if (!hasRights)
                {
                    from.SendLocalizedMessage(1050034); // The devourer lies dead.  Unfortunately you did not sufficiently prove your worth in combating the devourer.  Victoria shall summon another incarnation of the devourer to the circle of stones.  Try again noble adventurer.
                    ((TheSummoningQuest)this.System).WaitForSummon = true;
                }
                else
                {
                    from.SendLocalizedMessage(1050035); // The devourer lies dead.  Search his corpse to claim your prize!

                    if (this.m_Daemon != null)
                        this.m_CorpseWithSkull = this.m_Daemon.Corpse as Corpse;
                }
            }
        }

        public override void ChildDeserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            this.m_Daemon = reader.ReadMobile() as BoneDemon;
            this.m_CorpseWithSkull = reader.ReadItem() as Corpse;
        }

        public override void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)0); // version

            writer.Write((Mobile)this.m_Daemon);
            writer.Write((Item)this.m_CorpseWithSkull);
        }
    }
}