using UnityEditor;
using System.Linq;
using System;
using System.Collections.Generic;
using CC.Core.InputValidation;

namespace CC.SoundSystem
{

    /// Author: L1nkCC
    /// Created: 10/12/2023
    /// Last Edited: 11/6/2023
    /// 
    /// <summary>
    /// Class for accessing nodes by their Domain
    /// </summary>
    public static class Domain
    {

        enum LoadStatus
        {
            NotStarted,
            Started,
            Finished,
        }

        private static LoadStatus m_loadStatus = LoadStatus.NotStarted;
        private static Dictionary<string, HashSet<Node>> m_domains = null;

        /// <summary>
        /// Used to set the current domains. ONLY TO BE USED BY DOMAINSAVEUTILITIES
        /// </summary>
        /// <param name="newDomains">the new value for m_domains</param>
        public static void SetDomains(Dictionary<string, HashSet<Node>> newDomains)
        {
            m_domains = newDomains;
        }

        /// <summary>
        /// Assure that m_domains has been initialized with Saved Values
        /// </summary>
        public static void AssureLoad()
        {
            if (m_loadStatus == LoadStatus.NotStarted)//|| m_loadStatus.Status != TaskStatus.Running || m_loadStatus.Status != TaskStatus.RanToCompletion || m_loadStatus.Status != TaskStatus.WaitingToRun)
            {
                m_loadStatus = LoadStatus.Started;
                DomainSaveUtilities.LoadAllDomains();
                m_loadStatus = LoadStatus.Finished;
            }
        }

        #region Getters
        /// <summary>
        /// Returns all the names of domains. If there are no domains, create a default
        /// </summary>
        /// <returns>All Domain names</returns>
        public static string[] GetAll()
        {
            AssureLoad();
            if (m_domains.Count == 0)
            {
                CreateDefaultDomain();
            }
            return m_domains.Keys.ToArray();
        }

        /// <summary>
        /// Returns all the nodes of a particular domain
        /// </summary>
        /// <param name="domainName">Domain to source</param>
        /// <returns>All nodes in given domain null if otherwise</returns>
        public static Node[] GetNodes(string domainName)
        {
            AssureLoad();
            HashSet<Node> nodes;
            if (m_domains.TryGetValue(domainName, out nodes))
            {
                return nodes.ToArray();
            }
            return null;
        }
        /// <summary>
        /// Get a node with the specified name from the domain
        /// </summary>
        /// <param name="domainName">The domain to search</param>
        /// <param name="nodeName">The name to search for</param>
        /// <returns>A node with the specified name in the domain or null if one does not exist</returns>
        public static Node GetNode(string domainName, string nodeName)
        {
            AssureLoad();
            Node[] query = GetNodes(domainName).Where(node => node.name.Equals(nodeName)).ToArray();
            if (query.Length == 0) return null;
            return query[0];
        }

        /// <summary>
        /// Get root of domain
        /// </summary>
        /// <param name="domainName">Domain to search</param>
        /// <returns>Root node for domain. If it is not well defined/has only one, then this will return the first one it finds and null if the domain is empty</returns>
        public static Node GetRoot(string domainName)
        {
            AssureLoad();
            Node[] nodes = GetNodes(domainName);
            if (nodes.Length == 0) return null;
            return nodes.Where(node => node.IsRoot).ToArray()[0];
        }
        /// <summary>
        /// Get all roots of domain
        /// </summary>
        /// <param name="domainName">Domain to search</param>
        /// <returns>All roots that a domain has. This is better used for domains that are not well defined</returns>
        public static Node[] GetRoots(string domainName)
        {
            AssureLoad();
            return GetNodes(domainName).Where(node => node.IsRoot).ToArray();
        }

        /// <summary>
        /// Get the names of every Node in the Domain
        /// </summary>
        /// <param name="domainName">The domain to get nodes from</param>
        /// <returns>Array of node names in domain</returns>
        public static string[] GetNodeNames(string domainName)
        {
            AssureLoad();
            return GetNodes(domainName).Select(node => node.name).ToArray();
        }

