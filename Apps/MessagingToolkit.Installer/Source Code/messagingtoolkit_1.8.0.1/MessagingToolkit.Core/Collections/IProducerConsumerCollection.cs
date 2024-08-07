﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Core.Collections
{
    /// <summary>
    /// Defines methods to manipulate thread-safe collections intended for producer/consumer usage.
    /// </summary>
    /// <typeparam name="T">Specifies the type of elements in the collection.</typeparam>
    /// <remarks>
    /// All implementations of this interface must enable all members of this interface
    /// to be used concurrently from multiple threads.
    /// </remarks>
    public interface IProducerConsumerCollection<T> : IEnumerable<T>, ICollection
    {

        /// <summary>
        /// Copies the elements of the <see cref="IProducerConsumerCollection{T}"/> to
        /// an
        /// <see cref="T:System.Array"/>, starting at a specified index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of
        /// the elements copied from the <see cref="IProducerConsumerCollection{T}"/>.
        /// The array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array"/> at which copying
        /// begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is a null reference (Nothing in
        /// Visual Basic).</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than
        /// zero.</exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> is equal to or greater than the
        /// length of the <paramref name="array"/>
        /// -or- The number of elements in the source <see cref="ConcurrentQueue{T}"/> is greater than the
        /// available space from <paramref name="index"/> to the end of the destination <paramref
        /// name="array"/>.
        /// </exception>
        void CopyTo(T[] array, int index);

        /// <summary>
        /// Attempts to add an object to the <see
        /// cref="IProducerConsumerCollection{T}"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see
        /// cref="IProducerConsumerCollection{T}"/>.</param>
        /// <returns>true if the object was added successfully; otherwise, false.</returns>
        /// <exception cref="T:System.ArgumentException">The <paramref name="item"/> was invalid for this collection.</exception>
        bool TryAdd(T item);

        /// <summary>
        /// Attempts to remove and return an object from the <see cref="IProducerConsumerCollection{T}"/>.
        /// </summary>
        /// <param name="item">
        /// When this method returns, if the object was removed and returned successfully, <paramref
        /// name="item"/> contains the removed object. If no object was available to be removed, the value is
        /// unspecified.
        /// </param>
        /// <returns>true if an object was removed and returned successfully; otherwise, false.</returns>
        bool TryTake(out T item);

        /// <summary>
        /// Copies the elements contained in the <see cref="IProducerConsumerCollection{T}"/> to a new array.
        /// </summary>
        /// <returns>A new array containing the elements copied from the <see cref="IProducerConsumerCollection{T}"/>.</returns>
        T[] ToArray();

    }
}
