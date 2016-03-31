using System;

namespace Server.Items
{
    public class FlameSpurtTrap : BaseTrap
    {
        private Item m_Spurt;
        private Timer m_Timer;
        [Constructable]
        public FlameSpurtTrap()
            : base(0x1B71)
        {
            this.Visible = false;
        }

        public FlameSpurtTrap(Serial serial)
            : base(serial)
        {
        }

        public virtual void StartTimer()
        {
            if (this.m_Timer == null)
                this.m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0), new TimerCallback(Refresh));
        }

        public virtual void StopTimer()
        {
            if (this.m_Timer != null)
                this.m_Timer.Stop();

            this.m_Timer = null;
        }

        public virtual void CheckTimer()
        {
            Map map = this.Map;

            if (map != null && map.GetSector(this.GetWorldLocation()).Active)
                this.StartTimer();
            else
                this.StopTimer();
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            base.OnLocationChange(oldLocation);

            this.CheckTimer();
        }

        public override void OnMapChange()
        {
            base.OnMapChange();

            this.CheckTimer();
        }

        public override void OnSectorActivate()
        {
            base.OnSectorActivate();

            this.StartTimer();
        }

        public override void OnSectorDeactivate()
        {
            base.OnSectorDeactivate();

            this.StopTimer();
        }

        public override void OnDelete()
        {
            base.OnDelete();

            if (this.m_Spurt != null)
                this.m_Spurt.Delete();
        }

        public virtual void Refresh()
        {
            if (this.Deleted)
                return;

            bool foundPlayer = false;

            foreach (Mobile mob in this.GetMobilesInRange(3))
            {
                if (!mob.Player || !mob.Alive || mob.IsStaff())
                    continue;

                if (((this.Z + 8) >= mob.Z && (mob.Z + 16) > this.Z))
                {
                    foundPlayer = true;
                    break;
                }
            }

            if (!foundPlayer)
            {
                if (this.m_Spurt != null)
                    this.m_Spurt.Delete();

                this.m_Spurt = null;
            }
            else if (this.m_Spurt == null || this.m_Spurt.Deleted)
            {
                this.m_Spurt = new Static(0x3709);
                this.m_Spurt.MoveToWorld(this.Location, this.Map);

                Effects.PlaySound(this.GetWorldLocation(), this.Map, 0x309);
            }
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (m.IsPlayer())
                return true;

            if (m.Player && m.Alive)
            {
                this.CheckTimer();

                Spells.SpellHelper.Damage(TimeSpan.FromTicks(1), m, m, Utility.RandomMinMax(1, 30));
                m.PlaySound(m.Female ? 0x327 : 0x437);
            }

            return false;
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            base.OnMovement(m, oldLocation);

            if (m.Location == oldLocation || !m.Player || !m.Alive || m.IsStaff())
                return;

            if (this.CheckRange(m.Location, oldLocation, 1))
            {
                this.CheckTimer();

                Spells.SpellHelper.Damage(TimeSpan.FromTicks(1), m, m, Utility.RandomMinMax(1, 10));
                m.PlaySound(m.Female ? 0x327 : 0x437);

                if (m.Body.IsHuman)
                    m.Animate(20, 1, 1, true, false, 0);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((Item)this.m_Spurt);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        Item item = reader.ReadItem();

                        if (item != null)
                            item.Delete();

                        this.CheckTimer();

                        break;
                    }
            }
        }
    }
}