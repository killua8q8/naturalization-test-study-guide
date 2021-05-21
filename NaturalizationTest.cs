using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;

namespace Naturalization_Test
{
    class NaturalizationTest
    {
        private static DataSet NaturalizationTestDataSet;

        private static void Main()
        {
            Console.CursorVisible = false;
            Console.Write("System Initializing . . . ");
            InitializeTestData();
            Console.WriteLine("Initialized");
            Menu();
        }

        private static void Menu()
        {
            ConsoleKeyInfo keypressed = new();
            ConsoleKey[] KeyOne = new ConsoleKey[] { ConsoleKey.D1, ConsoleKey.NumPad1 };
            ConsoleKey[] KeyTwo = new ConsoleKey[] { ConsoleKey.D2, ConsoleKey.NumPad2 };
            ConsoleKey[] KeyThree = new ConsoleKey[] { ConsoleKey.D3, ConsoleKey.NumPad3 };
            ConsoleKey[] KeyFour = new ConsoleKey[] { ConsoleKey.D4, ConsoleKey.NumPad4 };

            DrawMainMenu();
            while (!KeyFour.Contains(keypressed.Key))
            {
                keypressed = Console.ReadKey(true);
                if (KeyOne.Contains(keypressed.Key))
                {
                    Study();
                    DrawMainMenu();
                }
                else if (KeyTwo.Contains(keypressed.Key))
                {
                    FlashCard();
                    DrawMainMenu();
                }
                else if (KeyThree.Contains(keypressed.Key))
                {
                    PickTen();
                    DrawMainMenu();
                }
                else if (KeyFour.Contains(keypressed.Key))
                {
                    Console.WriteLine("Good bye!");
                    break;
                }
                keypressed = new();
            }
        }

        private static void Study()
        {
            ConsoleKeyInfo keypressed = new();
            DataTable questions = NaturalizationTestDataSet.Tables["Questions"];
            DataTable answers = NaturalizationTestDataSet.Tables["Answers"];

            int[] questionIds = questions.AsEnumerable().Select(r => r.Field<int>("id")).ToArray();
            int curId = questionIds.Min();
            int minId = questionIds.Min();
            int maxId = questionIds.Max();

            DrawFrame("Study Mode", "\t\t[P]revious\t\t[N]ext\t\t[E]xit");
            while (keypressed.Key != ConsoleKey.E)
            {
                DataRow question = questions.AsEnumerable().Single(r => r.Field<int>("id") == curId);
                DataRow[] answerRows = answers.Select($"questionId = {question.Field<int>("id")}");
                DrawQuestion(question);
                DrawAnswers(answerRows);

                if (curId == maxId)
                {
                    DrawControlText("\t\t[P]revious\t\t[E]xit");
                }
                else if (curId == minId)
                {
                    DrawControlText("\t\t[N]ext\t\t[E]xit");
                }
                else
                {
                    DrawControlText("\t\t[P]revious\t\t[N]ext\t\t[E]xit");
                }

                keypressed = Console.ReadKey(true);
                if (keypressed.Key == ConsoleKey.N)
                {
                    curId = Math.Min(curId + 1, maxId);
                }
                else if (keypressed.Key == ConsoleKey.P)
                {
                    curId = Math.Max(curId - 1, minId);
                }
            }
        }

