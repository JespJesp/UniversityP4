
using Ast.Tables;
using Runtime.Objects;

namespace Ast.Nodes;

public abstract class VariableNode(Node parent, bool createsNestedScope = false) : Node(parent, createsNestedScope)
{
	public string Id = "";

	protected sealed override void Annotate(NodeTable ancestors, SemanticSymbolTable symbols)
	{
		symbols.Add(this);
		AdditionalAnnotation(ancestors, symbols);
	}

	protected sealed override void Evaluate(NodeTable ancestors, RuntimeVariableTable variables)
	{
		variables.Upsert(this.GetRuntimeObject(), this);
		AdditionalEvaluation(ancestors, variables);
	}

	/// <summary>
	/// For example, if it is a PatternNode which has a field "ThePattern" for its Pattern runtime object, 
	/// this method should just be "return this.ThePattern;"
	/// </summary>
	protected abstract RuntimeObject GetRuntimeObject();

	/// <summary>
	/// VariableNodes automatically add their symbol to the symbol table. 
	/// If you want to do more annotation, use this method for that.
	/// </summary>
	protected abstract void AdditionalAnnotation(NodeTable ancestors, SemanticSymbolTable symbols);

	/// <summary>
	/// VariableNodes automatically add their runtime object to the variable table. 
	/// If you want to do more evaluation, use this method for that.
	/// </summary>
	protected abstract void AdditionalEvaluation(NodeTable ancestors, RuntimeVariableTable variables);
}

