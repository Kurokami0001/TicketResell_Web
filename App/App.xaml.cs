﻿using System;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;

namespace App
{
    public partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
            UnhandledException += App_UnhandledException;
            
            Configuration.APIUrl = "http://localhost:5296/api";
            
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            
            Ioc.Default.ConfigureServices(new ServiceCollection()
                .AddSingleton<IConfiguration>(configuration)
                .InstallServices(configuration).BuildServiceProvider());
            
            Startup.InstallFrame();
        }

        private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            //TODO: Add logging
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            MainWindow = Ioc.Default.GetService<MainWindow>()!;
            MainWindow.Activate();
        }

        public static WindowEx MainWindow { get; set; }

        public static UIElement? AppTitlebar { get; set; }
    }
}