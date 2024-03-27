﻿using System.Collections.Immutable;
using FakeItEasy;
using FluentAssertions;
using Mdk.CommandLine.CommandLine;
using Mdk.CommandLine.IngameScript.Pack;
using Mdk.CommandLine.IngameScript.Pack.DefaultProcessors;
using Mdk.CommandLine.SharedApi;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace MDK.CommandLine.Tests.ScriptCombiners;

[TestFixture]
public class CombinerTests
{
    [Test]
    public async Task CombineAsync_With5DocumentsWithVariousUsingDeclarations_CombinesAndUnifiesUsingDeclarations()
    {
        // Arrange
        var workspace = new AdhocWorkspace();
        var project = workspace.AddProject("TestProject", LanguageNames.CSharp);
        var document1 = project.AddDocument("TestDocument1",
            """
            using System;
            class Program {}

            """);
        project = document1.Project;
        var document2 = project.AddDocument("TestDocument2",
            """
            using System;
            using System.Collections.Generic;
            class OtherClass {}

            """);
        project = document2.Project;
        var document3 = project.AddDocument("TestDocument3",
            """
            using System.Linq;
            class AnotherClass {}

            """);
        project = document3.Project;
        var document4 = project.AddDocument("TestDocument4",
            """
            using System.Text;
            using System.Collections.Generic;
            class YetAnotherClass {}

            """);
        project = document4.Project;
        var document5 = project.AddDocument("TestDocument5",
            """
            using System.Threading.Tasks;
            class AndAnotherClass {}

            """);
        project = document5.Project;
        var combiner = new Combiner();
        var parameters = new Parameters
        {
            Verb = Verb.Pack,
            PackVerb = 
            {
                MinifierLevel = MinifierLevel.None,
                TrimUnusedTypes = false,
                ProjectFile = @"A:\Fake\Path\Project.csproj",
                Output = @"A:\Fake\Path\Output"
            }
        };
        var context = new PackContext(
            parameters,
            A.Fake<IConsole>(o => o.Strict()),
            A.Fake<IInteraction>(o => o.Strict()),
            A.Fake<IFileFilter>(o => o.Strict()),
            A.Fake<IImmutableSet<string>>(o => o.Strict())
        );

        // Act
        var result = await combiner.CombineAsync(project, new[] { document1, document2, document3, document4, document5 }, context);

        // Assert
        result.Should().NotBeNull();
        var syntaxTree = await result.GetSyntaxTreeAsync();
        syntaxTree.Should().NotBeNull();
        var root = await syntaxTree!.GetRootAsync();
        root.Should().NotBeNull();
        var usingDirectives = root.DescendantNodes().OfType<UsingDirectiveSyntax>().Select(u => u.Name?.ToString()).ToList();
        usingDirectives.Should().HaveCount(5);
        usingDirectives.Should().BeEquivalentTo("System", "System.Collections.Generic", "System.Linq", "System.Text", "System.Threading.Tasks");
    }
}