        /// <summary>
        /// Returns the name of the domain the passed node is in
        /// </summary>
        /// <param name="node">Node to get domain of</param>
        /// <returns>Domain name</returns>
        public static string GetDomainOf(Node node)
        {
            AssureLoad();
            foreach (string domainName in GetAll())
            {
                if (m_domains.GetValueOrDefault(domainName).Contains(node)) return domainName;
            }
            return null;
        }
        #endregion
        #region Status
        /// <summary>
        /// Checks to see if the domain passed has only one node that has no parents / is a root
        /// </summary>
        /// <param name="domain">Domain to search</param>
        /// <returns>if domain contains only one root</returns>
        public static bool HasOneRoot(string domain)
        {
            AssureLoad();
            int countOfRoots = 0;
            Node[] nodes = GetNodes(domain);
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i].IsRoot) countOfRoots++;
            }
            return countOfRoots == 1;
        }
        #endregion
        #region Domain
        /// <summary>
        /// Create a node for each name passed in at a folder with fileName
        /// </summary>
        /// <param name="domainName">Folder Node scriptable objects will be saved</param>
        /// <param name="nodeNames">Names of Nodes to be created</param>
        public static void CreateDomain(string domainName, string[] nodeNames)
        {
            AssureLoad();
            ValidateInputs(domainName, nodeNames);
            HashSet<Node> nodes = new();
            foreach (string nodeName in nodeNames)
            {
                nodes.Add(Node.CreateInstance(nodeName));
            }
            m_domains.Add(domainName, nodes);
        }

        /// <summary>
        /// Set up a default Domain so domain folder is never empty
        /// </summary>
        public static void CreateDefaultDomain()
        {
            AssureLoad();
            try
            {
                string domainName = "Default";
                CreateDomain(domainName, new string[0]);
                Node master = Node.CreateInstance("Master");
                Node background = Node.CreateInstance("Background", master);
                Node UI = Node.CreateInstance("UI", master);
                Node PlayerActions = Node.CreateInstance("PlayerActions", master);
                Node EntityActions = Node.CreateInstance("EntityActions", master);
                AddNode(domainName, master);
                AddNode(domainName, background);
                AddNode(domainName, UI);
                AddNode(domainName, PlayerActions);
                AddNode(domainName, EntityActions);
            }
            catch (InputValidationException e)
            {
                throw new ApplicationException("Cannot Delete Default Domain if it is the only domain that exists. \n" + e.Message);
            }

        }


        /// <summary>
        /// Delete Domain by name, If no domain would remain, this creates a defualt Domain.
        /// </summary>
        /// <param name="domainName">Folder to Delete with path relative to the Domains Folder</param>
        public static void DeleteDomain(string domainName)
        {
            AssureLoad();
            string[] domainNames = Domain.GetAll();
            if (domainNames.Length == 1 && m_domains.ContainsKey(domainName))
            {
                CreateDefaultDomain();
            }
            m_domains.Remove(domainName);
        }

        #endregion
        #region Nodes
        /// <summary>
        /// Saves and adds a node to the domain specified, saving it to the domain's folder
        /// NOTE: Will throw a DomainUniqueNamingException() if the passed node shares a name with a node already in the domain
        /// </summary>
        /// <param name="domainName">The domain the node will be added</param>
        /// <param name="node">The node to be saved</param>
        public static void AddNode(string domainName, Node node)
        {
            AssureLoad();
            ValidateNodeName(domainName, node.name);

            HashSet<Node> nodesInDomain;
            if (m_domains.TryGetValue(domainName, out nodesInDomain))
            {
                if (nodesInDomain.Add(node))
                {
                    m_domains.Remove(domainName);
                    m_domains.Add(domainName, nodesInDomain);
                }
            }
        }
        /// <summary>
        /// Removes specified node from domain if it is in the domain. Throws an Error if it is not
        /// </summary>
        /// <param name="domainName">Domain to search</param>
        /// <param name="node">Node to delete</param>
        public static void DeleteNode(string domainName, Node node)
        {
            AssureLoad();
            if (!GetNodeNames(domainName).Contains(node.name)) throw new System.ArgumentException("Domain " + domainName + " does not contain a node named " + node.name);
            HashSet<Node> nodesInDomain;
            if (m_domains.TryGetValue(domainName, out nodesInDomain))
            {
                if (nodesInDomain.Remove(node))
                {
                    if (!node.IsRoot) node.Parent.RemoveChild(node);
                    foreach (Node child in node.Children)
                    {
                        node.RemoveChild(child);
                    }
                    m_domains.Remove(domainName);
                    m_domains.Add(domainName, nodesInDomain);
                }
            }
        }

        /// <summary>
        /// Clear all Nodes out of a domain.
        /// </summary>
        /// <param name="domainName">Domain to clear</param>
        public static void ClearDomain(string domainName)
        {
            AssureLoad();
            m_domains.Remove(domainName);
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Validate Inputs For IO operations
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="nodeNames"></param>
        private static void ValidateInputs(string domainName, string[] nodeNames)
        {
            AssureLoad();
            if (string.IsNullOrWhiteSpace(domainName)) throw new InputValidationException("Domain Name must not be white space or null");
            if (m_domains.ContainsKey(domainName)) throw new InputValidationException("Domain " + domainName + " Already Exists. Please Enter another name for the domain");
            domainName.ValidateInput();

            if (nodeNames != null)
            {
                if (nodeNames.Distinct().Count() != nodeNames.Length) throw new InputValidationException("Node Names are not distinct. Please Enter names that are unique");
                nodeNames.ValidateInput();
            }
        }

        /// <summary>
        /// Check to make sure a nodeName is a valid input. Will throw a InputValidationException if not
        /// </summary>
        /// <param name="domainName">Domain to check</param>
        /// <param name="nodeName">Node that may be added</param>
        public static void ValidateNodeName(string domainName, string nodeName)
        {
            AssureLoad();
            if (GetNodeNames(domainName).Contains(nodeName)) throw new InputValidationException("Node Names are not distinct. Please Enter names that are unique");
            nodeName.ValidateInput();
        }
        #endregion
    }
}