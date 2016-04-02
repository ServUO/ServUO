using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Samurai
{
    public class Haochi : BaseQuester
    {
        [Constructable]
        public Haochi()
            : base("the Honorable Samurai Legend")
        {
        }

        public Haochi(Serial serial)
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

            this.Hue = 0x8403;

            this.Female = false;
            this.Body = 0x190;
            this.Name = "Daimyo Haochi";
        }

        public override void InitOutfit()
        {
            this.HairItemID = 0x204A;
            this.HairHue = 0x901;

            this.AddItem(new SamuraiTabi());
            this.AddItem(new JinBaori());

            this.AddItem(new PlateHaidate());
            this.AddItem(new StandardPlateKabuto());
            this.AddItem(new PlateMempo());
            this.AddItem(new PlateDo());
            this.AddItem(new PlateHiroSode());
        }

        public override int GetAutoTalkRange(PlayerMobile pm)
        {
            return 2;
        }

        public override void OnTalk(PlayerMobile player, bool contextMenu)
        {
            QuestSystem qs = player.Quest;

            if (qs is HaochisTrialsQuest)
            {
                if (HaochisTrialsQuest.HasLostHaochisKatana(player))
                {
                    qs.AddConversation(new LostSwordConversation());
                    return;
                }

                QuestObjective obj = qs.FindObjective(typeof(FindHaochiObjective));

                if (obj != null && !obj.Completed)
                {
                    obj.Complete();
                    return;
                }

                obj = qs.FindObjective(typeof(FirstTrialReturnObjective));

                if (obj != null && !obj.Completed)
                {
                    player.AddToBackpack(new LeatherDo());
                    obj.Complete();
                    return;
                }

                obj = qs.FindObjective(typeof(SecondTrialReturnObjective));

                if (obj != null && !obj.Completed)
                {
                    if (((SecondTrialReturnObjective)obj).Dragon)
                        player.AddToBackpack(new LeatherSuneate());

                    obj.Complete();
                    return;
                }

                obj = qs.FindObjective(typeof(ThirdTrialReturnObjective));

                if (obj != null && !obj.Completed)
                {
                    player.AddToBackpack(new LeatherHiroSode());
                    obj.Complete();
                    return;
                }

                obj = qs.FindObjective(typeof(FourthTrialReturnObjective));

                if (obj != null && !obj.Completed)
                {
                    if (!((FourthTrialReturnObjective)obj).KilledCat)
                    {
                        Container cont = GetNewContainer();
                        cont.DropItem(new LeatherHiroSode());
                        cont.DropItem(new JinBaori());
                        player.AddToBackpack(cont);
                    }

                    obj.Complete();
                    return;
                }

                obj = qs.FindObjective(typeof(FifthTrialReturnObjective));

                if (obj != null && !obj.Completed)
                {
                    Container pack = player.Backpack;
                    if (pack != null)
                    {
                        Item katana = pack.FindItemByType(typeof(HaochisKatana));
                        if (katana != null)
                        {
                            katana.Delete();
                            obj.Complete();

                            obj = qs.FindObjective(typeof(FifthTrialIntroObjective));
                            if (obj != null && ((FifthTrialIntroObjective)obj).StolenTreasure)
                                qs.AddConversation(new SixthTrialIntroConversation(true));
                            else
                                qs.AddConversation(new SixthTrialIntroConversation(false));
                        }
                    }

                    return;
                }

                obj = qs.FindObjective(typeof(SixthTrialReturnObjective));

                if (obj != null && !obj.Completed)
                {
                    obj.Complete();
                    return;
                }

                obj = qs.FindObjective(typeof(SeventhTrialReturnObjective));

                if (obj != null && !obj.Completed)
                {
                    BaseWeapon weapon = new Daisho();
                    BaseRunicTool.ApplyAttributesTo(weapon, Utility.Random(1, 3), 10, 30);
                    player.AddToBackpack(weapon);

                    BaseArmor armor = new LeatherDo();
                    BaseRunicTool.ApplyAttributesTo(armor, Utility.Random(1, 3), 10, 20);
                    player.AddToBackpack(armor);

                    obj.Complete();
                    return;
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