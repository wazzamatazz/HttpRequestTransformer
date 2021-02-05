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
        /// The callback to invoke prior to sending a request.
        /// </summary>
        private readonly HttpRequestTransformDelegate? _requestCallback;

        /// <summary>
        /// The callback to invoke after receiving a request.
        /// </summary>
        private readonly HttpResponseTransformDelegate? _responseCallback;


        /// <summary>
        /// Creates a new <see cref="HttpRequestTransformHandler"/> object.
        /// </summary>
        /// <param name="requestCallback">
        ///   The callback to invoke prior to sending a request.
        /// </param>
        /// <param name="responseCallback">
        ///   The callback to invoke after receiving a response.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="requestCallback"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="responseCallback"/> is <see langword="null"/>.
        /// </exception>
        public HttpRequestTransformHandler(
            HttpRequestTransformDelegate requestCallback,
            HttpResponseTransformDelegate responseCallback
        ) {
            _requestCallback = requestCallback ?? throw new ArgumentNullException(nameof(requestCallback));
            _responseCallback = responseCallback ?? throw new ArgumentNullException(nameof(responseCallback));
        }


        /// <summary>
        /// Creates a new <see cref="HttpRequestTransformHandler"/> object.
        /// </summary>
        /// <param name="requestCallback">
        ///   The callback to invoke prior to sending a request.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="requestCallback"/> is <see langword="null"/>.
        /// </exception>
        public HttpRequestTransformHandler(HttpRequestTransformDelegate requestCallback) {
            _requestCallback = requestCallback ?? throw new ArgumentNullException(nameof(requestCallback));
        }


        /// <summary>
        /// Creates a new <see cref="HttpRequestTransformHandler"/> object.
        /// </summary>
        /// <param name="requestCallback">
        ///   The callback to invoke prior to sending a request.
        /// </param>
        /// <param name="responseCallback">
        ///   The callback to invoke after receiving a response.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="responseCallback"/> is <see langword="null"/>.
        /// </exception>
        public HttpRequestTransformHandler(HttpResponseTransformDelegate responseCallback) {
            _responseCallback = responseCallback ?? throw new ArgumentNullException(nameof(responseCallback));
        }


        /// <inheritdoc/>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            if (_requestCallback != null) {
                await _requestCallback.Invoke(request, cancellationToken).ConfigureAwait(false);
            }
            
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            if (_responseCallback != null) {
                await _responseCallback.Invoke(response, cancellationToken).ConfigureAwait(false);
            }

            return response;
        }

    }
}
