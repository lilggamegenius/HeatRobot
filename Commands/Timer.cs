namespace HeatRobot.Commands{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using ChatSharp;
	using global::HeatBot.Utils;
	using NLog;
	using NMaier.GetOptNet;
	using Time = System.Threading.Timer;
	using Stopwatch = System.Timers.Timer;
	[PermissionLevel]
	public class Timer : ICommand{
		internal const string Usage = "Usage: timer <Command>";
		internal const string Epilogue = "This command has no options";
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
		private List<Time> Timers = new List<Time>();
		static Timer(){
			Timer info = new Timer();
			Program.RegisterCommand(nameof(Timer), info);
		}

		public async Task HandleCommand(HeatBot bot, PrivateMessage e, IList<string> args){
			e.Channel.SendMessage("60 seconds before we start");

			/*TimerOptions opts = new TimerOptions();
			try{
				opts.Parse(args);

			}
			catch(GetOptException){
				await Help(bot, e, null);
			}*/
		}

		public async Task Help(HeatBot bot, PrivateMessage e, IList<string> args){
			e.Channel.SendMessage(new TimerOptions().AssembleUsage(int.MaxValue), e.User);
		}
	}

	[GetOptOptions(OnUnknownArgument = UnknownArgumentsAction.Ignore, UsageIntro = Timer.Usage, UsageEpilog = Timer.Epilogue)]
	public class TimerOptions : GetOpt{
		[Parameters(Exact = 1)] public List<string> Parameters = new List<string>();
	}
}
