using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace Models
{
    public class NodeModel
    {
        public int id { get; set; }
        public Nullable<int> parent_id { get; set; }
        public bool answer { get; set; }
        public int age { get; set; }
        public string name { get; set; }

        public int step { get; set; }

        public Dictionary<int, NodeModel> children { get; set; }

        public NodeModel()
        {
            parent_id = null;
            children = new Dictionary<int, NodeModel>();
        }

        public string Info() => $"id: {id}, parent id: {parent_id}, answer: {answer}, age: {age}, name: {name}, step: {step}";
    }
}