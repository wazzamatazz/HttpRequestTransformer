#if NETSTANDARD2_1_OR_GREATER

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Jaahas.Http {

    /// <summary>
    /// <see cref="DelegatingHandler"/> that adds Brotli compression to outgoing HTTP request 
    /// content.
    /// </summary>
    public sealed class BrotliCompressor : DelegatingHandler {

        /// <summary>
        /// A callback that will decide if the content of a given <see cref="HttpRequestMessage"/> 
        /// should be compressed.
        /// </summary>
        private readonly Func<HttpRequestMessage, bool>? _callback;


        /// <summary>
        /// Creates a new <see cref="GZipCompressor"/> object.
        /// </summary>
        /// <param name="callback">
        ///   A callback that will decide if the content of a given <see cref="HttpRequestMessage"/> 
        ///   should be compressed. Specify <see langword="null"/> to apply compression to all 
        ///   outgoing requests.
        /// </param>
        public BrotliCompressor(Func<HttpRequestMessage, bool>? callback = null) {
            _callback = callback;
        }


        /// <inheritdoc/>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            if (request.Content != null && (_callback?.Invoke(request) ?? true)) {
                request.Content = new BrotliHttpContent(request.Content);
            }
            return base.SendAsync(request, cancellationToken);
        }

    }
}

#endif
