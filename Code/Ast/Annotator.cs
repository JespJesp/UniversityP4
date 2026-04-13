using Ast.NodeArchetypes;
using Ast.Nodes;

namespace Ast;

public static class Annotator
{
	public static void Annotate(ProgramNode programNode)
	{
		programNode.AnnotateTree();
	}
}