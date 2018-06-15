extends Node2D

var timeout = false
var lightning = preload("res://NPCs/Lightning.tscn")


func _ready():
	$Sprite/AnimationPlayer.current_animation = "float"


func _physics_process(delta):
	var body = $Sprite/RayCast2D.get_collider()
	if $Sprite/RayCast2D.is_colliding():
		fire()


func fire():
	if not timeout:
		$Sprite/RayCast2D.add_child(lightning.instance(), true)
		$Timer.start()
		timeout = true


func _on_Timer_timeout():
	timeout = false