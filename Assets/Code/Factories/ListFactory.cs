using Data;
using UI;
using UnityEngine;

namespace Factories
{
    public class ListFactory
    {
        public ListPanel CreateList(GameObject prefab, Transform parent, ListData listData)
        {
            var list = Object.Instantiate(prefab, parent);
            var listView = list.GetComponent<ListPanel>();
            listView.Init(listData.Uid, listData.ListName, listData.Panels, listData.IsSortable);
            
            return listView;
        }
    }
}