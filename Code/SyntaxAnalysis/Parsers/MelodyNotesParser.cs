using LexicalAnalysis;
using Runtime;
using System.Globalization;

namespace SyntaxAnalysis.Parsers;

public static class MelodyNotesParser
{
	public static void Parse(SyntaxAnalyzer a, Melody melody)
	{
		a.ConsumeToken(TokenType.NotesKeyword);

		ParseChords(a, melody);
	}

	private static void ParseChords(SyntaxAnalyzer a, Melody melody)
	{
		while (a.TryConsumeIndents(2))
		{
			float startBeat = default;
			float endBeat = default;

			a.ConsumeToken(TokenType.Float, () =>
			{
				startBeat = float.Parse(a.CursorToken().Value, CultureInfo.InvariantCulture);
			});
			a.ConsumeToken(TokenType.Float, () =>
			{
				endBeat = float.Parse(a.CursorToken().Value, CultureInfo.InvariantCulture);
			});

			while (a.CursorToken().Type == TokenType.Identifier)
			{
				Note chordNote = new(melody)
				{
					StartBeat = startBeat,
					EndBeat = endBeat,
				};

				a.ConsumeToken(TokenType.Identifier, () =>
				{
					chordNote.ThePitch = new Pitch(a.CursorToken().Value);
				});


				NoteModifiersParser.TryParse(a, chordNote);

				melody.Notes.Add(chordNote);
			}
		}
	}
}