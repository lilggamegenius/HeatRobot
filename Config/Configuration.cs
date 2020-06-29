using ChatSharp;
using Newtonsoft.Json;

namespace HeatRobot.Config{
	public class Configuration{
		private const string DefaultName = "heat_robot";
		public string Nickname = DefaultName;
		public string RealName = "";

		public string oauthToken = "oauth:token goes here";

		public string[] CommandPrefixes = {"!"};

		public string server = "irc.twitch.tv";
		public bool SSL = true;
		public ushort port = 6697;
		public bool AutoSplitMessage = false;
		public bool FloodProtection = true;
		public int FloodProtectionDelay = 1000;
		public bool IgnoreInvalidSSL = true;

		public string botOwnerName = "heatphoenix";
		[JsonIgnore] public HeatBot heatBot;
		[JsonIgnore] public IrcClient IrcClient;
		[JsonIgnore] public IrcUser IrcSelf;


	}
}
