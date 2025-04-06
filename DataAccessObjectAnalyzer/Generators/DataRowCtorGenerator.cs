using DataAccessObjectAnalyzer.Extensions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SyntaxExtensions = DataAccessObjectAnalyzer.Extensions.SyntaxExtensions;

namespace DataAccessObjectAnalyzer.Generators
{
    /// <summary>
    /// Datarow constructor generator. Used to generate constructor for DAO class with a single DataRow object parameter.
    /// </summary>
    public class DataRowCtorGenerator : CSharpSyntaxWalker
    {
        /// <summary>
        /// Template used during <see cref="VisitClassDeclaration(ClassDeclarationSyntax)"/> for caching purposes.
        /// </summary>
        public static string BaseTemplate = File.ReadAllText(Path.Combine(SyntaxExtensions.GetSrcFilePath(), "../..", "Template/DataRowCtorTemplate.txt"));

        /// <summary>
        /// Result string text of the source.
        /// </summary>
        public string ResultString => _sb.ToString();

        private readonly ClassDeclarationSyntax _targetClassNode;
        private readonly StringBuilder _sb = new StringBuilder(2048);
        private List<MemberDeclarationSyntax> _propertiesAndFields = new List<MemberDeclarationSyntax>();
        
        /// <summary>
        /// Ctor.
        /// </summary>
        public DataRowCtorGenerator(ClassDeclarationSyntax targetClassNode)
        {
            _targetClassNode = targetClassNode;
        }

        /// <inheritdoc/>
        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            if (node != _targetClassNode)
                return; 

            base.VisitClassDeclaration(node);

            _sb.AppendLine(BaseTemplate);
            _sb.Replace("%rowCtorParams%", GetRowConstructorParams());
        }

        /// <inheritdoc/>
        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            base.VisitPropertyDeclaration(node);

            if (!node.IsPropertyStatic() && !node.HasAttribute("DatabaseIgnore") && !node.IsReadonly())
                _propertiesAndFields.Add(node);     
        }

        /// <inheritdoc/>
        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            base.VisitFieldDeclaration(node);

            if (node.HasAttribute("ObservableProperty") && !node.HasAttribute("DatabaseIgnore") && !node.IsReadonly())
                _propertiesAndFields.Add(node);     
        }

        /// <summary>
        /// Gets the parameters to use for this base ctor.
        /// </summary>
        /// <returns></returns>
        private string GetRowConstructorParams()
        {
            StringBuilder stringBuilder = new StringBuilder(512);

            var all = _propertiesAndFields.OrderBy(static n => n.SpanStart);
            var lastMember = all.Last();

            foreach (MemberDeclarationSyntax property in all)
            {
                if (property is PropertyDeclarationSyntax propertyDecl)
                {
                    stringBuilder.Append($"row.GetConverted<{propertyDecl.Type}>(\"{propertyDecl.Identifier.ValueText}\")");
                }
                else if (property is FieldDeclarationSyntax fieldDecl)
                {
                    string fullName = fieldDecl.Declaration.Variables[0].Identifier.ValueText;
                    char firstLetter = fullName[1];
                    string tmp = fullName.Remove(0, 2);

                    string newValue = char.ToUpper(firstLetter).ToString() + tmp;

                    stringBuilder.Append($"row.GetConverted<{fieldDecl.Declaration.Type}>(\"{newValue}\")");
                }

                if (lastMember != property) 
                    stringBuilder.Append(',');
            }

            return stringBuilder.ToString();
        }
    }
}
