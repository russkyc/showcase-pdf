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
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

namespace Showcase.Utilities;

public static class AppUtils
{
    public static IStorageProvider GetStorageProvider(this Application application)
    {
        if (application.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopStyleApplicationLifetime)
        {
            return desktopStyleApplicationLifetime
                .Windows
                .FirstOrDefault(window => window.IsActive)
                .StorageProvider;
        }

        throw new PlatformNotSupportedException();
    }
    
    public static Window GetActiveWindow(this Application application)
    {
        if (application.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopStyleApplicationLifetime)
        {
            return desktopStyleApplicationLifetime
                .Windows
                .First(window => window.IsActive);
        }

        throw new PlatformNotSupportedException();
    }
    
    public static Window GetMainWindow(this Application application)
    {
        if (application.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopStyleApplicationLifetime)
        {
            return desktopStyleApplicationLifetime
                .MainWindow!;
        }

        throw new PlatformNotSupportedException();
    }
    
    public static bool HasWindow<T>(this Application application) where T : Window
    {
        if (application.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopStyleApplicationLifetime)
        {
            return desktopStyleApplicationLifetime
                .Windows.OfType<T>()
                .Any();
        }

        throw new PlatformNotSupportedException();
    }
    
    public static void SetMainWindow(this Application application, Window window)
    {
        if (application.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopStyleApplicationLifetime)
        {
            desktopStyleApplicationLifetime.MainWindow = window;
        }

        throw new PlatformNotSupportedException();
    }
    
    public static Window GetActiveWindow<T>(this Application application) where T : Window
    {
        if (application.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopStyleApplicationLifetime)
        {
            return desktopStyleApplicationLifetime
                .Windows.OfType<T>()
                .First();
        }

        throw new PlatformNotSupportedException();
    }

    public static IEnumerable<Window> GetWindows(this Application application)
    {
        if (application.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopStyleApplicationLifetime)
        {
            return desktopStyleApplicationLifetime.Windows;
        }

        throw new PlatformNotSupportedException();
    }
}