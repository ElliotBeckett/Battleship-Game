using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

/*  Battleship Game
 *  Written by Elliot Beckett
 *  29 October 2020
 *  Version 1.0
 */

namespace BattleshipTest
{
    internal class Program
    {
        /*
         * A custom function to read files
         * The file read is the string passed into the function
         */

        public static string ReadFile(string filePath)
        {
            string readText = File.ReadAllText(filePath);
            return readText;
        }

        /*
         * This function is used to compare the victory conditions of the game, whether the player wins or looses
         * It takes two variables X and Y or Cannonshots and Enemy ships. Depending on what is higher, it will return the game winner
         */

        private static string GameWinnerCheck(int x, int y)
        {
            string gameWinner = "This is the default answer";

            System.Media.SoundPlayer victory = new System.Media.SoundPlayer("victory.wav");
            System.Media.SoundPlayer looser = new System.Media.SoundPlayer("scream.wav");

            if (x > 0 && y == 0)
            {
                gameWinner = String.Format($"Congratulations! You won the game :) You managed to do it with {x} cannon balls remaining");
                victory.Play();
            }
            else
                if (x == 0 && y > 0)
            {
                Console.Write(ReadFile("skull.txt"));
                Console.WriteLine(Environment.NewLine);
                gameWinner = String.Format($"Unfortunately you didn't win :( There were still {y} enemy ships left");
                looser.Play();
            }

            return gameWinner;
        }

