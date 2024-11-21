namespace ShrimplePawns;

/// <summary>
/// The base client that you should inherit off of.
/// </summary>
public abstract class Client : Component
{
	[HostSync]
	public System.Guid ConnectionId { get; private set; }

	[HostSync]
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
		ConnectionId = connection.Id;
		GameObject.Name = $"{Connection.DisplayName} - CLIENT";
	}

	/// <summary>
	/// Creates a <see cref="GameObject" /> from the given prefab file and assigns it as the client's current pawn.
	/// This must be called on the host!
	/// </summary>
	/// <param name="prefabFile">The prefab file that is used to create the pawn.</param>
	public void AssignPawn( PrefabFile prefabFile )
	{
		var obj = SceneUtility.GetPrefabScene( prefabFile ).Clone();
		InternalAssign( obj );
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

	private T InternalAssign<T>( GameObject obj ) where T : Pawn
	{
		return (T)InternalAssign( obj );
	}

	private Pawn InternalAssign( GameObject obj )
	{
		if ( !Connection.Local?.IsHost ?? false )
		{
			Log.Warning( "Attempting to assign pawn on non-host client!" );
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

		if ( Connection is null )
			Log.Warning( $"Client does not have a connection! Assigning to host." );

		var assignedConnection = Connection ?? Connection.Host;
		obj.NetworkSpawn( assignedConnection );
		obj.Name = $"{assignedConnection.DisplayName} - {obj.Name.ToUpper()} Pawn";

		Pawn = pawn;
		Pawn.OnAssign( this );

		return pawn;
	}
}
