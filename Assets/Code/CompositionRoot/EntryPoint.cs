using Factories;
using Services;
using Services.AssetManagement;
using Services.Saving;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CompositionRoot
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private Transform _listsArea;
        [SerializeField] private EventSystem _eventSystem;

        void Start()
        {
            IAssetProvider assetProvider = new ResourcesLoader();
            IDataSaver dataSaver = new JsonSaver();
            var raycaster = new UIRaycaster(_eventSystem);
            var listFactory = new ListFactory();
            var panelFactory = new PanelFactory();

            var listsController = new ListsController(assetProvider, dataSaver, raycaster, _listsArea, listFactory, panelFactory);
            listsController.Init();
        }
    }
}