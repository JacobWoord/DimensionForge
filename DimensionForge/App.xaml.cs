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

namespace DimensionForge
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

      public  ServiceProvider serviceProvider;

        public App()
        {
            ServiceCollection serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<MainViewModel>();
            serviceCollection.AddSingleton<Canvas2DViewModel>();
            serviceCollection.AddSingleton<MainWindow>();
            serviceCollection.AddSingleton<Canvas3DViewModel>();

            
            serviceCollection.AddTransient<Canvas2DView>();
            serviceCollection.AddTransient<ViewPort3DXView>();

            serviceProvider = serviceCollection.BuildServiceProvider();
            Ioc.Default.ConfigureServices(serviceProvider);

        }

        protected override void OnStartup(StartupEventArgs e)
        {

            MainWindow = serviceProvider.GetService<MainWindow>();
            MainWindow.DataContext = serviceProvider.GetService<MainViewModel>();

            MainWindow.Show();

            base.OnStartup(e);
        }

    }
}
