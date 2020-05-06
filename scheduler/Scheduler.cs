using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace scheduler
{
    //Класс Планировщик
    class Scheduler
    {
        /// <summary>
        /// Объекты классов БД
        /// </summary>
        private IQueryable<Parties> parties;
        private IQueryable<Nomenclatures> nomenclatures;
        private IQueryable<MachineTools> machine_tools;
        private IQueryable<Times> times;

        /// <summary>
        /// Вспомогательные элементы
        /// </summary>
        private List<List<int>> TimeMap;//Таблица времени обработки номенклатуры на каждом станке
        public List<List<string>> StringTimeMap;//Таблица времени обработки номенклатуры на каждом станке с дополнительным элементом "Header" для вывода в DGV
        public List<int> ScheduleResult;//Результирующий список ид номенклатур 
        public ObservableCollection<Gantt> GanttTree; //Дерево для графика Гантта

        /// <summary>
        /// В конструкторе происходит чтение данных из файлов
        /// </summary>
        /// <param name="path">путь к файлам</param>
        public Scheduler(string path)
        {
            ConnectionExcel pConnect = new ConnectionExcel(path + "\\parties.xlsx");
            this.parties = from a in pConnect.UrlConnetion.Worksheet<Parties>(0) select a;

            ConnectionExcel nConnect = new ConnectionExcel(path + "\\nomenclatures.xlsx");
            this.nomenclatures = from a in nConnect.UrlConnetion.Worksheet<Nomenclatures>(0) select a;

            ConnectionExcel mConnect = new ConnectionExcel(path + "\\machine_tools.xlsx");
            this.machine_tools = from a in mConnect.UrlConnetion.Worksheet<MachineTools>(0) select a;

            ConnectionExcel tConnect = new ConnectionExcel(path + "\\times.xlsx");
            this.times = from a in tConnect.UrlConnetion.Worksheet<Times>(0) select a;
        }

        /// <summary>
        /// Функция планировщик в которои в зависимости от выбранного типа инициализируются классы реализующие методы Планирования
        /// Планирование производится только по списку номенклатур, а не по всем партиям
        /// </summary>
        /// <param name="method"></param>
        /// <returns>Результирующий список ид номенклатур</returns>
        public List<int> Schedule(string method)
        {
            ScheduleResult = new List<int>();
            if(method == "johnson")
            {
                Johnson johnson = new Johnson();
                ScheduleResult = johnson.JohnsonSchedule(TimeMap, 5);
            }
            else
            {
                Petrov_Sokolitsin petrov_sokolitsin = new Petrov_Sokolitsin();
                ScheduleResult = petrov_sokolitsin.Petrov_SokolitsinSchedule(TimeMap); 
            }
            return ScheduleResult;
        }

        /// <summary>
        /// Создание вспомогательных сводных таблиц для дальнейших работ
        /// </summary>
        public void CreateTimeMaps()
        {
            if (times != null)
            {
                TimeMap = new List<List<int>>();
                StringTimeMap = new List<List<string>>();

                StringTimeMap.Add(GetFirstColl());

                foreach (var machine in machine_tools)
                {
                    var tList = GetNomenclaturesTimeOnTheMachine(machine.id);
                    TimeMap.Add(tList);
                    var stList = tList.ConvertAll<string>(delegate (int i) { return i.ToString(); });
                    stList.Insert(0, machine.name);
                    StringTimeMap.Add(stList);
                }
            }
        }

        private List<string> GetFirstColl()
        {
            List<string> coll = new List<string>();
            coll.Add("Номенклатура");
            coll.AddRange((from n in nomenclatures select n.nomenclature).ToList());
            return coll;
        }

        /// <summary>
        /// Функция заполнения дерева для Диаграммы Гантта
        /// </summary>
        /// <param name="result">Списо ди номенклатур после планирования</param>
        public ObservableCollection<Gantt> CreateGanttTree(List<int> result)
        {
            GanttTree = new ObservableCollection<Gantt>();

            for (int i = 0; i < TimeMap.Count; i++)
            {
                ObservableCollection<G_Nomenclature> G_N = new ObservableCollection<G_Nomenclature>();
                foreach (var nomenclature_id in result)
                {
                    ObservableCollection<G_PartiesParametrs> G_PP = new ObservableCollection<G_PartiesParametrs>();
                    var nName = nomenclatures.FirstOrDefault(n => n.id == nomenclature_id).nomenclature;
                    G_PartiesParametrs LastRec_PP = null;
                    G_PartiesParametrs LastRec_GN = null;

                    if (TimeMap[i][nomenclature_id] == 0) continue;
                    else
                    {
                        foreach (var batch in parties.Where(c => c.nomenclature_id == nomenclature_id))
                        {
                            LastRec_PP = G_PP.LastOrDefault();
                            if (LastRec_PP == null)
                            {
                                if (G_N.LastOrDefault() != null)
                                {
                                    LastRec_GN = G_N.LastOrDefault().PartiesParametrs.LastOrDefault();
                                }
                                else if(GanttTree.LastOrDefault() != null)
                                {
                                    if(GanttTree.LastOrDefault().Nomenclatures.FirstOrDefault(n => n.GN_NomenclatureName == nName) != null)
                                    {
                                        LastRec_GN = GanttTree.LastOrDefault().Nomenclatures.FirstOrDefault(n => n.GN_NomenclatureName == nName).PartiesParametrs.FirstOrDefault();
                                    }
                                }
                                LastRec_PP = LastRec_GN;
                            }
                            G_PP.Add(new G_PartiesParametrs
                            {
                                GP_Batch_ID = batch.id + 1,
                                GP_WorkingHours = TimeMap[i][nomenclature_id],
                                GP_Start = LastRec_PP == null ? 0 : LastRec_PP.GP_End,
                                GP_LeftMargin = new System.Windows.Thickness(LastRec_PP == null ? -20 : LastRec_PP.GP_End-20, 0, 0, 0),
                                GP_End = LastRec_PP == null ? 0 + TimeMap[i][nomenclature_id] : LastRec_PP.GP_End + TimeMap[i][nomenclature_id]
                            });
                        }
                    }
                    G_N.Add(new G_Nomenclature
                    {
                        GN_NomenclatureName = nName,
                        GN_Start = LastRec_GN == null ? 0 : LastRec_GN.GP_End,
                        GN_End = G_PP.LastOrDefault().GP_End,
                        GN_WorkingHours = G_PP.LastOrDefault().GP_End - (LastRec_GN == null ? 0 : LastRec_GN.GP_End),
                        GN_LeftMargin = new System.Windows.Thickness(G_PP.FirstOrDefault().GP_Start, 0, 0, 0),
                        PartiesParametrs = G_PP
                    });
                }
                GanttTree.Add(new Gantt
                {
                    MachineTool = $"Печь {i + 1}",
                    IsNodeExpanded = true,
                    Nomenclatures = G_N
                });
            }
            return GanttTree;
        }

        /// <summary>
        /// Создание таблицы (Партия|Номенклатура)
        /// </summary>
        /// <returns></returns>
        public dynamic CreateListPartiesWithNomenclature()
        {
            if (parties != null)
            {
                var qr = from p in parties.ToList()
                         join n in nomenclatures.ToList() on p.nomenclature_id equals n.id
                         select new { Batch_ID = p.id + 1, Nomenclature = n.nomenclature };

                return qr;
            }
            return null;
        }

        private List<int> GetNomenclaturesTimeOnTheMachine(int tId)
        {
            //Какие партии и за какое время может обработать станок
            var MachineTime = from t in times.ToList()
                              where t.machine_tool_id == tId
                              select new { t.nomenclature_id, t.operation_time };

            //Список времени за которое станок обработает каждую партию
            List<int> BatchsTime = new List<int>();
            foreach (var p in nomenclatures.ToList())
            {
                var t = MachineTime.FirstOrDefault(c => c.nomenclature_id == p.id);
                BatchsTime.Add(t == null ? 0 : t.operation_time);
            }

            return BatchsTime;
        }

        #region Создание матрицы времени по Партии (если время не будет зависеть от номенклатуры)
        public void Parties_CreateTimeMaps()
        {
            if (parties != null)
            {
                TimeMap = new List<List<int>>();
                StringTimeMap = new List<List<string>>();

                StringTimeMap.Add(Parties_GetFirstColl());

                foreach (var machine in machine_tools)
                {
                    var tList = Parties_GetBatchsTimeOnTheMachine(machine.id);
                    TimeMap.Add(tList);
                    var stList = tList.ConvertAll<string>(delegate (int i) { return i.ToString(); });
                    stList.Insert(0, machine.name);
                    StringTimeMap.Add(stList);
                }
            }
        }

        /// <summary>
        /// Возвращает список времени за которое станок обработает каждую партию
        /// </summary>
        /// <param name="tId">Ид станка</param>
        /// <returns>Возвращает список времени за которое станок обработает каждую партию</returns>
        private List<int> Parties_GetBatchsTimeOnTheMachine(int tId)
        {
            //Какие партии и за какое время может обработать станок
            var MachineTime = from t in times.ToList()
                              where t.machine_tool_id == tId
                              select new { t.nomenclature_id, t.operation_time };

            //Список времени за которое станок обработает каждую партию
            List<int> BatchsTime = new List<int>();
            foreach (var p in parties.ToList())
            {
                var t = MachineTime.FirstOrDefault(c => c.nomenclature_id == p.nomenclature_id);
                BatchsTime.Add(t == null ? 0 : t.operation_time);
            }

            return BatchsTime;
        }

        private List<string> Parties_GetFirstColl()
        {
            List<string> coll = new List<string>();
            var qr = from p in parties.ToList()
                     join n in nomenclatures.ToList() on p.nomenclature_id equals n.id
                     select ((p.id + 1).ToString() + " " + n.nomenclature);
            coll.Add("Партия");
            coll.AddRange(qr);
            return coll;
        }
        #endregion
    }
}
