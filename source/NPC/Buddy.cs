using Godot;
using System;

public class Buddy : KinematicBody2D
{
	/*
	public int Health = 50;
	int Damage = 20;
	FireStarter target;
	bool toggler = true;
	AnimatedSprite anim;
	
	public override void _Ready(){
		anim = ((AnimatedSprite) GetNode("AnimatedSprite"));
	}
	
	public void Tick(){
		toggler = true;
	}
	
	public override void _PhysicsProcess(float delta)
	{
		if(!FireStarter.paused){
			if(target != null && 0 < target.Health){
				KinematicCollision2D coll = MoveAndCollide(Position.DirectionTo(target.Position).Normalized()*2);
				if(coll != null){
					anim.Animation = "Attack";
					if(coll.Collider is FireStarter){
						if(((FireStarter) coll.Collider).Hit(Damage)){
							target = null;
						}
					}
				} else {
					anim.Animation = "Walking";
				}
			} else {
				anim.Animation = "Idle";
				FireStarter closestFoe = null;
				int closest = 10000;
				foreach(FireStarter firestarter in ((Game) GetNode("/root/Main/Game")).firestarters){
					if(0 < firestarter.Health && firestarter.Position.DistanceTo(Position) < closest){
						closestFoe = firestarter;
						closest = (int) firestarter.Position.DistanceTo(Position);
					}
				}
				target = closestFoe;
			}
			if(Health < 0){
				QueueFree();
			}
		}
	}
	
	public bool Hit(int value){
		if(toggler){
			toggler = false;
			Health -= value;
		}
		return Health < 0;
	}*/
}
