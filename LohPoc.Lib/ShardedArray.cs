namespace LohPoc.Lib
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class ShardedArray<T> : IEnumerable<T>
    {
        private const int MaxShardSize = 10000;

        private readonly T[][] data;

        public ShardedArray(int length)
        {
            this.Length = length;

            var shardCount = (int)Math.Ceiling((double)length / MaxShardSize);

            this.data = new T[shardCount][];

            var shardIndex = 0;
            while (shardIndex < (shardCount - 1))
            {
                this.data[shardIndex++] = new T[MaxShardSize];
            }

            var initializedCapacity = shardIndex * MaxShardSize;
            if (length > initializedCapacity)
            {
                this.data[shardIndex++] = new T[length - initializedCapacity];
            }
        }

        public ShardedArray(IEnumerable<T> source)
        {
            var data = Enumerable.Empty<IEnumerable<T>>();

            var currentShard = Enumerable.Empty<T>();
            var currentShardSize = 0;

            foreach (var item in source)
            {
                currentShard = currentShard.Append(item);
                currentShardSize++;

                if (currentShardSize >= MaxShardSize)
                {
                    data = data.Append(currentShard);
                    this.Length += currentShardSize;

                    currentShard = Enumerable.Empty<T>();
                    currentShardSize = 0;
                }
            }

            if (currentShardSize > 0)
            {
                data = data.Append(currentShard);
                this.Length += currentShardSize;
            }

            this.data = data.Select(shard => shard.ToArray()).ToArray();
        }

        public T this[int index]
        {
            get
            {
                this.ValidateOutOfBounds(index);

                return this.data[index / MaxShardSize][index % MaxShardSize];
            }
            set
            {
                this.ValidateOutOfBounds(index);

                this.data[index / MaxShardSize][index % MaxShardSize] = value;
            }
        }

        public int Length { get; }

        public IEnumerator<T> GetEnumerator()
        {
            // TODO: check out list enumerator
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            // TODO: check out list enumerator
            throw new NotImplementedException();
        }

        private void ValidateOutOfBounds(int index)
        {
            // The underlying array is sharded, so in order to have useful index/length information
            // in the message we validate the index manually.
            if (index < 0 || index >= this.Length)
            {
                throw new IndexOutOfRangeException(
                    $"Index {index} is out of bounds (array length is {this.Length}).");
            }
        }
    }
}
