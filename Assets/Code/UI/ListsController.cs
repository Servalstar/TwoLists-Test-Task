using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Factories;
using ScriptableObjects;
using Services;
using Services.AssetManagement;
using Services.Saving;
using UnityEngine;

namespace UI
{
    public class ListsController
    {
        private const string SAVE_KEY = "SavedLists";
        private const string SCRIPTABLE_KEY = "ListsData";
        private const string SORTABLE_LIST_KEY = "SortableList";
        private const string PANEL_KEY = "MovablePanel";
        private const string EMPTY_PANEL_KEY = "EmptyPanel";
        
        private readonly IAssetProvider _assetProvider;
        private readonly IDataSaver _dataSaver;
        private readonly UIRaycaster _uiRaycaster;
        private readonly Transform _listsArea;
        private readonly ListFactory _listFactory;
        private readonly PanelFactory _panelFactory;
        
        private Dictionary<string, ListPanel> _listPanels = new Dictionary<string, ListPanel>();
        private Data _data;

        public ListsController(IAssetProvider assetProvider, IDataSaver dataSaver, 
            UIRaycaster uiRaycaster, Transform listsArea,
            ListFactory listFactory, PanelFactory panelFactory)
        {
            _assetProvider = assetProvider;
            _dataSaver = dataSaver;
            _uiRaycaster = uiRaycaster;
            _listsArea = listsArea;
            _listFactory = listFactory;
            _panelFactory = panelFactory;
        }

        public void Init()
        {
            LoadSave();

            var listPrefab = _assetProvider.Load<GameObject>(SORTABLE_LIST_KEY);
            var panelPrefab = _assetProvider.Load<GameObject>(PANEL_KEY);
            var emptyPanelPrefab = _assetProvider.Load<GameObject>(EMPTY_PANEL_KEY);
            
            foreach (var listData in _data.List)
            {
                var list = _listFactory.CreateList(listPrefab, _listsArea, listData);
                var emptyPanel = _panelFactory.CreateEmptyPanel(emptyPanelPrefab, list.transform);
                var panels = new List<MovablePanel>();
                CreatePanels(listData, panelPrefab, list, panels);
                
                list.Subscribe(SwapPanels, ReplacePanelToAnotherList, SortByName, SortByNumber);
                list.BindPanels(panels, emptyPanel);

                _listPanels.Add(listData.Uid, list);
            }
        }

        private void CreatePanels(ListData listData, GameObject panelPrefab, ListPanel list, List<MovablePanel> panels)
        {
            foreach (var panelData in listData.Panels)
            {
                var panel = _panelFactory.CreatePanel(panelPrefab, list.GetPanelsArea(), panelData, _uiRaycaster);
                panels.Add(panel);
            }
        }

        private void LoadSave()
        {
            _data = new Data();
            
            if (_dataSaver.SaveExists(SAVE_KEY))
                _data = _dataSaver.Load<Data>(SAVE_KEY);
            else
                _data.List = _assetProvider.Load<ListsDataBase>(SCRIPTABLE_KEY).Lists;
        }

        private void SwapPanels(string listUid, string firstPanelUid, string secondPanelUid)
        {
            var panels = GetList(listUid).Panels;
            var firstPanelIndex = panels.FindIndex(x => x.Uid == firstPanelUid);
            var secondPanelIndex = panels.FindIndex(x => x.Uid == secondPanelUid);
            (panels[firstPanelIndex], panels[secondPanelIndex]) = (panels[secondPanelIndex], panels[firstPanelIndex]);

            _listPanels[listUid].UpdateList(panels, firstPanelUid);
            SaveLists();
        }

        private void SortByName(string listUid, bool sortByAscending) => 
            SortList(listUid, sortByAscending, data => data.Name);

        private void SortByNumber(string listUid, bool sortByAscending) => 
            SortList(listUid, sortByAscending, data => data.Number);

        private void SortList<T>(string listUid, bool sortByAscending, Func<PanelData, T> keySelector)
        {
            var list = GetList(listUid);

            if (sortByAscending)
                list.Panels = list.Panels.OrderBy(keySelector).ToList();
            else
                list.Panels = list.Panels.OrderByDescending(keySelector).ToList();
            
            _listPanels[listUid].UpdateList(list.Panels);
            SaveLists();
        }

        private void ReplacePanelToAnotherList(string oldListUid, string newListUid, string panelUid)
        {
            var oldListPanels = GetList(oldListUid);
            var newListPanels = GetList(newListUid);
            var panelData = oldListPanels.Panels.First(x => x.Uid == panelUid);
            oldListPanels.Panels.Remove(panelData);
            newListPanels.Panels.Add(panelData);

            var panel = _listPanels[oldListUid].RemovePanel(panelUid);
            _listPanels[newListUid].InsertPanel(panel);
            
            _listPanels[oldListUid].UpdateList(oldListPanels.Panels);
            _listPanels[newListUid].UpdateList(newListPanels.Panels);
            SaveLists();
        }

        private ListData GetList(string listUid) => 
            _data.List.First(x => x.Uid == listUid);

        private void SaveLists() => 
            _dataSaver.Save(SAVE_KEY, _data);

        private class Data
        {
            public List<ListData> List;
        }
    }
}