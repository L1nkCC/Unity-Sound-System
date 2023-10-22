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
        NodeConnectorView ConnectorView;
        TwoPaneSplitView SplitView;
        VisualElement LeftPane;
        VisualElement RightPane;
        VisualElement DomainEditorView;

        DomainWindow DomainEditorWindow;


        string m_selectedDomain = Domain.GetAll()[0];
        //Current Status Variables
        string[] Domains => Domain.GetAll();

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
            ConnectorView.OnConnectionChange();
            ConnectorView.LoadDomain(m_selectedDomain);
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
            ConnectorView = new NodeConnectorView();

            DomainEditorWindow = CreateInstance<DomainWindow>();
            DomainEditorView = new IMGUIContainer(DomainEditorWindow.OnGUI);

        }
        private void InitCallbacks()
        {
            DomainEditorWindow.Content.OnDomainSelectionChange += (string domainName) => { m_selectedDomain = domainName; UpdateConnector(); };
            DomainEditorWindow.Deletor.OnDomainDeleted += (string domainName) => { DomainDeleted(domainName); UpdateConnector(); };
            DomainEditorWindow.Content.OnNodeSelected += (Node node) => { ConnectorView.Focus(node); };
            ConnectorView.OnConnectionChange += () => rootVisualElement.UpdateDomainNotices(m_selectedDomain);
            ConnectorView.OnAddNode += () => rootVisualElement.UpdateDomainNotices(m_selectedDomain);
        }

        private void ConnectUIElements()
        {
            rootVisualElement.Add(SplitView);
            LeftPane.Add(DomainEditorView);
            SplitView.Add(LeftPane);
            RightPane.Add(ConnectorView);
            SplitView.Add(RightPane);
        }

        private void DomainDeleted(string domainName)
        {
            if (domainName.Equals(m_selectedDomain))
            {
                if (m_selectedDomain.Equals(Domain.GetAll()[0]))
                {
                    m_selectedDomain = Domain.GetAll()[1];
                }
                else
                {
                    m_selectedDomain = Domain.GetAll()[0];
                }
            }
        }

        public void UpdateConnector()
        {
            ConnectorView.LoadDomain(m_selectedDomain);
        }
    }
}
