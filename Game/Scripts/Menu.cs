using Godot;
using System;

public partial class Menu : Node {

    [ Export ] public bool blackBackground = true;

    private bool disclaimer = true;
    private Game game;

    public override void _Ready() {

        game = GetTree().Root.GetNode<Game>( "CanvasLayer/Game" );

        // GetNode<Button>( "MarginContainer/GridContainer/Start" ).Text = "New Game";
        GetNode<Button>( "MarginContainer/GridContainer/Reset" ).Visible = false;
        GetNode<AnimationPlayer>( "Disclaimer/AnimationPlayer" ).Play( "Start" );

        foreach ( Button button in GetNode<GridContainer>( "MarginContainer/GridContainer" ).GetChildren() ) {

            button.Pressed += () => OnButtonPressed( button );
            button.FocusEntered += () => buttonFocus( true, button );
            button.FocusExited += () => buttonFocus( false, button );

        }

    }
    
    public override void _UnhandledKeyInput( InputEvent @event ) {
    
        if ( @event.IsActionPressed( "Confirm" ) && disclaimer &&!blackBackground ) {

            disclaimer = false;
            GetNode<AnimationPlayer>( "Disclaimer/AnimationPlayer" ).Seek( 8, true );

        }

        GC.Collect();
    
    }

    private void OnAnimationPlayerAnimationFinished( String animationName ) { 
        
        if ( animationName == "Start" ) disclaimer = false; GetNode<Button>( "MarginContainer/GridContainer/Start" ).GrabFocus();
        
    }

    private void OnButtonPressed( Button button ) {

        switch ( button.Name ) {

            case "Start": game.changeScene( "World_01" ); GetNode<Button>( "MarginContainer/GridContainer/Start" ).ReleaseFocus(); break;

            case "Settings": game.GetNode<Settings>( "PopupScenes/Settings" ).open( button ); break;

            case "Quit": GetTree().Quit(); break;

        }

    }

    private void buttonFocus( bool focus, Button button ) {

        button.GetNode<RichTextLabel>( "RichTextLabel" ).Modulate = Color.Color8( 255, 255, 255, ( byte )( focus ? 255 : 0 ) );
        button.SelfModulate = Color.Color8( 255, 255, 255, ( byte )( focus ? 0 : 255 ) );

    }

}