using System;

namespace Server.Engines.Harvest
{
    public class HarvestResource
    {
        public HarvestResource(double reqSkill, double minSkill, double maxSkill, object message, params Type[] types)
        {
            ReqSkill = reqSkill;
            MinSkill = minSkill;
            MaxSkill = maxSkill;
            Types = types;
            SuccessMessage = message;
        }

        public Type[] Types { get; }
        public double ReqSkill { get; }
        public double MinSkill { get; }
        public double MaxSkill { get; }
        public object SuccessMessage { get; }

        public void SendSuccessTo(Mobile m)
        {
            if (SuccessMessage is int message)
                m.SendLocalizedMessage(message);

            else if (SuccessMessage is string stringMessage)
                m.SendMessage(stringMessage);
        }
    }
}
