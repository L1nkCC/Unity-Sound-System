using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System.IO;
using CC.Core.Utilities.IO;
/// <summary>
/// Helpful links
/// https://stackoverflow.com/questions/17593101/how-to-write-a-gui-editor-for-graph-or-tree-structures
/// https://forum.unity.com/threads/simple-node-editor.189230/
/// https://forum.unity.com/threads/unity-ui-flowchart.283423/
/// 
/// https://docs.unity3d.com/Manual/UIE-HowTo-CreateEditorWindow.html
/// </summary>

namespace CC.SoundSystem.Editor
{
    public class SoundSystemEditorWindow : EditorWindow
    {
        //Components
        ListView NodeValues;
        DropdownField DomainSelector;
        NodeConnectorView ConnectorView;
        TwoPaneSplitView SplitView;
        VisualElement LeftPane;
        VisualElement RightPane;

        System.Action OnDomainSelectionChange = ()=>{};

        string m_selectedDomain = Domain.GetAll()[0];
        //Current Status Variables
        string[] Domains => Domain.GetAll();
        string SelectedDomain {
            get
            {
                return m_selectedDomain;
            } 
            set
            {
                m_selectedDomain = value;
                OnDomainSelectionChange();
            }
        }

        [MenuItem("Window/CC/Sound System/GraphView")]
        public static void CreateWindow()
        {
            GetWindow<SoundSystemEditorWindow>();

        }
        private void OnEnable()
        {
            titleContent = new("Sound System");
            Init();
        }

        public void CreateGUI()
        {
            ConnectUIElements();

            UpdateList();
            ConnectorView.OnConnectionChange();
        }

        private void Init()
        {
            InitElements();
            InitCallbacks();
        }

        private void InitElements()
        {
            SplitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);
            LeftPane = new VisualElement();
            RightPane = new VisualElement();
            NodeValues = new ListView();
            DomainSelector = new DropdownField(Domains.ToList(), 0);
            ConnectorView = new NodeConnectorView();

        }
        private void InitCallbacks()
        {
            DomainSelector.RegisterValueChangedCallback(evt => SelectedDomain = evt.newValue);
            OnDomainSelectionChange += UpdateList;
            OnDomainSelectionChange += UpdateConnector;
            ConnectorView.OnConnectionChange += () => rootVisualElement.UpdateDomainNotices(SelectedDomain);
            ConnectorView.OnAddNode += () => rootVisualElement.UpdateDomainNotices(SelectedDomain);
        }

        private void ConnectUIElements()
        {
            rootVisualElement.Add(SplitView);
            LeftPane.Add(DomainSelector);
            LeftPane.Add(NodeValues);
            SplitView.Add(LeftPane);
            RightPane.Add(ConnectorView);
            SplitView.Add(RightPane);
        }

        public void UpdateList()
        {
            NodeValues.itemsSource = Domain.GetNodeNames(SelectedDomain);
            NodeValues.Rebuild();
        }
        public void UpdateConnector()
        {
            ConnectorView.LoadDomain(SelectedDomain);
        }
    }
}