        private static void Main(string[] args)
        {
            /*
             * Setting up our initial variables that will be used throughout
             * These variables are global, so they do not need to be reset between games
             */

            bool playAgain = false; //Making sure our game runs - May remove
            bool showGameMenu = true; //Our variable for displaying the game menu
            string scoreboardFilePath = "scores.txt"; //Path to our scores file
            var headerSpliter = new string('-', (5 * 17)); // Creating a variable for the grid header
            var regexItem = new Regex("^[a-zA-Z' ]*$"); //This Regex variable is used to check player names, so that they only contain approved characters

            System.Media.SoundPlayer cannon = new System.Media.SoundPlayer("cannon.wav");
            System.Media.SoundPlayer hit = new System.Media.SoundPlayer("hit.wav");
            System.Media.SoundPlayer splash = new System.Media.SoundPlayer("splash.wav");

            //Setup Random Number Generator
            Random randNum = new Random();

            //Increasing the width of the console to make it easier to read the instructions and scores
            Console.SetWindowSize(150, 40);

            //Welcome Banner
            Console.WriteLine("\t\t~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine("\t\t~~ Welcome to the Battleship Game ~~");
            Console.WriteLine("\t\t~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine(Environment.NewLine);
            //This reads the ship.txt file and prints out a cool battleship.
            Console.WriteLine(ReadFile("ship.txt"));

            /*
            * In this section, we ask for the players name (which will be used throughout the game and for the scoreboard)
            * We also check what the user inputs to make sure they have given a valid input.
            * By using a while loop, we can force the player to keep inputting a name, until is passes our checks.
            * When is passes our checks, it breaks the loop and players can continue.
            */

            Console.WriteLine("\n\nWelcome to the Battleship Game, a simple but fun console game, created using C#");

            bool validName = false;
            string playerName = ""; // An empty string that the player name will override, we need to declare it outside the name check however.

            while (!validName)
            {
                try //A Try/Catch check is added incase players input something unexpected (such as numbers etc)
                {
                    Console.Write("\n\nTo begin, please tell me your name Cap'n: ");
                    playerName = Console.ReadLine();
                    bool isDigitPresent = playerName.Any(c => char.IsDigit(c));

                    //Our first check is to make sure the name isn't Null/Empty, so players haven't just hit enter
                    if (string.IsNullOrEmpty(playerName))
                    {
                        Console.Write($"\nThat wasn't quite right, Your name can't be empty\n\nPlease try again: ");
                    }
                    //This check makes sure that the player hasn't just put one letter in for their name
                    else if (playerName.Length <= 1)
                    {
                        Console.Write($"\nThat wasn't quite right, Your name can't be one letter\n\nPlease try again: ");
                    }
                    //The next check makes sure that there are no numbers or special characters in the name
                    else if (!regexItem.IsMatch(playerName))
                    {
                        Console.Write($"\nThat wasn't quite right, Your name can't have numbers or special characters in it\n\nPlease try again: ");
                    }
                    //If they pass the above checks, the name is checked to see if it has the Cap'n prefix
                    else
                    {
                        string capnTitle = "Cap'n";
                        bool capnTitleCheck = playerName.Contains(capnTitle);

                        if (capnTitleCheck)
                        {
                            //If the player has already put Cap'n in their name, we just pass through
                            validName = true;
                        }
                        else
                        {
                            //If it doesn't, we add the prefix ourself and move on
                            playerName = "Cap'n " + playerName;
                            validName = true;
                        }
                    }
                }
                catch (Exception playerNameEx)
                {
                    // Our catch clause triggers if the player input anything other than a character (so a string of text or number)
                    // It will print out the error, instead of crashing the game
                    Console.WriteLine("\n\nThat wasn't quite right and caused an error.\n\nThe error was: " + playerNameEx.Message);
                }
            }//ENDvalidNameWhile

            /* This is the game menu code, players will use this to start new games, read the instructions and scores, as well as close the game
             * To ensure that players don't accidently input the wrong menu option, the menu runs inside a While loop that checks for a valid input
             * The game won't progress until the player inputs an allowed menu option. The game will continue to prompt the player until they do
             * Additionally, the menu option is wrapped in a Try/Catch statement, this is to prevent input exceptions such as players using a number
             */
            Console.WriteLine("\n\nThe below game menu is the main method to control the game. \nIt will let you start a new game, read the game's instructions, view past scores or exit the game");

            while (showGameMenu)
            {
                Console.WriteLine("\n\n~~ Game Menu ~~ ");

                Console.Write("\n\nPress B to play a new game \n\nPress I for instructions \n\nPress S for scores \n\nPress E to exit the game \n\nInput your choice here and then press enter >> ");

                bool validMenuInput = false; //This bool ensures the menu displays and continues to loop until we are happy with the users input

                while (!validMenuInput)
                {
                    try
                    {
                        // Reading in our players choice and converting it to lower case, means we don't have to define as many cases
                        char menuSelection = char.Parse(Console.ReadLine().ToLower());
                        switch (menuSelection)
                        {
                            case 'b': //This option starts a new game for the players
                                playAgain = true;
                                validMenuInput = true;
                                showGameMenu = false;
                                break;

                            case 's': // This option reads in the scores.txt file and prints it out to the screen, with a nice header
                                Console.Write("\n\n\n** Game Scoreboard **");
                                Console.Write("\n---------------------------\n\n");
                                Console.WriteLine("\n" + ReadFile("scores.txt") + "\n\nPress any key to return to the game menu: ");
                                Console.ReadKey();
                                validMenuInput = true;
                                break;

                            case 'i': // This option reads in the instructions.txt file and prints it out to the screen
                                Console.Write("\n\n\n** Game Instructions **");
                                Console.Write("\n---------------------\n\n");
                                Console.WriteLine("\n" + ReadFile("instructions.txt") + "\n\nPress any key to return to the game menu: ");
                                Console.ReadKey();
                                validMenuInput = true;
                                break;

                            case 'e': // This option closes the game after displaying an exit message
                                Console.WriteLine("Thanks for playing, the game will now close :)");
                                Thread.Sleep(500);
                                Environment.Exit(0);
                                break;

                            default: // This is the default case, if the player presses anything other than the options above, it will print this error
                                Console.Write("\nThat was not a valid choice, please try again >>");
                                Console.Write("\n\nPress B to play a new game \n\nPress I for instructions \n\nPress S for scores \n\nPress E to exit the game \n\nInput your choice here and then press enter >> ");
                                break;
                        }
                    }
                    catch (Exception menuException)
                    {
                        // Our catch clause triggers if the player input anything other than a character (so a string of text or number)
                        // It will print out the error, instead of crashing the game
                        Console.WriteLine($"\nThere was an issue with your choice and it created an error. \n\nThe error generated was: {menuException.Message}");
                        Console.Write("\n\nPress B to play a new game \n\nPress I for instructions \n\nPress S for scores \n\nPress E to exit the game \n\nInput your choice here and then press enter >> ");
                    }
                }

                /*
                 * This is the start of our game code and the play screens.
                 * It begins by asking the players what difficulty level they want to play.
                 * They get a different amount of cannon balls depending on what level they select.
                 *
                 * There is also a debug mode for testing. It will print the locations of all the ships and give users 50 cannon balls.
                 * While in debug mode, scores won't be recorded, as that is cheating.
                 *
                 * Finally, everytime we require any input from the players, the code is wrapped in a While & Try/Catch loop.
                 * The loop continues until the player provides a valid input, depending on the situation.
                 * This is done to ensure we don't get any input errors, which will cause the game to crash.
                 */

                while (playAgain)
                {
                    /*
                     * The below variables will need to be reset between each round of the game
                     * That is why they are being declared here
                     */
                    int cannonShots = 0; // Used to track how many cannon shots players will have during the game
                    int playerScore = 0; //Used to track players score i.e how many ships they sunk
                    int shipCount = 0; // Used to keep track of ships plotted
                    int enemyShips = 5; // Total count of enemy ships to sink
                    int mapSize = 5; // Sets the limits of our array, this can be adjusted for different game difficulty
                    bool debugMode = false; // A debug tool for testing
                    int xGuess = 0; // Setting empty variables for our X Guess. This is so we can test it later
                    int yGuess = 0; // Setting empty variables for our y Guess. This is so we can test it later

                    Console.Write($"\n\nHello {playerName}, What difficulty level would you like to play? \n\nPress E for Easy \n\nPress M for Medium \n\nPress H for Hard \n\nInput your choice here and press enter >>");
                    bool validDifficultyInput = false; //Starting our While loop as false, so it loops until we get a valid input

                    while (!validDifficultyInput)
                    {
                        try
                        {
                            // Reading in our players choice and converting it to lower case, means we don't have to define as many cases
                            char difficultyLevel = char.Parse(Console.ReadLine().ToLower());

                            switch (difficultyLevel)
                            {
                                case 'e': //Easy difficulty
                                    cannonShots = enemyShips * 3;
                                    validDifficultyInput = true;
                                    break;

                                case 'm': //Medium difficulty
                                    cannonShots = enemyShips * 2;
                                    validDifficultyInput = true;
                                    break;

                                case 'h': //Hard difficulty
                                    cannonShots = enemyShips + 3;
                                    validDifficultyInput = true;
                                    break;

                                case 'd': //Debug mode, for testing only
                                    cannonShots = 50;
                                    validDifficultyInput = true;
                                    debugMode = true;
                                    break;

                                default: // This is the default case, if the player presses anything other than the options above, it will print this error
                                    Console.WriteLine("\n\nThat was not a valid choice, please try again");
                                    Console.Write("\n\nWhat difficulty level would you like to play? \n\nPress E for Easy \n\nPress M for Medium \n\nPress H for Hard \n\nInput your choice here and press enter >>");
                                    break;
                            }//End difficultyLevel switch
                        }
                        // Our catch clause triggers if the player input anything other than a character (so a string of text or number)
                        // It will print out the error, instead of crashing the game
                        catch (Exception difficultyLevelEx)
                        {
                            Console.WriteLine($"\nThere was an issue with your choice and it created an error. \n\nThe error generated was: {difficultyLevelEx.Message}");
                            Console.Write("\n\nWhat difficulty level would you like to play? \n\nPress E for Easy \n\nPress M for Medium \n\nPress H for Hard \n\nInput your choice here and press enter >>");
                        }
                    }//End difficultyLevel while

                    // Creation of our boolean array, which will be used as ship locations
                    bool[,] mapArray = new bool[mapSize, mapSize];
                    // Creation of our int array, which will be used to track player guesses
                    string[] guessArray = new string[cannonShots];
                    // This int arryI is used to increment the array position everytime the player takes a shot, allowing the array to loop
                    int arryI = 0;

                    Console.Clear(); //Clearing the console, to save on screen clutter
                    Console.WriteLine($"Ok, {playerName} let's start sinking some ships");

                    /*
                     * Code to plot the ships to our array
                     * The While loop continues until the ship count is less than the total amount of enemy ships
                     * After every successful completion of the loop, the ship count is increased by 1
                     * Therefore, the loop only continues for 5 cycles
                     */

                    while (shipCount < enemyShips)
                    {
                        /*
                         * We generate two random numbers, which will be used as X and Y Co-Ordinates for the battleships
                         */
                        Thread.Sleep(5); //There is a slight thread pause, this is to help generate a more random number
                        int randomX = randNum.Next(0, mapSize);
                        Thread.Sleep(3); //There is a slight thread pause, this is to help generate a more random number
                        int randomY = randNum.Next(0, mapSize);

                        /*
                         * The below If statement checks to see if the co-ordinates provided by the random numbers are NOT true (so false)
                         * If they are false, it switches the co-ordinates to true and that becomes an enemy ship
                         * It then increments the ship count, so it can eventually end the loop
                         */

                        if (mapArray[randomX, randomY] != true)
                        {
                            mapArray[randomX, randomY] = true;
                            shipCount++;
                        }
                    }

                    /*
                     * Since it's hard for players to imagine a grid while playing, this code prints out a representation of the grid.
                     * To save on screen clutter, it only prints out once, but it does help with guessing.
                     */

                    if (!debugMode)
                    {
                        Console.WriteLine("\nThis is a representation of the game grid, use the X and Y values to guess your targets");
                        Console.WriteLine(Environment.NewLine);
                        for (int j = 0; j < mapSize; j++)
                        {
                            Console.Write($" | X{j + 1} \t\t");
                        }
                        Console.WriteLine(Environment.NewLine);
                        for (int y = 0; y < mapSize; y++)
                        {
                            for (int x = 0; x < mapSize; x++)
                            {
                                Console.Write(" | ");

                                Console.Write(string.Format(" 0\t\t"));
                            }
                            Console.Write("| Y " + (y + 1) + " |");
                            Console.Write("\n " + headerSpliter + "-\n");
                        }
                    }
                    while (cannonShots > 0 && enemyShips > 0)
                    {
                        /*
                         * The below code is used for debug purposes.
                         * It prints a crude visual representation of the Battleship grid and where the 'ships' are (the true/false values of the array)
                         * This makes testing much easier.
                         * Due to the advantages provided by debug mode, players scores won't be recoreded to file.
                         * This is just for testing purposes only
                         */

                        if (debugMode)
                        {
                            Console.WriteLine("\n\t~~~ Debug Mode is enabled, scores will NOT be recorded and counted. ~~~");
                            Console.WriteLine("\n\t\t\t~~~ This is for testing purposes only ~~~");
                            Console.WriteLine(Environment.NewLine);
                            for (int j = 0; j < mapSize; j++)
                            {
                                Console.Write($" | X{j + 1} \t\t");
                            }
                            Console.WriteLine(Environment.NewLine);
                            for (int y = 0; y < mapSize; y++)
                            {
                                for (int x = 0; x < mapSize; x++)
                                {
                                    Console.Write(" | ");
                                    Console.Write(string.Format(" {0}\t", mapArray[x, y]));
                                }
                                Console.Write("| Y " + (y + 1) + " |");
                                Console.Write("\n " + headerSpliter + "-\n");
                            }
                        }

                        /*
                         * The two below While loops check the users X and Y guesses to make sure they are valid.
                         * We dont want players guessing outside our array limits or inputting an invalid guess (like a string)
                         * Once they pass the tests, their guesses will be checked against ship locations
                         */

                        bool validXGuess = false; // Used to test players X guess
                        bool validYGuess = false; // Used to test players Y guess
                        while (!validXGuess)
                        {
                            try //A Try/Catch check is added incase players input something unexpected (such as numbers etc)
                            {
                                Console.Write("\nInput your X coordinate guess: ");
                                xGuess = Int16.Parse(Console.ReadLine());

                                //Our first check is to make sure the guess isn't too low or null - Causing Array out of bounds errors
                                if (xGuess == 0)
                                {
                                    Console.Write($"\nThat wasn't quite right, Your guess can't be empty or less than 1\nPlease try again\n");
                                }
                                //Our next check is to make sure the guess isn't too high and outside the map size - Causing Array out of bounds errors
                                else if (xGuess > mapSize)
                                {
                                    Console.Write($"\nThat wasn't quite right, Your guess can't be larger than the map size\nPlease try again\n");
                                }
                                //If they pass the above checks, their guess is considered valid and can end the loop
                                else
                                {
                                    validXGuess = true;
                                }
                            }
                            catch (Exception validXGuessEx)
                            {
                                // Our catch clause triggers if the player input anything other than an integer (so a string of text or a character)
                                // It will print out the error, instead of crashing the game
                                Console.WriteLine($"\nThere was an issue with your choice and it created an error. \n\nThe error generated was: {validXGuessEx.Message}");
                                Console.WriteLine("\nPlease only use numbers (1-5) not letters or words");
                            }
                        }//ENDvalidXGuessWhile

                        while (!validYGuess)
                        {
                            try //A Try/Catch check is added incase players input something unexpected (such as numbers etc)
                            {
                                Console.Write("\nInput your Y coordinate guess: ");
                                yGuess = Int16.Parse(Console.ReadLine());

                                //Our first check is to make sure the guess isn't too low or null - Causing Array out of bounds errors
                                if (yGuess == 0)
                                {
                                    Console.Write($"\nThat wasn't quite right, Your guess can't be empty or less than 1\nPlease try again\n");
                                }
                                //Our next check is to make sure the guess isn't too high and outside the map size - Causing Array out of bounds errors
                                else if (yGuess > mapSize)
                                {
                                    Console.Write($"\nThat wasn't quite right, Your guess can't be larger than the map size\nPlease try again\n");
                                }
                                //If they pass the above checks, their guess is considered valid and can end the loop
                                else
                                {
                                    validYGuess = true;
                                }
                            }
                            catch (Exception validYGuessEx)
                            {
                                //This prints out any errors that may have been caused by player input. Such as invalid inputs
                                Console.WriteLine($"\nThere was an issue with your choice and it created an error. \n\nThe error generated was: {validYGuessEx.Message}");
                                Console.WriteLine("\nPlease only use numbers (1-5) not letters or words");
                            }
                        }//ENDvalidYGuessWhile

                        /*
                         * The below segment of code combines the two player guesses so that we can put them into an array
                         * The array will print out after every shot taken, showing players where they have guessed previously
                         * Additionally, it will tell the players if they hit or miss anything.
                         * While it adds to screen clutter, it is important for players to know where they have guessed before,
                         * Otherwise they could guess the same co-ordinates over and over again by accident.
                         */

                        string playerGuess = "";
                        string playerResult = "";
                        try
                        {
                            playerGuess = "X: " + xGuess + ", Y: " + yGuess;
                        }
                        catch (Exception playerGuessEX)
                        {
                            Console.WriteLine($"\nThere was an issue with your choice and it created an error. \n\nThe error generated was: {playerGuessEX.Message}");
                        }

                        Console.WriteLine($"Firing Cannons at {xGuess}, {yGuess}");
                        cannon.Play();

                        /*
                         *  Using the following If Statement, we check the players guess against the value of the array at that index
                         *  If it is true, the player has hit an enemy ship and scores a point
                         *  If it is false, the player has missed and not hit anything
                         *  Due to array's starting at Index 0, we have to -1 from the players guess (since they are guessing numbers between 1-5)
                         *
                         */

                        if (mapArray[xGuess - 1, yGuess - 1] == true)
                        {
                            hit.Play();
                            mapArray[xGuess - 1, yGuess - 1] = false;
                            playerResult = playerGuess + " \nResult: You hit an enemy ship!";
                            guessArray[arryI] = playerResult;
                            playerScore++;
                            enemyShips--;
                            Console.WriteLine($"\nCongratulations you scored a hit and sunk an enemy ship \nThere are {enemyShips} enemy ships left");
                            arryI++;
                        }
                        else
                        {
                            splash.Play();
                            playerResult = playerGuess + " \nResult: You missed and didn't hit anything";
                            guessArray[arryI] = playerResult;
                            Console.WriteLine($"\nYou missed! \nThere are still {enemyShips} enemy ships remaining");
                            arryI++;
                        }
                        cannonShots--;
                        Console.WriteLine($"After taking a shot, you have {cannonShots} shots remaining\n");

                        /*
                         * The below code prints out a list of all the players guesses, so they can see where they have picked before
                         */
                        Console.WriteLine("So far, you have made these guesses: \n");
                        for (int k = 0; k < guessArray.Length; k++)
                        {
                            if (!(guessArray[k] == null || guessArray[k] == "" || guessArray[k] == " "))
                            {
                                Console.WriteLine($"Turn Number {k + 1}\n{guessArray[k]}\n");
                            }
                        } //End For
                    }// End Gameplay While

                    //This prints out our win/loose message after comparing the game results
                    Console.WriteLine(GameWinnerCheck(cannonShots, enemyShips));

                    /*
                     * This code writes our scores to file, except for if the player is using debug mode
                     * We don't want to record the scores of our test games, as it gives an unfair advantage
                     */
                    if (!debugMode)
                    {
                        File.AppendAllText(scoreboardFilePath, $"\nPlayer Name: {playerName} \nPlayer Score: {playerScore} \nCannon Balls Left: {cannonShots} \n{GameWinnerCheck(cannonShots, enemyShips)}\n");
                    }

                    /*
                     * The below While & Try/Catch loop asks the players if they want to play the game again.
                     * It works similarly to the previous menus, it continues to cycle until the players make a valid choice.
                     *
                     * If players want to play again, it starts a new game, if not then it displays the main menu again
                     */
                    bool validPlayAgainInput = false;
                    while (!validPlayAgainInput)
                    {
                        Console.WriteLine("\n\nWould you like to play again? \n\nPress y for Yes \n\nPress n for No?");
                        try
                        {
                            char playAgainInput = char.Parse(Console.ReadLine().ToLower());
                            switch (playAgainInput)
                            {
                                case 'y':
                                    playAgain = true;
                                    validPlayAgainInput = true;
                                    Console.Clear();
                                    break;

                                case 'n':
                                    playAgain = false;
                                    validPlayAgainInput = true;
                                    Console.Clear();
                                    showGameMenu = true;
                                    break;

                                default:
                                    Console.WriteLine("That was not a valid choice, please try again");
                                    break;
                            }
                        }
                        catch (Exception playAgainInputEx)
                        {
                            Console.WriteLine($"\nThere was an issue with your choice and it created an error. \n\nThe error generated was: {playAgainInputEx.Message}");
                        }
                    }//End PlayAgainInput check
                }//END While playAgain
                playAgain = false;
            }//END ShowMenu
            Console.ReadLine();
        }//End Main
    }//End Class
}//End Namespace