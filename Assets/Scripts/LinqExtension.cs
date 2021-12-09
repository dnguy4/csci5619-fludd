using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Author is reustmd from StackOverflow
// Code was taken from this link
// https://stackoverflow.com/questions/438188/split-a-collection-into-n-parts-with-linq
static class LinqExtension
{
    public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> list, int parts)
    {
        return list.Select((item, index) => new { index, item })
                   .GroupBy(x => x.index % parts)
                   .Select(x => x.Select(y => y.item));
    }
}
