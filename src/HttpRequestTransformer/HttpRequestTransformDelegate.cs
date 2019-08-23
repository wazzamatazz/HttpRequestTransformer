using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Jaahas.Http {

    /// <summary>
    /// Delegate that can be used to modify an <see cref="HttpRequestMessage"/> prior to sending.
    /// </summary>
    /// <param name="request">
    ///   The request message.
    /// </param>
    /// <param name="cancellationToken">
    ///   The cancellation token for the operation.
    /// </param>
    /// <returns>
    ///   A task that will update the <paramref name="request"/> as required.
    /// </returns>
    public delegate Task HttpRequestTransformDelegate(HttpRequestMessage request, CancellationToken cancellationToken);

}
