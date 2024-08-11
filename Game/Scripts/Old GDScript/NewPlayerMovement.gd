extends CharacterBody2D

@export var moveSpeed : float = 100
@export var runSpeed : float = 200

@onready var animationTree = $AnimationTree
@onready var ray = $RayCast2D
@onready var stateMachine = animationTree.get( "parameters/playback" )

var inputDirection = Vector2.ZERO
var targetPosition = Vector2.ZERO
var tileSize = 16
var moving = false
var running = false

func _physics_process( delta ):
	
	if not moving:
		
		if inputDirection.y == 0: inputDirection.x = int( Input.is_action_pressed( "Right" ) ) - int( Input.is_action_pressed( "Left" ) )
		if inputDirection.x == 0: inputDirection.y = int( Input.is_action_pressed( "Down" ) ) - int( Input.is_action_pressed( "Up" ) )
		
		if inputDirection != Vector2.ZERO:
			
			animationTree.set( "parameters/Idle/blend_position", inputDirection )
			animationTree.set( "parameters/Walk/blend_position", inputDirection )
			
			targetPosition = position + ( inputDirection * tileSize )
			moving = true
		
		else: stateMachine.travel( "Idle" )

	if inputDirection != Vector2.ZERO:
		
		stateMachine.travel( "Walk" )
		
		ray.target_position = inputDirection * tileSize / 2
		ray.force_raycast_update()
		
		if !ray.is_colliding():
		
			velocity = ( runSpeed if running else moveSpeed ) * inputDirection * delta
			
			var distance = position.distance_to( targetPosition )
			
			if distance < velocity.length():
				
				velocity = inputDirection * distance
				moving = false
			
			move_and_collide( velocity )
		
		else: moving = false
	
	else: stateMachine.travel( "Idle" )

func _unhandled_key_input( event ):
	
	match event.keycode:
		
		69:
			if ray.is_colliding() and event.pressed:
				
				var object = ray.get_collider()
				
				if object.is_in_group( "interactable" ): print( "Henlo" )
	
		4194325: running = event.pressed
