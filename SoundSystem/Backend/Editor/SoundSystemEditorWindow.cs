using UnityEditor;
using UnityEngine.UIElements;
/// 
/// Helpful links
/// https://stackoverflow.com/questions/17593101/how-to-write-a-gui-editor-for-graph-or-tree-structures
/// https://forum.unity.com/threads/simple-node-editor.189230/
/// https://forum.unity.com/threads/unity-ui-flowchart.283423/
/// 
/// https://docs.unity3d.com/Manual/UIE-HowTo-CreateEditorWindow.html
/// 

namespace CC.SoundSystem.Editor
{
    /// Author: L1nkCC
    /// Created: 10/22/2023
    /// Last Edited: 10/24/2023
    /// 
    /// <summary>
    /// Window for All Sound System Editing
    /// </summary>
    public class SoundSystemEditorWindow : EditorWindow
    {
        //Components
        NodeConnectorView ConnectorView;
        TwoPaneSplitView SplitView;
        VisualElement LeftPane;
        VisualElement RightPane;
        IMGUIContainer DomainEditorView;

        //Window Refrenced in DomainEditorView's IMGUIContainer
        DomainWindow DomainEditorWindow;

        //Currently selected Domain
        string m_selectedDomain;

        /// <summary>
        /// Allow For creation through Unity Tool bar
        /// </summary>
        [MenuItem("Window/CC/Sound System/Node Editor Graph")]
        public static void CreateWindow()
        {
            GetWindow<SoundSystemEditorWindow>();

        }
        /// <summary>
        /// Construct Window Details and Extended Elements
        /// </summary>
        private void OnEnable()
        {
            titleContent = new("Sound System");
            Init();
        }

        /// <summary>
        /// Connect Elements and Setup View with Default values
        /// </summary>
        public void CreateGUI()
        {
            ConnectUIElements();
            ConnectorView.OnConnectionChange();
            ConnectorView.LoadDomain(m_selectedDomain);
        }
        /// <summary>
        /// Initalize Window 
        /// </summary>
        private void Init()
        {
            m_selectedDomain = Domain.GetAll()[0];
            InitElements();
            InitCallbacks();
        }
        /// <summary>
        /// Set Intial Values of UIElements
        /// </summary>
        private void InitElements()
        {
            SplitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);
            LeftPane = new VisualElement();
            RightPane = new VisualElement();
            ConnectorView = new NodeConnectorView();

            DomainEditorWindow = CreateInstance<DomainWindow>();
            DomainEditorView = new IMGUIContainer(DomainEditorWindow.OnGUI);

        }
        /// <summary>
        /// Connect Callbacks to call necessary functions for Window to work
        /// </summary>
        private void InitCallbacks()
        {
            DomainEditorWindow.Deletor.OnDomainDeleted += (string domainName) => { DomainDeleted(domainName); UpdateConnector(); };
            DomainEditorWindow.Content.OnDomainSelectionChange += (string domainName) => { m_selectedDomain = domainName; UpdateConnector(); };
            DomainEditorWindow.Content.OnNodeSelected += (Node node) => { ConnectorView.Focus(node); };
            DomainEditorWindow.Content.OnSort += () => { ConnectorView.LoadDomain(m_selectedDomain);};
            ConnectorView.OnConnectionChange += () => rootVisualElement.UpdateDomainNotices(m_selectedDomain);
            ConnectorView.OnAddNode += () => rootVisualElement.UpdateDomainNotices(m_selectedDomain);

        }
        /// <summary>
        /// Organizing Elements into appropriate heiarchies
        /// </summary>
        private void ConnectUIElements()
        {
            rootVisualElement.Add(SplitView);
            LeftPane.Add(DomainEditorView);
            SplitView.Add(LeftPane);
            RightPane.Add(ConnectorView);
            SplitView.Add(RightPane);
        }
        /// <summary>
        /// Handle Deleting a Domain that Editor is currently viewing
        /// </summary>
        /// <param name="domainName">Domain Deleted</param>
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

        /// <summary>
        /// Update Connector to be in sync with Editor
        /// </summary>
        public void UpdateConnector()
        {
            ConnectorView.LoadDomain(m_selectedDomain);
        }
    }
}
