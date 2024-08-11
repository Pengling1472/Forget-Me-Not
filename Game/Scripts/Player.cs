using Godot;
using System;

public partial class Player : CharacterBody2D {

	[ Export ] public float walkSpeed = 150;
	[ Export ] public float runSpeed = 300;

	private Game game;
	private AnimationTree animationTree;
	private RayCast2D ray;
	private AnimationNodeStateMachinePlayback stateMachine;
	private Vector2 inputDirection = Vector2.Zero;
	private Vector2 targetPosition = Vector2.Zero;
	private bool running = false;
	private bool moving = false;
	private bool walkPosition = false;
	private bool movingVerticle = false;
	private bool movingHorizontal = false;
	private float tileSize = 32;

	public override void _Ready() {
		
		game = GetTree().Root.GetNode<Game>( "CanvasLayer/Game" );
		animationTree = GetNode<AnimationTree>( "AnimationTree" );
		ray = GetNode<RayCast2D>( "RayCast2D" );
		stateMachine = ( AnimationNodeStateMachinePlayback )animationTree.Get( "parameters/playback" );

	}

	public override void _PhysicsProcess( double delta ) {

		running = Input.IsActionPressed( "Run" );
	
		if ( !moving ) {

			movingVerticle = ( Input.IsActionPressed( "Up" ) || Input.IsActionPressed( "Down" ) ) && !movingHorizontal ? true : false;
			movingHorizontal = ( Input.IsActionPressed( "Right" ) || Input.IsActionPressed( "Left" ) ) && !movingVerticle ? true : false;

			inputDirection = GetTree().Paused ? Vector2.Zero : new Vector2(
				Input.GetActionStrength( "Right" ) - Input.GetActionStrength( "Left" ),
				Input.GetActionStrength( "Down" ) - Input.GetActionStrength( "Up" )
			);

			if ( inputDirection != Vector2.Zero ) {

				if ( inputDirection.X != 0 && movingVerticle ) { inputDirection.Y = 0; } else if ( inputDirection.Y != 0 && movingHorizontal ) inputDirection.X = 0;

				animationTree.Set( "parameters/Idle/blend_position", inputDirection );
				animationTree.Set( "parameters/Walk/blend_position", inputDirection );
				animationTree.Set( "parameters/Run/blend_position", inputDirection );

				targetPosition = Position + ( inputDirection * tileSize );
				moving = true;

			} else { stateMachine.Travel( "Idle" ); }

		}
		
		if ( inputDirection != Vector2.Zero ) {

			stateMachine.Travel( running ? "Run" : "Walk" );

			ray.TargetPosition = inputDirection * tileSize / 2;
			ray.ForceRaycastUpdate();

			if ( !ray.IsColliding() ) {

				Velocity = ( running ? runSpeed : walkSpeed ) * inputDirection * ( float )delta;

				var distance = Position.DistanceTo( targetPosition );

				if ( distance < Velocity.Length() ) {

					Velocity = inputDirection * distance;

					walkPosition = !walkPosition;
					moving = false;

				}

				MoveAndCollide( Velocity );

			} else { moving = false; }

		} else stateMachine.Travel( "Idle" );

	}

	public override void _UnhandledKeyInput( InputEvent @event ) {

		switch ( true ) {

			case true when GetTree().Root.GetNode<AnimationPlayer>( "CanvasLayer/Game/Loading/LoadingAnimation" ).CurrentAnimation != "": break;

			case true when @event.IsActionPressed( "Menu" ) && !GetTree().Paused: game.GetNode<Settings>( "PopupScenes/Settings" ).open( null ); break;

			case true when @event.IsActionPressed( "Confirm" ) && ray.IsColliding():

				var rayCollider = ray.GetCollider();

				switch ( true ) {

					case true when ( ( Node ) rayCollider ).IsInGroup( "Interactable" ):

						( ( NPC ) rayCollider ).changeDirection( ray.TargetPosition / ( tileSize / 2 ) );

						( ( NPC ) rayCollider ).onInteract();

						break;

					case true when ( ( Node ) rayCollider ).IsInGroup( "Checkpoint" ):

						

						break;

				}
				
				break;

		}

		GC.Collect();

	}

}