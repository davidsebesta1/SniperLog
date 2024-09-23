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
    [Generator]
    public class MainGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var daoClasses = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => IsCandidateClass(s),
                    transform: static (context, _) => GetClassWithInterfaces(context))
                .Where(classDecl => classDecl != null);

            context.RegisterSourceOutput(daoClasses, (spc, classDecl) =>
            {
                string generatedCode = GenerateDAOCodeForClass(classDecl);

                SourceText sourceText = CSharpSyntaxTree.ParseText(generatedCode.Trim()).GetRoot().NormalizeWhitespace().SyntaxTree.GetText();
                SourceText sourceResultEncoded = SourceText.From(sourceText.ToString(), Encoding.UTF8);

                spc.AddSource($"{classDecl.Identifier.Text}.g.cs", sourceResultEncoded);
            });
        }

        private static bool IsCandidateClass(SyntaxNode node)
        {
            return node is ClassDeclarationSyntax classDecl && classDecl.BaseList != null && classDecl.BaseList.Types.Any(n => n?.Type.ToString() == "IDataAccessObject");
        }

        private static ClassDeclarationSyntax GetClassWithInterfaces(GeneratorSyntaxContext context)
        {
            return context.Node as ClassDeclarationSyntax;
        }

        private static string GenerateDAOCodeForClass(ClassDeclarationSyntax classNode)
        {
            var stringBuilder = new StringBuilder();
            string className = classNode.Identifier.Text;

            // You can reuse your original code-generation logic here
            stringBuilder.Append(File.ReadAllText(Path.Combine(SyntaxExtensions.GetSrcFilePath(), "..", "Template/DAOPartialTemplate.txt")));

            GenerateNoteSaveableCode(stringBuilder, classNode);
            GenerateImageSaveableCode(stringBuilder, classNode);
            GenerateDAOCode(stringBuilder, classNode);

            return stringBuilder.ToString();
        }

        private static void GenerateNoteSaveableCode(StringBuilder stringBuilder, ClassDeclarationSyntax classNode)
        {
            if (classNode.BaseList != null && classNode.BaseList.Types.Any(n => n?.Type.ToString() == "INoteSaveable"))
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
            if (classNode.BaseList != null && classNode.BaseList.Types.Any(n => n?.Type.ToString() == "IImageSaveable"))
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