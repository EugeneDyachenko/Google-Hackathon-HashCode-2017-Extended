using Mylibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace VideosProblem
{
    class Solution
    {
        // file names
        const string Kittens = "kittens.in";
        const string MeAtTheZoo = "me_at_the_zoo.in";
        const string TrendingToday = "trending_today.in";
        const string VideosWorthSpreading = "videos_worth_spreading.in";


        static int VideosCount;
        static int EndpointCount;
        static int RequestCount;
        static int CacheCount;
        static int CacheSize;

        static string[] VideoSizes;
        static List<EndPoint> EndPoints = new List<EndPoint>();
        static List<Request> Requests = new List<Request>();
        static Dictionary<int, int> CashesFreeSpace = new Dictionary<int, int>(CacheCount);

        static Dictionary<int, List<int>> VideosInCaches = new Dictionary<int, List<int>>();

        static string PathToInput = Kittens;
        static int iterator = 2;

        static private void Main()
        {
            string[] input = ReadingandsaveFromFile.ReadingLiness(PathToInput, 1);

            string[] info = input[0].Split(' ');
            VideosCount = int.Parse(info[0]);
            EndpointCount = int.Parse(info[1]);
            RequestCount = int.Parse(info[2]);
            CacheCount = int.Parse(info[3]);
            CacheSize = int.Parse(info[4]);

            VideoSizes = input[1].Split(' ');

            ReadEndpoints(input);
            ReadRequests(input);
            Initialized();

            Requests.Sort((Request r1, Request r2) => { return r2.Requests - r1.Requests; });

            for (int i = 0; i < Requests.Count; i++)
            {
                AddToHash(Requests[i]);
                Console.WriteLine("Video " + Requests[i].Requests);
            }

            Write();

            for (int i = 0; i < CacheCount; i++)
            {
                Console.WriteLine(CashesFreeSpace[i]);
            }

            Console.ReadLine();
        }

        private static void Initialized()
        {
            for (int i = 0; i < CacheCount; i++)
            {
                VideosInCaches.Add(i, new List<int>());
            }

            for (int i = 0; i < CacheCount; i++)
            {
                CashesFreeSpace.Add(i, CacheSize);
            }
        }

        private static void ReadRequests(string[] input)
        {
            for (int i = iterator; i < input.Length; i++)
            {
                string[] requestString = input[i].Split(' ');
                Request request = new Request();

                request.Video = int.Parse(requestString[0]);
                request.EndPoint = int.Parse(requestString[1]);
                request.Requests = int.Parse(requestString[2]);

                int currentIndex = 0;
                Console.WriteLine("checking:  " + i);
                for (int j = 0; j < Requests.Count; j++)
                {
                    if (Requests[j].Video == request.Video && Requests[j].EndPoint == request.EndPoint)
                    {
                        currentIndex = j;
                        break;
                    }
                }

                if (currentIndex != 0)
                {
                    Requests[currentIndex].Requests += request.Requests;
                }
                else
                {
                    Requests.Add(request);
                }
            }
        }

        private static void ReadEndpoints(string[] input)
        {
            int endPointIterator = 1;

            while (true)
            {
                Console.WriteLine("~~~ EndPoint: " + endPointIterator++);
                string[] endPointString = input[iterator].Split(' ');

                if (endPointString.Length != 2)
                {
                    break;
                }

                EndPoint endPoint = new EndPoint();

                endPoint.Latency = int.Parse(endPointString[0]);
                endPoint.Size = int.Parse(endPointString[1]);

                for (int i = iterator + 1; i < endPoint.Size + iterator + 1; i++)
                {
                    string[] cacheString = input[i].Split(' ');
                    endPoint.LatencyToCashes.Add(int.Parse(cacheString[0]), int.Parse(cacheString[1]));

                }

                EndPoints.Add(endPoint);

                iterator += endPoint.Size + 1;
            }
        }

        static private void Write()
        {
            StringBuilder finalResult = new StringBuilder();

            int currentCachesCount = CacheCount;

            for (int i = 0; i < VideosInCaches.Count; i++)
            {
                if (VideosInCaches[i].Count == 0)
                {
                    currentCachesCount--;
                }
            }

            finalResult.AppendLine(currentCachesCount.ToString());

            for (int i = 0; i < VideosInCaches.Count; i++)
            {
                string next = i + " ";
                for (int j = 0; j < VideosInCaches[i].Count; j++)
                {
                    next += VideosInCaches[i][j] + " ";
                }
                if (next.Length > 2)
                    finalResult.AppendLine(next);
            }

            StreamWriter sw = new StreamWriter(PathToInput.Substring(0, 5) + ".txt");
            sw.Write(finalResult.ToString().TrimEnd('\r', '\n'));
            sw.Close();
        }

        static private void AddToHash(Request r)
        {
            
            int endpointKey = r.EndPoint;
            EndPoint endPoint = EndPoints[endpointKey];


            Dictionary<int, int> caches1 = endPoint.LatencyToCashes;

            var cachesList = caches1.ToList();
            cachesList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));



            for (int i = 0; i < cachesList.Count; i++)
            {
                // Print(i + " " + cachesList[i]);

                int video = int.Parse(VideoSizes[r.Video]);
                
                if (CashesFreeSpace[cachesList[i].Key] - video > 0)
                {
                    if (VideosInCaches[cachesList[i].Key].IndexOf(r.Video) == -1)
                    {
                        CashesFreeSpace[cachesList[i].Key] -= video;
                        VideosInCaches[cachesList[i].Key].Add(r.Video);
                        
                        break;
                    }
                }
            }
        }
    }
}