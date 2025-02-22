# Shrimple Pawns

> _"no we won't allow pawn games"_ -garry

s&box library that provides support for the classic entity Pawn system. This is useful in cases where the player can take control of different controllers. For example, an FPS game where you have the regular player state but also a spectating state.

## Example

```csharp
// Inherit the pawn component.
//
// Make sure to provide the PawnAttribute with a path to a prefab
// if you decide to use the template version of AssignPawn.
[Pawn( "spectate.prefab" )]
public sealed class SpectatePawn : Pawn
{

}
```

```csharp
// All of the following code should be called on the host! Since we use [HostSync] internally.
//
// Setup and assign the client a connection. Make sure this clientObj is already networked.
var client = clientObj.Components.Get<Client>();
client.AssignConnection( channel );

// Give the client a pawn via one of the methods below.

// Option #1
// The pawn component MUST have PawnAttribute.
client.AssignPawn<SpectatePawn>();

// Option #2
// The pawn component does NOT need PawnAttribute in this case.
client.AssignPawn(SpectatePrefab);

// Option #3
// The gameobject already exists and you want to assign it
var obj = SceneUtility.GetPrefabScene( SpectatePrefab ).Clone();
client.AssignPawn(obj);
```

[A full example can be viewed here.](https://github.com/Small-Fish-Dev/shrimple_pawns/tree/main/code/Example)
