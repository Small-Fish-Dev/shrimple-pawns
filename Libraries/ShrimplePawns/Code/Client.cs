namespace ShrimplePawns;

/// <summary>
/// The base client that you should inherit off of.
/// </summary>
public abstract class Client : Component
{
	[Sync( SyncFlags.FromHost )]
	public System.Guid ConnectionId { get; private set; }

	[Sync( SyncFlags.FromHost )]
	protected Pawn Pawn { get; set; }

	public Connection Connection => Connection.Find( ConnectionId );

	/// <summary>
	/// Get the pawn only if it is valid.
	/// </summary>
	public bool TryGetPawn<T>( out T pawn ) where T : Pawn
	{
		pawn = GetPawn<T>();
		return pawn.IsValid();
	}

	/// <summary>
	/// Get the pawn.
	/// </summary>
	public T GetPawn<T>() where T : Pawn
	{
		return Pawn as T;
	}

	/// <summary>
	/// Get the pawn.
	/// </summary>
	public Pawn GetPawn()
	{
		return Pawn;
	}

	/// <summary>
	/// Assigns the given connection as the client's current connection.
	/// This connection will be used to give ownership to any future assigned pawns.
	/// This must be called on the host!
	/// </summary>
	/// <param name="connection">The connection to assign to the client.</param>
	public void AssignConnection( Connection connection )
	{
		if ( !Connection.Local.IsHost )
		{
			Log.Warning( "Failed to call AssignConnection(...) due to being invoked on non-host client!" );
			return;
		}

		if ( connection == null )
		{
			Log.Warning( "Failed to call AssignConnection(...) due to null connection param!" );
			return;
		}

		ConnectionId = connection.Id;
		GameObject.Name = $"{connection.DisplayName} - CLIENT";
	}

	/// <summary>
	/// Creates a <see cref="GameObject" /> from the given prefab file and assigns it as the client's current pawn.
	/// This must be called on the host!
	/// </summary>
	/// <param name="prefabFile">The prefab file that is used to create the pawn.</param>
	public Pawn AssignPawn( PrefabFile prefabFile )
	{
		var obj = SceneUtility.GetPrefabScene( prefabFile ).Clone();
		return InternalAssign( obj );
	}

	/// <summary>
	/// Assigns the given pawn type as the client's current pawn.
	/// The pawn type must have a <see cref="PawnAttribute" /> defined in order to use this method.
	/// This must be called on the host!
	/// </summary>
	/// <returns>The pawn component that the client was assigned to.</returns>
	public T AssignPawn<T>() where T : Pawn
	{
		var pawnAttribute = TypeLibrary.GetType<T>().GetAttribute<PawnAttribute>();

		var path = pawnAttribute?.PrefabPath;
		if ( string.IsNullOrEmpty( path ) )
		{
			Log.Warning( $"{typeof( T )} had no PawnAttribute prefab assigned to it." );
			return null;
		}

		var obj = SceneUtility.GetPrefabScene( ResourceLibrary.Get<PrefabFile>( path ) ).Clone();
		return InternalAssign<T>( obj ); ;
	}

	/// <summary>
	/// Takes a <see cref="GameObject" /> that already exists, networked spawns it, and assigns it to the client.
	/// This must be called on the host!
	/// </summary>
	/// <param name="obj">The pawn game object that will be networked spawned and assigned to the client.</param>
	public Pawn AssignPawn( GameObject obj )
	{
		return InternalAssign( obj );
	}

	private T InternalAssign<T>( GameObject obj ) where T : Pawn
	{
		return (T)InternalAssign( obj );
	}

	private Pawn InternalAssign( GameObject obj )
	{
		if ( !Connection.Local?.IsHost ?? false )
		{
			obj.Destroy();
			Log.Warning( "Attempted to call AssignPawn(...) on non-host client!" );
			return null;
		}

		var pawn = obj.Components.Get<Pawn>();
		if ( !pawn.IsValid() )
		{
			obj.Destroy();
			Log.Warning( $"Assigned GameObject ({obj.Name}) with no pawn component!" );
			return null;
		}

		if ( Pawn.IsValid() )
			Pawn.OnUnassign();

		var assignedConnection = Connection ?? Connection.Host;

		if ( !obj.Network.Active )
			obj.NetworkSpawn( assignedConnection );
		else
			obj.Network.AssignOwnership( assignedConnection );

		obj.Name = $"{assignedConnection.DisplayName} - PAWN";

		Pawn = pawn;
		Pawn.OnAssign( this );

		return pawn;
	}
}
