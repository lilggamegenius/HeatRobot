namespace HeatRobot{
	using System;
	using System.Threading.Tasks;
	using ChatSharp;
	using ChatSharp.Events;
	using global::HeatBot.Utils;
	using HeatRobot.Commands;
	using HeatRobot.Config;
	using NLog;

	public class HeatBot{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
		private static Configuration Config=>Program.Config;
		public IrcClient IrcClient;
		public IrcUser IrcSelf;

		public HeatBot(){
			IrcSelf = Config.IrcSelf = new IrcUser(null, Config.Nickname, Config.Nickname, Config.oauthToken, Config.RealName);
			IrcClient = Config.IrcClient = new IrcClient($"{Config.server}:{Config.port}", IrcSelf, Config.SSL);
			IrcClient.IgnoreInvalidSSL = Config.IgnoreInvalidSSL;
			IrcClient.RawMessageRecieved += OnRawMessageRecieved;
			IrcClient.RawMessageSent += OnRawMessageSent;
			IrcClient.ConnectionComplete += OnConnect;
			IrcClient.UserJoinedChannel += OnUserJoinedChannel;
			IrcClient.PrivateMessageRecieved += OnPrivateMessageRecieved;
			IrcClient.UserQuit += OnUserQuit;
			//ircClient.ChannelMessageRecieved += onChannelMessageRecieved;
			IrcClient.Error += OnError;
			IrcClient.ConnectAsync();
		}
		public void ExitHandler(object sender, EventArgs args){IrcClient.Quit("Shutting down");}

		public async Task<bool> CommandHandler(PrivateMessage privateMessage){
			if(privateMessage.User.Hostmask == IrcSelf.Hostmask){ return false; }

			string message = privateMessage.Message;
			string messageLower = message.ToLower();
			string[] args = message.SplitMessage();
			IrcChannel channel;
			if(privateMessage.IsChannelMessage){
				channel = privateMessage.Channel;
			}
			else{
				return false;

			}

			string first = args[0].ToLower();
			bool commandByName = first.StartsWith(IrcSelf.Nick.ToLower());
			bool commandByPrefix = messageLower.StartsWithAny(Config.CommandPrefixes);
			if(commandByName || commandByPrefix){
				string command = null;
				int offset = 1;
				if(commandByName){
					if(args.Length < 2){ return false; }

					command = args[1];
					offset = 2;
				}

				if(commandByPrefix){
					foreach(string prefix in Config.CommandPrefixes){
						if(messageLower.StartsWith(prefix)){
							if(prefix.EndsWith(" ")){
								if(args.Length < 2){ return false; }

								command = args[1];
								offset = 2;
								break;
							}

							command = args[0].Substring(prefix.Length);
							break;
						}
					}
				}

				if(command == null){ return false; }

				if(Program.CommandList.ContainsCommand(command)){
					if(!LilGUtil.CheckPermission(command, privateMessage)){
						channel.SendMessage($"Sorry, you don't have the permission to run {command}");
						return true;
					}

					ICommand icommand = Program.CommandList[command];
					ArraySegment<string> segment = new ArraySegment<string>(args, offset, args.Length - offset);
					try{
						await Task.Run(()=>icommand.HandleCommand(this, privateMessage, segment));
					} catch(Exception ex){
						Logger.Error(ex, "Problem processing command: \n{0}", ex.StackTrace);
						channel.SendMessage($"Sorry there was a problem processing the command: {ex.Message}");
						return true;
					}

					return true;
				}
			}

			return false;
		}
		private async void OnChannelMessageRecieved(PrivateMessage privateMessage){
			LogMessage(privateMessage);
			bool isCommand = await CommandHandler(privateMessage);
			if(!isCommand){ await Program.CommandList.Message(this, privateMessage); }

			Logger.Info("{0} from {1} by {2}: {3}",
						isCommand ? "Command" : "Message",
						privateMessage.Source,
						privateMessage.User.Hostmask,
						privateMessage.Message);
		}
		private void LogMessage(PrivateMessage message){
			Logger.Info($"{message.Channel}: <{message.User.Nick}> {message.Message}");
		}

		private async void OnPrivateMessageRecieved(object sender, PrivateMessageEventArgs e){
			PrivateMessage privateMessage = e.PrivateMessage;

			if(privateMessage.IsChannelMessage){
				OnChannelMessageRecieved(privateMessage);
				return;
			}

			bool isCommand = await CommandHandler(privateMessage);
			if(!isCommand){ await Program.CommandList.Message(this, privateMessage); }

			Logger.Info("{0} from {1}: {2}", isCommand ? "Command" : "Message", privateMessage.User.Hostmask, privateMessage.Message);
		}

		private void OnUserJoinedChannel(object sender, ChannelUserEventArgs e){Logger.Info("User {0} Joined channel {1}", e.User.Hostmask, e.Channel.Name);}

		private void OnUserQuit(object sender, UserEventArgs e){
			Logger.Info("User {0} Quit", e.User.Hostmask);
		}

		private void OnRawMessageRecieved(object sender, RawMessageEventArgs args){
			if(args.Message.Contains(" :tmi.twitch.tv USERSTATE #")){
				OnUserState(sender, args);
			}
			Logger.Debug("<<< {0}", args.Message);
		}

		private void OnRawMessageSent(object sender, RawMessageEventArgs args){
			Logger.Debug(">>> {0}", args.Message);
		}

		private void OnUserState(object sender, RawMessageEventArgs args){

		}

		private void OnConnect(object sender, EventArgs e){
			Task.Run(()=>{
				IrcClient.SendRawMessage("CAP REQ :twitch.tv/membership twitch.tv/commands twitch.tv/tags");
				IrcClient.JoinChannel("#" + Config.botOwnerName);
			});
		}

		private void OnError(object sender, ErrorEventArgs e){Logger.Error(e.Error, e.Error.Message);}

		//private enum CtcpCommands{ PING, FINGER, VERSION, USERINFO, CLIENTINFO, SOURCE, TIME, PAGE, AVATAR }
	}
}
