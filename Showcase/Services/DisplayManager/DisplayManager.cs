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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Showcase.Models.Entities;
using Showcase.Models.Messages;
using Showcase.Services.Configuration.Interfaces;
using Showcase.Services.DisplayManager.Interfaces;
using Showcase.Services.WindowManager.Interfaces;
using Showcase.Utilities.Extensions;
using Showcase.Views;

namespace Showcase.Services.DisplayManager;

public partial class DisplayManager : ObservableObject, IDisplayManager
{
    private readonly IAppConfig _appConfig;
    private readonly IWindowFactory _windowFactory;
    [ObservableProperty] private ObservableCollection<Display>? _displays = new();

    public DisplayManager(
        IWindowFactory windowFactory,
        IAppConfig appConfig)
    {
        _windowFactory = windowFactory;
        _appConfig = appConfig;
        
        WeakReferenceMessenger
            .Default
            .Register<DisplaysChangedMessage>(this, OnDisplaysUpdated);
        
        RefreshDisplays();
    }

    public void RefreshDisplays()
    {
        Displays = GetDisplays()
            .Select(display =>
            {
                var currentDisplay =
                    Displays.FirstOrDefault(currentDisplays => currentDisplays.Index == display.Index);

                if (currentDisplay is not null)
                {
                    display.Enabled = currentDisplay.Enabled;
                }

                if (_appConfig.Displays.Any(savedDisplay => savedDisplay.Index == display.Index))
                {
                    var savedDisplay = _appConfig
                        .Displays
                        .First(
                            savedDisplay => savedDisplay.Index == display.Index);
                    display.Enabled = savedDisplay.Enabled;
                }

                return display;
            })
            .ToObservableCollection();
    }

    private void OnDisplaysUpdated(object recipient, DisplaysChangedMessage message)
    {
        RefreshDisplays();
        OnPropertyChanged(nameof(Displays));
    }

    public void CreateDisplay(Display display)
    {
        _appConfig.Displays = Displays.Where(display => display.Enabled).ToList();
        _windowFactory.CreateScreenWindow(display);
    }

    public void CloseDisplay(Display display)
    {
        Application
            .Current
            .GetWindows()
            .OfType<ScreenView>()
            .First(window =>
            {
                var screen = window
                    .Screens
                    .ScreenFromWindow(window);
                return screen.Bounds.X == display.BoundsX
                       && screen.Bounds.Y == display.BoundsY;
            })
            .Close();
        _appConfig.Displays = Displays.Where(display => display.Enabled).ToList();
    }

    IEnumerable<Display> GetDisplays()
    {
        var count = 0;
        var displays = new List<Display>();

        Application
            .Current
            .GetActiveWindow()
            .Screens
            .All
            .ToList()
            .ForEach(screen =>
            {
                displays.Add(
                    new Display
                    {
                        Index = count,
                        Primary = screen.IsPrimary,
                        Name = $"Display {count + 1}",
                        BoundsY = screen.WorkingArea.Y,
                        BoundsX = screen.WorkingArea.X,
                        Width = screen.WorkingArea.Width,
                        Height = screen.WorkingArea.Height,
                        Scaling = screen.Scaling
                    });
                count++;
            });

        return displays;
    }
}