        private static void FlashCard()
        {
            ConsoleKeyInfo keypressed = new();
            DataTable questions = NaturalizationTestDataSet.Tables["Questions"];
            DataTable answers = NaturalizationTestDataSet.Tables["Answers"];

            Random rnd = new();
            int[] questionIds = questions.AsEnumerable().Select(r => r.Field<int>("id")).ToArray();
            for (int i = 0; i < questionIds.Length - 1; i++)
            {
                int r = rnd.Next(questionIds.Length - 1);
                int temp = questionIds[i];
                questionIds[i] = questionIds[r];
                questionIds[r] = temp;
            }
            int curIdx = 0;
            bool showAnswer = false;

            DrawFrame("Flash Card", "\t\t[C]ontinue\t\t[E]xit");
            while (keypressed.Key != ConsoleKey.E)
            {
                DataRow question = questions.AsEnumerable().Single(r => r.Field<int>("id") == questionIds[curIdx]);
                DrawQuestion(question);
                if (showAnswer)
                {
                    DataRow[] answerRows = answers.Select($"questionId = {question.Field<int>("id")}");
                    DrawAnswers(answerRows);
                } else
                {
                    DrawAnswers(Array.Empty<DataRow>());
                }

                if (curIdx == questionIds.Length - 1)
                {
                    DrawControlText(showAnswer ? "\t\t[E]xit" : "\t\t[A]nswer\t\t[E]xit");
                }
                else
                {
                    DrawControlText(showAnswer ? "\t\t[N]ext\t\t[E]xit" : "\t\t[A]nswer\t\t[S]kip\t\t[E]xit");
                }

                keypressed = Console.ReadKey(true);
                if (keypressed.Key == ConsoleKey.A && !showAnswer)
                {
                    showAnswer = true;
                }
                else if (keypressed.Key == ConsoleKey.N && showAnswer)
                {
                    if (curIdx != questionIds.Length - 1)
                    {
                        curIdx = Math.Min(curIdx + 1, questionIds.Length - 1);
                        showAnswer = false;
                    }
                }
                else if (keypressed.Key == ConsoleKey.S && !showAnswer)
                {
                    curIdx = Math.Min(curIdx + 1, questionIds.Length - 1);
                }
            }
        }

        private static void PickTen()
        {
            ConsoleKeyInfo keypressed = new();
            DataTable questions = NaturalizationTestDataSet.Tables["Questions"];
            DataTable answers = NaturalizationTestDataSet.Tables["Answers"];

            Random rnd = new();
            List<int> pickedQuestions = new();
            int[] questionIds = questions.AsEnumerable().Select(r => r.Field<int>("id")).ToArray();
            while (pickedQuestions.Count < 10)
            {
                int r = rnd.Next(questionIds.Length - 1);
                int id = questionIds[r];
                if (!pickedQuestions.Contains(id))
                {
                    pickedQuestions.Add(id);
                }
            }
            int curIdx = 0;
            bool showAnswer = false;

            DrawFrame("Pick 10", "\t\t[C]ontinue\t\t[E]xit");
            while (keypressed.Key != ConsoleKey.E)
            {
                DataRow question = questions.AsEnumerable().Single(r => r.Field<int>("id") == pickedQuestions[curIdx]);
                DrawQuestion(question);
                if (showAnswer)
                {
                    DataRow[] answerRows = answers.Select($"questionId = {question.Field<int>("id")}");
                    DrawAnswers(answerRows);
                }
                else
                {
                    DrawAnswers(Array.Empty<DataRow>());
                }

                if (curIdx == pickedQuestions.Count - 1 && showAnswer)
                {
                    DrawControlText("\t\t[E]xit");
                }

                keypressed = Console.ReadKey(true);
                if (keypressed.Key == ConsoleKey.C)
                {
                    if (showAnswer)
                    {
                        if (curIdx != pickedQuestions.Count - 1)
                        {
                            curIdx++;
                            showAnswer = false;
                        }
                    }
                    else
                    {
                        showAnswer = true;
                    }
                }
            }
        }

        private static void DrawMainMenu()
        {
            Console.Clear();
            Console.WriteLine("Welcome to Naturalization Test Study Guide.");
            Console.WriteLine("");
            Console.WriteLine("Please enter the number of the options below:");
            Console.WriteLine("[1]: Study");
            Console.WriteLine("[2]: Flash Card");
            Console.WriteLine("[3]: Pick 10");
            Console.WriteLine("[4]: Exit");
            Console.WriteLine("");
        }

