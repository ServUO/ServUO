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

#region References
using System;
using System.Drawing;
using System.Text.RegularExpressions;
#endregion

namespace VitaNex.Text
{
	public static class BBCodeUtility
	{
		/*
		(Description)		(Syntax)						(Args)				(Examples)
		Line Break:			[br]												[br]
		URL:				[url] link [/url]									[url]http://www.google.com[/url]
		URL Labeled:		[url=arg] text [/url]			hyperlink			[url=http://www.google.com]Google Search[/url]
		Center Align:		[center] text [/center]								[center]Hello World[/center]
		Left Align:			[left] text [/left]									[left]Hello World[/left]
		Right Align:		[right] text [/right]								[right]Hello World[/right]
		Small Font:			[small] text [/small]								[small]Hello World[/small]
		Big Font:			[big] text [/big]									[big]Hello World[/big]
		Bold:				[b] text [/b]										[b]Hello World[/b]
		Italic:				[i] text [/i]										[i]Hello World[/i]
		Underline:			[u] text [/u]										[u]Hello World[/u]
		Strikeout:			[s] text [/s]										[s]Hello World[/s]
		Font Size:			[size=arg] text [/size]			int 1 - 4			[size=4]Hello World[/color]
		Font Color:			[color=arg] text [/color]		hex	x6				[color=#FFFFFF]Hello World[/color]
		**********:			*************************		named color			[color=white]Hello World[/color]
		*/

		public static RegexOptions DefaultRegexOptions = RegexOptions.IgnoreCase | RegexOptions.Singleline;

		public static readonly Regex RegexLineBreak = new Regex(@"\[br\]", DefaultRegexOptions),
									 RegexCenterText = new Regex(@"\[center\](.+?)\[\/center\]", DefaultRegexOptions),
									 RegexLeftText = new Regex(@"\[left\](.+?)\[\/left\]", DefaultRegexOptions),
									 RegexRightText = new Regex(@"\[right\](.+?)\[\/right\]", DefaultRegexOptions),
									 RegexSmallText = new Regex(@"\[small\](.+?)\[\/small\]", DefaultRegexOptions),
									 RegexBigText = new Regex(@"\[big\](.+?)\[\/big\]", DefaultRegexOptions),
									 RegexBoldText = new Regex(@"\[b\](.+?)\[\/b\]", DefaultRegexOptions),
									 RegexItalicText = new Regex(@"\[i\](.+?)\[\/i\]", DefaultRegexOptions),
									 RegexUnderlineText = new Regex(@"\[u\](.+?)\[\/u\]", DefaultRegexOptions),
									 RegexStrikeOutText = new Regex(@"\[s\](.+?)\[\/s\]", DefaultRegexOptions),
									 RegexUrl = new Regex(@"\[url\](.+?)\[\/url\]", DefaultRegexOptions),
									 RegexUrlAnchored = new Regex(@"\[url=([^\]]+)\]([^\]]+)\[\/url\]", DefaultRegexOptions),
									 RegexColorAnchored = new Regex(@"\[color=([^\]]+)\]([^\]]+)\[\/color\]", DefaultRegexOptions),
									 RegexSizeAnchored = new Regex(@"\[size=([^\]]+)\]([^\]]+)\[\/size\]", DefaultRegexOptions),
									 RegexImage = new Regex(@"\[img\](.+?)\[\/img\]", DefaultRegexOptions),
									 RegexImageAnchored = new Regex(@"\[img=([^\]]+)\]([^\]]+)\[\/img\]", DefaultRegexOptions),
									 RegexStripMisc = new Regex(@"\[([^\]]+)\]([^\]]+)\[\/[^\]]+\]", DefaultRegexOptions);

		public static string ParseBBCode(
			this string input,
			Color? defaultColor = null,
			int defaultSize = 2,
			bool imgAsLink = true,
			bool stripMisc = false)
		{
			if (String.IsNullOrWhiteSpace(input))
			{
				return input ?? String.Empty;
			}

			input = RegexLineBreak.Replace(input, "<br>");
			input = RegexCenterText.Replace(input, "<center>$1</center>");
			input = RegexLeftText.Replace(input, "<left>$1</left>");
			input = RegexRightText.Replace(input, "<right>$1</right>");
			input = RegexSmallText.Replace(input, "<small>$1</small>");
			input = RegexBigText.Replace(input, "<big>$1</big>");
			input = RegexBoldText.Replace(input, "<b>$1</b>");
			input = RegexItalicText.Replace(input, "<i>$1</i>");
			input = RegexUnderlineText.Replace(input, "<u>$1</u>");
			input = RegexStrikeOutText.Replace(input, "<s>$1</s>");

			input = RegexUrl.Replace(input, "<a href=\"$1\">$1</a>");
			input = RegexUrlAnchored.Replace(input, "<a href=\"$1\">$2</a>");

			if (imgAsLink)
			{
				input = RegexImage.Replace(input, "<a href=\"$1\">$1</a>");
				input = RegexImageAnchored.Replace(input, "<a href=\"$1\">$2</a>");
			}

			input = RegexSizeAnchored.Replace(input, "<basefont size=$1>$2<basefont size=" + defaultSize + ">");

			if (defaultColor != null)
			{
				input = RegexColorAnchored.Replace(
					input,
					"<basefont color=$1>$2<basefont color=#" + defaultColor.Value.ToRgb().ToString("X6") + ">");
			}
			else
			{
				input = RegexColorAnchored.Replace(input, "<basefont color=$1>$2");
			}

			if (stripMisc)
			{
				input = RegexStripMisc.Replace(input, "($1) $2");
			}

			return input;
		}
	}
}