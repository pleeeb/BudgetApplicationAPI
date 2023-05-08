using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace BudgetApplicationAPITests
{
    internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        internal TestAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new TestAsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestAsyncEnumerable<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            var resultType = typeof(TResult).GetGenericArguments()[0];
            var queryable = new TestAsyncQueryable<object>(Enumerable.Empty<object>().AsQueryable());
            var resultQueryable = queryable.Provider.CreateQuery<object>(expression);
            var resultAsyncEnumerable = resultQueryable.AsAsyncEnumerable();
            var result = resultAsyncEnumerable
                .Select(e => TypeMapping(resultType, e))
                .ToListAsync(cancellationToken)
                .GetAwaiter()
                .GetResult();

            return (TResult)typeof(Enumerable)
                .GetMethod(nameof(Enumerable.ToList))
                .MakeGenericMethod(resultType)
                .Invoke(null, new[] { result });
        }

        /*public async ValueTask<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            var resultType = typeof(TResult).GetGenericArguments()[0];
            var asyncEnumerable = (IAsyncEnumerable<object>)_inner.CreateQuery(expression)
                .AsAsyncEnumerable();
            var result = await asyncEnumerable
                .Select(e => TypeMapping(resultType, e))
                .ToListAsync(cancellationToken);

            return (TResult)typeof(Enumerable)
                .GetMethod(nameof(Enumerable.ToList))
                .MakeGenericMethod(resultType)
                .Invoke(null, new[] { result });
        }*/

        private static object TypeMapping(Type targetType, object value)
        {
            if (value == null)
            {
                return null;
            }

            var sourceType = value.GetType();
            var convertedValue = Convert.ChangeType(value, targetType);
            return convertedValue;
        }
    }

    internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        { }

        public TestAsyncEnumerable(Expression expression)
            : base(expression)
        { }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
    }

    internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public void Dispose()
        {
            _inner.Dispose();
        }

        public async ValueTask<bool> MoveNextAsync()
        {
            return await Task.FromResult(_inner.MoveNext());
        }

        public T Current => _inner.Current;

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return ValueTask.CompletedTask;
        }
    }

    public class TestAsyncQueryable<T> : IQueryable<T>
    {
        private readonly IQueryable<T> _data;

        public TestAsyncQueryable(IQueryable<T> data)
        {
            _data = data;
            Provider = new TestAsyncQueryProvider<T>(_data.Provider);
            Expression = _data.Expression;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        public Type ElementType => _data.ElementType;

        public Expression Expression { get; }

        public IQueryProvider Provider { get; }

        public async ValueTask DisposeAsync()
        {
            if (_data is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync();
            }
        }
    }
}
