using System.Collections.Generic;
using System.Threading.Tasks;
using NLog;

namespace HeatRobot.Commands{
	using ChatSharp;

	// ReSharper disable once UnusedMember.Global
	// Loaded via reflection
	[PermissionLevel]
	public class Ping : ICommand{
		internal const string Usage = "Usage: ping";
		internal const string Epilogue = "This command has no options";
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
		static Ping(){Program.RegisterCommand(nameof(Ping), new Ping());}

		public async Task HandleCommand(HeatBot bot, PrivateMessage e, IList<string> args){e.Channel.SendMessage("Pong");}

		public async Task Help(HeatBot bot, PrivateMessage e, IList<string> args){e.Channel.SendMessage(">_>");}
	}
}
