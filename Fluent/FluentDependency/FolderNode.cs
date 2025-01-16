using MinimalisticWPF.Extension;
using System;
using System.Collections.Generic;
using System.IO;

namespace MinimalisticWPF.Fluent.FluentDependency
{
    public class FolderNode
    {
        public FolderNode() { }

        private string _name = string.Empty;
        internal string _customroot = string.Empty;

        public string Path
        {
            get
            {
                var result = System.IO.Path.Combine(Root, Name);
                if (result.IsPathValid())
                {
                    return result;
                }
                return string.Empty;
            }
        }
        public int Depth { get; internal set; } = 0;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    if (Parent != null && Parent.Children.Any(x => x.Name == value && x.Depth == Depth))
                    {
                        throw new InvalidOperationException("A node with the same name already exists at this depth.");
                    }
                    Parent?.Children.Remove(this);
                    _name = value;
                    Parent?.Children.Add(this);
                }
            }
        }
        public string Root
        {
            get
            {
                if (!string.IsNullOrEmpty(_customroot))
                {
                    return _customroot;
                }
                if (Parent != null && !string.IsNullOrEmpty(Parent.Path))
                {
                    return Parent.Path;
                }
                return string.Empty;
            }
            set
            {
                if (System.IO.Path.Combine(Root, value).IsPathValid())
                {
                    _customroot = value;
                }
            }
        }
        public FolderNode? Parent { get; internal set; }
        public HashSet<FolderNode> Children { get; internal set; } = [];

        public static void BuildLink(FolderNode current, ICollection<FolderNode> nodes)
        {
            current.Children.Clear();
            foreach (var child in current.FindChildren(nodes))
            {
                current.Children.Add(child);
                BuildLink(child, nodes);
            }
        }
        public IEnumerable<FolderNode> FindChildren(ICollection<FolderNode> nodes)
        {
            return nodes.Where(n => n.Parent == this);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Depth);
        }
        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (FolderNode)obj;
            return Name == other.Name && Depth == other.Depth;
        }
    }
}
