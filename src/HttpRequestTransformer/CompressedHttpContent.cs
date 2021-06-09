using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Jaahas.Http {

    /// <summary>
    /// Base class for an <see cref="HttpContent"/> implementation that compresses HTTP content.
    /// </summary>
    /// <remarks>
    ///   When overriding the <see cref="CreateCompressionStream"/> method, implementers must 
    ///   return a stream that keeps the provided inner stream open when the outer stream is 
    ///   disposed.
    /// </remarks>
    public abstract class CompressedHttpContent : HttpContent {

        /// <summary>
        /// The inner <see cref="HttpContent"/> to compress.
        /// </summary>
        private readonly HttpContent _inner;

        /// <summary>
        /// The value to set on the <see cref="System.Net.Http.Headers.HttpContentHeaders.ContentEncoding"/> 
        /// header.
        /// </summary>
        public abstract string ContentEncoding { get; }


        /// <summary>
        /// Creates a new <see cref="CompressedHttpContent"/> object.
        /// </summary>
        /// <param name="inner">
        ///   The <see cref="HttpContent"/> to compress.
        /// </param>
        protected CompressedHttpContent(HttpContent inner) {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));

            foreach (var header in _inner.Headers) {
                Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            Headers.ContentEncoding.Add(ContentEncoding);
        }


        /// <inheritdoc/>
        protected override bool TryComputeLength(out long length) {
            length = -1;
            return false;
        }


        /// <summary>
        /// Creates a compression stream that wraps the specified inner <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">
        ///   The inner stream to write the compressed content to.
        /// </param>
        /// <param name="context">
        ///   Information about the transport (channel binding token, for example). This parameter 
        ///   may be <see langword="null"/>.
        /// </param>
        /// <returns>
        ///   A <see cref="Stream"/> to write the HTTP content to.
        /// </returns>
        /// <remarks>
        ///   The resulting <see cref="Stream"/> will be disposed at the end of the 
        ///   <see cref="HttpContent.SerializeToStreamAsync(Stream, TransportContext)"/> method. 
        ///   You must create the <see cref="Stream"/> in such a way that it does not dispose of 
        ///   the <paramref name="stream"/> parameter when it is disposed.
        /// </remarks>
        protected abstract Stream CreateCompressionStream(Stream stream, TransportContext context);


        /// <inheritdoc/>
        protected sealed override async Task SerializeToStreamAsync(Stream stream, TransportContext context) {
            using (var compressionStream = CreateCompressionStream(stream, context)) {
                await _inner.CopyToAsync(compressionStream, context).ConfigureAwait(false);
                await compressionStream.FlushAsync().ConfigureAwait(false);
            }
        }

    }
}
