using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Necro
{
    public class AnimateMaabusCorpseObjective : QuestObjective
    {
        private static readonly QuestItemInfo[] m_Info = new QuestItemInfo[]
        {
            new QuestItemInfo(1023643, 8787)// spellbook
        };
        public AnimateMaabusCorpseObjective()
        {
        }

        public override object Message
        {
            get
            {
                /* Re-animate the corpse of Maabus using your <a href="?ForceTopic112">Animate Dead</a>
                * spell and question him about the Kronus rituals.
                */
                return 1060102;
            }
        }
        public override QuestItemInfo[] Info
        {
            get
            {
                return m_Info;
            }
        }
        public override void OnComplete()
        {
            this.System.AddConversation(new MaabasConversation());
        }
    }

    public class FindCrystalCaveObjective : QuestObjective
    {
        public FindCrystalCaveObjective()
        {
        }

        public override object Message
        {
            get
            {
                /* Take the teleporter in the corner of Maabus' tomb to
                * the crystal cave where the calling scroll is kept.
                */
                return 1060104;
            }
        }
        public override void OnComplete()
        {
            this.System.AddConversation(new HorusConversation());
        }
    }

    public class FindMardothAboutVaultObjective : QuestObjective
    {
        public FindMardothAboutVaultObjective()
        {
        }

        public override object Message
        {
            get
            {
                /* Infiltrate the city of the Paladins and figure out a way into
                * the Vault. See Mardoth for help with this objective.
                */
                return 1060106;
            }
        }
        public override void OnComplete()
        {
            this.System.AddConversation(new MardothVaultConversation());
        }
    }

    public class FindMaabusTombObjective : QuestObjective
    {
        public FindMaabusTombObjective()
        {
        }

        public override object Message
        {
            get
            {
                /* Step onto the teleporter near Mardoth and follow the path
                * of glowing runes to the tomb of Maabus.
                */
                return 1060124;
            }
        }
        public override void CheckProgress()
        {
            if (this.System.From.Map == Map.Malas && this.System.From.InRange(new Point3D(2024, 1240, -90), 3))
                this.Complete();
        }

        public override void OnComplete()
        {
            this.System.AddObjective(new FindMaabusCorpseObjective());
        }
    }

    public class FindMaabusCorpseObjective : QuestObjective
    {
        public FindMaabusCorpseObjective()
        {
        }

        public override object Message
        {
            get
            {
                /* This is the tomb of Maabus.  Enter within and find
                * the corpse of the ancient necromancer.
                */
                return 1061142;
            }
        }
        public override void CheckProgress()
        {
            if (this.System.From.Map == Map.Malas && this.System.From.InRange(new Point3D(2024, 1223, -90), 3))
                this.Complete();
        }

        public override void OnComplete()
        {
            this.System.AddObjective(new AnimateMaabusCorpseObjective());
        }
    }

    public class FindCityOfLightObjective : QuestObjective
    {
        public FindCityOfLightObjective()
        {
        }

        public override object Message
        {
            get
            {
                /* Use the teleporter near Mardoth to be transported
                * to the Paladin City of Light.
                */
                return 1060108;
            }
        }
        public override void CheckProgress()
        {
            if (this.System.From.Map == Map.Malas && this.System.From.InRange(new Point3D(1076, 519, -90), 5))
                this.Complete();
        }

        public override void OnComplete()
        {
            this.System.AddObjective(new FindVaultOfSecretsObjective());
        }
    }

    public class FindVaultOfSecretsObjective : QuestObjective
    {
        private static readonly QuestItemInfo[] m_Info = new QuestItemInfo[]
        {
            new QuestItemInfo(1023676, 3679)// glowing rune
        };
        public FindVaultOfSecretsObjective()
        {
        }

        public override object Message
        {
            get
            {
                /* Follow the road paved with glowing runes to
                * find the Vault of Secrets.  Be careful not
                * to give yourself away as a Necromancer while
                * in the city.
                */
                return 1060109;
            }
        }
        public override QuestItemInfo[] Info
        {
            get
            {
                return m_Info;
            }
        }
        public override void CheckProgress()
        {
            if (this.System.From.Map == Map.Malas && this.System.From.InRange(new Point3D(1072, 455, -90), 1))
                this.Complete();
        }

        public override void OnComplete()
        {
            this.System.AddConversation(new VaultOfSecretsConversation());
        }
    }

    public class FetchAbraxusScrollObjective : QuestObjective
    {
        public FetchAbraxusScrollObjective()
        {
        }

        public override object Message
        {
            get
            {
                // <a href="?ForceTopic127">Summon your Horde Minion familiar</a> to fetch the scroll for you.
                return 1060196;
            }
        }
        public override void CheckProgress()
        {
            if (this.System.From.Map == Map.Malas && this.System.From.InRange(new Point3D(1076, 450, -84), 5))
            {
                HordeMinionFamiliar hmf = Spells.Necromancy.SummonFamiliarSpell.Table[this.System.From] as HordeMinionFamiliar;

                if (hmf != null && hmf.InRange(this.System.From, 5) && !hmf.QuestOverride)
                {
                    this.System.From.SendLocalizedMessage(1060113); // You instinctively will your familiar to fetch the scroll for you.
                    //hmf.TargetLocation = new Point2D(1076, 450);

                    if (hmf.AIObject != null)
                    {
                        hmf.CurrentSpeed = .2;
                        hmf.QuestOverride = true;
                        hmf.AIObject.MoveTo(new Point3D(1076, 450, -89), false, 0);
                    }
                    
                }
            }
        }

        public override void OnComplete()
        {
            this.System.AddObjective(new RetrieveAbraxusScrollObjective());
        }
    }

    public class RetrieveAbraxusScrollObjective : QuestObjective
    {
        public RetrieveAbraxusScrollObjective()
        {
        }

        public override object Message
        {
            get
            {
                /* Double click your Horde Minion to open his pack and retrieve
                * the Scroll of Abraxus that he looted for you.
                */
                return 1060199;
            }
        }
        public override void OnComplete()
        {
            this.System.AddConversation(new ReadAbraxusScrollConversation());
        }
    }

    public class ReadAbraxusScrollObjective : QuestObjective
    {
        public ReadAbraxusScrollObjective()
        {
        }

        public override object Message
        {
            get
            {
                /* Find the Crystal Cave password by reading (double click)
                * the golden scroll entitled "Scroll of Abraxus" that you
                * got from your familiar..
                */
                return 1060125;
            }
        }
        public override void OnComplete()
        {
            this.System.AddObjective(new ReturnToCrystalCaveObjective());
        }
    }

    public class ReturnToCrystalCaveObjective : QuestObjective
    {
        private static readonly QuestItemInfo[] m_Info = new QuestItemInfo[]
        {
            new QuestItemInfo(1026153, 6178)// teleporter
        };
        public ReturnToCrystalCaveObjective()
        {
        }

        public override object Message
        {
            get
            {
                /* Now that you have the password, return to the Crystal Cave
                * to speak with the guard there.  Use the teleporter outside
                * of the vault to get there if necessary.
                */
                return 1060115;
            }
        }
        public override QuestItemInfo[] Info
        {
            get
            {
                return m_Info;
            }
        }
        public override void OnComplete()
        {
            this.System.AddObjective(new SpeakCavePasswordObjective());
        }
    }

    public class SpeakCavePasswordObjective : QuestObjective
    {
        public SpeakCavePasswordObjective()
        {
        }

        public override object Message
        {
            get
            {
                /* Speak the secret word that you read in the scroll
                * stolen from the Vault to Horus the guard, using
                * his <a href="?ForceTopic90">context menu</a>.
                */
                return 1060117;
            }
        }
        public override void OnComplete()
        {
            this.System.AddConversation(new SecondHorusConversation());
        }
    }

    public class FindCallingScrollObjective : QuestObjective
    {
        private int m_SkitteringHoppersKilled;
        private bool m_HealConversationShown;
        private bool m_SkitteringHoppersDisposed;
        public FindCallingScrollObjective()
        {
        }

        public override object Message
        {
            get
            {
                /* Enter the Crystal Cave and find the Scroll of Calling.
                * The barrier will now allow you to pass.
                */
                return 1060119;
            }
        }
        public override bool IgnoreYoungProtection(Mobile from)
        {
            return !this.m_SkitteringHoppersDisposed && from is SkitteringHopper;
        }

        public override bool GetKillEvent(BaseCreature creature, Container corpse)
        {
            return !this.m_SkitteringHoppersDisposed;
        }

        public override void OnKill(BaseCreature creature, Container corpse)
        {
            if (creature is SkitteringHopper)
            {
                if (!this.m_HealConversationShown)
                {
                    this.m_HealConversationShown = true;
                    this.System.AddConversation(new HealConversation());
                }

                if (++this.m_SkitteringHoppersKilled >= 5)
                {
                    this.m_SkitteringHoppersDisposed = true;
                    this.System.AddObjective(new FindHorusAboutRewardObjective());
                }
            }
        }

        public override void OnComplete()
        {
            this.System.AddObjective(new FindMardothAboutKronusObjective());
        }

        public override void ChildDeserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            this.m_SkitteringHoppersKilled = reader.ReadEncodedInt();
            this.m_HealConversationShown = reader.ReadBool();
            this.m_SkitteringHoppersDisposed = reader.ReadBool();
        }

        public override void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)0); // version

            writer.WriteEncodedInt((int)this.m_SkitteringHoppersKilled);
            writer.Write((bool)this.m_HealConversationShown);
            writer.Write((bool)this.m_SkitteringHoppersDisposed);
        }
    }

    public class FindHorusAboutRewardObjective : QuestObjective
    {
        public FindHorusAboutRewardObjective()
        {
        }

        public override object Message
        {
            get
            {
                /* You have disposed of the creatures as Horus has asked.
                * See him on your way out of the Crystal Cave to claim your reward.
                */
                return 1060126;
            }
        }
        public override void OnComplete()
        {
            this.System.AddConversation(new HorusRewardConversation());
        }
    }

    public class FindMardothAboutKronusObjective : QuestObjective
    {
        public FindMardothAboutKronusObjective()
        {
        }

        public override object Message
        {
            get
            {
                /* You have obtained the scroll of calling. See Mardoth
                * for further instructions.
                */
                return 1060127;
            }
        }
        public override void OnComplete()
        {
            this.System.AddConversation(new MardothKronusConversation());
        }
    }

    public class FindWellOfTearsObjective : QuestObjective
    {
        private static readonly Rectangle2D m_WellOfTearsArea = new Rectangle2D(2080, 1346, 10, 10);
        private bool m_Inside;
        public FindWellOfTearsObjective()
        {
        }

        public override object Message
        {
            get
            {
                /* Follow the red lanterns to the Well of Tears where
                * you will perform the calling of Kronus.
                */
                return 1060128;
            }
        }
        public override void CheckProgress()
        {
            if (this.System.From.Map == Map.Malas && m_WellOfTearsArea.Contains(this.System.From.Location))
            {
                if (DarkTidesQuest.HasLostCallingScroll(this.System.From))
                {
                    if (!this.m_Inside)
                        this.System.AddConversation(new LostCallingScrollConversation(false));
                }
                else
                {
                    this.Complete();
                }

                this.m_Inside = true;
            }
            else
            {
                this.m_Inside = false;
            }
        }

        public override void OnComplete()
        {
            this.System.AddObjective(new UseCallingScrollObjective());
        }
    }

    public class UseCallingScrollObjective : QuestObjective
    {
        public UseCallingScrollObjective()
        {
        }

        public override object Message
        {
            get
            {
                /* Use the Scroll of Calling (double click) near the
                * Well of Tears to charge the waters for the arrival
                * of Kronus.
                */
                return 1060130;
            }
        }
    }

    public class FindMardothEndObjective : QuestObjective
    {
        private bool m_Victory;
        public FindMardothEndObjective(bool victory)
        {
            this.m_Victory = victory;
        }

        // Serialization
        public FindMardothEndObjective()
        {
        }

        public override object Message
        {
            get
            {
                if (this.m_Victory)
                {
                    /* Victory! You have done as Mardoth has asked of you.
                    * Take as much of your foe's loot as you can carry
                    * and return to Mardoth for your reward.
                    */
                    return 1060131;
                }
                else
                {
                    /* Although you were slain by the cowardly paladin,
                    * you managed to complete the rite of calling as
                    * instructed. Return to Mardoth.
                    */
                    return 1060132;
                }
            }
        }
        public override void OnComplete()
        {
            this.System.AddConversation(new MardothEndConversation());
        }

        public override void ChildDeserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            this.m_Victory = reader.ReadBool();
        }

        public override void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)0); // version

            writer.Write((bool)this.m_Victory);
        }
    }

    public class FindBankObjective : QuestObjective
    {
        public FindBankObjective()
        {
        }

        public override object Message
        {
            get
            {
                /* Use the enchanted sextant in your pack to locate
                * the nearest bank.  Go there and speak with the
                * Banker.
                */
                return 1060134;
            }
        }
        public override void CheckProgress()
        {
            if (this.System.From.Map == Map.Malas && this.System.From.InRange(new Point3D(2048, 1345, -84), 5))
                this.Complete();
        }

        public override void OnComplete()
        {
            this.System.AddObjective(new CashBankCheckObjective());
        }
    }

    public class CashBankCheckObjective : QuestObjective
    {
        public CashBankCheckObjective()
        {
        }

        public override object Message
        {
            get
            {
                /* You have arrived at the Bank. <a href="?ForceTopic38">Open your bank box</a>
                * and then <a href="?ForceTopic86">cash the check</a> that Mardoth gave you.
                */
                return 1060644;
            }
        }
        public override void OnComplete()
        {
            this.System.AddConversation(new BankerConversation());
        }
    }
}