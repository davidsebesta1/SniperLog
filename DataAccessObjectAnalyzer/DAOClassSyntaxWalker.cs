using DataAccessObjectAnalyzer.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace DataAccessObjectAnalyzer
{
    /// <summary>
    /// Main syntax walker to generate whole partial class for any DAO class.
    /// </summary>
    public class DAOClassSyntaxWalker : CSharpSyntaxWalker
    {
        /// <summary>
        /// Main <see cref="StringBuilder"/>.
        /// </summary>
        public static StringBuilder MainStringBuilder = new StringBuilder(2048);

        /// <summary>
        /// Template used during <see cref="VisitClassDeclaration(ClassDeclarationSyntax)"/> for caching purposes.
        /// </summary>
        public static string BaseTemplate = File.ReadAllText(Path.Combine(Extensions.SyntaxExtensions.GetSrcFilePath(), "..", "Template/DAOPartialTemplate.txt"));

        private readonly GeneratorExecutionContext _context;

        /// <summary>
        /// Ctor.
        /// </summary>
        public DAOClassSyntaxWalker(GeneratorExecutionContext context)
        {
            _context = context;
        }

        /// <inheritdoc/>
        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            base.VisitClassDeclaration(node);

            if (node.BaseList != null && node.BaseList.Types.Any(static n => n != null && n.Type.ToString() == "IDataAccessObject"))
                return;

            MainStringBuilder.Append(BaseTemplate);

            GenerateNoteSaveableCode(node);
            GenerateImageSaveableCode(node);
            GenerateDAOCodeForClass(node);

            SourceText sourceText = CSharpSyntaxTree.ParseText(MainStringBuilder.ToString().Trim()).GetRoot().NormalizeWhitespace().SyntaxTree.GetText();
            SourceText sourceResultEncoded = SourceText.From(sourceText.ToString(), Encoding.UTF8);

            string className = node.Identifier.Text;
            _context.AddSource($"{className}.g.cs", sourceResultEncoded);
            MainStringBuilder.Clear();
        }

        /// <summary>
        /// Generates the additional DAO code for the class.
        /// </summary>
        /// <param name="classNode"></param>
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

        /// <summary>
        /// Generates the IImageSaveable code.
        /// </summary>
        /// <param name="classNode">Target class node.</param>
        private void GenerateImageSaveableCode(ClassDeclarationSyntax classNode)
        {
            if (classNode.BaseList != null && classNode.BaseList.Types.Any(static n => n != null && n.Type.ToString() == "IImageSaveable"))
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

        /// <summary>
        /// Generates the INotesSaveable code.
        /// </summary>
        /// <param name="classNode">Target class node.</param>
        private void GenerateNoteSaveableCode(ClassDeclarationSyntax classNode)
        {
            if (classNode.BaseList != null && classNode.BaseList.Types.Any(static n => n != null && n.Type.ToString() == "INoteSaveable"))
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