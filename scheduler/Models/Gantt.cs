using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace scheduler
{
    //Класс модель дерева для Диаграммы Гантта
    class Gantt
    {
        public string MachineTool { get; set; }
        public ObservableCollection<G_Nomenclature> Nomenclatures { get; set; }
        public bool IsNodeExpanded { get; set; }

        public Gantt()
        {
            Nomenclatures = new ObservableCollection<G_Nomenclature>();
        }
    }

    class G_Nomenclature
    {
        public string GN_NomenclatureName { get; set; }
        public int GN_WorkingHours { get; set; }
        public int GN_Start { get; set; }
        public int GN_End { get; set; }
        public Thickness GN_LeftMargin { get; set; }
        public ObservableCollection<G_PartiesParametrs> PartiesParametrs { get; set; }

    public G_Nomenclature()
        {
            PartiesParametrs = new ObservableCollection<G_PartiesParametrs>();
        }
    }

    class G_PartiesParametrs
    {
        public int GP_Batch_ID { get; set; }
        public string GP_Nomenclarure { get; set; }
        public int GP_WorkingHours { get; set; }
        public int GP_Start { get; set; }
        public int GP_End { get; set; }
        public Thickness GP_LeftMargin { get; set; }
    }
}
