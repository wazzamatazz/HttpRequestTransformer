using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Jaahas.Http {

    /// <summary>
    /// Provides factory methods to simplify creation of <see cref="HttpClient"/> objects in 
    /// applications that do not use dependency injection to create clients.
    /// </summary>
    /// <remarks>
    ///   It is recommended that you only use this class in situations where it is not practical or
    ///   desired to use Microsoft.Extensions.Http to manage <see cref="HttpClient"/> creation.
    /// </remarks>
    public static class HttpClientFactory {

        /// <summary>
        /// Creates a new <see cref="HttpClient"/> using the specified message handlers.
        /// </summary>
        /// <param name="additionalHandlers">
        ///   The additional handlers to add to the request pipeline for the client.
        /// </param>
        /// <returns>
        ///   A new <see cref="HttpClient"/> that will dispose the underlying message handler
        ///   pipeline when disposed.
        /// </returns>
        public static HttpClient Create(params DelegatingHandler[] additionalHandlers) {
            return new HttpClient(CreatePipeline(additionalHandlers), true);
        }
        
        
        /// <summary>
        /// Creates a new <see cref="HttpClient"/> using the specified message handlers.
        /// </summary>
        /// <param name="primaryHandler">
        ///   The primary handler for the client.
        /// </param>
        /// <param name="additionalHandlers">
        ///   The additional handlers to add to the request pipeline for the client.
        /// </param>
        /// <returns>
        ///   A new <see cref="HttpClient"/> that will dispose the underlying message handler
        ///   pipeline when disposed.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="primaryHandler"/> is <see langword="null"/>.
        /// </exception>
        public static HttpClient Create(HttpMessageHandler primaryHandler, params DelegatingHandler[] additionalHandlers) {
            return new HttpClient(CreatePipeline(primaryHandler, additionalHandlers), true);
        }


        /// <summary>
        /// Creates a new <see cref="HttpClient"/> using the specified message handlers.
        /// </summary>
        /// <param name="primaryHandler">
        ///   The primary handler for the client.
        /// </param>
        /// <param name="additionalHandlers">
        ///   The additional handlers to add to the request pipeline for the client.
        /// </param>
        /// <returns>
        ///   A new <see cref="HttpClient"/> that will dispose the underlying message handler
        ///   pipeline when disposed.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="primaryHandler"/> is <see langword="null"/>.
        /// </exception>
        public static HttpClient Create(HttpMessageHandler primaryHandler, IEnumerable<DelegatingHandler>? additionalHandlers) {
            return new HttpClient(CreatePipeline(primaryHandler, additionalHandlers), true);
        }
        
        
        /// <summary>
        /// Creates a new <see cref="HttpClient"/> pipeline using the specified message handlers.
        /// </summary>
        /// <param name="additionalHandlers">
        ///   The additional handlers to add to the request pipeline.
        /// </param>
        /// <returns>
        ///   The outer <see cref="HttpMessageHandler"/> for the pipeline.
        /// </returns>
        public static HttpMessageHandler CreatePipeline(params DelegatingHandler[] additionalHandlers) {
            return CreatePipeline(new HttpClientHandler(), (IEnumerable<DelegatingHandler>) additionalHandlers);
        }


        /// <summary>
        /// Creates a new <see cref="HttpClient"/> pipeline using the specified message handlers.
        /// </summary>
        /// <param name="primaryHandler">
        ///   The primary handler for the pipeline.
        /// </param>
        /// <param name="additionalHandlers">
        ///   The additional handlers to add to the request pipeline.
        /// </param>
        /// <returns>
        ///   The outer <see cref="HttpMessageHandler"/> for the pipeline.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="primaryHandler"/> is <see langword="null"/>.
        /// </exception>
        public static HttpMessageHandler CreatePipeline(HttpMessageHandler primaryHandler, params DelegatingHandler[] additionalHandlers) {
            return CreatePipeline(primaryHandler, (IEnumerable<DelegatingHandler>) additionalHandlers);
        }


        /// <summary>
        /// Creates a new <see cref="HttpClient"/> pipeline using the specified message handlers.
        /// </summary>
        /// <param name="primaryHandler">
        ///   The primary handler for the pipeline.
        /// </param>
        /// <param name="additionalHandlers">
        ///   The additional handlers to add to the request pipeline.
        /// </param>
        /// <returns>
        ///   The outer <see cref="HttpMessageHandler"/> for the pipeline.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="primaryHandler"/> is <see langword="null"/>.
        /// </exception>
        public static HttpMessageHandler CreatePipeline(HttpMessageHandler primaryHandler, IEnumerable<DelegatingHandler>? additionalHandlers) {
            if (primaryHandler == null) {
                throw new ArgumentNullException(nameof(primaryHandler));
            }

            if (additionalHandlers == null) {
                return primaryHandler;
            }

            var next = primaryHandler;

            foreach (var handler in additionalHandlers.Reverse()) {
                if (handler == null) {
                    continue;
                }
                handler.InnerHandler = next;
                next = handler;
            }

            return next;
        }

    }
}
