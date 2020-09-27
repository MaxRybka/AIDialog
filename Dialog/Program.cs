using System;
using System.Collections.Generic;
using System.Linq;

namespace Dialog
{
    class Program
    {
        static void Main(string[] args)
        {
            BotHandler bot = new BotHandler();
            UserHandler user = new UserHandler();

            bot.Say("Hi! What you want to talk about?");

            string answer;

            do
            {
                answer = user.ReadInput();
                bot.ProcessInput(answer);
            }
            while (!bot.IsDone);

            bot.Say("Bye!");

        }
    }
}
