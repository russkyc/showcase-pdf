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
using System.IO;
using System.Security.Cryptography;

namespace Showcase.Utilities.Extensions;

public static class FileExtensions
{
    public static string Name(this string source)
    {
        var file = new FileInfo(source);
        return file.Name.Replace(".pdf", string.Empty);
    }
    public static string Md5(this string source)
    {
        using var md5 = MD5.Create();
        using var stream = File.OpenRead(source);
        var hash = md5.ComputeHash(stream);
        
        return BitConverter
            .ToString(hash)
            .Replace("-", String.Empty)
            .ToLowerInvariant();
    }

    public static void CreateDirectoryIfNotExists(this string path)
    {
        try
        {
            var directory = new DirectoryInfo(path);
            
            if (directory.Exists) return;

            directory.Create();
        }
        catch (Exception _)
        {
            Directory.CreateDirectory(path);
        }
    }
}