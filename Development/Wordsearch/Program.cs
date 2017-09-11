using System;
using System.IO;
using System.Linq;
/*\
 *  To-do
 *  1 - create a file validation system
 *  
 *  
 *
 * File format
 *   <Number of Columns>, <Number of Rows>, <Total Number of hidden words>
 *   number of lines in document = number of hiddenwords + 1;
 *   <Word>, <Column Co-ordinate of first letter> (must be less than number of columns - 1) , <Row Co-ordinate first letter> (must be less than the number of rows -1 , <Direction>
 *   what if the word is longer than the table? 
 *   what if the column co-ordinate is greater than the number of columns 
 *   what if the row co-ordinate is greater than the number of rows
 *   
 *   
 *   
 * File details
 * 
 *  - different elements are sperated via ","  
 *   element list
 *   1 - number of columns
 *   2 - number of rows
 *   3 - number of hidden words
 *   
 *   
 *   
 *   
 *   
\*/
namespace Wordsearch
{
    class Program
    {
        public enum direction
        {
            upRight, upLeft, downRight, downLeft
        };
        public struct word
        {
            public string wrd;
            public string direction;
            public int length;
            public int row;
            public int column;
            public bool found;
            public int lastRow;
            public int lastColumn;
            public int[,] wordCordinates;
            // constructor 
            public word(string wrd, int column, int row, string direction)
            {
                this.wrd = wrd;
                this.length = wrd.Length;
                this.row = row;
                this.column = column;
                this.found = false;
                this.direction = direction;
                this.wordCordinates = new int[length, 2];
                switch (this.direction)
                {
                    case "left":
                        lastRow = row;
                        lastColumn = column - (length - 1);
                        // Console.WriteLine(lastRow + " " + lastColumn + " " + row + " " + column);
                        for (int x = 0; x < wordCordinates.GetLength(0); x++)
                        {
                            wordCordinates[x, 0] = row;
                            wordCordinates[x, 1] = (column) - x;
                        }
                        break;
                    case "right":
                        lastRow = row;
                        lastColumn = (column + length) - 1;
                        // Console.WriteLine(lastRow);
                        for (int i = 0; i < wordCordinates.GetLength(0); i++)
                        {
                            wordCordinates[i, 0] = row;
                            wordCordinates[i, 1] = column + i;
                        }
                        break;
                    case "down":
                        lastRow = row + (length - 1);
                        lastColumn = column;
                        for (int i = 0; i < wordCordinates.GetLength(0); i++)
                        {
                            wordCordinates[i, 0] = row + i;
                            wordCordinates[i, 1] = column;
                        }
                        break;
                    case "up":
                        lastRow = row - (length - 1);
                        lastColumn = column;
                        for (int i = 0; i < wordCordinates.GetLength(0); i++)
                        {
                            wordCordinates[i, 0] = row - i;
                            wordCordinates[i, 1] = column;
                        }
                        break;
                    case "leftup":
                        lastRow = row - (length - 1);
                        lastColumn = column - (length - 1);
                        for (int i = 0; i < wordCordinates.GetLength(0); i++)
                        {
                            wordCordinates[i, 0] = row - i;
                            wordCordinates[i, 1] = column - i;
                        }
                        break;
                    case "rightup":
                        lastRow = row - (length - 1);
                        lastColumn = column + (length - 1);
                        for (int i = 0; i < wordCordinates.GetLength(0); i++)
                        {
                            wordCordinates[i, 0] = row - i;
                            wordCordinates[i, 1] = column + i;
                        }
                        break;
                    case "rightdown":
                        lastRow = row + (length - 1);
                        lastColumn = column + (length - 1);
                        for (int i = 0; i < wordCordinates.GetLength(0); i++)
                        {
                            wordCordinates[i, 0] = row + i;
                            wordCordinates[i, 1] = column + i;
                        }
                        break;
                    case "leftdown":
                        lastRow = row + (length - 1);
                        lastColumn = column - (length - 1);
                        for (int i = 0; i < wordCordinates.GetLength(0); i++)
                        {
                            wordCordinates[i, 0] = row + i;
                            wordCordinates[i, 1] = column - i;
                        }
                        break;
                    default:
                        lastRow = 0;
                        lastColumn = 0;
                        Console.WriteLine("file issue invalid direction");
                        Console.Write("press enter to exit");
                        Console.ReadKey();
                        Environment.Exit(0);
                        break;
                }

            }

        }
        struct puzzle
        {
            // top level structure
            public word[] words;
            public int rows;
            public int columns;
            public int noWords;

        }

