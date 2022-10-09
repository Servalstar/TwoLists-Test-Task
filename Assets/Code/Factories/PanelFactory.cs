using Data;
using Services;
using UI;
using UnityEngine;

namespace Factories
{
    public class PanelFactory
    {
        public MovablePanel CreatePanel(GameObject prefab, Transform parent, PanelData panelData,
            UIRaycaster uiRaycaster)
        {
            var panel = Object.Instantiate(prefab, parent);
            var panelView = panel.GetComponent<MovablePanel>();
            panelView.Init(panelData.Uid, panelData.Name, panelData.Number, uiRaycaster);
            
            return panelView;
        }
        
        public GameObject CreateEmptyPanel(GameObject prefab, Transform parent)
        {
            var panel = Object.Instantiate(prefab, parent);

            return panel;
        }
    }
}