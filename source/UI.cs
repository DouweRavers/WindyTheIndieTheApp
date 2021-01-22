using Godot;
using System;
using System.Collections.Generic;

public class UI : CanvasLayer
{
	//local variables
	String function, item, itemName;
	int increaseSpeed, increaseArrows, increaseDamage;
	float increaseShotSpeed;
	double arrowPrice, itemPrice;
	
	//children
	Popup shop;
	Control playview;
	AudioStreamPlayer click, buy, buynomoney;
	Timer clickTimer, messagetimer;
	Label 	treecount, moneycount, arrowcount, message, arrowPriceDisplay, 
			healthdisplay, damagedisplay, leveldisplay;
	TextureButton shopdisplay;
	ProgressBar levelprogress;
	TextureRect healthbar;

	//reference
	private Player player;
	private Map map;
	private Game game;
	
	public override void _Ready(){
		shop = (Popup) GetNode("Shop");
		playview = (Control) GetNode("PlayUI");
		click = (AudioStreamPlayer) GetNode("click");
		buy = (AudioStreamPlayer) GetNode("Shop/Buy");
		buynomoney = (AudioStreamPlayer) GetNode("Shop/BuyButNoMoney");
		clickTimer = (Timer) GetNode("clickTimer");
		messagetimer = (Timer) GetNode("Message/Timer");
		treecount = (Label) GetNode("TreeCount/Label");
		moneycount = (Label) GetNode("MoneyCount/Label");
		arrowcount = (Label) GetNode("ArrowCount/Label");
		message = (Label) GetNode("Message");
		healthdisplay = (Label) GetNode("Health/Label");
		damagedisplay = (Label) GetNode("Shop/DamageCount/Label");
		leveldisplay = (Label) GetNode("LevelProgress/Label");
		arrowPriceDisplay = (Label) GetNode("Shop/Background/ExtraArrows/ArrowPrice/Title/Price");
		shopdisplay = (TextureButton) GetNode("Shop/Background/ShopFrame/ShopDIsplay");
		healthbar = (TextureRect) GetNode("Health/slider");
		levelprogress = (ProgressBar) GetNode("LevelProgress");
		
		item = "sharpness";
		itemSelect();
	}
	
	float slowdown = 0;
	public override void _Process(float delta){		
		slowdown += delta;
		if(0.1 < slowdown){
			treecount.Text = ""+map.GetTreeCount();
			moneycount.Text = ""+Math.Round(player.GetMoney());
			arrowcount.Text = ""+player.arrows;
			arrowPriceDisplay.Text = ""+Math.Round(arrowPrice, 1);
			healthbar.RectScale = new Vector2((float) player.getHealthFactor(), (float) player.getHealthFactor());
			healthdisplay.Text = ""+player.Health+"\n"+player.maxHealth;
			damagedisplay.Text = ""+player.damage;
			leveldisplay.Text = ""+game.level;
			slowdown = 0;
		}
	}
	
	public void Init(	int incspeed, int incarrows, int incdamage, float incshotspeed, 
						double arrowprice ,Player p, Game g, Map m){
		increaseSpeed = incspeed;
		increaseArrows = incarrows;
		increaseDamage = incdamage;
		increaseShotSpeed = incshotspeed;
		player = p;
		game = g;
		map = m;
		arrowPrice = arrowprice;
	}
	public void setLevelProgression(double factor){
		levelprogress.Value = 100*factor;
	}
	
	public void announceMessage(String m, float time){
		message.Text = m;
		messagetimer.WaitTime = time;
		messagetimer.Start();
	}
	
	private void _On_Button(String func){
		click.Play();
		clickTimer.Start();
		function = func;
		preTimeout();
	}
	
	private void preTimeout(){
		switch(function){
			case "shop":
				player.canShoot++;
				break;
			case "add arrows":
				((Label) arrowPriceDisplay.GetParent()).Hide();
				break;
			case "sharpness":
				item = function;
				function = "select";
				break;
			default:
				break;
		}
	}
	private void _timeout()
	{
		switch(function){
			case "shop":
				Shop();
				player.canShoot--;
				break;
			case "add arrows":
				((Label) arrowPriceDisplay.GetParent()).Show();
				if(arrowPrice < player.money){
					if(player.increaseArrows(increaseArrows)){
						buy.Play();
						player.money -= arrowPrice;
					}
				}
				else
					buynomoney.Play();
				break;
			case "add arrowdeadliness":
				player.increaseDamage(increaseDamage);
				break;
			case "add speed":
				player.increaseSpeed(increaseSpeed);
				break;
			case "select":
				itemSelect();
				break;
			case "buy item":
				itemBuy();
				break;
			default:
				break;
		}
		function = null;
	}

	private void Shop(){
		if(shop.Visible){
			GetTree().Paused = false;
			shop.Hide();
			playview.Show();
		} else {
			GetTree().Paused = true;
			shop.Show();
			playview.Hide();
		}
	}
	
	private void itemSelect(){
		((TextureRect) shopdisplay.GetNode("BuyIcon")).Texture = 
			((TextureRect) GetNode("Shop/Background/ShopFrame/BowSelection/BowShop/"+item+"/button/icon")).Texture;
		switch(item){
			case "sharpness":
				itemName = "sharpness arrow";
				itemPrice = 20;
				break;
		}
		((Label) shopdisplay.GetNode("Name")).Text = itemName;
		((Label) shopdisplay.GetNode("Name/Price")).Text = ""+itemPrice;
	}
	
	private void itemBuy(){
		if(itemPrice < player.money){
			buy.Play();
			player.pay(itemPrice);
			switch(item){
				case "sharpness":
					player.increaseDamage(10);
					break;
			}
		} else {
			buynomoney.Play();
		}
	}
	
	private void _EndMessage()
	{
		message.Text = "";
	}
}