        static void Main(string[] args)
        {

            // initilise
          /*  try
            {
                if (args[0] == "" || args == null)
                {

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Please provide a command line argument");
                Console.WriteLine("Press enter to exit");
                Console.ReadKey();
                Environment.Exit(0);
            }*/
            puzzle game = makePuzzle("File03.txt");
            char[,] map = generateRandomMap(game.columns, game.rows);
            // loop
            try
            {
                drawWords(game.words, map);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("there was a issue when drawing the words check you text file to make sure all the coordinates are within the bounds of the game board, this includes the length of the word!");
                Console.Write("Press enter to exit");
                Console.ReadKey();
                Environment.Exit(0);
            }
            bool hasWon = false;
            int[,] holder = new int[0, 0];
            bool lastInputWasIncorrect = false;
            do
            {
                int[,] coordsOfInput = new int[0, 0];
                if (lastInputWasIncorrect)
                {
                    coordsOfInput = getIntArray(holder);
                }

                drawMap(map, game.words, coordsOfInput, lastInputWasIncorrect);
                lastInputWasIncorrect = false;
                printWords(game.words);
                holder = playerInput(map);

                bool valid = validatePlayerInput(holder);
                if (valid)
                {
                    lastInputWasIncorrect = checkPlayerInput(holder, game.words);
                }
                hasWon = checkIfWon(game.words);
                Console.ReadKey();
                Console.Clear();
            } while (hasWon == false);
            // the player has won if they break out of this loop
            holder = new int[0, 0];
            drawMap(map, game.words, holder, false);
            Console.WriteLine("You have won!!");
            Console.ReadKey();
        }

