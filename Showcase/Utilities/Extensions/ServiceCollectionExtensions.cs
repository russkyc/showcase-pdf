// MIT License
// 
// Copyright (c) 2024 Russell Camo (Russkyc)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.IO;
using JsonFlatFileDataStore;
using Microsoft.Extensions.DependencyInjection;
using Showcase.Services.Configuration;
using Showcase.Services.Configuration.Interfaces;
using Showcase.Services.Datastore;
using Showcase.Services.Datastore.Interfaces;
using Showcase.Services.DisplayManager;
using Showcase.Services.DisplayManager.Interfaces;
using Showcase.Services.PdfReader;
using Showcase.Services.PdfReader.Interfaces;
using Showcase.Services.WindowManager;
using Showcase.Services.WindowManager.Interfaces;
using Showcase.ViewModels;
using Showcase.Views;

namespace Showcase.Utilities.Extensions;

public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddShowcaseViews(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient(
            services => new FilesView()
            {
                DataContext = services.GetService<FilesViewModel>()
            });

        serviceCollection.AddTransient(
            services => new PresenterView()
            {
                DataContext = services.GetService<PresenterViewModel>()
            });

        serviceCollection.AddTransient(
            services => new ScreenView()
            {
                DataContext = services.GetService<ScreenViewModel>()
            });

        serviceCollection.AddTransient(
            services => new SettingsView()
            {
                DataContext = services.GetService<SettingsViewModel>()
            });

        serviceCollection.AddTransient(_ => new AboutView());

        return serviceCollection;
    }

    public static IServiceCollection AddShowcaseViewModels(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton(
            services => new FilesViewModel(
                services.GetService<IPdfManager>(),
                services.GetService<IWindowFactory>(),
                services.GetService<IPresentationStore>()));

        serviceCollection.AddSingleton(
            services => new SettingsViewModel(
                services.GetService<IDisplayManager>()));

        serviceCollection.AddSingleton(
            services => new PresenterViewModel(
                services.GetService<IAppConfig>(),
                services.GetService<IWindowFactory>(),
                services.GetService<IDisplayManager>(),
                services.GetService<IPresentationStore>()));

        serviceCollection.AddSingleton(_ => new ScreenViewModel());

        return serviceCollection;
    }

    public static IServiceCollection AddShowcaseServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IAppConfig, AppConfig>(
            _ => new AppConfig(
                Path.Combine(
                    Environment.CurrentDirectory,"config.json")));

        serviceCollection.AddSingleton<IPresentationStore, PresentationStore>(
            _ => new PresentationStore(
                new DataStore(
                    Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        "Russkyc",
                        "ShowcasePdf", 
                        "data.json"))));

        serviceCollection.AddSingleton<IPdfManager, PdfManager>(
            _ => new PdfManager(
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Russkyc",
                    "ShowcasePdf",
                    "Presentations")));

        serviceCollection.AddSingleton<IDisplayManager, DisplayManager>();

        serviceCollection.AddSingleton<IWindowFactory, WindowFactory>();

        return serviceCollection;
    }
}