using System;
using System.Threading;
using System.Windows.Input;

namespace SkipCode
{
    public static class EscapeKey
    {
        private static bool escapePressed = false;
        private static Thread thread;
        
        private static void CheckEscapePressed()
        {
            while (true)
            {
                if (Keyboard.IsKeyDown(Key.Escape))
                {
                    EscapeKey.escapePressed = true;
                }
                else
                {
                    EscapeKey.escapePressed = false;
                }
            }
        }

        public static void StartListening()
        {
            thread = new System.Threading.Thread(EscapeKey.CheckEscapePressed);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Priority = ThreadPriority.Lowest;
            thread.Start();
        }

        public static bool Pressed
        {
            get
            {
                return EscapeKey.escapePressed;
            }
            set
            {
                throw new Exception("EscapeKey.Pressed value is read only.");
            }
        }

        public static void StopListening()
        {
            thread.Abort();
            EscapeKey.escapePressed = false;
        }
    }
}
