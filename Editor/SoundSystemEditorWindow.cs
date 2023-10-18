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

        SerializedObject m_serialized;
        DomainEditorWindow m_domainWindowSerialized;

        
        ListView NodeValues;
        DropdownField DomainSelector; 

        [SerializeField] int m_selectedDomainIndex = 0;

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

            var attempt = new NodeConnectorView();


            m_selectedDomainIndex = DomainSelector.index;
            NodeValues.itemsSource = Domain.GetNodeNames(SelectedDomain);
            NodeValues.Rebuild();
            //var attempt2 = new GraphElement();

            //attempt.AddElement();
            //new IMGUIContainer(m_domainWindowSerialized.OnGUI);


            /*typeof(Utilities.SoundTree<>).GetType().MakeGenericType(new System.Type[] { SelectedEnum });
            var tree = typeof(Utilities.SoundTree<>).GetConstructor(new System.Type[] { SelectedEnum }).Invoke(new string[0]);

            throw new System.Exception("tree + " + tree.ToString());*/

            //var propertyV = new ListView(tree);

            rootVisualElement.Add(splitView);
            rootVisualElement.Add(new HelpBox("This is a helpbox", HelpBoxMessageType.Error));
            leftPane.Add(DomainSelector);
            leftPane.Add(NodeValues);
            splitView.Add(leftPane);
            rightPane.Add(attempt);
            splitView.Add(rightPane);
        }

    }
}
