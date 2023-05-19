global using CommunityToolkit.Mvvm.Collections;
global using CommunityToolkit.Mvvm.Input;
global using CommunityToolkit.Mvvm.ComponentModel;
global using CommunityToolkit.Mvvm.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DimensionForge.Main.ViewModels;
using DimensionForge._2D.ViewModels;
using DimensionForge._2D.Views;
using DimensionForge._3D.ViewModels;
using DimensionForge._3D.Views;
using log4net;
using log4net.Config;
using System.IO;
using System.Diagnostics;
using DimensionForge._3D.Views.ToolbarViews;

namespace DimensionForge
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(App));

        public  ServiceProvider serviceProvider;


        public App()
        {
            ServiceCollection serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<MainViewModel>();
           
            serviceCollection.AddSingleton<Canvas2DViewModel>();
            serviceCollection.AddSingleton<MainWindow>();
            serviceCollection.AddSingleton<Canvas3DViewModel>();
 

            serviceCollection.AddTransient<CoordinationViewModel>();
            serviceCollection.AddTransient<ItemsListViewModel>();
            serviceCollection.AddTransient<FloorTexturesViewModel>();
            serviceCollection.AddTransient<MoveControlsViewModel>();
           
            serviceProvider = serviceCollection.BuildServiceProvider();
            Ioc.Default.ConfigureServices(serviceProvider);

        }

        protected override void OnStartup(StartupEventArgs e)
        {

            MainWindow = serviceProvider.GetService<MainWindow>();
            MainWindow.DataContext = serviceProvider.GetService<MainViewModel>();
            MainWindow.Show();

            base.OnStartup(e);

            XmlConfigurator.Configure(new FileInfo("log4net.config"));
            log.Debug("Application started");
            
        }

    }
}
