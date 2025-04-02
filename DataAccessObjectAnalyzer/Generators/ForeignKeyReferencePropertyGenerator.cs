using DataAccessObjectAnalyzer.DataStructures;
using DataAccessObjectAnalyzer.Extensions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SyntaxExtensions = DataAccessObjectAnalyzer.Extensions.SyntaxExtensions;

namespace DataAccessObjectAnalyzer.Generators
{
    public class ForeignKeyReferencePropertyGenerator : CSharpSyntaxWalker
    {
        /// <summary>
        /// Template used during <see cref="VisitClassDeclaration(ClassDeclarationSyntax)"/> for caching purposes.
        /// </summary>
        public static string BaseTemplate = File.ReadAllText(Path.Combine(SyntaxExtensions.GetSrcFilePath(), "../..", "Template/FKPropertyTemplate.txt"));

        /// <summary>
        /// Notify property changed template for custom DAO.
        /// </summary>
        public static string NotifyTemplate = (File.ReadAllText(Path.Combine(SyntaxExtensions.GetSrcFilePath(), "../..", "Template/NotifyPropertyChangedTemplate.txt")));

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
        public ForeignKeyReferencePropertyGenerator(ClassDeclarationSyntax targetClassNode)
        {
            _targetClassNode = targetClassNode;
        }

        /// <inheritdoc/>
        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            if (node == _targetClassNode)
                return;

            base.VisitClassDeclaration(node);
            GenerateReferences();
        }

        /// <inheritdoc/>
        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            base.VisitPropertyDeclaration(node);

            if (!node.HasAttribute("ForeignKey"))
                return;

            _propertiesAndFields.Add(node);
        }

        /// <inheritdoc/>
        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            base.VisitFieldDeclaration(node);

            if (!node.HasAttribute("ForeignKey"))
                return;

            _propertiesAndFields.Add(node);
        }

        /// <summary>
        /// Generates the references source code for the ref-ID properties.
        /// </summary>
        private void GenerateReferences()
        {
            string templateText = BaseTemplate;
            foreach (MemberDeclarationSyntax member in _propertiesAndFields)
            {
                _sb.AppendLine(templateText);

                if (member is PropertyDeclarationSyntax propertyDeclSyntax)
                {
                    ForeignKeyAttribute fkAttribute = propertyDeclSyntax.GetForeignKeyAttribute();

                    string name = propertyDeclSyntax.Identifier.ValueText;
                    _sb.Replace("%refClass%", fkAttribute.ReferencedClass);
                    _sb.Replace("%refClassId%", fkAttribute.PropertyName);
                    _sb.Replace("%thisId%", name);
                }
                else if (member is FieldDeclarationSyntax fieldDeclSyntax)
                {
                    ForeignKeyAttribute fkAttribute = fieldDeclSyntax.GetForeignKeyAttribute();

                    bool generateNotifyChanged = fieldDeclSyntax.HasAttribute("ObservableProperty");

                    string fullName = fieldDeclSyntax.Declaration.Variables[0].Identifier.ValueText;
                    char firstLetter = fullName[1];
                    string tmp = fullName.Remove(0, 2);

                    string name = char.ToUpper(firstLetter).ToString() + tmp;
                    string generateNotifyTypeName = fieldDeclSyntax.Declaration.Type.ToString();

                    if (generateNotifyChanged)
                    {
                        _sb.AppendLine(NotifyTemplate);
                    }

                    _sb.Replace("%refClass%", fkAttribute.ReferencedClass);
                    _sb.Replace("%refClassId%", fkAttribute.PropertyName);
                    _sb.Replace("%thisId%", name);

                    _sb.Replace("%propertyName%", name);
                    _sb.Replace("%propertyType%", generateNotifyTypeName);
                }
            }
        }
    }
}
