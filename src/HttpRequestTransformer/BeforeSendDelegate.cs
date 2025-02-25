using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Jaahas.Http {

    /// <summary>
    /// Delegate that can be used to inspect or modify an <see cref="HttpRequestMessage"/>.
    /// </summary>
    /// <param name="request">
    ///   The request message.
    /// </param>
    /// <param name="cancellationToken">
    ///   The cancellation token for the operation.
    /// </param>
    /// <returns>
    ///   A task that will inspect or update the <paramref name="request"/> as required.
    /// </returns>
    [Obsolete("Use HttpRequestPipelineDelegate instead.")]
    public delegate Task BeforeSendDelegate(HttpRequestMessage request, CancellationToken cancellationToken);

}
