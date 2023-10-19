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
        //Serializations
        SerializedObject m_serialized;
        DomainEditorWindow m_domainWindowSerialized;

        //Components
        ListView NodeValues;
        DropdownField DomainSelector;

        //inputValues
        [SerializeField] int m_selectedDomainIndex = 0;


        //Current Status Variables
        string[] Domains => Domain.GetAll();
        string SelectedDomain => Domains[m_selectedDomainIndex];
        Node[] Nodes => Domain.GetNodes(SelectedDomain);  

        [MenuItem("Window/CC/Sound System/GraphView")]
        public static void CreateWindow()
        {
            GetWindow<SoundSystemEditorWindow>();

        }
        private void OnEnable()
        {
            titleContent = new("Sound System");
            m_serialized = new SerializedObject(this);
            m_domainWindowSerialized = CreateInstance<DomainEditorWindow>();
            NodeValues = new ListView();
            DomainSelector = new DropdownField(Domains.ToList(), 0);
        }

        public void CreateGUI()
        {
            var splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);
            var leftPane = new VisualElement();
            var rightPane = new VisualElement();

            var ConnectorView = new NodeConnectorView();
            ConnectorView.OnConnectionChange += () => rootVisualElement.AddDomainNotices(SelectedDomain);
            ConnectorView.OnAddNode += () => rootVisualElement.AddDomainNotices(SelectedDomain);
            m_selectedDomainIndex = DomainSelector.index;
            NodeValues.itemsSource = Domain.GetNodeNames(SelectedDomain);
            NodeValues.Rebuild();
            //attempt.AddElement();
            //new IMGUIContainer(m_domainWindowSerialized.OnGUI);

            rootVisualElement.Add(splitView);
            leftPane.Add(DomainSelector);
            leftPane.Add(NodeValues);
            splitView.Add(leftPane);
            rightPane.Add(ConnectorView);
            splitView.Add(rightPane);
            ConnectorView.OnConnectionChange();
        }
    }
}
