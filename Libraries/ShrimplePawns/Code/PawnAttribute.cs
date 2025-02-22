using System;

namespace ShrimplePawns;

/// <summary>
/// This attribute should be given to any class that derives Pawn.cs
/// </summary>
/// <param name="prefabPath">The path to the prefab that contains the derived pawn component.</param>
[AttributeUsage( AttributeTargets.Class )]
public class PawnAttribute( string prefabPath ) : Attribute
{
	public string PrefabPath { get; private set; } = prefabPath;
}
