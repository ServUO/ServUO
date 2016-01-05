using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a raptor corpse")]
    public class Raptor : BaseCreature
    {
        public static Type[] VArtifacts =
        {
            typeof (RaptorClaw)
        };

        private bool firstSummoned = true;
        private DateTime recoverDelay;

        [Constructable]
        public Raptor()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a raptor";
            Body = 730;

            SetStr(407, 455);
            SetDex(139, 153);
            SetInt(104, 135);

            SetHits(347, 392);

            SetDamage(11, 17);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 45, 50);
            SetResistance(ResistanceType.Fire, 50, 60);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 20, 30);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 75.5, 89.0);
            SetSkill(SkillName.Tactics, 80.3, 93.8);
            SetSkill(SkillName.Wrestling, 66.9, 81.5);

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 65.1;

            Fame = 7500;
            Karma = -7500;

            QLPoints = 20;

            //	VirtualArmor = 25;    
        }

        public Raptor(Serial serial) : base(serial)
        {
        }

        public override int Meat
        {
            get { return 7; }
        }

        public override int Hides
        {
            get { return 11; }
        }

        public override HideType HideType
        {
            get { return HideType.Horned; }
        }

        public override PackInstinct PackInstinct
        {
            get { return PackInstinct.Ostard; }
        }

        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.BleedAttack;
        }

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

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.25)
            {
                c.DropItem(new AncientPotteryFragments());
            }

            if (Utility.RandomDouble() < 0.05)
            {
                c.DropItem(new RaptorTeeth());
            }

            if (c != null && !c.Deleted && c is Corpse)
            {
                var corpse = (Corpse) c;
                if (Utility.RandomDouble() < 0.01 && corpse.Killer != null && !corpse.Killer.Deleted)
                {
                    GiveVArtifactTo(corpse.Killer);
                }
            }
        }

        public static void GiveVArtifactTo(Mobile m)
        {
            var item = (Item) Activator.CreateInstance(VArtifacts[Utility.Random(VArtifacts.Length)]);

            if (m.AddToBackpack(item))
                m.SendLocalizedMessage(1062317);
                    // For your valor in combating the fallen beast, a special artifact has been bestowed on you.
            else
                m.SendMessage("As your backpack is full, your reward has been placed at your feet.");
            {
            }
        }

        public void SpawnRaptors(Mobile target)
        {
            var map = Map;

            if (map == null)
                return;

            var newRaptors = Utility.RandomMinMax(1, 2);

            for (var i = 0; i < newRaptors; ++i)
            {
                var raptor = new Raptor();

                raptor.Team = Team;
                raptor.FightMode = FightMode.Closest;

                var validLocation = false;
                var loc = Location;

                for (var j = 0; !validLocation && j < 10; ++j)
                {
                    var x = X + Utility.Random(3) - 1;
                    var y = Y + Utility.Random(3) - 1;
                    var z = map.GetAverageZ(x, y);

                    if (validLocation = map.CanFit(x, y, Z, 16, false, false))
                        loc = new Point3D(x, y, Z);
                    else if (validLocation = map.CanFit(x, y, z, 16, false, false))
                        loc = new Point3D(x, y, z);
                }

                raptor.MoveToWorld(loc, map);
                raptor.Combatant = target;
            }
        }

        public void DoSpecialAbility(Mobile target)
        {
            if (0.03 >= Utility.RandomDouble())
                SpawnRaptors(target);
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            DoSpecialAbility(defender);

            defender.Damage(Utility.Random(20, 10), this);
            defender.Stam -= Utility.Random(20, 10);
            defender.Mana -= Utility.Random(20, 10);
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            DoSpecialAbility(attacker);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            var version = reader.ReadInt();
        }
    }
}