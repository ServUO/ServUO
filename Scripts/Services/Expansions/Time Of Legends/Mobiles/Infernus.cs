using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an infernus corpse")]
    public class Infernus : BaseCreature
    {
        [Constructable]
        public Infernus()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, .2, .4)
        {
            Name = "an infernus";
            Body = 0x9F;
            Hue = 1955;
            BaseSoundID = 278;

            SetStr(318, 400);
            SetDex(97, 115);
            SetInt(184, 266);

            SetDamage(11, 14);

            SetHits(202, 243);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 80, 90);
            SetResistance(ResistanceType.Cold, 25, 35);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 35, 45);

            SetDamageType(ResistanceType.Fire, 100);

            SetSkill(SkillName.MagicResist, 90, 100);
            SetSkill(SkillName.Tactics, 60, 70);
            SetSkill(SkillName.Wrestling, 80);

            PackGold(500, 600);

            Fame = 10000;
            Karma = -10000;
        }

        private DateTime _NextDrop;

        public override void OnActionCombat()
        {
            Mobile combatant = Combatant as Mobile;

            if (DateTime.UtcNow < _NextDrop || combatant == null || combatant.Deleted || combatant.Map != Map || !InRange(combatant, 12))
                return;

            DropFire(combatant);

            _NextDrop = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(10, 15));
        }

        public void DropFire(Mobile m)
        {
            for (int x = m.X - 1; x <= m.X + 1; x++)
            {
                for (int y = m.Y - 1; y <= m.Y + 1; y++)
                {
                    IPoint3D p = new Point3D(x, y, Map.GetAverageZ(x, y)) as IPoint3D;
                    Server.Spells.SpellHelper.GetSurfaceTop(ref p);

                    if (((x == 0 && y == 0) || .5 > Utility.RandomDouble()) && Map.CanSpawnMobile(p.X, p.Y, p.Z))
                    {
                        var item = new FireItem(this);
                        item.MoveToWorld(new Point3D(p), this.Map);
                    }
                }
            }
        }

        public class FireItem : Item
        {
            public Infernus Mobile { get; private set; }
            public Timer Timer { get; private set; }

            private DateTime _EndTime;

            public FireItem(Infernus mobile)
                : base(0x19AB)
            {
                Mobile = mobile;

                _EndTime = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(30, 40));

                Timer = Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), OnTick);
            }

            public override bool OnMoveOver(Mobile from)
            {
                if (Mobile != null && Mobile != from && Server.Spells.SpellHelper.ValidIndirectTarget(Mobile, from) && Mobile.CanBeHarmful(from, false))
                {
                    Mobile.DoHarmful(from);

                    AOS.Damage(from, Mobile, Utility.RandomMinMax(50, 85), 0, 100, 0, 0, 0);
                    Effects.PlaySound(this.Location, this.Map, 0x1DD);
                    from.PrivateOverheadMessage(Server.Network.MessageType.Regular, 0x20, 1156084, from.NetState); // *The trail of fire scorches you, setting you ablaze!*
                }

                return true;
            }

            public void OnTick()
            {
                if (_EndTime < DateTime.UtcNow)
                {
                    Delete();
                }
                else
                {
                    IPooledEnumerable eable = this.Map.GetMobilesInRange(this.Location, 0);

                    foreach (Mobile m in eable)
                        OnMoveOver(m);

                    eable.Free();
                }
            }

            public override void Delete()
            {
                base.Delete();

                if (Timer != null)
                {
                    Timer.Stop();
                    Timer = null;
                }
            }

            public FireItem(Serial serial)
                : base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
            }

            public override void Deserialize(GenericReader reader)
            {
            }
        }

        public Infernus(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}