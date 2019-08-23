using System;

namespace System.Net.Http {

    /// <summary>
    /// Extensions for <see cref="HttpRequestMessage"/>.
    /// </summary>
    public static class HttpRequestMessageExtensions {

        /// <summary>
        /// The key to use for the request state object in the <see cref="HttpRequestMessage.Properties"/>
        /// dictionary.
        /// </summary>
        public static readonly string StatePropertyName = typeof(Jaahas.Http.HttpRequestTransformHandler).FullName + ".State";


        /// <summary>
        /// Adds a state property to the request's <see cref="HttpRequestMessage.Properties"/> 
        /// dictionary.
        /// </summary>
        /// <typeparam name="T">
        ///   The type of the state object.
        /// </typeparam>
        /// <param name="request">
        ///   The request.
        /// </param>
        /// <param name="state">
        ///   The state object to add to the request's <see cref="HttpRequestMessage.Properties"/> 
        ///   dictionary.
        /// </param>
        /// <returns>
        ///   The <paramref name="request"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="request"/> is <see langword="null"/>.
        /// </exception>
        public static HttpRequestMessage AddStateProperty<T>(this HttpRequestMessage request, T state) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }

            request.Properties[StatePropertyName] = state;
            return request;
        }


        /// <summary>
        /// Gets the value of the request's state property from the <see cref="HttpRequestMessage.Properties"/> 
        /// dictionary.
        /// </summary>
        /// <typeparam name="T">
        ///   The type of the state object.
        /// </typeparam>
        /// <param name="request">
        ///   The request.
        /// </param>
        /// <returns>
        ///   The state object value. If the <see cref="HttpRequestMessage.Properties"/> dictionary 
        ///   does not contain a state property, or the state property is not an instance of 
        ///   <typeparamref name="T"/>, the default value of <typeparamref name="T"/> will be returned.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="request"/> is <see langword="null"/>.
        /// </exception>
        public static T GetStateProperty<T>(this HttpRequestMessage request) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }

            if (!request.Properties.TryGetValue(StatePropertyName, out var o) || !(o is T value)) {
                return default(T);
            }

            return value;
        }


        /// <summary>
        /// Removes the state property from the request's <see cref="HttpRequestMessage.Properties"/> 
        /// dictionary.
        /// </summary>
        /// <param name="request">
        ///   The request.
        /// </param>
        /// <returns>
        ///   The <paramref name="request"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="request"/> is <see langword="null"/>.
        /// </exception>
        public static HttpRequestMessage RemoveStateProperty(this HttpRequestMessage request) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }

            request.Properties.Remove(StatePropertyName);

            return request;
        }

    }
}
