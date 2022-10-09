using System;
using System.Collections.Generic;

namespace Data
{
    [Serializable]
    public class ListData
    {
        public string Uid;
        public string ListName;
        public List<PanelData> Panels;
        public bool IsSortable;
    }
}