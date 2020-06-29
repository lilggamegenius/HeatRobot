using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using ChatSharp;

namespace HeatRobot.Commands{

	public interface ICommand{
		Task HandleCommand(HeatBot bot, PrivateMessage e, IList<string> args);
		Task Help(HeatBot bot, PrivateMessage e, IList<string> args);
	}

	public abstract class ISaveable<T>
		where T:ISaveable<T>{
		private const string PATH_FORMAT = "Data/{0}/{1}.json";
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
		protected virtual string name=>GetType().Name;
		protected virtual string folder=>GetType().Name;
		public virtual async Task SaveData(T data = null){
			await Task.Run(()=>{
				FileInfo file = new FileInfo(string.Format(PATH_FORMAT + ".temp", folder, name));
				FileInfo oldFile = new FileInfo(string.Format(PATH_FORMAT, folder, name));
				if(file.DirectoryName == null){
					return; // Why would this ever be null?
				}

				Directory.CreateDirectory(file.DirectoryName);
				using(JsonTextWriter textWriter = new JsonTextWriter(new StreamWriter(file.Create()))){
					Program.Serializer.Serialize(textWriter, data ?? this); // Save to temporary directory
					Logger.Debug($"Saving data to file {file.FullName}");
				}

				oldFile.Delete();
				file.MoveTo(oldFile.FullName); // Move to correct location
				Logger.Debug("Replaced save file with temp file");
			});
		}
		public virtual async Task<T> LoadData(){
			FileInfo file = new FileInfo(string.Format(PATH_FORMAT, folder, name));
			if(file.Exists){
				return await Task.Run(()=>{
					using(StreamReader sr = new StreamReader(file.OpenRead()))
					using(JsonTextReader reader = new JsonTextReader(sr)){
						Logger.Debug($"Loading data from file {file.FullName}");
						T temp = Program.Serializer.Deserialize<T>(reader);
						return temp;
					}
				});
			}

			await SaveData();
			return (T)this;
		}
	}

	public class CommandList{
		public delegate Task OnMessageDelegate(HeatBot bot, PrivateMessage e);
		public CommandList()=>commands = new Dictionary<string, ICommand>();
		public Dictionary<string, ICommand> commands{get;}

		public ICommand this[string i]{
			get=>commands[i.ToLower()];
			set=>commands[i.ToLower()] = value;
		}
		public event OnMessageDelegate onMessage;

		public async Task Message(HeatBot bot, PrivateMessage e){
			if(onMessage != null){ await onMessage(bot, e); }
		}

		public bool ContainsCommand(string command)=>commands.ContainsKey(command.ToLower());
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class PermissionLevel : Attribute{
		public Level Level{get;}
		public PermissionLevel(Level level = Level.Viewer){
			Level = level;
		}
	}

	public enum Level{Viewer, Mod, Broadcaster}
	//public enum Modes{ None, Voice, Halfop, Op, SuperOp, Owner, BotOwner }
}
