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
    ///   If your application uses a dependency injection service provider, it is recommended that 
    ///   you use the service provider to create and manage the lifecycle of <see cref="HttpClient"/> 
    ///   instances instead of the static methods on this class.
    /// </remarks>
    public static class HttpClientFactory {

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
        ///   A new <see cref="HttpClient"/> that will dispose of the <paramref name="primaryHandler"/> 
        ///   when disposed.
        /// </returns>
        /// <remarks>
        ///   The create multiple clients that reuse the same pipeline, use the <see cref="CreatePipeline(HttpMessageHandler, DelegatingHandler[])"/> 
        ///   or <see cref="CreatePipeline(HttpMessageHandler, IEnumerable{DelegatingHandler}?)"/> 
        ///   method and create <see cref="HttpClient"/> instances manually via the <see cref="HttpClient(HttpMessageHandler, bool)"/> 
        ///   constructor.
        /// </remarks>
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
        ///   A new <see cref="HttpClient"/> that will dispose of the <paramref name="primaryHandler"/> 
        ///   when disposed.
        /// </returns>
        /// <remarks>
        ///   The create multiple clients that reuse the same pipeline, use the <see cref="CreatePipeline(HttpMessageHandler, DelegatingHandler[])"/> 
        ///   or <see cref="CreatePipeline(HttpMessageHandler, IEnumerable{DelegatingHandler}?)"/> 
        ///   method and create <see cref="HttpClient"/> instances manually via the <see cref="HttpClient(HttpMessageHandler, bool)"/> 
        ///   constructor.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="primaryHandler"/> is <see langword="null"/>.
        /// </exception>
        public static HttpClient Create(HttpMessageHandler primaryHandler, IEnumerable<DelegatingHandler>? additionalHandlers) {
            return new HttpClient(CreatePipeline(primaryHandler, additionalHandlers), true);
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

            if (additionalHandlers == null || !additionalHandlers.Any()) {
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
