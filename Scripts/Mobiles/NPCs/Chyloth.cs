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
                return this.m_Bell;
            }
            set
            {
                this.m_Bell = value;
            }
        }
        public Mobile AngryAt
        {
            get
            {
                return this.m_AngryAt;
            }
            set
            {
                this.m_AngryAt = value;
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
            this.InitStats(100, 100, 25);

            this.Hue = 0x8455;
            this.Body = 0x190;

            this.Name = "Chyloth";
        }

        public override void InitOutfit()
        {
            this.EquipItem(new ChylothShroud());
            this.EquipItem(new ChylothStaff());
        }

        public virtual void BeginGiveWarning()
        {
            if (this.Deleted || this.m_AngryAt == null)
                return;

            Timer.DelayCall(TimeSpan.FromSeconds(4.0), new TimerCallback(EndGiveWarning));
        }

        public virtual void EndGiveWarning()
        {
            if (this.Deleted || this.m_AngryAt == null)
                return;

            this.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1050013, this.m_AngryAt.Name); // You have summoned me in vain ~1_NAME~!  Only the dead may cross!
            this.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1050014); // Why have you disturbed me, mortal?!?

            this.BeginSummonDragon();
        }

        public virtual void BeginSummonDragon()
        {
            if (this.Deleted || this.m_AngryAt == null)
                return;

            Timer.DelayCall(TimeSpan.FromSeconds(30.0), new TimerCallback(EndSummonDragon));
        }

        public virtual void BeginRemove(TimeSpan delay)
        {
            Timer.DelayCall(delay, new TimerCallback(EndRemove));
        }

        public virtual void EndRemove()
        {
            if (this.Deleted)
                return;

            Point3D loc = this.Location;
            Map map = this.Map;

            Effects.SendLocationParticles(EffectItem.Create(loc, map, EffectItem.DefaultDuration), 0x3728, 10, 10, 0, 0, 2023, 0);
            Effects.PlaySound(loc, map, 0x1FE);

            this.Delete();
        }

        public virtual void EndSummonDragon()
        {
            if (this.Deleted || this.m_AngryAt == null)
                return;

            Map map = this.m_AngryAt.Map;

            if (map == null)
                return;

            if (!this.m_AngryAt.Region.IsPartOf("Doom"))
                return;

            this.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1050015); // Feel the wrath of my legions!!!
            this.PublicOverheadMessage(MessageType.Regular, 0x3B2, false, "MUHAHAHAHA HAHAH HAHA"); // A wee bit crazy, aren't we?

            SkeletalDragon dragon = new SkeletalDragon();

            int offset = Utility.Random(8) * 2;

            bool foundLoc = false;

            for (int i = 0; i < m_Offsets.Length; i += 2)
            {
                int x = this.m_AngryAt.X + m_Offsets[(offset + i) % m_Offsets.Length];
                int y = this.m_AngryAt.Y + m_Offsets[(offset + i + 1) % m_Offsets.Length];

                if (map.CanSpawnMobile(x, y, this.m_AngryAt.Z))
                {
                    dragon.MoveToWorld(new Point3D(x, y, this.m_AngryAt.Z), map);
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
                dragon.MoveToWorld(this.m_AngryAt.Location, map);

            dragon.Combatant = this.m_AngryAt;

            if (this.m_Bell != null)
                this.m_Bell.Dragon = dragon;
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (dropped is GoldenSkull)
            {
                dropped.Delete();

                this.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1050046, from.Name); // Very well, ~1_NAME~, I accept your token. You may cross.
                this.BeginRemove(TimeSpan.FromSeconds(4.0));

                Party p = PartySystem.Party.Get(from);

                if (p != null)
                {
                    for (int i = 0; i < p.Members.Count; ++i)
                    {
                        PartyMemberInfo pmi = (PartyMemberInfo)p.Members[i];
                        Mobile member = pmi.Mobile;

                        if (member != from && member.Map == Map.Malas && member.Region.IsPartOf("Doom"))
                        {
                            if (this.m_AngryAt == member)
                                this.m_AngryAt = null;

                            member.CloseGump(typeof(ChylothPartyGump));
                            member.SendGump(new ChylothPartyGump(from, member));
                        }
                    }
                }

                if (this.m_AngryAt == from)
                    this.m_AngryAt = null;

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
            this.m_Leader = leader;
            this.m_Member = member;

            this.Closable = false;

            this.AddPage(0);

            this.AddImage(0, 0, 3600);

            this.AddImageTiled(0, 14, 15, 200, 3603);
            this.AddImageTiled(380, 14, 14, 200, 3605);
            this.AddImage(0, 201, 3606);
            this.AddImageTiled(15, 201, 370, 16, 3607);
            this.AddImageTiled(15, 0, 370, 16, 3601);
            this.AddImage(380, 0, 3602);
            this.AddImage(380, 201, 3608);
            this.AddImageTiled(15, 15, 365, 190, 2624);

            this.AddRadio(30, 140, 9727, 9730, true, 1);
            this.AddHtmlLocalized(65, 145, 300, 25, 1050050, 0x7FFF, false, false); // Yes, let's go!

            this.AddRadio(30, 175, 9727, 9730, false, 0);
            this.AddHtmlLocalized(65, 178, 300, 25, 1050049, 0x7FFF, false, false); // No thanks, I'd rather stay here.

            this.AddHtmlLocalized(30, 20, 360, 35, 1050047, 0x7FFF, false, false); // Another player has paid Chyloth for your passage across lake Mortis:

            this.AddHtmlLocalized(30, 105, 345, 40, 1050048, 0x5B2D, false, false); // Do you wish to accept their invitation at this time?

            this.AddImage(65, 72, 5605);

            this.AddImageTiled(80, 90, 200, 1, 9107);
            this.AddImageTiled(95, 92, 200, 1, 9157);

            this.AddLabel(90, 70, 1645, leader.Name);

            this.AddButton(290, 175, 247, 248, 2, GumpButtonType.Reply, 0);

            this.AddImageTiled(15, 14, 365, 1, 9107);
            this.AddImageTiled(380, 14, 1, 190, 9105);
            this.AddImageTiled(15, 205, 365, 1, 9107);
            this.AddImageTiled(15, 14, 1, 190, 9105);
            this.AddImageTiled(0, 0, 395, 1, 9157);
            this.AddImageTiled(394, 0, 1, 217, 9155);
            this.AddImageTiled(0, 216, 395, 1, 9157);
            this.AddImageTiled(0, 0, 1, 217, 9155);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 2 && info.IsSwitched(1))
            {
                if (this.m_Member.Region.IsPartOf("Doom"))
                {
                    this.m_Leader.SendLocalizedMessage(1050054, this.m_Member.Name); // ~1_NAME~ has accepted your invitation to cross lake Mortis.

                    Chyloth.TeleportToFerry(this.m_Member);
                }
                else
                {
                    this.m_Member.SendLocalizedMessage(1050051); // The invitation has been revoked.
                }
            }
            else
            {
                this.m_Member.SendLocalizedMessage(1050052); // You have declined their invitation.
                this.m_Leader.SendLocalizedMessage(1050053, this.m_Member.Name); // ~1_NAME~ has declined your invitation to cross lake Mortis.
            }
        }
    }
}