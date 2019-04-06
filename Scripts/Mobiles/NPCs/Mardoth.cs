using System;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Necro
{
    public class Mardoth : BaseQuester
    {
        [Constructable]
        public Mardoth()
            : base("the Ancient Necromancer")
        {
        }

        public Mardoth(Serial serial)
            : base(serial)
        {
        }

        public override void InitBody()
        {
            this.InitStats(100, 100, 25);

            this.Hue = 0x8849;
            this.Body = 0x190;

            this.Name = "Mardoth";
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player != null)
            {
                QuestSystem qs = player.Quest;

                if (qs is DarkTidesQuest)
                {
                    if (dropped is DarkTidesHorn)
                    {
                        if (player.Young)
                        {
                            DarkTidesHorn horn = (DarkTidesHorn)dropped;

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
                }
            }

            return base.OnDragDrop(from, dropped);
        }

        public override void InitOutfit()
        {
            this.AddItem(new Sandals(0x1));
            this.AddItem(new Robe(0x66D));
            this.AddItem(new BlackStaff());
            this.AddItem(new WizardsHat(0x1));

            this.FacialHairItemID = 0x2041;
            this.FacialHairHue = 0x482;

            this.HairItemID = 0x203C;
            this.HairHue = 0x482;

            Item gloves = new BoneGloves();
            gloves.Hue = 0x66D;
            this.AddItem(gloves);

            Item gorget = new PlateGorget();
            gorget.Hue = 0x1;
            this.AddItem(gorget);
        }

        public override int GetAutoTalkRange(PlayerMobile m)
        {
            return 3;
        }

        public override bool CanTalkTo(PlayerMobile to)
        {
            DarkTidesQuest qs = to.Quest as DarkTidesQuest;

            if (qs == null)
                return (to.Quest == null && QuestSystem.CanOfferQuest(to, typeof(DarkTidesQuest)));

            return (qs.FindObjective(typeof(FindMardothAboutVaultObjective)) != null);
        }

        public override void OnTalk(PlayerMobile player, bool contextMenu)
        {
            QuestSystem qs = player.Quest;

            if (qs is DarkTidesQuest)
            {
                if (DarkTidesQuest.HasLostCallingScroll(player))
                {
                    qs.AddConversation(new LostCallingScrollConversation(true));
                }
                else
                {
                    QuestObjective obj = qs.FindObjective(typeof(FindMardothAboutVaultObjective));

                    if (obj != null && !obj.Completed)
                    {
                        obj.Complete();
                    }
                    else
                    {
                        obj = qs.FindObjective(typeof(FindMardothAboutKronusObjective));

                        if (obj != null && !obj.Completed)
                        {
                            obj.Complete();
                        }
                        else
                        {
                            obj = qs.FindObjective(typeof(FindMardothEndObjective));

                            if (obj != null && !obj.Completed)
                            {
                                Container cont = GetNewContainer();

                                cont.DropItem(new PigIron(20));
                                cont.DropItem(new NoxCrystal(20));
                                cont.DropItem(new BatWing(25));
                                cont.DropItem(new DaemonBlood(20));
                                cont.DropItem(new GraveDust(20));

                                BaseWeapon weapon = new BoneHarvester();

                                weapon.Slayer = SlayerName.OrcSlaying;

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

                                cont.DropItem(weapon);

                                if (!Core.TOL)
                                {
                                    cont.DropItem(new BankCheck(2000));
                                }
                                else
                                {
                                    Banker.Deposit(player, 2000, true);
                                }

                                cont.DropItem(new EnchantedSextant());

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
                            else if (contextMenu)
                            {
                                this.FocusTo(player);
                                player.SendLocalizedMessage(1061821); // Mardoth has nothing more for you at this time.
                            }
                        }
                    }
                }
            }
            else if (qs == null && QuestSystem.CanOfferQuest(player, typeof(DarkTidesQuest)))
            {
                new DarkTidesQuest(player).SendOffer();
            }
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