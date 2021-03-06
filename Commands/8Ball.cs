using System.Collections.Generic;
using System.Threading.Tasks;
using NLog;

namespace HeatRobot.Commands{
	using ChatSharp;
	using global::HeatBot.Utils;

	[PermissionLevel]
	public class Magic8Ball : ICommand{
		internal const string Usage = "Usage: 8Ball";
		internal const string Epilogue = "This command has no options";
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
		static Magic8Ball(){
			Magic8Ball magic8Ball = new Magic8Ball();
			Program.RegisterCommand("8Ball", magic8Ball);
		}

		public async Task HandleCommand(HeatBot bot, PrivateMessage e, IList<string> args){
			int choice = LilGUtil.RandInt(1, 20);
			string response = "";
			switch(choice){
				case 1:
					response = "It is certain";
					break;
				case 2:
					response = "It is decidedly so";
					break;
				case 3:
					response = "Without a doubt";
					break;
				case 4:
					response = "Yes - definitely";
					break;
				case 5:
					response = "You may rely on it";
					break;
				case 6:
					response = "As I see it, yes";
					break;
				case 7:
					response = "Most likely";
					break;
				case 8:
					response = "Outlook good";
					break;
				case 9:
					response = "Signs point to yes";
					break;
				case 10:
					response = "Yes";
					break;
				case 11:
					response = "Reply hazy, try again";
					break;
				case 12:
					response = "Ask again later";
					break;
				case 13:
					response = "Better not tell you now";
					break;
				case 14:
					response = "Cannot predict now";
					break;
				case 15:
					response = "Concentrate and ask again";
					break;
				case 16:
					response = "Don't count on it";
					break;
				case 17:
					response = "My reply is no";
					break;
				case 18:
					response = "My sources say no";
					break;
				case 19:
					response = "Outlook not so good";
					break;
				case 20:
					response = "Very doubtful";
					break;
			}

			e.Channel.SendMessage(response, e.User);
		}

		#pragma warning disable 1998
		public async Task Help(HeatBot bot, PrivateMessage e, IList<string> args){}
		#pragma warning restore 1998
	}
}
