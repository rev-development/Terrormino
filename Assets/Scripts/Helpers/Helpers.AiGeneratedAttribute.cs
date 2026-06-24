using System;
using UnityEngine;

namespace Helpers
{
    /// <summary>
    /// Marks a class, struct, or method as written (fully or partially) by an AI tool.
    /// Intended for codebase auditing: reviewing AI-assisted code, comparing which tool
    /// produced better results, and pruning before publishing. AllowMultiple is true
    /// since a single member may be touched by more than one tool over its lifetime —
    /// add a new attribute instance per contribution rather than overwriting the last one.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Class
        | AttributeTargets.Struct
        | AttributeTargets.Method
        | AttributeTargets.Constructor,
        AllowMultiple = true,
        Inherited = false)]
    public class AiGeneratedAttribute : Attribute
    {
        public string Tool { get; }

        public string Version { get; }

        /// <summary>
        /// Optional free-text note — e.g. "fully generated", "AI-assisted refactor",
        /// "reviewed and revised by Rev on 2026-06-20".
        /// </summary>
        public string Note { get; }

        public AiGeneratedAttribute(string tool, string version = null, string note = null)
        {
            Tool = tool;
            Version = version;
            Note = note;
        }
    }
}
