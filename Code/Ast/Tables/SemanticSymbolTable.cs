using Ast.Nodes;

namespace Ast.Tables;

public class SemanticSymbolTable
{
	/// <summary>
	/// Key: node type and id.
	/// Value: scope depth.
	/// </summary>
	private Dictionary<(Type, string), int> _symbols = new();

	public SemanticSymbolTable Clone()
	{
		return new()
		{
			_symbols = new(this._symbols)
		};
	}

	public bool Contains(Type type, string id)
	{
		return _symbols.ContainsKey((type, id));
	}

	public void Add(VariableNode symbolNode)
	{
		// TODO: Throw semantic error if you try to overwrite a symbol inside of the same scope level
		// (because then it isn't a shadow variable, but a double declaration - which is erroneous)

		if (_symbols.TryGetValue((symbolNode.GetType(), symbolNode.Id), out int oldScope))
		{
			if (symbolNode.ScopeDepth <= oldScope)
			{
				Annotator.AddSemanticError(symbolNode, $"ID: '{symbolNode.Id}'. Scope depth: '{symbolNode.ScopeDepth}'. Double declaration within the same scope level.");
			}
		}

		_symbols[(symbolNode.GetType(), symbolNode.Id)] = symbolNode.ScopeDepth;
	}

}