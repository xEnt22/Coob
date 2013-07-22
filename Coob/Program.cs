﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Coob.CoobEventArgs;
using Jint.Native;

namespace Coob
{
    class Root
    {
        public static Coob Coob;
        public static IScriptHandler Scripting;
        public static ScriptManager ScriptManager;

        static void Main(string[] args)
        {
            Console.Title = "Coob";
            Console.TreatControlCAsInput = true;
            Log.Info("Starting Coob.");

            Scripting = new JavascriptEngine();

            ScriptManager = new ScriptManager();
            ScriptManager.ScriptHandlers.Add(Scripting);
            ScriptManager.Initialize();

            Scripting.Run();

            Coob = new Coob(new CoobOptions
                            {
                                Port = 12345
                            });

            Scripting.SetParameter("coob", Root.Coob);

            var initializeEventArgs = new InitializeEventArgs(0);
            if (ScriptManager.CallEvent("OnInitialize", initializeEventArgs).Canceled)
                return;

            Coob.Options.WorldSeed = initializeEventArgs.WorldSeed; // Not sure if this is the best way to do this

            Coob.StartMessageHandler();

            while(Coob.Running)
            {
                var input = Console.ReadLine().ToLower();

                if (input == "exit") // Temporary way to quit server properly. Seems to fuck up because the console hates life.
                    Coob.StopMessageHandler();
            }

            Log.Info("Stopping server...");
            //Scripting.CallFunction("onQuit");
            ScriptManager.CallEvent("OnQuit", new QuitEventArgs(null));

            Log.Display(); // "Needs" to be called here since it normally gets called in the message handler (which isn't called anymore since server stopped).
        }
    }
}
