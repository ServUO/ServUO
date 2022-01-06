using Server.Gumps;

namespace Server.Services.ContainerSort
{
	public class GumpRequestHandler
	{
		public static void Initialize()
		{
			EventSink.ContainerSortSettingsRequest += EventSink_ContainerSortSettingsRequest;
		}

		public static void EventSink_ContainerSortSettingsRequest(ContainerSortSettingsRequestEventArgs e)
		{
			e.Mobile.SendGump(new ContainerSortSettingsGump(e.Mobile, e.Container));
		}
	}
}