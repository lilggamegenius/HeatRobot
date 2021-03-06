﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NLog;

namespace HeatBot.Utils{
	using ChatSharp;

	public static class IrcUtils{
		public const char CtcpChar = '\u0001';
		public const char ColorChar = '\u0003';
		public const char BoldChar = '\u0002';
		public const char ItalicsChar = '\u001D';
		public const char UnderlineChar = '\u001F';
		public const char ReverseChar = '\u0016';
		public const char BellChar = '\u0007';
		public const char SymbolForBellChar = '␇';
		public const char NewLineChar = '\n';
		public const char SymbolForNewLineChar = '␤';
		public const char CharageReturnChar = '\r';
		public const char SymbolForCharageReturnChar = '␍';

		private const string EscapePrefix = "@!";
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ToColor(this string str, byte color)=>color >= 16 ? $"{ColorChar}{str}" : $"{ColorChar}{color:00}{str}{ColorChar}";

		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static string ToBold(this string str)=>$"{BoldChar}{str}{BoldChar}";

		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static string ToItalics(this string str)=>$"{ItalicsChar}{str}{ItalicsChar}";

		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static string ToUnderline(this string str)=>$"{UnderlineChar}{str}{UnderlineChar}";

		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static string ToCtcp(this string str)=>$"{CtcpChar}{str}{CtcpChar}";

		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsCtcp(this string str)=>str.StartsWith(CtcpChar.ToString()) && str.EndsWith(CtcpChar.ToString());

		public static void SendMessage(this IrcChannel channel, string message, IrcUser user){
			channel.SendMessage($"{user.Nick}: {message}");
		}

		public static bool Contains(this string str, char ch){
			if(str == null){ return false; }

			foreach(char c in str){
				if(c == ch){ return true; }
			}

			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string SanitizeForIRC(this string str)=>str.Replace(BellChar, SymbolForBellChar)
																 .Replace(NewLineChar, SymbolForNewLineChar)
																 .Replace(CharageReturnChar, SymbolForCharageReturnChar)
																 .Replace("\0", string.Empty);
		public static bool MatchHostMask(this string hostmask, string pattern){
			string nick = hostmask.Substring(0, hostmask.IndexOf("!", StringComparison.Ordinal));
			string userName = hostmask.Substring(hostmask.IndexOf("!", StringComparison.Ordinal) + 1, hostmask.IndexOf("@", StringComparison.Ordinal));
			string hostname = hostmask.Substring(hostmask.IndexOf("@", StringComparison.Ordinal) + 1);
			string patternNick = pattern.Substring(0, pattern.IndexOf("!", StringComparison.Ordinal));
			string patternUserName = pattern.Substring(pattern.IndexOf("!", StringComparison.Ordinal) + 1, pattern.IndexOf("@", StringComparison.Ordinal));
			string patternHostname = pattern.Substring(pattern.IndexOf("@", StringComparison.Ordinal) + 1);
			return hostname.WildCardMatch(patternHostname) && userName.WildCardMatch(patternUserName) && nick.WildCardMatch(patternNick);
		}
	}
}
