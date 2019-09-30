using System;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Text;


namespace Zork
{
    public class Game
    {
        [JsonIgnore]
        public static Game Instance { get; private set; }

        public World World { get;  set; }

        [JsonIgnore]
        public Player Player { get; private set; }

        [JsonIgnore]
        private bool IsRunning { get; }

        [JsonIgnore]
        public CommandManager CommandManager { get; }

        public Game(World world, Player player)
        {
            World = world;
            Player = player;
        }

        public Game() => CommandManager = new CommandManager();


        private static readonly string ScriptDirectory = "Scripts";
        private static readonly string ScriptFileExtension = "*.csx";

        public void Run()
        {
            mIsRunning = true;
            Room previousRoom = null;
            while (IsRunning)
            {
                Console.WriteLine(Player.Location);
                if (previousRoom != Player.Location)
                {
                    CommandManager.PerformCommand(this, "LOOK");
                    previousRoom = Player.Location;
                }

                Console.Write("\n> ");
                if(CommandManager.PerformCommand(this, Console.ReadLine().Trim()))
                {
                    Player.Moves++;
                }
                else
                {
                    Console.WriteLine("That is not a verb I recognize");
                }
            }
        }

        public void restart()
        {
            mIsRunning = false;
            mIsRestarting = true;
            Console.Clear;
        }

        public void Quit() mIsrunning = false;


        public static Game Load(string filename)
        {
            Game game = JsonConvert.DeserializeObject<Game>(File.ReadAllText(filename));
            game.Player = game.World.SpawnPlayer();

            return game;
        }

        private void LoadCommands()
        {
            var commandMethods = (from type in Assembly.GetExecutingAssembly().GetTypes()
                                  from method in type.GetMethods()
                                  let attribute = method.GetCustomAttribute<CommandAttribute>()
                                  where type.IsClass && type.GetCustomAttribute<CommandClassAttribute>() != null
                                  where attribute != null
                                  select new Command(attribute.CommandName, attribute.Verbs,
                                  (Action<Game, CommandContext>)Delegate.CreateDelegate(typeof(Action<Game, CommandContext>), method)));

            CommandManager.AddCommands(commandMethods);
        }

        private void LoadScripts()
        {
            foreach (string file in Directory.EnumerateFiles(ScriptDirectory, ScriptFileExtension))
            {
                try
                {
                    var scriptOptions = ScriptOptions.Default.AddReferences(Assembly.GetExecutingAssembly());
#if (DEBUG)
                    scriptOptions = scriptOptions.WithEmitDebugInformation(true)
                        .WithFilePath(new FileInfo(file).FullName)
                        .WithFileEncoding(Encoding.UTF8);
#endif
                    string script = File.ReadAllText(file);
                    CSharpScript.RunAsync(script, scriptOptions).Wait();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error compiling script: {file} Error: {ex.Message}");
                }
            }
        }
    }
}
