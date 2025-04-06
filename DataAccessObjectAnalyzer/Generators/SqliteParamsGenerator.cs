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
    /// Generates GetSqlParams() method to get all SqlParameters array for all properties of the targeted class.
    /// </summary>
    public class SqliteParamsGenerator : CSharpSyntaxWalker
    {
        /// <summary>
        /// Template used during <see cref="VisitClassDeclaration(ClassDeclarationSyntax)"/> for caching purposes.
        /// </summary>
        public static string BaseTemplate = File.ReadAllText(Path.Combine(SyntaxExtensions.GetSrcFilePath(), "../..", "Template/SqliteParamsTemplate.txt"));

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
        public SqliteParamsGenerator(ClassDeclarationSyntax targetClassNode)
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
            _sb.Replace("%sqliteParams%", GetParamsString(true));
            _sb.Replace("%sqliteParamsNoId%", GetParamsString(false));
        }

        /// <inheritdoc/>
        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            base.VisitPropertyDeclaration(node);

            if (!node.IsPropertyStatic() && !node.HasAttribute("DatabaseIgnore") && !node.IsReadonly())
            {
                _propertiesAndFields.Add(node);
            }
        }

        /// <inheritdoc/>
        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            base.VisitFieldDeclaration(node);

            if (node.HasAttribute("ObservableProperty") && !node.HasAttribute("DatabaseIgnore") && !node.IsReadonly())
            {
                _propertiesAndFields.Add(node);
            }
        }

        /// <summary>
        /// Gets the params source code string.
        /// </summary>
        /// <param name="includeId">Whether the ID should be included.</param>
        /// <returns>String representation of the source code.</returns>
        private string GetParamsString(bool includeId)
        {
            StringBuilder stringBuilder = new StringBuilder(1024);

            var all = _propertiesAndFields.OrderBy(static n => n.SpanStart);

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

                if (lastProperty != property) 
                    stringBuilder.Append(',');
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Gets the ternary operator to handle nullable DB values.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <param name="nullable">Whether is it nullable.</param>
        /// <returns>String to get the db null or no.</returns>
        private string GetReturnCheckDBNullable(string name, bool nullable)
        {
            return nullable ? $"{name} != null ? {name} : DBNull.Value" : name;
        }
    }
}
