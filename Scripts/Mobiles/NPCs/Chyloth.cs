using System;
using Server.Engines.PartySystem;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Engines.Quests.Doom
{
    public class Chyloth : BaseQuester
    {
        private static readonly int[] m_Offsets = new int[]
        {
            -1, -1,
            -1, 0,
            -1, 1,
            0, -1,
            0, 1,
            1, -1,
            1, 0,
            1, 1
        };

        private Mobile m_AngryAt;
        private BellOfTheDead m_Bell;

        [Constructable]
        public Chyloth()
            : base("the Ferryman")
        {
        }

        public Chyloth(Serial serial)
            : base(serial)
        {
        }

        public BellOfTheDead Bell
        {
            get
            {
                return m_Bell;
            }
            set
            {
                m_Bell = value;
            }
        }
        public Mobile AngryAt
        {
            get
            {
                return m_AngryAt;
            }
            set
            {
                m_AngryAt = value;
            }
        }
        public static void TeleportToFerry(Mobile from)
        {
            Point3D loc = new Point3D(408, 251, 2);
            Map map = Map.Malas;

            Effects.SendLocationParticles(EffectItem.Create(loc, map, EffectItem.DefaultDuration), 0x3728, 10, 10, 0, 0, 2023, 0);
            Effects.PlaySound(loc, map, 0x1FE);

            BaseCreature.TeleportPets(from, loc, map);

            from.MoveToWorld(loc, map);
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Hue = 0x8455;
            Body = 0x190;

            Name = "Chyloth";
        }

        public override void InitOutfit()
        {
            EquipItem(new ChylothShroud());
            EquipItem(new ChylothStaff());
        }

        public virtual void BeginGiveWarning()
        {
            if (Deleted || m_AngryAt == null)
                return;

            Timer.DelayCall(TimeSpan.FromSeconds(4.0), new TimerCallback(EndGiveWarning));
        }

        public virtual void EndGiveWarning()
        {
            if (Deleted || m_AngryAt == null)
                return;

            PublicOverheadMessage(MessageType.Regular, 0x3B2, 1050013, m_AngryAt.Name); // You have summoned me in vain ~1_NAME~!  Only the dead may cross!
            PublicOverheadMessage(MessageType.Regular, 0x3B2, 1050014); // Why have you disturbed me, mortal?!?

            BeginSummonDragon();
        }

        public virtual void BeginSummonDragon()
        {
            if (Deleted || m_AngryAt == null)
                return;

            Timer.DelayCall(TimeSpan.FromSeconds(30.0), new TimerCallback(EndSummonDragon));
        }

        public virtual void BeginRemove(TimeSpan delay)
        {
            Timer.DelayCall(delay, new TimerCallback(EndRemove));
        }

        public virtual void EndRemove()
        {
            if (Deleted)
                return;

            Point3D loc = Location;
            Map map = Map;

            Effects.SendLocationParticles(EffectItem.Create(loc, map, EffectItem.DefaultDuration), 0x3728, 10, 10, 0, 0, 2023, 0);
            Effects.PlaySound(loc, map, 0x1FE);

            Delete();
        }

        public virtual void EndSummonDragon()
        {
            if (Deleted || m_AngryAt == null)
                return;

            Map map = m_AngryAt.Map;

            if (map == null)
                return;

            if (!m_AngryAt.Region.IsPartOf("Doom"))
                return;

            PublicOverheadMessage(MessageType.Regular, 0x3B2, 1050015); // Feel the wrath of my legions!!!
            PublicOverheadMessage(MessageType.Regular, 0x3B2, false, "MUHAHAHAHA HAHAH HAHA"); // A wee bit crazy, aren't we?

            SkeletalDragon dragon = new SkeletalDragon();

            int offset = Utility.Random(8) * 2;

            bool foundLoc = false;

            for (int i = 0; i < m_Offsets.Length; i += 2)
            {
                int x = m_AngryAt.X + m_Offsets[(offset + i) % m_Offsets.Length];
                int y = m_AngryAt.Y + m_Offsets[(offset + i + 1) % m_Offsets.Length];

                if (map.CanSpawnMobile(x, y, m_AngryAt.Z))
                {
                    dragon.MoveToWorld(new Point3D(x, y, m_AngryAt.Z), map);
                    foundLoc = true;
                    break;
                }
                else
                {
                    int z = map.GetAverageZ(x, y);

                    if (map.CanSpawnMobile(x, y, z))
                    {
                        dragon.MoveToWorld(new Point3D(x, y, z), map);
                        foundLoc = true;
                        break;
                    }
                }
            }

            if (!foundLoc)
                dragon.MoveToWorld(m_AngryAt.Location, map);

            dragon.Combatant = m_AngryAt;

            if (m_Bell != null)
                m_Bell.Dragon = dragon;
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (dropped is GoldenSkull)
            {
                dropped.Delete();

                PublicOverheadMessage(MessageType.Regular, 0x3B2, 1050046, from.Name); // Very well, ~1_NAME~, I accept your token. You may cross.
                BeginRemove(TimeSpan.FromSeconds(4.0));

                Party p = PartySystem.Party.Get(from);

                if (p != null)
                {
                    for (int i = 0; i < p.Members.Count; ++i)
                    {
                        PartyMemberInfo pmi = (PartyMemberInfo)p.Members[i];
                        Mobile member = pmi.Mobile;

                        if (member != from && member.Map == Map.Malas && member.Region.IsPartOf("Doom"))
                        {
                            if (m_AngryAt == member)
                                m_AngryAt = null;

                            member.CloseGump(typeof(ChylothPartyGump));
                            member.SendGump(new ChylothPartyGump(from, member));
                        }
                    }
                }

                if (m_AngryAt == from)
                    m_AngryAt = null;

                TeleportToFerry(from);

                return false;
            }

            return base.OnDragDrop(from, dropped);
        }

        public override bool CanTalkTo(PlayerMobile to)
        {
            return false;
        }

        public override void OnTalk(PlayerMobile player, bool contextMenu)
        {
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

    public class ChylothPartyGump : Gump
    {
        private readonly Mobile m_Leader;
        private readonly Mobile m_Member;
        public ChylothPartyGump(Mobile leader, Mobile member)
            : base(150, 50)
        {
            m_Leader = leader;
            m_Member = member;

            Closable = false;

            AddPage(0);

            AddImage(0, 0, 3600);

            AddImageTiled(0, 14, 15, 200, 3603);
            AddImageTiled(380, 14, 14, 200, 3605);
            AddImage(0, 201, 3606);
            AddImageTiled(15, 201, 370, 16, 3607);
            AddImageTiled(15, 0, 370, 16, 3601);
            AddImage(380, 0, 3602);
            AddImage(380, 201, 3608);
            AddImageTiled(15, 15, 365, 190, 2624);

            AddRadio(30, 140, 9727, 9730, true, 1);
            AddHtmlLocalized(65, 145, 300, 25, 1050050, 0x7FFF, false, false); // Yes, let's go!

            AddRadio(30, 175, 9727, 9730, false, 0);
            AddHtmlLocalized(65, 178, 300, 25, 1050049, 0x7FFF, false, false); // No thanks, I'd rather stay here.

            AddHtmlLocalized(30, 20, 360, 35, 1050047, 0x7FFF, false, false); // Another player has paid Chyloth for your passage across lake Mortis:

            AddHtmlLocalized(30, 105, 345, 40, 1050048, 0x5B2D, false, false); // Do you wish to accept their invitation at this time?

            AddImage(65, 72, 5605);

            AddImageTiled(80, 90, 200, 1, 9107);
            AddImageTiled(95, 92, 200, 1, 9157);

            AddLabel(90, 70, 1645, leader.Name);

            AddButton(290, 175, 247, 248, 2, GumpButtonType.Reply, 0);

            AddImageTiled(15, 14, 365, 1, 9107);
            AddImageTiled(380, 14, 1, 190, 9105);
            AddImageTiled(15, 205, 365, 1, 9107);
            AddImageTiled(15, 14, 1, 190, 9105);
            AddImageTiled(0, 0, 395, 1, 9157);
            AddImageTiled(394, 0, 1, 217, 9155);
            AddImageTiled(0, 216, 395, 1, 9157);
            AddImageTiled(0, 0, 1, 217, 9155);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 2 && info.IsSwitched(1))
            {
                if (m_Member.Region.IsPartOf("Doom"))
                {
                    m_Leader.SendLocalizedMessage(1050054, m_Member.Name); // ~1_NAME~ has accepted your invitation to cross lake Mortis.

                    Chyloth.TeleportToFerry(m_Member);
                }
                else
                {
                    m_Member.SendLocalizedMessage(1050051); // The invitation has been revoked.
                }
            }
            else
            {
                m_Member.SendLocalizedMessage(1050052); // You have declined their invitation.
                m_Leader.SendLocalizedMessage(1050053, m_Member.Name); // ~1_NAME~ has declined your invitation to cross lake Mortis.
            }
        }
    }
}