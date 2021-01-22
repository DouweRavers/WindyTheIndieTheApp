using Godot;
using System;

public class Tree : StaticBody2D
{
	//control
	public int x, y;
	public bool burning = false;
	
	//local variables	
	private double Health, burnDamage;
	private string phase;
	private float shakevalue;
	
	//children
	Timer grow, messageTimer, buttontimer;
	AnimatedSprite animation;
	Label message;
	TouchScreenButton endFire;
	AudioStreamPlayer2D fire;
	//references
	Player player;
	Map map;
	
	public override void _Ready()
	{
		Random rand = new Random();
		grow = (Timer) GetNode("GrowTimer");
		messageTimer = (Timer) GetNode("messageTimer");
		buttontimer = (Timer) GetNode("endFire/Timer");
		grow.WaitTime = rand.Next(5, 40);
		grow.Start();
		animation = (AnimatedSprite) GetNode("AnimatedSprite");
		message = (Label) GetNode("messages");
		endFire = (TouchScreenButton) GetNode("endFire");
		phase = animation.Animation;
		fire = (AudioStreamPlayer2D) GetNode("Fire");
	}
	
	float deltasum = 0;
	public override void _Process(float delta){
		deltasum += delta;
		if(0 < shakevalue)
			shakevalue -= 2*delta;
		float fl = (float) Math.Sin(deltasum*1000);
		Position += new Vector2(shakevalue*fl, 0);
	}
	
	public void Init(int mapx, int mapy, double Hitpoints, Map m, Player p){
		x = mapx;
		y = mapy;
		map = m;
		player = p;
		Health = Hitpoints;
		burnDamage = Hitpoints/10;
	}
	
	public void OverrideGrowPhase(String p){
		animation.Animation = p;
		phase = p;
	}
	
	public void setFire(){
		burning = true;
		endFire.Visible = true;
		animation.Animation = "Fire";
		grow.WaitTime = 1f;
		grow.Start();
	}
	
	public void Damage(double damage){
		Health -= damage;
		shakevalue = 1f;
		if(Health < 0){
			map.removeTree(this);
		}
	}
	
	private void _Grow()
	{
		Random rand = new Random();
		messageTimer.Start();
		switch(animation.Animation){
			case "Sapling":
				message.Set("custom_color/font_color", new Color(0,1,0));
				animation.Animation = phase = "Young";
				Health *= 1.5;
				grow.WaitTime = (float) rand.Next(10, 60);
				grow.Start();
				break;
			case "Young":
				animation.Animation = phase = "Grown";
				Health *= 1.5;
				grow.WaitTime = (float) rand.Next(20, 100);
				grow.Start();
				break;
			case "Grown":
				player.addMoney(0.5);
				grow.WaitTime = (float) rand.Next(1, 20);
				map.PlaceTree(x+rand.Next(5)-rand.Next(5), y+rand.Next(5)-rand.Next(5));
				grow.Start();
				break;
			case "Fire":
				if(!fire.Playing){
					fire.Play();
				}
				message.Text = "- "+burnDamage;
				Damage(burnDamage);
				grow.WaitTime = 1f;
				grow.Start();
				break;
			default:
				break;
		}
	}
	
	private void _messageTimer()
	{
		message.Text = "";
	}
	
	private void _on_endFire()
	{
		player.canShoot++;
		buttontimer.Start();
		burning = false;
		animation.Animation = phase;
		grow.WaitTime = 1f;
		grow.Start();
		endFire.Visible = false;
	}
	
	private void _on_Timer_timeout()
	{
		player.canShoot--;
	}
}








