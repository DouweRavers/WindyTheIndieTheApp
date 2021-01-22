using Godot;
using System;

public class Arrow : KinematicBody2D
{
	Vector2 ShootDir;
	int Damage;
	
	public void Shoot(Vector2 dir, int damage){
		ShootDir = dir;
		Damage = damage;
	}
	
	public override void _PhysicsProcess(float delta)
	{
		if(ShootDir != null){
			KinematicCollision2D coll = MoveAndCollide(ShootDir * 10);
			if(coll != null){
				if(coll.Collider is Torcher){
					((Torcher) coll.Collider).damage(Damage);
					QueueFree();
				} else if(coll.Collider is Cutter){
					((Cutter) coll.Collider).Damage(Damage);
					QueueFree();
				} else if(coll.Collider is Buddy){
					//((Buddy) coll.Collider).Health -= Damage;
					QueueFree();
				}
			}
		}
	}
	
	private void _on_Timer_timeout()
	{
		QueueFree();
	}
}


