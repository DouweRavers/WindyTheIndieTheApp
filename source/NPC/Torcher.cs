using Godot;
using System;

public class Torcher : KinematicBody2D
{
	//control
	
	//local variables
	double Health, maxHealth, worth;
	Tree target;
	int speed = 20;
	Random rand;
	
	//references
	Map map;
	Game game;
	Player player;
	
	//childresn
	AnimatedSprite animation;
	ProgressBar healthdisplay;
	
	public override void _Ready(){
		animation = (AnimatedSprite) GetNode("AnimatedSprite");
		healthdisplay = (ProgressBar) GetNode("Health");
		target = null;
		rand = new Random();
	}
	
	public void Init(double h, Map m, Game g, Player p){
		map = m;
		game = g;
		player = p;
		Health = maxHealth = worth = h;
		worth /= 30;
	}
	
	public override void _PhysicsProcess(float delta){
		if(target == null){
			animation.Animation = "Idle";
			target = map.getClosestTree(Position);
		} else {
			KinematicCollision2D collide = MoveAndCollide(speed * (Position.DirectionTo(target.Position).Normalized() + 0.5f * new Vector2(rand.Next(10)-5, rand.Next(10)-5).Normalized()) * delta);
			if(collide != null && collide.Collider is Tree){
				animation.Animation = "Idle";
				Tree tree = (Tree) collide.Collider;
				tree.setFire();
				target = null;
			} else {
				animation.Animation = "Walking";
			}
		}
	}
	
	public void damage(double damage){
		Health -= damage;
		healthdisplay.Value = 100 * Health/maxHealth;
		if(Health < 0){
			game.removeTorcher(this);
			player.addMoney(worth);
		}
	}
}
