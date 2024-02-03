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
using System.IO;
using System.Threading.Tasks;
using PDFtoImage;
using Showcase.Models.Entities;
using Showcase.Services.PdfReader.Interfaces;
using Showcase.Utilities.Extensions;
using SkiaSharp;

namespace Showcase.Services.PdfReader;

public class PdfManager : IPdfManager
{
    public async Task<ShowcasePresentation> GetPresentation(string source)
    {
        var path = source;
        var name = source.Name();
        var md5 = source.Md5();
        var presentation = new ShowcasePresentation();

        await Task.Run(async () =>
        {
            presentation.Name = name;
            presentation.Path = path;
            presentation.Md5 = md5;
            presentation.DataFolder = @$"{Environment.CurrentDirectory}\Presentations\{name}\";
            
            var bytes = await File.ReadAllBytesAsync(source);
            
            var slides = await GetSlidesAsync(presentation,bytes);
            presentation.PreviewSlide = slides[0];
            presentation.Slides = slides;
        });
        
        presentation.Created = DateTime.Now;
        presentation.LastOpened = DateTime.Now;

        return presentation;
    }


    public async Task<List<ShowcaseSlide>> GetSlidesAsync(ShowcasePresentation presentation, byte[] bytes)
    {
        List<ShowcaseSlide> slides = new List<ShowcaseSlide>();
        int page = 0;

        await foreach (var skBitmap in Conversion.ToImagesAsync(bytes, dpi: 200, withAspectRatio: true)
                           .ConfigureAwait(false))
        {
            presentation.DataFolder.CreateDirectoryIfNotExists();
            var filePath = $"{presentation.DataFolder}{Guid.NewGuid()}";

            await using var stream = new FileStream(filePath, FileMode.Create);
            await Task.Run(() =>
                skBitmap.Encode(SKEncodedImageFormat.Webp, 95)
                    .SaveTo(stream));

            slides.Add(new ShowcaseSlide
            {
                Page = page,
                PageSource = filePath
            });

            page++;
        }

        return slides;
    }

    
}