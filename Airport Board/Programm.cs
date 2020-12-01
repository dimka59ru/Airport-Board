using System;

namespace Airport_Board
{
    public static class Programm
    {
        [STAThread]
        public static void Main()
        {
                var app = new App();                
                app.InitializeComponent();
                app.Run();
        }
    }
}
