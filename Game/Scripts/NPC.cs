using Godot;
using System;

public partial class NPC : CharacterBody2D {

    [ Export ] public Godot.Collections.Array<Vector2> movement;
    [ Export ] public Json json;
    
    private Sprite2D sprite;
    private bool moving = false;

    public override void _Ready() {
    
        sprite = GetNode<Sprite2D>( "Character" );

        if ( movement == null ) { SetPhysicsProcess( false ); } else if ( movement.Count == 0 ) SetPhysicsProcess( false ); 
    
    }

    public void changeDirection( Vector2 newDirection ) {

        if ( newDirection == Vector2.Right ) {

            sprite.FlipH = true;
            sprite.FrameCoords = new Vector2I( 1, sprite.FrameCoords.Y );

            return;

        }

        sprite.FlipH = false;
        sprite.FrameCoords = new Vector2I( newDirection == Vector2.Up ? 0 : newDirection == Vector2.Left ? 1 : 2, sprite.FrameCoords.Y );  

    }

    public void onInteract() {

        ( ( Dialog ) GetTree().Root.GetNode( "CanvasLayer/Game/PopupScenes/DialogBox" ) ).open( Godot.Json.ParseString( FileAccess.GetFileAsString( json.ResourcePath ) ), json.ResourcePath.GetBaseName().TrimPrefix( "res://Dialogues/" ) );

    }

}
