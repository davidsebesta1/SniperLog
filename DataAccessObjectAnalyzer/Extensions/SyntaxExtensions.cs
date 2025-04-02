using DataAccessObjectAnalyzer.DataStructures;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DataAccessObjectAnalyzer.Extensions
{
    /// <summary>
    /// Extensíons for declaration syntaxes.
    /// </summary>
    public static class SyntaxExtensions
    {
        public const string ForeignKeyAttributeName = "ForeignKey";
        /// <summary>
        /// Gets the source file path from which this method is being called from.
        /// </summary>
        /// <returns>An absolute path to the source file.</returns>
        /// https://stackoverflow.com/questions/47841441/how-do-i-get-the-path-to-the-current-c-sharp-source-code-file
        public static string GetSrcFilePath([CallerFilePath] string callerFilePath = null)
        {
            return callerFilePath ?? string.Empty;
        }

        /// <summary>
        /// Checks whether has this class any foreign key attributes.
        /// </summary>
        /// <param name="classNode">This class syntax.</param>
        /// <returns>Whether this class has any foreign key attribute.</returns>
        public static bool HasAnyForeignKeyProperty(this ClassDeclarationSyntax classNode)
        {
            return classNode.Members.OfType<PropertyDeclarationSyntax>().Any(static n => HasAttribute(n, ForeignKeyAttributeName)) || classNode.Members.OfType<FieldDeclarationSyntax>().Any(n => HasAttribute(n, ForeignKeyAttributeName));
        }

        /// <summary>
        /// Checks whether this member has attribute of the specified type name.
        /// </summary>
        /// <param name="memberNode">This member declaration.</param>
        /// <param name="name">Target attribute name.</param>
        /// <returns>Whether this member has the specified attribute applied.</returns>
        public static bool HasAttribute(this MemberDeclarationSyntax memberNode, string name)
        {
            return memberNode.AttributeLists.Any(attrList => attrList.Attributes.Any(attr => attr.Name.ToString() == name));
        }

        /// <summary>
        /// Gets the foreign key attribute from this member declaration.
        /// </summary>
        /// <param name="memberNode">This member declaration.</param>
        /// <returns>Foreign key attribute struct.</returns>
        public static ForeignKeyAttribute GetForeignKeyAttribute(this MemberDeclarationSyntax memberNode)
        {
            foreach (AttributeListSyntax attributes in memberNode.AttributeLists)
            {
                foreach (AttributeSyntax attr in attributes.Attributes)
                {
                    if (attr.Name.ToString() == ForeignKeyAttributeName)
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

        /// <summary>
        /// Gets foreign key declearations from this class.
        /// </summary>
        /// <param name="classNode">This class node.</param>
        /// <returns>Enumarable of the member nodes.</returns>
        public static IEnumerable<MemberDeclarationSyntax> GetForeignKeys(this ClassDeclarationSyntax classNode)
        {
            foreach (MemberDeclarationSyntax property in classNode.Members.OfType<PropertyDeclarationSyntax>().Cast<MemberDeclarationSyntax>().Concat(classNode.Members.OfType<FieldDeclarationSyntax>()))
            {
                if (HasAttribute(property, ForeignKeyAttributeName))
                    yield return property;
            }
        }

        /// <summary>
        /// Gets all fields from this class node.
        /// </summary>
        /// <param name="classNode">This class node.</param>
        /// <returns>Enumarable of the fields.</returns>
        public static IEnumerable<FieldDeclarationSyntax> GetAllFields(this ClassDeclarationSyntax classNode)
        {
            foreach (FieldDeclarationSyntax property in classNode.Members.OfType<FieldDeclarationSyntax>())
                yield return property;
        }
        
        /// <summary>
        /// Gets all properties form this class node.
        /// </summary>
        /// <param name="classNode">This class node.</param>
        /// <returns>Enumerable of the properties.</returns>
        public static IEnumerable<PropertyDeclarationSyntax> GetAllProperties(this ClassDeclarationSyntax classNode)
        {
            foreach (PropertyDeclarationSyntax property in classNode.Members.OfType<PropertyDeclarationSyntax>())
                yield return property;
        }

        /// <summary>
        /// Gets whether this property is declared as static.
        /// </summary>
        /// <param name="propertyNode">This property node.</param>
        /// <returns>Whether this property is static.</returns>
        public static bool IsPropertyStatic(this PropertyDeclarationSyntax propertyNode)
        {
            return propertyNode.Modifiers.Any(static n => n.IsKind(SyntaxKind.StaticKeyword));
        }

        /// <summary>
        /// Gets whether this property is declared as readonly.
        /// </summary>
        /// <param name="propertyNode">This property node.</param>
        /// <returns>Whether this property is readonly.</returns>
        public static bool IsReadonly(this PropertyDeclarationSyntax propertyNode)
        {
            bool isExpressionBodied = propertyNode.ExpressionBody != null;
            bool isGetOnly = propertyNode.AccessorList?.Accessors.Any(static n => n.IsKind(SyntaxKind.SetAccessorDeclaration)) == false;

            return isExpressionBodied || isGetOnly;
        }

        /// <summary>
        /// Gets whether this field is declared as readonly.
        /// </summary>
        /// <param name="fieldNode">This field node.</param>
        /// <returns>Whether this field is readonly.</returns>
        public static bool IsReadonly(this FieldDeclarationSyntax fieldNode)
        {
            return fieldNode.Modifiers.Any(SyntaxKind.ReadOnlyKeyword);
        }

        /// <summary>
        /// Gets whether this property is declared as nullable.
        /// </summary>
        /// <param name="propertyNode">This property node.</param>
        /// <returns>Whether this property is nullable.</returns>
        public static bool IsNullable(this PropertyDeclarationSyntax propertyNode)
        {
            return propertyNode.Type is NullableTypeSyntax;
        }

        /// <summary>
        /// Gets whether this field is declared as nullable.
        /// </summary>
        /// <param name="fieldNode">This field node.</param>
        /// <returns>Whether this field is nullable.</returns>
        public static bool IsNullable(this FieldDeclarationSyntax fieldNode)
        {
            return fieldNode.Declaration.Type is NullableTypeSyntax;
        }
    }
}
