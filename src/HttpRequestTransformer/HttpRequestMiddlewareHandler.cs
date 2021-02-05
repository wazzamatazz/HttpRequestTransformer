using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Jaahas.Http {

    /// <summary>
    /// <see cref="DelegatingHandler"/> that allows HTTP requests and responses to be modified via 
    /// a delegate.
    /// </summary>
    /// <remarks>
    ///   Use this class to create <see cref="DelegatingHandler"/> instances for an HTTP request 
    ///   pipeline without having to extend <see cref="DelegatingHandler"/> yourself.
    /// </remarks>
    public class HttpRequestMiddlewareHandler : DelegatingHandler {

        /// <summary>
        /// The middleware delegate.
        /// </summary>
        private readonly HttpRequestMiddlewareDelegate _handler;


        /// <summary>
        /// Creates a new <see cref="HttpRequestMiddlewareHandler"/> object.
        /// </summary>
        /// <param name="handler">
        ///   The request handler delegate.
        /// </param>
        public HttpRequestMiddlewareHandler(HttpRequestMiddlewareDelegate handler) {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }


        /// <inheritdoc/>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            // Invoke the middleware and tell it to send to the next handler in the pipeline.
            return _handler.Invoke(request, base.SendAsync, cancellationToken);
        }

    }
}
