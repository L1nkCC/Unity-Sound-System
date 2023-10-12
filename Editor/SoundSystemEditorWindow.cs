using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;
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

        string m_enumNamespace = CC.Enum.Editor.EnumEditorHandler.STANDARD_NAMESPACE;
        SerializedObject m_serialized;
        DomainEditorWindow m_domainWindowSerialized;


        [SerializeField] int m_selectedDomainIndex = 0;

        string[] Domains => DomainEditorHandler.GetDomains();
        string SelectedDomain => Domains[m_selectedDomainIndex];
        Node[] Nodes => DomainEditorHandler.GetNodes(SelectedDomain);  

        [MenuItem("Window/CC/Sound System/Draw Menu")]
        public static void CreateWindow()
        {
            GetWindow<SoundSystemEditorWindow>();
        }
        private void OnEnable()
        {
            titleContent = new("Sound System");
            m_serialized = new SerializedObject(this);
            m_domainWindowSerialized = CreateInstance<DomainEditorWindow>();
        }

        public void CreateGUI()
        {
            var splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);
            var leftPane = new VisualElement();
            var rightPane = new VisualElement();


            var domainSelection = new DropdownField("Domains : ", Domains.ToList(), 0);
            var NodeValues = new ListView(Nodes);
            NodeValues.makeItem = () => new ();
            NodeValues.bindItem = (item, index) => { (item as Label).text = Nodes[index].name; };
            NodeValues.itemsSource = Nodes;


            var attempt = new IMGUIContainer(m_domainWindowSerialized.OnGUI);

            /*typeof(Utilities.SoundTree<>).GetType().MakeGenericType(new System.Type[] { SelectedEnum });
            var tree = typeof(Utilities.SoundTree<>).GetConstructor(new System.Type[] { SelectedEnum }).Invoke(new string[0]);

            throw new System.Exception("tree + " + tree.ToString());*/
            
            //var propertyV = new ListView(tree);

            rootVisualElement.Add(splitView);
            leftPane.Add(domainSelection);
            leftPane.Add(NodeValues);
            splitView.Add(leftPane);
            rightPane.Add(attempt);
            splitView.Add(rightPane);



           
        }

        public System.Type[] LoadEnums()
        {
            var asm = Assembly.GetAssembly(typeof(CC.Enum.Editor.EnumEditorHandler));
            return asm.GetTypes().Where(type => type.IsEnum && type.Namespace.Equals(m_enumNamespace)).ToArray();
        }
    }
}
