﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RemoveRedundantParenthesesCodeFixProvider))]
    [Shared]
    public class RemoveRedundantParenthesesCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.RemoveRedundantParentheses); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out ParenthesizedExpressionSyntax parenthesizedExpression))
                return;

            CodeAction codeAction = CodeAction.Create(
                "Remove redundant parentheses",
                cancellationToken => RemoveRedundantParenthesesRefactoring.RefactorAsync(context.Document, parenthesizedExpression, cancellationToken),
                GetEquivalenceKey(DiagnosticIdentifiers.RemoveRedundantParentheses));

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }
    }
}
