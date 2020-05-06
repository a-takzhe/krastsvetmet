using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scheduler
{
    /// <summary>
    /// Класс реализующий метод Петрова-Соколицына
    /// </summary>
    class Petrov_Sokolitsin
    {
        private List<List<int>> ConditionMatrix;
        private List<int> SummList;

        public List<int> Petrov_SokolitsinSchedule(List<List<int>> TimeMap)
        {
            FirstSumm(TimeMap);
            SecondSumm(TimeMap);
            Difference(TimeMap);

            int index = SummList.IndexOf(SummList.Min());
            return ConditionMatrix[index];
        }
        private void FirstSumm(List<List<int>> TimeMap)
        {
            ConditionMatrix = new List<List<int>>();
            SummList = new List<int>();
            List<int> ConditionList = new List<int>();
            List<int> tmp = new List<int>();

            for (int i = 0; i < TimeMap[0].Count; i++)
            {
                int amountTime = 0;
                for (int j = 0; j < TimeMap.Count-1; j++)
                {
                    amountTime += TimeMap[j][i];
                }
                tmp.Add(amountTime);
            }

            foreach (var el in tmp.OrderBy(c => c).ToList())
            {
                int index = tmp.IndexOf(el);
                ConditionList.Add(index);
                tmp[index] = -1;
            }

            SummList.Add(TotalTime(TimeMap, ConditionList));
            ConditionMatrix.Add(ConditionList);
        }

        private void SecondSumm(List<List<int>> TimeMap)
        {
            List<int> ConditionList = new List<int>();
            List<int> tmp = new List<int>();

            for (int i = 0; i < TimeMap[0].Count; i++)
            {
                int amountTime = 0;
                for (int j = 1; j < TimeMap.Count; j++)
                {
                    amountTime += TimeMap[j][i];
                }
                tmp.Add(amountTime);
            }

            foreach (var el in tmp.OrderByDescending(c => c).ToList())
            {
                int index = tmp.IndexOf(el);
                ConditionList.Add(index);
                tmp[index] = -1;
            }

            SummList.Add(TotalTime(TimeMap, ConditionList));
            ConditionMatrix.Add(ConditionList);
        }

        private void Difference(List<List<int>> TimeMap)
        {
            List<int> ConditionList = new List<int>();
            List<int> tmp = new List<int>();

            for (int i = 0; i < TimeMap[0].Count; i++)
            {
                tmp.Add(TimeMap[TimeMap.Count-1][i]- TimeMap[0][i]);
            }

            foreach (var el in tmp.OrderByDescending(c => c).ToList())
            {
                int index = tmp.IndexOf(el);
                ConditionList.Add(index);
                tmp[index] = -1;
            }

            SummList.Add(TotalTime(TimeMap, ConditionList));
            ConditionMatrix.Add(ConditionList);
        }

        private int TotalTime(List<List<int>> TimeMap, List<int> ConditionList)
        {
            List<List<int>> SortedTimeMap = new List<List<int>>();
            foreach(var el in TimeMap)
            {
                SortedTimeMap.Add(new List<int>(el));
            }
            
            for(int i = 0; i < TimeMap.Count; i++)
            {
                for(int j = 0; j < TimeMap[0].Count; j++)
                {
                    SortedTimeMap[i][j] = TimeMap[i][ConditionList[j]];
                }
            }

            for (int i = 0; i < TimeMap.Count; i++)
            {
                for (int j = 0; j < TimeMap[0].Count; j++)
                {
                    if (i == 0 && j != 0)
                    {
                        SortedTimeMap[i][j] += SortedTimeMap[i][j - 1];
                    }
                    else if(j==0 && i != 0)
                    {
                        SortedTimeMap[i][j] += SortedTimeMap[i - 1][j];
                    }
                    else if (i != 0 && j != 0)
                    {
                        SortedTimeMap[i][j] += Math.Max(SortedTimeMap[i][j - 1], SortedTimeMap[i - 1][j]);
                    }
                }
            }

            return SortedTimeMap[SortedTimeMap.Count-1][SortedTimeMap[0].Count-1];
        }

    }
}
