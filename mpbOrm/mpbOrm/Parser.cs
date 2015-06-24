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

namespace mpbOrm
{
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The parser is used to turn filters and order bys into correct SQL
    /// </summary>
    public class Parser<TEntity> : IParser<TEntity> 
    {
        public EntityMap<TEntity> Map { get; private set; }

        public Parser(EntityMap<TEntity> map)
        {
            this.Map = map;
        }

        public string Parse(string str)
        {
            return Regex.Replace(str, @"\{.+?\}", new MatchEvaluator((match) =>
            {
                var propertyInfo = typeof(TEntity).GetProperties()
                    .SingleOrDefault(x => x.Name == match.Value.Substring(1, match.Value.Length - 2));
                if (propertyInfo != null)
                    return this.Map.ColumnName(propertyInfo);
                else
                    return match.Value;
            }), RegexOptions.CultureInvariant | RegexOptions.Multiline);
        }
    }
}
