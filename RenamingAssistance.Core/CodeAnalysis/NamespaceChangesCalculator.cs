using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;

namespace RenamingAssistance.Core.CodeAnalysis
{
    public class NamespaceChangesCalculator
    {
        public async Task<ICollection<DocumentChanges>> CalculateChangesAsync(
            ICollection<Document> documents,
            IProgress<ProgressInfo> progress,
            CancellationToken cancellationToken)
        {
            if (!documents.Any())
            {
                return null;
            }

            var context = new ChangesCalculationsContext(documents);

            var itemInProgress = 1;
            foreach (var document in documents)
            {
                var namespaceDeclaration = await GetNamespaceDeclarationIfNeedToFixItAsync(document, context, cancellationToken);

                if (namespaceDeclaration != null)
                {
                    var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
                    var declaredMembers = namespaceDeclaration.ChildNodes().Where(x => x is MemberDeclarationSyntax);
                    foreach (var declaredMember in declaredMembers)
                    {
                        var memberSymbol = semanticModel.GetDeclaredSymbol(declaredMember);
                        await BuildSymbolReferencesAsync(memberSymbol, document, context, cancellationToken);
                    }
                }

                progress.Report(
                    new ProgressInfo($"{itemInProgress} of {documents.Count} namespca in progress...",
                    itemInProgress * 100 / documents.Count));
            }

            return context.Changes;
        }

        private async Task<NamespaceDeclarationSyntax> GetNamespaceDeclarationIfNeedToFixItAsync(Document document, ChangesCalculationsContext context, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken);
            var nodes = syntaxRoot.DescendantNodes().ToList();

            var namespaceNodes = nodes.OfType<NamespaceDeclarationSyntax>().ToList();

            if (!namespaceNodes.Any() || namespaceNodes.Count > 1)
            {
                // TODO: need to handle this case
                return null;
            }

            var namespaceDeclaration = namespaceNodes.Single();
            var expectedNamespace = document.GetExpectedNamespace();
            if (namespaceDeclaration.Name.GetText().ToString().Trim() == expectedNamespace)
            {
                return null;
            }

            context.AddChange(namespaceDeclaration.Name.Span, document, document.GetExpectedNamespace());

            return namespaceDeclaration;
        }

        private async Task BuildSymbolReferencesAsync(
            ISymbol memberSymbol,
            Document documentNamespaceDefinition,
            ChangesCalculationsContext context,
            CancellationToken cancellationToken)
        {
            var symbolReferences = await SymbolFinder.FindReferencesAsync(memberSymbol, documentNamespaceDefinition.Project.Solution, cancellationToken);
            var namedTypeSymbolReferences = symbolReferences.Where(x => x.Definition.Kind == SymbolKind.NamedType);

            foreach (var reference in namedTypeSymbolReferences)
            {
                foreach (var location in reference.Locations)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var syntaxRoot = await location.Document.GetSyntaxRootAsync(cancellationToken);
                    var semanticModel = await location.Document.GetSemanticModelAsync(cancellationToken);
                    var referenceNode = syntaxRoot.FindNode(location.Location.SourceSpan);

                    if (referenceNode.Parent is QualifiedNameSyntax)
                    {
                        var namespaceNode = ((QualifiedNameSyntax)referenceNode.Parent).Left;
                        context.AddChange(namespaceNode.Span, location.Document, documentNamespaceDefinition.GetExpectedNamespace());
                    }
                    else if (referenceNode.Parent is MemberAccessExpressionSyntax)
                    {
                        var namespaceNode = ((MemberAccessExpressionSyntax)referenceNode.Parent).Expression;
                        if (namespaceNode != referenceNode)
                        {
                            context.AddChange(namespaceNode.Span, location.Document, documentNamespaceDefinition.GetExpectedNamespace());
                        }
                    }
                }
            }
        }
    }
}
