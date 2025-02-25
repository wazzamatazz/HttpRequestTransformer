using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;

namespace Jaahas.Http {

    /// <summary>
    /// <see cref="CompressedHttpContent"/> implementation that uses <see cref="GZipStream"/>.
    /// </summary>
    public sealed class GZipHttpContent : CompressedHttpContent {

        /// <inheritdoc/>
        public override string ContentEncoding => "gzip";


        /// <summary>
        /// Creates a new <see cref="GZipHttpContent"/> object.
        /// </summary>
        /// <param name="inner">
        ///   The inner HTTP content to compress.
        /// </param>
        public GZipHttpContent(HttpContent inner) : base(inner) { }


        /// <inheritdoc/>
        protected override Stream CreateCompressionStream(Stream stream, TransportContext context)
            => new GZipStream(stream, CompressionMode.Compress, true);

    }
}
