using System;
using System.Collections.Generic;
using System.Text;

namespace CGMonitor.Monitors
{
    public class CircularBuffer
    {
        private readonly float[] _buffer;
        private int _head = 0;
        private int _tail;
        private int _length;
        private int _bufferSize;
        public int Capacity { get => _bufferSize; }
        private readonly Object _lock = new object();

        public CircularBuffer(int capacity)
        {
            _buffer = new float[capacity];
            _bufferSize = capacity;
            _head = capacity - 1;
        }
        public void Add(float value)
        {
            lock (_lock)
            {
                _buffer[_head] = value;
                _head = (_head + 1) % _bufferSize;
            }
        }
        public void Clear()
        {
            lock (_lock)
            {
                Array.Clear(_buffer, 0, _bufferSize);
                _head = 0;
            }
        }

        public (float current, float max) GetStats(float minPossibleMax = 16.6f)
        {
            lock (_lock)
            {
                float max = minPossibleMax;
                for (int i = 0; i < _bufferSize; i++)
                {
                    if (_buffer[i] > max) max = _buffer[i];
                }

                int lastIndex = (_head - 1 + _bufferSize) % _bufferSize;
                float current = _buffer[lastIndex];

                return (current, max);
            }
        }

        public (float low1Percent, float low0_1Percent) GetLowFpsMetrics()
        {
            lock (_lock)
            {
                var validFrames = _buffer.Where(ft => ft > 0).ToArray();
                if (validFrames.Length == 0) return (0f, 0f);

                Array.Sort(validFrames); 

                int count1Percent = Math.Max(1, (int)(validFrames.Length * 0.01));
                int count0_1Percent = Math.Max(1, (int)(validFrames.Length * 0.001));

                float avgFt1Percent = validFrames[^count1Percent..].Average();
                float avgFt0_1Percent = validFrames[^count0_1Percent..].Average();

                return (1000f / avgFt1Percent, 1000f / avgFt0_1Percent);
            }
        }
        public float[] GetOrderedArray()
        {
            lock (_lock)
            {
                float[] ordered = new float[_bufferSize];
                Array.Copy(_buffer, _head, ordered, 0, _bufferSize - _head);
                Array.Copy(_buffer, 0, ordered, _bufferSize - _head, _head);
                return ordered;
            }
        }

    }
}
