using Runtime.Objects;

namespace Ast.NodeArchetypes;

public abstract class SymbolNode : BranchNode
{
	public string Id = "";

	protected SymbolNode(Node parent, bool createsNestedScope = false) : base(parent, createsNestedScope)
	{
	}

	protected sealed override void Annotate()
	{
		_symbolTable.Upsert(this.GetRuntimeObject(), this);
		AdditionalAnnotation();
	}

	/// <summary>
	/// SymbolNodes automatically add their symbol to the symbol table. 
	/// If you want to do more validation, use this method for that.
	/// </summary>
	protected virtual void AdditionalAnnotation() { }

	/// <summary>
	/// For example, if it is a PatternNode that has a field "ThePattern" for its Pattern runtime object, 
	/// this method should just be "return this.ThePattern;"
	/// </summary>
	protected abstract RuntimeObject GetRuntimeObject();
}

