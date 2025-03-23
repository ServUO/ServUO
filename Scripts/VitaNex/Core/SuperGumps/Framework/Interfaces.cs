#region Header
//               _,-'/-'/
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2023  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #                                       #
#endregion

namespace VitaNex.SuperGumps
{
	public interface ISuperGumpPages
	{
		int EntriesPerPage { get; set; }
		int PageCount { get; }
		int Page { get; set; }

		void PreviousPage();
		void PreviousPage(int delta);

		void NextPage();
		void NextPage(int delta);

		void FirstPage();
		void LastPage();
	}
}
