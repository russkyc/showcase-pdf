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
using System.Linq;
using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Showcase.Services.WindowManager.Interfaces;
using Showcase.Utilities;
using Showcase.Utilities.Extensions;
using Showcase.ViewModels;
using Showcase.Views;

namespace Showcase.Services.WindowManager;

public class WindowFactory : IWindowFactory
{
    private readonly IServiceProvider _serviceProvider;

    public WindowFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void CreatePresenterWindow()
    {
        if (Application
            .Current
            .HasWindow<PresenterView>())
        {
            Application
                .Current
                .GetActiveWindow<FilesView>()
                .Close();

            return;
        }
        
        var window = new PresenterView();
        window.DataContext = _serviceProvider.GetService<PresenterViewModel>();
        window.Show();
        
        CreateScreenWindow(1);
        
        Application
            .Current
            .GetActiveWindow<FilesView>()
            .Close();

    }

    public void CreateScreenWindow(int display)
    {
        if (!Application
            .Current
            .HasWindow<PresenterView>())
        {
            return;
        }
        
        var presenter = Application
            .Current
            .GetActiveWindow<PresenterView>();

        if (presenter.Screens.ScreenCount == 1) return;
        var screen = presenter.Screens.All[display];

        if (Application
            .Current
            .GetWindows()
            .OfType<ScreenView>()
            .Any(
                window => window
                    .Screens
                    .ScreenFromWindow(window)
                    .HasSameBounds(screen)))
        {
            return;
        }

        var viewModel = _serviceProvider.GetService<PresenterViewModel>();

        var window = new ScreenView();
        presenter.Closing += (sender, args) => window.Close();

        window.DataContext = viewModel;
        window.IsHitTestVisible = false;
        window.Width = screen.WorkingArea.Width;
        window.Height = screen.WorkingArea.Height;
        window.Position = new PixelPoint(screen.WorkingArea.X, screen.WorkingArea.Y);

        window.Show(presenter);
        presenter.Activate();
    }

    public void CreateStartupWindow()
    {
        var window = new FilesView();
        window.DataContext = _serviceProvider.GetService<FilesViewModel>();
        
        if (Application
            .Current
            .HasWindow<PresenterView>())
        {
            window.ShowDialog(Application.Current.GetActiveWindow<PresenterView>());
            return;
        }
        
        window.Show();
    }

    public void CreateAboutWindow()
    {
        new AboutView().ShowDialog(Application.Current.GetActiveWindow());
    }
}