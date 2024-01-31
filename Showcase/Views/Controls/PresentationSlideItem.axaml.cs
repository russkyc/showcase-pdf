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

using Avalonia.Controls;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Messaging;
using Showcase.Models.Entities;
using Showcase.Models.Messages;

namespace Showcase.Views.Controls;

public partial class PresentationSlideItem : UserControl
{
    public PresentationSlideItem()
    {
        InitializeComponent();
        WeakReferenceMessenger.Default.Register<SlideChangedMessage>(this,OnSlideChanged);
    }

    private void OnSlideChanged(object recipient, SlideChangedMessage message)
    {
        if (DataContext is null || message.Value is null)
        {
            SlideButton.IsChecked = false;
            return;
        }
        
        var context = (ShowcaseSlide)DataContext;

        if (message.Value.Page == context.Page)
        {
            Focus();
            SlideButton.IsChecked = true;
        }
    }

    private async void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        var slide = (ShowcaseSlide)DataContext;
        WeakReferenceMessenger.Default.Send(new SlideChangedMessage(slide));
    }
}