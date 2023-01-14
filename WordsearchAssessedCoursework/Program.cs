using System;
using System.IO;

/* Explicit Name Convention
 * 
 * Method names start with an uppercase character and then camel case
 * Parameters in the method start with a p for parameter
 * Local variables start with lowercase and then camel case
 */ 
namespace WordsearchAssessedCoursework
{
    class Program
    {
        /// <summary>
        /// Return a number between the maximum and the minimum value, inclusive
        /// </summary>
        /// <param name="question">What we ask from the user</param>
        /// <param name="max">max value, not inclusive</param>
        /// <param name="min">min value, inclusive</param>
        /// <returns>return an integer between min and max</returns>
        static int GetNumInRange(string pQuestion, int pMax, int pMin)
        {
            int answere = pMax+1;
            do
            {
                Console.WriteLine(pQuestion);
                Console.WriteLine($"Enter a number between {pMin} and {pMax}.");
                try
                {
                    answere = int.Parse(Console.ReadLine());
                }
                catch 
                {
                    Console.WriteLine("Enter a valid input. ");
                }
            } while (answere < pMin || answere > pMax);
            return answere;
        }
        /// <summary>
        /// Outputs a selection list from a string array, then pass it to the GetNumInRange method.
        /// </summary>
        /// <param name="pQuestion">what we ask from the user</param>
        /// <param name="option">The option array that the user can select</param>
        /// <returns>returns a number</returns>
        static int Selection(string pQuestion, string[] pOption)
        {
            string display = pQuestion + "\r\n";
            for (int i = 0; i < pOption.Length; i++)
            {
                display += $"{(i + 1)}. {pOption[i]}\r\n";
            }
            return GetNumInRange(display, pOption.Length, 1);
        }
        struct Puzzle 
        {
            public int height;
            public int width;
            public int numWords;
        }
        struct Word 
        {
            public string word;
            public int x_pos;
            public int y_pos;
            public string direction;
            public int x_posEnd;
            public int y_posEnd;
            public int found;
        }
        /// <summary>
        /// Read the main information about the Puzzle, like height, width, number of words 
        /// </summary>
        /// <param name="pFileNum">the number from the selected option</param>
        /// <returns></returns>
        static Puzzle[] LoadPuzzle(int pFileNum) 
        {
            StreamReader reader = new StreamReader($"File0{pFileNum}.wrd");

            reader.Close();
            Puzzle[] data = new Puzzle[1];
            try
            {
                reader = new StreamReader($"File0{pFileNum}.wrd");
                string line = reader.ReadLine();
                string[] values = line.Split(',');
                data[0].width = int.Parse(values[0]);
                data[0].height = int.Parse(values[1]);
                data[0].numWords = int.Parse(values[2]);
            }
            catch
            {
                Console.WriteLine("The file is incorrect, try another file");
            }
            finally 
            {
                reader.Close();
            }
            return data;
        }
        /// <summary>
        /// Reads the details about the words like, the word, x postition, y position and direction,
        /// then pass it to the check method to see if the file correct or not.
        /// </summary>
        /// <param name="pFileNum"></param>
        /// <param name="pSize"></param>
        /// <param name="pWidth"></param>
        /// <param name="pHeight"></param>
        /// <returns></returns>
        static Word[] LoadWords(int pFileNum, int pSize, int pWidth, int pHeight)
        {
            StreamReader reader = new StreamReader($"File0{pFileNum}.wrd");

            int numberOfLines = 0;
            while (!reader.EndOfStream)
            {
                reader.ReadLine();
                numberOfLines++;
            }

            reader.Close();

            reader = new StreamReader($"File0{pFileNum}.wrd");
            Word[] words = new Word[pSize];
            reader.ReadLine();//skip first line
            for (int i =0; i<numberOfLines-1;i++) {
                string line = reader.ReadLine();
                string[] values = line.Split(',');
                words[i].word = values[0];
                words[i].x_pos = int.Parse(values[1]);
                words[i].y_pos = int.Parse(values[2]);
                words[i].direction = values[3];
            }

            bool pass = CheckFormat(words,numberOfLines,pWidth,pHeight);


            if (pass)
            {
                return words;
            }
            else 
            {
                throw new FormatException();
            }
        }
        /// <summary>
        /// Check the informations are correct or not, then returns true or false.
        /// </summary>
        /// <param name="pwords"></param>
        /// <param name="pNumOfLines"></param>
        /// <param name="pWidth"></param>
        /// <param name="pHeight"></param>
        /// <returns>Return true if it pass the conditions.</returns>
        static bool CheckFormat(Word[] pwords, int pNumOfLines, int pWidth, int pHeight)
        {
            bool con = false;
            bool con2 = false;

            //check for Words outside of grid
            for (int i = 0; i < pNumOfLines - 1; i++)
            {
                string wLength = pwords[i].word;
                if ((pwords[i].direction == "right" || pwords[i].direction == "left") && pWidth < wLength.Length)
                {
                    con2 = false;
                    break;
                }
                else if ((pwords[i].direction == "down" || pwords[i].direction == "up") && pHeight < wLength.Length)
                {
                    con2 = false;
                    break;
                }
                else
                {
                    con2 = true;
                }
            }
            //check for overlapping
            for (int i = 0; i < pNumOfLines - 2; i++)
            {
                if (pwords[i].x_pos == pwords[i + 1].x_pos && pwords[i].y_pos == pwords[i + 1].y_pos)
                {
                    con = false;
                    break;
                }
                else
                {
                    con = true;
                }
            }

            return (con && con2);
        }
        /// <summary>
        /// Create a grid, fill it with random letters, then fill it with the words
        /// </summary>
        /// <param name="pWidth">width of the grid</param>
        /// <param name="pHeight">Height of the grid</param>
        /// <param name="pWords">Struc which contains all the data about the words</param>
        /// <returns>Returns the grid</returns>
        static char[,] SetUpGrid(int pWidth, int pHeight, Word[] pWords) 
        {
            //setup grid with random letters
            string abc = "abcdefghijklmnopqrstuvwxyz";
            Random rng = new Random();
            char[,] grid = new char[pWidth, pHeight];

            for (int rows = 0; rows < grid.GetLength(1); rows++)
            {
                for (int columns = 0; columns < grid.GetLength(0); columns++)
                {
                    grid[columns, rows] = abc[rng.Next(abc.Length)];
                }
            }

            //Place the words inside the grid
            for (int i=0;i<pWords.Length;i++) 
            {
                pWords[i].found = 0;
                char[] wArray = pWords[i].word.ToCharArray();

                if (pWords[i].direction == "right")
                {
                    for (int j = 0; j < wArray.Length; j++)
                    {
                         grid[pWords[i].x_pos + j, pWords[i].y_pos] = wArray[j];
                    }
                    pWords[i].x_posEnd = pWords[i].x_pos + wArray.Length-1;
                    pWords[i].y_posEnd = pWords[i].y_pos;
                }
                else if (pWords[i].direction == "left")
                {
                    for (int j = 0; j < wArray.Length; j++)
                    {
                        grid[(pWords[i].x_pos - wArray.Length + 1) + j, pWords[i].y_pos] = wArray[(wArray.Length - 1) - j];
                    }
                    pWords[i].x_posEnd = pWords[i].x_pos - wArray.Length+1;
                    pWords[i].y_posEnd = pWords[i].y_pos;
                }
                else if (pWords[i].direction == "down") 
                {
                    for (int j = 0; j < wArray.Length; j++)
                    {
                        grid[pWords[i].x_pos, pWords[i].y_pos+j] = wArray[j];
                    }
                    pWords[i].x_posEnd = pWords[i].x_pos;
                    pWords[i].y_posEnd = pWords[i].y_pos + wArray.Length - 1;
                }
                else if (pWords[i].direction == "up")
                {
                    for (int j = 0; j < wArray.Length; j++)
                    {
                        grid[pWords[i].x_pos, (pWords[i].y_pos - wArray.Length + 1) + j] = wArray[(wArray.Length - 1) - j];
                    }
                    pWords[i].x_posEnd = pWords[i].x_pos;
                    pWords[i].y_posEnd = pWords[i].y_pos - wArray.Length + 1;
                }
                else if (pWords[i].direction == "rightdown")
                {
                    for (int j = 0; j < wArray.Length; j++)
                    {
                        grid[pWords[i].x_pos+j, pWords[i].y_pos+j] = wArray[j];
                    }
                    pWords[i].x_posEnd = pWords[i].x_pos+ (wArray.Length-1);
                    pWords[i].y_posEnd = pWords[i].y_pos+ (wArray.Length-1);
                }
                else if (pWords[i].direction == "rightup")
                {
                    for (int j = 0; j < wArray.Length; j++)
                    {
                        grid[pWords[i].x_pos + j, pWords[i].y_pos - j] = wArray[j];
                    }
                    pWords[i].x_posEnd = pWords[i].x_pos + wArray.Length - 1;
                    pWords[i].y_posEnd = pWords[i].y_pos - wArray.Length + 1;
                }
                else if (pWords[i].direction == "leftup")
                {
                    for (int j = 0; j < wArray.Length; j++)
                    {
                        grid[pWords[i].x_pos - j, pWords[i].y_pos - j] = wArray[j];
                    }
                    pWords[i].x_posEnd = pWords[i].x_pos - wArray.Length + 1;
                    pWords[i].y_posEnd = pWords[i].y_pos - wArray.Length + 1;
                }
                else if (pWords[i].direction == "leftdown")
                {
                    for (int j = 0; j < wArray.Length; j++)
                    {
                        grid[pWords[i].x_pos - j, pWords[i].y_pos + j] = wArray[j];
                    }
                    pWords[i].x_posEnd = pWords[i].x_pos - wArray.Length + 1;
                    pWords[i].y_posEnd = pWords[i].y_pos + wArray.Length - 1;
                }
            }

            SavePuzzle(grid, pWidth, pHeight, pWords);
            DrawGrid(grid, pWidth, pHeight,pWords, -1, -1, -1, -1);
            return grid;
        }
        /// <summary>
        /// Draw the grid to the console with the word included
        /// </summary>
        /// <param name="pGrid">The letter and the positions</param>
        /// <param name="pWidth">Width of the puzzle</param>
        /// <param name="pHeight">Height of the puzzle</param>
        /// <param name="pWords">The words and the infomations bout it</param>
        /// <param name="pSmallerC">Smaler value what the user enter, for the column</param>
        /// <param name="pBiggerC">The greater value what the user enter for the column</param>
        /// <param name="pSmallerR">The smaler value for the row</param>
        /// <param name="pBiggerR">The greater value for the row</param>
        /// <returns></returns>
        static Word[] DrawGrid(char[,] pGrid, int pWidth, int pHeight, Word[] pWords, int pSmallerC,int pBiggerC, int pSmallerR, int pBiggerR) 
        {
            //output the row numbers
            Console.Clear();
            Console.Write(" ");
            for (int rows = 0; rows < 1; rows++)
            {
                for (int columns = 0; columns < pGrid.GetLength(0); columns++)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($" {columns}");
                    Console.ResetColor();
                }
            }
            Console.WriteLine();

