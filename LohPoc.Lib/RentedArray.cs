namespace LohPoc.Lib
{
    using System;
    using System.Buffers;

    public class RentedArray<T> : IDisposable
    {
        private static readonly ArrayPool<T> Pool = ArrayPool<T>.Shared;

        private readonly T[] data;
        private bool isDisposed = false;

        public RentedArray(int length)
        {
            this.data = Pool.Rent(length);
            this.Length = length;
        }

        public T this[int index]
        {
            get
            {
                this.ValidateOutOfBounds(index);

                return this.data[index];
            }
            set
            {
                this.ValidateOutOfBounds(index);

                this.data[index] = value;
            }
        }

        public int Length { get; }

        public void Dispose()
        {
            if (this.isDisposed)
            {
                return;
            }

            Pool.Return(this.data);
            this.isDisposed = true;
        }

        private void ValidateOutOfBounds(int index)
        {
            // The rented array may be larger then what we ask for, so we can't validate the
            // size against the underlying array length property.
            if (index < 0 || index >= this.Length)
            {
                throw new IndexOutOfRangeException(
                    $"Index {index} is out of bounds (array length is {this.Length}).");
            }
        }
    }
}
