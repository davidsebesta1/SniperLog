using DataAccessObjectAnalyzer.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace DataAccessObjectAnalyzer
{
    public class DAOClassSyntaxWalker : CSharpSyntaxWalker
    {
        public static StringBuilder MainStringBuilder = new StringBuilder(2048);

        private readonly GeneratorExecutionContext _context;

        public static string GetSrcFilePath([CallerFilePath] string callerFilePath = null)
        {
            return callerFilePath ?? string.Empty;
        }

        public DAOClassSyntaxWalker(GeneratorExecutionContext context)
        {
            _context = context;
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            base.VisitClassDeclaration(node);

            if (node.BaseList != null && node.BaseList.Types.Any(n => n != null && n.Type.ToString() == "IDataAccessObject"))
            {
                MainStringBuilder.Append(File.ReadAllText(Path.Combine(GetSrcFilePath(), "..", "Template/DAOPartialTemplate.txt")));

                GenerateNoteSaveableCode(node);
                GenerateImageSaveableCode(node);
                GenerateDAOCodeForClass(node);

                SourceText sourceText = CSharpSyntaxTree.ParseText(MainStringBuilder.ToString().Trim()).GetRoot().NormalizeWhitespace().SyntaxTree.GetText();
                SourceText sourceResultEncoded = SourceText.From(sourceText.ToString(), Encoding.UTF8);

                string className = node.Identifier.Text;
                _context.AddSource($"{className}.g.cs", sourceResultEncoded);
                MainStringBuilder.Clear();
            }
        }

        private void GenerateDAOCodeForClass(ClassDeclarationSyntax classNode)
        {
            string className = classNode.Identifier.Text;

            BaseQueriesGenerator baseQueriesGenerator = new BaseQueriesGenerator(classNode);
            baseQueriesGenerator.Visit(classNode);
            MainStringBuilder.Replace("%BaseQueries%", baseQueriesGenerator.ResultString);

            ForeignKeyReferencePropertyGenerator foreignKeyReferencePropertyGenerator = new ForeignKeyReferencePropertyGenerator(classNode);
            foreignKeyReferencePropertyGenerator.Visit(classNode);
            MainStringBuilder.Replace("%ReferencedProperties%", foreignKeyReferencePropertyGenerator.ResultString);

            DataRowCtorGenerator dataRowCtorGenerator = new DataRowCtorGenerator(classNode);
            dataRowCtorGenerator.Visit(classNode);
            MainStringBuilder.Replace("%RowConstructor%", dataRowCtorGenerator.ResultString);

            SqliteParamsGenerator sqliteParamsGenerator = new SqliteParamsGenerator(classNode);
            sqliteParamsGenerator.Visit(classNode);
            MainStringBuilder.Replace("%SqliteParamsMethods%", sqliteParamsGenerator.ResultString);

            MainStringBuilder.Replace("%CurYear%", DateTime.Now.Year.ToString());
            MainStringBuilder.Replace("%Namespace%", "SniperLog.Models");
            MainStringBuilder.Replace("%ClassName%", className);
        }

        private void GenerateImageSaveableCode(ClassDeclarationSyntax classNode)
        {
            if (classNode.BaseList != null && classNode.BaseList.Types.Any(n => n != null && n.Type.ToString() == "IImageSaveable"))
            {
                ImageSaveableGenerator imageSaveableGenerator = new ImageSaveableGenerator(classNode);
                imageSaveableGenerator.Visit(classNode);
                MainStringBuilder.Replace("%ImageSaveable%", imageSaveableGenerator.ResultString);
            }
            else
            {
                MainStringBuilder.Replace("%ImageSaveable%", string.Empty);
            }
        }

        private void GenerateNoteSaveableCode(ClassDeclarationSyntax classNode)
        {
            if (classNode.BaseList != null && classNode.BaseList.Types.Any(n => n != null && n.Type.ToString() == "INoteSaveable"))
            {
                NoteSaveableGenerator noteSaveableGenerator = new NoteSaveableGenerator(classNode);
                noteSaveableGenerator.Visit(classNode);
                MainStringBuilder.Replace("%NoteSaveable%", noteSaveableGenerator.ResultString);
            }
            else
            {
                MainStringBuilder.Replace("%NoteSaveable%", string.Empty);
            }
        }
    }
}