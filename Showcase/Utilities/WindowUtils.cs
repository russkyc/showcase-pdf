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
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Showcase.Views;

namespace Showcase.Utilities;

public static class WindowUtils
{
    public static void TryShowWindow<T>(object? dataContext, bool hideCurrent = false) where T : Window
    {
        if (Application
                .Current?
                .ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime
            desktopStyleApplicationLifetime)
        {
            return;
        }

        if (desktopStyleApplicationLifetime
                .Windows
                .OfType<T>()
                .FirstOrDefault() is not Window window)
        {
            var newWindow = Activator.CreateInstance<T>();
            newWindow.DataContext = dataContext;

            if (typeof(T) == typeof(ScreenView))
            {
                if (newWindow.Screens.ScreenCount > 1)
                {
                    var area = newWindow.Screens.All[1].WorkingArea;
                    newWindow.Width = area.Width;
                    newWindow.Height = area.Height;
                    newWindow.Position = new PixelPoint(area.X, area.Y);
                }
            }
            
            newWindow.Show();
            //desktopStyleApplicationLifetime.MainWindow = newWindow;
        
            if (hideCurrent)
            {
                desktopStyleApplicationLifetime.Windows
                    .Where(window => window.GetType() != typeof(T))
                    .ToList()
                    .ForEach(window => window.Close());
            }
            return;
        }
        
        if (window.IsFocused || window.IsActive)
        {
            return;
        }
            
        window.Focus();
        //desktopStyleApplicationLifetime.MainWindow = window;
        
        if (hideCurrent)
        {
            desktopStyleApplicationLifetime.Windows
                .Where(window => window.GetType() != typeof(T))
                .ToList()
                .ForEach(window => window.Close());
        }
    }
}