using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.IO;
using System.Text;
using SyntaxExtensions = DataAccessObjectAnalyzer.Extensions.SyntaxExtensions;

namespace DataAccessObjectAnalyzer.Generators
{
    public class NoteSaveableGenerator : CSharpSyntaxWalker
    {
        private readonly ClassDeclarationSyntax _targetClassNode;

        private readonly StringBuilder _sb = new StringBuilder(2048);

        public string ResultString => _sb.ToString();

        public NoteSaveableGenerator(ClassDeclarationSyntax targetClassNode)
        {
            _targetClassNode = targetClassNode;
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            if (node == _targetClassNode)
            {
                base.VisitClassDeclaration(node);
                
                _sb.AppendLine(File.ReadAllText(Path.Combine(SyntaxExtensions.GetSrcFilePath(), "../..", "Template/NoteSaveableTemplate.txt")));
            }
        }
    }
}
