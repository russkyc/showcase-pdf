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
using Avalonia;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Showcase.Models.Entities;
using Showcase.Models.Messages;
using Showcase.Services.Datastore.Interfaces;
using Showcase.Services.PdfReader.Interfaces;
using Showcase.Services.WindowManager.Interfaces;
using Showcase.Utilities;
using Showcase.Utilities.Extensions;

namespace Showcase.ViewModels;

public partial class FilesViewModel : ObservableObject
{
    private readonly IPdfManager _pdfManager;
    private readonly IWindowFactory _windowFactory;
    private readonly IPresentationStore _presentationStore;

    [ObservableProperty] private bool _importing;
    [ObservableProperty] private bool _hasRecent;
    [ObservableProperty] private ObservableCollection<ShowcasePresentation>? _list;

    public FilesViewModel(
        IPdfManager pdfManager,
        IWindowFactory windowFactory,
        IPresentationStore presentationStore)
    {
        _pdfManager = pdfManager;
        _windowFactory = windowFactory;
        _presentationStore = presentationStore;

        List = _presentationStore
            .GetPresentations()
            .ToObservableCollection();

        HasRecent = List.Any();
    }

    [RelayCommand]
    async Task OpenRecent()
    {
        _windowFactory.CreatePresenterWindow();
        
        WeakReferenceMessenger
            .Default
            .Send(new PresentationOpenedMessage(List.First()));
    }

    [RelayCommand]
    async Task OpenFile()
    {
        var files = await Application
            .Current
            .GetStorageProvider()
            .OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Import PDF File",
                AllowMultiple = false, FileTypeFilter = new[]
                {
                    new FilePickerFileType("Pdf")
                    {
                        Patterns = new[] { "*.pdf" }
                    }
                }
            });

        if (files.Count == 0)
        {
            return;
        }

        Importing = true;
        HasRecent = false;
        
        var presentation = await _pdfManager.GetPresentation(files.First().Path.LocalPath);
        await _presentationStore.AddPresentation(presentation);

        List = _presentationStore
            .GetPresentations()
            .ToObservableCollection();

        Importing = false;
        HasRecent = true;
        
        _windowFactory.CreatePresenterWindow();

        await Task.Delay(500);

        WeakReferenceMessenger
            .Default
            .Send(new PresentationOpenedMessage(presentation));
    }

    [RelayCommand]
    async Task OpenPresentation(ShowcasePresentation presentation)
    {
        _windowFactory.CreatePresenterWindow();

        presentation.LastOpened = DateTime.Now;
        await _presentationStore.UpdatePresentation(presentation);
        
        List = _presentationStore
            .GetPresentations()
            .ToObservableCollection();

        WeakReferenceMessenger
            .Default
            .Send(new PresentationOpenedMessage(presentation));
    }
}