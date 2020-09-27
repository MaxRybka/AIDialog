using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dialog
{
    class BotHandler
    {
        public string BotName { get; private set; } = "BOT";
        
        /// <summary>
        /// Path to existing data file
        /// </summary>
        private readonly string dataFilePath = "data/library.json";

        public bool IsDone { get; private set; } = false;

        private List<DataElement> loadedData = new List<DataElement>();

        private List<string> exitKeys = new List<string>()
            {
                "exit",
                "quit",
                "stop",
                "bye"
            };

        private List<string> deadEndQuestions  = new List<string>()
            {
                "I don't understand. Is there anything else you want to talk about?",
                "I am not quite sure what are you talking about. Is there anything else?"
            };

        public BotHandler()
        {
            LoadData(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dataFilePath));
        }

        public void ProcessInput(string input)
        {
            //remove all signs
            input = new string((from c in input
                                where char.IsWhiteSpace(c) || char.IsLetterOrDigit(c) || c.Equals('\'') || c.Equals('?')
                                select c).ToArray());

            //change to lower case
            input = input.ToLower();

            //exit app
            if (exitKeys.Any((a) => a == input))
            {
                IsDone = true;
                return;
            }

            //split input in tokens
            string[] inputTokens = input.Split(' ');
            

            List<string> answers = new List<string>();

            //check with every rule in dataset
            foreach (DataElement rule in loadedData)
            {
                int ruleIndex = 0;
                bool isSkipSign = false;
                string parametr = "";
                string[] ruleTokens = rule.key.Split(' ');

                if (ruleTokens[0].Equals("*"))
                {
                    ruleIndex++;
                    isSkipSign = true;
                }

                for (int i = 0; i < inputTokens.Length && ruleIndex < ruleTokens.Length; i++)
                {

                    if (inputTokens[i].Equals(ruleTokens[ruleIndex]))
                    {
                        ruleIndex++;
                        isSkipSign = false;
                    }
                    else if (ruleTokens[ruleIndex].Equals("<a>"))
                    {
                        parametr = inputTokens[i];
                        ruleIndex++;
                        isSkipSign = false;
                    }
                    else if (ruleTokens[ruleIndex].Equals("*"))
                    {
                        ruleIndex++;
                        isSkipSign = true;
                    }
                    else if (isSkipSign)
                    {
                        isSkipSign = true;
                    }
                    else
                    {
                        break;
                    }


                    if (ruleTokens.Length == ruleIndex)
                    {
                        answers.Add(GetRandom(rule.answer).Replace("<a>", parametr));
                        break;
                    }
                }

                //check if there * sign left
                while(ruleIndex < ruleTokens.Length)
                {
                    if (ruleTokens[ruleIndex].Equals("*"))
                    {
                        ruleIndex++;
                    }
                    else
                    {
                        break;
                    }

                    if (ruleTokens.Length == ruleIndex)
                    {
                        answers.Add(GetRandom(rule.answer).Replace("<a>", parametr));
                        break;
                    }
                }
            }

            //random answer from dead end questions
            if(answers.Count == 0)
            {
                Say(GetRandom(deadEndQuestions));
                return;
            }


            Say(answers.First());
        }

        public void Say(string sentence)
        {
            Console.WriteLine($"{BotName}>> {sentence}\n");
        }

        private void LoadData(string filePath)
        {
            loadedData = JsonConvert.DeserializeObject<List<DataElement>>(File.ReadAllText(filePath));
        }

        private string GetRandom(List<string> list)
        {
            Random rnd = new Random();
            int index = rnd.Next(list.Count);
            return list[index];
        }

    }
}
