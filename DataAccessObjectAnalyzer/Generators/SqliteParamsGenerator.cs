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
    public class SqliteParamsGenerator : CSharpSyntaxWalker
    {
        private readonly ClassDeclarationSyntax _targetClassNode;

        private readonly StringBuilder _sb = new StringBuilder(2048);

        public string ResultString => _sb.ToString();

        private List<MemberDeclarationSyntax> _propertiesAndFields = new List<MemberDeclarationSyntax>();

        public SqliteParamsGenerator(ClassDeclarationSyntax targetClassNode)
        {
            _targetClassNode = targetClassNode;
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            if (node == _targetClassNode)
            {
                base.VisitClassDeclaration(node);

                _sb.AppendLine(File.ReadAllText(Path.Combine(SyntaxExtensions.GetSrcFilePath(), "../..", "Template/SqliteParamsTemplate.txt")));
                _sb.Replace("%sqliteParams%", GetParamsString(true));
                _sb.Replace("%sqliteParamsNoId%", GetParamsString(false));
            }
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            base.VisitPropertyDeclaration(node);

            if (!node.IsPropertyStatic() && !node.HasAttribute("DatabaseIgnore") && !node.IsReadonly())
            {
                _propertiesAndFields.Add(node);
            }
        }

        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            base.VisitFieldDeclaration(node);

            if (node.HasAttribute("ObservableProperty") && !node.HasAttribute("DatabaseIgnore") && !node.IsReadonly())
            {
                _propertiesAndFields.Add(node);
            }
        }

        private string GetParamsString(bool includeId)
        {
            StringBuilder stringBuilder = new StringBuilder(1024);

            var all = _propertiesAndFields.OrderBy(n => n.SpanStart);

            var lastProperty = all.Last();
            foreach (var property in all)
            {
                if (property is PropertyDeclarationSyntax propertyDecl)
                {
                    if (propertyDecl.Identifier.ValueText == "ID")
                    {
                        if (includeId) stringBuilder.Append($"new SqliteParameter(@\"{propertyDecl.Identifier.ValueText}\", {GetReturnCheckDBNullable(propertyDecl.Identifier.ValueText, propertyDecl.IsNullable())})");
                        if (includeId && lastProperty != property) stringBuilder.Append(',');
                        continue;
                    }
                    else
                    {
                        stringBuilder.Append($"new SqliteParameter(@\"{propertyDecl.Identifier.ValueText}\", {GetReturnCheckDBNullable(propertyDecl.Identifier.ValueText, propertyDecl.IsNullable())})");
                    }
                }
                else if (property is FieldDeclarationSyntax fieldDecl)
                {
                    string fullName = fieldDecl.Declaration.Variables[0].Identifier.ValueText;
                    char firstLetter = fullName[1];
                    string tmp = fullName.Remove(0, 2);

                    string newValue = char.ToUpper(firstLetter).ToString() + tmp;

                    stringBuilder.Append($"new SqliteParameter(@\"{newValue}\", {GetReturnCheckDBNullable(newValue, fieldDecl.IsNullable())})");
                }

                if (lastProperty != property) stringBuilder.Append(',');
            }

            return stringBuilder.ToString();
        }

        private string GetReturnCheckDBNullable(string name, bool nullable)
        {
            return nullable ? $"{name} != null ? {name} : DBNull.Value" : name;
        }
    }
}
