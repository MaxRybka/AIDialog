using System;
using System.Collections.Generic;
using System.Text;

namespace Dialog
{
    class UserHandler
    {
        public readonly string UserName = "YOU";


        public string ReadInput()
        {
            string res;
            Console.Write($"{UserName}>> ");
            res = Console.ReadLine();
            Console.WriteLine("");
            return res;
        } 
    }
}
