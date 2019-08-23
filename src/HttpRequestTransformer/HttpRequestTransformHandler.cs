using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Jaahas.Http {

    /// <summary>
    /// <see cref="HttpMessageHandler"/> that is used to transform an HTTP request prior to 
    /// sending via a callback. This can be used to e.g. set an <c>Authorize</c> header on 
    /// outgoing requests according to some state value stored in the 
    /// <see cref="HttpRequestMessage.Properties"/> dictionary.
    /// </summary>
    public class HttpRequestTransformHandler : DelegatingHandler {

        /// <summary>
        /// The callback to invoke prior to sending the request.
        /// </summary>
        private readonly HttpRequestTransformDelegate _callback;


        /// <summary>
        /// Creates a new <see cref="HttpRequestTransformHandler"/> object.
        /// </summary>
        /// <param name="callback">
        ///   The callback to invoke prior to every request.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="callback"/> is <see langword="null"/>.
        /// </exception>
        public HttpRequestTransformHandler(HttpRequestTransformDelegate callback) {
            _callback = callback ?? throw new ArgumentNullException(nameof(callback));
        }


        /// <summary>
        /// Passes an outgoing request to the registered callback and then passes the request on 
        /// to an inner handler.
        /// </summary>
        /// <param name="request">
        ///   The outgoing request.
        /// </param>
        /// <param name="cancellationToken">
        ///   The cancellation token for the operation.
        /// </param>
        /// <returns>
        ///   The response message.
        /// </returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            await _callback.Invoke(request, cancellationToken).ConfigureAwait(false);
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

    }
}
