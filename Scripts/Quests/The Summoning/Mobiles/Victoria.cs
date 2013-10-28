using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Doom
{
    public class Victoria : BaseQuester
    {
        private const int AltarRange = 24;
        private SummoningAltar m_Altar;
        [Constructable]
        public Victoria()
            : base("the Sorceress")
        {
        }

        public Victoria(Serial serial)
            : base(serial)
        {
        }

        public override int TalkNumber
        {
            get
            {
                return 6159;
            }
        }// Ask about Chyloth
        public override bool ClickTitle
        {
            get
            {
                return true;
            }
        }
        public override bool IsActiveVendor
        {
            get
            {
                return true;
            }
        }
        public override bool DisallowAllMoves
        {
            get
            {
                return false;
            }
        }
        public SummoningAltar Altar
        {
            get
            {
                if (this.m_Altar == null || this.m_Altar.Deleted || this.m_Altar.Map != this.Map || !Utility.InRange(this.m_Altar.Location, this.Location, AltarRange))
                {
                    foreach (Item item in this.GetItemsInRange(AltarRange))
                    {
                        if (item is SummoningAltar)
                        {
                            this.m_Altar = (SummoningAltar)item;
                            break;
                        }
                    }
                }

                return this.m_Altar;
            }
        }
        public override void InitSBInfo()
        {
            this.m_SBInfos.Add(new SBMage());
        }

        public override void InitBody()
        {
            this.InitStats(100, 100, 25);

            this.Female = true;
            this.Hue = 0x8835;
            this.Body = 0x191;

            this.Name = "Victoria";
        }

        public override void InitOutfit()
        {
            this.EquipItem(new GrandGrimoire());

            this.EquipItem(this.SetHue(new Sandals(), 0x455));
            this.EquipItem(this.SetHue(new SkullCap(), 0x455));
            this.EquipItem(this.SetHue(new PlainDress(), 0x455));

            this.HairItemID = 0x203C;
            this.HairHue = 0x482;
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player != null)
            {
                QuestSystem qs = player.Quest;

                if (qs is TheSummoningQuest)
                {
                    if (dropped is DaemonBone)
                    {
                        DaemonBone bones = (DaemonBone)dropped;

                        QuestObjective obj = qs.FindObjective(typeof(CollectBonesObjective));

                        if (obj != null && !obj.Completed)
                        {
                            int need = obj.MaxProgress - obj.CurProgress;

                            if (bones.Amount < need)
                            {
                                obj.CurProgress += bones.Amount;
                                bones.Delete();

                                qs.ShowQuestLogUpdated();
                            }
                            else
                            {
                                obj.Complete();
                                bones.Consume(need);

                                if (!bones.Deleted)
                                {
                                    // TODO: Accurate?
                                    this.SayTo(from, 1050038); // You have already given me all the Daemon bones necessary to weave the spell.  Keep these for a later time.
                                }
                            }
                        }
                        else
                        {
                            // TODO: Accurate?
                            this.SayTo(from, 1050038); // You have already given me all the Daemon bones necessary to weave the spell.  Keep these for a later time.
                        }

                        return false;
                    }
                }
            }

            return base.OnDragDrop(from, dropped);
        }

        public override bool CanTalkTo(PlayerMobile to)
        {
            return (to.Quest == null && QuestSystem.CanOfferQuest(to, typeof(TheSummoningQuest)));
        }

        public override void OnTalk(PlayerMobile player, bool contextMenu)
        {
            QuestSystem qs = player.Quest;

            if (qs == null && QuestSystem.CanOfferQuest(player, typeof(TheSummoningQuest)))
            {
                this.Direction = this.GetDirectionTo(player);
                new TheSummoningQuest(this, player).SendOffer();
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
}