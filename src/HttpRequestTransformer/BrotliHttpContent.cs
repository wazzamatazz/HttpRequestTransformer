#if NETSTANDARD2_1_OR_GREATER

using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;

namespace Jaahas.Http {

    /// <summary>
    /// <see cref="CompressedHttpContent"/> implementation that uses <see cref="BrotliStream"/>.
    /// </summary>
    public class BrotliHttpContent : CompressedHttpContent {

        /// <inheritdoc/>
        public override string ContentEncoding => "br";


        /// <summary>
        /// Creates a new <see cref="BrotliHttpContent"/> object.
        /// </summary>
        /// <param name="inner">
        ///   The inner HTTP content to compress.
        /// </param>
        public BrotliHttpContent(HttpContent inner) : base(inner) { }


        /// <inheritdoc/>
        protected override Stream CreateCompressionStream(Stream stream, TransportContext context)
            => new BrotliStream(stream, CompressionMode.Compress, true);

    }
}

#endif
