using Godot;
using System;

public class Cutter : KinematicBody2D
{
	//control
	
	//local variables
	double Health, maxHealth, worth, damage;
	Node2D target;
	int speed = 40;
	bool recharged = true;
	Random rand;
	
	//references
	Map map;
	Game game;
	Player player;
	
	//childresn
	AnimatedSprite animation;
	ProgressBar healthdisplay;
	Timer recharger;
	
	public override void _Ready(){
		animation = (AnimatedSprite) GetNode("AnimatedSprite");
		healthdisplay = (ProgressBar) GetNode("Health");
		recharger = (Timer) GetNode("Timer");
		target = null;
		rand = new Random();
	}
	
	public void Init(double h, Map m, Game g, Player p){
		map = m;
		game = g;
		player = p;
		Health = maxHealth = worth = damage = h;
		worth /= 20;
		damage /= 2;
	}
	
	public override void _PhysicsProcess(float delta){
		if(target == null){
			animation.Animation = "Idle";
			if(map.getClosestTree(Position).Position.DistanceTo(Position) <= player.Position.DistanceTo(Position)){
				target = (Node2D) map.getClosestTree(Position);
			} else {
				target = (Node2D) player;
			}
		} else {
			KinematicCollision2D collide = MoveAndCollide(speed * (Position.DirectionTo(target.Position).Normalized() + 0.5f * new Vector2(rand.Next(10)-5, rand.Next(10)-5).Normalized()) * delta);
			if(collide != null){
				if(collide.Collider is Tree){
					animation.Animation = "Chopping";
					Tree tree = (Tree) collide.Collider;
					if(recharged){
						tree.Damage(damage);
						recharged = false;
						recharger.Start();
						target = null;
					}

				} if(collide.Collider is Player){
					animation.Animation = "Chopping";
					if(recharged){
						player.Damage(damage);
						recharged = false;
						recharger.Start();
						target = null;
					}
				}
			} else {
				animation.Animation = "Walking";
			}
			if(target is Tree && ((Tree) target).burning){
				target = null;
			}
		}
	}
	
	public void Damage(double d){
		Health -= d;
		healthdisplay.Value = 100 * Health/maxHealth;
		if(Health < 0){
			game.removeCutter(this);
			player.addMoney(worth);
		}
	}
	
	private void _on_Timer_timeout()
	{
		recharged = true;
	}
}



