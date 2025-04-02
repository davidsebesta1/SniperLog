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
    /// Generator for base SQL queries - Insert, Update and Delete.
    /// </summary>
    public class BaseQueriesGenerator : CSharpSyntaxWalker
    {
        /// <summary>
        /// Template used during <see cref="VisitClassDeclaration(ClassDeclarationSyntax)"/> for caching purposes.
        /// </summary>
        public static string BaseQueriestTemplate = File.ReadAllText(Path.Combine(SyntaxExtensions.GetSrcFilePath(), "../..", "Template/BaseQueriesTemplate.txt"));

        /// <summary>
        /// Result string text of the source.
        /// </summary>
        public string ResultString => _sb.ToString();

        private readonly ClassDeclarationSyntax _targetClassNode;
        private readonly StringBuilder _sb = new StringBuilder(2048);
        private readonly List<MemberDeclarationSyntax> _propertiesAndFields = new List<MemberDeclarationSyntax>();

        /// <summary>
        /// Ctor.
        /// </summary>
        public BaseQueriesGenerator(ClassDeclarationSyntax targetClassNode)
        {
            _targetClassNode = targetClassNode;
        }

        /// <inheritdoc/>
        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            if (node != _targetClassNode)
                return;

            base.VisitClassDeclaration(node);

            _sb.AppendLine(BaseQueriestTemplate);
            _sb.Replace("%selectAllQuery%", GetSelectAllQuery());
            _sb.Replace("%insertQuery%", GetInsertQuery());
            _sb.Replace("%insertQueryNoId%", GetInsertNoIdQuery());
            _sb.Replace("%deleteQuery%", GetDeleteQuery());
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
        /// Gets the delete query with all required params.
        /// </summary>
        /// <returns>String SQL query.</returns>
        private string GetDeleteQuery()
        {
            MemberDeclarationSyntax pkMember = _propertiesAndFields.FirstOrDefault(static n => n.HasAttribute("PrimaryKey"));

            string pkVarName = null;
            if (pkMember is PropertyDeclarationSyntax propertyDecl)
            {
                pkVarName = propertyDecl.Identifier.ValueText;
            }
            else if (pkMember is FieldDeclarationSyntax fieldDecl)
            {
                string fullName = fieldDecl.Declaration.Variables[0].Identifier.ValueText;
                char firstLetter = fullName[1];
                string tmp = fullName.Remove(0, 2);

                pkVarName = char.ToUpper(firstLetter).ToString() + tmp;
            }

            return $"\"DELETE FROM {_targetClassNode.Identifier.Text} WHERE {_targetClassNode.Identifier.Text}.{pkVarName} = @{pkVarName}\";";
        }

        /// <summary>
        /// Gets the insert query (add) with all required params.
        /// </summary>
        /// <returns>String SQL query.</returns>
        private string GetInsertNoIdQuery()
        {
            StringBuilder stringBuilder = new StringBuilder(512);

            var all = _propertiesAndFields.OrderBy(n => n.SpanStart);
            var lastMember = all.Last();

            stringBuilder.Append($"\"INSERT OR REPLACE INTO {_targetClassNode.Identifier.Text}(");

            foreach (MemberDeclarationSyntax property in all)
            {
                if (property.HasAttribute("PrimaryKey")) continue;

                if (property is PropertyDeclarationSyntax propertyDecl)
                {
                    stringBuilder.Append($"{propertyDecl.Identifier.ValueText}");
                }
                else if (property is FieldDeclarationSyntax fieldDecl)
                {
                    string fullName = fieldDecl.Declaration.Variables[0].Identifier.ValueText;
                    char firstLetter = fullName[1];
                    string tmp = fullName.Remove(0, 2);

                    string newValue = char.ToUpper(firstLetter).ToString() + tmp;

                    stringBuilder.Append($"{newValue}");
                }

                if (lastMember != property) stringBuilder.Append(',');
            }

            stringBuilder.Append($") VALUES(");

            foreach (MemberDeclarationSyntax property2 in all)
            {
                if (property2.HasAttribute("PrimaryKey")) continue;

                if (property2 is PropertyDeclarationSyntax propertyDecl2)
                {
                    stringBuilder.Append($"@{propertyDecl2.Identifier.ValueText}");
                }
                else if (property2 is FieldDeclarationSyntax fieldDecl2)
                {
                    string fullName = fieldDecl2.Declaration.Variables[0].Identifier.ValueText;
                    char firstLetter = fullName[1];
                    string tmp = fullName.Remove(0, 2);

                    string newValue = char.ToUpper(firstLetter).ToString() + tmp;

                    stringBuilder.Append($"@{newValue}");
                }

                if (lastMember != property2) stringBuilder.Append(',');
            }
            stringBuilder.Append($")");

            MemberDeclarationSyntax pkMember = _propertiesAndFields.FirstOrDefault(static n => n.HasAttribute("PrimaryKey"));

            string pkVarName = null;
            if (pkMember is PropertyDeclarationSyntax propertyDeclPk)
            {
                pkVarName = propertyDeclPk.Identifier.ValueText;
            }
            else if (pkMember is FieldDeclarationSyntax fieldDeclPk)
            {
                string fullName = fieldDeclPk.Declaration.Variables[0].Identifier.ValueText;
                char firstLetter = fullName[1];
                string tmp = fullName.Remove(0, 2);

                pkVarName = char.ToUpper(firstLetter).ToString() + tmp;
            }

            if (pkVarName != null)
            {
                stringBuilder.Append($" RETURNING {pkVarName}");
            }

            stringBuilder.Append("\";");

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Gets the insert query (edit) with all required params.
        /// </summary>
        /// <returns>String SQL query.</returns>
        private string GetInsertQuery()
        {
            StringBuilder stringBuilder = new StringBuilder(512);

            var all = _propertiesAndFields.OrderBy(static n => n.SpanStart);
            var lastMember = all.Last();

            stringBuilder.Append($"\"INSERT OR REPLACE INTO {_targetClassNode.Identifier.Text}(");

            foreach (MemberDeclarationSyntax property in all)
            {
                if (property is PropertyDeclarationSyntax propertyDecl)
                {
                    stringBuilder.Append($"{propertyDecl.Identifier.ValueText}");
                }
                else if (property is FieldDeclarationSyntax fieldDecl)
                {
                    string fullName = fieldDecl.Declaration.Variables[0].Identifier.ValueText;
                    char firstLetter = fullName[1];
                    string tmp = fullName.Remove(0, 2);

                    string newValue = char.ToUpper(firstLetter).ToString() + tmp;

                    stringBuilder.Append($"{newValue}");
                }

                if (lastMember != property) stringBuilder.Append(',');
            }

            stringBuilder.Append($") VALUES(");

            foreach (MemberDeclarationSyntax property2 in all)
            {
                if (property2 is PropertyDeclarationSyntax propertyDecl2)
                {
                    stringBuilder.Append($"@{propertyDecl2.Identifier.ValueText}");
                }
                else if (property2 is FieldDeclarationSyntax fieldDecl2)
                {
                    string fullName = fieldDecl2.Declaration.Variables[0].Identifier.ValueText;
                    char firstLetter = fullName[1];
                    string tmp = fullName.Remove(0, 2);

                    string newValue = char.ToUpper(firstLetter).ToString() + tmp;

                    stringBuilder.Append($"@{newValue}");
                }

                if (lastMember != property2) stringBuilder.Append(',');
            }
            stringBuilder.Append($")");

            MemberDeclarationSyntax pkMember = _propertiesAndFields.FirstOrDefault(static n => n.HasAttribute("PrimaryKey"));

            string pkVarName = null;
            if (pkMember is PropertyDeclarationSyntax propertyDeclPk)
            {
                pkVarName = propertyDeclPk.Identifier.ValueText;
            }
            else if (pkMember is FieldDeclarationSyntax fieldDeclPk)
            {
                string fullName = fieldDeclPk.Declaration.Variables[0].Identifier.ValueText;
                char firstLetter = fullName[1];
                string tmp = fullName.Remove(0, 2);

                pkVarName = char.ToUpper(firstLetter).ToString() + tmp;
            }

            if (pkVarName != null)
            {
                stringBuilder.Append($" RETURNING {pkVarName}");
            }

            stringBuilder.Append("\";");

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Gets the select all query.
        /// </summary>
        /// <returns>String SQL query.</returns>
        private string GetSelectAllQuery()
        {
            MemberDeclarationSyntax pkMember = _propertiesAndFields.FirstOrDefault(static n => n.HasAttribute("PrimaryKey"));

            string pkVarName = null;
            if (pkMember is PropertyDeclarationSyntax propertyDecl)
            {
                pkVarName = propertyDecl.Identifier.ValueText;
            }
            else if (pkMember is FieldDeclarationSyntax fieldDecl)
            {
                string fullName = fieldDecl.Declaration.Variables[0].Identifier.ValueText;
                char firstLetter = fullName[1];
                string tmp = fullName.Remove(0, 2);

                pkVarName = char.ToUpper(firstLetter).ToString() + tmp;
            }

            return $"\"SELECT * FROM {_targetClassNode.Identifier.Text}\";";
        }
    }
}