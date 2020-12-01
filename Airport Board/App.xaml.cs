﻿using Airport_Board.Services;
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

            builder.RegisterType<MainWindowViewModel>()
                    .AsSelf()
                    .SingleInstance();            

            return builder.Build();
        }
    }
}
