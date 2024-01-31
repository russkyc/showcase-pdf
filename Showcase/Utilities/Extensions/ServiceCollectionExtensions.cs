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

using Microsoft.Extensions.DependencyInjection;
using Showcase.Services.Configuration;
using Showcase.Services.Configuration.Interfaces;
using Showcase.Services.Datastore;
using Showcase.Services.Datastore.Interfaces;
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
        serviceCollection.AddSingleton(services => new FilesView()
        {
            DataContext = services.GetService<FilesViewModel>()
        });
        serviceCollection.AddSingleton(services => new PresenterView()
        {
            DataContext = services.GetService<PresenterViewModel>()
        });
        serviceCollection.AddSingleton(services => new ScreenView()
        {
            DataContext = services.GetService<PresenterViewModel>()
        });
        
        return serviceCollection;
    }
    
    public static IServiceCollection AddShowcaseViewModels(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<FilesViewModel>();
        serviceCollection.AddSingleton<PresenterViewModel>();
        
        return serviceCollection;
    }

    public static IServiceCollection AddShowcaseServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IPresentationStore, PresentationStore>();
        serviceCollection.AddSingleton<IWindowFactory, WindowFactory>();
        serviceCollection.AddSingleton<IPdfManager, PdfManager>();
        serviceCollection.AddSingleton<IAppConfig, AppConfig>(_ => new AppConfig("config.json"));
        
        return serviceCollection;
    }
}