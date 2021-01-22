using Godot;
using System;

public class InfiniteMode : Node
{
	//game elements
	Game game;
	Map map;
	Player player;
	UI ui;
	
	//local variables
	int time = 0;
	
	public override void _Ready(){
		game = (Game) GetNode("Game");
		map = (Map) GetNode("Game/Map");
		player = (Player) GetNode("Game/Map/Player");
		ui = (UI) GetNode("UI");
		Init();
	}
	
	private void Init(){
		ui.Init(10, 10, 10, 0.7f, 5, player, game, map);
		player.Init(100, 100, 100, 30, 0, 1f, game);
		map.Init(player, 100);
		game.Init(10, map, player, ui);
	}
	
	private void Tick(){
		game.Tick(time);
		time++;
	}
}
