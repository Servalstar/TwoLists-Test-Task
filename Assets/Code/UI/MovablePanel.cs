using System;
using System.Collections.Generic;
using Services;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class MovablePanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Text _nameText;
        [SerializeField] private Text _numberTex;

        public string Uid { get; private set; }
        public string ParentListUid { get; private set; }

        private Action<string, string> _swapPanels;
        private bool _isMove;
        private Vector3 _startPanelPosition;
        private Vector3 _startMousePosition;
        private Action<string> _detachPanel;
        private Action<string> _attachPanel;
        private List<RaycastResult> _results = new List<RaycastResult>();
        private UIRaycaster _uiRaycaster;

        void Update()
        {
            if (_isMove)
            {
                transform.position = _startPanelPosition + (Input.mousePosition - _startMousePosition);
                CheckHits();
            }
        }

        public void Init(string uid, string name, int number, UIRaycaster uiRaycaster)
        {
            _uiRaycaster = uiRaycaster;
            Uid = uid;
            _nameText.text = name;
            _numberTex.text = number.ToString();
            gameObject.SetActive(true);
        }
        
        public void Subscribe(Action<string> attachPanel, Action<string> detachPanel, Action<string, string> swapPanels)
        {
            _attachPanel = attachPanel;
            _detachPanel = detachPanel;
            _swapPanels = swapPanels;
        }

        public void SetParentList(string uid) => 
            ParentListUid = uid;

        public void OnPointerDown(PointerEventData eventData)
        {
            _isMove = true;
            _startPanelPosition = transform.position;
            _startMousePosition = Input.mousePosition;
            _detachPanel?.Invoke(Uid);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isMove = false;
            _attachPanel?.Invoke(Uid);
        }
        
        private void CheckHits()
        {
            _uiRaycaster.GetHits(transform.position, _results);
            
            if (_results.Count > 0)
            {
                foreach (var hit in _results)
                {
                    if (hit.gameObject.TryGetComponent(out ListPanel listPanel) && listPanel.Uid != ParentListUid)
                        listPanel.ReplacePanels(ParentListUid, Uid);
                    else if (hit.gameObject.TryGetComponent(out MovablePanel anotherPanel) && anotherPanel.Uid != Uid &&
                             anotherPanel.ParentListUid == ParentListUid)
                        _swapPanels?.Invoke(Uid, anotherPanel.Uid);
                }
            }
        }
    }
}