using Airport_Board.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Airport_Board.Services
{
    internal class WindowsUserDialogService : IUserDialogService
    {
        public void ShowInformation(string Information, string Caption) => MessageBox.Show(Information, Caption, MessageBoxButton.OK, MessageBoxImage.Information);        
    }
}
