﻿// The MIT License (MIT)
//
// Copyright (c) 2014 Bernhard Johannessen
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

namespace mpbOrm
{
    using System.Collections.Generic;

    /// <summary>
    /// A list for working with paged sets. It contains extra properties with
    /// information about the paged set.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list</typeparam>
    public class PagedResult<T> : List<T>
    {
        /// <summary>
        /// The total number of elements in the set
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// The page of the set contained in this list
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// The size of a page
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// The total number of pages in the set.
        /// 
        /// The page count is less than zero if it is impossible to determine
        /// the number of pages
        /// </summary>
        public int PageCount
        {
            get
            {
                if (this.PageSize > 1)
                    return ((this.Total - 1) / this.PageSize) + 1;
                else if (this.PageSize == 1)
                    return this.Total;
                else return -1;
            }
        }
    }
}
