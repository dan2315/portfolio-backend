using System.Collections;
using System.Linq.Expressions;

public class Test<T> : IQueryable
{
    public Type ElementType => throw new NotImplementedException();

    public Expression Expression => throw new NotImplementedException();

    public IQueryProvider Provider => throw new NotImplementedException();

    public IEnumerator GetEnumerator()
    {
        throw new NotImplementedException();
    }
}