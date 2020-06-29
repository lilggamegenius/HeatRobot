using System.Collections.Generic;
using System.Threading.Tasks;
using NLog;
using NMaier.GetOptNet;
using HeatBot.Utils;
using ChatSharp;

namespace HeatRobot.Commands{
	[PermissionLevel]
	public class Info : ICommand{
		internal const string Usage = "Usage: help <Command>";
		internal const string Epilogue = "This command has no options";
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
		static Info(){
			Info info = new Info();
			Program.RegisterCommand(nameof(Info), info);
			Program.RegisterCommand("Help", info);
		}

		public async Task HandleCommand(HeatBot bot, PrivateMessage e, IList<string> args){
			InfoOptions opts = new InfoOptions();
			try{
				opts.Parse(args);
				if(Program.CommandList.ContainsCommand(opts.Parameters[0])){
					await Program.CommandList[opts.Parameters[0]].Help(bot, e, null);
					return;
				}

				e.Channel.SendMessage("That command doesn't exist.");
			}
			catch(GetOptException){
				await Help(bot, e, null);
			}
		}

		public async Task Help(HeatBot bot, PrivateMessage e, IList<string> args){
			e.Channel.SendMessage(new InfoOptions().AssembleUsage(int.MaxValue), e.User);
		}
	}

	[GetOptOptions(OnUnknownArgument = UnknownArgumentsAction.Ignore, UsageIntro = Info.Usage, UsageEpilog = Info.Epilogue)]
	public class InfoOptions : GetOpt{
		[Parameters(Exact = 1)] public List<string> Parameters = new List<string>();
	}
}
