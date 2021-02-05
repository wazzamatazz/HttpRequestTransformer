using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Jaahas.Http {

    /// <summary>
    /// Delegate that can inspect or modify an HTTP request or response.
    /// </summary>
    /// <param name="request">
    ///   The HTTP request.
    /// </param>
    /// <param name="next">
    ///   A delegate that will invoke the next handler in the pipeline.
    /// </param>
    /// <param name="cancellationToken">
    ///   The cancellation token for the operation.
    /// </param>
    /// <returns>
    ///   A <see cref="Task{TResult}"/> that will return the <see cref="HttpResponseMessage"/>.
    /// </returns>
    /// <remarks>
    ///   Under normal circumstances, you should invoke the <paramref name="next"/> delegate to 
    ///   pass the request onto the next handler in the HTTP request pipeline. Alternatively, you 
    ///   can construct the <see cref="HttpResponseMessage"/> result yourself, for use in unit 
    ///   tests etc.
    /// </remarks>
    public delegate Task<HttpResponseMessage> HttpRequestMiddlewareDelegate(
        HttpRequestMessage request,
        HttpHandlerDelegate next, 
        CancellationToken cancellationToken
    );


    /// <summary>
    /// A delegate that sends an <see cref="HttpRequestMessage"/> and returns the resulting 
    /// <see cref="HttpResponseMessage"/>.
    /// </summary>
    /// <param name="requestMessage">
    ///   The HTTP requst message.
    /// </param>
    /// <param name="cancellationToken">
    ///   The cancellation token for the operation.
    /// </param>
    /// <returns>
    ///   A <see cref="Task{TResult}"/> that will return the HTTP response message.
    /// </returns>
    public delegate Task<HttpResponseMessage> HttpHandlerDelegate(HttpRequestMessage requestMessage, CancellationToken cancellationToken);

}