            //draw the grid
            for (int rows = 0; rows < pHeight; rows++)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(rows);
                Console.ResetColor();
                for (int columns = 0; columns < pWidth; columns++)
                {
                    if ((columns >= pSmallerC && rows >= pSmallerR) && (columns <= pBiggerC && rows <= pBiggerR))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write($" {pGrid[columns, rows]}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write($" {pGrid[columns, rows]}");
                    }
                }
                Console.WriteLine();
            }
            //output the words
            for (int i = 0; i < pWords.Length; i++) 
            {
                if (pWords[i].found == 1)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(pWords[i].word);
                    Console.ResetColor();
                }
                else 
                {
                    Console.WriteLine(pWords[i].word);
                }
            }
            return pWords;
        }/// <summary>
        /// Asking a question until geting a yes or no answere
        /// </summary>
        /// <param name="question">What we ask from the user</param>
        /// <returns>Returns yes or no</returns>
        static string GetYesOrNo(string question) 
        {
            string answere;
            Console.WriteLine("Congratulation, you won!");
            do
            {
                Console.WriteLine(question);
                answere=Console.ReadLine();
            } while (answere !="y" && answere != "n" && answere != "no" && answere != "yes");
            
            return answere;
        }
        /// <summary>
        /// Automaticly saves the grid, the width of the grid, the the height of the grid and the words to a wordsearch.sav file,
        /// if the user closes the program.
        /// </summary>
        /// <param name="pGrid">letter, positions.</param>
        /// <param name="pWidth">Width of the grid.</param>
        /// <param name="pHeight">Height of the grid.</param>
        /// <param name="pWords">Informations about the words.</param>
        static void SavePuzzle(char[,] pGrid, int pWidth, int pHeight, Word[] pWords) 
        {
            StreamWriter writer = new StreamWriter("wordsearch.sav");
            writer.WriteLine($"{pWidth},{pHeight},{pWords.Length}");

            for (int rows = 0; rows < pHeight; rows++)
            {
                for (int columns = 0; columns < pWidth; columns++)
                {
                    writer.Write(pGrid[columns, rows]);
                }
                writer.WriteLine();
            }
            for (int i = 0; i < pWords.Length; i++) 
            {
                writer.WriteLine($"{pWords[i].word},{pWords[i].x_pos},{pWords[i].y_pos},{pWords[i].x_posEnd},{pWords[i].y_posEnd},{pWords[i].found}");
            }
            writer.Close();

        }
        /// <summary>
        /// Load the puzzle, where it was left from a wordsearch.sav file.
        /// </summary>
        /// <param name="pData">informations about the Puzzle, like widht, height, number of words.</param>
        /// <param name="grid">The letters form the grid and positions of the letters.</param>
        /// <returns>Returns the words and the informations about it.</returns>
        static Word[] LoadSavedPuzzle(out Puzzle[] pData,out char[,]grid) 
        {
            pData = new Puzzle[3];
            StreamReader reader = new StreamReader("wordsearch.sav");
            int numberOfLines = 0;
            while (!reader.EndOfStream)
            {
                reader.ReadLine();
                numberOfLines++;
            }
            reader.Close();

            reader = new StreamReader("wordsearch.sav");
            string line = reader.ReadLine();
            string[] values = line.Split(',');
            pData[0].width = int.Parse(values[0]);
            pData[0].height = int.Parse(values[1]);
            pData[0].numWords = int.Parse(values[2]);
            grid = new char[pData[0].width, pData[0].height];

            for (int i =0;i< pData[0].height; i++) 
            {
                line = reader.ReadLine();
                char[] ch = line.ToCharArray();
                for (int j=0;j< pData[0].width; j++) 
                {
                    grid[j, i] = ch[j];
                }
            }


            Word[] wData = new Word[pData[0].numWords];
            for (int i = pData[0].height + 2; i < numberOfLines+1; i++) 
            {
                line = reader.ReadLine();
                values = line.Split(',');
                wData[i - (pData[0].height + 2)].word = values[0];
                wData[i - (pData[0].height + 2)].x_pos = int.Parse(values[1]);
                wData[i - (pData[0].height + 2)].y_pos = int.Parse(values[2]);
                wData[i - (pData[0].height + 2)].x_posEnd = int.Parse(values[3]);
                wData[i - (pData[0].height + 2)].y_posEnd = int.Parse(values[4]);
                wData[i - (pData[0].height + 2)].found = int.Parse(values[5]);

            }
            for (int i=0;i<wData.Length;i++) 
            {
                if (wData[i].found==1) 
                {
                    pData[0].numWords --;
                }
            }
            reader.Close();

            DrawGrid(grid, pData[0].width, pData[0].height, wData,-1, -1, -1, -1);
            return wData;
        }
        /// <summary>
        /// Checks if the user input is match to the positions of the words.
        /// If it match then check it if it was found before or not.
        /// If it corrects then pass it to the save method and to the draw method.
        /// </summary>
        /// <param name="pColum1">Firs column coordinate, what the user enter</param>
        /// <param name="pRow1">Firs row coordinate, what the user enter</param>
        /// <param name="pColum2">Secound column coordinate, what the user enter</param>
        /// <param name="pRow2">Secound row coordinate, what the user enter</param>
        /// <param name="pData">Data about he puzzle</param>
        /// <param name="pWord">Data about the words</param>
        /// <param name="pNumWord">Number of words</param>
        /// <param name="pGrid">The letters and the coordinates</param>
        static int GameLogick(int pColum1, int pRow1, int pColum2, int pRow2, Puzzle[] pData, Word[] pWord,int pNumWord,char[,] pGrid) 
        {
            int smallerC;
            int biggerC;
            int smallerR;
            int biggerR;

            if (pColum1 < pColum2)
            {
                smallerC = pColum1;
                biggerC = pColum2;
            }
            else 
            {
                smallerC = pColum2;
                biggerC = pColum1;
            }

            if (pRow1 < pRow2)
            {
                smallerR = pRow1;
                biggerR = pRow2;
            }
            else
            {
                smallerR = pRow2;
                biggerR = pRow1;
            }

            for (int i=0;i<pWord.Length;i++) 
            {
                if (pColum1 == pWord[i].x_pos && pRow1 == pWord[i].y_pos)
                {
                    if (pColum2 == pWord[i].x_posEnd && pRow2 == pWord[i].y_posEnd)
                    {
                        if (pWord[i].found == 1)
                        {
                            Console.WriteLine($"You already found the word {pWord[i].word}!");
                            break;
                        }
                        else
                        {
                            pNumWord = pNumWord - 1;
                            pWord[i].found = 1;
                            SavePuzzle(pGrid, pData[0].width, pData[0].height, pWord);
                            DrawGrid(pGrid, pData[0].width, pData[0].height, pWord, smallerC, biggerC, smallerR, biggerR);
                        }
                    }
                }
                else 
                {
                    DrawGrid(pGrid, pData[0].width, pData[0].height, pWord, smallerC, biggerC, smallerR, biggerR);
                }
            }
            return pNumWord;
        }

        static void Main()
        {
            string answere;
            do
            {
                Console.Clear();
                Console.WriteLine("Wordsearch Application");
                string[] menuOption = { "Use default wordsearch", "Load wordsearch from file", "Resume last saved wordsearch" };
                int menuSelect = Selection("Select an option:", menuOption);

                //Default HardCoded
                if (menuSelect == 1)
                {

                    Console.Clear();
                    int height = 5;
                    int width = 9;
                    Puzzle[] data = new Puzzle[1];
                    data[0].height = height;
                    data[0].width = width;
                    data[0].numWords = 2;

                    Word[] words = new Word[2];
                    words[0].direction = "right";
                    words[0].word = "algorithm";
                    words[0].x_pos = 0;
                    words[0].y_pos = 1;
                    words[0].x_posEnd = words[0].x_pos + (words[0].word).Length - 1;
                    words[0].y_posEnd = words[0].y_pos;
                    words[0].found = 0;

                    words[1].word = "virus";
                    words[1].direction = "left";
                    words[1].x_pos = 5;
                    words[1].y_pos = 4;
                    words[1].x_posEnd = words[1].x_pos - words[1].word.Length + 1;
                    words[1].y_posEnd = words[1].y_pos;
                    words[1].found = 0;

                    char[,] grid = SetUpGrid(width, height, words);

                    do
                    {
                        int userColum1 = GetNumInRange("Select a Colum", data[0].width, 0);
                        int userRow1 = GetNumInRange("Select a Row", data[0].height, 0);
                        int userColum2 = GetNumInRange("Select a Colum", data[0].width, 0);
                        int userRow2 = GetNumInRange("Select a Row", data[0].height, 0);
                        data[0].numWords = GameLogick(userColum1, userRow1, userColum2, userRow2, data, words, data[0].numWords, grid);
                    } while (data[0].numWords > 0);

                }
                //Selectable
                else if (menuSelect == 2)
                {
                    Puzzle[] data;
                    Word[] words;
                    char[,] grid;
                    int numWords;
                    while (true)
                    {
                        try
                        {
                            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.wrd");
                            int fileSelect = Selection("Select a file to load", files);
                            data = LoadPuzzle(fileSelect);
                            numWords = data[0].numWords;
                            words = LoadWords(fileSelect, numWords, data[0].width, data[0].height);
                            grid = SetUpGrid(data[0].width, data[0].height, words);
                            break;
                        }
                        catch
                        {
                            Console.WriteLine("The file is incorrect, try another file");
                        }
                    }
                    do
                    {
                        int userColum1 = GetNumInRange("Select a Colum", data[0].width, 0);
                        int userRow1 = GetNumInRange("Select a Row", data[0].height, 0);
                        int userColum2 = GetNumInRange("Select a Colum", data[0].width, 0);
                        int userRow2 = GetNumInRange("Select a Row", data[0].height, 0);
                        numWords = GameLogick(userColum1, userRow1, userColum2, userRow2, data, words, numWords, grid);
                    } while (numWords > 0);

                }
                //Saved option
                else
                {
                    char[,] grid;
                    Puzzle[] data;
                    Word[] words = LoadSavedPuzzle(out data, out grid);
                    do
                    {
                        int userColum1 = GetNumInRange("Select a Colum", data[0].width, 0);
                        int userRow1 = GetNumInRange("Select a Row", data[0].height, 0);
                        int userColum2 = GetNumInRange("Select a Colum", data[0].width, 0);
                        int userRow2 = GetNumInRange("Select a Row", data[0].height, 0);
                        data[0].numWords = GameLogick(userColum1, userRow1, userColum2, userRow2, data, words, data[0].numWords, grid);
                    } while (data[0].numWords > 0);
                }
                answere = GetYesOrNo("Would you like to play again? y/n");
            } while (answere != "n" && answere != "no");
        }
    }
}
