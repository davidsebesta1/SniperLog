using DataAccessObjectAnalyzer.DataStructures;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DataAccessObjectAnalyzer.Extensions
{
    public static class SyntaxExtensions
    {
        public static string GetSrcFilePath([CallerFilePath] string callerFilePath = null)
        {
            return callerFilePath ?? string.Empty;
        }

        public static bool HasAnyForeignKeyProperty(this ClassDeclarationSyntax node)
        {
            return node.Members.OfType<PropertyDeclarationSyntax>().Any(n => HasAttribute(n, "ForeignKey")) || node.Members.OfType<FieldDeclarationSyntax>().Any(n => HasAttribute(n, "ForeignKey"));
        }

        public static bool HasAttribute(this MemberDeclarationSyntax propertyDeclaration, string name)
        {
            return propertyDeclaration.AttributeLists.Any(attrList => attrList.Attributes.Any(attr => attr.Name.ToString() == name));
        }

        public static ForeignKeyAttribute GetForeignKeyAttribute(this MemberDeclarationSyntax propertyDeclaration)
        {
            foreach (AttributeListSyntax attributes in propertyDeclaration.AttributeLists)
            {
                foreach (AttributeSyntax attr in attributes.Attributes)
                {
                    if (attr.Name.ToString() == "ForeignKey")
                    {
                        var arguments = attr.ArgumentList.Arguments;

                        string referencedTypeName = ForeignKeyAttribute.TypeofRegex.Match(arguments[0].Expression.ToString()).Groups[1].Value;
                        string referencedPropertyName = ForeignKeyAttribute.NameofRegex.Match(arguments[1].Expression.ToString()).Groups[1].Value.Replace(referencedTypeName + ".", string.Empty);

                        return new ForeignKeyAttribute(referencedTypeName, referencedPropertyName);
                    }
                }
            }

            return default;
        }

        public static IEnumerable<MemberDeclarationSyntax> GetForeignKeys(this ClassDeclarationSyntax node)
        {
            foreach (MemberDeclarationSyntax property in node.Members.OfType<PropertyDeclarationSyntax>().Cast<MemberDeclarationSyntax>().Concat(node.Members.OfType<FieldDeclarationSyntax>()))
            {
                if (HasAttribute(property, "ForeignKey")) yield return property;
            }
        }

        public static IEnumerable<FieldDeclarationSyntax> GetAllFields(this ClassDeclarationSyntax node)
        {
            foreach (FieldDeclarationSyntax property in node.Members.OfType<FieldDeclarationSyntax>())
            {
                yield return property;
            }
        }

        public static IEnumerable<PropertyDeclarationSyntax> GetAllProperties(this ClassDeclarationSyntax node)
        {
            foreach (PropertyDeclarationSyntax property in node.Members.OfType<PropertyDeclarationSyntax>())
            {
                yield return property;
            }
        }

        public static bool IsPropertyStatic(this PropertyDeclarationSyntax propertyNode)
        {
            return propertyNode.Modifiers.Any(n => n.IsKind(SyntaxKind.StaticKeyword));
        }

        public static bool IsReadonly(this PropertyDeclarationSyntax propertyNode)
        {
            bool isExpressionBodied = propertyNode.ExpressionBody != null;
            bool isGetOnly = propertyNode.AccessorList?.Accessors.Any(a => a.IsKind(SyntaxKind.SetAccessorDeclaration)) == false;

            return isExpressionBodied || isGetOnly;
        }

        public static bool IsReadonly(this FieldDeclarationSyntax fieldDeclarationSyntax)
        {
            return fieldDeclarationSyntax.Modifiers.Any(SyntaxKind.ReadOnlyKeyword);
        }

        public static bool IsNullable(this PropertyDeclarationSyntax propertyDeclaration)
        {
            return propertyDeclaration.Type is NullableTypeSyntax;
        }

        public static bool IsNullable(this FieldDeclarationSyntax fieldDeclaration)
        {
            return fieldDeclaration.Declaration.Type is NullableTypeSyntax;
        }
    }
}
