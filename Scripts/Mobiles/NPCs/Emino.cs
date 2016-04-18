using System;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Ninja
{
    public class Emino : BaseQuester
    {
        [Constructable]
        public Emino()
            : base("the Notorious")
        {
        }

        public Emino(Serial serial)
            : base(serial)
        {
        }

        public override int TalkNumber
        {
            get
            {
                return -1;
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);

            this.Hue = 0x83FE;

            this.Female = false;
            this.Body = 0x190;
            this.Name = "Daimyo Emino";
        }

        public override void InitOutfit()
        {
            this.HairItemID = 0x203B;
            this.HairHue = 0x901;

            this.AddItem(new MaleKimono());
            this.AddItem(new SamuraiTabi());
            this.AddItem(new Bandana());

            this.AddItem(new PlateHaidate());
            this.AddItem(new PlateDo());
            this.AddItem(new PlateHiroSode());

            Nunchaku nunchaku = new Nunchaku();
            nunchaku.Movable = false;
            this.AddItem(nunchaku);
        }

        public override int GetAutoTalkRange(PlayerMobile pm)
        {
            return 2;
        }

        public override void OnTalk(PlayerMobile player, bool contextMenu)
        {
            QuestSystem qs = player.Quest;

            if (qs is EminosUndertakingQuest)
            {
                if (EminosUndertakingQuest.HasLostNoteForZoel(player))
                {
                    Item note = new NoteForZoel();

                    if (player.PlaceInBackpack(note))
                    {
                        qs.AddConversation(new LostNoteConversation());
                    }
                    else
                    {
                        note.Delete();
                        player.SendLocalizedMessage(1046260); // You need to clear some space in your inventory to continue with the quest.  Come back here when you have more space in your inventory.
                    }
                }
                else if (EminosUndertakingQuest.HasLostEminosKatana(player))
                {
                    qs.AddConversation(new LostSwordConversation());
                }
                else
                {
                    QuestObjective obj = qs.FindObjective(typeof(FindEminoBeginObjective));

                    if (obj != null && !obj.Completed)
                    {
                        obj.Complete();
                    }
                    else
                    {
                        obj = qs.FindObjective(typeof(UseTeleporterObjective));

                        if (obj != null && !obj.Completed)
                        {
                            Item note = new NoteForZoel();

                            if (player.PlaceInBackpack(note))
                            {
                                obj.Complete();

                                player.AddToBackpack(new LeatherNinjaPants());
                                player.AddToBackpack(new LeatherNinjaMitts());
                            }
                            else
                            {
                                note.Delete();
                                player.SendLocalizedMessage(1046260); // You need to clear some space in your inventory to continue with the quest.  Come back here when you have more space in your inventory.
                            }
                        }
                        else
                        {
                            obj = qs.FindObjective(typeof(ReturnFromInnObjective));

                            if (obj != null && !obj.Completed)
                            {
                                Container cont = GetNewContainer();

                                for (int i = 0; i < 10; i++)
                                    cont.DropItem(new LesserHealPotion());

                                cont.DropItem(new LeatherNinjaHood());
                                cont.DropItem(new LeatherNinjaJacket());

                                if (player.PlaceInBackpack(cont))
                                {
                                    obj.Complete();
                                }
                                else
                                {
                                    cont.Delete();
                                    player.SendLocalizedMessage(1046260); // You need to clear some space in your inventory to continue with the quest.  Come back here when you have more space in your inventory.
                                }
                            }
                            else
                            {
                                if (qs.IsObjectiveInProgress(typeof(SlayHenchmenObjective)))
                                {
                                    qs.AddConversation(new ContinueSlayHenchmenConversation());
                                }
                                else
                                {
                                    obj = qs.FindObjective(typeof(GiveEminoSwordObjective));

                                    if (obj != null && !obj.Completed)
                                    {
                                        Item katana = null;

                                        if (player.Backpack != null)
                                            katana = player.Backpack.FindItemByType(typeof(EminosKatana));

                                        if (katana != null)
                                        {
                                            bool stolenTreasure = false;

                                            HallwayWalkObjective walk = qs.FindObjective(typeof(HallwayWalkObjective)) as HallwayWalkObjective;
                                            if (walk != null)
                                                stolenTreasure = walk.StolenTreasure;

                                            Kama kama = new Kama();

                                            if (stolenTreasure)
                                                BaseRunicTool.ApplyAttributesTo(kama, 1, 10, 20);
                                            else
                                                BaseRunicTool.ApplyAttributesTo(kama, 1, 10, 30);

                                            if (player.PlaceInBackpack(kama))
                                            {
                                                katana.Delete();
                                                obj.Complete();

                                                if (stolenTreasure)
                                                    qs.AddConversation(new EarnLessGiftsConversation());
                                                else
                                                    qs.AddConversation(new EarnGiftsConversation());
                                            }
                                            else
                                            {
                                                kama.Delete();
                                                player.SendLocalizedMessage(1046260); // You need to clear some space in your inventory to continue with the quest.  Come back here when you have more space in your inventory.
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            base.OnMovement(m, oldLocation);

            if (!m.Frozen && !m.Alive && this.InRange(m, 4) && !this.InRange(oldLocation, 4) && this.InLOS(m))
            {
                if (m.Map == null || !m.Map.CanFit(m.Location, 16, false, false))
                {
                    m.SendLocalizedMessage(502391); // Thou can not be resurrected there!
                }
                else
                {
                    this.Direction = this.GetDirectionTo(m);

                    m.PlaySound(0x214);
                    m.FixedEffect(0x376A, 10, 16);

                    m.CloseGump(typeof(ResurrectGump));
                    m.SendGump(new ResurrectGump(m, ResurrectMessage.Healer));
                }
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