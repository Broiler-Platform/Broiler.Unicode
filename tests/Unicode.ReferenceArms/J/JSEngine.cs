// REFERENCE ARM J: the engine the copies of Broiler.JS's members raise their errors through. It is not a copy:
// it stands for the two factories of Broiler.JS's JSEngine at 4d70821e the copied members call,
// JSEngine.NewSyntaxError (Broiler.JS/Broiler.JavaScript.Engine/Core/JSEngine.cs:135) and
// JSEngine.NewRangeError (:151), which build the JSException (Broiler.JS/Broiler.JavaScript.Runtime/
// JSException.cs:12) a script sees. Here each builds the arm's recorded exception, carrying the error's kind
// (its constructor's name), so that the comparison records a refusal by its kind.

using System;
using Broiler.JavaScript.Runtime;

namespace Broiler.JavaScript.Engine.Core
{
    /// <summary>The arm's error factories, in the source's namespace.</summary>
    internal static class JSEngine
    {
        public static JSException NewSyntaxError(string message) => new("SyntaxError", message);

        public static JSException NewRangeError(string message) => new("RangeError", message);
    }
}

namespace Broiler.JavaScript.Runtime
{
    /// <summary>The arm's recorded exception: an engine error by its kind, in the source's namespace.</summary>
    internal sealed class JSException(string kind, string message) : Exception(message)
    {
        public string Kind { get; } = kind;
    }
}
