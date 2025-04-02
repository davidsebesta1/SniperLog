using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.IO;
using System.Text;
using SyntaxExtensions = DataAccessObjectAnalyzer.Extensions.SyntaxExtensions;

namespace DataAccessObjectAnalyzer.Generators
{
    /// <summary>
    /// Generates save, delete methods and source properties for any class that implements INoteSaveable interface.
    /// </summary>
    public class NoteSaveableGenerator : CSharpSyntaxWalker
    {
        /// <summary>
        /// Template used during <see cref="VisitClassDeclaration(ClassDeclarationSyntax)"/> for caching purposes.
        /// </summary>
        public static string BaseTemplate = File.ReadAllText(Path.Combine(SyntaxExtensions.GetSrcFilePath(), "../..", "Template/NoteSaveableTemplate.txt"));

        /// <summary>
        /// Result string text of the source.
        /// </summary>
        public string ResultString => _sb.ToString();

        private readonly ClassDeclarationSyntax _targetClassNode;
        private readonly StringBuilder _sb = new StringBuilder(2048);

        /// <summary>
        /// Ctor.
        /// </summary>
        public NoteSaveableGenerator(ClassDeclarationSyntax targetClassNode)
        {
            _targetClassNode = targetClassNode;
        }

        /// <inheritdoc/>
        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            if (node == _targetClassNode)
            {
                base.VisitClassDeclaration(node);
                
                _sb.AppendLine(BaseTemplate);
            }
        }
    }
}
