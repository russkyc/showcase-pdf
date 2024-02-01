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

using Avalonia.Animation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Showcase.Models.Entities;
using Showcase.Models.Messages;

namespace Showcase.ViewModels;

public partial class ScreenViewModel : ObservableObject
{

    [ObservableProperty] private bool _live;
    [ObservableProperty] private ShowcaseSlide? _activeSlide;
    [ObservableProperty] private IPageTransition? _transition;

    public ScreenViewModel()
    {
        WeakReferenceMessenger
            .Default
            .Register<LiveChangedMessage>(this, OnLiveModeChanged);
        
        WeakReferenceMessenger
            .Default
            .Register<SlideChangedMessage>(this, OnSlideChanged);

        WeakReferenceMessenger
            .Default
            .Register<TransitionChangedMessage>(this, OnSlideTransitionChanged);
        
        WeakReferenceMessenger
            .Default
            .Register<PresentationOpenedMessage>(this, OnPresentationOpened);
    }

    private void OnPresentationOpened(object recipient, PresentationOpenedMessage message)
    {
        ActiveSlide = null;
    }

    private void OnLiveModeChanged(object recipient, LiveChangedMessage message)
    {
        Live = message.Value;
    }

    private void OnSlideTransitionChanged(object recipient, TransitionChangedMessage message)
    {
        if (!Live) return;
        Transition = message.Value;
    }

    private void OnSlideChanged(object recipient, SlideChangedMessage message)
    {
        if (!Live) return;
        ActiveSlide = message.Value;
    }
}