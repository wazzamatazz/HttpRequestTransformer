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
    public delegate Task<HttpResponseMessage> HttpRequestPipelineDelegate(
        HttpRequestMessage request,
        HttpMessageHandlerDelegate next, 
        CancellationToken cancellationToken
    );

}
