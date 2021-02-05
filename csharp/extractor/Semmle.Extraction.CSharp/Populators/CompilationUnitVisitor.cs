using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Semmle.Util.Logging;

namespace Semmle.Extraction.CSharp.Populators
{
    internal class CompilationUnitVisitor : TypeOrNamespaceVisitor
    {
        public CompilationUnitVisitor(Context cx)
            : base(cx, cx.TrapWriter.Writer, null) { }

        public override void VisitExternAliasDirective(ExternAliasDirectiveSyntax node)
        {
            // This information is not yet extracted.
            Cx.ExtractionError("Not implemented extern alias directive", node.ToFullString(), Cx.CreateLocation(node.GetLocation()), "", Severity.Info);
        }

        public override void VisitCompilationUnit(CompilationUnitSyntax compilationUnit)
        {
            foreach (var m in compilationUnit.ChildNodes())
            {
                Cx.Try(m, null, () => ((CSharpSyntaxNode)m).Accept(this));
            }

            // Gather comments:
            foreach (var trivia in compilationUnit.DescendantTrivia(compilationUnit.Span, descendIntoTrivia: true))
            {
                CommentPopulator.ExtractComment(Cx, trivia);
            }

            foreach (var trivia in compilationUnit.GetLeadingTrivia())
            {
                CommentPopulator.ExtractComment(Cx, trivia);
            }

            foreach (var trivia in compilationUnit.GetTrailingTrivia())
            {
                CommentPopulator.ExtractComment(Cx, trivia);
            }
        }
    }
}
