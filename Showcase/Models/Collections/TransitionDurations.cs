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
using Showcase.Models.Entities;

namespace Showcase.Models.Collections;

public static class TransitionDurations
{
    public static IEnumerable<TransitionDuration> GetDurations()
    {
        yield return new TransitionDuration
        {
            Name = "0.25s",
            Duration = TimeSpan.FromMilliseconds(250)
        };
        yield return new TransitionDuration
        {
            Name = "0.5s",
            Duration = TimeSpan.FromMilliseconds(500)
        };
        yield return new TransitionDuration
        {
            Name = "1.0s",
            Duration = TimeSpan.FromMilliseconds(1000)
        };
        yield return new TransitionDuration
        {
            Name = "1.5s",
            Duration = TimeSpan.FromMilliseconds(1500)
        };
        yield return new TransitionDuration
        {
            Name = "2.0s",
            Duration = TimeSpan.FromMilliseconds(2000)
        };
        yield return new TransitionDuration
        {
            Name = "2.5s",
            Duration = TimeSpan.FromMilliseconds(2500)
        };
        yield return new TransitionDuration
        {
            Name = "3.0s",
            Duration = TimeSpan.FromMilliseconds(3000)
        };
        yield return new TransitionDuration
        {
            Name = "3.5s",
            Duration = TimeSpan.FromMilliseconds(3500)
        };
        yield return new TransitionDuration
        {
            Name = "4.0s",
            Duration = TimeSpan.FromMilliseconds(4000)
        };
        yield return new TransitionDuration
        {
            Name = "4.5s",
            Duration = TimeSpan.FromMilliseconds(4500)
        };
        yield return new TransitionDuration
        {
            Name = "5.0s",
            Duration = TimeSpan.FromMilliseconds(5000)
        };
    }
}