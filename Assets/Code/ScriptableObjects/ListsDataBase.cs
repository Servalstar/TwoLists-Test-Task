using System.Collections.Generic;
using Data;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "ListsData", menuName = "ScriptableObjects/Lists")]
    public class ListsDataBase : ScriptableObject
    {
        public List<ListData> Lists;
    }
}