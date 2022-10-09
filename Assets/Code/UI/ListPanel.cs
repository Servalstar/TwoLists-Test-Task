using System;
using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ListPanel : MonoBehaviour
    {
        [SerializeField] private Text _listName;
        [SerializeField] private Text _numberOfPanels;
        [SerializeField] private Transform _panelsArea;
        [SerializeField] private Toggle _sortByNameToogle;
        [SerializeField] private Toggle _sortByNumberToogle;
        
        private Action<string, string, string> _swapPanels;
        private Action<string, string, string> _replacePanelToAnotherList;
        private Action<string, bool> _sortByName;
        private Action<string, bool> _sortByNumber;
        private List<MovablePanel> _panels;
        private GameObject _emptyPanel;

        public string Uid { get; private set; }

        private void OnEnable()
        {
            _sortByNameToogle.onValueChanged.AddListener(SortByName);
            _sortByNumberToogle.onValueChanged.AddListener(SortByNumber);
        }
        
        private void OnDisable()
        {
            _sortByNameToogle.onValueChanged.RemoveAllListeners();
            _sortByNumberToogle.onValueChanged.RemoveAllListeners();
        }

        public void Init(string uid, string name, List<PanelData> panelDatas, bool isSortable)
        {
            _listName.text = name;
            Uid = uid;
            _numberOfPanels.text = panelDatas.Count.ToString();
            
            _sortByNameToogle.gameObject.SetActive(isSortable);
            _sortByNumberToogle.gameObject.SetActive(isSortable);
        }
        
        public void Subscribe(Action<string, string, string> swapPanels, 
            Action<string, string, string> replacePanelToAnotherList, 
            Action<string, bool> sortByName, Action<string, bool> sortByNumber)
        {
            _swapPanels = swapPanels;
            _replacePanelToAnotherList = replacePanelToAnotherList;
            _sortByName = sortByName;
            _sortByNumber = sortByNumber;
        }

        public Transform GetPanelsArea() => 
            _panelsArea;
        
        public void UpdateList(List<PanelData> panelDatas, string targetPanelUid = null)
        {
            for (int i = 0; i < panelDatas.Count; i++)
            {
                var panel = _panels.Find(x => x.Uid == panelDatas[i].Uid);
                
                if (string.IsNullOrEmpty(targetPanelUid) || targetPanelUid != panelDatas[i].Uid)
                    panel.transform.SetSiblingIndex(i);
            }
            
            _numberOfPanels.text = panelDatas.Count.ToString();
        }

        public void BindPanels(List<MovablePanel> panels, GameObject emptyPanel)
        {
            _emptyPanel = emptyPanel;
            _panels = panels;
            
            foreach (var panel in panels) 
                SetPanel(panel);
        }

        public void ReplacePanels(string oldListUid, string panelUid) => 
            _replacePanelToAnotherList?.Invoke(oldListUid, Uid, panelUid);

        public MovablePanel RemovePanel(string panelUid)
        {
            var panel = _panels.Find(x => x.Uid == panelUid);
            _panels.Remove(panel);
            DeactivateEmptyPanel();

            return panel;
        }

        public void InsertPanel(MovablePanel panel)
        {
            _panels.Add(panel);
            SetPanel(panel);
            DetachPanel(panel.Uid);
        }
        
        private void SetPanel(MovablePanel panel)
        {
            panel.SetParentList(Uid);
            panel.Subscribe(AttachPanel, DetachPanel, SwapPanels);
        }

        private void AttachPanel(string panelUid)
        {
            var panel = _panels.Find(x => x.Uid == panelUid);
            var panelIndex = _emptyPanel.transform.GetSiblingIndex();
            panel.transform.parent = GetPanelsArea();
            panel.transform.SetSiblingIndex(panelIndex);
            DeactivateEmptyPanel();
        }

        private void DetachPanel(string panelUid)
        {
            var panel = _panels.Find(x => x.Uid == panelUid);
            var panelIndex = panel.transform.GetSiblingIndex();
            panel.transform.parent = transform.parent.parent;
            ActivateEmptyPanel(panelIndex);
        }

        private void SwapPanels(string panelUid, string replacablePanelUid) => 
            _swapPanels?.Invoke(Uid, panelUid, replacablePanelUid);

        private void ActivateEmptyPanel(int panelIndex)
        {
            _emptyPanel.SetActive(true);
            _emptyPanel.transform.parent = GetPanelsArea();
            _emptyPanel.transform.SetSiblingIndex(panelIndex);
        }
        
        private void DeactivateEmptyPanel()
        {
            _emptyPanel.SetActive(false);
            _emptyPanel.transform.parent = transform;
        }
        
        private void SortByName(bool isOn) => 
            _sortByName?.Invoke(Uid, !isOn);

        private void SortByNumber(bool isOn) => 
            _sortByNumber?.Invoke(Uid, !isOn);
    }
}