using System;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Engines.XmlSpawner2
{
    public class XmlUse : XmlAttachment
    {
        public DateTime m_EndTime;
        private bool m_BlockDefaultUse;
        private string m_Condition;// additional condition required for use
        private string m_TargetingAction;// action performed when the target cursor is brought up
        private string m_TargetCondition;// condition test applied when target is selected to determine whether it is appropriate
        private string m_TargetFailureAction;// action performed if target condition is not met
        private string m_TargetFailureReward;// action performed if target condition is not met
        private string m_SuccessAction;// action performed on successful use or targeting
        private string m_SuccessReward;// action performed on successful use or targeting
        private string m_FailureAction;// action performed if the player cannot use the object for reasons other than range, refractory, or maxuses
        private string m_FailureReward;// action performed if the player cannot use the object for reasons other than range, refractory, or maxuses
        private string m_RefractoryAction;// action performed if the object is used before the refractory interval expires
        private string m_RefractoryReward;// action performed if the object is used before the refractory interval expires
        private string m_MaxUsesAction;// action performed if the object is used when the maxuses are exceeded
        private string m_MaxUsesReward;// action performed if the object is used when the maxuses are exceeded
        private int m_NUses = 0;
        private int m_MaxRange = 3;// must be within 3 tiles to use by default
        private int m_MaxTargetRange = 30;// must be within 30 tiles to target by default
        private int m_MaxUses = 0;
        private TimeSpan m_Refractory = TimeSpan.Zero;
        private bool m_RequireLOS = false;
        private bool m_AllowCarried = true;
        private bool m_TargetingEnabled = false;
        public XmlUse(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public XmlUse()
        {
        }

        [Attachable]
        public XmlUse(int maxuses)
        {
            this.MaxUses = maxuses;
        }

        [Attachable]
        public XmlUse(int maxuses, double refractory)
        {
            this.MaxUses = maxuses;
            this.Refractory = TimeSpan.FromSeconds(refractory);
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool TargetingEnabled
        {
            get
            {
                return this.m_TargetingEnabled;
            }
            set
            {
                this.m_TargetingEnabled = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool AllowCarried
        {
            get
            {
                return this.m_AllowCarried;
            }
            set
            {
                this.m_AllowCarried = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool RequireLOS
        {
            get
            {
                return this.m_RequireLOS;
            }
            set
            {
                this.m_RequireLOS = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxRange
        {
            get
            {
                return this.m_MaxRange;
            }
            set
            {
                this.m_MaxRange = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxTargetRange
        {
            get
            {
                return this.m_MaxTargetRange;
            }
            set
            {
                this.m_MaxTargetRange = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int NUses
        {
            get
            {
                return this.m_NUses;
            }
            set
            {
                this.m_NUses = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxUses
        {
            get
            {
                return this.m_MaxUses;
            }
            set
            {
                this.m_MaxUses = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan Refractory
        {
            get
            {
                return this.m_Refractory;
            }
            set
            {
                this.m_Refractory = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool BlockDefaultUse
        {
            get
            {
                return this.m_BlockDefaultUse;
            }
            set
            {
                this.m_BlockDefaultUse = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string Condition
        {
            get
            {
                return this.m_Condition;
            }
            set
            {
                this.m_Condition = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string TargetCondition
        {
            get
            {
                return this.m_TargetCondition;
            }
            set
            {
                this.m_TargetCondition = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string TargetingAction
        {
            get
            {
                return this.m_TargetingAction;
            }
            set
            {
                this.m_TargetingAction = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string TargetFailureAction
        {
            get
            {
                return this.m_TargetFailureAction;
            }
            set
            {
                this.m_TargetFailureAction = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string TargetFailureReward
        {
            get
            {
                return this.m_TargetFailureReward;
            }
            set
            {
                this.m_TargetFailureReward = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string SuccessAction
        {
            get
            {
                return this.m_SuccessAction;
            }
            set
            {
                this.m_SuccessAction = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string SuccessReward
        {
            get
            {
                return this.m_SuccessReward;
            }
            set
            {
                this.m_SuccessReward = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string FailureAction
        {
            get
            {
                return this.m_FailureAction;
            }
            set
            {
                this.m_FailureAction = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string FailureReward
        {
            get
            {
                return this.m_FailureReward;
            }
            set
            {
                this.m_FailureReward = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string RefractoryAction
        {
            get
            {
                return this.m_RefractoryAction;
            }
            set
            {
                this.m_RefractoryAction = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string RefractoryReward
        {
            get
            {
                return this.m_RefractoryReward;
            }
            set
            {
                this.m_RefractoryReward = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string MaxUsesAction
        {
            get
            {
                return this.m_MaxUsesAction;
            }
            set
            {
                this.m_MaxUsesAction = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string MaxUsesReward
        {
            get
            {
                return this.m_MaxUsesReward;
            }
            set
            {
                this.m_MaxUsesReward = value;
            }
        }
        public bool CheckMaxUses
        {
            get
            {
                // is there a use limit?
                if (this.MaxUses > 0 && this.NUses >= this.MaxUses)
                    return false;

                return true;
            }
        }
        public bool CheckRefractory
        {
            get
            {
                // is there a refractory limit?
                // if it is still refractory then return
                if (this.Refractory > TimeSpan.Zero && DateTime.UtcNow < this.m_EndTime)
                    return false;

                return true;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)3);
            // version 3
            writer.Write(this.m_MaxTargetRange);
            // version 2
            writer.Write(this.m_TargetingEnabled);
            writer.Write(this.m_TargetingAction);
            writer.Write(this.m_TargetCondition);
            writer.Write(this.m_TargetFailureAction);
            writer.Write(this.m_TargetFailureReward);
            // version 1
            writer.Write(this.m_AllowCarried);
            // version 0
            writer.Write(this.m_RequireLOS);
            writer.Write(this.m_MaxRange);
            writer.Write(this.m_Refractory);
            writer.Write(this.m_EndTime - DateTime.UtcNow);
            writer.Write(this.m_MaxUses);
            writer.Write(this.m_NUses);
            writer.Write(this.m_BlockDefaultUse);
            writer.Write(this.m_Condition);
            writer.Write(this.m_SuccessAction);
            writer.Write(this.m_SuccessReward);
            writer.Write(this.m_FailureAction);
            writer.Write(this.m_FailureReward);
            writer.Write(this.m_RefractoryAction);
            writer.Write(this.m_RefractoryReward);
            writer.Write(this.m_MaxUsesAction);
            writer.Write(this.m_MaxUsesReward);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch (version)
            {
                case 3:
                    this.m_MaxTargetRange = reader.ReadInt();
                    goto case 2;
                case 2:
                    this.m_TargetingEnabled = reader.ReadBool();
                    this.m_TargetingAction = reader.ReadString();
                    this.m_TargetCondition = reader.ReadString();
                    this.m_TargetFailureAction = reader.ReadString();
                    this.m_TargetFailureReward = reader.ReadString();
                    goto case 1;
                case 1:
                    this.m_AllowCarried = reader.ReadBool();
                    goto case 0;
                case 0:
                    // version 0
                    this.m_RequireLOS = reader.ReadBool();
                    this.m_MaxRange = reader.ReadInt();
                    this.Refractory = reader.ReadTimeSpan();
                    TimeSpan remaining = reader.ReadTimeSpan();
                    this.m_EndTime = DateTime.UtcNow + remaining;
                    this.m_MaxUses = reader.ReadInt();
                    this.m_NUses = reader.ReadInt();
                    this.m_BlockDefaultUse = reader.ReadBool();
                    this.m_Condition = reader.ReadString();
                    this.m_SuccessAction = reader.ReadString();
                    this.m_SuccessReward = reader.ReadString();
                    this.m_FailureAction = reader.ReadString();
                    this.m_FailureReward = reader.ReadString();
                    this.m_RefractoryAction = reader.ReadString();
                    this.m_RefractoryReward = reader.ReadString();
                    this.m_MaxUsesAction = reader.ReadString();
                    this.m_MaxUsesReward = reader.ReadString();
                    break;
            }
        }

        public void ExecuteActions(Mobile mob, object target, string actions)
        {
            if (actions == null || actions.Length <= 0)
                return;
            // execute any action associated with it
            // allow for multiple action strings on a single line separated by a semicolon

            string[] args = actions.Split(';');

            for (int j = 0; j < args.Length; j++)
            {
                this.ExecuteAction(mob, target, args[j]);
            }
        }

        public void OutOfRange(Mobile from)
        {
            if (from == null)
                return;

            from.SendLocalizedMessage(500446); // That is too far away.
        }

        // disable the default use of the target
        public override bool BlockDefaultOnUse(Mobile from, object target)
        {
            return (this.BlockDefaultUse || !(this.CheckRange(from, target) && this.CheckCondition(from, target) && this.CheckMaxUses && this.CheckRefractory));
        }

        // this is called when the attachment is on the user
        public override void OnUser(object target)
        {
            Mobile from = this.AttachedTo as Mobile;

            this.TryToUse(from, target);
        }

        // this is called when the attachment is on the target being used
        public override void OnUse(Mobile from)
        {
            object target = this.AttachedTo;

            // if a target tries to use itself, then ignore it, it will be handled by OnUser
            if (target == from)
                return;

            this.TryToUse(from, target);
        }

        private void ExecuteAction(Mobile mob, object target, string action)
        {
            if (action == null || action.Length <= 0)
                return;

            string status_str = null;
            Server.Mobiles.XmlSpawner.SpawnObject TheSpawn = new Server.Mobiles.XmlSpawner.SpawnObject(null, 0);

            TheSpawn.TypeName = action;
            string substitutedtypeName = BaseXmlSpawner.ApplySubstitution(null, target, mob, action);
            string typeName = BaseXmlSpawner.ParseObjectType(substitutedtypeName);

            Point3D loc = new Point3D(0, 0, 0);
            Map map = null;

            if (target is Item)
            {
                Item ti = target as Item;
                if (ti.Parent == null)
                {
                    loc = ti.Location;
                    map = ti.Map;
                }
                else if (ti.RootParent is Item)
                {
                    loc = ((Item)ti.RootParent).Location;
                    map = ((Item)ti.RootParent).Map;
                }
                else if (ti.RootParent is Mobile)
                {
                    loc = ((Mobile)ti.RootParent).Location;
                    map = ((Mobile)ti.RootParent).Map;
                }
            }
            else if (target is Mobile)
            {
                Mobile ti = target as Mobile;

                loc = ti.Location;
                map = ti.Map;
            }

            if (BaseXmlSpawner.IsTypeOrItemKeyword(typeName))
            {
                BaseXmlSpawner.SpawnTypeKeyword(target, TheSpawn, typeName, substitutedtypeName, true, mob, loc, map, out status_str);
            }
            else
            {
                // its a regular type descriptor so find out what it is
                Type type = SpawnerType.GetType(typeName);
                try
                {
                    string[] arglist = BaseXmlSpawner.ParseString(substitutedtypeName, 3, "/");
                    object o = Server.Mobiles.XmlSpawner.CreateObject(type, arglist[0]);

                    if (o == null)
                    {
                        status_str = "invalid type specification: " + arglist[0];
                    }
                    else if (o is Mobile)
                    {
                        Mobile m = (Mobile)o;
                        if (m is BaseCreature)
                        {
                            BaseCreature c = (BaseCreature)m;
                            c.Home = loc; // Spawners location is the home point
                        }

                        m.Location = loc;
                        m.Map = map;

                        BaseXmlSpawner.ApplyObjectStringProperties(null, substitutedtypeName, m, mob, target, out status_str);
                    }
                    else if (o is Item)
                    {
                        Item item = (Item)o;
                        BaseXmlSpawner.AddSpawnItem(null, target, TheSpawn, item, loc, map, mob, false, substitutedtypeName, out status_str);
                    }
                }
                catch
                {
                }
            }

            this.ReportError(mob, status_str);
        }

        private void ReportError(Mobile mob, string status_str)
        {
            if (status_str != null && mob != null && !mob.Deleted && mob is PlayerMobile && mob.AccessLevel > AccessLevel.Player)
            {
                mob.SendMessage(33, String.Format("{0}:{1}", this.Name, status_str));
            }
        }

        // return true to allow use
        private bool CheckCondition(Mobile from, object target)
        {
            // test the condition if there is one
            if (this.Condition != null && this.Condition.Length > 0)
            {
                string status_str;

                return BaseXmlSpawner.CheckPropertyString(null, target, this.Condition, from, out status_str);
            }

            return true;
        }

        // return true to allow use
        private bool CheckTargetCondition(Mobile from, object target)
        {
            // test the condition if there is one
            if (this.TargetCondition != null && this.TargetCondition.Length > 0)
            {
                string status_str;

                return BaseXmlSpawner.CheckPropertyString(null, target, this.TargetCondition, from, out status_str);
            }

            return true;
        }

        // return true to allow use
        private bool CheckRange(Mobile from, object target)
        {
            if (from == null || !(target is IEntity) || this.MaxRange < 0)
                return false;

            Map map = ((IEntity)target).Map;
            Point3D loc = ((IEntity)target).Location;

            if (map != from.Map)
                return false;

            // check for allowed use in pack
            if (target is Item)
            {
                Item targetitem = (Item)target;
                // is it carried by the user?
                if (targetitem.RootParent == from)
                {
                    return this.AllowCarried;
                }
                else if (targetitem.Parent != null)
                {
                    return false;
                }
            }

            bool haslos = true;
            if (this.RequireLOS)
            {
                // check los as well
                haslos = from.InLOS(target);
            }

            return from.InRange(loc, this.MaxRange) && haslos;
        }

        private void TryToTarget(Mobile from, object target, XmlUse xa)
        {
            if (from == null)
                return;

            this.ExecuteActions(from, target, this.TargetingAction);

            if (xa != null)
            {
                from.Target = new XmlUseTarget(xa.MaxTargetRange, target, xa);
            }
        }

        private void TryToUse(Mobile from, object target)
        {
            if (this.CheckRange(from, target) && this.CheckCondition(from, target) && this.CheckMaxUses && this.CheckRefractory)
            {
                // check for targeting
                if (this.TargetingEnabled)
                {
                    this.TryToTarget(from, target, this);
                }
                else
                {
                    // success
                    this.ExecuteActions(from, target, this.SuccessAction);
                    this.ExecuteActions(from, target, this.SuccessReward);

                    this.m_EndTime = DateTime.UtcNow + this.Refractory;
                    this.NUses++;
                }
            }
            else
            {
                // failure
                if (!this.CheckRange(from, target))
                {
                    this.OutOfRange(from);
                }
                else if (!this.CheckRefractory)
                {
                    this.ExecuteActions(from, target, this.RefractoryAction);
                    this.ExecuteActions(from, target, this.RefractoryReward);
                }
                else if (!this.CheckMaxUses)
                {
                    this.ExecuteActions(from, target, this.MaxUsesAction);
                    this.ExecuteActions(from, target, this.MaxUsesReward);
                }
                else
                {
                    this.ExecuteActions(from, target, this.FailureAction);
                    this.ExecuteActions(from, target, this.FailureReward);
                }
            }
        }

        public class XmlUseTarget : Target
        {
            private readonly object m_objectused;
            private readonly XmlUse m_xa;
            public XmlUseTarget(int range, object objectused, XmlUse xa)
                : base(range, true, TargetFlags.None)
            {
                this.m_objectused = objectused;
                this.m_xa = xa;
                this.CheckLOS = false;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (from == null || targeted == null || this.m_xa == null)
                    return;

                // success
                if (this.m_xa.CheckTargetCondition(from, targeted))
                {
                    this.m_xa.ExecuteActions(from, targeted, this.m_xa.SuccessAction);
                    this.m_xa.ExecuteActions(from, targeted, this.m_xa.SuccessReward);

                    this.m_xa.m_EndTime = DateTime.UtcNow + this.m_xa.Refractory;
                    this.m_xa.NUses++;
                }
                else
                {
                    this.m_xa.ExecuteActions(from, targeted, this.m_xa.TargetFailureAction);
                    this.m_xa.ExecuteActions(from, targeted, this.m_xa.TargetFailureReward);
                }
            }
        }
    }
}