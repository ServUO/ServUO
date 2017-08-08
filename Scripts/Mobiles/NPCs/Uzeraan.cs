using System;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Haven
{
    public class Uzeraan : BaseQuester
    {
        [Constructable]
        public Uzeraan()
            : base("the Conjurer")
        {
        }

        public Uzeraan(Serial serial)
            : base(serial)
        {
        }

        public override void InitBody()
        {
            this.InitStats(100, 100, 25);

            this.Hue = 0x83F3;

            this.Female = false;
            this.Body = 0x190;
            this.Name = "Uzeraan";
        }

        public override void InitOutfit()
        {
            this.AddItem(new Robe(0x4DD));
            this.AddItem(new WizardsHat(0x8A5));
            this.AddItem(new Shoes(0x8A5));

            this.HairItemID = 0x203C;
            this.HairHue = 0x455;

            this.FacialHairItemID = 0x203E;
            this.FacialHairHue = 0x455;

            BlackStaff staff = new BlackStaff();
            staff.Movable = false;
            this.AddItem(staff);
        }

        public override int GetAutoTalkRange(PlayerMobile pm)
        {
            return 3;
        }

        public override bool CanTalkTo(PlayerMobile to)
        {
            return to.Quest is UzeraanTurmoilQuest;
        }

        public override void OnTalk(PlayerMobile player, bool contextMenu)
        {
            QuestSystem qs = player.Quest;

            if (qs is UzeraanTurmoilQuest)
            {
                if (UzeraanTurmoilQuest.HasLostScrollOfPower(player))
                {
                    qs.AddConversation(new LostScrollOfPowerConversation(true));
                }
                else if (UzeraanTurmoilQuest.HasLostFertileDirt(player))
                {
                    qs.AddConversation(new LostFertileDirtConversation(true));
                }
                else if (UzeraanTurmoilQuest.HasLostDaemonBlood(player))
                {
                    qs.AddConversation(new LostDaemonBloodConversation());
                }
                else if (UzeraanTurmoilQuest.HasLostDaemonBone(player))
                {
                    qs.AddConversation(new LostDaemonBoneConversation());
                }
                else
                {
                    if (player.Profession == 2) // magician
                    {
                        Container backpack = player.Backpack;

                        if (backpack == null ||
                            backpack.GetAmount(typeof(BlackPearl)) < 30 ||
                            backpack.GetAmount(typeof(Bloodmoss)) < 30 ||
                            backpack.GetAmount(typeof(Garlic)) < 30 ||
                            backpack.GetAmount(typeof(Ginseng)) < 30 ||
                            backpack.GetAmount(typeof(MandrakeRoot)) < 30 ||
                            backpack.GetAmount(typeof(Nightshade)) < 30 ||
                            backpack.GetAmount(typeof(SulfurousAsh)) < 30 ||
                            backpack.GetAmount(typeof(SpidersSilk)) < 30)
                        {
                            qs.AddConversation(new FewReagentsConversation());
                        }
                    }

                    QuestObjective obj = qs.FindObjective(typeof(FindUzeraanBeginObjective));

                    if (obj != null && !obj.Completed)
                    {
                        obj.Complete();
                    }
                    else
                    {
                        obj = qs.FindObjective(typeof(FindUzeraanFirstTaskObjective));

                        if (obj != null && !obj.Completed)
                        {
                            obj.Complete();
                        }
                        else
                        {
                            obj = qs.FindObjective(typeof(FindUzeraanAboutReportObjective));

                            if (obj != null && !obj.Completed)
                            {
                                Container cont = GetNewContainer();

                                if (player.Profession == 2) // magician
                                {
                                    cont.DropItem(new MarkScroll(5));
                                    cont.DropItem(new RecallScroll(5));
                                    for (int i = 0; i < 5; i++)
                                    {
                                        cont.DropItem(new RecallRune());
                                    }
                                }
                                else
                                {
                                    cont.DropItem(new Gold(300));
                                    for (int i = 0; i < 6; i++)
                                    {
                                        cont.DropItem(new NightSightPotion());
                                        cont.DropItem(new LesserHealPotion());
                                    }
                                }

                                if (!player.PlaceInBackpack(cont))
                                {
                                    cont.Delete();
                                    player.SendLocalizedMessage(1046260); // You need to clear some space in your inventory to continue with the quest.  Come back here when you have more space in your inventory.
                                }
                                else
                                {
                                    obj.Complete();
                                }
                            }
                            else
                            {
                                obj = qs.FindObjective(typeof(ReturnScrollOfPowerObjective));

                                if (obj != null && !obj.Completed)
                                {
                                    this.FocusTo(player);
                                    this.SayTo(player, 1049378); // Hand me the scroll, if you have it.
                                }
                                else
                                {
                                    obj = qs.FindObjective(typeof(ReturnFertileDirtObjective));

                                    if (obj != null && !obj.Completed)
                                    {
                                        this.FocusTo(player);
                                        this.SayTo(player, 1049381); // Hand me the Fertile Dirt, if you have it.
                                    }
                                    else
                                    {
                                        obj = qs.FindObjective(typeof(ReturnDaemonBloodObjective));

                                        if (obj != null && !obj.Completed)
                                        {
                                            this.FocusTo(player);
                                            this.SayTo(player, 1049379); // Hand me the Vial of Blood, if you have it.
                                        }
                                        else
                                        {
                                            obj = qs.FindObjective(typeof(ReturnDaemonBoneObjective));

                                            if (obj != null && !obj.Completed)
                                            {
                                                this.FocusTo(player);
                                                this.SayTo(player, 1049380); // Hand me the Daemon Bone, if you have it.
                                            }
                                            else
                                            {
                                                this.SayTo(player, 1049357); // I have nothing more for you at this time.
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

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player != null)
            {
                QuestSystem qs = player.Quest;

                if (qs is UzeraanTurmoilQuest)
                {
                    if (dropped is UzeraanTurmoilHorn)
                    {
                        if (player.Young)
                        {
                            UzeraanTurmoilHorn horn = (UzeraanTurmoilHorn)dropped;

                            if (horn.Charges < 10)
                            {
                                this.SayTo(from, 1049384); // I have recharged the item for you.
                                horn.Charges = 10;
                            }
                            else
                            {
                                this.SayTo(from, 1049385); // That doesn't need recharging yet.
                            }
                        }
                        else
                        {
                            player.SendLocalizedMessage(1114333); //You must be young to have this item recharged.
                        }

                        return false;
                    }

                    if (dropped is SchmendrickScrollOfPower)
                    {
                        QuestObjective obj = qs.FindObjective(typeof(ReturnScrollOfPowerObjective));

                        if (obj != null && !obj.Completed)
                        {
                            Container cont = GetNewContainer();

                            cont.DropItem(new TreasureMap(player.Young ? 0 : 1, Map.Trammel));
                            cont.DropItem(new Shovel());
                            cont.DropItem(new UzeraanTurmoilHorn());

                            if (!player.PlaceInBackpack(cont))
                            {
                                cont.Delete();
                                player.SendLocalizedMessage(1046260); // You need to clear some space in your inventory to continue with the quest.  Come back here when you have more space in your inventory.
                                return false;
                            }
                            else
                            {
                                dropped.Delete();
                                obj.Complete();
                                return true;
                            }
                        }
                    }
                    else if (dropped is QuestFertileDirt)
                    {
                        QuestObjective obj = qs.FindObjective(typeof(ReturnFertileDirtObjective));

                        if (obj != null && !obj.Completed)
                        {
                            Container cont = GetNewContainer();

                            if (player.Profession == 2) // magician
                            {
                                cont.DropItem(new BlackPearl(20));
                                cont.DropItem(new Bloodmoss(20));
                                cont.DropItem(new Garlic(20));
                                cont.DropItem(new Ginseng(20));
                                cont.DropItem(new MandrakeRoot(20));
                                cont.DropItem(new Nightshade(20));
                                cont.DropItem(new SulfurousAsh(20));
                                cont.DropItem(new SpidersSilk(20));

                                for (int i = 0; i < 3; i++)
                                    cont.DropItem(Loot.RandomScroll(0, 23, SpellbookType.Regular));
                            }
                            else
                            {
                                cont.DropItem(new Gold(300));
                                cont.DropItem(new Bandage(25));

                                for (int i = 0; i < 5; i++)
                                    cont.DropItem(new LesserHealPotion());
                            }

                            if (!player.PlaceInBackpack(cont))
                            {
                                cont.Delete();
                                player.SendLocalizedMessage(1046260); // You need to clear some space in your inventory to continue with the quest.  Come back here when you have more space in your inventory.
                                return false;
                            }
                            else
                            {
                                dropped.Delete();
                                obj.Complete();
                                return true;
                            }
                        }
                    }
                    else if (dropped is QuestDaemonBlood)
                    {
                        QuestObjective obj = qs.FindObjective(typeof(ReturnDaemonBloodObjective));

                        if (obj != null && !obj.Completed)
                        {
                            Item reward;

                            if (player.Profession == 2) // magician
                            {
                                Container cont = GetNewContainer();

                                cont.DropItem(new ExplosionScroll(4));
                                cont.DropItem(new MagicWizardsHat());

                                reward = cont;
                            }
                            else
                            {
                                BaseWeapon weapon;
                                switch ( Utility.Random(6) )
                                {
                                    case 0:
                                        weapon = new Broadsword();
                                        break;
                                    case 1:
                                        weapon = new Cutlass();
                                        break;
                                    case 2:
                                        weapon = new Katana();
                                        break;
                                    case 3:
                                        weapon = new Longsword();
                                        break;
                                    case 4:
                                        weapon = new Scimitar();
                                        break;
                                    default:
                                        weapon = new VikingSword();
                                        break;
                                }

                                if (Core.AOS)
                                {
                                    BaseRunicTool.ApplyAttributesTo(weapon, 3, 20, 40);
                                }
                                else
                                {
                                    weapon.DamageLevel = (WeaponDamageLevel)BaseCreature.RandomMinMaxScaled(2, 4);
                                    weapon.AccuracyLevel = (WeaponAccuracyLevel)BaseCreature.RandomMinMaxScaled(2, 4);
                                    weapon.DurabilityLevel = (WeaponDurabilityLevel)BaseCreature.RandomMinMaxScaled(2, 4);
                                }

                                weapon.Slayer = SlayerName.Silver;

                                reward = weapon;
                            }

                            if (!player.PlaceInBackpack(reward))
                            {
                                reward.Delete();
                                player.SendLocalizedMessage(1046260); // You need to clear some space in your inventory to continue with the quest.  Come back here when you have more space in your inventory.
                                return false;
                            }
                            else
                            {
                                dropped.Delete();
                                obj.Complete();
                                return true;
                            }
                        }
                    }
                    else if (dropped is QuestDaemonBone)
                    {
                        QuestObjective obj = qs.FindObjective(typeof(ReturnDaemonBoneObjective));

                        if (obj != null && !obj.Completed)
                        {
                            Container cont = GetNewContainer();

                            if (!Core.TOL)
                            {
                                cont.DropItem(new BankCheck(2000));
                            }
                            else
                            {
                                Banker.Deposit(from, 2000, true);
                            }

                            cont.DropItem(new EnchantedSextant());

                            if (!player.PlaceInBackpack(cont))
                            {
                                cont.Delete();
                                player.SendLocalizedMessage(1046260); // You need to clear some space in your inventory to continue with the quest.  Come back here when you have more space in your inventory.
                                return false;
                            }
                            else
                            {
                                dropped.Delete();
                                obj.Complete();
                                return true;
                            }
                        }
                    }
                }
            }

            return base.OnDragDrop(from, dropped);
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            base.OnMovement(m, oldLocation);

            if (m is PlayerMobile && !m.Frozen && !m.Alive && this.InRange(m, 4) && !this.InRange(oldLocation, 4) && this.InLOS(m))
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