using System;
using System.Collections.Generic;
using Data;
using UnityEditor;
using UnityEngine;

namespace ScriptableObjects
{
    [CustomEditor(typeof(ListsDataBase))]
    public class ListsDataBaseEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            ListsDataBase listsDataBase = (ListsDataBase)target;

            if (GUILayout.Button("SetRandomUIDs")) 
                SetUIDs(listsDataBase.Lists);

            DrawDefaultInspector();
        }

        private void SetUIDs(List<ListData> listDatas)
        {
            foreach (var list in listDatas)
            {
                list.Uid = Guid.NewGuid().ToString();
                
                foreach (var panel in list.Panels) 
                    panel.Uid = Guid.NewGuid().ToString();
            }
        }
    }
}