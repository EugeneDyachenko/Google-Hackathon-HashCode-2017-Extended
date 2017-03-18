using System.Collections.Generic;

namespace VideosProblem
{
    class EndPoint
    {
        public int Latency;
        public int Size;

        public Dictionary<int, int> LatencyToCashes = new Dictionary<int, int>();
    }
}