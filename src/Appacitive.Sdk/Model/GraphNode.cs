using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public class GraphNode
    {
        public GraphNode()
        {
            this.Children = new Dictionary<string, List<GraphNode>>();
        }

        public Article Article { get; internal set; }

        public Connection Connection { get; internal set; }

        internal Dictionary<string, List<GraphNode>> Children { get; private set; }

        public List<GraphNode> this[string name]
        {
            get
            {
                return this.GetChildren(name);
            }
        }

        public IEnumerable<string> Properties()
        {
            return this.Children.Keys;
        }

        public GraphNode Parent { get; internal set; }

        public List<GraphNode> GetChildren(string name)
        {
            List<GraphNode> children = null;
            if (this.Children.TryGetValue(name, out children) == true)
                return children;
            else return new List<GraphNode>();
        }

        internal void AddChildNode(string name, GraphNode node)
        {
            if (this.Children.ContainsKey(name) == false)
                this.Children[name] = new List<GraphNode>();
            this.Children[name].Add(node);
        }
    }
}
