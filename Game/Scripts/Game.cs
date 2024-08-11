using Godot;
using System;

public partial class Game : Control {

    private Vector2 viewportSize;
    private Node currentScene;

    public override void _Ready() {

        DisplayServer.MouseSetMode( DisplayServer.MouseMode.Hidden );
        currentScene = GetNode<Node>( "Scenes/SubViewport/Menu" );

        AudioServer.SetBusVolumeDb( AudioServer.GetBusIndex( "Music" ), ( 0.3f * ( ( int ) ( ( Godot.Collections.Dictionary ) ( ( Godot.Collections.Dictionary ) GetNode<Global>( "/root/Global" ).Settings )[ "audio" ] )[ "music" ] ) ) - 30 );
        AudioServer.SetBusVolumeDb( AudioServer.GetBusIndex( "Sound" ), ( 0.3f * ( ( int ) ( ( Godot.Collections.Dictionary ) ( ( Godot.Collections.Dictionary ) GetNode<Global>( "/root/Global" ).Settings )[ "audio" ] )[ "sound" ] ) ) - 30 );

    }

    public async void changeScene( String sceneName ) {

        var loadingAnimation = GetNode<AnimationPlayer>( "Loading/LoadingAnimation" );
        var iconAnimation = GetNode<AnimationPlayer>( "Loading/IconAnimation" );

        loadingAnimation.Play( "FadeIn" );
        iconAnimation.Play( "Animation" );

        await ToSignal( loadingAnimation, "animation_finished" );

        GetNode<TextureRect>( "Border" ).Texture = ResourceLoader.Load( "res://assets/Borders/spring.png" ) as Texture2D;

        ResourceLoader.LoadThreadedRequest( $"res://Scenes/{ sceneName }.tscn" );

        while ( true ) {

            var sceneLoadStatus = ResourceLoader.LoadThreadedGetStatus( $"res://Scenes/{ sceneName }.tscn" );

            if ( sceneLoadStatus == ResourceLoader.ThreadLoadStatus.Loaded ) break;

        }

        loadingAnimation.Play( "FadeOut" );
        iconAnimation.Stop();

        var newScene = ( ResourceLoader.LoadThreadedGet( $"res://Scenes/{ sceneName }.tscn" ) as PackedScene ).Instantiate();

        GetNode<SubViewport>( "Scenes/SubViewport" ).AddChild( newScene );
        GetNode<SubViewport>( "Scenes/SubViewport" ).MoveChild( newScene, 0 );

        currentScene.QueueFree();
        currentScene = newScene;

    }

}