using Godot;
using System;
using System.Collections.Generic;

public class Game : Node2D
{
	//control
	
	//local variables
	double torcherhealth = 30;
	List<Torcher> torchers;
	List<Cutter> cutters;
	
	GamePhases gamephase = GamePhases.START;
	int startTime = 0;
	Random random;
	public int level, leveltorchers, levelcutters;
	
	//references
	PackedScene torcherresource, cutterresource;
	
	//children
	Map map;
	Player player;
	UI ui;
	
	public enum GamePhases{
		START, LEVEL, RECESS
	}

	public override void _Ready(){
		//torcher setup
		torcherresource = (PackedScene) ResourceLoader.Load("res://Prefab/NPC/Torcher.tscn");
		torchers = new List<Torcher>();
		
		//cutter setup
		cutterresource = (PackedScene) ResourceLoader.Load("res://Prefab/NPC/Cutter.tscn");
		cutters = new List<Cutter>();
		
		random = new Random();
	}
	
	public void Init(int size, Map m, Player p, UI u){
		map = m;
		player = p;
		ui = u;
		
		setWorldSize(size);
		player.Position = map.MapToWorld(new Vector2(size/2, size/2));
		map.PlaceTree(random.Next(size/4, size/2), 1+random.Next(size-1), "Grown");
		map.PlaceTree(random.Next(size/2, size*3/4), 1+random.Next(size-1), "Young");
	}
	
	public void Tick(int time){
		int reltime = time - startTime;
		switch(gamephase){
			case GamePhases.START:
				if(time == 0 ) {
					ui.announceMessage("Good luck!!!", 1);
				}
				if(time == 1) {
					gamephase = GamePhases.LEVEL;
					level = 1;
					leveltorchers = 5;
					levelcutters = 0;
					startTime = time+1;
				}
				break;
			case GamePhases.LEVEL:
				if( reltime == 0 ) {
					ui.announceMessage("Level "+level, 2);
				} else if( reltime == 2 ){
					ui.announceMessage("map size "+map.size, 2);
				}
				//spawn torchers
				if(0 < leveltorchers && torchers.Count <= level){
					leveltorchers--;
					spawnTorcher();
				}
				//spawn cutters
				else if(0 < levelcutters && cutters.Count <= level/2){
					levelcutters--;
					spawnCutter();
				}
				else if(leveltorchers == 0 && torchers.Count == 0 &&
						levelcutters == 0 && cutters.Count == 0
				){
					gamephase = GamePhases.RECESS;
					startTime = time+1;
				}
				double progress = (((double) level*3-leveltorchers-torchers.Count)+((double) level-levelcutters-cutters.Count))/(level*4);
				ui.setLevelProgression(progress);
				break;
			case GamePhases.RECESS:
				if( reltime == 0 ) {
					ui.announceMessage("Level "+level+" finished\nGood job!", 3);
				}
				if(reltime == 5) {
					level++;
					if(level%3 == 0){
						setWorldSize((int) Math.Round(map.size*1.2, 0));
					}
					leveltorchers = 3*level;
					levelcutters = level;
					torcherhealth += 0.3*torcherhealth;
					ui.setLevelProgression(0);
					startTime = time+1;
					gamephase = GamePhases.LEVEL;
				}
				break;
			default:
				ui.announceMessage("Error\nSomething went wrong but here are infinite enemies :) ", 2);
				if(random.Next(5) == 0){
					leveltorchers--;
					spawnTorcher();
				}
				break;
		}
	}
	
	public void GameOver(){
		GetTree().ChangeScene("GameOver.tscn");
	}
	
	private void setWorldSize(int worldsize){
		Camera2D cam = (Camera2D) player.GetNode("Camera2D");
		map.DrawBase(worldsize);
		cam.LimitRight = worldsize * 64;
		cam.LimitBottom = worldsize * 64;
	}
	/*		TORCHER FUNCTIONS 		*/
	public void removeTorcher(Torcher torcher){
		torchers.Remove(torcher);
		torcher.QueueFree();
	}
	
	private void spawnTorcher(){
		int x, y;
		if(random.Next(2) == 0){
			x = random.Next(map.size);
			if(random.Next(2) == 0) y = 0;
			else y = map.size-1;
		} else {
			y = random.Next(map.size);
			if(random.Next(2) == 0) x = 0;
			else x = map.size-1;
		}
		newTorcher(x, y);
	}
	
	private void newTorcher(int xmap, int ymap){
		Torcher torcher = (Torcher) torcherresource.Instance();
		torcher.Init(torcherhealth, map, this, player);
		torchers.Add(torcher);
		torcher.Position = map.MapToWorld(new Vector2(xmap, ymap));
		map.AddChild(torcher);
	}
	
	/*		TORCHER FUNCTIONS 		*/
	public void removeCutter(Cutter cutter){
		cutters.Remove(cutter);
		cutter.QueueFree();
	}
	
	private void spawnCutter(){
		int x, y;
		if(random.Next(2) == 0){
			x = random.Next(map.size);
			if(random.Next(2) == 0) y = 0;
			else y = map.size-1;
		} else {
			y = random.Next(map.size);
			if(random.Next(2) == 0) x = 0;
			else x = map.size-1;
		}
		newCutter(x, y);
	}
	
	private void newCutter(int xmap, int ymap){
		Cutter cutter = (Cutter) cutterresource.Instance();
		cutter.Init(torcherhealth*1.5, map, this, player);
		cutters.Add(cutter);
		cutter.Position = map.MapToWorld(new Vector2(xmap, ymap));
		map.AddChild(cutter);
	}
}
