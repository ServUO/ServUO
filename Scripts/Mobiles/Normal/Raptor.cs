using Server.Items;
using System;
using System.Collections.Generic;
namespace Server.Mobiles
{
    [CorpseName("a raptor corpse")]
    public class Raptor : BaseCreature
    {
        private const int MaxFriends = 2;

        private bool m_IsFriend;
        private readonly List<Mobile> m_Friends = new List<Mobile>();
        private InternalTimer m_FriendsTimer;

        [Constructable]
        public Raptor()
            : this(false)
        {
        }

        [Constructable]
        public Raptor(bool isFriend)
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.175, 0.350)
        {
            m_IsFriend = isFriend;

            Name = "a raptor";
            Body = 730;

            SetStr(404, 471);
            SetDex(132, 155);
            SetInt(105, 145);

            SetHits(343, 400);

            SetDamage(11, 17);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 45, 50);
            SetResistance(ResistanceType.Fire, 50, 60);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 20, 30);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 75.1, 90.0);
            SetSkill(SkillName.Tactics, 75.1, 100.0);
            SetSkill(SkillName.Wrestling, 70.1, 95.1);

            Fame = 7500;
            Karma = -7500;

            Tamable = !isFriend;
            MinTameSkill = 107.1;
            ControlSlots = 2;

            SetWeaponAbility(WeaponAbility.BleedAttack);
        }

        public override int TreasureMapLevel => 3;

        public override int Meat => 7;

        public override int Hides => 11;

        public override HideType HideType => HideType.Horned;

        public override PackInstinct PackInstinct => PackInstinct.Ostard;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 2);
        }

        public override int GetIdleSound()
        {
            return 1573;
        }

        public override int GetAngerSound()
        {
            return 1570;
        }

        public override int GetHurtSound()
        {
            return 1572;
        }

        public override int GetDeathSound()
        {
            return 1571;
        }


        public override void OnCombatantChange()
        {
            if (!m_IsFriend && !Controlled && Combatant != null && m_FriendsTimer == null)
            {
                m_FriendsTimer = new InternalTimer(this);
                m_FriendsTimer.Start();
            }
        }

        public void CheckFriends()
        {
            if (!Alive || Combatant == null || Controlled || Map == null || Map == Map.Internal)
            {
                m_Friends.ForEach(f => f.Delete());
                m_Friends.Clear();

                m_FriendsTimer.Stop();
                m_FriendsTimer = null;
            }
            else
            {
                int count = 0;

                for (int i = 0; i < m_Friends.Count; i++)
                {
                    // remove dead friends

                    Mobile friend = m_Friends[i];

                    if (friend == null || friend.Deleted)
                        m_Friends.Remove(friend);
                    else
                        count++;
                }

                for (int i = count; i < MaxFriends; i++)
                {
                    // spawn new friends

                    BaseCreature friend = new Raptor(true);
                    Point3D loc = Location;
                    bool validLocation = false;
                    for (int j = 0; !validLocation && j < 10; ++j)
                    {
                        int x = X + Utility.Random(3) - 1;
                        int y = Y + Utility.Random(3) - 1;
                        int z = Map.GetAverageZ(x, y);

                        if (validLocation = Map.CanFit(x, y, Z, 16, false, false))
                            loc = new Point3D(x, y, Z);
                        else if (validLocation = Map.CanFit(x, y, z, 16, false, false))
                            loc = new Point3D(x, y, z);
                    }

                    friend.MoveToWorld(loc, Map);
                    friend.Combatant = Combatant;

                    if (friend.AIObject != null)
                        friend.AIObject.Action = ActionType.Combat;

                    m_Friends.Add(friend);
                }
            }
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (!Controlled && Utility.RandomDouble() < 0.25)
            {
                c.DropItem(new AncientPotteryFragments());
            }

            if (!Controlled && Utility.RandomDouble() <= 0.005)
            {
                c.DropItem(new RaptorClaw());
            }
        }

        public Raptor(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(2);

            writer.Write(m_IsFriend);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version > 0)
                m_IsFriend = reader.ReadBool();

            if (version == 1)
                SetWeaponAbility(WeaponAbility.BleedAttack);

            if (m_IsFriend)
                Delete();
        }

        private class InternalTimer : Timer
        {
            private readonly Raptor m_Owner;

            public InternalTimer(Raptor owner)
                : base(TimeSpan.Zero, TimeSpan.FromSeconds(30.0))
            {
                m_Owner = owner;
            }

            protected override void OnTick()
            {
                m_Owner.CheckFriends();
            }
        }
    }
}
