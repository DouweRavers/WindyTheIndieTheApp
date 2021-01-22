using Godot;
using System;

public class Player : KinematicBody2D
{
	//control
	public int canMove, canShoot;
	public double money;
	
	//stats
	public int speed, arrows, damage;
	private float shotSpeed;
	
	//local variables
	Vector2 walkDir, lastTouch;
	bool aim, recharged, touching;
	int maxSpeed, maxArrows, maxDamage;
	float maxShotSpeed;
	public double maxHealth, Health;
	
	//references
	Game game;

	//children
	AnimatedSprite graphics, bow;
	AudioStreamPlayer bowsound, walkingsound;
	Timer recharger, draw;
	PackedScene arrow;

	public override void _Ready(){
		graphics = (AnimatedSprite) GetNode("Graphics");
		bow = (AnimatedSprite) GetNode("Bow");
		bowsound = (AudioStreamPlayer) GetNode("bowsound");
		walkingsound = (AudioStreamPlayer) GetNode("walkingsound");
		recharger = (Timer) GetNode("Recharger");
		draw = (Timer) GetNode("draw");
		arrow = (PackedScene) ResourceLoader.Load("res://Prefab/Objects/Arrow.tscn");
		walkDir = new Vector2(0,0);
		lastTouch = new Vector2(0,0);
		aim = false;
		recharged = true;
		canShoot = 0;
		canMove = 0;
		touching = false;
	}

	public void Init(double myhealth, int myspeed, int myarrows, int mydamage, double mymoney, float myshotspeed, Game g){
		speed = myspeed;
		maxSpeed = 5*myspeed;
		arrows = myarrows;
		maxArrows = 10*myarrows;
		damage = mydamage;
		maxDamage = 25*mydamage;
		money = mymoney;
		shotSpeed = myshotspeed;
		maxShotSpeed = myshotspeed/10;
		recharger.WaitTime = myshotspeed;
		Health = maxHealth = myhealth;
		game = g;
	}

	public override void _PhysicsProcess(float delta)
	{
		GetInput();
		lastTouch = GetGlobalMousePosition();
		HandleAnimation();
		MoveAndCollide(speed * walkDir * delta);
		bow.LookAt(lastTouch);
	}
	
	public override void _Input(InputEvent @event){
		//pressed
		if (@event is InputEventScreenTouch){
			if(((InputEventScreenTouch) @event).Pressed)
				touching = true;
			else
				touching = false;
		}
	}
	
	public void Damage(double damage){
		Health -= damage;
		GD.Print(""+Health);
		if(Health <= 0){
			game.GameOver();
		}
	}
	
	public double getHealthFactor(){
		return Health/maxHealth;
	}

	public bool increaseSpeed(int increaseValue){
		bool allowed = true;
		if(speed + increaseValue < maxSpeed)
			speed += increaseValue;
		else
			allowed = false;
		return allowed;
	}
	
	public bool increaseArrows(int increaseValue){
		bool allowed = true;
		if(arrows + increaseValue < maxArrows)
			arrows += increaseValue;
		else
			allowed = false;
		return allowed;
	}
	
	public bool increaseDamage(int increaseValue){
		bool allowed = true;
		if(damage + increaseValue < maxDamage)
			damage += increaseValue;
		else
			allowed = false;
		return allowed;
	}
	
	public bool increaseShotSpeed(float factor){
		bool allowed = true;
		if(maxShotSpeed < factor * shotSpeed)
			shotSpeed *= factor;
		else
			allowed = false;
		return allowed;
	}
	
	public double GetMoney(){
		return money;
	}
	
	public bool pay(double amount){
		bool possible = true;
		if(money - amount < 0)
			possible = false;
		else
			money -= amount;
		return possible;
	}
	
	public void addMoney(double amount){
		money += amount;
	}
	
	private void _timeout()
	{
		recharged = true;
	}
	
	private void HandleAnimation(){
		if(walkDir.Length() != 0){
			graphics.Animation = "WalkingS";
			if(!walkingsound.Playing)
				walkingsound.Play();
		} else {
			graphics.Animation = "Idle";
			if(walkingsound.Playing)
				walkingsound.Stop();
		}
		if(aim && bow.Animation != "Draw"){
			bow.Animation = "Draw";
			bowsound.Play();
		} else if(!aim && bow.Animation != "Release") {
			bowsound.Stop();
			bow.Animation = "Release";
		} else if(canShoot != 0){
			bowsound.Stop();
			bow.Animation = "Idle";
		}
	}
	
	private void GetInput(){
		//movement
		walkDir = new Vector2(0, 0);
		if (Input.IsActionPressed("ui_right")){
			walkDir.x += 1;
		}

		if (Input.IsActionPressed("ui_left")){
			walkDir.x -= 1;
		}

		if (Input.IsActionPressed("ui_down")){
			walkDir.y += 1;
		}
		
		if (Input.IsActionPressed("ui_up")){
			walkDir.y -= 1;
		}
		if(walkDir.Length() != 0)
			canShoot++;
		//shooting
		if(touching && canShoot == 0){
			if(recharged && !aim){
				aim = true;
				recharged = false;
				draw.Start();
			}
		} else if(aim && canShoot == 0){
			aim = false;
			shootArrow();
		} else if(canShoot != 0){
			aim = false;
			recharged = true;
			draw.Stop();
		}
		if(walkDir.Length() != 0)
			canShoot--;

	}
	
	private void shootArrow(){
		Arrow FiredArrow = (Arrow) arrow.Instance();
		GetParent().AddChild(FiredArrow);
		FiredArrow.Position = Position+bow.Position;
		FiredArrow.LookAt(lastTouch);
		FiredArrow.Position += FiredArrow.Position.DirectionTo(lastTouch).Normalized()*10;
		int Damage = (int) (damage * (1.5-draw.TimeLeft/draw.WaitTime));
		FiredArrow.Shoot(FiredArrow.Position.DirectionTo(lastTouch), Damage); 
		arrows--;
	}
}



