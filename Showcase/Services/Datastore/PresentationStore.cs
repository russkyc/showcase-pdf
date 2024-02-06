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
using System.Threading.Tasks;
using JsonFlatFileDataStore;
using Showcase.Models.Entities;
using Showcase.Services.Datastore.Interfaces;

namespace Showcase.Services.Datastore;

public class PresentationStore : IPresentationStore
{
    private IDataStore _dataStore;

    public PresentationStore(IDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public IEnumerable<ShowcasePresentation> GetPresentations()
    {
        return _dataStore.GetCollection<ShowcasePresentation>().AsQueryable()
            .OrderByDescending(presentation => presentation.LastOpened)
            .ThenByDescending(presentation => presentation.Created);
    }

    public async Task<bool> AddPresentation(ShowcasePresentation presentation)
    {
        if (_dataStore.GetCollection<ShowcasePresentation>().AsQueryable()
            .Any(entry => entry.Name.Equals(presentation.Name)))
        {
            return await _dataStore.GetCollection<ShowcasePresentation>()
                .UpdateOneAsync(entry => entry.Id == presentation.Id,
                    presentation);
        }

        return await _dataStore.GetCollection<ShowcasePresentation>().InsertOneAsync(presentation);
    }

    public async Task<bool> UpdatePresentation(ShowcasePresentation presentation)
    {
        return await _dataStore.GetCollection<ShowcasePresentation>()
            .UpdateOneAsync(entry => entry.Id == presentation.Id, presentation);
    }

    public async Task<bool> AddPresentations(IEnumerable<ShowcasePresentation> presentations)
    {
        return await _dataStore.GetCollection<ShowcasePresentation>().InsertManyAsync(presentations);
    }

    public async Task<bool> DeletePresentation(ShowcasePresentation presentation)
    {
        return await _dataStore.GetCollection<ShowcasePresentation>()
            .DeleteOneAsync(presentation.Id);
    }
}