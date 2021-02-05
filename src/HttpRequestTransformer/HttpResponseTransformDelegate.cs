using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Jaahas.Http {

    /// <summary>
    /// Delegate that can be used to inspect or modify an <see cref="HttpResponseMessage"/>.
    /// </summary>
    /// <param name="response">
    ///   The request message.
    /// </param>
    /// <param name="cancellationToken">
    ///   The cancellation token for the operation.
    /// </param>
    /// <returns>
    ///   A task that will inspect or update the <paramref name="response"/> as required.
    /// </returns>
    public delegate Task HttpResponseTransformDelegate(HttpResponseMessage response, CancellationToken cancellationToken);

}
