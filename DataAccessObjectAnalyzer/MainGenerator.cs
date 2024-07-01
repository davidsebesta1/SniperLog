using Microsoft.CodeAnalysis;

namespace DataAccessObjectAnalyzer
{
    [Generator]
    public class MainGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {

        }

        public void Execute(GeneratorExecutionContext context)
        {
            var syntaxTrees = context.Compilation.SyntaxTrees;

            foreach (SyntaxTree syntaxTree in syntaxTrees)
            {
                SyntaxNode root = syntaxTree.GetRoot();

                DAOClassSyntaxWalker walker = new DAOClassSyntaxWalker(context);
                walker.Visit(root);
            }
        }
    }
}