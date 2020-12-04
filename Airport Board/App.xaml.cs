using Airport_Board.Services;
using Airport_Board.Services.Interfaces;
using Airport_Board.ViewModels;
using Autofac;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Airport_Board
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IContainer Container { get; set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Container = Configure();            
        }

        public static IContainer Configure()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<GetScheduleFromJsonFileService>()
                    .As<IGetScheduleFromFileService>()
                    .SingleInstance();

            builder.RegisterType<WindowsUserDialogService>()
                    .As<IUserDialogService>();

            builder.RegisterType<MainWindowViewModel>()
                    .AsSelf()
                    .SingleInstance();

            builder.RegisterType<FlightInfoViewModel>()
                    .AsSelf()
                    .SingleInstance();

            // Используется две таких модели
            builder.RegisterType<CountPassengersInfoViewModel>()
                    .AsSelf();                    

            return builder.Build();
        }
    }
}
