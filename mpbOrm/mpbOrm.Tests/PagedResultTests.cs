// The MIT License (MIT)
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

namespace mpbOrm.Tests
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class PagedResultTests
    {
        [Test]
        public void PageCountReturnsCorrectNumberOfPages()
        {
            var pagedResult = new PagedResult<object>() { Page = 1, PageSize = 5, Total = 11 };
            Assert.AreEqual(3, pagedResult.PageCount);
            var pagedResult2 = new PagedResult<object>() { Page = 2, PageSize = 5, Total = 10 };
            Assert.AreEqual(2, pagedResult2.PageCount);
            var pagedResult3 = new PagedResult<object>() { Page = 3, PageSize = 1, Total = 23 };
            Assert.AreEqual(23, pagedResult3.PageCount);
            var pagedResult4 = new PagedResult<object>() { Page = 4, PageSize = 0, Total = 523 };
            Assert.AreEqual(-1, pagedResult4.PageCount);
        }
    }
}