﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diary
{
    internal class Program
    {
        static void Main()
        {
            HandleUserLogin.Login();
            UserIO.MainLoop();
        }
    }
}
