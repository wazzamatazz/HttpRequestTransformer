using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Jaahas.Http {

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
    public delegate Task<HttpResponseMessage> HttpMessageHandlerDelegate(HttpRequestMessage requestMessage, CancellationToken cancellationToken);

}