        private static void DrawFrame(string title, string controlText)
        {
            Console.Clear();
            Console.WriteLine(new string(' ', (Console.WindowWidth - title.Length) / 2) + title);
            Console.WriteLine(new string('■', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.WindowHeight - 2);
            Console.Write(new string('■', Console.WindowWidth));
            DrawControlText(controlText);
        }

        private static void DrawControlText(string controlText)
        {
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            Console.Write(controlText);
        }

        private static void DrawQuestion(DataRow questionRow)
        {
            int screenWidth = Console.WindowWidth;

            string category = questionRow.Field<string>("category");
            string section = questionRow.Field<string>("section");
            string question = questionRow.Field<string>("question");

            Console.SetCursorPosition(0, 4);
            Console.WriteLine(new string(' ', screenWidth));
            Console.WriteLine(new string(' ', screenWidth));
            Console.WriteLine(new string(' ', screenWidth));
            Console.WriteLine(new string(' ', screenWidth));

            Console.SetCursorPosition(0, 4);
            Console.WriteLine(new string(' ', (screenWidth - category.Length) / 2) + category);
            Console.WriteLine(new string(' ', (screenWidth - section.Length) / 2) + section);
            Console.SetCursorPosition(0, 7);
            Console.WriteLine(new string(' ', (screenWidth - question.Length) / 2) + question);
        }

        private static void DrawAnswers(DataRow[] answerRows)
        {
            int maxDrawableRows = Console.WindowHeight - 12;
            Console.SetCursorPosition(0, 8);
            for (int i = 0; i < maxDrawableRows; i++)
            {
                Console.WriteLine(new string(' ', Console.WindowWidth));
            }

            if (answerRows.Length == 0)
            {
                return;
            }

            int columns = ((answerRows.Length - 1) / maxDrawableRows) + 1;
            int rows = ((answerRows.Length - 1) / columns) + 1;
            int columnWidth = Console.WindowWidth / columns;
            for (int c = 0; c < columns; c++)
            {
                int r = 0;
                int start = c * rows;
                int end = Math.Min(start + rows, answerRows.Length);
                foreach (DataRow row in answerRows[start..end])
                {
                    string answer = row.Field<string>("answer");
                    Console.SetCursorPosition(c * columnWidth, 10 + r++);
                    Console.Write(new string(' ', (columnWidth - answer.Length) / 2) + answer);
                }
            }

        }

        private static void InitializeTestData()
        {
            XmlDocument testDataXml = new();
            try
            {
                testDataXml.Load(@"TestData.xml");
            }
            catch (System.IO.FileNotFoundException)
            {
                Console.WriteLine("Cannot load test data file. Make sure an xml file named TestData is at the same directory of this program.");
                Environment.Exit(1);
            }

            XmlElement categories = testDataXml.DocumentElement["Categories"];
            if (!categories.HasChildNodes)
            {
                Console.WriteLine("No test data found in xml file.");
                Environment.Exit(1);
            }

            NaturalizationTestDataSet = new("NaturalizationTestDataSet");

            DataTable testQuestions = NaturalizationTestDataSet.Tables.Add("Questions");
            DataColumn pkQuestionId = testQuestions.Columns.Add("id", typeof(int));
            pkQuestionId.AutoIncrement = true;
            pkQuestionId.AutoIncrementSeed = 1;
            pkQuestionId.AutoIncrementStep = 1;
            _ = testQuestions.Columns.Add("category", typeof(string));
            _ = testQuestions.Columns.Add("section", typeof(string));
            _ = testQuestions.Columns.Add("question", typeof(string));
            testQuestions.PrimaryKey = new DataColumn[] { pkQuestionId };

            DataTable testAnswers = NaturalizationTestDataSet.Tables.Add("Answers");
            DataColumn pkAnswerId = testAnswers.Columns.Add("id", typeof(int));
            pkAnswerId.AutoIncrement = true;
            pkAnswerId.AutoIncrementSeed = 1;
            pkAnswerId.AutoIncrementStep = 1;
            _ = testAnswers.Columns.Add("questionId", typeof(int));
            _ = testAnswers.Columns.Add("answer", typeof(string));
            testAnswers.PrimaryKey = new DataColumn[] { pkAnswerId };

            NaturalizationTestDataSet.Relations.Add(new DataRelation("QuestionAnswer", pkQuestionId, testAnswers.Columns["questionId"]));

            foreach (XmlNode category in categories)
            {
                string categoryText = category["Text"].InnerText;

                XmlNodeList sections = category["Sections"].GetElementsByTagName("Section");
                foreach (XmlNode section in sections)
                {
                    string sectionText = section["Text"].InnerText;

                    XmlNodeList questions = section["Questions"].GetElementsByTagName("Question");
                    foreach (XmlNode question in questions)
                    {
                        string questionText = question["Text"].InnerText;
                        DataRow qRow = testQuestions.LoadDataRow(new object[] { null, categoryText, sectionText, questionText }, false);

                        XmlNodeList answers = question["Answers"].GetElementsByTagName("Answer");
                        foreach (XmlNode answer in answers)
                        {
                            string answerText = answer["Text"].InnerText;
                            _ = testAnswers.LoadDataRow(new object[] { null, qRow["id"], answerText }, false);
                        }
                    }
                }
            }

        }
    }
}
