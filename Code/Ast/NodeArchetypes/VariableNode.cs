using Ast.Tables;
using Runtime.Objects;

namespace Ast.NodeArchetypes;

public abstract class VariableNode : BranchNode
{
	public string Id = "";

	protected VariableNode(Node parent, bool createsNestedScope = false) : base(parent, createsNestedScope)
	{
	}

	protected sealed override void Validate(SemanticSymbolTable symbols)
	{
		symbols.Add(this);
		AdditionalValidation(symbols);
	}


	protected sealed override void Evaluate(RuntimeVariableTable variables)
	{
		variables.Upsert(this.GetRuntimeObject(), this);
		AdditionalEvaluation(variables);
	}

	/// <summary>
	/// VariableNodes automatically add their symbol to the symbol table. 
	/// If you want to do more validation, use this method for that.
	/// </summary>
	protected virtual void AdditionalValidation(SemanticSymbolTable symbols) {}

	/// <summary>
	/// VariableNodes automatically add their runtime object to the variable table. 
	/// If you want to do more evaluation, use this method for that.
	/// </summary>
	protected virtual void AdditionalEvaluation(RuntimeVariableTable variables) { }

	/// <summary>
	/// For example, if it is a PatternNode that has a field "ThePattern" for its Pattern runtime object, 
	/// this method should just be "return this.ThePattern;"
	/// </summary>
	protected abstract RuntimeObject GetRuntimeObject();
}

