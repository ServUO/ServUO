/* Poker Cards, originally by flowerbudd, 2003.
 * Modified by Ednyved to:
 *  + name the card puller
 *  + display tarot card gumps when they exist and close them upon puller's next pull.
 *  + optionally make sounds upon pull.
 * If the noises bother your players, set IsNoisy to false, and the cards will pipe down.
 * */

using System;
using Server;
using Server.Network;
using Server.Gumps;

namespace Server.Items
{
    public class tarotpoker : Item
    {
        private bool m_IsNoisy;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsNoisy
        {
            get { return m_IsNoisy; }
            set { m_IsNoisy = value; }
        }

        [Constructable]
        public tarotpoker()
            : base(0x12AB)
        {
            Stackable = false;
            Name = "Deck of Tarot Poker Cards";
            Weight = 0.5;
            m_IsNoisy = true;
        }
        public tarotpoker(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this.GetWorldLocation(), 4))
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.

            else switch (Utility.Random(22))
                {
                    default:
                    case 0:
                        {
                            from.CloseGump(typeof(DeathGump));
                            from.CloseGump(typeof(FoolGump));
                            from.CloseGump(typeof(HangedmanGump));
                            from.CloseGump(typeof(HPGump));
                            from.CloseGump(typeof(JusticeGump));
                            from.CloseGump(typeof(MageGump));
                            from.CloseGump(typeof(StarGump));
                            from.CloseGump(typeof(TowerGump));
                            from.CloseGump(typeof(WoFGump));
                            from.CloseGump(typeof(WorldGump));
                            from.SendGump(new FoolGump());

                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("{0} pulls 'The Fool'", from.Name));
                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("Everyone bets 50."));
                            break;
                        }
                    case 1:
                        {
                            from.CloseGump(typeof(DeathGump));
                            from.CloseGump(typeof(FoolGump));
                            from.CloseGump(typeof(HangedmanGump));
                            from.CloseGump(typeof(HPGump));
                            from.CloseGump(typeof(JusticeGump));
                            from.CloseGump(typeof(MageGump));
                            from.CloseGump(typeof(StarGump));
                            from.CloseGump(typeof(TowerGump));
                            from.CloseGump(typeof(WoFGump));
                            from.CloseGump(typeof(WorldGump));
                            from.SendGump(new MageGump());

                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("{0} pulls 'The Mage'", from.Name));
                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("Everyone bets 500."));
                            break;
                        }
                    case 2:
                        {

                            from.CloseGump(typeof(DeathGump));
                            from.CloseGump(typeof(FoolGump));
                            from.CloseGump(typeof(HangedmanGump));
                            from.CloseGump(typeof(HPGump));
                            from.CloseGump(typeof(JusticeGump));
                            from.CloseGump(typeof(MageGump));
                            from.CloseGump(typeof(StarGump));
                            from.CloseGump(typeof(TowerGump));
                            from.CloseGump(typeof(WoFGump));
                            from.CloseGump(typeof(WorldGump));
                            from.SendGump(new HPGump());

                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("{0} pulls 'The High Priestess'", from.Name));
                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("Everyone bets 350."));
                            break;
                        }
                    case 3:
                        {
                            from.CloseGump(typeof(DeathGump));
                            from.CloseGump(typeof(FoolGump));
                            from.CloseGump(typeof(HangedmanGump));
                            from.CloseGump(typeof(HPGump));
                            from.CloseGump(typeof(JusticeGump));
                            from.CloseGump(typeof(MageGump));
                            from.CloseGump(typeof(StarGump));
                            from.CloseGump(typeof(TowerGump));
                            from.CloseGump(typeof(WoFGump));
                            from.CloseGump(typeof(WorldGump));

                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("{0} pulls 'The Empress'", from.Name));
                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("Bet 250."));
                            break;
                        }
                    case 4:
                        {
                            from.CloseGump(typeof(DeathGump));
                            from.CloseGump(typeof(FoolGump));
                            from.CloseGump(typeof(HangedmanGump));
                            from.CloseGump(typeof(HPGump));
                            from.CloseGump(typeof(JusticeGump));
                            from.CloseGump(typeof(MageGump));
                            from.CloseGump(typeof(StarGump));
                            from.CloseGump(typeof(TowerGump));
                            from.CloseGump(typeof(WoFGump));
                            from.CloseGump(typeof(WorldGump));

                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("{0} pulls 'The Emperor'", from.Name));
                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("Bet 500 gp."));
                            break;
                        }
                    case 5:
                        {
                            from.CloseGump(typeof(DeathGump));
                            from.CloseGump(typeof(FoolGump));
                            from.CloseGump(typeof(HangedmanGump));
                            from.CloseGump(typeof(HPGump));
                            from.CloseGump(typeof(JusticeGump));
                            from.CloseGump(typeof(MageGump));
                            from.CloseGump(typeof(StarGump));
                            from.CloseGump(typeof(TowerGump));
                            from.CloseGump(typeof(WoFGump));
                            from.CloseGump(typeof(WorldGump));

                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("{0} pulls 'The Hierophant'", from.Name));
                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("Bet 50 gp."));
                            break;
                        }
                    case 6:
                        {
                            from.CloseGump(typeof(DeathGump));
                            from.CloseGump(typeof(FoolGump));
                            from.CloseGump(typeof(HangedmanGump));
                            from.CloseGump(typeof(HPGump));
                            from.CloseGump(typeof(JusticeGump));
                            from.CloseGump(typeof(MageGump));
                            from.CloseGump(typeof(StarGump));
                            from.CloseGump(typeof(TowerGump));
                            from.CloseGump(typeof(WoFGump));
                            from.CloseGump(typeof(WorldGump));

                            if (m_IsNoisy)
                            {
                                from.PlaySound(from.Female ? 811 : 1085);//Ooo
                            }
                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("{0} pulls 'The Lovers'", from.Name));
                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("You will split the winning pot, unless you pull Death."));
                            break;
                        }
                    case 7:
                        {
                            from.CloseGump(typeof(DeathGump));
                            from.CloseGump(typeof(FoolGump));
                            from.CloseGump(typeof(HangedmanGump));
                            from.CloseGump(typeof(HPGump));
                            from.CloseGump(typeof(JusticeGump));
                            from.CloseGump(typeof(MageGump));
                            from.CloseGump(typeof(StarGump));
                            from.CloseGump(typeof(TowerGump));
                            from.CloseGump(typeof(WoFGump));
                            from.CloseGump(typeof(WorldGump));

                            if (m_IsNoisy)
                            {
                                from.PlaySound(from.Female ? 796 : 1068);//disgusted noise
                            }
                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("{0} pulls 'Temperance'", from.Name));
                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("{0} must now put up half the amount of the current pot!", from.Name));
                            break;
                        }
                    case 8:
                        {
                            from.CloseGump(typeof(DeathGump));
                            from.CloseGump(typeof(FoolGump));
                            from.CloseGump(typeof(HangedmanGump));
                            from.CloseGump(typeof(HPGump));
                            from.CloseGump(typeof(JusticeGump));
                            from.CloseGump(typeof(MageGump));
                            from.CloseGump(typeof(StarGump));
                            from.CloseGump(typeof(TowerGump));
                            from.CloseGump(typeof(WoFGump));
                            from.CloseGump(typeof(WorldGump));
                            from.SendGump(new JusticeGump());

                            if (m_IsNoisy)
                            {
                                from.PlaySound(from.Female ? 816 : 1090); //sigh...
                            }
                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("{0} pulls 'Justice'", from.Name));
                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("Skip your next turn."));
                            break;
                        }
                    case 9:
                        {
                            from.CloseGump(typeof(DeathGump));
                            from.CloseGump(typeof(FoolGump));
                            from.CloseGump(typeof(HangedmanGump));
                            from.CloseGump(typeof(HPGump));
                            from.CloseGump(typeof(JusticeGump));
                            from.CloseGump(typeof(MageGump));
                            from.CloseGump(typeof(StarGump));
                            from.CloseGump(typeof(TowerGump));
                            from.CloseGump(typeof(WoFGump));
                            from.CloseGump(typeof(WorldGump));

                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("{0} pulls 'The Hermit'", from.Name));
                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("Bet 10 Gp."));
                            break;
                        }
                    case 10:
                        {
                            from.CloseGump(typeof(DeathGump));
                            from.CloseGump(typeof(FoolGump));
                            from.CloseGump(typeof(HangedmanGump));
                            from.CloseGump(typeof(HPGump));
                            from.CloseGump(typeof(JusticeGump));
                            from.CloseGump(typeof(MageGump));
                            from.CloseGump(typeof(StarGump));
                            from.CloseGump(typeof(TowerGump));
                            from.CloseGump(typeof(WoFGump));
                            from.CloseGump(typeof(WorldGump));
                            from.SendGump(new WoFGump());

                            if (m_IsNoisy)
                            {
                                from.PlaySound(from.Female ? 778 : 1049); //ah!
                            }
                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("{0} pulls 'The Wheel of Fortune'", from.Name));
                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("{0} may now take one-tenth of the pot!", from.Name));
                            break;
                        }
                    case 11:
                        {
                            from.CloseGump(typeof(DeathGump));
                            from.CloseGump(typeof(FoolGump));
                            from.CloseGump(typeof(HangedmanGump));
                            from.CloseGump(typeof(HPGump));
                            from.CloseGump(typeof(JusticeGump));
                            from.CloseGump(typeof(MageGump));
                            from.CloseGump(typeof(StarGump));
                            from.CloseGump(typeof(TowerGump));
                            from.CloseGump(typeof(WoFGump));
                            from.CloseGump(typeof(WorldGump));

                            if (m_IsNoisy)
                            {
                                from.PlaySound(from.Female ? 794 : 1066); //giggle
                            }
                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("{0} pulls 'Strength'", from.Name));
                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("All players BUT {0} must bet 300.", from.Name));
                            break;
                        }
                    case 12:
                        {
                            from.CloseGump(typeof(DeathGump));
                            from.CloseGump(typeof(FoolGump));
                            from.CloseGump(typeof(HangedmanGump));
                            from.CloseGump(typeof(HPGump));
                            from.CloseGump(typeof(JusticeGump));
                            from.CloseGump(typeof(MageGump));
                            from.CloseGump(typeof(StarGump));
                            from.CloseGump(typeof(TowerGump));
                            from.CloseGump(typeof(WoFGump));
                            from.CloseGump(typeof(WorldGump));

                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("{0} pulls 'The Chariot'", from.Name));
                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("Put in double the amount of the last bet!"));
                            break;
                        }
                    case 13:
                        {
                            from.CloseGump(typeof(DeathGump));
                            from.CloseGump(typeof(FoolGump));
                            from.CloseGump(typeof(HangedmanGump));
                            from.CloseGump(typeof(HPGump));
                            from.CloseGump(typeof(JusticeGump));
                            from.CloseGump(typeof(MageGump));
                            from.CloseGump(typeof(StarGump));
                            from.CloseGump(typeof(TowerGump));
                            from.CloseGump(typeof(WoFGump));
                            from.CloseGump(typeof(WorldGump));
                            from.SendGump(new DeathGump());

                            if (m_IsNoisy)
                            {
                                from.PlaySound(Utility.Random(from.Female ? 0x314 : 0x423, from.Female ? 4 : 5)); //death sounds
                            }
                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("{0} pulls 'Death'", from.Name));
                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("You lose! Leave the game now!"));
                            break;
                        }
                    case 14:
                        {

                            from.CloseGump(typeof(DeathGump));
                            from.CloseGump(typeof(FoolGump));
                            from.CloseGump(typeof(HangedmanGump));
                            from.CloseGump(typeof(HPGump));
                            from.CloseGump(typeof(JusticeGump));
                            from.CloseGump(typeof(MageGump));
                            from.CloseGump(typeof(StarGump));
                            from.CloseGump(typeof(TowerGump));
                            from.CloseGump(typeof(WoFGump));
                            from.CloseGump(typeof(WorldGump));
                            from.SendGump(new HangedmanGump());

                            if (m_IsNoisy)
                            {
                                from.PlaySound(from.Female ? 793 : 1065); //gasp!
                            }
                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("{0} pulls 'Hanged Man'", from.Name));
                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("{0}'s bet must match the amount of the current pot!", from.Name));
                            break;
                        }
                    case 15:
                        {
                            from.CloseGump(typeof(DeathGump));
                            from.CloseGump(typeof(FoolGump));
                            from.CloseGump(typeof(HangedmanGump));
                            from.CloseGump(typeof(HPGump));
                            from.CloseGump(typeof(JusticeGump));
                            from.CloseGump(typeof(MageGump));
                            from.CloseGump(typeof(StarGump));
                            from.CloseGump(typeof(TowerGump));
                            from.CloseGump(typeof(WoFGump));
                            from.CloseGump(typeof(WorldGump));

                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("{0} pulls 'The Devil'", from.Name));
                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("Everyone bets 500."));
                            break;
                        }
                    case 16:
                        {

                            from.CloseGump(typeof(DeathGump));
                            from.CloseGump(typeof(FoolGump));
                            from.CloseGump(typeof(HangedmanGump));
                            from.CloseGump(typeof(HPGump));
                            from.CloseGump(typeof(JusticeGump));
                            from.CloseGump(typeof(MageGump));
                            from.CloseGump(typeof(StarGump));
                            from.CloseGump(typeof(TowerGump));
                            from.CloseGump(typeof(WoFGump));
                            from.CloseGump(typeof(WorldGump));
                            from.SendGump(new TowerGump());

                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("{0} pulls 'The Tower'", from.Name));
                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("The player across from {0} must bet 250.", from.Name));
                            break;
                        }
                    case 17:
                        {
                            from.CloseGump(typeof(DeathGump));
                            from.CloseGump(typeof(FoolGump));
                            from.CloseGump(typeof(HangedmanGump));
                            from.CloseGump(typeof(HPGump));
                            from.CloseGump(typeof(JusticeGump));
                            from.CloseGump(typeof(MageGump));
                            from.CloseGump(typeof(StarGump));
                            from.CloseGump(typeof(TowerGump));
                            from.CloseGump(typeof(WoFGump));
                            from.CloseGump(typeof(WorldGump));
                            from.SendGump(new StarGump());

                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("{0} pulls 'The Star'", from.Name));
                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("Everyone bets 200."));
                            break;
                        }
                    case 18:
                        {
                            from.CloseGump(typeof(DeathGump));
                            from.CloseGump(typeof(FoolGump));
                            from.CloseGump(typeof(HangedmanGump));
                            from.CloseGump(typeof(HPGump));
                            from.CloseGump(typeof(JusticeGump));
                            from.CloseGump(typeof(MageGump));
                            from.CloseGump(typeof(StarGump));
                            from.CloseGump(typeof(TowerGump));
                            from.CloseGump(typeof(WoFGump));
                            from.CloseGump(typeof(WorldGump));

                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("{0} pulls 'The Moon'", from.Name));
                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("Everyone bets 150."));
                            break;
                        }
                    case 19:
                        {
                            from.CloseGump(typeof(DeathGump));
                            from.CloseGump(typeof(FoolGump));
                            from.CloseGump(typeof(HangedmanGump));
                            from.CloseGump(typeof(HPGump));
                            from.CloseGump(typeof(JusticeGump));
                            from.CloseGump(typeof(MageGump));
                            from.CloseGump(typeof(StarGump));
                            from.CloseGump(typeof(TowerGump));
                            from.CloseGump(typeof(WoFGump));
                            from.CloseGump(typeof(WorldGump));

                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("{0} pulls 'The Sun'", from.Name));
                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("Everyone bets 250."));
                            break;
                        }
                    case 20:
                        {
                            from.CloseGump(typeof(DeathGump));
                            from.CloseGump(typeof(FoolGump));
                            from.CloseGump(typeof(HangedmanGump));
                            from.CloseGump(typeof(HPGump));
                            from.CloseGump(typeof(JusticeGump));
                            from.CloseGump(typeof(MageGump));
                            from.CloseGump(typeof(StarGump));
                            from.CloseGump(typeof(TowerGump));
                            from.CloseGump(typeof(WoFGump));
                            from.CloseGump(typeof(WorldGump));

                            if (m_IsNoisy)
                            {
                                from.PlaySound(from.Female ? 783 : 1054);// Woo-hoo!
                            }
                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("{0} pulls 'The Judgement'", from.Name));
                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("{0} wins the game and takes the pot!", from.Name));
                            break;
                        }
                    case 21:
                        {
                            from.CloseGump(typeof(DeathGump));
                            from.CloseGump(typeof(FoolGump));
                            from.CloseGump(typeof(HangedmanGump));
                            from.CloseGump(typeof(HPGump));
                            from.CloseGump(typeof(JusticeGump));
                            from.CloseGump(typeof(MageGump));
                            from.CloseGump(typeof(StarGump));
                            from.CloseGump(typeof(TowerGump));
                            from.CloseGump(typeof(WoFGump));
                            from.CloseGump(typeof(WorldGump));
                            from.SendGump(new WorldGump());

                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("{0} pulls 'The World'", from.Name));
                            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("Everyone bets 1000."));
                            break;
                        }
                }
        }

        private class DeathGump : Gump
        {
            public DeathGump()
                : base(0, 0)
            {
                AddImage(242, 191, 0x7725);
            }
        }

        private class FoolGump : Gump
        {
            public FoolGump()
                : base(0, 0)
            {
                AddImage(242, 191, 0x7728);
            }
        }

        private class HangedmanGump : Gump
        {
            public HangedmanGump()
                : base(0, 0)
            {
                AddImage(242, 191, 0x7729);
            }
        }

        private class HPGump : Gump
        {
            public HPGump()
                : base(0, 0)
            {
                AddImage(242, 191, 0x772A);
            }
        }

        private class JusticeGump : Gump
        {
            public JusticeGump()
                : base(0, 0)
            {
                AddImage(242, 191, 0x7727);
            }
        }

        private class MageGump : Gump
        {
            public MageGump()
                : base(0, 0)
            {
                AddImage(242, 191, 0x772B);
            }
        }

        private class StarGump : Gump
        {
            public StarGump()
                : base(0, 0)
            {
                AddImage(242, 191, 0x772C);
            }
        }

        private class TowerGump : Gump
        {
            public TowerGump()
                : base(0, 0)
            {
                AddImage(242, 191, 0x772D);
            }
        }

        private class WoFGump : Gump
        {
            public WoFGump()
                : base(0, 0)
            {
                AddImage(242, 191, 0x7726);
            }
        }

        private class WorldGump : Gump
        {
            public WorldGump()
                : base(0, 0)
            {
                AddImage(242, 191, 0x772E);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write((bool)m_IsNoisy);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_IsNoisy = reader.ReadBool();
                        break;
                    }
            }

        }
    }

}
