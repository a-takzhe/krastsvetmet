using System.Collections.Generic;
using System.Linq;

namespace scheduler
{
    /// <summary>
    /// Класс реализующий пять Обобщений Джонсона
    /// </summary>
    class Johnson
    {
        private struct HelpForFirdGeneralization
        {
            public int nmID { get; set; }
            public int mtID { get; set; }
            public int bottleneck { get; set; }
        }
        List<List<int>> GeneralizMatrix;

        public List<int> JohnsonSchedule(List<List<int>> TimeMap, int generalization)
        {
            if(generalization == 1)
            {
                return firstGeneralization(TimeMap);
            }
            else if (generalization == 2)
            {
                return secondGeneralization(TimeMap);
            }
            else if (generalization == 3)
            {
                return thirdGeneralization(TimeMap);
            }
            else if (generalization == 4)
            {
                return fourGeneralization(TimeMap);
            }
            else
            {
                return fiveGeneralization(TimeMap);
            }

        }

        private List<int> firstGeneralization(List<List<int>> TimeMap)
        {
            GeneralizMatrix = new List<List<int>>();
            List<int> GeneralizList = new List<int>();
            var tmpList = new List<int>(TimeMap[0]);
            
            foreach(var el in TimeMap[0].OrderBy(c => c))
            {
                for(int i = 0; i < tmpList.Count; i++)
                {
                    if(tmpList[i] == el)
                    {
                        GeneralizList.Add(i);
                        tmpList[i] = -1;
                        break;
                    }
                }
            }

            GeneralizMatrix.Add(GeneralizList);
            return GeneralizMatrix[GeneralizMatrix.Count-1];
        }

        private List<int> secondGeneralization(List<List<int>> TimeMap)
        {
            if (GeneralizMatrix == null)
            {
                GeneralizMatrix = new List<List<int>>();
            }

            List<int> GeneralizList = new List<int>();
            var tmpList = new List<int>(TimeMap[TimeMap.Count - 1]);

            foreach (var el in TimeMap[TimeMap.Count - 1].OrderByDescending(c => c))
            {
                for (int i = 0; i < tmpList.Count; i++)
                {
                    if (tmpList[i] == el)
                    {
                        GeneralizList.Add(i);
                        tmpList[i] = -1;
                        break;
                    }
                }
            }

            GeneralizMatrix.Add(GeneralizList);
            return GeneralizMatrix[GeneralizMatrix.Count - 1];
        }

        private List<int> thirdGeneralization(List<List<int>> TimeMap)
        {
            if (GeneralizMatrix == null)
            {
                GeneralizMatrix = new List<List<int>>();
            }

            List<int> GeneralizList = new List<int>();
            List<HelpForFirdGeneralization> thrGnrlLIst = new List<HelpForFirdGeneralization>();

            for (int i = 0; i < TimeMap[0].Count; i++)
            {
                List<int> tmp = new List<int>();
                for (int j = 0; j < TimeMap.Count; j++)
                {
                    tmp.Add(TimeMap[j][i]);
                }
                thrGnrlLIst.Add(new HelpForFirdGeneralization
                {
                    nmID = i,
                    bottleneck = tmp.Max(),
                    mtID = tmp.IndexOf(tmp.Max())
                });
            }

            foreach(var el in thrGnrlLIst.OrderByDescending(c => c.bottleneck).OrderByDescending(c => c.mtID))
            {
                GeneralizList.Add(el.nmID);
            }
            
            GeneralizMatrix.Add(GeneralizList);
            return GeneralizMatrix[GeneralizMatrix.Count - 1];
        }

        private List<int> fourGeneralization(List<List<int>> TimeMap)
        {
            if (GeneralizMatrix == null)
            {
                GeneralizMatrix = new List<List<int>>();
            }

            List<int> GeneralizList = new List<int>();
            List<int> tmp = new List<int>();

            for (int i = 0; i < TimeMap[0].Count; i++)
            {
                int amountTime = 0;
                for (int j = 0; j < TimeMap.Count; j++)
                {
                    amountTime += TimeMap[j][i];
                }
                tmp.Add(amountTime);
            }

            foreach(var el in tmp.OrderByDescending(c => c).ToList())
            {
                int index = tmp.IndexOf(el);
                GeneralizList.Add(index);
                tmp[index] = -1;
            }

            GeneralizMatrix.Add(GeneralizList);
            return GeneralizMatrix[GeneralizMatrix.Count - 1];
        }

        private List<int> fiveGeneralization(List<List<int>> TimeMap)
        {
            firstGeneralization(TimeMap);
            secondGeneralization(TimeMap);
            thirdGeneralization(TimeMap);
            fourGeneralization(TimeMap);
            ///////////////////////////////
            List<int> GeneralizList = new List<int>();
            Dictionary<int,int> tmp = new Dictionary<int, int>();

            for (int i = 0; i < TimeMap[0].Count; i++)
            {
                int summ = 0;
                for(int j = 0; j < 3; j++)
                {
                    summ += GeneralizMatrix[j].IndexOf(i);
                }
                tmp.Add(i, summ);
            }

            foreach (var el in tmp.OrderBy(c => c.Value).ToList())
            {
                GeneralizList.Add(el.Key);
            }

            GeneralizMatrix.Add(GeneralizList);
            return GeneralizMatrix[GeneralizMatrix.Count - 1];
        }
    }
}
