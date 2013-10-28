using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Haven
{
    public class UzeraanTurmoilQuest : QuestSystem
    {
        private static readonly Type[] m_TypeReferenceTable = new Type[]
        {
            typeof(Haven.AcceptConversation),
            typeof(Haven.UzeraanTitheConversation),
            typeof(Haven.UzeraanFirstTaskConversation),
            typeof(Haven.UzeraanReportConversation),
            typeof(Haven.SchmendrickConversation),
            typeof(Haven.UzeraanScrollOfPowerConversation),
            typeof(Haven.DryadConversation),
            typeof(Haven.UzeraanFertileDirtConversation),
            typeof(Haven.UzeraanDaemonBloodConversation),
            typeof(Haven.UzeraanDaemonBoneConversation),
            typeof(Haven.BankerConversation),
            typeof(Haven.RadarConversation),
            typeof(Haven.LostScrollOfPowerConversation),
            typeof(Haven.LostFertileDirtConversation),
            typeof(Haven.DryadAppleConversation),
            typeof(Haven.LostDaemonBloodConversation),
            typeof(Haven.LostDaemonBoneConversation),
            typeof(Haven.FindUzeraanBeginObjective),
            typeof(Haven.TitheGoldObjective),
            typeof(Haven.FindUzeraanFirstTaskObjective),
            typeof(Haven.KillHordeMinionsObjective),
            typeof(Haven.FindUzeraanAboutReportObjective),
            typeof(Haven.FindSchmendrickObjective),
            typeof(Haven.FindApprenticeObjective),
            typeof(Haven.ReturnScrollOfPowerObjective),
            typeof(Haven.FindDryadObjective),
            typeof(Haven.ReturnFertileDirtObjective),
            typeof(Haven.GetDaemonBloodObjective),
            typeof(Haven.ReturnDaemonBloodObjective),
            typeof(Haven.GetDaemonBoneObjective),
            typeof(Haven.ReturnDaemonBoneObjective),
            typeof(Haven.CashBankCheckObjective),
            typeof(Haven.FewReagentsConversation)
        };
        private bool m_HasLeftTheMansion;
        public UzeraanTurmoilQuest(PlayerMobile from)
            : base(from)
        {
        }

        // Serialization
        public UzeraanTurmoilQuest()
        {
        }

        public override Type[] TypeReferenceTable
        {
            get
            {
                return m_TypeReferenceTable;
            }
        }
        public override object Name
        {
            get
            {
                // "Uzeraan's Turmoil"
                return 1049007;
            }
        }
        public override object OfferMessage
        {
            get
            {
                /* <I>The guard speaks to you as you come closer... </I><BR><BR>
                * 
                * Greetings traveler! <BR><BR>
                * 
                * Uzeraan, the lord of this house and overseer of this city -
                * <a href="?ForceTopic72">Haven</a>, has requested an audience with you. <BR><BR>
                * 
                * Hordes of gruesome hell spawn are beginning to overrun the
                * city and terrorize the inhabitants.  No one seems to be able
                * to stop them.<BR><BR>
                * 
                * Our fine city militia is falling to the evil creatures
                * one battalion after the other.<BR><BR>
                * 
                * Uzeraan, whom you can find through these doors, is looking to
                * hire mercenaries to aid in the battle. <BR><BR>
                * 
                * Will you assist us?
                */
                return 1049008;
            }
        }
        public override TimeSpan RestartDelay
        {
            get
            {
                return TimeSpan.MaxValue;
            }
        }
        public override bool IsTutorial
        {
            get
            {
                return true;
            }
        }
        public override int Picture
        {
            get
            {
                switch ( this.From.Profession )
                {
                    case 1:
                        return 0x15C9; // warrior
                    case 2:
                        return 0x15C1; // magician
                    default:
                        return 0x15D3; // paladin
                }
            }
        }
        public static bool HasLostScrollOfPower(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;

            if (pm == null)
                return false;

            QuestSystem qs = pm.Quest;

            if (qs is UzeraanTurmoilQuest)
            {
                if (qs.IsObjectiveInProgress(typeof(ReturnScrollOfPowerObjective)))
                {
                    Container pack = from.Backpack;

                    return (pack == null || pack.FindItemByType(typeof(SchmendrickScrollOfPower)) == null);
                }
            }

            return false;
        }

        public static bool HasLostFertileDirt(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;

            if (pm == null)
                return false;

            QuestSystem qs = pm.Quest;

            if (qs is UzeraanTurmoilQuest)
            {
                if (qs.IsObjectiveInProgress(typeof(ReturnFertileDirtObjective)))
                {
                    Container pack = from.Backpack;

                    return (pack == null || pack.FindItemByType(typeof(QuestFertileDirt)) == null);
                }
            }

            return false;
        }

        public static bool HasLostDaemonBlood(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;

            if (pm == null)
                return false;

            QuestSystem qs = pm.Quest;

            if (qs is UzeraanTurmoilQuest)
            {
                if (qs.IsObjectiveInProgress(typeof(ReturnDaemonBloodObjective)))
                {
                    Container pack = from.Backpack;

                    return (pack == null || pack.FindItemByType(typeof(QuestDaemonBlood)) == null);
                }
            }

            return false;
        }

        public static bool HasLostDaemonBone(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;

            if (pm == null)
                return false;

            QuestSystem qs = pm.Quest;

            if (qs is UzeraanTurmoilQuest)
            {
                if (qs.IsObjectiveInProgress(typeof(ReturnDaemonBoneObjective)))
                {
                    Container pack = from.Backpack;

                    return (pack == null || pack.FindItemByType(typeof(QuestDaemonBone)) == null);
                }
            }

            return false;
        }

        public override void Slice()
        {
            if (!this.m_HasLeftTheMansion && (this.From.Map != Map.Trammel || this.From.X < 3573 || this.From.X > 3611 || this.From.Y < 2568 || this.From.Y > 2606))
            {
                this.m_HasLeftTheMansion = true;
                this.AddConversation(new RadarConversation());
            }

            base.Slice();
        }

        public override void Accept()
        {
            base.Accept();

            this.AddConversation(new AcceptConversation());
        }

        public override void ChildDeserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            this.m_HasLeftTheMansion = reader.ReadBool();
        }

        public override void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)0); // version

            writer.Write((bool)this.m_HasLeftTheMansion);
        }
    }
}