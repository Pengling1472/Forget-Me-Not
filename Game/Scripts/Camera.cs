using Godot;
using System;

public partial class Camera : Camera2D {

    [ Export ] public CharacterBody2D player;

    public override void _PhysicsProcess( double delta ) {
    
        GlobalPosition = player.GlobalPosition;

        ForceUpdateScroll();

    }

}