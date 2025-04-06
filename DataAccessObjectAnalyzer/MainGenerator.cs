using DataAccessObjectAnalyzer.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.IO;
using System.Linq;
using System.Text;
using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using SyntaxExtensions = DataAccessObjectAnalyzer.Extensions.SyntaxExtensions;

namespace DataAccessObjectAnalyzer
{
    /// <summary>
    /// Generator entry point.
    /// </summary>
    [Generator]
    public class MainGenerator : IIncrementalGenerator
    {
        /// <summary>
        /// Template used during <see cref="GenerateDAOCodeForClass"/> for caching purposes.
        /// </summary>
        public static string BaseTemplate = File.ReadAllText(Path.Combine(SyntaxExtensions.GetSrcFilePath(), "..", "Template/DAOPartialTemplate.txt"));

        /// <inheritdoc/>
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            IncrementalValuesProvider<ClassDeclarationSyntax> daoClasses = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => IsCandidateClass(s),
                    transform: static (context, _) => GetClassDeclaration(context))
                .Where(classDecl => classDecl != null);

            context.RegisterSourceOutput(daoClasses, (spc, classDecl) =>
            {
                string generatedCode = GenerateDAOCodeForClass(classDecl);

                SourceText sourceText = CSharpSyntaxTree.ParseText(generatedCode.Trim()).GetRoot().NormalizeWhitespace().SyntaxTree.GetText();
                SourceText sourceResultEncoded = SourceText.From(sourceText.ToString(), Encoding.UTF8);

                spc.AddSource($"{classDecl.Identifier.Text}.g.cs", sourceResultEncoded);
            });
        }

        /// <summary>
        /// Gets whether this class is candidate for partial code generation.
        /// </summary>
        /// <param name="classNode">The class node the check one</param>
        /// <returns>Whether the code should be generated for this class.</returns>
        private static bool IsCandidateClass(SyntaxNode classNode)
        {
            return classNode is ClassDeclarationSyntax classDecl && classDecl.BaseList != null && classDecl.BaseList.Types.Any(n => n?.Type.ToString() == "IDataAccessObject");
        }

        /// <summary>
        /// Gets the class node from the syntax context.
        /// </summary>
        /// <param name="context">The generator syntax context.</param>
        /// <returns>Class declaration syntax.</returns>
        private static ClassDeclarationSyntax GetClassDeclaration(GeneratorSyntaxContext context)
        {
            return context.Node as ClassDeclarationSyntax;
        }

        /// <summary>
        /// Generates the DAO class code for this class.
        /// </summary>
        /// <param name="classNode">Class node to generate the code for.</param>
        /// <returns></returns>
        private static string GenerateDAOCodeForClass(ClassDeclarationSyntax classNode)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(BaseTemplate);

            GenerateNoteSaveableCode(stringBuilder, classNode);
            GenerateImageSaveableCode(stringBuilder, classNode);
            GenerateDAOCode(stringBuilder, classNode);

            return stringBuilder.ToString();
        }

        private static void GenerateNoteSaveableCode(StringBuilder stringBuilder, ClassDeclarationSyntax classNode)
        {
            if (classNode.BaseList != null && classNode.BaseList.Types.Any(static n => n?.Type.ToString() == "INoteSaveable"))
            {
                NoteSaveableGenerator noteSaveableGenerator = new NoteSaveableGenerator(classNode);
                noteSaveableGenerator.Visit(classNode);
                stringBuilder.Replace("%NoteSaveable%", noteSaveableGenerator.ResultString);
            }
            else
            {
                stringBuilder.Replace("%NoteSaveable%", string.Empty);
            }
        }

        private static void GenerateImageSaveableCode(StringBuilder stringBuilder, ClassDeclarationSyntax classNode)
        {
            if (classNode.BaseList != null && classNode.BaseList.Types.Any(static n => n?.Type.ToString() == "IImageSaveable"))
            {
                ImageSaveableGenerator imageSaveableGenerator = new ImageSaveableGenerator(classNode);
                imageSaveableGenerator.Visit(classNode);
                stringBuilder.Replace("%ImageSaveable%", imageSaveableGenerator.ResultString);
            }
            else
            {
                stringBuilder.Replace("%ImageSaveable%", string.Empty);
            }
        }

        private static void GenerateDAOCode(StringBuilder stringBuilder, ClassDeclarationSyntax classNode)
        {
            string className = classNode.Identifier.Text;

            BaseQueriesGenerator baseQueriesGenerator = new BaseQueriesGenerator(classNode);
            baseQueriesGenerator.Visit(classNode);
            stringBuilder.Replace("%BaseQueries%", baseQueriesGenerator.ResultString);

            ForeignKeyReferencePropertyGenerator foreignKeyReferencePropertyGenerator = new ForeignKeyReferencePropertyGenerator(classNode);
            foreignKeyReferencePropertyGenerator.Visit(classNode);
            stringBuilder.Replace("%ReferencedProperties%", foreignKeyReferencePropertyGenerator.ResultString);

            DataRowCtorGenerator dataRowCtorGenerator = new DataRowCtorGenerator(classNode);
            dataRowCtorGenerator.Visit(classNode);
            stringBuilder.Replace("%RowConstructor%", dataRowCtorGenerator.ResultString);

            SqliteParamsGenerator sqliteParamsGenerator = new SqliteParamsGenerator(classNode);
            sqliteParamsGenerator.Visit(classNode);
            stringBuilder.Replace("%SqliteParamsMethods%", sqliteParamsGenerator.ResultString);

            stringBuilder.Replace("%CurYear%", DateTime.Now.Year.ToString());
            stringBuilder.Replace("%Namespace%", "SniperLog.Models");
            stringBuilder.Replace("%ClassName%", className);
        }
    }
}