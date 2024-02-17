﻿using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;

namespace Mdk.CommandLine.IngameScript.Api;

/// <summary>
/// A processor that works on the syntax tree of the individual code files before they are combined.
/// </summary>
public interface IScriptPreprocessor
{
    /// <summary>
    /// Processes the syntax tree of an individual code file before it is combined.
    /// </summary>
    /// <param name="syntaxTree">The syntax tree of the code file to process.</param>
    /// <param name="metadata">Information about the project being processed.</param>
    /// <returns></returns>
    Task<CSharpSyntaxTree> ProcessAsync(CSharpSyntaxTree syntaxTree, ScriptProjectMetadata metadata);
}