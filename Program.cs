using System.IO;
using HeatRobot.Config;
using Newtonsoft.Json;
using NLog;
using System;
using global::HeatRobot.Commands;

namespace HeatRobot{
	using System.Collections.Generic;
	using System.Linq;
	using System.Net.Mime;
	using System.Runtime.CompilerServices;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Timers;

	class Program{
		public const string Version = "HeatBot v0.1";
		public static readonly JsonSerializer Serializer = new JsonSerializer();
		internal static readonly CommandList CommandList;
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
		private static FileInfo _configFile;
		public static Configuration Config;

		static Program(){
			CommandList = new CommandList();
			IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies()
											   .SelectMany(s=>s.GetTypes())
											   .Where(p=>typeof(ICommand).IsAssignableFrom(p) && p.IsClass && !p.IsAbstract);
			foreach(Type command in types){
				RuntimeHelpers.RunClassConstructor(command.TypeHandle);
				Logger.Info("Loaded command {0} as {1}", command.Name, command.FullName);
			}
			Serializer.Formatting = Formatting.Indented;
		}

		public static int Main(string[] args){
			return AsyncMain(args).GetAwaiter().GetResult();
		}

		public static async Task<int> AsyncMain(string[] args){
			AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
			Logger.Debug("Current directory is {0}", Directory.GetCurrentDirectory());
			string configFilePath;
			#if DEBUG
			configFilePath = "Data/debugConfig.json";
			#else
				configFilePath = "Data/config.json";
			#endif
			_configFile = new FileInfo(configFilePath);
			Logger.Info("Config Path = {0}", _configFile);
			try{
				if(!_configFile.Exists){
					_configFile.Directory.Create(); // Create Data folder if it doesn't exist
					using var textWriter = new JsonTextWriter(new StreamWriter(_configFile.Create()));
					Serializer.Serialize(textWriter, new Configuration());
					Logger.Error("Config file missing, Empty created at: {0}", _configFile.FullName);
					throw new FileNotFoundException(configFilePath);
				}
				using(StreamReader sr = new StreamReader(_configFile.OpenRead()))
				using(JsonTextReader reader = new JsonTextReader(sr)){
					Config = Serializer.Deserialize<Configuration>(reader);
					Array.Sort(Config.CommandPrefixes, (x, y)=>y.Length.CompareTo(x.Length));
					new Thread(()=>{
						AppDomain.CurrentDomain.ProcessExit += (Config.heatBot = new HeatBot()).ExitHandler;
					}).Start();
				}
			} catch(Exception e){
				Logger.Error(e, "Error starting bot: {0}", e);
				return 1;
			}

			await Task.Delay(-1);
			return 0;
		}

		private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e){
			Exception exception = e.ExceptionObject as Exception;
			Logger.Fatal(exception, "Unhandled Exception caught");
		}

		public static void RegisterCommand(string commandName, ICommand command){CommandList[commandName.ToLower()] = command;}
	}
}
