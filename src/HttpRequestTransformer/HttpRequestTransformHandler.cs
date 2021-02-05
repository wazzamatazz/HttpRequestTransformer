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
    [Obsolete("Use " + nameof(HttpRequestMiddlewareHandler) + " instead.", false)]
    public class HttpRequestTransformHandler : HttpRequestMiddlewareHandler {

        /// <summary>
        /// Creates a new <see cref="HttpRequestTransformHandler"/> object.
        /// </summary>
        /// <param name="beforeSend">
        ///   The callback to invoke prior to sending a request.
        /// </param>
        /// <param name="responseReceived">
        ///   The callback to invoke after receiving a response.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="beforeSend"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="responseReceived"/> is <see langword="null"/>.
        /// </exception>
        public HttpRequestTransformHandler(
            HttpRequestTransformDelegate beforeSend,
            HttpResponseTransformDelegate responseReceived
        ) : base(CreateDelegate(beforeSend, responseReceived)) {
            if (beforeSend == null) {
                throw new ArgumentNullException(nameof(beforeSend));
            }
            if (responseReceived == null) {
                throw new ArgumentNullException(nameof(responseReceived));
            }
        }


        /// <summary>
        /// Creates a new <see cref="HttpRequestTransformHandler"/> object.
        /// </summary>
        /// <param name="beforeSend">
        ///   The callback to invoke prior to sending a request.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="beforeSend"/> is <see langword="null"/>.
        /// </exception>
        public HttpRequestTransformHandler(HttpRequestTransformDelegate beforeSend) : base(CreateDelegate(beforeSend, null)) {
            if (beforeSend == null) {
                throw new ArgumentNullException(nameof(beforeSend));
            }
        }


        /// <summary>
        /// Creates a new <see cref="HttpRequestTransformHandler"/> object.
        /// </summary>
        /// <param name="responseReceived">
        ///   The callback to invoke after receiving a response.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="responseReceived"/> is <see langword="null"/>.
        /// </exception>
        public HttpRequestTransformHandler(HttpResponseTransformDelegate responseReceived) : base(CreateDelegate(null, responseReceived)) {
            if (responseReceived == null) {
                throw new ArgumentNullException(nameof(responseReceived));
            }
        }


        private static HttpRequestMiddlewareDelegate CreateDelegate(HttpRequestTransformDelegate? beforeSend, HttpResponseTransformDelegate? responseReceived) {
            return (request, next, cancellationToken) => SendAsync(request, beforeSend, responseReceived, next, cancellationToken);
        }


        private static async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpRequestTransformDelegate? beforeSend, HttpResponseTransformDelegate? responseReceived, HttpHandlerDelegate next, CancellationToken cancellationToken) {
            if (beforeSend != null) {
                await beforeSend.Invoke(request, cancellationToken).ConfigureAwait(false);
            }
            
            var response = await next(request, cancellationToken).ConfigureAwait(false);

            if (responseReceived != null) {
                await responseReceived.Invoke(response, cancellationToken).ConfigureAwait(false);
            }

            return response;
        }

    }
}
