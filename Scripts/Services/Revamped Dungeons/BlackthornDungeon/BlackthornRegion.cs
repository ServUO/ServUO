using System;
using System.Collections;
using System.Xml;
using Server;
using Server.Mobiles;
using Server.Network;

namespace Server.Regions
{
    public class BlackthornCastle : MondainRegion
    {
        public BlackthornCastle(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        {
        }

        private Hashtable m_Table;

        public Hashtable Table
        {
            get { return m_Table; }
        }

        public override void OnEnter(Mobile m)
        {
            base.OnEnter(m);

            if (m.Player && m.Mounted && m.AccessLevel == AccessLevel.Player)
            {
                ((BaseMount)m).Rider = null;
            }

            if (m is BaseCreature)
                this.StartTimer(m);
        }

        public override void OnLocationChanged(Mobile m, Point3D oldLocation)
        {
            base.OnLocationChanged(m, oldLocation);

            this.StopTimer(m);

            if (m is BaseCreature)
                this.StartTimer(m);
        }

        public void StartTimer(Mobile m)
        {
            if (this.m_Table == null)
                this.m_Table = new Hashtable();

            m_Table[m] = Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(1), new TimerStateCallback(Stable), m);
        }

        public void StopTimer(Mobile m)
        {
            if (this.m_Table == null)
                this.m_Table = new Hashtable();

            if (this.m_Table[m] != null)
            {
                Timer timer = (Timer)this.m_Table[m];

                timer.Stop();
            }
        }

        public void Stable(object state)
        {
            if (state is Mobile)
                this.Stable((Mobile)state);
        }

        public virtual void Stable(Mobile m)
        {
            BaseCreature pet = (BaseCreature)m;

            if (pet.ControlMaster != null)
            {
                pet.ControlTarget = null;
                pet.ControlOrder = OrderType.Stay;
                pet.Internalize();
                pet.SummonMaster = null;
                pet.IsStabled = true;
                pet.StabledBy = pet.ControlMaster;

                pet.ControlMaster.Stabled.Add(pet);

                pet.ControlMaster.SendLocalizedMessage(1153050, pet.Name); //Pets are not permitted in this location. Your pet named ~1_NAME~ has been sent to the stables.
                pet.SetControlMaster(null);

                this.StopTimer(m);
            }
        }
    }

    public class BlackthornDungeonRegion : MondainRegion
    {
        private static readonly Point3D[] Random_Locations =
        {
            new Point3D(6459, 2781, 0),
            new Point3D(6451, 2781, 0),
            new Point3D(6443, 2781, 0), 
            new Point3D(6409, 2792, 0),
            new Point3D(6356, 2781, 0), 
            new Point3D(6272, 2702, 0), 
            new Point3D(6272, 2656, 0),
            new Point3D(6456, 2623, 0),
        };

        public BlackthornDungeonRegion(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        {
        }

        private Hashtable m_Table;

        public Hashtable Table
        {
            get { return m_Table; }
        }

        public override void OnLocationChanged(Mobile m, Point3D oldLocation)
        {
            base.OnLocationChanged(m, oldLocation);

            StopTimer(m);

            if (0.01 > Utility.RandomDouble() && m.IsPlayer() && m.Alive && m.AccessLevel == AccessLevel.Player)
                StartTimer(m);
        }

        public override void OnExit(Mobile m)
        {
            base.OnExit(m);

            StopTimer(m);
        }

        public void StartTimer(Mobile m)
        {
            if (m_Table == null)
                m_Table = new Hashtable();

            m_Table[m] = Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(5), new TimerStateCallback(Damage), m);
        }

        public void StopTimer(Mobile m)
        {
            if (m_Table == null)
                m_Table = new Hashtable();

            if (m_Table[m] != null)
            {
                Timer timer = (Timer)m_Table[m];

                timer.Stop();
            }
        }

        public void Damage(object state)
        {
            if (state is Mobile)
                Damage((Mobile)state);
        }

        public virtual void Damage(Mobile m)
        {
            if (m.IsPlayer() && !m.Alive)
                StopTimer(m);
            
            switch (Utility.Random(3))
            {
                case 0:
                    m.Location = Random_Locations[Utility.Random(Random_Locations.Length)];
                    break;
                case 1:
                    m.RevealingAction();
                    break;
                case 2:
                    PoisonGas(m);                    
                    break;
            } 
        }

        public virtual void PoisonGas(Mobile m)
        {
            if (m.Location != Point3D.Zero)
            {
                Effects.SendLocationEffect(m.Location, m.Map, 4518, 16, 1, 1166, 0);
                Effects.PlaySound(m.Location, m.Map, 0x231);
                m.LocalOverheadMessage(MessageType.Regular, 0x22, 500855); // You are enveloped by a noxious gas cloud!                
                m.ApplyPoison(m, Poison.Lethal);                             
            }
        }
    }
}
