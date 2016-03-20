using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Zento
{
    public class AnsellaGryen : BaseQuester
    {
        [Constructable]
        public AnsellaGryen()
        {
        }

        public AnsellaGryen(Serial serial)
            : base(serial)
        {
        }

        public override void InitBody()
        {
            this.InitStats(100, 100, 25);

            this.Hue = 0x83EA;

            this.Female = true;
            this.Body = 0x191;
            this.Name = "Ansella Gryen";
        }

        public override void InitOutfit()
        {
            this.HairItemID = 0x203B;
            this.HairHue = 0x1BB;

            this.AddItem(new SamuraiTabi(0x8FD));
            this.AddItem(new FemaleKimono(0x4B6));
            this.AddItem(new Obi(0x526));

            this.AddItem(new GoldBracelet());
        }

        public override int GetAutoTalkRange(PlayerMobile m)
        {
            if (m.Quest == null)
                return 3;
            else
                return -1;
        }

        public override void OnTalk(PlayerMobile player, bool contextMenu)
        {
            QuestSystem qs = player.Quest;

            if (qs is TerribleHatchlingsQuest)
            {
                if (qs.IsObjectiveInProgress(typeof(FirstKillObjective)))
                {
                    qs.AddConversation(new DirectionConversation());
                }
                else if (qs.IsObjectiveInProgress(typeof(SecondKillObjective)) ||
                         qs.IsObjectiveInProgress(typeof(ThirdKillObjective)))
                {
                    qs.AddConversation(new TakeCareConversation());
                }
                else
                {
                    QuestObjective obj = qs.FindObjective(typeof(ReturnObjective));

                    if (obj != null && !obj.Completed)
                    {
                        Container cont = GetNewContainer();

                        cont.DropItem(new Gold(Utility.RandomMinMax(100, 200)));

                        if (Utility.RandomBool())
                        {
                            BaseWeapon weapon = Loot.Construct(Loot.SEWeaponTypes) as BaseWeapon;

                            if (weapon != null)
                            {
                                BaseRunicTool.ApplyAttributesTo(weapon, 3, 10, 30);
                                cont.DropItem(weapon);
                            }
                        }
                        else
                        {
                            BaseArmor armor = Loot.Construct(Loot.SEArmorTypes) as BaseArmor;

                            if (armor != null)
                            {
                                BaseRunicTool.ApplyAttributesTo(armor, 1, 10, 20);
                                cont.DropItem(armor);
                            }
                        }

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
                }
            }
            else
            {
                TerribleHatchlingsQuest newQuest = new TerribleHatchlingsQuest(player);
                bool inRestartPeriod = false;

                if (qs != null)
                {
                    if (contextMenu)
                        this.SayTo(player, 1063322); // Before you can help me with the Terrible Hatchlings, you'll need to finish the quest you've already taken!
                }
                else if (QuestSystem.CanOfferQuest(player, typeof(TerribleHatchlingsQuest), out inRestartPeriod))
                {
                    newQuest.SendOffer();
                }
                else if (inRestartPeriod && contextMenu)
                {
                    this.SayTo(player, 1049357); // I have nothing more for you at this time.
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }

        public override void TurnToTokuno()
        {
        }
    }
}