extends Control

@onready var viewport_size = get_viewport().get_visible_rect().size
@onready var currentScene = $SubViewportContainer/SubViewport/Scene

func _ready():
	
	currentScene.connect( "changeScene", handleSceneChange )
	
	self.position = viewport_size / 2
	
	$TextureRect.size = viewport_size
	$TextureRect.position = - viewport_size / 2
	
	get_tree().get_root().connect( "size_changed", _on_viewport_size_changed )

func _on_viewport_size_changed():
	
	viewport_size = get_viewport().get_visible_rect().size
	
	self.position = viewport_size / 2
	
	$TextureRect.size = viewport_size
	$TextureRect.position = - viewport_size / 2

func handleSceneChange( sceneName ):
	
	var newScene = load( "res://Scenes/" + sceneName + ".tscn" ).instantiate()
	
	$SubViewportContainer/SubViewport.add_child( newScene )
	
	currentScene.queue_free()
	currentScene = newScene
	newScene.connect( "changeScene", handleSceneChange )
