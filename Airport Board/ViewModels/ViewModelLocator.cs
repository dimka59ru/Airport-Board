using Autofac;
using System;
using System.Collections.Generic;
using System.Text;

namespace Airport_Board.ViewModels
{
    internal class ViewModelLocator
    {
        public MainWindowViewModel MainWindowViewModel
        {
            get
            {
                using var scope = App.Container.BeginLifetimeScope();
                var vm = scope.Resolve<MainWindowViewModel>();
                return vm;
            }
        }
    }
}
