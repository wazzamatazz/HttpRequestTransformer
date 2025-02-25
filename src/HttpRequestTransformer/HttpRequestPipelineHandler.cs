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
    public sealed class HttpRequestPipelineHandler : DelegatingHandler {

        /// <summary>
        /// The middleware delegate.
        /// </summary>
        private readonly HttpRequestPipelineDelegate _handler;


        /// <summary>
        /// Creates a new <see cref="HttpRequestPipelineHandler"/> object using the specified 
        /// <see cref="HttpRequestPipelineDelegate"/> callback.
        /// </summary>
        /// <param name="handler">
        ///   The callback that will be invoked when an HTTP message is being sent.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="handler"/> is <see langword="null"/>.
        /// </exception>
        public HttpRequestPipelineHandler(HttpRequestPipelineDelegate handler) {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }


        /// <summary>
        /// Creates a new <see cref="HttpRequestPipelineHandler"/> object that invokes a 
        /// <see cref="BeforeSendDelegate"/> before every HTTP request is sent.
        /// </summary>
        /// <param name="beforeSend">
        ///   The delegate to invoke before sending an HTTP request.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="beforeSend"/> is <see langword="null"/>.
        /// </exception>
        [Obsolete("Use the constructor that accepts a HttpRequestPipelineDelegate instead. This constructor will be removed in a future version.")]
        public HttpRequestPipelineHandler(BeforeSendDelegate beforeSend) : this(async (req, next, ct) => {
            await beforeSend.Invoke(req, ct).ConfigureAwait(false);
            return await next(req, ct).ConfigureAwait(false);
        }) { 
            if (beforeSend == null) {
                throw new ArgumentNullException(nameof(beforeSend));
            }
        }


        /// <summary>
        /// Creates a new <see cref="HttpRequestPipelineHandler"/> object that invokes a 
        /// <see cref="ResponseReceivedDelegate"/> after every HTTP response is received.
        /// </summary>
        /// <param name="responseReceived">
        ///   The delegate to invoke when an HTTP response has been received.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="responseReceived"/> is <see langword="null"/>.
        /// </exception>
        [Obsolete("Use the constructor that accepts a HttpRequestPipelineDelegate instead. This constructor will be removed in a future version.")]
        public HttpRequestPipelineHandler(ResponseReceivedDelegate responseReceived) : this(async (req, next, ct) => {
            var response = await next(req, ct).ConfigureAwait(false);
            await responseReceived.Invoke(response, ct).ConfigureAwait(false);
            return response;
        }) {
            if (responseReceived == null) {
                throw new ArgumentNullException(nameof(responseReceived));
            }
        }


        /// <summary>
        /// Creates a new <see cref="HttpRequestPipelineHandler"/> object that invokes a 
        /// <see cref="BeforeSendDelegate"/> before event HTTP request is send, and a 
        /// <see cref="ResponseReceivedDelegate"/> after every HTTP response is received.
        /// </summary>
        /// <param name="beforeSend">
        ///   The delegate to invoke before sending an HTTP request.
        /// </param>
        /// <param name="responseReceived">
        ///   The delegate to invoke when an HTTP response has been received.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="beforeSend"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="responseReceived"/> is <see langword="null"/>.
        /// </exception>
        [Obsolete("Use the constructor that accepts a HttpRequestPipelineDelegate instead. This constructor will be removed in a future version.")]
        public HttpRequestPipelineHandler(
            BeforeSendDelegate beforeSend,
            ResponseReceivedDelegate responseReceived
        ) : this(async (req, next, ct) => {
            await beforeSend.Invoke(req, ct).ConfigureAwait(false);
            var response = await next(req, ct).ConfigureAwait(false);
            await responseReceived.Invoke(response, ct).ConfigureAwait(false);
            return response;
        }) {
            if (beforeSend == null) {
                throw new ArgumentNullException(nameof(beforeSend));
            }
            if (responseReceived == null) {
                throw new ArgumentNullException(nameof(responseReceived));
            }
        }


        /// <inheritdoc/>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            // Invoke the middleware and tell it to send to the next handler in the pipeline.
            return _handler.Invoke(request, base.SendAsync, cancellationToken);
        }

    }
}