        // general methords
        private static char[,] generateRandomMap(int noColumns, int noRows)
        {

            char[,] output = new char[noRows, noColumns];
            Random rad = new Random(); // random seeded to current time
            for (int x = 0; x < noRows; x++)
            {
                for (int y = 0; y < noColumns; y++)
                {
                    int randomNumber = rad.Next(0, 26);
                    if (debug())
                    {
                        /*
                        Console.WriteLine(x + " " + y);
                        Console.WriteLine((char)(randomNumber + 97));
                        */
                    }
                    output[x, y] = (char)(randomNumber + 97);
                }
            }

            return output;
        }
        /// <summary>
        /// this function handles all of the map drawing and colouring 
        /// it takes the coords provided by the word structs and then checks them against the letters that are being drawn
        /// it uses the same methord for drawing the incorrect letters
        /// 
        /// </summary>
        /// <param name="map">provide the actual map to be drawn</param>
        /// <param name="words">provides the words to check against the coordanates that the word struct provides</param>
        /// <param name="incorrectCoords"> if the last input was incorrect this value will provide the coord numbers between the two coords provided</param>
        /// <param name="lastInputWasIncorrect"> a bool that is true if the last input was incorrect</param>
        private static void drawMap(char[,] map, word[] words, int[,] incorrectCoords, bool lastInputWasIncorrect)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            string topBorder = "  ";
            if (map.GetLength(0) > 10)
            {
                topBorder = topBorder + " ";
            }
            for (int i = 0; i < map.GetLength(1); i++)
            {
                topBorder = topBorder + i + " ";
            }
            Console.WriteLine(topBorder);
            for (int x = 0; x < map.GetLength(0); x++)
            {
                // draw the border
                if (map.GetLength(0) > 10)
                {
                    if (x < 10)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(x + "  ");

                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(x + " ");

                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(x + " ");
                }
                // draw the grid



                for (int y = 0; y < map.GetLength(1); y++)
                {


                    Console.ForegroundColor = ConsoleColor.White;
                    for (int i = 0; i < words.Length; i++)
                    {
                        for (int u = 0; u < words[i].wrd.Length; u++)
                        {
                            if (words[i].wordCordinates[u, 0] == x && words[i].wordCordinates[u, 1] == y && words[i].found)
                            {

                                Console.ForegroundColor = ConsoleColor.Green;
                            }
                        }
                    }
                    if (lastInputWasIncorrect)
                    {
                        for (int i = 0; i < incorrectCoords.GetLength(0); i++)
                        {
                            if (incorrectCoords[i, 0] == x && incorrectCoords[i, 1] == y)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                            }
                        }
                    }
                    Console.Write(map[x, y] + " ");
                }

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
            }

        }

        /// <summary>
        /// this generates a puzzle struct directly from the file provided from the command line;
        /// </summary>
        /// <returns> the puzzle struct that the rest of the game relys on</returns>
        private static puzzle makePuzzle(string src)
        {
            // needs error programing in;

            string[] lines = readInFile(src);// get the file lines

            string[] properties = lines[0].Split(',');
            puzzle game = new puzzle();
            try
            {
                game.columns = int.Parse(properties[0].Trim());
                game.rows = int.Parse(properties[1].Trim());
                game.noWords = int.Parse(properties[2].Trim());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("\n \n File error with topline, should contain 3 numbers and two commas! ");
                Console.WriteLine("Press enter to exit");
                Console.ReadKey();
                Environment.Exit(0);
            }


            // checking those variables            
            if (lines.Length > game.noWords + 1)
            {
                Console.WriteLine("There are more lines that there should be in the text file");
                Console.Write("Press enter to exit");
                Console.ReadKey();
                Environment.Exit(0);
            }
            else if (lines.Length < game.noWords + 1)
            {
                Console.WriteLine("There are two few lines to for the number of Words specified on line 1");
                Console.Write("Press enter to exit");
                Console.ReadKey();
                Environment.Exit(0);

            }
            // words
            word[] words = new word[game.noWords];
            try
            {

                for (int i = 1; i <= game.noWords; i++)
                {
                    string[] data = lines[i].Split(',');
                    if (data[0] == "" || data[3] == "")
                    {
                        Console.WriteLine("skiped");
                        continue;
                    }
                    //Console.WriteLine(data[0] + " " + data[3]);
                    // Console.WriteLine(data.Length);
                    words[i - 1] = new word(data[0].ToLower().Trim(), int.Parse(data[1].Trim()), int.Parse(data[2].Trim()), data[3].ToLower().Trim());
                    //Console.WriteLine(data[3].ToLower().Trim());
                }
            }
            catch
            {
                Console.WriteLine("There was a format error with the text file");
                Console.Write("Please Press enter to exit");
                Console.ReadKey();
                Environment.Exit(0);
            }
            game.words = words;
            // puzzle assignment and return
            return game;
        }
        /// <summary>
        /// This is the controller for all the graphics methords 
        /// </summary>
        /// <param name="words"> provides all the words to draw</param>
        /// <param name="map">provides the map to draw the words too</param>
        private static void drawWords(word[] words, char[,] map)
        {
            // for every word we need to check that it fits the map if it doesn't we need to throw a exception and then close the program
            // I have two options, build for error or program for error afterwards
            int[,] letterCoords;
            int totalWordLength = 0;
            int counter = 0;
            for (int i = 0; i < words.Length; i++)
            {
                totalWordLength = totalWordLength + words[i].length;
            }
            letterCoords = new int[totalWordLength, 2];
            for (int x = 0; x < totalWordLength; x++)
            {
                letterCoords[x, 0] = 2147483647;
                letterCoords[x, 1] = 2147483647;
            }
            for (int i = 0; i < words.Length; i++)
            {
                //Console.WriteLine(i);
                switch (words[i].direction)
                {
                    case "left":
                        drawLeft(words[i], map, letterCoords, ref counter);
                        break;
                    case "right":
                        drawRight(words[i], map, letterCoords, ref counter);
                        break;
                    case "up":
                        drawUp(words[i], map, letterCoords, ref counter);
                        break;
                    case "down":
                        drawDown(words[i], map, letterCoords, ref counter);
                        break;
                    case "leftup":
                        drawLeftUp(words[i], map, letterCoords, ref counter);
                        break;
                    case "rightup":
                        drawRightUp(words[i], map, letterCoords, ref counter);
                        break;
                    case "rightdown":
                        drawRightDown(words[i], map, letterCoords, ref counter);
                        break;
                    case "leftdown":
                        drawLeftDown(words[i], map, letterCoords, ref counter);
                        break;
                }
            }
        }
        // drawing methrods
        /// <summary>
        /// All of the drawing methods are structured the same, they each break up the word into a char array and then draw the words letter by letter on the map
        /// it handles all the error catching with drawing the word on the map provided
        /// </summary>
        /// <param name="word">The word struct that is passed to the functions is the word to be drawn</param>
        /// <param name="map">Provids the map to be drawn on</param>
        /// <param name="letterCoords">Provides the coordinates of all the other letters that have meaning that have been drawn on the board</param>
        /// <param name="counter">Provides the number of letters that have currently been drawn on the map</param>
        private static void drawUp(word word, char[,] map, int[,] letterCoords, ref int counter)
        {
            char[] charArray = word.wrd.ToCharArray();
            for (int i = 0; i < word.wrd.Length; i++)
            {

                try
                {
                    for (int u = 0; u < letterCoords.GetLength(0); u++)
                    {
                        if (letterCoords[u, 0] == (word.row - i) && letterCoords[u, 1] == (word.column) && charArray[i] != map[word.row - i, (word.column)])
                        {
                            throw new Exception("There was a error with the text file a word is overlaping with another word");
                        }
                        else
                        {

                            continue;
                        }

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.Write("Press enter to exit");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
                try
                {
                    letterCoords[counter, 0] = word.row - i;
                    letterCoords[counter, 1] = (word.column);
                    counter++;
                    map[(word.row - i), word.column] = charArray[i];
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("A word tried to render outside of the map please check your map file and try again");
                    Console.Write("Press enter to exit ");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }
        }
        private static void drawDown(word word, char[,] map, int[,] letterCoords, ref int counter)
        {
            // Console.WriteLine("test");
            char[] charArray = word.wrd.ToCharArray();
            for (int i = 0; i < word.wrd.Length; i++)
            {
                try
                {
                    for (int u = 0; u < letterCoords.GetLength(0) - 1; u++)
                    {
                        if (letterCoords[u, 0] == (word.row + i) && letterCoords[u, 1] == (word.column) && charArray[i] != map[word.row + i, (word.column)])
                        {
                            //Console.WriteLine(word.row + i + " " + word.column + " " + charArray[i] + " " + map[word.row + i, word.column]);
                            throw new Exception("There was a error with the text file a word is overlaping with another word");
                        }
                        else
                        {

                            continue;
                        }


                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.Write("Press enter to exit");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
                try
                {
                    letterCoords[counter, 0] = word.row + i;
                    letterCoords[counter, 1] = (word.column);
                    counter++;
                    map[(word.row + i), word.column] = charArray[i];
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("A word tried to render outside of the map please check your map file and try again");
                    Console.Write("Press enter to exit ");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }
        }
        private static void drawRight(word word, char[,] map, int[,] letterCoords, ref int counter)
        {
            char[] charArray = word.wrd.ToCharArray();
            for (int i = 0; i < word.wrd.Length; i++)
            {
                try
                {
                    for (int u = 0; u < letterCoords.GetLength(0); u++)
                    {
                        if (letterCoords[u, 0] == word.row && letterCoords[u, 1] == (word.column + i) && charArray[i] != map[word.row, (word.column + i)])
                        {
                            throw new Exception("There was a error with the text file a word is overlaping with another word");
                        }
                        else
                        {

                            continue;
                        }


                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.Write("Press enter to exit");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
                try
                {
                    letterCoords[counter, 0] = word.row;
                    letterCoords[counter, 1] = (word.column + i);
                    counter++;
                    map[(word.row), (word.column) + i] = charArray[i];
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("A word tried to render outside of the map please check your map file and try again");
                    Console.Write("Press enter to exit ");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }
        }
        private static void drawLeft(word word, char[,] map, int[,] letterCoords, ref int counter)
        {
            char[] charArray = word.wrd.ToCharArray();

            for (int i = 0; i < word.wrd.Length; i++)
            {
                try
                {
                    for (int u = 0; u < letterCoords.GetLength(0); u++)
                    {
                        if (letterCoords[u, 0] == word.row && letterCoords[u, 1] == (word.column - i) && charArray[i] != map[word.row, (word.column - i)])
                        {
                            throw new Exception("There was a error with the text file a word is overlaping with another word");
                        }
                        else
                        {

                            continue;
                        }


                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.Write("Press enter to exit");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
                try
                {
                    letterCoords[counter, 0] = word.row;
                    letterCoords[counter, 1] = (word.column - i);
                    counter++;
                    map[(word.row), (word.column) - i] = charArray[i];
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("A word tried to render outside of the map please check your map file and try again");
                    Console.Write("Press enter to exit ");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }
        }
        private static void drawLeftUp(word word, char[,] map, int[,] letterCoords, ref int counter)
        {
            char[] charArray = word.wrd.ToCharArray();

            for (int i = 0; i < word.wrd.Length; i++)
            {
                try
                {
                    for (int u = 0; u < letterCoords.GetLength(0); u++)
                    {
                        if (letterCoords[u, 0] == word.row && letterCoords[u, 1] == (word.column - i) && charArray[i] != map[word.row - i, (word.column - i)])
                        {
                            throw new Exception("There was a error with the text file a word is overlaping with another word");
                        }
                        else
                        {

                            continue;
                        }


                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.Write("Press enter to exit");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
                try
                {
                    letterCoords[counter, 0] = word.row - i;
                    letterCoords[counter, 1] = (word.column - i);
                    counter++;
                    map[(word.row - i), (word.column - i)] = charArray[i];
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("A word tried to render outside of the map please check your map file and try again");
                    Console.Write("Press enter to exit ");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }


        }
        private static void drawRightUp(word word, char[,] map, int[,] letterCoords, ref int counter)
        {
            char[] charArray = word.wrd.ToCharArray();

            for (int i = 0; i < word.wrd.Length; i++)
            {
                try
                {
                    for (int u = 0; u < letterCoords.GetLength(0); u++)
                    {
                        if (letterCoords[u, 0] == (word.row - i) && letterCoords[u, 1] == (word.column + i) && charArray[i] != map[word.row - i, (word.column + i)])
                        {
                            throw new Exception("There was a error with the text file a word is overlaping with another word");
                        }
                        else
                        {

                            continue;
                        }


                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.Write("Press enter to exit");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
                try
                {
                    letterCoords[counter, 0] = word.row - i;
                    letterCoords[counter, 1] = (word.column + i);
                    counter++;
                    map[(word.row - i), (word.column + i)] = charArray[i];
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("A word tried to render outside of the map please check your map file and try again");
                    Console.Write("Press enter to exit ");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }


        }
        private static void drawRightDown(word word, char[,] map, int[,] letterCoords, ref int counter)
        {
            char[] charArray = word.wrd.ToCharArray();

            for (int i = 0; i < word.wrd.Length; i++)
            {
                try
                {
                    for (int u = 0; u < letterCoords.GetLength(0); u++)
                    {
                        if (letterCoords[u, 0] == (word.row + i) && letterCoords[u, 1] == (word.column + i) && charArray[i] != map[word.row + i, (word.column + i)])
                        {
                            throw new Exception("There was a error with the text file a word is overlaping with another word");
                        }
                        else
                        {

                            continue;
                        }


                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.Write("Press enter to exit");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
                try
                {
                    letterCoords[counter, 0] = word.row + i;
                    letterCoords[counter, 1] = (word.column + i);
                    counter++;
                    map[(word.row + i), (word.column + i)] = charArray[i];
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("A word tried to render outside of the map please check your map file and try again");
                    Console.Write("Press enter to exit ");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }


        }
        private static void drawLeftDown(word word, char[,] map, int[,] letterCoords, ref int counter)
        {
            char[] charArray = word.wrd.ToCharArray();

            for (int i = 0; i < word.wrd.Length; i++)
            {
                try
                {
                    for (int u = 0; u < letterCoords.GetLength(0); u++)
                    {
                        if (letterCoords[u, 0] == (word.row + i) && letterCoords[u, 1] == (word.column - i) && charArray[i] != map[word.row + i, (word.column - i)])
                        {
                            throw new Exception("There was a error with the text file a word is overlaping with another word");
                        }
                        else
                        {

                            continue;
                        }


                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.Write("Press enter to exit");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
                try
                {
                    letterCoords[counter, 0] = word.row + i;
                    letterCoords[counter, 1] = (word.column - i);
                    counter++;
                    map[(word.row + i), (word.column - i)] = charArray[i];
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("A word tried to render outside of the map please check your map file and try again");
                    Console.Write("Press enter to exit ");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }


        }
        private static void printWords(word[] words)
        {
            Console.WriteLine();
            Console.WriteLine("Words List: ");
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].found)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                Console.WriteLine(words[i].wrd);

            }
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Gray;

        }

        // input
        private static int[,] playerInput(char[,] map)
        {
            int[,] output = new int[2, 2];
            Console.WriteLine("Start co-ordinate");
            for (int i = 0; i < 2; i++)
            {
                bool playerinput = false;
                do
                {
                    playerinput = false;
                    try
                    {
                        Console.WriteLine("Please enter a Row co-ordiante between 0 and " + (map.GetLength(0) - 1));
                        Console.Write("Row co-ordiante: ");
                        output[i, 0] = int.Parse(Console.ReadLine());
                        playerinput = true;
                    }
                    catch
                    {
                        Console.WriteLine("Please enter a valid input");

                    }
                } while (!playerinput || !(output[i, 0] < 0 || output[i, 0] < map.GetLength(0)));
                playerinput = false;
                do
                {
                    playerinput = false;
                    try
                    {
                        Console.WriteLine("Please enter a Column co-ordiante between 0 and " + (map.GetLength(1) - 1));
                        Console.Write("Column co-ordiante: ");
                        output[i, 1] = int.Parse(Console.ReadLine());
                        playerinput = true;
                    }
                    catch
                    {
                        Console.WriteLine("Please enter a valid input");

                    }
                } while (!playerinput || !(output[i, 1] < 0 || output[i, 1] < map.GetLength(1)));
                if (i == 0)
                {
                    Console.WriteLine("Second Cordinate");
                }
            }
            return output;
        }
        private static bool validatePlayerInput(int[,] coords)
        {
            bool coordsAreOk = false;
            /*
             conditions for if the coords are ok
             the row is the same
             the column is the same
             
             diagonals -
            upRight - row-- column++
            upLeft - row-- column--
            downRight row++ column++
            downLeft row++ column --
             differences should be the same;
             */
            if (coords[0, 0] == coords[1, 0])
            {
                // Console.WriteLine("rows are the same");
                coordsAreOk = true;
            }
            else if (coords[0, 1] == coords[1, 1])
            {
                //Console.WriteLine("Columns are the same");
                coordsAreOk = true;
            }
            else if ((Math.Abs(coords[0, 0] - coords[1, 0])) == (Math.Abs(coords[0, 1] - coords[1, 1])))
            {
                //Console.WriteLine("valid Input");
                coordsAreOk = true;
            }
            return coordsAreOk;
        }
        private static bool checkPlayerInput(int[,] coords, word[] words)
        {
            bool wordHasBeenFound = false;
            // structure of input [Coord Number, xy]
            // lets print the coords so we know we have loaded them correctly then check them against the row and last row properties of the word struct
            for (int word = 0; word < words.Length; word++)
            {




                if (words[word].row == coords[0, 0] && words[word].column == coords[0, 1])
                {

                    if (words[word].lastRow == coords[1, 0] && words[word].lastColumn == coords[1, 1])
                    {
                        Console.WriteLine("Found word!!");
                        wordHasBeenFound = true;
                        words[word].found = true;
                    }

                }





            }
            return !wordHasBeenFound;
        }

        // game util
        private static int[,] getIntArray(int[,] coords)
        {
            int[,] output = new int[0, 2];
            int hor = coords[1, 1] - coords[0, 1];
            int vir = coords[1, 0] - coords[0, 0];
            int absHor = Math.Abs(coords[0, 1] - coords[1, 1]);
            int absVir = Math.Abs(coords[0, 0] - coords[1, 0]);

            if (absHor == 0)
            {
                output = new int[(absVir + 1), 2];
                if (vir < 0)
                {
                    for (int i = 0; i < (absVir + 1); i++)
                    {
                        output[i, 0] = coords[0, 0] - i;
                        output[i, 1] = coords[0, 1];
                    }
                }
                else
                {

                    for (int i = 0; i < (absVir + 1); i++)
                    {
                        output[i, 0] = coords[0, 0] + i;
                        output[i, 1] = coords[0, 1];
                    }
                }
            }
            else if (absVir == 0)
            {
                output = new int[(absHor + 1), 2];
                if (hor < 0)
                {
                    for (int i = 0; i < (absHor + 1); i++)
                    {
                        output[i, 0] = coords[0, 0];
                        output[i, 1] = coords[0, 1] - i;
                    }
                }
                else
                {
                    for (int i = 0; i < (absHor + 1); i++)
                    {
                        output[i, 0] = coords[0, 0];
                        output[i, 1] = coords[0, 1] + i;
                    }
                }
            }
            else
            {
                if ((absHor == 1 && absVir == 1) || (absHor == 2 && absVir == 2))
                {
                    output = new int[(((int)Math.Ceiling(Math.Sqrt(Math.Pow(absHor, 2) + Math.Pow(absVir, 2))))), 2];
                }
                else
                {
                    output = new int[(((int)Math.Ceiling(Math.Sqrt(Math.Pow(absHor, 2) + Math.Pow(absVir, 2)))) - 1), 2];
                }
                // this is the bit that is incorrect I need to make some sort of logic choice to 
                direction dir = direction.upLeft;
                if (hor > 0 && vir > 0)
                {
                    dir = direction.downRight;
                    // Console.WriteLine("downRight");
                }
                else if (hor < 0 && vir > 0)
                {
                    dir = direction.downLeft;
                    // Console.WriteLine("downLeft");
                }
                else if (hor < 0 && vir < 0)
                {
                    dir = direction.upLeft;
                    // Console.WriteLine("upLeft");
                }
                else if (hor > 0 && vir < 0)
                {
                    dir = direction.upRight;
                    //  Console.WriteLine("upRight");
                }
                switch (dir)
                {
                    case direction.upLeft:
                        for (int i = 0; i < output.GetLength(0); i++)
                        {
                            output[i, 0] = coords[0, 0] - i;
                            output[i, 1] = coords[0, 1] - i;
                        }
                        break;
                    case direction.upRight:
                        for (int i = 0; i < output.GetLength(0); i++)
                        {
                            output[i, 0] = coords[0, 0] - i;
                            output[i, 1] = coords[0, 1] + i;
                        }
                        break;
                    case direction.downLeft:
                        for (int i = 0; i < output.GetLength(0); i++)
                        {
                            output[i, 0] = coords[0, 0] + i; // row
                            output[i, 1] = coords[0, 1] - i; // column
                        }
                        break;
                    case direction.downRight:
                        for (int i = 0; i < output.GetLength(0); i++)
                        {
                            output[i, 0] = coords[0, 0] + i;
                            output[i, 1] = coords[0, 1] + i;
                        }
                        break;

                }

            }


            return output;
        }
        private static bool checkIfWon(word[] words)
        {
            bool hasWon = true;
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].found == false)
                {
                    hasWon = false;
                    break;
                }
            }
            return hasWon;
        }

        // general util
        private static string[] readInFile(string input)
        {
            int lineCount = 0;
            string[] output = new string[0];
            try
            {
                lineCount = File.ReadLines(@input).Count();
                output = new string[lineCount];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("There was a issue loading the file");
                Console.WriteLine("Press enter to exit");
                Console.ReadKey();
                Environment.Exit(0);
            }

            StreamReader sr = null;
            try
            {
                sr = new StreamReader(@input);


                for (int x = 0; x < lineCount; x++)
                {

                    output[x] = sr.ReadLine();
                    if (output[x] == "")
                    {
                        Console.WriteLine("There is a blank line that needs deleting");
                        Console.Write("press enter to exit");
                        Console.ReadKey();

                        Environment.Exit(0);
                    }

                    // Console.WriteLine(output[x]);

                }
                sr.Dispose();

            }
            catch (Exception e)
            {
                sr.Dispose();
                Console.WriteLine(e.Message);
                Console.WriteLine("Check your file location");
                Console.Write("Press enter to exit");
                Console.ReadKey();

                Environment.Exit(0);
            }

            // Read the stream to a string, and write the string to the console.



            return output;
        }
        private static bool debug()
        {
            return true;
        }

    }
}
