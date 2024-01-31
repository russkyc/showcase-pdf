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
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Animation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DebounceThrottle;
using Showcase.Models.Collections;
using Showcase.Models.Entities;
using Showcase.Models.Messages;
using Showcase.Services.Configuration.Interfaces;
using Showcase.Services.Datastore.Interfaces;
using Showcase.Services.WindowManager.Interfaces;
using Showcase.Utilities.Extensions;

namespace Showcase.ViewModels;

public partial class PresenterViewModel : ObservableObject
{
    private readonly IAppConfig _appConfig;
    private readonly IWindowFactory _windowFactory;
    private readonly IPresentationStore _presentationStore;
    private readonly DebounceDispatcher _debounceDispatcher = new(300);

    [ObservableProperty] private ShowcaseSlide? _activeSlide;
    [ObservableProperty] private IPageTransition? _transition;
    [ObservableProperty] private TransitionDuration? _duration;
    [ObservableProperty] private ShowcasePresentation? _activePresentation;
    [ObservableProperty] private ObservableCollection<IPageTransition>? _transitions = new();
    [ObservableProperty] private ObservableCollection<TransitionDuration>? _durations = new();

    public PresenterViewModel(
        IAppConfig appConfig,
        IWindowFactory windowFactory,
        IPresentationStore presentationStore)
    {
        _appConfig = appConfig;
        _windowFactory = windowFactory;
        _presentationStore = presentationStore;

        Durations = TransitionDurations
            .GetDurations()
            .ToObservableCollection();

        Duration = Durations
            .First(duration => duration.Name.Equals(_appConfig.Duration));

        Transitions = PageTransitions
            .GetTransitions(Duration.Duration)
            .ToObservableCollection();

        Transition = Transitions
            .First(transition
                => transition.GetType().Name
                    .Replace(
                        "Transition",
                        string.Empty,
                        StringComparison.OrdinalIgnoreCase)
                    .Equals(_appConfig.Transition));

        WeakReferenceMessenger
            .Default
            .Register<PresentationOpenedMessage>(this, OnPresentationOpened);

        WeakReferenceMessenger
            .Default
            .Register<SlideChangedMessage>(this, OnSlideChanged);
        
        WeakReferenceMessenger
            .Default
            .Register<SlideUpdatedMessage>(this, OnSlideUpdated);
    }

    [RelayCommand]
    void OpenSelectionDialog()
    {
        _windowFactory.CreateStartupWindow();
    }

    [RelayCommand]
    async Task Next()
    {
        if (ActiveSlide is null)
        {
            WeakReferenceMessenger
                .Default
                .Send(
                    new SlideChangedMessage(
                        ActivePresentation
                            .Slides
                            .FirstOrDefault()));
            return;
        }

        WeakReferenceMessenger
            .Default
            .Send(
                new SlideChangedMessage(
                    ActivePresentation
                        .Slides
                        .FirstOrDefault(
                            slide => slide.Page == ActiveSlide.Page + 1)));
    }

    [RelayCommand]
    async Task Previous()
    {
        if (ActiveSlide is null)
        {
            WeakReferenceMessenger
                .Default
                .Send(
                    new SlideChangedMessage(
                        ActivePresentation
                            .Slides
                            .FirstOrDefault()));
            return;
        }

        WeakReferenceMessenger
            .Default
            .Send(
                new SlideChangedMessage(
                    ActivePresentation
                        .Slides
                        .FirstOrDefault(
                            slide => slide.Page == ActiveSlide.Page - 1)));
    }

    [RelayCommand]
    async Task OpenAbout()
    {
        _windowFactory.CreateAboutWindow();
    }

    private async void OnSlideUpdated(object recipient, SlideUpdatedMessage message)
    {
        await _debounceDispatcher.DebounceAsync(() => _presentationStore.UpdatePresentation(ActivePresentation));
    }

    partial void OnTransitionChanged(IPageTransition? selectedTransition)
    {
        if (selectedTransition is null || Duration is null)
        {
            return;
        }

        dynamic transition = selectedTransition;
        transition.Duration = Duration.Duration;

        _appConfig.Transition = ((object)transition)
            .GetType()
            .Name
            .Replace(
                "Transition",
                string.Empty,
                StringComparison.OrdinalIgnoreCase);

        _appConfig.Transition = ((object)transition)
            .GetType()
            .Name
            .Replace(
                "Transition",
                string.Empty,
                StringComparison.OrdinalIgnoreCase);

        WeakReferenceMessenger
            .Default
            .Send(
                new TransitionChangedMessage(transition));
    }

    partial void OnDurationChanged(TransitionDuration? duration)
    {
        if (Transition is null || duration is null)
        {
            return;
        }

        dynamic transition = Transition;
        transition.Duration = duration.Duration;
        _appConfig.Duration = duration.Name;

        WeakReferenceMessenger
            .Default
            .Send(
                new TransitionChangedMessage(transition));
    }

    private void OnSlideChanged(object recipient, SlideChangedMessage message)
    {
        ActiveSlide = message.Value;
    }

    private void OnPresentationOpened(object recipient, PresentationOpenedMessage message)
    {
        ActiveSlide = null;
        ActivePresentation = message.Value;
        ActivePresentation.LastOpened = DateTime.Now;
        
        _presentationStore.UpdatePresentation(ActivePresentation);
    }
}