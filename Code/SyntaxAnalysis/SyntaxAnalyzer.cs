using LexicalAnalysis;
using Song;
using Visitation;

namespace SyntaxAnalysis;

public class SyntaxAnalyzer
{
	private Stack<IVisitable> _visitableStack = new Stack<IVisitable>();
	private Pattern _currentPattern;
	private Queue<Token> _tokens;

	public Pattern Parse(List<Token> tokens)
	{
		_tokens = new Queue<Token>(tokens);
		_currentPattern = new Pattern();

		while (_tokens.Count > 0)
		{
			var token = _tokens.Dequeue();
			ProcessToken(token);
		}

		return _currentPattern;
	}

	private void ProcessToken(Token token)
	{
		switch (token.Type)
		{
			case TokenType.Number:
				if (_visitableStack.Count == 0)
				{
					// This is the pattern length
					_currentPattern.Length = int.Parse(token.Value);
				}
				else if (_visitableStack.Peek() is Pattern)
				{
					// This is a note start time
					var note = new Note();
					note.StartTime = int.Parse(token.Value);
					_visitableStack.Push(note);
				}
				break;

			case TokenType.Identifier:
				if (token.Value.Contains("_"))
				{
					// Pattern name with underscore
					var parts = token.Value.Split('_');
					if (parts.Length == 2 && int.TryParse(parts[0], out _))
					{
						_currentPattern.Name = parts[1];
					}
				}
				else if (_visitableStack.Peek() is Note)
				{
					// This is the note pitch
					var note = (Note)_visitableStack.Pop();
					note.Pitch = token.Value;
					_currentPattern.Notes.Add(note);
				}
				break;

			case TokenType.String:
				if (_visitableStack.Count == 0 || _visitableStack.Peek() is Pattern)
				{
					// This is a sample filename
					var sample = new Sample { FileName = token.Value };
					_currentPattern.Samples.Add(sample);
				}
				break;

			case TokenType.SampleKeyword:
				_visitableStack.Push(_currentPattern);
				break;

			case TokenType.NotesKeyword:
				// Just a marker, no action needed
				break;

			case TokenType.Hyphen:
				// Handle hyphen between note times
				if (_visitableStack.Peek() is Note note)
				{
					// Hyphen is processed when we get the end time
				}
				break;

			case TokenType.NewLine:
			case TokenType.EndOfFile:
				// Ignore
				break;
		}
	}
}
