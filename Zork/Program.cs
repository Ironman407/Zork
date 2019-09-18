using System;

namespace Zork
{

    class Program
    {
        private static int LocationColumn = 1;

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Zork!");
            
            Commands command = Commands.UNKNOWN;
            while (command != Commands.QUIT)
            {
                Console.WriteLine(Rooms[LocationColumn]);
                Console.Write("> ");
                command = ToCommand(Console.ReadLine().Trim());

                string outputString = "";
                switch (command)
                {
                    case Commands.LOOK:
                        outputString = "This is an open field west of a white house, with a boarded front door. \nA rubber mat saying 'Welcome to Zork!' lies by the door.";
                        break;
                    case Commands.NORTH:
                    case Commands.SOUTH:
                    case Commands.EAST:
                    case Commands.WEST:
                        if(Move(command))
                        {
                            outputString = $"You moved {command}.";
                        }
                        else
                        {
                            outputString = "The way is shut!";
                        }
                        
                        break;
                    case Commands.QUIT:
                        outputString = "Thank you for playing!";
                        break;

                    default:
                        Console.WriteLine("Unknown command.");
                        break;

                        
                }

                Console.WriteLine(outputString);
            }
        }

        private static bool Move(Commands command)
        {
            bool isValidMove = false;

            switch (command)
            {
                case Commands.NORTH:
                case Commands.SOUTH:
                    break;
                case Commands.EAST when LocationColumn < Rooms.Length - 1:
                    isValidMove = true;
                    LocationColumn++;
                    break;
                case Commands.WEST when LocationColumn > 0:
                    isValidMove = true;
                    LocationColumn--;
                    break;

                default:
                    isValidMove = false;
                    break;
            }

            return isValidMove;
        }

        private static string[] Rooms = { "Forest", "West of House", "Behind House", "Clearing", "Canyon View" };

        private static Commands ToCommand(string commandString) => (Enum.TryParse<Commands>(commandString, true, out Commands result) ? result : Commands.UNKNOWN);
        
            
        
    }
}
